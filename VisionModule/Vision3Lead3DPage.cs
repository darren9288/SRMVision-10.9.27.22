using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SharedMemory;
using VisionProcessing;
using Common;

namespace VisionModule
{
    public partial class Vision3Lead3DPage : Form
    {
        #region Member Variables

        private int m_intMachineStatusPrev = 1;
        private int m_intUserGroup;
        // Count
        private int m_intPassCount = 0, m_intPassCountPrev = -1;
        private int m_intFailCount = 0, m_intFailCountPrev = -1;
        private int m_intTestedTotal = 0, m_intTestedTotalPrev = -1;
        private int m_intLeadOffsetFailCount = 0, m_intLeadOffsetFailCountPrev = -1;
        private int m_intLeadSkewFailCount = 0, m_intLeadSkewFailCountPrev = -1;
        private int m_intLeadWidthFailCount = 0, m_intLeadWidthFailCountPrev = -1;
        private int m_intLeadLengthFailCount = 0, m_intLeadLengthFailCountPrev = -1;
        private int m_intLeadLengthVarianceFailCount = 0, m_intLeadLengthVarianceFailCountPrev = -1;
        private int m_intLeadPitchFailCount = 0, m_intLeadPitchFailCountPrev = -1;
        private int m_intLeadPitchVarianceFailCount = 0, m_intLeadPitchVarianceFailCountPrev = -1;
        private int m_intLeadStandOffFailCount = 0, m_intLeadStandOffFailCountPrev = -1;
        private int m_intLeadStandOffVarianceFailCount = 0, m_intLeadStandOffVarianceFailCountPrev = -1;
        private int m_intLeadCoplanFailCount = 0, m_intLeadCoplanFailCountPrev = -1;
        private int m_intLeadAGVFailCount = 0, m_intLeadAGVFailCountPrev = -1;
        private int m_intLeadSpanFailCount = 0, m_intLeadSpanFailCountPrev = -1;
        private int m_intLeadSweepsFailCount = 0, m_intLeadSweepsFailCountPrev = -1;
        private int m_intLeadUnCutTiebarFailCount = 0, m_intLeadUnCutTiebarFailCountPrev = -1;
        private int m_intLeadMissingFailCount = 0, m_intLeadMissingFailCountPrev = -1;
        private int m_intLeadContaminationFailCount = 0, m_intLeadContaminationFailCountPrev = -1;

        private int m_intLeadPkgDefectFailCount = 0, m_intLeadPkgDefectFailCountPrev = -1;
        private int m_intLeadPkgDimensionFailCount = 0, m_intLeadPkgDimensionFailCountPrev = -1;

        private int m_intLeadFailCount = 0, m_intLeadFailCountPrev = -1;
        private int m_intPositionFailCount = 0, m_intPositionFailCountPrev = -1;
        private int m_intEmptyUnitFailCount = 0, m_intEmptyUnitFailCountPrev = -1;
        private int m_intNoTemplateFailCount = 0, m_intNoTemplateFailCountPrev = -1;
        private int m_intEdgeNotFoundFailCount = 0, m_intEdgeNotFoundFailCountPrev = -1;
        private int m_intPin1FailCount = 0, m_intPin1FailCountPrev = -1;
        private string m_strResult = "", m_strResultPrev = "";

        CustomOption m_smCustomOption;
        ProductionInfo m_smProductionInfo;
        VisionInfo m_smVisionInfo;
        private UserRight m_objUserRight = new UserRight();
        #endregion



        public Vision3Lead3DPage(CustomOption smCustomOption, ProductionInfo smProductionInfo, VisionInfo smVisionInfo)
        {
            InitializeComponent();

            m_smCustomOption = smCustomOption;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            m_intUserGroup = m_smProductionInfo.g_intUserGroup;
            //lbl_LotID.Text = m_smProductionInfo.g_strLotID;
            lbl_LotID.Text = m_smProductionInfo.g_arrSingleLotID[m_smVisionInfo.g_intVisionIndex];
            lbl_OperatorID.Text = m_smProductionInfo.g_strOperatorID;
            UpdateInfo();
            ResetCounterBackColor();
            CustomizeGUI();
            LoadTemplateImage();
            DisableField();
          
            if (m_smVisionInfo.g_intForceYZero == 0)
                chk_ForceYtozero.Checked = false;
            else
                chk_ForceYtozero.Checked = true;
        }
        public void DisableField()
        {
            string strChild1 = "Learn Page";
            string strChild2 = "Reset Counter Button";
            if (m_smProductionInfo.g_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Reset.Enabled = false;
            }
            else
            {
                btn_Reset.Enabled = true;
            }
        }

        private void ResetCounterBackColor()
        {
            lbl_LeadFailCount.BackColor = Color.White;
            lbl_PositionFailCount.BackColor = Color.White;
            lbl_EmptyUnitFailCount.BackColor = Color.White;
            lbl_NoTemplateFailCount.BackColor = Color.White;
            lbl_EdgeNotFoundFailCount.BackColor = Color.White;
            lbl_Pin1FailCount.BackColor = Color.White;
            lbl_LeadOffsetFailCount.BackColor = Color.White;
            lbl_LeadSkewFailCount.BackColor = Color.White;
            lbl_LeadWidthFailCount.BackColor = Color.White;
            lbl_LeadLengthFailCount.BackColor = Color.White;
            lbl_LeadLengthVarianceFailCount.BackColor = Color.White;
            lbl_LeadPitchFailCount.BackColor = Color.White;
            lbl_LeadPitchGapVarianceFailCount.BackColor = Color.White;
            lbl_LeadStandOffFailCount.BackColor = Color.White;
            lbl_LeadStandOffVarianceFailCount.BackColor = Color.White;
            lbl_LeadCoplanFailCount.BackColor = Color.White;
            lbl_LeadAGVFailCount.BackColor = Color.White;
            lbl_LeadSpanFailCount.BackColor = Color.White;
            lbl_LeadSweepsFailCount.BackColor = Color.White;
            lbl_LeadUnCutTiebarFailCount.BackColor = Color.White;
            lbl_LeadMissingFailCount.BackColor = Color.White;
            lbl_LeadContaminationFailCount.BackColor = Color.White;
            lbl_LeadPkgDefectFailCount.BackColor = Color.White;
            lbl_LeadPkgDimensionFailCount.BackColor = Color.White;

            lbl_LeadFailPercent.BackColor = Color.White;
            lbl_PositionFailPercent.BackColor = Color.White;
            lbl_EmptyUnitFailPercent.BackColor = Color.White;
            lbl_NoTemplateFailPercent.BackColor = Color.White;
            lbl_EdgeNotFoundFailPercent.BackColor = Color.White;
            lbl_Pin1FailPercent.BackColor = Color.White;
            lbl_LeadOffsetFailPercent.BackColor = Color.White;
            lbl_LeadSkewFailPercent.BackColor = Color.White;
            lbl_LeadWidthFailPercent.BackColor = Color.White;
            lbl_LeadLengthFailPercent.BackColor = Color.White;
            lbl_LeadLengthVarianceFailPercent.BackColor = Color.White;
            lbl_LeadPitchFailPercent.BackColor = Color.White;
            lbl_LeadPitchGapVarianceFailPercent.BackColor = Color.White;
            lbl_LeadStandOffFailPercent.BackColor = Color.White;
            lbl_LeadStandOffVarianceFailPercent.BackColor = Color.White;
            lbl_LeadCoplanFailPercent.BackColor = Color.White;
            lbl_LeadAGVFailPercent.BackColor = Color.White;
            lbl_LeadSpanFailPercent.BackColor = Color.White;
            lbl_LeadSweepsFailPercent.BackColor = Color.White;
            lbl_LeadUnCutTiebarFailPercent.BackColor = Color.White;
            lbl_LeadMissingFailPercent.BackColor = Color.White;
            lbl_LeadContaminationFailPercent.BackColor = Color.White;
            lbl_LeadPkgDefectFailPercent.BackColor = Color.White;
            lbl_LeadPkgDimensionFailPercent.BackColor = Color.White;
        }
        /// <summary>
        /// Make sure that only when form is focused, then enable timer.
        /// By this way, CPU usage can be reduced.
        /// </summary>
        public void ActivateTimer(bool blnEnable)
        {
            timer_Live.Enabled = blnEnable;
        }

        public void CustomizeGUI()
        {
            if ((m_smCustomOption.g_intWantLead3D & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                pnl_LeadSkew.Visible = false;
                pnl_LeadOffset.Visible = false;
                pnl_LeadWidth.Visible = false;
                pnl_LeadLength.Visible = false;
                pnl_LeadLengthVariance.Visible = false;
                pnl_LeadPitch.Visible = false;
                pnl_LeadPitchGapVariance.Visible = false;
                pnl_LeadStandOff.Visible = false;
                pnl_LeadStandOffVariance.Visible = false;
                pnl_LeadCoplan.Visible = false;
                pnl_LeadAGV.Visible = false;
                pnl_LeadSpan.Visible = false;
                pnl_LeadSweeps.Visible = false;
                pnl_LeadUnCutTiebar.Visible = false;
                pnl_LeadMissing.Visible = false;
                pnl_LeadContamination.Visible = false;
                pnl_Lead.Visible = false;
            }
            else
            {
                int intFailMask = m_smVisionInfo.g_arrLead3D[0].ref_intFailOptionMask;

                //Offset
                if ((intFailMask & 0x20000) > 0)
                {
                    pnl_LeadOffset.Visible = true;
                }
                else
                {
                    if (m_intLeadOffsetFailCount == 0)
                    {
                        pnl_LeadOffset.Visible = false;
                    }
                }

                //Skew
                if ((intFailMask & 0x100) > 0)
                {
                    pnl_LeadSkew.Visible = true;
                }
                else
                {
                    if (m_intLeadSkewFailCount == 0)
                    {
                        pnl_LeadSkew.Visible = false;
                    }
                }

                //Width
                if ((intFailMask & 0x40) > 0)
                {
                    pnl_LeadWidth.Visible = true;
                }
                else
                {
                    if (m_intLeadWidthFailCount == 0)
                    {
                        pnl_LeadWidth.Visible = false;
                    }
                }
                
                //Length
                if ((intFailMask & 0x80) > 0)
                {
                    pnl_LeadLength.Visible = true;
                }
                else
                {
                    if (m_intLeadLengthFailCount == 0)
                    {
                        pnl_LeadLength.Visible = false;
                    }
                }

                //Length Variance
                if ((intFailMask & 0x800) > 0)
                {
                    pnl_LeadLengthVariance.Visible = true;
                }
                else
                {
                    if (m_intLeadLengthVarianceFailCount == 0)
                    {
                        pnl_LeadLengthVariance.Visible = false;
                    }
                }

                //Pitch
                if ((intFailMask & 0x200) > 0)
                {
                    pnl_LeadPitch.Visible = true;
                }
                else
                {
                    if (m_intLeadPitchFailCount == 0)
                    {
                        pnl_LeadPitch.Visible = false;
                    }
                }

                //Pitch Variance
                if ((intFailMask & 0x2000) > 0)
                {
                    pnl_LeadPitchGapVariance.Visible = true;
                }
                else
                {
                    if (m_intLeadPitchVarianceFailCount == 0)
                    {
                        pnl_LeadPitchGapVariance.Visible = false;
                    }
                }

                //Stand Off
                if ((intFailMask & 0x01) > 0)
                {
                    pnl_LeadStandOff.Visible = true;
                }
                else
                {
                    if (m_intLeadStandOffFailCount == 0)
                    {
                        pnl_LeadStandOff.Visible = false;
                    }
                }

                //Stand Off Variance
                if ((intFailMask & 0x4000) > 0)
                {
                    pnl_LeadStandOffVariance.Visible = true;
                }
                else
                {
                    if (m_intLeadStandOffVarianceFailCount == 0)
                    {
                        pnl_LeadStandOffVariance.Visible = false;
                    }
                }

                //Coplan
                if ((intFailMask & 0x02) > 0)
                {
                    pnl_LeadCoplan.Visible = true;
                }
                else
                {
                    if (m_intLeadCoplanFailCount == 0)
                    {
                        pnl_LeadCoplan.Visible = false;
                    }
                }

                //Average Gray Value
                if ((intFailMask & 0x40000) > 0)
                {
                    pnl_LeadAGV.Visible = true;
                }
                else
                {
                    if (m_intLeadAGVFailCount == 0)
                    {
                        pnl_LeadAGV.Visible = false;
                    }
                }

                //Span
                if ((intFailMask & 0x1000) > 0)
                {
                    pnl_LeadSpan.Visible = true;
                }
                else
                {
                    if (m_intLeadSpanFailCount == 0)
                    {
                        pnl_LeadSpan.Visible = false;
                    }
                }

                //Sweeps
                if ((intFailMask & 0x04) > 0)
                {
                    pnl_LeadSweeps.Visible = true;
                }
                else
                {
                    if (m_intLeadSweepsFailCount == 0)
                    {
                        pnl_LeadSweeps.Visible = false;
                    }
                }

                //Un-Cut Tiebar
                if ((intFailMask & 0x600) > 0)
                {
                    pnl_LeadUnCutTiebar.Visible = true;
                }
                else
                {
                    if (m_intLeadUnCutTiebarFailCount == 0)
                    {
                        pnl_LeadUnCutTiebar.Visible = false;
                    }
                }

                //Missing
                //if (m_intLeadMissingFailCount != 0)
                //{
                    pnl_LeadMissing.Visible = true;
                //}
                //else
                //{
                //    pnl_LeadMissing.Visible = false;
                //}

                //Contamination
                if (((intFailMask & 0x8000) > 0) || ((intFailMask & 0x10000) > 0))
                {
                    pnl_LeadContamination.Visible = true;
                }
                else
                {
                    if (m_intLeadContaminationFailCount == 0)
                    {
                        pnl_LeadContamination.Visible = false;
                    }
                }
            }

            if ((m_smCustomOption.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                pnl_LeadPkgDefect.Visible = false;
                pnl_LeadPkgDimension.Visible = false;
            }
            else
            {
                //Remove contamination if package is on
                //pnl_LeadContamination.Visible = false;

                if (m_smVisionInfo.g_arrLead3D[0].GetWantInspectPackage())
                {
                    pnl_LeadPkgDefect.Visible = true;
                }
                else
                {
                    if (m_intLeadPkgDefectFailCount == 0)
                    {
                        pnl_LeadPkgDefect.Visible = false;
                    }
                }

                if ((m_smVisionInfo.g_arrLead3D[0].ref_intFailPkgOptionMask & 0x01) > 0)
                {
                    pnl_LeadPkgDimension.Visible = true;
                }
                else
                {
                    if (m_intLeadPkgDimensionFailCount == 0)
                    {
                        pnl_LeadPkgDimension.Visible = false;
                    }
                }

                //if (m_smVisionInfo.g_blnWantGauge)
                //{
                    pnl_EdgeNotFound.Visible = true;
                //}
                //else
                //{
                //    //only remove when fail count is 0, if fail count not 0 will remain
                //    if (m_intEdgeNotFoundFailCount == 0)
                //    {
                //        pnl_EdgeNotFound.Visible = false;
                //    }
                //}
            }

            if (m_smVisionInfo.g_blnWantPin1)
            {
                pnl_Pin1.Visible = true;
            }
            else
            {
                //only remove when fail count is 0, if fail count not 0 will remain
                if (m_intPin1FailCount == 0)
                {
                    pnl_Pin1.Visible = false;
                }
            }
            
            if (m_intNoTemplateFailCount != 0)
            {
                pnl_NoTemplate.Visible = true;
            }
            else
            {
                pnl_NoTemplate.Visible = false;
            }

            if (m_intPositionFailCount != 0)
            {
                pnl_Position.Visible = true;
            }
            else
            {
                pnl_Position.Visible = false;
            }

            //if (m_intEmptyUnitFailCount != 0)
            //{
            //    pnl_EmptyUnit.Visible = true;
            //}
            //else
            //{
                pnl_EmptyUnit.Visible = false;
            //}

            if ((m_smCustomOption.g_intWantPositioning & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                //lbl_Position.Visible = false;
                //lbl_PositionFailCount.Visible = false;
               
                chk_ForceYtozero.Visible = false;
            }
        }

        public void SetMultiViewGUI(bool blnSet)
        {
            if (blnSet)
            {
                this.Size = new Size(380, 285);
                //pnl_Detail.Location = new Point(181, 128);
                this.AutoScroll = true;

            }
            else
            {
                this.Size = new Size(319, 569);
                //pnl_Detail.Location = new Point(5, 231);
                this.AutoScroll = false;
            }
        }

        private void EnableButton(bool blnEnable)
        {
            btn_Reset.Enabled = blnEnable;
        }

        public void LoadTemplateImage()
        {
            return;

            if ((m_smCustomOption.g_intWantLead3D & (0x01 << m_smVisionInfo.g_intVisionPos)) == 0)
                return;

            string strRecipe = m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex];
            string strFilePath = m_smProductionInfo.g_strRecipePath + strRecipe +
                            "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\Template\\OrTemplate0.bmp";

            if (File.Exists(strFilePath))
            {
                pic_Template.Visible = true;
                pic_Template.Load(strFilePath);
            }
            else
                pic_Template.Visible = false;
        }

        private void ResetCount()
        {
            lbl_PassCount.Text = "0";
            lbl_TestedTotal.Text = "0";
            lbl_FailCount.Text = "0";

            lbl_LeadFailCount.Text = "0";
            lbl_PositionFailCount.Text = "0";
            lbl_EmptyUnitFailCount.Text = "0";
            lbl_NoTemplateFailCount.Text = "0";
            lbl_EdgeNotFoundFailCount.Text = "0";
            lbl_Pin1FailCount.Text = "0";

            lbl_LeadOffsetFailCount.Text = "0";
            lbl_LeadSkewFailCount.Text = "0";
            lbl_LeadWidthFailCount.Text = "0";
            lbl_LeadLengthFailCount.Text = "0";
            lbl_LeadLengthVarianceFailCount.Text = "0";
            lbl_LeadPitchFailCount.Text = "0";
            lbl_LeadPitchGapVarianceFailCount.Text = "0";
            lbl_LeadStandOffFailCount.Text = "0";
            lbl_LeadStandOffVarianceFailCount.Text = "0";
            lbl_LeadCoplanFailCount.Text = "0";
            lbl_LeadAGVFailCount.Text = "0";
            lbl_LeadSpanFailCount.Text = "0";
            lbl_LeadSweepsFailCount.Text = "0";
            lbl_LeadUnCutTiebarFailCount.Text = "0";
            lbl_LeadMissingFailCount.Text = "0";
            lbl_LeadContaminationFailCount.Text = "0";

            lbl_LeadPkgDefectFailCount.Text = "0";
            lbl_LeadPkgDimensionFailCount.Text = "0";

            lbl_LeadFailPercent.Text = "0.00";
            lbl_PositionFailPercent.Text = "0.00";
            lbl_EmptyUnitFailPercent.Text = "0.00";
            lbl_NoTemplateFailPercent.Text = "0.00";
            lbl_EdgeNotFoundFailPercent.Text = "0.00";
            lbl_Pin1FailPercent.Text = "0.00";

            lbl_LeadOffsetFailPercent.Text = "0.00";
            lbl_LeadSkewFailPercent.Text = "0.00";
            lbl_LeadWidthFailPercent.Text = "0.00";
            lbl_LeadLengthFailPercent.Text = "0.00";
            lbl_LeadLengthVarianceFailPercent.Text = "0.00";
            lbl_LeadPitchFailPercent.Text = "0.00";
            lbl_LeadPitchGapVarianceFailPercent.Text = "0.00";
            lbl_LeadStandOffFailPercent.Text = "0.00";
            lbl_LeadStandOffVarianceFailPercent.Text = "0.00";
            lbl_LeadCoplanFailPercent.Text = "0.00";
            lbl_LeadAGVFailPercent.Text = "0.00";
            lbl_LeadSpanFailPercent.Text = "0.00";
            lbl_LeadSweepsFailPercent.Text = "0.00";
            lbl_LeadUnCutTiebarFailPercent.Text = "0.00";
            lbl_LeadMissingFailPercent.Text = "0.00";
            lbl_LeadContaminationFailPercent.Text = "0.00";

            lbl_LeadPkgDefectFailPercent.Text = "0.00";
            lbl_LeadPkgDimensionFailPercent.Text = "0.00";

            lbl_Yield.Text = "0.00";
            lbl_Yield.BackColor = Color.White;
        }

        private void UpdateInfo()
        {
            m_strResult = m_smVisionInfo.g_strResult;
            if (m_strResult != m_strResultPrev)
            {
                switch (m_strResult)
                {
                    case "Pass":
                        lbl_ResultStatus.BackColor = Color.Lime;
                        lbl_ResultStatus.Text = "Pass";
                        break;
                    case "Fail":
                        lbl_ResultStatus.BackColor = Color.Red;
                        lbl_ResultStatus.Text = "Fail";
                        break;
                }

                m_strResultPrev = m_strResult;
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

            m_intLeadFailCount = m_smVisionInfo.g_intLeadFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fPadFailPercent = (m_intLeadFailCount / (float)m_intTestedTotal) * 100;
                if (fPadFailPercent > 100)
                    fPadFailPercent = 100;
                lbl_LeadFailPercent.Text = fPadFailPercent.ToString("f2");
            }
            else
                lbl_LeadFailPercent.Text = "0.00";

            if (m_intLeadFailCount != m_intLeadFailCountPrev)
            {
                lbl_LeadFailCount.BackColor = Color.Red;
                lbl_LeadFailPercent.BackColor = Color.Red;
                lbl_LeadFailCount.Text = m_intLeadFailCount.ToString();
                m_intLeadFailCountPrev = m_intLeadFailCount;
            }
            else
            {
                lbl_LeadFailCount.BackColor = Color.White;
                lbl_LeadFailPercent.BackColor = Color.White;
            }

            m_intLeadOffsetFailCount = m_smVisionInfo.g_intLeadOffsetFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadOffsetFailPercent = (m_intLeadOffsetFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadOffsetFailPercent > 100)
                    fLeadOffsetFailPercent = 100;
                lbl_LeadOffsetFailPercent.Text = fLeadOffsetFailPercent.ToString("f2");
            }
            else
                lbl_LeadOffsetFailPercent.Text = "0.00";

            if (m_intLeadOffsetFailCount != m_intLeadOffsetFailCountPrev)
            {
                lbl_LeadOffsetFailCount.BackColor = Color.Red;
                lbl_LeadOffsetFailPercent.BackColor = Color.Red;
                lbl_LeadOffsetFailCount.Text = m_intLeadOffsetFailCount.ToString();
                m_intLeadOffsetFailCountPrev = m_intLeadOffsetFailCount;
            }
            else
            {
                lbl_LeadOffsetFailCount.BackColor = Color.White;
                lbl_LeadOffsetFailPercent.BackColor = Color.White;
            }

            m_intLeadSkewFailCount = m_smVisionInfo.g_intLeadSkewFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadSkewFailPercent = (m_intLeadSkewFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadSkewFailPercent > 100)
                    fLeadSkewFailPercent = 100;
                lbl_LeadSkewFailPercent.Text = fLeadSkewFailPercent.ToString("f2");
            }
            else
                lbl_LeadSkewFailPercent.Text = "0.00";

            if (m_intLeadSkewFailCount != m_intLeadSkewFailCountPrev)
            {
                lbl_LeadSkewFailCount.BackColor = Color.Red;
                lbl_LeadSkewFailPercent.BackColor = Color.Red;
                lbl_LeadSkewFailCount.Text = m_intLeadSkewFailCount.ToString();
                m_intLeadSkewFailCountPrev = m_intLeadSkewFailCount;
            }
            else
            {
                lbl_LeadSkewFailCount.BackColor = Color.White;
                lbl_LeadSkewFailPercent.BackColor = Color.White;
            }

            m_intLeadWidthFailCount = m_smVisionInfo.g_intLeadWidthFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadWidthFailPercent = (m_intLeadWidthFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadWidthFailPercent > 100)
                    fLeadWidthFailPercent = 100;
                lbl_LeadWidthFailPercent.Text = fLeadWidthFailPercent.ToString("f2");
            }
            else
                lbl_LeadWidthFailPercent.Text = "0.00";

            if (m_intLeadWidthFailCount != m_intLeadWidthFailCountPrev)
            {
                lbl_LeadWidthFailCount.BackColor = Color.Red;
                lbl_LeadWidthFailPercent.BackColor = Color.Red;
                lbl_LeadWidthFailCount.Text = m_intLeadWidthFailCount.ToString();
                m_intLeadWidthFailCountPrev = m_intLeadWidthFailCount;
            }
            else
            {
                lbl_LeadWidthFailCount.BackColor = Color.White;
                lbl_LeadWidthFailPercent.BackColor = Color.White;
            }

            m_intLeadLengthFailCount = m_smVisionInfo.g_intLeadLengthFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadLengthFailPercent = (m_intLeadLengthFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadLengthFailPercent > 100)
                    fLeadLengthFailPercent = 100;
                lbl_LeadLengthFailPercent.Text = fLeadLengthFailPercent.ToString("f2");
            }
            else
                lbl_LeadLengthFailPercent.Text = "0.00";

            if (m_intLeadLengthFailCount != m_intLeadLengthFailCountPrev)
            {
                lbl_LeadLengthFailCount.BackColor = Color.Red;
                lbl_LeadLengthFailPercent.BackColor = Color.Red;
                lbl_LeadLengthFailCount.Text = m_intLeadLengthFailCount.ToString();
                m_intLeadLengthFailCountPrev = m_intLeadLengthFailCount;
            }
            else
            {
                lbl_LeadLengthFailCount.BackColor = Color.White;
                lbl_LeadLengthFailPercent.BackColor = Color.White;
            }

            m_intLeadLengthVarianceFailCount = m_smVisionInfo.g_intLeadLengthVarianceFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadLengthVarianceFailPercent = (m_intLeadLengthVarianceFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadLengthVarianceFailPercent > 100)
                    fLeadLengthVarianceFailPercent = 100;
                lbl_LeadLengthVarianceFailPercent.Text = fLeadLengthVarianceFailPercent.ToString("f2");
            }
            else
                lbl_LeadLengthVarianceFailPercent.Text = "0.00";

            if (m_intLeadLengthVarianceFailCount != m_intLeadLengthVarianceFailCountPrev)
            {
                lbl_LeadLengthVarianceFailCount.BackColor = Color.Red;
                lbl_LeadLengthVarianceFailPercent.BackColor = Color.Red;
                lbl_LeadLengthVarianceFailCount.Text = m_intLeadLengthVarianceFailCount.ToString();
                m_intLeadLengthVarianceFailCountPrev = m_intLeadLengthVarianceFailCount;
            }
            else
            {
                lbl_LeadLengthVarianceFailCount.BackColor = Color.White;
                lbl_LeadLengthVarianceFailPercent.BackColor = Color.White;
            }

            m_intLeadPitchFailCount = m_smVisionInfo.g_intLeadPitchGapFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadPitchFailPercent = (m_intLeadPitchFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadPitchFailPercent > 100)
                    fLeadPitchFailPercent = 100;
                lbl_LeadPitchFailPercent.Text = fLeadPitchFailPercent.ToString("f2");
            }
            else
                lbl_LeadPitchFailPercent.Text = "0.00";

            if (m_intLeadPitchFailCount != m_intLeadPitchFailCountPrev)
            {
                lbl_LeadPitchFailCount.BackColor = Color.Red;
                lbl_LeadPitchFailPercent.BackColor = Color.Red;
                lbl_LeadPitchFailCount.Text = m_intLeadPitchFailCount.ToString();
                m_intLeadPitchFailCountPrev = m_intLeadPitchFailCount;
            }
            else
            {
                lbl_LeadPitchFailCount.BackColor = Color.White;
                lbl_LeadPitchFailPercent.BackColor = Color.White;
            }

            m_intLeadPitchVarianceFailCount = m_smVisionInfo.g_intLeadPitchVarianceFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadPitchVarianceFailPercent = (m_intLeadPitchVarianceFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadPitchVarianceFailPercent > 100)
                    fLeadPitchVarianceFailPercent = 100;
                lbl_LeadPitchGapVarianceFailPercent.Text = fLeadPitchVarianceFailPercent.ToString("f2");
            }
            else
                lbl_LeadPitchGapVarianceFailPercent.Text = "0.00";

            if (m_intLeadPitchVarianceFailCount != m_intLeadPitchVarianceFailCountPrev)
            {
                lbl_LeadPitchGapVarianceFailCount.BackColor = Color.Red;
                lbl_LeadPitchGapVarianceFailPercent.BackColor = Color.Red;
                lbl_LeadPitchGapVarianceFailCount.Text = m_intLeadPitchVarianceFailCount.ToString();
                m_intLeadPitchVarianceFailCountPrev = m_intLeadPitchVarianceFailCount;
            }
            else
            {
                lbl_LeadPitchGapVarianceFailCount.BackColor = Color.White;
                lbl_LeadPitchGapVarianceFailPercent.BackColor = Color.White;
            }

            m_intLeadStandOffFailCount = m_smVisionInfo.g_intLeadStandOffFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadStandOffFailPercent = (m_intLeadStandOffFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadStandOffFailPercent > 100)
                    fLeadStandOffFailPercent = 100;
                lbl_LeadStandOffFailPercent.Text = fLeadStandOffFailPercent.ToString("f2");
            }
            else
                lbl_LeadStandOffFailPercent.Text = "0.00";

            if (m_intLeadStandOffFailCount != m_intLeadStandOffFailCountPrev)
            {
                lbl_LeadStandOffFailCount.BackColor = Color.Red;
                lbl_LeadStandOffFailPercent.BackColor = Color.Red;
                lbl_LeadStandOffFailCount.Text = m_intLeadStandOffFailCount.ToString();
                m_intLeadStandOffFailCountPrev = m_intLeadStandOffFailCount;
            }
            else
            {
                lbl_LeadStandOffFailCount.BackColor = Color.White;
                lbl_LeadStandOffFailPercent.BackColor = Color.White;
            }

            m_intLeadStandOffVarianceFailCount = m_smVisionInfo.g_intLeadStandOffVarianceFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadStandOffVarianceFailPercent = (m_intLeadStandOffVarianceFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadStandOffVarianceFailPercent > 100)
                    fLeadStandOffVarianceFailPercent = 100;
                lbl_LeadStandOffVarianceFailPercent.Text = fLeadStandOffVarianceFailPercent.ToString("f2");
            }
            else
                lbl_LeadStandOffVarianceFailPercent.Text = "0.00";

            if (m_intLeadStandOffVarianceFailCount != m_intLeadStandOffVarianceFailCountPrev)
            {
                lbl_LeadStandOffVarianceFailCount.BackColor = Color.Red;
                lbl_LeadStandOffVarianceFailPercent.BackColor = Color.Red;
                lbl_LeadStandOffVarianceFailCount.Text = m_intLeadStandOffVarianceFailCount.ToString();
                m_intLeadStandOffVarianceFailCountPrev = m_intLeadStandOffVarianceFailCount;
            }
            else
            {
                lbl_LeadStandOffVarianceFailCount.BackColor = Color.White;
                lbl_LeadStandOffVarianceFailPercent.BackColor = Color.White;
            }

            m_intLeadCoplanFailCount = m_smVisionInfo.g_intLeadCoplanFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadCoplanFailPercent = (m_intLeadCoplanFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadCoplanFailPercent > 100)
                    fLeadCoplanFailPercent = 100;
                lbl_LeadCoplanFailPercent.Text = fLeadCoplanFailPercent.ToString("f2");
            }
            else
                lbl_LeadCoplanFailPercent.Text = "0.00";

            if (m_intLeadCoplanFailCount != m_intLeadCoplanFailCountPrev)
            {
                lbl_LeadCoplanFailCount.BackColor = Color.Red;
                lbl_LeadCoplanFailPercent.BackColor = Color.Red;
                lbl_LeadCoplanFailCount.Text = m_intLeadCoplanFailCount.ToString();
                m_intLeadCoplanFailCountPrev = m_intLeadCoplanFailCount;
            }
            else
            {
                lbl_LeadCoplanFailCount.BackColor = Color.White;
                lbl_LeadCoplanFailPercent.BackColor = Color.White;
            }

            m_intLeadAGVFailCount = m_smVisionInfo.g_intLeadAGVFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadAGVFailPercent = (m_intLeadAGVFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadAGVFailPercent > 100)
                    fLeadAGVFailPercent = 100;
                lbl_LeadAGVFailPercent.Text = fLeadAGVFailPercent.ToString("f2");
            }
            else
                lbl_LeadAGVFailPercent.Text = "0.00";

            if (m_intLeadAGVFailCount != m_intLeadAGVFailCountPrev)
            {
                lbl_LeadAGVFailCount.BackColor = Color.Red;
                lbl_LeadAGVFailPercent.BackColor = Color.Red;
                lbl_LeadAGVFailCount.Text = m_intLeadAGVFailCount.ToString();
                m_intLeadAGVFailCountPrev = m_intLeadAGVFailCount;
            }
            else
            {
                lbl_LeadAGVFailCount.BackColor = Color.White;
                lbl_LeadAGVFailPercent.BackColor = Color.White;
            }

            m_intLeadSpanFailCount = m_smVisionInfo.g_intLeadSpanFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadSpanFailPercent = (m_intLeadSpanFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadSpanFailPercent > 100)
                    fLeadSpanFailPercent = 100;
                lbl_LeadSpanFailPercent.Text = fLeadSpanFailPercent.ToString("f2");
            }
            else
                lbl_LeadSpanFailPercent.Text = "0.00";

            if (m_intLeadSpanFailCount != m_intLeadSpanFailCountPrev)
            {
                lbl_LeadSpanFailCount.BackColor = Color.Red;
                lbl_LeadSpanFailPercent.BackColor = Color.Red;
                lbl_LeadSpanFailCount.Text = m_intLeadSpanFailCount.ToString();
                m_intLeadSpanFailCountPrev = m_intLeadSpanFailCount;
            }
            else
            {
                lbl_LeadSpanFailCount.BackColor = Color.White;
                lbl_LeadSpanFailPercent.BackColor = Color.White;
            }

            m_intLeadSweepsFailCount = m_smVisionInfo.g_intLeadSweepsFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadSweepsFailPercent = (m_intLeadSweepsFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadSweepsFailPercent > 100)
                    fLeadSweepsFailPercent = 100;
                lbl_LeadSweepsFailPercent.Text = fLeadSweepsFailPercent.ToString("f2");
            }
            else
                lbl_LeadSweepsFailPercent.Text = "0.00";

            if (m_intLeadSweepsFailCount != m_intLeadSweepsFailCountPrev)
            {
                lbl_LeadSweepsFailCount.BackColor = Color.Red;
                lbl_LeadSweepsFailPercent.BackColor = Color.Red;
                lbl_LeadSweepsFailCount.Text = m_intLeadSweepsFailCount.ToString();
                m_intLeadSweepsFailCountPrev = m_intLeadSweepsFailCount;
            }
            else
            {
                lbl_LeadSweepsFailCount.BackColor = Color.White;
                lbl_LeadSweepsFailPercent.BackColor = Color.White;
            }

            m_intLeadUnCutTiebarFailCount = m_smVisionInfo.g_intLeadUnCutTiebarFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadUnCutTiebarFailPercent = (m_intLeadUnCutTiebarFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadUnCutTiebarFailPercent > 100)
                    fLeadUnCutTiebarFailPercent = 100;
                lbl_LeadUnCutTiebarFailPercent.Text = fLeadUnCutTiebarFailPercent.ToString("f2");
            }
            else
                lbl_LeadUnCutTiebarFailPercent.Text = "0.00";

            if (m_intLeadUnCutTiebarFailCount != m_intLeadUnCutTiebarFailCountPrev)
            {
                lbl_LeadUnCutTiebarFailCount.BackColor = Color.Red;
                lbl_LeadUnCutTiebarFailPercent.BackColor = Color.Red;
                lbl_LeadUnCutTiebarFailCount.Text = m_intLeadUnCutTiebarFailCount.ToString();
                m_intLeadUnCutTiebarFailCountPrev = m_intLeadUnCutTiebarFailCount;
            }
            else
            {
                lbl_LeadUnCutTiebarFailCount.BackColor = Color.White;
                lbl_LeadUnCutTiebarFailPercent.BackColor = Color.White;
            }

            m_intLeadMissingFailCount = m_smVisionInfo.g_intLeadMissingFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadMissingFailPercent = (m_intLeadMissingFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadMissingFailPercent > 100)
                    fLeadMissingFailPercent = 100;
                lbl_LeadMissingFailPercent.Text = fLeadMissingFailPercent.ToString("f2");
            }
            else
                lbl_LeadMissingFailPercent.Text = "0.00";

            if (m_intLeadMissingFailCount != m_intLeadMissingFailCountPrev)
            {
                lbl_LeadMissingFailCount.BackColor = Color.Red;
                lbl_LeadMissingFailPercent.BackColor = Color.Red;
                lbl_LeadMissingFailCount.Text = m_intLeadMissingFailCount.ToString();
                m_intLeadMissingFailCountPrev = m_intLeadMissingFailCount;
            }
            else
            {
                lbl_LeadMissingFailCount.BackColor = Color.White;
                lbl_LeadMissingFailPercent.BackColor = Color.White;
            }

            m_intLeadContaminationFailCount = m_smVisionInfo.g_intLeadContaminationFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadContaminationFailPercent = (m_intLeadContaminationFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadContaminationFailPercent > 100)
                    fLeadContaminationFailPercent = 100;
                lbl_LeadContaminationFailPercent.Text = fLeadContaminationFailPercent.ToString("f2");
            }
            else
                lbl_LeadContaminationFailPercent.Text = "0.00";

            if (m_intLeadContaminationFailCount != m_intLeadContaminationFailCountPrev)
            {
                lbl_LeadContaminationFailCount.BackColor = Color.Red;
                lbl_LeadContaminationFailPercent.BackColor = Color.Red;
                lbl_LeadContaminationFailCount.Text = m_intLeadContaminationFailCount.ToString();
                m_intLeadContaminationFailCountPrev = m_intLeadContaminationFailCount;
            }
            else
            {
                lbl_LeadContaminationFailCount.BackColor = Color.White;
                lbl_LeadContaminationFailPercent.BackColor = Color.White;
            }

            m_intLeadPkgDefectFailCount = m_smVisionInfo.g_intLeadPkgDefectFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadPkgDefectFailPercent = (m_intLeadPkgDefectFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadPkgDefectFailPercent > 100)
                    fLeadPkgDefectFailPercent = 100;
                lbl_LeadPkgDefectFailPercent.Text = fLeadPkgDefectFailPercent.ToString("f2");
            }
            else
                lbl_LeadPkgDefectFailPercent.Text = "0.00";

            if (m_intLeadPkgDefectFailCount != m_intLeadPkgDefectFailCountPrev)
            {
                lbl_LeadPkgDefectFailCount.BackColor = Color.Red;
                lbl_LeadPkgDefectFailPercent.BackColor = Color.Red;
                lbl_LeadPkgDefectFailCount.Text = m_intLeadPkgDefectFailCount.ToString();
                m_intLeadPkgDefectFailCountPrev = m_intLeadPkgDefectFailCount;
            }
            else
            {
                lbl_LeadPkgDefectFailCount.BackColor = Color.White;
                lbl_LeadPkgDefectFailPercent.BackColor = Color.White;
            }

            m_intLeadPkgDimensionFailCount = m_smVisionInfo.g_intLeadPkgDimensionFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadPkgDimensionFailPercent = (m_intLeadPkgDimensionFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadPkgDimensionFailPercent > 100)
                    fLeadPkgDimensionFailPercent = 100;
                lbl_LeadPkgDimensionFailPercent.Text = fLeadPkgDimensionFailPercent.ToString("f2");
            }
            else
                lbl_LeadPkgDimensionFailPercent.Text = "0.00";

            if (m_intLeadPkgDimensionFailCount != m_intLeadPkgDimensionFailCountPrev)
            {
                lbl_LeadPkgDimensionFailCount.BackColor = Color.Red;
                lbl_LeadPkgDimensionFailPercent.BackColor = Color.Red;
                lbl_LeadPkgDimensionFailCount.Text = m_intLeadPkgDimensionFailCount.ToString();
                m_intLeadPkgDimensionFailCountPrev = m_intLeadPkgDimensionFailCount;
            }
            else
            {
                lbl_LeadPkgDimensionFailCount.BackColor = Color.White;
                lbl_LeadPkgDimensionFailPercent.BackColor = Color.White;
            }

            m_intPin1FailCount = m_smVisionInfo.g_intPin1FailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fPin1FailPercent = (m_intPin1FailCount / (float)m_intTestedTotal) * 100;
                if (fPin1FailPercent > 100)
                    fPin1FailPercent = 100;
                lbl_Pin1FailPercent.Text = fPin1FailPercent.ToString("f2");
            }
            else
                lbl_Pin1FailPercent.Text = "0.00";

            if (m_intPin1FailCount != m_intPin1FailCountPrev)
            {
                lbl_Pin1FailCount.BackColor = Color.Red;
                lbl_Pin1FailPercent.BackColor = Color.Red;
                lbl_Pin1FailCount.Text = m_intPin1FailCount.ToString();
                m_intPin1FailCountPrev = m_intPin1FailCount;
            }
            else
            {
                lbl_Pin1FailCount.BackColor = Color.White;
                lbl_Pin1FailPercent.BackColor = Color.White;
            }

            m_intEdgeNotFoundFailCount = m_smVisionInfo.g_intEdgeNotFoundFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fEdgeNotFoundFailPercent = (m_intEdgeNotFoundFailCount / (float)m_intTestedTotal) * 100;
                if (fEdgeNotFoundFailPercent > 100)
                    fEdgeNotFoundFailPercent = 100;
                lbl_EdgeNotFoundFailPercent.Text = fEdgeNotFoundFailPercent.ToString("f2");
            }
            else
                lbl_EdgeNotFoundFailPercent.Text = "0.00";

            if (m_intEdgeNotFoundFailCount != m_intEdgeNotFoundFailCountPrev)
            {
                lbl_EdgeNotFoundFailCount.BackColor = Color.Red;
                lbl_EdgeNotFoundFailPercent.BackColor = Color.Red;
                lbl_EdgeNotFoundFailCount.Text = m_intEdgeNotFoundFailCount.ToString();
                m_intEdgeNotFoundFailCountPrev = m_intEdgeNotFoundFailCount;
            }
            else
            {
                lbl_EdgeNotFoundFailCount.BackColor = Color.White;
                lbl_EdgeNotFoundFailPercent.BackColor = Color.White;
            }

            //-------------------------------------Special cases only display when fail happen------------------------------------------------
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
            
            m_intPositionFailCount = m_smVisionInfo.g_intPositionFailureTotal;
            if (m_intPositionFailCount != 0)
            {
                pnl_Position.Visible = true;
            }
            else
            {
                pnl_Position.Visible = false;
            }

            if (m_intTestedTotal != 0)
            {
                float fPositionFailPercent = (m_intPositionFailCount / (float)m_intTestedTotal) * 100;
                if (fPositionFailPercent > 100)
                    fPositionFailPercent = 100;
                lbl_PositionFailPercent.Text = fPositionFailPercent.ToString("f2");
            }
            else
                lbl_PositionFailPercent.Text = "0.00";

            if (m_intPositionFailCount != m_intPositionFailCountPrev)
            {
                lbl_PositionFailCount.BackColor = Color.Red;
                lbl_PositionFailPercent.BackColor = Color.Red;
                lbl_PositionFailCount.Text = m_intPositionFailCount.ToString();
                m_intPositionFailCountPrev = m_intPositionFailCount;
            }
            else
            {
                lbl_PositionFailCount.BackColor = Color.White;
                lbl_PositionFailPercent.BackColor = Color.White;
            }

            m_intEmptyUnitFailCount = m_smVisionInfo.g_intEmptyUnitFailureTotal;
            if (m_intEmptyUnitFailCount != 0)
            {
                pnl_EmptyUnit.Visible = true;
            }
            else
            {
                pnl_EmptyUnit.Visible = false;
            }

            if (m_intTestedTotal != 0)
            {
                float fEmptyUnitFailPercent = (m_intEmptyUnitFailCount / (float)m_intTestedTotal) * 100;
                if (fEmptyUnitFailPercent > 100)
                    fEmptyUnitFailPercent = 100;
                lbl_EmptyUnitFailPercent.Text = fEmptyUnitFailPercent.ToString("f2");
            }
            else
                lbl_EmptyUnitFailPercent.Text = "0.00";

            if (m_intEmptyUnitFailCount != m_intEmptyUnitFailCountPrev)
            {
                lbl_EmptyUnitFailCount.BackColor = Color.Red;
                lbl_EmptyUnitFailPercent.BackColor = Color.Red;
                lbl_EmptyUnitFailCount.Text = m_intEmptyUnitFailCount.ToString();
                m_intEmptyUnitFailCountPrev = m_intEmptyUnitFailCount;
            }
            else
            {
                lbl_EmptyUnitFailCount.BackColor = Color.White;
                lbl_EmptyUnitFailPercent.BackColor = Color.White;
            }

            if (m_smVisionInfo.g_blnNoGrabTime)
            {
                lbl_GrabDelay.Text = "0";
                lbl_GrabTime.Text = "0";
                lbl_ProcessTime.Text = m_smVisionInfo.g_objTotalTime.Duration.ToString("f2");
            }
            else
            {
                lbl_GrabDelay.Text = m_smVisionInfo.g_intCameraGrabDelay.ToString();
                lbl_GrabTime.Text = (Math.Max(0, m_smVisionInfo.g_objGrabTime.Duration - m_smVisionInfo.g_intCameraGrabDelay)).ToString("f0"); //29-05-2019 ZJYEOH : Need to minus grab delay as it included inside Grab time 
                lbl_ProcessTime.Text = (Math.Max(0, m_smVisionInfo.g_objTotalTime.Duration - m_smVisionInfo.g_objGrabTime.Duration)).ToString("f2");
            }
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
            ResetCounterBackColor();
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
        
        private void chk_ForceYtozero_Click(object sender, EventArgs e)
        {
            if (chk_ForceYtozero.Checked)
                m_smVisionInfo.g_intForceYZero = 1;
            else
                m_smVisionInfo.g_intForceYZero = 0;
        }
    }
}