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
using VisionProcessing;
using SharedMemory;
using Microsoft.Win32;
using System.IO;

namespace VisionProcessForm
{
    public partial class Lead3DSkewInspectOptionForm : Form
    {
        #region Member Variables
        private List<bool> m_arrCheckSkewPrev = new List<bool>();

        private bool m_blnInitDone = false;
        private int m_intUserGroup = 5;
        private bool m_blnUpdateInfo = false;
        private string m_strSelectedRecipe;

        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();

        #endregion
        public Lead3DSkewInspectOptionForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup)
        {
            InitializeComponent();
            
            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;

            UpdateGUI();

            m_blnInitDone = true;
            m_blnUpdateInfo = true;
        }
        private void UpdateGUI()
        {
            dgd_MiddleLead.Rows.Clear();
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
            {
                List<List<string>> arrBlobsFeaturesData;
                arrBlobsFeaturesData = m_smVisionInfo.g_arrLead3D[0].GetBlobsFeaturesSkewInspectOption();
                for (int i = 0; i < arrBlobsFeaturesData.Count; i++)
                {
                    dgd_MiddleLead.Rows.Add();
                    dgd_MiddleLead.Rows[i].HeaderCell.Value = arrBlobsFeaturesData[i][0] + arrBlobsFeaturesData[i][1];
                    dgd_MiddleLead.Rows[i].Cells[0].Value = Convert.ToBoolean(arrBlobsFeaturesData[i][2]);
                    m_arrCheckSkewPrev.Add(Convert.ToBoolean(arrBlobsFeaturesData[i][2]));
                }
            }
        }
        private void btn_Save_Click(object sender, EventArgs e)
        {

        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D[0].GetBlobsFeaturesNumber(); i++)
            {
                for (int h = 0; h < m_smVisionInfo.g_arrLead3D.Length; h++)
                {
                    m_smVisionInfo.g_arrLead3D[h].UpdateBlobFeatureCheckSkew(i, m_arrCheckSkewPrev[i]);
                }
            }
        }

        private void dgd_MiddleLead_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (!m_blnInitDone)
            //    return;
            
            //if (e.RowIndex < 0)
            //    return;

            //if (e.ColumnIndex < 0)
            //    return;

            //if (!m_blnUpdateInfo)
            //{
            //    return;
            //}

            //m_blnUpdateInfo = false;

            //int intStartRowNumber;
            //if (chk_SetToAllRows.Checked)
            //{
            //    intStartRowNumber = 0;
            //}
            //else
            //{
            //    intStartRowNumber = e.RowIndex;
            //}
            
            //int intTotalRows;
            //if (chk_SetToAllRows.Checked)
            //{
            //    intTotalRows = m_smVisionInfo.g_arrLead3D[0].GetBlobsFeaturesNumber();
            //}
            //else
            //{
            //    intTotalRows = intStartRowNumber + 1;
            //}
            
            //if (!chk_SetToAllRows.Checked)
            //{
            //    intTotalRows = intStartRowNumber + 1;
            //}
            //// Loop row index
            //for (int i = intStartRowNumber; i < intTotalRows; i++)
            //{
            //    if (e.ColumnIndex == 0)
            //    {
            //        bool blnValue = Convert.ToBoolean(dgd_MiddleLead.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);

            //        for (int h = 0; h < m_smVisionInfo.g_arrLead3D.Length; h++)
            //        {
            //            m_smVisionInfo.g_arrLead3D[h].UpdateBlobFeatureCheckSkew(i, blnValue);
            //        }
            //    }
            //}
            
            //UpdateGUI();
            ////dgd_MiddleLead.Refresh();
            //m_blnUpdateInfo = true;
        }

        private void dgd_MiddleLead_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgd_MiddleLead.IsCurrentCellDirty)
            {
                // This fires the cell value changed handler below
                dgd_MiddleLead.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgd_MiddleLead_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex < 0)
                return;

            if (!m_blnUpdateInfo)
            {
                return;
            }

            m_blnUpdateInfo = false;

            int intStartRowNumber;
            if (chk_SetToAllRows.Checked)
            {
                intStartRowNumber = 0;
            }
            else
            {
                intStartRowNumber = e.RowIndex;
            }

            int intTotalRows;
            if (chk_SetToAllRows.Checked)
            {
                intTotalRows = m_smVisionInfo.g_arrLead3D[0].GetBlobsFeaturesNumber();
            }
            else
            {
                intTotalRows = intStartRowNumber + 1;
            }

            //if (!chk_SetToAllRows.Checked)
            //{
            //    intTotalRows = intStartRowNumber + 1;
            //}
            // Loop row index
            for (int i = intStartRowNumber; i < intTotalRows; i++)
            {
                if (e.ColumnIndex == 0)
                {
                    bool blnValue = Convert.ToBoolean(dgd_MiddleLead.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue);

                    for (int h = 0; h < m_smVisionInfo.g_arrLead3D.Length; h++)
                    {
                        m_smVisionInfo.g_arrLead3D[h].UpdateBlobFeatureCheckSkew(i, blnValue);
                    }

                    if (i != e.RowIndex)
                        dgd_MiddleLead.Rows[i].Cells[e.ColumnIndex].Value = blnValue;
                }
            }

            //UpdateGUI();
            //dgd_MiddleLead.Refresh();
            m_blnUpdateInfo = true;
        }

        private void dgd_MiddleLead_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex < 0)
                return;

            if (!m_blnUpdateInfo)
            {
                return;
            }

            m_blnUpdateInfo = false;

            int intStartRowNumber;
            if (chk_SetToAllRows.Checked)
            {
                intStartRowNumber = 0;
            }
            else
            {
                intStartRowNumber = e.RowIndex;
            }

            int intTotalRows;
            if (chk_SetToAllRows.Checked)
            {
                intTotalRows = m_smVisionInfo.g_arrLead3D[0].GetBlobsFeaturesNumber();
            }
            else
            {
                intTotalRows = intStartRowNumber + 1;
            }

            //if (!chk_SetToAllRows.Checked)
            //{
            //    intTotalRows = intStartRowNumber + 1;
            //}
            // Loop row index
            for (int i = intStartRowNumber; i < intTotalRows; i++)
            {
                if (e.ColumnIndex == 0)
                {
                    bool blnValue = Convert.ToBoolean(dgd_MiddleLead.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue);

                    for (int h = 0; h < m_smVisionInfo.g_arrLead3D.Length; h++)
                    {
                        m_smVisionInfo.g_arrLead3D[h].UpdateBlobFeatureCheckSkew(i, blnValue);
                    }

                    if (i != e.RowIndex)
                        dgd_MiddleLead.Rows[i].Cells[e.ColumnIndex].Value = blnValue;
                }
            }

            //UpdateGUI();
            //dgd_MiddleLead.Refresh();
            m_blnUpdateInfo = true;
        }
    }
}
