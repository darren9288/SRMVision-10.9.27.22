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
using System.IO;

namespace VisionProcessForm
{
    public partial class LearnPositionForm : Form
    {
        #region Member Variables
        private int m_intDisplayStepNo = 1;
        private bool m_blnInitDone = false;
        private bool m_blnWantLearnOrient = false;

        private int m_intUserGroup;
        private string m_strFolderPath;
        private string m_strSelectedRecipe;
        private string path;
        private int intBlackArea;
        private VisionInfo m_smVisionInfo;
        private CustomOption m_smCustomizeInfo;
        private UserRight m_objUserRight = new UserRight();
        private ProductionInfo m_smProductionInfo;
        private List<ROI> m_arrLGaugeROI = new List<ROI>();
        private List<ROI> m_arrPRSROI = new List<ROI>();
        
        private bool m_blnWantPosReference = false;

        #endregion

        public LearnPositionForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;
            m_smCustomizeInfo = smCustomizeInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = smVisionInfo;
            
            txt_MinWhiteArea.Text = m_smVisionInfo.g_objPositioning.ref_intEmptyWhiteArea.ToString();
            m_strFolderPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Positioning\\";
            path = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\";
            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                if ((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    m_blnWantLearnOrient = false;   // Orient feature in LearnPositionForm will be disabled if WantMark is true because the Orient will be learned in LearnMarkForm.
                else
                    m_blnWantLearnOrient = true;
            }
            else
                m_blnWantLearnOrient = false;

            switch (m_smVisionInfo.g_strVisionName)
            {
                case "InPocketPkgPos":
                    m_blnWantPosReference = true;   // InPocket Position need a reference object to find the pocket offset.
                    break;
            }

            if (m_smVisionInfo.g_blnWantCheckEmpty)
                DisableField2();

            UpdateGUI();

            m_blnInitDone = true;
        }

        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild2 = "Learn Empty Unit Page";
            string strChild3 = "Save Button";

            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetInPocketChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
            {
                btn_SaveEmptyPattern.Enabled = false;
                btn_SaveEmptyThreshold.Enabled = false;
            }

            strChild3 = "Empty Pocket Threshold";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetInPocketChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
            {
                btn_Threshold.Enabled = false;

                srmLabel26.Visible = lbl_EmptyThreshold.Visible = btn_Threshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

            }

            strChild3 = "Empty Pocket Min Black Area";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetInPocketChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
            {
                txt_MinWhiteArea.Enabled = false;

                srmLabel16.Visible = txt_MinWhiteArea.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            //string strChild1 = "Learn Page";
            //strChild2 = "";

            //strChild2 = "Advance Setting";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //{
            //    btn_AdvancedSettings.Visible = false;
            //}

            //strChild2 = "Gauge Setting";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //{
            //    btn_GaugeAdvanceSetting.Visible = false;
            //}

            //strChild2 = "Learn Template";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //{
            //    btn_Previous.Enabled = false;
            //    btn_Next.Enabled = false;
            //}

            //strChild1 = "Learn Page";
            //strChild2 = "Save Button";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //{
            //    btn_Save.Enabled = false;
            //}

            if (!m_blnWantLearnOrient)
            {
                btn_AdvancedSettings.Visible = false;
            }
        }

        /// <summary>
        /// Put line gauge of 4 borders
        /// </summary>
        private void AddGauge(int intIndex)
        {
            if ((m_smVisionInfo.g_arrPositioningGauges.Count - 1) < intIndex)
            {
                m_smVisionInfo.g_arrPositioningGauges.Add(new LGauge(m_smVisionInfo.g_WorldShape));

                switch (intIndex)
                {
                    case 0: m_smVisionInfo.g_arrPositioningGauges[intIndex].ref_GaugeAngle = 0;
                        break;
                    case 1: m_smVisionInfo.g_arrPositioningGauges[intIndex].ref_GaugeAngle = 90;
                        break;
                    case 2: m_smVisionInfo.g_arrPositioningGauges[intIndex].ref_GaugeAngle = 180;
                        break;
                    case 3: m_smVisionInfo.g_arrPositioningGauges[intIndex].ref_GaugeAngle = 270;
                        break;
                }

                m_smVisionInfo.g_arrPositioningGauges[intIndex].SetGaugePlacement(m_smVisionInfo.g_arrPositioningROIs[intIndex]);
            }
        }

        private void AddSearchROI()
        {
            for (int i = 0; i < 4; i++)
            {
                if ((m_smVisionInfo.g_arrPositioningROIs.Count - 1) < i)
                {
                    m_smVisionInfo.g_arrPositioningROIs.Add(new ROI());

                    m_smVisionInfo.g_arrPositioningROIs[i].ref_ROIPositionX = 100;
                    m_smVisionInfo.g_arrPositioningROIs[i].ref_ROIPositionY = 100;
                    m_smVisionInfo.g_arrPositioningROIs[i].ref_ROIWidth = 100;
                    m_smVisionInfo.g_arrPositioningROIs[i].ref_ROIHeight = 100;                    
                }

                switch (i)
                {
                    case 0: m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName = "Top";
                        break;
                    case 1: m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName = "Right";
                        break;
                    case 2: m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName = "Bottom";
                        break;
                    case 3: m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName = "Left";
                        break;
                }

                m_smVisionInfo.g_arrPositioningROIs[i].ref_intType = 1;
                m_smVisionInfo.g_arrPositioningROIs[i].AttachImage(m_smVisionInfo.g_arrImages[0]);
                AddGauge(i);
            }
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

            strChild2 = "Gauge Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_GaugeAdvanceSetting.Visible = false;
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

            if (!m_blnWantLearnOrient)
            {
                btn_AdvancedSettings.Visible = false;
            }
        }

        /// <summary>
        ///  Setup each learning steps
        /// </summary>
        /// <returns>true = no error, false = error during learning step</returns>
        private void SetupSteps()
        {
            switch (m_smVisionInfo.g_intLearnStepNo)
            {
                case 0: // Select Learn Type (Unit or Empty Unit) and Positioning Method (Pattern Matching or Line Gauges)
                    lbl_TitleStep1.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step1;

                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewROI = false;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewSearchROI = false;

                    if (m_intUserGroup <= m_objUserRight.GetGroupLevel3("Learn Page", "Learn Template"))
                        btn_Next.Enabled = true;
                    btn_Previous.Enabled = false;
                    break;

                case 1: // Use gauge to measure object
                    lbl_TitleStep2.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step2;

                    AddSearchROI();

                    for (int i = 0; i < 4; i++)
                    {
                        m_smVisionInfo.g_arrPositioningGauges[i].SetGaugePlacement(m_smVisionInfo.g_arrPositioningROIs[i]);
                        m_smVisionInfo.g_arrPositioningGauges[i].Measure(m_smVisionInfo.g_arrPositioningROIs[i]);
                    }

                    m_blnInitDone = false;
                    txt_SamplingStep.Text = Convert.ToInt32(m_smVisionInfo.g_arrPositioningGauges[0].ref_GaugeSamplingStep).ToString();
                    m_blnInitDone = true;

                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnViewGauge = true;
                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.AT_VM_UpdateResult = true;
                    btn_Previous.Enabled = true;
                    if (m_blnWantLearnOrient)
                    {
                        btn_Next.Enabled = true;
                        btn_Save.Visible = false;
                    }
                    else
                    {
                        btn_Next.Enabled = false;
                        btn_Save.Visible = true;
                        tp_Step2.Controls.Add(btn_Save);
                    }
                    break;

                case 2: // Define search ROI
                    lbl_TitleStep3.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step3;

                    AddROI("Search ROI", 1);

                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewROI = true;

                    btn_Previous.Enabled = true;
                    break;
                case 3: // Rotate Unit
                    lbl_TitleStep4.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step4;

                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.g_blnDragROI = false;
                                     
                    btn_Next.Enabled = true;
                    break;
                case 4:
                    lbl_TitleStepPocket.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Pocket;

                    AddROI("Pocket ROI", 2);

                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnDragROI = true;

                    btn_Next.Enabled = true;
                    break;
                case 5: // Define Unit ROI
                    lbl_TitleStep5.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step5;
                    //tp_Step5.Controls.Add(btn_PRSSave);

                    AddROI("Train ROI", 2);

                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewDontCareArea = false;
                    
                    btn_Next.Enabled = true;
                    break;
                case 6:
                    lbl_TitleStepDontCareArea.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_DontCareArea;
                    tp_DontCareArea.Controls.Add(btn_PRSSave);

                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewDontCareArea = true;

                    if (m_blnWantLearnOrient)
                    {
                        btn_Next.Enabled = true;
                        btn_PRSSave.Visible = false;
                    }
                    else
                    {
                        btn_Next.Enabled = false;
                        btn_PRSSave.Visible = true;
                        tp_DontCareArea.Controls.Add(btn_PRSSave);
                    }
                    break;
                case 7:
                    lbl_TitleStep6.Text = "Learn Empty Pattern";
                    lbl_TitleStep6.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_EmptyUnit;
                    //tp_EmptyUnit.Controls.Add(btn_PRSSave);

                    AddROI("Search ROI", 1, 0);

                    //AddROI("Train ROI", 2);

                    if (m_smVisionInfo.g_arrMarkROIs.Count > 0 && m_smVisionInfo.g_arrMarkROIs[0].Count > 0)//File.Exists(path + "\\ROI.xml"))
                    {
                        m_smVisionInfo.g_arrPositioningROIs[0] = m_smVisionInfo.g_arrMarkROIs[0][0];
                        m_smVisionInfo.g_arrPositioningROIs[0].ref_strROIName = "Search ROI";
                    }
                    //if(m_smVisionInfo.g_arrMarkROIs[0].Count > 1)
                    //{
                    //    m_smVisionInfo.g_arrPositioningROIs[1] = m_smVisionInfo.g_arrMarkROIs[0][1];
                    //    m_smVisionInfo.g_arrPositioningROIs[1].ref_strROIName = "Train ROI";
                    //}

                    AddROI("Empty ROI",2, 1);

                    m_smVisionInfo.g_arrPositioningROIs[0].AttachImage(m_smVisionInfo.g_arrImages[0]); //m_smVisionInfo.g_intSelectedImage

                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    btn_Previous.Enabled = true;
                    btn_Next.Enabled = true;
                    btn_Save.Visible = false;
                    btn_PRSSave.Visible = false;
                    if (!m_smVisionInfo.g_blnWantUseEmptyThreshold)
                        tp_EmptyUnit.Controls.Add(btn_SaveEmptyThreshold);
                    if (m_smVisionInfo.g_blnWantCheckEmpty)
                    {
                        btn_Previous.Enabled = false;
                        if(m_smVisionInfo.g_blnWantUseEmptyThreshold)
                        btn_Next.Enabled = true;
                        else
                            btn_Next.Enabled = false;
                    }
                    break;
                case 8:
                    lbl_TitleStepOrientROI.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Orient;

                    AddROI("Orient ROI", 2);

                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewDontCareArea = false;

                    btn_Previous.Enabled = true;
                    btn_Next.Enabled = false;
                    if (cbo_method.SelectedIndex == 0) // Line Gauge Method
                    {
                        btn_Save.Visible = true;
                        btn_PRSSave.Visible = false;
                        tp_Orient.Controls.Add(btn_Save);
                    }
                    else
                    {
                        btn_Save.Visible = false;
                        btn_PRSSave.Visible = true;
                        tp_Orient.Controls.Add(btn_PRSSave);
                    }
                    break;
                case 9:
                    lbl_TitleStep6.Text = "Learn Empty Threshold";
                    lbl_TitleStep6.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_threshold;
                    lbl_EmptyThreshold.Text = m_smVisionInfo.g_objPositioning.ref_intEmptyThreshold.ToString();

                    AddROI("Search ROI", 1, 0);

                    //AddROI("Train ROI", 2);

                    if (m_smVisionInfo.g_arrMarkROIs[0].Count > 0)//File.Exists(path + "\\ROI.xml"))
                    {
                        m_smVisionInfo.g_arrPositioningROIs[0] = m_smVisionInfo.g_arrMarkROIs[0][0];
                        m_smVisionInfo.g_arrPositioningROIs[0].ref_strROIName = "Search ROI";
                    }
                    //if (m_smVisionInfo.g_arrMarkROIs[0].Count > 1)
                    //{
                    //    m_smVisionInfo.g_arrPositioningROIs[1] = m_smVisionInfo.g_arrMarkROIs[0][1];
                    //    m_smVisionInfo.g_arrPositioningROIs[1].ref_strROIName = "Train ROI";
                    //}
                    //if(m_smVisionInfo.g_blnWantUseEmptyPattern)
                    AddROI("Empty ROI", 2, 1);
                    //AddROI("Threshold ROI", 2);

                    m_smVisionInfo.g_arrPositioningROIs[0].AttachImage(m_smVisionInfo.g_arrImages[0]); //m_smVisionInfo.g_intSelectedImage

                    if (m_smVisionInfo.g_blnWantUseEmptyPattern)
                        intBlackArea = ROI.GetPixelArea(m_smVisionInfo.g_arrPositioningROIs[1], m_smVisionInfo.g_objPositioning.ref_intEmptyThreshold, 0);
                    else
                        intBlackArea = ROI.GetPixelArea(m_smVisionInfo.g_arrPositioningROIs[1], m_smVisionInfo.g_objPositioning.ref_intEmptyThreshold, 0);

                    lbl_EmptyWhiteArea.Text = intBlackArea.ToString();

                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.g_blnDragROI = false;
                    btn_Previous.Enabled = true;
                    btn_Next.Enabled = false;
                    btn_Save.Visible = false;
                    btn_PRSSave.Visible = false;
                    if (m_smVisionInfo.g_blnWantCheckEmpty)
                    {
                        //if(m_smVisionInfo.g_blnWantUseEmptyPattern)
                        btn_Previous.Enabled = true;
                        //else
                        //    btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                    }
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void btn_Threshold_Click(object sender, EventArgs e)
        {
            Position objPosition = m_smVisionInfo.g_objPositioning;
            int intThreshold = m_smVisionInfo.g_intThresholdValue = objPosition.ref_intEmptyThreshold;
            ROI objROI ;
            if (m_smVisionInfo.g_arrPositioningROIs.Count > 1 && m_smVisionInfo.g_blnWantUseEmptyPattern)
                objROI = m_smVisionInfo.g_arrPositioningROIs[1];
            else
                objROI = m_smVisionInfo.g_arrPositioningROIs[1];

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, objROI);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
                objPosition.ref_intEmptyThreshold = intThreshold;
            else
            {
                objPosition.ref_intEmptyThreshold = m_smVisionInfo.g_intThresholdValue;
                lbl_EmptyThreshold.Text = objPosition.ref_intEmptyThreshold.ToString();
                intBlackArea = ROI.GetPixelArea(objROI, m_smVisionInfo.g_intThresholdValue, 0);
                lbl_EmptyWhiteArea.Text = intBlackArea.ToString();
            }
          
            objThresholdForm.Dispose();

          //  objPosition.BuildObjects(m_smVisionInfo.g_arrPositioningROIs[0]);
            //m_smVisionInfo.g_blnViewPackageObjectBuilded = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void btn_SaveEmptyThreshold_Click(object sender, EventArgs e)
        {
            //Save Pattern
            //if (m_smVisionInfo.g_blnWantUseEmptyPattern)
            {
                //m_smVisionInfo.g_arrPositioningROIs[0].SaveImage("D:\\TS\\0.bmp");
                //m_smVisionInfo.g_arrPositioningROIs[1].SaveImage("D:\\TS\\1.bmp");
                m_smVisionInfo.g_objPositioning.LearnEmptyPattern(m_smVisionInfo.g_arrPositioningROIs[1]);

                int intTotalROICount = m_smVisionInfo.g_arrPositioningROIs.Count;
                for (int i = 2; i < intTotalROICount; i++)
                {
                    m_smVisionInfo.g_arrPositioningROIs.RemoveAt(2);
                }

                // 
                //   STDeviceEdit.CopySettingFile(m_strFolderPath, "ROI.xml");
                ROI.SaveFile(m_strFolderPath + "\\ROI.xml", m_smVisionInfo.g_arrPositioningROIs);
                //   STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Empty ROI", m_strFolderPath, "ROI.xml");

                CopyFiles m_objCopy = new CopyFiles();
                string strCurrentDateTIme = DateTime.Now.ToString();
                DateTime DT = Convert.ToDateTime(strCurrentDateTIme);
                string strCurrentDateTIme1 = strCurrentDateTIme.Replace(':', '.');
                string strCurrentDateTIme2 = strCurrentDateTIme.ToString();
                string strPath = m_smProductionInfo.g_strHistoryDataLocation + "Data\\" + DT.ToString("yyyy-MM") + "\\" + strCurrentDateTIme1 + "\\" + m_strSelectedRecipe + "-" + m_smVisionInfo.g_strVisionFolderName + "-Learn Empty\\";
                

                if (File.Exists(m_strFolderPath + "Template\\EmptyOriTemplate.bmp"))
                    STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Empty", "EmptyOriTemplate.bmp", "EmptyOriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
                else
                    STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Empty", "", "EmptyOriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);

                m_objCopy.CopyAllImageFiles(m_strFolderPath + "Template\\", strPath + "Old\\");
                
                // Save template images
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].SaveImage(m_strFolderPath + "\\Template\\EmptyOriTemplate.bmp");
                m_smVisionInfo.g_objPositioning.SaveEmptyPattern(m_strFolderPath + "\\Template\\EmptyTemplate0.mch");
                m_smVisionInfo.g_arrPositioningROIs[1].SaveImage(m_strFolderPath + "\\Template\\EmptyTemplate0.bmp"); // [2] to [0] 

                m_objCopy.CopyAllImageFiles(m_strFolderPath + "Template\\", strPath + "New\\");
                
            }

            //Save Threshold
            if (m_smVisionInfo.g_blnWantUseEmptyThreshold)
            {
                // Save template images
                m_smVisionInfo.g_arrImages[0].SaveImage(m_strFolderPath + "\\Template\\ThresholdOriTemplate.bmp");

                m_smVisionInfo.g_arrPositioningROIs[0].SaveImage(m_strFolderPath + "\\Template\\ThresholdTemplate0.bmp"); // [2] to [0] 

                m_smVisionInfo.g_objPositioning.ref_intMethod = 0;

                // Save Setting
                if (txt_MinWhiteArea.Text == "")
                {
                    SRMMessageBox.Show("Minimum White Area cannot be empty!");
                    return;
                }
                m_smVisionInfo.g_objPositioning.ref_intEmptyWhiteArea = Convert.ToInt32(txt_MinWhiteArea.Text);
            }
            //
            //STDeviceEdit.CopySettingFile(m_strFolderPath, "Settings.xml");
            m_smVisionInfo.g_objPositioning.SavePosition(m_strFolderPath + "Settings.xml", false, "General", true);
           // STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Pad Settings", m_strFolderPath, "Settings.xml");

           // STDeviceEdit.CopySettingFile(m_strFolderPath, "ROI.xml");
            ROI.SaveFile(m_strFolderPath + "\\ROI.xml", m_smVisionInfo.g_arrPositioningROIs);
            // STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Pad ROI", m_strFolderPath, "ROI.xml");

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            Close();
            Dispose();
        }

        /// <summary>
        /// update display of settings
        /// </summary>
        private void UpdateGUI()
        {
            cbo_method.SelectedIndex = m_smVisionInfo.g_objPositioning.ref_intMethod;
            lbl_DieWidth.Text = m_smVisionInfo.g_objPositioning.ref_fSampleDieWidth.ToString();
            lbl_DieHeight.Text = m_smVisionInfo.g_objPositioning.ref_fSampleDieHeight.ToString();
            cbo_UnitType.SelectedIndex = 0;
            cbo_DontCareAreaDrawMethod.SelectedIndex = 0;

            if (m_smVisionInfo.g_arrImages.Count <= 1)
            {
                lbl_Image.Visible = false;
                cbo_ImageNo.Visible = false;
                m_smVisionInfo.g_intSelectedImage = 0;
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                {
                    cbo_ImageNo.Items.Add("Image " + (i + 1).ToString());
                }

                cbo_ImageNo.SelectedIndex = m_smVisionInfo.g_objPositioning.ref_intPositionImageIndex;
                m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objPositioning.ref_intPositionImageIndex;
            }

            
        }

        private void AddROI(string strROIName, int intROIType)
        {
           
            // Check is Pocket ROI exist?
            int intIndexFound = m_smVisionInfo.g_arrPositioningROIs.Count;
            for (int i = 0; i < m_smVisionInfo.g_arrPositioningROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName == strROIName)
                {
                    intIndexFound = i;
                    break;
                }
            }
            // Add ROI if not found
            if (intIndexFound == m_smVisionInfo.g_arrPositioningROIs.Count)
            {
                ROI objROI = new ROI();
                objROI.ref_strROIName = strROIName;
                objROI.ref_intType = intROIType;
                objROI.ref_ROIPositionX = 100;
                objROI.ref_ROIPositionY = 100;
                objROI.ref_ROIWidth = 100;
                objROI.ref_ROIHeight = 100;
                m_smVisionInfo.g_arrPositioningROIs.Add(objROI);
                
            }
        
            if (strROIName == "Search ROI")
            {
                // Attach Search ROI to image
                m_smVisionInfo.g_arrPositioningROIs[intIndexFound].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            }
            else if (strROIName == "Orient ROI")
            {
                bool blnFound = false;
                // Find Train ROI (unit ROI) index
                for (int i = 0; i < m_smVisionInfo.g_arrPositioningROIs.Count; i++)
                {
                    if (m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName == "Train ROI")
                    {
                        // Attach Orient ROI to Unit ROI
                        m_smVisionInfo.g_arrPositioningROIs[intIndexFound].AttachImage(m_smVisionInfo.g_arrPositioningROIs[i]);
                        blnFound = true;
                    }
                }

                if (!blnFound)
                {
                    //Attach Orient ROI to Search ROI
                    m_smVisionInfo.g_arrPositioningROIs[intIndexFound].AttachImage(m_smVisionInfo.g_arrPositioningROIs[0]);
                }                
            }
            else
            {

                //Attach Pocket ROI to Search ROI
                m_smVisionInfo.g_arrPositioningROIs[intIndexFound].AttachImage(m_smVisionInfo.g_arrPositioningROIs[0]);

                if (m_smVisionInfo.g_arrPositioningROIs[intIndexFound].ref_ROIWidth == 0 || m_smVisionInfo.g_arrPositioningROIs[intIndexFound].ref_ROIHeight == 0)
                {
                    m_smVisionInfo.g_arrPositioningROIs[intIndexFound].ref_ROIWidth = 100;
                    m_smVisionInfo.g_arrPositioningROIs[intIndexFound].ref_ROIHeight = 100;

                }
            }
        }

        private void AddROI(string strROIName, int intROIType, int intArrayIndex)
        {
            if (intArrayIndex >= m_smVisionInfo.g_arrPositioningROIs.Count)
            {
                for (int i = m_smVisionInfo.g_arrPositioningROIs.Count; i <= intArrayIndex; i++)
                {
                    if (i == intArrayIndex)
                    {
                        ROI objROI = new ROI();
                        objROI.ref_strROIName = strROIName;
                        objROI.ref_intType = intROIType;
                        objROI.ref_ROIPositionX = 100;
                        objROI.ref_ROIPositionY = 100;
                        objROI.ref_ROIWidth = 100;
                        objROI.ref_ROIHeight = 100;
                        m_smVisionInfo.g_arrPositioningROIs.Add(objROI);
                    }
                    else
                    {
                        m_smVisionInfo.g_arrPositioningROIs.Add(new ROI());
                    }
                }
            }

            if (m_smVisionInfo.g_arrPositioningROIs[intArrayIndex].ref_strROIName != strROIName)
            {
                m_smVisionInfo.g_arrPositioningROIs[intArrayIndex].ref_strROIName = strROIName;
            }

            if (m_smVisionInfo.g_arrPositioningROIs[intArrayIndex].ref_intType != intROIType)
            {
                m_smVisionInfo.g_arrPositioningROIs[intArrayIndex].ref_intType = intROIType;
            }


            if (strROIName == "Search ROI")
            {
                // Attach Search ROI to image
                m_smVisionInfo.g_arrPositioningROIs[intArrayIndex].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            }
            else if (strROIName == "Orient ROI")
            {
                bool blnFound = false;
                // Find Train ROI (unit ROI) index
                for (int i = 0; i < m_smVisionInfo.g_arrPositioningROIs.Count; i++)
                {
                    if (m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName == "Train ROI")
                    {
                        // Attach Orient ROI to Unit ROI
                        m_smVisionInfo.g_arrPositioningROIs[intArrayIndex].AttachImage(m_smVisionInfo.g_arrPositioningROIs[i]);
                        blnFound = true;
                    }
                }

                if (!blnFound)
                {
                    //Attach Orient ROI to Search ROI
                    m_smVisionInfo.g_arrPositioningROIs[intArrayIndex].AttachImage(m_smVisionInfo.g_arrPositioningROIs[0]);
                }
            }
            else
            {
                m_smVisionInfo.g_arrPositioningROIs[intArrayIndex].ref_strROIName = strROIName;

                //Attach Pocket ROI to Search ROI
                m_smVisionInfo.g_arrPositioningROIs[intArrayIndex].AttachImage(m_smVisionInfo.g_arrPositioningROIs[0]);

                if (m_smVisionInfo.g_arrPositioningROIs[intArrayIndex].ref_ROIWidth == 0 || m_smVisionInfo.g_arrPositioningROIs[intArrayIndex].ref_ROIHeight == 0)
                {
                    m_smVisionInfo.g_arrPositioningROIs[intArrayIndex].ref_ROIWidth = 100;
                    m_smVisionInfo.g_arrPositioningROIs[intArrayIndex].ref_ROIHeight = 100;

                }

                for (int i = 2; i < m_smVisionInfo.g_arrPositioningROIs.Count; i++)
                {
                    if (m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName == strROIName)
                    {
                        m_smVisionInfo.g_arrPositioningROIs.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private void txt_SamplingStep_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_SamplingStep.Text == null || txt_SamplingStep.Text == "" || txt_SamplingStep.Text == " ")
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPositioningGauges.Count; i++)
            {
                m_smVisionInfo.g_arrPositioningGauges[i].ref_GaugeSamplingStep = float.Parse(txt_SamplingStep.Text);
            }
        }


        private void cbo_method_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            // Backup ROI if no same
            if (m_smVisionInfo.g_objPositioning.ref_intMethod != cbo_method.SelectedIndex)
            {
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

                    m_smVisionInfo.g_arrPositioningROIs.Clear();
                    for (int i = 0; i < m_arrPRSROI.Count; i++)
                    {
                        ROI objROI = new ROI();
                        m_arrPRSROI[i].CopyToNew(ref objROI);
                        if (m_smVisionInfo.g_arrPositioningROIs.Count <= i)
                            m_smVisionInfo.g_arrPositioningROIs.Add(objROI);
                        else
                            m_smVisionInfo.g_arrPositioningROIs[i] = objROI;
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

                    m_smVisionInfo.g_arrPositioningROIs.Clear();
                    for (int i = 0; i < m_arrLGaugeROI.Count; i++)
                    {
                        ROI objROI = new ROI();
                        m_arrLGaugeROI[i].CopyToNew(ref objROI);
                        if (m_smVisionInfo.g_arrPositioningROIs.Count <= i)
                            m_smVisionInfo.g_arrPositioningROIs.Add(objROI);
                        else
                            m_smVisionInfo.g_arrPositioningROIs[i] = objROI;
                    }
                }
            }

            m_smVisionInfo.g_objPositioning.ref_intMethod = cbo_method.SelectedIndex;
        }


        private void btn_GaugeAdvanceSetting_Click(object sender, EventArgs e)
        {
            AdvancedLGaugeForm objAdvancedForm = new AdvancedLGaugeForm(m_smVisionInfo, m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Positioning\\Gauge.xml",m_smProductionInfo, false);

            if (objAdvancedForm.ShowDialog() == DialogResult.OK)
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    m_smProductionInfo.g_blnSaveRecipeToServer = true;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objPositioning.LoadPosition(m_strFolderPath + "Settings.xml", "General");

            if (m_smVisionInfo.g_objPositioning.ref_intMethod == 0)
            {
                LGauge.LoadFile(m_strFolderPath + "\\Gauge.xml", m_smVisionInfo.g_arrPositioningGauges, m_smVisionInfo.g_WorldShape);
                ROI.LoadFile(m_strFolderPath + "\\ROI.xml", m_smVisionInfo.g_arrPositioningROIs);
            }
            else
            {
                m_smVisionInfo.g_objPositioning.LoadPattern(m_strFolderPath + "\\Template\\Template0.mch",
                    m_strFolderPath + "\\Template\\OrientTemplate0.mch");
                ROI.LoadFile(m_strFolderPath + "\\ROI.xml", m_smVisionInfo.g_arrPositioningROIs);
            }

            m_smVisionInfo.g_objPositioning.LoadEmptyPattern(m_strFolderPath + "\\Template\\EmptyTemplate0.mch");
            m_smVisionInfo.g_objPositioning.LoadEmptyThreshold(m_strFolderPath + "Settings.xml", "General");
            Polygon.LoadPolygon(m_strFolderPath + "\\Template\\Polygon.xml", m_smVisionInfo.g_arrPolygon);

            Close();
            Dispose();
        }
                
        private void btn_Next_Click(object sender, EventArgs e)
        {
            switch (m_smVisionInfo.g_intLearnStepNo)
            {
                case 0:
                    if (cbo_UnitType.SelectedIndex == 0)
                    {
                        if (cbo_method.SelectedIndex == 1) // Pattern Match is chosen
                            m_smVisionInfo.g_intLearnStepNo += 2;
                        else
                            m_smVisionInfo.g_intLearnStepNo++;  // Line Gauge
                    }
                    else
                    {
                        //if (m_smVisionInfo.g_blnWantUseEmptyPattern)
                            m_smVisionInfo.g_intLearnStepNo = 7;    // Go to Empty ROI
                        //else
                        //    m_smVisionInfo.g_intLearnStepNo = 9;    // Go to Empty Threshold
                    }
                    break;
                case 1: // Line Gauge Method
                case 6: // PRS method - Dont Care Step
                    if (m_blnWantLearnOrient)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 8;  // Orient ROI
                    }
                    break;
                case 3:
                    if (m_smVisionInfo.g_strVisionName == "TapePocketPosition" || m_smVisionInfo.g_strVisionName == "InPocketPkgPos")
                        m_smVisionInfo.g_intLearnStepNo++;
                    else
                        m_smVisionInfo.g_intLearnStepNo += 2;
                    break;
                case 7:
                    m_smVisionInfo.g_intLearnStepNo=9;
                    break;
                default:
                    m_smVisionInfo.g_intLearnStepNo++;
                    break;
            }
                    
            m_intDisplayStepNo++;
            SetupSteps();

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Previous_Click(object sender, EventArgs e)
        {
            switch (m_smVisionInfo.g_intLearnStepNo)
            {
                case 2:
                    if (cbo_method.SelectedIndex == 1) // Pattern Match is being chosen
                        m_smVisionInfo.g_intLearnStepNo -= 2;
                    else
                        m_smVisionInfo.g_intLearnStepNo--;
                    break;
                case 5:
                    if (m_smVisionInfo.g_strVisionName == "TapePocketPosition" || m_smVisionInfo.g_strVisionName == "InPocketPkgPos")
                        m_smVisionInfo.g_intLearnStepNo--;
                    else 
                        m_smVisionInfo.g_intLearnStepNo -= 2;
                    break;
                case 7:
                    m_smVisionInfo.g_intLearnStepNo = 0;
                    break;
                case 8:
                    if (cbo_method.SelectedIndex == 0) // Line Gauge method
                        m_smVisionInfo.g_intLearnStepNo = 1; // Go to Line Gauge Page
                    else // PRS method 
                        m_smVisionInfo.g_intLearnStepNo = 6; ;  // Go to Dont care page
                    break;
                case 9:
                    //if (m_smVisionInfo.g_blnWantUseEmptyPattern)
                        m_smVisionInfo.g_intLearnStepNo = 7;
                   
                    break;
                default:
                    m_smVisionInfo.g_intLearnStepNo--;
                    break;
            }

            m_intDisplayStepNo--;
            SetupSteps();

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Rotate_Click(object sender, EventArgs e)
        {
            if (txt_RotateAngle.Text == null || txt_RotateAngle.Text == "")
                return;

            float fAngle = float.Parse(txt_RotateAngle.Text);

            m_smVisionInfo.g_arrPositioningROIs[0].AttachImage(m_smVisionInfo.g_arrImages[0]);
            if (sender == btn_ClockWise)          
                ImageDrawing.Rotate0Degree(m_smVisionInfo.g_arrImages[0], -fAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);           
            else if (sender == btn_CounterClockWise)
                ImageDrawing.Rotate0Degree(m_smVisionInfo.g_arrImages[0], fAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);  

            m_smVisionInfo.g_arrPositioningROIs[0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_PRSSave_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objPositioning.ref_intMethod = 1;

            if (m_smVisionInfo.g_strVisionName == "TapePocketPosition" || m_smVisionInfo.g_strVisionName == "InPocketPkgPos")
            {
                float fTrainROICenterX = 0, fTrainROICenterY = 0, fPocketROICenterX = 0, fPocketROICenterY = 0;
                for (int i = 0; i < m_smVisionInfo.g_arrPositioningROIs.Count; i++)
                {

                    if (m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName == "Train ROI")
                    {
                        fTrainROICenterX = m_smVisionInfo.g_arrPositioningROIs[i].ref_ROITotalX + (float)m_smVisionInfo.g_arrPositioningROIs[i].ref_ROIWidth / 2;
                        fTrainROICenterY = m_smVisionInfo.g_arrPositioningROIs[i].ref_ROITotalY + (float)m_smVisionInfo.g_arrPositioningROIs[i].ref_ROIHeight / 2;
                    }


                    if (m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName == "Pocket ROI")
                    {
                        fPocketROICenterX = m_smVisionInfo.g_arrPositioningROIs[i].ref_ROITotalX + (float)m_smVisionInfo.g_arrPositioningROIs[i].ref_ROIWidth / 2;
                        fPocketROICenterY = m_smVisionInfo.g_arrPositioningROIs[i].ref_ROITotalY + (float)m_smVisionInfo.g_arrPositioningROIs[i].ref_ROIHeight / 2;
                    }
                }


                m_smVisionInfo.g_objPositioning.ref_fCompensateX = fPocketROICenterX - fTrainROICenterX;
                m_smVisionInfo.g_objPositioning.ref_fCompensateY = fPocketROICenterY - fTrainROICenterY;
            }
            else
            {
                m_smVisionInfo.g_objPositioning.ref_fCompensateX = 0;
                m_smVisionInfo.g_objPositioning.ref_fCompensateY = 0;
            }

            int intTrainROIIndex = 1;   // Position ROI
            int intOrientROIIndex = 1;
            for (int i = 0; i < m_smVisionInfo.g_arrPositioningROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName == "Train ROI")
                    intTrainROIIndex = i;

                if (m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName == "Orient ROI")
                    intOrientROIIndex = i;
            }

            m_smVisionInfo.g_objPositioning.ref_fOrientPosX = (float)m_smVisionInfo.g_arrPositioningROIs[intTrainROIIndex].ref_ROIWidth / 2-
                                                              m_smVisionInfo.g_arrPositioningROIs[intOrientROIIndex].ref_ROICenterX;
            m_smVisionInfo.g_objPositioning.ref_fOrientPosY = (float)m_smVisionInfo.g_arrPositioningROIs[intTrainROIIndex].ref_ROIHeight / 2 -
                                                              m_smVisionInfo.g_arrPositioningROIs[intOrientROIIndex].ref_ROICenterY;

            
            STDeviceEdit.CopySettingFile(m_strFolderPath, "Settings.xml");
            m_smVisionInfo.g_objPositioning.SavePosition(m_strFolderPath + "Settings.xml", false, "General", true);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Position", m_smProductionInfo.g_strLotID);

            m_smVisionInfo.g_objPositioning.LearnPatternWithPolygon(m_smVisionInfo.g_arrPositioningROIs[intTrainROIIndex], 
                m_smVisionInfo.g_arrPositioningROIs[intOrientROIIndex], m_smVisionInfo.g_arrPolygon[0][0]);
            
            int intTotalROICount = m_smVisionInfo.g_arrPositioningROIs.Count;
            for (int i = 4; i < intTotalROICount; i++)
            {
                m_smVisionInfo.g_arrPositioningROIs.RemoveAt(3);
            }
            STDeviceEdit.CopySettingFile(m_strFolderPath, "ROI.xml");
            ROI.SaveFile(m_strFolderPath + "\\ROI.xml",m_smVisionInfo.g_arrPositioningROIs);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Position ROI", m_smProductionInfo.g_strLotID);
            // Save template images
            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].SaveImage(m_strFolderPath + "\\Template\\OriTemplate.bmp");
            m_smVisionInfo.g_objPositioning.SavePattern(m_strFolderPath + "\\Template\\Template0.mch",
                m_strFolderPath + "\\Template\\OrientTemplate0.mch");
            m_smVisionInfo.g_arrPositioningROIs[intTrainROIIndex].SaveImage(m_strFolderPath + "\\Template\\Template0.bmp");
            m_smVisionInfo.g_arrPositioningROIs[intOrientROIIndex].SaveImage(m_strFolderPath + "\\Template\\OrientTemplate0.bmp");

            STDeviceEdit.CopySettingFile(m_strFolderPath, "\\Template\\Polygon.xml");
            Polygon.SavePolygon(m_strFolderPath + "\\Template\\Polygon.xml", m_smVisionInfo.g_arrPolygon);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Position Polygon", m_smProductionInfo.g_strLotID);
            
            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            Close();
            Dispose();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            // Save template images
            m_smVisionInfo.g_arrImages[0].SaveImage(m_strFolderPath + "\\Template\\OriTemplate.bmp");

            ROI objUnitROI = new ROI();
            objUnitROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
            objUnitROI.LoadAngleROISetting(m_smVisionInfo.g_objPositioning.ref_fObjectAngle,
                                m_smVisionInfo.g_objPositioning.ref_fObjectCenter.X, m_smVisionInfo.g_objPositioning.ref_fObjectCenter.Y,
                                m_smVisionInfo.g_objPositioning.ref_fObjectWidth, m_smVisionInfo.g_objPositioning.ref_fObjectHeight,
                                0, 0);
            objUnitROI.SaveImage(m_strFolderPath + "\\Template\\Template0.bmp");

            m_smVisionInfo.g_objPositioning.ref_intMethod = 0;

            // Save Setting
            
            STDeviceEdit.CopySettingFile(m_strFolderPath, "Settings.xml");
            m_smVisionInfo.g_objPositioning.SavePosition(m_strFolderPath + "Settings.xml", false, "General", true);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Position", m_smProductionInfo.g_strLotID);
            //XmlParser objFile = new XmlParser(m_strFolderPath + "\\Settings.xml");
            //objFile.WriteSectionElement("General");
            //objFile.WriteElement1Value("Method", cbo_method.SelectedIndex);
            //objFile.WriteElement1Value("DieWidth", Convert.ToSingle(lbl_DieWidth.Text));
            //objFile.WriteElement1Value("DieHeight", Convert.ToSingle(lbl_DieHeight.Text));
            //objFile.WriteEndElement();

            STDeviceEdit.CopySettingFile(m_strFolderPath, "ROI.xml");
            ROI.SaveFile(m_strFolderPath + "\\ROI.xml", m_smVisionInfo.g_arrPositioningROIs);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Position ROI", m_smProductionInfo.g_strLotID);

            STDeviceEdit.CopySettingFile(m_strFolderPath, "Gauge.xml");
            LGauge.SaveFile(m_strFolderPath + "\\Gauge.xml", m_smVisionInfo.g_arrPositioningGauges);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Position Gauge", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_objPositioning.ref_fSampleDieWidth = Convert.ToInt32(lbl_DieWidth.Text);
            m_smVisionInfo.g_objPositioning.ref_fSampleDieHeight = Convert.ToInt32(lbl_DieHeight.Text);

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            Close();
            Dispose();
        }




        private void LearnPositionForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
            m_smVisionInfo.g_objPositioning.LoadEmptyThreshold(m_strFolderPath + "Settings.xml", "General");
            
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
            if (m_smVisionInfo.g_blnWantCheckEmpty)
            {
                m_smVisionInfo.g_blnViewEmptyImage = true;

                if (m_smVisionInfo.g_blnWantUseEmptyPattern)
                    m_smVisionInfo.g_intLearnStepNo = 7;
                else if (m_smVisionInfo.g_blnWantUseEmptyThreshold)
                    // 2020 01 14 - CCEGNG : Chagne from 9 to 7. 
                    //              Display Pocket ROI page also even though no using Pocket Pattern. THis Pocket ROI will use to calculate pixel area. 
                    m_smVisionInfo.g_intLearnStepNo = 7;    
            }
            lbl_StepNo.BringToFront();
            SetupSteps();
           
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.g_blncboImageView = false;
        }

        private void LearnPositionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            if (m_smVisionInfo.g_blnWantCheckEmpty)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Learn Empty Form Closed", "Exit Learn Empty Form", "", "", m_smProductionInfo.g_strLotID);
            }
            else
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Learn Position Form Closed", "Exit Learn Position Form", "", "", m_smProductionInfo.g_strLotID);
            }
            
            m_smVisionInfo.g_blnViewEmptyImage = false;
            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_blnViewSearchROI = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_objPositioning.ref_bFinalResult = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
            m_smVisionInfo.g_blnViewDontCareArea = false;
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

            if (m_smVisionInfo.AT_VM_UpdateResult)
            {
                if (m_smVisionInfo.g_objPositioning.ref_intMethod == 0)
                {
                    if (m_smVisionInfo.g_arrPositioningGauges.Count >= 3)
                    {

                        double fTotalAngle = 0;
                        for (int i = 0; i < m_smVisionInfo.g_arrPositioningROIs.Count; i++)
                        {
                            if (m_smVisionInfo.g_arrPositioningROIs[i].GetROIHandle())
                            {
                                lbl_ValidateScore.Text = m_smVisionInfo.g_arrPositioningGauges[i].ref_ObjectScore.ToString("f2");
                                lbl_LineAngle.Text = m_smVisionInfo.g_arrPositioningGauges[i].ref_ObjectAngle.ToString("f2");
                            }

                            if (i < m_smVisionInfo.g_arrPositioningGauges.Count)
                            {
                                fTotalAngle += m_smVisionInfo.g_arrPositioningGauges[i].ref_ObjectAngle;
                                m_smVisionInfo.g_arrPositioningGauges[i].GetObjectLine();
                            }
                        }

                        PointF pTopRightCorner = Line.GetCrossPoint(m_smVisionInfo.g_arrPositioningGauges[0].ref_ObjectLine, m_smVisionInfo.g_arrPositioningGauges[1].ref_ObjectLine);
                        PointF pTopLeftCorner = Line.GetCrossPoint(m_smVisionInfo.g_arrPositioningGauges[0].ref_ObjectLine, m_smVisionInfo.g_arrPositioningGauges[3].ref_ObjectLine);
                        PointF pBottomRightCorner = Line.GetCrossPoint(m_smVisionInfo.g_arrPositioningGauges[2].ref_ObjectLine, m_smVisionInfo.g_arrPositioningGauges[1].ref_ObjectLine);
                        PointF pBottomLeftCorner = Line.GetCrossPoint(m_smVisionInfo.g_arrPositioningGauges[2].ref_ObjectLine, m_smVisionInfo.g_arrPositioningGauges[3].ref_ObjectLine);

                        double intHeight1 = Math.Sqrt(Math.Pow(pBottomLeftCorner.X - pTopLeftCorner.X, 2) + Math.Pow(pBottomLeftCorner.Y - pTopLeftCorner.Y, 2));
                        double intHeight2 = Math.Sqrt(Math.Pow(pBottomRightCorner.X - pTopRightCorner.X, 2) + Math.Pow(pBottomRightCorner.Y - pTopRightCorner.Y, 2));
                        double intWidth1 = Math.Sqrt(Math.Pow(pTopRightCorner.X - pTopLeftCorner.X, 2) + Math.Pow(pTopRightCorner.Y - pTopLeftCorner.Y, 2));
                        double intWidth2 = Math.Sqrt(Math.Pow(pBottomRightCorner.X - pBottomLeftCorner.X, 2) + Math.Pow(pBottomRightCorner.Y - pBottomLeftCorner.Y, 2));

                        lbl_DieWidth.Text = Math.Round((Math.Max(intWidth1, intWidth2) / m_smVisionInfo.g_fCalibPixelXInUM)).ToString("f0");
                        lbl_DieHeight.Text = Math.Round((Math.Max(intHeight1, intHeight2) / m_smVisionInfo.g_fCalibPixelYInUM)).ToString("f0");
                        lbl_DieAngle.Text = (fTotalAngle / 4).ToString("f2");
                    }
                }
                m_smVisionInfo.AT_VM_UpdateResult = false;
            }
        }

        private void cbo_UnitType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (cbo_UnitType.SelectedIndex == 0)
            {
                cbo_method.Visible = true;
                srmLabel2.Visible = true;

                if (cbo_ImageNo.Items.Count > 0)
                    cbo_ImageNo.SelectedIndex = m_smVisionInfo.g_objPositioning.ref_intPositionImageIndex;
            }
            else
            {
                cbo_method.Visible = false;
                srmLabel2.Visible = false;

                if (cbo_ImageNo.Items.Count > 0)
                    cbo_ImageNo.SelectedIndex = m_smVisionInfo.g_objPositioning.ref_intEmptyImageIndex;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_SaveEmptyPattern_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objPositioning.LearnEmptyPattern(m_smVisionInfo.g_arrPositioningROIs[2]);// [2] to [0] 

            int intTotalROICount = m_smVisionInfo.g_arrPositioningROIs.Count;
            for (int i = 3; i < intTotalROICount; i++)
            {
                m_smVisionInfo.g_arrPositioningROIs.RemoveAt(3);
            }

            
            STDeviceEdit.CopySettingFile(m_strFolderPath, "ROI.xml");
            ROI.SaveFile(m_strFolderPath + "\\ROI.xml", m_smVisionInfo.g_arrPositioningROIs);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Empty ROI", m_smProductionInfo.g_strLotID);
            
            CopyFiles m_objCopy = new CopyFiles();
            string strCurrentDateTIme = DateTime.Now.ToString();
            DateTime DT = Convert.ToDateTime(strCurrentDateTIme);
            string strCurrentDateTIme1 = strCurrentDateTIme.Replace(':', '.');
            string strCurrentDateTIme2 = strCurrentDateTIme.ToString();
            string strPath = m_smProductionInfo.g_strHistoryDataLocation + "Data\\" + DT.ToString("yyyy-MM") + "\\" + strCurrentDateTIme1 + "\\" + m_strSelectedRecipe + "-" + m_smVisionInfo.g_strVisionFolderName + "-Learn Empty\\";

            if (File.Exists(m_strFolderPath + "Template\\EmptyOriTemplate.bmp"))
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Empty", "EmptyOriTemplate.bmp", "EmptyOriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
            else
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Empty", "", "EmptyOriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);

            m_objCopy.CopyAllImageFiles(m_strFolderPath + "Template\\", strPath + "Old\\");
            
            // Save template images
            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].SaveImage(m_strFolderPath + "\\Template\\EmptyOriTemplate.bmp");
            m_smVisionInfo.g_objPositioning.SaveEmptyPattern(m_strFolderPath + "\\Template\\EmptyTemplate0.mch");
            m_smVisionInfo.g_arrPositioningROIs[2].SaveImage(m_strFolderPath + "\\Template\\EmptyTemplate0.bmp"); // [2] to [0] 

            m_objCopy.CopyAllImageFiles(m_strFolderPath + "Template\\", strPath + "New\\");
          

            Close();
            Dispose();
        }

        private void cbo_DontCareAreaDrawMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_arrPolygon[0][m_smVisionInfo.g_intSelectedTemplate].ref_intFormMode = cbo_DontCareAreaDrawMethod.SelectedIndex;
        }

        private void btn_Undo_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_arrPolygon[0][m_smVisionInfo.g_intSelectedTemplate].UndoPolygon();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_AdvancedSettings_Click(object sender, EventArgs e)
        {
            AdvancedPositionForm objAdvancedPositionForm = new AdvancedPositionForm(m_smVisionInfo, m_strSelectedRecipe, m_intUserGroup, m_smProductionInfo);

            if (objAdvancedPositionForm.ShowDialog() == DialogResult.OK)
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    m_smProductionInfo.g_blnSaveRecipeToServer = true;
            }

            objAdvancedPositionForm.Dispose();
        }

        private void cbo_ImageNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbo_UnitType.SelectedIndex == 0)
            {
                m_smVisionInfo.g_objPositioning.ref_intPositionImageIndex = cbo_ImageNo.SelectedIndex;

                if (m_smVisionInfo.g_objPositioning2 != null)
                    m_smVisionInfo.g_objPositioning2.ref_intPositionImageIndex = cbo_ImageNo.SelectedIndex;

                m_smVisionInfo.g_intSelectedImage = cbo_ImageNo.SelectedIndex; ;
            }
            else
            {
                m_smVisionInfo.g_objPositioning.ref_intEmptyImageIndex = cbo_ImageNo.SelectedIndex;

                if (m_smVisionInfo.g_objPositioning2 != null)
                    m_smVisionInfo.g_objPositioning2.ref_intEmptyImageIndex = cbo_ImageNo.SelectedIndex;

                m_smVisionInfo.g_intSelectedImage = cbo_ImageNo.SelectedIndex; ;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

     
    }
}