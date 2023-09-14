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
    public partial class CheckPresentOtherSettingForm : Form
    {
        #region Member Variables

        private bool m_blnInitDone = false;

        private int m_intUserGroup;
        private string m_strFolderPath;
        private string m_strSelectedRecipe;

        private VisionInfo m_smVisionInfo;
        private CustomOption m_smCustomizeInfo;
        private UserRight m_objUserRight = new UserRight();
        private ProductionInfo m_smProductionInfo;
        

        #endregion
        public CheckPresentOtherSettingForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            InitializeComponent();

            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;
            m_smCustomizeInfo = smCustomizeInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = smVisionInfo;

            m_strFolderPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\CheckPresent\\";
            
            UpdateGUI();

            m_blnInitDone = true;
        }

        private void UpdateGUI()
        {
            lbl_UnitPresentThreshold.Text = m_smVisionInfo.g_objUnitPresent.ref_intThresholdValue.ToString();
        }


        private void btn_ThresholdUnitROI_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objUnitPresent.ref_intThresholdValue;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPositioningROIs[0], true, false, new List<int>());
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_objUnitPresent.ref_intThresholdValue = m_smVisionInfo.g_intThresholdValue;
            }
            lbl_UnitPresentThreshold.Text = m_smVisionInfo.g_objUnitPresent.ref_intThresholdValue.ToString();
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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

        private void CheckPresentOtherSettingForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewUnitPresentObjectsBuilded = false;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            if(m_smVisionInfo.g_arrPositioningROIs.Count > 0)
                m_smVisionInfo.g_arrPositioningROIs[0].AttachImage(m_smVisionInfo.g_arrImages[0]);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void CheckPresentOtherSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Unit Present Setting Form Closed", "Exit Unit Present Setting Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.VM_AT_SettingInDialog = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
    }
}
