using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharedMemory;
using VisionProcessing;
using Common;

namespace VisionProcessForm
{
    public partial class PadUpdateToleranceUpperLowerLimitForm : Form
    {
        #region Member Variables

        private bool m_blnInitDone = false;
        private int m_intUserGroup = 5;
        private string m_strSelectedRecipe;

        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        public float ref_fLowerLimit_Area { get { return Convert.ToSingle(dgd_Setting.Rows[0].Cells[0].Value.ToString()); } }
        public float ref_fLowerLimit_WidthLength { get { return Convert.ToSingle(dgd_Setting.Rows[0].Cells[1].Value.ToString()); } }
        public float ref_fLowerLimit_PitchGap { get { return Convert.ToSingle(dgd_Setting.Rows[0].Cells[2].Value.ToString()); } }
        public float ref_fUpperLimit_Area { get { return Convert.ToSingle(dgd_Setting.Rows[1].Cells[0].Value.ToString()); } }
        public float ref_fUpperLimit_WidthLength { get { return Convert.ToSingle(dgd_Setting.Rows[1].Cells[1].Value.ToString()); } }
        public float ref_fUpperLimit_PitchGap { get { return Convert.ToSingle(dgd_Setting.Rows[1].Cells[2].Value.ToString()); } }

        public bool ref_blnSetToCenterROI { get { return chk_SetToCenterROI.Checked; } }
        public bool ref_blnSetToSideROI { get { return chk_SetToSideROI.Checked; } }
        #endregion

        public PadUpdateToleranceUpperLowerLimitForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionINfo)
        {
            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = smVisionInfo;
            m_intUserGroup = intUserGroup;
            m_smProductionInfo = smProductionINfo;
            InitializeComponent();

            UpdateGUI();
            m_blnInitDone = true;
        }

             
        
        private void UpdateGUI()
        {
            dgd_Setting.Rows.Add();
            dgd_Setting.Rows[0].HeaderCell.Value = "Lower Limit (%) < 100";
            dgd_Setting.Rows.Add();
            dgd_Setting.Rows[1].HeaderCell.Value = "Upper Limit (%) > 100";

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
             m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Settings.xml";

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("UpperLowerLimit");
            dgd_Setting.Rows[0].Cells[0].Value = objFileHandle.GetValueAsFloat("LowerLimit_Area", 80);
            dgd_Setting.Rows[0].Cells[1].Value = objFileHandle.GetValueAsFloat("LowerLimit_WidthLength", 80);
            dgd_Setting.Rows[0].Cells[2].Value = objFileHandle.GetValueAsFloat("LowerLimit_PitchGap", 80);
            dgd_Setting.Rows[1].Cells[0].Value = objFileHandle.GetValueAsFloat("UpperLimit_Area", 120);
            dgd_Setting.Rows[1].Cells[1].Value = objFileHandle.GetValueAsFloat("UpperLimit_WidthLength", 120);
            dgd_Setting.Rows[1].Cells[2].Value = objFileHandle.GetValueAsFloat("UpperLimit_PitchGap", 120);

            if ((m_smCustomizeInfo.g_intWantPad5S & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_blnCheck4Sides)
            {
                chk_SetToCenterROI.Checked = objFileHandle.GetValueAsBoolean("SetToCenterROI", true);
                chk_SetToSideROI.Checked = objFileHandle.GetValueAsBoolean("SetToSideROI", true);
            }
            else
            {
                chk_SetToCenterROI.Checked = true;
                chk_SetToSideROI.Checked = false;
            }

            if ((m_smCustomizeInfo.g_intWantPad5S & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_blnCheck4Sides)
            {
                chk_SetToCenterROI.Visible = true;
                chk_SetToSideROI.Visible = true;
            }
            else
            {
                chk_SetToCenterROI.Visible = false;
                chk_SetToSideROI.Visible = false;
            }
        }

        private bool IsSettingCorrect()
        {
            float fValue = 0;

            bool blnResult = true;
            if (float.TryParse(dgd_Setting.Rows[0].Cells[0].Value.ToString(), out fValue))
            {
                if (fValue >= 100)
                {
                    blnResult = false;
                    dgd_Setting.Rows[0].Cells[0].Style.ForeColor = Color.Red;
                    dgd_Setting.Rows[0].Cells[0].Style.SelectionForeColor = Color.Red;
                }
                else
                {
                    dgd_Setting.Rows[0].Cells[0].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[0].Cells[0].Style.SelectionForeColor = Color.Black;
                }
            }
            else
            {
                blnResult = false;
                dgd_Setting.Rows[0].Cells[0].Style.ForeColor = Color.Red;
                dgd_Setting.Rows[0].Cells[0].Style.SelectionForeColor = Color.Red;
            }

            if (float.TryParse(dgd_Setting.Rows[0].Cells[1].Value.ToString(), out fValue))
            {
                if (fValue >= 100)
                {
                    blnResult = false;
                    dgd_Setting.Rows[0].Cells[1].Style.ForeColor = Color.Red;
                    dgd_Setting.Rows[0].Cells[1].Style.SelectionForeColor = Color.Red;
                }
                else
                {
                    dgd_Setting.Rows[0].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[0].Cells[1].Style.SelectionForeColor = Color.Black;
                }
            }
            else
            {
                blnResult = false;
                dgd_Setting.Rows[0].Cells[1].Style.ForeColor = Color.Red;
                dgd_Setting.Rows[0].Cells[1].Style.SelectionForeColor = Color.Red;
            }

            if (float.TryParse(dgd_Setting.Rows[0].Cells[2].Value.ToString(), out fValue))
            {
                if (fValue >= 100)
                {
                    blnResult = false;
                    dgd_Setting.Rows[0].Cells[2].Style.ForeColor = Color.Red;
                    dgd_Setting.Rows[0].Cells[2].Style.SelectionForeColor = Color.Red;
                }
                else
                {
                    dgd_Setting.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
                }
            }
            else
            {
                blnResult = false;
                dgd_Setting.Rows[0].Cells[2].Style.ForeColor = Color.Red;
                dgd_Setting.Rows[0].Cells[2].Style.SelectionForeColor = Color.Red;
            }

            if (float.TryParse(dgd_Setting.Rows[1].Cells[0].Value.ToString(), out fValue))
            {
                if (fValue <= 100)
                {
                    blnResult = false;
                    dgd_Setting.Rows[1].Cells[0].Style.ForeColor = Color.Red;
                    dgd_Setting.Rows[1].Cells[0].Style.SelectionForeColor = Color.Red;
                }
                else
                {
                    dgd_Setting.Rows[1].Cells[0].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[1].Cells[0].Style.SelectionForeColor = Color.Black;
                }
            }
            else
            {
                blnResult = false;
                dgd_Setting.Rows[1].Cells[0].Style.ForeColor = Color.Red;
                dgd_Setting.Rows[1].Cells[0].Style.SelectionForeColor = Color.Red;
            }

            if (float.TryParse(dgd_Setting.Rows[1].Cells[1].Value.ToString(), out fValue))
            {
                if (fValue <= 100)
                {
                    blnResult = false;
                    dgd_Setting.Rows[1].Cells[1].Style.ForeColor = Color.Red;
                    dgd_Setting.Rows[1].Cells[1].Style.SelectionForeColor = Color.Red;
                }
                else
                {
                    dgd_Setting.Rows[1].Cells[1].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[1].Cells[1].Style.SelectionForeColor = Color.Black;
                }
            }
            else
            {
                blnResult = false;
                dgd_Setting.Rows[1].Cells[1].Style.ForeColor = Color.Red;
                dgd_Setting.Rows[1].Cells[1].Style.SelectionForeColor = Color.Red;
            }

            if (float.TryParse(dgd_Setting.Rows[1].Cells[2].Value.ToString(), out fValue))
            {
                if (fValue <= 100)
                {
                    blnResult = false;
                    dgd_Setting.Rows[1].Cells[2].Style.ForeColor = Color.Red;
                    dgd_Setting.Rows[1].Cells[2].Style.SelectionForeColor = Color.Red;
                }
                else
                {
                    dgd_Setting.Rows[1].Cells[2].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[1].Cells[2].Style.SelectionForeColor = Color.Black;
                }
            }
            else
            {
                blnResult = false;
                dgd_Setting.Rows[1].Cells[2].Style.ForeColor = Color.Red;
                dgd_Setting.Rows[1].Cells[2].Style.SelectionForeColor = Color.Red;
            }

            return blnResult;

        }

        private void PadUpdateToleranceUpperLowerLimitForm_Load(object sender, EventArgs e)
        {

        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btn_SaveAndUpdateToTable_Click(object sender, EventArgs e)
        {
            if (!IsSettingCorrect())
            {
                SRMMessageBox.Show("Wrong setting! Please make sure values are in digit, below 100 for Lower Limit, and above 100 for upper limit.");
                return;
            }

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                   m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Settings.xml";

            
            STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("UpperLowerLimit");
            objFileHandle.WriteElement1Value("LowerLimit_Area", dgd_Setting.Rows[0].Cells[0].Value.ToString());
            objFileHandle.WriteElement1Value("LowerLimit_WidthLength", dgd_Setting.Rows[0].Cells[1].Value.ToString());
            objFileHandle.WriteElement1Value("LowerLimit_PitchGap", dgd_Setting.Rows[0].Cells[2].Value.ToString());
            objFileHandle.WriteElement1Value("UpperLimit_Area", dgd_Setting.Rows[1].Cells[0].Value.ToString());
            objFileHandle.WriteElement1Value("UpperLimit_WidthLength", dgd_Setting.Rows[1].Cells[1].Value.ToString());
            objFileHandle.WriteElement1Value("UpperLimit_PitchGap", dgd_Setting.Rows[1].Cells[2].Value.ToString());
            objFileHandle.WriteElement1Value("SetToCenterROI", chk_SetToCenterROI.Checked);
            objFileHandle.WriteElement1Value("SetToSideROI", chk_SetToSideROI.Checked);
            objFileHandle.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Pad Settings", m_smProductionInfo.g_strLotID);
            
            this.DialogResult = DialogResult.Yes; // Return yes to update tolerance to table.

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            this.Close();
        }

        private void btn_SaveAndClose_Click(object sender, EventArgs e)
        {
            if (!IsSettingCorrect())
            {
                SRMMessageBox.Show("Wrong setting! Please make sure values are in digit, below 100 for Lower Limit, and above 100 for upper limit.");
                return;
            }

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Settings.xml";
            
            STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("UpperLowerLimit");
            objFileHandle.WriteElement1Value("LowerLimit_Area", dgd_Setting.Rows[0].Cells[0].Value.ToString());
            objFileHandle.WriteElement1Value("LowerLimit_WidthLength", dgd_Setting.Rows[0].Cells[1].Value.ToString());
            objFileHandle.WriteElement1Value("LowerLimit_PitchGap", dgd_Setting.Rows[0].Cells[2].Value.ToString());
            objFileHandle.WriteElement1Value("UpperLimit_Area", dgd_Setting.Rows[1].Cells[0].Value.ToString());
            objFileHandle.WriteElement1Value("UpperLimit_WidthLength", dgd_Setting.Rows[1].Cells[1].Value.ToString());
            objFileHandle.WriteElement1Value("UpperLimit_PitchGap", dgd_Setting.Rows[1].Cells[2].Value.ToString());
            objFileHandle.WriteElement1Value("SetToCenterROI", chk_SetToCenterROI.Checked);
            objFileHandle.WriteElement1Value("SetToSideROI", chk_SetToSideROI.Checked);
            objFileHandle.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Pad Settings", m_smProductionInfo.g_strLotID);
            
            this.DialogResult = DialogResult.OK;

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            this.Close();
        }

        private void dgd_Setting_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            IsSettingCorrect();

        }
    }
}