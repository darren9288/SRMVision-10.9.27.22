using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Common;
using SharedMemory;

namespace AutoMode
{
    public partial class SelectVisionDisplay : Form
    {
        #region Member Variables

        private VisionInfo[] m_smVSInfo;
        private int[] m_arrSelectedVisionModules = new int[8];

        #endregion

        #region Properties

        public int[] ref_arrSelectedVisionModules { get { return m_arrSelectedVisionModules; } }

        #endregion

        public SelectVisionDisplay(VisionInfo[] smVSInfo, int[] arrSelectedVisionModules)
        {
            InitializeComponent();

            m_arrSelectedVisionModules = arrSelectedVisionModules;
            m_smVSInfo = smVSInfo;

            UpdateGUI();
        }

        public void UpdateGUI()
        {
            Control chk_VisionModule = chk_VisionModule1;

            for (int i = 0; i < 10; i++)
            {
                if (m_smVSInfo[i] != null)
                {
                    ((SRMControl.SRMCheckBox)chk_VisionModule).Text = m_smVSInfo[i].g_strVisionDisplayName + " " + m_smVSInfo[i].g_strVisionNameRemark;

                    for (int j = 0; j < 8; j++)
                    {
                        if (i == m_arrSelectedVisionModules[j])
                        {
                            ((SRMControl.SRMCheckBox)chk_VisionModule).Checked = true;
                            break;
                        }
                    }
                }
                else
                    ((SRMControl.SRMCheckBox)chk_VisionModule).Visible = false;

                chk_VisionModule = GetNextControl(chk_VisionModule, true);
            }

            Control cbo_VisionViewImage = cbo_Vision1ViewImage;

            for (int i = 0; i < 10; i++)
            {
                if (m_smVSInfo[i] != null)
                {
                    for (int j = 0; j < m_smVSInfo[i].g_arrImages.Count; j++)
                    {
                        ((SRMControl.SRMComboBox)cbo_VisionViewImage).Items.Add("Image " + (j + 1));
                    }

                    if (m_smVSInfo[i].g_intProductionViewImage < ((SRMControl.SRMComboBox)cbo_VisionViewImage).Items.Count)
                    {
                        ((SRMControl.SRMComboBox)cbo_VisionViewImage).SelectedIndex = m_smVSInfo[i].g_intProductionViewImage;
                    }
                    else
                    {
                        ((SRMControl.SRMComboBox)cbo_VisionViewImage).SelectedIndex = m_smVSInfo[i].g_intProductionViewImage = 0;
                    }

                }
                else
                    ((SRMControl.SRMComboBox)cbo_VisionViewImage).Visible = false;

                cbo_VisionViewImage = GetNextControl(cbo_VisionViewImage, true);
            }
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            int intSelectedCount = 0;
            Control chk_VisionModule = chk_VisionModule1;
            int[] arrVisionModules = new int[8];
            for (int i = 0; i < 10; i++)
            {
                if (m_smVSInfo[i] != null)
                {
                    if (((SRMControl.SRMCheckBox)chk_VisionModule).Checked)
                    {
                        arrVisionModules[intSelectedCount] = i;
                        intSelectedCount++;
                    }
                }

                chk_VisionModule = GetNextControl(chk_VisionModule, true);
            }

            if (intSelectedCount > 8)
            {
                SRMMessageBox.Show("Maximum only 8 vision modules can be selected.");
                return;
            }
            else if (intSelectedCount < 8)
            {
                SRMMessageBox.Show("Please select 8 vision modules.");
                return;
            }

            Control cbo_VisionViewImage = cbo_Vision1ViewImage;
            for (int i = 0; i < 10; i++)
            {
                if (m_smVSInfo[i] != null)
                {
                    m_smVSInfo[i].g_intProductionViewImage = m_smVSInfo[i].g_intSelectedImage = ((SRMControl.SRMComboBox)cbo_VisionViewImage).SelectedIndex;
                }

                cbo_VisionViewImage = GetNextControl(cbo_VisionViewImage, true);
            }

            m_arrSelectedVisionModules = arrVisionModules;

            Close();
            Dispose();
        }

        private void chk_VisionModule3_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}