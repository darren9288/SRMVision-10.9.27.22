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
    public partial class AdvancedOrientForm : Form
    {
        #region Member Variables
        private string m_strPath = "";
        private int m_intVisionType;
        private string m_strVisionName;
        private ProductionInfo m_smProductionInfo;
        #endregion

        #region Properties 
        public int ref_intDirection 
        { 
            get {
                if (radioBtn_2Directions.Checked)
                    return 2;
                else 
                    return 4;
            } 
        }
        public bool ref_blnWantSubROI { get { return chk_WantSubROI.Checked; } }

        #endregion


        public AdvancedOrientForm(string strVisionName, string strSelectedRecipe, int intVisionType, ProductionInfo smProductionInfo)
        {
            m_intVisionType = intVisionType;
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_strVisionName = strVisionName;
            m_strPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\" +
                strVisionName + "\\Orient\\Settings.xml";

            UpdateGUI();
        }



        private void UpdateGUI()
        {
         
            XmlParser objFileHandle = new XmlParser(m_strPath);
            objFileHandle.GetFirstSection("Advanced");
            radioBtn_2Directions.Checked = (objFileHandle.GetValueAsInt("Direction", 4) == 2);
            radioBtn_4Directions.Checked = (objFileHandle.GetValueAsInt("Direction", 4) == 4);
            chk_WantSubROI.Checked = objFileHandle.GetValueAsBoolean("WantSubROI", false);

            // if vision not just orient
            if ((m_intVisionType & 0x0E) > 0)
            {
                chk_WantSubROI.Checked = false;
                chk_WantSubROI.Visible = false;
            }
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
            if (radioBtn_2Directions.Checked)
                objFileHandle.WriteElement1Value("Direction", 2);
            else 
                objFileHandle.WriteElement1Value("Direction", 4);

            objFileHandle.WriteElement1Value("WantSubROI", chk_WantSubROI.Checked);

            objFileHandle.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_strVisionName + ">Mark", m_smProductionInfo.g_strLotID);
            
            Close();
        }

        

    }
}