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

namespace VisionModule
{
    public partial class Vision6OfflinePage : Form
    {

        #region Member Variables
        private bool m_blnFailSeal = false;
        private bool m_blnFailMark = false;
        private bool m_blnFailEmpty = false;

        private int m_intUserGroup = 5;
        private bool m_blnInitDone = false;
        private string m_strSelectedRecipe;

        private VisionInfo m_smVisionInfo;
        private UserRight m_objUserRight = new UserRight();
        private bool m_blnEnterTextBox = false;
        private ProductionInfo m_smProductionInfo;
        
        #endregion

        public Vision6OfflinePage(VisionInfo visionInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup)
        {
            InitializeComponent();

            m_intUserGroup = intUserGroup;
            //2020 07 22 - CCENG: this variable not going to use here bcos have bug.
            //           - The m_strSelectedRecipe is not updated after user change to other recipe.
            //           - In order to this m_strSelectedRecipe, make sure this m_strSelectedRecipe is set with latest recipe name. 
            //m_strSelectedRecipe = strSelectedRecipe;    
            m_smVisionInfo = visionInfo;
            m_smProductionInfo = smProductionInfo;
            
            DisableField();
            UpdateGUI();

            m_blnInitDone = true;
        }



        /// <summary>
        /// Customize GUI based on user access level
        /// </summary>
        private void DisableField()
        {
            string strChild1 = "Test Page";
            string strChild2 = "";

            //strChild2 = "Inspect";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //    btn_Inspect.Enabled = false;

            //strChild2 = "Tolerance Setting Page";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //{
            //    tab_VisionControl.TabPages.Remove(tabPage_LineWidth);
                
            //}
        }



        /// <summary>
        /// Customize GUI
        /// </summary>
        private void UpdateGUI()
        {


            chk_CheckEmpty.Checked = m_smVisionInfo.MN_PR_CheckEmptyUnit;
            UpdateRadioBtn();

            tab_VisionControl.Controls.Remove(tp_Score);

            //txt_Distance.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_fTemplateWidth[2] / m_smVisionInfo.g_fCalibPixelY, 2, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_FarSealLineWidth.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_fTemplateWidth[0] / m_smVisionInfo.g_fCalibPixelY, 2, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_NearSealLineWidth.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_fTemplateWidth[1] / m_smVisionInfo.g_fCalibPixelY, 2, MidpointRounding.AwayFromZero).ToString("f3");

            //txt_WidthLowerTolerance.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance / m_smVisionInfo.g_fCalibPixelY, 2, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_WidthUpperTolerance.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance / m_smVisionInfo.g_fCalibPixelY, 2, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_DistanceMinTolerance.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_fDistanceMinTolerance / m_smVisionInfo.g_fCalibPixelY, 2, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_DistanceMaxTolerance.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_fDistanceMaxTolerance / m_smVisionInfo.g_fCalibPixelY, 2, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_Seal1AreaFilter.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_intSeal1AreaFilter / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 2, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_Seal2AreaFilter.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_intSeal2AreaFilter / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 2, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_OverHeatMinArea.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 2, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_MinBrokenArea.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_intHoleMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 2, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_ShiftTolerance.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_fShiftPositionTolerance / m_smVisionInfo.g_fCalibPixelY).ToString("f3");
            //txt_MinBrokenWidth.Text = m_smVisionInfo.g_objSeal.ref_fMinBrokenWidth.ToString();

            //txt_MaxDistance.Text = (Convert.ToSingle(txt_Distance.Text) + Convert.ToSingle(txt_DistanceMaxTolerance.Text)).ToString();
            //txt_MaxFarLineWidth.Text = (Convert.ToSingle(txt_FarSealLineWidth.Text) + Convert.ToSingle(txt_WidthUpperTolerance.Text)).ToString();
            //txt_MaxNearLineWidth.Text = (Convert.ToSingle(txt_NearSealLineWidth.Text) + Convert.ToSingle(txt_WidthUpperTolerance.Text)).ToString();
            //txt_MinDistance.Text = (Convert.ToSingle(txt_Distance.Text) - Convert.ToSingle(txt_DistanceMinTolerance.Text)).ToString();
            //txt_MinFarLineWidth.Text = (Convert.ToSingle(txt_FarSealLineWidth.Text) - Convert.ToSingle(txt_WidthLowerTolerance.Text)).ToString();
            //txt_MinNearLineWidth.Text = (Convert.ToSingle(txt_NearSealLineWidth.Text) - Convert.ToSingle(txt_WidthLowerTolerance.Text)).ToString();
            //txt_SealScoreTolerance.Text = (m_smVisionInfo.g_objSeal.ref_fSealScoreTolerance * 100).ToString();

            //trackBar_PocketScoreTolerance.Value = Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fPocketMinScore * 100);
            //txt_PocketScoreTolerance.Text = trackBar_PocketScoreTolerance.Value.ToString();

            //trackBar_MarkScoreTolerance.Value = Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fMarkMinScore * 100);
            //txt_MarkScoreTolerance.Text = trackBar_MarkScoreTolerance.Value.ToString();

            //txt_Seal1AreaTolerance.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_arrSealAreaTolerance[0] / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 2, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_Seal2AreaTolerance.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_arrSealAreaTolerance[1] / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 2, MidpointRounding.AwayFromZero).ToString("f3");


        }


        /// <summary>
        /// Update inspection result on GUI
        /// </summary>
        private void UpdateInfo( )
        {
            UpdateRadioBtn();
            if (m_smVisionInfo.g_objSeal.ref_intSealFailMask > 0)
            {
                if (m_smVisionInfo.g_strErrorMessage == "Offline Test Pass!"|| m_smVisionInfo.g_strResult == "Pass"|| m_smVisionInfo.g_strResult == "Empty")
                {
                    if (m_smVisionInfo.g_strResult == "Empty")
                    {
                        lbl_TestResultIndicatorUnit1.BackColor = Color.Lime;
                        lbl_TestResultIndicatorUnit1.Text = "Empty";
                    }
                    else
                    {
                        lbl_TestResultIndicatorUnit1.BackColor = Color.Lime;
                        lbl_TestResultIndicatorUnit1.Text = "Pass";
                    }
                }
                else
                {
                    lbl_TestResultIndicatorUnit1.BackColor = Color.Red;
                    //lbl_TestResultIndicatorUnit1.Text = "Fail";
                    lbl_TestResultIndicatorUnit1.Text = m_smVisionInfo.g_strResult;
                }
            }
            else
            {
                if (m_smVisionInfo.g_strResult == "Empty")
                {
                    lbl_TestResultIndicatorUnit1.BackColor = Color.Lime;
                    lbl_TestResultIndicatorUnit1.Text = "Empty";
                }
                else
                {
                    lbl_TestResultIndicatorUnit1.BackColor = Color.Lime;
                    lbl_TestResultIndicatorUnit1.Text = "Pass";
                }
            }

            if (m_smVisionInfo.MN_PR_CheckEmptyUnit)
            {
                switch (m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex)
                {
                    case 0:
                        radioBtn_Template1.Checked = true;
                        break;
                    case 1:
                        radioBtn_Template2.Checked = true;
                        break;
                    case 2:
                        radioBtn_Template3.Checked = true;
                        break;
                    case 3:
                        radioBtn_Template4.Checked = true;
                        break;
                }
            }
            else
            {
                switch (m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex)
                {
                    case 0:
                        radioBtn_Template1.Checked = true;
                        break;
                    case 1:
                        radioBtn_Template2.Checked = true;
                        break;
                    case 2:
                        radioBtn_Template3.Checked = true;
                        break;
                    case 3:
                        radioBtn_Template4.Checked = true;
                        break;
                }
            }
            lbl_GrabDelay.Text = m_smVisionInfo.g_intCameraGrabDelay.ToString();
            lbl_GrabTime.Text = (Math.Max(0, m_smVisionInfo.g_objGrabTime.Duration - m_smVisionInfo.g_intCameraGrabDelay)).ToString("f0"); //29-05-2019 ZJYEOH : Need to minus grab delay as it included inside Grab time 
            lbl_ProcessTime.Text = (Math.Max(0, m_smVisionInfo.g_objTotalTime.Duration - m_smVisionInfo.g_objGrabTime.Duration)).ToString("f2");
            lbl_TotalTime.Text = m_smVisionInfo.g_objTotalTime.Duration.ToString("f2");
            dgd_LineWidth.Rows.Clear();
            int i = 0;
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x01) > 0)
            {
                for (i = 0; i < 2; i++)
                {
                    dgd_LineWidth.Rows.Add();

                    switch (i)
                    {
                        case 0:
                            dgd_LineWidth.Rows[i].Cells[0].Value = "Seal 1 Width";
                            break;
                        case 1:
                            dgd_LineWidth.Rows[i].Cells[0].Value = "Seal 2 Width";
                            break;
                            //   case 2:
                            //       dgd_LineWidth.Rows[i].Cells[0].Value = "Distance";
                            //      break;
                    }

                    // dgd_LineWidth.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[i] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
                    if (i == 0)
                    {
                        if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[3] == -1)
                        {
                            dgd_LineWidth.Rows[i].Cells[1].Value = "---";
                            dgd_LineWidth.Rows[i].Cells[1].Style.SelectionBackColor = Color.White;//Lime
                            dgd_LineWidth.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                            dgd_LineWidth.Rows[i].Cells[1].Style.BackColor = Color.White;//Lime
                            dgd_LineWidth.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_LineWidth.Rows[i].Cells[1].Value = Math.Max(0, Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[3] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero));

                            if ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[3] < m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance1))
                            {
                                if (m_smVisionInfo.g_objSeal.ref_blnFailSeal1 &&
                                    ((m_smVisionInfo.g_objSeal.ref_intSealFailMask & 0x10) > 0))
                                {
                                    dgd_LineWidth.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                                    dgd_LineWidth.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
                                    dgd_LineWidth.Rows[i].Cells[1].Style.BackColor = Color.Red;
                                    dgd_LineWidth.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                                }
                                else
                                {
                                    dgd_LineWidth.Rows[i].Cells[1].Style.SelectionBackColor = Color.Yellow;
                                    dgd_LineWidth.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                                    dgd_LineWidth.Rows[i].Cells[1].Style.BackColor = Color.Yellow;
                                    dgd_LineWidth.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                dgd_LineWidth.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                                dgd_LineWidth.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                                dgd_LineWidth.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                                dgd_LineWidth.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                            }
                        }

                        if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[4] == -1)
                        {
                            dgd_LineWidth.Rows[i].Cells[2].Value = "---";
                            dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.White;//Lime
                            dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.White;//Lime
                            dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_LineWidth.Rows[i].Cells[2].Value = Math.Max(0, Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[4] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero));

                            if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[4] > m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance1)
                            {
                                if (m_smVisionInfo.g_objSeal.ref_blnFailSeal1 &&
                                ((m_smVisionInfo.g_objSeal.ref_intSealFailMask & 0x10) > 0))
                                {
                                    dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                    dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                                    dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                    dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                }
                                else
                                {
                                    dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Yellow;
                                    dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                                    dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Yellow;
                                    dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                                dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                            }
                        }

                        if (m_smVisionInfo.g_objSeal.ref_fFailSealScore1 == -1)
                        {
                            dgd_LineWidth.Rows[i].Cells[3].Value = "---";
                            dgd_LineWidth.Rows[i].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                            dgd_LineWidth.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            dgd_LineWidth.Rows[i].Cells[3].Style.BackColor = Color.White;//Lime
                            dgd_LineWidth.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_LineWidth.Rows[i].Cells[3].Value = (m_smVisionInfo.g_objSeal.ref_fFailSealScore1 * 100).ToString("f2");
                            if (m_smVisionInfo.g_objSeal.ref_fFailSealScore1 < m_smVisionInfo.g_objSeal.ref_fSealScoreTolerance)
                            {
                                dgd_LineWidth.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                                dgd_LineWidth.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                                dgd_LineWidth.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                dgd_LineWidth.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_LineWidth.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_LineWidth.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                                dgd_LineWidth.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                dgd_LineWidth.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            }
                        }

                    }
                    else if (i == 1)
                    {
                        if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[5] == -1)
                        {
                            dgd_LineWidth.Rows[i].Cells[1].Value = "---";
                            dgd_LineWidth.Rows[i].Cells[1].Style.SelectionBackColor = Color.White;//Lime
                            dgd_LineWidth.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                            dgd_LineWidth.Rows[i].Cells[1].Style.BackColor = Color.White;//Lime
                            dgd_LineWidth.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_LineWidth.Rows[i].Cells[1].Value = Math.Max(0, Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[5] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero));

                            if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[5] < m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance2)
                            {
                                if (m_smVisionInfo.g_objSeal.ref_blnFailSeal2 &&
                                ((m_smVisionInfo.g_objSeal.ref_intSealFailMask & 0x10) > 0))
                                {
                                    dgd_LineWidth.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                                    dgd_LineWidth.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
                                    dgd_LineWidth.Rows[i].Cells[1].Style.BackColor = Color.Red;
                                    dgd_LineWidth.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                                }
                                else
                                {
                                    dgd_LineWidth.Rows[i].Cells[1].Style.SelectionBackColor = Color.Yellow;
                                    dgd_LineWidth.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                                    dgd_LineWidth.Rows[i].Cells[1].Style.BackColor = Color.Yellow;
                                    dgd_LineWidth.Rows[i].Cells[1].Style.ForeColor = Color.Black;

                                }
                            }
                            else
                            {

                                dgd_LineWidth.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                                dgd_LineWidth.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                                dgd_LineWidth.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                                dgd_LineWidth.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                            }
                        }

                        if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[6] == -1)
                        {
                            dgd_LineWidth.Rows[i].Cells[2].Value = "---";
                            dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.White;//Lime
                            dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.White;//Lime
                            dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_LineWidth.Rows[i].Cells[2].Value = Math.Max(0, Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[6] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero));

                            if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[6] > m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance2)
                            {
                                if (m_smVisionInfo.g_objSeal.ref_blnFailSeal2 &&
                                ((m_smVisionInfo.g_objSeal.ref_intSealFailMask & 0x10) > 0))
                                {
                                    dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                    dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                                    dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                    dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                }
                                else
                                {
                                    dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Yellow;
                                    dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                                    dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Yellow;
                                    dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                                dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime ;
                                dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                            }
                        }

                        if (m_smVisionInfo.g_objSeal.ref_fFailSealScore2 == -1)
                        {
                            dgd_LineWidth.Rows[i].Cells[3].Value = "---";
                            dgd_LineWidth.Rows[i].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                            dgd_LineWidth.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            dgd_LineWidth.Rows[i].Cells[3].Style.BackColor = Color.White;//Lime
                            dgd_LineWidth.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_LineWidth.Rows[i].Cells[3].Value = (m_smVisionInfo.g_objSeal.ref_fFailSealScore2 * 100).ToString("f2");
                            if (m_smVisionInfo.g_objSeal.ref_fFailSealScore2 < m_smVisionInfo.g_objSeal.ref_fSealScoreTolerance)
                            {
                                dgd_LineWidth.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                                dgd_LineWidth.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                                dgd_LineWidth.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                dgd_LineWidth.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_LineWidth.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_LineWidth.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                                dgd_LineWidth.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                dgd_LineWidth.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            }
                        }
                    }
                }
            }

            dgd_LineWidth.Rows.Add();
            dgd_LineWidth.Rows[i].Cells[0].Value = "Unit Present";
            float fMarkScore = -1;
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x80) > 0) || ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x40) > 0))
                fMarkScore = m_smVisionInfo.g_objSeal.GetMarkMinScore(m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex) * 100;
            if (fMarkScore >= 0 && m_smVisionInfo.g_objSeal.ref_blnWantUsePatternCheckUnitPresent)
            {
                if (fMarkScore > (m_smVisionInfo.g_objSeal.ref_fMarkMinScore * 100))
                {
                    dgd_LineWidth.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_LineWidth.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    dgd_LineWidth.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_LineWidth.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                }
                else
                {
                    m_blnFailSeal = true;
                    dgd_LineWidth.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_LineWidth.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    dgd_LineWidth.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_LineWidth.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                }
                dgd_LineWidth.Rows[i].Cells[3].Value = fMarkScore.ToString("f2");
            }
            else
            {
                dgd_LineWidth.Rows[i].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                dgd_LineWidth.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                dgd_LineWidth.Rows[i].Cells[3].Style.BackColor = Color.White;//Lime
                dgd_LineWidth.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                dgd_LineWidth.Rows[i].Cells[3].Value = "--";
            }


            i++;
            dgd_LineWidth.Rows.Add();
            dgd_LineWidth.Rows[i].Cells[0].Value = "Pocket Empty";
            float fEmptyScore = m_smVisionInfo.g_objSeal.GetPocketMinScore(m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex) * 100;
            if (fEmptyScore >= 0)
            {
                if (fEmptyScore > (m_smVisionInfo.g_objSeal.ref_fPocketMinScore * 100))
                {
                    dgd_LineWidth.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_LineWidth.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    dgd_LineWidth.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_LineWidth.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                }
                else
                {
                    m_blnFailEmpty = true;
                    dgd_LineWidth.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_LineWidth.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    dgd_LineWidth.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_LineWidth.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;

                }
                dgd_LineWidth.Rows[i].Cells[3].Value = fEmptyScore.ToString("f2");
            }
            else
            {
                dgd_LineWidth.Rows[i].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                dgd_LineWidth.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                dgd_LineWidth.Rows[i].Cells[3].Style.BackColor = Color.White;//Lime
                dgd_LineWidth.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                dgd_LineWidth.Rows[i].Cells[3].Value = "--";
            }

            //float fEmptyScore = m_smVisionInfo.g_objSeal.GetPocketMinScore(m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex) * 100;
            //dgd_EmptyScore.Rows.Clear();
            //dgd_EmptyScore.Rows.Add();
            //dgd_EmptyScore.Rows[0].Cells[0].Value = "Empty";
            //if (fEmptyScore >= 0)
            //{
            //    if (fEmptyScore > (m_smVisionInfo.g_objSeal.ref_fPocketMinScore*100))
            //    {
            //        dgd_EmptyScore.Rows[0].Cells[0].Style.SelectionBackColor = Color.Lime;
            //        dgd_EmptyScore.Rows[0].Cells[1].Style.SelectionBackColor = Color.Lime;
            //        dgd_EmptyScore.Rows[0].Cells[0].Style.BackColor = Color.Lime;
            //        dgd_EmptyScore.Rows[0].Cells[1].Style.BackColor = Color.Lime;
            //    }
            //    else
            //    {
            //        m_blnFailEmpty = true;
            //        dgd_EmptyScore.Rows[0].Cells[0].Style.SelectionBackColor = Color.Red;
            //        dgd_EmptyScore.Rows[0].Cells[1].Style.SelectionBackColor = Color.Red;
            //        dgd_EmptyScore.Rows[0].Cells[0].Style.BackColor = Color.Red;
            //        dgd_EmptyScore.Rows[0].Cells[1].Style.BackColor = Color.Red;
            //    }
            //    dgd_EmptyScore.Rows[0].Cells[1].Value = fEmptyScore.ToString("f2");
            //}
            //else
            //{
            //    dgd_EmptyScore.Rows[0].Cells[0].Style.SelectionBackColor = Color.Lime;
            //    dgd_EmptyScore.Rows[0].Cells[1].Style.SelectionBackColor = Color.Lime;
            //    dgd_EmptyScore.Rows[0].Cells[0].Style.BackColor = Color.Lime;
            //    dgd_EmptyScore.Rows[0].Cells[1].Style.BackColor = Color.Lime;
            //    dgd_EmptyScore.Rows[0].Cells[1].Value = "--";
            //}

            //float fMarkScore = -1;

            //if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x80) > 0) || ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x40) > 0))
            //    fMarkScore = m_smVisionInfo.g_objSeal.GetMarkMinScore(m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex) * 100;
            //dgd_MarkScore.Rows.Clear();
            //dgd_MarkScore.Rows.Add();
            //dgd_MarkScore.Rows[0].Cells[0].Value = "Mark";
            //if (fMarkScore >= 0 && m_smVisionInfo.g_objSeal.ref_blnWantUsePatternCheckUnitPresent)
            //{
            //    if (fMarkScore > (m_smVisionInfo.g_objSeal.ref_fMarkMinScore*100))
            //    {
            //        dgd_MarkScore.Rows[0].Cells[0].Style.SelectionBackColor = Color.Lime;
            //        dgd_MarkScore.Rows[0].Cells[1].Style.SelectionBackColor = Color.Lime;
            //        dgd_MarkScore.Rows[0].Cells[0].Style.BackColor = Color.Lime;
            //        dgd_MarkScore.Rows[0].Cells[1].Style.BackColor = Color.Lime;
            //    }
            //    else
            //    {
            //        m_blnFailMark = true;
            //        dgd_MarkScore.Rows[0].Cells[0].Style.SelectionBackColor = Color.Red;
            //        dgd_MarkScore.Rows[0].Cells[1].Style.SelectionBackColor = Color.Red;
            //        dgd_MarkScore.Rows[0].Cells[0].Style.BackColor = Color.Red;
            //        dgd_MarkScore.Rows[0].Cells[1].Style.BackColor = Color.Red;
            //    }
            //    dgd_MarkScore.Rows[0].Cells[1].Value = fMarkScore.ToString("f2");
            //}
            //else
            //{
            //    dgd_MarkScore.Rows[0].Cells[0].Style.SelectionBackColor = Color.Lime;
            //    dgd_MarkScore.Rows[0].Cells[1].Style.SelectionBackColor = Color.Lime;
            //    dgd_MarkScore.Rows[0].Cells[0].Style.BackColor = Color.Lime;
            //    dgd_MarkScore.Rows[0].Cells[1].Style.BackColor = Color.Lime;
            //    dgd_MarkScore.Rows[0].Cells[1].Value = "--";
            //}

            if (m_smVisionInfo.MN_PR_CheckEmptyUnit)
            {
                dgd_EmptyScore.Visible = true;
                dgd_MarkScore.Visible = false;
            }
            else
            {
                dgd_MarkScore.Visible = true;
                dgd_EmptyScore.Visible = false;
            }
        }

        //private void LoadGeneralSetting(string strFolderPath)
        //{
        //    XmlParser objFile = new XmlParser(strFolderPath + "General.xml", false);
        //    objFile.GetFirstSection("TemplateCounting");
        //    m_smVisionInfo.g_intPocketTemplateTotal = objFile.GetValueAsInt("TotalPocketTemplates", 0, 1);
        //    m_smVisionInfo.g_intMarkTemplateTotal = objFile.GetValueAsInt("TotalMarkTemplates", 0, 1);
        //    m_smVisionInfo.g_intPocketTemplateMask = objFile.GetValueAsInt("PocketTemplateMask", 0, 1);
        //    m_smVisionInfo.g_intMarkTemplateMask = objFile.GetValueAsInt("MarkTemplateMask", 0, 1);
        //}

        private void btn_Close_Click(object sender, EventArgs e)
        {
            m_blnFailSeal = false;
            m_blnFailMark = false;
            m_blnFailEmpty = false;
            UpdateTabPageHeaderImage();

            //m_STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Offline Test Page Closed", "Exit Offline Test Page", "", "", m_smProductionInfo.g_strLotID);
            m_smVisionInfo.g_blnViewOfflinePage = false;
           
            m_smVisionInfo.g_objSeal.ClearBlobData();
            m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewROI = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_OfflinePageView = false;
            m_smVisionInfo.AT_VM_ManualTestMode = false;

            // 2020 07 22 - No suppose to load seal data in offline page bcos the setting wont change in offline page.
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //    m_smVisionInfo.g_strVisionFolderName + "\\";
            //m_smVisionInfo.g_objSeal.LoadSeal(strPath + "Seal\\Settings.xml", "Settings", m_smVisionInfo.g_fCalibPixelX);
            //LoadGeneralSetting(strPath);

            this.Hide();
            //Dispose();
        }

        private void btn_Inspect_Click(object sender, EventArgs e)
        {
            //m_STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Start Offline Test", " Pressed Test Button", "", "", m_smProductionInfo.g_strLotID);

            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("btn_Inspect > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString());
            }

            // btn_Inspect.Enabled = false;

            // 29-07-2019 ZJYEOH : If user pressed test button, then Image should stop live
            if (m_smVisionInfo.AT_PR_StartLiveImage)
            {
                m_smVisionInfo.AT_PR_StartLiveImage = false;
                m_smVisionInfo.AT_PR_TriggerLiveImage = true;
            }

            m_smVisionInfo.AT_VM_ManualTestMode = true;
            m_smVisionInfo.PR_MN_TestDone = false;
            m_smVisionInfo.MN_PR_StartTest = true;
            if (chk_Grab.Checked)
                m_smVisionInfo.MN_PR_GrabImage = true;
            //if (tab_VisionControl.TabPages.Contains(tabPage_LineWidth))
            //    tab_VisionControl.SelectedTab = tabPage_LineWidth;
        }

        
        private void tab_VisionControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tab_VisionControl.SelectedTab.Name)
            {
                case "tabPage_LineWidth":
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    tabPage_Seal.Controls.Add(pnl_Template);
                    break;
                case "tp_Score":
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    tp_Score.Controls.Add(pnl_Template);
                    break;
                default:
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = true;
                    break;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        public void CustomizeGUI()
        {
            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("Vision6OfflinePage Call UpdateInfo B");
            }

            m_smVisionInfo.VM_AT_OfflinePageView = true;
            UpdateInfo();
            //Cursor.Current = Cursors.Default;
        
            
            //m_smVisionInfo.g_blnViewROI = true;
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        public void CloseOfflinePage()
        {
            m_smVisionInfo.g_blnViewOfflinePage = false;

            m_smVisionInfo.g_objSeal.ClearBlobData();
            m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewROI = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_OfflinePageView = false;
            m_smVisionInfo.AT_VM_ManualTestMode = false;

            // 2020 07 22 - No suppose to load seal data in offline page bcos the setting wont change in offline page.
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //    m_smVisionInfo.g_strVisionFolderName + "\\";
            //m_smVisionInfo.g_objSeal.LoadSeal(strPath + "Seal\\Settings.xml", "Settings", m_smVisionInfo.g_fCalibPixelX);
            //LoadGeneralSetting(strPath);

            this.Hide();
        }

        private void Vision6OfflinePage_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
            m_smVisionInfo.VM_AT_OfflinePageView = true;
            m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            // 2020 07 22 - No suppose to load seal data in offline page bcos the setting wont change in offline page.
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //  m_smVisionInfo.g_strVisionFolderName + "\\";
            //LoadGeneralSetting(strPath);

        }

        private void Vision6OfflinePage_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.VM_AT_OfflinePageView = false;
            //m_smVisionInfo.g_objSeal.ClearBlobData();
            //m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
            //m_smVisionInfo.g_blnViewGauge = false;
            //m_smVisionInfo.g_blnViewROI = false;

            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }


        private void timer_TestResult_Tick(object sender, EventArgs e)
        {
            if (!m_smVisionInfo.VM_AT_OfflinePageView)
                return;

            if (m_smVisionInfo.PR_MN_UpdateInfo)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("Vision6OfflinePage Call UpdateInfo A");
                }

                m_blnFailSeal = false;
                m_blnFailMark = false;
                m_blnFailEmpty = false;
                UpdateTabPageHeaderImage();
                UpdateInfo();
                UpdateDefect();
                UpdateTabPageHeaderImage();
                btn_Inspect.Enabled = true;
                m_smVisionInfo.PR_MN_UpdateInfo = false;
            }

            if (m_smVisionInfo.PR_MN_UpdateSettingInfo)
            {
                m_smVisionInfo.g_blnUpdateImageNoComboBox = true; // 2020-11-22 ZJYEOH : After exit any setting form Update image combo box because g_intProductionViewImage may not same with g_intSelectedImage
                m_smVisionInfo.PR_MN_UpdateSettingInfo = false;
            }
            //if (label2.Text != m_smVisionInfo.g_objSeal.m_strTrack2.ToString())
            //    label2.Text = m_smVisionInfo.g_objSeal.m_strTrack2;
        }

        private void UpdateDefect_New()
        {
            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            }

            dgd_Defect.Rows.Clear();

            //switch (m_smVisionInfo.g_objSeal.ref_intSealFailMask)
            //{
            //    // 1 = shift position; 2=distance; 4=bubble found; 8=broken line; 10=overseal; 20=insufficient 40=overheat 80=mark 100=pocket
            //    //case 1:
            //    //    dgd_Defect.Rows.Add();
            //    //    dgd_Defect.Rows[0].Cells[0].Value = (1).ToString();
            //    //    dgd_Defect.Rows[0].Cells[1].Value = ("Position Shifted").ToString();
            //    //    dgd_Defect.Rows[0].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailPosition, 3, MidpointRounding.AwayFromZero);
            //    //   // (m_smVisionInfo.g_objSeal.ref_FailPosition).ToString();
            //    //    break;
            //    case 2:
            int i = 0;
            // Fail Distance Update
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x08) > 0)
            {

                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.1. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }
                dgd_Defect.Rows.Add();
                dgd_Defect.Rows[i].Cells[0].Value = (1).ToString();
                dgd_Defect.Rows[i].Cells[1].Value = ("Seal Distance").ToString();
                dgd_Defect.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailDistance, 3, MidpointRounding.AwayFromZero);

                if ((m_smVisionInfo.g_objSeal.ref_intSealFailMask & 0x02) == 0)
                {
                    dgd_Defect.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                    dgd_Defect.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    dgd_Defect.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                    dgd_Defect.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Defect.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                    dgd_Defect.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                    dgd_Defect.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                    dgd_Defect.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Defect.Rows[i].Cells[0].Style.SelectionForeColor = Color.Black;
                    dgd_Defect.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                    dgd_Defect.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                    dgd_Defect.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    dgd_Defect.Rows[i].Cells[0].Style.ForeColor = Color.Black;
                    dgd_Defect.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Defect.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                    dgd_Defect.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                }
                else
                {
                    m_blnFailSeal = true;
                    dgd_Defect.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                    dgd_Defect.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgd_Defect.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                    dgd_Defect.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_Defect.Rows[i].Cells[0].Style.BackColor = Color.Red;
                    dgd_Defect.Rows[i].Cells[1].Style.BackColor = Color.Red;
                    dgd_Defect.Rows[i].Cells[2].Style.BackColor = Color.Red;
                    dgd_Defect.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_Defect.Rows[i].Cells[0].Style.SelectionForeColor = Color.Yellow;
                    dgd_Defect.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
                    dgd_Defect.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                    dgd_Defect.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    dgd_Defect.Rows[i].Cells[0].Style.ForeColor = Color.Yellow;
                    dgd_Defect.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                    dgd_Defect.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                    dgd_Defect.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                }

                i++;
            }

            // Fail Bubble Update
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x02) > 0)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.2. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }
                dgd_Defect.Rows.Add();
                dgd_Defect.Rows[i].Cells[0].Value = (1).ToString();
                dgd_Defect.Rows[i].Cells[1].Value = ("Bubble in Seal 1").ToString();
                dgd_Defect.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailBubble1, 5, MidpointRounding.AwayFromZero);
                //(m_smVisionInfo.g_objSeal.ref_FailBubble1).ToString();
                dgd_Defect.Rows.Add();
                dgd_Defect.Rows[1].Cells[0].Value = (1).ToString();
                dgd_Defect.Rows[1].Cells[1].Value = ("Bubble in Seal 2").ToString();
                dgd_Defect.Rows[1].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailBubble2, 5, MidpointRounding.AwayFromZero);
                //(m_smVisionInfo.g_objSeal.ref_FailBubble2).ToString();
                //if (dgd_Defect.Rows[1].Cells[3].Value.ToString() == "0")
                //    dgd_Defect.Rows.Remove(dgd_Defect.Rows[1]);
                //if (dgd_Defect.Rows[i].Cells[3].Value.ToString() == "0")
                //    dgd_Defect.Rows.Remove(dgd_Defect.Rows[i]);

                i++;
            }

            // Fail Broken Gap 
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x20) > 0)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.3. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }
                dgd_Defect.Rows.Add();
                dgd_Defect.Rows[i].Cells[0].Value = (1).ToString();
                dgd_Defect.Rows[i].Cells[1].Value = ("Broken Seal in Seal 1").ToString();
                dgd_Defect.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailBrokenSeal1, 5, MidpointRounding.AwayFromZero);

                //(m_smVisionInfo.g_objSeal.ref_FailBrokenSeal1).ToString();
                dgd_Defect.Rows.Add();
                dgd_Defect.Rows[1].Cells[0].Value = (1).ToString();
                dgd_Defect.Rows[1].Cells[1].Value = ("Broken Seal in Seal 2").ToString();
                dgd_Defect.Rows[1].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailBrokenSeal2, 5, MidpointRounding.AwayFromZero);

                ////(m_smVisionInfo.g_objSeal.ref_FailBrokenSeal2).ToString();
                //if ((string)dgd_Defect.Rows[1].Cells[2].Value.ToString() == "0")
                //    dgd_Defect.Rows.Remove(dgd_Defect.Rows[1]);
                //if ((string)dgd_Defect.Rows[i].Cells[2].Value.ToString() == "0")
                //    dgd_Defect.Rows.Remove(dgd_Defect.Rows[i]);

                i++;
            }

            // Fail Seal Width Over
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x01) > 0)
            {
                ////////if (m_smVisionInfo.g_blnTrackSealOption)
                ////////{
                ////////    STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.4. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                ////////}
                ////////dgd_Defect.Rows.Add();
                ////////dgd_Defect.Rows[i].Cells[0].Value = (1).ToString();
                ////////dgd_Defect.Rows[i].Cells[1].Value = ("Over Seal in Seal 1").ToString();
                ////////dgd_Defect.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailOverSeal1, 3, MidpointRounding.AwayFromZero);
                //////////(m_smVisionInfo.g_objSeal.ref_FailOverSeal1).ToString();
                ////////dgd_Defect.Rows.Add();
                ////////dgd_Defect.Rows[1].Cells[0].Value = (1).ToString();
                ////////dgd_Defect.Rows[1].Cells[1].Value = ("Over Seal in Seal 2").ToString();
                ////////dgd_Defect.Rows[1].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailOverSeal2, 3, MidpointRounding.AwayFromZero);
                //////////(m_smVisionInfo.g_objSeal.ref_FailOverSeal2).ToString();
                ////////if (dgd_Defect.Rows[1].Cells[2].Value.ToString() == "0")
                ////////    dgd_Defect.Rows.Remove(dgd_Defect.Rows[1]);
                ////////if (dgd_Defect.Rows[i].Cells[2].Value.ToString() == "0")
                ////////    dgd_Defect.Rows.Remove(dgd_Defect.Rows[i]);

                ////////i++;
            }

            // Fail Seal Width Insufficient
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x01) > 0)
            {
                ////////if (m_smVisionInfo.g_blnTrackSealOption)
                ////////{
                ////////    STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.5. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                ////////}
                ////////dgd_Defect.Rows.Add();
                ////////dgd_Defect.Rows[i].Cells[0].Value = (1).ToString();
                ////////dgd_Defect.Rows[i].Cells[1].Value = ("Insufficient Seal in Seal 1").ToString();
                ////////dgd_Defect.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailInsufficient1, 3, MidpointRounding.AwayFromZero);
                //////////(m_smVisionInfo.g_objSeal.ref_FailInsufficient1).ToString();
                ////////dgd_Defect.Rows.Add();
                ////////dgd_Defect.Rows[1].Cells[0].Value = (1).ToString();
                ////////dgd_Defect.Rows[1].Cells[1].Value = ("Insufficient Seal in Seal 2").ToString();
                ////////dgd_Defect.Rows[1].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailInsufficient2, 3, MidpointRounding.AwayFromZero);
                //////////(m_smVisionInfo.g_objSeal.ref_FailInsufficient2).ToString();
                ////////if (dgd_Defect.Rows[1].Cells[2].Value.ToString() == "0")
                ////////    dgd_Defect.Rows.Remove(dgd_Defect.Rows[1]);
                ////////if (dgd_Defect.Rows[i].Cells[2].Value.ToString() == "0")
                ////////    dgd_Defect.Rows.Remove(dgd_Defect.Rows[i]);

                ////////i++;
            }

            // Fail Over Heat 
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x10) > 0)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.6. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }
                dgd_Defect.Rows.Add();
                dgd_Defect.Rows[i].Cells[0].Value = (1).ToString();
                dgd_Defect.Rows[i].Cells[1].Value = ("Over Heat").ToString();
                dgd_Defect.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.GetOverHeatFailArea(0), 5, MidpointRounding.AwayFromZero);
                //(m_smVisionInfo.g_objSeal.ref_FailOverheatArea).ToString();

                i++;

                for (int j = 1; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                {
                    dgd_Defect.Rows.Add();
                    dgd_Defect.Rows[i].Cells[0].Value = (1).ToString();
                    dgd_Defect.Rows[i].Cells[1].Value = "Over Heat " + (j + 1).ToString();
                    dgd_Defect.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.GetOverHeatFailArea(j), 5, MidpointRounding.AwayFromZero);
                    //(m_smVisionInfo.g_objSeal.ref_FailOverheatArea).ToString();

                    i++;
                }
            }

            // Fail Unit Present Unit White Pixel
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x40) > 0)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.7. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }
                dgd_Defect.Rows.Add();
                dgd_Defect.Rows[i].Cells[0].Value = (1).ToString();
                if (m_smVisionInfo.g_objSeal.ref_blnWhiteOnBlack)
                    dgd_Defect.Rows[i].Cells[1].Value = ("Unit White Pixel").ToString();
                else
                    dgd_Defect.Rows[i].Cells[1].Value = ("Unit Black Pixel").ToString();
                dgd_Defect.Rows[i].Cells[3].Value = m_smVisionInfo.g_objSeal.ref_FailUnitPresentWhiteArea;

                i++;
            }

            // Fail Sprocket Hole Distance
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x100) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.8. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }
                if (m_smVisionInfo.g_objSealCircleGauges.ref_GaugeScore > m_smVisionInfo.g_objSealCircleGauges.ref_intMinScore)
                {
                    dgd_Defect.Rows.Add();
                    dgd_Defect.Rows[i].Cells[0].Value = (1).ToString();
                    dgd_Defect.Rows[i].Cells[1].Value = ("Sprocket Hole Distance").ToString();
                    dgd_Defect.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailSprocketHoleDistance, 3, MidpointRounding.AwayFromZero);

                    i++;
                }
            }

            // Fail Sprocket Hole Diameter
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x200) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.8. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }
                if (m_smVisionInfo.g_objSealCircleGauges.ref_GaugeScore > m_smVisionInfo.g_objSealCircleGauges.ref_intMinScore)
                {
                    dgd_Defect.Rows.Add();
                    dgd_Defect.Rows[i].Cells[0].Value = (1).ToString();
                    dgd_Defect.Rows[i].Cells[1].Value = ("Sprocket Hole Diameter").ToString();
                    dgd_Defect.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailSprocketHoleDiameter, 3, MidpointRounding.AwayFromZero);

                    i++;
                }
            }

            // Fail Sprocket Hole Defect
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x400) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.8. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }
                if (m_smVisionInfo.g_objSealCircleGauges.ref_GaugeScore > m_smVisionInfo.g_objSealCircleGauges.ref_intMinScore)
                {
                    dgd_Defect.Rows.Add();
                    dgd_Defect.Rows[i].Cells[0].Value = (1).ToString();
                    dgd_Defect.Rows[i].Cells[1].Value = ("Sprocket Hole Defect").ToString();
                    dgd_Defect.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailSprocketHoleDefectArea, 3, MidpointRounding.AwayFromZero);

                    i++;
                }
            }

            // Fail Sprocket Hole Broken
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x800) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.8. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }
                if (m_smVisionInfo.g_objSealCircleGauges.ref_GaugeScore > m_smVisionInfo.g_objSealCircleGauges.ref_intMinScore)
                {
                    dgd_Defect.Rows.Add();
                    dgd_Defect.Rows[i].Cells[0].Value = (1).ToString();
                    dgd_Defect.Rows[i].Cells[1].Value = ("Sprocket Hole Broken").ToString();
                    dgd_Defect.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailSprocketHoleBrokenArea, 3, MidpointRounding.AwayFromZero);

                    i++;
                }
            }

            // Fail Sprocket Hole Roundness
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x1000) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.8. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }
                if (m_smVisionInfo.g_objSealCircleGauges.ref_GaugeScore > m_smVisionInfo.g_objSealCircleGauges.ref_intMinScore)
                {
                    dgd_Defect.Rows.Add();
                    dgd_Defect.Rows[i].Cells[0].Value = (1).ToString();
                    dgd_Defect.Rows[i].Cells[1].Value = ("Sprocket Hole Roundness").ToString();
                    dgd_Defect.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailSprocketHoleRoundness, 3, MidpointRounding.AwayFromZero);

                    i++;
                }
            }

            // Fail Tape Scratches 
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x400) > 0)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.6. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }
                dgd_Defect.Rows.Add();
                dgd_Defect.Rows[i].Cells[0].Value = (1).ToString();
                dgd_Defect.Rows[i].Cells[1].Value = ("Tape Scratches").ToString();
                dgd_Defect.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.GetScratchesFailArea(0), 5, MidpointRounding.AwayFromZero);

                i++;

                for (int j = 1; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                {
                    dgd_Defect.Rows.Add();
                    dgd_Defect.Rows[i].Cells[0].Value = (1).ToString();
                    dgd_Defect.Rows[i].Cells[1].Value = "Tape Scratches " + (j + 1).ToString();
                    dgd_Defect.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.GetScratchesFailArea(j), 5, MidpointRounding.AwayFromZero);
                    //(m_smVisionInfo.g_objSeal.ref_FailOverheatArea).ToString();

                    i++;
                }
            }

            // Fail Seal Edge Straightness 
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x2000) > 0) && m_smVisionInfo.g_objSeal.ref_blnWantCheckSealEdgeStraightness)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.6. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }
                dgd_Defect.Rows.Add();
                dgd_Defect.Rows[i].Cells[0].Value = (1).ToString();
                dgd_Defect.Rows[i].Cells[1].Value = ("Seal Edge Straightness").ToString();
                dgd_Defect.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailSealEdgeStraightnessArea, 5, MidpointRounding.AwayFromZero);

                i++;
            }

            //default:
            //    if (m_smVisionInfo.g_blnTrackSealOption)
            //    {
            //        STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.9. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            //    }
            //    break;

            if (dgd_Defect.Rows.Count > 0)
            {
                m_blnFailSeal = true;
            }
        }

        //private void UpdateDefect()
        //{
        //    if (m_smVisionInfo.g_blnTrackSealOption)
        //    {
        //        STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
        //    }

        //    dgd_Defect.Rows.Clear();

        //    switch (m_smVisionInfo.g_objSeal.ref_intSealFailMask)
        //    {
        //        // 1 = shift position; 2=distance; 4=bubble found; 8=broken line; 10=overseal; 20=insufficient 40=overheat 80=mark 100=pocket
        //        //case 1:
        //        //    dgd_Defect.Rows.Add();
        //        //    dgd_Defect.Rows[0].Cells[0].Value = (1).ToString();
        //        //    dgd_Defect.Rows[0].Cells[1].Value = ("Position Shifted").ToString();
        //        //    dgd_Defect.Rows[0].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailPosition, 3, MidpointRounding.AwayFromZero);
        //        //   // (m_smVisionInfo.g_objSeal.ref_FailPosition).ToString();
        //        //    break;
        //        case 2:
        //            if (m_smVisionInfo.g_blnTrackSealOption)
        //            {
        //                STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.1. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
        //            }
        //            dgd_Defect.Rows.Add();
        //            dgd_Defect.Rows[0].Cells[0].Value = (1).ToString();
        //            dgd_Defect.Rows[0].Cells[1].Value = ("Seal Distance").ToString();
        //            dgd_Defect.Rows[0].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailDistance, 3, MidpointRounding.AwayFromZero);
        //            // (m_smVisionInfo.g_objSeal.ref_FailDistance).ToString();
        //            break;
        //        case 4:
        //            if (m_smVisionInfo.g_blnTrackSealOption)
        //            {
        //                STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.2. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
        //            }
        //            dgd_Defect.Rows.Add();
        //            dgd_Defect.Rows[0].Cells[0].Value = (1).ToString();
        //            dgd_Defect.Rows[0].Cells[1].Value = ("Seal 1 Broken Area").ToString();
        //            dgd_Defect.Rows[0].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailBubble1, 5, MidpointRounding.AwayFromZero);
        //            //(m_smVisionInfo.g_objSeal.ref_FailBubble1).ToString();
        //            dgd_Defect.Rows.Add();
        //            dgd_Defect.Rows[1].Cells[0].Value = (1).ToString();
        //            dgd_Defect.Rows[1].Cells[1].Value = ("Seal 2 Broken Area").ToString();
        //            dgd_Defect.Rows[1].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailBubble2, 5, MidpointRounding.AwayFromZero);
        //            //(m_smVisionInfo.g_objSeal.ref_FailBubble2).ToString();
        //            if (dgd_Defect.Rows[1].Cells[3].Value.ToString() == "0")
        //                dgd_Defect.Rows.Remove(dgd_Defect.Rows[1]);
        //            if (dgd_Defect.Rows[0].Cells[3].Value.ToString() == "0")
        //                dgd_Defect.Rows.Remove(dgd_Defect.Rows[0]);
        //            break;
        //        case 8:
        //            //if (m_smVisionInfo.g_blnTrackSealOption)
        //            //{
        //            //    STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.3. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
        //            //}
        //            //dgd_Defect.Rows.Add();
        //            //dgd_Defect.Rows[0].Cells[0].Value = (1).ToString();
        //            //dgd_Defect.Rows[0].Cells[1].Value = ("Broken Seal in Seal 1").ToString();
        //            //dgd_Defect.Rows[0].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailBrokenSeal1, 5, MidpointRounding.AwayFromZero);
        //            ////(m_smVisionInfo.g_objSeal.ref_FailBrokenSeal1).ToString();
        //            //dgd_Defect.Rows.Add();
        //            //dgd_Defect.Rows[1].Cells[0].Value = (1).ToString();
        //            //dgd_Defect.Rows[1].Cells[1].Value = ("Broken Seal in Seal 2").ToString();
        //            //dgd_Defect.Rows[1].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailBrokenSeal2, 5, MidpointRounding.AwayFromZero);
        //            //// (m_smVisionInfo.g_objSeal.ref_FailBrokenSeal2).ToString();
        //            //if (dgd_Defect.Rows[1].Cells[3].Value.ToString() == "0")
        //            //    dgd_Defect.Rows.Remove(dgd_Defect.Rows[1]);
        //            //if (dgd_Defect.Rows[0].Cells[3].Value.ToString() == "0")
        //            //    dgd_Defect.Rows.Remove(dgd_Defect.Rows[0]);
        //            //break;
        //            if (m_smVisionInfo.g_blnTrackSealOption)
        //            {
        //                STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.3. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
        //            }
        //            dgd_Defect.Rows.Add();
        //            dgd_Defect.Rows[0].Cells[0].Value = (1).ToString();
        //            dgd_Defect.Rows[0].Cells[1].Value = ("Seal 1 Broken Gap").ToString();
        //            dgd_Defect.Rows[0].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailBrokenSeal1, 4, MidpointRounding.AwayFromZero);

        //            //(m_smVisionInfo.g_objSeal.ref_FailBrokenSeal1).ToString();
        //            dgd_Defect.Rows.Add();
        //            dgd_Defect.Rows[1].Cells[0].Value = (1).ToString();
        //            dgd_Defect.Rows[1].Cells[1].Value = ("Seal 2 Broken Gap").ToString();
        //            dgd_Defect.Rows[1].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailBrokenSeal2, 4, MidpointRounding.AwayFromZero);

        //            //(m_smVisionInfo.g_objSeal.ref_FailBrokenSeal2).ToString();
        //            if ((string)dgd_Defect.Rows[1].Cells[2].Value.ToString() == "0")
        //                dgd_Defect.Rows.Remove(dgd_Defect.Rows[1]);
        //            if ((string)dgd_Defect.Rows[0].Cells[2].Value.ToString() == "0")
        //                dgd_Defect.Rows.Remove(dgd_Defect.Rows[0]);
        //            break;
        //        case 16:
        //            if (m_smVisionInfo.g_blnTrackSealOption)
        //            {
        //                STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.4. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
        //            }
        //            dgd_Defect.Rows.Add();
        //            dgd_Defect.Rows[0].Cells[0].Value = (1).ToString();
        //            dgd_Defect.Rows[0].Cells[1].Value = ("Over Seal in Seal 1").ToString();
        //            dgd_Defect.Rows[0].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailOverSeal1, 3, MidpointRounding.AwayFromZero);
        //            //(m_smVisionInfo.g_objSeal.ref_FailOverSeal1).ToString();
        //            dgd_Defect.Rows.Add();
        //            dgd_Defect.Rows[1].Cells[0].Value = (1).ToString();
        //            dgd_Defect.Rows[1].Cells[1].Value = ("Over Seal in Seal 2").ToString();
        //            dgd_Defect.Rows[1].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailOverSeal2, 3, MidpointRounding.AwayFromZero);
        //            //(m_smVisionInfo.g_objSeal.ref_FailOverSeal2).ToString();
        //            if (dgd_Defect.Rows[1].Cells[2].Value.ToString() == "0")
        //                dgd_Defect.Rows.Remove(dgd_Defect.Rows[1]);
        //            if (dgd_Defect.Rows[0].Cells[2].Value.ToString() == "0")
        //                dgd_Defect.Rows.Remove(dgd_Defect.Rows[0]);
        //            break;
        //        case 32:
        //            if (m_smVisionInfo.g_blnTrackSealOption)
        //            {
        //                STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.5. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
        //            }
        //            dgd_Defect.Rows.Add();
        //            dgd_Defect.Rows[0].Cells[0].Value = (1).ToString();
        //            dgd_Defect.Rows[0].Cells[1].Value = ("Insufficient Seal in Seal 1").ToString();
        //            dgd_Defect.Rows[0].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailInsufficient1, 3, MidpointRounding.AwayFromZero);
        //            //(m_smVisionInfo.g_objSeal.ref_FailInsufficient1).ToString();
        //            dgd_Defect.Rows.Add();
        //            dgd_Defect.Rows[1].Cells[0].Value = (1).ToString();
        //            dgd_Defect.Rows[1].Cells[1].Value = ("Insufficient Seal in Seal 2").ToString();
        //            dgd_Defect.Rows[1].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailInsufficient2, 3, MidpointRounding.AwayFromZero);
        //            //(m_smVisionInfo.g_objSeal.ref_FailInsufficient2).ToString();
        //            if (dgd_Defect.Rows[1].Cells[2].Value.ToString() == "0")
        //                dgd_Defect.Rows.Remove(dgd_Defect.Rows[1]);
        //            if (dgd_Defect.Rows[0].Cells[2].Value.ToString() == "0")
        //                dgd_Defect.Rows.Remove(dgd_Defect.Rows[0]);
        //            break;
        //        case 64:
        //            if (m_smVisionInfo.g_blnTrackSealOption)
        //            {
        //                STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.6. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
        //            }
        //            dgd_Defect.Rows.Add();
        //            dgd_Defect.Rows[0].Cells[0].Value = (1).ToString();
        //            dgd_Defect.Rows[0].Cells[1].Value = ("Over Heat").ToString();
        //            dgd_Defect.Rows[0].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailOverheatArea, 5, MidpointRounding.AwayFromZero);
        //            //(m_smVisionInfo.g_objSeal.ref_FailOverheatArea).ToString();
        //            break;
        //        case 1152:  // 0x480
        //            if (m_smVisionInfo.g_blnTrackSealOption)
        //            {
        //                STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.7. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
        //            }
        //            dgd_Defect.Rows.Add();
        //            dgd_Defect.Rows[0].Cells[0].Value = (1).ToString();
        //            if (m_smVisionInfo.g_objSeal.ref_blnWhiteOnBlack)
        //                dgd_Defect.Rows[0].Cells[1].Value = ("Unit White Pixel").ToString();
        //            else
        //                dgd_Defect.Rows[0].Cells[1].Value = ("Unit Black Pixel").ToString();
        //            dgd_Defect.Rows[0].Cells[3].Value = m_smVisionInfo.g_objSeal.ref_FailUnitPresentWhiteArea;
        //            break;
        //        case 512:   // 0x200
        //            if (m_smVisionInfo.g_blnTrackSealOption)
        //            {
        //                STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.8. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
        //            }
        //            if (m_smVisionInfo.g_objSealCircleGauges.ref_GaugeScore > m_smVisionInfo.g_objSealCircleGauges.ref_intMinScore)
        //            {
        //                dgd_Defect.Rows.Add();
        //                dgd_Defect.Rows[0].Cells[0].Value = (1).ToString();
        //                dgd_Defect.Rows[0].Cells[1].Value = ("Sprocket Hole Distance").ToString();
        //                dgd_Defect.Rows[0].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailSprocketHoleDistance, 3, MidpointRounding.AwayFromZero);
        //            }
        //            break;
        //        case 0x800:
        //            if (m_smVisionInfo.g_blnTrackSealOption)
        //            {
        //                STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.6. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
        //            }
        //            dgd_Defect.Rows.Add();
        //            dgd_Defect.Rows[0].Cells[0].Value = (1).ToString();
        //            dgd_Defect.Rows[0].Cells[1].Value = ("Tape Scratches").ToString();
        //            dgd_Defect.Rows[0].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_FailScratchesArea, 5, MidpointRounding.AwayFromZero);
        //            break;
        //        default:
        //            if (m_smVisionInfo.g_blnTrackSealOption)
        //            {
        //                STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1.9. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
        //            }
        //            break;
        //    }

        //    if (dgd_Defect.Rows.Count > 0)
        //    {
        //        m_blnFailSeal = true;
        //    }
        //}

        private void UpdateDefect()
        {
            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("Vision6OfflinePage Call UpdateDefect 1. intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            }

            dgd_Setting.Rows.Clear();

            int i = -1;
            // Display Distance Result
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x08) > 0)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Distance";
                dgd_Setting.Rows[i].Cells[2].Value = "mm";
                dgd_Setting.Rows[i].Cells[1].Value = Math.Max(0, Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero));


                if ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] >= 0 &&
                    ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] < m_smVisionInfo.g_objSeal.ref_fDistanceMinTolerance) ||
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] > m_smVisionInfo.g_objSeal.ref_fDistanceMaxTolerance))) ||
                    ((m_smVisionInfo.g_objSeal.ref_intSealFailMask & 0x02) > 0))
                {
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;

                    m_blnFailSeal = true;
                }
                else if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] < 0)
                {
                    dgd_Setting.Rows[i].Cells[1].Value = "---";
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }
            }

            // Display Sprocket Hole Distance Result
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x100) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Sprocket Hole Distance";
                dgd_Setting.Rows[i].Cells[2].Value = "mm";
                dgd_Setting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

                if (/*m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] >= 0 &&*/
                    ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] < m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMinTolerance) ||
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] > m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMaxTolerance)))
                {
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;

                    m_blnFailSeal = true;
                }
                else if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] < 0)
                {
                    dgd_Setting.Rows[i].Cells[1].Value = "---";
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }

            }

            // Display Sprocket Hole Diameter Result
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x200) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Sprocket Hole Diameter";
                dgd_Setting.Rows[i].Cells[2].Value = "mm";
                dgd_Setting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDiameter] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDiameter] >= 0 &&
                    ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDiameter] < m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMinTolerance) ||
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDiameter] > m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMaxTolerance)))
                {
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;

                    m_blnFailSeal = true;
                }
                else if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDiameter] < 0)
                {
                    dgd_Setting.Rows[i].Cells[1].Value = "---";
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }

            }

            // Display Sprocket Hole Defect Result
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x400) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Sprocket Hole Defect";
                dgd_Setting.Rows[i].Cells[2].Value = "mm^2";
                dgd_Setting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDefect] / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDefect] >= 0 &&
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDefect] > m_smVisionInfo.g_objSeal.ref_fSprocketHoleDefectMaxTolerance))
                {
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;

                    m_blnFailSeal = true;
                }
                else if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDefect] < 0)
                {
                    dgd_Setting.Rows[i].Cells[1].Value = "---";
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }

            }

            // Display Sprocket Hole Broken Result
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x800) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Sprocket Hole Broken";
                dgd_Setting.Rows[i].Cells[2].Value = "mm^2";
                dgd_Setting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleBroken] / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleBroken] >= 0 &&
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleBroken] > m_smVisionInfo.g_objSeal.ref_fSprocketHoleBrokenMaxTolerance))
                {
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;

                    m_blnFailSeal = true;
                }
                else if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleBroken] < 0)
                {
                    dgd_Setting.Rows[i].Cells[1].Value = "---";
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }

            }

            // Display Sprocket Hole Roundness Result
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x1000) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Sprocket Hole Roundness";
                dgd_Setting.Rows[i].Cells[2].Value = "";
                dgd_Setting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleRoundness], 3, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleRoundness] >= 0 &&
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleRoundness] > m_smVisionInfo.g_objSeal.ref_fSprocketHoleRoundnessMaxTolerance))
                {
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;

                    m_blnFailSeal = true;
                }
                else if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleRoundness] < 0)
                {
                    dgd_Setting.Rows[i].Cells[1].Value = "---";
                    if ((m_smVisionInfo.g_objSeal.ref_intSealFailMask & 0x10000) > 0)
                    {
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
                        m_blnFailSeal = true;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White;
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                    }
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }

            }

            // Display Over Heat Result
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x10) > 0)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Over Heat Size";
                dgd_Setting.Rows[i].Cells[2].Value = "mm^2";
                if (m_smVisionInfo.g_objSeal.GetOverHeatFailArea(0) == -999)
                {
                    dgd_Setting.Rows[i].Cells[1].Value = "---";
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White; //Lime
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White; //Lime
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[1].Value = Math.Round((m_smVisionInfo.g_objSeal.GetOverHeatFailArea(0)), 5, MidpointRounding.AwayFromZero);

                    if (m_smVisionInfo.g_objSeal.GetOverHeatFailArea(0) > m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(0))
                    {
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;

                        m_blnFailSeal = true;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                    }
                }

                for (int j = 1; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                {
                    i++;
                    dgd_Setting.Rows.Add();
                    dgd_Setting.Rows[i].Cells[0].Value = "Over Heat Size " + (j + 1).ToString();
                    dgd_Setting.Rows[i].Cells[2].Value = "mm^2";
                    if (m_smVisionInfo.g_objSeal.GetOverHeatFailArea(j) == -999)
                    {
                        dgd_Setting.Rows[i].Cells[1].Value = "---";
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White; //Lime
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White; //Lime
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[1].Value = Math.Round((m_smVisionInfo.g_objSeal.GetOverHeatFailArea(j)), 5, MidpointRounding.AwayFromZero);

                        if (m_smVisionInfo.g_objSeal.GetOverHeatFailArea(j) > m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(j))
                        {
                            dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                            dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;

                            m_blnFailSeal = true;
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                        }
                    }
                }
            }

            // Display Tape Scatches Result
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x10) > 0)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Tape Scratches Size";
                dgd_Setting.Rows[i].Cells[2].Value = "mm^2";
                if (m_smVisionInfo.g_objSeal.GetScratchesFailArea(0) == -999)
                {
                    dgd_Setting.Rows[i].Cells[1].Value = "---";
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White; //Lime
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White; //Lime
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[1].Value = Math.Round((m_smVisionInfo.g_objSeal.GetScratchesFailArea(0)), 5, MidpointRounding.AwayFromZero);

                    if (m_smVisionInfo.g_objSeal.GetScratchesFailArea(0) > m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(0))
                    {
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;

                        m_blnFailSeal = true;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                    }
                }

                for (int j = 1; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                {
                    i++;
                    dgd_Setting.Rows.Add();
                    dgd_Setting.Rows[i].Cells[0].Value = "Tape Scratches Size " + (j + 1).ToString();
                    dgd_Setting.Rows[i].Cells[2].Value = "mm^2";
                    if (m_smVisionInfo.g_objSeal.GetScratchesFailArea(j) == -999)
                    {
                        dgd_Setting.Rows[i].Cells[1].Value = "---";
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White; //Lime
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White; //Lime
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[1].Value = Math.Round((m_smVisionInfo.g_objSeal.GetScratchesFailArea(j)), 5, MidpointRounding.AwayFromZero);

                        if (m_smVisionInfo.g_objSeal.GetScratchesFailArea(j) > m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(j))
                        {
                            dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                            dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;

                            m_blnFailSeal = true;
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                        }
                    }
                }
            }

            // Display Tape Scatches Result
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x2000) > 0) && m_smVisionInfo.g_objSeal.ref_blnWantCheckSealEdgeStraightness)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Seal Edge Straightness";
                dgd_Setting.Rows[i].Cells[2].Value = "mm^2";
                if (m_smVisionInfo.g_objSeal.ref_FailSealEdgeStraightnessArea == -999)
                {
                    dgd_Setting.Rows[i].Cells[1].Value = "---";
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White; //Lime
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White; //Lime
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[1].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailSealEdgeStraightnessArea), 5, MidpointRounding.AwayFromZero);

                    if (m_smVisionInfo.g_objSeal.ref_FailSealEdgeStraightnessArea > (m_smVisionInfo.g_objSeal.ref_fSealEdgeStraightnessMaxTolerance / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;

                        m_blnFailSeal = true;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                    }
                }
            }

            // Display Unit Precense White Area
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x40) > 0)
            {
                if (m_smVisionInfo.g_objSeal.ref_blnWantUsePixelCheckUnitPresent)
                {
                    i++;
                    dgd_Setting.Rows.Add();
                    if (m_smVisionInfo.g_objSeal.ref_blnWhiteOnBlack)
                        dgd_Setting.Rows[i].Cells[0].Value = "Unit White Area";
                    else
                        dgd_Setting.Rows[i].Cells[0].Value = "Unit Black Area";
                    dgd_Setting.Rows[i].Cells[2].Value = "%";

                    if (m_smVisionInfo.g_objSeal.ref_FailUnitPresentWhiteArea == -999)
                    {
                        dgd_Setting.Rows[i].Cells[1].Value = "---";
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White; //Lime
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White; //Lime
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[1].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailUnitPresentWhiteArea), 5, MidpointRounding.AwayFromZero);

                        if ((m_smVisionInfo.g_objSeal.ref_FailUnitPresentWhiteArea < m_smVisionInfo.g_objSeal.ref_fMarkMinWhiteArea) ||
                            (m_smVisionInfo.g_objSeal.ref_FailUnitPresentWhiteArea > m_smVisionInfo.g_objSeal.ref_fMarkMaxWhiteArea))
                        {
                            dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                            dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;

                            m_blnFailSeal = true;
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                        }
                    }

                }
            }

            // Display Broken Area / Bubble
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x02) > 0)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Seal 1 Broken Area / Bubble";
                dgd_Setting.Rows[i].Cells[2].Value = "mm^2";

                if (m_smVisionInfo.g_objSeal.ref_FailBubble1 == -999)
                {
                    dgd_Setting.Rows[i].Cells[1].Value = "---";
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White; //Lime
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White; //Lime
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[1].Value = m_smVisionInfo.g_objSeal.ref_FailBubble1.ToString("F5");

                    if (m_smVisionInfo.g_objSeal.ref_FailBubble1 > (m_smVisionInfo.g_objSeal.ref_intHoleMinArea1 / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;

                        m_blnFailSeal = true;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                    }
                }

                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Seal 2 Broken Area / Bubble";
                dgd_Setting.Rows[i].Cells[2].Value = "mm^2";

                if (m_smVisionInfo.g_objSeal.ref_FailBubble2 == -999)
                {
                    dgd_Setting.Rows[i].Cells[1].Value = "---";
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White; //Lime
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White; //Lime
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[1].Value = m_smVisionInfo.g_objSeal.ref_FailBubble2.ToString("F5");

                    if (m_smVisionInfo.g_objSeal.ref_FailBubble2 > (m_smVisionInfo.g_objSeal.ref_intHoleMinArea2 / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)))
                    {
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;

                        m_blnFailSeal = true;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                    }
                }
            }

            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x20) > 0)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Seal 1 Broken Gap";
                dgd_Setting.Rows[i].Cells[2].Value = "mm";

                if (m_smVisionInfo.g_objSeal.ref_FailBrokenSeal1 == -999)
                {
                    dgd_Setting.Rows[i].Cells[1].Value = "---";
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White; //Lime
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White; //Lime
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[1].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailBrokenSeal1), 3, MidpointRounding.AwayFromZero);

                    if (m_smVisionInfo.g_objSeal.ref_FailBrokenSeal1 > (m_smVisionInfo.g_objSeal.ref_MinBrokenSealGap / m_smVisionInfo.g_fCalibPixelX))
                    {
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;

                        m_blnFailSeal = true;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                    }
                }

                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Seal 2 Broken Gap";
                dgd_Setting.Rows[i].Cells[2].Value = "mm";

                if (m_smVisionInfo.g_objSeal.ref_FailBrokenSeal2 == -999)
                {
                    dgd_Setting.Rows[i].Cells[1].Value = "---";
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.White; //Lime
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White; //Lime
                    dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[1].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailBrokenSeal2), 3, MidpointRounding.AwayFromZero);

                    if (m_smVisionInfo.g_objSeal.ref_FailBrokenSeal2 > (m_smVisionInfo.g_objSeal.ref_MinBrokenSealGap2 / m_smVisionInfo.g_fCalibPixelX))
                    {
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;

                        m_blnFailSeal = true;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                    }
                }
            }            
        }


        private void txt_WidthUpperTolerance_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewDimension = true;
            m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 0;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_WidthUpperTolerance_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewDimension = false;
            m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = -1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_DistanceMaxTolerance_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewDimension = true;
            m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_DistanceMaxTolerance_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewDimension = false;
            m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = -1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

    



    

   

        private void txt_ScoreTolerance_Enter(object sender, EventArgs e)
        {
            m_blnEnterTextBox = true;
        }

        private void txt_ScoreTolerance_Leave(object sender, EventArgs e)
        {
            m_blnEnterTextBox = false;
        }


        private void txt_MarkScoreTolerance_Enter(object sender, EventArgs e)
        {
            m_blnEnterTextBox = true;
        }

        private void txt_MarkScoreTolerance_Leave(object sender, EventArgs e)
        {
            m_blnEnterTextBox = false;
        }

      

        private void txt_Seal1AreaFilter_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewDimension = true;
            m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 2;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Seal2AreaFilter_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewDimension = true;
            m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 3;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_OverHeatMinArea_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewDimension = true;
            m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 4;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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

        private void txt_MinBrokenArea_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewDimension = true;
            m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 5;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MinBrokenArea_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewDimension = false;
            m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = -1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void chk_CheckEmpty_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.MN_PR_CheckEmptyUnit = chk_CheckEmpty.Checked;
            if (m_smVisionInfo.MN_PR_CheckEmptyUnit)
            {
                dgd_EmptyScore.Visible = true;
                dgd_MarkScore.Visible = false;
            }
            else
            {
                dgd_MarkScore.Visible = true;
                dgd_EmptyScore.Visible = false;
            }
            UpdateRadioBtn();
        }

        private void radioBtn_SelectTemplate_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_smVisionInfo.MN_PR_CheckEmptyUnit)
            {
                if (radioBtn_Template1.Checked)
                    m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = 0;
                else if (radioBtn_Template2.Checked)
                    m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex  = 1;
                else if (radioBtn_Template3.Checked)
                    m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex  = 2;
                else if (radioBtn_Template4.Checked)
                    m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = 3;
            }
            else
            {
                if (radioBtn_Template1.Checked)
                    m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = 0;
                else if (radioBtn_Template2.Checked)
                    m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex =  1;
                else if (radioBtn_Template3.Checked)
                    m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex =  2;
                else if (radioBtn_Template4.Checked)
                    m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex =  3;

            }

            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("Vision6OfflinePage Call UpdateInfo C");
            }

            UpdateInfo();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void UpdateRadioBtn()
        {
            if (m_smVisionInfo.MN_PR_CheckEmptyUnit)
            {
                if ((m_smVisionInfo.g_intPocketTemplateMask & 0x01) > 0)
                    radioBtn_Template1.Enabled = true;
                else
                    radioBtn_Template1.Enabled = false;
                if ((m_smVisionInfo.g_intPocketTemplateMask & 0x02) > 0)
                    radioBtn_Template2.Enabled = true;
                else
                    radioBtn_Template2.Enabled = false;
                if ((m_smVisionInfo.g_intPocketTemplateMask & 0x04) > 0)
                    radioBtn_Template3.Enabled = true;
                else
                    radioBtn_Template3.Enabled = false;
                if ((m_smVisionInfo.g_intPocketTemplateMask & 0x08) > 0)
                    radioBtn_Template4.Enabled = true;
                else
                    radioBtn_Template4.Enabled = false;

                switch (m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex)
                {
                    case 0:
                        if (!radioBtn_Template1.Enabled)
                        {
                            if (radioBtn_Template2.Enabled)
                            {
                                radioBtn_Template2.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = 1;
                            }
                            else if (radioBtn_Template3.Enabled)
                            {
                                radioBtn_Template3.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = 2;
                            }
                            else if (radioBtn_Template4.Enabled)
                            {
                                radioBtn_Template4.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = 3;
                            }
                        }
                        else
                            radioBtn_Template1.Checked = true;
                        break;
                    case 1:
                        if (!radioBtn_Template2.Enabled)
                        {
                            if (radioBtn_Template1.Enabled)
                            {
                                radioBtn_Template1.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = 0;
                            }
                            else if (radioBtn_Template3.Enabled)
                            {
                                radioBtn_Template3.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = 2;
                            }
                            else if (radioBtn_Template4.Enabled)
                            {
                                radioBtn_Template4.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = 3;
                            }
                        }
                        else
                            radioBtn_Template2.Checked = true;
                        break;
                    case 2:
                        if (!radioBtn_Template3.Enabled)
                        {
                            if (radioBtn_Template1.Enabled)
                            {
                                radioBtn_Template1.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = 0;
                            }
                            else if (radioBtn_Template2.Enabled)
                            {
                                radioBtn_Template2.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = 1;
                            }
                            else if (radioBtn_Template4.Enabled)
                            {
                                radioBtn_Template4.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = 3;
                            }
                        }
                        else
                            radioBtn_Template3.Checked = true;
                        break;
                    case 3:
                        if (!radioBtn_Template4.Enabled)
                        {
                            if (radioBtn_Template1.Enabled)
                            {
                                radioBtn_Template1.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = 0;
                            }
                            else if (radioBtn_Template2.Enabled)
                            {
                                radioBtn_Template2.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = 1;
                            }
                            else if (radioBtn_Template3.Enabled)
                            {
                                radioBtn_Template3.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = 2;
                            }
                        }
                        else
                            radioBtn_Template4.Checked = true;
                        break;
                }
            }
            else
            {
                if ((m_smVisionInfo.g_intMarkTemplateMask & 0x01) > 0)
                    radioBtn_Template1.Enabled = true;
                else
                    radioBtn_Template1.Enabled = false;
                if ((m_smVisionInfo.g_intMarkTemplateMask & 0x02) > 0)
                    radioBtn_Template2.Enabled = true;
                else
                    radioBtn_Template2.Enabled = false;
                if ((m_smVisionInfo.g_intMarkTemplateMask & 0x04) > 0)
                    radioBtn_Template3.Enabled = true;
                else
                    radioBtn_Template3.Enabled = false;
                if ((m_smVisionInfo.g_intMarkTemplateMask & 0x08) > 0)
                    radioBtn_Template4.Enabled = true;
                else
                    radioBtn_Template4.Enabled = false;

                switch (m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex)
                {
                    case 0:
                        if (!radioBtn_Template1.Enabled)
                        {
                            if (radioBtn_Template2.Enabled)
                            {
                                radioBtn_Template2.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = 1;
                            }
                            else if (radioBtn_Template3.Enabled)
                            {
                                radioBtn_Template3.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = 2;
                            }
                            else if (radioBtn_Template4.Enabled)
                            {
                                radioBtn_Template4.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = 3;
                            }
                        }
                        else
                            radioBtn_Template1.Checked = true;
                        break;
                    case 1:
                        if (!radioBtn_Template2.Enabled)
                        {
                            if (radioBtn_Template1.Enabled)
                            {
                                radioBtn_Template1.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = 0;
                            }
                            else if (radioBtn_Template3.Enabled)
                            {
                                radioBtn_Template3.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = 2;
                            }
                            else if (radioBtn_Template4.Enabled)
                            {
                                radioBtn_Template4.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = 3;
                            }
                        }
                        else
                            radioBtn_Template2.Checked = true;
                        break;
                    case 2:
                        if (!radioBtn_Template3.Enabled)
                        {
                            if (radioBtn_Template1.Enabled)
                            {
                                radioBtn_Template1.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = 0;
                            }
                            else if (radioBtn_Template2.Enabled)
                            {
                                radioBtn_Template2.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = 1;
                            }
                            else if (radioBtn_Template4.Enabled)
                            {
                                radioBtn_Template4.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = 3;
                            }
                        }
                        else
                            radioBtn_Template3.Checked = true;
                        break;
                    case 3:
                        if (!radioBtn_Template4.Enabled)
                        {
                            if (radioBtn_Template1.Enabled)
                            {
                                radioBtn_Template1.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = 0;
                            }
                            else if (radioBtn_Template2.Enabled)
                            {
                                radioBtn_Template2.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = 1;
                            }
                            else if (radioBtn_Template3.Enabled)
                            {
                                radioBtn_Template3.Checked = true;
                                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = 2;
                            }
                        }
                        else
                            radioBtn_Template4.Checked = true;
                        break;
                }
            }
        }

        public void LoadEvent()
        {  // Cursor.Current = Cursors.Default;
            m_smVisionInfo.VM_AT_OfflinePageView = true;
            m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            // 2020 07 22 - No suppose to load seal data in offline page bcos the setting wont change in offline page.
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //  m_smVisionInfo.g_strVisionFolderName + "\\";
            //LoadGeneralSetting(strPath);


            //// 2019 01 11 - CCENG: Don't put this function in Load Form Event function because the Form_Load event will cannot be triggered when you hide and show it again.
            //Cursor.Current = Cursors.Default;
            //m_smVisionInfo.g_blnViewSealInspection = true;
            //m_smVisionInfo.VM_AT_OfflinePageView = true;
            //m_smVisionInfo.VM_AT_SettingInDialog = true;
        }
        public void OnOffTimer(bool blnOn)
        {
            timer_TestResult.Enabled = blnOn;
        }

        public bool GetTimerStatus()
        {
            return timer_TestResult.Enabled;
        }

        private void UpdateTabPageHeaderImage()
        {
            if (m_blnFailSeal)
                tabPage_Seal.ImageIndex = 1;
            else
                tabPage_Seal.ImageIndex = 0;

            if (chk_CheckEmpty.Checked)
            {
                if (m_blnFailEmpty)
                    tp_Score.ImageIndex = 1;
                else
                    tp_Score.ImageIndex = 0;
            }
            else
            {
                if (m_blnFailMark)
                    tp_Score.ImageIndex = 1;
                else
                    tp_Score.ImageIndex = 0;
            }
           
        }

        private void lbl_SetReferenceChars_Click(object sender, EventArgs e)
        {

        }
    }
}
