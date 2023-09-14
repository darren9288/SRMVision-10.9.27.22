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
    public partial class Vision3OfflinePage : Form
    {
        #region Member Variables
        private bool[] m_blnFailPad = new bool[5] { false, false, false, false, false };
        private bool[] m_blnFailPackage = new bool[5] { false, false, false, false, false };
        private bool m_blnFailOrient;
        private bool m_blnFailPackageSize = false;
        private bool m_blnFailPin1 = false;
        private bool m_blnFailPosition = false;
        private bool m_blnFailPH = false;
        private bool m_blnInitDone = false;

        private DataGridView[] m_dgdView = new DataGridView[5];
        private DataGridView[] m_dgdDefectTable = new DataGridView[1];
        private DataGridView[] m_dgdPkgDefectTable = new DataGridView[1];

        private CustomOption m_smCustomOption;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo m_smVisionInfo;
        private int m_intUserGroup = 5;
        private int m_intPadIndex = 0;
        private int m_intSettingType = 0;
        private UserRight m_objUserRight = new UserRight();
        
        #endregion

        public Vision3OfflinePage(CustomOption smCustomOption, ProductionInfo smProductionInfo, VisionInfo smVisionInfo, int intUserGroup)
        {
            InitializeComponent();
            m_intUserGroup = intUserGroup;
            m_smCustomOption = smCustomOption;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            
            m_dgdView[0] = dgd_MiddlePad;
           
            m_dgdDefectTable[0] = dgd_MiddleDefect;
            m_dgdPkgDefectTable[0] = dgd_PkgDefect;

            m_smVisionInfo.g_intSelectedROI = 0;

            DisableField();
            CustomizeGUI();

            m_blnInitDone = true;
        }

        private void DisableField()
        {
            string strChild1 = "Test Page";
            string strChild2 = "";

            //strChild2 = "Inspect";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //    btn_Inspect.Enabled = false;
        }

        public void CustomizeGUI()
        {
            if (!m_blnInitDone)
            {
                lbl_ResultStatus.Text = "-----";
                lbl_ResultStatus.BackColor = Color.Gray;
            }

            if ((m_smCustomOption.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                UpdatePositionOrient(0);
            else
            {
                if (m_smVisionInfo.g_blnCheckPad)
                    UpdatePosition(0); //m_intPadIndex
            }

            if ((m_smCustomOption.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                if (tab_Result.TabPages.Contains(tp_Orient))
                    tab_Result.TabPages.Remove(tp_Orient);
            }

            if (m_smVisionInfo.g_blnWantCheckPH)
            {
                if (!tab_Result.TabPages.Contains(tp_PH))
                    tab_Result.Controls.Add(tp_PH);
            }
            else
            {
                if (tab_Result.TabPages.Contains(tp_PH))
                    tab_Result.Controls.Remove(tp_PH);
            }

            if (m_smVisionInfo.g_blnWantPin1 && m_smVisionInfo.g_arrPin1 != null && m_smVisionInfo.g_arrPin1.Count > 0 && m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(0))
            {
                if (!tab_Result.TabPages.Contains(tp_Pin1))
                    tab_Result.Controls.Add(tp_Pin1);

                UpdatePin1();
            }
            else
            {
                if (tab_Result.TabPages.Contains(tp_Pin1))
                    tab_Result.Controls.Remove(tp_Pin1);
            }    

            switch (m_smVisionInfo.g_intSelectedROI)
            {
                case 0:
                    m_intPadIndex = 0;
                    m_smVisionInfo.g_intSelectedROIMask = 0x01;
                    radioBtn_Middle.Checked = true;
                    break;
                case 1:
                    m_intPadIndex = 1;
                    m_smVisionInfo.g_intSelectedROIMask = 0x02;
                    radioBtn_Up.Checked = true;
                    break;
                case 2:
                    m_intPadIndex = 2;
                    m_smVisionInfo.g_intSelectedROIMask = 0x04;
                    radioBtn_Right.Checked = true;
                    break;
                case 3:
                    m_intPadIndex = 3;
                    m_smVisionInfo.g_intSelectedROIMask = 0x08;
                    radioBtn_Down.Checked = true;
                    break;
                case 4:
                    m_intPadIndex = 4;
                    m_smVisionInfo.g_intSelectedROIMask = 0x10;
                    radioBtn_Left.Checked = true;
                    break;
            }

            if (!m_smVisionInfo.g_blnCheck4Sides)
            {
                radioBtn_Down.Enabled = false;
                radioBtn_Left.Enabled = false;
                radioBtn_Right.Enabled = false;
                radioBtn_Up.Enabled = false;
            }
            else
            {
                radioBtn_Down.Enabled = true;
                radioBtn_Left.Enabled = true;
                radioBtn_Right.Enabled = true;
                radioBtn_Up.Enabled = true;
            }

            if (m_smVisionInfo.g_arrPad.Length == 1)
            {
                pnl_PadIndex.Visible = false;
            }

            ReadPadTemplateDataToGrid(m_intPadIndex, m_dgdView[0]);

            int intFailOptionMask = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intFailOptionMask;

            m_dgdView[0].Columns[0].Visible = ((intFailOptionMask & 0x100) > 0);
            m_dgdView[0].Columns[1].Visible = ((intFailOptionMask & 0x20) > 0);
            m_dgdView[0].Columns[2].Visible = ((intFailOptionMask & 0xC0) > 0);
            m_dgdView[0].Columns[3].Visible = ((intFailOptionMask & 0xC0) > 0);
            m_dgdView[0].Columns[4].Visible = ((intFailOptionMask & 0x600) > 0);
            m_dgdView[0].Columns[5].Visible = ((intFailOptionMask & 0x600) > 0);
            m_dgdView[0].Columns[6].Visible = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckBrokenPadArea;
            m_dgdView[0].Columns[7].Visible = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckBrokenPadLength;
            m_dgdView[0].Columns[8].Visible = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckExcessPadArea;
            m_dgdView[0].Columns[9].Visible = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckSmearPadLength;

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantEdgeLimit_Pad)
            {
                if ((intFailOptionMask & 0x4000) > 0)
                {
                    m_dgdView[0].Columns[10].Visible = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckPadEdgeLimit;
                    m_dgdView[0].Columns[11].Visible = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckPadEdgeLimit;
                    m_dgdView[0].Columns[12].Visible = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckPadEdgeLimit;
                    m_dgdView[0].Columns[13].Visible = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckPadEdgeLimit;
                }
                else
                {
                    m_dgdView[0].Columns[10].Visible = false;
                    m_dgdView[0].Columns[11].Visible = false;
                    m_dgdView[0].Columns[12].Visible = false;
                    m_dgdView[0].Columns[13].Visible = false;
                }
            }
            else
            {
                m_dgdView[0].Columns[10].Visible = false;
                m_dgdView[0].Columns[11].Visible = false;
                m_dgdView[0].Columns[12].Visible = false;
                m_dgdView[0].Columns[13].Visible = false;
            }

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantStandOff_Pad)
            {
                if ((intFailOptionMask & 0x8000) > 0)
                {
                    m_dgdView[0].Columns[14].Visible = m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(0);
                    m_dgdView[0].Columns[15].Visible = m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(1);
                    m_dgdView[0].Columns[16].Visible = m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(2);
                    m_dgdView[0].Columns[17].Visible = m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(3);
                }
                else
                {
                    m_dgdView[0].Columns[14].Visible = false;
                    m_dgdView[0].Columns[15].Visible = false;
                    m_dgdView[0].Columns[16].Visible = false;
                    m_dgdView[0].Columns[17].Visible = false;
                }
            }
            else
            {
                m_dgdView[0].Columns[14].Visible = false;
                m_dgdView[0].Columns[15].Visible = false;
                m_dgdView[0].Columns[16].Visible = false;
                m_dgdView[0].Columns[17].Visible = false;
            }

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantEdgeDistance_Pad)
            {
                if ((intFailOptionMask & 0x10000) > 0)
                {
                    m_dgdView[0].Columns[18].Visible = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckPadEdgeDistance;
                    m_dgdView[0].Columns[19].Visible = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckPadEdgeDistance;
                    m_dgdView[0].Columns[20].Visible = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckPadEdgeDistance;
                    m_dgdView[0].Columns[21].Visible = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckPadEdgeDistance;
                }
                else
                {
                    m_dgdView[0].Columns[18].Visible = false;
                    m_dgdView[0].Columns[19].Visible = false;
                    m_dgdView[0].Columns[20].Visible = false;
                    m_dgdView[0].Columns[21].Visible = false;
                }
            }
            else
            {
                m_dgdView[0].Columns[18].Visible = false;
                m_dgdView[0].Columns[19].Visible = false;
                m_dgdView[0].Columns[20].Visible = false;
                m_dgdView[0].Columns[21].Visible = false;
            }

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantSpan_Pad)
            {
                if ((intFailOptionMask & 0x20000) > 0 || (intFailOptionMask & 0x40000) > 0)
                {
                    dgd_Span.Visible = true;
                    if (dgd_Span.RowCount == 0)
                    {
                        dgd_Span.Rows.Add();
                        dgd_Span.Rows[0].Cells[0].Value = "Span X";
                        dgd_Span.Rows[0].Cells[0].Style.BackColor = Color.White;
                        dgd_Span.Rows[0].Cells[0].Style.SelectionBackColor = Color.White;
                        dgd_Span.Rows[0].Cells[1].Style.BackColor = Color.White;
                        dgd_Span.Rows[0].Cells[1].Style.SelectionBackColor = Color.White;
                    }
                    if (dgd_Span.RowCount == 1)
                    {
                        dgd_Span.Rows.Add();
                        dgd_Span.Rows[1].Cells[0].Value = "Span Y";
                        dgd_Span.Rows[1].Cells[0].Style.BackColor = Color.White;
                        dgd_Span.Rows[1].Cells[0].Style.SelectionBackColor = Color.White;
                        dgd_Span.Rows[1].Cells[1].Style.BackColor = Color.White;
                        dgd_Span.Rows[1].Cells[1].Style.SelectionBackColor = Color.White;
                    }
                    dgd_Span.Rows[0].Visible = (intFailOptionMask & 0x20000) > 0;
                    dgd_Span.Rows[1].Visible = (intFailOptionMask & 0x40000) > 0;
                }
                else
                {
                    dgd_Span.Visible = false;
                }
            }
            else
            {
                dgd_Span.Visible = false;
            }

            m_dgdView[0].Columns[22].Visible = (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 2);
            m_dgdView[0].Columns[23].Visible = (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 3);
            m_dgdView[0].Columns[24].Visible = (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 4);
            m_dgdView[0].Columns[25].Visible = (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 5);
            m_dgdView[0].Columns[26].Visible = (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 6);
            m_dgdView[0].Columns[27].Visible = (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 7);
            m_dgdView[0].Columns[28].Visible = (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 8);
            m_dgdView[0].Columns[29].Visible = (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 9);
            m_dgdView[0].Columns[30].Visible = (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 10);
            m_dgdView[0].Columns[31].Visible = (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 11);

            //---------------------------------------- Pad Package ----------------------------------------
            dgd_PadPackage.Rows.Clear();
            dgd_PkgDefect.Rows.Clear();
            if ((m_smVisionInfo.g_arrPadROIs.Count == 1) || ((m_smVisionInfo.g_arrPadROIs.Count == 5) && !m_smVisionInfo.g_blnCheck4Sides) || ((m_smVisionInfo.g_arrPad.Length > 1) && ((m_smVisionInfo.g_arrPad[1].ref_intFailPkgOptionMask & 0x01) == 0)))
            {
                dgd_PadPackage.Columns[3].Visible = false;

                dgd_PadPackage.Size = new Size(310, 74);
                dgd_PkgDefect.Location = new Point(0, 74);
                dgd_PkgDefect.Size = new Size(310, 243);
            }
            else
            {
                dgd_PadPackage.Columns[3].Visible = true;
                if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_arrPad[1].ref_blnWantIndividualSideThickness)
                {
                    dgd_PadPackage.Size = new Size(310, 135);
                    dgd_PkgDefect.Location = new Point(0, 135);
                    dgd_PkgDefect.Size = new Size(310, 182);
                }
                else
                {
                    dgd_PadPackage.Size = new Size(310, 74);
                    dgd_PkgDefect.Location = new Point(0, 74);
                    dgd_PkgDefect.Size = new Size(310, 243);
                }
            }

            tab_Result.Controls.Remove(tp_SidePad);

            // Set display events
            m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            // Reject: 2019 04 22 - CCENG: Hide datagrid table for Contamination Checking under Pad. because all inspection defect data will be displayed in package datagrid table.
            if (m_smVisionInfo.g_strVisionName == "Pad5SPkg" || m_smVisionInfo.g_strVisionName == "PadPkg")
            {
                // 2019 05 11 - CCENG: Should not hide dgd_MiddlePad even though no surface contamination checking under pad, but broken pad and excess pad will be showed in this table also.
                //dgd_MiddlePad.Size = new Size(310, 317);
                //dgd_MiddleDefect.Visible = false;
            }
            else if (m_smVisionInfo.g_strVisionName == "Pad5S" || m_smVisionInfo.g_strVisionName == "Pad" || m_smVisionInfo.g_strVisionName == "BottomOrientPad") // || m_smVisionInfo.g_strVisionName == "BottomOPadPkg")
            {
                tab_Result.Controls.Remove(tp_Package);
            }


        }

        private void UpdatePin1()
        {
            if (!(m_smVisionInfo.g_blnWantPin1 && m_smVisionInfo.g_arrPin1 != null && m_smVisionInfo.g_arrPin1.Count > 0 && m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(0)))
                return;

            dgd_Pin1Result.Rows.Clear();
            float fScore = m_smVisionInfo.g_arrPin1[0].GetResultScore(0);

            if (fScore < 0)
                return;

            dgd_Pin1Result.Rows.Add();
            dgd_Pin1Result.Rows[0].Cells[0].Value = "Result";
            dgd_Pin1Result.Rows[0].Cells[1].Value = (m_smVisionInfo.g_arrPin1[0].GetMinScoreSetting(0) * 100).ToString("f0");
            dgd_Pin1Result.Rows[0].Cells[2].Value = Math.Max(0, (fScore * 100)).ToString("f2");
            if (fScore >= 0 && fScore < m_smVisionInfo.g_arrPin1[0].GetMinScoreSetting(0))
            {
                m_blnFailPin1 = true;
                dgd_Pin1Result.Rows[0].Cells[1].Style.BackColor = Color.Red;
                dgd_Pin1Result.Rows[0].Cells[1].Style.SelectionBackColor = Color.Red;
                dgd_Pin1Result.Rows[0].Cells[1].Style.ForeColor = Color.Yellow;
                dgd_Pin1Result.Rows[0].Cells[1].Style.SelectionForeColor = Color.Yellow;
                dgd_Pin1Result.Rows[0].Cells[2].Style.BackColor = Color.Red;
                dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionBackColor = Color.Red;
                dgd_Pin1Result.Rows[0].Cells[2].Style.ForeColor = Color.Yellow;
                dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionForeColor = Color.Yellow;
            }
            else
            {
                dgd_Pin1Result.Rows[0].Cells[1].Style.BackColor = Color.Lime;
                dgd_Pin1Result.Rows[0].Cells[1].Style.SelectionBackColor = Color.Lime;
                dgd_Pin1Result.Rows[0].Cells[1].Style.ForeColor = Color.Black;
                dgd_Pin1Result.Rows[0].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_Pin1Result.Rows[0].Cells[2].Style.BackColor = Color.Lime;
                dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_Pin1Result.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
            }
            
        }


        private void UpdatePosition(int intPadIndex)
        {
            intPadIndex = 0;
            dgd_PositionResult.Rows.Clear();
            float Angle = 0, XTolerance = 0, YTolerance = 0;
            m_smVisionInfo.g_arrPad[intPadIndex].GetPositionResult(ref Angle, ref XTolerance, ref YTolerance);

            dgd_PositionResult.Rows.Add();
            dgd_PositionResult.Rows[0].Cells[0].Value = "Angle";
            dgd_PositionResult.Rows[0].Cells[1].Value = m_smVisionInfo.g_arrPad[intPadIndex].ref_fAngleTolerance.ToString("f4");
            dgd_PositionResult.Rows[0].Cells[2].Value = Angle.ToString("f4");
            if (Math.Abs(Angle) >= m_smVisionInfo.g_arrPad[intPadIndex].ref_fAngleTolerance)
            {
                if ((m_smCustomOption.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    if (m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientAngleTolerance)
                        m_blnFailPosition = true;
                }
                else
                    m_blnFailPosition = true;
                dgd_PositionResult.Rows[0].Cells[1].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[0].Cells[1].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[0].Cells[1].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[0].Cells[1].Style.SelectionForeColor = Color.Yellow;
                dgd_PositionResult.Rows[0].Cells[2].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[0].Cells[2].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Yellow;
            }
            else
            {
                dgd_PositionResult.Rows[0].Cells[1].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[0].Cells[1].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[0].Cells[1].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[0].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_PositionResult.Rows[0].Cells[2].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
            }
            dgd_PositionResult.Rows.Add();
            dgd_PositionResult.Rows[1].Cells[0].Value = "X Tol.(mm)";
            dgd_PositionResult.Rows[1].Cells[1].Value = m_smVisionInfo.g_arrPad[intPadIndex].ref_fXTolerance.ToString("f4");
            dgd_PositionResult.Rows[1].Cells[2].Value = XTolerance.ToString("f4");
            if (Math.Abs(XTolerance) >= m_smVisionInfo.g_arrPad[intPadIndex].ref_fXTolerance)
            {
                if ((m_smCustomOption.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    if (m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientXTolerance)
                        m_blnFailPosition = true;
                }
                else
                    m_blnFailPosition = true;
                dgd_PositionResult.Rows[1].Cells[1].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[1].Cells[1].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[1].Cells[1].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[1].Cells[1].Style.SelectionForeColor = Color.Yellow;
                dgd_PositionResult.Rows[1].Cells[2].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[1].Cells[2].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[1].Cells[2].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[1].Cells[2].Style.SelectionForeColor = Color.Yellow;
            }
            else
            {
                dgd_PositionResult.Rows[1].Cells[1].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[1].Cells[1].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[1].Cells[1].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[1].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_PositionResult.Rows[1].Cells[2].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[1].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[1].Cells[2].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[1].Cells[2].Style.SelectionForeColor = Color.Black;
            }
            dgd_PositionResult.Rows.Add();
            dgd_PositionResult.Rows[2].Cells[0].Value = "Y Tol.(mm)";
            dgd_PositionResult.Rows[2].Cells[1].Value = m_smVisionInfo.g_arrPad[intPadIndex].ref_fYTolerance.ToString("f4");
            dgd_PositionResult.Rows[2].Cells[2].Value = YTolerance.ToString("f4");
            if (Math.Abs(YTolerance) >= m_smVisionInfo.g_arrPad[intPadIndex].ref_fYTolerance)
            {
                if ((m_smCustomOption.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    if (m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientYTolerance)
                        m_blnFailPosition = true;
                }
                else
                    m_blnFailPosition = true;
                dgd_PositionResult.Rows[2].Cells[1].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[2].Cells[1].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[2].Cells[1].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[2].Cells[1].Style.SelectionForeColor = Color.Yellow;
                dgd_PositionResult.Rows[2].Cells[2].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[2].Cells[2].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[2].Cells[2].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[2].Cells[2].Style.SelectionForeColor = Color.Yellow;
            }
            else
            {
                dgd_PositionResult.Rows[2].Cells[1].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[2].Cells[1].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[2].Cells[1].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[2].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_PositionResult.Rows[2].Cells[2].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[2].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[2].Cells[2].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[2].Cells[2].Style.SelectionForeColor = Color.Black;
            }


            if (!m_smVisionInfo.g_arrPad[intPadIndex].ref_blnPadFound)
            {
                m_blnFailPosition = false;
                dgd_PositionResult.Rows[0].Cells[2].Value = "---";
                dgd_PositionResult.Rows[0].Cells[1].Style.BackColor = Color.White;// Lime
                dgd_PositionResult.Rows[0].Cells[1].Style.SelectionBackColor = Color.White;// Lime
                dgd_PositionResult.Rows[0].Cells[1].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[0].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_PositionResult.Rows[0].Cells[2].Style.BackColor = Color.White;// Lime
                dgd_PositionResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.White;// Lime
                dgd_PositionResult.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;

                dgd_PositionResult.Rows[1].Cells[2].Value = "---";
                dgd_PositionResult.Rows[1].Cells[1].Style.BackColor = Color.White;// Lime
                dgd_PositionResult.Rows[1].Cells[1].Style.SelectionBackColor = Color.White;// Lime
                dgd_PositionResult.Rows[1].Cells[1].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[1].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_PositionResult.Rows[1].Cells[2].Style.BackColor = Color.White;// Lime
                dgd_PositionResult.Rows[1].Cells[2].Style.SelectionBackColor = Color.White;// Lime
                dgd_PositionResult.Rows[1].Cells[2].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[1].Cells[2].Style.SelectionForeColor = Color.Black;

                dgd_PositionResult.Rows[2].Cells[2].Value = "---";
                dgd_PositionResult.Rows[2].Cells[1].Style.BackColor = Color.White;// Lime
                dgd_PositionResult.Rows[2].Cells[1].Style.SelectionBackColor = Color.White;// Lime
                dgd_PositionResult.Rows[2].Cells[1].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[2].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_PositionResult.Rows[2].Cells[2].Style.BackColor = Color.White;// Lime
                dgd_PositionResult.Rows[2].Cells[2].Style.SelectionBackColor = Color.White;// Lime
                dgd_PositionResult.Rows[2].Cells[2].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[2].Cells[2].Style.SelectionForeColor = Color.Black;
            }

            if ((m_smCustomOption.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                dgd_PositionResult.Rows[0].Visible = m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientAngleTolerance;
                dgd_PositionResult.Rows[1].Visible = m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientXTolerance;
                dgd_PositionResult.Rows[2].Visible = m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientYTolerance;
            }
        }
        private void UpdatePositionOrient(int intPadIndex)
        {
            intPadIndex = 0;
            dgd_PositionResult.Rows.Clear();
            float fAngleResult = Math.Abs(m_smVisionInfo.g_objPadOrient.ref_fDegAngleResult); //GetResultAngle()
            float CenterX = 0;
            float CenterY = 0;
            float fXAfterRotated = m_smVisionInfo.g_objPadOrient.ref_fTemplateX;
            float fYAfterRotated = m_smVisionInfo.g_objPadOrient.ref_fTemplateY;
            float fCenterXDiff = 0;
            float fCenterYDiff = 0;
            int intOrientAngle = 0;
            if (m_smVisionInfo.g_intOrientResult[1] < 4)
            {
                switch (m_smVisionInfo.g_intOrientResult[1])
                {
                    default:
                    case 0:
                        intOrientAngle = 0;
                        break;
                    case 1:
                        intOrientAngle = 90;
                        break;
                    case 2:
                        intOrientAngle = 180;
                        break;
                    case 3:
                        intOrientAngle = -90;
                        break;
                }

            }

            //CenterX = m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX + (float)(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIWidth) / 2;
            //CenterY = m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY + (float)(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIHeight) / 2;

            ////2020-09-24 ZJYEOH : Should use current angle to rotate template center point because when get center point different, the object center point is based on current angle
            //fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_objPadOrient.ref_fTemplateX - CenterX) * Math.Cos(intOrientAngle * Math.PI / 180)) - //m_smVisionInfo.g_objPadOrient.ref_intRotatedAngle
            //                   ((m_smVisionInfo.g_objPadOrient.ref_fTemplateY - CenterY) * Math.Sin(intOrientAngle * Math.PI / 180)));
            //fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_objPadOrient.ref_fTemplateX - CenterX) * Math.Sin(intOrientAngle * Math.PI / 180)) +
            //                    ((m_smVisionInfo.g_objPadOrient.ref_fTemplateY - CenterY) * Math.Cos(intOrientAngle * Math.PI / 180)));

            fCenterXDiff = m_smVisionInfo.g_objPadOrient.GetCenterXDiff(fXAfterRotated, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX);
            fCenterYDiff = m_smVisionInfo.g_objPadOrient.GetCenterYDiff(fYAfterRotated, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY);

            dgd_PositionResult.Rows.Add();
            dgd_PositionResult.Rows[0].Cells[0].Value = "Angle";
            dgd_PositionResult.Rows[0].Cells[1].Value = m_smVisionInfo.g_arrPad[intPadIndex].ref_fAngleTolerance.ToString("f4");
            dgd_PositionResult.Rows[0].Cells[2].Value = fAngleResult.ToString("f4");
            if (Math.Abs(fAngleResult) >= m_smVisionInfo.g_arrPad[intPadIndex].ref_fAngleTolerance)
            {
                if (m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientAngleTolerance)
                    m_blnFailPosition = true;
                dgd_PositionResult.Rows[0].Cells[1].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[0].Cells[1].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[0].Cells[1].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[0].Cells[1].Style.SelectionForeColor = Color.Yellow;
                dgd_PositionResult.Rows[0].Cells[2].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[0].Cells[2].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Yellow;
            }
            else
            {
                dgd_PositionResult.Rows[0].Cells[1].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[0].Cells[1].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[0].Cells[1].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[0].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_PositionResult.Rows[0].Cells[2].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
            }
            dgd_PositionResult.Rows.Add();
            dgd_PositionResult.Rows[1].Cells[0].Value = "X Tol.(mm)";
            dgd_PositionResult.Rows[1].Cells[1].Value = m_smVisionInfo.g_arrPad[intPadIndex].ref_fXTolerance.ToString("f4");
            dgd_PositionResult.Rows[1].Cells[2].Value = fCenterXDiff.ToString("f4");
            if (Math.Abs(fCenterXDiff) >= m_smVisionInfo.g_arrPad[intPadIndex].ref_fXTolerance)
            {
                if (m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientXTolerance)
                    m_blnFailPosition = true;
                dgd_PositionResult.Rows[1].Cells[1].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[1].Cells[1].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[1].Cells[1].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[1].Cells[1].Style.SelectionForeColor = Color.Yellow;
                dgd_PositionResult.Rows[1].Cells[2].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[1].Cells[2].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[1].Cells[2].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[1].Cells[2].Style.SelectionForeColor = Color.Yellow;
            }
            else
            {
                dgd_PositionResult.Rows[1].Cells[1].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[1].Cells[1].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[1].Cells[1].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[1].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_PositionResult.Rows[1].Cells[2].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[1].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[1].Cells[2].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[1].Cells[2].Style.SelectionForeColor = Color.Black;
            }
            dgd_PositionResult.Rows.Add();
            dgd_PositionResult.Rows[2].Cells[0].Value = "Y Tol.(mm)";
            dgd_PositionResult.Rows[2].Cells[1].Value = m_smVisionInfo.g_arrPad[intPadIndex].ref_fYTolerance.ToString("f4");
            dgd_PositionResult.Rows[2].Cells[2].Value = fCenterYDiff.ToString("f4");
            if (Math.Abs(fCenterYDiff) >= m_smVisionInfo.g_arrPad[intPadIndex].ref_fYTolerance)
            {
                if (m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientYTolerance)
                    m_blnFailPosition = true;
                dgd_PositionResult.Rows[2].Cells[1].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[2].Cells[1].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[2].Cells[1].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[2].Cells[1].Style.SelectionForeColor = Color.Yellow;
                dgd_PositionResult.Rows[2].Cells[2].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[2].Cells[2].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[2].Cells[2].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[2].Cells[2].Style.SelectionForeColor = Color.Yellow;
            }
            else
            {
                dgd_PositionResult.Rows[2].Cells[1].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[2].Cells[1].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[2].Cells[1].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[2].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_PositionResult.Rows[2].Cells[2].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[2].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[2].Cells[2].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[2].Cells[2].Style.SelectionForeColor = Color.Black;
            }


            if (dgd_OrientResult.Rows.Count == 0 || (Convert.ToDouble(m_smVisionInfo.g_objPadOrient.ref_fMinScore * 100) > Convert.ToDouble(dgd_OrientResult.Rows[0].Cells[2].Value)))
            {
                m_blnFailPosition = false;
                dgd_PositionResult.Rows[0].Cells[2].Value = "---";
                dgd_PositionResult.Rows[0].Cells[1].Style.BackColor = Color.White;//Lime
                dgd_PositionResult.Rows[0].Cells[1].Style.SelectionBackColor = Color.White;//Lime
                dgd_PositionResult.Rows[0].Cells[1].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[0].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_PositionResult.Rows[0].Cells[2].Style.BackColor = Color.White;//Lime
                dgd_PositionResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.White;//Lime
                dgd_PositionResult.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;

                dgd_PositionResult.Rows[1].Cells[2].Value = "---";
                dgd_PositionResult.Rows[1].Cells[1].Style.BackColor = Color.White;//Lime
                dgd_PositionResult.Rows[1].Cells[1].Style.SelectionBackColor = Color.White;//Lime
                dgd_PositionResult.Rows[1].Cells[1].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[1].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_PositionResult.Rows[1].Cells[2].Style.BackColor = Color.White;//Lime
                dgd_PositionResult.Rows[1].Cells[2].Style.SelectionBackColor = Color.White;//Lime
                dgd_PositionResult.Rows[1].Cells[2].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[1].Cells[2].Style.SelectionForeColor = Color.Black;

                dgd_PositionResult.Rows[2].Cells[2].Value = "---";
                dgd_PositionResult.Rows[2].Cells[1].Style.BackColor = Color.White;//Lime
                dgd_PositionResult.Rows[2].Cells[1].Style.SelectionBackColor = Color.White;//Lime
                dgd_PositionResult.Rows[2].Cells[1].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[2].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_PositionResult.Rows[2].Cells[2].Style.BackColor = Color.White;//Lime
                dgd_PositionResult.Rows[2].Cells[2].Style.SelectionBackColor = Color.White;//Lime
                dgd_PositionResult.Rows[2].Cells[2].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[2].Cells[2].Style.SelectionForeColor = Color.Black;
            }

            dgd_PositionResult.Rows[0].Visible = m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientAngleTolerance;
            dgd_PositionResult.Rows[1].Visible = m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientXTolerance;
            dgd_PositionResult.Rows[2].Visible = m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientYTolerance;

        }
        private void ReadPadTemplateDataToGrid(int intPadIndex, DataGridView dgd_PadSetting)
        {
            dgd_PadSetting.Rows.Clear();
            string strBlobsFeatures = m_smVisionInfo.g_arrPad[intPadIndex].GetBlobsFeaturesInspectRealData();
            string[] strFeature = strBlobsFeatures.Split('#');
            int intBlobsCount = m_smVisionInfo.g_arrPad[intPadIndex].GetBlobsFeaturesNumber();

            if (intBlobsCount != dgd_PadSetting.Rows.Count)
            {
                dgd_PadSetting.Rows.Clear();
                for (int i = 0; i < intBlobsCount; i++)
                {
                    dgd_PadSetting.Rows.Add();
                    dgd_PadSetting.Rows[i].HeaderCell.Value = "Pad " + (i + 1);
                }
            }
        }

        private void UpdateInfo()
        {
            float fOrientScore = 0.0f;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {

                //if (m_smVisionInfo.g_arrPadOrient[i].GetMinScore() != -1)
                //{
                //    fOrientScore = m_smVisionInfo.g_arrPadOrient[i].GetMinScore() * 100;
                //}
                //else
                //{

                //}
            }


            if ((m_smCustomOption.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                UpdateOrientResult();
                if (m_smVisionInfo.g_strResult == "Fail")
                {
                    lbl_ResultStatus.BackColor = Color.Red;
                }
                else
                {
                    lbl_ResultStatus.BackColor = Color.Lime;
                }
            }
            else
            {
                if (m_smVisionInfo.g_strResult == "Fail")
                {
                    lbl_ResultStatus.Text = "Fail";
                    lbl_ResultStatus.BackColor = Color.Red;
                }
                else
                {
                    lbl_ResultStatus.Text = "Pass";
                    lbl_ResultStatus.BackColor = Color.Lime;
                }
            }

            if ((m_smCustomOption.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                UpdateOrientResultOnly();
            }

            if (m_smVisionInfo.g_blnWantCheckPH && m_smVisionInfo.g_blnCheckPH)
            {
                tab_Result.SelectedTab = tp_PH;
                float fWidth = 0.0f, fHeight = 0.0f, fArea = 0.0f;
                int intFailMask = 0;
                m_smVisionInfo.g_objPositioning.GetDefectInfo(ref fWidth, ref fHeight, ref fArea, ref intFailMask);
                if (intFailMask > 0 || fArea == -1)
                {
                    lbl_ResultStatus.Text = "Fail PH";
                    lbl_ResultStatus.BackColor = Color.Red;

                    if (dgd_PHResult.Rows.Count == 0)
                        dgd_PHResult.Rows.Add(new DataGridViewRow());
                    m_blnFailPH = true;
                    dgd_PHResult.Rows[0].DefaultCellStyle.BackColor = Color.Red;
                    dgd_PHResult.Rows[0].DefaultCellStyle.SelectionBackColor = Color.Red;
                    dgd_PHResult.Rows[0].DefaultCellStyle.ForeColor = Color.Yellow;
                    dgd_PHResult.Rows[0].DefaultCellStyle.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    lbl_ResultStatus.Text = "Pass PH";
                    lbl_ResultStatus.BackColor = Color.Lime;

                    if (dgd_PHResult.Rows.Count == 0)
                        dgd_PHResult.Rows.Add(new DataGridViewRow());

                    dgd_PHResult.Rows[0].DefaultCellStyle.BackColor = Color.Lime;
                    dgd_PHResult.Rows[0].DefaultCellStyle.SelectionBackColor = Color.Lime;
                    dgd_PHResult.Rows[0].DefaultCellStyle.ForeColor = Color.Black;
                    dgd_PHResult.Rows[0].DefaultCellStyle.SelectionForeColor = Color.Black;
                }

                dgd_PHResult.Rows[0].Cells[0].Value = "Area";
                if (fArea == -1)
                    dgd_PHResult.Rows[0].Cells[1].Value = 0;
                else
                    dgd_PHResult.Rows[0].Cells[1].Value = fArea;
                m_smVisionInfo.g_objPositioning.ref_blnDrawPHResult = true;
            }
            else if (!m_smVisionInfo.g_blnCheckPH)
            {
                if (dgd_PHResult.Rows.Count > 0)
                    dgd_PHResult.Rows.Clear();
            }

            if(m_smVisionInfo.g_blnNoGrabTime)
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

            m_smVisionInfo.g_blnPadSelecting = false;
            m_smVisionInfo.g_blnUpdatePadSetting = true;
            m_smVisionInfo.g_intPadSelectedNumber = -1;
            m_smVisionInfo.g_intPadDefectSelectedNumber = -1;
            m_smVisionInfo.g_intPadPkgDefectSelectedNumber = -1;
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.g_blnViewPadInspection = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (m_smVisionInfo.g_intSelectedImage != m_smVisionInfo.g_intProductionViewImage)
            {
                m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_intProductionViewImage;

                m_smVisionInfo.g_blnUpdateImageNoComboBox = true;
            }
        }

        private void UpdatePageSetting()
        {
            CustomizeGUI();
        }
        private void UpdatePadFailSign()
        {
            if (chk_WantCheckPH.Checked)
                return;

            for (int a = 0; a < m_smVisionInfo.g_arrPad.Length; a++)
            {
                if (a > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (a == m_intPadIndex)
                    continue;

                int intBlobsCount = m_smVisionInfo.g_arrPad[a].GetBlobsFeaturesNumber();
                int intFailOptionMask = m_smVisionInfo.g_arrPad[a].ref_intFailOptionMask;
                
                for (int i = 0; i < intBlobsCount; i++)
                {
                    List<string> arrResultList = new List<string>();
                    //if ((m_smVisionInfo.g_arrPad[intPadIndex].ref_intFailResultMask & 0x1000) == 0)
                    arrResultList = m_smVisionInfo.g_arrPad[a].GetBlobFeaturesResult_WithPassFailIndicator(i);

                    long intFailMask = Convert.ToInt64(arrResultList[arrResultList.Count - 1]);
                    
                    if ((intFailMask & 0x01) > 0)
                    {
                        if ((intFailOptionMask & 0x100) > 0)
                            m_blnFailPad[a] = true;
                      
                    }
                   
                    if ((intFailMask & 0x02) > 0)
                    {
                        if ((intFailOptionMask & 0x20) > 0)
                            m_blnFailPad[a] = true;
                    
                    }
                   
                    if ((intFailMask & 0x04) > 0)
                    {
                        if ((intFailOptionMask & 0xC0) > 0)
                            m_blnFailPad[a] = true;
                       
                    }
                    
                    if ((intFailMask & 0x08) > 0)
                    {
                        if ((intFailOptionMask & 0xC0) > 0)
                            m_blnFailPad[a] = true;
                     
                    }
                    
                    if ((intFailMask & 0x10) > 0)
                    {
                        if ((intFailOptionMask & 0x600) > 0)
                            m_blnFailPad[a] = true;
                       
                    }
                    
                    if ((intFailMask & 0x20) > 0)
                    {
                        if ((intFailOptionMask & 0x600) > 0)
                            m_blnFailPad[a] = true;
                       
                    }
                    
                    if ((intFailMask & 0x40) > 0)
                    {
                        if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckBrokenPadArea)
                            m_blnFailPad[a] = true;
                      
                    }
                   
                    if ((intFailMask & 0x80) > 0)
                    {
                        if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckBrokenPadLength)
                            m_blnFailPad[a] = true;
                  
                    }
                   
                    if ((intFailMask & 0x100) > 0)
                    {
                        if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckExcessPadArea)
                            m_blnFailPad[a] = true;
                     
                    }
                  
                    if ((intFailMask & 0x2000) > 0)
                    {
                        if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckSmearPadLength)
                            m_blnFailPad[a] = true;
                    
                    }
                    
                    if (m_smVisionInfo.g_arrPad[a].ref_blnWantEdgeLimit_Pad)
                    {
                        if ((intFailMask & 0x200000) > 0)
                        {
                            if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckPadEdgeLimit)
                                m_blnFailPad[a] = true;
                        }

                        if ((intFailMask & 0x400000) > 0)
                        {
                            if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckPadEdgeLimit)
                                m_blnFailPad[a] = true;
                        }

                        if ((intFailMask & 0x800000) > 0)
                        {
                            if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckPadEdgeLimit)
                                m_blnFailPad[a] = true;
                        }

                        if ((intFailMask & 0x1000000) > 0)
                        {
                            if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckPadEdgeLimit)
                                m_blnFailPad[a] = true;
                        }
                    }

                    if (m_smVisionInfo.g_arrPad[a].ref_blnWantStandOff_Pad)
                    {
                        if ((intFailMask & 0x2000000) > 0)
                        {
                            if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckPadStandOff)
                                m_blnFailPad[a] = true;
                        }

                        if ((intFailMask & 0x4000000) > 0)
                        {
                            if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckPadStandOff)
                                m_blnFailPad[a] = true;
                        }

                        if ((intFailMask & 0x8000000) > 0)
                        {
                            if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckPadStandOff)
                                m_blnFailPad[a] = true;
                        }

                        if ((intFailMask & 0x10000000) > 0)
                        {
                            if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckPadStandOff)
                                m_blnFailPad[a] = true;
                        }
                    }

                    if (m_smVisionInfo.g_arrPad[a].ref_blnWantEdgeDistance_Pad)
                    {
                        if ((intFailMask & 0x40000000) > 0)
                        {
                            if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckPadEdgeDistance)
                                m_blnFailPad[a] = true;
                        }

                        if ((intFailMask & 0x80000000) > 0)
                        {
                            if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckPadEdgeDistance)
                                m_blnFailPad[a] = true;
                        }

                        if ((intFailMask & 0x100000000) > 0)
                        {
                            if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckPadEdgeDistance)
                                m_blnFailPad[a] = true;
                        }

                        if ((intFailMask & 0x200000000) > 0)
                        {
                            if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckPadEdgeDistance)
                                m_blnFailPad[a] = true;
                        }
                    }

                    if (m_smVisionInfo.g_arrPad[a].ref_blnWantSpan_Pad)
                    {
                        if ((intFailMask & 0x400000000) > 0)
                        {
                            if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckPadSpanX)
                                m_blnFailPad[a] = true;
                        }

                        if ((intFailMask & 0x800000000) > 0)
                        {
                            if (m_smVisionInfo.g_arrPad[a].ref_blnWantCheckPadSpanY)
                                m_blnFailPad[a] = true;
                        }
                    }

                    if ((intFailMask & 0x10000) > 0)
                    {
                        if (m_smVisionInfo.g_arrPad[a].ref_intMaxExtraLineCount > 2)
                            m_blnFailPad[a] = true;
                    
                    }
                   
                    if ((intFailMask & 0x20000) > 0)
                    {
                        if (m_smVisionInfo.g_arrPad[a].ref_intMaxExtraLineCount > 3)
                            m_blnFailPad[a] = true;
                     
                    }
                   
                    if ((intFailMask & 0x40000) > 0)
                    {
                        if (m_smVisionInfo.g_arrPad[a].ref_intMaxExtraLineCount > 4)
                            m_blnFailPad[a] = true;
                    
                    }
                   
                    if ((intFailMask & 0x80000) > 0)
                    {
                        if (m_smVisionInfo.g_arrPad[a].ref_intMaxExtraLineCount > 5)
                            m_blnFailPad[a] = true;
                       
                    }
                  
                    if ((intFailMask & 0x100000) > 0)
                    {
                        if (m_smVisionInfo.g_arrPad[a].ref_intMaxExtraLineCount > 6)
                            m_blnFailPad[a] = true;
                     
                    }

                    if ((intFailMask & 0x1000000000) > 0)
                    {
                        if (m_smVisionInfo.g_arrPad[a].ref_intMaxExtraLineCount > 7)
                            m_blnFailPad[a] = true;

                    }

                    if ((intFailMask & 0x2000000000) > 0)
                    {
                        if (m_smVisionInfo.g_arrPad[a].ref_intMaxExtraLineCount > 8)
                            m_blnFailPad[a] = true;

                    }

                    if ((intFailMask & 0x4000000000) > 0)
                    {
                        if (m_smVisionInfo.g_arrPad[a].ref_intMaxExtraLineCount > 9)
                            m_blnFailPad[a] = true;

                    }

                    if ((intFailMask & 0x8000000000) > 0)
                    {
                        if (m_smVisionInfo.g_arrPad[a].ref_intMaxExtraLineCount > 10)
                            m_blnFailPad[a] = true;

                    }

                    if ((intFailMask & 0x10000000000) > 0)
                    {
                        if (m_smVisionInfo.g_arrPad[a].ref_intMaxExtraLineCount > 11)
                            m_blnFailPad[a] = true;

                    }

                    //Missing Pad
                    if ((intFailMask & 0x20000000) > 0)
                    {
                        m_blnFailPad[a] = true;
                    }
                }

                List<List<string>> arrDefectList = m_smVisionInfo.g_arrPad[a].GetDefectList();
                for (int i = 0; i < arrDefectList.Count; i++)
                {
                  
                    int intFailMask = Convert.ToInt32(arrDefectList[i][0]);

                    
                    if (intFailMask > 0)
                    {
                        if (m_dgdDefectTable[0].Columns[1].Visible)
                            m_blnFailPad[a] = true;
           
                    }
                  
                    if ((intFailMask & 0x01) > 0)
                    {
                        if (m_dgdDefectTable[0].Columns[2].Visible)
                            m_blnFailPad[a] = true;
              
                    }
            
                    if ((intFailMask & 0x02) > 0)
                    {
                        if (m_dgdDefectTable[0].Columns[3].Visible)
                            m_blnFailPad[a] = true;
                  
                    }

                    
                    if ((intFailMask & 0x04) > 0)
                    {
                        if (m_dgdDefectTable[0].Columns[4].Visible)
                            m_blnFailPad[a] = true;

                    }

                }

                if (arrDefectList.Count == 0 && ((m_smCustomOption.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
                {
                    List<List<string>> arrColorDefectList = m_smVisionInfo.g_arrPad[a].GetColorDefectList();
                    for (int i = 0; i < arrColorDefectList.Count; i++)
                    {

                        int intFailMask = Convert.ToInt32(arrColorDefectList[i][0]);


                        if (intFailMask > 0)
                        {
                            if (m_dgdDefectTable[0].Columns[1].Visible)
                                m_blnFailPad[a] = true;

                        }

                        if ((intFailMask & 0x01) > 0)
                        {
                            if (m_dgdDefectTable[0].Columns[2].Visible)
                                m_blnFailPad[a] = true;

                        }

                        if ((intFailMask & 0x02) > 0)
                        {
                            if (m_dgdDefectTable[0].Columns[3].Visible)
                                m_blnFailPad[a] = true;

                        }


                        if ((intFailMask & 0x04) > 0)
                        {
                            if (m_dgdDefectTable[0].Columns[4].Visible)
                                m_blnFailPad[a] = true;

                        }

                    }
                }
            }
        }
        private void UpdateScore(int intPadIndex, DataGridView dgd_PadSetting)
        {
            int intFailOptionMask = m_smVisionInfo.g_arrPad[intPadIndex].ref_intFailOptionMask;

            for (int i = 0; i < dgd_PadSetting.Rows.Count; i++)
            {
                List<string> arrResultList = new List<string>();
                //if ((m_smVisionInfo.g_arrPad[intPadIndex].ref_intFailResultMask & 0x1000) == 0)
                arrResultList = m_smVisionInfo.g_arrPad[intPadIndex].GetBlobFeaturesResult_WithPassFailIndicator(i);

                long intFailMask = Convert.ToInt64(arrResultList[arrResultList.Count - 1]);

                dgd_PadSetting.Rows[i].Cells[0].Value = arrResultList[0];
                //Offset
                if ((intFailMask & 0x01) > 0 && (intFailOptionMask & 0x100) > 0)
                {
                    if (dgd_PadSetting.Columns[0].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[0].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[0].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[0].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[0].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[0].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[0].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[0].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[0].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[0].Style.SelectionForeColor = Color.Black;
                }

                //Area
                dgd_PadSetting.Rows[i].Cells[1].Value = arrResultList[1];
                if ((intFailMask & 0x02) > 0 && (intFailOptionMask & 0x20) > 0)
                {
                    if (dgd_PadSetting.Columns[1].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[1].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[1].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[1].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[1].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }

                //Width
                dgd_PadSetting.Rows[i].Cells[2].Value = arrResultList[2];
                if ((intFailMask & 0x04) > 0 && (intFailOptionMask & 0xC0) > 0)
                {
                    if (dgd_PadSetting.Columns[2].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[2].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[2].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[2].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[2].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[2].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                }

                //Length
                dgd_PadSetting.Rows[i].Cells[3].Value = arrResultList[3];
                if ((intFailMask & 0x08) > 0 && (intFailOptionMask & 0xC0) > 0)
                {
                    if (dgd_PadSetting.Columns[3].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[3].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[3].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[3].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[3].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }

                //Pitch
                dgd_PadSetting.Rows[i].Cells[4].Value = arrResultList[4];
                if ((intFailMask & 0x10) > 0 && (intFailOptionMask & 0x600) > 0)
                {
                    if (dgd_PadSetting.Columns[4].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[4].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[4].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[4].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[4].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[4].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                }

                //Gap
                dgd_PadSetting.Rows[i].Cells[5].Value = arrResultList[5];
                if ((intFailMask & 0x20) > 0 && (intFailOptionMask & 0x600) > 0)
                {
                    if (dgd_PadSetting.Columns[5].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[5].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[5].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[5].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[5].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[5].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[5].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[5].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[5].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[5].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[5].Style.SelectionForeColor = Color.Black;
                }

                // Broken Area
                dgd_PadSetting.Rows[i].Cells[6].Value = arrResultList[6];
                if ((intFailMask & 0x40) > 0 && (intFailOptionMask & 0x18) > 0)
                {
                    if (dgd_PadSetting.Columns[6].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[6].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[6].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[6].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[6].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[6].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[6].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[6].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[6].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[6].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[6].Style.SelectionForeColor = Color.Black;
                }

                // Broken Length
                dgd_PadSetting.Rows[i].Cells[7].Value = arrResultList[7];
                if ((intFailMask & 0x80) > 0 && (intFailOptionMask & 0x18) > 0)
                {
                    if (dgd_PadSetting.Columns[7].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[7].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[7].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[7].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[7].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[7].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[7].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[7].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[7].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[7].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[7].Style.SelectionForeColor = Color.Black;
                }

                // Excess
                dgd_PadSetting.Rows[i].Cells[8].Value = arrResultList[8];
                if ((intFailMask & 0x100) > 0 && (intFailOptionMask & 0x800) > 0)
                {
                    if (dgd_PadSetting.Columns[8].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[8].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[8].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[8].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[8].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[8].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[8].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[8].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[8].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[8].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[8].Style.SelectionForeColor = Color.Black;
                }

                // Smear Length
                dgd_PadSetting.Rows[i].Cells[9].Value = arrResultList[9];
                if ((intFailMask & 0x2000) > 0 && (intFailOptionMask & 0x2000) > 0)
                {
                    if (dgd_PadSetting.Columns[9].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[9].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[9].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[9].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[9].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[9].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[9].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[9].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[9].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[9].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[9].Style.SelectionForeColor = Color.Black;
                }

                // Edge Limit Top
                dgd_PadSetting.Rows[i].Cells[10].Value = arrResultList[10];
                if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantEdgeLimit_Pad && (intFailMask & 0x200000) > 0 && (intFailOptionMask & 0x4000) > 0)
                {
                    if (dgd_PadSetting.Columns[10].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[10].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[10].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[10].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[10].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[10].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[10].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[10].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[10].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[10].Style.SelectionForeColor = Color.Black;
                }

                // Edge Limit Right
                dgd_PadSetting.Rows[i].Cells[11].Value = arrResultList[11];
                if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantEdgeLimit_Pad && (intFailMask & 0x400000) > 0 && (intFailOptionMask & 0x4000) > 0)
                {
                    if (dgd_PadSetting.Columns[11].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[11].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[11].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[11].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[11].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[11].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[11].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[11].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[11].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[11].Style.SelectionForeColor = Color.Black;
                }

                // Edge Limit Bottom
                dgd_PadSetting.Rows[i].Cells[12].Value = arrResultList[12];
                if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantEdgeLimit_Pad && (intFailMask & 0x800000) > 0 && (intFailOptionMask & 0x4000) > 0)
                {
                    if (dgd_PadSetting.Columns[12].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[12].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[12].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[12].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[12].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[12].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[12].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[12].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[12].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[12].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[12].Style.SelectionForeColor = Color.Black;
                }

                // Edge Limit Left
                dgd_PadSetting.Rows[i].Cells[13].Value = arrResultList[13];
                if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantEdgeLimit_Pad && (intFailMask & 0x1000000) > 0 && (intFailOptionMask & 0x4000) > 0)
                {
                    if (dgd_PadSetting.Columns[13].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[13].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[13].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[13].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[13].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[13].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[13].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[13].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[13].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[13].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[13].Style.SelectionForeColor = Color.Black;
                }

                // Stand Off Top
                dgd_PadSetting.Rows[i].Cells[14].Value = arrResultList[14];
                if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantStandOff_Pad && (intFailMask & 0x2000000) > 0 && (intFailOptionMask & 0x8000) > 0)
                {
                    if (dgd_PadSetting.Columns[14].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[14].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[14].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[14].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[14].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[14].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[14].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[14].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[14].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[14].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[14].Style.SelectionForeColor = Color.Black;
                }

                // Stand Off Bottom
                dgd_PadSetting.Rows[i].Cells[15].Value = arrResultList[15];
                if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantStandOff_Pad && (intFailMask & 0x4000000) > 0 && (intFailOptionMask & 0x8000) > 0)
                {
                    if (dgd_PadSetting.Columns[15].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[15].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[15].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[15].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[15].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[15].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[15].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[15].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[15].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[15].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[15].Style.SelectionForeColor = Color.Black;
                }

                // Stand Off Left
                dgd_PadSetting.Rows[i].Cells[16].Value = arrResultList[16];
                if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantStandOff_Pad && (intFailMask & 0x8000000) > 0 && (intFailOptionMask & 0x8000) > 0)
                {
                    if (dgd_PadSetting.Columns[16].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[16].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[16].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[16].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[16].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[16].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[16].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[16].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[16].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[16].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[16].Style.SelectionForeColor = Color.Black;
                }

                // Stand Off Right
                dgd_PadSetting.Rows[i].Cells[17].Value = arrResultList[17];
                if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantStandOff_Pad && (intFailMask & 0x10000000) > 0 && (intFailOptionMask & 0x8000) > 0)
                {
                    if (dgd_PadSetting.Columns[17].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[17].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[17].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[17].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[17].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[17].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[17].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[17].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[17].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[17].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[17].Style.SelectionForeColor = Color.Black;
                }

                // Edge Distance Top
                dgd_PadSetting.Rows[i].Cells[18].Value = arrResultList[18];
                if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantEdgeDistance_Pad && (intFailMask & 0x40000000) > 0 && (intFailOptionMask & 0x10000) > 0)
                {
                    if (dgd_PadSetting.Columns[18].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[18].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[18].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[18].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[18].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[18].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[18].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[18].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[18].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[18].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[18].Style.SelectionForeColor = Color.Black;
                }

                // Edge Distance Right
                dgd_PadSetting.Rows[i].Cells[19].Value = arrResultList[19];
                if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantEdgeDistance_Pad && (intFailMask & 0x80000000) > 0 && (intFailOptionMask & 0x10000) > 0)
                {
                    if (dgd_PadSetting.Columns[19].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[19].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[19].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[19].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[19].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[19].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[19].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[19].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[19].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[19].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[19].Style.SelectionForeColor = Color.Black;
                }

                // Edge Distance Bottom
                dgd_PadSetting.Rows[i].Cells[20].Value = arrResultList[20];
                if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantEdgeDistance_Pad && (intFailMask & 0x100000000) > 0 && (intFailOptionMask & 0x10000) > 0)
                {
                    if (dgd_PadSetting.Columns[20].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[20].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[20].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[20].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[20].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[20].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[20].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[20].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[20].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[20].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[20].Style.SelectionForeColor = Color.Black;
                }

                // Edge Distance Left
                dgd_PadSetting.Rows[i].Cells[21].Value = arrResultList[21];
                if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantEdgeDistance_Pad && (intFailMask & 0x200000000) > 0 && (intFailOptionMask & 0x10000) > 0)
                {
                    if (dgd_PadSetting.Columns[21].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[21].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[21].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[21].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[21].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[21].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[21].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[21].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[21].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[21].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[21].Style.SelectionForeColor = Color.Black;
                }

                // Line 3 Length
                dgd_PadSetting.Rows[i].Cells[22].Value = arrResultList[22];
                if ((intFailMask & 0x10000) > 0 && (intFailOptionMask & 0xC0) > 0)
                {
                    if (dgd_PadSetting.Columns[22].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[22].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[22].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[22].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[22].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[22].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[22].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[22].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[22].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[22].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[22].Style.SelectionForeColor = Color.Black;
                }

                // Line 4 Length
                dgd_PadSetting.Rows[i].Cells[23].Value = arrResultList[23];
                if ((intFailMask & 0x20000) > 0 && (intFailOptionMask & 0xC0) > 0)
                {
                    if (dgd_PadSetting.Columns[23].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[23].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[23].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[23].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[23].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[23].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[23].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[23].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[23].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[23].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[23].Style.SelectionForeColor = Color.Black;
                }

                // Line 5 Length
                dgd_PadSetting.Rows[i].Cells[24].Value = arrResultList[24];
                if ((intFailMask & 0x40000) > 0 && (intFailOptionMask & 0xC0) > 0)
                {
                    if (dgd_PadSetting.Columns[24].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[24].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[24].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[24].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[24].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[24].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[24].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[24].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[24].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[24].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[24].Style.SelectionForeColor = Color.Black;
                }

                // Line 6 Length
                dgd_PadSetting.Rows[i].Cells[25].Value = arrResultList[25];
                if ((intFailMask & 0x80000) > 0 && (intFailOptionMask & 0xC0) > 0)
                {
                    if (dgd_PadSetting.Columns[25].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[25].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[25].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[25].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[25].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[25].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[25].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[25].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[25].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[25].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[25].Style.SelectionForeColor = Color.Black;
                }

                // Line 7 Length
                dgd_PadSetting.Rows[i].Cells[26].Value = arrResultList[26];
                if ((intFailMask & 0x100000) > 0 && (intFailOptionMask & 0xC0) > 0)
                {
                    if (dgd_PadSetting.Columns[26].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[26].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[26].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[26].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[26].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[26].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[26].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[26].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[26].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[26].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[26].Style.SelectionForeColor = Color.Black;
                }

                // Line 8 Length
                dgd_PadSetting.Rows[i].Cells[27].Value = arrResultList[27];
                if ((intFailMask & 0x800000000) > 0 && (intFailOptionMask & 0xC0) > 0)
                {
                    if (dgd_PadSetting.Columns[27].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[27].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[27].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[27].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[27].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[27].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[27].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[27].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[27].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[27].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[27].Style.SelectionForeColor = Color.Black;
                }

                // Line 8 Length
                dgd_PadSetting.Rows[i].Cells[27].Value = arrResultList[27];
                if ((intFailMask & 0x1000000000) > 0 && (intFailOptionMask & 0xC0) > 0)
                {
                    if (dgd_PadSetting.Columns[27].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[27].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[27].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[27].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[27].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[27].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[27].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[27].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[27].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[27].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[27].Style.SelectionForeColor = Color.Black;
                }

                // Line 9 Length
                dgd_PadSetting.Rows[i].Cells[28].Value = arrResultList[28];
                if ((intFailMask & 0x2000000000) > 0 && (intFailOptionMask & 0xC0) > 0)
                {
                    if (dgd_PadSetting.Columns[28].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[28].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[28].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[28].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[28].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[28].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[28].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[28].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[28].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[28].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[28].Style.SelectionForeColor = Color.Black;
                }

                // Line 10 Length
                dgd_PadSetting.Rows[i].Cells[29].Value = arrResultList[29];
                if ((intFailMask & 0x4000000000) > 0 && (intFailOptionMask & 0xC0) > 0)
                {
                    if (dgd_PadSetting.Columns[29].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[29].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[29].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[29].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[29].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[29].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[29].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[29].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[29].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[29].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[29].Style.SelectionForeColor = Color.Black;
                }

                // Line 11 Length
                dgd_PadSetting.Rows[i].Cells[30].Value = arrResultList[30];
                if ((intFailMask & 0x8000000000) > 0 && (intFailOptionMask & 0xC0) > 0)
                {
                    if (dgd_PadSetting.Columns[30].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[30].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[30].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[30].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[30].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[30].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[30].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[30].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[30].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[30].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[30].Style.SelectionForeColor = Color.Black;
                }

                // Line 12 Length
                dgd_PadSetting.Rows[i].Cells[31].Value = arrResultList[31];
                if ((intFailMask & 0x10000000000) > 0 && (intFailOptionMask & 0xC0) > 0)
                {
                    if (dgd_PadSetting.Columns[31].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_PadSetting.Rows[i].Cells[31].Style.BackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[31].Style.SelectionBackColor = Color.Red;
                    dgd_PadSetting.Rows[i].Cells[31].Style.ForeColor = Color.Yellow;
                    dgd_PadSetting.Rows[i].Cells[31].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[31].Style.BackColor = GetColor(dgd_PadSetting.Rows[i].Cells[31].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[31].Style.SelectionBackColor = GetColor(dgd_PadSetting.Rows[i].Cells[31].Value.ToString());
                    dgd_PadSetting.Rows[i].Cells[31].Style.ForeColor = Color.Black;
                    dgd_PadSetting.Rows[i].Cells[31].Style.SelectionForeColor = Color.Black;
                }

                //Span
                if (dgd_Span.RowCount == 0)
                {
                    dgd_Span.Rows.Add();
                    dgd_Span.Rows[0].Cells[0].Value = "Span X";
                    dgd_Span.Rows[0].Cells[0].Style.BackColor = Color.White;
                    dgd_Span.Rows[0].Cells[0].Style.SelectionBackColor = Color.White;
                    dgd_Span.Rows[0].Cells[1].Style.BackColor = Color.White;
                    dgd_Span.Rows[0].Cells[1].Style.SelectionBackColor = Color.White;
                }
                if (dgd_Span.RowCount == 1)
                {
                    dgd_Span.Rows.Add();
                    dgd_Span.Rows[1].Cells[0].Value = "Span Y";
                    dgd_Span.Rows[1].Cells[0].Style.BackColor = Color.White;
                    dgd_Span.Rows[1].Cells[0].Style.SelectionBackColor = Color.White;
                    dgd_Span.Rows[1].Cells[1].Style.BackColor = Color.White;
                    dgd_Span.Rows[1].Cells[1].Style.SelectionBackColor = Color.White;
                }

                // Span X
                dgd_Span.Rows[0].Cells[1].Value = arrResultList[32];
                if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantSpan_Pad && (intFailMask & 0x400000000) > 0 && (intFailOptionMask & 0x20000) > 0)
                {
                    if (dgd_Span.Rows[0].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_Span.Rows[0].Cells[0].Style.BackColor = Color.Red;
                    dgd_Span.Rows[0].Cells[0].Style.SelectionBackColor = Color.Red;
                    dgd_Span.Rows[0].Cells[0].Style.ForeColor = Color.Yellow;
                    dgd_Span.Rows[0].Cells[0].Style.SelectionForeColor = Color.Yellow;
                    dgd_Span.Rows[0].Cells[1].Style.BackColor = Color.Red;
                    dgd_Span.Rows[0].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgd_Span.Rows[0].Cells[1].Style.ForeColor = Color.Yellow;
                    dgd_Span.Rows[0].Cells[1].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_Span.Rows[0].Cells[0].Style.BackColor = GetColor(dgd_Span.Rows[0].Cells[1].Value.ToString());
                    dgd_Span.Rows[0].Cells[0].Style.SelectionBackColor = GetColor(dgd_Span.Rows[0].Cells[1].Value.ToString());
                    dgd_Span.Rows[0].Cells[0].Style.ForeColor = Color.Black;
                    dgd_Span.Rows[0].Cells[0].Style.SelectionForeColor = Color.Black;
                    dgd_Span.Rows[0].Cells[1].Style.BackColor = GetColor(dgd_Span.Rows[0].Cells[1].Value.ToString());
                    dgd_Span.Rows[0].Cells[1].Style.SelectionBackColor = GetColor(dgd_Span.Rows[0].Cells[1].Value.ToString());
                    dgd_Span.Rows[0].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Span.Rows[0].Cells[1].Style.SelectionForeColor = Color.Black;
                }

                // Span Y
                dgd_Span.Rows[1].Cells[1].Value = arrResultList[33];
                if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantSpan_Pad && (intFailMask & 0x800000000) > 0 && (intFailOptionMask & 0x40000) > 0)
                {
                    if (dgd_Span.Rows[1].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_Span.Rows[1].Cells[0].Style.BackColor = Color.Red;
                    dgd_Span.Rows[1].Cells[0].Style.SelectionBackColor = Color.Red;
                    dgd_Span.Rows[1].Cells[0].Style.ForeColor = Color.Yellow;
                    dgd_Span.Rows[1].Cells[0].Style.SelectionForeColor = Color.Yellow;
                    dgd_Span.Rows[1].Cells[1].Style.BackColor = Color.Red;
                    dgd_Span.Rows[1].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgd_Span.Rows[1].Cells[1].Style.ForeColor = Color.Yellow;
                    dgd_Span.Rows[1].Cells[1].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_Span.Rows[1].Cells[0].Style.BackColor = GetColor(dgd_Span.Rows[1].Cells[1].Value.ToString());
                    dgd_Span.Rows[1].Cells[0].Style.SelectionBackColor = GetColor(dgd_Span.Rows[1].Cells[1].Value.ToString());
                    dgd_Span.Rows[1].Cells[0].Style.ForeColor = Color.Black;
                    dgd_Span.Rows[1].Cells[0].Style.SelectionForeColor = Color.Black;
                    dgd_Span.Rows[1].Cells[1].Style.BackColor = GetColor(dgd_Span.Rows[1].Cells[1].Value.ToString());
                    dgd_Span.Rows[1].Cells[1].Style.SelectionBackColor = GetColor(dgd_Span.Rows[1].Cells[1].Value.ToString());
                    dgd_Span.Rows[1].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Span.Rows[1].Cells[1].Style.SelectionForeColor = Color.Black;
                }

                //Missing Pad
                if ((intFailMask & 0x20000000) > 0)
                {
                    for (int j = 0; j < dgd_PadSetting.Columns.Count; j++)
                    {
                        if (dgd_PadSetting.Columns[j].Visible == true)
                        {
                            dgd_PadSetting.Rows[i].Cells[j].Style.BackColor = Color.Red;
                            dgd_PadSetting.Rows[i].Cells[j].Style.SelectionBackColor = Color.Red;
                            dgd_PadSetting.Rows[i].Cells[j].Style.ForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[j].Style.SelectionForeColor = Color.Yellow;
                        }
                    }
                    m_blnFailPad[intPadIndex] = true;
                }
            }
        }

        private void UpdateDefectTable(int intPadIndex, DataGridView dgd_DefectTable)
        {
            dgd_DefectTable.Rows.Clear();

            if (chk_WantCheckPH.Checked)
                return;

            List<List<string>> arrDefectList = m_smVisionInfo.g_arrPad[intPadIndex].GetDefectList();
            for (int i = 0; i < arrDefectList.Count; i++)
            {
                dgd_DefectTable.Rows.Add();
                dgd_DefectTable.Rows[i].Cells[0].Value = (i + 1).ToString();
                dgd_DefectTable.Rows[i].Cells[0].Style.BackColor = Color.FromArgb(215, 228, 242);
                dgd_DefectTable.Rows[i].Cells[0].Style.SelectionBackColor = Color.Yellow;

                if (m_smVisionInfo.g_intPadDefectSelectedNumber == i)
                    dgd_DefectTable.Rows[i].Cells[0].Selected = true;
                else
                    dgd_DefectTable.Rows[i].Cells[0].Selected = false;

                int intFailMask = Convert.ToInt32(arrDefectList[i][0]);
                
                dgd_DefectTable.Rows[i].Cells[1].Value = arrDefectList[i][1];
                if (intFailMask > 0)
                {
                    if (dgd_DefectTable.Columns[1].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_DefectTable.Rows[i].Cells[1].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_DefectTable.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                    dgd_DefectTable.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    dgd_DefectTable.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_DefectTable.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }

                dgd_DefectTable.Rows[i].Cells[2].Value = arrDefectList[i][2];
                if ((intFailMask & 0x01) > 0)
                {
                    if (dgd_DefectTable.Columns[2].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_DefectTable.Rows[i].Cells[2].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_DefectTable.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                    dgd_DefectTable.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                    dgd_DefectTable.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                    dgd_DefectTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                }

                dgd_DefectTable.Rows[i].Cells[3].Value = arrDefectList[i][3];
                if ((intFailMask & 0x02) > 0)
                {
                    if (dgd_DefectTable.Columns[3].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_DefectTable.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_DefectTable.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_DefectTable.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_DefectTable.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_DefectTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }

                dgd_DefectTable.Rows[i].Cells[4].Value = arrDefectList[i][4];
                if ((intFailMask & 0x04) > 0)
                {
                    if (dgd_DefectTable.Columns[4].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_DefectTable.Rows[i].Cells[4].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_DefectTable.Rows[i].Cells[4].Style.BackColor = Color.Lime;
                    dgd_DefectTable.Rows[i].Cells[4].Style.SelectionBackColor = Color.Lime;
                    dgd_DefectTable.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                    dgd_DefectTable.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                }
            }
        }
        private void UpdateColorDefectTable(int intPadIndex, DataGridView dgd_DefectTable)
        {

            if (m_smVisionInfo.g_arrPad[intPadIndex].GetDefectList().Count > 0 && (m_smVisionInfo.g_arrPad[intPadIndex].ref_intFailResultMask & 0x1001) > 0)
                return;

            //dgd_DefectTable.Rows.Clear();
            int intContaminationCount = m_smVisionInfo.g_arrPad[intPadIndex].GetDefectList().Count;
            List<List<string>> arrDefectList = m_smVisionInfo.g_arrPad[intPadIndex].GetColorDefectList();
            for (int i = 0; i < arrDefectList.Count; i++)
            {
                dgd_DefectTable.Rows.Add();
                dgd_DefectTable.Rows[i + intContaminationCount].Cells[0].Value = (i + 1 + intContaminationCount).ToString();
                dgd_DefectTable.Rows[i + intContaminationCount].Cells[0].Style.BackColor = Color.FromArgb(215, 228, 242);
                dgd_DefectTable.Rows[i + intContaminationCount].Cells[0].Style.SelectionBackColor = Color.Yellow;

                if (m_smVisionInfo.g_intPadDefectSelectedNumber == (i + intContaminationCount))
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[0].Selected = true;
                else
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[0].Selected = false;

                int intFailMask = Convert.ToInt32(arrDefectList[i][0]);

                if (intFailMask == 0x10)
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[1].Value = "Missing " + arrDefectList[i][1];
                else
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[1].Value = arrDefectList[i][1];

                if (intFailMask > 0)
                {
                    if (dgd_DefectTable.Columns[1].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[1].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[1].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[1].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[1].Style.BackColor = Color.Lime;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[1].Style.SelectionBackColor = Color.Lime;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[1].Style.ForeColor = Color.Black;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[1].Style.SelectionForeColor = Color.Black;
                }

                if (Convert.ToDouble(arrDefectList[i][2]) > 0)
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[2].Value = arrDefectList[i][2];
                else
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[2].Value = "-";
                if ((intFailMask & 0x01) > 0)
                {
                    if (dgd_DefectTable.Columns[2].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[2].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[2].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[2].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[2].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[2].Style.BackColor = Color.Lime;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[2].Style.SelectionBackColor = Color.Lime;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[2].Style.ForeColor = Color.Black;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[2].Style.SelectionForeColor = Color.Black;
                }

                if (Convert.ToDouble(arrDefectList[i][3]) > 0)
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[3].Value = arrDefectList[i][3];
                else
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[3].Value = "-";
                if ((intFailMask & 0x02) > 0)
                {
                    if (dgd_DefectTable.Columns[3].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[3].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[3].Style.BackColor = Color.Lime;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[3].Style.ForeColor = Color.Black;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[3].Style.SelectionForeColor = Color.Black;
                }

                if (Convert.ToDouble(arrDefectList[i][4]) > 0)
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[4].Value = arrDefectList[i][4];
                else
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[4].Value = "-";
                if (((intFailMask & 0x04) > 0) || ((intFailMask & 0x08) > 0))
                {
                    if (dgd_DefectTable.Columns[4].Visible)
                        m_blnFailPad[intPadIndex] = true;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[4].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[4].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[4].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[4].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[4].Style.BackColor = Color.Lime;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[4].Style.SelectionBackColor = Color.Lime;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[4].Style.ForeColor = Color.Black;
                    dgd_DefectTable.Rows[i + intContaminationCount].Cells[4].Style.SelectionForeColor = Color.Black;
                }
            }
        }
        private void UpdatePkgFailSign()
        {
            if (chk_WantCheckPH.Checked)
                return;

            for (int a = 0; a < m_smVisionInfo.g_arrPad.Length; a++)
            {
                if (a > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (a == m_intPadIndex)
                    continue;

                List<List<string>> arrDefectList = m_smVisionInfo.g_arrPad[a].GetPkgDefectList();
                for (int i = 0; i < arrDefectList.Count; i++)
                {
                    int intFailMask = Convert.ToInt32(arrDefectList[i][1]);
                    
                    if (intFailMask > 0)
                    {
                        if (m_dgdPkgDefectTable[0].Columns[1].Visible)
                            m_blnFailPackage[a] = true;
                    }
               
                    
                    if ((intFailMask & 0x01) > 0)
                    {
                        if (m_dgdPkgDefectTable[0].Columns[2].Visible)
                            m_blnFailPackage[a] = true;
                    }
                 
                    
                    if ((intFailMask & 0x02) > 0)
                    {
                        if (m_dgdPkgDefectTable[0].Columns[3].Visible)
                            m_blnFailPackage[a] = true;
                    }
                 
                    if ((intFailMask & 0x0C) > 0)
                    {
                        if (m_dgdPkgDefectTable[0].Columns[4].Visible)
                            m_blnFailPackage[a] = true;
                  
                    }
                 
                }
            }
        }
        private void UpdateOrientResult()
        {
            lbl_ResultStatus.Text = "";

            switch (m_smVisionInfo.g_intOrientResult[0])
            {
                case 0:
                    lbl_ResultStatus.Text += "0";
                    break;
                case 1:
                    if (m_smCustomOption.g_intOrientIO == 0)
                    {
                        lbl_ResultStatus.Text += "-90";
                    }
                    else
                    {
                        lbl_ResultStatus.Text += "90";
                    }
                    break;
                case 2:
                    lbl_ResultStatus.Text += "180";
                    break;
                case 3:
                    if (m_smCustomOption.g_intOrientIO == 0)
                    {
                        lbl_ResultStatus.Text += "90";
                    }
                    else
                    {
                        lbl_ResultStatus.Text += "-90";
                    }
                    break;
                case 4:
                    lbl_ResultStatus.Text += "Fail";
                    break;
            }

        }
        private void UpdateOrientResultOnly()
        {
            //Orient table
            dgd_OrientResult.Rows.Clear();
            dgd_OrientResult.Rows.Add();
            dgd_OrientResult.Rows[0].Cells[0].Value = "Orient";
            dgd_OrientResult.Rows[0].Cells[1].Value = m_smVisionInfo.g_objPadOrient.ref_fMinScore * 100;
            dgd_OrientResult.Rows[0].Cells[2].Value = "----";
            dgd_OrientResult.Rows[0].Cells[2].Style.BackColor = Color.White;//Lime
            dgd_OrientResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.White;//Lime
            dgd_OrientResult.Rows[0].Cells[2].Style.ForeColor = Color.Black;
            dgd_OrientResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
            
            float fScore = -1;
          
                fScore = m_smVisionInfo.g_objPadOrient.GetMinScore() * 100;

            if (fScore >= 0)    //if (fScore > 0) // 2019 09 16 - CCENG: Change > to >= make sure orient result table will be updated when score is 0.
            {
                dgd_OrientResult.Rows[0].Cells[2].Value = fScore.ToString("f2");

                if (Convert.ToDouble(m_smVisionInfo.g_objPadOrient.ref_fMinScore * 100) <= Convert.ToDouble(dgd_OrientResult.Rows[0].Cells[2].Value))
                {
                    dgd_OrientResult.Rows[0].Cells[2].Style.BackColor = Color.Lime;
                    dgd_OrientResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.Lime;
                    dgd_OrientResult.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                    dgd_OrientResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
                    
                }
                else
                {
                    m_blnFailOrient = true;
                    dgd_OrientResult.Rows[0].Cells[2].Style.BackColor = Color.Red;
                    dgd_OrientResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.Red;
                    dgd_OrientResult.Rows[0].Cells[2].Style.ForeColor = Color.Yellow;
                    dgd_OrientResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Yellow;
                }
            }
            else
            {
                dgd_OrientResult.Rows[0].Cells[2].Value = "----";
                dgd_OrientResult.Rows[0].Cells[2].Style.BackColor = Color.White;//Lime
                dgd_OrientResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.White;//Lime
                dgd_OrientResult.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                dgd_OrientResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
            }
            

        }
        private void UpdatePkgResultTable(int intPadIndex, DataGridView dgd_Package)
        {
            //for (int x = dgd_Package.RowCount - 1; x > 0; x--)
            //{
            //    dgd_Package.Rows.RemoveAt(x);
            //}

            //dgd_Package.Rows.Clear();
            //dgd_Package.Rows.Add();
            //dgd_Package.Rows[0].Cells[0].Value = "Template";
            //dgd_Package.Rows[0].Cells[1].Value = m_smVisionInfo.g_arrPad[0].ref_fUnitWidth.ToString("f4");
            //dgd_Package.Rows[0].Cells[2].Value = m_smVisionInfo.g_arrPad[0].ref_fUnitHeight.ToString("f4");
            //dgd_Package.Rows[0].Cells[3].Value = m_smVisionInfo.g_arrPad[0].ref_fUnitThickness.ToString("f4");

            // Check Inspect Package Size ON or not
            if (m_smVisionInfo.g_arrPad.Length == 1)
            {
                if ((m_smVisionInfo.g_arrPad[0].ref_intFailPkgOptionMask & 0x01) == 0)  // 2019 07 08 - CCENG: not need to use function GetWantInspectPackage() because option pkg size has separated from group of package option already.
                {
                    dgd_Package.Rows.Clear();
                    return;
                }
            }
            else
            {
                if (((m_smVisionInfo.g_arrPad[0].ref_intFailPkgOptionMask & 0x01) == 0) && !m_smVisionInfo.g_blnCheck4Sides)
                {
                    dgd_Package.Rows.Clear();
                    return;
                }
                else if (((m_smVisionInfo.g_arrPad[0].ref_intFailPkgOptionMask & 0x01) == 0) && m_smVisionInfo.g_blnCheck4Sides && ((m_smVisionInfo.g_arrPad[1].ref_intFailPkgOptionMask & 0x01) == 0))
                {
                    dgd_Package.Rows.Clear();
                    return;
                }
            }

            // 2020-06-01 ZJYEOh : clear package data grid view if check PH
            if (chk_WantCheckPH.Checked)
            {
                dgd_Package.Rows.Clear();
                return;
            }
            dgd_Package.Rows.Clear();
            float fWidthMin = m_smVisionInfo.g_arrPad[0].GetUnitWidthMin(1);
            float fWidthMax = m_smVisionInfo.g_arrPad[0].GetUnitWidthMax(1);
            float fHeightMin = m_smVisionInfo.g_arrPad[0].GetUnitHeightMin(1);
            float fHeightMax = m_smVisionInfo.g_arrPad[0].GetUnitHeightMax(1);
            float fThicknessMin = m_smVisionInfo.g_arrPad[0].GetUnitThicknessMin(1);
            float fThicknessMax = m_smVisionInfo.g_arrPad[0].GetUnitThicknessMax(1);
            float fWidth = (m_smVisionInfo.g_arrPad[0].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[0].GetResultDownWidth_RectGauge4L(1)) / 2;

            // 2019-10-25 ZJYEOH : Add Offset to package width
            fWidth += m_smVisionInfo.g_arrPad[0].ref_fPackageWidthOffsetMM;

            float fHeight = (m_smVisionInfo.g_arrPad[0].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[0].GetResultRightHeight_RectGauge4L(1)) / 2;

            // 2019-10-25 ZJYEOH : Add Offset to package height
            fHeight += m_smVisionInfo.g_arrPad[0].ref_fPackageHeightOffsetMM;

            float fThickness = 0;
            float fTotalThickness = 0;
            int intCount = 0;
            for (int j = 1; j < m_smVisionInfo.g_arrPad.Length; j++)
            {
                if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (j == 1 || j == 3)
                    fTotalThickness += m_smVisionInfo.g_arrPad[j].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[j].GetResultRightHeight_RectGauge4L(1);
                else
                    fTotalThickness += m_smVisionInfo.g_arrPad[j].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[j].GetResultDownWidth_RectGauge4L(1);

                intCount += 2;
            }
            fThickness = fTotalThickness / intCount;

            // 2019-10-25 ZJYEOH : Add Offset to package thickness
            if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
                fThickness += m_smVisionInfo.g_arrPad[1].ref_fPackageThicknessOffsetMM;

            if (dgd_Package.Rows.Count == 0)
                dgd_Package.Rows.Add();
            dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[0].Value = "Result";

            if ((m_smVisionInfo.g_arrPad[0].ref_intFailPkgOptionMask & 0x01) > 0)
            {
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Value = fWidth.ToString("f4");
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Value = fHeight.ToString("f4");
            }
            else
            {
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Value = "----";
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Value = "----";
            }

            if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides && ((m_smVisionInfo.g_arrPad[1].ref_intFailPkgOptionMask & 0x01) > 0) && m_smVisionInfo.g_arrPad[1].GetOverallWantGaugeMeasurePkgSize(true))
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Value = fThickness.ToString("f4");
            else
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Value = "----";

            if (((fWidth < fWidthMin || fWidth > fWidthMax) && dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Value.ToString() != "----") ||
                ((fHeight < fHeightMin || fHeight > fHeightMax) && dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Value.ToString() != "----") || 
                ((fThickness < fThicknessMin || fThickness > fThicknessMax) && dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Visible && dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Value.ToString() != "----"))
            {
                if (dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Value.ToString() != "----")
                {
                    if (fWidth < fWidthMin || fWidth > fWidthMax)
                    {
                        if (dgd_Package.Columns[1].Visible)
                            m_blnFailPackageSize = true; //m_blnFailPackage[intPadIndex] = true;
                        dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.BackColor = Color.Red;
                        dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.BackColor = Color.Lime;
                        dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.SelectionBackColor = Color.Lime;
                    }
                }
                else
                {
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.BackColor = Color.White;
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.SelectionBackColor = Color.White;
                }

                if (dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Value.ToString() != "----")
                {
                    if (fHeight < fHeightMin || fHeight > fHeightMax)
                    {
                        if (dgd_Package.Columns[2].Visible)
                            m_blnFailPackageSize = true;  //m_blnFailPackage[intPadIndex] = true;
                        dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.BackColor = Color.Red;
                        dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.BackColor = Color.Lime;
                        dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.SelectionBackColor = Color.Lime;
                    }
                }
                else
                {
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.BackColor = Color.White;
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.SelectionBackColor = Color.White;
                }

                if (dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Value.ToString() != "----")
                {
                    if (fThickness < fThicknessMin || fThickness > fThicknessMax)
                    {
                        if (dgd_Package.Columns[3].Visible)
                            m_blnFailPackageSize = true; //m_blnFailPackage[intPadIndex] = true;
                        dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Style.BackColor = Color.Red;
                        dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Style.SelectionBackColor = Color.Lime;
                    }
                }
                else
                {
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Style.BackColor = Color.White;
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Style.SelectionBackColor = Color.White;
                }
                
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[0].Style.BackColor = Color.Red;
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[0].Style.SelectionBackColor = Color.Red;
            }
            else
            {
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[0].Style.BackColor = Color.Lime;
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[0].Style.SelectionBackColor = Color.Lime;

                if (dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Value.ToString() != "----")
                {
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.BackColor = Color.Lime;
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.SelectionBackColor = Color.Lime;
                }
                else
                {
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.BackColor = Color.White;
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.SelectionBackColor = Color.White;
                }

                if (dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Value.ToString() != "----")
                {
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.BackColor = Color.Lime;
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.SelectionBackColor = Color.Lime;
                }
                else
                {
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.BackColor = Color.White;
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.SelectionBackColor = Color.White;
                }

                if (dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Value.ToString() != "----")
                {
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Style.SelectionBackColor = Color.Lime;
                }
                else
                {
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Style.BackColor = Color.White;
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Style.SelectionBackColor = Color.White;
                }
            }

            if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides && m_smVisionInfo.g_arrPad[1].ref_blnWantIndividualSideThickness && ((m_smVisionInfo.g_arrPad[1].ref_intFailPkgOptionMask & 0x01) > 0) && m_smVisionInfo.g_arrPad[1].GetOverallWantGaugeMeasurePkgSize(true))
            {
                for (int j = 1; j < m_smVisionInfo.g_arrPad.Length; j++)
                {
                    float f4SThickness = 0;

                    if (dgd_Package.RowCount == j)
                        dgd_Package.Rows.Add();

                    if (j == 1)
                        dgd_Package.Rows[j].Cells[0].Value = "Top";
                    else if (j == 2)
                        dgd_Package.Rows[j].Cells[0].Value = "Right";
                    else if (j == 3)
                        dgd_Package.Rows[j].Cells[0].Value = "Bottom";
                    else if (j == 4)
                        dgd_Package.Rows[j].Cells[0].Value = "Left";

                    dgd_Package.Rows[j].Cells[1].Value = "---";
                    dgd_Package.Rows[j].Cells[2].Value = "---";
                    dgd_Package.Rows[j].Cells[1].Style.BackColor = Color.White;
                    dgd_Package.Rows[j].Cells[1].Style.SelectionBackColor = Color.White;
                    dgd_Package.Rows[j].Cells[2].Style.BackColor = Color.White;
                    dgd_Package.Rows[j].Cells[2].Style.SelectionBackColor = Color.White;

                    if (j == 1 || j == 3)
                        f4SThickness = (m_smVisionInfo.g_arrPad[j].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[j].GetResultRightHeight_RectGauge4L(1)) / 2;
                    else
                        f4SThickness = (m_smVisionInfo.g_arrPad[j].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[j].GetResultDownWidth_RectGauge4L(1)) / 2;

                    f4SThickness += m_smVisionInfo.g_arrPad[j].ref_fPackageThicknessOffsetMM;

                    dgd_Package.Rows[j].Cells[3].Value = f4SThickness.ToString("f4");

                    if (f4SThickness < fThicknessMin || f4SThickness > fThicknessMax)
                    {
                        if (dgd_Package.Columns[3].Visible)
                            m_blnFailPackageSize = true; //m_blnFailPackage[intPadIndex] = true;
                        dgd_Package.Rows[j].Cells[3].Style.BackColor = Color.Red;
                        dgd_Package.Rows[j].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_Package.Rows[j].Cells[0].Style.BackColor = Color.Red;
                        dgd_Package.Rows[j].Cells[0].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_Package.Rows[j].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Package.Rows[j].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Package.Rows[j].Cells[0].Style.BackColor = Color.Lime;
                        dgd_Package.Rows[j].Cells[0].Style.SelectionBackColor = Color.Lime;
                    }
                }
            }
        }

        private void UpdatePkgDefectTable(int intPadIndex, DataGridView dgd_PkgDefectTable)
        {
            dgd_PkgDefectTable.Rows.Clear();

            if (chk_WantCheckPH.Checked)
                return;

            List<List<string>> arrDefectList = m_smVisionInfo.g_arrPad[intPadIndex].GetPkgDefectList();
            for (int i = 0; i < arrDefectList.Count; i++)
            {
                dgd_PkgDefectTable.Rows.Add();
                //if(m_smVisionInfo.g_intImageMergeType!=0)
                //    if(Convert.ToSingle(arrDefectList[i][0])>2)
                //    dgd_PkgDefectTable.Rows[i].Cells[0].Value = (Convert.ToSingle(arrDefectList[i][0])-1).ToString();
                //else
                    dgd_PkgDefectTable.Rows[i].Cells[0].Value = arrDefectList[i][0].ToString();
                dgd_PkgDefectTable.Rows[i].Cells[0].Style.BackColor = Color.FromArgb(215, 228, 242);
                dgd_PkgDefectTable.Rows[i].Cells[0].Style.SelectionBackColor = Color.Yellow;

                if (m_smVisionInfo.g_intPadPkgDefectSelectedNumber == i)
                    dgd_PkgDefectTable.Rows[i].Cells[0].Selected = true;
                else
                    dgd_PkgDefectTable.Rows[i].Cells[0].Selected = false;

                int intFailMask = Convert.ToInt32(arrDefectList[i][1]);

                // 2019 04 24 - CCENG: g_intSelectedImage value will only be reassigned if detected defect is fail (intFailMask > 0)
                if (m_smVisionInfo.PR_MN_TestDone && m_smVisionInfo.AT_VM_ManualTestMode && intFailMask != 0x00)
                {
                    //int intImageNo = Convert.ToInt32(arrDefectList[i][0]) - 1;
                    //if (m_smVisionInfo.g_intImageMergeType == 1) // Merge Grab 1&2
                    //{
                    //    if ((intImageNo >= 1) && ((m_smVisionInfo.g_arrImages.Count - m_smVisionInfo.g_intImageMergeType) > 1))
                    //        intImageNo = intImageNo + 1;
                    //}
                    //else if (m_smVisionInfo.g_intImageMergeType == 3) // Merge Grab 1&2, Merge Grab 3&4
                    //{
                    //    //if (intImageNo < 2)
                    //    //    intImageNo = 0;
                    //    //else if (intImageNo < 4)
                    //    //    intImageNo = 1;
                    //    //else
                    //    //    intImageNo = 2;

                    //    //2020-01-09 ZJYEOH : 
                    //    if (intImageNo == 1)
                    //        intImageNo = intImageNo + 1;
                    //    else if (intImageNo == 2)
                    //        intImageNo = intImageNo + 2;
                    //}

                    //if (m_smVisionInfo.g_intImageMergeType == 4)
                    //{
                    //    //intViewImageCount = 2;
                    //    intImageNo = m_smVisionInfo.g_arrImages.Count;
                    //    if (m_smVisionInfo.g_arrImages.Count <= 3)
                    //        intImageNo = 1;
                    //    else if (m_smVisionInfo.g_arrImages.Count <= 5)
                    //        intImageNo = 2;
                    //    else
                    //        intImageNo = intImageNo - 3;
                    //}

                    int intImageNo = ImageDrawing.GetArrayImageIndex(Convert.ToInt32(arrDefectList[i][0]) - 1, m_smVisionInfo.g_intVisionIndex);

                    if (m_smVisionInfo.g_intSelectedImage != intImageNo)
                    {
                        m_smVisionInfo.g_intSelectedImage = intImageNo;
                        m_smVisionInfo.g_blnUpdateImageNoComboBox = true;
                    }
                }

                dgd_PkgDefectTable.Rows[i].Cells[1].Value = arrDefectList[i][2];
                if (intFailMask > 0)
                {
                    if (dgd_PkgDefectTable.Columns[1].Visible)
                        m_blnFailPackage[intPadIndex] = true;
                    dgd_PkgDefectTable.Rows[i].Cells[1].Style.BackColor = Color.Red;
                    dgd_PkgDefectTable.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgd_PkgDefectTable.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                    dgd_PkgDefectTable.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PkgDefectTable.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                    dgd_PkgDefectTable.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    dgd_PkgDefectTable.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_PkgDefectTable.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }

                if (arrDefectList[i][3].Contains("-1"))
                    dgd_PkgDefectTable.Rows[i].Cells[2].Value = "---";
                else
                    dgd_PkgDefectTable.Rows[i].Cells[2].Value = arrDefectList[i][3];
                if ((intFailMask & 0x01) > 0)
                {
                    if (dgd_PkgDefectTable.Columns[2].Visible)
                        m_blnFailPackage[intPadIndex] = true;
                    dgd_PkgDefectTable.Rows[i].Cells[2].Style.BackColor = Color.Red;
                    dgd_PkgDefectTable.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                    dgd_PkgDefectTable.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                    dgd_PkgDefectTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PkgDefectTable.Rows[i].Cells[2].Style.BackColor = GetColor(dgd_PkgDefectTable.Rows[i].Cells[2].Value.ToString());
                    dgd_PkgDefectTable.Rows[i].Cells[2].Style.SelectionBackColor = GetColor(dgd_PkgDefectTable.Rows[i].Cells[2].Value.ToString());
                    dgd_PkgDefectTable.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                    dgd_PkgDefectTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                }

                if (arrDefectList[i][4].Contains("-1"))
                    dgd_PkgDefectTable.Rows[i].Cells[3].Value = "---";
                else
                    dgd_PkgDefectTable.Rows[i].Cells[3].Value = arrDefectList[i][4];
                if ((intFailMask & 0x02) > 0)
                {
                    if (dgd_PkgDefectTable.Columns[3].Visible)
                        m_blnFailPackage[intPadIndex] = true;
                    dgd_PkgDefectTable.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_PkgDefectTable.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_PkgDefectTable.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_PkgDefectTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PkgDefectTable.Rows[i].Cells[3].Style.BackColor = GetColor(dgd_PkgDefectTable.Rows[i].Cells[3].Value.ToString());
                    dgd_PkgDefectTable.Rows[i].Cells[3].Style.SelectionBackColor = GetColor(dgd_PkgDefectTable.Rows[i].Cells[3].Value.ToString());
                    dgd_PkgDefectTable.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_PkgDefectTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }

                dgd_PkgDefectTable.Rows[i].Cells[4].Value = arrDefectList[i][5];
                if ((intFailMask & 0x0C) > 0)
                {
                    if (dgd_PkgDefectTable.Columns[4].Visible)
                        m_blnFailPackage[intPadIndex] = true;
                    dgd_PkgDefectTable.Rows[i].Cells[4].Style.BackColor = Color.Red;
                    dgd_PkgDefectTable.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
                    dgd_PkgDefectTable.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                    dgd_PkgDefectTable.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PkgDefectTable.Rows[i].Cells[4].Style.BackColor = Color.Lime;
                    dgd_PkgDefectTable.Rows[i].Cells[4].Style.SelectionBackColor = Color.Lime;
                    dgd_PkgDefectTable.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                    dgd_PkgDefectTable.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                }
            }
            m_smVisionInfo.g_arrPad[intPadIndex].ref_blnViewPkgResultDrwaing = true;
        }

        private void UpdateTabPage()
        {
            switch (m_intSettingType)
            {
                case 0:                     
                    tp_Pad.Controls.Add(pnl_PadIndex);
                    break;
                case 1:
                    tp_Package.Controls.Add(pnl_PadIndex);
                    break;
                //case 2:
                //    tp_Position.Controls.Add(pnl_PadIndex);
                //    break;
            }
        }

        public void OnOffTimer(bool blnOn)
        {
            timer_PadResult.Enabled = blnOn;
        }

        public bool GetTimerStatus()
        {
            return timer_PadResult.Enabled;
        }

        public void CloseOfflinePage()
        {
            m_smVisionInfo.g_objPositioning.ref_blnDrawPHResult = false;
            m_smVisionInfo.g_blnViewOfflinePage = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewThreshold = false;
            m_smVisionInfo.g_blnTemplateManualSelect = false;
            m_smVisionInfo.g_blnPadInpected = false;
            m_smVisionInfo.g_blnViewPadInspection = false;
            m_smVisionInfo.g_blnInspectAllTemplate = true;
            m_smVisionInfo.g_blnDrawPin1Result = false;
            m_smVisionInfo.AT_VM_ManualTestMode = false;

            if (m_smVisionInfo.g_objPositioning != null)
                m_smVisionInfo.g_objPositioning.ref_bFinalResult = false;

            // Clear ROI drag handler
            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPadROIs[i].Count > 0)
                {
                    if (m_smVisionInfo.g_arrPadROIs[i][0].GetROIHandle())
                        m_smVisionInfo.g_arrPadROIs[i][0].ClearDragHandle();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                m_smVisionInfo.g_arrPad[i].ref_blnViewPkgSizeDrawing = false;
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;
                m_smVisionInfo.g_arrPad[i].ref_blnViewPkgResultDrwaing = false;
            }

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_intProductionViewImage;
            m_smVisionInfo.g_blnUpdateImageNoComboBox = true;
            m_smVisionInfo.VM_AT_OfflinePageView = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            this.Hide();
        }

        private void Vision3OfflinePage_Load(object sender, EventArgs e)
        {
        }

        private void Vision3OfflinePage_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            m_blnFailPad = new bool[5] { false, false, false, false, false };
            m_blnFailPackage = new bool[5] { false, false, false, false, false };
            m_blnFailOrient = false;
            m_blnFailPackageSize = false;
            m_blnFailPin1 = false;
            m_blnFailPosition = false;
            m_blnFailPH = false;
            UpdateTabPageHeaderImage();

            //m_STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Offline Test Page Closed", "Exit Offline Test Page", "", "", m_smProductionInfo.g_strLotID);
            m_smVisionInfo.g_objPositioning.ref_blnDrawPHResult = false;
            m_smVisionInfo.g_blnViewOfflinePage = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewThreshold = false;
            m_smVisionInfo.g_blnTemplateManualSelect = false;
            m_smVisionInfo.g_blnPadInpected = false;
            m_smVisionInfo.g_blnViewPadInspection = false;
            m_smVisionInfo.g_blnViewOrientObject = false;
            m_smVisionInfo.g_blnInspectAllTemplate = true;
            m_smVisionInfo.g_blnDrawPin1Result = false;
            m_smVisionInfo.AT_VM_ManualTestMode = false;

            if (m_smVisionInfo.g_objPositioning != null)
                m_smVisionInfo.g_objPositioning.ref_bFinalResult = false;

            // Clear ROI drag handler
            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPadROIs[i].Count > 0)
                {
                    if (m_smVisionInfo.g_arrPadROIs[i][0].GetROIHandle())
                        m_smVisionInfo.g_arrPadROIs[i][0].ClearDragHandle();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                m_smVisionInfo.g_arrPad[i].ref_blnViewPkgSizeDrawing = false;
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;
                m_smVisionInfo.g_arrPad[i].ref_blnViewPkgResultDrwaing = false;
            }

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_intProductionViewImage;
            m_smVisionInfo.g_blnUpdateImageNoComboBox = true;
            m_smVisionInfo.VM_AT_OfflinePageView = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            this.Hide();
        }

        private void btn_Inspect_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objPositioning.ref_blnDrawPHResult = false;
            m_smVisionInfo.g_blnViewPadSettingDrawing = false;

            if (m_smVisionInfo.g_blnDrawing)
                return;

            //m_STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Start Offline Test", " Pressed Test Button", "", "", m_smProductionInfo.g_strLotID);

            // 29-07-2019 ZJYEOH : If user pressed test button, then Image should stop live
            if (m_smVisionInfo.AT_PR_StartLiveImage)
            {
                m_smVisionInfo.AT_PR_StartLiveImage = false;
                m_smVisionInfo.AT_PR_TriggerLiveImage = true;
            }

            btn_Inspect.Enabled = false;

            if (chk_WantCheckPH.Checked)
            {
                m_smVisionInfo.g_blnViewPHImage = true;
                m_smVisionInfo.g_blnCheckPH = true;
            }
            else
            {
                m_smVisionInfo.g_blnViewPHImage = false;
                m_smVisionInfo.g_blnCheckPH = false;
            }

            if (chk_Grab.Checked)
                m_smVisionInfo.MN_PR_GrabImage = true;

            if (m_smVisionInfo.g_blnWantCheckPH)
            {
                m_smVisionInfo.g_intSelectedImage = 0;
                if (m_smVisionInfo.g_arrPHROIs.Count > 0)
                    m_smVisionInfo.g_arrPHROIs[0].AttachImage(m_smVisionInfo.g_arrImages[0]);
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
                {
                    if (m_smVisionInfo.g_arrPadROIs[i].Count > 0)
                        m_smVisionInfo.g_arrPadROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
                }
            }
            m_smVisionInfo.AT_VM_ManualTestMode = true;

            m_smVisionInfo.PR_MN_TestDone = false;
            m_smVisionInfo.MN_PR_StartTest = true;

        }

        private void timer_PadResult_Tick(object sender, EventArgs e)
        {
            if (!m_smVisionInfo.VM_AT_OfflinePageView)
                return;
            
            if (m_smVisionInfo.PR_MN_UpdateSettingInfo)
            {
                UpdatePageSetting();
                m_smVisionInfo.g_blnUpdateImageNoComboBox = true; // 2020-11-22 ZJYEOH : After exit any setting form Update image combo box because g_intProductionViewImage may not same with g_intSelectedImage
                m_smVisionInfo.PR_MN_UpdateSettingInfo = false;
            }

            if (m_smVisionInfo.PR_MN_UpdateInfo)
            {
                m_blnFailPad = new bool[5] { false, false, false, false, false };
                m_blnFailPackage = new bool[5] { false, false, false, false, false };
                m_blnFailOrient = false;
                m_blnFailPackageSize = false;
                m_blnFailPin1 = false;
                m_blnFailPosition = false;
                m_blnFailPH = false;
                UpdateTabPageHeaderImage();
                UpdateInfo();

                //2020-05-27 ZJYEOH : Just to get other pad direction result pass or fail
                UpdatePadFailSign();
                UpdatePkgFailSign();

                UpdateScore(m_intPadIndex, m_dgdView[0]);

                if ((m_smCustomOption.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    UpdatePositionOrient(0);
                else
                {
                    if (m_smVisionInfo.g_blnCheckPad)
                        UpdatePosition(0); //m_intPadIndex
                }
                UpdatePin1();
                UpdatePkgResultTable(m_intPadIndex, dgd_PadPackage);
                UpdateDefectTable(m_intPadIndex, m_dgdDefectTable[0]);
                if ((m_smCustomOption.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    UpdateColorDefectTable(m_intPadIndex, m_dgdDefectTable[0]);
                UpdatePkgDefectTable(m_intPadIndex, m_dgdPkgDefectTable[0]);
                //if (m_objPadMeasureSettingForm != null)
                //{
                //    if (m_objPadMeasureSettingForm.ref_blnFormOpen)
                //        m_objPadMeasureSettingForm.ref_blnUpdateInfo = true;
                //}
                UpdateTabPageHeaderImage();
                m_smVisionInfo.PR_MN_UpdateInfo = false;
                btn_Inspect.Enabled = true;
            }

            if (m_smVisionInfo.g_blnUpdateSelectedROI)
            {
                m_smVisionInfo.g_blnUpdateSelectedROI = false;

                for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
                {
                    if (m_smVisionInfo.g_arrPadROIs[i][0].GetROIHandle())
                    {
                        switch(i)
                        {
                            case 0:
                                radioBtn_Middle.Checked = true;
                                m_intPadIndex = 0;
                                break;
                            case 1:
                                radioBtn_Up.Checked = true;
                                m_intPadIndex = 1;
                                break;
                            case 2:
                                radioBtn_Right.Checked = true;
                                m_intPadIndex = 2;
                                break;
                            case 3:
                                radioBtn_Down.Checked = true;
                                m_intPadIndex = 3;
                                break;
                            case 4:
                                radioBtn_Left.Checked = true;
                                m_intPadIndex = 4;
                                break;
                        }
                    }
                }
                UpdateTabPageHeaderImage();
                if ((m_smCustomOption.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    UpdatePositionOrient(0);
                else
                {
                    if (m_smVisionInfo.g_blnCheckPad)
                        UpdatePosition(0); //m_intPadIndex
                }
                UpdatePin1();
                ReadPadTemplateDataToGrid(m_intPadIndex, m_dgdView[0]);

                //2020-05-27 ZJYEOH : Just to get other pad direction result pass or fail
                UpdatePadFailSign();
                UpdatePkgFailSign();

                UpdateScore(m_intPadIndex, m_dgdView[0]);
                UpdatePkgResultTable(m_intPadIndex, dgd_PadPackage);
                UpdateDefectTable(m_intPadIndex, m_dgdDefectTable[0]);
                if ((m_smCustomOption.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    UpdateColorDefectTable(m_intPadIndex, m_dgdDefectTable[0]);
                UpdatePkgDefectTable(m_intPadIndex, m_dgdPkgDefectTable[0]);
                UpdateTabPageHeaderImage();
            }

            if (m_smVisionInfo.g_blnViewPHImage && m_smVisionInfo.g_blnWantCheckPH)
                chk_WantCheckPH.Checked = true;
        }

        private void radioBtn_PadIndex_Click(object sender, EventArgs e)
        {
            if (sender == radioBtn_Middle)
            {
                m_intPadIndex = 0;
                m_smVisionInfo.g_intSelectedROIMask = 0x01;
                m_smVisionInfo.g_intSelectedROI = 0;
            }
            else if (sender == radioBtn_Up)
            {
                m_intPadIndex = 1;
                m_smVisionInfo.g_intSelectedROIMask = 0x02;
                m_smVisionInfo.g_intSelectedROI = 1;
            }
            else if (sender == radioBtn_Right)
            {
                m_intPadIndex = 2;
                m_smVisionInfo.g_intSelectedROIMask = 0x04;
                m_smVisionInfo.g_intSelectedROI = 2;
            }
            else if (sender == radioBtn_Down)
            {
                m_intPadIndex = 3;
                m_smVisionInfo.g_intSelectedROIMask = 0x08;
                m_smVisionInfo.g_intSelectedROI = 3;
            }
            else if (sender == radioBtn_Left)
            {
                m_intPadIndex = 4;
                m_smVisionInfo.g_intSelectedROIMask = 0x10;
                m_smVisionInfo.g_intSelectedROI = 4;
            }

            // Clear ROI drag handler
            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPadROIs[i][0].GetROIHandle())
                    m_smVisionInfo.g_arrPadROIs[i][0].ClearDragHandle();
            }

            m_smVisionInfo.g_blnPadSelecting = false;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                m_smVisionInfo.g_arrPad[i].ref_blnPkgDefectSelected = false;
            }
            m_smVisionInfo.g_intPadPkgDefectSelectedNumber = -1;
            m_smVisionInfo.g_intPadDefectSelectedNumber = -1;

            UpdatePageSetting();
            ReadPadTemplateDataToGrid(m_intPadIndex, m_dgdView[0]);
            UpdateScore(m_intPadIndex, m_dgdView[0]);
            UpdatePkgResultTable(m_intPadIndex, dgd_PadPackage);
            UpdateDefectTable(m_intPadIndex, m_dgdDefectTable[0]);
            if ((m_smCustomOption.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                UpdateColorDefectTable(m_intPadIndex, m_dgdDefectTable[0]);
            UpdatePkgDefectTable(m_intPadIndex, m_dgdPkgDefectTable[0]);
            if ((m_smCustomOption.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                UpdatePositionOrient(0);
            else
            {
                if (m_smVisionInfo.g_blnCheckPad)
                    UpdatePosition(0); //m_intPadIndex
            }
           
            UpdatePin1();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void tab_Result_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == tp_Pad)
            {
                m_intSettingType = 0;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_Package)
            {
                m_intSettingType = 1;
                UpdateTabPage();
            }
            //else if (e.TabPage == tp_Position)
            //{
            //    m_intSettingType = 2;
            //    UpdateTabPage();
            //}
        }

        public void LoadEvent()
        {
            // 2019 01 11 - CCENG: Don't put this function in Load Form Event function because the Form_Load event will cannot be triggered when you hide and show it again.
            Cursor.Current = Cursors.Default;
            m_smVisionInfo.g_blnViewPadInspection = true;
            m_smVisionInfo.VM_AT_OfflinePageView = true;
        }

        private void dgd_PkgDefect_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int r = e.RowIndex;

            m_smVisionInfo.g_intPadPkgDefectSelectedNumber = r;
            m_smVisionInfo.g_blnPadSelecting = true;
            m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnPkgDefectSelected = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_MiddleDefect_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int r = e.RowIndex;

            m_smVisionInfo.g_intPadDefectSelectedNumber = r;
            m_smVisionInfo.g_blnPadSelecting = true;
            m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnPadDefectSelected = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void UpdateTabPageHeaderImage()
        {

            if (m_blnFailPad[0] || m_blnFailPackage[0])
                radioBtn_Middle.ForeColor = Color.Red;
            else
                radioBtn_Middle.ForeColor = Color.Black;

            if (m_blnFailPad[1] || m_blnFailPackage[1])
                radioBtn_Up.ForeColor = Color.Red;
            else
                radioBtn_Up.ForeColor = Color.Black;

            if (m_blnFailPad[2] || m_blnFailPackage[2])
                radioBtn_Right.ForeColor = Color.Red;
            else
                radioBtn_Right.ForeColor = Color.Black;

            if (m_blnFailPad[3] || m_blnFailPackage[3])
                radioBtn_Down.ForeColor = Color.Red;
            else
                radioBtn_Down.ForeColor = Color.Black;

            if (m_blnFailPad[4] || m_blnFailPackage[4])
                radioBtn_Left.ForeColor = Color.Red;
            else
                radioBtn_Left.ForeColor = Color.Black;


            if (m_blnFailPad[0] || m_blnFailPad[1] || m_blnFailPad[2] || m_blnFailPad[3] || m_blnFailPad[4])
                tp_Pad.ImageIndex = 1;
            else
                tp_Pad.ImageIndex = 0;

            if (m_blnFailPackageSize || m_blnFailPackage[0] || m_blnFailPackage[1] || m_blnFailPackage[2] || m_blnFailPackage[3] || m_blnFailPackage[4])
                tp_Package.ImageIndex = 1;
            else
                tp_Package.ImageIndex = 0;

            if (m_blnFailPH)
                tp_PH.ImageIndex = 1;
            else
                tp_PH.ImageIndex = 0;

            if (m_blnFailPosition)
                tp_Position.ImageIndex = 1;
            else
                tp_Position.ImageIndex = 0;

            if (m_blnFailPin1)
                tp_Pin1.ImageIndex = 1;
            else
                tp_Pin1.ImageIndex = 0;


            if (m_blnFailOrient)
                tp_Orient.ImageIndex = 1;
            else
                tp_Orient.ImageIndex = 0;

        }

        private Color GetColor(string strValue)
        {
            if (strValue == "---")
                return Color.White;
            else
                return Color.Lime;

        }

        private void chk_WantCheckPH_CheckedChanged(object sender, EventArgs e)
        {
            if (!chk_WantCheckPH.Checked)
            {
                m_smVisionInfo.g_blnViewPHImage = false;
                m_smVisionInfo.g_blnUpdateImageNoComboBox = false;
            }
            else
            {
                m_smVisionInfo.g_blnViewPHImage = true;
                m_smVisionInfo.g_blnUpdateImageNoComboBox = true;
                m_smVisionInfo.g_intSelectedImage = 0;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }
    }
}