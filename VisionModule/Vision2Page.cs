using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SharedMemory;
using Common;
using System.IO;
using VisionProcessing;


namespace VisionModule
{
    public partial class Vision2Page : Form
    {
        #region Member Variables

        private int m_intMachineStatusPrev = 1;
        
        // Count
        private int m_intPassCount = 0, m_intPassCountPrev = -1;
        private int m_intFailCount = 0, m_intFailCountPrev = -1;
        private int m_intTestedTotal = 0, m_intTestedTotalPrev = -1;
        private int m_intCheckPresentFailCount = 0, m_intCheckPresentFailCountPrev = -1;
        private int m_intNoTemplateFailCount = 0, m_intNoTemplateFailCountPrev = -1;
        private string m_strResult = "", m_strResultPrev = "";

        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo m_smVisionInfo;
        private UserRight m_objUserRight = new UserRight();
        #endregion



        public Vision2Page(CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, VisionInfo smVisionInfo)
        {
            InitializeComponent();

            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;

            //lbl_LotID.Text = m_smProductionInfo.g_strLotID;
            lbl_LotID.Text = m_smProductionInfo.g_arrSingleLotID[m_smVisionInfo.g_intVisionIndex];
            lbl_OperatorID.Text = m_smProductionInfo.g_strOperatorID;
            UpdateInfo();
            LoadTemplateImage();
            ResetCounterBackColor();
            CustomizeGUI();
        }

        private void ResetCounterBackColor()
        {
            lbl_CheckPresentFailCount.BackColor = Color.White;
            lbl_CheckPresentFailPercent.BackColor = Color.White;
            lbl_NoTemplateFailCount.BackColor = Color.White;
            lbl_NoTemplateFailPercent.BackColor = Color.White;
        }

        public void CustomizeGUI()
        {
            if ((m_smCustomizeInfo.g_intWantCheckPresent & (0x01 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                pnl_CheckPresent.Visible = false;
            }

            if (m_intNoTemplateFailCount != 0)
            {
                pnl_NoTemplate.Visible = true;
            }
            else
            {
                pnl_NoTemplate.Visible = false;
            }
        }
        /// <summary>
        /// Make sure that only when form is focused, then enable timer.
        /// By this way, CPU usage can be reduced.
        /// </summary>
        public void ActivateTimer(bool blnEnable)
        {
            timer_Live.Enabled = blnEnable;
        }

        public void SetMultiViewGUI(bool blnSet)
        {
            if (blnSet)
            {
                this.Size = new Size(380, 285);

            }
            else
            {
                this.Size = new Size(319, 569);
            }
        }


        private void EnableButton(bool blnEnable)
        {
            btn_Reset.Enabled = blnEnable;
        }

        public void LoadTemplateImage()
        {
            return;

            string strRecipe = m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex];
            string strPath;
            strPath = m_smProductionInfo.g_strRecipePath + strRecipe +
                            "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Positioning\\Template\\Template0.bmp";
            pic_Learn1.Visible = true;
            if (m_smVisionInfo.g_objPositioning.ref_intMethod == 1)
            {
                pic_Learn1.Visible = true;
                if (File.Exists(strPath))
                    pic_Learn1.Load(strPath);
            }
            else
            {
                pic_Learn1.Visible = false;
                return;
            }
        }

        private void ResetCount()
        {
            //m_smVisionInfo.g_intPositionFailureTotal = 0;
            //m_smVisionInfo.g_intPassTotal = 0;
            //m_smVisionInfo.g_intTestedTotal = 0;
            //m_smVisionInfo.g_intPassImageCount = 0;
            //m_smVisionInfo.g_intFailImageCount = 0;
            //m_smVisionInfo.g_intLowYieldUnitCount = 0;

            //m_smVisionInfo.VS_AT_UpdateQuantity = true;

            lbl_PassCount.Text = "0";
            lbl_TestedTotal.Text = "0";
            lbl_Yield.Text = "0.00";
            lbl_CheckPresentFailCount.Text = "0";
            lbl_Yield.BackColor = Color.White;

            lbl_CheckPresentFailCount.Text = "0";
            lbl_FailCount.Text = "0";
            lbl_CheckPresentFailPercent.Text = "0.00";
            lbl_NoTemplateFailCount.Text = "0";
            lbl_NoTemplateFailPercent.Text = "0.00";
        }

        private void UpdateInfo()
        {

            switch (m_smVisionInfo.g_strResult)
            {
                case "Pass":
                    lbl_ResultStatus.BackColor = Color.Lime;
                    if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                    {
                        if (lbl_ResultStatus.Text != "Pass")
                            lbl_ResultStatus.Text = "Pass";
                    }
                    else
                    {
                        switch (m_smVisionInfo.g_objPositioning.ref_intHighestScoreDirection)
                        {
                            case 0:
                                if (lbl_ResultStatus.Text != "0")
                                    lbl_ResultStatus.Text = "0";
                                break;
                            case 1:
                                if (lbl_ResultStatus.Text != "-90")
                                    lbl_ResultStatus.Text = "-90";
                                break;
                            case 2:
                                if (lbl_ResultStatus.Text != "180")
                                    lbl_ResultStatus.Text = "180";
                                break;
                            case 3:
                                if (lbl_ResultStatus.Text != "90")
                                    lbl_ResultStatus.Text = "90";
                                break;
                            default:
                                lbl_ResultStatus.Text = "";
                                break;

                        }
                    }
                    break;
                case "Fail":
                    lbl_ResultStatus.BackColor = Color.Red;
                    if (lbl_ResultStatus.Text != "Fail")
                        lbl_ResultStatus.Text = "Fail";
                    break;
            }

            m_intPassCount = m_smVisionInfo.g_intPassTotal;
            if (m_intPassCount != m_intPassCountPrev)
            {
                lbl_PassCount.Text = m_intPassCount.ToString();
                m_intPassCountPrev = m_intPassCount;
            }

            m_intFailCount = m_smVisionInfo.g_intTestedTotal - m_smVisionInfo.g_intPassTotal;
            if (m_intFailCount != m_intFailCountPrev)
            {
                lbl_FailCount.Text = m_intFailCount.ToString();
                m_intFailCountPrev = m_intFailCount;
            }

            m_intTestedTotal = m_smVisionInfo.g_intTestedTotal;
            if (m_intTestedTotal != m_intTestedTotalPrev)
            {
                lbl_TestedTotal.Text = m_intTestedTotal.ToString();

                if (m_intTestedTotal != 0)
                {
                    float fYield = (m_intPassCount / (float)m_intTestedTotal) * 100;
                    lbl_Yield.Text = fYield.ToString("f2");

                    if (fYield <= m_smVisionInfo.g_fLowYield)
                        lbl_Yield.BackColor = Color.Red;
                    else
                        lbl_Yield.BackColor = Color.White;
                }
                else
                {
                    lbl_Yield.Text = "0.00";
                    lbl_Yield.BackColor = Color.White;
                }

                m_intTestedTotalPrev = m_intTestedTotal;
            }

            m_intCheckPresentFailCount = m_smVisionInfo.g_intPositionFailureTotal;
          
            if (m_intTestedTotal != 0)
            {
                float fCheckPresentFailPercent = (m_intCheckPresentFailCount / (float)m_intTestedTotal) * 100;
                if (fCheckPresentFailPercent > 100)
                    fCheckPresentFailPercent = 100;
                lbl_CheckPresentFailPercent.Text = fCheckPresentFailPercent.ToString("f2");
            }
            else
                lbl_CheckPresentFailPercent.Text = "0.00";

            if (m_intCheckPresentFailCount != m_intCheckPresentFailCountPrev)
            {
                lbl_CheckPresentFailCount.BackColor = Color.Red;
                lbl_CheckPresentFailPercent.BackColor = Color.Red;
                lbl_CheckPresentFailCount.Text = m_intCheckPresentFailCount.ToString();
                m_intCheckPresentFailCountPrev = m_intCheckPresentFailCount;
            }
            else
            {
                lbl_CheckPresentFailCount.BackColor = Color.White;
                lbl_CheckPresentFailPercent.BackColor = Color.White;
            }

            m_intNoTemplateFailCount = m_smVisionInfo.g_intNoTemplateFailureTotal;
            if (m_intNoTemplateFailCount != 0)
            {
                pnl_NoTemplate.Visible = true;
            }
            else
            {
                pnl_NoTemplate.Visible = false;
            }

            if (m_intTestedTotal != 0)
            {
                float fNoTemplateFailPercent = (m_intNoTemplateFailCount / (float)m_intTestedTotal) * 100;
                if (fNoTemplateFailPercent > 100)
                    fNoTemplateFailPercent = 100;
                lbl_NoTemplateFailPercent.Text = fNoTemplateFailPercent.ToString("f2");
            }
            else
                lbl_NoTemplateFailPercent.Text = "0.00";

            if (m_intNoTemplateFailCount != m_intNoTemplateFailCountPrev)
            {
                lbl_NoTemplateFailCount.BackColor = Color.Red;
                lbl_NoTemplateFailPercent.BackColor = Color.Red;
                lbl_NoTemplateFailCount.Text = m_intNoTemplateFailCount.ToString();
                m_intNoTemplateFailCountPrev = m_intNoTemplateFailCount;
            }
            else
            {
                lbl_NoTemplateFailCount.BackColor = Color.White;
                lbl_NoTemplateFailPercent.BackColor = Color.White;
            }

            lbl_GrabDelay.Text = m_smVisionInfo.g_intCameraGrabDelay.ToString();
            lbl_GrabTime.Text = (Math.Max(0, m_smVisionInfo.g_objGrabTime.Duration - m_smVisionInfo.g_intCameraGrabDelay)).ToString("f0");
            lbl_ProcessTime.Text = (Math.Max(0, m_smVisionInfo.g_objTotalTime.Duration - m_smVisionInfo.g_objGrabTime.Duration)).ToString("f2");
            lbl_TotalTime.Text = m_smVisionInfo.g_objTotalTime.Duration.ToString("f2");

        }

        private void UpdateLotNo()
        {
            //lbl_LotID.Text = m_smProductionInfo.g_strLotID;
            lbl_LotID.Text = m_smProductionInfo.g_arrSingleLotID[m_smVisionInfo.g_intVisionIndex];
            lbl_OperatorID.Text = m_smProductionInfo.g_strOperatorID;
            UpdateInfo();
        }


        private void btn_Reset_Click(object sender, EventArgs e)
        {
            ResetCount();
            m_smProductionInfo.VM_TH_UpdateCount = true;
        }




        private void timer_Live_Tick(object sender, EventArgs e)
        {
          
            if (m_smVisionInfo.g_intMachineStatus == 2 && m_intMachineStatusPrev != 2)
            {
                EnableButton(false);
                m_intMachineStatusPrev = 2;
            }
            else if (m_intMachineStatusPrev != m_smVisionInfo.g_intMachineStatus)
            {
                EnableButton(true);
                m_intMachineStatusPrev = m_smVisionInfo.g_intMachineStatus;
            }

            if (m_smVisionInfo.g_blnResetGUIDisplayCount)
            {
                m_smVisionInfo.g_blnResetGUIDisplayCount = false;
                ResetCount();
                ResetCounterBackColor();
                m_smProductionInfo.VM_TH_UpdateCount = true;
            }

            if (m_smVisionInfo.PR_VM_UpdateQuantity)
            {
                UpdateInfo();
                m_smVisionInfo.PR_VM_UpdateQuantity = false;
            }

            if (m_smVisionInfo.PG_VM_LoadTemplate)
            {
                LoadTemplateImage();
                m_smVisionInfo.PG_VM_LoadTemplate = false;
            }

            if (m_smVisionInfo.AT_VM_UpdateProduction)
            {
                UpdateLotNo();
                LoadTemplateImage();
                UpdateInfo();
                m_smVisionInfo.AT_VM_UpdateProduction = false;
            }
        }

    
    }
}