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
using System.IO;

namespace VisionProcessForm
{
    public partial class MarkOrientToleranceSettingForm : Form
    {
        #region Member Variables

        private bool m_blnInitDone = false;
        private int m_intDecimal = 3;
        private int m_intDecimal2 = 6;
        private int m_intUserGroup = 5;
        private string m_strSelectedRecipe;
        private int m_intVisionType = 0;    // Mask Bit 0x01:Orient, 0x02:Mark, 0x08:Pkg, 0x10:Lead, 0x20:Orient0Deg
        private int m_intSettingType = 0;   //0=Main, 1=GeneralSetting, 2=OrientSetting, 3=MarkSetting
                                            //private int m_intPageSelectedIndex = 0;
                                            //private bool m_blnSaveSettingChanges = true;
        private bool m_blnChangeScoreSetting = true;
        //private bool m_blnOrientReady = true;
        //private bool m_blnMarkReady = true;
        private bool m_blnEnterTextBox = false;
        private bool m_blnWantSet1ToAll = false;
        private float m_fValuePrev = 0;

        private DataGridView m_dgdSettingView = new DataGridView();
        private DataGridView m_dgdMarkSettingView = new DataGridView();
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        #endregion


        #region Properties


        #endregion

        public MarkOrientToleranceSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, int intVisionType)
        {
            InitializeComponent();

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm 1");
            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;
            m_intVisionType = intVisionType;
            m_dgdSettingView = dgd_Setting;
            m_dgdMarkSettingView = dgd_MarkSetting;

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm 2");

            DisableField2();

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm 3");

            UpdateGUI();

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm 4");

            m_smVisionInfo.g_intSelectedPackageDefect = -1;
            m_smVisionInfo.g_intSelectedPackageLengthType = 0;

            m_blnInitDone = true;
        }

        private int GetBiggerTemplateUnitNo()
        {
            if (m_smVisionInfo.g_intUnitsOnImage == 1)
                return 0;
            else
            {
                if (m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex >= m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex)
                    return 0;
                else
                    return 1;
            }
        }

        private void DisableField()
        {
            string strChild1 = "Test Page";
            string strChild2 = "";

            //strChild2 = "Tolerance Setting Page";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //{
            //    btn_GlobalSetting.Enabled = false;
            //    m_blnChangeScoreSetting = false;
            //}

            strChild1 = "Tolerance Page";
            strChild2 = "Mark Tolerance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                dgd_MarkSetting.ReadOnly = true;
                dgd_Setting.ReadOnly = true;              
            }
            strChild1 = "Tolerance Page";
            strChild2 = "Mark Global Tolerance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
             
                btn_GlobalSetting.Enabled = false;

            }
            strChild1 = "Tolerance Page";
            strChild2 = "Package Tolerance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                dgd_PkgSetting.ReadOnly = true;
                group_GeneralDefectSetting.Enabled = false;
                group_DarkField2DefectSetting.Enabled = false;
                group_DarkField3DefectSetting.Enabled = false;
                group_DarkField4DefectSetting.Enabled = false;
                group_CrackDefectSetting.Enabled = false;
                group_ChippedOffDefectSetting.Enabled = false;
                group_MoldFlashDefectSetting.Enabled = false;
            }
            strChild1 = "Tolerance Page";
            strChild2 = "Orient Tolerance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                trackBar_OrientTolerance.Enabled = false;
                txt_OrientAngle.Enabled = false;
                txt_OrientTolerance.Enabled = false;
                txt_OrientX.Enabled = false;
                txt_OrientY.Enabled = false;
            }
            strChild1 = "Tolerance Page";
            strChild2 = "Pin1 Tolerance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                trackBar_Pin1Tolerance.Enabled = false;
                txt_Pin1Tolerance.Enabled = false;
              
            }
            strChild1 = "Tolerance Page";
            strChild2 = "Empty Tolerance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                trackBar_EmptyTolerance.Enabled = false;
                txt_EmptyScore.Enabled = false;
                txt_MinWhiteArea.Enabled = false;

            }
            strChild1 = "Tolerance Page";
            strChild2 = "Sitting Offset Tolerance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_MaxSittingHeight.Enabled = false;
                txt_MaxSittingWidth.Enabled = false;
                txt_MinSittingWidth.Enabled = false;
                txt_MinSittingHeight.Enabled = false;
            }

            strChild1 = "Tolerance Page";
            strChild2 = "Save Tolerance Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
            }
            
            strChild1 = "Tolerance Page";
            strChild2 = "Package Size Offset Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                group_PackageOffset.Visible = false;
            }
            else
            {
                group_PackageOffset.Visible = true;
            }

            if (m_smVisionInfo.g_intUnitsOnImage == 1)
            {
                lbl_Score2.Visible = false;
                lbl_ScoreTitle2.Visible = false;
                lbl_ScorePercent2.Visible = false;

                if (m_smVisionInfo.g_blnWantPin1)
                {
                    lbl_Pin1Score2.Visible = false;
                    lbl_Pin1ScoreTitle2.Visible = false;
                    lbl_Pin1ScorePercent2.Visible = false;
                }
            }

            if (!m_smVisionInfo.g_blnWantPin1 || !m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(0))
            {
                tab_VisionControl.TabPages.Remove(tp_Pin1);
            }

            if (((m_intVisionType & 0x02) > 0) && m_smVisionInfo.g_blnWantSkipMark) // 2018 05 14 - CCENG: Disable mark setting and result table if Mark Vision is registered and WantSkipMark is true.
            {
                dgd_MarkSetting.Enabled = false;
                btn_GlobalSetting.Enabled = false;
            }
            else if (((m_intVisionType & 0x06) == 0) || m_smVisionInfo.g_blnWantSkipMark)   // If mark ocv and ocr are disabled
            {
                tab_VisionControl.TabPages.Remove(tp_Mark);
                tab_VisionControl.TabPages.Remove(tp_Mark2);
            }

            if (((m_intVisionType & 0x08) == 0))   // If package is disabled
            {
                tab_VisionControl.TabPages.Remove(tp_Package);
                tab_VisionControl.TabPages.Remove(tp_PackageNew);
                tab_VisionControl.TabPages.Remove(tp_PackageNew2);
            }
            else
            {
                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnUseDetailDefectCriteria)
                {
                    if (tab_VisionControl.TabPages.Contains(tp_PackageNew))
                        tab_VisionControl.TabPages.Remove(tp_PackageNew);
                    if (tab_VisionControl.TabPages.Contains(tp_PackageNew2))
                        tab_VisionControl.TabPages.Remove(tp_PackageNew2);
                }
            }

            if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
            {
                if (tab_VisionControl.TabPages.Contains(tp_PackageNew2))
                    tab_VisionControl.TabPages.Remove(tp_PackageNew2);
            }

            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrient":
                case "MarkOrient":
                case "MOPkg":
                case "MOLiPkg":
                case "MOLi":
                    break;
                default:
                    tab_VisionControl.TabPages.Remove(tp_Orient);
                    break;
            }

            if (!m_smVisionInfo.g_blnWantCheckEmpty)
            {
                tab_VisionControl.TabPages.Remove(tp_Empty);

            }
            else
            {
                if (!m_smVisionInfo.g_blnWantUseEmptyPattern)
                {
                    group_EmptyScoreSetting.Visible = false;

                }

                if (!m_smVisionInfo.g_blnWantUseEmptyThreshold)
                {
                    group_EmptyPocketSetting.Visible = false;
                }
            }

            if (!m_smVisionInfo.g_blnWantCheckPocketPosition)
            {
                tab_VisionControl.TabPages.Remove(tp_PocketPosition);
            }

            if (!m_smVisionInfo.g_blnWantCheckUnitSitProper)
            {
                tab_VisionControl.TabPages.Remove(tp_SitOffset); 
            }
            // 2019 04 12-CCENG: Orient setting still need to display because will use orient inspection to check unit is 0 angle or not. So not need to remove the orient tab page even though g_intWantOrient0Deg is true.
            // if Want Mark but dont want orient result
            //if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            //{
            //    tab_VisionControl.TabPages.Remove(tp_Orient);
            //}

            // If not Inpocket module
            if (m_smVisionInfo.g_intUnitsOnImage == 1)
            {
                dgd_Setting.Columns[2].Visible = false;
                dgd_Setting.Columns[5].Visible = false;
                dgd_Setting.Columns[8].Visible = false;
                dgd_Setting.Columns[12].Visible = false;
                dgd_MarkSetting.Columns[2].Visible = false;
                dgd_PkgSetting.Columns[3].Visible = false;
            }
        }
        private int GetUserRightGroup_Child2(string Child1, string Child2)
        {
            //NewUserRight objUserRight = new NewUserRight(false);
            switch (m_smVisionInfo.g_strVisionName)
            {
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
            }

            return 1;
        }

        private void DisableField2()
        {
            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 1");

            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "Tolerance";
            string strChild2 = "";

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 2");

            strChild2 = "Mark TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                dgd_MarkSetting.Enabled = false;
                dgd_Setting.Enabled = false;
                btn_GlobalSetting.Enabled = false;
                dgd_OCRSettings.Enabled = false;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 3");
            strChild2 = "Package TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                dgd_PkgSetting.Enabled = false;
                group_GeneralDefectSetting.Enabled = false;
                group_ChippedOffDefectSetting.Enabled = false;
                group_MoldFlashDefectSetting.Enabled = false;
                group_CrackDefectSetting.Enabled = false;
                group_DarkField2DefectSetting.Enabled = false;
                group_DarkField3DefectSetting.Enabled = false;
                group_DarkField4DefectSetting.Enabled = false;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 4");
            strChild2 = "Package Size Offset Setting";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                btn_PackageOffset.Enabled = false;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 5");
            strChild2 = "Orient TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                group_OrientScoreSetting.Enabled = false;
                group_OrientAngleSetting.Enabled = false;
                group_OrientDistanceSetting.Enabled = false;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 6");

            strChild2 = "Pin1 TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                group_Pin1Setting.Enabled = false;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 7");

            strChild2 = "Empty TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                group_EmptyScoreSetting.Enabled = false;
                group_EmptyPocketSetting.Enabled = false;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 8");
            strChild2 = "Sitting Offset TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                group_SitOffsetWidthSetting.Enabled = false;
                group_SitOffsetHeightSetting.Enabled = false;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 9");

            strChild2 = "Color TabPage";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                gbox_ColorDefect1.Enabled = false;
                gbox_ColorDefect2.Enabled = false;
                gbox_ColorDefect3.Enabled = false;
                gbox_ColorDefect4.Enabled = false;
                gbox_ColorDefect5.Enabled = false;
                dgd_Color.Enabled = false;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 10");

            strChild2 = "Pocket Position TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                group_PocketPositionSetting.Enabled = false;
                group_PocketPositionCorrectionSetting.Enabled = false;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 11");

            strChild2 = "Save Button";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
            }

            if (m_smVisionInfo.g_intUnitsOnImage == 1)
            {
                lbl_Score2.Visible = false;
                lbl_ScoreTitle2.Visible = false;
                lbl_ScorePercent2.Visible = false;

                if (m_smVisionInfo.g_blnWantPin1)
                {
                    lbl_Pin1Score2.Visible = false;
                    lbl_Pin1ScoreTitle2.Visible = false;
                    lbl_Pin1ScorePercent2.Visible = false;
                }
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 12");

            if (!m_smVisionInfo.g_blnWantPin1 || !m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(0))
            {
                tab_VisionControl.TabPages.Remove(tp_Pin1);
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 13");
            if (((m_intVisionType & 0x02) > 0) && m_smVisionInfo.g_blnWantSkipMark) // 2018 05 14 - CCENG: Disable mark setting and result table if Mark Vision is registered and WantSkipMark is true.
            {
                STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 13.1");
                dgd_MarkSetting.Enabled = false;
                btn_GlobalSetting.Enabled = false;
            }
            else if (((m_intVisionType & 0x06) == 0) || m_smVisionInfo.g_blnWantSkipMark)   // If mark ocv and ocr are disabled
            {
                STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 13.2");
                tab_VisionControl.TabPages.Remove(tp_Mark);
                tab_VisionControl.TabPages.Remove(tp_Mark2);
                //tab_VisionControl.TabPages.Remove(tp_OCR);
            }
            else if ((m_intVisionType & 0x06) > 0)
            {
                STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 13.3");
                if (m_smVisionInfo.g_arrMarks != null && m_smVisionInfo.g_arrMarks[0] != null &&
                     !m_smVisionInfo.g_arrMarks[0].ref_blnCheckMark)
                {
                    tab_VisionControl.TabPages.Remove(tp_Mark);
                    tab_VisionControl.TabPages.Remove(tp_Mark2);
                    //tab_VisionControl.TabPages.Remove(tp_OCR);
                }
            }
            
            if (((m_intVisionType & 0x40) > 0) && m_smVisionInfo.g_blnWantSkipMark) // 2018 05 14 - CCENG: Disable mark setting and result table if Mark Vision is registered and WantSkipMark is true.
            {
                dgd_OCRSettings.Enabled = false;
            }
            else if (((m_intVisionType & 0x40) == 0) || m_smVisionInfo.g_blnWantSkipMark)   // If mark ocv and ocr are disabled
            {
                tab_VisionControl.TabPages.Remove(tp_OCR);
            }
            else if ((m_intVisionType & 0x40) > 0)
            {
                if (m_smVisionInfo.g_arrMarks != null && m_smVisionInfo.g_arrMarks[0] != null &&
                     !m_smVisionInfo.g_arrMarks[0].ref_blnCheckMark)
                {
                    tab_VisionControl.TabPages.Remove(tp_OCR);
                }
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 14");

            if (((m_intVisionType & 0x08) == 0))   // If package is disabled
            {
                STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 14.1");
                if (tab_VisionControl.TabPages.Contains(tp_Package))
                    tab_VisionControl.TabPages.Remove(tp_Package);
                if (tab_VisionControl.TabPages.Contains(tp_PackageNew))
                    tab_VisionControl.TabPages.Remove(tp_PackageNew);
                if (tab_VisionControl.TabPages.Contains(tp_PackageNew2))
                    tab_VisionControl.TabPages.Remove(tp_PackageNew2);
                if (tab_VisionControl.TabPages.Contains(tp_Color))
                    tab_VisionControl.TabPages.Remove(tp_Color);
                if (tab_VisionControl.TabPages.Contains(tp_Color2))
                    tab_VisionControl.TabPages.Remove(tp_Color2);
                if (tab_VisionControl.TabPages.Contains(tp_Color_Table))
                    tab_VisionControl.TabPages.Remove(tp_Color_Table);
            }
            else
            {
                STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 14.2");
                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnUseDetailDefectCriteria)
                {
                    STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 14.2.1");
                    if (tab_VisionControl.TabPages.Contains(tp_PackageNew))
                        tab_VisionControl.TabPages.Remove(tp_PackageNew);
                    if (tab_VisionControl.TabPages.Contains(tp_PackageNew2))
                        tab_VisionControl.TabPages.Remove(tp_PackageNew2);
                }

                STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 14.3");
                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                {
                    STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 14.3.1");
                    if (tab_VisionControl.TabPages.Contains(tp_Color))
                        tab_VisionControl.TabPages.Remove(tp_Color);
                    if (tab_VisionControl.TabPages.Contains(tp_Color2))
                        tab_VisionControl.TabPages.Remove(tp_Color2);
                    if (tab_VisionControl.TabPages.Contains(tp_Color_Table))
                        tab_VisionControl.TabPages.Remove(tp_Color_Table);
                }
                else
                {
                    STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 14.3.2");
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count == 0)
                    {
                        if (tab_VisionControl.TabPages.Contains(tp_Color))
                            tab_VisionControl.TabPages.Remove(tp_Color);
                        if (tab_VisionControl.TabPages.Contains(tp_Color2))
                            tab_VisionControl.TabPages.Remove(tp_Color2);
                        if (tab_VisionControl.TabPages.Contains(tp_Color_Table))
                            tab_VisionControl.TabPages.Remove(tp_Color_Table);
                    }
                    if (tab_VisionControl.TabPages.Contains(tp_Color))
                        tab_VisionControl.TabPages.Remove(tp_Color);
                    if (tab_VisionControl.TabPages.Contains(tp_Color2))
                        tab_VisionControl.TabPages.Remove(tp_Color2);
                    //if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count < 2)
                    //{
                    //    gbox_ColorDefect2.Visible = false;
                    //}
                    //if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count < 3)
                    //{
                    //    gbox_ColorDefect3.Visible = false;
                    //}
                    //if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count < 4)
                    //{
                    //    if (tab_VisionControl.TabPages.Contains(tp_Color2))
                    //        tab_VisionControl.TabPages.Remove(tp_Color2);
                    //}
                    //if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count < 5)
                    //{
                    //    gbox_ColorDefect5.Visible = false;
                    //}

                    List<int> arrColorDefectSkipNo = new List<int>();
                    int intTotalColorCount_Center = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectCount(ref arrColorDefectSkipNo);

                    for (int intColorThresIndex = 0; intColorThresIndex < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count; intColorThresIndex++)
                    {
                        if (arrColorDefectSkipNo.Contains(intColorThresIndex))
                        {
                            if (dgd_Color.RowCount <= intColorThresIndex)
                                dgd_Color.Rows.Add();
                            dgd_Color.Rows[intColorThresIndex].Visible = false;
                            continue;
                        }

                        if (dgd_Color.RowCount <= intColorThresIndex)
                            dgd_Color.Rows.Add();
                        dgd_Color.Rows[intColorThresIndex].Cells[0].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[intColorThresIndex];
                        dgd_Color.Rows[intColorThresIndex].Cells[2].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                        dgd_Color.Rows[intColorThresIndex].Cells[3].Value = (dgd_Color.Rows[intColorThresIndex].Cells[3] as DataGridViewComboBoxCell).Items[m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionFailCondition(intColorThresIndex)];
                        dgd_Color.Rows[intColorThresIndex].Cells[4].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                        dgd_Color.Rows[intColorThresIndex].Cells[6].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectType[intColorThresIndex] == 0)
                        {
                            dgd_Color.Rows[intColorThresIndex].Cells[6].ReadOnly = true;
                            dgd_Color.Rows[intColorThresIndex].Cells[6].Style.BackColor = SystemColors.Control;
                            dgd_Color.Rows[intColorThresIndex].Cells[6].Style.SelectionBackColor = SystemColors.Control;
                        }
                        dgd_Color.Rows[intColorThresIndex].Cells[8].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        dgd_Color.Rows[intColorThresIndex].Cells[10].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);

                        //if (intColorThresIndex == 0)
                        //{
                        //    gbox_ColorDefect1.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[intColorThresIndex];
                        //    txt_ColorDefect1Length.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                        //    txt_ColorDefect1Width.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                        //    txt_ColorDefect1MinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        //    txt_ColorDefect1MinArea.Enabled = (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectType[intColorThresIndex] == 1);
                        //    txt_ColorDefect1MaxArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        //    txt_ColorDefect1TotalArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        //}
                        //else if (intColorThresIndex == 1)
                        //{
                        //    gbox_ColorDefect2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[intColorThresIndex];
                        //    txt_ColorDefect2Length.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                        //    txt_ColorDefect2Width.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                        //    txt_ColorDefect2MinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        //    txt_ColorDefect2MinArea.Enabled = (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectType[intColorThresIndex] == 1);
                        //    txt_ColorDefect2MaxArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        //    txt_ColorDefect2TotalArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        //}
                        //else if (intColorThresIndex == 2)
                        //{
                        //    gbox_ColorDefect3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[intColorThresIndex];
                        //    txt_ColorDefect3Length.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                        //    txt_ColorDefect3Width.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                        //    txt_ColorDefect3MinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        //    txt_ColorDefect3MinArea.Enabled = (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectType[intColorThresIndex] == 1);
                        //    txt_ColorDefect3MaxArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        //    txt_ColorDefect3TotalArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        //}
                        //else if (intColorThresIndex == 3)
                        //{
                        //    gbox_ColorDefect4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[intColorThresIndex];
                        //    txt_ColorDefect4Length.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                        //    txt_ColorDefect4Width.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                        //    txt_ColorDefect4MinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        //    txt_ColorDefect4MinArea.Enabled = (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectType[intColorThresIndex] == 1);
                        //    txt_ColorDefect4MaxArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        //    txt_ColorDefect4TotalArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        //}
                        //else if (intColorThresIndex == 4)
                        //{
                        //    gbox_ColorDefect5.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[intColorThresIndex];
                        //    txt_ColorDefect5Length.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                        //    txt_ColorDefect5Width.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                        //    txt_ColorDefect5MinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        //    txt_ColorDefect5MinArea.Enabled = (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectType[intColorThresIndex] == 1);
                        //    txt_ColorDefect5MaxArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        //    txt_ColorDefect5TotalArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                        //}
                    }
                }
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 15");
            if (m_smVisionInfo.g_arrPackage.Count > m_smVisionInfo.g_intSelectedUnit && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
            {
                if (tab_VisionControl.TabPages.Contains(tp_PackageNew2))
                    tab_VisionControl.TabPages.Remove(tp_PackageNew2);
            }

            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && (m_smVisionInfo.g_strVisionName.Contains("BottomPosition") || m_smVisionInfo.g_strVisionName.Contains("BottomOrient")))
            {
                if (!m_smVisionInfo.g_blnOrientWantPackage)
                {
                    if (tab_VisionControl.TabPages.Contains(tp_Package))
                        tab_VisionControl.TabPages.Remove(tp_Package);
                }

                if (tab_VisionControl.TabPages.Contains(tp_PackageNew))
                    tab_VisionControl.TabPages.Remove(tp_PackageNew);
                if (tab_VisionControl.TabPages.Contains(tp_PackageNew2))
                    tab_VisionControl.TabPages.Remove(tp_PackageNew2);
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 16");
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrient":
                case "BottomPosition":
                case "MarkOrient":
                case "MOPkg":
                case "MOLiPkg":
                case "MOLi":
                    break;
                default:
                    tab_VisionControl.TabPages.Remove(tp_Orient);
                    break;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 17");
            if (!m_smVisionInfo.g_blnWantCheckEmpty)
            {
                tab_VisionControl.TabPages.Remove(tp_Empty);

            }
            else
            {
                if (!m_smVisionInfo.g_blnWantUseEmptyPattern)
                {
                    group_EmptyScoreSetting.Visible = false;

                }

                if (!m_smVisionInfo.g_blnWantUseEmptyThreshold)
                {
                    group_EmptyPocketSetting.Visible = false;
                }
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 18");
            if (!m_smVisionInfo.g_blnWantCheckPocketPosition)
            {
                tab_VisionControl.TabPages.Remove(tp_PocketPosition);
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 19");
            if (!m_smVisionInfo.g_blnWantCheckUnitSitProper)
            {
                tab_VisionControl.TabPages.Remove(tp_SitOffset);
            }
            // 2019 04 12-CCENG: Orient setting still need to display because will use orient inspection to check unit is 0 angle or not. So not need to remove the orient tab page even though g_intWantOrient0Deg is true.
            // if Want Mark but dont want orient result
            //if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            //{
            //    tab_VisionControl.TabPages.Remove(tp_Orient);
            //}

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 20");
            // If not Inpocket module
            if (m_smVisionInfo.g_intUnitsOnImage == 1)
            {
                if (dgd_Setting.Columns.Count > 2)
                    dgd_Setting.Columns[2].Visible = false;
                if (dgd_Setting.Columns.Count > 5)
                    dgd_Setting.Columns[5].Visible = false;
                if (dgd_Setting.Columns.Count > 8)
                    dgd_Setting.Columns[8].Visible = false;
                if (dgd_Setting.Columns.Count > 12)
                    dgd_Setting.Columns[12].Visible = false;
                if (dgd_OCRSettings.Columns.Count > 2)
                    dgd_OCRSettings.Columns[2].Visible = false;
                if (dgd_MarkSetting.Columns.Count > 2)
                    dgd_MarkSetting.Columns[2].Visible = false;
                if (dgd_PkgSetting.Columns.Count > 3)
                    dgd_PkgSetting.Columns[3].Visible = false;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 21");
            int intFailMask = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetFailOptionMask(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
            if ((intFailMask & 0x01) == 0)   // 0x01=Excess Mark
            {
                if (dgd_Setting.Columns.Count > 4)
                    dgd_Setting.Columns[4].Visible = false;
                if (dgd_Setting.Columns.Count > 5)
                    dgd_Setting.Columns[5].Visible = false;
                if (dgd_Setting.Columns.Count > 6)
                    dgd_Setting.Columns[6].Visible = false;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 22");
            if ((intFailMask & 0x10) == 0)   // 0x01=Miss Mark
            {
                if (dgd_Setting.Columns.Count > 7)
                    dgd_Setting.Columns[7].Visible = false;
                if (dgd_Setting.Columns.Count > 8)
                    dgd_Setting.Columns[8].Visible = false;
                if (dgd_Setting.Columns.Count > 9)
                    dgd_Setting.Columns[9].Visible = false;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 23");
            if (!m_smVisionInfo.g_blnWantCheckMarkBroken || ((intFailMask & 0x20) == 0))
            {
                if (dgd_Setting.Columns.Count > 10)
                    dgd_Setting.Columns[10].Visible = false;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 24");
            if (!m_smVisionInfo.g_blnWantCheckMarkAverageGrayValue || ((intFailMask & 0x200) == 0))
            {
                if (dgd_Setting.Columns.Count > 11)
                    dgd_Setting.Columns[11].Visible = false;
                if (dgd_Setting.Columns.Count > 12)
                    dgd_Setting.Columns[12].Visible = false;
                if (dgd_Setting.Columns.Count > 13)
                    dgd_Setting.Columns[13].Visible = false;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - DisableField2() > 25");
        }
        private void UpdateGUI()
        {
            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 1");

            UpdateUnitDisplay();

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 2");
            // ======================================================================================

            // ----------------- Mark Orient --------------------------------------------------------

            // ======================================================================================

            // 2022 02 17 - CCENG: Dun set selectedTemplate to 0. During offline test, if final selectedTemplate is not 0, 
            //                     when user click enter MarkOrientToleranceSettingForm, so remain display result at selectedTemplate not 0.
            //for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            //{
            //    m_smVisionInfo.g_arrMarks[u].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 0;
            //}

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 3");
            if ((m_intVisionType & 0x01) > 0)
            {
                if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    group_OrientAngleSetting.Visible = true;
                    group_OrientDistanceSetting.Visible = true;

                    if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_strVisionName.Contains("BottomPosition"))
                    {
                        tp_Orient.Text = "Positon";
                        lbl_ScoreTitle.Text = "Unit 1 Score Setting";
                        group_OrientScoreSetting.Text = "Unit Score Setting";
                        group_OrientAngleSetting.Text = "Unit Angle Setting";
                        group_OrientDistanceSetting.Text = "Unit Distance Setting";

                        if (m_smCustomizeInfo.g_intLanguageCulture != 1)
                        {
                            tp_Orient.Text = "位置";
                            lbl_ScoreTitle.Text = "单元1分数设定";
                            group_OrientScoreSetting.Text = "单元容许度设定";
                            group_OrientAngleSetting.Text = "单元角度设定";
                            group_OrientDistanceSetting.Text = "单元位置设定";
                        }
                    }
                }
                else
                {
                    group_OrientAngleSetting.Visible = false;
                    group_OrientDistanceSetting.Visible = false;
                }
                if (m_smVisionInfo.g_blnUnitInspected[0] && m_smVisionInfo.g_arrOrients.Count > 0 && m_smVisionInfo.g_arrOrients[0].Count > m_smVisionInfo.g_intSelectedTemplate)
                {
                    float fScore;
                    fScore = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetMinScore() * 100;
                    if (fScore < 0)
                        lbl_Score.Text = "----";
                    else
                        lbl_Score.Text = fScore.ToString("f2");

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
                    lbl_OrientAngle.Text = Math.Abs(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_fDegAngleResult).ToString("f4");//GetResultAngle()
                    //float CenterX = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) / 2; //(float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) / 2;
                    //float CenterY = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) / 2; //(float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) / 2;

                    ////2020-09-24 ZJYEOH : Should use current angle to rotate template center point because when get center point different, the object center point is based on current angle
                    //float fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateX - CenterX) * Math.Cos(intOrientAngle * Math.PI / 180)) - //m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_intRotatedAngle
                    //                        ((m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateY - CenterY) * Math.Sin(intOrientAngle * Math.PI / 180)));
                    //float fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateX - CenterX) * Math.Sin(intOrientAngle * Math.PI / 180)) +
                    //                        ((m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateY - CenterY) * Math.Cos(intOrientAngle * Math.PI / 180)));

                    float fXAfterRotated = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateX;
                    float fYAfterRotated = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateY;
                    float fAngleResult = Math.Abs(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fDegAngleResult);//GetResultAngle()
                    float fCenterXDiff = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].GetCenterXDiff(fXAfterRotated, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX); //m_smVisionInfo.g_fOrientCenterX[0];
                    float fCenterYDiff = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].GetCenterYDiff(fYAfterRotated, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY);// m_smVisionInfo.g_fOrientCenterY[0];

                    lbl_OrientX.Text = fCenterXDiff.ToString("f4");
                    lbl_OrientY.Text = fCenterYDiff.ToString("f4");

                    //lbl_OrientX.Text = Math.Abs(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateX - m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fObjectX).ToString("f4");
                    //lbl_OrientY.Text = Math.Abs(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateY - m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fObjectY).ToString("f4");
                }
                else
                {
                    lbl_Score.Text = "----";
                    lbl_OrientAngle.Text = "----";
                    lbl_OrientX.Text = "----";
                    lbl_OrientY.Text = "----";
                }

                if (m_smVisionInfo.g_blnUnitInspected[1] && m_smVisionInfo.g_intUnitsOnImage == 2)
                {
                    float fScore;
                    fScore = m_smVisionInfo.g_arrOrients[1][m_smVisionInfo.g_intSelectedTemplate].GetMinScore() * 100;
                    if (fScore < 0)
                        lbl_Score2.Text = "----";
                    else
                        lbl_Score2.Text = fScore.ToString("f2");
                }
                else
                    lbl_Score2.Text = "----";

                LoadOrientSettings(m_smVisionInfo.g_intSelectedTemplate);

                if (m_smVisionInfo.g_blnWantPin1)
                {
                    if (m_smVisionInfo.g_blnUnitInspected[0])
                    {
                        float fScore;
                        fScore = m_smVisionInfo.g_arrPin1[0].GetResultScore(m_smVisionInfo.g_intSelectedTemplate) * 100;
                        if (fScore < 0)
                            lbl_Pin1Score.Text = "----";
                        else
                            lbl_Pin1Score.Text = fScore.ToString("f2");
                    }
                    else
                        lbl_Pin1Score.Text = "----";

                    if (m_smVisionInfo.g_blnUnitInspected[1] && m_smVisionInfo.g_intUnitsOnImage == 2)
                    {
                        float fScore;
                        fScore = m_smVisionInfo.g_arrPin1[1].GetResultScore(m_smVisionInfo.g_intSelectedTemplate) * 100;
                        if (fScore < 0)
                            lbl_Pin1Score2.Text = "----";
                        else
                            lbl_Pin1Score2.Text = fScore.ToString("f2");
                    }
                    else
                        lbl_Pin1Score2.Text = "----";

                    LoadPin1Settings(m_smVisionInfo.g_intSelectedTemplate);
                }
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 4");
            if (!(m_smVisionInfo.g_blnWantSkipMark || (m_intVisionType == 1)))
            {
                if ((m_intVisionType & 0x06) > 0)
                {
                    AddMarkSettingGUI();
                    UpdateMarkSettingGUI();
                    UpdateMarkResultTable(!m_smVisionInfo.g_blnMarkInspected);
                }

                if ((m_intVisionType & 0x02) > 0)
                {
                    // Update grid from OCV
                    UpdateOCVSettingTable();
                    UpdateOCRSettingTable();
                }
                else
                {
                    // Update Grid from OCR
                    UpdateOCRPatternTable();
                    //txt_RefCharManual.Text = m_smVisionInfo.g_arrMarks[0].GetRefChars(0);
                }
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 5");

            if ((m_smVisionInfo.g_arrMarks[0].GetNumTemplates() <= 1) || !m_smVisionInfo.g_blnWantMultiTemplates)
            {
                panel_Template.Visible = false;
                //pnl_PageButton.Size = new Size(pnl_PageButton.Width, pnl_PageButton.Height - panel_Template.Height);
                //pnl_PageButton.Location = new Point(pnl_PageButton.Location.X, pnl_PageButton.Location.Y + panel_Template.Height);
            }
            else
            {
                cbo_TemplateNo.Items.Clear();
                for (int i = 0; i < m_smVisionInfo.g_arrMarks[0].GetNumTemplates(); i++)
                {
                    cbo_TemplateNo.Items.Add((i + 1));
                }

                if (cbo_TemplateNo.Items.Contains((m_smVisionInfo.g_intSelectedTemplate + 1)))
                    cbo_TemplateNo.SelectedItem = (m_smVisionInfo.g_intSelectedTemplate + 1);
                else
                    cbo_TemplateNo.SelectedIndex = 0;

                //cbo_TemplateNo.SelectedIndex = m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 0;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 6");
            //-----------------------------------Package----------------------------------
            if ((m_intVisionType & 0x08) > 0)
            {
                AddPkgSettingGUI();
                UpdatePkgSettingGUI();
                UpdatePkgResultTable(false); //UpdatePkgResultTable(!m_smVisionInfo.g_blnMarkInspected);
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 7");
            chk_SetAllTemplates.Checked = m_blnWantSet1ToAll = m_smVisionInfo.g_blnWantSet1ToAll;
            chk_SetAllTemplates.Visible = !m_smVisionInfo.g_blnWantSet1ToAll;//2020-05-11 ZJYEOH : Hide Set to All checkbox when user tick set 1 to all in advance setting

            if ((m_intVisionType & 0x01) > 0)
                m_intSettingType = 0;   // Display Orient tab page
            else if ((m_intVisionType & 0x02) > 0)
            {
                if (m_smVisionInfo.g_blnUseOCR)
                {
                    m_intSettingType = 8;
                }
                else
                {
                    m_intSettingType = 1;   // Display mark tab page
                }
            }
            else
                m_intSettingType = 4;   // Display Package tab page

            if ((m_intVisionType & 0x20) > 0)
            {
                if ((m_intVisionType & 0x02) > 0)
                {
                    if (m_smVisionInfo.g_blnUseOCR)
                    {
                        m_intSettingType = 8;
                    }
                    else
                    {
                        m_intSettingType = 1;   // Display mark tab page
                    }
                }
                else
                    m_intSettingType = 4;   // Display Package tab page
            }

            if (m_smVisionInfo.g_blnWantCheckUnitSitProper)
            {
                m_intSettingType = 5;
            }
            if (m_smVisionInfo.g_blnWantCheckEmpty)
            {
                m_intSettingType = 6;
            }
            if (m_smVisionInfo.g_blnWantCheckPocketPosition)
            {
                m_intSettingType = 7;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 8");
            UpdateTabPage();

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 9");
            ViewOrHideResultColumn(chk_DisplayResult.Checked);

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 10");
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            m_intSettingType = (int)subkey1.GetValue("CurrentSettingType_MoTolerance" + "_" + m_smVisionInfo.g_strVisionDisplayName, 0);

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 11");
            if ((m_intVisionType & 0x20) > 0 && m_intSettingType == 0)
            {
                if ((m_intVisionType & 0x02) > 0)
                {
                    if (m_smVisionInfo.g_blnUseOCR)
                    {
                        m_intSettingType = 8;
                    }
                    else
                    {
                        m_intSettingType = 1;   // Display mark tab page
                    }
                }
                else
                    m_intSettingType = 4;   // Display Package tab page
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 12");

            switch (m_intSettingType)
            {
                default:
                case 0:
                    {
                        tab_VisionControl.SelectedTab = tp_Orient;
                    }
                    break;
                case 1:
                    {
                        tab_VisionControl.SelectedTab = tp_Mark;
                    }
                    break;
                case 2:
                    {
                        tab_VisionControl.SelectedTab = tp_Mark2;
                    }
                    break;
                case 3:
                    {
                        tab_VisionControl.SelectedTab = tp_Pin1;
                    }
                    break;
                case 4:
                    {
                        tab_VisionControl.SelectedTab = tp_Package;
                        //tab_VisionControl.SelectedTab = tp_PackageNew;
                    }
                    break;
                case 5:
                    {
                        tab_VisionControl.SelectedTab = tp_SitOffset;
                    }
                    break;
                case 6:
                    {
                        tab_VisionControl.SelectedTab = tp_Empty;
                    }
                    break;
                case 7:
                    {
                        tab_VisionControl.SelectedTab = tp_PocketPosition;
                    }
                    break;
                case 8:
                    {
                        tab_VisionControl.SelectedTab = tp_OCR;
                    }
                    break;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 13");

            if (!tab_VisionControl.Contains(tab_VisionControl.SelectedTab)) //Set to default Orient when sharing between 2 vision  
            {
                m_intSettingType = 0;
                tab_VisionControl.SelectedTab = tp_Orient;
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 14");
            UpdateTabPage(); //Update again to avoid missing component when sharing between 2 vision  

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 15");
            chk_DisplayResult.Checked = Convert.ToBoolean(subkey1.GetValue("CurrentDisplayResult_MoTolerance", false));
            ViewOrHideResultColumn(chk_DisplayResult.Checked);

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 16");
            if (m_smVisionInfo.g_objPocketPosition != null)
            {
                txt_PocketPositionRef.Text = m_smVisionInfo.g_objPocketPosition.ref_intPocketPositionReference.ToString();
                txt_PocketPositionTol.Text = m_smVisionInfo.g_objPocketPosition.ref_intPocketPositionTolerance.ToString();
            }

            STTrackLog.WriteLine("MarkOrientToleranceSettingForm - UpdateGUI() > 17");
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

            srmLabel18.Text = strUnitDisplay;
            srmLabel19.Text = strUnitDisplay;
            srmLabel20.Text = strUnitDisplay;
            srmLabel21.Text = strUnitDisplay;
        }

        private void UpdateTabPage()
        {
            switch (m_intSettingType)
            {
                case 0: // Orient Page                        
                    lbl_PageLabel.Text = "Orient Setting";
                    tp_Orient.Controls.Add(pnl_PageButton);
                    lbl_Notice1.Visible = lbl_Notice2.Visible = lbl_Notice3.Visible = false;
                    break;
                case 1: // Mark Page                        
                    lbl_PageLabel.Text = "Mark Setting";
                    tp_Mark.Controls.Add(pnl_PageButton);
                    lbl_Notice1.Visible = lbl_Notice2.Visible = lbl_Notice3.Visible = true;
                    break;
                case 2: // Mark2 Page
                    lbl_PageLabel.Text = "Mark2 Setting";
                    tp_Mark2.Controls.Add(pnl_PageButton);
                    lbl_Notice1.Visible = lbl_Notice2.Visible = lbl_Notice3.Visible = false;
                    break;
                case 3: // Pin1 Page
                    lbl_PageLabel.Text = "Pin 1 Setting";
                    tp_Pin1.Controls.Add(pnl_PageButton);
                    lbl_Notice1.Visible = lbl_Notice2.Visible = lbl_Notice3.Visible = false;
                    break;
                case 4: // Pkg Page
                    lbl_PageLabel.Text = "Package Setting";
                    //tp_Package.Controls.Add(pnl_PageButton);          // 2020 04 22 - CCENG: Package setting no multi template
                    lbl_Notice1.Visible = lbl_Notice2.Visible = lbl_Notice3.Visible = false;

                    //m_smVisionInfo.g_blnPackageInspected = false;// true;
                    m_smVisionInfo.g_blnViewPackageDefectSetting = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnViewRotatedPackageImage = false;
                    m_smVisionInfo.g_blnViewMarkInspection = true;// false;
                    break;
                case 5: // Sitting Offset Page
                    lbl_PageLabel.Text = "Sitting Offset Setting";
                    //tp_SitOffset.Controls.Add(pnl_PageButton);        // 2020 04 22 - CCENG: No multi template
                    lbl_Notice1.Visible = lbl_Notice2.Visible = lbl_Notice3.Visible = false;
                    break;
                case 6: // Empty Page
                    lbl_PageLabel.Text = "Empty Setting";
                    //tp_Empty.Controls.Add(pnl_PageButton);            // 2020 04 22 - CCENG: No multi template
                    lbl_Notice1.Visible = lbl_Notice2.Visible = lbl_Notice3.Visible = false;
                    break;
                case 7: // Pocket Position Page
                    lbl_PageLabel.Text = "Pocket Position Setting";
                    //tp_PocketPosition.Controls.Add(pnl_PageButton);   // 2020 04 22 - CCENG: No multi template
                    lbl_Notice1.Visible = lbl_Notice2.Visible = lbl_Notice3.Visible = false;
                    break;
                case 8: // OCR Page
                    lbl_PageLabel.Text = "OCR Setting";
                    tp_OCR.Controls.Add(pnl_PageButton);
                    lbl_Notice1.Visible = lbl_Notice2.Visible = lbl_Notice3.Visible = true;
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void UpdateSelectedTemplateChange()
        {
            if (m_smVisionInfo.g_blnWantCheckEmpty)
            {
                string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Positioning\\";
                m_smVisionInfo.g_objPositioning.LoadEmptyThreshold(strFolderPath + "Settings.xml", "General");
                if (!tab_VisionControl.TabPages.Contains(tp_Empty))
                    tab_VisionControl.TabPages.Add(tp_Empty);
                txt_MinWhiteArea.Text = m_smVisionInfo.g_objPositioning.ref_intEmptyWhiteArea.ToString();
                txt_EmptyScore.Text = m_smVisionInfo.g_objPositioning.ref_intMinEmptyScore.ToString();
            }

            if (m_smVisionInfo.g_blnWantCheckPocketPosition)
            {
                txt_PocketPositionTolerance.Text = m_smVisionInfo.g_objPocketPosition.ref_fPositionXTolerance.ToString();
                lbl_TemplatePocketDistance.Text = (m_smVisionInfo.g_objPocketPosition.ref_fTemplateXDistance * 1000 / m_smVisionInfo.g_fCalibPixelX).ToString();
                lbl_CurrentPocketDistance.Text = (m_smVisionInfo.g_objPocketPosition.ref_fResultXDistance * 1000 / m_smVisionInfo.g_fCalibPixelX).ToString();
                txt_PocketPatternMinScore.Text = m_smVisionInfo.g_objPocketPosition.ref_intMinMatchingScore.ToString();
            }

            if ((m_intVisionType & 0x01) > 0)
            {
                if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    group_OrientAngleSetting.Visible = true;
                    group_OrientDistanceSetting.Visible = true;
                }
                else
                {
                    group_OrientAngleSetting.Visible = false;
                    group_OrientDistanceSetting.Visible = false;
                }
                if (m_smVisionInfo.g_blnUnitInspected[0])
                {
                    float fScore;
                    fScore = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetMinScore() * 100;
                    if (fScore < 0)
                        lbl_Score.Text = "----";
                    else
                        lbl_Score.Text = fScore.ToString("f2");

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
                    lbl_OrientAngle.Text = Math.Abs(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_fDegAngleResult).ToString("f4");//GetResultAngle()
                    //float CenterX = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) / 2; //(float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) / 2;
                    //float CenterY = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) / 2; //(float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) / 2;
                    ////2020-09-24 ZJYEOH : Should use current angle to rotate template center point because when get center point different, the object center point is based on current angle
                    //float fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateX - CenterX) * Math.Cos(intOrientAngle * Math.PI / 180)) - //m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_intRotatedAngle 
                    //                        ((m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateY - CenterY) * Math.Sin(intOrientAngle * Math.PI / 180)));
                    //float fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateX - CenterX) * Math.Sin(intOrientAngle * Math.PI / 180)) +
                    //                        ((m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateY - CenterY) * Math.Cos(intOrientAngle * Math.PI / 180)));

                    float fXAfterRotated = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateX;
                    float fYAfterRotated = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateY;
                    float fAngleResult = Math.Abs(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fDegAngleResult);//GetResultAngle()
                    float fCenterXDiff = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].GetCenterXDiff(fXAfterRotated, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX); //m_smVisionInfo.g_fOrientCenterX[0];
                    float fCenterYDiff = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].GetCenterYDiff(fYAfterRotated, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY);// m_smVisionInfo.g_fOrientCenterY[0];

                    lbl_OrientX.Text = fCenterXDiff.ToString("f4");
                    lbl_OrientY.Text = fCenterYDiff.ToString("f4");

                    //lbl_OrientX.Text = Math.Abs(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateX - m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fObjectX).ToString("f4");
                    //lbl_OrientY.Text = Math.Abs(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateY - m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fObjectY).ToString("f4");
                }
                else
                {
                    lbl_Score.Text = "----";
                    lbl_OrientAngle.Text = "----";
                    lbl_OrientX.Text = "----";
                    lbl_OrientY.Text = "----";
                }

                if (m_smVisionInfo.g_blnUnitInspected[1] && m_smVisionInfo.g_intUnitsOnImage == 2)
                {
                    float fScore;
                    fScore = m_smVisionInfo.g_arrOrients[1][m_smVisionInfo.g_intSelectedTemplate].GetMinScore() * 100;
                    if (fScore < 0)
                        lbl_Score2.Text = "----";
                    else
                        lbl_Score2.Text = fScore.ToString("f2");
                }
                else
                    lbl_Score2.Text = "----";

                LoadOrientSettings(m_smVisionInfo.g_intSelectedTemplate);

                if (m_smVisionInfo.g_blnWantPin1)
                {
                    if (m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(m_smVisionInfo.g_intSelectedTemplate))
                    {
                        if (!tab_VisionControl.TabPages.Contains(tp_Pin1))
                            tab_VisionControl.TabPages.Add(tp_Pin1);
                    }
                    else
                    {
                        if (tab_VisionControl.TabPages.Contains(tp_Pin1))
                            tab_VisionControl.TabPages.Remove(tp_Pin1);
                    }

                    if (m_smVisionInfo.g_blnUnitInspected[0])
                    {
                        float fScore;
                        fScore = m_smVisionInfo.g_arrPin1[0].GetResultScore(m_smVisionInfo.g_intSelectedTemplate) * 100;
                        if (fScore < 0)
                            lbl_Pin1Score.Text = "----";
                        else
                            lbl_Pin1Score.Text = fScore.ToString("f2");
                    }
                    else
                        lbl_Pin1Score.Text = "----";

                    if (m_smVisionInfo.g_blnUnitInspected[1] && m_smVisionInfo.g_intUnitsOnImage == 2)
                    {
                        float fScore;
                        fScore = m_smVisionInfo.g_arrPin1[1].GetResultScore(m_smVisionInfo.g_intSelectedTemplate) * 100;
                        if (fScore < 0)
                            lbl_Pin1Score2.Text = "----";
                        else
                            lbl_Pin1Score2.Text = fScore.ToString("f2");
                    }
                    else
                        lbl_Pin1Score2.Text = "----";

                    LoadPin1Settings(m_smVisionInfo.g_intSelectedTemplate);
                }
                else
                {
                    if (tab_VisionControl.TabPages.Contains(tp_Pin1))
                        tab_VisionControl.TabPages.Remove(tp_Pin1);
                }
            }

            if ((m_intVisionType & 0x02) > 0)
            {
                UpdateOCVSettingTable();
                UpdateOCRSettingTable();
                UpdateMarkSettingGUI();
                UpdateMarkResultTable(!m_smVisionInfo.g_blnMarkInspected);
            }

            if ((m_intVisionType & 0x08) > 0)
            {
                UpdatePkgSettingGUI();
                UpdatePkgResultTable(!m_smVisionInfo.g_blnMarkInspected);
            }
        }

        private void UpdateOCRPatternTable()
        {
            //dgd_OcrPattern.Rows.Clear();
            //int intClass;

            //int intPatternChars = m_smVisionInfo.g_arrMarks[0].GetNumPatterns();
            //for (int i = 0; i < intPatternChars; i++)
            //{
            //    dgd_OcrPattern.Rows.Add();
            //    dgd_OcrPattern.Rows[i].Cells[0].Value = i + 1;

            //    dgd_OcrPattern.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[0].GetPattern(i);
            //    intClass = m_smVisionInfo.g_arrMarks[0].GetPatternClass(i);
            //    switch (intClass)
            //    {
            //        case 0: dgd_OcrPattern.Rows[i].Cells[2].Value = "Digit";
            //            break;
            //        case 1: dgd_OcrPattern.Rows[i].Cells[2].Value = "UpperCase";
            //            break;
            //        case 2: dgd_OcrPattern.Rows[i].Cells[2].Value = "LowerCase";
            //            break;
            //        case 3: dgd_OcrPattern.Rows[i].Cells[2].Value = "Special";
            //            break;
            //        case 4: dgd_OcrPattern.Rows[i].Cells[2].Value = "Extended";
            //            break;
            //    }

            //    dgd_OcrPattern.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[0].GetCharSetting(i);
            //}
        }

        private void UpdateOCRResultTable()
        {
            //dgd_OcrSetting.Rows.Clear();
            //int intColumn = 0;

            //for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            //{
            //    if (u == 0)
            //        intColumn = 3;
            //    else
            //        intColumn = 1;

            //    int intNumChars = m_smVisionInfo.g_arrMarks[0].GetNumChars();
            //    for (int i = 0; i < intNumChars; i++)
            //    {
            //        if (u == 0)
            //        {
            //            dgd_OcrSetting.Rows.Add();
            //            dgd_OcrSetting.Rows[i].Cells[0].Value = i + 1;
            //        }

            //        dgd_OcrSetting.Rows[i].Cells[intColumn].Value = m_smVisionInfo.g_arrMarks[u].GetChar(i).ToString();
            //        dgd_OcrSetting.Rows[i].Cells[intColumn + 1].Value = m_smVisionInfo.g_arrMarks[u].GetCharScore(i).ToString("F2");

            //        if (m_smVisionInfo.g_arrMarks[u].GetCharResult(i))
            //        {
            //            dgd_OcrSetting.Rows[i].Cells[intColumn].Style.BackColor = Color.Lime;
            //            dgd_OcrSetting.Rows[i].Cells[intColumn].Style.SelectionBackColor = Color.Lime;
            //            dgd_OcrSetting.Rows[i].Cells[intColumn + 1].Style.BackColor = Color.Lime;
            //            dgd_OcrSetting.Rows[i].Cells[intColumn + 1].Style.SelectionBackColor = Color.Lime;
            //        }
            //        else
            //        {
            //            dgd_OcrSetting.Rows[i].Cells[intColumn].Style.BackColor = Color.Red;
            //            dgd_OcrSetting.Rows[i].Cells[intColumn].Style.SelectionBackColor = Color.Red;
            //            dgd_OcrSetting.Rows[i].Cells[intColumn + 1].Style.BackColor = Color.Red;
            //            dgd_OcrSetting.Rows[i].Cells[intColumn + 1].Style.SelectionBackColor = Color.Red;
            //        }
            //    }
            //}
        }
        public void UpdateOCRSettingTable()
        {
            if (((m_smCustomizeInfo.g_intWantOCR2 & (1 << m_smVisionInfo.g_intVisionPos)) == 0) || (!m_smVisionInfo.g_blnUseOCR && !m_smVisionInfo.g_blnUseOCRandOCV))
            {
                return;
            }
            
            dgd_OCRSettings.Rows.Clear();
            int intColumn = 0;
            int counter = 0;
            int counter2 = 0;
            char[] s = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCR2GetMessage(0).ToCharArray();
            List<int> s1 = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetOCRResult();

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (u == 0)
                {
                    intColumn = 3;
                }
                else
                {
                    intColumn = 2;
                }

                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == '\n' || s[i] == ' ')
                    {
                        counter++;
                        continue;
                    }
                    else
                    {

                        if (dgd_OCRSettings.Rows.Count == counter2)
                        {
                            dgd_OCRSettings.Rows.Add();
                            dgd_OCRSettings.Rows[counter2].Cells[0].Value = s[counter].ToString();

                            dgd_OCRSettings.Rows[counter2].Cells[1].Value = m_smVisionInfo.g_arrMarks[u].GetOCRCharSetting(counter2).ToString();
                            dgd_OCRSettings.Rows[counter2].Cells[1].Style.BackColor = Color.White;
                            dgd_OCRSettings.Rows[counter2].Cells[1].Style.SelectionBackColor = Color.White;
                            dgd_OCRSettings.Rows[counter2].Cells[1].Style.ForeColor = Color.Black;
                            dgd_OCRSettings.Rows[counter2].Cells[1].Style.SelectionForeColor = Color.Black;
                            dgd_OCRSettings.Rows[counter2].Cells[3].Value = "----";
                            dgd_OCRSettings.Rows[counter2].Cells[3].Style.BackColor = Color.White;//Lime
                            dgd_OCRSettings.Rows[counter2].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                            dgd_OCRSettings.Rows[counter2].Cells[3].Style.ForeColor = Color.Black;
                            dgd_OCRSettings.Rows[counter2].Cells[3].Style.SelectionForeColor = Color.Black;
                        }

                        List<int> fScoreValue = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetOCRResult();

                        if (fScoreValue.Count > counter2 && fScoreValue[counter2] >= 0)
                        {
                            dgd_OCRSettings.Rows[counter2].Cells[intColumn].Value = fScoreValue[counter2].ToString();

                            if (Convert.ToDouble(dgd_OCRSettings.Rows[counter2].Cells[1].Value) <= Convert.ToDouble(dgd_OCRSettings.Rows[counter2].Cells[intColumn].Value))
                            {
                                dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.BackColor = Color.Lime;
                                dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.SelectionBackColor = Color.Lime;
                                dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.ForeColor = Color.Black;
                                dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                if (m_smVisionInfo.m_blnUpdateResultUsingByPassSetting)
                                {
                                    if (m_smVisionInfo.g_intMinMarkScore <= Convert.ToDouble(dgd_OCRSettings.Rows[counter2].Cells[intColumn].Value))
                                    {
                                        dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.BackColor = Color.LightSeaGreen;
                                        dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.SelectionBackColor = Color.LightSeaGreen;
                                        dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.ForeColor = Color.Black;
                                        dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.SelectionForeColor = Color.Black;
                                    }
                                    else
                                    {
                                        dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.BackColor = Color.Red;
                                        dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.SelectionBackColor = Color.Red;
                                        dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.ForeColor = Color.Yellow;
                                        dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.SelectionForeColor = Color.Yellow;
                                    }
                                }
                                else
                                {
                                    dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.BackColor = Color.Red;
                                    dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.SelectionBackColor = Color.Red;
                                    dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.ForeColor = Color.Yellow;
                                    dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.SelectionForeColor = Color.Yellow;
                                }
                            }
                        }
                        else
                        {
                            dgd_OCRSettings.Rows[counter2].Cells[intColumn].Value = "----";
                            dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.BackColor = Color.White;//Lime
                            dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.SelectionBackColor = Color.White;//Lime
                            dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.ForeColor = Color.Black;
                            dgd_OCRSettings.Rows[counter2].Cells[intColumn].Style.SelectionForeColor = Color.Black;
                        }
                    }
                    counter++;
                    counter2++;
                }
            }
        }
        private void UpdateOCVSettingTable()
        {
            dgd_Setting.Rows.Clear();
            int intColumn = 0;
            int intExcessColumn = 0;
            int intBrolenColumn = 0;
            int intAGVColumn = 0;


            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (u == 0)
                {
                    intColumn = 3;
                    intExcessColumn = 6;
                    intBrolenColumn = 9;
                    intAGVColumn = 13;
                }
                else
                {
                    intColumn = 2;
                    intExcessColumn = 5;
                    intBrolenColumn = 8;
                    intAGVColumn = 12;
                }

                for (int i = 0; i < m_smVisionInfo.g_arrMarks[u].GetNumChars(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate); i++)
                {
                    if (dgd_Setting.Rows.Count == i)
                    {
                        dgd_Setting.Rows.Add();
                        dgd_Setting.Rows[i].Cells[0].Value = "Mark " + (i + 1);

                        string strMarkType = "";
                        switch (m_smVisionInfo.g_arrMarks[0].GetMarktype(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate))
                        {
                            case 1:
                                strMarkType = "(L) ";
                                break;
                            case 2:
                                strMarkType = "(S1) ";
                                break;
                            case 3:
                                strMarkType = "(S2) ";
                                break;
                        }

                        if (m_smVisionInfo.g_arrMarks[u].GetEnableMarkSetting(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate))
                        {
                            dgd_Setting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[u].GetCharSetting(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString();
                            dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White;
                            dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White;
                            dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[3].Value = "----";
                            dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;

                            dgd_Setting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrMarks[u].GetCharMaxExcessAreaSetting(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                            dgd_Setting.Rows[i].Cells[4].Style.BackColor = Color.White;
                            dgd_Setting.Rows[i].Cells[4].Style.SelectionBackColor = Color.White;
                            dgd_Setting.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[5].Value = "----";
                            dgd_Setting.Rows[i].Cells[5].Style.BackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[5].Style.SelectionBackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[5].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[5].Style.SelectionForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[6].Value = "----";
                            dgd_Setting.Rows[i].Cells[6].Style.BackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[6].Style.SelectionBackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[6].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[6].Style.SelectionForeColor = Color.Black;

                            if (!m_smVisionInfo.g_arrMarks[0].CheckWantInspectExcessMark(m_smVisionInfo.g_arrMarks[0].GetMarktype(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate)))
                            {
                                dgd_Setting.Rows[i].Cells[4].Style.BackColor = Color.LightGray;
                                dgd_Setting.Rows[i].Cells[4].Style.SelectionBackColor = Color.LightGray;
                                dgd_Setting.Rows[i].Cells[4].ReadOnly = true;
                            }

                            dgd_Setting.Rows[i].Cells[7].Value = m_smVisionInfo.g_arrMarks[u].GetCharMaxBrokenAreaSetting(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                            dgd_Setting.Rows[i].Cells[7].Style.BackColor = Color.White;
                            dgd_Setting.Rows[i].Cells[7].Style.SelectionBackColor = Color.White;
                            dgd_Setting.Rows[i].Cells[7].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[7].Style.SelectionForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[8].Value = "----";
                            dgd_Setting.Rows[i].Cells[8].Style.BackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[8].Style.SelectionBackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[8].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[8].Style.SelectionForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[9].Value = "----";
                            dgd_Setting.Rows[i].Cells[9].Style.BackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[9].Style.SelectionBackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[9].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[9].Style.SelectionForeColor = Color.Black;

                            if (!m_smVisionInfo.g_arrMarks[0].CheckWantInspectMissingMark(m_smVisionInfo.g_arrMarks[0].GetMarktype(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate)))
                            {
                                dgd_Setting.Rows[i].Cells[7].Style.BackColor = Color.LightGray;
                                dgd_Setting.Rows[i].Cells[7].Style.SelectionBackColor = Color.LightGray;
                                dgd_Setting.Rows[i].Cells[7].ReadOnly = true;
                            }

                            dgd_Setting.Rows[i].Cells[10].Value = m_smVisionInfo.g_arrMarks[u].GetCharWantBrokenMarkSetting(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString();

                            if (!m_smVisionInfo.g_arrMarks[0].CheckWantInspectBrokenMark(m_smVisionInfo.g_arrMarks[0].GetMarktype(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate)))
                            {
                                dgd_Setting.Rows[i].Cells[10].Style.BackColor = Color.LightGray;
                                dgd_Setting.Rows[i].Cells[10].Style.SelectionBackColor = Color.LightGray;
                                //dgd_Setting.Rows[i].Cells[10].Value = false;
                                dgd_Setting.Rows[i].Cells[10].ReadOnly = true;
                            }

                            dgd_Setting.Rows[i].Cells[11].Value = m_smVisionInfo.g_arrMarks[u].GetMaxAGVPercent(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                            dgd_Setting.Rows[i].Cells[11].Style.BackColor = Color.White;
                            dgd_Setting.Rows[i].Cells[11].Style.SelectionBackColor = Color.White;
                            dgd_Setting.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[11].Style.SelectionForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[12].Value = "----";
                            dgd_Setting.Rows[i].Cells[12].Style.BackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[12].Style.SelectionBackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[12].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[12].Style.SelectionForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[13].Value = "----";
                            dgd_Setting.Rows[i].Cells[13].Style.BackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[13].Style.SelectionBackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[13].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[13].Style.SelectionForeColor = Color.Black;

                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[1].Value = "----"; // m_smVisionInfo.g_arrMarks[u].GetCharSetting(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString();
                            dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.LightGray;
                            dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.LightGray;
                            dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[3].Value = "----";
                            dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.LightGray;//Lime
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.LightGray;//Lime
                            dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;

                            dgd_Setting.Rows[i].Cells[4].Value = "----";
                            dgd_Setting.Rows[i].Cells[4].Style.BackColor = Color.LightGray;
                            dgd_Setting.Rows[i].Cells[4].Style.SelectionBackColor = Color.LightGray;
                            dgd_Setting.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[5].Value = "----";
                            dgd_Setting.Rows[i].Cells[5].Style.BackColor = Color.LightGray;//Lime
                            dgd_Setting.Rows[i].Cells[5].Style.SelectionBackColor = Color.LightGray;//Lime
                            dgd_Setting.Rows[i].Cells[5].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[5].Style.SelectionForeColor = Color.Black;

                            dgd_Setting.Rows[i].Cells[6].Value = 100.ToString();
                            dgd_Setting.Rows[i].Cells[6].Style.BackColor = Color.LightGray;
                            dgd_Setting.Rows[i].Cells[6].Style.SelectionBackColor = Color.LightGray;
                            dgd_Setting.Rows[i].Cells[6].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[6].Style.SelectionForeColor = Color.Black;

                            dgd_Setting.Rows[i].Cells[7].Value = "----";
                            dgd_Setting.Rows[i].Cells[7].Style.BackColor = Color.LightGray;//Lime
                            dgd_Setting.Rows[i].Cells[7].Style.SelectionBackColor = Color.LightGray;//Lime
                            dgd_Setting.Rows[i].Cells[7].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[7].Style.SelectionForeColor = Color.Black;

                            dgd_Setting.Rows[i].Cells[11].Value = "----";
                            dgd_Setting.Rows[i].Cells[11].Style.BackColor = Color.LightGray;//Lime
                            dgd_Setting.Rows[i].Cells[11].Style.SelectionBackColor = Color.LightGray;//Lime
                            dgd_Setting.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[11].Style.SelectionForeColor = Color.Black;

                            dgd_Setting.Rows[i].Cells[10].Value = "false";

                            if (!m_smVisionInfo.g_arrMarks[0].CheckWantInspectBrokenMark(m_smVisionInfo.g_arrMarks[0].GetMarktype(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate)))
                            {
                                dgd_Setting.Rows[i].Cells[10].Style.BackColor = Color.LightGray;
                                dgd_Setting.Rows[i].Cells[10].Style.SelectionBackColor = Color.LightGray;
                                //dgd_Setting.Rows[i].Cells[10].Value = false;
                                dgd_Setting.Rows[i].Cells[10].ReadOnly = true;
                            }

                        }
                    }

                    bool blnEnableMark = true;
                    float fScoreValue = -1;
                    float fExcessValue = -1;
                    float fBrokenValue = -1;
                    float fAGVPercent = -999;
                    int intWantBrokenMarkValue = -1;
                    if (m_smVisionInfo.g_blnMarkInspected)
                    {
                        fScoreValue = m_smVisionInfo.g_arrMarks[u].GetCharScore(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                        blnEnableMark = m_smVisionInfo.g_arrMarks[u].GetEnableMarkSetting(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                        fExcessValue = m_smVisionInfo.g_arrMarks[u].GetCharExcessArea(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                        fBrokenValue = m_smVisionInfo.g_arrMarks[u].GetCharBrokenArea(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                        fAGVPercent = m_smVisionInfo.g_arrMarks[u].GetCharAverageGrayDiff(i, m_smVisionInfo.g_intSelectedTemplate);
                        if (m_smVisionInfo.g_arrMarks[u].GetCharWantBrokenMark(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate))
                            intWantBrokenMarkValue = 1;
                        else
                            intWantBrokenMarkValue = 0;

                    }

                    if (fScoreValue >= 0 && blnEnableMark)
                    {
                        dgd_Setting.Rows[i].Cells[intColumn].Value = fScoreValue.ToString("F2");

                        //if (m_smVisionInfo.g_arrMarks[u].GetCharResult(i))
                        if (Convert.ToDouble(dgd_Setting.Rows[i].Cells[1].Value) <= Convert.ToDouble(dgd_Setting.Rows[i].Cells[intColumn].Value))
                        {
                            dgd_Setting.Rows[i].Cells[intColumn].Style.BackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[intColumn].Style.SelectionBackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[intColumn].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[intColumn].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            if (m_smVisionInfo.m_blnUpdateResultUsingByPassSetting)
                            {
                                if (m_smVisionInfo.g_intMinMarkScore <= Convert.ToDouble(dgd_Setting.Rows[i].Cells[intColumn].Value))
                                {
                                    dgd_Setting.Rows[i].Cells[intColumn].Style.BackColor = Color.LightSeaGreen;
                                    dgd_Setting.Rows[i].Cells[intColumn].Style.SelectionBackColor = Color.LightSeaGreen;
                                    dgd_Setting.Rows[i].Cells[intColumn].Style.ForeColor = Color.Black;
                                    dgd_Setting.Rows[i].Cells[intColumn].Style.SelectionForeColor = Color.Black;
                                }
                                else
                                {
                                    dgd_Setting.Rows[i].Cells[intColumn].Style.BackColor = Color.Red;
                                    dgd_Setting.Rows[i].Cells[intColumn].Style.SelectionBackColor = Color.Red;
                                    dgd_Setting.Rows[i].Cells[intColumn].Style.ForeColor = Color.Yellow;
                                    dgd_Setting.Rows[i].Cells[intColumn].Style.SelectionForeColor = Color.Yellow;
                                }
                            }
                            else
                            {
                                dgd_Setting.Rows[i].Cells[intColumn].Style.BackColor = Color.Red;
                                dgd_Setting.Rows[i].Cells[intColumn].Style.SelectionBackColor = Color.Red;
                                dgd_Setting.Rows[i].Cells[intColumn].Style.ForeColor = Color.Yellow;
                                dgd_Setting.Rows[i].Cells[intColumn].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[intColumn].Value = "----";
                        if (blnEnableMark)
                        {
                            dgd_Setting.Rows[i].Cells[intColumn].Style.BackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[intColumn].Style.SelectionBackColor = Color.White;//Lime
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[intColumn].Style.BackColor = Color.LightGray;//Lime
                            dgd_Setting.Rows[i].Cells[intColumn].Style.SelectionBackColor = Color.LightGray;//Lime
                        }
                        dgd_Setting.Rows[i].Cells[intColumn].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[intColumn].Style.SelectionForeColor = Color.Black;
                    }

                    if (fExcessValue >= 0 && blnEnableMark && m_smVisionInfo.g_arrMarks[u].CheckWantInspectExcessMark(m_smVisionInfo.g_arrMarks[u].GetMarktype(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate)))
                    {
                        dgd_Setting.Rows[i].Cells[intExcessColumn].Value = fExcessValue.ToString(GetDecimalFormat());
                        float fCharMaxExcessAreaSetting = m_smVisionInfo.g_arrMarks[u].GetCharMaxExcessAreaSetting(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);

                        if (fCharMaxExcessAreaSetting >= fExcessValue)
                        {
                            dgd_Setting.Rows[i].Cells[intExcessColumn].Style.BackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[intExcessColumn].Style.SelectionBackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[intExcessColumn].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[intExcessColumn].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[intExcessColumn].Style.BackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[intExcessColumn].Style.SelectionBackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[intExcessColumn].Style.ForeColor = Color.Yellow;
                            dgd_Setting.Rows[i].Cells[intExcessColumn].Style.SelectionForeColor = Color.Yellow;
                        }
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[intExcessColumn].Value = "----";
                        if (blnEnableMark)
                        {
                            dgd_Setting.Rows[i].Cells[intExcessColumn].Style.BackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[intExcessColumn].Style.SelectionBackColor = Color.White;//Lime
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[intExcessColumn].Style.BackColor = Color.LightGray;//Lime
                            dgd_Setting.Rows[i].Cells[intExcessColumn].Style.SelectionBackColor = Color.LightGray;//Lime
                        }
                        dgd_Setting.Rows[i].Cells[intExcessColumn].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[intExcessColumn].Style.SelectionForeColor = Color.Black;
                    }

                    if (fBrokenValue >= 0 && blnEnableMark && m_smVisionInfo.g_arrMarks[u].CheckWantInspectMissingMark(m_smVisionInfo.g_arrMarks[u].GetMarktype(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate)))
                    {
                        dgd_Setting.Rows[i].Cells[intBrolenColumn].Value = fBrokenValue.ToString(GetDecimalFormat());
                        float fCharMaxBrokenAreaSetting = m_smVisionInfo.g_arrMarks[u].GetCharMaxBrokenAreaSetting(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);

                        if (fCharMaxBrokenAreaSetting >= fBrokenValue)
                        {
                            dgd_Setting.Rows[i].Cells[intBrolenColumn].Style.BackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[intBrolenColumn].Style.SelectionBackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[intBrolenColumn].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[intBrolenColumn].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[intBrolenColumn].Style.BackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[intBrolenColumn].Style.SelectionBackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[intBrolenColumn].Style.ForeColor = Color.Yellow;
                            dgd_Setting.Rows[i].Cells[intBrolenColumn].Style.SelectionForeColor = Color.Yellow;
                        }
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[intBrolenColumn].Value = "----";
                        if (blnEnableMark)
                        {
                            dgd_Setting.Rows[i].Cells[intBrolenColumn].Style.BackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[intBrolenColumn].Style.SelectionBackColor = Color.White;//Lime
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[intBrolenColumn].Style.BackColor = Color.LightGray;//Lime
                            dgd_Setting.Rows[i].Cells[intBrolenColumn].Style.SelectionBackColor = Color.LightGray;//Lime
                        }
                        dgd_Setting.Rows[i].Cells[intBrolenColumn].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[intBrolenColumn].Style.SelectionForeColor = Color.Black;
                    }

                    //if (m_smVisionInfo.g_blnWantCheckMarkBroken)
                    //{
                    //    if ((m_smVisionInfo.g_arrMarks[u].GetFailOptionMask(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate) & 0x20) > 0)
                    //        dgd_Setting.Columns[10].Visible = true;
                    //    else
                    //        dgd_Setting.Columns[10].Visible = false;
                    //}
                    //else
                    //    dgd_Setting.Columns[10].Visible = false;

                    if (intWantBrokenMarkValue >= 0 && blnEnableMark &&
                        m_smVisionInfo.g_arrMarks[u].CheckWantInspectBrokenMark(m_smVisionInfo.g_arrMarks[u].GetMarktype(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate)))
                    {
                        bool blnCharMaxBrokenAreaSetting = m_smVisionInfo.g_arrMarks[u].GetCharWantBrokenMarkSetting(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);

                        if (blnCharMaxBrokenAreaSetting && (blnCharMaxBrokenAreaSetting && intWantBrokenMarkValue == 0) && m_smVisionInfo.g_blnMarkInspected)
                        {
                            if (m_smVisionInfo.g_arrMarks[u].ref_blnCharResult[m_smVisionInfo.g_intSelectedTemplate].Length == 0)
                            {
                                dgd_Setting.Rows[i].Cells[10].Style.BackColor = Color.White;
                                dgd_Setting.Rows[i].Cells[10].Style.SelectionBackColor = Color.White;
                                dgd_Setting.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                dgd_Setting.Rows[i].Cells[10].Style.SelectionForeColor = Color.Black;
                            }

                            else if (m_smVisionInfo.g_arrMarks[u].ref_blnCharResult[m_smVisionInfo.g_intSelectedTemplate][i])
                            {
                                dgd_Setting.Rows[i].Cells[10].Style.BackColor = Color.Lime;
                                dgd_Setting.Rows[i].Cells[10].Style.SelectionBackColor = Color.Lime;
                                dgd_Setting.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                dgd_Setting.Rows[i].Cells[10].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                dgd_Setting.Rows[i].Cells[10].Style.BackColor = Color.Red;
                                dgd_Setting.Rows[i].Cells[10].Style.SelectionBackColor = Color.Red;
                                dgd_Setting.Rows[i].Cells[10].Style.ForeColor = Color.Yellow;
                                dgd_Setting.Rows[i].Cells[10].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[10].Style.BackColor = Color.White;
                            dgd_Setting.Rows[i].Cells[10].Style.SelectionBackColor = Color.White;
                            dgd_Setting.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[10].Style.SelectionForeColor = Color.Black;
                        }
                    }
                    else
                    {
                        if (blnEnableMark)
                        {
                            dgd_Setting.Rows[i].Cells[10].Style.BackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[10].Style.SelectionBackColor = Color.White;//Lime
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[10].Style.BackColor = Color.LightGray;//Lime
                            dgd_Setting.Rows[i].Cells[10].Style.SelectionBackColor = Color.LightGray;//Lime
                        }
                        dgd_Setting.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[10].Style.SelectionForeColor = Color.Black;
                    }

                    if (blnEnableMark && fAGVPercent != -999)
                    {
                        dgd_Setting.Rows[i].Cells[intAGVColumn].Value = fAGVPercent.ToString("F2");

                        double dValue = 0;
                        if (double.TryParse(dgd_Setting.Rows[i].Cells[11].Value.ToString(), out dValue))
                        {
                            if (Convert.ToDouble(dgd_Setting.Rows[i].Cells[11].Value) >= Math.Abs(Convert.ToDouble(dgd_Setting.Rows[i].Cells[intAGVColumn].Value)))
                            {
                                dgd_Setting.Rows[i].Cells[intAGVColumn].Style.BackColor = Color.Lime;
                                dgd_Setting.Rows[i].Cells[intAGVColumn].Style.SelectionBackColor = Color.Lime;
                                dgd_Setting.Rows[i].Cells[intAGVColumn].Style.ForeColor = Color.Black;
                                dgd_Setting.Rows[i].Cells[intAGVColumn].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                dgd_Setting.Rows[i].Cells[intAGVColumn].Style.BackColor = Color.Red;
                                dgd_Setting.Rows[i].Cells[intAGVColumn].Style.SelectionBackColor = Color.Red;
                                dgd_Setting.Rows[i].Cells[intAGVColumn].Style.ForeColor = Color.Yellow;
                                dgd_Setting.Rows[i].Cells[intAGVColumn].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[intAGVColumn].Value = "----";
                            dgd_Setting.Rows[i].Cells[intAGVColumn].Style.BackColor = Color.White;
                            dgd_Setting.Rows[i].Cells[intAGVColumn].Style.SelectionBackColor = Color.White;
                            dgd_Setting.Rows[i].Cells[intAGVColumn].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[intAGVColumn].Style.SelectionForeColor = Color.Black;
                        }

                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[intAGVColumn].Value = "----";
                        if (blnEnableMark)
                        {
                            dgd_Setting.Rows[i].Cells[intAGVColumn].Style.BackColor = Color.White;//Lime
                            dgd_Setting.Rows[i].Cells[intAGVColumn].Style.SelectionBackColor = Color.White;//Lime
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[intAGVColumn].Style.BackColor = Color.LightGray;//Lime
                            dgd_Setting.Rows[i].Cells[intAGVColumn].Style.SelectionBackColor = Color.LightGray;//Lime
                        }
                        dgd_Setting.Rows[i].Cells[intAGVColumn].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[intAGVColumn].Style.SelectionForeColor = Color.Black;
                    }


                }
            }
        }

        private void UpdateMarkSettingGUI()
        {
            int intSelectedUnit = GetBiggerTemplateUnitNo();

            for (int i = 0; i < dgd_MarkSetting.Rows.Count; i++)
            {
                switch (dgd_MarkSetting.Rows[i].Cells[0].Value.ToString())
                {
                    case "Max Excess Area":
                        dgd_MarkSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetExcessMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString();
                        break;
                    case "Max Total Excess Area":
                        dgd_MarkSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetGroupExcessMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                        break;
                    case "Max Extra Area":
                        dgd_MarkSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetExtraMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                        break;
                    case "Max Total Extra Area":
                        dgd_MarkSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetGroupExtraMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                        break;
                    case "Max Miss Area":
                        dgd_MarkSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetMissingMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString();
                        break;
                    case "Char Shift Tolerance":
                        dgd_MarkSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetCharShiftXY(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString();
                        break;
                    case "Text Shift Tole X":
                        dgd_MarkSetting.Rows[i].Cells[1].Value = "NA";
                        break;
                    case "Text Shift Tole Y":
                        dgd_MarkSetting.Rows[i].Cells[1].Value = "NA";
                        break;
                    case "Text Min Score":
                        dgd_MarkSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetTextMinScore(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString();
                        break;
                    case "Min Top Edge Distance":
                        dgd_MarkSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetUnCheckAreaTop(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                        break;
                    case "Min Btm Edge Distance":
                        dgd_MarkSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetUnCheckAreaBottom(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                        break;
                    case "Min Left Edge Distance":
                        dgd_MarkSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetUnCheckAreaLeft(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                        break;
                    case "Min Right Edge Distance":
                        dgd_MarkSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetUnCheckAreaRight(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                        break;
                    case "Mark Angle Tol.":
                        dgd_MarkSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetMarkAngleTolerance(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString("f4");
                        break;
                    case "Min No Mark Area":
                        dgd_MarkSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetNoMarkMaximumBlobArea().ToString(GetDecimalFormat());
                        break;
                }
            }
        }

        private void UpdatePkgSettingGUI()
        {
            int intSelectedUnit = GetBiggerTemplateUnitNo();

            for (int i = 0; i < dgd_PkgSetting.Rows.Count; i++)
            {
                switch (dgd_PkgSetting.Rows[i].Cells[0].Value.ToString())
                {
                    case "Package Width":
                        dgd_PkgSetting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetUnitWidthMin(1), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        dgd_PkgSetting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetUnitWidthMax(1), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        break;
                    case "Package Length":
                        dgd_PkgSetting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetUnitHeightMin(1), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        dgd_PkgSetting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetUnitHeightMax(1), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        break;
                    case "Package Angle":
                        dgd_PkgSetting.Rows[i].Cells[1].Value = "NA";
                        dgd_PkgSetting.Rows[i].Cells[1].ReadOnly = true;
                        dgd_PkgSetting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetUnitAngleMax(), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        break;
                    case "Scratches Length":
                        dgd_PkgSetting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(2, 0) / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        break;
                    case "Contamination Length":
                        dgd_PkgSetting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(4, 0) / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        break;
                    case "Chipped Off Length":
                        dgd_PkgSetting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(1, 0) / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        break;
                    case "Cracked Length":
                        dgd_PkgSetting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(0, 0) / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        break;
                    case "Mold Flash Length":
                        dgd_PkgSetting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(3, 0) / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        break;
                    case "Scratches Area":
                        dgd_PkgSetting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(2) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        break;
                    case "Contamination Area":
                        dgd_PkgSetting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(4) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        break;
                    case "Chipped Off Area":
                        dgd_PkgSetting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(1) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        break;
                    case "Cracked Area":
                        dgd_PkgSetting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(0) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        break;
                    case "Mold Flash Area":
                        dgd_PkgSetting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(3) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        break;
                    case "Void Length":
                        dgd_PkgSetting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(5, 0) / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        break;
                    case "Void Area":
                        dgd_PkgSetting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(5) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                        break;

                }
            }

            //General
            txt_GeneralVerticalBright.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(2, 1) / m_smVisionInfo.g_fCalibPixelY, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
            txt_GeneralVerticalDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(4, 1) / m_smVisionInfo.g_fCalibPixelY, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
            txt_GeneralHorizontalBright.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(2, 0) / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
            txt_GeneralHorizontalDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(4, 0) / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
            txt_GeneralAreaBright.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(2) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
            txt_GeneralAreaDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(4) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
            txt_GeneralTotalAreaBright.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectTotalAreaParam(2) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
            txt_GeneralTotalAreaDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectTotalAreaParam(4) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
            txt_GeneralChipAreaBright.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(1) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
            txt_GeneralChipAreaDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(6) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
            cbo_GeneralBrightDefectFailCondition.SelectedIndex = m_smVisionInfo.g_arrPackage[intSelectedUnit].ref_intBrightDefectDimensionFailCondition;
            cbo_GeneralDarkDefectFailCondition.SelectedIndex = m_smVisionInfo.g_arrPackage[intSelectedUnit].ref_intDarkDefectDimensionFailCondition;

            //condition for Dark Field 2
            if (m_smVisionInfo.g_arrPackage[intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
            {
                txt_Dark2VerticalDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(7, 1) / m_smVisionInfo.g_fCalibPixelY, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_Dark2HorizontalDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(7, 0) / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_Dark2AreaDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(7) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_Dark2TotalAreaDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectTotalAreaParam(7) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                cbo_GeneralDark2DefectFailCondition.SelectedIndex = m_smVisionInfo.g_arrPackage[intSelectedUnit].ref_intDark2DefectDimensionFailCondition;
            }

            if (m_smVisionInfo.g_arrPackage[intSelectedUnit].ref_blnSeperateDarkField3DefectSetting)
            {
                txt_Dark3VerticalDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(8, 1) / m_smVisionInfo.g_fCalibPixelY, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_Dark3HorizontalDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(8, 0) / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_Dark3AreaDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(8) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_Dark3TotalAreaDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectTotalAreaParam(8) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                cbo_GeneralDark3DefectFailCondition.SelectedIndex = m_smVisionInfo.g_arrPackage[intSelectedUnit].ref_intDark3DefectDimensionFailCondition;
            }

            if (m_smVisionInfo.g_arrPackage[intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
            {
                txt_Dark4VerticalDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(9, 1) / m_smVisionInfo.g_fCalibPixelY, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_Dark4HorizontalDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(9, 0) / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_Dark4AreaDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(9) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_Dark4TotalAreaDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectTotalAreaParam(9) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                cbo_GeneralDark4DefectFailCondition.SelectedIndex = m_smVisionInfo.g_arrPackage[intSelectedUnit].ref_intDark4DefectDimensionFailCondition;
            }

            //condition for crack
            if (m_smVisionInfo.g_arrPackage[intSelectedUnit].ref_blnSeperateCrackDefectSetting)
            {
                txt_CrackVerticalDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(0, 1) / m_smVisionInfo.g_fCalibPixelY, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_CrackHorizontalDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(0, 0) / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_CrackAreaDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(0) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                cbo_CrackDarkDefectFailCondition.SelectedIndex = m_smVisionInfo.g_arrPackage[intSelectedUnit].ref_intCrackDarkDefectDimensionFailCondition;
            }

            //condition for Chipped
            if (m_smVisionInfo.g_arrPackage[intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
            {
                //txt_VoidVerticalDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(5, 1) / m_smVisionInfo.g_fCalibPixelY, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                //txt_VoidHorizontalDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(5, 0) / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_ChippedAreaBright.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(1) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_ChippedAreaDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(6) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_ChippedHorizontalBright.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(1, 0) / (m_smVisionInfo.g_fCalibPixelX), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_ChippedVerticalBright.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(1, 1) / (m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_ChippedHorizontalDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(6, 0) / (m_smVisionInfo.g_fCalibPixelX), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_ChippedVerticalDark.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(6, 1) / (m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                cbo_ChippedBrightFailCondition.SelectedIndex = m_smVisionInfo.g_arrPackage[intSelectedUnit].ref_intChippedBrightDefectDimensionFailCondition;
                cbo_ChippedDarkFailCondition.SelectedIndex = m_smVisionInfo.g_arrPackage[intSelectedUnit].ref_intChippedDarkDefectDimensionFailCondition;
            }

            //condition for mold flash
            if (m_smVisionInfo.g_arrPackage[intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
            {
                txt_MoldFlashVerticalBright.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(3, 1) / m_smVisionInfo.g_fCalibPixelY, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_MoldFlashHorizontalBright.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectParam(3, 0) / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_MoldFlashAreaBright.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectAreaParam(3) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
                txt_MoldFlashTotalAreaBright.Text = Math.Round(m_smVisionInfo.g_arrPackage[intSelectedUnit].GetDefectTotalAreaParam(3) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 4, MidpointRounding.AwayFromZero).ToString("F" + m_intDecimal);
            }
        }

        private void UpdateMarkResultTable(bool blnReset)
        {
            for (int i = 0; i < dgd_MarkSetting.Rows.Count; i++)
            {
                switch (dgd_MarkSetting.Rows[i].Cells[0].Value.ToString())
                {
                    case "Max Excess Area":
                        dgd_MarkSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[0].GetResultBiggestExcessArea(m_smVisionInfo.g_intSelectedTemplate).ToString();
                        if (!blnReset && Convert.ToInt32(dgd_MarkSetting.Rows[i].Cells[3].Value) > Convert.ToInt32(dgd_MarkSetting.Rows[i].Cells[1].Value))
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }

                        if (m_smVisionInfo.g_intUnitsOnImage > 1)
                        {
                            dgd_MarkSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[1].GetResultBiggestExcessArea(m_smVisionInfo.g_intSelectedTemplate).ToString();
                            if (!blnReset && Convert.ToInt32(dgd_MarkSetting.Rows[i].Cells[2].Value) > Convert.ToInt32(dgd_MarkSetting.Rows[i].Cells[1].Value))
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        break;
                    case "Max Total Excess Area":
                        dgd_MarkSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[0].GetResultGroupExcessArea(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                        if (!blnReset && Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[3].Value) > Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[1].Value))
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }

                        if (m_smVisionInfo.g_intUnitsOnImage > 1)
                        {
                            dgd_MarkSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[1].GetResultGroupExcessArea(m_smVisionInfo.g_intSelectedTemplate).ToString();
                            if (!blnReset && Convert.ToInt32(dgd_MarkSetting.Rows[i].Cells[2].Value) > Convert.ToInt32(dgd_MarkSetting.Rows[i].Cells[1].Value))
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        break;
                    case "Max Extra Area":
                        dgd_MarkSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[0].GetResultBiggestExtraArea(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                        if (!blnReset && Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[3].Value) > Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[1].Value))
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }

                        if (m_smVisionInfo.g_intUnitsOnImage > 1)
                        {
                            dgd_MarkSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[1].GetResultBiggestExtraArea(m_smVisionInfo.g_intSelectedTemplate).ToString();
                            if (!blnReset && Convert.ToInt32(dgd_MarkSetting.Rows[i].Cells[2].Value) > Convert.ToInt32(dgd_MarkSetting.Rows[i].Cells[1].Value))
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        break;
                    case "Max Total Extra Area":
                        dgd_MarkSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[0].GetResultGroupExtraArea(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                        if (!blnReset && Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[3].Value) > Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[1].Value))
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }

                        if (m_smVisionInfo.g_intUnitsOnImage > 1)
                        {
                            dgd_MarkSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[1].GetResultGroupExtraArea(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                            if (!blnReset && Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[2].Value) > Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[1].Value))
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        break;
                    case "Max Miss Area":
                        dgd_MarkSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[0].GetResultBiggestMissingArea(m_smVisionInfo.g_intSelectedTemplate).ToString();
                        if (!blnReset && Convert.ToInt32(dgd_MarkSetting.Rows[i].Cells[3].Value) > Convert.ToInt32(dgd_MarkSetting.Rows[i].Cells[1].Value))
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }

                        if (m_smVisionInfo.g_intUnitsOnImage > 1)
                        {
                            dgd_MarkSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[1].GetResultBiggestMissingArea(m_smVisionInfo.g_intSelectedTemplate).ToString();
                            if (!blnReset && Convert.ToInt32(dgd_MarkSetting.Rows[i].Cells[2].Value) > Convert.ToInt32(dgd_MarkSetting.Rows[i].Cells[1].Value))
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        break;
                    case "Char Shift Tolerance":
                        dgd_MarkSetting.Rows[i].Cells[2].Value = "NA";
                        dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                        dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                        dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                        dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;

                        dgd_MarkSetting.Rows[i].Cells[3].Value = "NA";
                        dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        break;
                    case "Text Shift Tole X":
                        dgd_MarkSetting.Rows[i].Cells[3].Value = "NA";
                        dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        break;
                    case "Text Shift Tole Y":
                        dgd_MarkSetting.Rows[i].Cells[3].Value = "NA";
                        dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        break;
                    case "Text Min Score":
                        dgd_MarkSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[0].GetResultTextMatchScore(m_smVisionInfo.g_intSelectedTemplate).ToString("F1");
                        if (!blnReset && Convert.ToSingle(dgd_MarkSetting.Rows[i].Cells[3].Value) < Convert.ToSingle(dgd_MarkSetting.Rows[i].Cells[1].Value))
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }

                        if (m_smVisionInfo.g_intUnitsOnImage > 1)
                        {
                            dgd_MarkSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[1].GetResultTextMatchScore(m_smVisionInfo.g_intSelectedTemplate).ToString("F1");
                            if (!blnReset && Convert.ToSingle(dgd_MarkSetting.Rows[i].Cells[2].Value) < Convert.ToSingle(dgd_MarkSetting.Rows[i].Cells[1].Value))
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        break;
                    case "Min Top Edge Distance":
                        if (m_smVisionInfo.g_arrMarks[0].GetResultTextShiftedTop(m_smVisionInfo.g_intSelectedTemplate).ToString() == "-999")
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[0].GetResultTextShiftedTop(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                            if (!blnReset && Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[3].Value) < Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[1].Value))
                            {
                                dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            }
                        }

                        if (m_smVisionInfo.g_intUnitsOnImage > 1)
                        {
                            if (m_smVisionInfo.g_arrMarks[1].GetResultTextShiftedTop(m_smVisionInfo.g_intSelectedTemplate).ToString() == "-999")
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[1].GetResultTextShiftedTop(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                                if (!blnReset && Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[2].Value) < Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[1].Value))
                                {
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                                }
                                else
                                {
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                                }
                            }
                        }
                        break;
                    case "Min Btm Edge Distance":
                        if (m_smVisionInfo.g_arrMarks[0].GetResultTextShiftedBottom(m_smVisionInfo.g_intSelectedTemplate).ToString() == "-999")
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[0].GetResultTextShiftedBottom(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                            if (!blnReset && Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[3].Value) < Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[1].Value))
                            {
                                dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            }
                        }

                        if (m_smVisionInfo.g_intUnitsOnImage > 1)
                        {
                            if (m_smVisionInfo.g_arrMarks[1].GetResultTextShiftedBottom(m_smVisionInfo.g_intSelectedTemplate).ToString() == "-999")
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[1].GetResultTextShiftedBottom(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                                if (!blnReset && Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[2].Value) < Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[1].Value))
                                {
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                                }
                                else
                                {
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                                }
                            }
                        }
                        break;
                    case "Min Left Edge Distance":
                        if (m_smVisionInfo.g_arrMarks[0].GetResultTextShiftedLeft(m_smVisionInfo.g_intSelectedTemplate).ToString() == "-999")
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[0].GetResultTextShiftedLeft(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                            if (!blnReset && Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[3].Value) < Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[1].Value))
                            {
                                dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            }
                        }

                        if (m_smVisionInfo.g_intUnitsOnImage > 1)
                        {
                            if (m_smVisionInfo.g_arrMarks[1].GetResultTextShiftedLeft(m_smVisionInfo.g_intSelectedTemplate).ToString() == "-999")
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[1].GetResultTextShiftedLeft(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                                if (!blnReset && Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[2].Value) < Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[1].Value))
                                {
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                                }
                                else
                                {
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                                }
                            }
                        }
                        break;
                    case "Min Right Edge Distance":
                        if (m_smVisionInfo.g_arrMarks[0].GetResultTextShiftedRight(m_smVisionInfo.g_intSelectedTemplate).ToString() == "-999")
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[0].GetResultTextShiftedRight(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                            if (!blnReset && Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[3].Value) < Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[1].Value))
                            {
                                dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            }
                        }

                        if (m_smVisionInfo.g_intUnitsOnImage > 1)
                        {
                            if (m_smVisionInfo.g_arrMarks[1].GetResultTextShiftedRight(m_smVisionInfo.g_intSelectedTemplate).ToString() == "-999")
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {

                                dgd_MarkSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[1].GetResultTextShiftedRight(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                                if (!blnReset && Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[2].Value) < Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[1].Value))
                                {
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                                }
                                else
                                {
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                                }
                            }
                        }
                        break;
                    case "Mark Angle Tol.":
                        //dgd_MarkSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[0].GetResultMarkAngle(m_smVisionInfo.g_intSelectedTemplate).ToString();
                        dgd_MarkSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_fOrientAngle[0].ToString("f4");
                        if (!blnReset && Math.Abs(Convert.ToSingle(dgd_MarkSetting.Rows[i].Cells[3].Value.ToString())) > Convert.ToSingle(dgd_MarkSetting.Rows[i].Cells[1].Value.ToString()))
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }

                        if (m_smVisionInfo.g_intUnitsOnImage > 1)
                        {
                            dgd_MarkSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_fOrientAngle[1].ToString("f4");
                            if (!blnReset && Math.Abs(Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[2].Value.ToString())) > Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[1].Value.ToString()))
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        break;
                    case "Min No Mark Area":
                        if (m_smVisionInfo.g_blnWantCheckNoMark)
                        {
                            dgd_MarkSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[0].GetResultTotalBlobArea().ToString(GetDecimalFormat());
                            if ((float)Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[1].Value) > (float)Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[3].Value))
                            {
                                dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_MarkSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                dgd_MarkSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            }

                            if (m_smVisionInfo.g_intUnitsOnImage > 1)
                            {
                                dgd_MarkSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[1].GetResultTotalBlobArea().ToString(GetDecimalFormat());
                                if ((float)Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[1].Value) > (float)Convert.ToDouble(dgd_MarkSetting.Rows[i].Cells[2].Value))
                                {
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                                }
                                else
                                {
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                    dgd_MarkSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void UpdatePkgResultTable(bool blnReset)
        {
            for (int i = 0; i < dgd_PkgSetting.Rows.Count; i++)
            {
                switch (dgd_PkgSetting.Rows[i].Cells[0].Value.ToString())
                {
                    case "Package Width":
                        if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnPkgSizeInspectionDone)
                        {
                            dgd_PkgSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetResultWidth(1).ToString("F" + m_intDecimal);
                            if (!blnReset && (Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[4].Value) < Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[1].Value) ||
                                Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[4].Value) > Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[2].Value)))
                            {
                                dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = Color.Red;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = Color.Lime;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Lime;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        else
                        {
                            dgd_PkgSetting.Rows[i].Cells[4].Value = "---";
                            dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = Color.Lime;
                            dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Lime;
                            dgd_PkgSetting.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                            dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                        }

                        if (m_smVisionInfo.g_intUnitsOnImage > 1)
                        {
                            if (m_smVisionInfo.g_arrPackage[1].ref_blnPkgSizeInspectionDone)
                            {
                                dgd_PkgSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrPackage[1].GetResultWidth(1).ToString("F" + m_intDecimal);
                                if (!blnReset && (Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[3].Value) < Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[1].Value) ||
                                    Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[3].Value) > Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[2].Value)))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                                }
                                else
                                {
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                dgd_PkgSetting.Rows[i].Cells[3].Value = "---";
                                dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_PkgSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        break;
                    case "Package Length":
                        if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnPkgSizeInspectionDone)
                        {
                            dgd_PkgSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetResultHeight(1).ToString("F" + m_intDecimal);
                            if (!blnReset && (Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[4].Value) < Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[1].Value) ||
                                Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[4].Value) > Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[2].Value)))
                            {
                                dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = Color.Red;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = Color.Lime;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Lime;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        else
                        {
                            dgd_PkgSetting.Rows[i].Cells[4].Value = "---";
                            dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = Color.Lime;
                            dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Lime;
                            dgd_PkgSetting.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                            dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                        }

                        if (m_smVisionInfo.g_intUnitsOnImage > 1)
                        {
                            if (m_smVisionInfo.g_arrPackage[1].ref_blnPkgSizeInspectionDone)
                            {
                                dgd_PkgSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrPackage[1].GetResultHeight(1).ToString("F" + m_intDecimal);
                                if (!blnReset && (Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[3].Value) < Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[1].Value) ||
                                    Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[3].Value) > Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[2].Value)))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                                }
                                else
                                {
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                dgd_PkgSetting.Rows[i].Cells[3].Value = "---";
                                dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_PkgSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        break;
                    case "Package Angle":
                        if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnPkgSizeInspectionDone)
                        {
                            dgd_PkgSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetResultAngle().ToString("F" + m_intDecimal);
                            if (!blnReset && (Math.Abs(Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[4].Value)) > Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[2].Value)))
                            {
                                dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = Color.Red;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = Color.Lime;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Lime;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                                dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        else
                        {
                            dgd_PkgSetting.Rows[i].Cells[4].Value = "---";
                            dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = Color.Lime;
                            dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Lime;
                            dgd_PkgSetting.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                            dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                        }
                        if (m_smVisionInfo.g_intUnitsOnImage > 1)
                        {
                            if (m_smVisionInfo.g_arrPackage[1].ref_blnPkgSizeInspectionDone)
                            {
                                dgd_PkgSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrPackage[1].GetResultAngle().ToString("F" + m_intDecimal);
                                if (!blnReset && (Math.Abs(Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[3].Value)) > Convert.ToSingle(dgd_PkgSetting.Rows[i].Cells[2].Value)))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                                }
                                else
                                {
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                    dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                dgd_PkgSetting.Rows[i].Cells[3].Value = "---";
                                dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_PkgSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        break;
                }
            }
        }

        private void AddMarkSettingGUI()
        {
            dgd_MarkSetting.Rows.Clear();

            int intSelectedUnit = GetBiggerTemplateUnitNo();

            int intFailMask = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetFailOptionMask(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);

            int i = 0;

            //dgd_MarkSetting.Rows.Add();
            //dgd_MarkSetting.Rows[i].Cells[0].Value = "Max Excess Area";
            //dgd_MarkSetting.Rows[i].Cells[1].Value = "10";
            //dgd_MarkSetting.Rows[i].Cells[2].Value = "0";
            //dgd_MarkSetting.Rows[i].Cells[3].Value = "0";
            //dgd_MarkSetting.Rows[i].Cells[4].Value = "pix";
            //i++;

            if ((intFailMask & 0x06) > 0)   // 0x02 = Extra Mark Center Area, 0x04=Extra Mark Side Area
            {
                dgd_MarkSetting.Rows.Add();
                dgd_MarkSetting.Rows[i].Cells[0].Value = "Max Extra Area";
                dgd_MarkSetting.Rows[i].Cells[1].Value = "10";
                dgd_MarkSetting.Rows[i].Cells[2].Value = "0";
                dgd_MarkSetting.Rows[i].Cells[3].Value = "0";
                dgd_MarkSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetDisplayUnitName(m_smCustomizeInfo.g_intMarkUnitDisplay, true);
                i++;
            }

            if ((intFailMask & 0x08) > 0)   // 0x08=Extra Mark Group Area
            {
                dgd_MarkSetting.Rows.Add();
                dgd_MarkSetting.Rows[i].Cells[0].Value = "Max Total Extra Area";
                dgd_MarkSetting.Rows[i].Cells[1].Value = "20";
                dgd_MarkSetting.Rows[i].Cells[2].Value = "0";
                dgd_MarkSetting.Rows[i].Cells[3].Value = "0";
                dgd_MarkSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetDisplayUnitName(m_smCustomizeInfo.g_intMarkUnitDisplay, true);
                i++;
            }

            if (((intFailMask & 0x100) > 0) && m_smVisionInfo.g_blnWantCheckMarkTotalExcess)   // 0x100=Excess Mark Group Area
            {
                dgd_MarkSetting.Rows.Add();
                dgd_MarkSetting.Rows[i].Cells[0].Value = "Max Total Excess Area";
                dgd_MarkSetting.Rows[i].Cells[1].Value = "100";
                dgd_MarkSetting.Rows[i].Cells[2].Value = "0";
                dgd_MarkSetting.Rows[i].Cells[3].Value = "0";
                dgd_MarkSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetDisplayUnitName(m_smCustomizeInfo.g_intMarkUnitDisplay, true);
                i++;
            }

            //dgd_MarkSetting.Rows.Add();
            //dgd_MarkSetting.Rows[i].Cells[0].Value = "Max Miss Area";
            //dgd_MarkSetting.Rows[i].Cells[1].Value = "10";
            //dgd_MarkSetting.Rows[i].Cells[2].Value = "0";
            //dgd_MarkSetting.Rows[i].Cells[3].Value = "0";
            //dgd_MarkSetting.Rows[i].Cells[4].Value = "pix";
            //i++;

            //dgd_MarkSetting.Rows.Add();
            //dgd_MarkSetting.Rows[i].Cells[0].Value = "Char Shift Tolerance";
            //dgd_MarkSetting.Rows[i].Cells[1].Value = "5";
            //dgd_MarkSetting.Rows[i].Cells[2].Value = "NA";
            //dgd_MarkSetting.Rows[i].Cells[3].Value = "NA";
            //dgd_MarkSetting.Rows[i].Cells[4].Value = "pix";
            //i++;

            //dgd_MarkSetting.Rows.Add();
            //dgd_MarkSetting.Rows[i].Cells[0].Value = "Text Shift Tole X";
            //dgd_MarkSetting.Rows[i].Cells[1].Value = "20";
            //dgd_MarkSetting.Rows[i].Cells[2].Value = "NA";
            //dgd_MarkSetting.Rows[i].Cells[3].Value = "pix";
            //i++;

            //dgd_MarkSetting.Rows.Add();
            //dgd_MarkSetting.Rows[i].Cells[0].Value = "Text Shift Tole Y";
            //dgd_MarkSetting.Rows[i].Cells[1].Value = "20";
            //dgd_MarkSetting.Rows[i].Cells[2].Value = "NA";
            //dgd_MarkSetting.Rows[i].Cells[3].Value = "pix";
            //i++;

            //dgd_MarkSetting.Rows.Add();
            //dgd_MarkSetting.Rows[i].Cells[0].Value = "Text Min Score";
            //dgd_MarkSetting.Rows[i].Cells[1].Value = "50";
            //dgd_MarkSetting.Rows[i].Cells[2].Value = "0";
            //dgd_MarkSetting.Rows[i].Cells[3].Value = "0";
            //dgd_MarkSetting.Rows[i].Cells[4].Value = "%";
            //i++;

            if ((intFailMask & 0x40) > 0)
            {
                dgd_MarkSetting.Rows.Add();
                dgd_MarkSetting.Rows[i].Cells[0].Value = "Min Top Edge Distance";
                dgd_MarkSetting.Rows[i].Cells[1].Value = "5";
                dgd_MarkSetting.Rows[i].Cells[2].Value = "NA";
                dgd_MarkSetting.Rows[i].Cells[3].Value = "NA";
                dgd_MarkSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetDisplayUnitName(m_smCustomizeInfo.g_intMarkUnitDisplay, false);
                i++;

                dgd_MarkSetting.Rows.Add();
                dgd_MarkSetting.Rows[i].Cells[0].Value = "Min Btm Edge Distance";
                dgd_MarkSetting.Rows[i].Cells[1].Value = "5";
                dgd_MarkSetting.Rows[i].Cells[2].Value = "NA";
                dgd_MarkSetting.Rows[i].Cells[3].Value = "NA";
                dgd_MarkSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetDisplayUnitName(m_smCustomizeInfo.g_intMarkUnitDisplay, false);
                i++;

                dgd_MarkSetting.Rows.Add();
                dgd_MarkSetting.Rows[i].Cells[0].Value = "Min Left Edge Distance";
                dgd_MarkSetting.Rows[i].Cells[1].Value = "5";
                dgd_MarkSetting.Rows[i].Cells[2].Value = "NA";
                dgd_MarkSetting.Rows[i].Cells[3].Value = "NA";
                dgd_MarkSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetDisplayUnitName(m_smCustomizeInfo.g_intMarkUnitDisplay, false);
                i++;

                dgd_MarkSetting.Rows.Add();
                dgd_MarkSetting.Rows[i].Cells[0].Value = "Min Right Edge Distance";
                dgd_MarkSetting.Rows[i].Cells[1].Value = "5";
                dgd_MarkSetting.Rows[i].Cells[2].Value = "NA";
                dgd_MarkSetting.Rows[i].Cells[3].Value = "NA";
                dgd_MarkSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetDisplayUnitName(m_smCustomizeInfo.g_intMarkUnitDisplay, false);
                i++;
            }

            if (m_smVisionInfo.g_blnWantCheckMarkAngle && (intFailMask & 0x2000) > 0)   // 0x2000 = Mark Angle
            {
                dgd_MarkSetting.Rows.Add();
                dgd_MarkSetting.Rows[i].Cells[0].Value = "Mark Angle Tol.";
                dgd_MarkSetting.Rows[i].Cells[1].Value = "15";
                dgd_MarkSetting.Rows[i].Cells[2].Value = "NA";
                dgd_MarkSetting.Rows[i].Cells[3].Value = "NA";
                dgd_MarkSetting.Rows[i].Cells[4].Value = "deg";
                i++;
            }

            if (m_smVisionInfo.g_blnWantCheckNoMark)
            {
                dgd_MarkSetting.Rows.Add();
                dgd_MarkSetting.Rows[i].Cells[0].Value = "Min No Mark Area";
                dgd_MarkSetting.Rows[i].Cells[1].Value = "20";
                dgd_MarkSetting.Rows[i].Cells[2].Value = "0";
                dgd_MarkSetting.Rows[i].Cells[3].Value = "0";
                dgd_MarkSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetDisplayUnitName(m_smCustomizeInfo.g_intMarkUnitDisplay, true);
                i++;
            }
        }

        private void AddPkgSettingGUI()
        {
            dgd_PkgSetting.Rows.Clear();

            int i = 0;
            dgd_PkgSetting.Rows.Add();
            dgd_PkgSetting.Rows[i].Cells[0].Value = "Package Width";
            dgd_PkgSetting.Rows[i].Cells[1].Value = "0";
            dgd_PkgSetting.Rows[i].Cells[2].Value = "0";
            dgd_PkgSetting.Rows[i].Cells[3].Value = "NA";
            dgd_PkgSetting.Rows[i].Cells[4].Value = "NA";
            dgd_PkgSetting.Rows[i].Cells[5].Value = "mm";
            i++;

            dgd_PkgSetting.Rows.Add();
            dgd_PkgSetting.Rows[i].Cells[0].Value = "Package Length";
            dgd_PkgSetting.Rows[i].Cells[1].Value = "0";
            dgd_PkgSetting.Rows[i].Cells[2].Value = "0";
            dgd_PkgSetting.Rows[i].Cells[3].Value = "NA";
            dgd_PkgSetting.Rows[i].Cells[4].Value = "NA";
            dgd_PkgSetting.Rows[i].Cells[5].Value = "mm";
            i++;

            if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x2000) > 0)
            {
                dgd_PkgSetting.Rows.Add();
                dgd_PkgSetting.Rows[i].Cells[0].Value = "Package Angle";
                dgd_PkgSetting.Rows[i].Cells[1].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[2].Value = "0";
                dgd_PkgSetting.Rows[i].Cells[3].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[4].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[5].Value = "deg";
                i++;
            }

            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnUseDetailDefectCriteria)
            {
                dgd_PkgSetting.Rows.Add();
                dgd_PkgSetting.Rows[i].Cells[0].Value = "Scratches Area";
                dgd_PkgSetting.Rows[i].Cells[1].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[2].Value = "0";
                dgd_PkgSetting.Rows[i].Cells[3].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[4].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[5].Value = "mm^2";
                dgd_PkgSetting.Rows[i].Cells[1].ReadOnly = true;
                dgd_PkgSetting.Rows[i].Cells[1].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.DarkGray;
                i++;

                dgd_PkgSetting.Rows.Add();
                dgd_PkgSetting.Rows[i].Cells[0].Value = "Scratches Length";
                dgd_PkgSetting.Rows[i].Cells[1].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[2].Value = "0";
                dgd_PkgSetting.Rows[i].Cells[3].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[4].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[5].Value = "mm";
                dgd_PkgSetting.Rows[i].Cells[1].ReadOnly = true;
                dgd_PkgSetting.Rows[i].Cells[1].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.DarkGray;
                i++;

                dgd_PkgSetting.Rows.Add();
                dgd_PkgSetting.Rows[i].Cells[0].Value = "Contamination Area";
                dgd_PkgSetting.Rows[i].Cells[1].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[2].Value = "0";
                dgd_PkgSetting.Rows[i].Cells[3].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[4].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[5].Value = "mm^2";
                dgd_PkgSetting.Rows[i].Cells[1].ReadOnly = true;
                dgd_PkgSetting.Rows[i].Cells[1].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.DarkGray;
                i++;

                dgd_PkgSetting.Rows.Add();
                dgd_PkgSetting.Rows[i].Cells[0].Value = "Contamination Length";
                dgd_PkgSetting.Rows[i].Cells[1].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[2].Value = "0";
                dgd_PkgSetting.Rows[i].Cells[3].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[4].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[5].Value = "mm";
                dgd_PkgSetting.Rows[i].Cells[1].ReadOnly = true;
                dgd_PkgSetting.Rows[i].Cells[1].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.DarkGray;
                i++;

                dgd_PkgSetting.Rows.Add();
                dgd_PkgSetting.Rows[i].Cells[0].Value = "Chipped Off Area";
                dgd_PkgSetting.Rows[i].Cells[1].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[2].Value = "0";
                dgd_PkgSetting.Rows[i].Cells[3].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[4].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[5].Value = "mm^2";
                dgd_PkgSetting.Rows[i].Cells[1].ReadOnly = true;
                dgd_PkgSetting.Rows[i].Cells[1].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.DarkGray;
                i++;

                // 18-04-2019 ZJYEOH : Temporary hide as now only check for Area
                //dgd_PkgSetting.Rows.Add();
                //dgd_PkgSetting.Rows[i].Cells[0].Value = "Chipped Off Length";
                //dgd_PkgSetting.Rows[i].Cells[1].Value = "NA";
                //dgd_PkgSetting.Rows[i].Cells[2].Value = "0";
                //dgd_PkgSetting.Rows[i].Cells[3].Value = "NA";
                //dgd_PkgSetting.Rows[i].Cells[4].Value = "NA";
                //dgd_PkgSetting.Rows[i].Cells[5].Value = "mm";
                //dgd_PkgSetting.Rows[i].Cells[1].ReadOnly = true;
                //dgd_PkgSetting.Rows[i].Cells[1].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                //dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                //dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.DarkGray;
                //i++;

                dgd_PkgSetting.Rows.Add();
                dgd_PkgSetting.Rows[i].Cells[0].Value = "Cracked Area";
                dgd_PkgSetting.Rows[i].Cells[1].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[2].Value = "0";
                dgd_PkgSetting.Rows[i].Cells[3].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[4].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[5].Value = "mm^2";
                dgd_PkgSetting.Rows[i].Cells[1].ReadOnly = true;
                dgd_PkgSetting.Rows[i].Cells[1].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.DarkGray;
                i++;

                dgd_PkgSetting.Rows.Add();
                dgd_PkgSetting.Rows[i].Cells[0].Value = "Cracked Length";
                dgd_PkgSetting.Rows[i].Cells[1].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[2].Value = "0";
                dgd_PkgSetting.Rows[i].Cells[3].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[4].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[5].Value = "mm";
                dgd_PkgSetting.Rows[i].Cells[1].ReadOnly = true;
                dgd_PkgSetting.Rows[i].Cells[1].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.DarkGray;
                i++;

                dgd_PkgSetting.Rows.Add();
                dgd_PkgSetting.Rows[i].Cells[0].Value = "Mold Flash Area";
                dgd_PkgSetting.Rows[i].Cells[1].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[2].Value = "0";
                dgd_PkgSetting.Rows[i].Cells[3].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[4].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[5].Value = "mm^2";
                dgd_PkgSetting.Rows[i].Cells[1].ReadOnly = true;
                dgd_PkgSetting.Rows[i].Cells[1].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.DarkGray;
                i++;

                // 17-04-2019 ZJYEOH : Temporary hide as now only check for Area
                //dgd_PkgSetting.Rows.Add();
                //dgd_PkgSetting.Rows[i].Cells[0].Value = "Mold Flash Length";
                //dgd_PkgSetting.Rows[i].Cells[1].Value = "NA";
                //dgd_PkgSetting.Rows[i].Cells[2].Value = "0";
                //dgd_PkgSetting.Rows[i].Cells[3].Value = "NA";
                //dgd_PkgSetting.Rows[i].Cells[4].Value = "NA";
                //dgd_PkgSetting.Rows[i].Cells[5].Value = "mm";
                //dgd_PkgSetting.Rows[i].Cells[1].ReadOnly = true;
                //dgd_PkgSetting.Rows[i].Cells[1].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                //dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                //dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.DarkGray;
                //i++;

                dgd_PkgSetting.Rows.Add();
                dgd_PkgSetting.Rows[i].Cells[0].Value = "Void Area";
                dgd_PkgSetting.Rows[i].Cells[1].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[2].Value = "0";
                dgd_PkgSetting.Rows[i].Cells[3].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[4].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[5].Value = "mm^2";
                dgd_PkgSetting.Rows[i].Cells[1].ReadOnly = true;
                dgd_PkgSetting.Rows[i].Cells[1].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.DarkGray;
                i++;

                dgd_PkgSetting.Rows.Add();
                dgd_PkgSetting.Rows[i].Cells[0].Value = "Void Length";
                dgd_PkgSetting.Rows[i].Cells[1].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[2].Value = "0";
                dgd_PkgSetting.Rows[i].Cells[3].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[4].Value = "NA";
                dgd_PkgSetting.Rows[i].Cells[5].Value = "mm";
                dgd_PkgSetting.Rows[i].Cells[1].ReadOnly = true;
                dgd_PkgSetting.Rows[i].Cells[1].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[3].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                dgd_PkgSetting.Rows[i].Cells[4].Style.BackColor = dgd_PkgSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.DarkGray;
                i++;
            }

            //condition for crack
            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
                group_DarkField2DefectSetting.Visible = true;

            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting)
                group_DarkField3DefectSetting.Visible = true;

            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
                group_DarkField4DefectSetting.Visible = true;

            //condition for crack
            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                group_CrackDefectSetting.Visible = true;

            //condition for void
            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
                group_ChippedOffDefectSetting.Visible = true;

            //condition for mold flash
            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                group_MoldFlashDefectSetting.Visible = true;
        }

        private void LoadOrientSettings(int intTemplateNo)
        {
            if (m_smVisionInfo.g_arrOrients.Count > 0)
            {
                if (m_smVisionInfo.g_arrOrients[0].Count > 0)
                {
                    if (Convert.ToInt32(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_fMinScore * 100) != 0)
                        trackBar_OrientTolerance.Value = Convert.ToInt32(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_fMinScore * 100);

                    txt_OrientTolerance.Text = trackBar_OrientTolerance.Value.ToString();
                    txt_OrientAngle.Text = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_fAngleTolerance.ToString("f4");
                    txt_OrientX.Text = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_fXTolerance.ToString("f4");
                    txt_OrientY.Text = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_fYTolerance.ToString("f4");
                }
            }
        }

        private void LoadPin1Settings(int intTemplateNo)
        {
            if (m_smVisionInfo.g_arrPin1.Count > 0)
            {
                if (Convert.ToInt32(m_smVisionInfo.g_arrPin1[0].GetMinScoreSetting(m_smVisionInfo.g_intSelectedTemplate) * 100) != 0)
                    trackBar_Pin1Tolerance.Value = Convert.ToInt32(m_smVisionInfo.g_arrPin1[0].GetMinScoreSetting(m_smVisionInfo.g_intSelectedTemplate) * 100);

                txt_Pin1Tolerance.Text = trackBar_Pin1Tolerance.Value.ToString();
            }
        }

        private void ViewOrHideResultColumn(bool blnWantView)
        {
            int intFailMask = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetFailOptionMask(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
            
            if (m_smVisionInfo.g_intUnitsOnImage > 1)
            {
                dgd_Setting.Columns[2].Visible = blnWantView;
                if ((intFailMask & 0x01) > 0)   // 0x01=Excess Mark
                    dgd_Setting.Columns[5].Visible = blnWantView;
                if ((intFailMask & 0x10) > 0)   // 0x01=Miss Mark
                    dgd_Setting.Columns[8].Visible = blnWantView;
                if (m_smVisionInfo.g_blnWantCheckMarkAverageGrayValue && ((intFailMask & 0x200) > 0))
                    dgd_Setting.Columns[12].Visible = blnWantView;
                dgd_OCRSettings.Columns[2].Visible = blnWantView;
                dgd_MarkSetting.Columns[2].Visible = blnWantView;
                dgd_PkgSetting.Columns[3].Visible = blnWantView;
            }

            dgd_Setting.Columns[3].Visible = blnWantView;
            if ((intFailMask & 0x01) > 0)   // 0x01=Excess Mark
                dgd_Setting.Columns[6].Visible = blnWantView;
            if ((intFailMask & 0x10) > 0)   // 0x01=Miss Mark
                dgd_Setting.Columns[9].Visible = blnWantView;
            if (m_smVisionInfo.g_blnWantCheckMarkAverageGrayValue && ((intFailMask & 0x200) > 0))
                dgd_Setting.Columns[13].Visible = blnWantView;
            dgd_OCRSettings.Columns[3].Visible = blnWantView;
            dgd_MarkSetting.Columns[3].Visible = blnWantView;
            dgd_PkgSetting.Columns[4].Visible = blnWantView;

            groupBox_OrientScore.Visible = blnWantView;
            groupBox_Pin1Score.Visible = blnWantView;

            lbl_OrientAngle.Visible = blnWantView;
            lbl_OrientX.Visible = blnWantView;
            lbl_OrientY.Visible = blnWantView;
            srmLabel8.Visible = blnWantView;
            srmLabel10.Visible = blnWantView;
            srmLabel12.Visible = blnWantView;
            srmLabel15.Visible = blnWantView;
            srmLabel19.Visible = blnWantView;
            srmLabel21.Visible = blnWantView;
            srmLabel22.Visible = blnWantView;
            srmLabel25.Visible = blnWantView;
            lbl_EmptyScore.Visible = blnWantView;
            lbl_EmptyWhiteArea.Visible = blnWantView;
            srmLabel17.Visible = blnWantView;
            srmLabel1.Visible = blnWantView;
            srmLabel6.Visible = blnWantView;
            if (blnWantView)
            {
                group_SitOffsetWidthSetting.Size = new Size(254, 74);
                group_SitOffsetHeightSetting.Size = new Size(254, 74);
            }
            else
            {
                group_SitOffsetWidthSetting.Size = new Size(170, 74);
                group_SitOffsetHeightSetting.Size = new Size(170, 74);
            }
            lbl_SittingWidth.Visible = blnWantView;
            lbl_SittingHeight.Visible = blnWantView;
        }

        private void SaveOrientSettings(string strFolderPath)
        {
            
            STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
            XmlParser objFile = new XmlParser(strFolderPath + "Template\\Template.xml");

            if (m_smVisionInfo.g_arrOrients.Count > 0)
            {
                int intTemplateCount = m_smVisionInfo.g_arrOrients[0].Count;
                for (int i = 0; i < intTemplateCount; i++)
                {
                    objFile.WriteSectionElement("Template" + i);
                    objFile.WriteElement1Value("MinScore", m_smVisionInfo.g_arrOrients[0][i].ref_fMinScore);
                    objFile.WriteElement1Value("MaxAngle", m_smVisionInfo.g_arrOrients[0][i].ref_fAngleTolerance);
                    objFile.WriteElement1Value("MaxX", m_smVisionInfo.g_arrOrients[0][i].ref_fXTolerance);
                    objFile.WriteElement1Value("MaxY", m_smVisionInfo.g_arrOrients[0][i].ref_fYTolerance);
                }
            }
            objFile.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Orient Template", m_smProductionInfo.g_strLotID);
            if (m_smVisionInfo.g_arrOrients.Count > 0)
            {
                if (m_smVisionInfo.g_arrOrients[0].Count > 0)
                {
                    STDeviceEdit.CopySettingFile(strFolderPath, "Settings.xml");
                    m_smVisionInfo.g_arrOrients[0][0].SaveOrient(strFolderPath + "Settings.xml", false, "General", true);
                    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Orient Tolerance Setting", m_smProductionInfo.g_strLotID);
                }
            }

            if (m_smVisionInfo.g_arrPin1 != null)
            {
                // Load Pin 1
                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                {
                    STDeviceEdit.CopySettingFile(strFolderPath + "Template\\", "Pin1Template.xml");
                    m_smVisionInfo.g_arrPin1[u].SavePin1Setting(strFolderPath + "Template\\");
                    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Pin1 Tolerance Setting", m_smProductionInfo.g_strLotID);
                }
            }
            
        }

        private void ReadOrientSettings(string strFolderPath)
        {
            XmlParser objFileHandle = new XmlParser(strFolderPath + "Template\\Template.xml");
            int intParentCount = objFileHandle.GetFirstSectionCount();

            for (int j = 0; j < m_smVisionInfo.g_intUnitsOnImage; j++)
            {
                for (int i = 0; i < intParentCount; i++)
                {
                    if (j < m_smVisionInfo.g_arrOrients.Count && i < m_smVisionInfo.g_arrOrients[j].Count)
                    {
                        objFileHandle.GetFirstSection("Template" + i);
                        m_smVisionInfo.g_arrOrients[j][i].ref_fMinScore = objFileHandle.GetValueAsFloat("MinScore", 0.7f);
                        m_smVisionInfo.g_arrOrients[j][i].ref_fAngleTolerance = objFileHandle.GetValueAsFloat("MaxAngle", 0);
                        m_smVisionInfo.g_arrOrients[j][i].ref_fXTolerance = objFileHandle.GetValueAsFloat("MaxX", 0);
                        m_smVisionInfo.g_arrOrients[j][i].ref_fYTolerance = objFileHandle.GetValueAsFloat("MaxY", 0);
                    }
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrOrients.Count; i++)
            {
                for (int j = 0; j < m_smVisionInfo.g_arrOrients[i].Count; j++)
                {
                    m_smVisionInfo.g_arrOrients[i][j].LoadOrient(strFolderPath + "Settings.xml", "General");
                }
            }
        }

        private void LoadMarkSettings(string strFolderPath)
        {
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].LoadTemplate(strFolderPath, m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkDontCareROIs.Count, m_smVisionInfo.g_objWhiteImage);
                m_smVisionInfo.g_arrMarks[u].LoadTemplateOCR(strFolderPath);
            }
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
                
                STDeviceEdit.CopySettingFile(strFolderPath, "\\Settings.xml");
                
                if (u == 0)
                    m_smVisionInfo.g_arrPackage[u].SavePackage(strFolderPath + "\\Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                else
                    m_smVisionInfo.g_arrPackage[u].SavePackage(strFolderPath + "\\Settings2.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);

                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Package Tolerance Setting", m_smProductionInfo.g_strLotID);
                

                m_smVisionInfo.g_arrPackage[u].SetBlobSettings(m_smVisionInfo.g_arrPackage[u].ref_intPkgViewThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intPkgViewMinArea,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewHighThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewLowThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewMinArea);
            }
        }


        private void btn_Save_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            string PositionstrFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Positioning\\";
            string PocketPositionstrFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\PocketPosition\\";
            if ((m_intVisionType & 0x01) > 0)
            {
                SaveOrientSettings(strFolderPath + "Orient\\");
            }

            if (m_smVisionInfo.g_blnWantCheckUnitSitProper)
            {
                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                {
                    float fMinValue;
                    float fMaxValue = 0;

                    float.TryParse(Convert.ToString(txt_MinSittingHeight.Text), out fMinValue);
                    m_smVisionInfo.g_arrPackage[u].SetSittingHeightMin(fMinValue, m_smCustomizeInfo.g_intUnitDisplay);

                    float.TryParse(Convert.ToString(txt_MaxSittingHeight.Text), out fMaxValue);
                    m_smVisionInfo.g_arrPackage[u].SetSittingHeightMax(fMaxValue, m_smCustomizeInfo.g_intUnitDisplay);

                    float.TryParse(Convert.ToString(txt_MinSittingWidth.Text), out fMinValue);
                    m_smVisionInfo.g_arrPackage[u].SetSittingWidthMin(fMinValue, m_smCustomizeInfo.g_intUnitDisplay);

                    float.TryParse(Convert.ToString(txt_MaxSittingWidth.Text), out fMaxValue);
                    m_smVisionInfo.g_arrPackage[u].SetSittingWidthMax(fMaxValue, m_smCustomizeInfo.g_intUnitDisplay);

                    if (u == 0)
                        m_smVisionInfo.g_arrPackage[u].SavePackage(strFolderPath + "\\Package\\Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                    else
                        m_smVisionInfo.g_arrPackage[u].SavePackage(strFolderPath + "\\Package\\Settings2.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                }
            }

            if (m_smVisionInfo.g_blnWantCheckEmpty)
            {
                if (txt_MinWhiteArea.Text == "")
                {
                    SRMMessageBox.Show("Minimum White Area cannot be empty!");
                    return;
                }
                if (txt_EmptyScore.Text == "")
                {
                    SRMMessageBox.Show("Minimum Empty Score cannot be empty!");
                    return;
                }
                m_smVisionInfo.g_objPositioning.ref_intMinEmptyScore = Convert.ToInt32(txt_EmptyScore.Text);
                m_smVisionInfo.g_objPositioning.ref_intEmptyWhiteArea = Convert.ToInt32(txt_MinWhiteArea.Text);
                m_smVisionInfo.g_objPositioning.SavePosition(PositionstrFolderPath + "Settings.xml",false, "General",true);
                m_smVisionInfo.g_objPositioning.LoadPosition(PositionstrFolderPath + "Settings.xml", "General");
            }

            if (m_smVisionInfo.g_blnWantCheckPocketPosition)
            {
                if (txt_PocketPositionTolerance.Text == "")
                {
                    SRMMessageBox.Show("Pocket Position Tolerance cannot be empty!");
                    return;
                }
                m_smVisionInfo.g_objPocketPosition.SavePocketPosition(PocketPositionstrFolderPath + "Settings.xml", false, "Settings", true);
            }

            if (((m_intVisionType & 0x02) > 0) || ((m_intVisionType & 0x04) > 0))
            {
                // Save Mark Settings
                
                STDeviceEdit.CopySettingFile(strFolderPath + "Mark\\Template\\", "Template.xml");
                m_smVisionInfo.g_arrMarks[0].SaveTemplate(strFolderPath + "Mark\\Template\\", false);
                m_smVisionInfo.g_arrMarks[0].SaveTemplateOCR(strFolderPath + "Mark\\Template\\", false);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Mark Tolerance Setting", m_smProductionInfo.g_strLotID);
                

                // 2020 08 30 - CCENG: Need Save Advance Mark Settings bcos NoMarkMaximumBlob is saved in Advance Setting initially
                string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Settings.xml";
                XmlParser objFileHandle = new XmlParser(strPath);
                objFileHandle.WriteSectionElement("Advanced");
                objFileHandle.WriteElement1Value("NoMarkMaximumBlob", (int)Math.Round(m_smVisionInfo.g_arrMarks[0].ref_fNoMarkMaximumBlobArea));
                objFileHandle.WriteEndElement();
            }

            if ((m_intVisionType & 0x08) > 0)
            {
                SavePackageSettings(strFolderPath + "Package\\");
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

            if ((m_intVisionType & 0x01) > 0)
                ReadOrientSettings(strFolderPath + "Orient\\");

            if (((m_intVisionType & 0x02) > 0) || ((m_intVisionType & 0x04) > 0))
            {
                LoadMarkSettings(strFolderPath + "Mark\\Template\\");

                // 2020 08 30 - CCENG: Load Advance Setting Mark bcos NoMarkMaximumBlob is saved in Advance Setting.
                string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Settings.xml";
                XmlParser objFileHandle = new XmlParser(strPath);
                objFileHandle.GetFirstSection("Advanced");
                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                {
                    m_smVisionInfo.g_arrMarks[u].ref_fNoMarkMaximumBlobArea = m_smVisionInfo.g_arrMarks[u].GetNoMarkMaximumBlobArea_InMM(objFileHandle.GetValueAsFloat("NoMarkMaximumBlob", 200));
                }
            }

            if (((m_intVisionType & 0x08) > 0))
                LoadPackageSetting(strFolderPath + "Package\\");

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPin1[u].LoadTemplate(strFolderPath + "Orient\\Template\\");
            }

            if (m_smVisionInfo.g_blnWantCheckPocketPosition)
            {
                m_smVisionInfo.g_objPocketPosition.LoadPocketPosition(strFolderPath + "PocketPosition\\Settings.xml", "Settings");
            }

            this.Close();
            this.Dispose();
        }

        private void tab_VisionControl_Selected(object sender, TabControlEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (e.TabPage == tp_Orient)
            {
                m_intSettingType = 0;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_Mark)
            {
                m_intSettingType = 1;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_Mark2)
            {
                m_intSettingType = 2;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_Pin1)
            {
                m_intSettingType = 3;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_Package)
            {
                m_intSettingType = 4;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_SitOffset)
            {
                m_intSettingType = 5;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_Empty)
            {
                m_intSettingType = 6;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_PocketPosition)
            {
                m_intSettingType = 7;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_OCR)
            {
                m_intSettingType = 8;
                UpdateTabPage();
            }
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("CurrentSettingType_MoTolerance" + "_" + m_smVisionInfo.g_strVisionDisplayName, m_intSettingType);
        }

        private void cbo_TemplateNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = cbo_TemplateNo.SelectedIndex;
            }

            UpdateSelectedTemplateChange();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_SetAllTemplates_Click(object sender, EventArgs e)
        {
            m_blnWantSet1ToAll = chk_SetAllTemplates.Checked;
        }

        private void txt_OrientTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnEnterTextBox)
                return;

            trackBar_OrientTolerance.Value = Convert.ToInt32(txt_OrientTolerance.Text);

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (m_blnWantSet1ToAll || m_smVisionInfo.g_blnWantSkipMark || (m_intVisionType == 0x01))
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrOrients[u].Count; i++)
                        m_smVisionInfo.g_arrOrients[u][i].ref_fMinScore = (trackBar_OrientTolerance.Value / 100.0f);
                }
                else
                    m_smVisionInfo.g_arrOrients[u][m_smVisionInfo.g_intSelectedTemplate].ref_fMinScore = (trackBar_OrientTolerance.Value / 100.0f);
            }
        }

        private void trackBar_OrientTolerance_Scroll(object sender, EventArgs e)
        {

            txt_OrientTolerance.Text = trackBar_OrientTolerance.Value.ToString();

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (m_blnWantSet1ToAll || m_smVisionInfo.g_blnWantSkipMark || (m_intVisionType == 0x01))
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrOrients[u].Count; i++)
                        m_smVisionInfo.g_arrOrients[u][i].ref_fMinScore = (trackBar_OrientTolerance.Value / 100.0f);
                }
                else
                    m_smVisionInfo.g_arrOrients[u][m_smVisionInfo.g_intSelectedTemplate].ref_fMinScore = (trackBar_OrientTolerance.Value / 100.0f);
            }
        }

        private void dgd_MarkSetting_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnChangeScoreSetting)
                return;

            if (e.RowIndex < 0)
                return;
        }

        private void dgd_MarkSetting_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            /*
             * Please take note dgd_MarkSetting_CellValueChanged event will be triggered during loading and when value change in cell.
             * dgd_MarkSetting_CellEndEdit will not be triggred although value change. It will only been triggered after user finish change cell value manually (even same value), 
             */

            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                int i = e.RowIndex;
                string strRowName = ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString();
                int intValue;
                int intValuePrev;
                float fValue;
                float fValuePrev;

                switch (strRowName)
                {
                    case "Max Excess Area":
                        intValuePrev = m_smVisionInfo.g_arrMarks[u].GetExcessMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                        if (!int.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out intValue))
                        {
                            dgd_MarkSetting.Rows[i].Cells[1].Value = intValuePrev.ToString();
                            SRMMessageBox.Show("Please enter numeric value without decimal point!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrMarks[u].SetExcessMinArea(intValue, m_blnWantSet1ToAll);
                        }
                        break;
                    case "Max Total Excess Area":
                        if (m_smCustomizeInfo.g_intMarkUnitDisplay == 0)
                        {
                            fValuePrev = m_smVisionInfo.g_arrMarks[u].GetGroupExcessMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                            if (!int.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out intValue))
                            {
                                dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString(GetDecimalFormat());
                                SRMMessageBox.Show("Please enter numeric value without decimal point!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrMarks[u].SetGroupExcessMinArea(intValue, m_blnWantSet1ToAll);
                            }
                        }
                        else
                        {
                            fValuePrev = m_smVisionInfo.g_arrMarks[u].GetGroupExcessMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                            if (!float.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out fValue))
                            {
                                dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString(GetDecimalFormat());
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrMarks[u].SetGroupExcessMinArea(fValue, m_blnWantSet1ToAll);
                            }
                        }
                        break;
                    case "Max Extra Area":
                        if (m_smCustomizeInfo.g_intMarkUnitDisplay == 0)
                        {
                            fValuePrev = m_smVisionInfo.g_arrMarks[u].GetExtraMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                            if (!int.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out intValue))
                            {
                                dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString(GetDecimalFormat());
                                SRMMessageBox.Show("Please enter numeric value without decimal point!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrMarks[u].SetExtraMinArea(intValue, m_blnWantSet1ToAll);
                            }
                        }
                        else
                        {
                            fValuePrev = m_smVisionInfo.g_arrMarks[u].GetExtraMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                            if (!float.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out fValue))
                            {
                                dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString(GetDecimalFormat());
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrMarks[u].SetExtraMinArea(fValue, m_blnWantSet1ToAll);
                            }
                        }
                        break;
                    case "Max Total Extra Area":
                        if (m_smCustomizeInfo.g_intMarkUnitDisplay == 0)
                        {
                            fValuePrev = m_smVisionInfo.g_arrMarks[u].GetGroupExtraMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                            if (!int.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out intValue))
                            {
                                dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString(GetDecimalFormat());
                                SRMMessageBox.Show("Please enter numeric value without decimal point!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrMarks[u].SetGroupExtraMinArea(intValue, m_blnWantSet1ToAll);
                            }
                        }
                        else
                        {
                            fValuePrev = m_smVisionInfo.g_arrMarks[u].GetGroupExtraMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                            if (!float.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out fValue))
                            {
                                dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString(GetDecimalFormat());
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrMarks[u].SetGroupExtraMinArea(fValue, m_blnWantSet1ToAll);
                            }
                        }
                        break;
                    case "Max Miss Area":
                        intValuePrev = m_smVisionInfo.g_arrMarks[u].GetMissingMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                        if (!int.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out intValue))
                        {
                            dgd_MarkSetting.Rows[i].Cells[1].Value = intValuePrev.ToString();
                            SRMMessageBox.Show("Please enter numeric value without decimal point!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrMarks[u].SetMissingMinArea(intValue, m_blnWantSet1ToAll);
                        }
                        break;
                    case "Char Shift Tolerance":
                        intValuePrev = m_smVisionInfo.g_arrMarks[u].GetCharShiftXY(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                        if (!int.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out intValue))
                        {
                            dgd_MarkSetting.Rows[i].Cells[1].Value = intValuePrev.ToString();
                            SRMMessageBox.Show("Please enter numeric value without decimal point!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrMarks[u].SetCharShiftXY(intValue, m_blnWantSet1ToAll);
                        }
                        break;
                    case "Text Shift Tole X":
                        break;
                    case "Text Shift Tole Y":
                        break;
                    case "Text Min Score":
                        intValuePrev = m_smVisionInfo.g_arrMarks[u].GetTextMinScore(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                        if (!int.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out intValue))
                        {
                            dgd_MarkSetting.Rows[i].Cells[1].Value = intValuePrev.ToString();
                            SRMMessageBox.Show("Please enter numeric value without decimal point!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrMarks[u].SetTextMinScore(intValue, m_blnWantSet1ToAll);
                        }
                        break;
                    case "Min Top Edge Distance":
                        if (m_smCustomizeInfo.g_intMarkUnitDisplay == 0)
                        {
                            fValuePrev = m_smVisionInfo.g_arrMarks[u].GetUnCheckAreaTop(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                            if (!int.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out intValue))
                            {
                                dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString(GetDecimalFormat());
                                SRMMessageBox.Show("Please enter numeric value without decimal point!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrMarks[u].SetUnCheckAreaTop(intValue, m_blnWantSet1ToAll);
                            }
                        }
                        else
                        {
                            fValuePrev = m_smVisionInfo.g_arrMarks[u].GetUnCheckAreaTop(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                            if (!float.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out fValue))
                            {
                                dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString(GetDecimalFormat());
                                SRMMessageBox.Show("Please enter numeric value without decimal point!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrMarks[u].SetUnCheckAreaTop(fValue, m_blnWantSet1ToAll);
                            }
                        }
                        break;
                    case "Min Btm Edge Distance":
                        if (m_smCustomizeInfo.g_intMarkUnitDisplay == 0)
                        {
                            fValuePrev = m_smVisionInfo.g_arrMarks[u].GetUnCheckAreaBottom(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                            if (!int.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out intValue))
                            {
                                dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString(GetDecimalFormat());
                                SRMMessageBox.Show("Please enter numeric value without decimal point!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrMarks[u].SetUnCheckAreaBottom(intValue, m_blnWantSet1ToAll);
                            }
                        }
                        else
                        {
                            fValuePrev = m_smVisionInfo.g_arrMarks[u].GetUnCheckAreaBottom(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                            if (!float.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out fValue))
                            {
                                dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString(GetDecimalFormat());
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrMarks[u].SetUnCheckAreaBottom(fValue, m_blnWantSet1ToAll);
                            }
                        }
                        break;
                    case "Min Left Edge Distance":
                        if (m_smCustomizeInfo.g_intMarkUnitDisplay == 0)
                        {
                            fValuePrev = m_smVisionInfo.g_arrMarks[u].GetUnCheckAreaLeft(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                            if (!int.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out intValue))
                            {
                                dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString(GetDecimalFormat());
                                SRMMessageBox.Show("Please enter numeric value without decimal point!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrMarks[u].SetUnCheckAreaLeft(intValue, m_blnWantSet1ToAll);
                            }
                        }
                        else
                        {
                            fValuePrev = m_smVisionInfo.g_arrMarks[u].GetUnCheckAreaLeft(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                            if (!float.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out fValue))
                            {
                                dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString(GetDecimalFormat());
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrMarks[u].SetUnCheckAreaLeft(fValue, m_blnWantSet1ToAll);
                            }
                        }
                        break;
                    case "Min Right Edge Distance":
                        if (m_smCustomizeInfo.g_intMarkUnitDisplay == 0)
                        {
                            fValuePrev = m_smVisionInfo.g_arrMarks[u].GetUnCheckAreaRight(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                            if (!int.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out intValue))
                            {
                                dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString(GetDecimalFormat());
                                SRMMessageBox.Show("Please enter numeric value without decimal point!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrMarks[u].SetUnCheckAreaRight(intValue, m_blnWantSet1ToAll);
                            }
                        }
                        else
                        {
                            fValuePrev = m_smVisionInfo.g_arrMarks[u].GetUnCheckAreaRight(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                            if (!float.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out fValue))
                            {
                                dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString(GetDecimalFormat());
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrMarks[u].SetUnCheckAreaRight(fValue, m_blnWantSet1ToAll);
                            }
                        }
                        break;
                    case "Broken Size":
                        intValuePrev = m_smVisionInfo.g_arrMarks[u].GetBrokenSize(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                        if (!int.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out intValue))
                        {
                            dgd_MarkSetting.Rows[i].Cells[1].Value = intValuePrev.ToString();
                            SRMMessageBox.Show("Please enter numeric value without decimal point!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrMarks[u].SetBrokenSize(intValue, m_blnWantSet1ToAll);
                        }
                        break;
                    case "Mark Angle Tol.":
                        fValuePrev = m_smVisionInfo.g_arrMarks[u].GetMarkAngleTolerance(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                        if (!float.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out fValue))
                        {
                            dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString();
                            SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrMarks[u].SetMarkAngleTolerance(fValue, m_blnWantSet1ToAll);
                        }
                        break;
                    case "Min No Mark Area":
                        if (m_smCustomizeInfo.g_intMarkUnitDisplay == 0)
                        {
                            fValuePrev = m_smVisionInfo.g_arrMarks[u].GetNoMarkMaximumBlobArea();
                            if (!int.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out intValue))
                            {
                                dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString(GetDecimalFormat());
                                SRMMessageBox.Show("Please enter numeric value without decimal point!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrMarks[u].SetNoMarkMaximumBlobArea(intValue);
                            }
                        }
                        else
                        {
                            fValuePrev = m_smVisionInfo.g_arrMarks[u].GetNoMarkMaximumBlobArea();
                            if (!float.TryParse(Convert.ToString(dgd_MarkSetting.Rows[i].Cells[1].Value), out fValue))
                            {
                                dgd_MarkSetting.Rows[i].Cells[1].Value = fValuePrev.ToString(GetDecimalFormat());
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrMarks[u].SetNoMarkMaximumBlobArea(fValue);
                            }
                        }
                        break;
                    default:
                        SRMMessageBox.Show("Cannot find row name [" + strRowName + "].");
                        break;
                }
            }
        }

        private void btn_GlobalSetting_Click(object sender, EventArgs e)
        {
            //int intCurrentSetValue;
            //if (dgd_Setting.Rows.Count > 0)
            //    intCurrentSetValue = Convert.ToInt32(dgd_Setting.Rows[0].Cells[1].Value);
            //else
            //    intCurrentSetValue = 75;

            //SetCharsValueForm2 objSetCharValueForm = new SetCharsValueForm2(-1, intCurrentSetValue, true);
            //objSetCharValueForm.Location = new Point(769, 310);
            //if (objSetCharValueForm.ShowDialog() == DialogResult.OK)
            //{
            //    for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            //        for (int i = 0; i < dgd_Setting.Rows.Count; i++)
            //            m_smVisionInfo.g_arrMarks[u].SetCharSetting(i, objSetCharValueForm.ref_intSetValue, true, m_blnWantSet1ToAll);
            //}

            //UpdateOCVSettingTable();
        }

        private void dgd_Setting_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnChangeScoreSetting)
                return;

            if (e.RowIndex < 0)
                return;

            if (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.LightGray ||
               (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.LightGray))
                return;


            //Skip if col is result score
            if (e.ColumnIndex == 2 || e.ColumnIndex == 3 ||
                e.ColumnIndex == 5 || e.ColumnIndex == 6 ||
                e.ColumnIndex == 8 || e.ColumnIndex == 9 ||
                e.ColumnIndex == 10 ||
                e.ColumnIndex == 12 || e.ColumnIndex == 13)
                return;

            // Skip if cell value is ---
            if ((((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "---") ||
                (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "----") ||
                (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == ""))
                return;

            switch (e.ColumnIndex)
            {
                case 0:
                    {
                        bool blnEnableMark = m_smVisionInfo.g_arrMarks[0].GetEnableMarkSetting(e.RowIndex);

                        //int intCurrentSetValue = Convert.ToInt32(dgd_Setting.Rows[e.RowIndex].Cells[1].Value);
                        SetCharsEnableForm objSetCharsEnableForm = new SetCharsEnableForm(e.RowIndex + 1, blnEnableMark);
                        objSetCharsEnableForm.Location = new Point(769, 310);
                        if (objSetCharsEnableForm.ShowDialog() == DialogResult.OK)
                        {
                            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                            {
                                if (objSetCharsEnableForm.ref_blnSetAllRows)
                                {
                                    //2020-05-08 ZJYEOH : To solve other template with more mark cannot be set
                                    if (m_blnWantSet1ToAll)
                                        m_smVisionInfo.g_arrMarks[u].SetCharEnable(objSetCharsEnableForm.ref_blnEnableMark);
                                    else
                                    {
                                        for (int i = 0; i < dgd_Setting.Rows.Count; i++)
                                            m_smVisionInfo.g_arrMarks[u].SetCharEnable(i, objSetCharsEnableForm.ref_blnEnableMark);
                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrMarks[u].SetCharEnable(e.RowIndex, objSetCharsEnableForm.ref_blnEnableMark, m_blnWantSet1ToAll);


                            }
                        }

                        UpdateOCVSettingTable();
                    }
                    break;
                case 1:
                    {
                        int intCurrentSetValue = Convert.ToInt32(dgd_Setting.Rows[e.RowIndex].Cells[1].Value);
                        SetCharsValueForm2 objSetCharValueForm = new SetCharsValueForm2(e.RowIndex + 1, intCurrentSetValue);
                        objSetCharValueForm.Location = new Point(769, 310);
                        if (objSetCharValueForm.ShowDialog() == DialogResult.OK)
                        {
                            RegistryKey Key = Registry.LocalMachine.OpenSubKey("Software", true);
                            RegistryKey subKey = Key.OpenSubKey("SVG\\Visions\\", true);
                            RegistryKey subKey2 = Key.OpenSubKey("SVG\\Visions\\" + m_smVisionInfo.g_strVisionFolderName, true);
                            bool blnSecureOnOff = Convert.ToBoolean(subKey.GetValue("SecureOnOff-MarkScore", false));
                            int intMinScoreSetting = Convert.ToInt32(subKey2.GetValue("Secure-MinMarkScoreSetting", -1));
                            if (intMinScoreSetting == -1)   // If parameter no exist
                            {
                                intMinScoreSetting = 30;    // Default 30%
                                // if registry does not have this parameter, user cannot know what parameter name they have to set. In order to make it user friendly, we write it to system so that user can find the parameter, then they can modified the value. 
                                subKey2.SetValue("Secure-MinMarkScoreSetting", intMinScoreSetting.ToString());
                            }

                            // 2021 01 25 - CCENG: Customer request mark score cannot lower than minimum mark score advance setting.
                            if (blnSecureOnOff && objSetCharValueForm.ref_intSetValue < intMinScoreSetting)
                            {
                                SRMMessageBox.Show("Mark Setting cannot lower than Minimum Mark Score [" + intMinScoreSetting.ToString() + "%].");

                            }
                            else
                            {
                                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                                {
                                    if (objSetCharValueForm.ref_blnSetAllRows)
                                    {
                                        //2020-05-08 ZJYEOH : To solve other template with more mark cannot be set
                                        if (m_blnWantSet1ToAll)
                                            m_smVisionInfo.g_arrMarks[u].SetCharSetting(objSetCharValueForm.ref_intSetValue);
                                        else
                                        {
                                            for (int i = 0; i < dgd_Setting.Rows.Count; i++)
                                                m_smVisionInfo.g_arrMarks[u].SetCharSetting(i, objSetCharValueForm.ref_intSetValue);
                                        }
                                    }
                                    else
                                        m_smVisionInfo.g_arrMarks[u].SetCharSetting(e.RowIndex, objSetCharValueForm.ref_intSetValue, m_blnWantSet1ToAll);
                                }
                            }
                        }

                        UpdateOCVSettingTable();
                    }
                    break;
                case 4:
                    {
                        float fCurrentSetValue = (float)Convert.ToDouble(dgd_Setting.Rows[e.RowIndex].Cells[4].Value);
                        SetCharsExcessMissAreaValueForm objSetValueForm = new SetCharsExcessMissAreaValueForm(m_smCustomizeInfo, "Max Excess Area", e.RowIndex + 1, fCurrentSetValue);
                        objSetValueForm.Location = new Point(769, 310);
                        if (objSetValueForm.ShowDialog() == DialogResult.OK)
                        {
                            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                            {
                                if (objSetValueForm.ref_blnSetAllRows)
                                {
                                    //2020-05-08 ZJYEOH : To solve other template with more mark cannot be set
                                    if (m_blnWantSet1ToAll)
                                        m_smVisionInfo.g_arrMarks[u].SetCharMaxExcessAreaSetting(objSetValueForm.ref_fSetValue);
                                    else
                                    {
                                        for (int i = 0; i < dgd_Setting.Rows.Count; i++)
                                        {
                                            if (((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor == Color.LightGray)
                                                continue;
                                            m_smVisionInfo.g_arrMarks[u].SetCharMaxExcessAreaSetting(i, objSetValueForm.ref_fSetValue);
                                        }
                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrMarks[u].SetCharMaxExcessAreaSetting(e.RowIndex, objSetValueForm.ref_fSetValue, m_blnWantSet1ToAll);
                            }
                        }

                        UpdateOCVSettingTable();
                    }
                    break;
                case 7:
                    {
                        float fCurrentSetValue = (float)Convert.ToDouble(dgd_Setting.Rows[e.RowIndex].Cells[7].Value);
                        SetCharsExcessMissAreaValueForm objSetValueForm = new SetCharsExcessMissAreaValueForm(m_smCustomizeInfo, "Max Miss Area", e.RowIndex + 1, fCurrentSetValue);
                        objSetValueForm.Location = new Point(769, 310);
                        if (objSetValueForm.ShowDialog() == DialogResult.OK)
                        {
                            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                            {
                                if (objSetValueForm.ref_blnSetAllRows)
                                {
                                    //2020-05-08 ZJYEOH : To solve other template with more mark cannot be set
                                    if (m_blnWantSet1ToAll)
                                        m_smVisionInfo.g_arrMarks[u].SetCharMaxBrokenAreaSetting(objSetValueForm.ref_fSetValue);
                                    else
                                    {
                                        for (int i = 0; i < dgd_Setting.Rows.Count; i++)
                                        {
                                            if (((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor == Color.LightGray)
                                                continue;
                                            m_smVisionInfo.g_arrMarks[u].SetCharMaxBrokenAreaSetting(i, objSetValueForm.ref_fSetValue);
                                        }
                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrMarks[u].SetCharMaxBrokenAreaSetting(e.RowIndex, objSetValueForm.ref_fSetValue, m_blnWantSet1ToAll);
                            }
                        }

                        UpdateOCVSettingTable();
                    }
                    break;
                case 11:
                    {
                        int intCurrentSetValue = Convert.ToInt32(dgd_Setting.Rows[e.RowIndex].Cells[11].Value);
                        SetCharsValueForm2 objSetCharValueForm = new SetCharsValueForm2(e.RowIndex + 1, intCurrentSetValue);
                        objSetCharValueForm.Location = new Point(769, 310);
                        if (objSetCharValueForm.ShowDialog() == DialogResult.OK)
                        {
                            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                            {
                                if (objSetCharValueForm.ref_blnSetAllRows)
                                {
                                    //2020-05-08 ZJYEOH : To solve other template with more mark cannot be set
                                    if (m_blnWantSet1ToAll)
                                        m_smVisionInfo.g_arrMarks[u].SetMaxAGVPercent(objSetCharValueForm.ref_intSetValue);
                                    else
                                    {
                                        for (int i = 0; i < dgd_Setting.Rows.Count; i++)
                                            m_smVisionInfo.g_arrMarks[u].SetMaxAGVPercent(i, objSetCharValueForm.ref_intSetValue);
                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrMarks[u].SetMaxAGVPercent(e.RowIndex, objSetCharValueForm.ref_intSetValue, m_blnWantSet1ToAll);
                            }
                        }

                        UpdateOCVSettingTable();
                    }
                    break;
            }


        }

        private void txt_Pin1Tolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnEnterTextBox)
                return;

            trackBar_Pin1Tolerance.Value = Convert.ToInt32(txt_Pin1Tolerance.Text);

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (m_blnWantSet1ToAll || m_smVisionInfo.g_blnWantSkipMark || (m_intVisionType == 0x01))
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrPin1[u].ref_arrTemplateSetting.Count; i++)
                        m_smVisionInfo.g_arrPin1[u].SetMinScoreSetting(i, trackBar_Pin1Tolerance.Value / 100.0f);
                }
                else
                    m_smVisionInfo.g_arrPin1[u].SetMinScoreSetting(m_smVisionInfo.g_intSelectedTemplate, trackBar_Pin1Tolerance.Value / 100.0f);
            }
        }

        private void trackBar_Pin1Tolerance_Scroll(object sender, EventArgs e)
        {
            txt_Pin1Tolerance.Text = trackBar_Pin1Tolerance.Value.ToString();

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (m_blnWantSet1ToAll || m_smVisionInfo.g_blnWantSkipMark || (m_intVisionType == 0x01))
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrPin1[u].ref_arrTemplateSetting.Count; i++)
                        m_smVisionInfo.g_arrPin1[u].SetMinScoreSetting(i, trackBar_Pin1Tolerance.Value / 100.0f);
                }
                else
                    m_smVisionInfo.g_arrPin1[u].SetMinScoreSetting(m_smVisionInfo.g_intSelectedTemplate, trackBar_Pin1Tolerance.Value / 100.0f);
            }
        }

        private void txt_OrientTolerance_Enter(object sender, EventArgs e)
        {
            m_blnEnterTextBox = true;
        }

        private void txt_OrientTolerance_Leave(object sender, EventArgs e)
        {
            m_blnEnterTextBox = false;
        }

        private void txt_OrientAngle_Enter(object sender, EventArgs e)
        {
            m_blnEnterTextBox = true;
        }

        private void txt_OrientAngle_Leave(object sender, EventArgs e)
        {
            m_blnEnterTextBox = false;
        }
        private void txt_OrientX_Enter(object sender, EventArgs e)
        {
            m_blnEnterTextBox = true;
            m_smVisionInfo.g_blnViewOrientSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_OrientX_Leave(object sender, EventArgs e)
        {
            m_blnEnterTextBox = false;
            m_smVisionInfo.g_blnViewOrientSetting = false;
        }
        private void txt_OrientY_Enter(object sender, EventArgs e)
        {
            m_blnEnterTextBox = true;
            m_smVisionInfo.g_blnViewOrientSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_OrientY_Leave(object sender, EventArgs e)
        {
            m_blnEnterTextBox = false;
            m_smVisionInfo.g_blnViewOrientSetting = false;
        }

        private void txt_Pin1Tolerance_Enter(object sender, EventArgs e)
        {
            m_blnEnterTextBox = true;
        }

        private void txt_Pin1Tolerance_Leave(object sender, EventArgs e)
        {
            m_blnEnterTextBox = false;
        }

        private void chk_DisplayResult_Click(object sender, EventArgs e)
        {
            ViewOrHideResultColumn(chk_DisplayResult.Checked);

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("CurrentDisplayResult_MoTolerance", chk_DisplayResult.Checked);
        }

        private void MarkOrientToleranceSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Mask Bit 0x01:Orient, 0x02:Mark, 0x08:Pkg, 0x10:Lead, 0x20:Orient0Deg
            
            if (((m_intVisionType & 0x01) > 0) || ((m_intVisionType & 0x02) > 0))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark Tolerance Setting Form Closed", "Exit Mark Tolerance Form", "", "", m_smProductionInfo.g_strLotID);
            }
            if ((m_intVisionType & 0x08) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package Tolerance Setting Form Closed", "Exit Package Tolerance Form", "", "", m_smProductionInfo.g_strLotID);
            }
            

            m_smVisionInfo.g_blnPackageInspected = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPkgProcessImage = false;
            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnDrawMarkResult = false;
            m_smVisionInfo.g_blnDrawPin1Result = false;
            m_smVisionInfo.g_blnDrawMark2DCodeResult = false;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
            m_smVisionInfo.g_blnViewOrientObject = false;
            m_smVisionInfo.PR_MN_UpdateSettingInfo = true;
        }

        private void MarkOrientToleranceSettingForm_Load(object sender, EventArgs e)
        {
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            if (m_smVisionInfo.g_blnWantCheckEmpty)
            {

                string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Positioning\\";
                m_smVisionInfo.g_objPositioning.LoadPosition(strFolderPath + "Settings.xml", "General");
                m_smVisionInfo.g_objPositioning.LoadEmptyThreshold(strFolderPath + "Settings.xml", "General");
                if (!tab_VisionControl.TabPages.Contains(tp_Empty))
                    tab_VisionControl.TabPages.Add(tp_Empty);
                txt_MinWhiteArea.Text = m_smVisionInfo.g_objPositioning.ref_intEmptyWhiteArea.ToString();
                txt_EmptyScore.Text = m_smVisionInfo.g_objPositioning.ref_intMinEmptyScore.ToString();
                trackBar_EmptyTolerance.Value = Math.Max(Convert.ToInt32(txt_EmptyScore.Text), 1);
                lbl_EmptyScore.Text = m_smVisionInfo.g_objPositioning.ref_fEmptyObjectScore.ToString();
                if (m_smVisionInfo.g_arrPositioningROIs.Count > 1)
                {
                    int intBlackArea = 0;
                    if (m_smVisionInfo.g_blnWantUseEmptyPattern)
                        intBlackArea = ROI.GetPixelArea(m_smVisionInfo.g_arrPositioningROIs[1], m_smVisionInfo.g_objPositioning.ref_intEmptyThreshold, 0);
                    else
                        intBlackArea = ROI.GetPixelArea(m_smVisionInfo.g_arrPositioningROIs[0], m_smVisionInfo.g_objPositioning.ref_intEmptyThreshold, 0);

                    //int intWhiteArea = ROI.GetPixelArea(m_smVisionInfo.g_arrPositioningROIs[1], m_smVisionInfo.g_objPositioning.ref_intEmptyThreshold, 1);  // g_arrPositioningROIs[1] is empty ROI.
                    lbl_EmptyWhiteArea.Text = intBlackArea.ToString();
                }
                else
                    lbl_EmptyWhiteArea.Text = "0";
            }

            if (m_smVisionInfo.g_blnWantCheckPocketPosition)
            {
                txt_PocketPositionTolerance.Text = m_smVisionInfo.g_objPocketPosition.ref_fPositionXTolerance.ToString();
                lbl_TemplatePocketDistance.Text = (m_smVisionInfo.g_objPocketPosition.ref_fTemplateXDistance * 1000 / m_smVisionInfo.g_fCalibPixelX).ToString();
                lbl_CurrentPocketDistance.Text = (m_smVisionInfo.g_objPocketPosition.ref_fResultXDistance * 1000 / m_smVisionInfo.g_fCalibPixelX).ToString();
                txt_PocketPatternMinScore.Text = m_smVisionInfo.g_objPocketPosition.ref_intMinMatchingScore.ToString();
            }

            if (m_smVisionInfo.g_blnWantCheckUnitSitProper)
            {
                string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Package\\";

                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].LoadPackage(strFolderPath + "\\Settings.xml", "Settings", m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);

                if (m_smVisionInfo.g_arrPackage.Count > 1)
                {
                    if (File.Exists(strFolderPath + "\\Settings2.xml"))
                        m_smVisionInfo.g_arrPackage[1].LoadPackage(strFolderPath + "\\Settings2.xml", "Settings", m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                    else
                        m_smVisionInfo.g_arrPackage[1].LoadPackage(strFolderPath + "\\Settings.xml", "Settings", m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                }
                txt_MinSittingWidth.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetSittingWidthMin(1).ToString("F4");
                txt_MaxSittingWidth.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetSittingWidthMax(1).ToString("F4");
                txt_MinSittingHeight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetSittingHeightMin(1).ToString("F4");
                txt_MaxSittingHeight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetSittingHeightMax(1).ToString("F4");
                lbl_SittingWidth.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetResultWidth(1).ToString("F" + m_intDecimal);
                lbl_SittingHeight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetResultHeight(1).ToString("F" + m_intDecimal);
            }
        }

        private void dgd_PkgSetting_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            /*
             * Please take note dgd_PkgSetting_CellValueChanged event will be triggered during loading and when value change in cell.
             * dgd_PkgSetting_CellEndEdit will not be triggred although value change. It will only been triggered after user finish change cell value manually (even same value), 
             */

            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                int i = e.RowIndex;
                int c = e.ColumnIndex;
                string strRowName = ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString();
                switch (((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString())
                {
                    case "Package Width":
                        {
                            if (c == 1)
                            {
                                float fValue;
                                float fValuePrev = m_smVisionInfo.g_arrPackage[u].GetUnitWidthMin(1);
                                if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[c].Value), out fValue))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[c].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                    if (fValue > m_smVisionInfo.g_arrPackage[u].GetUnitWidthMax(1))
                                    {
                                        dgd_PkgSetting.Rows[i].Cells[c].Value = fValuePrev.ToString("F" + m_intDecimal);
                                        SRMMessageBox.Show("Package Min Width cannot be larger than Package Max Width!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        m_smVisionInfo.g_arrPackage[u].SetUnitWidthMin(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                                    }
                                }
                            }
                            else if (c == 2)
                            {
                                float fValue;
                                float fValuePrev = m_smVisionInfo.g_arrPackage[u].GetUnitWidthMax(1);
                                if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[c].Value), out fValue))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[c].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                    if (fValue < m_smVisionInfo.g_arrPackage[u].GetUnitWidthMin(1))
                                    {
                                        dgd_PkgSetting.Rows[i].Cells[c].Value = fValuePrev.ToString("F" + m_intDecimal);
                                        SRMMessageBox.Show("Package Max Width cannot be smaller than Package Min Width!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        m_smVisionInfo.g_arrPackage[u].SetUnitWidthMax(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                                    }
                                }
                            }
                        }
                        break;
                    case "Package Length":
                        {
                            if (c == 1)
                            {
                                float fValue;
                                float fValuePrev = m_smVisionInfo.g_arrPackage[u].GetUnitHeightMin(1);
                                if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[c].Value), out fValue))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[c].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                    if (fValue > m_smVisionInfo.g_arrPackage[u].GetUnitHeightMax(1))
                                    {
                                        dgd_PkgSetting.Rows[i].Cells[c].Value = fValuePrev.ToString("F" + m_intDecimal);
                                        SRMMessageBox.Show("Package Min Height cannot be larger than Package Max Height!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        m_smVisionInfo.g_arrPackage[u].SetUnitHeightMin(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                                    }
                                }
                            }
                            else if (c == 2)
                            {
                                float fValue;
                                float fValuePrev = m_smVisionInfo.g_arrPackage[u].GetUnitHeightMax(1);
                                if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[c].Value), out fValue))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[c].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                    if (fValue < m_smVisionInfo.g_arrPackage[u].GetUnitHeightMin(1))
                                    {
                                        dgd_PkgSetting.Rows[i].Cells[c].Value = fValuePrev.ToString("F" + m_intDecimal);
                                        SRMMessageBox.Show("Package Max Height cannot be smaller than Package Min Height!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        m_smVisionInfo.g_arrPackage[u].SetUnitHeightMax(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                                    }
                                }
                            }
                        }
                        break;
                    case "Package Angle":
                        {
                            if (c == 2)
                            {
                                float fValue;
                                float fValuePrev = m_smVisionInfo.g_arrPackage[u].GetUnitAngleMax();
                                if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[c].Value), out fValue))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[c].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrPackage[u].SetUnitAngleMax(fValue);
                                }
                            }
                        }
                        break;
                    case "Scratches Length":
                        {
                            float fValue;
                            float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam(2, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);
                            if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[2].Value), out fValue))
                            {
                                dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam(2, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        break;
                    case "Contamination Length":
                        {
                            float fValue;
                            float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam(4, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);
                            if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[2].Value), out fValue))
                            {
                                dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam(4, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        break;
                    case "Chipped Off Length":
                        {
                            float fValue;
                            float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam(1, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);
                            if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[2].Value), out fValue))
                            {
                                dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Chip Defect length cannot larger or equal to half the unit Height or Width!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        break;
                    case "Cracked Length":
                        {
                            float fValue;
                            float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam(0, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);
                            if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[2].Value), out fValue))
                            {
                                dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam(0, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        break;
                    case "Mold Flash Length":
                        {
                            float fValue;
                            float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam(3, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);
                            if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[2].Value), out fValue))
                            {
                                dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam(3, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        break;
                    case "Void Length":
                        {
                            float fValue;
                            float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam(5, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);
                            if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[2].Value), out fValue))
                            {
                                dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam(5, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        break;
                    case "Scratches Area":
                        {
                            float fValue;
                            float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam(2) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                            if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[2].Value), out fValue))
                            {
                                dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam(2, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Defect area cannot larger than unit area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        break;
                    case "Contamination Area":
                        {
                            float fValue;
                            float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam(4) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                            if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[2].Value), out fValue))
                            {
                                dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam(4, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Defect area cannot larger than unit area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        break;
                    case "Chipped Off Area":
                        {
                            float fValue;
                            float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam(1) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                            if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[2].Value), out fValue))
                            {
                                dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam(1, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Defect area cannot larger than unit area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        break;
                    case "Cracked Area":
                        {
                            float fValue;
                            float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam(0) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                            if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[2].Value), out fValue))
                            {
                                dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam(0, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Defect area cannot larger than unit area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        break;
                    case "Mold Flash Area":
                        {
                            float fValue;
                            float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam(3) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                            if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[2].Value), out fValue))
                            {
                                dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam(3, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Defect area cannot larger than unit area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        break;
                    case "Void Area":
                        {
                            float fValue;
                            float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam(5) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                            if (!float.TryParse(Convert.ToString(dgd_PkgSetting.Rows[i].Cells[2].Value), out fValue))
                            {
                                dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam(5, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                                {
                                    dgd_PkgSetting.Rows[i].Cells[2].Value = fValuePrev.ToString("F" + m_intDecimal);
                                    SRMMessageBox.Show("Defect area cannot larger than unit area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        break;
                    default:
                        SRMMessageBox.Show("Cannot find row name [" + strRowName + "].");
                        break;
                }

                m_smVisionInfo.g_blnViewPackageDefectSetting = true;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }

            UpdatePkgSettingGUI();
        }

        private void dgd_PkgSetting_CellClick(object sender, DataGridViewCellEventArgs e)
        {


            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            int i = e.RowIndex;

            if (i == -1)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                string strRowName = ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString();
                switch (((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString())
                {
                    case "Scratches Length":
                        m_smVisionInfo.g_intSelectedPackageDefect = 2;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                        break;
                    case "Contamination Length":
                        m_smVisionInfo.g_intSelectedPackageDefect = 4;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                        break;
                    case "Chipped Off Length":
                        m_smVisionInfo.g_blnViewRotatedPackageImage = true;
                        m_smVisionInfo.g_intSelectedPackageDefect = 1;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                        break;
                    case "Cracked Length":
                        m_smVisionInfo.g_intSelectedPackageDefect = 0;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                        break;
                    case "Mold Flash Length":
                        m_smVisionInfo.g_intSelectedPackageDefect = 3;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                        break;
                    case "Scratches Area":
                        m_smVisionInfo.g_intSelectedPackageDefect = 2;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = true;
                        break;
                    case "Contamination Area":
                        m_smVisionInfo.g_intSelectedPackageDefect = 4;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = true;
                        break;
                    case "Chipped Off Area":
                        m_smVisionInfo.g_blnViewRotatedPackageImage = true;
                        m_smVisionInfo.g_intSelectedPackageDefect = 1;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = true;
                        break;
                    case "Cracked Area":
                        m_smVisionInfo.g_intSelectedPackageDefect = 0;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = true;
                        break;
                    case "Mold Flash Area":
                        m_smVisionInfo.g_intSelectedPackageDefect = 3;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = true;
                        break;
                    case "Void Length":
                        m_smVisionInfo.g_intSelectedPackageDefect = 5;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                        break;
                    case "Void Area":
                        m_smVisionInfo.g_intSelectedPackageDefect = 5;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = true;
                        break;
                    case "Package Width":
                    case "Package Length":
                        break;
                    case "Package Angle":
                        break;
                    default:
                        SRMMessageBox.Show("Cannot find row name [" + strRowName + "].");
                        break;
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_PkgSetting_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SittingOffset_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            //m_smVisionInfo.g_arrPackage[u].ref_fUnitWidthMin = Convert.ToSingle(txt_SittingMinWidth.Text);
            //m_smVisionInfo.g_arrPackage[u].ref_fUnitHeightMin = Convert.ToSingle(txt_SittingMinHeight.Text);
            //m_smVisionInfo.g_arrPackage[u].ref_fUnitWidthMax = Convert.ToSingle(txt_SittingMaxWidth.Text);
            //m_smVisionInfo.g_arrPackage[u].ref_fUnitHeightMax = Convert.ToSingle(txt_SittingMaxHeight.Text);

            float fValue = 0;
            float fValuePrev = 0;
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {

                fValuePrev = m_smVisionInfo.g_arrPackage[u].GetUnitWidthMin(1);
                if (!float.TryParse(Convert.ToString(txt_MinSittingWidth.Text), out fValue))
                {
                    txt_MinSittingWidth.Text = fValuePrev.ToString("F4");
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (fValue > m_smVisionInfo.g_arrPackage[u].GetUnitWidthMax(1))
                    {
                        txt_MinSittingWidth.Text = fValuePrev.ToString("F4");
                        SRMMessageBox.Show("Min Width cannot be larger than Max Width!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                  
                }

                fValuePrev = m_smVisionInfo.g_arrPackage[u].GetUnitWidthMax(1);
                if (!float.TryParse(Convert.ToString(txt_MaxSittingWidth.Text), out fValue))
                {
                    txt_MaxSittingWidth.Text = fValuePrev.ToString("F4");
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (fValue < m_smVisionInfo.g_arrPackage[u].GetUnitWidthMin(1))
                    {
                        txt_MaxSittingWidth.Text = fValuePrev.ToString("F4");
                        SRMMessageBox.Show("Max Width cannot be smaller than Min Width!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                   
                }

                fValuePrev = m_smVisionInfo.g_arrPackage[u].GetUnitHeightMin(1);
                if (!float.TryParse(Convert.ToString(txt_MinSittingHeight.Text), out fValue))
                {
                    txt_MinSittingHeight.Text = fValuePrev.ToString("F4");
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (fValue > m_smVisionInfo.g_arrPackage[u].GetUnitHeightMax(1))
                    {
                        txt_MinSittingHeight.Text = fValuePrev.ToString("F4");
                        SRMMessageBox.Show("Min Height cannot be larger than Max Height!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                }

                fValuePrev = m_smVisionInfo.g_arrPackage[u].GetUnitHeightMax(1);
                if (!float.TryParse(Convert.ToString(txt_MaxSittingHeight.Text), out fValue))
                {
                    txt_MaxSittingHeight.Text = fValuePrev.ToString("F4");
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (fValue < m_smVisionInfo.g_arrPackage[u].GetUnitHeightMin(1))
                    {
                        txt_MaxSittingHeight.Text = fValuePrev.ToString("F4");
                        SRMMessageBox.Show("Max Height cannot be smaller than Min Height!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                  
                }

            }
        }

        private void txt_OrientAngle_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnEnterTextBox)
                return;
            
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (m_blnWantSet1ToAll || m_smVisionInfo.g_blnWantSkipMark || (m_intVisionType == 0x01))
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrOrients[u].Count; i++)
                        m_smVisionInfo.g_arrOrients[u][i].ref_fAngleTolerance = (float)(Convert.ToDouble(txt_OrientAngle.Text));
                }
                else
                    m_smVisionInfo.g_arrOrients[u][m_smVisionInfo.g_intSelectedTemplate].ref_fAngleTolerance = (float)(Convert.ToDouble(txt_OrientAngle.Text));
            }
        }

        private void txt_OrientX_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnEnterTextBox)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (m_blnWantSet1ToAll || m_smVisionInfo.g_blnWantSkipMark || (m_intVisionType == 0x01))
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrOrients[u].Count; i++)
                        m_smVisionInfo.g_arrOrients[u][i].ref_fXTolerance = (float)(Convert.ToDouble(txt_OrientX.Text));
                }
                else
                    m_smVisionInfo.g_arrOrients[u][m_smVisionInfo.g_intSelectedTemplate].ref_fXTolerance = (float)(Convert.ToDouble(txt_OrientX.Text));
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_OrientY_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnEnterTextBox)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (m_blnWantSet1ToAll || m_smVisionInfo.g_blnWantSkipMark || (m_intVisionType == 0x01))
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrOrients[u].Count; i++)
                        m_smVisionInfo.g_arrOrients[u][i].ref_fYTolerance = (float)(Convert.ToDouble(txt_OrientY.Text));
                }
                else
                    m_smVisionInfo.g_arrOrients[u][m_smVisionInfo.g_intSelectedTemplate].ref_fYTolerance = (float)(Convert.ToDouble(txt_OrientY.Text));
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_EmptyTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnEnterTextBox)
                return;

            trackBar_EmptyTolerance.Value = Convert.ToInt32(txt_EmptyScore.Text);
            
            //m_smVisionInfo.g_objPositioning.ref_intMinEmptyScore = (int)(trackBar_EmptyTolerance.Value / 100.0f);
             
        }

   

        private void trackBar_EmptyTolerance_Scroll(object sender, EventArgs e)
        {
            txt_EmptyScore.Text = trackBar_EmptyTolerance.Value.ToString();
            
          //m_smVisionInfo.g_objPositioning.ref_intMinEmptyScore = (int)(trackBar_EmptyTolerance.Value / 100.0f);
               
        }

        private void txt_GeneralVerticalBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_GeneralVerticalBright.Text, out fValue))
                {
                    txt_GeneralVerticalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam(2, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_GeneralVerticalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_GeneralVerticalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_GeneralVerticalDark.Text, out fValue))
                {
                    txt_GeneralVerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Dark, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_GeneralVerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_GeneralHorizontalBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_GeneralHorizontalBright.Text, out fValue))
                {
                    txt_GeneralHorizontalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Bright, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_GeneralHorizontalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_GeneralHorizontalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_GeneralHorizontalDark.Text, out fValue))
                {
                    txt_GeneralHorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Dark, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_GeneralHorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_GeneralAreaBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam((int)Package.eDefect.Bright) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_GeneralAreaBright.Text, out fValue))
                {
                    txt_GeneralAreaBright.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam((int)Package.eDefect.Bright, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_GeneralAreaBright.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_GeneralAreaDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam((int)Package.eDefect.Dark) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_GeneralAreaDark.Text, out fValue))
                {
                    txt_GeneralAreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam((int)Package.eDefect.Dark, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_GeneralAreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_GeneralTotalAreaBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectTotalAreaParam((int)Package.eDefect.Bright) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_GeneralTotalAreaBright.Text, out fValue))
                {
                    txt_GeneralTotalAreaBright.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectTotalAreaParam((int)Package.eDefect.Bright, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_GeneralTotalAreaBright.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_GeneralTotalAreaDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectTotalAreaParam((int)Package.eDefect.Dark) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_GeneralTotalAreaDark.Text, out fValue))
                {
                    txt_GeneralTotalAreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectTotalAreaParam((int)Package.eDefect.Dark, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_GeneralTotalAreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_GeneralChipAreaBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam(1) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_GeneralChipAreaBright.Text, out fValue))
                {
                    txt_GeneralChipAreaBright.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam(1, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_GeneralChipAreaBright.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_GeneralChipAreaDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam(6) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_GeneralChipAreaDark.Text, out fValue))
                {
                    txt_GeneralChipAreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam(6, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_GeneralChipAreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_CrackVerticalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_CrackVerticalDark.Text, out fValue))
                {
                    txt_CrackVerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Crack, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_CrackVerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_CrackHorizontalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_CrackHorizontalDark.Text, out fValue))
                {
                    txt_CrackHorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Crack, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_CrackHorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_CrackAreaDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam((int)Package.eDefect.Crack) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_CrackAreaDark.Text, out fValue))
                {
                    txt_CrackAreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam((int)Package.eDefect.Crack, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_CrackAreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_VoidVerticalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.Void, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_VoidVerticalDark.Text, out fValue))
                {
                    txt_VoidVerticalDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Void, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_VoidVerticalDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_VoidHorizontalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.Void, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_VoidHorizontalDark.Text, out fValue))
                {
                    txt_VoidHorizontalDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Void, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_VoidHorizontalDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_ChipppedOffAreaBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam((int)Package.eDefect.ChipBright) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_ChippedAreaBright.Text, out fValue))
                {
                    txt_ChippedAreaBright.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam((int)Package.eDefect.ChipBright, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_ChippedAreaBright.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }
        private void txt_ChipppedOffAreaDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam((int)Package.eDefect.ChipDark) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_ChippedAreaDark.Text, out fValue))
                {
                    txt_ChippedAreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam((int)Package.eDefect.ChipDark, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_ChippedAreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }
        private void txt_MoldFlashVerticalBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam(3, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_MoldFlashVerticalBright.Text, out fValue))
                {
                    txt_MoldFlashVerticalBright.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam(3, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_MoldFlashVerticalBright.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_MoldFlashHorizontalBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam(3, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_MoldFlashHorizontalBright.Text, out fValue))
                {
                    txt_MoldFlashHorizontalBright.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam(3, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_MoldFlashHorizontalBright.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_MoldFlashAreaBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam(3) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_MoldFlashAreaBright.Text, out fValue))
                {
                    txt_MoldFlashAreaBright.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam(3, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_MoldFlashAreaBright.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_DefectLeave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralVerticalBright_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 2;
            m_smVisionInfo.g_intSelectedPackageLengthType = 1;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam(2, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        }

        private void txt_GeneralVerticalDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 4;
            m_smVisionInfo.g_intSelectedPackageLengthType = 1;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam((int)Package.eDefect.Dark, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        }

        private void txt_GeneralHorizontalBright_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 2;
            m_smVisionInfo.g_intSelectedPackageLengthType = 0;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam((int)Package.eDefect.Bright, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);
        }

        private void txt_GeneralHorizontalDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 4;
            m_smVisionInfo.g_intSelectedPackageLengthType = 0;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam((int)Package.eDefect.Dark, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);

        }

        private void txt_GeneralAreaBright_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 2;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralAreaDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = 4;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralTotalAreaBright_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 2;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralTotalAreaDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 4;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralChipAreaBright_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 1;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralChipAreaDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 6;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CrackVerticalDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 0;
            m_smVisionInfo.g_intSelectedPackageLengthType = 1;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam((int)Package.eDefect.Crack, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        }

        private void txt_CrackHorizontalDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 0;
            m_smVisionInfo.g_intSelectedPackageLengthType = 0;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam((int)Package.eDefect.Crack, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);
        }

        private void txt_CrackAreaDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 0;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_VoidVerticalDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 5;
            m_smVisionInfo.g_intSelectedPackageLengthType = 1;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_VoidHorizontalDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 5;
            m_smVisionInfo.g_intSelectedPackageLengthType = 0;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_VoidAreaDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 5;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldFlashAreaBright_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 3;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralVerticalBright_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 2;
            m_smVisionInfo.g_intSelectedPackageLengthType = 1;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_Setting_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!m_blnChangeScoreSetting)
                return;

            if (e.ColumnIndex < 0)
                return;

            //Skip if col is result score
            if (e.ColumnIndex == 2 || e.ColumnIndex == 3 ||
                e.ColumnIndex == 5 || e.ColumnIndex == 6 ||
                e.ColumnIndex == 8 || e.ColumnIndex == 9 ||
                e.ColumnIndex == 10 ||
                e.ColumnIndex == 12 || e.ColumnIndex == 13)
                return;

            switch (e.ColumnIndex)
            {
                case 0:
                    {
                        SetCharsEnableForm objSetCharsEnableForm = new SetCharsEnableForm(-1, true);
                        objSetCharsEnableForm.Location = new Point(769, 310);
                        if (objSetCharsEnableForm.ShowDialog() == DialogResult.OK)
                        {
                            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                                for (int i = 0; i < dgd_Setting.Rows.Count; i++)
                                    m_smVisionInfo.g_arrMarks[u].SetCharEnable(i, objSetCharsEnableForm.ref_blnEnableMark, m_blnWantSet1ToAll);
                        }

                        UpdateOCVSettingTable();
                    }
                    break;
                case 1:
                    {
                        int intCurrentSetValue;
                        if (dgd_Setting.Rows.Count > 0)
                            intCurrentSetValue = Convert.ToInt32(dgd_Setting.Rows[0].Cells[1].Value);
                        else
                            intCurrentSetValue = 75;

                        SetCharsValueForm2 objSetCharValueForm = new SetCharsValueForm2(-1, intCurrentSetValue);
                        objSetCharValueForm.Location = new Point(769, 310);
                        if (objSetCharValueForm.ShowDialog() == DialogResult.OK)
                        {
                            RegistryKey Key = Registry.LocalMachine.OpenSubKey("Software", true);
                            RegistryKey subKey = Key.OpenSubKey("SVG\\Visions\\", true);
                            RegistryKey subKey2 = Key.OpenSubKey("SVG\\Visions\\" + m_smVisionInfo.g_strVisionFolderName, true);
                            bool blnSecureOnOff = Convert.ToBoolean(subKey.GetValue("SecureOnOff-MarkScore", false));
                            int intMinScoreSetting = Convert.ToInt32(subKey2.GetValue("Secure-MinMarkScoreSetting", -1));
                            if (intMinScoreSetting == -1)   // If parameter no exist
                            {
                                intMinScoreSetting = 30;    // Default 30%
                                // if registry does not have this parameter, user cannot know what parameter name they have to set. In order to make it user friendly, we write it to system so that user can find the parameter, then they can modified the value. 
                                subKey2.SetValue("Secure-MinMarkScoreSetting", intMinScoreSetting.ToString());
                            }

                            // 2021 01 25 - CCENG: Customer request mark score cannot lower than minimum mark score advance setting.
                            if (blnSecureOnOff && objSetCharValueForm.ref_intSetValue < intMinScoreSetting)
                            {
                                SRMMessageBox.Show("Mark Setting cannot lower than Minimum Mark Score [" + intMinScoreSetting.ToString() + "%].");

                            }
                            else
                            {
                                // 2021 01 25 - CCENG: This code has bug. Will change the dont care mark to inspect mark.
                                //for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                                //    for (int i = 0; i < dgd_Setting.Rows.Count; i++)
                                //        m_smVisionInfo.g_arrMarks[u].SetCharSetting(i, objSetCharValueForm.ref_intSetValue, true, m_blnWantSet1ToAll);

                                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                                {
                                    if (objSetCharValueForm.ref_blnSetAllRows)
                                    {
                                        //2020-05-08 ZJYEOH : To solve other template with more mark cannot be set
                                        if (m_blnWantSet1ToAll)
                                            m_smVisionInfo.g_arrMarks[u].SetCharSetting(objSetCharValueForm.ref_intSetValue);
                                        else
                                        {
                                            for (int i = 0; i < dgd_Setting.Rows.Count; i++)
                                                m_smVisionInfo.g_arrMarks[u].SetCharSetting(i, objSetCharValueForm.ref_intSetValue);
                                        }
                                    }
                                    else
                                        m_smVisionInfo.g_arrMarks[u].SetCharSetting(e.RowIndex, objSetCharValueForm.ref_intSetValue, m_blnWantSet1ToAll);
                                }
                            }
                        }

                        UpdateOCVSettingTable();
                    }
                    break;
                case 4:
                    {
                        float fCurrentSetValue = (float)Convert.ToDouble(dgd_Setting.Rows[0].Cells[4].Value);
                        SetCharsExcessMissAreaValueForm objSetValueForm = new SetCharsExcessMissAreaValueForm(m_smCustomizeInfo, "Max Excess Area", -1, fCurrentSetValue);
                        objSetValueForm.Location = new Point(769, 310);
                        if (objSetValueForm.ShowDialog() == DialogResult.OK)
                        {
                            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                            {
                                for (int i = 0; i < dgd_Setting.Rows.Count; i++)
                                {
                                    if (((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor == Color.LightGray)
                                        continue;
                                    m_smVisionInfo.g_arrMarks[u].SetCharMaxExcessAreaSetting(i, objSetValueForm.ref_fSetValue, m_blnWantSet1ToAll);
                                }
                            }
                        }

                        UpdateOCVSettingTable();
                    }
                    break;
                case 7:
                    {
                        float intCurrentSetValue = (float)Convert.ToDouble(dgd_Setting.Rows[0].Cells[7].Value);
                        SetCharsExcessMissAreaValueForm objSetValueForm = new SetCharsExcessMissAreaValueForm(m_smCustomizeInfo, "Max Miss Area", -1, intCurrentSetValue);
                        objSetValueForm.Location = new Point(769, 310);
                        if (objSetValueForm.ShowDialog() == DialogResult.OK)
                        {
                            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                            {
                                for (int i = 0; i < dgd_Setting.Rows.Count; i++)
                                {
                                    {
                                        if (((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor == Color.LightGray)
                                            continue;
                                        m_smVisionInfo.g_arrMarks[u].SetCharMaxBrokenAreaSetting(i, objSetValueForm.ref_fSetValue, m_blnWantSet1ToAll);
                                    }
                                }
                            }
                        }

                        UpdateOCVSettingTable();
                    }
                    break;
                case 11:
                    {
                        int intCurrentSetValue;
                        if (dgd_Setting.Rows.Count > 0)
                            intCurrentSetValue = Convert.ToInt32(dgd_Setting.Rows[0].Cells[11].Value);
                        else
                            intCurrentSetValue = 20;

                        SetCharsValueForm2 objSetCharValueForm = new SetCharsValueForm2(-1, intCurrentSetValue);
                        objSetCharValueForm.Location = new Point(769, 310);
                        if (objSetCharValueForm.ShowDialog() == DialogResult.OK)
                        {
                            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                            {
                                if (objSetCharValueForm.ref_blnSetAllRows)
                                {
                                    //2020-05-08 ZJYEOH : To solve other template with more mark cannot be set
                                    if (m_blnWantSet1ToAll)
                                        m_smVisionInfo.g_arrMarks[u].SetMaxAGVPercent(objSetCharValueForm.ref_intSetValue);
                                    else
                                    {
                                        for (int i = 0; i < dgd_Setting.Rows.Count; i++)
                                            m_smVisionInfo.g_arrMarks[u].SetMaxAGVPercent(i, objSetCharValueForm.ref_intSetValue);
                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrMarks[u].SetMaxAGVPercent(e.RowIndex, objSetCharValueForm.ref_intSetValue, m_blnWantSet1ToAll);
                            }
                        }

                        UpdateOCVSettingTable();
                    }
                    break;
            }
        }

        private void txt_PocketPositionTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objPocketPosition.ref_fPositionXTolerance = Convert.ToInt32(txt_PocketPositionTolerance.Text);
        }

        private void txt_PocketPatternMinScore_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objPocketPosition.ref_intMinMatchingScore = Convert.ToInt32(txt_PocketPatternMinScore.Text);
        }

        private void srmLabel70_Click(object sender, EventArgs e)
        {

        }

        private void txt_PocketPositionRef_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objPocketPosition.ref_intPocketPositionReference = Convert.ToInt32(txt_PocketPositionRef.Text);
        }

        private void txt_PocketPositionTol_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objPocketPosition.ref_intPocketPositionTolerance = Convert.ToInt32(txt_PocketPositionTol.Text);
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
        }

        private void txt_Dark2VerticalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_Dark2VerticalDark.Text, out fValue))
                {
                    txt_Dark2VerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Dark2, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_Dark2VerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_Dark3VerticalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                //m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.Dark3, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_Dark3VerticalDark.Text, out fValue))
                {
                    txt_Dark3VerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Dark3, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_Dark3VerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark4VerticalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;

                if (!float.TryParse(txt_Dark4VerticalDark.Text, out fValue))
                {
                    txt_Dark4VerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Dark4, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_Dark4VerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark2HorizontalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_Dark2HorizontalDark.Text, out fValue))
                {
                    txt_Dark2HorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Dark2, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_Dark2HorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_Dark3HorizontalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_Dark3HorizontalDark.Text, out fValue))
                {
                    txt_Dark3HorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Dark3, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_Dark3HorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark4HorizontalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_Dark4HorizontalDark.Text, out fValue))
                {
                    txt_Dark4HorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Dark4, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_Dark4HorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark2AreaDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam((int)Package.eDefect.Dark2) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_Dark2AreaDark.Text, out fValue))
                {
                    txt_Dark2AreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam((int)Package.eDefect.Dark2, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_Dark2AreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_Dark3AreaDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam((int)Package.eDefect.Dark3) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_Dark3AreaDark.Text, out fValue))
                {
                    txt_Dark3AreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam((int)Package.eDefect.Dark3, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_Dark3AreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark4AreaDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectAreaParam((int)Package.eDefect.Dark4) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_Dark4AreaDark.Text, out fValue))
                {
                    txt_Dark4AreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectAreaParam((int)Package.eDefect.Dark4, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_Dark4AreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark2VerticalDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 7;
            m_smVisionInfo.g_intSelectedPackageLengthType = 1;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam((int)Package.eDefect.Dark2, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

        }

        private void txt_Dark3VerticalDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 8;
            m_smVisionInfo.g_intSelectedPackageLengthType = 1;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam((int)Package.eDefect.Dark3, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

        }

        private void txt_Dark4VerticalDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 9;
            m_smVisionInfo.g_intSelectedPackageLengthType = 1;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam((int)Package.eDefect.Dark4, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

        }

        private void txt_Dark2HorizontalDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 7;
            m_smVisionInfo.g_intSelectedPackageLengthType = 0;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam((int)Package.eDefect.Dark2, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);

        }

        private void txt_Dark3HorizontalDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 8;
            m_smVisionInfo.g_intSelectedPackageLengthType = 0;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam((int)Package.eDefect.Dark3, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);
        }

        private void txt_Dark4HorizontalDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 9;
            m_smVisionInfo.g_intSelectedPackageLengthType = 0;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam((int)Package.eDefect.Dark4, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);
        }

        private void txt_Dark2AreaDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = 7;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark3AreaDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = 8;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark4AreaDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = 9;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark2TotalAreaDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectTotalAreaParam((int)Package.eDefect.Dark2) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_Dark2TotalAreaDark.Text, out fValue))
                {
                    txt_Dark2TotalAreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectTotalAreaParam((int)Package.eDefect.Dark2, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_Dark2TotalAreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_Dark3TotalAreaDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectTotalAreaParam((int)Package.eDefect.Dark3) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_Dark3TotalAreaDark.Text, out fValue))
                {
                    txt_Dark3TotalAreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectTotalAreaParam((int)Package.eDefect.Dark3, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_Dark3TotalAreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark4TotalAreaDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectTotalAreaParam((int)Package.eDefect.Dark4) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_Dark4TotalAreaDark.Text, out fValue))
                {
                    txt_Dark4TotalAreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectTotalAreaParam((int)Package.eDefect.Dark4, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_Dark4TotalAreaDark.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark2TotalAreaDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 7;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark3TotalAreaDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 8;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark4TotalAreaDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 9;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldFlashTotalAreaDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                float fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectTotalAreaParam(3) / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                if (!float.TryParse(txt_MoldFlashTotalAreaBright.Text, out fValue))
                {
                    txt_MoldFlashTotalAreaBright.Text = fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectTotalAreaParam(3, fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        txt_MoldFlashTotalAreaBright.Text = fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit Area!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_MoldFlashTotalAreaBright_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 3;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_Setting_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnChangeScoreSetting)
                return;

            if (e.ColumnIndex < 0)
                return;

            //Skip if col is result score
            if (e.ColumnIndex != 10)
                return;

            switch (e.ColumnIndex)
            {
                case 10:
                    {
                        if (e.RowIndex < 0) //2021-01-05 ZJYEOH: if row index less than 1 means toggle all row setting
                        {
                            for (int i = 0; i < dgd_Setting.RowCount; i++)
                            {
                                bool blnNewResult = !Convert.ToBoolean(dgd_Setting.Rows[i].Cells[10].Value);
                                dgd_Setting.Rows[i].Cells[10].Value = blnNewResult;
                                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                                {
                                    m_smVisionInfo.g_arrMarks[u].SetCharWantBrokenMarkSetting(i, blnNewResult, false);
                                }
                            }
                            UpdateOCVSettingTable();

                        }
                        else
                        {
                            bool blnNewResult = !Convert.ToBoolean(dgd_Setting.Rows[e.RowIndex].Cells[10].Value);
                            dgd_Setting.Rows[e.RowIndex].Cells[10].Value = blnNewResult;
                            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                            {
                                m_smVisionInfo.g_arrMarks[u].SetCharWantBrokenMarkSetting(e.RowIndex, blnNewResult, false);
                            }
                            UpdateOCVSettingTable();
                        }
                    }
                    break;
            }
        }

        private string GetDecimalFormat()
        {
            switch (m_smCustomizeInfo.g_intMarkUnitDisplay)
            {
                case 0:
                    return string.Empty;
                    break;
                case 1:
                case 2:
                case 3:
                    return ("F" + m_intDecimal);
                    break;
            }

            return string.Empty;
        }

        private void cbo_GeneralBrightDefectFailCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intBrightDefectDimensionFailCondition = cbo_GeneralBrightDefectFailCondition.SelectedIndex;
            }

        }

        private void cbo_GeneralDarkDefectFailCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intDarkDefectDimensionFailCondition = cbo_GeneralDarkDefectFailCondition.SelectedIndex;
            }

        }

        private void cbo_CrackDarkDefectFailCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intCrackDarkDefectDimensionFailCondition = cbo_CrackDarkDefectFailCondition.SelectedIndex;
            }

        }

        private void cbo_GeneralDark2DefectFailCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intDark2DefectDimensionFailCondition = cbo_GeneralDark2DefectFailCondition.SelectedIndex;
            }

        }

        private void cbo_GeneralDark3DefectFailCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intDark3DefectDimensionFailCondition = cbo_GeneralDark3DefectFailCondition.SelectedIndex;
            }
        }

        private void cbo_GeneralDark4DefectFailCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intDark4DefectDimensionFailCondition = cbo_GeneralDark4DefectFailCondition.SelectedIndex;
            }
        }

        private void btn_PackageOffset_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            PackageOffsetSetting objform = new PackageOffsetSetting(m_smVisionInfo, m_smCustomizeInfo, m_smProductionInfo, m_smVisionInfo.g_intSelectedUnit, false);
            if(objform.ShowDialog() == DialogResult.Yes)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = true;
            }
        }
        private void txt_ColorDefectLength_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            
            float fValue = 0;

            if (((TextBox)sender).Name.Contains("1"))
            {
                if (float.TryParse(txt_ColorDefect1Length.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 0);
                }
            }
            else if (((TextBox)sender).Name.Contains("2"))
            {
                if (float.TryParse(txt_ColorDefect2Length.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 1);
                }
            }
            else if (((TextBox)sender).Name.Contains("3"))
            {
                if (float.TryParse(txt_ColorDefect3Length.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 2);
                }
            }
            else if (((TextBox)sender).Name.Contains("4"))
            {
                if (float.TryParse(txt_ColorDefect4Length.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 3);
                }
            }
            else if (((TextBox)sender).Name.Contains("5"))
            {
                if (float.TryParse(txt_ColorDefect5Length.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 4);
                }
            }

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ColorDefectWidth_Center_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;

            if (((TextBox)sender).Name.Contains("1"))
            {
                if (float.TryParse(txt_ColorDefect1Width.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 0);
                }
            }
            else if (((TextBox)sender).Name.Contains("2"))
            {
                if (float.TryParse(txt_ColorDefect2Width.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 1);
                }
            }
            else if (((TextBox)sender).Name.Contains("3"))
            {
                if (float.TryParse(txt_ColorDefect3Width.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 2);
                }
            }
            else if (((TextBox)sender).Name.Contains("4"))
            {
                if (float.TryParse(txt_ColorDefect4Width.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 3);
                }
            }
            else if (((TextBox)sender).Name.Contains("5"))
            {
                if (float.TryParse(txt_ColorDefect5Width.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 4);
                }
            }

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ColorDefectMinArea_Center_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;

            if (((TextBox)sender).Name.Contains("1"))
            {
                if (float.TryParse(txt_ColorDefect1MinArea.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 0);
                }
            }
            else if (((TextBox)sender).Name.Contains("2"))
            {
                if (float.TryParse(txt_ColorDefect2MinArea.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 1);
                }
            }
            else if (((TextBox)sender).Name.Contains("3"))
            {
                if (float.TryParse(txt_ColorDefect3MinArea.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 2);
                }
            }
            else if (((TextBox)sender).Name.Contains("4"))
            {
                if (float.TryParse(txt_ColorDefect4MinArea.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 3);
                }
            }
            else if (((TextBox)sender).Name.Contains("5"))
            {
                if (float.TryParse(txt_ColorDefect5MinArea.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 4);
                }
            }

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ColorDefectMaxArea_Center_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;

            if (((TextBox)sender).Name.Contains("1"))
            {
                if (float.TryParse(txt_ColorDefect1MaxArea.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 0);
                }
            }
            else if (((TextBox)sender).Name.Contains("2"))
            {
                if (float.TryParse(txt_ColorDefect2MaxArea.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 1);
                }
            }
            else if (((TextBox)sender).Name.Contains("3"))
            {
                if (float.TryParse(txt_ColorDefect3MaxArea.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 2);
                }
            }
            else if (((TextBox)sender).Name.Contains("4"))
            {
                if (float.TryParse(txt_ColorDefect4MaxArea.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 3);
                }
            }
            else if (((TextBox)sender).Name.Contains("5"))
            {
                if (float.TryParse(txt_ColorDefect5MaxArea.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 4);
                }
            }

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ColorDefectTotalArea_Center_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            
            float fValue = 0;

            if (((TextBox)sender).Name.Contains("1"))
            {
                if (float.TryParse(txt_ColorDefect1TotalArea.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 0);
                }
            }
            else if (((TextBox)sender).Name.Contains("2"))
            {
                if (float.TryParse(txt_ColorDefect2TotalArea.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 1);
                }
            }
            else if (((TextBox)sender).Name.Contains("3"))
            {
                if (float.TryParse(txt_ColorDefect3TotalArea.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 2);
                }
            }
            else if (((TextBox)sender).Name.Contains("4"))
            {
                if (float.TryParse(txt_ColorDefect4TotalArea.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 3);
                }
            }
            else if (((TextBox)sender).Name.Contains("5"))
            {
                if (float.TryParse(txt_ColorDefect5TotalArea.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 4);
                }
            }

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_Color_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;
            
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.ColumnIndex == 1 || e.ColumnIndex == 5 || e.ColumnIndex == 7 || e.ColumnIndex == 9)
                return;

            float fValue = 0;

            switch (e.ColumnIndex)
            {
                case 2:
                    if (float.TryParse(dgd_Color.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                    {
                        //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, e.RowIndex);
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, dgd_Color.Rows[e.RowIndex].Cells[0].Value.ToString());
                    }
                    break;
                case 3:
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionFailCondition((dgd_Color.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell).Items.IndexOf(dgd_Color.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue), dgd_Color.Rows[e.RowIndex].Cells[0].Value.ToString());
                    break;
                case 4:
                    if (float.TryParse(dgd_Color.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                    {
                        //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, e.RowIndex);
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, dgd_Color.Rows[e.RowIndex].Cells[0].Value.ToString());
                    }
                    break;
                case 6:
                    if (float.TryParse(dgd_Color.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                    {
                        //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, e.RowIndex);
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, dgd_Color.Rows[e.RowIndex].Cells[0].Value.ToString());
                    }
                    break;
                case 8:
                    if (float.TryParse(dgd_Color.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                    {
                        //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, e.RowIndex);
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, dgd_Color.Rows[e.RowIndex].Cells[0].Value.ToString());
                    }
                    break;
                case 10:
                    if (float.TryParse(dgd_Color.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                    {
                        //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, e.RowIndex);
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, dgd_Color.Rows[e.RowIndex].Cells[0].Value.ToString());
                    }
                    break;
            }
            
            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void dgd_OCRSettings_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnChangeScoreSetting)
                return;

            if (e.RowIndex < 0)
                return;

            if (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.LightGray ||
               (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.LightGray))
                return;


            //Skip if col is result score
            if (e.ColumnIndex == 2 || e.ColumnIndex == 3 ||
                e.ColumnIndex == 5 || e.ColumnIndex == 6 ||
                e.ColumnIndex == 8 || e.ColumnIndex == 9 ||
                e.ColumnIndex == 10 ||
                e.ColumnIndex == 12 || e.ColumnIndex == 13)
                return;

            // Skip if cell value is ---
            if ((((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "---") ||
                (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "----") ||
                (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == ""))
                return;

            switch (e.ColumnIndex)
            {
                case 1:
                    {
                        int intCurrentSetValue = Convert.ToInt32(dgd_OCRSettings.Rows[e.RowIndex].Cells[1].Value);
                        SetCharsValueForm2 objSetCharValueForm = new SetCharsValueForm2(e.RowIndex + 1, intCurrentSetValue);
                        objSetCharValueForm.Location = new Point(769, 310);
                        if (objSetCharValueForm.ShowDialog() == DialogResult.OK)
                        {
                            RegistryKey Key = Registry.LocalMachine.OpenSubKey("Software", true);
                            RegistryKey subKey = Key.OpenSubKey("SVG\\Visions\\", true);
                            RegistryKey subKey2 = Key.OpenSubKey("SVG\\Visions\\" + m_smVisionInfo.g_strVisionFolderName, true);
                            bool blnSecureOnOff = Convert.ToBoolean(subKey.GetValue("SecureOnOff-MarkScore", false));
                            int intMinScoreSetting = Convert.ToInt32(subKey2.GetValue("Secure-MinMarkScoreSetting", -1));
                            if (intMinScoreSetting == -1)   // If parameter no exist
                            {
                                intMinScoreSetting = 30;    // Default 30%
                                // if registry does not have this parameter, user cannot know what parameter name they have to set. In order to make it user friendly, we write it to system so that user can find the parameter, then they can modified the value. 
                                subKey2.SetValue("Secure-MinMarkScoreSetting", intMinScoreSetting.ToString());
                            }

                            // 2021 01 25 - CCENG: Customer request mark score cannot lower than minimum mark score advance setting.
                            if (blnSecureOnOff && objSetCharValueForm.ref_intSetValue < intMinScoreSetting)
                            {
                                SRMMessageBox.Show("Mark Setting cannot lower than Minimum Mark Score [" + intMinScoreSetting.ToString() + "%].");

                            }
                            else
                            {
                                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                                {
                                    if (objSetCharValueForm.ref_blnSetAllRows)
                                    {
                                        //2020-05-08 ZJYEOH : To solve other template with more mark cannot be set
                                        if (m_blnWantSet1ToAll)
                                            m_smVisionInfo.g_arrMarks[u].SetOCRCharSetting(objSetCharValueForm.ref_intSetValue);
                                        else
                                        {
                                            for (int i = 0; i < dgd_OCRSettings.Rows.Count; i++)
                                                m_smVisionInfo.g_arrMarks[u].SetOCRCharSetting(i, objSetCharValueForm.ref_intSetValue);
                                        }
                                    }
                                    else
                                        m_smVisionInfo.g_arrMarks[u].SetOCRCharSetting(e.RowIndex, objSetCharValueForm.ref_intSetValue, m_blnWantSet1ToAll);
                                }
                            }
                        }

                        UpdateOCRSettingTable();
                    }
                    break;
            }
        }

        private void dgd_OCRSettings_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!m_blnChangeScoreSetting)
                return;

            if (e.ColumnIndex < 0)
                return;

            //Skip if col is result score
            if (e.ColumnIndex == 2 || e.ColumnIndex == 3 ||
                e.ColumnIndex == 5 || e.ColumnIndex == 6 ||
                e.ColumnIndex == 8 || e.ColumnIndex == 9 ||
                e.ColumnIndex == 10 ||
                e.ColumnIndex == 12 || e.ColumnIndex == 13)
                return;

            switch (e.ColumnIndex)
            {
                case 1:
                    {
                        int intCurrentSetValue;
                        if (dgd_OCRSettings.Rows.Count > 0)
                            intCurrentSetValue = Convert.ToInt32(dgd_OCRSettings.Rows[0].Cells[1].Value);
                        else
                            intCurrentSetValue = 75;

                        SetCharsValueForm2 objSetCharValueForm = new SetCharsValueForm2(-1, intCurrentSetValue);
                        objSetCharValueForm.Location = new Point(769, 310);
                        if (objSetCharValueForm.ShowDialog() == DialogResult.OK)
                        {
                            RegistryKey Key = Registry.LocalMachine.OpenSubKey("Software", true);
                            RegistryKey subKey = Key.OpenSubKey("SVG\\Visions\\", true);
                            RegistryKey subKey2 = Key.OpenSubKey("SVG\\Visions\\" + m_smVisionInfo.g_strVisionFolderName, true);
                            bool blnSecureOnOff = Convert.ToBoolean(subKey.GetValue("SecureOnOff-MarkScore", false));
                            int intMinScoreSetting = Convert.ToInt32(subKey2.GetValue("Secure-MinMarkScoreSetting", -1));
                            if (intMinScoreSetting == -1)   // If parameter no exist
                            {
                                intMinScoreSetting = 30;    // Default 30%
                                // if registry does not have this parameter, user cannot know what parameter name they have to set. In order to make it user friendly, we write it to system so that user can find the parameter, then they can modified the value. 
                                subKey2.SetValue("Secure-MinMarkScoreSetting", intMinScoreSetting.ToString());
                            }

                            // 2021 01 25 - CCENG: Customer request mark score cannot lower than minimum mark score advance setting.
                            if (blnSecureOnOff && objSetCharValueForm.ref_intSetValue < intMinScoreSetting)
                            {
                                SRMMessageBox.Show("Mark Setting cannot lower than Minimum Mark Score [" + intMinScoreSetting.ToString() + "%].");

                            }
                            else
                            {
                                // 2021 01 25 - CCENG: This code has bug. Will change the dont care mark to inspect mark.
                                //for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                                //    for (int i = 0; i < dgd_Setting.Rows.Count; i++)
                                //        m_smVisionInfo.g_arrMarks[u].SetCharSetting(i, objSetCharValueForm.ref_intSetValue, true, m_blnWantSet1ToAll);

                                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                                {
                                    if (objSetCharValueForm.ref_blnSetAllRows)
                                    {
                                        //2020-05-08 ZJYEOH : To solve other template with more mark cannot be set
                                        if (m_blnWantSet1ToAll)
                                            m_smVisionInfo.g_arrMarks[u].SetOCRCharSetting(objSetCharValueForm.ref_intSetValue);
                                        else
                                        {
                                            for (int i = 0; i < dgd_OCRSettings.Rows.Count; i++)
                                                m_smVisionInfo.g_arrMarks[u].SetOCRCharSetting(i, objSetCharValueForm.ref_intSetValue);
                                        }
                                    }
                                    else
                                        m_smVisionInfo.g_arrMarks[u].SetOCRCharSetting(e.RowIndex, objSetCharValueForm.ref_intSetValue, m_blnWantSet1ToAll);
                                }
                            }
                        }

                        UpdateOCRSettingTable();
                    }
                    break;
            }
        }

        private void txt_ChippedVerticalBright_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 1;
            m_smVisionInfo.g_intSelectedPackageLengthType = 1;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam(1, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

        }

        private void txt_ChippedVerticalDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 6;
            m_smVisionInfo.g_intSelectedPackageLengthType = 1;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam((int)Package.eDefect.ChipDark, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        }

        private void txt_ChippedHorizontalBright_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 1;
            m_smVisionInfo.g_intSelectedPackageLengthType = 0;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam((int)Package.eDefect.ChipBright, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);
        }

        private void txt_ChippedHorizontalDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 6;
            m_smVisionInfo.g_intSelectedPackageLengthType = 0;
            m_smVisionInfo.g_blnViewPackageAreaDefect = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_fValuePrev = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam((int)Package.eDefect.ChipDark, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);
        }

        private void txt_ChippedAreaBright_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 1;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChippedAreaDark_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedPackageDefect = 6;
            m_smVisionInfo.g_blnViewPackageAreaDefect = true;
            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChippedVerticalBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_ChippedVerticalBright.Text, out fValue))
                {
                    txt_ChippedVerticalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_ChippedVerticalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_ChippedVerticalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_ChippedVerticalDark.Text, out fValue))
                {
                    txt_ChippedVerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.ChipDark, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_ChippedVerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_ChippedHorizontalBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_ChippedHorizontalBright.Text, out fValue))
                {
                    txt_ChippedHorizontalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.ChipBright, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_ChippedHorizontalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void txt_ChippedHorizontalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_ChippedHorizontalDark.Text, out fValue))
                {
                    txt_ChippedHorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.ChipDark, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_ChippedHorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //UpdatePkgSettingGUI();
        }

        private void cbo_ChippedBrightFailCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intChippedBrightDefectDimensionFailCondition = cbo_ChippedBrightFailCondition.SelectedIndex;
            }
        }

        private void cbo_ChippedDarkFailCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intChippedDarkDefectDimensionFailCondition = cbo_ChippedDarkFailCondition.SelectedIndex;
            }
        }

        private void txt_Dark3VerticalDark_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_Dark3VerticalDark.Text, out fValue))
                {
                    txt_Dark3VerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    float fValuePrev_Width = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.Dark3, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);

                    if (fValue < fValuePrev_Width)
                    {
                        txt_Dark3VerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Length cannot smaller than width!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Dark3, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_Dark3VerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark4VerticalDark_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;

                if (!float.TryParse(txt_Dark4VerticalDark.Text, out fValue))
                {
                    txt_Dark4VerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    float fValuePrev_Width = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.Dark4, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);

                    if (fValue < fValuePrev_Width)
                    {
                        txt_Dark4VerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Length cannot smaller than width!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Dark4, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_Dark4VerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark2HorizontalDark_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_Dark2HorizontalDark.Text, out fValue))
                {
                    txt_Dark2HorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    float fValuePrev_Length = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.Dark2, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

                    if (fValue > fValuePrev_Length)
                    {
                        txt_Dark2HorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Width cannot bigger than length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Dark2, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_Dark2HorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark3HorizontalDark_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_Dark3HorizontalDark.Text, out fValue))
                {
                    txt_Dark3HorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    float fValuePrev_Length = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.Dark3, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

                    if (fValue > fValuePrev_Length)
                    {
                        txt_Dark3HorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Width cannot bigger than length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Dark3, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_Dark3HorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark4HorizontalDark_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_Dark4HorizontalDark.Text, out fValue))
                {
                    txt_Dark4HorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    float fValuePrev_Length = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.Dark4, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

                    if (fValue > fValuePrev_Length)
                    {
                        txt_Dark4HorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Width cannot bigger than length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Dark4, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_Dark4HorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralVerticalBright_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_GeneralVerticalBright.Text, out fValue))
                {
                    txt_GeneralVerticalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    float fValuePrev_Width = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.Bright, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);

                    if (fValue < fValuePrev_Width)
                    {
                        txt_GeneralVerticalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Length cannot smaller than width!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Bright, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_GeneralVerticalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void txt_GeneralHorizontalBright_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_GeneralHorizontalBright.Text, out fValue))
                {
                    txt_GeneralHorizontalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    float fValuePrev_Length = (float)Math.Round(m_smVisionInfo.g_arrPackage[0].GetDefectParam((int)Package.eDefect.Bright, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

                    if (fValue > fValuePrev_Length)
                    {
                        txt_GeneralHorizontalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Width cannot bigger than length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Bright, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_GeneralHorizontalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralHorizontalDark_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_GeneralHorizontalDark.Text, out fValue))
                {
                    txt_GeneralHorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    float fValuePrev_Length = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.Dark, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

                    if (fValue > fValuePrev_Length)
                    {
                        txt_GeneralHorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Width cannot bigger than length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Dark, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_GeneralHorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CrackHorizontalDark_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_CrackHorizontalDark.Text, out fValue))
                {
                    txt_CrackHorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    float fValuePrev_Length = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.Crack, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

                    if (fValue > fValuePrev_Length)
                    {
                        txt_CrackHorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Width cannot bigger than length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Crack, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_CrackHorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChippedHorizontalBright_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_ChippedHorizontalBright.Text, out fValue))
                {
                    txt_ChippedHorizontalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    float fValuePrev_Length = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.ChipBright, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

                    if (fValue > fValuePrev_Length)
                    {
                        txt_ChippedHorizontalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Width cannot bigger than length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.ChipBright, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_ChippedHorizontalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChippedHorizontalDark_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_ChippedHorizontalDark.Text, out fValue))
                {
                    txt_ChippedHorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    float fValuePrev_Length = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.ChipDark, 1) / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

                    if (fValue > fValuePrev_Length)
                    {
                        txt_ChippedHorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Width cannot bigger than length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.ChipDark, fValue * m_smVisionInfo.g_fCalibPixelX, 0))
                    {
                        txt_ChippedHorizontalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralVerticalDark_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_GeneralVerticalDark.Text, out fValue))
                {
                    txt_GeneralVerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    float fValuePrev_Width = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.Dark, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);

                    if (fValue < fValuePrev_Width)
                    {
                        txt_GeneralVerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Length cannot smaller than width!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Dark, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_GeneralVerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CrackVerticalDark_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_CrackVerticalDark.Text, out fValue))
                {
                    txt_CrackVerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    float fValuePrev_Width = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.Crack, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);

                    if (fValue < fValuePrev_Width)
                    {
                        txt_CrackVerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Length cannot smaller than width!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Crack, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_CrackVerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChippedVerticalBright_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_ChippedVerticalBright.Text, out fValue))
                {
                    txt_ChippedVerticalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    float fValuePrev_Width = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.ChipBright, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);

                    if (fValue < fValuePrev_Width)
                    {
                        txt_ChippedVerticalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Length cannot smaller than width!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.ChipBright, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_ChippedVerticalBright.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChippedVerticalDark_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_ChippedVerticalDark.Text, out fValue))
                {
                    txt_ChippedVerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    float fValuePrev_Width = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.ChipDark, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);

                    if (fValue < fValuePrev_Width)
                    {
                        txt_ChippedVerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Length cannot smaller than width!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.ChipDark, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_ChippedVerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark2VerticalDark_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                float fValue;
                if (!float.TryParse(txt_Dark2VerticalDark.Text, out fValue))
                {
                    txt_Dark2VerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    float fValuePrev_Width = (float)Math.Round(m_smVisionInfo.g_arrPackage[u].GetDefectParam((int)Package.eDefect.Dark2, 0) / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero);

                    if (fValue < fValuePrev_Width)
                    {
                        txt_Dark2VerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Length cannot smaller than width!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!m_smVisionInfo.g_arrPackage[u].SetDefectParam((int)Package.eDefect.Dark2, fValue * m_smVisionInfo.g_fCalibPixelY, 1))
                    {
                        txt_Dark2VerticalDark.Text = m_fValuePrev.ToString("F" + m_intDecimal);
                        SRMMessageBox.Show("Defect length cannot larger than unit length!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPackageTotalAreaDefect = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
    }
}