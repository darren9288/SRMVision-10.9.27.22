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
using System.IO;

namespace VisionProcessForm
{
    public partial class AdvancedLGaugeForm : Form
    {
        #region Member Variables

        private int m_intGaugeTransChoice;
        private int m_intGaugeTransType;
        private int m_intGaugeMinAmp;
        private int m_intGaugeMinArea;
        private int m_intGaugeFilter;
        private int m_intGaugeThickness;
        private int m_intGaugeThreshold;
        private int m_intGaugeFilteringPass;
        private float m_fGaugeFilteringThreshold;

        private bool m_blnInitDone = false;
        private bool m_blnCalibration = false;
        private string m_strPath = "";
        private VisionInfo m_smVisionInfo;
        private List<List<LGauge>> m_arrLGauge2 = new List<List<LGauge>>();
        private List<LGauge> m_arrLGauge = new List<LGauge>();
        private ProductionInfo m_smProductionInfo;
       
        #endregion

        #region Properties

        public int ref_intGaugeTransChoice { get { return m_intGaugeTransChoice; } }
        public int ref_intGaugeTransType { get { return m_intGaugeTransType; } }
        public int ref_intGaugeMinAmp { get { return m_intGaugeMinAmp; } }
        public int ref_intGaugeMinArea { get { return m_intGaugeMinArea; } }
        public int ref_intGaugeFilter { get { return m_intGaugeFilter; } }
        public int ref_intGaugeThickness { get { return m_intGaugeThickness; } }
        public int ref_intGaugeThreshold { get { return m_intGaugeThreshold; } }
        public int ref_intGaugeFilteringPass { get { return m_intGaugeFilteringPass; } }
        public float ref_fGaugeFilteringThreshold { get { return m_fGaugeFilteringThreshold; } }

        #endregion

        public AdvancedLGaugeForm(VisionInfo smVisionInfo, string strPath, ProductionInfo smProductionInfo, bool blnCalibration)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            m_strPath = strPath;
            m_blnCalibration = blnCalibration;

            switch (m_smVisionInfo.g_strVisionName)
            {
                case "Seal":
                    m_arrLGauge2 = m_smVisionInfo.g_arrSealGauges;
                    UpdateGUI(m_arrLGauge2[0][0]);
                    break;
                default:
                    if (blnCalibration)
                    {
                        m_arrLGauge = m_smVisionInfo.g_objCalibration.ref_arrLGauge;
                        UpdateGUI(m_arrLGauge[0]);
                    }
                    else
                    {
                        m_arrLGauge = m_smVisionInfo.g_arrPositioningGauges;
                        UpdateGUI(m_arrLGauge[0]);
                    }
                    break;
            }

            m_blnInitDone = true;
        }

        private void UpdateGUI(LGauge objGauge)
        {
            cbo_TransType.SelectedIndex = objGauge.ref_GaugeTransType;
            cbo_TransChoice.SelectedIndex = objGauge.ref_GaugeTransChoice;
            txt_MeasThickness.Text = objGauge.ref_GaugeThickness.ToString();
            txt_MeasFilter.Text = objGauge.ref_GaugeFilter.ToString();
            txt_MeasMinAmp.Text = objGauge.ref_GaugeMinAmplitude.ToString();
            txt_MeasMinArea.Text = objGauge.ref_GaugeMinArea.ToString();
            txt_FilteringPass.Text = objGauge.ref_GaugeFilterPasses.ToString();
            txt_FilteringThreshold.Text = objGauge.ref_GaugeFilterThreshold.ToString();
            txt_threshold.Text = objGauge.ref_GaugeThreshold.ToString();
        }

        private void LoadGaugeSetting()
        {
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "Seal":
                    LGauge.LoadFile(m_strPath, m_arrLGauge2, m_smVisionInfo.g_WorldShape); 
                    break;
                default:
                    if(File.Exists(m_strPath))
                        LGauge.LoadFile(m_strPath, m_arrLGauge, m_smVisionInfo.g_WorldShape);
                    break;
            }
        }

        private void SetTextToVariables()
        {
            m_intGaugeTransChoice = Convert.ToInt32(cbo_TransChoice.SelectedIndex);
            m_intGaugeTransType = Convert.ToInt32(cbo_TransType.SelectedIndex);
            m_intGaugeMinAmp = Convert.ToInt32(txt_MeasMinAmp.Text);
            m_intGaugeMinArea = Convert.ToInt32(txt_MeasMinArea.Text);
            m_intGaugeFilter = Convert.ToInt32(txt_MeasFilter.Text);
            m_intGaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
            m_intGaugeThreshold = Convert.ToInt32(txt_threshold.Text);

            m_intGaugeFilteringPass = Convert.ToInt32(txt_FilteringPass.Text);
            m_fGaugeFilteringThreshold = Convert.ToSingle(txt_FilteringThreshold.Text);

        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "Seal":
                    STDeviceEdit.CopySettingFile(m_strPath, "");
                    LGauge.SaveFile(m_strPath, m_arrLGauge2);
                    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName+">"+m_smVisionInfo.g_strVisionName + " LGauge", m_smProductionInfo.g_strLotID);
                    SetTextToVariables();
                    break;
                default:
                  
                    STDeviceEdit.CopySettingFile(m_strPath, "");
                    LGauge.SaveFile(m_strPath, m_arrLGauge);
                    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">" + m_smVisionInfo.g_strVisionName + " LGauge", m_smProductionInfo.g_strLotID);
                    SetTextToVariables();

                    break;
            }
            
            Close();
            Dispose();
        }

        private void txt_GaugeSetting_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch (m_smVisionInfo.g_strVisionName)
            {
                case "Seal":
                    for (int i = 0; i < m_arrLGauge2.Count; i++)
                    {
                        for (int j = 0; j < m_arrLGauge2[i].Count; j++)
                        {
                            m_arrLGauge2[i][j].SetGaugeAdvancedSetting(Convert.ToInt32(txt_MeasMinAmp.Text),
                                Convert.ToInt32(txt_MeasMinArea.Text), Convert.ToInt32(txt_MeasFilter.Text),
                                Convert.ToInt32(txt_MeasThickness.Text), cbo_TransChoice.SelectedIndex,
                                cbo_TransType.SelectedIndex, Convert.ToInt32(txt_FilteringPass.Text), Convert.ToInt32(txt_threshold.Text),
                                Convert.ToSingle(txt_FilteringThreshold.Text));
                        }
                    }
                    break;
                default:
                    for (int i = 0; i < m_arrLGauge.Count; i++)
                    {
                        m_arrLGauge[i].SetGaugeAdvancedSetting(Convert.ToInt32(txt_MeasMinAmp.Text),
                            Convert.ToInt32(txt_MeasMinArea.Text), Convert.ToInt32(txt_MeasFilter.Text),
                            Convert.ToInt32(txt_MeasThickness.Text), cbo_TransChoice.SelectedIndex,
                            cbo_TransType.SelectedIndex, Convert.ToInt32(txt_FilteringPass.Text), Convert.ToInt32(txt_threshold.Text),
                            Convert.ToSingle(txt_FilteringThreshold.Text));
                    }
                    break;
            }

            SetTextToVariables();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            LoadGaugeSetting();
        }
    }
}