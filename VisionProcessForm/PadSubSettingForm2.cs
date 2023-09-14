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
using System.IO;
namespace VisionProcessForm
{
    public partial class PadSubSettingForm2 : Form
    {
        private bool m_blnInitDone = false;
        private string m_strSelectedRecipe;
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();

        #region Properties

        public bool ref_blnUseBorderLimit_Center { get { return chk_UseBorderLimit_Center.Checked; } }
        public bool ref_blnUseBorderLimit_Top { get { return chk_UseBorderLimit_Top.Checked; } }
        public bool ref_blnUseBorderLimit_Right { get { return chk_UseBorderLimit_Right.Checked; } }
        public bool ref_blnUseBorderLimit_Bottom { get { return chk_UseBorderLimit_Bottom.Checked; } }
        public bool ref_blnUseBorderLimit_Left { get { return chk_UseBorderLimit_Left.Checked; } }

        #endregion

        public PadSubSettingForm2(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, int intSelectedTabPage, bool blnCheck4Sides)
        {
            InitializeComponent();

            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;

            if ((m_smVisionInfo.g_arrPad.Length == 1) || (m_smVisionInfo.g_arrPad.Length > 1 && !blnCheck4Sides))
            {
                panel_SelectEdge.Height = 74;
                this.Height = this.Height - 120;
                btn_OK.Location = new Point(btn_OK.Location.X, btn_OK.Location.Y - 120);
                btn_Cancel.Location = new Point(btn_Cancel.Location.X, btn_Cancel.Location.Y - 120);
            }

            UpdateGUI();

            m_blnInitDone = true;
        }

        private void UpdateGUI()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                switch (i)
                {
                    case 0:
                        chk_UseBorderLimit_Center.Checked = m_smVisionInfo.g_arrPad[i].ref_blnWantUseBorderLimitAsOffset;
                        break;
                    case 1:
                        chk_UseBorderLimit_Top.Checked = m_smVisionInfo.g_arrPad[i].ref_blnWantUseBorderLimitAsOffset;
                        break;
                    case 2:
                        chk_UseBorderLimit_Right.Checked = m_smVisionInfo.g_arrPad[i].ref_blnWantUseBorderLimitAsOffset;
                        break;
                    case 3:
                        chk_UseBorderLimit_Bottom.Checked = m_smVisionInfo.g_arrPad[i].ref_blnWantUseBorderLimitAsOffset;
                        break;
                    case 4:
                        chk_UseBorderLimit_Left.Checked = m_smVisionInfo.g_arrPad[i].ref_blnWantUseBorderLimitAsOffset;
                        break;
                }

            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void srmCheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
