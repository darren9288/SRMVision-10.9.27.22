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
    public partial class AdvancedPositionForm : Form
    {
        #region Member Variables
        private string m_strPath;
        private string m_strSelectedRecipe = "Default";
        private int m_intUserGroup = 5;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        #endregion

        #region Properties

        #endregion

        public AdvancedPositionForm(VisionInfo visionInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = visionInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_intUserGroup = intUserGroup;

            UpdateGUI();
        }

        /// <summary>
        /// Display current pad settings
        /// </summary>
        private void UpdateGUI()
        {
            if (m_smVisionInfo.g_objPositioning.ref_intDirections == 2)
            {
                radioBtn_2Directions.Checked = true;
            }
            else
            {
                radioBtn_4Directions.Checked = true;
            }
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (radioBtn_2Directions.Checked)
                m_smVisionInfo.g_objPositioning.ref_intDirections = 2;
            else
                m_smVisionInfo.g_objPositioning.ref_intDirections = 4;

            if (m_smVisionInfo.g_objPositioning2 != null)
            {
                if (radioBtn_2Directions.Checked)
                    m_smVisionInfo.g_objPositioning2.ref_intDirections = 2;
                else
                    m_smVisionInfo.g_objPositioning2.ref_intDirections = 4;
            }
            
            //STDeviceEdit.CopySettingFile(m_strPath, "");
            m_smVisionInfo.g_objPositioning.SaveAdvanceOrient(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Positioning\\Settings.xml", false, "Orient", false);
            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Positioning", m_smProductionInfo.g_strLotID);
            
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }
    }
}