using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedMemory;
using VisionProcessing;
using Common;
using Microsoft.Win32;
using System.IO;
namespace VisionProcessForm
{
    public partial class PadROIToleranceSettingForm : Form
    {
        private Point m_pTop, m_pRight, m_pBottom, m_pLeft;
        private int[] m_intTopPrev = new int[5] { 0, 0, 0, 0, 0 };
        private int[] m_intRightPrev = new int[5] { 0, 0, 0, 0, 0 };
        private int[] m_intBottomPrev = new int[5] { 0, 0, 0, 0, 0 };
        private int[] m_intLeftPrev = new int[5] { 0, 0, 0, 0, 0 };
        private int m_intPadSelectedIndex = 0;
        private bool m_blnInitDone = false;
        private bool m_blnSetText = false;
        private string m_strSelectedRecipe;
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;

        private UserRight m_objUserRight = new UserRight();
        public PadROIToleranceSettingForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, int intSelectedTabPage)
        {
            InitializeComponent();

            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;

            cbo_ROI.Items.Clear();
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == 0)
                    cbo_ROI.Items.Add("Center");
                else if (i == 1)
                    cbo_ROI.Items.Add("Top");
                else if (i == 2)
                    cbo_ROI.Items.Add("Right");
                else if (i == 3)
                    cbo_ROI.Items.Add("Bottom");
                else if (i == 4)
                    cbo_ROI.Items.Add("Left");

                m_intTopPrev[i] = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop;
                m_intRightPrev[i] = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight;
                m_intBottomPrev[i] = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom;
                m_intLeftPrev[i] = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft;
            }

            if (cbo_ROI.Items.Count == 1)
            {
                cbo_ROI.Visible = false;
                lbl_ROI.Visible = false;
                chk_SetToAllSideROI.Visible = false;
            }

            if (cbo_ROI.Items.Count > 0)
            {
                cbo_ROI.SelectedIndex = 0;
            }

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            chk_SetToAllEdge.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllEdges_Pad", false));
            if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
                chk_SetToAllSideROI.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllSide_Pad", false));
            
            UpdateGUI();

            m_pTop = new Point(pnl_Top.Location.X, pnl_Top.Location.Y);
            m_pRight = new Point(pnl_Right.Location.X, pnl_Right.Location.Y);
            m_pBottom = new Point(pnl_Bottom.Location.X, pnl_Bottom.Location.Y);
            m_pLeft = new Point(pnl_Left.Location.X, pnl_Left.Location.Y);

            m_blnInitDone = true;
        }

        private void UpdateGUI()
        {
            picUnitROI.Image = ils_ImageListTree.Images[m_intPadSelectedIndex];
            //txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromTop.ToString();
            //if (txt_StartPixelFromTop.Text == "0")
            //{
            //    txt_StartPixelFromTop.Text = "1";
            //    txt_StartPixelFromTop.Text = "0";
            //}
            //txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromRight.ToString();
            //if (txt_StartPixelFromRight.Text == "0")
            //{
            //    txt_StartPixelFromRight.Text = "1";
            //    txt_StartPixelFromRight.Text = "0";
            //}
            //txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromBottom.ToString();
            //if (txt_StartPixelFromBottom.Text == "0")
            //{
            //    txt_StartPixelFromBottom.Text = "1";
            //    txt_StartPixelFromBottom.Text = "0";
            //}
            //txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromLeft.ToString();
            //if (txt_StartPixelFromLeft.Text == "0")
            //{
            //    txt_StartPixelFromLeft.Text = "1";
            //    txt_StartPixelFromLeft.Text = "0";
            //}

            switch (m_intPadSelectedIndex)
            {
                case 0:
                case 1:
                    txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromTop.ToString();
                    if (txt_StartPixelFromTop.Text == "0")
                    {
                        txt_StartPixelFromTop.Text = "1";
                        txt_StartPixelFromTop.Text = "0";
                    }
                    txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromRight.ToString();
                    if (txt_StartPixelFromRight.Text == "0")
                    {
                        txt_StartPixelFromRight.Text = "1";
                        txt_StartPixelFromRight.Text = "0";
                    }
                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromBottom.ToString();
                    if (txt_StartPixelFromBottom.Text == "0")
                    {
                        txt_StartPixelFromBottom.Text = "1";
                        txt_StartPixelFromBottom.Text = "0";
                    }
                    txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromLeft.ToString();
                    if (txt_StartPixelFromLeft.Text == "0")
                    {
                        txt_StartPixelFromLeft.Text = "1";
                        txt_StartPixelFromLeft.Text = "0";
                    }
                    break;
                case 2:
                    txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromRight.ToString();
                    if (txt_StartPixelFromTop.Text == "0")
                    {
                        txt_StartPixelFromTop.Text = "1";
                        txt_StartPixelFromTop.Text = "0";
                    }
                    txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromBottom.ToString();
                    if (txt_StartPixelFromRight.Text == "0")
                    {
                        txt_StartPixelFromRight.Text = "1";
                        txt_StartPixelFromRight.Text = "0";
                    }
                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromLeft.ToString();
                    if (txt_StartPixelFromBottom.Text == "0")
                    {
                        txt_StartPixelFromBottom.Text = "1";
                        txt_StartPixelFromBottom.Text = "0";
                    }
                    txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromTop.ToString();
                    if (txt_StartPixelFromLeft.Text == "0")
                    {
                        txt_StartPixelFromLeft.Text = "1";
                        txt_StartPixelFromLeft.Text = "0";
                    }
                    break;
                case 3:
                    txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromBottom.ToString();
                    if (txt_StartPixelFromTop.Text == "0")
                    {
                        txt_StartPixelFromTop.Text = "1";
                        txt_StartPixelFromTop.Text = "0";
                    }
                    txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromLeft.ToString();
                    if (txt_StartPixelFromRight.Text == "0")
                    {
                        txt_StartPixelFromRight.Text = "1";
                        txt_StartPixelFromRight.Text = "0";
                    }
                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromTop.ToString();
                    if (txt_StartPixelFromBottom.Text == "0")
                    {
                        txt_StartPixelFromBottom.Text = "1";
                        txt_StartPixelFromBottom.Text = "0";
                    }
                    txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromRight.ToString();
                    if (txt_StartPixelFromLeft.Text == "0")
                    {
                        txt_StartPixelFromLeft.Text = "1";
                        txt_StartPixelFromLeft.Text = "0";
                    }
                    break;
                case 4:
                    txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromLeft.ToString();
                    if (txt_StartPixelFromTop.Text == "0")
                    {
                        txt_StartPixelFromTop.Text = "1";
                        txt_StartPixelFromTop.Text = "0";
                    }
                    txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromTop.ToString();
                    if (txt_StartPixelFromRight.Text == "0")
                    {
                        txt_StartPixelFromRight.Text = "1";
                        txt_StartPixelFromRight.Text = "0";
                    }
                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromRight.ToString();
                    if (txt_StartPixelFromBottom.Text == "0")
                    {
                        txt_StartPixelFromBottom.Text = "1";
                        txt_StartPixelFromBottom.Text = "0";
                    }
                    txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_intPadROIStartPixelFromBottom.ToString();
                    if (txt_StartPixelFromLeft.Text == "0")
                    {
                        txt_StartPixelFromLeft.Text = "1";
                        txt_StartPixelFromLeft.Text = "0";
                    }
                    break;
            }
        }

        private void SavePadSetting(string strFolderPath)
        {
            //strFolderPath = strFolderPath + "Pad\\";
          
            //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //{
            //    string strSectionName = "";
            //    if (i == 0)
            //        strSectionName = "CenterROI";
            //    else if (i == 1)
            //        strSectionName = "TopROI";
            //    else if (i == 2)
            //        strSectionName = "RightROI";
            //    else if (i == 3)
            //        strSectionName = "BottomROI";
            //    else if (i == 4)
            //        strSectionName = "LeftROI";

            //    //
            //    //STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
            //    m_smVisionInfo.g_arrPad[i].SavePad(strFolderPath + "Template\\Template.xml",
            //        false, strSectionName, true);
            //    //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Pad Template", strFolderPath, "Template\\Template.xml");
                
            //}
            
        }

        private void LoadPadSetting(string strPath)
        {
            //strPath = strPath + "Pad\\";
            //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //{
              
            //    // Load Pad Template Setting
            //    string strSectionName = "";
            //    if (i == 0)
            //        strSectionName = "CenterROI";
            //    else if (i == 1)
            //        strSectionName = "TopROI";
            //    else if (i == 2)
            //        strSectionName = "RightROI";
            //    else if (i == 3)
            //        strSectionName = "BottomROI";
            //    else if (i == 4)
            //        strSectionName = "LeftROI";

            //    m_smVisionInfo.g_arrPad[i].LoadPad(strPath + "Template\\Template.xml", strSectionName);

            //}
        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                            m_smVisionInfo.g_strVisionFolderName + "\\";

            SavePadSetting(strPath + "Pad\\");
            
            this.Close();
            this.Dispose();
        }
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            LoadPadSetting(strFolderPath + "Pad\\");

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = m_intTopPrev[i];
                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_intRightPrev[i];
                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_intBottomPrev[i];
                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = m_intLeftPrev[i];
            }

            this.Close();
            this.Dispose();
        }

        private void cbo_ROI_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_intPadSelectedIndex = cbo_ROI.SelectedIndex;
            m_blnInitDone = false;

            switch (m_intPadSelectedIndex)
            {
                case 0:
                case 1:
                    pnl_Top.Location = m_pTop;
                    pnl_Right.Location = m_pRight;
                    pnl_Bottom.Location = m_pBottom;
                    pnl_Left.Location = m_pLeft;
                    break;
                case 2:
                    pnl_Top.Location = m_pRight;
                    pnl_Right.Location = m_pBottom;
                    pnl_Bottom.Location = m_pLeft;
                    pnl_Left.Location = m_pTop;
                    break;
                case 3:
                    pnl_Top.Location = m_pBottom;
                    pnl_Right.Location = m_pLeft;
                    pnl_Bottom.Location = m_pTop;
                    pnl_Left.Location = m_pRight;
                    break;
                case 4:
                    pnl_Top.Location = m_pLeft;
                    pnl_Right.Location = m_pTop;
                    pnl_Bottom.Location = m_pRight;
                    pnl_Left.Location = m_pBottom;
                    break;
            }

            UpdateGUI();
   
            m_blnInitDone = true;
          
        }

        private void chk_SetToAllEdges_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllEdges_Pad", chk_SetToAllEdge.Checked);
        }

        private void chk_SetToAllSideROI_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllSide_Pad", chk_SetToAllSideROI.Checked);
        }

        private void txt_StartPixelFromTop_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnSetText)
                return;

            m_blnSetText = true;
            string m_strPadName = "";
            int temp = Convert.ToInt32(txt_StartPixelFromTop.Text);
            bool bln_messagehide = false;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != m_intPadSelectedIndex)
                    continue;
                
                if (chk_SetToAllEdge.Checked)
                {
                    txt_StartPixelFromRight.Text = txt_StartPixelFromTop.Text;
                    txt_StartPixelFromBottom.Text = txt_StartPixelFromTop.Text;
                    txt_StartPixelFromLeft.Text = txt_StartPixelFromTop.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                bln_messagehide = false;
                if (i != m_intPadSelectedIndex)
                    continue;
                
                //m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_StartPixelFromTop.Text);
                //if (chk_SetToAllEdge.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_StartPixelFromTop.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_StartPixelFromTop.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_StartPixelFromTop.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top

                        if (i == 0)
                            m_strPadName = "Center";
                        else
                            m_strPadName = "Top";

                        if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                        (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                        < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                        {
                            SRMMessageBox.Show(m_strPadName +" pad top edge cannot larger than Search ROI, recommended value: " +  ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))- m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                            txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop.ToString();
                        }
                        else 
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp; 

                        if (chk_SetToAllEdge.Checked)
                        {
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                            > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                                txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;


                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                            < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))- m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                                txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                             (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                             > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                                if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom <= -10)
                                {
                                    if (-5 < temp && temp <= 0)
                                        bln_messagehide = true;
                                }

                                if (!bln_messagehide)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom.ToString();
                                }
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                        }
                        break;
                    case 2: // Right
                        m_strPadName = "Right";
                        if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                        (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                        > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                        {
                            SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                            txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight.ToString();
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                        if (chk_SetToAllEdge.Checked)
                        {
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                            < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                                txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                                txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                             (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                             < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                                if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft <= -10)
                                {
                                    if (-5 < temp && temp <= 0)
                                        bln_messagehide=true;
                                }

                                if (!bln_messagehide)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft.ToString();
                                }
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                        }
                        break;
                    case 3: // Bottom
                        m_strPadName = "Botttom";
                        if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) +
                            temp) > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                        {
                            SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                            txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom.ToString();
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                        if (chk_SetToAllEdge.Checked)
                        {
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                            > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                                txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                            < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                                txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                            < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                                if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop <= -10)
                                {
                                    if (-5 < temp && temp <= 0)
                                        bln_messagehide=true;
                                }

                                if (!bln_messagehide)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop.ToString();
                                }
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;
                        }
                        break;
                    case 4: // Left
                        m_strPadName = "Left";
                        if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                        (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                         < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                        {
                            SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                            txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft.ToString();
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;

                        if (chk_SetToAllEdge.Checked)
                        {
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                            > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                             {
                                SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                                txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                             < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                                txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                            > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                                if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight <= -10)
                                {
                                    if (-5 < temp && temp <= 0)
                                        bln_messagehide=true;
                                }

                                if (!bln_messagehide)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight.ToString();
                                }
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                bln_messagehide = false;
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == m_intPadSelectedIndex || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_strPadName = "Top";
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                             < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                            if (chk_SetToAllEdge.Checked)
                            {
                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                                    > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                                {
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                     (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                                    if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom <= -10)
                                    {
                                        if (-5 < temp && temp <= 0)
                                            bln_messagehide = true;
                                    }

                                    if(!bln_messagehide)
                                    {
                                        SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                        (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");
                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                            < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                            }
                            break;
                        case 2: // Right
                            m_strPadName = "Right";
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                            > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                            if (chk_SetToAllEdge.Checked)
                            {
                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                                 < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;


                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                                    > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");                                

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;


                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                 (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                                 < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                                {
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                                    if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft <= -10)
                                    {
                                        if (-5 < temp && temp <= 0)
                                            bln_messagehide = true;
                                    }

                                    if(!bln_messagehide)
                                    {
                                        SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                        (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                            }
                            break;
                        case 3: // Bottom
                            m_strPadName = "Bottom";
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                            if (chk_SetToAllEdge.Checked)
                            {
                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                                 < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                                {
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                                    if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop <= -10)
                                    {
                                        if (-5 < temp && temp <= 0)
                                            bln_messagehide = true;
                                    }

                                    if(!bln_messagehide)
                                    {
                                        SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                        (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");
                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                             < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                            }
                            break;
                        case 4: // Left
                            m_strPadName = "Left";
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                            < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;

                            if (chk_SetToAllEdge.Checked)
                            {
                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                                {
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                                    if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight <= -10)
                                    {
                                        if (-5 < temp && temp <= 0)
                                            bln_messagehide = true;
                                    }

                                    if(!bln_messagehide)
                                    {
                                        SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                        (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");
                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                                     > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                                < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;
        }
                            break;
                    }
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_blnSetText = false;
        }
        private void MeasureGauge()
        {
            //m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPad[0].ref_fImageGain); // 2019-12-16 ZJYEOH : No need add gain for whole image as all ROIs have separated Gain
            // Set RectGauge4L Placement
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SetGaugePlace_BasedOnEdgeROI();

                m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ResetGaugeSettingToUserVariables();

                bool blnGaugeResult;
                // 2019 08 01 - CCENG: Measure on g_objPackageImage but display original image
                //if (m_smVisionInfo.g_blnViewPackageImage)
                //blnGaugeResult = m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objWhiteImage);
                if (i == 0 && m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
                {
                    // 2020-03-24 ZJYEOH : no need measure center ROI gauge if use side pkg to measure center pkg
                    blnGaugeResult = true;
                }
                else
                {
                    if (((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
                        blnGaugeResult = m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_objWhiteImage, true); // 2019-12-16 ZJYEOH : Need use this new measure gauge function as all ROIs have separated Gain
                    else
                        blnGaugeResult = m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_objWhiteImage, true); // 2019-12-16 ZJYEOH : Need use this new measure gauge function as all ROIs have separated Gain
                }
                //else
                //    blnGaugeResult = m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

                if (!blnGaugeResult)
                {
                    m_smVisionInfo.g_strErrorMessage = "*" + m_smVisionInfo.g_arrPad[i].ref_strErrorMessage;
                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                }
            }
            //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //{
            //    if (m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize)
            //    {
            //        m_smVisionInfo.g_arrPad[i].SetRectGauge4LPlacement(m_smVisionInfo.g_arrPadROIs[i][1]);

            //        m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ResetGaugeSettingToUserVariables();

            //        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPad[0].ref_fImageGain);
            //        AttachImageToROI(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_objPackageImage);
            //        m_smVisionInfo.g_blnViewPackageImage = true;
            //        if (m_smVisionInfo.g_blnViewPackageImage)
            //            m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_objPackageImage);
            //        else
            //            m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            //    }
            //}
        }
        private void txt_StartPixel_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewPadExtendROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixel_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadExtendROI = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void PadROIToleranceSettingForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadSettingDrawing = true;
        }

        private void PadROIToleranceSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.g_blnViewPadSettingDrawing = false;
        }

        private void txt_StartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if(m_blnSetText)
                return;

            m_blnSetText = true;
            string m_strPadName = "";
            int temp = Convert.ToInt32(txt_StartPixelFromRight.Text);
            bool bln_messagehide = false;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != m_intPadSelectedIndex)
                    continue;

                if (chk_SetToAllEdge.Checked)
                {
                    txt_StartPixelFromTop.Text = txt_StartPixelFromRight.Text;
                    txt_StartPixelFromBottom.Text = txt_StartPixelFromRight.Text;
                    txt_StartPixelFromLeft.Text = txt_StartPixelFromRight.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                bln_messagehide = false;
                if (i != m_intPadSelectedIndex)
                    continue;

                //m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_StartPixelFromRight.Text);
                //if (chk_SetToAllEdge.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_StartPixelFromRight.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_StartPixelFromRight.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_StartPixelFromRight.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        if (i == 0)
                            m_strPadName = "Center";
                        else
                            m_strPadName = "Top";

                        if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                        (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                        > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                        {
                            SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                            txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight.ToString();
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                        if (chk_SetToAllEdge.Checked)
                        {
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                               (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                               < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                               (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                                txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                               (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                               < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                                txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                             (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                             > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                                if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom <= -10)
                                {
                                    if (-5 < temp && temp <= 0)
                                        bln_messagehide=true;
                                }

                                if(!bln_messagehide)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom.ToString();
                                }
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;
                        }
                        break;
                    case 2: // Right
                        m_strPadName = "Right";
                        if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                           (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                           > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                        {
                            SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                            txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom.ToString();
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                        if (chk_SetToAllEdge.Checked)
                        {
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                               (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                                < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                                txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                            > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                                txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                               (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                               < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                                if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft <= -10)
                                {
                                    if (-5 < temp && temp <= 0)
                                        bln_messagehide=true;
                                }

                                if (!bln_messagehide)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft.ToString();
                                }
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                        }
                        break;
                    case 3: // Bottom
                        m_strPadName = "Bottom";
                        if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                           (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                            < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                        {
                            SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                            txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft.ToString();
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;

                        if (chk_SetToAllEdge.Checked)
                        {
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                               (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                               > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                                txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                            > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                                txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                            < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                                if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop <= -10)
                                {
                                    if (-5 < temp && temp <= 0)
                                        bln_messagehide = true;
                                }

                                if (!bln_messagehide)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop.ToString();
                                }
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;
                        }
                        break;
                    case 4: // Left
                        m_strPadName = "Left";
                        if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                            < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                        {
                            SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                            txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop.ToString();
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                        if (chk_SetToAllEdge.Checked)
                        {
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                               (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) +
                               temp) > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                                txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) -
                              temp) < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                                txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                            > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                                if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight <= -10)
                                {
                                    if (-5 < temp && temp <= 0)
                                        bln_messagehide = true;
                                }

                                if (!bln_messagehide)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight.ToString();
                                }
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                bln_messagehide = false;
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == m_intPadSelectedIndex || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_strPadName = "Top";
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                            > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                            if (chk_SetToAllEdge.Checked)
                            {
                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                   (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                                   < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                   (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                                   > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                                {
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                                    if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom <= -10)
                                    {
                                        if (-5 < temp && temp <= 0)
                                            bln_messagehide = true;
                                    }

                                    if(!bln_messagehide)
                                    {
                                        SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                        (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");
                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                   (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                                  < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                            }
                            break;
                        case 2: // Right
                            m_strPadName = "Right";
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                               (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) +
                               temp) > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");


                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                            if (chk_SetToAllEdge.Checked)
                            {
                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                  (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                                  < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                   (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) -
                                   temp) < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                                {
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                                    if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft <= -10)
                                    {
                                        if (-5 < temp && temp <= 0)
                                            bln_messagehide = true;
                                    }
            
                                    if(!bln_messagehide)
                                    {
                                        SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                        (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");
                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                            }
                            break;
                        case 3: // Bottom
                            m_strPadName = "Bottom";
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                               (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                              < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                            if (chk_SetToAllEdge.Checked)
                            {
                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                   (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                                    < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                                {
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                                    if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop <= -10)
                                    {
                                        if (-5 < temp && temp <= 0)
                                            bln_messagehide = true;
                                    }

                                    if(!bln_messagehide)
                                    {
                                        SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                        (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");
                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                   (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                                   > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));
                                }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;
                            }
                            break;
                        case 4: // Left
                            m_strPadName = "Left";
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                               (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) -
                              temp) < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                            if (chk_SetToAllEdge.Checked)
                            {
                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                                {
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                                    if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight <= -10)
                                    {
                                        if (-5 < temp && temp <= 0)
                                            bln_messagehide = true;
                                    }

                                    if(!bln_messagehide)
                                    {
                                        SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                        (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");
                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                   (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) +
                                 temp) > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");


                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                   (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                                   < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");


                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                            }
                            break;
                    }
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_blnSetText = false;
        }

        private void txt_StartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnSetText)
                return;

            m_blnSetText = true;
            string m_strPadName = "";
            int temp = Convert.ToInt32(txt_StartPixelFromBottom.Text);
            bool bln_messagehide = false;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != m_intPadSelectedIndex)
                    continue;

                if (chk_SetToAllEdge.Checked)
                {
                    txt_StartPixelFromTop.Text = txt_StartPixelFromBottom.Text;
                    txt_StartPixelFromRight.Text = txt_StartPixelFromBottom.Text;
                    txt_StartPixelFromLeft.Text = txt_StartPixelFromBottom.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                bln_messagehide = false;
                if (i != m_intPadSelectedIndex)
                    continue;

                //m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                //if (chk_SetToAllEdge.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        if (i == 0)
                            m_strPadName = "Center";
                        else
                            m_strPadName = "Top";

                        if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) +
                           temp) > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                            if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom <= -10)
                            {
                                if (-5 < temp && temp <= 0)
                                    bln_messagehide = true;
                            }

                            if(!bln_messagehide)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom.ToString();
                            }
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                        if (chk_SetToAllEdge.Checked)
                        {
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) -
                                temp) < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                                txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                                txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                                < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                                txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                        }
                        break;
                    case 2: // Right
                        m_strPadName = "Right";
                        if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                           < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                            if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft <= -10)
                            {
                                if (-5 < temp && temp <= 0)
                                    bln_messagehide=true;
                            }

                            if (!bln_messagehide)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft.ToString();
                            }
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp; 

                        if (chk_SetToAllEdge.Checked)
                        {
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) -
                                temp) < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                                txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                                txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) +
                                temp) > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                                txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;
                        }
                        break;
                    case 3: // Bottom
                        m_strPadName = "Bottom";
                        if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                            < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                            if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop <= -10)
                            {
                                if (-5 < temp && temp <= 0)
                                    bln_messagehide=true;
                            }

                            if(!bln_messagehide)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop.ToString();
                            }
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                        if (chk_SetToAllEdge.Checked)
                        {
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) +
                                temp) > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                                txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                                txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                                < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                                txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                        }
                        break;
                    case 4: // Left
                        m_strPadName = "Left";
                        if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                            > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                            if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight <= -10)
                            {
                                if (-5 < temp && temp <= 0)
                                    bln_messagehide=true;
                            }

                            if (!bln_messagehide)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight.ToString();
                            }
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                        if (chk_SetToAllEdge.Checked)
                        {
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                                 < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                                txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;


                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                                txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;


                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                                < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                                txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                bln_messagehide = false;
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == m_intPadSelectedIndex || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_strPadName = "Top";
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) +
                                temp) > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                                if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom <= -10)
                                {
                                    if (-5 < temp && temp <= 0)
                                        bln_messagehide = true;
                                }

                                if(!bln_messagehide)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");
                                }
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                            if (chk_SetToAllEdge.Checked)
                            {
                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                                    < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                    > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                                    < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                   (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                            }
                            break;
                        case 2: // Right
                            m_strPadName = "Right";
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                                 < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                                if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft <= -10)
                                {
                                    if (-5 < temp && temp <= 0)
                                        bln_messagehide = true;
                                }

                                if(!bln_messagehide)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");
                                }
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;

                            if (chk_SetToAllEdge.Checked)
                            {
                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                                     < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                    > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                                     > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;
                            }
                            break;
                        case 3: // Bottom
                            m_strPadName = "Bottom";
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                                < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                                if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop <= -10)
                                {
                                    if (-5 < temp && temp <= 0)
                                        bln_messagehide = true;
                                }

                                if(!bln_messagehide)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");
                                }
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                            if (chk_SetToAllEdge.Checked)
                            {
                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) +
                                    temp) > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                    > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                                    < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                            }
                            break;
                        case 4: // Left
                            m_strPadName = "Left";
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                                if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight <= -10)
                                {
                                    if (-5 < temp && temp <= 0)
                                        bln_messagehide = true;
                                }

                                if(!bln_messagehide)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");
                                }
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                            if (chk_SetToAllEdge.Checked)
                            {
                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                                    < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;


                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                                     > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;


                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                                    < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                            }
                            break;
                    }
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_blnSetText = false;
        }

        private void txt_StartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnSetText)
                return;

            m_blnSetText = true;
            string m_strPadName = "";
            int temp = Convert.ToInt32(txt_StartPixelFromLeft.Text);
            bool bln_messagehide = false;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != m_intPadSelectedIndex)
                    continue;

                if (chk_SetToAllEdge.Checked)
                {
                    txt_StartPixelFromTop.Text = txt_StartPixelFromLeft.Text;
                    txt_StartPixelFromRight.Text = txt_StartPixelFromLeft.Text;
                    txt_StartPixelFromBottom.Text = txt_StartPixelFromLeft.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                bln_messagehide = false;
                if (i != m_intPadSelectedIndex)
                    continue;

                //m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                //if (chk_SetToAllEdge.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        if (i == 0)
                            m_strPadName = "Center";
                        else
                            m_strPadName = "Top";
                        if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                           (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                           < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                        {
                            SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                            txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft.ToString();
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;

                        if (chk_SetToAllEdge.Checked)
                        {
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                             < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                                txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                                txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                                if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom <= -10)
                                {
                                    if (-5 < temp && temp <= 0)
                                       bln_messagehide=true;
                                }

                                if (!bln_messagehide)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom.ToString();
                                }
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;
                        }
                        break;
                    case 2: // Right
                        m_strPadName = "Right";
                        if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                        (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                        < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                        {
                            SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                            txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop.ToString();
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                        if (chk_SetToAllEdge.Checked)
                        {
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                 (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                  > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                                txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) +
                                temp) > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                                txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                            < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                                if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft <= -10)
                                {
                                    if (-5 < temp && temp <= 0)
                                        bln_messagehide=true;
                                }

                                if (!bln_messagehide)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft.ToString();
                                }
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                        }
                        break;
                    case 3: // Bottom
                        m_strPadName = "Bottom";
                        if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                           (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                            > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                        {
                            SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                            txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight.ToString();
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                        if (chk_SetToAllEdge.Checked)
                        {
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                               (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                                < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                                txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                               > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                                txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                            < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                                if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop <= -10)
                                {
                                    if (-5 < temp && temp <= 0)
                                        bln_messagehide=true;
                                }

                                if (!bln_messagehide)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop.ToString();
                                }
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;
                        }
                        break;
                    case 4: // Left
                        m_strPadName = "Left";
                        if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                            > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                        {
                            SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                            txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom.ToString();
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                        if (chk_SetToAllEdge.Checked)
                        {
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) -temp)
                            < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                                txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                               (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                               < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                                txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft.ToString();
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;

                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                            > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                                if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight <= -10)
                                {
                                    if (-5 < temp && temp <= 0)
                                        bln_messagehide=true;
                                }

                                if (!bln_messagehide)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight.ToString();
                                }
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                bln_messagehide = false;
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == m_intPadSelectedIndex || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_strPadName = "Top";
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                               (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                                < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;

                            if (chk_SetToAllEdge.Checked)
                            {
                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                                 < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                     (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                      > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                                    > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                                {
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));

                                    if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom <= -10)
                                    {
                                        if (-5 < temp && temp <= 0)
                                            bln_messagehide = true;
                                    }

                                    if(!bln_messagehide)
                                    {
                                        SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                        (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");
                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;
                            }
                                break;
                        case 2: // Right
                            m_strPadName = "Right";
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                            (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                             < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                            if (chk_SetToAllEdge.Checked)
                            {
                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                   (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                                    < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                                {
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;

                                    if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft <= -10)
                                    {
                                        if (-5 < temp && temp <= 0)
                                            bln_messagehide = true;
                                    }

                                    if(!bln_messagehide)
                                    {
                                        SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                        (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");
                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                     (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                      > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                                    > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");


                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;
                            }
                            break;
                        case 3: // Bottom
                            m_strPadName = "Bottom";
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                            if (chk_SetToAllEdge.Checked)
                            {
                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                                < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                                {
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;

                                    if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop <= -10)
                                    {
                                        if (-5 < temp && temp <= 0)
                                            bln_messagehide = true;
                                    }

                                    if(!bln_messagehide)
                                    {
                                        SRMMessageBox.Show(m_strPadName + " pad botttom edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                        (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");
                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                   (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                                    < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                                     > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;
                            }
                            break;
                        case 4: // Left
                            m_strPadName = "Left";
                            if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) + temp)
                                > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                            {
                                SRMMessageBox.Show(m_strPadName + " pad left edge cannot larger than Search ROI, recomended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2))).ToString() + " or below");

                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y +
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2));
                            }
                            else
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = temp;

                            if (chk_SetToAllEdge.Checked)
                            {
                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) - temp)
                                 < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad right edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) + temp)
                                     > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                                {
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2));

                                    if (m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight <= -10)
                                    {
                                        if (-5 < temp && temp <= 0)
                                            bln_messagehide = true;
                                    }

                                    if(!bln_messagehide)
                                    {
                                        SRMMessageBox.Show(m_strPadName + " pad bottom edge cannot larger than Search ROI, rcommended value: " + (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X +
                                        (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2))).ToString() + " or below");
                                    }
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = temp;

                                if ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                   (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) - temp)
                                    < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                                {
                                    SRMMessageBox.Show(m_strPadName + " pad top edge cannot larger than Search ROI, recommended value: " + ((int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX).ToString() + " or below");

                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)) - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = temp;
                            }
                            break;
                    }
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_blnSetText = false;
        }
    }
}
