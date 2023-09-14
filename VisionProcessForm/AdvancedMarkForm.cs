using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using SharedMemory;
namespace VisionProcessForm
{
    public partial class AdvancedMarkForm : Form
    {
        #region Member Variables
        private bool m_blnWantSkipMarkPrev;
        private string m_strPath;
        private string m_strSelectedRecipe = "Default";
        private string m_strVisionName = "";
        private int m_intUserGroup = 5;
        private ProductionInfo m_smProductionInfo;
        #endregion

        #region Properties
        public bool ref_blnWhiteOnBlack { get { return chk_WhiteOnBlack.Checked; } }
        public bool ref_blnWantMultiGroups { get { return chk_MultiGroups.Checked; } }
        public bool ref_blnWantBuildText { get { return chk_WantBuildTexts.Checked; } }
        public bool ref_blnWantMultiTemplates { get { return chk_MultiTemplates.Checked; } }
        public bool ref_blnWantSet1ToAll { get { return chk_Set1ToAll.Checked; } }
        public bool ref_blnWantSkipMark { get { return chk_SkipMarkInspection.Checked; } }

        #endregion

        public AdvancedMarkForm(string strVisionName, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_strVisionName = strVisionName;
            m_intUserGroup = intUserGroup;
            m_strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
               strVisionName + "\\Mark\\Settings.xml";

            UpdateGUI();

            m_blnWantSkipMarkPrev = chk_SkipMarkInspection.Checked;
        }

        private void UpdateGUI()
        {
            XmlParser objFileHandle = new XmlParser(m_strPath);
            objFileHandle.GetFirstSection("Advanced");
            chk_WhiteOnBlack.Checked = objFileHandle.GetValueAsBoolean("WhiteOnBlack", true);
            chk_MultiGroups.Checked = objFileHandle.GetValueAsBoolean("WantMultiGroups", false);
            chk_WantBuildTexts.Checked = objFileHandle.GetValueAsBoolean("WantBuildTexts", false);
            chk_MultiTemplates.Checked = objFileHandle.GetValueAsBoolean("WantMultiTemplates", true);
            chk_Set1ToAll.Checked = objFileHandle.GetValueAsBoolean("WantSet1ToAll", false);
            chk_SkipMarkInspection.Checked = objFileHandle.GetValueAsBoolean("WantSkipMark", false);    
        }


        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (m_blnWantSkipMarkPrev != chk_SkipMarkInspection.Checked)
            {
                if (SRMMessageBox.Show("Change the Skip Mark Inspection will delete all previous templates. Are you sure you want to continue?",
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    return;
                }
            }
            
            STDeviceEdit.CopySettingFile(m_strPath, "");
            XmlParser objFileHandle = new XmlParser(m_strPath);
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("WhiteOnBlack", chk_WhiteOnBlack.Checked);
            objFileHandle.WriteElement1Value("WantMultiGroups", chk_MultiGroups.Checked);
            objFileHandle.WriteElement1Value("WantBuildTexts", chk_WantBuildTexts.Checked);
            objFileHandle.WriteElement1Value("WantMultiTemplates", chk_MultiTemplates.Checked);
            objFileHandle.WriteElement1Value("WantSet1ToAll", chk_Set1ToAll.Checked);
            objFileHandle.WriteElement1Value("WantSkipMark", chk_SkipMarkInspection.Checked);
            objFileHandle.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_strVisionName + ">Mark", m_smProductionInfo.g_strLotID);
            
            Close();
        }
    }
}