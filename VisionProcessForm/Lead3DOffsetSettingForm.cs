using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using Common;
using ImageAcquisition;
using VisionProcessing;
using SharedMemory;

namespace VisionProcessForm
{
    public partial class Lead3DOffsetSettingForm : Form
    {
        #region Member Variables
        private bool m_blnShow = false;
        private bool m_blnUpdateSelectedROISetting = false;
        private bool m_blnInitDone = false;
        private bool m_blnUpdateInfo = false;
        private int m_intUserGroup = 5;
        private int m_intLeadIndex = 0;
        private int m_intDecimal = 3;
        private string m_strSelectedRecipe;
        private string m_strUnitLabel = "";
        private bool m_blnChangeScoreSetting = true;
        private List<int> m_arrLeadRowIndex = new List<int>();
        private List<int> m_arrGroupRowIndex = new List<int>();

        private DataGridView[] m_dgdView = new DataGridView[5];
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        #endregion
        
        public Lead3DOffsetSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            InitializeComponent();


            m_smProductionInfo = smProductionInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_intUserGroup = intUserGroup;

            m_dgdView[0] = dgd_MiddleLead;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].BackupPreviousTolerance();
            }

            //DisableField();
            UpdateGUI();

            m_blnInitDone = true;
            
        }

        private void UpdateGUI()
        {
            ReadLeadTemplateDataToGrid(m_intLeadIndex, m_dgdView[0]);

            UpdateUnitDisplay();
        }
        private void UpdateUnitDisplay()
        {
            switch (m_smCustomizeInfo.g_intUnitDisplay)
            {
                case 1:
                default:
                    m_strUnitLabel = "mm";
                    m_intDecimal = 4;
                    break;
                case 2:
                    m_strUnitLabel = "mil";
                    m_intDecimal = 3;
                    break;
                case 3:
                    m_strUnitLabel = "um";
                    m_intDecimal = 1;
                    break;
            }
        }
        private void ReadLeadTemplateDataToGrid(int intLeadIndex, DataGridView dgd_LeadSetting)
        {
            dgd_LeadSetting.Rows.Clear();

            List<List<string>> arrBlobsFeaturesData;

            arrBlobsFeaturesData = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetLeadOffsetSettingValue();

            for (int i = 0; i < arrBlobsFeaturesData.Count; i++)
            {
                dgd_LeadSetting.Rows.Add();
                dgd_LeadSetting.Rows[i].HeaderCell.Value = arrBlobsFeaturesData[i][0] + " " + (Convert.ToInt32(arrBlobsFeaturesData[i][1]) + 1).ToString();

                dgd_LeadSetting.Rows[i].Cells[0].Value = arrBlobsFeaturesData[i][2];
                dgd_LeadSetting.Rows[i].Cells[1].Value = arrBlobsFeaturesData[i][3];
                dgd_LeadSetting.Rows[i].Cells[2].Value = arrBlobsFeaturesData[i][4];
                dgd_LeadSetting.Rows[i].Cells[3].Value = arrBlobsFeaturesData[i][5];
                dgd_LeadSetting.Rows[i].Cells[4].Value = arrBlobsFeaturesData[i][6];
                dgd_LeadSetting.Rows[i].Cells[5].Value = arrBlobsFeaturesData[i][7];

                m_arrLeadRowIndex.Add(i);
            }
        }
        
        private void btn_Save_Click(object sender, EventArgs e)
        {
          
            this.Close();
            this.Dispose();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].SetPreviousToleranceToTemplate();
            }

            this.Close();
            this.Dispose();
        }
    
        private void dgd_MiddleLead_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnChangeScoreSetting)
                return;

            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex < 0)
                return;

            string strCurrentSetValue = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            string strDisplayMessage = "Set value to " + ((DataGridView)sender).Rows[e.RowIndex].HeaderCell.Value.ToString();

            int intSetAllROI;
            if (m_intLeadIndex == 0)
                intSetAllROI = 0;
            else
                intSetAllROI = 1;

            SetValueForm objSetValueForm = new SetValueForm(strDisplayMessage, m_strUnitLabel, e.RowIndex, m_intDecimal, strCurrentSetValue, intSetAllROI, false, false);

            objSetValueForm.TopMost = true;
            if (objSetValueForm.ShowDialog() == DialogResult.OK)
            {
                int intStartRowNumber;
                if (objSetValueForm.ref_blnSetAllRows)
                {
                    intStartRowNumber = 0;
                }
                else
                {
                    intStartRowNumber = e.RowIndex;
                }

                int intStartIndex;
                int intEndIndex;
                if (objSetValueForm.ref_blnSetAllROI)
                {
                    intStartIndex = 1;
                    intEndIndex = m_smVisionInfo.g_arrLead3D.Length;
                }
                else
                {
                    intStartIndex = m_intLeadIndex;
                    intEndIndex = m_intLeadIndex + 1;
                }

                int intTotalRows;
                if (objSetValueForm.ref_blnSetAllRows)
                {
                    intTotalRows = m_smVisionInfo.g_arrLead3D[m_intLeadIndex].GetBlobsFeaturesNumber();

                    //if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantUseGroupToleranceSetting)
                    //{
                    //    intTotalRows += m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intPadGroupCount;  // 2019 12 23 - CCENG: total pad rows + additional group rows.
                    //}
                }
                else
                {
                    intTotalRows = intStartRowNumber + 1;
                }

                // Loop row index
                for (int i = intStartRowNumber; i < intTotalRows; i++)
                {

                    if (((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value.ToString() != "---")
                    {
                        // Set new insert value into table
                        //if (objSetValueForm.ref_blnSetAllEdges)
                        //{
                        //    for (int j = 0; j < ((DataGridView)sender).Columns.Count; j++)
                        //    {
                        //        ((DataGridView)sender).Rows[i].Cells[j].Value = objSetValueForm.ref_fSetValue.ToString("F4");
                        //    }
                        //}
                        //else
                            ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F4");
                    }
                }

                // --------------------------------------------------------------------------------------

                // Loop Pad index
                for (int j = intStartIndex; j < intEndIndex; j++)
                {
                    intTotalRows = m_smVisionInfo.g_arrLead3D[j].GetBlobsFeaturesNumber();
                    //if (m_smVisionInfo.g_arrPad[j].ref_blnWantUseGroupToleranceSetting)
                    //{
                    //    intTotalRows = e.RowIndex + m_smVisionInfo.g_arrPad[j].ref_intPadGroupCount;
                    //}
                    //else 
                    if (!objSetValueForm.ref_blnSetAllRows)
                    {
                        intTotalRows = intStartRowNumber + 1;
                    }
                    // Loop row index
                    for (int i = intStartRowNumber; i < intTotalRows; i++)
                    {
                        //if (objSetValueForm.ref_blnSetAllEdges)
                        //{
                        //    for (int k = 0; k < ((DataGridView)sender).Columns.Count; k++)
                        //    {
                        //        m_smVisionInfo.g_arrPad[j].UpdatePadOffsetSettingValueToPixel(i, k,
                        //        objSetValueForm.ref_fSetValue);
                        //    }
                        //}
                        //else
                            m_smVisionInfo.g_arrLead3D[j].UpdateLeadOffsetSettingValueToPixel(i, e.ColumnIndex,
                                    objSetValueForm.ref_fSetValue);
                    }
                }

            }
            
            this.TopMost = true;
        }

        private void dgd_MiddleLead_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!m_blnChangeScoreSetting)
                return;

            if (e.ColumnIndex < 0)
                return;

            string strCurrentSetValue = "";
            for (int i = 0; i < ((DataGridView)sender).Rows.Count; i++)
            {
                float fSetValue = 0;
                if (float.TryParse(((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value.ToString(), out fSetValue))
                {
                    strCurrentSetValue = fSetValue.ToString();
                    break;
                }
            }
            if (strCurrentSetValue == "")
                return;

            //SetValueForm objSetValueForm = new SetValueForm(e.RowIndex, intDecimalPlaces, strCurrentSetValue);
            SetValueForm objSetValueForm = new SetValueForm("Set value to all leads", m_strUnitLabel, e.RowIndex, m_intDecimal, strCurrentSetValue, true, false, false);
            objSetValueForm.TopMost = true;
            if (objSetValueForm.ShowDialog() == DialogResult.OK)
            {
                int intStartRowNumber;
                if (objSetValueForm.ref_blnSetAllRows)
                {
                    intStartRowNumber = 0;
                }
                else
                {
                    intStartRowNumber = e.RowIndex;
                }

                //Validate min, max value
                for (int i = intStartRowNumber; i < ((DataGridView)sender).Rows.Count; i++)
                {
                    if (((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value.ToString() != "---")
                    {

                        // Set new insert value into table
                        //if (objSetValueForm.ref_blnSetAllEdges)
                        //{
                        //    for (int j = 0; j < ((DataGridView)sender).Columns.Count; j++)
                        //    {
                        //        ((DataGridView)sender).Rows[i].Cells[j].Value = objSetValueForm.ref_fSetValue.ToString("F4");
                        //    }
                        //}
                        //else
                            ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F4");


                    }

                    //Update template setting
                    //if (intLengthMode == 1)
                    {
                        m_smVisionInfo.g_arrLead3D[m_intLeadIndex].UpdateLeadOffsetSettingValueToPixel(i,
                             float.Parse(((DataGridView)sender).Rows[i].Cells[0].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[1].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[2].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[4].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[5].Value.ToString()));


                    }

                    if (!objSetValueForm.ref_blnSetAllRows)
                        break;
                }
            }
            
            this.TopMost = true;
        }
    
    }
}
