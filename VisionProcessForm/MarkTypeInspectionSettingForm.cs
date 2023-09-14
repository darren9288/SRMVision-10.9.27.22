using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedMemory;
using VisionProcessing;
using Common;
using Microsoft.Win32;

namespace VisionProcessForm
{
    public partial class MarkTypeInspectionSettingForm : Form
    {
        private string m_strSelectedRecipe;
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();

        #region Properties
        public bool ref_blnWantCheckCharExcessMark { get { return chk_WantCheckCharExcessMark.Checked; } }
        public bool ref_blnWantCheckCharMissingMark { get { return chk_WantCheckCharMissingMark.Checked; } }
        public bool ref_blnWantCheckCharBrokenMark { get { return chk_WantCheckCharBrokenMark.Checked; } }
        public bool ref_blnWantCheckLogoExcessMark { get { return chk_WantCheckLogoExcessMark.Checked; } }
        public bool ref_blnWantCheckLogoMissingMark { get { return chk_WantCheckLogoMissingMark.Checked; } }
        public bool ref_blnWantCheckLogoBrokenMark { get { return chk_WantCheckLogoBrokenMark.Checked; } }
        public bool ref_blnWantCheckSymbol1ExcessMark { get { return chk_WantCheckSymbol1ExcessMark.Checked; } }
        public bool ref_blnWantCheckSymbol1MissingMark { get { return chk_WantCheckSymbol1MissingMark.Checked; } }
        public bool ref_blnWantCheckSymbol1BrokenMark { get { return chk_WantCheckSymbol1BrokenMark.Checked; } }
        public bool ref_blnWantCheckSymbol2ExcessMark { get { return chk_WantCheckSymbol2ExcessMark.Checked; } }
        public bool ref_blnWantCheckSymbol2MissingMark { get { return chk_WantCheckSymbol2MissingMark.Checked; } }
        public bool ref_blnWantCheckSymbol2BrokenMark { get { return chk_WantCheckSymbol2BrokenMark.Checked; } }
        #endregion
        public MarkTypeInspectionSettingForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup)
        {
            InitializeComponent();

            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;

            UpdateGUI();

        }
        private void UpdateGUI()
        {
            chk_WantCheckCharExcessMark.Checked = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckCharExcessMark;
            chk_WantCheckCharMissingMark.Checked = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckCharMissingMark;
            chk_WantCheckCharBrokenMark.Checked = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckCharBrokenMark;
            chk_WantCheckLogoExcessMark.Checked = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckLogoExcessMark;
            chk_WantCheckLogoMissingMark.Checked = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckLogoMissingMark;
            chk_WantCheckLogoBrokenMark.Checked = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckLogoBrokenMark;
            chk_WantCheckSymbol1ExcessMark.Checked = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol1ExcessMark;
            chk_WantCheckSymbol1MissingMark.Checked = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol1MissingMark;
            chk_WantCheckSymbol1BrokenMark.Checked = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol1BrokenMark;
            chk_WantCheckSymbol2ExcessMark.Checked = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol2ExcessMark;
            chk_WantCheckSymbol2MissingMark.Checked = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol2MissingMark;
            chk_WantCheckSymbol2BrokenMark.Checked = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol2BrokenMark;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
