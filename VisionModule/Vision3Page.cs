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
    public partial class Vision3Page : Form
    {
        #region Member Variables

        private int m_intMachineStatusPrev = 1;
        private int m_intUserGroup;
        // Count
        private int m_intOrientFailCount = 0, m_intOrientFailCountPrev = -1;
        private int m_intPassCount = 0, m_intPassCountPrev = -1;
        private int m_intFailCount = 0, m_intFailCountPrev = -1;
        private int m_intTestedTotal = 0, m_intTestedTotalPrev = -1;
        private int m_intCenterPadOffsetFailCount = 0, m_intCenterPadOffsetFailCountPrev = -1;
        private int m_intCenterPadAreaFailCount = 0, m_intCenterPadAreaFailCountPrev = -1;
        private int m_intCenterPadDimensionFailCount = 0, m_intCenterPadDimensionFailCountPrev = -1;
        private int m_intCenterPadPitchGapFailCount = 0, m_intCenterPadPitchGapFailCountPrev = -1;
        private int m_intCenterPadBrokenFailCount = 0, m_intCenterPadBrokenFailCountPrev = -1;
        private int m_intCenterPadExcessFailCount = 0, m_intCenterPadExcessFailCountPrev = -1;
        private int m_intCenterPadSmearFailCount = 0, m_intCenterPadSmearFailCountPrev = -1;
        private int m_intCenterPadEdgeLimitFailCount = 0, m_intCenterPadEdgeLimitFailCountPrev = -1;
        private int m_intCenterPadStandOffFailCount = 0, m_intCenterPadStandOffFailCountPrev = -1;
        private int m_intCenterPadEdgeDistanceFailCount = 0, m_intCenterPadEdgeDistanceFailCountPrev = -1;
        private int m_intCenterPadSpanFailCount = 0, m_intCenterPadSpanFailCountPrev = -1;
        private int m_intCenterPadContaminationFailCount = 0, m_intCenterPadContaminationFailCountPrev = -1;
        private int m_intCenterPadMissingFailCount = 0, m_intCenterPadMissingFailCountPrev = -1;
        private int m_intCenterPadColorDefectFailCount = 0, m_intCenterPadColorDefectFailCountPrev = -1;

        private int m_intSidePadOffsetFailCount = 0, m_intSidePadOffsetFailCountPrev = -1;
        private int m_intSidePadAreaFailCount = 0, m_intSidePadAreaFailCountPrev = -1;
        private int m_intSidePadDimensionFailCount = 0, m_intSidePadDimensionFailCountPrev = -1;
        private int m_intSidePadPitchGapFailCount = 0, m_intSidePadPitchGapFailCountPrev = -1;
        private int m_intSidePadBrokenFailCount = 0, m_intSidePadBrokenFailCountPrev = -1;
        private int m_intSidePadExcessFailCount = 0, m_intSidePadExcessFailCountPrev = -1;
        private int m_intSidePadSmearFailCount = 0, m_intSidePadSmearFailCountPrev = -1;
        private int m_intSidePadEdgeLimitFailCount = 0, m_intSidePadEdgeLimitFailCountPrev = -1;
        private int m_intSidePadStandOffFailCount = 0, m_intSidePadStandOffFailCountPrev = -1;
        private int m_intSidePadEdgeDistanceFailCount = 0, m_intSidePadEdgeDistanceFailCountPrev = -1;
        private int m_intSidePadSpanFailCount = 0, m_intSidePadSpanFailCountPrev = -1;
        private int m_intSidePadContaminationFailCount = 0, m_intSidePadContaminationFailCountPrev = -1;
        private int m_intSidePadMissingFailCount = 0, m_intSidePadMissingFailCountPrev = -1;
        private int m_intSidePadColorDefectFailCount = 0, m_intSidePadColorDefectFailCountPrev = -1;
        private int m_intCenterPkgDefectFailCount = 0, m_intCenterPkgDefectFailCountPrev = -1;

        private int m_intCenterPkgDimensionFailCount = 0, m_intCenterPkgDimensionFailCountPrev = -1;
        private int m_intSidePkgDefectFailCount = 0, m_intSidePkgDefectFailCountPrev = -1;
        private int m_intSidePkgDimensionFailCount = 0, m_intSidePkgDimensionFailCountPrev = -1;

        private int m_intPadFailCount = 0, m_intPadFailCountPrev = -1;

        private void label15_Click(object sender, EventArgs e)
        {
            
        }

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



        public Vision3Page(CustomOption smCustomOption, ProductionInfo smProductionInfo, VisionInfo smVisionInfo)
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
            lbl_OrientFailCount.BackColor = Color.White;
            lbl_PadFailCount.BackColor = Color.White;
            lbl_PositionFailCount.BackColor = Color.White;
            lbl_EmptyUnitFailCount.BackColor = Color.White;
            lbl_NoTemplateFailCount.BackColor = Color.White;
            lbl_EdgeNotFoundFailCount.BackColor = Color.White;
            lbl_Pin1FailCount.BackColor = Color.White;
            lbl_CenterPadOffsetFailCount.BackColor = Color.White;
            lbl_CenterPadAreaFailCount.BackColor = Color.White;
            lbl_CenterPadDimensionFailCount.BackColor = Color.White;
            lbl_CenterPadPitchGapFailCount.BackColor = Color.White;
            lbl_CenterPadBrokenFailCount.BackColor = Color.White;
            lbl_CenterPadExcessFailCount.BackColor = Color.White;
            lbl_CenterPadSmearFailCount.BackColor = Color.White;
            lbl_CenterPadEdgeLimitFailCount.BackColor = Color.White;
            lbl_CenterPadStandOffFailCount.BackColor = Color.White;
            lbl_CenterPadEdgeDistanceFailCount.BackColor = Color.White;
            lbl_CenterPadSpanFailCount.BackColor = Color.White;
            lbl_CenterPadContaminationFailCount.BackColor = Color.White;
            lbl_CenterPadMissingFailCount.BackColor = Color.White;
            lbl_CenterPadColorDefectFailCount.BackColor = Color.White;
            lbl_SidePadOffsetFailCount.BackColor = Color.White;
            lbl_SidePadAreaFailCount.BackColor = Color.White;
            lbl_SidePadDimensionFailCount.BackColor = Color.White;
            lbl_SidePadPitchGapFailCount.BackColor = Color.White;
            lbl_SidePadBrokenFailCount.BackColor = Color.White;
            lbl_SidePadExcessFailCount.BackColor = Color.White;
            lbl_SidePadSmearFailCount.BackColor = Color.White;
            lbl_SidePadEdgeLimitFailCount.BackColor = Color.White;
            lbl_SidePadStandOffFailCount.BackColor = Color.White;
            lbl_SidePadEdgeDistanceFailCount.BackColor = Color.White;
            lbl_SidePadSpanFailCount.BackColor = Color.White;
            lbl_SidePadContaminationFailCount.BackColor = Color.White;
            lbl_SidePadMissingFailCount.BackColor = Color.White;
            lbl_SidePadColorDefectFailCount.BackColor = Color.White;
            lbl_CenterPkgDefectFailCount.BackColor = Color.White;
            lbl_CenterPkgDimensionFailCount.BackColor = Color.White;
            lbl_SidePkgDefectFailCount.BackColor = Color.White;
            lbl_SidePkgDimensionFailCount.BackColor = Color.White;

            lbl_OrientFailPercent.BackColor = Color.White;
            lbl_PadFailPercent.BackColor = Color.White;
            lbl_PositionFailPercent.BackColor = Color.White;
            lbl_EmptyUnitFailPercent.BackColor = Color.White;
            lbl_NoTemplateFailPercent.BackColor = Color.White;
            lbl_EdgeNotFoundFailPercent.BackColor = Color.White;
            lbl_Pin1FailPercent.BackColor = Color.White;
            lbl_CenterPadOffsetFailPercent.BackColor = Color.White;
            lbl_CenterPadAreaFailPercent.BackColor = Color.White;
            lbl_CenterPadDimensionFailPercent.BackColor = Color.White;
            lbl_CenterPadPitchGapFailPercent.BackColor = Color.White;
            lbl_CenterPadBrokenFailPercent.BackColor = Color.White;
            lbl_CenterPadExcessFailPercent.BackColor = Color.White;
            lbl_CenterPadSmearFailPercent.BackColor = Color.White;
            lbl_CenterPadEdgeLimitFailPercent.BackColor = Color.White;
            lbl_CenterPadStandOffFailPercent.BackColor = Color.White;
            lbl_CenterPadEdgeDistanceFailPercent.BackColor = Color.White;
            lbl_CenterPadSpanFailPercent.BackColor = Color.White;
            lbl_CenterPadContaminationFailPercent.BackColor = Color.White;
            lbl_CenterPadMissingFailPercent.BackColor = Color.White;
            lbl_CenterPadColorDefectFailPercent.BackColor = Color.White;
            lbl_SidePadOffsetFailPercent.BackColor = Color.White;
            lbl_SidePadAreaFailPercent.BackColor = Color.White;
            lbl_SidePadDimensionFailPercent.BackColor = Color.White;
            lbl_SidePadPitchGapFailPercent.BackColor = Color.White;
            lbl_SidePadBrokenFailPercent.BackColor = Color.White;
            lbl_SidePadExcessFailPercent.BackColor = Color.White;
            lbl_SidePadSmearFailPercent.BackColor = Color.White;
            lbl_SidePadEdgeLimitFailPercent.BackColor = Color.White;
            lbl_SidePadStandOffFailPercent.BackColor = Color.White;
            lbl_SidePadEdgeDistanceFailPercent.BackColor = Color.White;
            lbl_SidePadSpanFailPercent.BackColor = Color.White;
            lbl_SidePadContaminationFailPercent.BackColor = Color.White;
            lbl_SidePadMissingFailPercent.BackColor = Color.White;
            lbl_SidePadColorDefectFailPercent.BackColor = Color.White;
            lbl_CenterPkgDefectFailPercent.BackColor = Color.White;
            lbl_CenterPkgDimensionFailPercent.BackColor = Color.White;
            lbl_SidePkgDefectFailPercent.BackColor = Color.White;
            lbl_SidePkgDimensionFailPercent.BackColor = Color.White;
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
            //int intPositionY = lbl_Detail.Location.Y + lbl_Detail.Size.Height - 1;

            //if (((m_smCustomOption.g_intWantPad & (1 << m_smVisionInfo.g_intVisionPos)) == 0) &&
            //    ((m_smCustomOption.g_intWantPad5S & (1 << m_smVisionInfo.g_intVisionPos)) == 0))
            //{
            //    lbl_Pad.Visible = false;
            //    lbl_PadFailCount.Visible = false;
            //    //lbl_Position.Location = new Point(lbl_Position.Location.X, intPositionY);
            //    //lbl_PositionFailCount.Location = new Point(lbl_PositionFailCount.Location.X, intPositionY);
            //    intPositionY += 17;
            //}
            //else
            //{
            //    intPositionY += 34;
            //    lbl_Position.Location = new Point(lbl_Position.Location.X, intPositionY);
            //    lbl_PositionFailCount.Location = new Point(lbl_PositionFailCount.Location.X, intPositionY);
            //    intPositionY += 17;
            //}

            //if ((m_smCustomOption.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            //{
            //    lbl_Package.Visible = false;
            //    lbl_PackageFailCount.Visible = false;
            //}
            //else
            //{
            //    lbl_Package.Location = new Point(lbl_Package.Location.X, intPositionY);
            //    lbl_PackageFailCount.Location = new Point(lbl_PackageFailCount.Location.X, intPositionY);
            //    intPositionY += 17;
            //}

            //if (!m_smVisionInfo.g_blnWantPin1)
            //{
            //    lbl_Pin1.Visible = false;
            //    lbl_Pin1FailCount.Visible = false;
            //}
            //else
            //{
            //    lbl_Pin1.Visible = true;
            //    lbl_Pin1FailCount.Visible = true;
            //    lbl_Pin1.Location = new Point(lbl_Pin1.Location.X, intPositionY);
            //    lbl_Pin1FailCount.Location = new Point(lbl_Pin1FailCount.Location.X, intPositionY);
            //}

            if ((m_smCustomOption.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                pic_Template.Visible = false;
                pnl_Orient.Visible = false;
                lblOrientationResult.Visible = false;
            }
            else
            {
                if ((m_smCustomOption.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    pnl_Orient.Visible = false;
                    lblOrientationResult.Visible = false;
                }
            }

            if ((m_smCustomOption.g_intWantPad & (1 << m_smVisionInfo.g_intVisionPos)) == 0 && (m_smCustomOption.g_intWantPad5S & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                // Remove both center and side pad
                //if (pnl_Detail.Controls.Contains(pnl_Pad))
                //    pnl_Detail.Controls.Remove(pnl_Pad);
                //if (pnl_Detail.Controls.Contains(pnl_CenterPadArea))
                //    pnl_Detail.Controls.Remove(pnl_CenterPadArea);
                //if (pnl_Detail.Controls.Contains(pnl_CenterPadOffset))
                //    pnl_Detail.Controls.Remove(pnl_CenterPadOffset);
                //if (pnl_Detail.Controls.Contains(pnl_CenterPadDimension))
                //    pnl_Detail.Controls.Remove(pnl_CenterPadDimension);
                //if (pnl_Detail.Controls.Contains(pnl_CenterPadContamination))
                //    pnl_Detail.Controls.Remove(pnl_CenterPadContamination);
                //if (pnl_Detail.Controls.Contains(pnl_CenterPadBroken))
                //    pnl_Detail.Controls.Remove(pnl_CenterPadBroken);
                //if (pnl_Detail.Controls.Contains(pnl_CenterPadExcess))
                //    pnl_Detail.Controls.Remove(pnl_CenterPadExcess);
                //if (pnl_Detail.Controls.Contains(pnl_CenterPadPitchGap))
                //    pnl_Detail.Controls.Remove(pnl_CenterPadPitchGap);
                //if (pnl_Detail.Controls.Contains(pnl_CenterPadSmear))
                //    pnl_Detail.Controls.Remove(pnl_CenterPadSmear);
                //if (pnl_Detail.Controls.Contains(pnl_CenterPadMissing))
                //    pnl_Detail.Controls.Remove(pnl_CenterPadMissing);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadArea))
                //    pnl_Detail.Controls.Remove(pnl_SidePadArea);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadOffset))
                //    pnl_Detail.Controls.Remove(pnl_SidePadOffset);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadDimension))
                //    pnl_Detail.Controls.Remove(pnl_SidePadDimension);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadContamination))
                //    pnl_Detail.Controls.Remove(pnl_SidePadContamination);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadBroken))
                //    pnl_Detail.Controls.Remove(pnl_SidePadBroken);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadExcess))
                //    pnl_Detail.Controls.Remove(pnl_SidePadExcess);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadPitchGap))
                //    pnl_Detail.Controls.Remove(pnl_SidePadPitchGap);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadSmear))
                //    pnl_Detail.Controls.Remove(pnl_SidePadSmear);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadMissing))
                //    pnl_Detail.Controls.Remove(pnl_SidePadMissing);

                pnl_Pad.Visible = false;
                pnl_CenterPadArea.Visible = false;
                pnl_CenterPadOffset.Visible = false;
                pnl_CenterPadDimension.Visible = false;
                pnl_CenterPadContamination.Visible = false;
                pnl_CenterPadBroken.Visible = false;
                pnl_CenterPadExcess.Visible = false;
                pnl_CenterPadPitchGap.Visible = false;
                pnl_CenterPadSmear.Visible = false;
                pnl_CenterPadEdgeLimit.Visible = false;
                pnl_CenterPadStandOff.Visible = false;
                pnl_CenterPadEdgeDistance.Visible = false;
                pnl_CenterPadSpan.Visible = false;
                pnl_CenterPadMissing.Visible = false;
                pnl_CenterPadColorDefect.Visible = false;
                pnl_SidePadArea.Visible = false;
                pnl_SidePadOffset.Visible = false;
                pnl_SidePadDimension.Visible = false;
                pnl_SidePadContamination.Visible = false;
                pnl_SidePadBroken.Visible = false;
                pnl_SidePadExcess.Visible = false;
                pnl_SidePadPitchGap.Visible = false;
                pnl_SidePadSmear.Visible = false;
                pnl_SidePadEdgeLimit.Visible = false;
                pnl_SidePadStandOff.Visible = false;
                pnl_SidePadEdgeDistance.Visible = false;
                pnl_SidePadSpan.Visible = false;
                pnl_SidePadMissing.Visible = false;
                pnl_SidePadColorDefect.Visible = false;
            }
            else if ((m_smCustomOption.g_intWantPad & (1 << m_smVisionInfo.g_intVisionPos)) != 0 && (m_smCustomOption.g_intWantPad5S & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                //// Remove side Pad
                //if (pnl_Detail.Controls.Contains(pnl_SidePadArea))
                //    pnl_Detail.Controls.Remove(pnl_SidePadArea);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadOffset))
                //    pnl_Detail.Controls.Remove(pnl_SidePadOffset);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadDimension))
                //    pnl_Detail.Controls.Remove(pnl_SidePadDimension);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadContamination))
                //    pnl_Detail.Controls.Remove(pnl_SidePadContamination);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadBroken))
                //    pnl_Detail.Controls.Remove(pnl_SidePadBroken);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadExcess))
                //    pnl_Detail.Controls.Remove(pnl_SidePadExcess);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadPitchGap))
                //    pnl_Detail.Controls.Remove(pnl_SidePadPitchGap);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadSmear))
                //    pnl_Detail.Controls.Remove(pnl_SidePadSmear);
                //if (pnl_Detail.Controls.Contains(pnl_SidePadMissing))
                //    pnl_Detail.Controls.Remove(pnl_SidePadMissing);
                //if (pnl_Detail.Controls.Contains(pnl_SidePkgDefect))
                //    pnl_Detail.Controls.Remove(pnl_SidePkgDefect);
                //if (pnl_Detail.Controls.Contains(pnl_SidePkgDimension))
                //    pnl_Detail.Controls.Remove(pnl_SidePkgDimension);

                pnl_SidePadArea.Visible = false;
                pnl_SidePadOffset.Visible = false;
                pnl_SidePadDimension.Visible = false;
                pnl_SidePadContamination.Visible = false;
                pnl_SidePadBroken.Visible = false;
                pnl_SidePadExcess.Visible = false;
                pnl_SidePadPitchGap.Visible = false;
                pnl_SidePadSmear.Visible = false;
                pnl_SidePadEdgeLimit.Visible = false;
                pnl_SidePadStandOff.Visible = false;
                pnl_SidePadEdgeDistance.Visible = false;
                pnl_SidePadSpan.Visible = false;
                pnl_SidePadMissing.Visible = false;
                pnl_SidePadColorDefect.Visible = false;
                pnl_SidePkgDefect.Visible = false;
                pnl_SidePkgDimension.Visible = false;

                int intFailMask = m_smVisionInfo.g_arrPad[0].ref_intFailOptionMask;
                if ((intFailMask & 0x20) > 0)
                {
                    pnl_CenterPadArea.Visible = true;
                }
                else
                {
                    if (m_intCenterPadAreaFailCount == 0)
                    {
                        pnl_CenterPadArea.Visible = false;
                    }
                }

                if ((intFailMask & 0xC0) > 0)
                {
                    pnl_CenterPadDimension.Visible = true;
                }
                else
                {
                    if (m_intCenterPadDimensionFailCount == 0)
                    {
                        pnl_CenterPadDimension.Visible = false;
                    }
                }

                if ((intFailMask & 0x100) > 0)
                {
                    pnl_CenterPadOffset.Visible = true;
                }
                else
                {
                    if (m_intCenterPadOffsetFailCount == 0)
                    {
                        pnl_CenterPadOffset.Visible = false;
                    }
                }

                if ((intFailMask & 0x1001) > 0)
                {
                    pnl_CenterPadContamination.Visible = true;
                }
                else
                {
                    if (m_intCenterPadContaminationFailCount == 0)
                    {
                        pnl_CenterPadContamination.Visible = false;
                    }
                }

                if ((intFailMask & 0x18) > 0)
                {
                    pnl_CenterPadBroken.Visible = true;
                }
                else
                {
                    if (m_intCenterPadBrokenFailCount == 0)
                    {
                        pnl_CenterPadBroken.Visible = false;
                    }
                }

                if ((intFailMask & 0x600) > 0)
                {
                    pnl_CenterPadPitchGap.Visible = true;
                }
                else
                {
                    if (m_intCenterPadPitchGapFailCount == 0)
                    {
                        pnl_CenterPadPitchGap.Visible = false;
                    }
                }

                if ((intFailMask & 0x800) > 0)
                {
                    pnl_CenterPadExcess.Visible = true;
                }
                else
                {
                    if (m_intCenterPadExcessFailCount == 0)
                    {
                        pnl_CenterPadExcess.Visible = false;
                    }
                }

                if ((intFailMask & 0x2000) > 0)
                {
                    pnl_CenterPadSmear.Visible = true;
                }
                else
                {
                    if (m_intCenterPadSmearFailCount == 0)
                    {
                        pnl_CenterPadSmear.Visible = false;
                    }
                }

                if (((intFailMask & 0x4000) > 0) && m_smVisionInfo.g_arrPad[0].ref_blnWantEdgeLimit_Pad)
                {
                    pnl_CenterPadEdgeLimit.Visible = true;
                }
                else
                {
                    if (m_intCenterPadEdgeLimitFailCount == 0)
                    {
                        pnl_CenterPadEdgeLimit.Visible = false;
                    }
                }

                if (((intFailMask & 0x8000) > 0) && m_smVisionInfo.g_arrPad[0].ref_blnWantStandOff_Pad)
                {
                    pnl_CenterPadStandOff.Visible = true;
                }
                else
                {
                    if (m_intCenterPadStandOffFailCount == 0)
                    {
                        pnl_CenterPadStandOff.Visible = false;
                    }
                }

                if (((intFailMask & 0x10000) > 0) && m_smVisionInfo.g_arrPad[0].ref_blnWantEdgeDistance_Pad)
                {
                    pnl_CenterPadEdgeDistance.Visible = true;
                }
                else
                {
                    if (m_intCenterPadEdgeDistanceFailCount == 0)
                    {
                        pnl_CenterPadEdgeDistance.Visible = false;
                    }
                }

                if (((intFailMask & 0x20000) > 0 || (intFailMask & 0x40000) > 0) && m_smVisionInfo.g_arrPad[0].ref_blnWantSpan_Pad)
                {
                    pnl_CenterPadSpan.Visible = true;
                }
                else
                {
                    if (m_intCenterPadSpanFailCount == 0)
                    {
                        pnl_CenterPadSpan.Visible = false;
                    }
                }

                if ((m_smCustomOption.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                {
                    pnl_CenterPkgDefect.Visible = false;
                    pnl_CenterPkgDimension.Visible = false;
                }
                else
                {
                    //Remove contamination if package is on
                    pnl_CenterPadContamination.Visible = false;
                    pnl_SidePadContamination.Visible = false;

                    if (m_smVisionInfo.g_arrPad[0].GetWantInspectPackage())
                    {
                        pnl_CenterPkgDefect.Visible = true;
                    }
                    else
                    {
                        if (m_intCenterPkgDefectFailCount == 0)
                        {
                            pnl_CenterPkgDefect.Visible = false;
                        }
                    }

                    if ((m_smVisionInfo.g_arrPad[0].ref_intFailPkgOptionMask & 0x01) > 0)
                    {
                        pnl_CenterPkgDimension.Visible = true;
                    }
                    else
                    {
                        if (m_intCenterPkgDimensionFailCount == 0)
                        {
                            pnl_CenterPkgDimension.Visible = false;
                        }
                    }
                }
                
                if ((m_smCustomOption.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                {
                    pnl_CenterPadColorDefect.Visible = false;
                }
                else
                {
                    if (m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask > 0)
                    {
                        pnl_CenterPadColorDefect.Visible = true;
                    }
                    else
                    {
                        if (m_intCenterPadColorDefectFailCount == 0)
                        {
                            pnl_CenterPadColorDefect.Visible = false;
                        }
                    }
                }
            }
            else
            {
                // center pad
                int intFailMask = m_smVisionInfo.g_arrPad[0].ref_intFailOptionMask;
                if ((intFailMask & 0x20) > 0)
                {
                    pnl_CenterPadArea.Visible = true;
                }
                else
                {
                    if (m_intCenterPadAreaFailCount == 0)
                    {
                        pnl_CenterPadArea.Visible = false;
                    }
                }

                if ((intFailMask & 0xC0) > 0)
                {
                    pnl_CenterPadDimension.Visible = true;
                }
                else
                {
                    if (m_intCenterPadDimensionFailCount == 0)
                    {
                        pnl_CenterPadDimension.Visible = false;
                    }
                }

                if ((intFailMask & 0x100) > 0)
                {
                    pnl_CenterPadOffset.Visible = true;
                }
                else
                {
                    if (m_intCenterPadOffsetFailCount == 0)
                    {
                        pnl_CenterPadOffset.Visible = false;
                    }
                }

                if ((intFailMask & 0x1001) > 0)
                {
                    pnl_CenterPadContamination.Visible = true;
                }
                else
                {
                    if (m_intCenterPadContaminationFailCount == 0)
                    {
                        pnl_CenterPadContamination.Visible = false;
                    }
                }

                if ((intFailMask & 0x18) > 0)
                {
                    pnl_CenterPadBroken.Visible = true;
                }
                else
                {
                    if (m_intCenterPadBrokenFailCount == 0)
                    {
                        pnl_CenterPadBroken.Visible = false;
                    }
                }

                if ((intFailMask & 0x600) > 0)
                {
                    pnl_CenterPadPitchGap.Visible = true;
                }
                else
                {
                    if (m_intCenterPadPitchGapFailCount == 0)
                    {
                        pnl_CenterPadPitchGap.Visible = false;
                    }
                }

                if ((intFailMask & 0x800) > 0)
                {
                    pnl_CenterPadExcess.Visible = true;
                }
                else
                {
                    if (m_intCenterPadExcessFailCount == 0)
                    {
                        pnl_CenterPadExcess.Visible = false;
                    }
                }

                if ((intFailMask & 0x2000) > 0)
                {
                    pnl_CenterPadSmear.Visible = true;
                }
                else
                {
                    if (m_intCenterPadSmearFailCount == 0)
                    {
                        pnl_CenterPadSmear.Visible = false;
                    }
                }

                if (((intFailMask & 0x4000) > 0) && m_smVisionInfo.g_arrPad[0].ref_blnWantEdgeLimit_Pad) 
                {
                    pnl_CenterPadEdgeLimit.Visible = true;
                }
                else
                {
                    if (m_intCenterPadEdgeLimitFailCount == 0)
                    {
                        pnl_CenterPadEdgeLimit.Visible = false;
                    }
                }

                if (((intFailMask & 0x8000) > 0) && m_smVisionInfo.g_arrPad[0].ref_blnWantStandOff_Pad)
                {
                    pnl_CenterPadStandOff.Visible = true;
                }
                else
                {
                    if (m_intCenterPadStandOffFailCount == 0)
                    {
                        pnl_CenterPadStandOff.Visible = false;
                    }
                }

                if (((intFailMask & 0x10000) > 0) && m_smVisionInfo.g_arrPad[0].ref_blnWantEdgeDistance_Pad)
                {
                    pnl_CenterPadEdgeDistance.Visible = true;
                }
                else
                {
                    if (m_intCenterPadEdgeDistanceFailCount == 0)
                    {
                        pnl_CenterPadEdgeDistance.Visible = false;
                    }
                }

                if (((intFailMask & 0x20000) > 0 || (intFailMask & 0x40000) > 0) && m_smVisionInfo.g_arrPad[0].ref_blnWantSpan_Pad)
                {
                    pnl_CenterPadSpan.Visible = true;
                }
                else
                {
                    if (m_intCenterPadSpanFailCount == 0)
                    {
                        pnl_CenterPadSpan.Visible = false;
                    }
                }

                //Center Color Defect
                if ((m_smCustomOption.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                {
                    pnl_CenterPadColorDefect.Visible = false;
                }
                else
                {
                    if (m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask > 0)
                    {
                        pnl_CenterPadColorDefect.Visible = true;
                    }
                    else
                    {
                        if (m_intCenterPadColorDefectFailCount == 0)
                        {
                            pnl_CenterPadColorDefect.Visible = false;
                        }
                    }
                }

                //Side Pad
                int intSideFailMask = m_smVisionInfo.g_arrPad[1].ref_intFailOptionMask;
                if ((intSideFailMask & 0x20) > 0)
                {
                    pnl_SidePadArea.Visible = true;
                }
                else
                {
                    if (m_intSidePadAreaFailCount == 0)
                    {
                        pnl_SidePadArea.Visible = false;
                    }
                }

                if ((intSideFailMask & 0xC0) > 0)
                {
                    pnl_SidePadDimension.Visible = true;
                }
                else
                {
                    if (m_intSidePadDimensionFailCount == 0)
                    {
                        pnl_SidePadDimension.Visible = false;
                    }
                }

                if ((intSideFailMask & 0x100) > 0)
                {
                    pnl_SidePadOffset.Visible = true;
                }
                else
                {
                    if (m_intSidePadOffsetFailCount == 0)
                    {
                        pnl_SidePadOffset.Visible = false;
                    }
                }

                if ((intSideFailMask & 0x1001) > 0)
                {
                    pnl_SidePadContamination.Visible = true;
                }
                else
                {
                    if (m_intSidePadContaminationFailCount == 0)
                    {
                        pnl_SidePadContamination.Visible = false;
                    }
                }

                if ((intSideFailMask & 0x18) > 0)
                {
                    pnl_SidePadBroken.Visible = true;
                }
                else
                {
                    if (m_intSidePadBrokenFailCount == 0)
                    {
                        pnl_SidePadBroken.Visible = false;
                    }
                }

                if ((intSideFailMask & 0x600) > 0)
                {
                    pnl_SidePadPitchGap.Visible = true;
                }
                else
                {
                    if (m_intSidePadPitchGapFailCount == 0)
                    {
                        pnl_SidePadPitchGap.Visible = false;
                    }
                }

                if ((intSideFailMask & 0x800) > 0)
                {
                    pnl_SidePadExcess.Visible = true;
                }
                else
                {
                    if (m_intSidePadExcessFailCount == 0)
                    {
                        pnl_SidePadExcess.Visible = false;
                    }
                }

                if ((intSideFailMask & 0x2000) > 0)
                {
                    pnl_SidePadSmear.Visible = true;
                }
                else
                {
                    if (m_intSidePadSmearFailCount == 0)
                    {
                        pnl_SidePadSmear.Visible = false;
                    }
                }

                if (((intSideFailMask & 0x4000) > 0) && m_smVisionInfo.g_arrPad[1].ref_blnWantEdgeLimit_Pad) 
                {
                    pnl_SidePadEdgeLimit.Visible = true;
                }
                else
                {
                    if (m_intSidePadEdgeLimitFailCount == 0)
                    {
                        pnl_SidePadEdgeLimit.Visible = false;
                    }
                }

                if (((intSideFailMask & 0x8000) > 0) && m_smVisionInfo.g_arrPad[1].ref_blnWantStandOff_Pad)
                {
                    pnl_SidePadStandOff.Visible = true;
                }
                else
                {
                    if (m_intSidePadStandOffFailCount == 0)
                    {
                        pnl_SidePadStandOff.Visible = false;
                    }
                }

                if (((intSideFailMask & 0x10000) > 0) && m_smVisionInfo.g_arrPad[1].ref_blnWantEdgeDistance_Pad)
                {
                    pnl_SidePadEdgeDistance.Visible = true;
                }
                else
                {
                    if (m_intSidePadEdgeDistanceFailCount == 0)
                    {
                        pnl_SidePadEdgeDistance.Visible = false;
                    }
                }

                if (((intSideFailMask & 0x20000) > 0 || (intSideFailMask & 0x40000) > 0) && m_smVisionInfo.g_arrPad[1].ref_blnWantSpan_Pad)
                {
                    pnl_SidePadSpan.Visible = true;
                }
                else
                {
                    if (m_intSidePadSpanFailCount == 0)
                    {
                        pnl_SidePadSpan.Visible = false;
                    }
                }

                //Side Color Defect
                if ((m_smCustomOption.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                {
                    pnl_SidePadColorDefect.Visible = false;
                }
                else
                {
                    if (m_smVisionInfo.g_arrPad[1].ref_intFailColorOptionMask > 0)
                    {
                        pnl_SidePadColorDefect.Visible = true;
                    }
                    else
                    {
                        if (m_intSidePadColorDefectFailCount == 0)
                        {
                            pnl_SidePadColorDefect.Visible = false;
                        }
                    }
                }

                if ((m_smCustomOption.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                {
                    pnl_CenterPkgDefect.Visible = false;
                    pnl_CenterPkgDimension.Visible = false;
                    pnl_SidePkgDefect.Visible = false;
                    pnl_SidePkgDimension.Visible = false;
                }
                else
                {
                    //Remove contamination if package is on
                    pnl_CenterPadContamination.Visible = false;
                    pnl_SidePadContamination.Visible = false;

                    if (m_smVisionInfo.g_arrPad[0].GetWantInspectPackage())
                    {
                        pnl_CenterPkgDefect.Visible = true;
                    }
                    else
                    {
                        if (m_intCenterPkgDefectFailCount == 0)
                        {
                            pnl_CenterPkgDefect.Visible = false;
                        }
                    }

                    if ((m_smVisionInfo.g_arrPad[0].ref_intFailPkgOptionMask & 0x01) > 0)
                    {
                        pnl_CenterPkgDimension.Visible = true;
                    }
                    else
                    {
                        if (m_intCenterPkgDimensionFailCount == 0)
                        {
                            pnl_CenterPkgDimension.Visible = false;
                        }
                    }

                    if (m_smVisionInfo.g_arrPad[1].GetWantInspectPackage())
                    {
                        pnl_SidePkgDefect.Visible = true;
                    }
                    else
                    {
                        if (m_intSidePkgDefectFailCount == 0)
                        {
                            pnl_SidePkgDefect.Visible = false;
                        }
                    }

                    if ((m_smVisionInfo.g_arrPad[1].ref_intFailPkgOptionMask & 0x01) > 0)
                    {
                        pnl_SidePkgDimension.Visible = true;
                    }
                    else
                    {
                        if (m_intSidePkgDimensionFailCount == 0)
                        {
                            pnl_SidePkgDimension.Visible = false;
                        }
                    }
                }

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

            if (m_smVisionInfo.g_blnWantGauge)
            {
                pnl_EdgeNotFound.Visible = true;
            }
            else
            {
                //only remove when fail count is 0, if fail count not 0 will remain
                if (m_intEdgeNotFoundFailCount == 0)
                {
                    pnl_EdgeNotFound.Visible = false;
                }
            }

            if (m_intCenterPadMissingFailCount != 0)
            {
                pnl_CenterPadMissing.Visible = true;
            }
            else
            {
                pnl_CenterPadMissing.Visible = false;
            }

            if (m_intSidePadMissingFailCount != 0)
            {
                pnl_SidePadMissing.Visible = true;
            }
            else
            {
                pnl_SidePadMissing.Visible = false;
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

            if (m_intEmptyUnitFailCount != 0)
            {
                pnl_EmptyUnit.Visible = true;
            }
            else
            {
                pnl_EmptyUnit.Visible = false;
            }

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
            //return;

            if ((m_smCustomOption.g_intWantOrient & (0x01 << m_smVisionInfo.g_intVisionPos)) == 0)
                return;

            string strRecipe = m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex];
            string strFilePath = m_smProductionInfo.g_strRecipePath + strRecipe +
                            "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Orient\\Template\\Template0.bmp";
            //string strFilePath = m_smProductionInfo.g_strRecipePath + strRecipe +
            //                "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Template\\OrientTemplate0.bmp";

            if (File.Exists(strFilePath))
            {
                FileStream fileStream = new FileStream(strFilePath, FileMode.Open, FileAccess.Read);
                pic_Template.Image = Image.FromStream(fileStream);
                fileStream.Close();

                pic_Template.Visible = true;
                //pic_Template.Load(strFilePath);
            }
            else
                pic_Template.Visible = false;
        }

        private void ResetCount()
        {
            //m_smVisionInfo.g_intPackageFailureTotal = 0;
            //m_smVisionInfo.g_intPadFailureTotal = 0;
            //m_smVisionInfo.g_intPin1FailureTotal = 0;
            //m_smVisionInfo.g_intPositionFailureTotal = 0;
            //m_smVisionInfo.g_intEmptyUnitFailureTotal = 0;

            //m_smVisionInfo.g_intPassTotal = 0;
            //m_smVisionInfo.g_intTestedTotal = 0;
            //m_smVisionInfo.g_intPassImageCount = 0;
            //m_smVisionInfo.g_intFailImageCount = 0;
            //m_smVisionInfo.g_intLowYieldUnitCount = 0;

            //m_smVisionInfo.VS_AT_UpdateQuantity = true;

            lbl_PassCount.Text = "0";
            lbl_TestedTotal.Text = "0";
            lbl_FailCount.Text = "0";

            lbl_OrientFailCount.Text = "0";
            lbl_PadFailCount.Text = "0";
            lbl_PositionFailCount.Text = "0";
            lbl_EmptyUnitFailCount.Text = "0";
            lbl_NoTemplateFailCount.Text = "0";
            lbl_EdgeNotFoundFailCount.Text = "0";
            lbl_Pin1FailCount.Text = "0";
            lbl_CenterPadOffsetFailCount.Text = "0";
            lbl_CenterPadAreaFailCount.Text = "0";
            lbl_CenterPadDimensionFailCount.Text = "0";
            lbl_CenterPadPitchGapFailCount.Text = "0";
            lbl_CenterPadBrokenFailCount.Text = "0";
            lbl_CenterPadExcessFailCount.Text = "0";
            lbl_CenterPadSmearFailCount.Text = "0";
            lbl_CenterPadEdgeLimitFailCount.Text = "0";
            lbl_CenterPadStandOffFailCount.Text = "0";
            lbl_CenterPadEdgeDistanceFailCount.Text = "0";
            lbl_CenterPadSpanFailCount.Text = "0";
            lbl_CenterPadContaminationFailCount.Text = "0";
            lbl_CenterPadMissingFailCount.Text = "0";
            lbl_CenterPadColorDefectFailCount.Text = "0";
            lbl_SidePadOffsetFailCount.Text = "0";
            lbl_SidePadAreaFailCount.Text = "0";
            lbl_SidePadDimensionFailCount.Text = "0";
            lbl_SidePadPitchGapFailCount.Text = "0";
            lbl_SidePadBrokenFailCount.Text = "0";
            lbl_SidePadExcessFailCount.Text = "0";
            lbl_SidePadSmearFailCount.Text = "0";
            lbl_SidePadEdgeLimitFailCount.Text = "0";
            lbl_SidePadStandOffFailCount.Text = "0";
            lbl_SidePadEdgeDistanceFailCount.Text = "0";
            lbl_SidePadSpanFailCount.Text = "0";
            lbl_SidePadContaminationFailCount.Text = "0";
            lbl_SidePadMissingFailCount.Text = "0";
            lbl_SidePadColorDefectFailCount.Text = "0";
            lbl_CenterPkgDefectFailCount.Text = "0";
            lbl_CenterPkgDimensionFailCount.Text = "0";
            lbl_SidePkgDefectFailCount.Text = "0";
            lbl_SidePkgDimensionFailCount.Text = "0";

            lbl_OrientFailPercent.Text = "0.00";
            lbl_PadFailPercent.Text = "0.00";
            lbl_PositionFailPercent.Text = "0.00";
            lbl_EmptyUnitFailPercent.Text = "0.00";
            lbl_NoTemplateFailPercent.Text = "0.00";
            lbl_EdgeNotFoundFailPercent.Text = "0.00";
            lbl_Pin1FailPercent.Text = "0.00";
            lbl_CenterPadOffsetFailPercent.Text = "0.00";
            lbl_CenterPadAreaFailPercent.Text = "0.00";
            lbl_CenterPadDimensionFailPercent.Text = "0.00";
            lbl_CenterPadPitchGapFailPercent.Text = "0.00";
            lbl_CenterPadBrokenFailPercent.Text = "0.00";
            lbl_CenterPadExcessFailPercent.Text = "0.00";
            lbl_CenterPadSmearFailPercent.Text = "0.00";
            lbl_CenterPadEdgeLimitFailPercent.Text = "0.00";
            lbl_CenterPadStandOffFailPercent.Text = "0.00";
            lbl_CenterPadEdgeDistanceFailPercent.Text = "0.00";
            lbl_CenterPadSpanFailPercent.Text = "0.00";
            lbl_CenterPadContaminationFailPercent.Text = "0.00";
            lbl_CenterPadMissingFailPercent.Text = "0.00";
            lbl_CenterPadColorDefectFailPercent.Text = "0.00";
            lbl_SidePadOffsetFailPercent.Text = "0.00";
            lbl_SidePadAreaFailPercent.Text = "0.00";
            lbl_SidePadDimensionFailPercent.Text = "0.00";
            lbl_SidePadPitchGapFailPercent.Text = "0.00";
            lbl_SidePadBrokenFailPercent.Text = "0.00";
            lbl_SidePadExcessFailPercent.Text = "0.00";
            lbl_SidePadSmearFailPercent.Text = "0.00";
            lbl_SidePadEdgeLimitFailPercent.Text = "0.00";
            lbl_SidePadStandOffFailPercent.Text = "0.00";
            lbl_SidePadEdgeDistanceFailPercent.Text = "0.00";
            lbl_SidePadSpanFailPercent.Text = "0.00";
            lbl_SidePadContaminationFailPercent.Text = "0.00";
            lbl_SidePadMissingFailPercent.Text = "0.00";
            lbl_SidePadColorDefectFailPercent.Text = "0.00";
            lbl_CenterPkgDefectFailPercent.Text = "0.00";
            lbl_CenterPkgDimensionFailPercent.Text = "0.00";
            lbl_SidePkgDefectFailPercent.Text = "0.00";
            lbl_SidePkgDimensionFailPercent.Text = "0.00";

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
                    case "Pass PH":
                        lbl_ResultStatus.BackColor = Color.Lime;
                        lbl_ResultStatus.Text = "Pass PH";
                        break;
                    case "Fail PH":
                        lbl_ResultStatus.BackColor = Color.Red;
                        lbl_ResultStatus.Text = "Fail PH";
                        break;
                }

                m_strResultPrev = m_strResult;
            }

            switch (m_smVisionInfo.g_intOrientResult[0])
            {
                case 0:
                    lblOrientationResult.BackColor = Color.Lime;
                    lblOrientationResult.Text = "0";
                    break;
                case 1:
                    lblOrientationResult.BackColor = Color.Lime;
                    lblOrientationResult.Text = "-90";
                    break;
                case 2:
                    lblOrientationResult.BackColor = Color.Lime;
                    lblOrientationResult.Text = "180";
                    break;
                case 3:
                    lblOrientationResult.BackColor = Color.Lime;
                    lblOrientationResult.Text = "90";
                    break;
                case 4:
                    lblOrientationResult.BackColor = Color.Red;
                    lblOrientationResult.Text = "Invalid";
                    break;
            }

            if (Convert.ToDouble(m_smVisionInfo.g_objPadOrient.ref_fMinScore * 100) > Convert.ToDouble(m_smVisionInfo.g_objPadOrient.GetMinScore() * 100))
            {
                lblOrientationResult.BackColor = Color.Red;
            }

            m_intPassCount = m_smVisionInfo.g_intPassTotal;
            if(m_intPassCount != m_intPassCountPrev)
            {
                lbl_PassCount.Text = m_intPassCount.ToString();
                m_intPassCountPrev = m_intPassCount;
            }

            m_intFailCount = Math.Max(0, m_smVisionInfo.g_intTestedTotal - m_smVisionInfo.g_intPassTotal);   // 2019 12 02 - CCENG: Add Max to prevent Fail count display -1. PassTotal will ++ first follow by TestedTotal ++. Sometime when machine run too fast, will get -0
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
            
            m_intOrientFailCount = m_smVisionInfo.g_intOrientFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fOrientFailPercent = (m_intOrientFailCount / (float)m_intTestedTotal) * 100;
                if (fOrientFailPercent > 100)
                    fOrientFailPercent = 100;
                lbl_OrientFailPercent.Text = fOrientFailPercent.ToString("f2");
            }
            else
                lbl_OrientFailPercent.Text = "0.00";

            if (m_intOrientFailCount != m_intOrientFailCountPrev)
            {
                lbl_OrientFailCount.BackColor = Color.Red;
                lbl_OrientFailPercent.BackColor = Color.Red;
                lbl_OrientFailCount.Text = m_intOrientFailCount.ToString();
                m_intOrientFailCountPrev = m_intOrientFailCount;
            }
            else
            {
                lbl_OrientFailCount.BackColor = Color.White;
                lbl_OrientFailPercent.BackColor = Color.White;
            }
            
            m_intPadFailCount = m_smVisionInfo.g_intPadFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fPadFailPercent = (m_intPadFailCount / (float)m_intTestedTotal) * 100;
                if (fPadFailPercent > 100)
                    fPadFailPercent = 100;
                lbl_PadFailPercent.Text = fPadFailPercent.ToString("f2");
            }
            else
                lbl_PadFailPercent.Text = "0.00";

            if (m_intPadFailCount != m_intPadFailCountPrev)
            {
                lbl_PadFailCount.BackColor = Color.Red;
                lbl_PadFailPercent.BackColor = Color.Red;
                lbl_PadFailCount.Text = m_intPadFailCount.ToString();
                m_intPadFailCountPrev = m_intPadFailCount;
            }
            else
            {
                lbl_PadFailCount.BackColor = Color.White;
                lbl_PadFailPercent.BackColor = Color.White;
            }

            m_intCenterPadOffsetFailCount = m_smVisionInfo.g_intCenterPadOffsetFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fCenterPadOffsetFailPercent = (m_intCenterPadOffsetFailCount / (float)m_intTestedTotal) * 100;
                if (fCenterPadOffsetFailPercent > 100)
                    fCenterPadOffsetFailPercent = 100;
                lbl_CenterPadOffsetFailPercent.Text = fCenterPadOffsetFailPercent.ToString("f2");
            }
            else
                lbl_CenterPadOffsetFailPercent.Text = "0.00";

            if (m_intCenterPadOffsetFailCount != m_intCenterPadOffsetFailCountPrev)
            {
                lbl_CenterPadOffsetFailCount.BackColor = Color.Red;
                lbl_CenterPadOffsetFailPercent.BackColor = Color.Red;
                lbl_CenterPadOffsetFailCount.Text = m_intCenterPadOffsetFailCount.ToString();
                m_intCenterPadOffsetFailCountPrev = m_intCenterPadOffsetFailCount;
            }
            else
            {
                lbl_CenterPadOffsetFailCount.BackColor = Color.White;
                lbl_CenterPadOffsetFailPercent.BackColor = Color.White;
            }

            m_intCenterPadAreaFailCount = m_smVisionInfo.g_intCenterPadAreaFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fCenterPadAreaFailPercent = (m_intCenterPadAreaFailCount / (float)m_intTestedTotal) * 100;
                if (fCenterPadAreaFailPercent > 100)
                    fCenterPadAreaFailPercent = 100;
                lbl_CenterPadAreaFailPercent.Text = fCenterPadAreaFailPercent.ToString("f2");
            }
            else
                lbl_CenterPadAreaFailPercent.Text = "0.00";

            if (m_intCenterPadAreaFailCount != m_intCenterPadAreaFailCountPrev)
            {
                lbl_CenterPadAreaFailCount.BackColor = Color.Red;
                lbl_CenterPadAreaFailPercent.BackColor = Color.Red;
                lbl_CenterPadAreaFailCount.Text = m_intCenterPadAreaFailCount.ToString();
                m_intCenterPadAreaFailCountPrev = m_intCenterPadAreaFailCount;
            }
            else
            {
                lbl_CenterPadAreaFailCount.BackColor = Color.White;
                lbl_CenterPadAreaFailPercent.BackColor = Color.White;
            }

            m_intCenterPadDimensionFailCount = m_smVisionInfo.g_intCenterPadDimensionFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fCenterPadDimensionFailPercent = (m_intCenterPadDimensionFailCount / (float)m_intTestedTotal) * 100;
                if (fCenterPadDimensionFailPercent > 100)
                    fCenterPadDimensionFailPercent = 100;
                lbl_CenterPadDimensionFailPercent.Text = fCenterPadDimensionFailPercent.ToString("f2");
            }
            else
                lbl_CenterPadDimensionFailPercent.Text = "0.00";

            if (m_intCenterPadDimensionFailCount != m_intCenterPadDimensionFailCountPrev)
            {
                lbl_CenterPadDimensionFailCount.BackColor = Color.Red;
                lbl_CenterPadDimensionFailPercent.BackColor = Color.Red;
                lbl_CenterPadDimensionFailCount.Text = m_intCenterPadDimensionFailCount.ToString();
                m_intCenterPadDimensionFailCountPrev = m_intCenterPadDimensionFailCount;
            }
            else
            {
                lbl_CenterPadDimensionFailCount.BackColor = Color.White;
                lbl_CenterPadDimensionFailPercent.BackColor = Color.White;
            }

            m_intCenterPadPitchGapFailCount = m_smVisionInfo.g_intCenterPadPitchGapFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fCenterPadPitchGapFailPercent = (m_intCenterPadPitchGapFailCount / (float)m_intTestedTotal) * 100;
                if (fCenterPadPitchGapFailPercent > 100)
                    fCenterPadPitchGapFailPercent = 100;
                lbl_CenterPadPitchGapFailPercent.Text = fCenterPadPitchGapFailPercent.ToString("f2");
            }
            else
                lbl_CenterPadPitchGapFailPercent.Text = "0.00";

            if (m_intCenterPadPitchGapFailCount != m_intCenterPadPitchGapFailCountPrev)
            {
                lbl_CenterPadPitchGapFailCount.BackColor = Color.Red;
                lbl_CenterPadPitchGapFailPercent.BackColor = Color.Red;
                lbl_CenterPadPitchGapFailCount.Text = m_intCenterPadPitchGapFailCount.ToString();
                m_intCenterPadPitchGapFailCountPrev = m_intCenterPadPitchGapFailCount;
            }
            else
            {
                lbl_CenterPadPitchGapFailCount.BackColor = Color.White;
                lbl_CenterPadPitchGapFailPercent.BackColor = Color.White;
            }

            m_intCenterPadBrokenFailCount = m_smVisionInfo.g_intCenterPadBrokenFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fCenterPadBrokenFailPercent = (m_intCenterPadBrokenFailCount / (float)m_intTestedTotal) * 100;
                if (fCenterPadBrokenFailPercent > 100)
                    fCenterPadBrokenFailPercent = 100;
                lbl_CenterPadBrokenFailPercent.Text = fCenterPadBrokenFailPercent.ToString("f2");
            }
            else
                lbl_CenterPadBrokenFailPercent.Text = "0.00";

            if (m_intCenterPadBrokenFailCount != m_intCenterPadBrokenFailCountPrev)
            {
                lbl_CenterPadBrokenFailCount.BackColor = Color.Red;
                lbl_CenterPadBrokenFailPercent.BackColor = Color.Red;
                lbl_CenterPadBrokenFailCount.Text = m_intCenterPadBrokenFailCount.ToString();
                m_intCenterPadBrokenFailCountPrev = m_intCenterPadBrokenFailCount;
            }
            else
            {
                lbl_CenterPadBrokenFailCount.BackColor = Color.White;
                lbl_CenterPadBrokenFailPercent.BackColor = Color.White;
            }

            m_intCenterPadExcessFailCount = m_smVisionInfo.g_intCenterPadExcessFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fCenterPadExcessFailPercent = (m_intCenterPadExcessFailCount / (float)m_intTestedTotal) * 100;
                if (fCenterPadExcessFailPercent > 100)
                    fCenterPadExcessFailPercent = 100;
                lbl_CenterPadExcessFailPercent.Text = fCenterPadExcessFailPercent.ToString("f2");
            }
            else
                lbl_CenterPadExcessFailPercent.Text = "0.00";

            if (m_intCenterPadExcessFailCount != m_intCenterPadExcessFailCountPrev)
            {
                lbl_CenterPadExcessFailCount.BackColor = Color.Red;
                lbl_CenterPadExcessFailPercent.BackColor = Color.Red;
                lbl_CenterPadExcessFailCount.Text = m_intCenterPadExcessFailCount.ToString();
                m_intCenterPadExcessFailCountPrev = m_intCenterPadExcessFailCount;
            }
            else
            {
                lbl_CenterPadExcessFailCount.BackColor = Color.White;
                lbl_CenterPadExcessFailPercent.BackColor = Color.White;
            }

            m_intCenterPadSmearFailCount = m_smVisionInfo.g_intCenterPadSmearFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fCenterPadSmearFailPercent = (m_intCenterPadSmearFailCount / (float)m_intTestedTotal) * 100;
                if (fCenterPadSmearFailPercent > 100)
                    fCenterPadSmearFailPercent = 100;
                lbl_CenterPadSmearFailPercent.Text = fCenterPadSmearFailPercent.ToString("f2");
            }
            else
                lbl_CenterPadSmearFailPercent.Text = "0.00";

            if (m_intCenterPadSmearFailCount != m_intCenterPadSmearFailCountPrev)
            {
                lbl_CenterPadSmearFailCount.BackColor = Color.Red;
                lbl_CenterPadSmearFailPercent.BackColor = Color.Red;
                lbl_CenterPadSmearFailCount.Text = m_intCenterPadSmearFailCount.ToString();
                m_intCenterPadSmearFailCountPrev = m_intCenterPadSmearFailCount;
            }
            else
            {
                lbl_CenterPadSmearFailCount.BackColor = Color.White;
                lbl_CenterPadSmearFailPercent.BackColor = Color.White;
            }

            m_intCenterPadEdgeLimitFailCount = m_smVisionInfo.g_intCenterPadEdgeLimitFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fCenterPadEdgeLimitFailPercent = (m_intCenterPadEdgeLimitFailCount / (float)m_intTestedTotal) * 100;
                if (fCenterPadEdgeLimitFailPercent > 100)
                    fCenterPadEdgeLimitFailPercent = 100;
                lbl_CenterPadEdgeLimitFailPercent.Text = fCenterPadEdgeLimitFailPercent.ToString("f2");
            }
            else
                lbl_CenterPadEdgeLimitFailPercent.Text = "0.00";

            if (m_intCenterPadEdgeLimitFailCount != m_intCenterPadEdgeLimitFailCountPrev)
            {
                lbl_CenterPadEdgeLimitFailCount.BackColor = Color.Red;
                lbl_CenterPadEdgeLimitFailPercent.BackColor = Color.Red;
                lbl_CenterPadEdgeLimitFailCount.Text = m_intCenterPadEdgeLimitFailCount.ToString();
                m_intCenterPadEdgeLimitFailCountPrev = m_intCenterPadEdgeLimitFailCount;
            }
            else
            {
                lbl_CenterPadEdgeLimitFailCount.BackColor = Color.White;
                lbl_CenterPadEdgeLimitFailPercent.BackColor = Color.White;
            }

            m_intCenterPadStandOffFailCount = m_smVisionInfo.g_intCenterPadStandOffFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fCenterPadStandOffFailPercent = (m_intCenterPadStandOffFailCount / (float)m_intTestedTotal) * 100;
                if (fCenterPadStandOffFailPercent > 100)
                    fCenterPadStandOffFailPercent = 100;
                lbl_CenterPadStandOffFailPercent.Text = fCenterPadStandOffFailPercent.ToString("f2");
            }
            else
                lbl_CenterPadStandOffFailPercent.Text = "0.00";

            if (m_intCenterPadStandOffFailCount != m_intCenterPadStandOffFailCountPrev)
            {
                lbl_CenterPadStandOffFailCount.BackColor = Color.Red;
                lbl_CenterPadStandOffFailPercent.BackColor = Color.Red;
                lbl_CenterPadStandOffFailCount.Text = m_intCenterPadStandOffFailCount.ToString();
                m_intCenterPadStandOffFailCountPrev = m_intCenterPadStandOffFailCount;
            }
            else
            {
                lbl_CenterPadStandOffFailCount.BackColor = Color.White;
                lbl_CenterPadStandOffFailPercent.BackColor = Color.White;
            }

            m_intCenterPadEdgeDistanceFailCount = m_smVisionInfo.g_intCenterPadEdgeDistanceFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fCenterPadEdgeDistanceFailPercent = (m_intCenterPadEdgeDistanceFailCount / (float)m_intTestedTotal) * 100;
                if (fCenterPadEdgeDistanceFailPercent > 100)
                    fCenterPadEdgeDistanceFailPercent = 100;
                lbl_CenterPadEdgeDistanceFailPercent.Text = fCenterPadEdgeDistanceFailPercent.ToString("f2");
            }
            else
                lbl_CenterPadEdgeDistanceFailPercent.Text = "0.00";

            if (m_intCenterPadEdgeDistanceFailCount != m_intCenterPadEdgeDistanceFailCountPrev)
            {
                lbl_CenterPadEdgeDistanceFailCount.BackColor = Color.Red;
                lbl_CenterPadEdgeDistanceFailPercent.BackColor = Color.Red;
                lbl_CenterPadEdgeDistanceFailCount.Text = m_intCenterPadEdgeDistanceFailCount.ToString();
                m_intCenterPadEdgeDistanceFailCountPrev = m_intCenterPadEdgeDistanceFailCount;
            }
            else
            {
                lbl_CenterPadEdgeDistanceFailCount.BackColor = Color.White;
                lbl_CenterPadEdgeDistanceFailPercent.BackColor = Color.White;
            }

            m_intCenterPadSpanFailCount = m_smVisionInfo.g_intCenterPadSpanFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fCenterPadSpanFailPercent = (m_intCenterPadSpanFailCount / (float)m_intTestedTotal) * 100;
                if (fCenterPadSpanFailPercent > 100)
                    fCenterPadSpanFailPercent = 100;
                lbl_CenterPadSpanFailPercent.Text = fCenterPadSpanFailPercent.ToString("f2");
            }
            else
                lbl_CenterPadSpanFailPercent.Text = "0.00";

            if (m_intCenterPadSpanFailCount != m_intCenterPadSpanFailCountPrev)
            {
                lbl_CenterPadSpanFailCount.BackColor = Color.Red;
                lbl_CenterPadSpanFailPercent.BackColor = Color.Red;
                lbl_CenterPadSpanFailCount.Text = m_intCenterPadSpanFailCount.ToString();
                m_intCenterPadSpanFailCountPrev = m_intCenterPadSpanFailCount;
            }
            else
            {
                lbl_CenterPadSpanFailCount.BackColor = Color.White;
                lbl_CenterPadSpanFailPercent.BackColor = Color.White;
            }

            m_intCenterPadContaminationFailCount = m_smVisionInfo.g_intCenterPadContaminationFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fCenterPadContaminationFailPercent = (m_intCenterPadContaminationFailCount / (float)m_intTestedTotal) * 100;
                if (fCenterPadContaminationFailPercent > 100)
                    fCenterPadContaminationFailPercent = 100;
                lbl_CenterPadContaminationFailPercent.Text = fCenterPadContaminationFailPercent.ToString("f2");
            }
            else
                lbl_CenterPadContaminationFailPercent.Text = "0.00";

            if (m_intCenterPadContaminationFailCount != m_intCenterPadContaminationFailCountPrev)
            {
                lbl_CenterPadContaminationFailCount.BackColor = Color.Red;
                lbl_CenterPadContaminationFailPercent.BackColor = Color.Red;
                lbl_CenterPadContaminationFailCount.Text = m_intCenterPadContaminationFailCount.ToString();
                m_intCenterPadContaminationFailCountPrev = m_intCenterPadContaminationFailCount;
            }
            else
            {
                lbl_CenterPadContaminationFailCount.BackColor = Color.White;
                lbl_CenterPadContaminationFailPercent.BackColor = Color.White;
            }

            m_intSidePadOffsetFailCount = m_smVisionInfo.g_intSidePadOffsetFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fSidePadOffsetFailPercent = (m_intSidePadOffsetFailCount / (float)m_intTestedTotal) * 100;
                if (fSidePadOffsetFailPercent > 100)
                    fSidePadOffsetFailPercent = 100;
                lbl_SidePadOffsetFailPercent.Text = fSidePadOffsetFailPercent.ToString("f2");
            }
            else
                lbl_SidePadOffsetFailPercent.Text = "0.00";

            if (m_intSidePadOffsetFailCount != m_intSidePadOffsetFailCountPrev)
            {
                lbl_SidePadOffsetFailCount.BackColor = Color.Red;
                lbl_SidePadOffsetFailPercent.BackColor = Color.Red;
                lbl_SidePadOffsetFailCount.Text = m_intSidePadOffsetFailCount.ToString();
                m_intSidePadOffsetFailCountPrev = m_intSidePadOffsetFailCount;
            }
            else
            {
                lbl_SidePadOffsetFailCount.BackColor = Color.White;
                lbl_SidePadOffsetFailPercent.BackColor = Color.White;
            }

            m_intSidePadAreaFailCount = m_smVisionInfo.g_intSidePadAreaFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fSidePadAreaFailPercent = (m_intSidePadAreaFailCount / (float)m_intTestedTotal) * 100;
                if (fSidePadAreaFailPercent > 100)
                    fSidePadAreaFailPercent = 100;
                lbl_SidePadAreaFailPercent.Text = fSidePadAreaFailPercent.ToString("f2");
            }
            else
                lbl_SidePadAreaFailPercent.Text = "0.00";

            if (m_intSidePadAreaFailCount != m_intSidePadAreaFailCountPrev)
            {
                lbl_SidePadAreaFailCount.BackColor = Color.Red;
                lbl_SidePadAreaFailPercent.BackColor = Color.Red;
                lbl_SidePadAreaFailCount.Text = m_intSidePadAreaFailCount.ToString();
                m_intSidePadAreaFailCountPrev = m_intSidePadAreaFailCount;
            }
            else
            {
                lbl_SidePadAreaFailCount.BackColor = Color.White;
                lbl_SidePadAreaFailPercent.BackColor = Color.White;
            }

            m_intSidePadDimensionFailCount = m_smVisionInfo.g_intSidePadDimensionFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fSidePadDimensionFailPercent = (m_intSidePadDimensionFailCount / (float)m_intTestedTotal) * 100;
                if (fSidePadDimensionFailPercent > 100)
                    fSidePadDimensionFailPercent = 100;
                lbl_SidePadDimensionFailPercent.Text = fSidePadDimensionFailPercent.ToString("f2");
            }
            else
                lbl_SidePadDimensionFailPercent.Text = "0.00";

            if (m_intSidePadDimensionFailCount != m_intSidePadDimensionFailCountPrev)
            {
                lbl_SidePadDimensionFailCount.BackColor = Color.Red;
                lbl_SidePadDimensionFailPercent.BackColor = Color.Red;
                lbl_SidePadDimensionFailCount.Text = m_intSidePadDimensionFailCount.ToString();
                m_intSidePadDimensionFailCountPrev = m_intSidePadDimensionFailCount;
            }
            else
            {
                lbl_SidePadDimensionFailCount.BackColor = Color.White;
                lbl_SidePadDimensionFailPercent.BackColor = Color.White;
            }

            m_intSidePadPitchGapFailCount = m_smVisionInfo.g_intSidePadPitchGapFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fSidePadPitchGapFailPercent = (m_intSidePadPitchGapFailCount / (float)m_intTestedTotal) * 100;
                if (fSidePadPitchGapFailPercent > 100)
                    fSidePadPitchGapFailPercent = 100;
                lbl_SidePadPitchGapFailPercent.Text = fSidePadPitchGapFailPercent.ToString("f2");
            }
            else
                lbl_SidePadPitchGapFailPercent.Text = "0.00";

            if (m_intSidePadPitchGapFailCount != m_intSidePadPitchGapFailCountPrev)
            {
                lbl_SidePadPitchGapFailCount.BackColor = Color.Red;
                lbl_SidePadPitchGapFailPercent.BackColor = Color.Red;
                lbl_SidePadPitchGapFailCount.Text = m_intSidePadPitchGapFailCount.ToString();
                m_intSidePadPitchGapFailCountPrev = m_intSidePadPitchGapFailCount;
            }
            else
            {
                lbl_SidePadPitchGapFailCount.BackColor = Color.White;
                lbl_SidePadPitchGapFailPercent.BackColor = Color.White;
            }

            m_intSidePadBrokenFailCount = m_smVisionInfo.g_intSidePadBrokenFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fSidePadBrokenFailPercent = (m_intSidePadBrokenFailCount / (float)m_intTestedTotal) * 100;
                if (fSidePadBrokenFailPercent > 100)
                    fSidePadBrokenFailPercent = 100;
                lbl_SidePadBrokenFailPercent.Text = fSidePadBrokenFailPercent.ToString("f2");
            }
            else
                lbl_SidePadBrokenFailPercent.Text = "0.00";

            if (m_intSidePadBrokenFailCount != m_intSidePadBrokenFailCountPrev)
            {
                lbl_SidePadBrokenFailCount.BackColor = Color.Red;
                lbl_SidePadBrokenFailPercent.BackColor = Color.Red;
                lbl_SidePadBrokenFailCount.Text = m_intSidePadBrokenFailCount.ToString();
                m_intSidePadBrokenFailCountPrev = m_intSidePadBrokenFailCount;
            }
            else
            {
                lbl_SidePadBrokenFailCount.BackColor = Color.White;
                lbl_SidePadBrokenFailPercent.BackColor = Color.White;
            }

            m_intSidePadExcessFailCount = m_smVisionInfo.g_intSidePadExcessFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fSidePadExcessFailPercent = (m_intSidePadExcessFailCount / (float)m_intTestedTotal) * 100;
                if (fSidePadExcessFailPercent > 100)
                    fSidePadExcessFailPercent = 100;
                lbl_SidePadExcessFailPercent.Text = fSidePadExcessFailPercent.ToString("f2");
            }
            else
                lbl_SidePadExcessFailPercent.Text = "0.00";

            if (m_intSidePadExcessFailCount != m_intSidePadExcessFailCountPrev)
            {
                lbl_SidePadExcessFailCount.BackColor = Color.Red;
                lbl_SidePadExcessFailPercent.BackColor = Color.Red;
                lbl_SidePadExcessFailCount.Text = m_intSidePadExcessFailCount.ToString();
                m_intSidePadExcessFailCountPrev = m_intSidePadExcessFailCount;
            }
            else
            {
                lbl_SidePadExcessFailCount.BackColor = Color.White;
                lbl_SidePadExcessFailPercent.BackColor = Color.White;
            }

            m_intSidePadSmearFailCount = m_smVisionInfo.g_intSidePadSmearFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fSidePadSmearFailPercent = (m_intSidePadSmearFailCount / (float)m_intTestedTotal) * 100;
                if (fSidePadSmearFailPercent > 100)
                    fSidePadSmearFailPercent = 100;
                lbl_SidePadSmearFailPercent.Text = fSidePadSmearFailPercent.ToString("f2");
            }
            else
                lbl_SidePadSmearFailPercent.Text = "0.00";

            if (m_intSidePadSmearFailCount != m_intSidePadSmearFailCountPrev)
            {
                lbl_SidePadSmearFailCount.BackColor = Color.Red;
                lbl_SidePadSmearFailPercent.BackColor = Color.Red;
                lbl_SidePadSmearFailCount.Text = m_intSidePadSmearFailCount.ToString();
                m_intSidePadSmearFailCountPrev = m_intSidePadSmearFailCount;
            }
            else
            {
                lbl_SidePadSmearFailCount.BackColor = Color.White;
                lbl_SidePadSmearFailPercent.BackColor = Color.White;
            }

            m_intSidePadEdgeLimitFailCount = m_smVisionInfo.g_intSidePadEdgeLimitFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fSidePadEdgeLimitFailPercent = (m_intSidePadEdgeLimitFailCount / (float)m_intTestedTotal) * 100;
                if (fSidePadEdgeLimitFailPercent > 100)
                    fSidePadEdgeLimitFailPercent = 100;
                lbl_SidePadEdgeLimitFailPercent.Text = fSidePadEdgeLimitFailPercent.ToString("f2");
            }
            else
                lbl_SidePadEdgeLimitFailPercent.Text = "0.00";

            if (m_intSidePadEdgeLimitFailCount != m_intSidePadEdgeLimitFailCountPrev)
            {
                lbl_SidePadEdgeLimitFailCount.BackColor = Color.Red;
                lbl_SidePadEdgeLimitFailPercent.BackColor = Color.Red;
                lbl_SidePadEdgeLimitFailCount.Text = m_intSidePadEdgeLimitFailCount.ToString();
                m_intSidePadEdgeLimitFailCountPrev = m_intSidePadEdgeLimitFailCount;
            }
            else
            {
                lbl_SidePadEdgeLimitFailCount.BackColor = Color.White;
                lbl_SidePadEdgeLimitFailPercent.BackColor = Color.White;
            }

            m_intSidePadStandOffFailCount = m_smVisionInfo.g_intSidePadStandOffFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fSidePadStandOffFailPercent = (m_intSidePadStandOffFailCount / (float)m_intTestedTotal) * 100;
                if (fSidePadStandOffFailPercent > 100)
                    fSidePadStandOffFailPercent = 100;
                lbl_SidePadStandOffFailPercent.Text = fSidePadStandOffFailPercent.ToString("f2");
            }
            else
                lbl_SidePadStandOffFailPercent.Text = "0.00";

            if (m_intSidePadStandOffFailCount != m_intSidePadStandOffFailCountPrev)
            {
                lbl_SidePadStandOffFailCount.BackColor = Color.Red;
                lbl_SidePadStandOffFailPercent.BackColor = Color.Red;
                lbl_SidePadStandOffFailCount.Text = m_intSidePadStandOffFailCount.ToString();
                m_intSidePadStandOffFailCountPrev = m_intSidePadStandOffFailCount;
            }
            else
            {
                lbl_SidePadStandOffFailCount.BackColor = Color.White;
                lbl_SidePadStandOffFailPercent.BackColor = Color.White;
            }

            m_intSidePadEdgeDistanceFailCount = m_smVisionInfo.g_intSidePadEdgeDistanceFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fSidePadEdgeDistanceFailPercent = (m_intSidePadEdgeDistanceFailCount / (float)m_intTestedTotal) * 100;
                if (fSidePadEdgeDistanceFailPercent > 100)
                    fSidePadEdgeDistanceFailPercent = 100;
                lbl_SidePadEdgeDistanceFailPercent.Text = fSidePadEdgeDistanceFailPercent.ToString("f2");
            }
            else
                lbl_SidePadEdgeDistanceFailPercent.Text = "0.00";

            if (m_intSidePadEdgeDistanceFailCount != m_intSidePadEdgeDistanceFailCountPrev)
            {
                lbl_SidePadEdgeDistanceFailCount.BackColor = Color.Red;
                lbl_SidePadEdgeDistanceFailPercent.BackColor = Color.Red;
                lbl_SidePadEdgeDistanceFailCount.Text = m_intSidePadEdgeDistanceFailCount.ToString();
                m_intSidePadEdgeDistanceFailCountPrev = m_intSidePadEdgeDistanceFailCount;
            }
            else
            {
                lbl_SidePadEdgeDistanceFailCount.BackColor = Color.White;
                lbl_SidePadEdgeDistanceFailPercent.BackColor = Color.White;
            }

            m_intSidePadSpanFailCount = m_smVisionInfo.g_intSidePadSpanFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fSidePadSpanFailPercent = (m_intSidePadSpanFailCount / (float)m_intTestedTotal) * 100;
                if (fSidePadSpanFailPercent > 100)
                    fSidePadSpanFailPercent = 100;
                lbl_SidePadSpanFailPercent.Text = fSidePadSpanFailPercent.ToString("f2");
            }
            else
                lbl_SidePadSpanFailPercent.Text = "0.00";

            if (m_intSidePadSpanFailCount != m_intSidePadSpanFailCountPrev)
            {
                lbl_SidePadSpanFailCount.BackColor = Color.Red;
                lbl_SidePadSpanFailPercent.BackColor = Color.Red;
                lbl_SidePadSpanFailCount.Text = m_intSidePadSpanFailCount.ToString();
                m_intSidePadSpanFailCountPrev = m_intSidePadSpanFailCount;
            }
            else
            {
                lbl_SidePadSpanFailCount.BackColor = Color.White;
                lbl_SidePadSpanFailPercent.BackColor = Color.White;
            }

            m_intSidePadContaminationFailCount = m_smVisionInfo.g_intSidePadContaminationFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fSidePadContaminationFailPercent = (m_intSidePadContaminationFailCount / (float)m_intTestedTotal) * 100;
                if (fSidePadContaminationFailPercent > 100)
                    fSidePadContaminationFailPercent = 100;
                lbl_SidePadContaminationFailPercent.Text = fSidePadContaminationFailPercent.ToString("f2");
            }
            else
                lbl_SidePadContaminationFailPercent.Text = "0.00";

            if (m_intSidePadContaminationFailCount != m_intSidePadContaminationFailCountPrev)
            {
                lbl_SidePadContaminationFailCount.BackColor = Color.Red;
                lbl_SidePadContaminationFailPercent.BackColor = Color.Red;
                lbl_SidePadContaminationFailCount.Text = m_intSidePadContaminationFailCount.ToString();
                m_intSidePadContaminationFailCountPrev = m_intSidePadContaminationFailCount;
            }
            else
            {
                lbl_SidePadContaminationFailCount.BackColor = Color.White;
                lbl_SidePadContaminationFailPercent.BackColor = Color.White;
            }

            m_intCenterPkgDefectFailCount = m_smVisionInfo.g_intCenterPkgDefectFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fCenterPkgDefectFailPercent = (m_intCenterPkgDefectFailCount / (float)m_intTestedTotal) * 100;
                if (fCenterPkgDefectFailPercent > 100)
                    fCenterPkgDefectFailPercent = 100;
                lbl_CenterPkgDefectFailPercent.Text = fCenterPkgDefectFailPercent.ToString("f2");
            }
            else
                lbl_CenterPkgDefectFailPercent.Text = "0.00";

            if (m_intCenterPkgDefectFailCount != m_intCenterPkgDefectFailCountPrev)
            {
                lbl_CenterPkgDefectFailCount.BackColor = Color.Red;
                lbl_CenterPkgDefectFailPercent.BackColor = Color.Red;
                lbl_CenterPkgDefectFailCount.Text = m_intCenterPkgDefectFailCount.ToString();
                m_intCenterPkgDefectFailCountPrev = m_intCenterPkgDefectFailCount;
            }
            else
            {
                lbl_CenterPkgDefectFailCount.BackColor = Color.White;
                lbl_CenterPkgDefectFailPercent.BackColor = Color.White;
            }

            m_intCenterPkgDimensionFailCount = m_smVisionInfo.g_intCenterPkgDimensionFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fCenterPkgDimensionFailPercent = (m_intCenterPkgDimensionFailCount / (float)m_intTestedTotal) * 100;
                if (fCenterPkgDimensionFailPercent > 100)
                    fCenterPkgDimensionFailPercent = 100;
                lbl_CenterPkgDimensionFailPercent.Text = fCenterPkgDimensionFailPercent.ToString("f2");
            }
            else
                lbl_CenterPkgDimensionFailPercent.Text = "0.00";

            if (m_intCenterPkgDimensionFailCount != m_intCenterPkgDimensionFailCountPrev)
            {
                lbl_CenterPkgDimensionFailCount.BackColor = Color.Red;
                lbl_CenterPkgDimensionFailPercent.BackColor = Color.Red;
                lbl_CenterPkgDimensionFailCount.Text = m_intCenterPkgDimensionFailCount.ToString();
                m_intCenterPkgDimensionFailCountPrev = m_intCenterPkgDimensionFailCount;
            }
            else
            {
                lbl_CenterPkgDimensionFailCount.BackColor = Color.White;
                lbl_CenterPkgDimensionFailPercent.BackColor = Color.White;
            }

            m_intSidePkgDefectFailCount = m_smVisionInfo.g_intSidePkgDefectFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fSidePkgDefectFailPercent = (m_intSidePkgDefectFailCount / (float)m_intTestedTotal) * 100;
                if (fSidePkgDefectFailPercent > 100)
                    fSidePkgDefectFailPercent = 100;
                lbl_SidePkgDefectFailPercent.Text = fSidePkgDefectFailPercent.ToString("f2");
            }
            else
                lbl_SidePkgDefectFailPercent.Text = "0.00";

            if (m_intSidePkgDefectFailCount != m_intSidePkgDefectFailCountPrev)
            {
                lbl_SidePkgDefectFailCount.BackColor = Color.Red;
                lbl_SidePkgDefectFailPercent.BackColor = Color.Red;
                lbl_SidePkgDefectFailCount.Text = m_intSidePkgDefectFailCount.ToString();
                m_intSidePkgDefectFailCountPrev = m_intSidePkgDefectFailCount;
            }
            else
            {
                lbl_SidePkgDefectFailCount.BackColor = Color.White;
                lbl_SidePkgDefectFailPercent.BackColor = Color.White;
            }

            m_intSidePkgDimensionFailCount = m_smVisionInfo.g_intSidePkgDimensionFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fSidePkgDimensionFailPercent = (m_intSidePkgDimensionFailCount / (float)m_intTestedTotal) * 100;
                if (fSidePkgDimensionFailPercent > 100)
                    fSidePkgDimensionFailPercent = 100;
                lbl_SidePkgDimensionFailPercent.Text = fSidePkgDimensionFailPercent.ToString("f2");
            }
            else
                lbl_SidePkgDimensionFailPercent.Text = "0.00";

            if (m_intSidePkgDimensionFailCount != m_intSidePkgDimensionFailCountPrev)
            {
                lbl_SidePkgDimensionFailCount.BackColor = Color.Red;
                lbl_SidePkgDimensionFailPercent.BackColor = Color.Red;
                lbl_SidePkgDimensionFailCount.Text = m_intSidePkgDimensionFailCount.ToString();
                m_intSidePkgDimensionFailCountPrev = m_intSidePkgDimensionFailCount;
            }
            else
            {
                lbl_SidePkgDimensionFailCount.BackColor = Color.White;
                lbl_SidePkgDimensionFailPercent.BackColor = Color.White;
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

            m_intCenterPadMissingFailCount = m_smVisionInfo.g_intCenterPadMissingFailureTotal;
            if (m_intCenterPadMissingFailCount != 0)
            {
                pnl_CenterPadMissing.Visible = true;
            }
            else
            {
                pnl_CenterPadMissing.Visible = false;
            }

            if (m_intTestedTotal != 0)
            {
                float fCenterPadMissingFailPercent = (m_intCenterPadMissingFailCount / (float)m_intTestedTotal) * 100;
                if (fCenterPadMissingFailPercent > 100)
                    fCenterPadMissingFailPercent = 100;
                lbl_CenterPadMissingFailPercent.Text = fCenterPadMissingFailPercent.ToString("f2");
            }
            else
                lbl_CenterPadMissingFailPercent.Text = "0.00";

            if (m_intCenterPadMissingFailCount != m_intCenterPadMissingFailCountPrev)
            {
                lbl_CenterPadMissingFailCount.BackColor = Color.Red;
                lbl_CenterPadMissingFailPercent.BackColor = Color.Red;
                lbl_CenterPadMissingFailCount.Text = m_intCenterPadMissingFailCount.ToString();
                m_intCenterPadMissingFailCountPrev = m_intCenterPadMissingFailCount;
            }
            else
            {
                lbl_CenterPadMissingFailCount.BackColor = Color.White;
                lbl_CenterPadMissingFailPercent.BackColor = Color.White;
            }

            m_intSidePadMissingFailCount = m_smVisionInfo.g_intSidePadMissingFailureTotal;
            if (m_intSidePadMissingFailCount != 0)
            {
                pnl_SidePadMissing.Visible = true;
            }
            else
            {
                pnl_SidePadMissing.Visible = false;
            }

            if (m_intTestedTotal != 0)
            {
                float fSidePadMissingFailPercent = (m_intSidePadMissingFailCount / (float)m_intTestedTotal) * 100;
                if (fSidePadMissingFailPercent > 100)
                    fSidePadMissingFailPercent = 100;
                lbl_SidePadMissingFailPercent.Text = fSidePadMissingFailPercent.ToString("f2");
            }
            else
                lbl_SidePadMissingFailPercent.Text = "0.00";

            if (m_intSidePadMissingFailCount != m_intSidePadMissingFailCountPrev)
            {
                lbl_SidePadMissingFailCount.BackColor = Color.Red;
                lbl_SidePadMissingFailPercent.BackColor = Color.Red;
                lbl_SidePadMissingFailCount.Text = m_intSidePadMissingFailCount.ToString();
                m_intSidePadMissingFailCountPrev = m_intSidePadMissingFailCount;
            }
            else
            {
                lbl_SidePadMissingFailCount.BackColor = Color.White;
                lbl_SidePadMissingFailPercent.BackColor = Color.White;
            }

            if ((m_smCustomOption.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {

                m_intCenterPadColorDefectFailCount = m_smVisionInfo.g_intCenterPadColorDefectFailureTotal;
                if (m_intCenterPadColorDefectFailCount != 0)
                {
                    pnl_CenterPadColorDefect.Visible = true;
                }
                else
                {
                    pnl_CenterPadColorDefect.Visible = false;
                }

                if (m_intTestedTotal != 0)
                {
                    float fCenterPadColorDefectFailPercent = (m_intCenterPadColorDefectFailCount / (float)m_intTestedTotal) * 100;
                    if (fCenterPadColorDefectFailPercent > 100)
                        fCenterPadColorDefectFailPercent = 100;
                    lbl_CenterPadColorDefectFailPercent.Text = fCenterPadColorDefectFailPercent.ToString("f2");
                }
                else
                    lbl_CenterPadColorDefectFailPercent.Text = "0.00";

                if (m_intCenterPadColorDefectFailCount != m_intCenterPadColorDefectFailCountPrev)
                {
                    lbl_CenterPadColorDefectFailCount.BackColor = Color.Red;
                    lbl_CenterPadColorDefectFailPercent.BackColor = Color.Red;
                    lbl_CenterPadColorDefectFailCount.Text = m_intCenterPadColorDefectFailCount.ToString();
                    m_intCenterPadColorDefectFailCountPrev = m_intCenterPadColorDefectFailCount;
                }
                else
                {
                    lbl_CenterPadColorDefectFailCount.BackColor = Color.White;
                    lbl_CenterPadColorDefectFailPercent.BackColor = Color.White;
                }

                m_intSidePadColorDefectFailCount = m_smVisionInfo.g_intSidePadColorDefectFailureTotal;
                if (m_intSidePadColorDefectFailCount != 0)
                {
                    pnl_SidePadColorDefect.Visible = true;
                }
                else
                {
                    pnl_SidePadColorDefect.Visible = false;
                }

                if (m_intTestedTotal != 0)
                {
                    float fSidePadColorDefectFailPercent = (m_intSidePadColorDefectFailCount / (float)m_intTestedTotal) * 100;
                    if (fSidePadColorDefectFailPercent > 100)
                        fSidePadColorDefectFailPercent = 100;
                    lbl_SidePadColorDefectFailPercent.Text = fSidePadColorDefectFailPercent.ToString("f2");
                }
                else
                    lbl_SidePadColorDefectFailPercent.Text = "0.00";

                if (m_intSidePadColorDefectFailCount != m_intSidePadColorDefectFailCountPrev)
                {
                    lbl_SidePadColorDefectFailCount.BackColor = Color.Red;
                    lbl_SidePadColorDefectFailPercent.BackColor = Color.Red;
                    lbl_SidePadColorDefectFailCount.Text = m_intSidePadColorDefectFailCount.ToString();
                    m_intSidePadColorDefectFailCountPrev = m_intSidePadColorDefectFailCount;
                }
                else
                {
                    lbl_SidePadColorDefectFailCount.BackColor = Color.White;
                    lbl_SidePadColorDefectFailPercent.BackColor = Color.White;
                }

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