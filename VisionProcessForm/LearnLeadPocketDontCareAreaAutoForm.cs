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
    public partial class LearnLeadPocketDontCareAreaAutoForm : Form
    {
        #region Member Variables
        private bool m_blnBlockOtherControlUpdate = false;
        private bool m_blnInitDone = false;
        private int m_intUserGroup = 5;
        private int m_intDisplayStepNo = 1;
        private string m_strSelectedRecipe;
        private int m_intSelectedROI = 0;
        private List<ImageDrawing> m_arrCurrentImage = new List<ImageDrawing>();
        private int m_intSelectedImagePrev = 0;
        private VisionInfo m_smVisionInfo;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        
        #endregion

        public LearnLeadPocketDontCareAreaAutoForm(VisionInfo visionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup)
        {
            InitializeComponent();

            m_smVisionInfo = visionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            
            m_strSelectedRecipe = strSelectedRecipe;
            m_intUserGroup = intUserGroup;
            m_intSelectedROI = m_smVisionInfo.g_intSelectedROI;
            DisableField2();
            CustomizeGUI();
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;
            m_blnInitDone = true;
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "Lead";
            string strChild2 = "Learn Dont Care Area Auto";
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
                cbo_ROI.Enabled = false;
                chk_ApplyToAllSide.Enabled = false;
                gb_Polarity.Enabled = false;
                txt_MeasThickness.Enabled = false;
                trackBar_MeasThickness.Enabled = false;
                txt_Derivative.Enabled = false;
                trackBar_Derivative.Enabled = false;
                txt_Gain.Enabled = false;
                trackBar_Gain.Enabled = false;
                txt_Threshold.Enabled = false;
                trackBar_Threshold.Enabled = false;
                txt_Offset.Enabled = false;
                trackBar_Offset.Enabled = false;
                txt_MaskThickness.Enabled = false;
                trackBar_MaskThickness.Enabled = false;
                txt_SearchLineNo.Enabled = false;
                chk_ShowDraggingBox.Enabled = false;
                chk_ShowSamplePoints.Enabled = false;
                chk_ShowTextureLines.Enabled = false;
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
            chk_TopROI.Checked = (m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x01) > 0;
            chk_RightROI.Checked = (m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x02) > 0;
            chk_BottomROI.Checked = (m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x04) > 0;
            chk_LeftROI.Checked = (m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x08) > 0;

        }

        private void LearnLeadPocketDontCareAreaAutoForm_Load(object sender, EventArgs e)
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

            //UpdatePocketEdgeDontCareImage();
            // Start Setup step 1
            Cursor.Current = Cursors.Default;
            m_smVisionInfo.g_intLearnStepNo = 0;
            SetupSteps(true);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.g_blncboImageView = false;

        }

        private void LearnLeadPocketDontCareAreaAutoForm_FormClosing(object sender, FormClosingEventArgs e)
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

            SaveROISetting(strFolderPath + "Lead\\PocketDontCareROIAuto.xml");

            LoadROISetting(strFolderPath + "Lead\\PocketDontCareROIAuto.xml", m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto, 5);
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

            //STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath + "Settings.xml");
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("LeadPocketDontCareROIMaskAuto", m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask);
            objFileHandle.WriteEndElement();
            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Advance Settings", strPath, "", m_smProductionInfo.g_strLotID);

            // Save Pocket Reference Pattern
            m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1].SaveImage(strPath + "Template\\AutoPocketTemplate" + 0 + ".bmp");

            for (int i = 1; i < m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto.Count; i++)
            {
                if ((i == 1) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x01) > 0))
                {
                    m_smVisionInfo.g_arrLead[i].ref_fAutoPocketReferenceOffsetX = m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1].ref_ROITotalCenterX - m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROITotalCenterX;
                    m_smVisionInfo.g_arrLead[i].ref_fAutoPocketReferenceOffsetY = m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1].ref_ROITotalCenterY - m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROITotalCenterY;
                    m_smVisionInfo.g_arrLead[i].SaveAutoPocketReferencePattern(m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1], strPath + "Template\\", 0);
                }
                if ((i == 2) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x02) > 0))
                {
                    m_smVisionInfo.g_arrLead[i].ref_fAutoPocketReferenceOffsetX = m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1].ref_ROITotalCenterX - m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROITotalCenterX;
                    m_smVisionInfo.g_arrLead[i].ref_fAutoPocketReferenceOffsetY = m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1].ref_ROITotalCenterY - m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROITotalCenterY;
                    m_smVisionInfo.g_arrLead[i].SaveAutoPocketReferencePattern(m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1], strPath + "Template\\", 0);
                }
                if ((i == 3) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x04) > 0))
                {
                    m_smVisionInfo.g_arrLead[i].ref_fAutoPocketReferenceOffsetX = m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1].ref_ROITotalCenterX - m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROITotalCenterX;
                    m_smVisionInfo.g_arrLead[i].ref_fAutoPocketReferenceOffsetY = m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1].ref_ROITotalCenterY - m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROITotalCenterY;
                    m_smVisionInfo.g_arrLead[i].SaveAutoPocketReferencePattern(m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1], strPath + "Template\\", 0);
                }
                if ((i == 4) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x08) > 0))
                {
                    m_smVisionInfo.g_arrLead[i].ref_fAutoPocketReferenceOffsetX = m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1].ref_ROITotalCenterX - m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROITotalCenterX;
                    m_smVisionInfo.g_arrLead[i].ref_fAutoPocketReferenceOffsetY = m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1].ref_ROITotalCenterY - m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROITotalCenterY;
                    m_smVisionInfo.g_arrLead[i].SaveAutoPocketReferencePattern(m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1], strPath + "Template\\", 0);
                }
            }

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

                m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.SavePocketEdgeGauge(strPath + "PocketEdgeGauge.xml", false, strSectionName, true);

                ////
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
            m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask = objFileHandle.GetValueAsInt("LeadPocketDontCareROIMaskAuto", 0, 1);

            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                m_smVisionInfo.g_arrLead[i].LoadAutoPocketReferencePattern(strPath + "Template\\AutoPocketMatcher" + 0.ToString() + ".mch");

            }

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

                m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.LoadPocketEdgeGauge(strPath + "PocketEdgeGauge.xml", strSectionName, m_smVisionInfo.g_WorldShape);
                m_smVisionInfo.g_arrLead[i].LoadLead(strPath + "Template\\Template.xml", strSectionName, m_smVisionInfo.g_arrImages.Count);
            }
        }

        private void SaveROISetting(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath, false);

            ROI objROI;
            for (int t = 0; t < m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto.Count; t++)
            {
                objFile.WriteSectionElement("Unit" + t, true);

                for (int j = 0; j < m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[t].Count; j++)
                {
                    objROI = m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[t][j];
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

            LoadROISetting(strFolderPath + "Lead\\PocketDontCareROIAuto.xml", m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto, 5);
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
        private void AddReferenceSearchROI(List<List<ROI>> arrROIs)
        {
            ROI objROI;

            if (arrROIs[0].Count == 0)
            {
                objROI = new ROI("Reference Search ROI", 1);
                int intPositionX = (640 / 2) - 100;
                int intPositionY = (480 / 2) - 100;
                objROI.LoadROISetting(intPositionX, intPositionY, 200, 200);

                objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);

                arrROIs[0].Add(objROI);
            }
            else
            {
                arrROIs[0][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            }
        }

        private void AddReferencePatternROI(List<List<ROI>> arrROIs)
        {
            ROI objROI;

            if (arrROIs[0].Count == 1)
            {
                objROI = new ROI("Reference Pattern ROI", 2);

                objROI.LoadROISetting(0, 0, arrROIs[0][0].ref_ROIWidth / 2, arrROIs[0][0].ref_ROIHeight / 2);

                objROI.AttachImage(arrROIs[0][0]);

                arrROIs[0].Add(objROI);
            }
            else
            {
                arrROIs[0][1].AttachImage(arrROIs[0][0]);
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
            switch (((CheckBox)sender).Name.ToString())
            {
                case "chk_TopROI":
                    if (((CheckBox)sender).Checked)
                        m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask |= 0x01;
                    else
                        m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask &= ~0x01;
                    break;
                case "chk_RightROI":
                    if (((CheckBox)sender).Checked)
                        m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask |= 0x02;
                    else
                        m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask &= ~0x02;
                    break;
                case "chk_BottomROI":
                    if (((CheckBox)sender).Checked)
                        m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask |= 0x04;
                    else
                        m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask &= ~0x04;
                    break;
                case "chk_LeftROI":
                    if (((CheckBox)sender).Checked)
                        m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask |= 0x08;
                    else
                        m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask &= ~0x08;
                    break;
            }

            if (m_smVisionInfo.g_intLearnStepNo == 2)
            {
                if (chk_TopROI.Checked || chk_BottomROI.Checked || chk_RightROI.Checked || chk_LeftROI.Checked)
                {
                    btn_Next.Enabled = true;
                }
                else
                {
                    btn_Next.Enabled = false;
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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
                case 0: //Define Reference Search ROI                  
                    {
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                        m_smVisionInfo.g_blnViewPackageImage = true;
                        //m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;
                        AddReferenceSearchROI(m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto);

                        m_smVisionInfo.g_blnViewROI = true;
                        m_smVisionInfo.g_blnDragROI = true;

                        lbl_StepNo.BringToFront();
                        lbl_Step1.BringToFront();
                        tabCtrl_Lead.SelectedTab = tp_ReferenceSearchROI;
                        btn_Next.Enabled = true;
                        btn_Previous.Enabled = false;
                    }
                    break;
                case 1: //Define Reference Pattern ROI
                        //m_smVisionInfo.g_intSelectedImage = 0;
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    //m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;

                    AddReferencePatternROI(m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto);

                    for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {
                        PointF pPatternCenter = new PointF(0, 0);
                        if (m_smVisionInfo.g_arrLead[i].FindAutoPocketReferencePattern(m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][0], ref pPatternCenter))
                        {
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1].LoadROISetting((int)Math.Round((pPatternCenter.X - m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][0].ref_ROIPositionX) - (m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1].ref_ROIWidth / 2)),
                             (int)Math.Round((pPatternCenter.Y - m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][0].ref_ROIPositionY) - (m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1].ref_ROIHeight / 2)),
                             m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1].ref_ROIWidth, m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1].ref_ROIHeight);
                        }
                    }
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;

                    lbl_StepNo.BringToFront();
                    lbl_Step2.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_ReferencePatternROI;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    break;
                case 2: //Define Define Don't Care Area                  
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                        {
                            m_smVisionInfo.g_arrLead[i].LearnAutoPocketReferencePattern(m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1]);
                            //m_smVisionInfo.g_arrLead[i].ref_fAutoPocketReferenceOffsetX = m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1].ref_ROITotalCenterX - m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROIPositionX;
                            //m_smVisionInfo.g_arrLead[i].ref_fAutoPocketReferenceOffsetY = m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][1].ref_ROITotalCenterY - m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROIPositionY;
                        }

                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].CopyTo(ref m_smVisionInfo.g_objPackageImage);

                        // 2019-10-01 ZJYEOH : Use user selected Image
                        m_smVisionInfo.g_blnViewGauge = false;
                        m_smVisionInfo.g_blnViewPackageImage = true;
                        m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;

                        AddPocketDontCareROI(m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto);

                        for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                        {
                            if ((i == 1) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x01) == 0))
                                continue;
                            else if ((i == 2) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x02) == 0))
                                continue;
                            else if ((i == 3) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x04) == 0))
                                continue;
                            else if ((i == 4) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x08) == 0))
                                continue;
                            PointF pPatternCenterPoint = new PointF(0, 0);
                            if (m_smVisionInfo.g_arrLead[i].FindAutoPocketReferencePattern(m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][0], ref pPatternCenterPoint))
                            {
                                m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].LoadROISetting((int)Math.Round((pPatternCenterPoint.X - m_smVisionInfo.g_arrLead[i].ref_fAutoPocketReferenceOffsetX) - (m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROIWidth / 2)),
                                 (int)Math.Round((pPatternCenterPoint.Y - m_smVisionInfo.g_arrLead[i].ref_fAutoPocketReferenceOffsetY) - (m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROIHeight / 2)),
                                 m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROIHeight);
                            }
                        }
                        m_smVisionInfo.g_blnViewROI = true;
                        m_smVisionInfo.g_blnDragROI = true;

                        lbl_StepNo.BringToFront();
                        lbl_Step3.BringToFront();
                        tabCtrl_Lead.SelectedTab = tp_SearchROI;
                        if (chk_TopROI.Checked || chk_BottomROI.Checked || chk_RightROI.Checked || chk_LeftROI.Checked)
                        {
                            btn_Next.Enabled = true;
                        }
                        else
                        {
                            btn_Next.Enabled = false;
                        }
                        btn_Previous.Enabled = true;

                      
                    }
                    break;
                case 3: //Define Don't Care Area setting
                    UpdateROIComboBox();
                    UpdateGaugeGUI();
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewGauge = true;
                    //m_smVisionInfo.g_blnViewRotatedImage = true;
                    //m_smVisionInfo.g_blnViewObjectsBuilded = false;

                    lbl_Step4.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_GaugeSetting;
                    btn_Next.Enabled = false;
                    btn_Previous.Enabled = true;
                    break;
                default:
                    break;
            }
        }

        private void chk_ApplyToAllSide_Click(object sender, EventArgs e)
        {
            if (m_blnBlockOtherControlUpdate)
                return;

            //m_blnBlockOtherControlUpdate = true;
            
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_blnBlockOtherControlUpdate = false;
        }

        private void UpdateROIComboBox()
        {
            cbo_ROI.Items.Clear();
            for (int i = 1; i < m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto.Count; i++)
            {
                if (i == 1 && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x01) > 0))
                    cbo_ROI.Items.Add("Top");
                else if (i == 2 && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x02) > 0))
                    cbo_ROI.Items.Add("Right");
                else if (i == 3 && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x04) > 0))
                    cbo_ROI.Items.Add("Bottom");
                else if (i == 4 && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x08) > 0))
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


            for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (i == 1 && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x01) == 0))
                    continue;
                else if (i == 2 && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x02) == 0))
                    continue;
                else if (i == 3 && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x04) == 0))
                    continue;
                else if (i == 4 && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x08) == 0))
                    continue;

                m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.SetPGaugePlace(m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROIPositionX, m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROIPositionY,
                    m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROIHeight);
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void UpdateGaugeGUI()
        {
            if (cbo_ROI.Items.Count == 0)
                return;

            int intLeadIndex = 0;
            if (cbo_ROI.SelectedItem.ToString() == "Top")
                intLeadIndex = 1;
            else if (cbo_ROI.SelectedItem.ToString() == "Right")
                intLeadIndex = 2;
            if (cbo_ROI.SelectedItem.ToString() == "Bottom")
                intLeadIndex = 3;
            if (cbo_ROI.SelectedItem.ToString() == "Left")
                intLeadIndex = 4;

            if (intLeadIndex == 0)
                return;

            txt_SearchLineNo.Text = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_intSearchLineNumber.ToString();

            chk_ShowDraggingBox.Checked = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_blnDrawDraggingBox;
            chk_ShowSamplePoints.Checked = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_blnDrawSamplingPoint;
            chk_ShowTextureLines.Checked = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_blnDrawTexture;

            txt_Gain.Text = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_fGaugeImageGain.ToString();
            trackBar_Gain.Value = Convert.ToInt32(Convert.ToSingle(txt_Gain.Text) * 10f);

            txt_Threshold.Text = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_intGaugeImageThreshold.ToString();
            trackBar_Threshold.Value = Convert.ToInt32(Convert.ToSingle(txt_Threshold.Text));

            txt_Offset.Text = m_smVisionInfo.g_arrLead[intLeadIndex].ref_intLineOffset.ToString();
            trackBar_Offset.Value = Convert.ToInt32(Convert.ToSingle(txt_Offset.Text));

            txt_MaskThickness.Text = m_smVisionInfo.g_arrLead[intLeadIndex].ref_intMaskThickness.ToString();
            trackBar_MaskThickness.Value = Convert.ToInt32(Convert.ToSingle(txt_MaskThickness.Text));

            //txt_MeasThickness.Text = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_GaugeThickness.ToString();
            //trackBar_MeasThickness.Value = Convert.ToInt32(Convert.ToSingle(txt_MeasThickness.Text));

            txt_MeasThickness.Text = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_intThickness.ToString();
            trackBar_MeasThickness.Value = Convert.ToInt32(Convert.ToSingle(txt_MeasThickness.Text));

            txt_Derivative.Text = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_GaugeThreshold.ToString();
            trackBar_Derivative.Value = Convert.ToInt32(Convert.ToSingle(txt_Derivative.Text));

            if (m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_GaugeTransType == 0)
                radioBtn_BW.Checked = true;
            else
                radioBtn_WB.Checked = true;

            UpdatePocketEdgeDontCareImage();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void cbo_ROI_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            //m_blnBlockOtherControlUpdate = true;
            
            if (cbo_ROI.SelectedItem.ToString() == "Top")
                m_intSelectedROI = m_smVisionInfo.g_intSelectedROI = 1;
            else if (cbo_ROI.SelectedItem.ToString() == "Right")
                m_intSelectedROI = m_smVisionInfo.g_intSelectedROI = 2;
            if (cbo_ROI.SelectedItem.ToString() == "Bottom")
                m_intSelectedROI = m_smVisionInfo.g_intSelectedROI = 3;
            if (cbo_ROI.SelectedItem.ToString() == "Left")
                m_intSelectedROI = m_smVisionInfo.g_intSelectedROI = 4;

            UpdateGaugeGUI();
          
            m_blnBlockOtherControlUpdate = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void radioBtn_Polarity_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            int intTransTypeIndex;
            if (radioBtn_BW.Checked)
                intTransTypeIndex = 0;
            else
                intTransTypeIndex = 1;
            
            if (chk_ApplyToAllSide.Checked)
            {
                for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    for (int k = 0; k < m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.Count; k++)
                    {
                        m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge[k].ref_GaugeTransType = intTransTypeIndex;
                        //m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_GaugeTransChoice = 2;
                    }
                }
            }
            else
            {
                for (int k = 0; k < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.Count; k++)
                {
                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge[k].ref_GaugeTransType = intTransTypeIndex;
                    //m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_GaugeTransChoice = 2;
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowDraggingBox_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_blnDrawDraggingBox = chk_ShowDraggingBox.Checked;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowSamplePoints_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_blnDrawSamplingPoint = chk_ShowSamplePoints.Checked;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void trackBar_Gain_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Gain.Text = (Convert.ToSingle(trackBar_Gain.Value) / 10).ToString();
        }

        private void txt_Gain_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            float fImageGain = Convert.ToSingle(txt_Gain.Text);

            if (chk_ApplyToAllSide.Checked)
            {
                for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                        m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_fGaugeImageGain = fImageGain;

                }
            }
            else
            {
                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_fGaugeImageGain = fImageGain;
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
                    m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_intGaugeImageThreshold = intThreshold;

                }
            }
            else
            {
                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_intGaugeImageThreshold = intThreshold;
            }
            UpdatePocketEdgeDontCareImage();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void trackBar_MeasThickness_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_MeasThickness.Text = (Convert.ToSingle(trackBar_MeasThickness.Value)).ToString();
        }

        private void txt_MeasThickness_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            int intMeasThickness = Convert.ToInt32(txt_MeasThickness.Text);
            if (chk_ApplyToAllSide.Checked)
            {
                for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_intThickness = intMeasThickness;

                }
            }
            else
            {
                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_intThickness = intMeasThickness;
            }
            //if (chk_ApplyToAllSide.Checked)
            //{
            //    for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
            //    {
            //        for (int k = 0; k < m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.Count; k++)
            //            m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge[k].ref_GaugeThickness = intMeasThickness;

            //    }
            //}
            //else
            //{
            //    for (int k = 0; k < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.Count; k++)
            //        m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge[k].ref_GaugeThickness = intMeasThickness;
            //}

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void trackBar_Derivative_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Derivative.Text = (Convert.ToSingle(trackBar_Derivative.Value)).ToString();
        }

        private void txt_Derivative_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            int intDerivative = Convert.ToInt32(txt_Derivative.Text);
            if (chk_ApplyToAllSide.Checked)
            {
                for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    for (int k = 0; k < m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.Count; k++)
                        m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge[k].ref_GaugeThreshold = intDerivative;

                }
            }
            else
            {
                for (int k = 0; k < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.Count; k++)
                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge[k].ref_GaugeThreshold = intDerivative;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void trackBar_Offset_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Offset.Text = (Convert.ToSingle(trackBar_Offset.Value)).ToString();
        }

        private void txt_Offset_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            int intOffset = Convert.ToInt32(txt_Offset.Text);

            if (chk_ApplyToAllSide.Checked)
            {
                for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].ref_intLineOffset = intOffset;

                }
            }
            else
            {
                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intLineOffset = intOffset;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void trackBar_MaskThickness_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_MaskThickness.Text = (Convert.ToSingle(trackBar_MaskThickness.Value)).ToString();
        }

        private void txt_MaskThickness_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            int intMaskThickness = Convert.ToInt32(txt_MaskThickness.Text);

            if (chk_ApplyToAllSide.Checked)
            {
                for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].ref_intMaskThickness = intMaskThickness;

                }
            }
            else
            {
                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intMaskThickness = intMaskThickness;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SearchLineNo_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;


            if (m_blnBlockOtherControlUpdate)
                return;

            int intSearchLineNo = Convert.ToInt32(txt_SearchLineNo.Text);

            if (chk_ApplyToAllSide.Checked)
            {
                for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_intSearchLineNumber = intSearchLineNo;

                    if (intSearchLineNo >= m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.Count)
                    {
                        for (int k = m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.Count; k < intSearchLineNo + 1; k++)
                        {
                            m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.Add(new PGauge(m_smVisionInfo.g_WorldShape));
                            m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.SetPGaugeSettingToNewAddedGauge(k);


                        }
                    }
                    else
                    {
                        while (m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.Count > intSearchLineNo + 1)
                            m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.RemoveAt(m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.Count - 1);
                    }

                    m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.SetPGaugePlace(m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROIPositionX, m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROIPositionY,
                        m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ref_ROIHeight);
                }
            }
            else
            {
                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_intSearchLineNumber = intSearchLineNo;
                if (intSearchLineNo >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.Count)
                {
                    for (int k = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.Count; k < intSearchLineNo + 1; k++)
                    {
                        m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.Add(new PGauge(m_smVisionInfo.g_WorldShape));
                        m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.SetPGaugeSettingToNewAddedGauge(k);
                    }
                }
                else
                {
                    while (m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.Count > intSearchLineNo + 1)
                        m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.RemoveAt(m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.ref_arrPocketEdgePGauge.Count - 1);
                }

                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPocketEdgeGauge.SetPGaugePlace(m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[m_smVisionInfo.g_intSelectedROI][0].ref_ROIPositionX, m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[m_smVisionInfo.g_intSelectedROI][0].ref_ROIPositionY,
                    m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[m_smVisionInfo.g_intSelectedROI][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[m_smVisionInfo.g_intSelectedROI][0].ref_ROIHeight);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void UpdatePocketEdgeDontCareImage()
        {
            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            
            ImageDrawing objSourceImage = m_smVisionInfo.g_objPackageImage;
            for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if ((i == 1) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x01) == 0))
                    continue;
                if ((i == 2) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x02) == 0))
                    continue;
                if ((i == 3) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x04) == 0))
                    continue;
                if ((i == 4) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x08) == 0))
                    continue;

                if (m_smVisionInfo.g_objPackageImage.ref_intImageWidth != objSourceImage.ref_intImageWidth || m_smVisionInfo.g_objPackageImage.ref_intImageHeight != objSourceImage.ref_intImageHeight)
                {
                    m_smVisionInfo.g_objPackageImage.SetImageSize(objSourceImage.ref_intImageWidth, objSourceImage.ref_intImageHeight);
                }
                m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].AttachImage(objSourceImage);
                //m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].SaveImage("D:\\TS\\OriROI_"+i.ToString() + ".bmp");
                m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].GainTo_ROIToROISamePosition(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_fGaugeImageGain);
                //m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].SaveImage("D:\\TS\\GainROI_" + i.ToString() + ".bmp");
                m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].ThresholdTo_ROIToROISamePosition(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_intGaugeImageThreshold);
                //m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].SaveImage("D:\\TS\\ThresholdROI_" + i.ToString() + ".bmp");
                //m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[i][0].AttachImage(m_smVisionInfo.g_objPackageImage);
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

            if (m_intSelectedROI != m_smVisionInfo.g_intSelectedROI && m_smVisionInfo.g_intLearnStepNo == 3)
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

                UpdateGaugeGUI();
                
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }

        }

        private void chk_ShowTextureLines_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                m_smVisionInfo.g_arrLead[i].ref_objPocketEdgeGauge.ref_blnDrawTexture = chk_ShowTextureLines.Checked;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
    }
}
