using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SharedMemory;
using Common;

namespace VisionModule
{
    public partial class VisionGRRPage : Form
    {
        private int m_intPartCountPrev = -1;
        private int m_intOperatorCountPrev = -1;
        private int m_intTrialPrev = -1;
        private bool m_blnInitDone = false;
        private string m_strResult = "", m_strResultPrev = "";

        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo m_smVisionInfo;

        public VisionGRRPage(CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, VisionInfo visionInfo)
        {
            InitializeComponent();

            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = visionInfo;
            m_smCustomizeInfo = smCustomizeInfo;

            CustomizeGUI();

            m_blnInitDone = true;
        }

        public bool CheckGRRFileExist(string strLotId)
        {
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "GRRReport\\" + strLotId))
                return true;
            else
                return false;
        }

        public void CustomizeGUI()
        {

            switch(m_smVisionInfo.g_objGRR.ref_intGRRMode)
            {
                case 0:
                    radioBtn_Static.Checked = true;
                    radioBtn_Dynamic.Checked = false;
                    //lbl_Parts2.Location = new Point(9, 34);
                    //lbl_Operators2.Location = new Point(9, 67);
                    //lbl_Trials2.Location = new Point(9, 102);
                    //lbl_PartsCounter.Location = new Point(71, 33);
                    //lbl_OperatorsCounter.Location = new Point(71, 65);
                    //lbl_TrialsCounter.Location = new Point(71, 100);
                    break;
                case 1:
                    radioBtn_Static.Checked = false;
                    radioBtn_Dynamic.Checked = true;
                    //lbl_Operators2.Location = new Point(9, 34);
                    //lbl_Trials2.Location = new Point(9, 67);
                    //lbl_Parts2.Location = new Point(9, 102);
                    //lbl_OperatorsCounter.Location = new Point(71, 33);
                    //lbl_TrialsCounter.Location = new Point(71, 65);
                    //lbl_PartsCounter.Location = new Point(71, 100);
                    break;
            }

            txt_Operators.Text = m_smVisionInfo.g_objGRR.ref_intMaxOp.ToString();
            txt_Parts.Text = m_smVisionInfo.g_objGRR.ref_intMaxPart.ToString();
            txt_Trials.Text = m_smVisionInfo.g_objGRR.ref_intMaxTrial.ToString();
        }

        public void InitPkg()
        {

            int intTotalPkgCount =1;
            //for (int i = 0; i < m_smVisionInfo.g_arrPackage.Count; i++)
            //    intTotalPkgCount += 1;

            // Set init setting
            m_smVisionInfo.g_objGRR.Init(m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime,
                                              m_smProductionInfo.g_strOperatorID,
                                              m_smProductionInfo.g_strRecipeID,
                                              m_smCustomizeInfo.g_strMachineID,
                                              m_smVisionInfo.g_strVisionName,
                                              intTotalPkgCount,
                                              Convert.ToInt32(txt_Parts.Text),
                                              Convert.ToInt32(txt_Operators.Text),
                                              Convert.ToInt32(txt_Trials.Text));

           
            string[] strTemplateFeature = new string[100];

            int intGroupIndex = 0;
            for (int p = 0; p < m_smVisionInfo.g_arrPackage.Count; p++)
            {

                //for (int i = 0; i < 2; i++)
                //{
                //    if(i==0)
                    m_smVisionInfo.g_objGRR.SetSpecification(0, intGroupIndex, Convert.ToSingle(m_smVisionInfo.g_arrPackage[p].ref_fUnitWidthMin), Convert.ToSingle(m_smVisionInfo.g_arrPackage[p].ref_fUnitWidthMax));

                   // if(i==1)
                    m_smVisionInfo.g_objGRR.SetSpecification(1, intGroupIndex, Convert.ToSingle(m_smVisionInfo.g_arrPackage[p].ref_fUnitHeightMin), Convert.ToSingle(m_smVisionInfo.g_arrPackage[p].ref_fUnitHeightMax));


                    intGroupIndex++;
              //  }
            }
        }

        public void InitInPocketPkg()
        {

            int intTotalInPocketPkgCount = 1;
            //for (int i = 0; i < m_smVisionInfo.g_arrPackage.Count; i++)
            //    intTotalPkgCount += 1;

            // Set init setting
            m_smVisionInfo.g_objGRR.Init(m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime,
                                              m_smProductionInfo.g_strOperatorID,
                                              m_smProductionInfo.g_strRecipeID,
                                              m_smCustomizeInfo.g_strMachineID,
                                              m_smVisionInfo.g_strVisionName,
                                              intTotalInPocketPkgCount,
                                              Convert.ToInt32(txt_Parts.Text),
                                              Convert.ToInt32(txt_Operators.Text),
                                              Convert.ToInt32(txt_Trials.Text));

        
            string[] strTemplateFeature = new string[100];

            int intGroupIndex = 0;
            for (int p = 0; p < m_smVisionInfo.g_arrPackage.Count; p++)
            {

                //for (int i = 0; i < 2; i++)
                //{
                //    if(i==0)
                m_smVisionInfo.g_objGRR.SetSpecification(0, intGroupIndex, Convert.ToSingle(m_smVisionInfo.g_arrPackage[p].ref_fUnitWidthMin), Convert.ToSingle(m_smVisionInfo.g_arrPackage[p].ref_fUnitWidthMax));

                // if(i==1)
                m_smVisionInfo.g_objGRR.SetSpecification(1, intGroupIndex, Convert.ToSingle(m_smVisionInfo.g_arrPackage[p].ref_fUnitHeightMin), Convert.ToSingle(m_smVisionInfo.g_arrPackage[p].ref_fUnitHeightMax));


                intGroupIndex++;
                //  }
            }
        }

        public void Init()
        {
            int intTotalPadCount = 0;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                intTotalPadCount += m_smVisionInfo.g_arrPad[i].GetBlobsFeaturesNumber();
            }
            // Set init setting
            m_smVisionInfo.g_objGRR.Init(m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime,
                                              m_smProductionInfo.g_strOperatorID,
                                              m_smProductionInfo.g_strRecipeID,
                                              m_smCustomizeInfo.g_strMachineID,
                                              m_smVisionInfo.g_strVisionName,
                                              intTotalPadCount,
                                              Convert.ToInt32(txt_Parts.Text),
                                              Convert.ToInt32(txt_Operators.Text),
                                              Convert.ToInt32(txt_Trials.Text));

            string strTemplateBlobsFeatures;
            string[] strTemplateFeature = new string[100];
            int intPadNumber;

            int intGroupIndex = 0;
            for (int p = 0; p < m_smVisionInfo.g_arrPad.Length; p++)
            {
                if (p > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                intPadNumber = m_smVisionInfo.g_arrPad[p].GetBlobsFeaturesNumber();

                for (int i = 0; i < intPadNumber; i++)
                {
                    strTemplateBlobsFeatures = m_smVisionInfo.g_arrPad[p].GetBlobFeaturesInspectRealData(i);
                    strTemplateFeature = strTemplateBlobsFeatures.Split('#');

                    for (int j = 0; j < strTemplateFeature.Length; j++)
                    {
                        if (strTemplateFeature[j] != "")
                            if (Convert.ToSingle(strTemplateFeature[j]) == -1)
                                strTemplateFeature[j] = "0";
                    }

                    m_smVisionInfo.g_objGRR.SetSpecification(0, intGroupIndex, 0f, Convert.ToSingle(strTemplateFeature[0]));
                    m_smVisionInfo.g_objGRR.SetSpecification(1, intGroupIndex, Convert.ToSingle(strTemplateFeature[1]), Convert.ToSingle(strTemplateFeature[2]));
                    m_smVisionInfo.g_objGRR.SetSpecification(2, intGroupIndex, Convert.ToSingle(strTemplateFeature[3]), Convert.ToSingle(strTemplateFeature[4]));
                    m_smVisionInfo.g_objGRR.SetSpecification(3, intGroupIndex, Convert.ToSingle(strTemplateFeature[5]), Convert.ToSingle(strTemplateFeature[6]));
                    m_smVisionInfo.g_objGRR.SetSpecification(4, intGroupIndex, Convert.ToSingle(strTemplateFeature[7]), Convert.ToSingle(strTemplateFeature[8]));
                    m_smVisionInfo.g_objGRR.SetSpecification(5, intGroupIndex, Convert.ToSingle(strTemplateFeature[9]), Convert.ToSingle(strTemplateFeature[10]));

                    intGroupIndex++;
                }
            }
        }

        public void InitLead3D()
        {
            int intTotalLeadCount = 0;
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != 0)
                    break;

                intTotalLeadCount += m_smVisionInfo.g_arrLead3D[i].GetBlobsFeaturesNumber();
            }
            // Set init setting
            m_smVisionInfo.g_objGRR.Init(m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime,
                                              m_smProductionInfo.g_strOperatorID,
                                              m_smProductionInfo.g_strRecipeID,
                                              m_smCustomizeInfo.g_strMachineID,
                                              m_smVisionInfo.g_strVisionName,
                                              intTotalLeadCount,
                                              Convert.ToInt32(txt_Parts.Text),
                                              Convert.ToInt32(txt_Operators.Text),
                                              Convert.ToInt32(txt_Trials.Text));

            string strTemplateBlobsFeatures;
            string[] strTemplateFeature = new string[100];
            int intLeadNumber;

            int intGroupIndex = 0;
            for (int p = 0; p < m_smVisionInfo.g_arrLead3D.Length; p++)
            {
                if (p != 0)
                    break;

                intLeadNumber = m_smVisionInfo.g_arrLead3D[p].GetBlobsFeaturesNumber();

                for (int i = 0; i < intLeadNumber; i++)
                {
                    strTemplateBlobsFeatures = m_smVisionInfo.g_arrLead3D[p].GetBlobFeaturesInspectRealData(i);
                    strTemplateFeature = strTemplateBlobsFeatures.Split('#');

                    for (int j = 0; j < strTemplateFeature.Length; j++)
                    {
                        if (strTemplateFeature[j] != "")
                            if (Convert.ToSingle(strTemplateFeature[j]) == -1)
                                strTemplateFeature[j] = "0";
                    }

                    m_smVisionInfo.g_objGRR.SetSpecification(0, intGroupIndex, 0f, Convert.ToSingle(strTemplateFeature[0]));
                    m_smVisionInfo.g_objGRR.SetSpecification(1, intGroupIndex, Convert.ToSingle(strTemplateFeature[1]), Convert.ToSingle(strTemplateFeature[2]));
                    m_smVisionInfo.g_objGRR.SetSpecification(2, intGroupIndex, Convert.ToSingle(strTemplateFeature[3]), Convert.ToSingle(strTemplateFeature[4]));
                    m_smVisionInfo.g_objGRR.SetSpecification(3, intGroupIndex, Convert.ToSingle(strTemplateFeature[5]), Convert.ToSingle(strTemplateFeature[6]));
                    m_smVisionInfo.g_objGRR.SetSpecification(4, intGroupIndex, Convert.ToSingle(strTemplateFeature[7]), Convert.ToSingle(strTemplateFeature[8]));
                    m_smVisionInfo.g_objGRR.SetSpecification(5, intGroupIndex, 0f, Convert.ToSingle(strTemplateFeature[9]));
                    m_smVisionInfo.g_objGRR.SetSpecification(6, intGroupIndex, Convert.ToSingle(strTemplateFeature[10]), Convert.ToSingle(strTemplateFeature[11]));
                    m_smVisionInfo.g_objGRR.SetSpecification(7, intGroupIndex, 0f, Convert.ToSingle(strTemplateFeature[12]));
                    //m_smVisionInfo.g_objGRR.SetSpecification(5, intGroupIndex, Convert.ToSingle(strTemplateFeature[9]), Convert.ToSingle(strTemplateFeature[10]));

                    intGroupIndex++;
                }
            }
        }

        public void EnableSetting(bool blnEnable)
        {
            txt_Parts.Enabled = blnEnable;
            txt_Operators.Enabled = blnEnable;
            txt_Trials.Enabled = blnEnable;
            timer_GRR.Enabled = blnEnable;
        }

        private void ResetGRRSetting()
        {
            m_smVisionInfo.g_objGRR.Reset();
            
            lbl_PartsCounter.Text = "0";
            lbl_OperatorsCounter.Text = "0";
            lbl_TrialsCounter.Text = "0";
        }







        private void timer_GRR_Tick(object sender, EventArgs e)
        {
            try
            {
                // Update part counter GUI
                if (m_smVisionInfo.g_objGRR.ref_intPartCount != m_intPartCountPrev)
                {
                    lbl_PartsCounter.Text = Convert.ToString(m_smVisionInfo.g_objGRR.ref_intPartCount);
                    m_intPartCountPrev = m_smVisionInfo.g_objGRR.ref_intPartCount;
                }

                // Update operator counter GUI
                if (m_smVisionInfo.g_objGRR.ref_intOperatorCount != m_intOperatorCountPrev)
                {
                    lbl_OperatorsCounter.Text = Convert.ToString(m_smVisionInfo.g_objGRR.ref_intOperatorCount);
                    m_intOperatorCountPrev = m_smVisionInfo.g_objGRR.ref_intOperatorCount;
                }

                // Update trial counter GUI
                if (m_smVisionInfo.g_objGRR.ref_intTrialCount != m_intTrialPrev)
                {
                    lbl_TrialsCounter.Text = Convert.ToString(m_smVisionInfo.g_objGRR.ref_intTrialCount);
                    m_intTrialPrev = m_smVisionInfo.g_objGRR.ref_intTrialCount;
                }

                //if (m_smVisionInfo.g_objGRR.ref_intGRRMode == 0 && !radioBtn_Static.Checked)
                //    radioBtn_Static.Checked = true;
                //else if (m_smVisionInfo.g_objGRR.ref_intGRRMode == 1 && !radioBtn_Dynamic.Checked)
                //    radioBtn_Dynamic.Checked = true;

                if (radioBtn_Static.Checked)
                {
                    m_smVisionInfo.g_objGRR.ref_intGRRMode = 0;
                }
                else if (radioBtn_Dynamic.Checked)
                {
                    m_smVisionInfo.g_objGRR.ref_intGRRMode = 1;
                }

                //Update Result
                m_strResult = m_smVisionInfo.g_strResult;
                if (m_strResult != m_strResultPrev)
                {
                    switch (m_strResult)
                    {
                        case "Pass":
                            lbl_ResultStatus1.BackColor = Color.Lime;
                            lbl_ResultStatus1.Text = "Pass";
                            break;
                        case "Fail":
                            lbl_ResultStatus1.BackColor = Color.Red;
                            lbl_ResultStatus1.Text = "Fail";
                            break;
                    }

                    m_strResultPrev = m_strResult;
                }

                if (txt_Parts.Text != "0" && txt_Operators.Text != "0" && txt_Trials.Text != "0")
                {
                    // GRR Test finish
                    if ((m_smVisionInfo.g_objGRR.ref_intPartCount > 0 && m_smVisionInfo.g_objGRR.ref_intPartCount >= Convert.ToInt32(txt_Parts.Text)) &&
                        (m_smVisionInfo.g_objGRR.ref_intPartCount > 0 && m_smVisionInfo.g_objGRR.ref_intOperatorCount >= Convert.ToInt32(txt_Operators.Text)) &&
                        (m_smVisionInfo.g_objGRR.ref_intPartCount > 0 && m_smVisionInfo.g_objGRR.ref_intTrialCount >= Convert.ToInt32(txt_Trials.Text)))
                    {
                        timer_GRR.Stop();
                        m_smVisionInfo.g_blnGRRON = false;
                        ResetGRRSetting();

                        if (SRMMessageBox.Show("GRR test finish!", "GRR") == DialogResult.OK)
                        {
                            //m_smVisionInfo.g_blnGRRON = true;
                            //timer_GRR.Start();
                        }

                        //timer_GRR.Start();
                    } 
                }
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("VisionGRRPage -> Timer Exception: " + ex.ToString());
            }
        }

        private void txt_CounterSetting_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_smVisionInfo.g_strVisionName == "MOPkg" || m_smVisionInfo.g_strVisionName == "MarkPkg" || m_smVisionInfo.g_strVisionName == "MOLiPkg")
                InitPkg();
            else if (m_smVisionInfo.g_strVisionName == "InPocketPkg" || m_smVisionInfo.g_strVisionName == "IPMLiPkg")
                InitInPocketPkg();
            else if ((m_smVisionInfo.g_strVisionName == "Li3DPkg") || (m_smVisionInfo.g_strVisionName == "Li3D"))
                InitLead3D();
            else   
            Init();

            //int intTotalPadCount = 0;
            //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //    intTotalPadCount += m_smVisionInfo.g_arrPad[i].GetBlobsFeaturesNumber();

            //m_smVisionInfo.g_objGRR.Init(m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime,
            //                                  m_smProductionInfo.g_strOperatorID,
            //                                  m_smProductionInfo.g_strRecipeID,
            //                                  m_smCustomizeInfo.g_strMachineID,
            //                                  m_smVisionInfo.g_strVisionName,
            //                                  intTotalPadCount,
            //                                  Convert.ToInt32(txt_Parts.Text),
            //                                  Convert.ToInt32(txt_Operators.Text),
            //                                  Convert.ToInt32(txt_Trials.Text));


            //string strTemplateBlobsFeatures;
            //string[] strTemplateFeature = new string[100];
            //int intPadNumber = m_smVisionInfo.g_arrPad[0].GetBlobsFeaturesNumber();

            //for (int i = 0; i < intPadNumber; i++)
            //{
            //    strTemplateBlobsFeatures = m_smVisionInfo.g_arrPad[0].GetBlobFeaturesInspectRealData(i);
            //    strTemplateFeature = strTemplateBlobsFeatures.Split('#');

            //    for (int j = 0; j < strTemplateFeature.Length; j++)
            //    {
            //        if (strTemplateFeature[j] != "")
            //            if (Convert.ToSingle(strTemplateFeature[j]) == -1)
            //                strTemplateFeature[j] = "0";
            //    }

            //    m_smVisionInfo.g_objGRR.SetSpecification(0, i, 0f, Convert.ToSingle(strTemplateFeature[0]));
            //    m_smVisionInfo.g_objGRR.SetSpecification(1, i, Convert.ToSingle(strTemplateFeature[1]), Convert.ToSingle(strTemplateFeature[2]));
            //    m_smVisionInfo.g_objGRR.SetSpecification(2, i, Convert.ToSingle(strTemplateFeature[3]), Convert.ToSingle(strTemplateFeature[4]));
            //    m_smVisionInfo.g_objGRR.SetSpecification(3, i, Convert.ToSingle(strTemplateFeature[5]), Convert.ToSingle(strTemplateFeature[6]));
            //    m_smVisionInfo.g_objGRR.SetSpecification(4, i, Convert.ToSingle(strTemplateFeature[7]), Convert.ToSingle(strTemplateFeature[8]));
            //    m_smVisionInfo.g_objGRR.SetSpecification(5, i, Convert.ToSingle(strTemplateFeature[9]), Convert.ToSingle(strTemplateFeature[10]));
            //}
        }

        private void group_InspectionCounter_Enter(object sender, EventArgs e)
        {

        }

        private void txt_Parts_Leave(object sender, EventArgs e)
        {
            int intValue = 0;

            if (int.TryParse(txt_Parts.Text, out intValue))
            {
                if (intValue < 2)
                    txt_Parts.Text = "3";
            }

        }
    }
}