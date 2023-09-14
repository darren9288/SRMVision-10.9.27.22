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
    public partial class LearnLeadPocketDontCareAreaManualForm : Form
    {
        #region Member Variables
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
        
        #endregion

        public LearnLeadPocketDontCareAreaManualForm(VisionInfo visionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup)
        {
            InitializeComponent();


            m_smVisionInfo = visionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            
            m_strSelectedRecipe = strSelectedRecipe;
            m_intUserGroup = intUserGroup;
            DisableField2();
            CustomizeGUI();
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;
            m_blnInitDone = true;
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "Lead";
            string strChild2 = "Learn Dont Care Area Manual";
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
            chk_TopROI.Checked = (m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x01) > 0;
            chk_RightROI.Checked = (m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x02) > 0;
            chk_BottomROI.Checked = (m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x04) > 0;
            chk_LeftROI.Checked = (m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x08) > 0;
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

            SaveROISetting(strFolderPath + "Lead\\PocketDontCareROIManual.xml");

            LoadROISetting(strFolderPath + "Lead\\PocketDontCareROIManual.xml", m_smVisionInfo.g_arrLeadPocketDontCareROIsManual, 5);
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
            objFileHandle.WriteElement1Value("LeadPocketDontCareROIMaskManual", m_smVisionInfo.g_intLeadPocketDontCareROIManualMask);
            objFileHandle.WriteEndElement();
            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Advance Settings", strPath, "", m_smProductionInfo.g_strLotID);


            // Save Pocket Reference Pattern
            m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1].SaveImage(strPath + "Template\\ManualPocketTemplate" + 0 + ".bmp");

            for (int i = 1; i < m_smVisionInfo.g_arrLeadPocketDontCareROIsManual.Count; i++)
            {
                if ((i == 1) && ((m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x01) > 0))
                {
                    m_smVisionInfo.g_arrLead[i].ref_fManualPocketReferenceOffsetX = m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1].ref_ROITotalCenterX - m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[i][0].ref_ROITotalCenterX;
                    m_smVisionInfo.g_arrLead[i].ref_fManualPocketReferenceOffsetY = m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1].ref_ROITotalCenterY - m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[i][0].ref_ROITotalCenterY;
                    m_smVisionInfo.g_arrLead[i].SaveManualPocketReferencePattern(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1], strPath + "Template\\", 0);
                }
                if ((i == 2) && ((m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x02) > 0))
                {
                    m_smVisionInfo.g_arrLead[i].ref_fManualPocketReferenceOffsetX = m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1].ref_ROITotalCenterX - m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[i][0].ref_ROITotalCenterX;
                    m_smVisionInfo.g_arrLead[i].ref_fManualPocketReferenceOffsetY = m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1].ref_ROITotalCenterY - m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[i][0].ref_ROITotalCenterY;
                    m_smVisionInfo.g_arrLead[i].SaveManualPocketReferencePattern(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1], strPath + "Template\\", 0);
                }
                if ((i == 3) && ((m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x04) > 0))
                {
                    m_smVisionInfo.g_arrLead[i].ref_fManualPocketReferenceOffsetX = m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1].ref_ROITotalCenterX - m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[i][0].ref_ROITotalCenterX;
                    m_smVisionInfo.g_arrLead[i].ref_fManualPocketReferenceOffsetY = m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1].ref_ROITotalCenterY - m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[i][0].ref_ROITotalCenterY;
                    m_smVisionInfo.g_arrLead[i].SaveManualPocketReferencePattern(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1], strPath + "Template\\", 0);
                }
                if ((i == 4) && ((m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x08) > 0))
                {
                    m_smVisionInfo.g_arrLead[i].ref_fManualPocketReferenceOffsetX = m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1].ref_ROITotalCenterX - m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[i][0].ref_ROITotalCenterX;
                    m_smVisionInfo.g_arrLead[i].ref_fManualPocketReferenceOffsetY = m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1].ref_ROITotalCenterY - m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[i][0].ref_ROITotalCenterY;
                    m_smVisionInfo.g_arrLead[i].SaveManualPocketReferencePattern(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1], strPath + "Template\\", 0);
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
            m_smVisionInfo.g_intLeadPocketDontCareROIManualMask = objFileHandle.GetValueAsInt("LeadPocketDontCareROIMaskManual", 0, 1);

            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                m_smVisionInfo.g_arrLead[i].LoadManualPocketReferencePattern(strPath + "Template\\ManualPocketMatcher" + 0.ToString() + ".mch");

            }
            
        }

        private void SaveROISetting(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath, false);

            ROI objROI;
            for (int t = 0; t < m_smVisionInfo.g_arrLeadPocketDontCareROIsManual.Count; t++)
            {
                objFile.WriteSectionElement("Unit" + t, true);

                for (int j = 0; j < m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[t].Count; j++)
                {
                    objROI = m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[t][j];
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

            LoadROISetting(strFolderPath + "Lead\\PocketDontCareROIManual.xml", m_smVisionInfo.g_arrLeadPocketDontCareROIsManual, 5);
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
                case 0: //Define Reference Search ROI                  
                    {
                        // 2019-10-01 ZJYEOH : Use user selected Image
                        m_smVisionInfo.g_blnViewPackageImage = false;
                        m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;
                        AddReferenceSearchROI(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual);

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
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;
                    
                    AddReferencePatternROI(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual);
                    
                    for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {
                        PointF pPatternCenter = new PointF(0, 0);
                        if (m_smVisionInfo.g_arrLead[i].FindManualPocketReferencePattern(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][0], ref pPatternCenter))
                        {
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1].LoadROISetting((int)Math.Round((pPatternCenter.X - m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][0].ref_ROIPositionX) - (m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1].ref_ROIWidth / 2)),
                             (int)Math.Round((pPatternCenter.Y - m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][0].ref_ROIPositionY) - (m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1].ref_ROIHeight / 2)),
                             m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1].ref_ROIWidth, m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1].ref_ROIHeight);
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
                case 2: //Define Don't Care Area

                    for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {
                        m_smVisionInfo.g_arrLead[i].LearnManualPocketReferencePattern(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1]);
                        //m_smVisionInfo.g_arrLead[i].ref_fManualPocketReferenceOffsetX = m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1].ref_ROITotalCenterX - m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[i][0].ref_ROIPositionX;
                        //m_smVisionInfo.g_arrLead[i].ref_fManualPocketReferenceOffsetY = m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][1].ref_ROITotalCenterY - m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[i][0].ref_ROIPositionY;
                    }

                    AddPocketDontCareROI(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual);

                    for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {
                        if ((i == 1) && ((m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x01) == 0))
                            continue;
                        else if ((i == 2) && ((m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x02) == 0))
                            continue;
                        else if ((i == 3) && ((m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x04) == 0))
                            continue;
                        else if ((i == 4) && ((m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x08) == 0))
                            continue;
                        PointF pPatternCenterPoint = new PointF(0, 0);
                        if (m_smVisionInfo.g_arrLead[i].FindManualPocketReferencePattern(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][0], ref pPatternCenterPoint))
                        {
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[i][0].LoadROISetting((int)Math.Round((pPatternCenterPoint.X - m_smVisionInfo.g_arrLead[i].ref_fManualPocketReferenceOffsetX) - (m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[i][0].ref_ROIWidth / 2)),
                             (int)Math.Round((pPatternCenterPoint.Y - m_smVisionInfo.g_arrLead[i].ref_fManualPocketReferenceOffsetY) - (m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[i][0].ref_ROIHeight / 2)),
                             m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[i][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[i][0].ref_ROIHeight);
                        }
                    }
                    m_smVisionInfo.g_blnDragROI = true;
                    //m_smVisionInfo.g_blnViewRotatedImage = true;
                    //m_smVisionInfo.g_blnViewObjectsBuilded = false;

                    lbl_Step3.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_PocketDontCare;
                    btn_Next.Enabled = false;
                    btn_Previous.Enabled = true;
                    break;
                default:
                    break;
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

                    objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);

                    arrROIs[i].Add(objROI);
                }
                else
                {
                    arrROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
                }
            }
        }

        private void chk_ROI_Click(object sender, EventArgs e)
        {
            switch(((CheckBox)sender).Name.ToString())
            {
                case "chk_TopROI":
                    if(((CheckBox)sender).Checked)
                        m_smVisionInfo.g_intLeadPocketDontCareROIManualMask |= 0x01;
                    else
                        m_smVisionInfo.g_intLeadPocketDontCareROIManualMask &= ~0x01;
                    break;
                case "chk_RightROI":
                    if (((CheckBox)sender).Checked)
                        m_smVisionInfo.g_intLeadPocketDontCareROIManualMask |= 0x02;
                    else
                        m_smVisionInfo.g_intLeadPocketDontCareROIManualMask &= ~0x02;
                    break;
                case "chk_BottomROI":
                    if (((CheckBox)sender).Checked)
                        m_smVisionInfo.g_intLeadPocketDontCareROIManualMask |= 0x04;
                    else
                        m_smVisionInfo.g_intLeadPocketDontCareROIManualMask &= ~0x04;
                    break;
                case "chk_LeftROI":
                    if (((CheckBox)sender).Checked)
                        m_smVisionInfo.g_intLeadPocketDontCareROIManualMask |= 0x08;
                    else
                        m_smVisionInfo.g_intLeadPocketDontCareROIManualMask &= ~0x08;
                    break;
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
        }
    }
}
