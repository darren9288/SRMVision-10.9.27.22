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
    public partial class LeadOffsetSettingForm : Form
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
        
        public LeadOffsetSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            InitializeComponent();


            m_smProductionInfo = smProductionInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_intUserGroup = intUserGroup;

            m_dgdView[0] = dgd_MiddleLead;

            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;

                m_smVisionInfo.g_arrLead[i].BackupPreviousTolerance();
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
            for (int x = 0; x < m_smVisionInfo.g_arrLead.Length; x++)
            {
                if (!m_smVisionInfo.g_arrLead[x].ref_blnSelected)
                    continue;
                
                string strBlobsFeatures = m_smVisionInfo.g_arrLead[x].GetLeadOffsetSettingValue();
                string[] strFeature = strBlobsFeatures.Split('#');
                string[] strFeature_BaseLead = { };
                
                int intBlobsCount = m_smVisionInfo.g_arrLead[x].GetBlobsFeaturesNumber();
                int intBlobID = m_smVisionInfo.g_arrLead[x].GetBlobsNoID();
                int intFeatureIndex = 0;
                for (int k = 0; k < intBlobsCount + (intBlobID - 1); k++)
                {
                    if (dgd_LeadSetting.Rows.Count < intBlobsCount + (intBlobID - 1))
                        dgd_LeadSetting.Rows.Add();
                }
              
                for (int i = intBlobID - 1; i < intBlobsCount + (intBlobID - 1); i++)
                {
                    if (i < 0)
                        continue;

                    dgd_LeadSetting.Rows[i].HeaderCell.Value = "Lead " + (i + 1);
                    
                    // Offset Setting
                    dgd_LeadSetting.Rows[i].Cells[0].Value = strFeature[intFeatureIndex++];
                    dgd_LeadSetting.Rows[i].Cells[1].Value = strFeature[intFeatureIndex++];
                    dgd_LeadSetting.Rows[i].Cells[2].Value = strFeature[intFeatureIndex++];
                    dgd_LeadSetting.Rows[i].Cells[3].Value = strFeature[intFeatureIndex++];
                   
                }
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
          
            this.Close();
            this.Dispose();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;

                m_smVisionInfo.g_arrLead[i].SetPreviousToleranceToTemplate();
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
            
            SetValueForm objSetValueForm = new SetValueForm(strDisplayMessage, m_strUnitLabel, e.RowIndex, m_intDecimal, strCurrentSetValue, 0, false, false);

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
                
                // Loop row index
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

                    for (int x = 0; x < m_smVisionInfo.g_arrLead.Length; x++)
                    {
                        if (!m_smVisionInfo.g_arrLead[x].ref_blnSelected)
                            continue;

                        int intBlobsCount = m_smVisionInfo.g_arrLead[x].GetBlobsFeaturesNumber();
                        int intBlobIndex = 0;
                        //int intStartIndex = m_smVisionInfo.g_arrLead[x].GetBlobsNoID();
                        bool blnFound = false;
                        for (int k = 0; k < intBlobsCount; k++) // for (int k = intStartIndex - 1; k < intBlobsCount + (intStartIndex - 1); k++)
                        {
                            int intBlobID = m_smVisionInfo.g_arrLead[x].GetBlobsNoID(k);
                            if (intBlobID == (i + 1))
                            {
                                intBlobIndex = k;
                                blnFound = true;
                                break;
                            }
                        }
                        //Update template setting
                        //if (intLengthMode == 1)
                        //{
                        if (blnFound)
                        {
                            m_smVisionInfo.g_arrLead[x].UpdateLeadOffsetSettingValueToPixel(intBlobIndex, e.ColumnIndex,
                                    objSetValueForm.ref_fSetValue);

                        }
                    }

                    if (!objSetValueForm.ref_blnSetAllRows)
                        break;
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
                    for (int x = 0; x < m_smVisionInfo.g_arrLead.Length; x++)
                    {
                        if (!m_smVisionInfo.g_arrLead[x].ref_blnSelected)
                            continue;

                        int intBlobsCount = m_smVisionInfo.g_arrLead[x].GetBlobsFeaturesNumber();
                        int intBlobIndex = 0;
                        //int intStartIndex = m_smVisionInfo.g_arrLead[x].GetBlobsNoID();
                        bool blnFound = false;
                        for (int k = 0; k < intBlobsCount; k++) // for (int k = intStartIndex - 1; k < intBlobsCount + (intStartIndex - 1); k++)
                        {
                            int intBlobID = m_smVisionInfo.g_arrLead[x].GetBlobsNoID(k);
                            if (intBlobID == (i + 1))
                            {
                                intBlobIndex = k;
                                blnFound = true;
                                break;
                            }
                        }
                        //Update template setting
                        //if (intLengthMode == 1)
                        //{
                        if (blnFound)
                        {
                            m_smVisionInfo.g_arrLead[x].UpdateLeadOffsetSettingValueToPixel(intBlobIndex,
                             float.Parse(((DataGridView)sender).Rows[i].Cells[0].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[1].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[2].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString()));
                        }
                    }

                    if (!objSetValueForm.ref_blnSetAllRows)
                        break;
                }
            }
            
            this.TopMost = true;
        }
    
    }
}
