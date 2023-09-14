using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Common;
using ImageAcquisition;
using VisionProcessing;
using SharedMemory;
using Microsoft.Win32;

namespace VisionProcessForm
{
    public partial class SealOtherSettingForm : Form
    {

        #region Member Variables
        
        private int m_intUserGroup = 5;
        private bool m_blnInitDone = false;
        
        private string m_strSelectedRecipe;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo m_smVisionInfo;
        private UserRight m_objUserRight = new UserRight();
        private bool m_blnEnterTextBox = false;
        private AdvancedSealCircleGaugeForm m_objAdvancedCircleGaugeForm;
        private AdvancedPostSealLGaugeForm m_objAdvancedForm;
        #endregion

        public SealOtherSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup)
        {
            InitializeComponent();

            m_intUserGroup = intUserGroup;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = visionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            
            DisableField2();
            UpdateGUI();
            UpdateMarkTemplateGUI();
            m_blnInitDone = true;
        }



        /// <summary>
        /// Customize GUI based on user access level
        /// </summary>
        private void DisableField()
        {
            string strChild1 = "Test Page";


            string strChild2 = "Tolerance Setting Page";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                
                tab_VisionControl.TabPages.Remove(tabPage_Setting);
            }

            strChild1 = "Setting Page";
            strChild2 = "Save Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
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

        /// <summary>
        /// Customize GUI based on user access level
        /// </summary>
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild2 = "Setting Page";
            string strChild3 = "";

            strChild3 = "Seal 1 Threshold Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_Seal1Threshold.Enabled = false;
                btn_Seal1Threshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Seal 2 Threshold Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_Seal2Threshold.Enabled = false;
                btn_Seal2Threshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                if (!btn_Seal1Threshold.Enabled)
                {
                    gb_SealThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }
            }

            strChild3 = "Seal 1 Min Seal Object Size";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                txt_Seal1AreaFilter.Enabled = false;
                txt_Seal1AreaFilter.Visible = srmLabel7.Visible = srmLabel8.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Seal 2 Min Seal Object Size";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                txt_Seal2AreaFilter.Enabled = false;
                txt_Seal2AreaFilter.Visible = srmLabel58.Visible = lbl_MinArea.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                if (!txt_Seal1AreaFilter.Enabled)
                {
                    pnl_MinSealObjectSize.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }
            }

            strChild3 = "Seal 1 Broken Threshold Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_Seal1BrokenAreaThreshold.Enabled = false;
                btn_Seal1BrokenAreaThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Seal 2 Broken Threshold Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_Seal2BrokenAreaThreshold.Enabled = false;
                btn_Seal2BrokenAreaThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                if (!btn_Seal1BrokenAreaThreshold.Enabled)
                {
                    gb_SealBrokenAreaThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }
            }

            strChild3 = "Over Heat Threshold Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_OverHeatThreshold.Enabled = false;
                gb_OverHeatThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Distance Threshold Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_DistanceThreshold.Enabled = false;
                gb_DistanceThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Unit Present Threshold Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_UnitPresentThreshold.Enabled = false;
                cbo_MarkTemplateNo.Enabled = false;
                gb_UnitPresentThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Seal Sprocket Hole Defect Threshold Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_SprocketHoleDefectThreshold.Enabled = false;
                grpbox_SprocketHoleDefectSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Seal Sprocket Hole Broken Threshold Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_SprocketHoleBrokenThreshold.Enabled = false;
                grpbox_SprocketHoleBrokenSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Seal Sprocket Hole Roundness Threshold Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_SprocketHoleRoundnessThreshold.Enabled = false;
                grpbox_SprocketHoleRoundnessSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Seal Edge Straightness Threshold Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_SealEdgeStraightnessThreshold.Enabled = false;
                gb_SealEdgeStraightnessThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Seal Edge Sensitivity";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                txt_SealEdgeSensitivity.Enabled = false;
                pnl_SealEdgeSensitivity.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Seal Config Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_SealConfig.Enabled = false;
                pnl_SealConfiguration.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Seal Gauge Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_GaugeAdvanceSetting.Enabled = false;
                btn_CircleGaugeSetting.Enabled = false;
                pnl_SealGauge.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_CircleGauge.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Seal Edge Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                txt_SealEdgeTolerance.Enabled = false;
                pnl_SealEdgeTolerance.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Seal Sprocket Hole Inspection Area Inward Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                txt_SprocketHoleInspectionAreaInwardTolerance.Enabled = false;
                pnl_SprocketHoleInspectionAreaInwardTolerance.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Seal Sprocket Hole Broken Inspection Area Outward Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                grpbox_SprocketHoleBrokenInspectionAreaTolerance.Enabled = false;
                grpbox_SprocketHoleBrokenInspectionAreaTolerance.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }
        }

        /// <summary>
        /// Customize GUI
        /// </summary>
        private void UpdateGUI()
        {
            chk_ShowDraggingBox_Circle.Checked = m_smVisionInfo.g_objSealCircleGauges.ref_blnDrawDraggingBox;
            chk_ShowSamplePoints_Circle.Checked = m_smVisionInfo.g_objSealCircleGauges.ref_blnDrawSamplingPoint;
            txt_Seal1AreaFilter.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_intSeal1AreaFilter / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero).ToString("f3");
            txt_Seal2AreaFilter.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_intSeal2AreaFilter / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero).ToString("f3");
            //2021-04-06 ZJYEOH : All unused GUI no need to init value
            //txt_OverHeatMinArea.Text = m_smVisionInfo.g_objSeal.ref_fOverHeatAreaMinTolerance.ToString(); //Math.Round(m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero).ToString("f3");   // 2020 01 07 - CCENG: Convert from Integer to float will cause missing value.
            //txt_MinBrokenArea.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_intHoleMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_ShiftTolerance.Text = Math.Round((m_smVisionInfo.g_objSeal.ref_fShiftPositionTolerance / m_smVisionInfo.g_fCalibPixelY),3,MidpointRounding.AwayFromZero).ToString("f3");

            //txt_Seal1AreaTolerance.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_arrSealAreaTolerance[0] / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_Seal2AreaTolerance.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_arrSealAreaTolerance[1] / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero).ToString("f3");

            txt_SealEdgeSensitivity.Text = m_smVisionInfo.g_objSeal.ref_intSealEdgeSensitivity.ToString();
            txt_SealEdgeTolerance.Text = m_smVisionInfo.g_objSeal.ref_intSealEdgeTolerance.ToString();
            txt_SprocketHoleInspectionAreaInwardTolerance.Text = m_smVisionInfo.g_objSeal.ref_intSprocketHoleInspectionAreaInwardTolerance.ToString();
            txt_SprocketHoleBrokenOutwardTolerance_Outer.Text = m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer.ToString();
            txt_SprocketHoleBrokenOutwardTolerance_Inner.Text = m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner.ToString();
            pnl_SprocketHoleInspectionAreaInwardTolerance.Visible = !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect;
            pnl_SprocketHoleBrokenInspectionAreaTolerance.Visible = !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness;
            // chk_CheckEmpty.Checked = m_smVisionInfo.MN_PR_CheckEmptyUnit;

            if (m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole && m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect && m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
            {
                pnl_CircleGauge.Visible = false;
            }

            if (m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
                grpbox_SprocketHoleDefectSetting.Visible = false;

            if (m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
            {
                grpbox_SprocketHoleBrokenSetting.Visible = false;
                grpbox_SprocketHoleRoundnessSetting.Visible = false;
            }

            if (!m_smVisionInfo.g_objSeal.ref_blnWantCheckSealEdgeStraightness)
            {
                gb_SealEdgeStraightnessThreshold.Visible = false;
                if (tab_VisionControl.TabPages.Contains(tabPage_Setting2))
                    tab_VisionControl.TabPages.Remove(tabPage_Setting2);
            }

            cbo_OverHeatROINo.Items.Clear();
            cbo_OverHeatROINo.Items.Add("ROI 1");
            for (int i = 1; i < m_smVisionInfo.g_arrSealROIs[4].Count; i++)
            {
                cbo_OverHeatROINo.Items.Add("ROI " + (i + 1).ToString());
            }
            m_smVisionInfo.g_intSelectedROI = cbo_OverHeatROINo.SelectedIndex = 0;

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");

            m_smVisionInfo.g_blnDrawThresholdAllSideROI = chk_SetToAllOverHeatROI.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllOverHeatROI_SealOtherSetting", false));

            if (m_smVisionInfo.g_arrSealROIs.Count <= 4 || m_smVisionInfo.g_arrSealROIs[4].Count == 1)
            {
                cbo_OverHeatROINo.Visible = false;
                chk_SetToAllOverHeatROI.Visible = false;
                m_smVisionInfo.g_blnDrawThresholdAllSideROI = chk_SetToAllOverHeatROI.Checked = false;
            }
        }



        private void SaveGeneralSetting(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "General.xml", false);
            
            STDeviceEdit.CopySettingFile(strFolderPath, "General.xml");
            objFile.WriteSectionElement("TemplateCounting");
            objFile.WriteElement1Value("TotalPocketTemplates", m_smVisionInfo.g_intPocketTemplateTotal);
            objFile.WriteElement1Value("TotalMarkTemplates", m_smVisionInfo.g_intMarkTemplateTotal);
            objFile.WriteElement1Value("PocketTemplateMask", m_smVisionInfo.g_intPocketTemplateMask);
            objFile.WriteElement1Value("MarkTemplateMask", m_smVisionInfo.g_intMarkTemplateMask);

            objFile.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Seal Other Setting", m_smProductionInfo.g_strLotID);
            
        }

        private void LoadGeneralSetting(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "General.xml", false);
            objFile.GetFirstSection("TemplateCounting");
            m_smVisionInfo.g_intPocketTemplateTotal = objFile.GetValueAsInt("TotalPocketTemplates", 0, 1);
            m_smVisionInfo.g_intMarkTemplateTotal = objFile.GetValueAsInt("TotalMarkTemplates", 0, 1);
            m_smVisionInfo.g_intPocketTemplateMask = objFile.GetValueAsInt("PocketTemplateMask", 0, 1);
            m_smVisionInfo.g_intMarkTemplateMask = objFile.GetValueAsInt("MarkTemplateMask", 0, 1);
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\";

            m_smVisionInfo.g_objSeal.LoadSeal(strPath + "Seal\\Settings.xml", "Settings", m_smVisionInfo.g_fCalibPixelX);
            LoadGeneralSetting(strPath);
            Close();
            Dispose();
          
        }

     

        private void btn_Save_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\";

            
            STDeviceEdit.CopySettingFile(strPath, "Seal\\Settings.xml");
            m_smVisionInfo.g_objSeal.SaveSeal(strPath + "Seal\\Settings.xml", false, "Settings", false, m_smVisionInfo.g_fCalibPixelX);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Seal Other Setting", m_smProductionInfo.g_strLotID);
            
            SaveGeneralSetting(strPath);

            m_smVisionInfo.g_objSeal.LoadMarkPatternImage(strPath + "Seal\\Template\\Mark\\");

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            Close();
            Dispose();
        }

        private void btn_NearThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedUnit = 2;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(2);

            if (m_smVisionInfo.g_objPackageImage == null)
                m_smVisionInfo.g_objPackageImage = new ImageDrawing(true);

            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            m_smVisionInfo.g_blnViewPackageImage = true;
            ImageDrawing.CloseBoxImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objSeal.ref_intSealEdgeSensitivity);

            m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPackageImage);
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intSeal2Threshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_objSeal.ref_intSeal2Threshold = m_smVisionInfo.g_intThresholdValue;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.g_blnViewPackageImage = false;
        }

        private void btn_FarThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedUnit = 1;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(2);

            if (m_smVisionInfo.g_objPackageImage == null)
                m_smVisionInfo.g_objPackageImage = new ImageDrawing(true);

            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            m_smVisionInfo.g_blnViewPackageImage = true;
            ImageDrawing.CloseBoxImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objSeal.ref_intSealEdgeSensitivity);

            m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPackageImage);

            //m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intSeal1Threshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_objSeal.ref_intSeal1Threshold = m_smVisionInfo.g_intThresholdValue;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.g_blnViewPackageImage = false;
        }



        private void tab_VisionControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tab_VisionControl.SelectedTab.Name)
            {
                case "tabPage_ScoreSetting":
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    //if (m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole)
                    //    m_smVisionInfo.g_blnViewGauge = false;
                    //else
                    //    m_smVisionInfo.g_blnViewGauge = true;
                    break;
                case "tabPage_Setting":
                case "tabPage_Setting2":
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    m_smVisionInfo.g_blnViewGauge = false;
                    break;
                default:
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = true;
                    break;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }



        private void txt_LeftMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_Seal2AreaFilter.Text == "" || txt_Seal2AreaFilter.Text == null)
                return;

            m_smVisionInfo.g_objSeal.ref_intSeal2AreaFilter = Convert.ToInt32(Convert.ToSingle(txt_Seal2AreaFilter.Text) * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);
            //m_smVisionInfo.g_objSeal.ref_intTemplateAreaAVG[1] = Convert.ToInt32(Convert.ToSingle(txt_Seal2MinArea.Text) * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_RightMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_Seal1AreaFilter.Text == "" || txt_Seal1AreaFilter.Text == null)
                return;

            m_smVisionInfo.g_objSeal.ref_intSeal1AreaFilter = Convert.ToInt32(Convert.ToSingle(txt_Seal1AreaFilter.Text) * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);
            //m_smVisionInfo.g_objSeal.ref_intTemplateAreaAVG[0] = Convert.ToInt32(Convert.ToSingle(txt_Seal1MinArea.Text) * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_OverHeatMinArea_TextChanged(object sender, EventArgs e)
        {
            //if (!m_blnInitDone || txt_OverHeatMinArea.Text == "" || txt_OverHeatMinArea.Text == null)
            //    return;

            ////m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea = Convert.ToInt32(Convert.ToSingle(txt_OverHeatMinArea.Text) * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);
            //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea = (int)Math.Floor((Convert.ToSingle(txt_OverHeatMinArea.Text) * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
            //m_smVisionInfo.g_objSeal.ref_fOverHeatAreaMinTolerance = Convert.ToSingle(txt_OverHeatMinArea.Text);

            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        //private void txt_MinBrokenArea_TextChanged(object sender, EventArgs e)
        //{
        //    if (!m_blnInitDone || txt_MinBrokenArea.Text == "" || txt_MinBrokenArea.Text == null)
        //        return;

        //    m_smVisionInfo.g_objSeal.ref_intHoleMinArea = Convert.ToInt32(float.Parse(txt_MinBrokenArea.Text) * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);
        //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        //}

        //private void txt_ShiftTolerance_TextChanged(object sender, EventArgs e)
        //{
        //    if (!m_blnInitDone || txt_ShiftTolerance.Text == "" || txt_ShiftTolerance.Text == null)
        //        return;

        //    m_smVisionInfo.g_objSeal.ref_fShiftPositionTolerance = Convert.ToInt32(float.Parse(txt_ShiftTolerance.Text) * m_smVisionInfo.g_fCalibPixelY);
        //}

        private void SealOtherSettingForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blncboImageView = false;
            //Cursor.Current = Cursors.Default;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            //m_smVisionInfo.g_blnViewROI = true;
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
        }

        private void SealOtherSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal Other Setting Form Closed", "Exit Seal Setting Form", "", "", m_smProductionInfo.g_strLotID);
            m_smVisionInfo.g_blnDrawThresholdAllSideROI = false;
            m_smVisionInfo.g_objSeal.ClearBlobData();
            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewROI = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
        }

        
      

        private void btn_OverHeatThreshold_Click(object sender, EventArgs e)
        {
            //m_smVisionInfo.g_intSelectedUnit = 4;
            //m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intOverHeatThreshold;

            //ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrSealROIs[4][0]);
            //Rectangle resolution = Screen.PrimaryScreen.Bounds;
            //objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            ////objThresholdForm.Location = new Point(769, 310);
            //if (objThresholdForm.ShowDialog() == DialogResult.OK)
            //{
            //    m_smVisionInfo.g_objSeal.ref_intOverHeatThreshold = m_smVisionInfo.g_intThresholdValue;
            //}
            //objThresholdForm.Dispose();

            m_smVisionInfo.g_intSelectedUnit = 4;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(4);
            m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedROI].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_objSeal.GetOverHeatLowThreshold(m_smVisionInfo.g_intSelectedROI);
            m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_objSeal.GetOverHeatHighThreshold(m_smVisionInfo.g_intSelectedROI);

            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrSealROIs[4][m_smVisionInfo.g_intSelectedROI]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (chk_SetToAllOverHeatROI.Checked)
                {
                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                    {
                        m_smVisionInfo.g_objSeal.SetScratchesLowThreshold(j, m_smVisionInfo.g_intLowThresholdValue);
                        m_smVisionInfo.g_objSeal.SetOverHeatLowThreshold(j, m_smVisionInfo.g_intLowThresholdValue);
                        m_smVisionInfo.g_objSeal.SetOverHeatHighThreshold(j, m_smVisionInfo.g_intHighThresholdValue);
                    }
                }
                else
                {
                    m_smVisionInfo.g_objSeal.SetScratchesLowThreshold(m_smVisionInfo.g_intSelectedROI, m_smVisionInfo.g_intLowThresholdValue);
                    m_smVisionInfo.g_objSeal.SetOverHeatLowThreshold(m_smVisionInfo.g_intSelectedROI, m_smVisionInfo.g_intLowThresholdValue);
                    m_smVisionInfo.g_objSeal.SetOverHeatHighThreshold(m_smVisionInfo.g_intSelectedROI, m_smVisionInfo.g_intHighThresholdValue);
                }
            }
            objThresholdForm.Dispose();
        }


        private void btn_SealConfig_Click(object sender, EventArgs e)
        {
            SealConfiguration objSealConfig = new SealConfiguration(
                                                    m_smVisionInfo.g_intPocketTemplateTotal,
                                                    m_smVisionInfo.g_intMarkTemplateTotal,
                                                    m_smVisionInfo.g_intPocketTemplateMask,
                                                    m_smVisionInfo.g_intMarkTemplateMask,
                                                    m_smVisionInfo.g_strVisionFolderName,
                                                    m_smProductionInfo.g_strRecipePath,
                                                    m_strSelectedRecipe);

            if (objSealConfig.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_intPocketTemplateMask = objSealConfig.ref_intPocketTemplateMask;
                m_smVisionInfo.g_intMarkTemplateMask = objSealConfig.ref_intMarkTemplateMask;
            }

            objSealConfig.Dispose();
        }

   
        //private void chk_CheckEmpty_Click(object sender, EventArgs e)
        //{
        //    m_smVisionInfo.MN_PR_CheckEmptyUnit = chk_CheckEmpty.Checked;
        //}

        private void txt_Seal1AreaTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_Seal1AreaTolerance.Text == "" || txt_Seal1AreaTolerance.Text == null)
                return;

            m_smVisionInfo.g_objSeal.ref_arrSealAreaTolerance[0] = Convert.ToSingle(txt_Seal1AreaTolerance.Text) * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY;
        }

        private void txt_Seal2AreaTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_Seal2AreaTolerance.Text == "" || txt_Seal2AreaTolerance.Text == null)
                return;

            m_smVisionInfo.g_objSeal.ref_arrSealAreaTolerance[1] = Convert.ToSingle(txt_Seal2AreaTolerance.Text) * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY;
        }

        private void txt_Seal1AreaFilter_Enter(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_blnViewDimension = true;
            m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 2;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Seal2AreaFilter_Enter(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_blnViewDimension = true;
            m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 3;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_OverHeatMinArea_Enter(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            // 2020 08 13 - CCENG: Hide it bcos dont want to draw seal setting dimension when enter form. Dont know why this txt_OverHeatMinArea_Enter is triggered when enter this form.
            //m_smVisionInfo.g_blnViewDimension = true;
            //m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 4;
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Seal1AreaFilter_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewDimension = false;
            m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = -1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Seal2AreaFilter_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewDimension = false;
            m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = -1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_OverHeatMinArea_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewDimension = false;
            m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = -1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void btn_DistanceThreshold_Click(object sender, EventArgs e)
        {
            STTrackLog.WriteLine("DistanceThreshold Click A. ref_intDistanceThreshold = " + m_smVisionInfo.g_objSeal.ref_intDistanceThreshold.ToString());
            m_smVisionInfo.g_intSelectedUnit = 3;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(3);
            m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intDistanceThreshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrSealROIs[3][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                STTrackLog.WriteLine("DistanceThreshold Click B. ref_intDistanceThreshold = " + m_smVisionInfo.g_objSeal.ref_intDistanceThreshold.ToString());
                m_smVisionInfo.g_objSeal.ref_intDistanceThreshold = m_smVisionInfo.g_intThresholdValue;
            }

            STTrackLog.WriteLine("DistanceThreshold Click C. ref_intDistanceThreshold = " + m_smVisionInfo.g_objSeal.ref_intDistanceThreshold.ToString());
            objThresholdForm.Dispose();
        }

        private void btn_TapeScratchesThreshold_Click(object sender, EventArgs e)
        {

        }

        private void btn_UnitPresentThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedUnit = 5;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(5);
            m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.GetMarkTemplateThreshold();//ref_intMarkPixelThreshold;
            m_smVisionInfo.g_fThresholdRelativeValue = m_smVisionInfo.g_objSeal.GetMarkTemplateThresholdRelative();
            m_smVisionInfo.g_arrSealROIs[5][2].LoadROISetting(m_smVisionInfo.g_objSeal.GetTemplateMarkROIPosition().X, m_smVisionInfo.g_objSeal.GetTemplateMarkROIPosition().Y,
                m_smVisionInfo.g_objSeal.GetTemplateMarkROISize().Width, m_smVisionInfo.g_objSeal.GetTemplateMarkROISize().Height);
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, true, m_smVisionInfo.g_objSeal.GetTemplateWantAutoThresholdRelative(), m_smVisionInfo.g_arrSealROIs[5][2]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                //m_smVisionInfo.g_objSeal.ref_intMarkPixelThreshold = m_smVisionInfo.g_intThresholdValue;
                m_smVisionInfo.g_objSeal.SetTemplateWantAutoThresholdRelative(objThresholdForm.ref_blnWantAutoThreshold);
                m_smVisionInfo.g_objSeal.SetMarkTemplateThreshold(m_smVisionInfo.g_intThresholdValue);
                m_smVisionInfo.g_objSeal.SetMarkTemplateThresholdRelative(m_smVisionInfo.g_fThresholdRelativeValue);
            }
            objThresholdForm.Dispose();
        }
        private void UpdateMarkTemplateGUI()
        {
            cbo_MarkTemplateNo.Items.Clear();

            // Update Template GUI
            for (int i = 0; i < m_smVisionInfo.g_intMarkTemplateTotal; i++)
            {
                cbo_MarkTemplateNo.Items.Add("Template " + (i + 1));
            }

            if (m_smVisionInfo.g_intMarkTemplateTotal == 0)
                cbo_MarkTemplateNo.Items.Add("Template 1");

            m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex = cbo_MarkTemplateNo.SelectedIndex = 0;
            if (m_smVisionInfo.g_arrSealROIs[5].Count > 2)
            {
                m_smVisionInfo.g_arrSealROIs[5][2].AttachImage(m_smVisionInfo.g_arrSealROIs[5][0]);
                m_smVisionInfo.g_arrSealROIs[5][2].LoadROISetting(m_smVisionInfo.g_objSeal.GetTemplateMarkROIPosition().X, m_smVisionInfo.g_objSeal.GetTemplateMarkROIPosition().Y,
                    m_smVisionInfo.g_objSeal.GetTemplateMarkROISize().Width, m_smVisionInfo.g_objSeal.GetTemplateMarkROISize().Height);
            }
        }
        private void btn_CircleGaugeSetting_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_objSealCircleGauges == null)
            {
                SRMMessageBox.Show("Please learn seal sprocket hole first!", "Error", MessageBoxButtons.OK);
            }
            else
            {
                m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);

                string strPath = m_smProductionInfo.g_strRecipePath +
                             m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Seal\\CircleGauge.xml";

                m_objAdvancedCircleGaugeForm = new AdvancedSealCircleGaugeForm(m_smVisionInfo, strPath, m_smProductionInfo.g_strRecipePath +
                                    m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName, m_smProductionInfo, m_smCustomizeInfo);

                m_objAdvancedCircleGaugeForm.Show();

                m_smVisionInfo.g_blnViewGauge = true;
                m_smVisionInfo.g_intGaugeDisplayIndex = 1;

                btn_Close.Enabled = btn_Save.Enabled = btn_SealConfig.Enabled = btn_CircleGaugeSetting.Enabled = btn_GaugeAdvanceSetting.Enabled = false;

                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
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

            if (m_objAdvancedCircleGaugeForm != null)
            {
                if (m_objAdvancedCircleGaugeForm.ref_blnShowForm)
                {
                    if (m_objAdvancedCircleGaugeForm.ref_blnMeasureGauge)
                    {
                        SetCircleGaugePlace();
                        m_objAdvancedCircleGaugeForm.ref_blnMeasureGauge = false;
                        m_smVisionInfo.g_objSealCircleGauges.Measure(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    }

                    tab_VisionControl.SelectedTab = tabPage_ScoreSetting;
                }
                else
                {
                    SetCircleGaugePlace();
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                    m_objAdvancedCircleGaugeForm.Close();
                    m_objAdvancedCircleGaugeForm.Dispose();
                    m_objAdvancedCircleGaugeForm = null;

                    btn_Close.Enabled = btn_Save.Enabled = btn_SealConfig.Enabled = btn_CircleGaugeSetting.Enabled = btn_GaugeAdvanceSetting.Enabled = chk_ShowDraggingBox_Circle.Enabled = chk_ShowSamplePoints_Circle.Enabled = chk_ShowDraggingBox_Line.Enabled = chk_ShowSamplePoints_Line.Enabled = true;
                }
            }

            if (m_objAdvancedForm != null)
            {
                if (m_objAdvancedForm.ref_blnShowForm)
                {
                    if (m_objAdvancedForm.ref_blnBuildGauge)
                    {
                        m_smVisionInfo.g_objSeal.BuildGauge(m_smVisionInfo.g_arrSealROIs, m_smVisionInfo.g_arrSealGauges, m_smVisionInfo.g_fCalibPixelY, true);
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    }

                    tab_VisionControl.SelectedTab = tabPage_ScoreSetting;
                }
                else
                {
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                    m_objAdvancedForm.Close();
                    m_objAdvancedForm.Dispose();
                    m_objAdvancedForm = null;

                    btn_Close.Enabled = btn_Save.Enabled = btn_SealConfig.Enabled = btn_CircleGaugeSetting.Enabled = btn_GaugeAdvanceSetting.Enabled = chk_ShowDraggingBox_Circle.Enabled = chk_ShowSamplePoints_Circle.Enabled = chk_ShowDraggingBox_Line.Enabled = chk_ShowSamplePoints_Line.Enabled = true;
                }
            }
        }

        private void SetCircleGaugePlace()
        {
            if (m_smVisionInfo.g_arrSealROIs.Count > 6)
                if (m_smVisionInfo.g_arrSealROIs[6].Count > 0)
                    m_smVisionInfo.g_objSealCircleGauges.SetGaugePlacement(m_smVisionInfo.g_arrSealROIs[6][0], 20, m_smVisionInfo.g_objSeal.ref_intSpocketHolePosition == 0);
        }

        private void chk_ShowDraggingBox_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objSealCircleGauges.ref_blnDrawDraggingBox = chk_ShowDraggingBox_Circle.Checked;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowSamplePoints_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objSealCircleGauges.ref_blnDrawSamplingPoint = chk_ShowSamplePoints_Circle.Checked;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SealEdgeSensitivity_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_SealEdgeSensitivity.Text == "")
                return;

            m_smVisionInfo.g_objSeal.ref_intSealEdgeSensitivity = Convert.ToInt32(txt_SealEdgeSensitivity.Text);
        }

        private void srmButton2_Click(object sender, EventArgs e)
        {

        }

        private void btn_GaugeAdvanceSetting_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrSealGauges == null || m_smVisionInfo.g_arrSealGauges.Count == 0)
            {
                SRMMessageBox.Show("Please learn seal line first!", "Error", MessageBoxButtons.OK);
            }
            else
            {
                m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(2);
                string strPath = m_smProductionInfo.g_strRecipePath +
             m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Seal\\CircleGauge.xml";

                m_objAdvancedForm = new AdvancedPostSealLGaugeForm(m_smVisionInfo, m_smProductionInfo.g_strRecipePath +
                                            m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Seal\\Gauge.xml", m_smProductionInfo, m_smCustomizeInfo);

                m_objAdvancedForm.Show();

                m_smVisionInfo.g_blnViewGauge = true;
                m_smVisionInfo.g_intGaugeDisplayIndex = 0;
                
                btn_Close.Enabled = btn_Save.Enabled = btn_SealConfig.Enabled = btn_CircleGaugeSetting.Enabled = btn_GaugeAdvanceSetting.Enabled = false;

                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }

            //if (m_objAdvancedForm.ShowDialog() == DialogResult.OK)
            //{
            //    LoadGaugeGlobalSetting();
            //    AddGauge();
            //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //    if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
            //        m_smProductionInfo.g_blnSaveRecipeToServer = true;
            //}
            //else
            //{
            //    LoadGaugeGlobalSetting();
            //    AddGauge();
            //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //}
        }

        private void txt_SealEdgeTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_SealEdgeTolerance.Text == "")
                return;

            m_smVisionInfo.g_objSeal.ref_intSealEdgeTolerance = Convert.ToInt32(txt_SealEdgeTolerance.Text);
        }

        private void cbo_MarkTemplateNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;


            m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex = cbo_MarkTemplateNo.SelectedIndex;
            m_smVisionInfo.g_arrSealROIs[5][2].LoadROISetting(m_smVisionInfo.g_objSeal.GetTemplateMarkROIPosition().X, m_smVisionInfo.g_objSeal.GetTemplateMarkROIPosition().Y,
                m_smVisionInfo.g_objSeal.GetTemplateMarkROISize().Width, m_smVisionInfo.g_objSeal.GetTemplateMarkROISize().Height);

        }

        private void btn_Seal1BrokenAreaThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedUnit = 1;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(1);

            if (m_smVisionInfo.g_objPackageImage == null)
                m_smVisionInfo.g_objPackageImage = new ImageDrawing(true);

            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            m_smVisionInfo.g_blnViewPackageImage = true;
            ImageDrawing.CloseBoxImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objSeal.ref_intSealEdgeSensitivity);

            m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPackageImage);

            //m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intSeal1BrokenAreaThreshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_objSeal.ref_intSeal1BrokenAreaThreshold = m_smVisionInfo.g_intThresholdValue;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.g_blnViewPackageImage = false;
        }

        private void btn_Seal2BrokenAreaThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedUnit = 2;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(1);

            if (m_smVisionInfo.g_objPackageImage == null)
                m_smVisionInfo.g_objPackageImage = new ImageDrawing(true);

            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            m_smVisionInfo.g_blnViewPackageImage = true;
            ImageDrawing.CloseBoxImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objSeal.ref_intSealEdgeSensitivity);

            m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPackageImage);
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intSeal2BrokenAreaThreshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_objSeal.ref_intSeal2BrokenAreaThreshold = m_smVisionInfo.g_intThresholdValue;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.g_blnViewPackageImage = false;
        }

        private void txt_SprocketHoleInspectionAreaInwardTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_SprocketHoleInspectionAreaInwardTolerance.Text == "")
                return;

            m_smVisionInfo.g_objSeal.ref_intSprocketHoleInspectionAreaInwardTolerance = Convert.ToInt32(txt_SprocketHoleInspectionAreaInwardTolerance.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SprocketHoleInspectionAreaInwardTolerance_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewSealSprocketHoleInspectionAreaInwardTolerance = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SprocketHoleInspectionAreaInwardTolerance_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewSealSprocketHoleInspectionAreaInwardTolerance = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_SprocketHoleDefectThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);
            ImageDrawing objImg_Temp = new ImageDrawing(true, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageWidth, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageHeight);

            float fScale = Math.Max(m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter - m_smVisionInfo.g_objSeal.ref_intSprocketHoleInspectionAreaInwardTolerance, 5) / m_smVisionInfo.g_objSealCircleGauges.ref_fTemplateObjectDiameter;

            ImageDrawing.ScaleImage(m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage, ref objImg_Temp, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageWidth / 2, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageHeight / 2,
                                  objImg_Temp.ref_intImageWidth / 2, objImg_Temp.ref_intImageHeight / 2,
                                  fScale, fScale);

            ROI objCircleROI1 = new ROI();
            objCircleROI1.AttachImage(objImg_Temp);
            objCircleROI1.LoadROISetting((int)Math.Round((objImg_Temp.ref_intImageWidth / 2) - (m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter / 2)),
                                                (int)Math.Round((objImg_Temp.ref_intImageHeight / 2) - (m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter / 2)),
                                                (int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter),
                                                (int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter));

            ROI objCircleROI2 = new ROI();
            objCircleROI2.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            objCircleROI2.LoadROISetting((int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX - (m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter / 2)),
                                        (int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY - (m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter / 2)),
                                        (int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter),
                                        (int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter));
           
            ROI.LogicOperationAddROI(objCircleROI1, objCircleROI2);
            m_smVisionInfo.g_blnViewSealSprocketHoleInspectionAreaInwardTolerance = true;
            STTrackLog.WriteLine("SprocketHoleDefectThreshold Click A. ref_intSprocketHoleDefectThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleDefectThreshold.ToString());
            m_smVisionInfo.g_intSelectedUnit = 6;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);
            m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intSprocketHoleDefectThreshold;
            m_smVisionInfo.g_blnViewSprocketHoleDefectThreshold = true;
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, objCircleROI1);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                STTrackLog.WriteLine("SprocketHoleDefectThreshold Click B. ref_intSprocketHoleDefectThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleDefectThreshold.ToString());
                m_smVisionInfo.g_objSeal.ref_intSprocketHoleDefectThreshold = m_smVisionInfo.g_intThresholdValue;
            }

            STTrackLog.WriteLine("SprocketHoleDefectThreshold Click C. ref_intSprocketHoleDefectThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleDefectThreshold.ToString());
            objThresholdForm.Dispose();
            objCircleROI2.Dispose();
            objCircleROI1.Dispose();
            objImg_Temp.Dispose();
            m_smVisionInfo.g_blnViewSprocketHoleDefectThreshold = false;
            m_smVisionInfo.g_blnViewSealSprocketHoleInspectionAreaInwardTolerance = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_SprocketHoleBrokenThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);
            ImageDrawing objImg_Temp = new ImageDrawing(true, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageWidth, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageHeight);

            float fScale = (m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / m_smVisionInfo.g_objSealCircleGauges.ref_fTemplateObjectDiameter;

            ImageDrawing.ScaleImage(m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage, ref objImg_Temp, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageWidth / 2, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageHeight / 2,
                                  m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX, m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY,
                                  fScale, fScale);
            
            ROI objCircleROI1 = new ROI();
            objCircleROI1.AttachImage(objImg_Temp);
            objCircleROI1.LoadROISetting((int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)));

            ROI objCircleROI2 = new ROI();
            objCircleROI2.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            objCircleROI2.LoadROISetting((int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                        (int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                        (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)),
                                        (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)));
            
            ROI.SubtractROI2(objCircleROI2, objCircleROI1);

            ImageDrawing objImg_Temp2 = new ImageDrawing(true, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageWidth, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageHeight);

            fScale = (m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner) / m_smVisionInfo.g_objSealCircleGauges.ref_fTemplateObjectDiameter;

            ImageDrawing.ScaleImage(m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage, ref objImg_Temp2, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageWidth / 2, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageHeight / 2,
                                  m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX, m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY,
                                  fScale, fScale);

            objCircleROI1.LoadROISetting((int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner)));
            objCircleROI2.AttachImage(objImg_Temp2);
            objCircleROI2.LoadROISetting((int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner)));

            ROI.LogicOperationBitwiseAndROI(objCircleROI1, objCircleROI2);

            objCircleROI1.LoadROISetting((int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)));

            m_smVisionInfo.g_blnViewSealSprocketHoleBrokenOutwardTolerance = true;
            STTrackLog.WriteLine("SprocketHoleBrokenThreshold Click A. ref_intSprocketHoleBrokenThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenThreshold.ToString());
            m_smVisionInfo.g_intSelectedUnit = 6;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);
            m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenThreshold;
            m_smVisionInfo.g_blnViewSprocketHoleBrokenThreshold = true;
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, objCircleROI1);//m_smVisionInfo.g_arrSealROIs[6][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                STTrackLog.WriteLine("SprocketHoleBrokenThreshold Click B. ref_intSprocketHoleBrokenThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenThreshold.ToString());
                m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenThreshold = m_smVisionInfo.g_intThresholdValue;
            }

            STTrackLog.WriteLine("SprocketHoleBrokenThreshold Click C. ref_intSprocketHoleBrokenThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenThreshold.ToString());
            objThresholdForm.Dispose();
            objImg_Temp2.Dispose();
            objCircleROI2.Dispose();
            objCircleROI1.Dispose();
            objImg_Temp.Dispose();
            m_smVisionInfo.g_blnViewSprocketHoleBrokenThreshold = false;
            m_smVisionInfo.g_blnViewSealSprocketHoleBrokenOutwardTolerance = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SprocketHoleBrokenOutwardTolerance_Outer_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_SprocketHoleBrokenOutwardTolerance_Outer.Text == "")
                return;

            m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer = Convert.ToInt32(txt_SprocketHoleBrokenOutwardTolerance_Outer.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SprocketHoleBrokenOutwardTolerance_Inner_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_SprocketHoleBrokenOutwardTolerance_Inner.Text == "")
                return;

            if (Convert.ToInt32(txt_SprocketHoleBrokenOutwardTolerance_Inner.Text) > m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)
            {
                SRMMessageBox.Show("Sprocket Hole Broken Outward Tolerance Inner Setting Cannot Larger Than Outer Setting.", "Error Setting", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_SprocketHoleBrokenOutwardTolerance_Inner.Text = m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner.ToString();
                return;
            }

            m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner = Convert.ToInt32(txt_SprocketHoleBrokenOutwardTolerance_Inner.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_SprocketHoleBrokenOutwardTolerance_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewSealSprocketHoleBrokenOutwardTolerance = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SprocketHoleBrokenOutwardTolerance_Leave(object sender, EventArgs e)
        {

            if (Convert.ToInt32(txt_SprocketHoleBrokenOutwardTolerance_Outer.Text) < m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner)
            {
                SRMMessageBox.Show("Sprocket Hole Broken Outward Tolerance Outer Setting Cannot Smaller Than Inner Setting.", "Error Setting", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_SprocketHoleBrokenOutwardTolerance_Outer.Text = m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner.ToString();
                return;
            }


            m_smVisionInfo.g_blnViewSealSprocketHoleBrokenOutwardTolerance = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_SprocketHoleRoundnessThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);
            ImageDrawing objImg_Temp = new ImageDrawing(true, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageWidth, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageHeight);

            float fScale = (m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / m_smVisionInfo.g_objSealCircleGauges.ref_fTemplateObjectDiameter;

            ImageDrawing.ScaleImage(m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage, ref objImg_Temp, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageWidth / 2, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageHeight / 2,
                                  m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX, m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY,
                                  fScale, fScale);

            ROI objCircleROI1 = new ROI();
            objCircleROI1.AttachImage(objImg_Temp);
            objCircleROI1.LoadROISetting((int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)));

            ROI objCircleROI2 = new ROI();
            objCircleROI2.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            objCircleROI2.LoadROISetting((int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                        (int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                        (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)),
                                        (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)));

            ROI.SubtractROI2(objCircleROI2, objCircleROI1);
            m_smVisionInfo.g_blnViewSealSprocketHoleBrokenOutwardTolerance = true;
            m_smVisionInfo.g_intLearnStepNo = 9;//2021-03-07 ZJYEOH : For drawing purpose only
            STTrackLog.WriteLine("SprocketHoleRoundnessThreshold Click A. ref_intSprocketHoleRoundnessThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleRoundnessThreshold.ToString());
            m_smVisionInfo.g_intSelectedUnit = 6;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);
            m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intSprocketHoleRoundnessThreshold;
            m_smVisionInfo.g_blnViewSprocketHoleRoundnessThreshold = true;
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, objCircleROI1);//m_smVisionInfo.g_arrSealROIs[6][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                STTrackLog.WriteLine("SprocketHoleRoundnessThreshold Click B. ref_intSprocketHoleRoundnessThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleRoundnessThreshold.ToString());
                m_smVisionInfo.g_objSeal.ref_intSprocketHoleRoundnessThreshold = m_smVisionInfo.g_intThresholdValue;
            }

            STTrackLog.WriteLine("SprocketHoleRoundnessThreshold Click C. ref_intSprocketHoleRoundnessThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleRoundnessThreshold.ToString());
            objThresholdForm.Dispose();
            objCircleROI2.Dispose();
            objCircleROI1.Dispose();
            objImg_Temp.Dispose();
            m_smVisionInfo.g_intLearnStepNo = 0;//2021-03-07 ZJYEOH : For drawing purpose only
            m_smVisionInfo.g_blnViewSprocketHoleRoundnessThreshold = false;
            m_smVisionInfo.g_blnViewSealSprocketHoleBrokenOutwardTolerance = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void btn_SealEdgeStraightnessThreshold_Click(object sender, EventArgs e)
        {
            STTrackLog.WriteLine("SealEdgeStraightnessThreshold Click A. ref_intSealEdgeStraightnessThreshold = " + m_smVisionInfo.g_objSeal.ref_intSealEdgeStraightnessThreshold.ToString());
            m_smVisionInfo.g_intSelectedUnit = 7;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(7);
            m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intSealEdgeStraightnessThreshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrSealROIs[7][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                STTrackLog.WriteLine("SealEdgeStraightnessThreshold Click B. ref_intSealEdgeStraightnessThreshold = " + m_smVisionInfo.g_objSeal.ref_intSealEdgeStraightnessThreshold.ToString());
                m_smVisionInfo.g_objSeal.ref_intSealEdgeStraightnessThreshold = m_smVisionInfo.g_intThresholdValue;
            }

            STTrackLog.WriteLine("SealEdgeStraightnessThreshold Click C. ref_intSealEdgeStraightnessThreshold = " + m_smVisionInfo.g_objSeal.ref_intSealEdgeStraightnessThreshold.ToString());
            objThresholdForm.Dispose();
        }

        private void cbo_OverHeatROINo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedROI = cbo_OverHeatROINo.SelectedIndex;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_SetToAllOverHeatROI_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllOverHeatROI_SealOtherSetting", chk_SetToAllOverHeatROI.Checked);
            m_smVisionInfo.g_blnDrawThresholdAllSideROI = chk_SetToAllOverHeatROI.Checked;
        }
        //private void txt_MinBrokenArea_Enter(object sender, EventArgs e)
        //{
        //    m_smVisionInfo.g_blnViewDimension = true;
        //    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 5;
        //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        //}

        //private void txt_MinBrokenArea_Leave(object sender, EventArgs e)
        //{
        //    m_smVisionInfo.g_blnViewDimension = false;
        //    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = -1;
        //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        //}


    }
}
