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
    public partial class PadGeneralSettingForm : Form
    {
        #region enum

        public enum DefectType 
        { 
            PadExtraArea = 0, // Foreign Material/Contamination
            PadBrokenArea = 1,
        };

        #endregion


        #region Member Variables

        private int m_intUserGroup = 5;
        private string m_strSelectedRecipe;

        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private DefectType m_intDefectType = DefectType.PadExtraArea;
        #endregion

        #region Properties

        //public bool ref_blnPadWhiteOnBlack { get { return chk_PadWhiteOnBlack.Checked; } }

        #endregion

        public PadGeneralSettingForm(VisionInfo visionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, 
            int intUserGroup, DefectType intDefetcType)
        {
            InitializeComponent();

            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;
            m_intDefectType = intDefetcType;

            UpdateGUI();
        }

        public void UpdateGUI()
        {
            switch (m_intDefectType)
            {
                case DefectType.PadExtraArea:
                    txt_XDimTolerance.Text = m_smVisionInfo.g_arrPad[0].GetExtraPadLengthLimit(m_smCustomizeInfo.g_intUnitDisplay).ToString("F4");
                    txt_YDimTolerance.Text = m_smVisionInfo.g_arrPad[0].GetExtraPadLengthLimit(m_smCustomizeInfo.g_intUnitDisplay).ToString("F4");
                    break;
                case DefectType.PadBrokenArea:
                    break;
            }

            UpdateUnitDisplay();
        }

        public void UpdateUnitDisplay()
        {
            string strUnitDisplay;
            switch (m_smCustomizeInfo.g_intUnitDisplay)
            {
                case 1:
                default:
                    strUnitDisplay = "mm";
                    break;
                case 2:
                    strUnitDisplay = "mil";
                    break;
                case 3:
                    strUnitDisplay = "um";
                    break;
            }

            srmLabel2.Text = strUnitDisplay;
            srmLabel4.Text = strUnitDisplay;
            srmLabel6.Text = strUnitDisplay + "^2";
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            switch (m_intDefectType)
            {
                case DefectType.PadExtraArea:
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        m_smVisionInfo.g_arrPad[0].SetExtraPadLengthLimit(Convert.ToSingle(txt_XDimTolerance.Text), m_smCustomizeInfo.g_intUnitDisplay);
                    }
                    break;
                case DefectType.PadBrokenArea:
                    break;
            }

            this.Close();
            this.Dispose();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}