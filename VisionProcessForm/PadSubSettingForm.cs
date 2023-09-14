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
    public partial class PadSubSettingForm : Form
    {
        private bool m_blnInitDone = false;
        private string m_strSelectedRecipe;
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();

        #region Properties

        public int ref_intSensitivityOnPadMethod_Center { get { return cbo_SensitivityOnPad_Center.SelectedIndex; } }
        public int ref_intSensitivityOnPadMethod_Top { get { return cbo_SensitivityOnPad_Top.SelectedIndex; } }
        public int ref_intSensitivityOnPadMethod_Right { get { return cbo_SensitivityOnPad_Right.SelectedIndex; } }
        public int ref_intSensitivityOnPadMethod_Bottom { get { return cbo_SensitivityOnPad_Bottom.SelectedIndex; } }
        public int ref_intSensitivityOnPadMethod_Left { get { return cbo_SensitivityOnPad_Left.SelectedIndex; } }

        public int ref_intSensitivityOnPadValue_Center { get { return Convert.ToInt32(txt_SensitivityValue_Center.Text); } }
        public int ref_intSensitivityOnPadValue_Top { get { return Convert.ToInt32(txt_SensitivityValue_Top.Text); } }
        public int ref_intSensitivityOnPadValue_Right { get { return Convert.ToInt32(txt_SensitivityValue_Right.Text); } }
        public int ref_intSensitivityOnPadValue_Bottom { get { return Convert.ToInt32(txt_SensitivityValue_Bottom.Text); } }
        public int ref_intSensitivityOnPadValue_Left { get { return Convert.ToInt32(txt_SensitivityValue_Left.Text); } }

        public bool ref_blnAutoSensitivity_Center { get { return chk_AutoSensitivity_Center.Checked; } }
        public bool ref_blnAutoSensitivity_Top { get { return chk_AutoSensitivity_Top.Checked; } }
        public bool ref_blnAutoSensitivity_Right { get { return chk_AutoSensitivity_Right.Checked; } }
        public bool ref_blnAutoSensitivity_Bottom { get { return chk_AutoSensitivity_Bottom.Checked; } }
        public bool ref_blnAutoSensitivity_Left { get { return chk_AutoSensitivity_Left.Checked; } }

        #endregion

        public PadSubSettingForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, int intSelectedTabPage, bool blnCheck4Sides)
        {
            InitializeComponent();

            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;

            if ((m_smVisionInfo.g_arrPad.Length == 1) || (m_smVisionInfo.g_arrPad.Length > 1 && !blnCheck4Sides))
            {
                panel_SelectEdge.Height = 60;
                this.Height = this.Height - 134;
                btn_OK.Location = new Point(btn_OK.Location.X, btn_OK.Location.Y - 134);
                btn_Cancel.Location = new Point(btn_Cancel.Location.X, btn_Cancel.Location.Y - 134);
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
                        cbo_SensitivityOnPad_Center.SelectedIndex = m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadMethod;
                        txt_SensitivityValue_Center.Text = m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadValue.ToString();
                        chk_AutoSensitivity_Center.Checked = m_smVisionInfo.g_arrPad[i].ref_intInspectPadMode == 0x01;
                        break;
                    case 1:
                        cbo_SensitivityOnPad_Top.SelectedIndex = m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadMethod;
                        txt_SensitivityValue_Top.Text = m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadValue.ToString();
                        chk_AutoSensitivity_Top.Checked = m_smVisionInfo.g_arrPad[i].ref_intInspectPadMode == 0x01;
                        break;
                    case 2:
                        cbo_SensitivityOnPad_Right.SelectedIndex = m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadMethod;
                        txt_SensitivityValue_Right.Text = m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadValue.ToString();
                        chk_AutoSensitivity_Right.Checked = m_smVisionInfo.g_arrPad[i].ref_intInspectPadMode == 0x01;
                        break;
                    case 3:
                        cbo_SensitivityOnPad_Bottom.SelectedIndex = m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadMethod;
                        txt_SensitivityValue_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadValue.ToString();
                        chk_AutoSensitivity_Bottom.Checked = m_smVisionInfo.g_arrPad[i].ref_intInspectPadMode == 0x01;
                        break;
                    case 4:
                        cbo_SensitivityOnPad_Left.SelectedIndex = m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadMethod;
                        txt_SensitivityValue_Left.Text = m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadValue.ToString();
                        chk_AutoSensitivity_Left.Checked = m_smVisionInfo.g_arrPad[i].ref_intInspectPadMode == 0x01;
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
