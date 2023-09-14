using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Common;
using VisionProcessing;
using SharedMemory;
using System.IO;
namespace VisionProcessForm
{
    public partial class PackageOffsetSetting : Form
    {
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo m_smVisionInfo;
        private float fValue;
        private float fValue2;
        private bool bln_ShowThickness = false;
        public PackageOffsetSetting(VisionInfo m_smVisionInfo, CustomOption m_smCustomizeInfo,ProductionInfo m_smProductionInfo, int intSelectedUnit,bool bln_thickness)
        {
            this.m_smVisionInfo = m_smVisionInfo;
            this.m_smCustomizeInfo = m_smCustomizeInfo;
            this.m_smProductionInfo = m_smProductionInfo;
            bln_ShowThickness = bln_thickness;
            InitializeComponent();

            if (bln_ShowThickness)
            {
                group_PackageOffset2.BringToFront();
                group_PackageOffset2.Visible = true;
                group_PackageOffset2.Enabled = true;
                txt_WidthOffset2.Text = m_smVisionInfo.g_arrPad[intSelectedUnit].ref_fPackageWidthOffsetMM.ToString();
                txt_HeightOffset2.Text = m_smVisionInfo.g_arrPad[intSelectedUnit].ref_fPackageHeightOffsetMM.ToString();

                if (m_smVisionInfo.g_arrPad.Length > 1)
                    txt_ThicknessOffset.Text = m_smVisionInfo.g_arrPad[1].ref_fPackageThicknessOffsetMM.ToString();
                else
                {
                    txt_ThicknessOffset.Visible = false;
                    srmLabel1.Visible = false;
                    srmLabel20.Visible = false;
                }
            }
            else
            {
                group_PackageOffset2.Visible = false;
                group_PackageOffset2.Enabled = false;
                group_PackageOffset2.SendToBack();
                txt_WidthOffset.Text = m_smVisionInfo.g_arrPackage[0].ref_fWidthOffsetMM.ToString();
                txt_HeightOffset.Text = m_smVisionInfo.g_arrPackage[0].ref_fHeightOffsetMM.ToString();
            }
        }

        private void txt_WidthOffset_TextChanged(object sender, EventArgs e)
        {
            if (!bln_ShowThickness)
            {
                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                {
                    float fValuePrev = m_smVisionInfo.g_arrPackage[u].ref_fWidthOffsetMM;
                    if (!float.TryParse(txt_WidthOffset.Text, out fValue))
                    {
                        txt_WidthOffset.Text = fValuePrev.ToString("F" + 3);
                        SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        fValue = fValuePrev;
                    }
                }
            }
        }

        private void txt_HeightOffset_TextChanged(object sender, EventArgs e)
        {
            if(!bln_ShowThickness)
            {
                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                {
                    float fValuePrev = m_smVisionInfo.g_arrPackage[u].ref_fHeightOffsetMM;
                    if (!float.TryParse(txt_HeightOffset.Text, out fValue2))
                    {
                        txt_HeightOffset.Text = fValuePrev.ToString("F" + 3);
                        SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        fValue2 = fValuePrev;
                    }
                }
            }
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            if(bln_ShowThickness)
            {
                if (float.TryParse(txt_WidthOffset2.Text, out fValue))
                    m_smVisionInfo.g_arrPad[0].ref_fPackageWidthOffsetMM = fValue;

                if (float.TryParse(txt_HeightOffset2.Text, out fValue))           
                    m_smVisionInfo.g_arrPad[0].ref_fPackageHeightOffsetMM = fValue;

                if (float.TryParse(txt_ThicknessOffset.Text, out fValue))
                {
                    for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                        m_smVisionInfo.g_arrPad[i].ref_fPackageThicknessOffsetMM = fValue;
                }
            }
            else
            {
                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_fWidthOffsetMM = fValue;
                    m_smVisionInfo.g_arrPackage[u].ref_fHeightOffsetMM = fValue2;
                }
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
    }
}
