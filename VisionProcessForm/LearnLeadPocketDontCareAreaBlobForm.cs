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
using SharedMemory;
using VisionProcessing;
using System.IO;
using Microsoft.Win32;

namespace VisionProcessForm
{
    public partial class LearnLeadPocketDontCareAreaBlobForm : Form
    {
        #region Member Variables

        private bool m_blnBlockOtherControlUpdate = false;
        private bool m_blnInitDone = false;
        private int m_intUserGroup = 5;
        private int m_intDisplayStepNo = 1;
        private string m_strSelectedRecipe;
        private string m_strPosition = "";
        private List<ImageDrawing> m_arrCurrentImage = new List<ImageDrawing>();
        private int m_intSelectedImagePrev = 0;
        private VisionInfo m_smVisionInfo;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        private int m_intSelectedROI = 0;

        #endregion

        public LearnLeadPocketDontCareAreaBlobForm(VisionInfo visionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup)
        {
            InitializeComponent();


            m_smVisionInfo = visionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            
            m_strSelectedRecipe = strSelectedRecipe;
            m_intUserGroup = intUserGroup;
            m_intSelectedROI = m_smVisionInfo.g_intSelectedROI;
            DisableField2();

            chk_TopROI.Checked = (m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x01) > 0;
            chk_RightROI.Checked = (m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x02) > 0;
            chk_BottomROI.Checked = (m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x04) > 0;
            chk_LeftROI.Checked = (m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x08) > 0;

            //CustomizeGUI();
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;
            m_blnInitDone = true;
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "Lead";
            string strChild2 = "Learn Dont Care Area Blob";
            string strChild3 = "Save Button";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_Save.Enabled = false;
            }

            strChild3 = "ROI Direction";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                gb_ROIDirection.Enabled = false;
            }

            strChild3 = "Dont Care Area Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                gbox_Setting.Enabled = false;
            }

        }
        private int GetUserRightGroup_Child3(string Child1, string Child2, string Child3)
        {
            //NewUserRight objUserRight = new NewUserRight(false);
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Orient":
                case "BottomOrient":
                    Child1 = "Orient";
                    return m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(Child1, Child2, Child3);
                    break;
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                case "Package":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetMarkOrientChild3Group(Child1, Child2, Child3);
                    break;
                case "IPMLi":
                case "IPMLiPkg":
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                    return m_smCustomizeInfo.objNewUserRight.GetInPocketChild3Group(Child1, Child2, Child3, m_smVisionInfo.g_intVisionNameNo);
                    break;
            }

            return 1;
        }
        private void CustomizeGUI()
        {
            txt_Threshold.Text = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intDontCareBlobThreshold.ToString();
            trackBar_Threshold.Value = Convert.ToInt32(Convert.ToSingle(txt_Threshold.Text));
            txt_MaxArea.Text = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intMaxShadowArea.ToString();
            txt_MinArea.Text = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intMinShadowArea.ToString();
            txt_ROIInward.Text = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intDontCareBlobROIInward.ToString();
            txt_MaxWidth.Text = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intMaxShadowWidth.ToString();
            chk_FlipToOppositeFunction.Checked = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_blnFlipToOppositeFunction;

            chk_ApplyToAllSide.Checked = true;  // 2021 09 02 - CCENG: Default set to all bcos most of the time, the setting are same for both size.

            UpdatePocketEdgeDontCareImage();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void UpdateROIComboBox()
        {
            cbo_ROI.Items.Clear();
            for (int i = 1; i < m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob.Count; i++)
            {
                if (i == 1 && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x01) > 0))
                    cbo_ROI.Items.Add("Top");
                else if (i == 2 && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x02) > 0))
                    cbo_ROI.Items.Add("Right");
                else if (i == 3 && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x04) > 0))
                    cbo_ROI.Items.Add("Bottom");
                else if (i == 4 && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x08) > 0))
                    cbo_ROI.Items.Add("Left");
            }

            if (cbo_ROI.Items.Count > 0)
            {
                cbo_ROI.SelectedIndex = 0;

                if (cbo_ROI.SelectedItem.ToString() == "Top")
                    m_intSelectedROI = m_smVisionInfo.g_intSelectedROI = 1;
                else if (cbo_ROI.SelectedItem.ToString() == "Right")
                    m_intSelectedROI = m_smVisionInfo.g_intSelectedROI = 2;
                if (cbo_ROI.SelectedItem.ToString() == "Bottom")
                    m_intSelectedROI = m_smVisionInfo.g_intSelectedROI = 3;
                if (cbo_ROI.SelectedItem.ToString() == "Left")
                    m_intSelectedROI = m_smVisionInfo.g_intSelectedROI = 4;
            }
            
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void LearnLeadPocketDontCareAreaForm_Load(object sender, EventArgs e)
        {
            m_intSelectedImagePrev = m_smVisionInfo.g_intSelectedImage;
            m_smVisionInfo.g_blnDrawMarkResult = false;
            m_smVisionInfo.g_blnDrawPin1Result = false;
            m_smVisionInfo.g_blnDrawMark2DCodeResult = false;
            m_smVisionInfo.g_blnDrawPkgResult = false;
            m_smVisionInfo.g_blnViewOrientObject = false;
            m_smVisionInfo.g_blnPackageInspected = false;
            m_smVisionInfo.g_blnViewLeadInspection = false;
            //m_smVisionInfo.g_blnLeadInspected = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;

            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                if (i < m_smVisionInfo.g_arrblnImageRotated.Length)
                    m_smVisionInfo.g_arrblnImageRotated[i] = true;
            }
            // Start Setup step 1
            Cursor.Current = Cursors.Default;
            m_smVisionInfo.g_intLearnStepNo = 0;
            SetupSteps(true);


            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.g_blncboImageView = false;

        }

        private void LearnLeadPocketDontCareAreaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.g_intSelectedImage = m_intSelectedImagePrev;
            //m_STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Learn Lead Form Closed", "Exit Learn Lead Form", "", "", m_smProductionInfo.g_strLotID);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.g_intLearnStepNo = 0;
            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;

        }

        private void btn_Save_Click(object sender, EventArgs e)
        {

            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            //m_smVisionInfo.g_intSelectedImage = 0;
            ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);

            SaveLeadSetting();

            SaveROISetting(strFolderPath + "Lead\\PocketDontCareROIBlob.xml");

            LoadROISetting(strFolderPath + "Lead\\PocketDontCareROIBlob.xml", m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob, 5);
            LoadLeadSetting();

            m_smVisionInfo.AT_PR_AttachImagetoROI = true;

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            this.Close();
            this.Dispose();
        }

        private void SaveLeadSetting()
        {
            
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                               m_smVisionInfo.g_strVisionFolderName + "\\Lead\\";
            
            if (((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x02) > 0) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x08) > 0))
                m_smVisionInfo.g_intLeadPocketDontCareROIBlobDistanceX = m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[2][0].ref_ROIPositionX - (m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[4][0].ref_ROIPositionX + m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[4][0].ref_ROIWidth);

            if (((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x01) > 0) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x04) > 0))
                m_smVisionInfo.g_intLeadPocketDontCareROIBlobDistanceY = m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[3][0].ref_ROIPositionY - (m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[1][0].ref_ROIPositionY + m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[1][0].ref_ROIHeight);

            //STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath + "Settings.xml");
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("LeadPocketDontCareROIMaskBlob", m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask);
            objFileHandle.WriteElement1Value("LeadPocketDontCareROIBlobDistanceX", m_smVisionInfo.g_intLeadPocketDontCareROIBlobDistanceX);
            objFileHandle.WriteElement1Value("LeadPocketDontCareROIBlobDistanceY", m_smVisionInfo.g_intLeadPocketDontCareROIBlobDistanceY);
            objFileHandle.WriteEndElement();
            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Advance Settings", strPath, "", m_smProductionInfo.g_strLotID);
            
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


                //
                STDeviceEdit.CopySettingFile(strPath, "Template\\Template.xml");
                m_smVisionInfo.g_arrLead[i].SaveLead(strPath + "Template\\Template.xml",
                    false, strSectionName, true);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Lead", m_smProductionInfo.g_strLotID);
                
            }
                
        }
        private void LoadLeadSetting()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\Lead\\";

            XmlParser objFileHandle = new XmlParser(strPath + "Settings.xml");
            objFileHandle.GetFirstSection("Advanced");
            m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask = objFileHandle.GetValueAsInt("LeadPocketDontCareROIMaskBlob", 0, 1);

            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
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
                
                m_smVisionInfo.g_arrLead[i].LoadLead(strPath + "Template\\Template.xml", strSectionName, m_smVisionInfo.g_arrImages.Count);
            }
        }

        private void SaveROISetting(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath, false);

            ROI objROI;
            for (int t = 0; t < m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob.Count; t++)
            {
                objFile.WriteSectionElement("Unit" + t, true);

                for (int j = 0; j < m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[t].Count; j++)
                {
                    objROI = m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[t][j];
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
        }
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            LoadROISetting(strFolderPath + "Lead\\PocketDontCareROIBlob.xml", m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob, 5);
            LoadLeadSetting();

            m_smVisionInfo.AT_PR_AttachImagetoROI = true;
            this.Close();
            this.Dispose();
        }
        private void LoadROISetting(string strPath, List<List<ROI>> arrROIs, int intROICount)
        {
            arrROIs.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount;
            ROI objROI;

            for (int i = 0; i < intROICount; i++)
            {
                arrROIs.Add(new List<ROI>());
                objFile.GetFirstSection("Unit" + i);
                intChildCount = objFile.GetSecondSectionCount();
                for (int j = 0; j < intChildCount; j++)
                {
                    objROI = new ROI();
                    objFile.GetSecondSection("ROI" + j);
                    objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 2);
                    objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 2);
                    objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 2);
                    objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 2);
                    objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 2);
                    objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 2);
                    objROI.ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 2);
                    objROI.ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 2);
                    objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 2));
                    if (objROI.ref_intType > 1)
                    {
                        objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                        objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                    }

                    arrROIs[i].Add(objROI);
                }
            }
        }
        private void btn_Previous_Click(object sender, EventArgs e)
        {
          
            if (m_smVisionInfo.g_intLearnStepNo > 0)
                m_smVisionInfo.g_intLearnStepNo--;
            
            m_intDisplayStepNo--;

            SetupSteps(false);

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_intLearnStepNo < 99)
                m_smVisionInfo.g_intLearnStepNo++;
            
            m_intDisplayStepNo++;

            SetupSteps(true);

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void SetupSteps(bool blnForward)
        {

            switch (m_smVisionInfo.g_intLearnStepNo)
            {
                case 0: //Define Don't Care Area
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    AddPocketDontCareROI(m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob);

                    for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {
                        if ((i == 1) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x01) == 0))
                            continue;
                        else if ((i == 2) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x02) == 0))
                            continue;
                        else if ((i == 3) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x04) == 0))
                            continue;
                        else if ((i == 4) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x08) == 0))
                            continue;

                        m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].LoadROISetting(m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROIPositionX, m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROIPositionY,
                                                                                                   m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROIHeight);
                    }
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;

                    lbl_StepNo.BringToFront();
                    lbl_Step1.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_PocketDontCare;
                    if (chk_TopROI.Checked || chk_BottomROI.Checked || chk_RightROI.Checked || chk_LeftROI.Checked)
                    {
                        btn_Next.Enabled = true;
                    }
                    else
                    {
                        btn_Next.Enabled = false;
                    }
                    btn_Previous.Enabled = false;
                    break;
                case 1: //Define Don't Care Area Setting
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    UpdateROIComboBox();
                    CustomizeGUI();

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = false;

                    lbl_Step2.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_Setting;
                    btn_Next.Enabled = false;
                    btn_Previous.Enabled = true;
                    break;
                default:
                    break;
            }
        }

        private void AddPocketDontCareROI(List<List<ROI>> arrROIs)
        {
            ROI objROI;

            for (int i = 1; i < arrROIs.Count; i++)
            {
                if (arrROIs[i].Count == 0)
                {
                    if (i == 1)
                    {
                        objROI = new ROI("T", 1);
                        int intPostX = (640 / 2) - 65;
                        int intPostY = (480 / 2) - 180;
                        objROI.LoadROISetting(intPostX, intPostY, 150, 50);
                    }
                    else if (i == 2)
                    {
                        objROI = new ROI("R", 1);
                        int intPostXR = (640 / 2) + 120;
                        int intPostYR = (480 / 2) - 70;
                        objROI.LoadROISetting(intPostXR, intPostYR, 50, 150);
                    }
                    else if (i == 3)
                    {
                        objROI = new ROI("B", 1);
                        int intPostXB = (640 / 2) - 65;
                        int intPostYB = (480 / 2) + 120;
                        objROI.LoadROISetting(intPostXB, intPostYB, 150, 50);
                    }
                    else
                    {
                        objROI = new ROI("L", 1);
                        int intPostXL = (640 / 2) - 170;
                        int intPostYL = (480 / 2) - 70;
                        objROI.LoadROISetting(intPostXL, intPostYL, 50, 150);
                    }

                    if (m_smVisionInfo.g_blnViewPackageImage)
                        objROI.AttachImage(m_smVisionInfo.g_objPackageImage);
                    else
                        objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);

                    arrROIs[i].Add(objROI);
                }
                else
                {
                    if (m_smVisionInfo.g_blnViewPackageImage)
                        arrROIs[i][0].AttachImage(m_smVisionInfo.g_objPackageImage);
                    else
                        arrROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
                }
            }
        }

        private void chk_ROI_Click(object sender, EventArgs e)
        {
            switch(((CheckBox)sender).Name.ToString())
            {
                case "chk_TopROI":
                    if (((CheckBox)sender).Checked)
                        m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask |= 0x01;
                    else
                        m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask &= ~0x01;
                    break;
                case "chk_RightROI":
                    if (((CheckBox)sender).Checked)
                        m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask |= 0x02;
                    else
                        m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask &= ~0x02;
                    break;
                case "chk_BottomROI":
                    if (((CheckBox)sender).Checked)
                        m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask |= 0x04;
                    else
                        m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask &= ~0x04;
                    break;
                case "chk_LeftROI":
                    if (((CheckBox)sender).Checked)
                        m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask |= 0x08;
                    else
                        m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask &= ~0x08;
                    break;
            }

            if (chk_TopROI.Checked || chk_BottomROI.Checked || chk_RightROI.Checked || chk_LeftROI.Checked)
            {
                btn_Next.Enabled = true;
            }
            else
            {
                btn_Next.Enabled = false;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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

            if (m_intSelectedROI != m_smVisionInfo.g_intSelectedROI && m_smVisionInfo.g_intLearnStepNo == 1)
            {
                m_intSelectedROI = m_smVisionInfo.g_intSelectedROI;

                if (m_smVisionInfo.g_intSelectedROI == 1)
                    cbo_ROI.SelectedItem = "Top";
                else if (m_smVisionInfo.g_intSelectedROI == 2)
                    cbo_ROI.SelectedItem = "Right";
                else if (m_smVisionInfo.g_intSelectedROI == 3)
                    cbo_ROI.SelectedItem = "Bottom";
                else if (m_smVisionInfo.g_intSelectedROI == 4)
                    cbo_ROI.SelectedItem = "Left";

                CustomizeGUI();

                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }

        }

        private void txt_MaxArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (chk_ApplyToAllSide.Checked)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].ref_intMaxShadowArea = Convert.ToInt32(txt_MaxArea.Text);
                }
            }
            else
            {
                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intMaxShadowArea = Convert.ToInt32(txt_MaxArea.Text);
            }
            
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ROIInward_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (chk_ApplyToAllSide.Checked)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].ref_intDontCareBlobROIInward = Convert.ToInt32(txt_ROIInward.Text);
                }
            }
            else
            {
                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intDontCareBlobROIInward = Convert.ToInt32(txt_ROIInward.Text);
            }

            UpdatePocketEdgeDontCareImage();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (chk_ApplyToAllSide.Checked)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].ref_intMinShadowArea = Convert.ToInt32(txt_MinArea.Text);
                }
            }
            else
            {
                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intMinShadowArea = Convert.ToInt32(txt_MinArea.Text);
            }
           
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Threshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            int intThreshold = Convert.ToInt32(txt_Threshold.Text);

            if (chk_ApplyToAllSide.Checked)
            {
                for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].ref_intDontCareBlobThreshold = intThreshold;

                }
            }
            else
            {
                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intDontCareBlobThreshold = intThreshold;
            }
            UpdatePocketEdgeDontCareImage();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void trackBar_Threshold_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Threshold.Text = (Convert.ToSingle(trackBar_Threshold.Value)).ToString();
        }

        private void chk_ApplyToAllSide_Click(object sender, EventArgs e)
        {
            if (m_blnBlockOtherControlUpdate)
                return;

            //m_blnBlockOtherControlUpdate = true;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_blnBlockOtherControlUpdate = false;
        }

        private void cbo_ROI_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;
            
            if (cbo_ROI.SelectedItem.ToString() == "Top")
                m_intSelectedROI = m_smVisionInfo.g_intSelectedROI = 1;
            else if (cbo_ROI.SelectedItem.ToString() == "Right")
                m_intSelectedROI = m_smVisionInfo.g_intSelectedROI = 2;
            if (cbo_ROI.SelectedItem.ToString() == "Bottom")
                m_intSelectedROI = m_smVisionInfo.g_intSelectedROI = 3;
            if (cbo_ROI.SelectedItem.ToString() == "Left")
                m_intSelectedROI = m_smVisionInfo.g_intSelectedROI = 4;

            CustomizeGUI();

            m_blnBlockOtherControlUpdate = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }
        private void UpdatePocketEdgeDontCareImage()
        {
            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].CopyTo(ref m_smVisionInfo.g_objPackageImage);

            ImageDrawing objSourceImage = m_smVisionInfo.g_objPackageImage;
            for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if ((i == 1) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x01) == 0))
                    continue;
                if ((i == 2) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x02) == 0))
                    continue;
                if ((i == 3) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x04) == 0))
                    continue;
                if ((i == 4) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x08) == 0))
                    continue;

                if (m_smVisionInfo.g_objPackageImage.ref_intImageWidth != objSourceImage.ref_intImageWidth || m_smVisionInfo.g_objPackageImage.ref_intImageHeight != objSourceImage.ref_intImageHeight)
                {
                    m_smVisionInfo.g_objPackageImage.SetImageSize(objSourceImage.ref_intImageWidth, objSourceImage.ref_intImageHeight);
                }
                ROI objROI = new ROI();
                objROI.AttachImage(m_smVisionInfo.g_objPackageImage);
                switch (i)
                {
                    case 1:
                        objROI.LoadROISetting(m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROITotalX,
                                m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROITotalY,
                                m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROIWidth,
                                m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROIHeight + m_smVisionInfo.g_arrLead[i].ref_intDontCareBlobROIInward);
                        break;
                    case 2:
                        objROI.LoadROISetting(m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROITotalX - m_smVisionInfo.g_arrLead[i].ref_intDontCareBlobROIInward,
                                m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROITotalY,
                                m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROIWidth + m_smVisionInfo.g_arrLead[i].ref_intDontCareBlobROIInward,
                                m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROIHeight);
                        break;
                    case 3:
                        objROI.LoadROISetting(m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROITotalX,
                                        m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROITotalY - m_smVisionInfo.g_arrLead[i].ref_intDontCareBlobROIInward,
                                        m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROIWidth,
                                        m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROIHeight + m_smVisionInfo.g_arrLead[i].ref_intDontCareBlobROIInward);
                        break;
                    case 4:
                        objROI.LoadROISetting(m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROITotalX,
                                m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROITotalY,
                                m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROIWidth + m_smVisionInfo.g_arrLead[i].ref_intDontCareBlobROIInward,
                                m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].ref_ROIHeight);
                        break;
                }

                objROI.AttachImage(objSourceImage);
                //m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].SaveImage("D:\\TS\\OriROI_"+i.ToString() + ".bmp");
                objROI.ThresholdTo_ROIToROISamePosition(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrLead[i].ref_intDontCareBlobThreshold);
                //m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].SaveImage("D:\\TS\\ThresholdROI_" + i.ToString() + ".bmp");
                //m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[i][0].AttachImage(m_smVisionInfo.g_objPackageImage);
                objROI.Dispose();
            }
        }

        private void txt_MaxWidth_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (chk_ApplyToAllSide.Checked)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].ref_intMaxShadowWidth = Convert.ToInt32(txt_MaxWidth.Text);
                }
            }
            else
            {
                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intMaxShadowWidth = Convert.ToInt32(txt_MaxWidth.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_FlipToOppositeFunction_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (chk_ApplyToAllSide.Checked)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].ref_blnFlipToOppositeFunction = chk_FlipToOppositeFunction.Checked;
                }
            }
            else
            {
                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_blnFlipToOppositeFunction = chk_FlipToOppositeFunction.Checked;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
    }
}
