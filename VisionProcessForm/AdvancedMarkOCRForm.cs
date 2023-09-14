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
    public partial class AdvancedMarkOCRForm : Form
    {
        #region Member Variables
        private string m_strPath;
        private string m_strSelectedRecipe = "Default";
        private string m_strVisionName = "";
        private int m_intUserGroup = 5;
        private ProductionInfo m_smProductionInfo;
        #endregion

        #region Properties
        public bool ref_blnWhiteOnBlack { get { return chk_WhiteOnBlack.Checked; } }
        public bool ref_blnWantMultiTemplates { get { return chk_MultiTemplates.Checked; } }
        public bool ref_blnWantRemoveBorderMode { get { return chk_RemoveBorderMode.Checked; } }
        public bool ref_blnWantRecogPosition { get { return chk_RecogCharPosition.Checked; } }

        #endregion

        public AdvancedMarkOCRForm(string strVisionName, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_strVisionName = strVisionName;
            m_intUserGroup = intUserGroup;
            m_strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
               strVisionName + "\\Mark\\Settings.xml";

            DisableField();
            UpdateGUI();
        }

        private void DisableField()
        {
        }

        private void UpdateGUI()
        {
            XmlParser objFileHandle = new XmlParser(m_strPath);
            objFileHandle.GetFirstSection("Advanced");
            chk_WhiteOnBlack.Checked = objFileHandle.GetValueAsBoolean("WhiteOnBlack", true);
            chk_MultiTemplates.Checked = objFileHandle.GetValueAsBoolean("WantMultiTemplates", true);
            chk_RemoveBorderMode.Checked = objFileHandle.GetValueAsBoolean("WantBorderRemoveMode", false);
            chk_RecogCharPosition.Checked = objFileHandle.GetValueAsBoolean("WantRecogCharPosition", false);
            chk_SkipMarkInspection.Checked = objFileHandle.GetValueAsBoolean("WantSkipMark", false);
        }


        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            
            STDeviceEdit.CopySettingFile(m_strPath, "");
            XmlParser objFileHandle = new XmlParser(m_strPath);
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("WhiteOnBlack", chk_WhiteOnBlack.Checked);
            objFileHandle.WriteElement1Value("WantMultiTemplates", chk_MultiTemplates.Checked);
            objFileHandle.WriteElement1Value("WantBorderRemoveMode", chk_RemoveBorderMode.Checked);
            objFileHandle.WriteElement1Value("WantRecogCharPosition", chk_RecogCharPosition.Checked);
            objFileHandle.WriteElement1Value("WantSkipMark", chk_SkipMarkInspection.Checked);
            objFileHandle.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_strVisionName + ">Mark", m_smProductionInfo.g_strLotID);
            
            Close();
        }
    }
}