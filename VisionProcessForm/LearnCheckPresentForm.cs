using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using SharedMemory;
using VisionProcessing;

namespace VisionProcessForm
{
    public partial class LearnCheckPresentForm : Form
    {
        #region Member Variables

        private int m_intDisplayStepNo = 1;
        private bool m_blnInitDone = false;
        private bool m_blnWantLearnOrient = false;

        private int m_intUserGroup;
        private string m_strFolderPath;
        private string m_strSelectedRecipe;
        
        private VisionInfo m_smVisionInfo;
        private CustomOption m_smCustomizeInfo;
        private UserRight m_objUserRight = new UserRight();
        private ProductionInfo m_smProductionInfo;
        private List<ROI> m_arrLGaugeROI = new List<ROI>();
        private List<ROI> m_arrPRSROI = new List<ROI>();

        private bool m_blnWantPosReference = false;

        #endregion

        public LearnCheckPresentForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;
            m_smCustomizeInfo = smCustomizeInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = smVisionInfo;
            
            m_strFolderPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\CheckPresent\\";

            DisableField();
            UpdateGUI();

            m_blnInitDone = true;
        } 
        
        private void AddSearchROI()
        {
            if ((m_smVisionInfo.g_arrPositioningROIs.Count) == 0)
            {
                m_smVisionInfo.g_arrPositioningROIs.Add(new ROI());

                m_smVisionInfo.g_arrPositioningROIs[0].ref_ROIPositionX = 100;
                m_smVisionInfo.g_arrPositioningROIs[0].ref_ROIPositionY = 100;
                m_smVisionInfo.g_arrPositioningROIs[0].ref_ROIWidth = 100;
                m_smVisionInfo.g_arrPositioningROIs[0].ref_ROIHeight = 100;
            }

            m_smVisionInfo.g_arrPositioningROIs[0].AttachImage(m_smVisionInfo.g_arrImages[0]);
        }

        private bool BuildObjects()
        {
            if (m_smVisionInfo.g_objUnitPresent == null)
            {
                SRMMessageBox.Show("UnitPresent object is not initialized yet.");
                return false;
            }

            // Build Search ROI
            int intBlobsSelectedCount = m_smVisionInfo.g_objUnitPresent.BuildObjects(m_smVisionInfo.g_arrPositioningROIs[0], true);
            lbl_TotalObjects.Text = intBlobsSelectedCount.ToString();

            m_smVisionInfo.g_objUnitPresent.FillBlobsToTemporaryObjects();

            if (intBlobsSelectedCount > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Customize GUI based on user access level
        /// </summary>
        private void DisableField()
        {
            string strChild1 = "Learn Page";
            string strChild2 = "";

            strChild2 = "Advance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_AdvancedSettings.Visible = false;
            }

            strChild2 = "Learn Template";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Previous.Enabled = false;
                btn_Next.Enabled = false;
            }
            strChild1 = "Learn Page";
            strChild2 = "Save Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
         
            }
        }

        private void SetupSteps(bool blnForward)
        {
            switch (m_smVisionInfo.g_objUnitPresent.ref_intDefineUnitMethod)
            {
                case 0:
                    SetupSteps_UnitROI(blnForward);
                    break;
                case 1:
                    SetupSteps_BlobObject(blnForward);
                    break;
            }
        }

        /// <summary>
        ///  Setup each learning steps
        /// </summary>
        /// <returns>true = no error, false = error during learning step</returns>
        private void SetupSteps_UnitROI(bool blnForward)
        {
            switch (m_smVisionInfo.g_intLearnStepNo)
            {
                case 0: // Define Search ROI

                    AddSearchROI();

                    lbl_TitleStep1.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_SearchROI;

                    m_smVisionInfo.g_blnViewObjectsBuilded = false;

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewSearchROI = true;

                    btn_Previous.Enabled = false;
                    break;
                case 1: // Unit ROI
                    lbl_TitleStep4.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_UnitROI;

                    if (blnForward)
                    {
                        m_smVisionInfo.g_objUnitPresent.SetTemporaryUnitROI(m_smVisionInfo.g_objUnitPresent.ref_intUnitROICountX,
                            m_smVisionInfo.g_objUnitPresent.ref_intUnitROICountY,
                            m_smVisionInfo.g_arrPositioningROIs[0], m_smVisionInfo.g_arrImages[0]);
                    }

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewSearchROI = true;

                    btn_Previous.Enabled = true;
                    btn_Next.Enabled = true;                   
                    break;
                case 2: // Confirm Unit ROI
                    lbl_TitleStep3.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Finish;

                    btn_Next.Enabled = false;
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        /// <summary>
        ///  Setup each learning steps
        /// </summary>
        /// <returns>true = no error, false = error during learning step</returns>
        private void SetupSteps_BlobObject(bool blnForward)
        {
            switch (m_smVisionInfo.g_intLearnStepNo)
            {
                case 0: // Define Search ROI

                    AddSearchROI();

                    lbl_TitleStep1.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_SearchROI;

                    m_smVisionInfo.g_blnViewObjectsBuilded = false;

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewSearchROI = true;

                    btn_Previous.Enabled = false;
                    break;
                case 1: // Segmentation
                    lbl_TitleStep2.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_SegmentObj;

                    if (blnForward)
                    {
                        if (BuildObjects())
                        {
                            m_smVisionInfo.g_blnViewObjectsBuilded = true;
                        }
                        else
                        {
                            m_smVisionInfo.g_blnViewObjectsBuilded = false;
                            m_smVisionInfo.g_intLearnStepNo--;
                            m_intDisplayStepNo--;
                            break;
                        }
                    }
                    else
                        m_smVisionInfo.g_blnViewObjectsBuilded = true;

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewSearchROI = true;

                    btn_Previous.Enabled = true;
                    btn_Next.Enabled = true;
                    break;
                case 2: // Confirm Unit ROI
                    lbl_TitleStep3.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Finish;

                    m_smVisionInfo.g_objUnitPresent.CopySelectedTemporaryObjectsToTemplateObjects();
                    m_smVisionInfo.g_objUnitPresent.DefineTemplateObjectsHalfPitch();
                    m_smVisionInfo.g_objUnitPresent.SortObjectNumber();

                    m_smVisionInfo.g_blnViewObjectsBuilded = false;

                    btn_Next.Enabled = false;
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        /// <summary>
        /// update display of settings
        /// </summary>
        private void UpdateGUI()
        {
            cbo_UnitType.SelectedIndex = 0;

            if (m_smVisionInfo.g_objUnitPresent.ref_blnWhiteOnBlack)
                cbo_ClassSelection.SelectedIndex = 0;
            else
                cbo_ClassSelection.SelectedIndex = 1;

            txt_MinArea.Text = m_smVisionInfo.g_objUnitPresent.ref_fFilterMinArea.ToString();
            txt_MaxArea.Text = m_smVisionInfo.g_objUnitPresent.ref_fFilterMaxArea.ToString();

            txt_UnitROICountX.Text = m_smVisionInfo.g_objUnitPresent.ref_intUnitROICountX.ToString();
            txt_UnitROICountY.Text = m_smVisionInfo.g_objUnitPresent.ref_intUnitROICountY.ToString();
            txt_TotalUnitROIs.Text = (m_smVisionInfo.g_objUnitPresent.ref_intUnitROICountX * m_smVisionInfo.g_objUnitPresent.ref_intUnitROICountY).ToString();

            chk_FollowFirstROISize.Checked = m_smVisionInfo.g_objUnitPresent.ref_blnFollowFirstROISize;
            chk_AdjustBasedOnCornerROI.Checked = m_smVisionInfo.g_objUnitPresent.ref_blnAdjustBasedOnCornerROI;
        }


        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objUnitPresent.LoadUnitPresent(m_strFolderPath + "Setting.xml", "UnitPresentSetting");

            Close();
            Dispose();
        }
                
        private void btn_Next_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intLearnStepNo++;
            m_intDisplayStepNo++;
            SetupSteps(true);

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Previous_Click(object sender, EventArgs e)
        {

            m_smVisionInfo.g_intLearnStepNo--;
            m_intDisplayStepNo--;
            SetupSteps(false);

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            // Save template images
            m_smVisionInfo.g_arrImages[0].SaveImage(m_strFolderPath + "\\Template\\OriTemplate.bmp");

            // Save search ROI
            m_smVisionInfo.g_arrPositioningROIs[0].SaveImage(m_strFolderPath + "\\Template\\Template0.bmp");

            ROI.SaveFile(m_strFolderPath + "ROI.xml", m_smVisionInfo.g_arrPositioningROIs);

            
            STDeviceEdit.CopySettingFile(m_strFolderPath, "\\Setting.xml");

            if (m_smVisionInfo.g_objUnitPresent.ref_intDefineUnitMethod == 0)
            {
                m_smVisionInfo.g_objUnitPresent.CopyTemporaryUnitROIToTemporaryUnitROI(m_smVisionInfo.g_arrPositioningROIs[0]);
                m_smVisionInfo.g_objUnitPresent.SaveUnitPresent(m_strFolderPath + "Setting.xml", false, "UnitPresentSetting", true);
            }
            else
            {
                m_smVisionInfo.g_objUnitPresent.SaveUnitPresent(m_strFolderPath + "Setting.xml", false, "UnitPresentSetting", true);
            }
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">UnitPresentSetting", m_smProductionInfo.g_strLotID);
            
            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            Close();
            Dispose();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_smVisionInfo.AT_VM_UpdateResult)
            {
                m_smVisionInfo.AT_VM_UpdateResult = false;
            }

            if (m_smVisionInfo.g_intLearnStepNo == 1)
            {
                if (Convert.ToInt32(lbl_TotalObjects.Text) != m_smVisionInfo.g_objUnitPresent.GetTotalSelectedTemporaryObjects())
                {
                    lbl_TotalObjects.Text = m_smVisionInfo.g_objUnitPresent.GetTotalSelectedTemporaryObjects().ToString();
                }
            }
        }

        private void btn_Undo_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_arrPolygon[0][m_smVisionInfo.g_intSelectedTemplate].UndoPolygon();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_AdvancedSettings_Click(object sender, EventArgs e)
        {
            AdvancedPositionForm objAdvancedPositionForm = new AdvancedPositionForm(m_smVisionInfo, m_strSelectedRecipe, m_intUserGroup , m_smProductionInfo);

            if (objAdvancedPositionForm.ShowDialog() == DialogResult.OK)
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    m_smProductionInfo.g_blnSaveRecipeToServer = true;
            }

            objAdvancedPositionForm.Dispose();
        }

        private void LearnCheckPresentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Learn Unit Present Form Closed", "Exit Learn Unit Present Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_blnViewObjectsBuilded = false;

            m_smVisionInfo.g_blnViewSearchROI = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.g_blnViewROI = false;
            
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_objPositioning.ref_bFinalResult = false;
            m_smVisionInfo.g_blnViewDontCareArea = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;

        }

        private void LearnCheckPresentForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;

            if (m_smVisionInfo.g_objPositioning.ref_intMethod == 0)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPositioningROIs.Count; i++)
                {
                    ROI objROI = new ROI();
                    m_smVisionInfo.g_arrPositioningROIs[i].CopyToNew(ref objROI);
                    if (m_arrLGaugeROI.Count <= i)
                        m_arrLGaugeROI.Add(objROI);
                    else
                        m_arrLGaugeROI[i] = objROI;
                }
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPositioningROIs.Count; i++)
                {
                    ROI objROI = new ROI();
                    m_smVisionInfo.g_arrPositioningROIs[i].CopyToNew(ref objROI);
                    if (m_arrPRSROI.Count <= i)
                        m_arrPRSROI.Add(objROI);
                    else
                        m_arrPRSROI[i] = objROI;
                }
            }

            if (m_smVisionInfo.g_arrPolygon.Count == 0)
            {
                m_smVisionInfo.g_arrPolygon.Add(new List<Polygon>());
                m_smVisionInfo.g_arrPolygon[0].Add(new Polygon());
            }

            m_smVisionInfo.g_intLearnStepNo = 0;
            lbl_StepNo.BringToFront();
            SetupSteps(true);

            m_smVisionInfo.VM_AT_SettingInDialog = true;
        }

        private void cbo_ClassSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (cbo_ClassSelection.SelectedIndex == 0)
                m_smVisionInfo.g_objUnitPresent.ref_blnWhiteOnBlack = true;
            else
                m_smVisionInfo.g_objUnitPresent.ref_blnWhiteOnBlack = false;

            BuildObjects();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (!float.TryParse(txt_MinArea.Text, out fValue))
                return;

            m_smVisionInfo.g_objUnitPresent.ref_fFilterMinArea = fValue;

            BuildObjects();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MaxArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (!float.TryParse(txt_MaxArea.Text, out fValue))
                return;

            m_smVisionInfo.g_objUnitPresent.ref_fFilterMaxArea = fValue;

            BuildObjects();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Threshold_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_blnViewObjectsBuilded = false;
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objUnitPresent.ref_intThresholdValue;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPositioningROIs[0], true, false, new List<int>());
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_objUnitPresent.ref_intThresholdValue = m_smVisionInfo.g_intThresholdValue;

                BuildObjects();
            }

            objThresholdForm.Dispose();

            m_smVisionInfo.g_blnViewObjectsBuilded = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_UndoObjects_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objUnitPresent.SetTemporaryBlobsToUnselected();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void tp_UnitROI_Click(object sender, EventArgs e)
        {

        }

        private void btn_ThresholdUnitROI_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objUnitPresent.ref_intThresholdValue;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPositioningROIs[0], true, false, new List<int>());
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_objUnitPresent.ref_intThresholdValue = m_smVisionInfo.g_intThresholdValue;
            }

            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_UnitROICount_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intValueX = 0;
            if (!int.TryParse(txt_UnitROICountX.Text, out intValueX))
                return;

            int intValueY = 0;
            if (!int.TryParse(txt_UnitROICountY.Text, out intValueY))
                return;

            txt_TotalUnitROIs.Text = (intValueX * intValueY).ToString();

            m_smVisionInfo.g_objUnitPresent.SetTemporaryUnitROI(intValueX, intValueY,
                m_smVisionInfo.g_arrPositioningROIs[0],
                m_smVisionInfo.g_arrImages[0]);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void chk_FollowFirstROISize_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objUnitPresent.ref_blnFollowFirstROISize = chk_FollowFirstROISize.Checked;
        }

        private void chk_AdjustBasedOnCornerROI_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objUnitPresent.ref_blnAdjustBasedOnCornerROI = chk_AdjustBasedOnCornerROI.Checked;
        }

   }
}