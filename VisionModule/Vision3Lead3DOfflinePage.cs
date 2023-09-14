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
    public partial class Vision3Lead3DOfflinePage : Form
    {
        #region Member Variables
        private bool m_blnFailLead3D = false;
        private bool m_blnFailPosition = false;
        private bool m_blnFailPackage = false;
        private bool m_blnFailPH = false;
        private bool m_blnFailPin1 = false;

        private DataGridView[] m_dgdView = new DataGridView[1];
        private DataGridView[] m_dgdDefectTable = new DataGridView[1];
        private DataGridView[] m_dgdExtraDefectTable = new DataGridView[1];
        private DataGridView[] m_dgdPkgDefectTable = new DataGridView[1];

        private CustomOption m_smCustomOption;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo m_smVisionInfo;
        private int m_intUserGroup = 5;
        private UserRight m_objUserRight = new UserRight();
        
        #endregion

        public Vision3Lead3DOfflinePage(CustomOption smCustomOption, ProductionInfo smProductionInfo, VisionInfo smVisionInfo, int intUserGroup)
        {
            InitializeComponent();
            m_intUserGroup = intUserGroup;
            m_smCustomOption = smCustomOption;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            
            m_dgdView[0] = dgd_Lead3D_EachLeadDimension;
            m_dgdDefectTable[0] = dgd_Lead3D_GroupDimension2;
            m_dgdExtraDefectTable[0] = dgd_Lead3D_GroupDimension;
            m_dgdPkgDefectTable[0] = dgd_PkgDefect;
            CustomizeGUI();
        }

        public void CustomizeGUI()
        {
            //UpdatePosition();
            dgd_PositionResult.Rows.Clear();

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

                //UpdatePin1();
                if ((m_smVisionInfo.g_blnWantPin1 && m_smVisionInfo.g_arrPin1 != null && m_smVisionInfo.g_arrPin1.Count > 0 && m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(0)))
                    dgd_Pin1Result.Rows.Clear();
            }
            else
            {
                if (tab_Result.TabPages.Contains(tp_Pin1))
                    tab_Result.Controls.Remove(tp_Pin1);
            }

            ReadLeadTemplateDataToGrid(0, m_dgdView[0]);
            m_dgdExtraDefectTable[0].Visible = true;
            if (!m_smVisionInfo.g_arrLead3D[0].ref_blnWantCheckExtraLeadLength)
            {
                m_dgdExtraDefectTable[0].Columns[2].Visible = false;
                m_dgdExtraDefectTable[0].Columns[3].Visible = false;
            }
            else
            {
                m_dgdExtraDefectTable[0].Columns[2].Visible = true;
                m_dgdExtraDefectTable[0].Columns[3].Visible = true;
            }
            if (!m_smVisionInfo.g_arrLead3D[0].ref_blnWantCheckExtraLeadArea && ((m_smVisionInfo.g_arrLead3D[0].ref_intFailOptionMask & 0x2000) == 0))
                m_dgdExtraDefectTable[0].Columns[4].Visible = false;
            else
                m_dgdExtraDefectTable[0].Columns[4].Visible = true;

            if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadDirection == 0) // Horizontal
            {
                m_dgdDefectTable[0].Columns[2].Visible = true; // Left
                m_dgdDefectTable[0].Columns[3].Visible = true; // Right
                m_dgdDefectTable[0].Columns[4].Visible = false; // Top
                m_dgdDefectTable[0].Columns[5].Visible = false; // Bottom
            }
            else
            {
                m_dgdDefectTable[0].Columns[2].Visible = false; // Left
                m_dgdDefectTable[0].Columns[3].Visible = false; // Right
                m_dgdDefectTable[0].Columns[4].Visible = true; // Top
                m_dgdDefectTable[0].Columns[5].Visible = true; // Bottom
            }

            int intFailOptionMask = m_smVisionInfo.g_arrLead3D[0].ref_intFailOptionMask;

            m_dgdView[0].Columns[0].Visible = ((intFailOptionMask & 0x20000) > 0);
            m_dgdView[0].Columns[1].Visible = ((intFailOptionMask & 0x100) > 0);
            m_dgdView[0].Columns[2].Visible = ((intFailOptionMask & 0x40) > 0);
            m_dgdView[0].Columns[3].Visible = ((intFailOptionMask & 0x80) > 0);
            m_dgdView[0].Columns[4].Visible = ((intFailOptionMask & 0x200) > 0);
            m_dgdView[0].Columns[5].Visible = ((intFailOptionMask & 0x400) > 0);
            m_dgdView[0].Columns[6].Visible = ((intFailOptionMask & 0x01) > 0);
            m_dgdView[0].Columns[7].Visible = ((intFailOptionMask & 0x08) > 0);
            m_dgdView[0].Columns[8].Visible = ((intFailOptionMask & 0x02) > 0);
            m_dgdView[0].Columns[9].Visible = ((intFailOptionMask & 0x40000) > 0);
            m_dgdView[0].Columns[10].Visible = ((intFailOptionMask & 0x100000) > 0);
            m_dgdView[0].Columns[11].Visible = ((intFailOptionMask & 0x100000) > 0);
            m_dgdView[0].Columns[12].Visible = ((intFailOptionMask & 0x200000) > 0);

            if (!m_smVisionInfo.g_arrLead3D[0].ref_blnWantUseAverageGrayValueMethod)
            {
                if (m_dgdView[0].Columns[9].Visible)
                    m_dgdView[0].Columns[9].Visible = false;
            }

            m_dgdExtraDefectTable[0].Rows.Clear();
            m_dgdDefectTable[0].Rows.Clear();

            //if (m_smVisionInfo.g_arrLead3D[0].ref_blnWantUsePkgToBaseTolerance)
            //{
            //    if (m_dgdView[0].Columns[1].Visible)
            //        m_dgdView[0].Columns[1].Visible = false;
            //}

            //---------------------------------------- Lead3D Package ----------------------------------------
            dgd_Lead3DPackage.Rows.Clear();
            dgd_PkgDefect.Rows.Clear();
          
                dgd_Lead3DPackage.Columns[3].Visible = false;
            
            // Set display events
            //m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (m_smVisionInfo.g_strVisionName == "Lead3D" || m_smVisionInfo.g_strVisionName == "Li3D")
                tab_Result.Controls.Remove(tp_Package);
        }

        private void ReadLeadTemplateDataToGrid(int intLeadIndex, DataGridView dgd_LeadSetting)
        {
            List<List<string>> arrBlobsFeatures = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetBlobsFeaturesInspectRealData();

            dgd_LeadSetting.Rows.Clear();
            for (int i = 0; i < arrBlobsFeatures.Count; i++)
            {
                dgd_LeadSetting.Rows.Add();
                dgd_LeadSetting.Rows[i].HeaderCell.Value = "Lead " + (i + 1);
            }
        }

        private void UpdateInfo()
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

            m_smVisionInfo.g_blnLead3DSelecting = false;
            m_smVisionInfo.g_blnUpdateLead3DSetting = true;
            m_smVisionInfo.g_intLead3DSelectedNumber = -1;
            m_smVisionInfo.g_intLead3DPkgDefectSelectedNumber = -1;
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.g_blnViewLeadInspection = true;
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

        private void UpdateScore(int intLeadIndex, DataGridView dgd_LeadSetting)
        {
            for (int i = 0; i < dgd_LeadSetting.Rows.Count; i++)
            {
                List<string> arrResultList = m_smVisionInfo.g_arrLead3D[0].GetBlobFeaturesResult_WithPassFailIndicator(i);

                int intFailMask = Convert.ToInt32(arrResultList[arrResultList.Count - 1]);

                // Offset
                dgd_LeadSetting.Rows[i].Cells[0].Value = arrResultList[0];
                if ((intFailMask & 0x20000) > 0)
                {
                    if (dgd_LeadSetting.Columns[0].Visible)
                        m_blnFailLead3D = true;
                    dgd_LeadSetting.Rows[i].Cells[0].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[0].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[0].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_LeadSetting.Rows[i].Cells[0].Value.ToString() == "---")
                    {
                        dgd_LeadSetting.Rows[i].Cells[0].Style.BackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[0].Style.SelectionBackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[0].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[0].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[0].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[0].Style.SelectionForeColor = Color.Black;
                    }
                }

                // Skew
                if (!m_smVisionInfo.g_arrLead3D[0].GetWantCheckSkew(i))
                {
                    dgd_LeadSetting.Rows[i].Cells[1].Value = "---";
                    dgd_LeadSetting.Rows[i].Cells[1].Style.BackColor = Color.White;
                    dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White;
                    dgd_LeadSetting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_LeadSetting.Rows[i].Cells[1].Value = arrResultList[1];
                    if ((intFailMask & 0x100) > 0)
                    {
                        if (dgd_LeadSetting.Columns[1].Visible)
                            m_blnFailLead3D = true;
                        dgd_LeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                        dgd_LeadSetting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                        dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        if (dgd_LeadSetting.Rows[i].Cells[1].Value.ToString() == "---")
                        {
                            dgd_LeadSetting.Rows[i].Cells[1].Style.BackColor = Color.White;
                            dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White;
                            dgd_LeadSetting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                            dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_LeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                            dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                            dgd_LeadSetting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                            dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                        }
                    }
                }

                // Width
                dgd_LeadSetting.Rows[i].Cells[2].Value = arrResultList[2];
                if ((intFailMask & 0x01) > 0)
                {
                    if (dgd_LeadSetting.Columns[2].Visible)
                        m_blnFailLead3D = true;
                    dgd_LeadSetting.Rows[i].Cells[2].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_LeadSetting.Rows[i].Cells[2].Value.ToString() == "---")
                    {
                        dgd_LeadSetting.Rows[i].Cells[2].Style.BackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                    }
                }

                // Length
                dgd_LeadSetting.Rows[i].Cells[3].Value = arrResultList[3];
                if ((intFailMask & 0x02) > 0)
                {
                    if (dgd_LeadSetting.Columns[3].Visible)
                        m_blnFailLead3D = true;
                    dgd_LeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_LeadSetting.Rows[i].Cells[3].Value.ToString() == "---")
                    {
                        dgd_LeadSetting.Rows[i].Cells[3].Style.BackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }

                // Pitch
                dgd_LeadSetting.Rows[i].Cells[4].Value = arrResultList[4];
                if ((intFailMask & 0x04) > 0)
                {
                    if (dgd_LeadSetting.Columns[4].Visible)
                        m_blnFailLead3D = true;
                    dgd_LeadSetting.Rows[i].Cells[4].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_LeadSetting.Rows[i].Cells[4].Value.ToString() == "---")
                    {
                        dgd_LeadSetting.Rows[i].Cells[4].Style.BackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i].Cells[4].Style.BackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                    }
                }

                // Gap
                dgd_LeadSetting.Rows[i].Cells[5].Value = arrResultList[5];
                if ((intFailMask & 0x08) > 0)
                {
                    if (dgd_LeadSetting.Columns[5].Visible)
                        m_blnFailLead3D = true;
                    dgd_LeadSetting.Rows[i].Cells[5].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[5].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[5].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[5].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_LeadSetting.Rows[i].Cells[5].Value.ToString() == "---")
                    {
                        dgd_LeadSetting.Rows[i].Cells[5].Style.BackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[5].Style.SelectionBackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[5].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[5].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i].Cells[5].Style.BackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[5].Style.SelectionBackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[5].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[5].Style.SelectionForeColor = Color.Black;
                    }
                }

                // Stand Off
                dgd_LeadSetting.Rows[i].Cells[6].Value = arrResultList[6];
                if ((intFailMask & 0x10) > 0)
                {
                    if (dgd_LeadSetting.Columns[6].Visible)
                        m_blnFailLead3D = true;
                    dgd_LeadSetting.Rows[i].Cells[6].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[6].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[6].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[6].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_LeadSetting.Rows[i].Cells[6].Value.ToString() == "---")
                    {
                        dgd_LeadSetting.Rows[i].Cells[6].Style.BackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[6].Style.SelectionBackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[6].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[6].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i].Cells[6].Style.BackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[6].Style.SelectionBackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[6].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[6].Style.SelectionForeColor = Color.Black;
                    }
                }

                // Solder Pad Length
                dgd_LeadSetting.Rows[i].Cells[7].Value = arrResultList[7];
                if ((intFailMask & 0x20) > 0)
                {
                    if (dgd_LeadSetting.Columns[7].Visible)
                        m_blnFailLead3D = true;
                    dgd_LeadSetting.Rows[i].Cells[7].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[7].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_LeadSetting.Rows[i].Cells[7].Value.ToString() == "---")
                    {
                        dgd_LeadSetting.Rows[i].Cells[7].Style.BackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionBackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[7].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i].Cells[7].Style.BackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionBackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[7].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionForeColor = Color.Black;
                    }
                }

                // Coplan
                dgd_LeadSetting.Rows[i].Cells[8].Value = arrResultList[8];
                if ((intFailMask & 0x40) > 0)
                {
                    if (dgd_LeadSetting.Columns[8].Visible)
                        m_blnFailLead3D = true;
                    dgd_LeadSetting.Rows[i].Cells[8].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[8].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[8].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[8].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_LeadSetting.Rows[i].Cells[8].Value.ToString() == "---")
                    {
                        dgd_LeadSetting.Rows[i].Cells[8].Style.BackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[8].Style.SelectionBackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[8].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[8].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i].Cells[8].Style.BackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[8].Style.SelectionBackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[8].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[8].Style.SelectionForeColor = Color.Black;
                    }
                }

                // Average Gray Value
                dgd_LeadSetting.Rows[i].Cells[9].Value = arrResultList[9];
                if ((intFailMask & 0x80) > 0)
                {
                    if (dgd_LeadSetting.Columns[9].Visible)
                        m_blnFailLead3D = true;
                    dgd_LeadSetting.Rows[i].Cells[9].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[9].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[9].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[9].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_LeadSetting.Rows[i].Cells[9].Value.ToString() == "---")
                    {
                        dgd_LeadSetting.Rows[i].Cells[9].Style.BackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[9].Style.SelectionBackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[9].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[9].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i].Cells[9].Style.BackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[9].Style.SelectionBackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[9].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[9].Style.SelectionForeColor = Color.Black;
                    }
                }

                // Lead Min Width
                dgd_LeadSetting.Rows[i].Cells[10].Value = arrResultList[10];
                if ((intFailMask & 0x200) > 0)
                {
                    dgd_LeadSetting.Rows[i].Cells[10].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[10].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[10].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[10].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_LeadSetting.Rows[i].Cells[10].Value.ToString() == "---")
                    {
                        dgd_LeadSetting.Rows[i].Cells[10].Style.BackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[10].Style.SelectionBackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[10].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i].Cells[10].Style.BackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[10].Style.SelectionBackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[10].Style.SelectionForeColor = Color.Black;
                    }
                }

                // Lead Max Width
                dgd_LeadSetting.Rows[i].Cells[11].Value = arrResultList[11];
                if ((intFailMask & 0x400) > 0)
                {
                    dgd_LeadSetting.Rows[i].Cells[11].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[11].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_LeadSetting.Rows[i].Cells[11].Value.ToString() == "---")
                    {
                        dgd_LeadSetting.Rows[i].Cells[11].Style.BackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionBackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i].Cells[11].Style.BackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionBackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionForeColor = Color.Black;
                    }
                }

                // Lead Max Burr
                dgd_LeadSetting.Rows[i].Cells[12].Value = arrResultList[12];
                if ((intFailMask & 0x800) > 0)
                {
                    dgd_LeadSetting.Rows[i].Cells[12].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[12].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[12].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[12].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_LeadSetting.Rows[i].Cells[12].Value.ToString() == "---")
                    {
                        dgd_LeadSetting.Rows[i].Cells[12].Style.BackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[12].Style.SelectionBackColor = Color.White;
                        dgd_LeadSetting.Rows[i].Cells[12].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[12].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i].Cells[12].Style.BackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[12].Style.SelectionBackColor = Color.Lime;
                        dgd_LeadSetting.Rows[i].Cells[12].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i].Cells[12].Style.SelectionForeColor = Color.Black;
                    }
                }

                //Missing Lead
                if ((intFailMask & 0x1000) > 0)
                {
                    for (int j = 0; j < dgd_LeadSetting.ColumnCount; j++)
                    {
                        dgd_LeadSetting.Rows[i].Cells[j].Style.BackColor = Color.Red;
                        dgd_LeadSetting.Rows[i].Cells[j].Style.SelectionBackColor = Color.Red;
                        dgd_LeadSetting.Rows[i].Cells[j].Style.ForeColor = Color.Yellow;
                        dgd_LeadSetting.Rows[i].Cells[j].Style.SelectionForeColor = Color.Yellow;
                    }
                }

            }
        }

        private void UpdateDefectTable(int intLeadIndex, DataGridView dgd_DefectTable)
        {
            dgd_DefectTable.Rows.Clear();

            List<List<string>> arrDefectList = new List<List<string>>();
            arrDefectList = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetDefectList();
            for (int i = 0; i < arrDefectList.Count; i++)
            {
                dgd_DefectTable.Rows.Add();
                dgd_DefectTable.Rows[i].Cells[0].Value = (i + 1).ToString();
                
                int intFailMask = Convert.ToInt32(arrDefectList[i][0]);
                
                dgd_DefectTable.Rows[i].Cells[1].Value = arrDefectList[i][1];
                if (intFailMask > 0)
                {
                    if (dgd_DefectTable.Columns[1].Visible)
                        m_blnFailLead3D = true;
                    dgd_DefectTable.Rows[i].Cells[1].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    //if (dgd_DefectTable.Rows[i].Cells[1].Value.ToString() == "---")
                    //{
                        dgd_DefectTable.Rows[i].Cells[1].Style.BackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[1].Style.SelectionBackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                    //}
                    //else
                    //{
                    //    dgd_DefectTable.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                    //    dgd_DefectTable.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    //    dgd_DefectTable.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    //    dgd_DefectTable.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                    //}
                }

                if (arrDefectList[i][2] == "Left")
                    dgd_DefectTable.Rows[i].Cells[2].Value = "---";
                else
                    dgd_DefectTable.Rows[i].Cells[2].Value = arrDefectList[i][2];
                if ((intFailMask & 0x01) > 0)
                {
                    if (dgd_DefectTable.Columns[2].Visible)
                        m_blnFailLead3D = true;
                    dgd_DefectTable.Rows[i].Cells[2].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_DefectTable.Rows[i].Cells[2].Value.ToString() == "---")
                    {
                        dgd_DefectTable.Rows[i].Cells[2].Style.BackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[2].Style.SelectionBackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_DefectTable.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                        dgd_DefectTable.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                        dgd_DefectTable.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                    }
                }

                if (arrDefectList[i][3] == "Right")
                    dgd_DefectTable.Rows[i].Cells[3].Value = "---";
                else
                    dgd_DefectTable.Rows[i].Cells[3].Value = arrDefectList[i][3];
                if ((intFailMask & 0x02) > 0)
                {
                    if (dgd_DefectTable.Columns[3].Visible)
                        m_blnFailLead3D = true;
                    dgd_DefectTable.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_DefectTable.Rows[i].Cells[3].Value.ToString() == "---")
                    {
                        dgd_DefectTable.Rows[i].Cells[3].Style.BackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[3].Style.SelectionBackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_DefectTable.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_DefectTable.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_DefectTable.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }

                if (arrDefectList[i][4] == "Top")
                    dgd_DefectTable.Rows[i].Cells[4].Value = "---";
                else
                    dgd_DefectTable.Rows[i].Cells[4].Value = arrDefectList[i][4];
                if ((intFailMask & 0x04) > 0)
                {
                    if (dgd_DefectTable.Columns[4].Visible)
                        m_blnFailLead3D = true;
                    dgd_DefectTable.Rows[i].Cells[4].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_DefectTable.Rows[i].Cells[4].Value.ToString() == "---")
                    {
                        dgd_DefectTable.Rows[i].Cells[4].Style.BackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[4].Style.SelectionBackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_DefectTable.Rows[i].Cells[4].Style.BackColor = Color.Lime;
                        dgd_DefectTable.Rows[i].Cells[4].Style.SelectionBackColor = Color.Lime;
                        dgd_DefectTable.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                    }
                }

                if (arrDefectList[i][5] == "Bottom")
                    dgd_DefectTable.Rows[i].Cells[5].Value = "---";
                else
                    dgd_DefectTable.Rows[i].Cells[5].Value = arrDefectList[i][5];
                if ((intFailMask & 0x08) > 0)
                {
                    if (dgd_DefectTable.Columns[5].Visible)
                        m_blnFailLead3D = true;
                    dgd_DefectTable.Rows[i].Cells[5].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[5].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[5].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i].Cells[5].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_DefectTable.Rows[i].Cells[5].Value.ToString() == "---")
                    {
                        dgd_DefectTable.Rows[i].Cells[5].Style.BackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[5].Style.SelectionBackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[5].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[5].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_DefectTable.Rows[i].Cells[5].Style.BackColor = Color.Lime;
                        dgd_DefectTable.Rows[i].Cells[5].Style.SelectionBackColor = Color.Lime;
                        dgd_DefectTable.Rows[i].Cells[5].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[5].Style.SelectionForeColor = Color.Black;
                    }
                }

                if (arrDefectList[i][6] == "Unit")
                    dgd_DefectTable.Rows[i].Cells[6].Value = "---";
                else
                    dgd_DefectTable.Rows[i].Cells[6].Value = arrDefectList[i][6];
                if ((intFailMask & 0x10) > 0)
                {
                    if (dgd_DefectTable.Columns[6].Visible)
                        m_blnFailLead3D = true;
                    dgd_DefectTable.Rows[i].Cells[6].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[6].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[6].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i].Cells[6].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_DefectTable.Rows[i].Cells[6].Value.ToString() == "---")
                    {
                        dgd_DefectTable.Rows[i].Cells[6].Style.BackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[6].Style.SelectionBackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[6].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[6].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_DefectTable.Rows[i].Cells[6].Style.BackColor = Color.Lime;
                        dgd_DefectTable.Rows[i].Cells[6].Style.SelectionBackColor = Color.Lime;
                        dgd_DefectTable.Rows[i].Cells[6].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[6].Style.SelectionForeColor = Color.Black;
                    }
                }
            }

            for (int i = 0; i < dgd_DefectTable.Rows.Count; i++)
            {
                switch (dgd_DefectTable.Rows[i].Cells[1].Value.ToString())
                {
                    case "Stand Off Variance":
                        if ((m_smVisionInfo.g_intOptionControlMask & 0x40) > 0 && m_dgdView[0].Rows[0].Cells[6].Value.ToString() == "---")
                        {
                            break;
                        }
                        else if ((m_smVisionInfo.g_intOptionControlMask & 0x40) > 0 && (m_smVisionInfo.g_intOptionControlMask & 0x20) > 0)
                        {
                            if (m_dgdView[0].Rows[0].Cells[6].Value.ToString() == "---")
                            {
                                dgd_DefectTable.Rows[i].Cells[2].Value = "---";
                                dgd_DefectTable.Rows[i].Cells[3].Value = "---";
                                dgd_DefectTable.Rows[i].Cells[4].Value = "---";
                                dgd_DefectTable.Rows[i].Cells[5].Value = "---";
                                dgd_DefectTable.Rows[i].Cells[6].Value = "---";
                            }
                            else
                                break;
                        }
                        break;
                    //case "Length Variance":
                    case "Leads Sweep":
                        for (int j = 0; j < m_dgdView[0].Rows.Count; j++)
                        {
                            if (m_dgdView[0].Rows[j].Cells[2].Value.ToString() == "0.0000")
                            {
                                int intFailDirection = m_smVisionInfo.g_arrLead3D[0].GetLeadFailDirection(j);
                                switch (intFailDirection)
                                {
                                    case 4:
                                        dgd_DefectTable.Rows[i].Cells[4].Value = "---";
                                        break;
                                    case 2:
                                        dgd_DefectTable.Rows[i].Cells[3].Value = "---";
                                        break;
                                    case 8:
                                        dgd_DefectTable.Rows[i].Cells[5].Value = "---";
                                        break;
                                    case 1:
                                        dgd_DefectTable.Rows[i].Cells[2].Value = "---";
                                        break;
                                }
                            }
                        }
                        break;
                    //case "Pitch Variance":
                    //    for (int j = 0; j < m_dgdView[0].Rows.Count; j++)
                    //    {
                    //        if (m_dgdView[0].Rows[j].Cells[3].Value.ToString() == "---")
                    //        {
                    //            int intFailDirection = m_smVisionInfo.g_arrLead3D[0].GetLeadPitchFailDirection(j);
                    //            switch (intFailDirection)
                    //            {
                    //                case 4:
                    //                    dgd_DefectTable.Rows[i].Cells[4].Value = "---";
                    //                    break;
                    //                case 2:
                    //                    dgd_DefectTable.Rows[i].Cells[3].Value = "---";
                    //                    break;
                    //                case 8:
                    //                    dgd_DefectTable.Rows[i].Cells[5].Value = "---";
                    //                    break;
                    //                case 1:
                    //                    dgd_DefectTable.Rows[i].Cells[2].Value = "---";
                    //                    break;
                    //            }
                    //        }
                    //    }
                    //    break;
                }
            }

            if (dgd_DefectTable.Rows.Count > 0)
            {
                dgd_DefectTable.Visible = true;
                if (dgd_DefectTable.Columns[2].Visible == true && dgd_DefectTable.Columns[3].Visible == true && dgd_DefectTable.Columns[4].Visible == true && dgd_DefectTable.Columns[5].Visible == true)
                    dgd_DefectTable.Size = new Size(dgd_DefectTable.Size.Width, (dgd_DefectTable.Rows.Count + 2) * 22);
                else if ((dgd_DefectTable.Columns[2].Visible == true && dgd_DefectTable.Columns[3].Visible == true) || (dgd_DefectTable.Columns[4].Visible == true && dgd_DefectTable.Columns[5].Visible == true))
                    dgd_DefectTable.Size = new Size(dgd_DefectTable.Size.Width, (dgd_DefectTable.Rows.Count + 1) * 22);
            }
            else
                dgd_DefectTable.Visible = false;
        }

        private void UpdateExtraDefectTable(int intLeadIndex, DataGridView dgd_DefectTable)
        {
            dgd_DefectTable.Rows.Clear();

            List<List<string>> arrDefectList = new List<List<string>>();
            arrDefectList = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetExtraDefectList();
            for (int i = 0; i < arrDefectList.Count; i++)
            {
                dgd_DefectTable.Rows.Add();
                dgd_DefectTable.Rows[i].Cells[0].Value = (i + 1).ToString();

                int intFailMask = Convert.ToInt32(arrDefectList[i][0]);

                dgd_DefectTable.Rows[i].Cells[1].Value = arrDefectList[i][1];
                if (intFailMask > 0)
                {
                    if (dgd_DefectTable.Columns[1].Visible)
                        m_blnFailLead3D = true;
                    dgd_DefectTable.Rows[i].Cells[1].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    //if (dgd_DefectTable.Rows[i].Cells[1].Value.ToString() == "---")
                    //{
                        dgd_DefectTable.Rows[i].Cells[1].Style.BackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[1].Style.SelectionBackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                    //}
                    //else
                    //{
                    //    dgd_DefectTable.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                    //    dgd_DefectTable.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    //    dgd_DefectTable.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    //    dgd_DefectTable.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                    //}
                }

                if (arrDefectList[i][2] == "-999.0000")
                    dgd_DefectTable.Rows[dgd_DefectTable.Rows.Count - 1].Cells[2].Value = "---";
                else
                    dgd_DefectTable.Rows[i].Cells[2].Value = arrDefectList[i][2];
                if ((intFailMask & 0x01) > 0)
                {
                    if (dgd_DefectTable.Columns[2].Visible)
                        m_blnFailLead3D = true;
                    dgd_DefectTable.Rows[i].Cells[2].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_DefectTable.Rows[i].Cells[2].Value.ToString() == "---")
                    {
                        dgd_DefectTable.Rows[i].Cells[2].Style.BackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[2].Style.SelectionBackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_DefectTable.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                        dgd_DefectTable.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                        dgd_DefectTable.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                    }
                }

                if (arrDefectList[i][3] == "-999.0000")
                    dgd_DefectTable.Rows[dgd_DefectTable.Rows.Count - 1].Cells[3].Value = "---";
                else
                    dgd_DefectTable.Rows[i].Cells[3].Value = arrDefectList[i][3];
                if ((intFailMask & 0x02) > 0)
                {
                    if (dgd_DefectTable.Columns[3].Visible)
                        m_blnFailLead3D = true;
                    dgd_DefectTable.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_DefectTable.Rows[i].Cells[3].Value.ToString() == "---")
                    {
                        dgd_DefectTable.Rows[i].Cells[3].Style.BackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[3].Style.SelectionBackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_DefectTable.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_DefectTable.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_DefectTable.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }

                if (arrDefectList[i][4] == "0.000000")
                    dgd_DefectTable.Rows[dgd_DefectTable.Rows.Count - 1].Cells[4].Value = "---";
                else
                    dgd_DefectTable.Rows[i].Cells[4].Value = arrDefectList[i][4];
                if ((intFailMask & 0x04) > 0)
                {
                    if (dgd_DefectTable.Columns[4].Visible)
                        m_blnFailLead3D = true;
                    dgd_DefectTable.Rows[i].Cells[4].Style.BackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
                    dgd_DefectTable.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                    dgd_DefectTable.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    if (dgd_DefectTable.Rows[i].Cells[4].Value.ToString() == "---")
                    {
                        dgd_DefectTable.Rows[i].Cells[4].Style.BackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[4].Style.SelectionBackColor = Color.White;
                        dgd_DefectTable.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                        dgd_DefectTable.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
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
            if (dgd_DefectTable.Rows.Count > 0)
            {
                dgd_DefectTable.Visible = true;
                dgd_DefectTable.Size = new Size(dgd_DefectTable.Size.Width, (dgd_DefectTable.Rows.Count + 1) * 22);
            }
            else
                dgd_DefectTable.Visible = false;
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

        public void OnOffTimer(bool blnOn)
        {
            timer_LeadResult.Enabled = blnOn;
        }

        public bool GetTimerStatus()
        {
            return timer_LeadResult.Enabled;
        }

        public void CloseOfflinePage()
        {
            m_smVisionInfo.g_blnViewLead3DPkgToBaseDrawing = false;
            m_smVisionInfo.g_blnViewLeadTipBuildAreaDrawing = false;
            m_smVisionInfo.g_blnViewOfflinePage = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewThreshold = false;
            m_smVisionInfo.g_blnTemplateManualSelect = false;
            m_smVisionInfo.g_blnLead3DInpected = false;
            m_smVisionInfo.g_blnViewLeadInspection = false;
            m_smVisionInfo.g_blnInspectAllTemplate = true;
            m_smVisionInfo.g_blnDrawPin1Result = false;
            m_smVisionInfo.AT_VM_ManualTestMode = false;

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_intProductionViewImage;
            m_smVisionInfo.g_blnUpdateImageNoComboBox = true;

            if (m_smVisionInfo.g_objPositioning != null)
                m_smVisionInfo.g_objPositioning.ref_bFinalResult = false;

            //m_smVisionInfo.g_intSelectedImage = 0;

            m_smVisionInfo.VM_AT_OfflinePageView = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            this.Hide();
        }

        private void Vision3OfflinePage_Load(object sender, EventArgs e)
        {
            //m_smVisionInfo.VM_AT_OfflinePageView = true;
        }

        private void Vision3OfflinePage_FormClosing(object sender, FormClosingEventArgs e)
        {
            //m_smVisionInfo.VM_AT_OfflinePageView = false;
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            m_blnFailLead3D = false;
            m_blnFailPosition = false;
            m_blnFailPackage = false;
            m_blnFailPH = false;
            m_blnFailPin1 = false;

            UpdateTabPageHeaderImage();

            //m_STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Offline Test Page Closed", "Exit Offline Test Page", "", "", m_smProductionInfo.g_strLotID);
            m_smVisionInfo.g_blnViewLead3DPkgToBaseDrawing = false;
            m_smVisionInfo.g_blnViewLeadTipBuildAreaDrawing = false;
            m_smVisionInfo.g_blnViewOfflinePage = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewThreshold = false;
            m_smVisionInfo.g_blnTemplateManualSelect = false;
            m_smVisionInfo.g_blnLead3DInpected = false;
            m_smVisionInfo.g_blnViewLeadInspection = false;
            m_smVisionInfo.g_blnInspectAllTemplate = true;
            m_smVisionInfo.g_blnDrawPin1Result = false;
            m_smVisionInfo.AT_VM_ManualTestMode = false;

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_intProductionViewImage;
            m_smVisionInfo.g_blnUpdateImageNoComboBox = true;

            if (m_smVisionInfo.g_objPositioning != null)
                m_smVisionInfo.g_objPositioning.ref_bFinalResult = false;

            //m_smVisionInfo.g_intSelectedImage = 0;

            m_smVisionInfo.VM_AT_OfflinePageView = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            this.Hide();
        }

        private void btn_Inspect_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewLead3DSettingDrawing = false;

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
                for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[i].Count > 0)
                        m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
                }
            }
          
            m_smVisionInfo.AT_VM_ManualTestMode = true;

            m_smVisionInfo.PR_MN_TestDone = false;
            m_smVisionInfo.MN_PR_StartTest = true;

        }

        private void timer_LeadResult_Tick(object sender, EventArgs e)
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
                m_blnFailLead3D = false;
                m_blnFailPosition = false;
                m_blnFailPackage = false;
                m_blnFailPH = false;
                m_blnFailPin1 = false;
                UpdateTabPageHeaderImage();
                UpdateInfo();
                UpdateScore(0, m_dgdView[0]);
                UpdatePosition();
                UpdatePin1();
                UpdateExtraDefectTable(0, m_dgdExtraDefectTable[0]);
                UpdateDefectTable(0, m_dgdDefectTable[0]);
                UpdatePkgResultTable(0, dgd_Lead3DPackage);
                UpdatePkgDefectTable(0, m_dgdPkgDefectTable[0]);
                UpdateTabPageHeaderImage();
                m_smVisionInfo.PR_MN_UpdateInfo = false;
                btn_Inspect.Enabled = true;
            }
        }

        public void LoadEvent()
        {
            // 2019 01 11 - CCENG: Don't put this function in Load Form Event function because the Form_Load event will cannot be triggered when you hide and show it again.
            Cursor.Current = Cursors.Default;
            //m_smVisionInfo.g_blnViewLeadInspection = true;
            m_smVisionInfo.VM_AT_OfflinePageView = true;
        }

        private void UpdatePosition()
        {
            dgd_PositionResult.Rows.Clear();
            float Angle = 0, XTolerance = 0, YTolerance = 0;
            m_smVisionInfo.g_arrLead3D[0].GetPositionResult_PatternMatch(ref Angle, ref XTolerance, ref YTolerance);

            dgd_PositionResult.Rows.Add();
            dgd_PositionResult.Rows[0].Cells[0].Value = "Angle";
            dgd_PositionResult.Rows[0].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fAngleTolerance.ToString("f4");
            dgd_PositionResult.Rows[0].Cells[2].Value = Angle.ToString("f4");
            if (Math.Abs(Angle) >= m_smVisionInfo.g_arrLead3D[0].ref_fAngleTolerance)
            {
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
            dgd_PositionResult.Rows[1].Cells[0].Value = "X Tol.";
            dgd_PositionResult.Rows[1].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fXTolerance.ToString("f4");
            dgd_PositionResult.Rows[1].Cells[2].Value = XTolerance.ToString("f4");
            if (Math.Abs(XTolerance) >= m_smVisionInfo.g_arrLead3D[0].ref_fXTolerance)
            {
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
            dgd_PositionResult.Rows[2].Cells[0].Value = "Y Tol.";
            dgd_PositionResult.Rows[2].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fYTolerance.ToString("f4");
            dgd_PositionResult.Rows[2].Cells[2].Value = YTolerance.ToString("f4");
            if (Math.Abs(YTolerance) >= m_smVisionInfo.g_arrLead3D[0].ref_fYTolerance)
            {
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

            dgd_PositionResult.Rows.Add();
            dgd_PositionResult.Rows[3].Cells[0].Value = "Base Line Angle(Top)";
            dgd_PositionResult.Rows[3].Cells[1].Value = m_smVisionInfo.g_arrLead3D[1].ref_fBaseLineMaxAngle.ToString("f4");
            dgd_PositionResult.Rows[3].Cells[2].Value = m_smVisionInfo.g_arrLead3D[1].ref_fBaseLineAngle.ToString("f4");
            if (Math.Abs(m_smVisionInfo.g_arrLead3D[1].ref_fBaseLineAngle) >= m_smVisionInfo.g_arrLead3D[1].ref_fBaseLineMaxAngle)
            {
                m_blnFailPosition = true;
                dgd_PositionResult.Rows[3].Cells[1].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[3].Cells[1].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[3].Cells[1].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[3].Cells[1].Style.SelectionForeColor = Color.Yellow;
                dgd_PositionResult.Rows[3].Cells[2].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[3].Cells[2].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[3].Cells[2].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[3].Cells[2].Style.SelectionForeColor = Color.Yellow;
            }
            else
            {
                dgd_PositionResult.Rows[3].Cells[1].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[3].Cells[1].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[3].Cells[1].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[3].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_PositionResult.Rows[3].Cells[2].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[3].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[3].Cells[2].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[3].Cells[2].Style.SelectionForeColor = Color.Black;
            }

            dgd_PositionResult.Rows.Add();
            dgd_PositionResult.Rows[4].Cells[0].Value = "Base Line Angle(Right)";
            dgd_PositionResult.Rows[4].Cells[1].Value = m_smVisionInfo.g_arrLead3D[2].ref_fBaseLineMaxAngle.ToString("f4");
            dgd_PositionResult.Rows[4].Cells[2].Value = m_smVisionInfo.g_arrLead3D[2].ref_fBaseLineAngle.ToString("f4");
            if (Math.Abs(m_smVisionInfo.g_arrLead3D[2].ref_fBaseLineAngle) >= m_smVisionInfo.g_arrLead3D[2].ref_fBaseLineMaxAngle)
            {
                m_blnFailPosition = true;
                dgd_PositionResult.Rows[4].Cells[1].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[4].Cells[1].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[4].Cells[1].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[4].Cells[1].Style.SelectionForeColor = Color.Yellow;
                dgd_PositionResult.Rows[4].Cells[2].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[4].Cells[2].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[4].Cells[2].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[4].Cells[2].Style.SelectionForeColor = Color.Yellow;
            }
            else
            {
                dgd_PositionResult.Rows[4].Cells[1].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[4].Cells[1].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[4].Cells[1].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[4].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_PositionResult.Rows[4].Cells[2].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[4].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[4].Cells[2].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[4].Cells[2].Style.SelectionForeColor = Color.Black;
            }

            dgd_PositionResult.Rows.Add();
            dgd_PositionResult.Rows[5].Cells[0].Value = "Base Line Angle(Bottom)";
            dgd_PositionResult.Rows[5].Cells[1].Value = m_smVisionInfo.g_arrLead3D[3].ref_fBaseLineMaxAngle.ToString("f4");
            dgd_PositionResult.Rows[5].Cells[2].Value = m_smVisionInfo.g_arrLead3D[3].ref_fBaseLineAngle.ToString("f4");
            if (Math.Abs(m_smVisionInfo.g_arrLead3D[3].ref_fBaseLineAngle) >= m_smVisionInfo.g_arrLead3D[3].ref_fBaseLineMaxAngle)
            {
                m_blnFailPosition = true;
                dgd_PositionResult.Rows[5].Cells[1].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[5].Cells[1].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[5].Cells[1].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[5].Cells[1].Style.SelectionForeColor = Color.Yellow;
                dgd_PositionResult.Rows[5].Cells[2].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[5].Cells[2].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[5].Cells[2].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[5].Cells[2].Style.SelectionForeColor = Color.Yellow;
            }
            else
            {
                dgd_PositionResult.Rows[5].Cells[1].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[5].Cells[1].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[5].Cells[1].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[5].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_PositionResult.Rows[5].Cells[2].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[5].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[5].Cells[2].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[5].Cells[2].Style.SelectionForeColor = Color.Black;
            }

            dgd_PositionResult.Rows.Add();
            dgd_PositionResult.Rows[6].Cells[0].Value = "Base Line Angle(Left)";
            dgd_PositionResult.Rows[6].Cells[1].Value = m_smVisionInfo.g_arrLead3D[4].ref_fBaseLineMaxAngle.ToString("f4");
            dgd_PositionResult.Rows[6].Cells[2].Value = m_smVisionInfo.g_arrLead3D[4].ref_fBaseLineAngle.ToString("f4");
            if (Math.Abs(m_smVisionInfo.g_arrLead3D[4].ref_fBaseLineAngle) >= m_smVisionInfo.g_arrLead3D[4].ref_fBaseLineMaxAngle)
            {
                m_blnFailPosition = true;
                dgd_PositionResult.Rows[6].Cells[1].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[6].Cells[1].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[6].Cells[1].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[6].Cells[1].Style.SelectionForeColor = Color.Yellow;
                dgd_PositionResult.Rows[6].Cells[2].Style.BackColor = Color.Red;
                dgd_PositionResult.Rows[6].Cells[2].Style.SelectionBackColor = Color.Red;
                dgd_PositionResult.Rows[6].Cells[2].Style.ForeColor = Color.Yellow;
                dgd_PositionResult.Rows[6].Cells[2].Style.SelectionForeColor = Color.Yellow;
            }
            else
            {
                dgd_PositionResult.Rows[6].Cells[1].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[6].Cells[1].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[6].Cells[1].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[6].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_PositionResult.Rows[6].Cells[2].Style.BackColor = Color.Lime;
                dgd_PositionResult.Rows[6].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_PositionResult.Rows[6].Cells[2].Style.ForeColor = Color.Black;
                dgd_PositionResult.Rows[6].Cells[2].Style.SelectionForeColor = Color.Black;
            }




        }

        private void UpdatePkgResultTable(int intLeadIndex, DataGridView dgd_Package)
        {
            //for (int x = dgd_Package.RowCount - 1; x > 0; x--)
            //{
            //    dgd_Package.Rows.RemoveAt(x);
            //}

          
            // Check Inspect Package Size ON or not
            if ((m_smVisionInfo.g_arrLead3D[0].ref_intFailPkgOptionMask & 0x01) == 0)  // 2019 07 08 - CCENG: not need to use function GetWantInspectPackage() because option pkg size has separated from group of package option already.
            {
                dgd_Package.Rows.Clear();
                return;
            }

            float fWidthMin = m_smVisionInfo.g_arrLead3D[0].GetUnitWidthMin(1);
            float fWidthMax = m_smVisionInfo.g_arrLead3D[0].GetUnitWidthMax(1);
            float fHeightMin = m_smVisionInfo.g_arrLead3D[0].GetUnitHeightMin(1);
            float fHeightMax = m_smVisionInfo.g_arrLead3D[0].GetUnitHeightMax(1);
            //float fThicknessMin = m_smVisionInfo.g_arrLead3D[0].GetUnitThicknessMin(1);
            //float fThicknessMax = m_smVisionInfo.g_arrLead3D[0].GetUnitThicknessMax(1);
            float fWidth = 0;
            if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                fWidth = m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fCenterUnitWidthMM;
            else
                fWidth = (m_smVisionInfo.g_arrLead3D[0].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[0].GetResultDownWidth_RectGauge4L(1)) / 2;

            //// 2019-10-25 ZJYEOH : Add Offset to package width
            //fWidth += m_smVisionInfo.g_arrLead3D[0].ref_fPackageWidthOffsetMM;

            float fHeight = 0;
            
            if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                fHeight = m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fCenterUnitHeightMM;
            else
                fHeight = (m_smVisionInfo.g_arrLead3D[0].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[0].GetResultRightHeight_RectGauge4L(1)) / 2;

            //// 2019-10-25 ZJYEOH : Add Offset to package height
            //fHeight += m_smVisionInfo.g_arrLead3D[0].ref_fPackageHeightOffsetMM;

            //float fThickness = 0;
            //float fTotalThickness = 0;
            //int intCount = 0;
            //for (int j = 1; j < m_smVisionInfo.g_arrLead3D.Length; j++)
            //{
            //    if (j == 1 || j == 3)
            //        fTotalThickness += m_smVisionInfo.g_arrLead3D[j].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[j].GetResultRightHeight_RectGauge4L(1);
            //    else
            //        fTotalThickness += m_smVisionInfo.g_arrLead3D[j].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[j].GetResultDownWidth_RectGauge4L(1);

            //    intCount += 2;
            //}
            //fThickness = fTotalThickness / intCount;

            //// 2019-10-25 ZJYEOH : Add Offset to package thickness
            //fThickness += m_smVisionInfo.g_arrLead3D[1].ref_fPackageThicknessOffsetMM;

            if (dgd_Package.Rows.Count == 0)
                dgd_Package.Rows.Add();
            dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[0].Value = "Result";
            dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Value = fWidth.ToString("f4");
            dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Value = fHeight.ToString("f4");
            //dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Value = fThickness.ToString("f4");

            if (!m_smVisionInfo.g_arrLead3D[0].ref_blnViewPkgSizeDrawing)
            {
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Value = "---";
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Value = "---";
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.BackColor = Color.Lime;
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.SelectionBackColor = Color.Lime;
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.BackColor = Color.Lime;
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.SelectionBackColor = Color.Lime;
                return;
            }


            if ((fWidth < fWidthMin || fWidth > fWidthMax) || (fHeight < fHeightMin || fHeight > fHeightMax))// || (fThickness < fThicknessMin || fThickness > fThicknessMax))
            {
                if (fWidth < fWidthMin || fWidth > fWidthMax)
                {
                    if (dgd_Package.Columns[1].Visible)
                        m_blnFailPackage = true;
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.BackColor = Color.Red;
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.SelectionBackColor = Color.Red;
                }
                else
                {
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.BackColor = Color.Lime;
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.SelectionBackColor = Color.Lime;
                }

                if (fHeight < fHeightMin || fHeight > fHeightMax)
                {
                    if (dgd_Package.Columns[2].Visible)
                        m_blnFailPackage = true;
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.BackColor = Color.Red;
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.SelectionBackColor = Color.Red;
                }
                else
                {
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.BackColor = Color.Lime;
                    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.SelectionBackColor = Color.Lime;
                }

                //if (fThickness < fThicknessMin || fThickness > fThicknessMax)
                //{
                //    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Style.BackColor = Color.Red;
                //    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Style.SelectionBackColor = Color.Red;
                //}
                //else
                //{
                //    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Style.BackColor = Color.Lime;
                //    dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Style.SelectionBackColor = Color.Lime;
                //}

                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[0].Style.BackColor = Color.Red;
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[0].Style.SelectionBackColor = Color.Red;
            }
            else
            {
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[0].Style.BackColor = Color.Lime;
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[0].Style.SelectionBackColor = Color.Lime;

                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.BackColor = Color.Lime;
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[1].Style.SelectionBackColor = Color.Lime;

                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.BackColor = Color.Lime;
                dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[2].Style.SelectionBackColor = Color.Lime;

                //dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Style.BackColor = Color.Lime;
                //dgd_Package.Rows[dgd_Package.RowCount - 1].Cells[3].Style.SelectionBackColor = Color.Lime;
            }
        }
        private void UpdatePkgDefectTable(int intLeadIndex, DataGridView dgd_PkgDefectTable)
        {
            dgd_PkgDefectTable.Rows.Clear();

            List<List<string>> arrDefectList = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetPkgDefectList();
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

                if (m_smVisionInfo.g_intLead3DPkgDefectSelectedNumber == i)
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
                    //    if (intImageNo < 2)
                    //        intImageNo = 0;
                    //    else if (intImageNo < 4)
                    //        intImageNo = 1;
                    //    else
                    //        intImageNo = 2;
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
                        m_blnFailPackage = true;
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
                        m_blnFailPackage = true;
                    dgd_PkgDefectTable.Rows[i].Cells[2].Style.BackColor = Color.Red;
                    dgd_PkgDefectTable.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                    dgd_PkgDefectTable.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                    dgd_PkgDefectTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PkgDefectTable.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                    dgd_PkgDefectTable.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
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
                        m_blnFailPackage = true;
                    dgd_PkgDefectTable.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_PkgDefectTable.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_PkgDefectTable.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_PkgDefectTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_PkgDefectTable.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_PkgDefectTable.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_PkgDefectTable.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_PkgDefectTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }

                dgd_PkgDefectTable.Rows[i].Cells[4].Value = arrDefectList[i][5];
                if ((intFailMask & 0x0C) > 0)
                {
                    if (dgd_PkgDefectTable.Columns[4].Visible)
                        m_blnFailPackage = true;
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
            m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_blnViewPkgResultDrawing = true;
        }

        private void dgd_PkgDefect_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int r = e.RowIndex;

            m_smVisionInfo.g_intLead3DPkgDefectSelectedNumber = r;
            m_smVisionInfo.g_blnLead3DSelecting = true;
            m_smVisionInfo.g_arrLead3D[0].ref_blnPkgDefectSelected = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void UpdateTabPageHeaderImage()
        {
            if (m_blnFailLead3D)
                tp_Lead3D.ImageIndex = 1;
            else
                tp_Lead3D.ImageIndex = 0;
            
            if (m_blnFailPackage)
                tp_Package.ImageIndex = 1;
            else
                tp_Package.ImageIndex = 0;

            if (m_blnFailPosition)
                tp_Position.ImageIndex = 1;
            else
                tp_Position.ImageIndex = 0;
            
            if (m_blnFailPH)
                tp_PH.ImageIndex = 1;
            else
                tp_PH.ImageIndex = 0;

            if (m_blnFailPin1)
                tp_Pin1.ImageIndex = 1;
            else
                tp_Pin1.ImageIndex = 0;
        }
    }
}