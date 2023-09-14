using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Common;
using ImageAcquisition;
using VisionProcessing;
using SharedMemory;
using Microsoft.Win32;

namespace VisionProcessForm
{
    public partial class SealToleranceSettingForm : Form
    {

        #region Member Variables
        
        private int m_intUserGroup = 5;
        private bool m_blnInitDone = false;

        private string m_strSelectedRecipe;
        
        private VisionInfo m_smVisionInfo;
        private UserRight m_objUserRight = new UserRight();
        private bool m_blnEnterTextBox = false;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private bool m_blnWantSet1ToAll = false;
        #endregion

        public SealToleranceSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo,  string strSelectedRecipe, int intUserGroup, ProductionInfo smProductioninfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductioninfo;
            m_intUserGroup = intUserGroup;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = visionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            
            DisableField2();
            UpdateGUI();

            m_blnInitDone = true;
        }



        /// <summary>
        /// Customize GUI based on user access level
        /// </summary>
        private void DisableField()
        {
            string strChild1 = "Test Page";
            string strChild2 = "Tolerance Setting Page";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                tab_VisionControl.TabPages.Remove(tabPage_LineWidth);
                
            }
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            //NewUserRight objUserRight = new NewUserRight(false);
            string strChild1 = "Tolerance";
            string strChild2 = "Seal TabPage";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2, m_smVisionInfo.g_intVisionNameNo))
            {
                dgd_Setting.Enabled = false;
                group_SealScoreSetting.Enabled = false;
            }

            strChild2 = "Seal Score TabPage";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2, m_smVisionInfo.g_intVisionNameNo))
            {
                dgd_LineWidth.Enabled = false;
                group_SealScoreSetting.Enabled = false;
            }

            strChild2 = "Seal Mark TabPage";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2, m_smVisionInfo.g_intVisionNameNo))
            {
                group_ScoreSetting2.Enabled = false;

            }

            strChild2 = "Empty Pocket TabPage";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2, m_smVisionInfo.g_intVisionNameNo))
            {
                group_ScoreSetting1.Enabled = false;

            }

        }
        /// <summary>
        /// Customize GUI
        /// </summary>
        private void UpdateGUI()
        {
           
            //txt_MinBrokenArea.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_intHoleMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero).ToString("f3");
            // txt_ShiftTolerance.Text = Math.Round((m_smVisionInfo.g_objSeal.ref_fShiftPositionTolerance / m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_Distance.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_fTemplateWidth[2] / m_smVisionInfo.g_fCalibPixelY, 2, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_FarSealLineWidth.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_fTemplateWidth[0] / m_smVisionInfo.g_fCalibPixelY, 2, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_NearSealLineWidth.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_fTemplateWidth[1] / m_smVisionInfo.g_fCalibPixelY, 2, MidpointRounding.AwayFromZero).ToString("f3");

            //txt_WidthLowerTolerance.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance / m_smVisionInfo.g_fCalibPixelY, 2, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_WidthUpperTolerance.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance / m_smVisionInfo.g_fCalibPixelY, 2, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_DistanceMinTolerance.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_fDistanceMinTolerance / m_smVisionInfo.g_fCalibPixelY, 2, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_DistanceMaxTolerance.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_fDistanceMaxTolerance / m_smVisionInfo.g_fCalibPixelY, 2, MidpointRounding.AwayFromZero).ToString("f3");


            //txt_MaxDistance.Text = (Convert.ToSingle(txt_Distance.Text) + Convert.ToSingle(txt_DistanceMaxTolerance.Text)).ToString();
            //txt_MaxFarLineWidth.Text = (Convert.ToSingle(txt_FarSealLineWidth.Text) + Convert.ToSingle(txt_WidthUpperTolerance.Text)).ToString();
            //txt_MaxNearLineWidth.Text = (Convert.ToSingle(txt_NearSealLineWidth.Text) + Convert.ToSingle(txt_WidthUpperTolerance.Text)).ToString();
            //txt_MinDistance.Text = (Convert.ToSingle(txt_Distance.Text) - Convert.ToSingle(txt_DistanceMinTolerance.Text)).ToString();
            //txt_MinFarLineWidth.Text = (Convert.ToSingle(txt_FarSealLineWidth.Text) - Convert.ToSingle(txt_WidthLowerTolerance.Text)).ToString();
            //txt_MinNearLineWidth.Text = (Convert.ToSingle(txt_NearSealLineWidth.Text) - Convert.ToSingle(txt_WidthLowerTolerance.Text)).ToString();
            //txt_SealScoreTolerance.Text = (m_smVisionInfo.g_objSeal.ref_fSealScoreTolerance * 100).ToString();

            trackBar_PocketScoreTolerance.Value = Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fPocketMinScore * 100);
            txt_PocketScoreTolerance.Text = trackBar_PocketScoreTolerance.Value.ToString();

            trackBar_MarkScoreTolerance.Value = Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fMarkMinScore * 100);
            txt_MarkScoreTolerance.Text = trackBar_MarkScoreTolerance.Value.ToString();

            trackBar_SealScoreTolerance.Value = Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fSealScoreTolerance * 100);
            txt_SealScoreTolerance.Text = trackBar_SealScoreTolerance.Value.ToString();

            UpdateInfo();

            ViewOrHideResultColumn(chk_DisplayResult.Checked);

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
           
            chk_DisplayResult.Checked = Convert.ToBoolean(subkey1.GetValue("CurrentDisplayResult_SealTolerance", false));
            chk_SetToAllOverHeatROI.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllOverHeatROI_SealTolerance", false));
            if (m_smVisionInfo.g_arrSealROIs.Count <= 4 || m_smVisionInfo.g_arrSealROIs[4].Count == 1)
            {
                chk_SetToAllOverHeatROI.Visible = false;
                chk_SetToAllOverHeatROI.Checked = false;
            }
            ViewOrHideResultColumn(chk_DisplayResult.Checked);
        }
        private void dgd_Setting_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            if (!m_blnInitDone)
                return;

            
                int i = e.RowIndex;
                int c = e.ColumnIndex;
                string strRowName = ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString();
                switch (((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString())
                {
                    case "Seal 1 Line Width":
                        {
                            if (c == 1)
                            {
                                float fValue;
                                float fValuePrev = m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance1 / m_smVisionInfo.g_fCalibPixelY;
                                
                                
                                if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue)||fValue.ToString()=="" || (fValue > int.MaxValue))
                                {
                                    dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                   // SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                   
                                        m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance1 = fValue * m_smVisionInfo.g_fCalibPixelY;
                                    
                                }
                            }
                            else if (c == 2)
                            {
                                float fValue;
                                float fValuePrev = m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance1 / m_smVisionInfo.g_fCalibPixelY;
                                if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                                {
                                    dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                    //SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                   
                                        m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance1 = fValue * m_smVisionInfo.g_fCalibPixelY;
                                    
                                }
                            }
                        }
                        break;
                    case "Seal 2 Line Width":
                        {
                            if (c == 1)
                            {
                                float fValue;
                                float fValuePrev = m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance2 / m_smVisionInfo.g_fCalibPixelY;


                                if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                                {
                                    dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                   // SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                  
                                        m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance2 = fValue * m_smVisionInfo.g_fCalibPixelY;
                                    
                                }
                            }
                            else if (c == 2)
                            {
                                float fValue;
                                float fValuePrev = m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance2 / m_smVisionInfo.g_fCalibPixelY;
                                if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                                {
                                    dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                 //   SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                  
                                        m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance2 = fValue * m_smVisionInfo.g_fCalibPixelY;
                                    
                                }
                            }
                        }
                        break;
                    case "Distance":
                        {
                            if (c == 1)
                            {
                                float fValue;
                                float fValuePrev = m_smVisionInfo.g_objSeal.ref_fDistanceMinTolerance / m_smVisionInfo.g_fCalibPixelY;
                                
                                if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                                {
                                    dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                   
                                        m_smVisionInfo.g_objSeal.ref_fDistanceMinTolerance = fValue * m_smVisionInfo.g_fCalibPixelY;
                                    
                                }
                            }
                            else if (c == 2)
                            {
                                float fValue;
                                float fValuePrev = m_smVisionInfo.g_objSeal.ref_fDistanceMaxTolerance / m_smVisionInfo.g_fCalibPixelY;
                                if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                                {
                                    dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                   // SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                    
                                        m_smVisionInfo.g_objSeal.ref_fDistanceMaxTolerance = fValue * m_smVisionInfo.g_fCalibPixelY;
                                 
                                }
                            }
                        }
                        break;
                case "Seal Edge Straightness":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fSealEdgeStraightnessMaxTolerance / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);
                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                // SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fSealEdgeStraightnessMaxTolerance = fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            }
                        }
                    }
                    break;
                case "Sprocket Hole Distance":
                    {
                        if (c == 1)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMinTolerance / m_smVisionInfo.g_fCalibPixelY;

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMinTolerance = fValue * m_smVisionInfo.g_fCalibPixelY;

                            }
                        }
                        else if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMaxTolerance / m_smVisionInfo.g_fCalibPixelY;
                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                // SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMaxTolerance = fValue * m_smVisionInfo.g_fCalibPixelY;

                            }
                        }
                    }
                    break;
                case "Sprocket Hole Diameter":
                    {
                        if (c == 1)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMinTolerance / m_smVisionInfo.g_fCalibPixelY;

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMinTolerance = fValue * m_smVisionInfo.g_fCalibPixelY;

                            }
                        }
                        else if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMaxTolerance / m_smVisionInfo.g_fCalibPixelY;
                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                // SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMaxTolerance = fValue * m_smVisionInfo.g_fCalibPixelY;

                            }
                        }
                    }
                    break;
                case "Sprocket Hole Defect":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fSprocketHoleDefectMaxTolerance / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);
                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                // SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fSprocketHoleDefectMaxTolerance = fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            }
                        }
                    }
                    break;
                case "Sprocket Hole Broken":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fSprocketHoleBrokenMaxTolerance / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);
                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                // SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fSprocketHoleBrokenMaxTolerance = fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            }
                        }
                    }
                    break;
                case "Sprocket Hole Roundness":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fSprocketHoleRoundnessMaxTolerance;
                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || fValue < 0 || fValue > 1)
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                if (fValue < 0 || fValue > 1)
                                    SRMMessageBox.Show("Please enter value between 0 and 1!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fSprocketHoleRoundnessMaxTolerance = fValue;

                            }
                        }
                    }
                    break;
                case "Seal 1 Broken Area / Bubble":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_intHoleMinArea1 / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_intHoleMinArea1 = (int)(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            }
                        }
                       
                    }
                    break;
                case "Seal 2 Broken Area / Bubble":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_intHoleMinArea2 / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_intHoleMinArea2 = (int)(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            }
                        }

                    }
                    break;
                case "Shift Tolerance":
                    {
                        if (c == 1)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fShiftPositionTolerance / m_smVisionInfo.g_fCalibPixelY;

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fShiftPositionTolerance = fValue * m_smVisionInfo.g_fCalibPixelY;

                            }
                        }
                      
                    }
                    break;
                case "Over Heat Size":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(0); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetOverHeatMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;

                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetOverHeatMinArea(0, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(0, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Over Heat Size 2":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(1); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetOverHeatMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetOverHeatMinArea(1, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(1, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Over Heat Size 3":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(2); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetOverHeatMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetOverHeatMinArea(2, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(2, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Over Heat Size 4":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(3); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetOverHeatMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetOverHeatMinArea(3, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(3, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Over Heat Size 5":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(4); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetOverHeatMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetOverHeatMinArea(4, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(4, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Tape Scratches Size":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(0); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetScratchesMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetScratchesMinArea(0, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(0, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Tape Scratches Size 2":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(1); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetScratchesMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetScratchesMinArea(1, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(1, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Tape Scratches Size 3":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(2); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetScratchesMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetScratchesMinArea(2, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(2, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Tape Scratches Size 4":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(3); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetScratchesMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetScratchesMinArea(3, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(3, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Tape Scratches Size 5":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(4); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetScratchesMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetScratchesMinArea(4, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(4, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Unit White Area":
                case "Unit Black Area":
                    {
                        if (c == 1)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fMarkMinWhiteArea;

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fMarkMinWhiteArea = fValue;

                            }
                        }
                        else if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fMarkMaxWhiteArea;

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fMarkMaxWhiteArea = fValue;

                            }
                        }
                    }
                    break;
                case "Seal 1 Broken Gap":
                    if (c == 2)
                    {
                        float fValue;
                        float fValuePrev = m_smVisionInfo.g_objSeal.ref_MinBrokenSealGap / m_smVisionInfo.g_fCalibPixelX;

                        if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                        {
                            dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                            //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {

                            m_smVisionInfo.g_objSeal.ref_MinBrokenSealGap = (fValue * m_smVisionInfo.g_fCalibPixelX);

                        }
                    }
                    break;

                case "Seal 2 Broken Gap":
                    if (c == 2)
                    {
                        float fValue;
                        float fValuePrev = m_smVisionInfo.g_objSeal.ref_MinBrokenSealGap2 / m_smVisionInfo.g_fCalibPixelX;

                        if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                        {
                            dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                            //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {

                            m_smVisionInfo.g_objSeal.ref_MinBrokenSealGap2 = (fValue * m_smVisionInfo.g_fCalibPixelX);

                        }
                    }
                    break;
                default:
                        SRMMessageBox.Show("Cannot find row name [" + strRowName + "].");
                        break;
                
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_Setting_CellClick(object sender, DataGridViewCellEventArgs e)
        {


            if (!m_blnInitDone)
                return;

            int i = e.RowIndex;

            if (i == -1)
                return;


                string strRowName = ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString();
            switch (((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString())
            {
                case "Seal 1 Line Width":
                    m_smVisionInfo.g_blnViewDimension = true;
                    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 0;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case "Seal 2 Line Width":
                    m_smVisionInfo.g_blnViewDimension = true;
                    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 6;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case "Distance":
                    m_smVisionInfo.g_blnViewDimension = true;
                    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 1;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case "Sprocket Hole Distance":
                    m_smVisionInfo.g_blnViewDimension = true;
                    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 8;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case "Sprocket Hole Diameter":
                    m_smVisionInfo.g_blnViewDimension = true;
                    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 10;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case "Sprocket Hole Defect":
                    m_smVisionInfo.g_blnViewDimension = true;
                    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 11;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case "Sprocket Hole Broken":
                    m_smVisionInfo.g_blnViewDimension = true;
                    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 12;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case "Sprocket Hole Roundness":
                    m_smVisionInfo.g_blnViewDimension = true;
                    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 13;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case "Seal 1 Broken Area / Bubble":
                    m_smVisionInfo.g_blnViewDimension = true;
                    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 5;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case "Seal 2 Broken Area / Bubble":
                    m_smVisionInfo.g_blnViewDimension = true;
                    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 7;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case "Over Heat Size":
                case "Over Heat Size 2":
                case "Over Heat Size 3":
                case "Over Heat Size 4":
                case "Over Heat Size 5":
                    m_smVisionInfo.g_blnViewDimension = true;
                    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 4;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case "Seal Edge Straightness":
                    m_smVisionInfo.g_blnViewDimension = true;
                    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 14;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case "Tape Scratches Size":
                case "Tape Scratches Size 2":
                case "Tape Scratches Size 3":
                case "Tape Scratches Size 4":
                case "Tape Scratches Size 5":
                    m_smVisionInfo.g_blnViewDimension = true;
                    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 9;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case "Shift Tolerance":
                    break;
                case "Unit White Area":
                case "Unit Black Area":
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case "Seal 1 Broken Gap":
                    m_smVisionInfo.g_blnViewDimension = true;
                    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 1;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case "Seal 2 Broken Gap":
                    m_smVisionInfo.g_blnViewDimension = true;
                    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 1;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                default:
                    SRMMessageBox.Show("Cannot find row name [" + strRowName + "].");
                    break;
            }
            

           
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_Setting_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            
            m_smVisionInfo.g_blnViewDimension = false;
            m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = -1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void UpdateInfo()
        {
            dgd_Setting.Rows.Clear();
            int i = -1;

            // Display Distance Setting and Result
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x08) > 0)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Distance";
                dgd_Setting.Rows[i].Cells[4].Value = "mm";

                dgd_Setting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fDistanceMinTolerance / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fDistanceMaxTolerance / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
                dgd_Setting.Rows[i].Cells[3].Value = Math.Max(0, Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero));


                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] >= 0 &&
                    ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] < m_smVisionInfo.g_objSeal.ref_fDistanceMinTolerance) ||
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] > m_smVisionInfo.g_objSeal.ref_fDistanceMaxTolerance)))
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
            }

            // Display Sprocket Hole Distance Setting and Result
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x100) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Sprocket Hole Distance";
                dgd_Setting.Rows[i].Cells[4].Value = "mm";

                dgd_Setting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMinTolerance / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMaxTolerance / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
                // 2020 11 27 - CCENG: Sprocket Hole distance allow to have negative value when the caver overlap the sprocket hole.
                //dgd_Setting.Rows[i].Cells[3].Value = Math.Max(0, Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero));
                dgd_Setting.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);


                if (/*m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] >= 0 &&*/
                    ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] < m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMinTolerance) ||
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] > m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMaxTolerance)))
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
            }

            // Display Sprocket Hole Diameter Setting and Result
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x200) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Sprocket Hole Diameter";
                dgd_Setting.Rows[i].Cells[4].Value = "mm";

                dgd_Setting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMinTolerance / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMaxTolerance / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
               
                dgd_Setting.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDiameter] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);


                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDiameter] >= 0 &&
                    ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDiameter] < m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMinTolerance) ||
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDiameter] > m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMaxTolerance)))
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
            }

            // Display Sprocket Hole Defect Setting and Result
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x400) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Sprocket Hole Defect";
                dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fSprocketHoleDefectMaxTolerance / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);

                dgd_Setting.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDefect] / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);
                
                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDefect] >= 0 &&
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDefect] > m_smVisionInfo.g_objSeal.ref_fSprocketHoleDefectMaxTolerance))
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
            }

            // Display Sprocket Hole Broken Setting and Result
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x800) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Sprocket Hole Broken";
                dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fSprocketHoleBrokenMaxTolerance / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);

                dgd_Setting.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleBroken] / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleBroken] >= 0 &&
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleBroken] > m_smVisionInfo.g_objSeal.ref_fSprocketHoleBrokenMaxTolerance))
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
            }

            // Display Sprocket Hole Roundness Setting and Result
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x1000) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Sprocket Hole Roundness";
                dgd_Setting.Rows[i].Cells[4].Value = "";

                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fSprocketHoleRoundnessMaxTolerance, 3, MidpointRounding.AwayFromZero);

                dgd_Setting.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleRoundness], 3, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleRoundness] >= 0 &&
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleRoundness] > m_smVisionInfo.g_objSeal.ref_fSprocketHoleRoundnessMaxTolerance))
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
            }

            // Display Over Heat Setting and Result
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x10) > 0)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Over Heat Size";
                dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                dgd_Setting.Rows[i].Cells[2].Value = m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(0).ToString(); //Math.Round((m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)), 5, MidpointRounding.AwayFromZero).ToString("f4");
                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                if (m_smVisionInfo.g_objSeal.GetOverHeatFailArea(0) == -999)
                {
                    dgd_Setting.Rows[i].Cells[3].Value = "---";
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.GetOverHeatFailArea(0)), 5, MidpointRounding.AwayFromZero);

                    if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }

                for (int j = 1; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                {
                    i++;
                    dgd_Setting.Rows.Add();
                    dgd_Setting.Rows[i].Cells[0].Value = "Over Heat Size " + (j + 1).ToString();
                    dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                    dgd_Setting.Rows[i].Cells[2].Value = m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(j).ToString(); //Math.Round((m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)), 5, MidpointRounding.AwayFromZero).ToString("f4");
                    dgd_Setting.Rows[i].Cells[1].Value = "NA";
                    dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    if (m_smVisionInfo.g_objSeal.GetOverHeatFailArea(j) == -999)
                    {
                        dgd_Setting.Rows[i].Cells[3].Value = "---";
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.GetOverHeatFailArea(j)), 5, MidpointRounding.AwayFromZero);

                        if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                        {
                            dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                    }
                }
            }

            // Display Tape Scratches Setting and Result
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x10) > 0)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Tape Scratches Size";
                dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                dgd_Setting.Rows[i].Cells[2].Value = m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(0).ToString(); //Math.Round((m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)), 5, MidpointRounding.AwayFromZero).ToString("f4");
                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                if (m_smVisionInfo.g_objSeal.GetScratchesFailArea(0) == -999)
                {
                    dgd_Setting.Rows[i].Cells[3].Value = "---";
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.GetScratchesFailArea(0)), 5, MidpointRounding.AwayFromZero);

                    if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }

                for (int j = 1; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                {
                    i++;
                    dgd_Setting.Rows.Add();
                    dgd_Setting.Rows[i].Cells[0].Value = "Tape Scratches Size " + (j + 1).ToString();
                    dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                    dgd_Setting.Rows[i].Cells[2].Value = m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(j).ToString(); //Math.Round((m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)), 5, MidpointRounding.AwayFromZero).ToString("f4");
                    dgd_Setting.Rows[i].Cells[1].Value = "NA";
                    dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    if (m_smVisionInfo.g_objSeal.GetScratchesFailArea(j) == -999)
                    {
                        dgd_Setting.Rows[i].Cells[3].Value = "---";
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.GetScratchesFailArea(j)), 5, MidpointRounding.AwayFromZero);

                        if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                        {
                            dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                    }
                }
            }

            // Display Unit Presence White Area Setting and Result
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x40) > 0)
            {
                if (m_smVisionInfo.g_objSeal.ref_blnWantUsePixelCheckUnitPresent)
                {
                    i++;
                    dgd_Setting.Rows.Add();
                    if (m_smVisionInfo.g_objSeal.ref_blnWhiteOnBlack)
                        dgd_Setting.Rows[i].Cells[0].Value = "Unit White Area";
                    else
                        dgd_Setting.Rows[i].Cells[0].Value = "Unit Black Area";

                    dgd_Setting.Rows[i].Cells[4].Value = "%";

                    dgd_Setting.Rows[i].Cells[1].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_fMarkMinWhiteArea), 5, MidpointRounding.AwayFromZero).ToString("f3");
                    dgd_Setting.Rows[i].Cells[2].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_fMarkMaxWhiteArea), 5, MidpointRounding.AwayFromZero).ToString("f3");
                    if (m_smVisionInfo.g_objSeal.ref_FailUnitPresentWhiteArea == -999)
                    {
                        dgd_Setting.Rows[i].Cells[3].Value = "---";
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailUnitPresentWhiteArea), 5, MidpointRounding.AwayFromZero);

                        if (((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) < (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[1].Value)) ||
                            ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value)))
                        {
                            dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                    }

                }
            }

            // Display Broken Area / Bubble Setting and Result
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x02) > 0)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Seal 1 Broken Area / Bubble";
                dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_intHoleMinArea1 / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 5, MidpointRounding.AwayFromZero).ToString("f3");
                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;

                if (m_smVisionInfo.g_objSeal.ref_FailBubble1 == -999)
                {
                    dgd_Setting.Rows[i].Cells[3].Value = "---";
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Value = m_smVisionInfo.g_objSeal.ref_FailBubble1.ToString("F5");// Math.Round((m_smVisionInfo.g_objSeal.ref_FailBubble1), 5, MidpointRounding.AwayFromZero);

                    if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Seal 2 Broken Area / Bubble";
                dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_intHoleMinArea2 / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 5, MidpointRounding.AwayFromZero).ToString("f3");
                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;

                if (m_smVisionInfo.g_objSeal.ref_FailBubble2 == -999)
                {
                    dgd_Setting.Rows[i].Cells[3].Value = "---";
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Value = m_smVisionInfo.g_objSeal.ref_FailBubble2.ToString("F5");   // Math.Round((m_smVisionInfo.g_objSeal.ref_FailBubble2), 5, MidpointRounding.AwayFromZero);

                    if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }

            }


            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x20) > 0)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Seal 1 Broken Gap";
                dgd_Setting.Rows[i].Cells[4].Value = "mm";

                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_MinBrokenSealGap / (m_smVisionInfo.g_fCalibPixelX), 4, MidpointRounding.AwayFromZero);
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;

                if (m_smVisionInfo.g_objSeal.ref_FailBrokenSeal1 == -999)
                {
                    dgd_Setting.Rows[i].Cells[3].Value = "---";
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailBrokenSeal1), 3, MidpointRounding.AwayFromZero);

                    if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }

                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Seal 2 Broken Gap";
                dgd_Setting.Rows[i].Cells[4].Value = "mm";

                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_MinBrokenSealGap2 / (m_smVisionInfo.g_fCalibPixelX), 4, MidpointRounding.AwayFromZero);
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;

                if (m_smVisionInfo.g_objSeal.ref_FailBrokenSeal2 == -999)
                {
                    dgd_Setting.Rows[i].Cells[3].Value = "---";
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailBrokenSeal2), 3, MidpointRounding.AwayFromZero);

                    if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }
            }

            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x2000) > 0) && m_smVisionInfo.g_objSeal.ref_blnWantCheckSealEdgeStraightness)
            {
                i++;
                dgd_Setting.Rows.Add();
                dgd_Setting.Rows[i].Cells[0].Value = "Seal Edge Straightness";
                dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                dgd_Setting.Rows[i].Cells[2].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_fSealEdgeStraightnessMaxTolerance / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)), 5, MidpointRounding.AwayFromZero).ToString("f4");
                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                if (m_smVisionInfo.g_objSeal.ref_FailSealEdgeStraightnessArea == -999)
                {
                    dgd_Setting.Rows[i].Cells[3].Value = "---";
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailSealEdgeStraightnessArea), 5, MidpointRounding.AwayFromZero);

                    if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }

            }

            // ------------------- Update For TabPage Seal Width and Score --------------------------------------------------------------------------------------------------------
            dgd_LineWidth.Rows.Clear();
            i = 0;
            dgd_LineWidth.Rows.Add();
            dgd_LineWidth.Rows[i].Cells[0].Value = "Seal 1 Min Width";
            dgd_LineWidth.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance1 / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
            if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.TopSealSmallestWidth] == -1)
            {
                dgd_LineWidth.Rows[i].Cells[2].Value = "---";
                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
            }
            else
            {
                dgd_LineWidth.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.TopSealSmallestWidth] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

                if ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.TopSealSmallestWidth] < m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance1))
                {
                    if (m_smVisionInfo.g_objSeal.ref_blnFailSeal1 &&
                        ((m_smVisionInfo.g_objSeal.ref_intSealFailMask & 0x10) > 0))
                    {
                        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                        dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Red;
                        dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Yellow;
                        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Yellow;
                        dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                    }
                }
                else
                {
                    dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                    dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                    dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                    dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                }
            }

            i++;
            dgd_LineWidth.Rows.Add();
            dgd_LineWidth.Rows[i].Cells[0].Value = "Seal 1 Max Width";
            dgd_LineWidth.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance1 / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
            if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.TopSealLargestWidth] == -1)
            {
                dgd_LineWidth.Rows[i].Cells[2].Value = "---";
                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
            }
            else
            {
                dgd_LineWidth.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.TopSealLargestWidth] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.TopSealLargestWidth] > m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance1)
                {
                    if (m_smVisionInfo.g_objSeal.ref_blnFailSeal1 &&
                    ((m_smVisionInfo.g_objSeal.ref_intSealFailMask & 0x10) > 0))
                    {
                        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                        dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Red;
                        dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Yellow;
                        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Yellow;
                        dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                    }
                }
                else
                {
                    dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                    dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                    dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                    dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                }
            }

            i++;
            dgd_LineWidth.Rows.Add();
            dgd_LineWidth.Rows[i].Cells[0].Value = "Seal 2 Min Width";
            dgd_LineWidth.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance2 / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
            if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.BtmSealSmallestWidth] == -1)
            {
                dgd_LineWidth.Rows[i].Cells[2].Value = "---";
                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
            }
            else
            {
                dgd_LineWidth.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.BtmSealSmallestWidth] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.BtmSealSmallestWidth] < m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance2)
                {
                    if (m_smVisionInfo.g_objSeal.ref_blnFailSeal2 &&
                    ((m_smVisionInfo.g_objSeal.ref_intSealFailMask & 0x10) > 0))
                    {
                        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                        dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Red;
                        dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Yellow;
                        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Yellow;
                        dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;

                    }
                }
                else
                {
                    dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                    dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                    dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                    dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                }
            }
            i++;
            dgd_LineWidth.Rows.Add();
            dgd_LineWidth.Rows[i].Cells[0].Value = "Seal 2 Max Width";
            dgd_LineWidth.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance2 / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
            if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.BtmSealLargestWidth] == -1)
            {
                dgd_LineWidth.Rows[i].Cells[2].Value = "---";
                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
            }
            else
            {
                dgd_LineWidth.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.BtmSealLargestWidth] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.BtmSealLargestWidth] > m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance2)
                {
                    if (m_smVisionInfo.g_objSeal.ref_blnFailSeal2 &&
                    ((m_smVisionInfo.g_objSeal.ref_intSealFailMask & 0x10) > 0))
                    {
                        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                        dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Red;
                        dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Yellow;
                        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Yellow;
                        dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                    }
                }
                else
                {
                    dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                    dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                    dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                    dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                }
            }

            float fScore;
            fScore = m_smVisionInfo.g_objSeal.GetPocketMinScore(m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex) * 100;
            if (fScore == -100)
            {
                lbl_Score.Text = "---";
                lbl_Score.BackColor = Color.LightGray;
                lbl_Score.ForeColor = Color.Black;
            }
            else
            {
                lbl_Score.Text = fScore.ToString("f2");
                if (fScore < Convert.ToSingle(txt_PocketScoreTolerance.Text))
                {
                    lbl_Score.BackColor = Color.Red;
                    lbl_Score.ForeColor = Color.Yellow;
                }
                else
                {
                    lbl_Score.BackColor = Color.Lime;
                    lbl_Score.ForeColor = Color.Black;
                }
            }

            fScore = m_smVisionInfo.g_objSeal.GetMarkMinScore(m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex) * 100;
            if (fScore == -100)
            {
                lbl_Score2.Text = "---";
                lbl_Score2.BackColor = Color.LightGray;
                lbl_Score2.ForeColor = Color.Black;
            }
            else
            {
                lbl_Score2.Text = fScore.ToString("f2");
                if (fScore < Convert.ToSingle(txt_MarkScoreTolerance.Text))
                {
                    lbl_Score2.BackColor = Color.Red;
                    lbl_Score2.ForeColor = Color.Yellow;
                }
                else
                {
                    lbl_Score2.BackColor = Color.Lime;
                    lbl_Score2.ForeColor = Color.Black;
                }
            }

            lbl_Score3.Text = (m_smVisionInfo.g_objSeal.ref_fFailSealScore1 * 100).ToString("f2");
            lbl_Score4.Text = (m_smVisionInfo.g_objSeal.ref_fFailSealScore2 * 100).ToString("f2");
            if ((m_smVisionInfo.g_objSeal.ref_fFailSealScore1 * 100) == -100)
            {
                lbl_Score3.Text = "---";
                lbl_Score3.BackColor = Color.LightGray;
                lbl_Score3.ForeColor = Color.Black;
            }
            else
            {
                if ((m_smVisionInfo.g_objSeal.ref_fFailSealScore1 * 100) < Convert.ToSingle(txt_SealScoreTolerance.Text))
                {
                    lbl_Score3.BackColor = Color.Red;
                    lbl_Score3.ForeColor = Color.Yellow;
                }
                else
                {
                    lbl_Score3.BackColor = Color.Lime;
                    lbl_Score3.ForeColor = Color.Black;
                }
            }

            if ((m_smVisionInfo.g_objSeal.ref_fFailSealScore2 * 100) == -100)
            {
                lbl_Score4.Text = "---";
                lbl_Score4.BackColor = Color.LightGray;
                lbl_Score4.ForeColor = Color.Black;
            }
            else
            {
                if ((m_smVisionInfo.g_objSeal.ref_fFailSealScore2 * 100) < Convert.ToSingle(txt_SealScoreTolerance.Text))
                {
                    lbl_Score4.BackColor = Color.Red;
                    lbl_Score4.ForeColor = Color.Yellow;
                }
                else
                {
                    lbl_Score4.BackColor = Color.Lime;
                    lbl_Score4.ForeColor = Color.Black;
                }
            }
        }
        private void UpdateSettingTable()
        {
            int i = -1;

            // Display Distance Setting and Result
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x08) > 0)
            {
                i++;
                dgd_Setting.Rows[i].Cells[0].Value = "Distance";
                dgd_Setting.Rows[i].Cells[4].Value = "mm";

                dgd_Setting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fDistanceMinTolerance / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fDistanceMaxTolerance / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
                dgd_Setting.Rows[i].Cells[3].Value = Math.Max(0, Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero));


                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] >= 0 &&
                    ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] < m_smVisionInfo.g_objSeal.ref_fDistanceMinTolerance) ||
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] > m_smVisionInfo.g_objSeal.ref_fDistanceMaxTolerance)))
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
            }

            // Display Sprocket Hole Distance Setting and Result
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x100) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole)
            {
                i++;
                dgd_Setting.Rows[i].Cells[0].Value = "Sprocket Hole Distance";
                dgd_Setting.Rows[i].Cells[4].Value = "mm";

                dgd_Setting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMinTolerance / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMaxTolerance / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
                // 2020 11 27 - CCENG: Sprocket Hole distance allow to have negative value when the caver overlap the sprocket hole.
                //dgd_Setting.Rows[i].Cells[3].Value = Math.Max(0, Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero));
                dgd_Setting.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);


                if (/*m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] >= 0 &&*/
                    ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] < m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMinTolerance) ||
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] > m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMaxTolerance)))
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
            }

            // Display Sprocket Hole Diameter Setting and Result
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x200) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
            {
                i++;
                dgd_Setting.Rows[i].Cells[0].Value = "Sprocket Hole Diameter";
                dgd_Setting.Rows[i].Cells[4].Value = "mm";

                dgd_Setting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMinTolerance / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMaxTolerance / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

                dgd_Setting.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDiameter] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);


                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDiameter] >= 0 &&
                    ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDiameter] < m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMinTolerance) ||
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDiameter] > m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMaxTolerance)))
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
            }

            // Display Sprocket Hole Defect Setting and Result
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x400) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
            {
                i++;
                dgd_Setting.Rows[i].Cells[0].Value = "Sprocket Hole Defect";
                dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fSprocketHoleDefectMaxTolerance / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);

                dgd_Setting.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDefect] / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDefect] >= 0 &&
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDefect] > m_smVisionInfo.g_objSeal.ref_fSprocketHoleDefectMaxTolerance))
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
            }

            // Display Sprocket Hole Broken Setting and Result
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x800) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
            {
                i++;
                dgd_Setting.Rows[i].Cells[0].Value = "Sprocket Hole Broken";
                dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fSprocketHoleBrokenMaxTolerance / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);

                dgd_Setting.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleBroken] / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleBroken] >= 0 &&
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleBroken] > m_smVisionInfo.g_objSeal.ref_fSprocketHoleBrokenMaxTolerance))
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
            }

            // Display Sprocket Hole Roundness Setting and Result
            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x1000) > 0) && !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
            {
                i++;
                dgd_Setting.Rows[i].Cells[0].Value = "Sprocket Hole Roundness";
                dgd_Setting.Rows[i].Cells[4].Value = "";

                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fSprocketHoleRoundnessMaxTolerance, 3, MidpointRounding.AwayFromZero);

                dgd_Setting.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleRoundness], 3, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleRoundness] >= 0 &&
                    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleRoundness] > m_smVisionInfo.g_objSeal.ref_fSprocketHoleRoundnessMaxTolerance))
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
            }

            // Display Over Heat Setting and Result
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x10) > 0)
            {
                i++;
                dgd_Setting.Rows[i].Cells[0].Value = "Over Heat Size";
                dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                dgd_Setting.Rows[i].Cells[2].Value = m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(0).ToString(); //Math.Round((m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)), 5, MidpointRounding.AwayFromZero).ToString("f4");
                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                if (m_smVisionInfo.g_objSeal.GetOverHeatFailArea(0) == -999)
                {
                    dgd_Setting.Rows[i].Cells[3].Value = "---";
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.GetOverHeatFailArea(0)), 5, MidpointRounding.AwayFromZero);

                    if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }

                for (int j = 1; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                {
                    i++;
                    dgd_Setting.Rows[i].Cells[0].Value = "Over Heat Size " + (j + 1).ToString();
                    dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                    dgd_Setting.Rows[i].Cells[2].Value = m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(j).ToString(); //Math.Round((m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)), 5, MidpointRounding.AwayFromZero).ToString("f4");
                    dgd_Setting.Rows[i].Cells[1].Value = "NA";
                    dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    if (m_smVisionInfo.g_objSeal.GetOverHeatFailArea(j) == -999)
                    {
                        dgd_Setting.Rows[i].Cells[3].Value = "---";
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.GetOverHeatFailArea(j)), 5, MidpointRounding.AwayFromZero);

                        if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                        {
                            dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                    }
                }
            }

            // Display Tape Scratches Setting and Result
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x10) > 0)
            {
                i++;
                dgd_Setting.Rows[i].Cells[0].Value = "Tape Scratches Size";
                dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                dgd_Setting.Rows[i].Cells[2].Value = m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(0).ToString(); //Math.Round((m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)), 5, MidpointRounding.AwayFromZero).ToString("f4");
                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                if (m_smVisionInfo.g_objSeal.GetScratchesFailArea(0) == -999)
                {
                    dgd_Setting.Rows[i].Cells[3].Value = "---";
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.GetScratchesFailArea(0)), 5, MidpointRounding.AwayFromZero);

                    if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }

                for (int j = 1; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                {
                    i++;
                    dgd_Setting.Rows[i].Cells[0].Value = "Tape Scratches Size " + (j + 1).ToString();
                    dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                    dgd_Setting.Rows[i].Cells[2].Value = m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(j).ToString(); //Math.Round((m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)), 5, MidpointRounding.AwayFromZero).ToString("f4");
                    dgd_Setting.Rows[i].Cells[1].Value = "NA";
                    dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    if (m_smVisionInfo.g_objSeal.GetScratchesFailArea(j) == -999)
                    {
                        dgd_Setting.Rows[i].Cells[3].Value = "---";
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.GetScratchesFailArea(j)), 5, MidpointRounding.AwayFromZero);

                        if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                        {
                            dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                    }
                }
            }

            // Display Unit Presence White Area Setting and Result
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x40) > 0)
            {
                if (m_smVisionInfo.g_objSeal.ref_blnWantUsePixelCheckUnitPresent)
                {
                    i++;
                    if (m_smVisionInfo.g_objSeal.ref_blnWhiteOnBlack)
                        dgd_Setting.Rows[i].Cells[0].Value = "Unit White Area";
                    else
                        dgd_Setting.Rows[i].Cells[0].Value = "Unit Black Area";

                    dgd_Setting.Rows[i].Cells[4].Value = "%";

                    dgd_Setting.Rows[i].Cells[1].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_fMarkMinWhiteArea), 5, MidpointRounding.AwayFromZero).ToString("f3");
                    dgd_Setting.Rows[i].Cells[2].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_fMarkMaxWhiteArea), 5, MidpointRounding.AwayFromZero).ToString("f3");
                    if (m_smVisionInfo.g_objSeal.ref_FailUnitPresentWhiteArea == -999)
                    {
                        dgd_Setting.Rows[i].Cells[3].Value = "---";
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailUnitPresentWhiteArea), 5, MidpointRounding.AwayFromZero);

                        if (((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) < (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[1].Value)) ||
                            ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value)))
                        {
                            dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                    }

                }
            }

            // Display Broken Area / Bubble Setting and Result
            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x02) > 0)
            {
                i++;
                dgd_Setting.Rows[i].Cells[0].Value = "Seal 1 Broken Area / Bubble";
                dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_intHoleMinArea1 / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 5, MidpointRounding.AwayFromZero).ToString("f3");
                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;

                if (m_smVisionInfo.g_objSeal.ref_FailBubble1 == -999)
                {
                    dgd_Setting.Rows[i].Cells[3].Value = "---";
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Value = m_smVisionInfo.g_objSeal.ref_FailBubble1.ToString("F5");// Math.Round((m_smVisionInfo.g_objSeal.ref_FailBubble1), 5, MidpointRounding.AwayFromZero);

                    if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }
                i++;
                dgd_Setting.Rows[i].Cells[0].Value = "Seal 2 Broken Area / Bubble";
                dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_intHoleMinArea2 / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 5, MidpointRounding.AwayFromZero).ToString("f3");
                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;

                if (m_smVisionInfo.g_objSeal.ref_FailBubble2 == -999)
                {
                    dgd_Setting.Rows[i].Cells[3].Value = "---";
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Value = m_smVisionInfo.g_objSeal.ref_FailBubble2.ToString("F5");   // Math.Round((m_smVisionInfo.g_objSeal.ref_FailBubble2), 5, MidpointRounding.AwayFromZero);

                    if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }

            }


            if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x20) > 0)
            {
                i++;
                dgd_Setting.Rows[i].Cells[0].Value = "Seal 1 Broken Gap";
                dgd_Setting.Rows[i].Cells[4].Value = "mm";

                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_MinBrokenSealGap / (m_smVisionInfo.g_fCalibPixelX), 4, MidpointRounding.AwayFromZero);
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;

                if (m_smVisionInfo.g_objSeal.ref_FailBrokenSeal1 == -999)
                {
                    dgd_Setting.Rows[i].Cells[3].Value = "---";
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailBrokenSeal1), 3, MidpointRounding.AwayFromZero);

                    if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }

                i++;
                dgd_Setting.Rows[i].Cells[0].Value = "Seal 2 Broken Gap";
                dgd_Setting.Rows[i].Cells[4].Value = "mm";

                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_MinBrokenSealGap2 / (m_smVisionInfo.g_fCalibPixelX), 4, MidpointRounding.AwayFromZero);
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;

                if (m_smVisionInfo.g_objSeal.ref_FailBrokenSeal2 == -999)
                {
                    dgd_Setting.Rows[i].Cells[3].Value = "---";
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailBrokenSeal2), 3, MidpointRounding.AwayFromZero);

                    if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }
            }

            if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x2000) > 0) && m_smVisionInfo.g_objSeal.ref_blnWantCheckSealEdgeStraightness)
            {
                i++;
                dgd_Setting.Rows[i].Cells[0].Value = "Seal Edge Straightness";
                dgd_Setting.Rows[i].Cells[4].Value = "mm^2";

                dgd_Setting.Rows[i].Cells[2].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_fSealEdgeStraightnessMaxTolerance / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)), 5, MidpointRounding.AwayFromZero).ToString("f4");
                dgd_Setting.Rows[i].Cells[1].Value = "NA";
                dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
                dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                if (m_smVisionInfo.g_objSeal.ref_FailSealEdgeStraightnessArea == -999)
                {
                    dgd_Setting.Rows[i].Cells[3].Value = "---";
                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailSealEdgeStraightnessArea), 5, MidpointRounding.AwayFromZero);

                    if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                }

            }
        }
        /// <summary>
        /// Update inspection result on GUI
        /// </summary>
        //private void UpdateInfo()
        //{
        //    dgd_Setting.Rows.Clear();
        //    int i = 0;
        //    for (int e = 0; d < 12; d++)
        //    {
        //        switch (d)
        //        {
        //            case 0:
        //                //if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x01) > 0)
        //                //{
        //                //    dgd_Setting.Rows.Add();
        //                //    dgd_Setting.Rows[i].Cells[0].Value = "Seal 1 Line Width";
        //                //}
        //                //else
        //                    continue;
        //                //break;
        //            case 1:
        //                //if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x01) > 0)
        //                //{
        //                //    dgd_Setting.Rows.Add();
        //                //    dgd_Setting.Rows[i].Cells[0].Value = "Seal 2 Line Width";
        //                //}
        //                //else
        //                    continue;
        //                //break;
        //            case 2:
        //                if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x08) > 0)
        //                {
        //                    dgd_Setting.Rows.Add();
        //                    dgd_Setting.Rows[i].Cells[0].Value = "Distance";
        //                    dgd_Setting.Rows[i].Cells[4].Value = "mm";
        //                }
        //                else
        //                    continue;
        //                break;
        //            case 3:
        //                if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x100) > 0)
        //                {
        //                    dgd_Setting.Rows.Add();
        //                    dgd_Setting.Rows[i].Cells[0].Value = "Sprocket Hole Distance";
        //                    dgd_Setting.Rows[i].Cells[4].Value = "mm";
        //                }
        //                else
        //                    continue;
        //                break;
        //            case 4:
        //                if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x04) > 0)
        //                {
        //                    dgd_Setting.Rows.Add();
        //                    dgd_Setting.Rows[i].Cells[0].Value = "Shift Tolerance";
        //                    dgd_Setting.Rows[i].Visible = false;
        //                }
        //                else
        //                    continue;
        //                break;
        //            case 5:
        //                if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x10) > 0)
        //                {
        //                    dgd_Setting.Rows.Add();
        //                    dgd_Setting.Rows[i].Cells[0].Value = "Over Heat Size";
        //                    dgd_Setting.Rows[i].Cells[4].Value = "mm^2";
        //                }
        //                else
        //                    continue;
        //                break;
        //            case 6:
        //                if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x10) > 0)
        //                {
        //                    dgd_Setting.Rows.Add();
        //                    dgd_Setting.Rows[i].Cells[0].Value = "Tape Scratches Size";
        //                    dgd_Setting.Rows[i].Cells[4].Value = "mm^2";
        //                }
        //                else
        //                    continue;
        //                break;
        //            case 7:
        //                if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x40) > 0)
        //                {
        //                    if (m_smVisionInfo.g_objSeal.ref_blnWantUsePixelCheckUnitPresent)
        //                    {
        //                        dgd_Setting.Rows.Add();
        //                        if(m_smVisionInfo.g_objSeal.ref_blnWhiteOnBlack)
        //                            dgd_Setting.Rows[i].Cells[0].Value = "Unit White Area";
        //                        else
        //                            dgd_Setting.Rows[i].Cells[0].Value = "Unit Black Area";

        //                        dgd_Setting.Rows[i].Cells[4].Value = "pix";
        //                    }
        //                    else
        //                        continue;
        //                }
        //                else
        //                    continue;
        //                break;
        //            case 8:
        //                if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x02) > 0)
        //                {
        //                    dgd_Setting.Rows.Add();
        //                    dgd_Setting.Rows[i].Cells[0].Value = "Seal 1 Broken Area / Bubble";
        //                    dgd_Setting.Rows[i].Cells[4].Value = "mm^2";
        //                }
        //                else
        //                    continue;
        //                break;
        //            case 9:
        //                if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x02) > 0)
        //                {
        //                    dgd_Setting.Rows.Add();
        //                    dgd_Setting.Rows[i].Cells[0].Value = "Seal 2 Broken Area / Bubble";
        //                    dgd_Setting.Rows[i].Cells[4].Value = "mm^2";
        //                }
        //                else
        //                    continue;
        //                break;
        //            case 10:
        //                if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x20) > 0)
        //                {
        //                    dgd_Setting.Rows.Add();
        //                    dgd_Setting.Rows[i].Cells[0].Value = "Seal 1 Broken Gap";
        //                    dgd_Setting.Rows[i].Cells[4].Value = "mm";
        //                }
        //                else
        //                    continue;
        //                break;
        //            case 11:
        //                if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x20) > 0)
        //                {
        //                    dgd_Setting.Rows.Add();
        //                    dgd_Setting.Rows[i].Cells[0].Value = "Seal 2 Broken Gap";
        //                    dgd_Setting.Rows[i].Cells[4].Value = "mm";
        //                }
        //                else
        //                    continue;
        //                break;
        //            default:
        //                continue;
        //        }

        //        // dgd_Setting.Rows[i].Cells[3].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[i] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        //        if (d == 0)
        //        {
        //            //dgd_Setting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance1 / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        //            //dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance1 / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        //            //dgd_Setting.Rows[i].Cells[3].Value = Math.Max(0, Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[d] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero));

        //            //if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[d] >= 0 &&
        //            //    ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[d] < m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance1) ||
        //            //    (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[d] > m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance1)))
        //            //{
        //            //    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
        //            //    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
        //            //    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
        //            //    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
        //            //}
        //            //else
        //            //{
        //            //    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //            //    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //            //    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //            //    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //            //}
        //        }
        //        else if (d == 1)
        //        {
        //            //dgd_Setting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance2 / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        //            //dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance2 / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        //            //dgd_Setting.Rows[i].Cells[3].Value = Math.Max(0, Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[d] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero));

        //            //if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[d] >= 0 &&
        //            //    ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[d] < m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance2) ||
        //            //(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[d] > m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance2)))
        //            //{
        //            //    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
        //            //    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
        //            //    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
        //            //    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
        //            //}
        //            //else
        //            //{
        //            //    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //            //    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //            //    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //            //    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //            //}
        //        }
        //        else if (d == 2)
        //        {
        //            dgd_Setting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fDistanceMinTolerance / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        //            dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fDistanceMaxTolerance / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        //            dgd_Setting.Rows[i].Cells[3].Value = Math.Max(0, Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero));


        //            if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] >= 0 &&
        //                ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] < m_smVisionInfo.g_objSeal.ref_fDistanceMinTolerance) ||
        //                (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.Distance] > m_smVisionInfo.g_objSeal.ref_fDistanceMaxTolerance)))
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
        //                dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
        //            }
        //            else
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //            }
        //        }
        //        else if (d == 3)
        //        {
        //            dgd_Setting.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMinTolerance / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        //            dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMaxTolerance / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        //            dgd_Setting.Rows[i].Cells[3].Value = Math.Max(0, Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero));


        //            if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] >= 0 &&
        //                ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] < m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMinTolerance) ||
        //                (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.SprocketHoleDistance] > m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMaxTolerance)))
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
        //                dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
        //            }
        //            else
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //            }
        //        }
        //        else if (d == 4)
        //        {
        //            dgd_Setting.Rows[i].Cells[2].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_fShiftPositionTolerance / m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero).ToString("f3");
        //            dgd_Setting.Rows[i].Cells[1].Value = "NA";
        //            dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
        //            dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
        //            dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
        //            dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailPosition), 3, MidpointRounding.AwayFromZero);

        //            if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
        //                dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
        //            }
        //            else
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //            }
        //        }
        //        else if (d == 5)
        //        {
        //            dgd_Setting.Rows[i].Cells[2].Value = m_smVisionInfo.g_objSeal.ref_fOverHeatAreaMinTolerance.ToString(); //Math.Round((m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)), 5, MidpointRounding.AwayFromZero).ToString("f4");
        //            dgd_Setting.Rows[i].Cells[1].Value = "NA";
        //            dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
        //            dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
        //            dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
        //            if (m_smVisionInfo.g_objSeal.ref_FailOverheatArea == -999)
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Value = "---";
        //                dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //            }
        //            else
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailOverheatArea), 5, MidpointRounding.AwayFromZero);

        //                if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
        //                {
        //                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
        //                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
        //                }
        //                else
        //                {
        //                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //                }
        //            }
        //        }
        //        else if (d == 6)
        //        {
        //            dgd_Setting.Rows[i].Cells[2].Value = m_smVisionInfo.g_objSeal.ref_fScratchesAreaMinTolerance.ToString(); //Math.Round((m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY)), 5, MidpointRounding.AwayFromZero).ToString("f4");
        //            dgd_Setting.Rows[i].Cells[1].Value = "NA";
        //            dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
        //            dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
        //            dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
        //            if (m_smVisionInfo.g_objSeal.ref_FailScratchesArea == -999)
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Value = "---";
        //                dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //            }
        //            else
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailScratchesArea), 5, MidpointRounding.AwayFromZero);

        //                if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
        //                {
        //                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
        //                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
        //                }
        //                else
        //                {
        //                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //                }
        //            }
        //        }
        //        else if (d == 7)
        //        {
        //            dgd_Setting.Rows[i].Cells[1].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_fMarkMinWhiteArea), 5, MidpointRounding.AwayFromZero).ToString("f3");
        //            dgd_Setting.Rows[i].Cells[2].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_fMarkMaxWhiteArea), 5, MidpointRounding.AwayFromZero).ToString("f3");
        //            if (m_smVisionInfo.g_objSeal.ref_FailUnitPresentWhiteArea == -999)
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Value = "---";
        //                dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //            }
        //            else
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailUnitPresentWhiteArea), 5, MidpointRounding.AwayFromZero);

        //                if (((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) < (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[1].Value)) ||
        //                    ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value)))
        //                {
        //                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
        //                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
        //                }
        //                else
        //                {
        //                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //                }
        //            }
        //        }
        //        else if (d == 8)
        //        {
        //            dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_intHoleMinArea1 / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 5, MidpointRounding.AwayFromZero).ToString("f3");
        //            dgd_Setting.Rows[i].Cells[1].Value = "NA";
        //            dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
        //            dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
        //            dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;

        //            if (m_smVisionInfo.g_objSeal.ref_FailBubble1 == -999)
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Value = "---";
        //                dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //            }
        //            else
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Value = m_smVisionInfo.g_objSeal.ref_FailBubble1.ToString("F5");// Math.Round((m_smVisionInfo.g_objSeal.ref_FailBubble1), 5, MidpointRounding.AwayFromZero);

        //                if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
        //                {
        //                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
        //                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
        //                }
        //                else
        //                {
        //                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //                }
        //            }
        //        }
        //        else if (d == 9)
        //        {
        //            dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_intHoleMinArea2 / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 5, MidpointRounding.AwayFromZero).ToString("f3");
        //            dgd_Setting.Rows[i].Cells[1].Value = "NA";
        //            dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
        //            dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
        //            dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;

        //            if (m_smVisionInfo.g_objSeal.ref_FailBubble2 == -999)
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Value = "---";
        //                dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //            }
        //            else
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Value = m_smVisionInfo.g_objSeal.ref_FailBubble2.ToString("F5");   // Math.Round((m_smVisionInfo.g_objSeal.ref_FailBubble2), 5, MidpointRounding.AwayFromZero);

        //                if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
        //                {
        //                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
        //                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
        //                }
        //                else
        //                {
        //                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //                }
        //            }
        //        }
        //        else if (d == 10)
        //        {
        //            dgd_Setting.Rows[i].Cells[1].Value = "NA";
        //            dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_MinBrokenSealGap / (m_smVisionInfo.g_fCalibPixelX), 4, MidpointRounding.AwayFromZero);
        //            dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
        //            dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
        //            dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;

        //            if (m_smVisionInfo.g_objSeal.ref_FailBrokenSeal1 == -999)
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Value = "---";
        //                dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //            }
        //            else
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailBrokenSeal1), 3, MidpointRounding.AwayFromZero);

        //                if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
        //                {
        //                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
        //                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
        //                }
        //                else
        //                {
        //                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //                }
        //            }
        //        }
        //        else if (d == 11)
        //        {
        //            dgd_Setting.Rows[i].Cells[1].Value = "NA";
        //            dgd_Setting.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_MinBrokenSealGap2 / (m_smVisionInfo.g_fCalibPixelX), 4, MidpointRounding.AwayFromZero);
        //            dgd_Setting.Rows[i].Cells[1].ReadOnly = true;
        //            dgd_Setting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
        //            dgd_Setting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;

        //            if (m_smVisionInfo.g_objSeal.ref_FailBrokenSeal2 == -999)
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Value = "---";
        //                dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //            }
        //            else
        //            {
        //                dgd_Setting.Rows[i].Cells[3].Value = Math.Round((m_smVisionInfo.g_objSeal.ref_FailBrokenSeal2), 3, MidpointRounding.AwayFromZero);

        //                if ((float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[3].Value) > (float)Convert.ToSingle(dgd_Setting.Rows[i].Cells[2].Value))
        //                {
        //                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Red;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
        //                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
        //                }
        //                else
        //                {
        //                    dgd_Setting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //                    dgd_Setting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //                    dgd_Setting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //                }
        //            }
        //        }

        //        i++;
        //    }

        //    // ------------------- Update For TabPage Seal Width and Score --------------------------------------------------------------------------------------------------------
        //    dgd_LineWidth.Rows.Clear();
        //    i = 0;
        //    dgd_LineWidth.Rows.Add();
        //    dgd_LineWidth.Rows[i].Cells[0].Value = "Seal 1 Min Width";
        //    dgd_LineWidth.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance1 / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        //    if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.TopSealSmallestWidth] == -1)
        //    {
        //        dgd_LineWidth.Rows[i].Cells[2].Value = "---";
        //        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
        //        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
        //        dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
        //        dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
        //    }
        //    else
        //    {
        //        dgd_LineWidth.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.TopSealSmallestWidth] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

        //        if ((m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.TopSealSmallestWidth] < m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance1))
        //        {
        //            if (m_smVisionInfo.g_objSeal.ref_blnFailSeal1 &&
        //                ((m_smVisionInfo.g_objSeal.ref_intSealFailMask & 0x10) > 0))
        //            {
        //                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Red;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
        //            }
        //            else
        //            {
        //                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Yellow;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Yellow;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
        //            }
        //        }
        //        else
        //        {
        //            dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
        //            dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
        //            dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
        //            dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
        //        }
        //    }

        //    i++;
        //    dgd_LineWidth.Rows.Add();
        //    dgd_LineWidth.Rows[i].Cells[0].Value = "Seal 1 Max Width";
        //    dgd_LineWidth.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance1 / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        //    if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.TopSealLargestWidth] == -1)
        //    {
        //        dgd_LineWidth.Rows[i].Cells[2].Value = "---";
        //        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
        //        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
        //        dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
        //        dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
        //    }
        //    else
        //    {
        //        dgd_LineWidth.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.TopSealLargestWidth] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

        //        if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.TopSealLargestWidth] > m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance1)
        //        {
        //            if (m_smVisionInfo.g_objSeal.ref_blnFailSeal1 &&
        //            ((m_smVisionInfo.g_objSeal.ref_intSealFailMask & 0x10) > 0))
        //            {
        //                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Red;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
        //            }
        //            else
        //            {
        //                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Yellow;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Yellow;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
        //            }
        //        }
        //        else
        //        {
        //            dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
        //            dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
        //            dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
        //            dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
        //        }
        //    }

        //    i++;
        //    dgd_LineWidth.Rows.Add();
        //    dgd_LineWidth.Rows[i].Cells[0].Value = "Seal 2 Min Width";
        //    dgd_LineWidth.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance2 / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        //    if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.BtmSealSmallestWidth] == -1)
        //    {
        //        dgd_LineWidth.Rows[i].Cells[2].Value = "---";
        //        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
        //        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
        //        dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
        //        dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
        //    }
        //    else
        //    {
        //        dgd_LineWidth.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.BtmSealSmallestWidth] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

        //        if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.BtmSealSmallestWidth] < m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance2)
        //        {
        //            if (m_smVisionInfo.g_objSeal.ref_blnFailSeal2 &&
        //            ((m_smVisionInfo.g_objSeal.ref_intSealFailMask & 0x10) > 0))
        //            {
        //                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Red;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
        //            }
        //            else
        //            {
        //                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Yellow;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Yellow;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;

        //            }
        //        }
        //        else
        //        {
        //            dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
        //            dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
        //            dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
        //            dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
        //        }
        //    }
        //    i++;
        //    dgd_LineWidth.Rows.Add();
        //    dgd_LineWidth.Rows[i].Cells[0].Value = "Seal 2 Max Width";
        //    dgd_LineWidth.Rows[i].Cells[1].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance2 / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);
        //    if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.BtmSealLargestWidth] == -1)
        //    {
        //        dgd_LineWidth.Rows[i].Cells[2].Value = "---";
        //        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
        //        dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
        //        dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
        //        dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
        //    }
        //    else
        //    {
        //        dgd_LineWidth.Rows[i].Cells[2].Value = Math.Round(m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.BtmSealLargestWidth] / m_smVisionInfo.g_fCalibPixelY, 3, MidpointRounding.AwayFromZero);

        //        if (m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[(int)SealBlog.ResultType.BtmSealLargestWidth] > m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance2)
        //        {
        //            if (m_smVisionInfo.g_objSeal.ref_blnFailSeal2 &&
        //            ((m_smVisionInfo.g_objSeal.ref_intSealFailMask & 0x10) > 0))
        //            {
        //                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Red;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
        //            }
        //            else
        //            {
        //                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Yellow;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Yellow;
        //                dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
        //            }
        //        }
        //        else
        //        {
        //            dgd_LineWidth.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
        //            dgd_LineWidth.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
        //            dgd_LineWidth.Rows[i].Cells[2].Style.BackColor = Color.Lime;
        //            dgd_LineWidth.Rows[i].Cells[2].Style.ForeColor = Color.Black;
        //        }
        //    }

        //    float fScore;
        //    fScore = m_smVisionInfo.g_objSeal.GetPocketMinScore(m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex) * 100;
        //    if (fScore == -100)
        //    {
        //        lbl_Score.Text = "---";
        //        lbl_Score.BackColor = Color.LightGray;
        //        lbl_Score.ForeColor = Color.Black;
        //    }
        //    else
        //    {
        //        lbl_Score.Text = fScore.ToString("f2");
        //        if (fScore < Convert.ToSingle(txt_PocketScoreTolerance.Text))
        //        {
        //            lbl_Score.BackColor = Color.Red;
        //            lbl_Score.ForeColor = Color.Yellow;
        //        }
        //        else
        //        {
        //            lbl_Score.BackColor = Color.Lime;
        //            lbl_Score.ForeColor = Color.Black;
        //        }
        //    }

        //    fScore = m_smVisionInfo.g_objSeal.GetMarkMinScore(m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex) * 100;
        //    if (fScore == -100)
        //    {
        //        lbl_Score2.Text = "---";
        //        lbl_Score2.BackColor = Color.LightGray;
        //        lbl_Score2.ForeColor = Color.Black;
        //    }
        //    else
        //    {
        //        lbl_Score2.Text = fScore.ToString("f2");
        //        if (fScore < Convert.ToSingle(txt_MarkScoreTolerance.Text))
        //        {
        //            lbl_Score2.BackColor = Color.Red;
        //            lbl_Score2.ForeColor = Color.Yellow;
        //        }
        //        else
        //        {
        //            lbl_Score2.BackColor = Color.Lime;
        //            lbl_Score2.ForeColor = Color.Black;
        //        }
        //    }

        //    lbl_Score3.Text = (m_smVisionInfo.g_objSeal.ref_fFailSealScore1 * 100).ToString("f2");
        //    lbl_Score4.Text = (m_smVisionInfo.g_objSeal.ref_fFailSealScore2 * 100).ToString("f2");
        //    if ((m_smVisionInfo.g_objSeal.ref_fFailSealScore1 * 100) == -100)
        //    {
        //        lbl_Score3.Text = "---";
        //        lbl_Score3.BackColor = Color.LightGray;
        //        lbl_Score3.ForeColor = Color.Black;
        //    }
        //    else
        //    {
        //        if ((m_smVisionInfo.g_objSeal.ref_fFailSealScore1 * 100) < Convert.ToSingle(txt_SealScoreTolerance.Text))
        //        {
        //            lbl_Score3.BackColor = Color.Red;
        //            lbl_Score3.ForeColor = Color.Yellow;
        //        }
        //        else
        //        {
        //            lbl_Score3.BackColor = Color.Lime;
        //            lbl_Score3.ForeColor = Color.Black;
        //        }
        //    }

        //    if ((m_smVisionInfo.g_objSeal.ref_fFailSealScore2 * 100) == -100)
        //    {
        //        lbl_Score4.Text = "---";
        //        lbl_Score4.BackColor = Color.LightGray;
        //        lbl_Score4.ForeColor = Color.Black;
        //    }
        //    else
        //    {
        //        if ((m_smVisionInfo.g_objSeal.ref_fFailSealScore2 * 100) < Convert.ToSingle(txt_SealScoreTolerance.Text))
        //        {
        //            lbl_Score4.BackColor = Color.Red;
        //            lbl_Score4.ForeColor = Color.Yellow;
        //        }
        //        else
        //        {
        //            lbl_Score4.BackColor = Color.Lime;
        //            lbl_Score4.ForeColor = Color.Black;
        //        }
        //    }
        //}

        private void chk_DisplayResult_Click(object sender, EventArgs e)
        {
            ViewOrHideResultColumn(chk_DisplayResult.Checked);

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("CurrentDisplayResult_SealTolerance", chk_DisplayResult.Checked);
        }

        private void SaveGeneralSetting(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "General.xml", false);
            
            STDeviceEdit.CopySettingFile(strFolderPath, "General.xml");
            objFile.WriteSectionElement("TemplateCounting");
            objFile.WriteElement1Value("TotalPocketTemplates", m_smVisionInfo.g_intPocketTemplateTotal);
            objFile.WriteElement1Value("TotalMarkTemplates", m_smVisionInfo.g_intMarkTemplateTotal);
            objFile.WriteElement1Value("PocketTemplateMask", m_smVisionInfo.g_intPocketTemplateMask);
            objFile.WriteElement1Value("MarkTemplateMask", m_smVisionInfo.g_intMarkTemplateMask);
          //  objFile.WriteElement1Value("PocketTemplateIndex", m_smVisionInfo.g_intPocketTemplateIndex);
          //  objFile.WriteElement1Value("MarkTemplateIndex", m_smVisionInfo.g_intMarkTemplateIndex);
            objFile.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Seal Tolerance Setting", m_smProductionInfo.g_strLotID);
            
        }

        private void LoadGeneralSetting(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "General.xml", false);
            objFile.GetFirstSection("TemplateCounting");
            m_smVisionInfo.g_intPocketTemplateTotal = objFile.GetValueAsInt("TotalPocketTemplates", 0, 1);
            m_smVisionInfo.g_intMarkTemplateTotal = objFile.GetValueAsInt("TotalMarkTemplates", 0, 1);
            m_smVisionInfo.g_intPocketTemplateMask = objFile.GetValueAsInt("PocketTemplateMask", 0, 1);
            m_smVisionInfo.g_intMarkTemplateMask = objFile.GetValueAsInt("MarkTemplateMask", 0, 1);
           // m_smVisionInfo.g_intPocketTemplateIndex = objFile.GetValueAsInt("PocketTemplateIndex", 0, 1);
           // m_smVisionInfo.g_intMarkTemplateIndex = objFile.GetValueAsInt("MarkTemplateIndex", 0, 1);
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\";

            m_smVisionInfo.g_objSeal.LoadSeal(strPath + "Seal\\Settings.xml", "Settings", m_smVisionInfo.g_fCalibPixelX);
            LoadGeneralSetting(strPath);
            Close();
            Dispose();
        }
        
        private void btn_Save_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\";

            
            STDeviceEdit.CopySettingFile(strPath, "Seal\\Settings.xml");
            m_smVisionInfo.g_objSeal.SaveSeal(strPath + "Seal\\Settings.xml", false, "Settings", false, m_smVisionInfo.g_fCalibPixelX);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Seal Tolerance Setting", m_smProductionInfo.g_strLotID);
            
            SaveGeneralSetting(strPath);

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            Close();
            Dispose();
        }

        private void tab_VisionControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tab_VisionControl.SelectedTab.Name)
            {
                case "tabPage_LineWidth":
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    tabPage_LineWidth.Controls.Add(chk_DisplayResult);
                    break;
                case "tabPage_PackageScoreSetting":
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    tabPage_PackageScoreSetting.Controls.Add(chk_DisplayResult);
                    break;
                case "tabPage_MarkScoreSetting":
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    tabPage_MarkScoreSetting.Controls.Add(chk_DisplayResult);
                    break;
                case "tabPage_SealScoreSetting":
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    tabPage_SealScoreSetting.Controls.Add(chk_DisplayResult);
                    break;
                default:
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = true;
                    break;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

     


        private void SealToleranceSettingForm_Load(object sender, EventArgs e)
        {
            //Cursor.Current = Cursors.Default;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            //m_smVisionInfo.g_blnViewROI = true;
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void SealToleranceSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal Tolerance Setting Form Closed", "Exit Seal Tolerance Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_objSeal.ClearBlobData();
            m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewROI = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
        }

        //private void txt_WidthUpperTolerance_TextChanged(object sender, EventArgs e)
        //{
        //    if (!m_blnInitDone || txt_WidthUpperTolerance.Text == "" || txt_WidthUpperTolerance.Text == null)
        //        return;

        //    m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance = float.Parse(txt_WidthUpperTolerance.Text) * m_smVisionInfo.g_fCalibPixelY;
        //    txt_MaxFarLineWidth.Text = (Convert.ToSingle(txt_FarSealLineWidth.Text) + Convert.ToSingle(txt_WidthUpperTolerance.Text)).ToString();
        //    txt_MaxNearLineWidth.Text = (Convert.ToSingle(txt_NearSealLineWidth.Text) + Convert.ToSingle(txt_WidthUpperTolerance.Text)).ToString();

        //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        //}

        //private void txt_WidthLowerTolerance_TextChanged(object sender, EventArgs e)
        //{
        //    if (!m_blnInitDone || txt_WidthLowerTolerance.Text == "" || txt_WidthLowerTolerance.Text == null)
        //        return;

        //    m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance = float.Parse(txt_WidthLowerTolerance.Text) * m_smVisionInfo.g_fCalibPixelY;
        //    txt_MinFarLineWidth.Text = (Convert.ToSingle(txt_FarSealLineWidth.Text) - Convert.ToSingle(txt_WidthLowerTolerance.Text)).ToString();
        //    txt_MinNearLineWidth.Text = (Convert.ToSingle(txt_NearSealLineWidth.Text) - Convert.ToSingle(txt_WidthLowerTolerance.Text)).ToString();

        //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        //}

        //private void txt_WidthUpperTolerance_Enter(object sender, EventArgs e)
        //{
        //    m_smVisionInfo.g_blnViewDimension = true;
        //    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 0;
        //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        //}

        //private void txt_WidthUpperTolerance_Leave(object sender, EventArgs e)
        //{
        //    m_smVisionInfo.g_blnViewDimension = false;
        //    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = -1;
        //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        //}

        //private void txt_DistanceMaxTolerance_Enter(object sender, EventArgs e)
        //{
        //    m_smVisionInfo.g_blnViewDimension = true;
        //    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = 1;
        //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        //}

        //private void txt_DistanceMaxTolerance_Leave(object sender, EventArgs e)
        //{
        //    m_smVisionInfo.g_blnViewDimension = false;
        //    m_smVisionInfo.g_objSeal.ref_intSelectedSealObject = -1;
        //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        //}

        //private void txt_DistanceMinTolerance_TextChanged(object sender, EventArgs e)
        //{
        //    if (!m_blnInitDone || txt_DistanceMinTolerance.Text == "" || txt_DistanceMinTolerance.Text == null)
        //        return;

        //    m_smVisionInfo.g_objSeal.ref_fDistanceMinTolerance = float.Parse(txt_DistanceMinTolerance.Text) * m_smVisionInfo.g_fCalibPixelY;

        //    txt_MinDistance.Text = (Convert.ToSingle(txt_Distance.Text) - Convert.ToSingle(txt_DistanceMinTolerance.Text)).ToString();

        //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        //}

        //private void txt_DistanceMaxTolerance_TextChanged(object sender, EventArgs e)
        //{
        //    if (!m_blnInitDone || txt_DistanceMaxTolerance.Text == "" || txt_DistanceMaxTolerance.Text == null)
        //        return;

        //    m_smVisionInfo.g_objSeal.ref_fDistanceMaxTolerance = float.Parse(txt_DistanceMaxTolerance.Text) * m_smVisionInfo.g_fCalibPixelY;

        //    txt_MaxDistance.Text = (Convert.ToSingle(txt_Distance.Text) + Convert.ToSingle(txt_DistanceMaxTolerance.Text)).ToString();

        //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        //}



        private void trackBar_ScoreTolerance_Scroll(object sender, EventArgs e)
        {
            txt_PocketScoreTolerance.Text = trackBar_PocketScoreTolerance.Value.ToString();
            m_smVisionInfo.g_objSeal.ref_fPocketMinScore = (trackBar_PocketScoreTolerance.Value / 100.0f);
        }

        private void txt_ScoreTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnEnterTextBox)
                return;

            trackBar_PocketScoreTolerance.Value = Convert.ToInt32(txt_PocketScoreTolerance.Text);
            m_smVisionInfo.g_objSeal.ref_fPocketMinScore = (trackBar_PocketScoreTolerance.Value / 100.0f);
            
        }

        private void txt_ScoreTolerance_Enter(object sender, EventArgs e)
        {
            m_blnEnterTextBox = true;
        }

        private void txt_ScoreTolerance_Leave(object sender, EventArgs e)
        {
            m_blnEnterTextBox = false;
        }


        private void txt_MarkScoreTolerance_Enter(object sender, EventArgs e)
        {
            m_blnEnterTextBox = true;
        }

        private void txt_MarkScoreTolerance_Leave(object sender, EventArgs e)
        {
            m_blnEnterTextBox = false;
        }

        private void trackBar_MarkScoreTolerance_Scroll(object sender, EventArgs e)
        {
            txt_MarkScoreTolerance.Text = trackBar_MarkScoreTolerance.Value.ToString();
            m_smVisionInfo.g_objSeal.ref_fMarkMinScore = (trackBar_MarkScoreTolerance.Value / 100.0f);
        }

        private void txt_MarkScoreTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnEnterTextBox)
                return;

            trackBar_MarkScoreTolerance.Value = Convert.ToInt32(txt_MarkScoreTolerance.Text);
            m_smVisionInfo.g_objSeal.ref_fMarkMinScore = (trackBar_MarkScoreTolerance.Value / 100.0f);
        }

        private void txt_SealScoreTolerance_Enter(object sender, EventArgs e)
        {
            m_blnEnterTextBox = true;
        }
        private void txt_SealScoreTolerance_Leave(object sender, EventArgs e)
        {
            m_blnEnterTextBox = false;
        }
        private void txt_SealScoreTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnEnterTextBox)
                return;
            trackBar_SealScoreTolerance.Value = Convert.ToInt32(txt_SealScoreTolerance.Text) ;
            m_smVisionInfo.g_objSeal.ref_fSealScoreTolerance = (trackBar_SealScoreTolerance.Value/100.0f);
        }
        private void trackBar_SealScoreTolerance_Scroll(object sender, EventArgs e)
        {
            txt_SealScoreTolerance.Text = trackBar_SealScoreTolerance.Value.ToString();
            m_smVisionInfo.g_objSeal.ref_fSealScoreTolerance = (trackBar_SealScoreTolerance.Value / 100.0f);
        }
        private void ViewOrHideResultColumn(bool blnWantDisplay)
        {
            dgd_Setting.Columns[3].Visible = blnWantDisplay;
            dgd_LineWidth.Columns[2].Visible = blnWantDisplay;
            lbl_ScoreTitle.Visible = blnWantDisplay;
            lbl_Score.Visible = blnWantDisplay;
            srmLabel26.Visible = blnWantDisplay;
            lbl_ScoreTitle2.Visible = blnWantDisplay;
            lbl_Score2.Visible = blnWantDisplay;
            lbl_ScorePercent2.Visible = blnWantDisplay;
            lbl_ScoreTitle3.Visible = blnWantDisplay;
            lbl_Score3.Visible = blnWantDisplay;
            lbl_ScorePercent3.Visible = blnWantDisplay;
            lbl_ScoreTitle4.Visible = blnWantDisplay;
            lbl_Score4.Visible = blnWantDisplay;
            lbl_ScorePercent4.Visible = blnWantDisplay;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_smProductionInfo.AT_ALL_InAuto && m_smProductionInfo.g_intAutoLogOutMinutes > 0 && m_intUserGroup != 5)
            {
                DateTime t = DateTime.Now;
                TimeSpan tSpan = t - m_smProductionInfo.g_DTStartAutoMode_IndividualForm;

                if (tSpan.Minutes >= m_smProductionInfo.g_intAutoLogOutMinutes)
                {
                    m_smProductionInfo.g_intUserGroup = 5;
                    m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
                    DisableField2();
                }
            }
        }

        private void dgd_Setting_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;


            int i = e.RowIndex;
            int c = e.ColumnIndex;
            string strRowName = ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString();
            switch (((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString())
            {
                case "Seal 1 Line Width":
                    {
                        if (c == 1)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance1 / m_smVisionInfo.g_fCalibPixelY;


                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                // SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance1 = fValue * m_smVisionInfo.g_fCalibPixelY;

                            }
                        }
                        else if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance1 / m_smVisionInfo.g_fCalibPixelY;
                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                //SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance1 = fValue * m_smVisionInfo.g_fCalibPixelY;

                            }
                        }
                    }
                    break;
                case "Seal 2 Line Width":
                    {
                        if (c == 1)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance2 / m_smVisionInfo.g_fCalibPixelY;


                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                // SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance2 = fValue * m_smVisionInfo.g_fCalibPixelY;

                            }
                        }
                        else if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance2 / m_smVisionInfo.g_fCalibPixelY;
                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                //   SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance2 = fValue * m_smVisionInfo.g_fCalibPixelY;

                            }
                        }
                    }
                    break;
                case "Distance":
                    {
                        if (c == 1)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fDistanceMinTolerance / m_smVisionInfo.g_fCalibPixelY;

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fDistanceMinTolerance = fValue * m_smVisionInfo.g_fCalibPixelY;

                            }
                        }
                        else if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fDistanceMaxTolerance / m_smVisionInfo.g_fCalibPixelY;
                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                // SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fDistanceMaxTolerance = fValue * m_smVisionInfo.g_fCalibPixelY;

                            }
                        }
                    }
                    break;
                case "Seal Edge Straightness":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fSealEdgeStraightnessMaxTolerance / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);
                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                // SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fSealEdgeStraightnessMaxTolerance = fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            }
                        }
                    }
                    break;
                case "Sprocket Hole Distance":
                    {
                        if (c == 1)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMinTolerance / m_smVisionInfo.g_fCalibPixelY;

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMinTolerance = fValue * m_smVisionInfo.g_fCalibPixelY;

                            }
                        }
                        else if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMaxTolerance / m_smVisionInfo.g_fCalibPixelY;
                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                // SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fSprocketHoleDistanceMaxTolerance = fValue * m_smVisionInfo.g_fCalibPixelY;

                            }
                        }
                    }
                    break;
                case "Sprocket Hole Diameter":
                    {
                        if (c == 1)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMinTolerance / m_smVisionInfo.g_fCalibPixelY;

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMinTolerance = fValue * m_smVisionInfo.g_fCalibPixelY;

                            }
                        }
                        else if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMaxTolerance / m_smVisionInfo.g_fCalibPixelY;
                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                // SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fSprocketHoleDiameterMaxTolerance = fValue * m_smVisionInfo.g_fCalibPixelY;

                            }
                        }
                    }
                    break;
                case "Sprocket Hole Defect":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fSprocketHoleDefectMaxTolerance / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);
                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                // SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fSprocketHoleDefectMaxTolerance = fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            }
                        }
                    }
                    break;
                case "Sprocket Hole Broken":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fSprocketHoleBrokenMaxTolerance / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);
                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                // SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fSprocketHoleBrokenMaxTolerance = fValue * (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            }
                        }
                    }
                    break;
                case "Sprocket Hole Roundness":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fSprocketHoleRoundnessMaxTolerance;
                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || fValue < 0 || fValue > 1)
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                if (fValue < 0 || fValue > 1)
                                    SRMMessageBox.Show("Please enter value between 0 and 1!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fSprocketHoleRoundnessMaxTolerance = fValue;

                            }
                        }
                    }
                    break;
                case "Seal 1 Broken Area / Bubble":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_intHoleMinArea1 / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_intHoleMinArea1 = (int)(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            }
                        }

                    }
                    break;
                case "Seal 2 Broken Area / Bubble":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_intHoleMinArea2 / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_intHoleMinArea2 = (int)(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            }
                        }

                    }
                    break;
                case "Shift Tolerance":
                    {
                        if (c == 1)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fShiftPositionTolerance / m_smVisionInfo.g_fCalibPixelY;

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fShiftPositionTolerance = fValue * m_smVisionInfo.g_fCalibPixelY;

                            }
                        }

                    }
                    break;
                case "Over Heat Size":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(0); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetOverHeatMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetOverHeatMinArea(0, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(0, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Over Heat Size 2":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(1); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetOverHeatMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetOverHeatMinArea(1, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(1, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Over Heat Size 3":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(2); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetOverHeatMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetOverHeatMinArea(2, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(2, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Over Heat Size 4":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(3); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetOverHeatMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetOverHeatMinArea(3, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(3, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Over Heat Size 5":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(4); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetOverHeatMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetOverHeatMinArea(4, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(4, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Tape Scratches Size":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(0); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetScratchesMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetScratchesMinArea(0, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(0, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Tape Scratches Size 2":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(1); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetScratchesMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetScratchesMinArea(1, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(1, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Tape Scratches Size 3":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(2); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetScratchesMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetScratchesMinArea(2, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(2, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Tape Scratches Size 4":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(3); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetScratchesMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetScratchesMinArea(3, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(3, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Tape Scratches Size 5":
                    {
                        if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(4); //m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY);

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (chk_SetToAllOverHeatROI.Checked)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                                    {
                                        m_smVisionInfo.g_objSeal.SetScratchesMinArea(j, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                        m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(j, fValue);
                                    }

                                    m_blnInitDone = false;
                                    UpdateSettingTable();
                                    m_blnInitDone = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_objSeal.SetScratchesMinArea(4, (int)Math.Floor(fValue * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY));
                                    m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(4, fValue);
                                }
                            }
                        }

                    }
                    break;
                case "Unit White Area":
                case "Unit Black Area":
                    {
                        if (c == 1)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fMarkMinWhiteArea;

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fMarkMinWhiteArea = fValue;

                            }
                        }
                        else if (c == 2)
                        {
                            float fValue;
                            float fValuePrev = m_smVisionInfo.g_objSeal.ref_fMarkMaxWhiteArea;

                            if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                            {
                                dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                                //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {

                                m_smVisionInfo.g_objSeal.ref_fMarkMaxWhiteArea = fValue;

                            }
                        }
                    }
                    break;
                case "Seal 1 Broken Gap":
                    if (c == 2)
                    {
                        float fValue;
                        float fValuePrev = m_smVisionInfo.g_objSeal.ref_MinBrokenSealGap / m_smVisionInfo.g_fCalibPixelX;

                        if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                        {
                            dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                            //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            m_smVisionInfo.g_objSeal.ref_MinBrokenSealGap = (fValue * m_smVisionInfo.g_fCalibPixelX);

                        }
                    }
                    break;
                case "Seal 2 Broken Gap":
                    if (c == 2)
                    {
                        float fValue;
                        float fValuePrev = m_smVisionInfo.g_objSeal.ref_MinBrokenSealGap2 / m_smVisionInfo.g_fCalibPixelX;

                        if (!float.TryParse(Convert.ToString(dgd_Setting.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                        {
                            dgd_Setting.Rows[i].Cells[c].Value = fValuePrev.ToString("F5");
                            //    SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {

                            m_smVisionInfo.g_objSeal.ref_MinBrokenSealGap2 = (fValue * m_smVisionInfo.g_fCalibPixelX);

                        }
                    }
                    break;

                default:
                    SRMMessageBox.Show("Cannot find row name [" + strRowName + "].");
                    break;

            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_LineWidth_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (e.ColumnIndex != 1)
                return;

            int i = e.RowIndex;
            int c = e.ColumnIndex;
            string strRowName = ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString();
            switch (((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString())
            {
                case "Seal 1 Min Width":
                    {
                        float fValue;
                        float fValuePrev = m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance1 / m_smVisionInfo.g_fCalibPixelY;


                        if (!float.TryParse(Convert.ToString(dgd_LineWidth.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                        {
                            dgd_LineWidth.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                        }
                        else
                        {
                            m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance1 = fValue * m_smVisionInfo.g_fCalibPixelY;
                        }
                    }
                    break;
                case "Seal 1 Max Width":
                    {
                        float fValue;
                        float fValuePrev = m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance1 / m_smVisionInfo.g_fCalibPixelY;


                        if (!float.TryParse(Convert.ToString(dgd_LineWidth.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                        {
                            dgd_LineWidth.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                        }
                        else
                        {
                            m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance1 = fValue * m_smVisionInfo.g_fCalibPixelY;
                        }
                    }
                    break;
                case "Seal 2 Min Width":
                    {
                        float fValue;
                        float fValuePrev = m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance2 / m_smVisionInfo.g_fCalibPixelY;


                        if (!float.TryParse(Convert.ToString(dgd_LineWidth.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                        {
                            dgd_LineWidth.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                        }
                        else
                        {
                            m_smVisionInfo.g_objSeal.ref_fWidthLowerTolerance2 = fValue * m_smVisionInfo.g_fCalibPixelY;
                        }
                    }
                    break;
                case "Seal 2 Max Width":
                    {
                        float fValue;
                        float fValuePrev = m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance2 / m_smVisionInfo.g_fCalibPixelY;


                        if (!float.TryParse(Convert.ToString(dgd_LineWidth.Rows[i].Cells[c].Value), out fValue) || fValue.ToString() == "" || (fValue > int.MaxValue))
                        {
                            dgd_LineWidth.Rows[i].Cells[c].Value = fValuePrev.ToString("F3");
                        }
                        else
                        {
                            m_smVisionInfo.g_objSeal.ref_fWidthUpperTolerance2 = fValue * m_smVisionInfo.g_fCalibPixelY;
                        }
                    }
                    break;
            }
        }

        private void dgd_LineWidth_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgd_LineWidth_CellLeave(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgd_LineWidth_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgd_LineWidth_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void chk_SetToAllOverHeatROI_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllOverHeatROI_SealTolerance", chk_SetToAllOverHeatROI.Checked);
        }
    }
}
