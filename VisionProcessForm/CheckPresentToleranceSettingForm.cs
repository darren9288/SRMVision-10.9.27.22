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
using SharedMemory;
using VisionProcessing;

namespace VisionProcessForm
{
    public partial class CheckPresentToleranceSettingForm : Form
    {
        #region Member Variables
        private bool m_blnFormOpen = false;
        private bool m_blnInitDone = false;
        private int m_intUserGroup;
        private string m_strFolderPath;
        private VisionInfo m_smVisionInfo;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();

        #endregion
        public bool ref_blnFormOpen { get { return m_blnFormOpen; } set { m_blnFormOpen = value; } }
        public CheckPresentToleranceSettingForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup)
        {
            InitializeComponent();

            m_intUserGroup = intUserGroup;
            m_smVisionInfo = smVisionInfo;
            m_smProductionInfo = smProductionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_strFolderPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\CheckPresent\\";
            UpdateGUI();
            m_blnInitDone = true;
        }
        private void UpdateGUI()
        {
            for (int i = 0; i < m_smVisionInfo.g_objUnitPresent.ref_intTemplateObjectsCount; i++)
            {
                dgd_UnitPresent.Rows.Add();
                dgd_UnitPresent.Rows[i].Cells[0].Value = (i + 1).ToString();
                dgd_UnitPresent.Rows[i].Cells[1].Value = m_smVisionInfo.g_objUnitPresent.GetTemplateObjectMinArea(i);
                dgd_UnitPresent.Rows[i].Cells[2].Value = m_smVisionInfo.g_objUnitPresent.GetSampleObjectArea(i);
                if (Convert.ToInt32(dgd_UnitPresent.Rows[i].Cells[1].Value) < Convert.ToInt32(dgd_UnitPresent.Rows[i].Cells[2].Value))
                {
                    dgd_UnitPresent.Rows[i].Cells[2].Style.BackColor = Color.Red;
                    dgd_UnitPresent.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                }
                else
                {
                    dgd_UnitPresent.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                    dgd_UnitPresent.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                }
            }
            
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objUnitPresent.SaveUnitPresent(m_strFolderPath + "Setting.xml", false, "UnitPresentSetting", true);

            Close();
            Dispose();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objUnitPresent.LoadUnitPresent(m_strFolderPath + "Setting.xml", "UnitPresentSetting");

            Close();
            Dispose();
        }

        private void CheckPresentToleranceSettingForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewUnitPresentObjectsBuilded = false;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_blnFormOpen = true;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void CheckPresentToleranceSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.VM_AT_SettingInDialog = false;
            m_blnFormOpen = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_UnitPresent_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || !m_blnInitDone || e.ColumnIndex != 1)
                return;

            string strCurrentSetValue = dgd_UnitPresent.Rows[e.RowIndex].Cells[1].Value.ToString();
            SetValueForm objSetValueForm = new SetValueForm("Set value to unit " + (e.RowIndex + 1).ToString(), "pixels", e.RowIndex, 0, strCurrentSetValue);
            objSetValueForm.Location = new Point(769, 310);
            if (objSetValueForm.ShowDialog() == DialogResult.OK)
            {
                if (objSetValueForm.ref_blnSetAllRows)
                {
                    for (int i = 0; i < dgd_UnitPresent.Rows.Count; i++)
                    {
                        dgd_UnitPresent.Rows[i].Cells[1].Value = objSetValueForm.ref_fSetValue;
                        m_smVisionInfo.g_objUnitPresent.SetTemplateObjectMinArea(i, objSetValueForm.ref_fSetValue);
                    }
                }
                else
                {
                    dgd_UnitPresent.Rows[e.RowIndex].Cells[1].Value = objSetValueForm.ref_fSetValue;
                    m_smVisionInfo.g_objUnitPresent.SetTemplateObjectMinArea(e.RowIndex, objSetValueForm.ref_fSetValue);
                }
            }

            if (Convert.ToInt32(dgd_UnitPresent.Rows[e.RowIndex].Cells[1].Value) < Convert.ToInt32(dgd_UnitPresent.Rows[e.RowIndex].Cells[2].Value))
            {
                dgd_UnitPresent.Rows[e.RowIndex].Cells[2].Style.BackColor = Color.Red;
                dgd_UnitPresent.Rows[e.RowIndex].Cells[2].Style.SelectionBackColor = Color.Red;
            }
            else
            {
                dgd_UnitPresent.Rows[e.RowIndex].Cells[2].Style.BackColor = Color.Lime;
                dgd_UnitPresent.Rows[e.RowIndex].Cells[2].Style.SelectionBackColor = Color.Lime;
            }
        }
    }
}
