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
    public partial class PosCrosshairSettingForm : Form
    {
        #region Member Variables

        private bool m_blnInitDone = false;
        private bool m_blnSaveWhenPressOK = false;
        private int m_intImageWidth = 640;
        private int m_intImageHeight = 480;
        private string m_strSavePath;
        private VisionInfo m_smVisionInfo;
        private Crosshair m_objCrosshair;
        private ProductionInfo m_smProductionInfo;
        #endregion

        public PosCrosshairSettingForm(VisionInfo smVisionInfo, string strSavePath, ProductionInfo smProductionInfo)
        {
            m_smVisionInfo = smVisionInfo;
            m_strSavePath = strSavePath;
            m_objCrosshair = m_smVisionInfo.g_objPositioning.ref_objCrosshair;
            m_blnSaveWhenPressOK = true;
            m_smProductionInfo = smProductionInfo;
            m_intImageWidth = m_smVisionInfo.g_intCameraResolutionWidth;
            m_intImageHeight = m_smVisionInfo.g_intCameraResolutionHeight;

            InitializeComponent();

            UpdateGUI();
            m_blnInitDone = true;
        }

        public PosCrosshairSettingForm(VisionInfo smVisionInfo, Crosshair objCrosshair, string strSavePath, bool blnSaveWhenPressOK)
        {
            m_smVisionInfo = smVisionInfo;
            m_strSavePath = strSavePath;
            m_objCrosshair = objCrosshair;
            m_blnSaveWhenPressOK = blnSaveWhenPressOK;

            m_intImageWidth = m_smVisionInfo.g_intCameraResolutionWidth;
            m_intImageHeight = m_smVisionInfo.g_intCameraResolutionHeight;

            InitializeComponent();

            UpdateGUI();
            m_blnInitDone = true;
        }
        
        
        private void UpdateGUI()
        {
            txt_CenterX.Minimum = -m_intImageWidth / 2;
            txt_CenterY.Maximum = m_intImageWidth / 2;
            txt_CenterY.Minimum = -m_intImageHeight / 2;
            txt_CenterY.Maximum = m_intImageHeight / 2;
            txt_ROIWidth.Maximum = m_intImageWidth - 10;
            txt_ROIHeight.Maximum = m_intImageHeight - 10;

            txt_CenterX.Value = m_objCrosshair.ref_intCrosshairX - m_intImageWidth / 2;
            txt_CenterY.Value = m_objCrosshair.ref_intCrosshairY - m_intImageHeight / 2;
            txt_ROIWidth.Value = m_objCrosshair.ref_objCrosshairROI.ref_ROIWidth;
            txt_ROIHeight.Value = m_objCrosshair.ref_objCrosshairROI.ref_ROIHeight;

            if (m_blnSaveWhenPressOK)
            {
                btn_Close.Visible = false;
            }
            else
            {
                btn_OK.Visible = false;
                btn_Cancel.Visible = false;
            }

        }




        private void txt_Center_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_objCrosshair.SetCrosshair((int)txt_CenterX.Value + m_intImageWidth / 2, (int)txt_CenterY.Value + m_intImageHeight / 2);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            m_objCrosshair.LoadCrosshair(m_strSavePath, "PositionCrosshair");

            Close();
            Dispose();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (m_blnSaveWhenPressOK)
            {
                
                STDeviceEdit.CopySettingFile(m_strSavePath, "");
                m_objCrosshair.SaveCrosshair(m_strSavePath, false, "PositionCrosshair", true);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Crosshair Settings", m_smProductionInfo.g_strLotID);
                
            }
            Close();
            Dispose();
        }

        private void PosCrosshairSettingForm_Load(object sender, EventArgs e)
        {
            m_objCrosshair.ref_blnDrawCrasshair = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void PosCrosshairSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_objCrosshair.ref_blnDrawCrasshair = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ROISize_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_objCrosshair.SetCrosshairROISize((int)txt_ROIWidth.Value, (int)txt_ROIHeight.Value);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }
    }
}