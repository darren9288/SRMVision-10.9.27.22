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
    public partial class LeadUpdateToleranceUpperLowerLimitForm : Form
    {
        #region Member Variables
        private bool m_blnLead3D = false;
        private bool m_blnInitDone = false;
        private int m_intUserGroup = 5;
        private string m_strSelectedRecipe;

        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        public float ref_fLowerLimit_WidthLength { get { return Convert.ToSingle(dgd_Setting.Rows[0].Cells[0].Value.ToString()); } }
        public float ref_fLowerLimit_Pitch { get { return Convert.ToSingle(dgd_Setting.Rows[0].Cells[1].Value.ToString()); } }
        public float ref_fLowerLimit_Gap { get { return Convert.ToSingle(dgd_Setting.Rows[0].Cells[2].Value.ToString()); } }
        public float ref_fLowerLimit_Span { get { return Convert.ToSingle(dgd_Setting.Rows[0].Cells[3].Value.ToString()); } }
        public float ref_fUpperLimit_WidthLength { get { return Convert.ToSingle(dgd_Setting.Rows[1].Cells[0].Value.ToString()); } }
        public float ref_fUpperLimit_Pitch { get { return Convert.ToSingle(dgd_Setting.Rows[1].Cells[1].Value.ToString()); } }
        public float ref_fUpperLimit_Gap { get { return Convert.ToSingle(dgd_Setting.Rows[1].Cells[2].Value.ToString()); } }
        public float ref_fUpperLimit_Span{ get { return Convert.ToSingle(dgd_Setting.Rows[1].Cells[3].Value.ToString()); } }
        #endregion

        public LeadUpdateToleranceUpperLowerLimitForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo, bool blnLead3D)
        {
            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = smVisionInfo;
            m_intUserGroup = intUserGroup;
            m_smProductionInfo = smProductionInfo;
            m_blnLead3D = blnLead3D;
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

            string str = "";
            if (m_blnLead3D)
                str = "Lead3D";
            else
                str = "Lead";

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
             m_smVisionInfo.g_strVisionFolderName + "\\"+ str + "\\Settings.xml";

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("UpperLowerLimit");
            dgd_Setting.Rows[0].Cells[0].Value = objFileHandle.GetValueAsFloat("LowerLimit_WidthLength", 80);
            dgd_Setting.Rows[0].Cells[1].Value = objFileHandle.GetValueAsFloat("LowerLimit_Pitch", 80);
            dgd_Setting.Rows[0].Cells[2].Value = objFileHandle.GetValueAsFloat("LowerLimit_Gap", 80);
            dgd_Setting.Rows[0].Cells[3].Value = objFileHandle.GetValueAsFloat("LowerLimit_Span", 80);
            dgd_Setting.Rows[1].Cells[0].Value = objFileHandle.GetValueAsFloat("UpperLimit_WidthLength", 120);
            dgd_Setting.Rows[1].Cells[1].Value = objFileHandle.GetValueAsFloat("UpperLimit_Pitch", 120);
            dgd_Setting.Rows[1].Cells[2].Value = objFileHandle.GetValueAsFloat("UpperLimit_Gap", 120);
            dgd_Setting.Rows[1].Cells[3].Value = objFileHandle.GetValueAsFloat("UpperLimit_Span", 120);
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

            if (float.TryParse(dgd_Setting.Rows[0].Cells[3].Value.ToString(), out fValue))
            {
                if (fValue >= 100)
                {
                    blnResult = false;
                    dgd_Setting.Rows[0].Cells[3].Style.ForeColor = Color.Red;
                    dgd_Setting.Rows[0].Cells[3].Style.SelectionForeColor = Color.Red;
                }
                else
                {
                    dgd_Setting.Rows[0].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[0].Cells[3].Style.SelectionForeColor = Color.Black;
                }
            }
            else
            {
                blnResult = false;
                dgd_Setting.Rows[0].Cells[3].Style.ForeColor = Color.Red;
                dgd_Setting.Rows[0].Cells[3].Style.SelectionForeColor = Color.Red;
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

            if (float.TryParse(dgd_Setting.Rows[1].Cells[3].Value.ToString(), out fValue))
            {
                if (fValue <= 100)
                {
                    blnResult = false;
                    dgd_Setting.Rows[1].Cells[3].Style.ForeColor = Color.Red;
                    dgd_Setting.Rows[1].Cells[3].Style.SelectionForeColor = Color.Red;
                }
                else
                {
                    dgd_Setting.Rows[1].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[1].Cells[3].Style.SelectionForeColor = Color.Black;
                }
            }
            else
            {
                blnResult = false;
                dgd_Setting.Rows[1].Cells[3].Style.ForeColor = Color.Red;
                dgd_Setting.Rows[1].Cells[3].Style.SelectionForeColor = Color.Red;
            }

            return blnResult;
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

            string str = "";
            if (m_blnLead3D)
                str = "Lead3D";
            else
                str = "Lead";

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                   m_smVisionInfo.g_strVisionFolderName + "\\"+str+"\\Settings.xml";
            
            STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("UpperLowerLimit");
            objFileHandle.WriteElement1Value("LowerLimit_WidthLength", dgd_Setting.Rows[0].Cells[0].Value.ToString());
            objFileHandle.WriteElement1Value("LowerLimit_Pitch", dgd_Setting.Rows[0].Cells[1].Value.ToString());
            objFileHandle.WriteElement1Value("LowerLimit_Gap", dgd_Setting.Rows[0].Cells[2].Value.ToString());
            objFileHandle.WriteElement1Value("LowerLimit_Span", dgd_Setting.Rows[0].Cells[3].Value.ToString());
            objFileHandle.WriteElement1Value("UpperLimit_WidthLength", dgd_Setting.Rows[1].Cells[0].Value.ToString());
            objFileHandle.WriteElement1Value("UpperLimit_Pitch", dgd_Setting.Rows[1].Cells[1].Value.ToString());
            objFileHandle.WriteElement1Value("UpperLimit_Gap", dgd_Setting.Rows[1].Cells[2].Value.ToString());
            objFileHandle.WriteElement1Value("UpperLimit_Span", dgd_Setting.Rows[1].Cells[3].Value.ToString());
            objFileHandle.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">"+str, m_smProductionInfo.g_strLotID);
            
            this.DialogResult = DialogResult.Yes; // Return yes to update tolerance to table.
            this.Close();
        }

        private void btn_SaveAndClose_Click(object sender, EventArgs e)
        {
            if (!IsSettingCorrect())
            {
                SRMMessageBox.Show("Wrong setting! Please make sure values are in digit, below 100 for Lower Limit, and above 100 for upper limit.");
                return;
            }

            string str = "";
            if (m_blnLead3D)
                str = "Lead3D";
            else
                str = "Lead";

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\"+str+"\\Settings.xml";
            
            STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("UpperLowerLimit");
            objFileHandle.WriteElement1Value("LowerLimit_WidthLength", dgd_Setting.Rows[0].Cells[0].Value.ToString());
            objFileHandle.WriteElement1Value("LowerLimit_Pitch", dgd_Setting.Rows[0].Cells[1].Value.ToString());
            objFileHandle.WriteElement1Value("LowerLimit_Gap", dgd_Setting.Rows[0].Cells[2].Value.ToString());
            objFileHandle.WriteElement1Value("LowerLimit_Span", dgd_Setting.Rows[0].Cells[3].Value.ToString());
            objFileHandle.WriteElement1Value("UpperLimit_WidthLength", dgd_Setting.Rows[1].Cells[0].Value.ToString());
            objFileHandle.WriteElement1Value("UpperLimit_Pitch", dgd_Setting.Rows[1].Cells[1].Value.ToString());
            objFileHandle.WriteElement1Value("UpperLimit_Gap", dgd_Setting.Rows[1].Cells[2].Value.ToString());
            objFileHandle.WriteElement1Value("UpperLimit_Span", dgd_Setting.Rows[1].Cells[3].Value.ToString());
            objFileHandle.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">"+str, m_smProductionInfo.g_strLotID);
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void dgd_Setting_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            IsSettingCorrect();

        }

        private void LeadUpdateToleranceUpperLowerLimitForm_Load(object sender, EventArgs e)
        {

        }
    }
}