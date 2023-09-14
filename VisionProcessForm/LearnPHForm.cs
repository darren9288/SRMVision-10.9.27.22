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
    public partial class LearnPHForm : Form
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
        private ProductionInfo m_smProductionInfo;
        private List<ROI> m_arrPRSROI = new List<ROI>();

        private UserRight m_objUserRight = new UserRight();
        #endregion
        public LearnPHForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;
            m_smCustomizeInfo = smCustomizeInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = smVisionInfo;
            
            txt_MinBlackArea.Text = m_smVisionInfo.g_objPositioning.ref_intPHBlackArea.ToString();
            txt_MinArea.Text = m_smVisionInfo.g_objPositioning.ref_intPHMinArea.ToString();
            m_strFolderPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Positioning\\";
            DisableField2();
        }
        private void DisableField()
        {
            string strChild1 = "Learn Page";
            string strChild2 = "Save Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_SavePH.Enabled = false;

            }
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild2 = "Learn PH Page";
            string strChild3 = "Save Button";

            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrientPad":
                case "BottomOPadPkg":
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild2, strChild3))
                    {
                        btn_SavePH.Enabled = false;
                    }

                    strChild3 = "PH Threshold";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild2, strChild3))
                    {
                        btn_PHThreshold.Enabled = false;

                    }

                    strChild3 = "PH Min Area";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild2, strChild3))
                    {
                        txt_MinArea.Enabled = false;

                    }

                    strChild3 = "PH Min Black Area";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild2, strChild3))
                    {
                        txt_MinBlackArea.Enabled = false;

                    }

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

                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild2, strChild3))
                    {
                        btn_SavePH.Enabled = false;
                    }

                    strChild3 = "PH Threshold";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild2, strChild3))
                    {
                        btn_PHThreshold.Enabled = false;

                    }

                    strChild3 = "PH Min Area";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild2, strChild3))
                    {
                        txt_MinArea.Enabled = false;

                    }

                    strChild3 = "PH Min Black Area";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild2, strChild3))
                    {
                        txt_MinBlackArea.Enabled = false;

                    }

                    break;
                case "Li3D":
                case "Li3DPkg":

                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild2, strChild3))
                    {
                        btn_SavePH.Enabled = false;
                    }

                    strChild3 = "PH Threshold";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild2, strChild3))
                    {
                        btn_PHThreshold.Enabled = false;

                    }

                    strChild3 = "PH Min Area";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild2, strChild3))
                    {
                        txt_MinArea.Enabled = false;

                    }

                    strChild3 = "PH Min Black Area";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild2, strChild3))
                    {
                        txt_MinBlackArea.Enabled = false;

                    }

                    break;
            }
        }
        private void LearnPHForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
            m_smVisionInfo.g_objPositioning.LoadPHSetting(m_strFolderPath + "Settings.xml", "General");
            
            m_smVisionInfo.g_intLearnStepNo = 0;
            m_smVisionInfo.g_blnViewPHImage = true;

            lbl_StepNo.BringToFront();
            SetupSteps();

            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.g_blncboImageView = false;
        }

        private void LearnPHForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Learn PH Form Closed", "Exit Learn PH Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_blnViewPHImage = false;
            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_blnViewPHObjectBuilded = false;
            m_smVisionInfo.g_blnViewSearchROI = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
            m_smVisionInfo.g_blnViewDontCareArea = false;
        }
        private void AddROI(string strROIName, int intROIType)
        {
            if (m_smVisionInfo.g_arrPHROIs.Count == 0)
            {
                m_smVisionInfo.g_arrPHROIs.Add(new ROI());
                m_smVisionInfo.g_arrPHROIs[0].ref_strROIName = strROIName;
                m_smVisionInfo.g_arrPHROIs[0].ref_intType = intROIType;
                m_smVisionInfo.g_arrPHROIs[0].ref_ROIPositionX = 100;
                m_smVisionInfo.g_arrPHROIs[0].ref_ROIPositionY = 100;
                m_smVisionInfo.g_arrPHROIs[0].ref_ROIWidth = 100;
                m_smVisionInfo.g_arrPHROIs[0].ref_ROIHeight = 100;
            

            }

            if (strROIName == "PH ROI")
            {
                // Attach Search ROI to image
                m_smVisionInfo.g_arrPHROIs[0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            }
            
        }
        private void SetupSteps()
        {
            lbl_TitleStep1.Text = "Learn PH";
            lbl_TitleStep1.BringToFront();
            tabCtrl_Setup.SelectedTab = tp_Step1;
            lbl_PHThreshold.Text = m_smVisionInfo.g_objPositioning.ref_intPHThreshold.ToString();

            AddROI("PH ROI", 1);

            m_smVisionInfo.g_arrPHROIs[0].AttachImage(m_smVisionInfo.g_arrImages[0]);

            m_smVisionInfo.g_objPositioning.BuildObjects(m_smVisionInfo.g_arrPHROIs[0]);


            lbl_PHBlackArea.Text = m_smVisionInfo.g_objPositioning.ref_intPHBlobBlackArea.ToString();
            m_smVisionInfo.g_blnViewPHObjectBuilded = true;
            m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.g_blnViewSearchROI = true;
            m_smVisionInfo.g_blnDragROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

     

        private void btn_SavePH_Click(object sender, EventArgs e)
        {
            CopyFiles m_objCopy = new CopyFiles();
            string strCurrentDateTIme = DateTime.Now.ToString();
            DateTime DT = Convert.ToDateTime(strCurrentDateTIme);
            string strCurrentDateTIme1 = strCurrentDateTIme.Replace(':', '.');
            string strCurrentDateTIme2 = strCurrentDateTIme.ToString();
            string strPath = m_smProductionInfo.g_strHistoryDataLocation + "Data\\" + DT.ToString("yyyy-MM") + "\\" + strCurrentDateTIme1 + "\\" + m_strSelectedRecipe + "-" + m_smVisionInfo.g_strVisionFolderName + "-Learn PH\\";
            

            if (File.Exists(m_strFolderPath + "Template\\PHOriTemplate.bmp"))
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn PH", "PHOriTemplate.bmp", "PHOriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
            else
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn PH", "", "PHOriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
            
            m_objCopy.CopyAllImageFiles(m_strFolderPath + "Template\\", strPath + "Old\\");
            
            // Save template images
            m_smVisionInfo.g_arrImages[0].SaveImage(m_strFolderPath + "\\Template\\PHOriTemplate.bmp");

            m_smVisionInfo.g_arrPHROIs[0].SaveImage(m_strFolderPath + "\\Template\\PHTemplate0.bmp"); // [2] to [0] 

           m_objCopy.CopyAllImageFiles(m_strFolderPath + "Template\\", strPath + "New\\");
            

            m_smVisionInfo.g_objPositioning.ref_intMethod = 0;

            // Save Setting
            if (txt_MinBlackArea.Text == "")
            {
                SRMMessageBox.Show("Minimum Black Area cannot be empty!");
                return;
            }
            m_smVisionInfo.g_objPositioning.ref_intPHBlackArea = Convert.ToInt32(txt_MinBlackArea.Text);
            m_smVisionInfo.g_objPositioning.ref_intPHMinArea = Convert.ToInt32(txt_MinArea.Text);
            m_smVisionInfo.g_objPositioning.SavePosition(m_strFolderPath + "Settings.xml", false, "General", true);
           
            ROI.SaveFile(m_strFolderPath + "\\PHROI.xml", m_smVisionInfo.g_arrPHROIs);

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            Close();
            Dispose();
        }

        private void btn_PHThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPHObjectBuilded = false;
            Position objPosition = m_smVisionInfo.g_objPositioning;
            int intThreshold = m_smVisionInfo.g_intThresholdValue = objPosition.ref_intPHThreshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPHROIs[0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
                objPosition.ref_intPHThreshold = intThreshold;
            else
            {
                objPosition.ref_intPHThreshold = m_smVisionInfo.g_intThresholdValue;
                lbl_PHThreshold.Text = objPosition.ref_intPHThreshold.ToString();
                
               
            }
            
            objThresholdForm.Dispose();

             objPosition.BuildObjects(m_smVisionInfo.g_arrPHROIs[0]);
            m_smVisionInfo.g_blnViewPHObjectBuilded = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            if(File.Exists(m_strFolderPath + "\\PHROI.xml"))
                ROI.LoadFile(m_strFolderPath + "\\PHROI.xml", m_smVisionInfo.g_arrPHROIs);
            if (File.Exists(m_strFolderPath + "\\Settings.xml"))
                m_smVisionInfo.g_objPositioning.LoadPHSetting(m_strFolderPath + "Settings.xml", "General");
         

            Close();
            Dispose();
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

            lbl_PHBlackArea.Text = m_smVisionInfo.g_objPositioning.ref_intPHBlobBlackArea.ToString();
            if(m_smVisionInfo.g_blnViewPHObjectBuilded == true)
                m_smVisionInfo.g_objPositioning.BuildObjects(m_smVisionInfo.g_arrPHROIs[0]);
        }
    }
}
