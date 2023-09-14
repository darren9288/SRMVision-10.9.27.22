using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Common;
using SharedMemory;
using User;
using VisionProcessing;

namespace VisionModule
{
    public partial class Vision4Page : Form
    {
        #region Member Variables

        private int m_intMachineStatusPrev = 1;

        // Count
        private int m_intPassCount = 0, m_intPassCountPrev = -1;
        private int m_intFailCount = 0, m_intFailCountPrev = -1;
        private int m_intTestedTotal = 0, m_intTestedTotalPrev = -1;
        private int m_intMarkFailCount = 0, m_intMarkFailCountPrev = -1;
        private int m_intLeadFailCount = 0, m_intLeadFailCountPrev = -1;
        private int m_intOrientFailCount = 0, m_intOrientFailCountPrev = -1;
        private int m_intPin1FailCount = 0, m_intPin1FailCountPrev = -1;
        private int m_intPackageFailCount = 0, m_intPackageFailCountPrev = -1;
        private int m_intEmptyUnitCount = 0, m_intEmptyUnitCountPrev = -1;
        private int m_intNoTemplateFailCount = 0, m_intNoTemplateFailCountPrev = -1;
        private int m_intPositionFailCount = 0, m_intPositionFailCountPrev = -1;
        private int m_intEdgeNotFoundFailCount = 0, m_intEdgeNotFoundFailCountPrev = -1;
        private int m_intPackageDefectFailCount = 0, m_intPackageDefectFailCountPrev = -1;
        private int m_intPackageColorDefectFailCount = 0, m_intPackageColorDefectFailCountPrev = -1;
        private string m_strResult = "", m_strResultPrev = "";
        private string m_strResult2 = "", m_strResult2Prev = "";

        private UserRight m_objUserRight = new UserRight();
        private CustomOption m_smCustomOption;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo m_smVisionInfo;

        #endregion

        public Vision4Page(CustomOption smCustomOption, ProductionInfo smProductionInfo, VisionInfo smVisionInfo)
        {
            InitializeComponent();

            m_smCustomOption = smCustomOption;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;

            //lbl_LotID.Text = m_smProductionInfo.g_strLotID;
            lbl_LotID.Text = m_smProductionInfo.g_arrSingleLotID[m_smVisionInfo.g_intVisionIndex];
            lbl_OperatorID.Text = m_smProductionInfo.g_strOperatorID;
            UpdateInfo();
            ResetCounterBackColor();
            CustomizeGUI();
            LoadTemplateImage();
            DisableField();
            
            if (m_smVisionInfo.g_intForceZero == 0)
                chk_ForceXtozero.Checked = false;
            else
                chk_ForceXtozero.Checked = true;

            if (m_smVisionInfo.g_intForceYZero == 0)
                chk_ForceYtozero.Checked = false;
            else
                chk_ForceYtozero.Checked = true;


            txt_UB.Text = m_smVisionInfo.g_intTemporaryValue.ToString();
            txt_Delay.Text = m_smVisionInfo.g_intTemporaryValue2.ToString();
        }
        private void ResetCounterBackColor()
        {
            lbl_MarkFailCount.BackColor = Color.White;
            lbl_LeadFailCount.BackColor = Color.White;
            lbl_OrientFailCount.BackColor = Color.White;
            lbl_Pin1FailCount.BackColor = Color.White;
            lbl_EmptyFailCount.BackColor = Color.White;
            lbl_PackageFailCount.BackColor = Color.White;
            lbl_PositionFailCount.BackColor = Color.White;
            lbl_NoTemplateFailCount.BackColor = Color.White;
            lbl_PackageDefectFailCount.BackColor = Color.White;
            lbl_PackageColorDefectFailCount.BackColor = Color.White;
            lbl_EdgeNotFoundFailCount.BackColor = Color.White;
            lbl_MarkFailPercent.BackColor = Color.White;
            lbl_LeadFailPercent.BackColor = Color.White;
            lbl_OrientFailPercent.BackColor = Color.White;
            lbl_Pin1FailPercent.BackColor = Color.White;
            lbl_EmptyFailPercent.BackColor = Color.White;
            lbl_PackageFailPercent.BackColor = Color.White;
            lbl_PositionFailPercent.BackColor = Color.White;
            lbl_NoTemplateFailPercent.BackColor = Color.White;
            lbl_PackageDefectFailPercent.BackColor = Color.White;
            lbl_PackageColorDefectFailPercent.BackColor = Color.White;
            lbl_EdgeNotFoundFailPercent.BackColor = Color.White;
        }

        /// <summary>
        /// Make sure that only when form is focused, then enable timer.
        /// By this way, CPU usage can be reduced.
        /// </summary>
        public void ActivateTimer(bool blnEnable)
        {
            timer_Live.Enabled = blnEnable;
        }

        public void CustomizeGUI()
        {
            //if (m_smVisionInfo.g_intUnitsOnImage == 1)
            //{
            //    lbl_ResultStatus1.Size = new Size(129, 45);
            //    lbl_ResultStatus1.Location = new Point(181, 29);

            //    lbl_Unit1.Visible = false;
            //    lbl_ResultStatus2.Visible = false;
            //    lbl_Unit2.Visible = false;
            //}

            //int intPositionY = lbl_Detail.Location.Y + lbl_Detail.Size.Height - 1;

            //if ((m_smCustomOption.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            //{
            //    lbl_Orient.Visible = false;
            //    lbl_OrientFailCount.Visible = false;
            //}
            //else
            //{
            //    intPositionY += 17;
            //}

            //if (!m_smVisionInfo.g_blnWantPin1)
            //{
            //    lbl_Pin1.Visible = false;
            //    lbl_Pin1FailCount.Visible = false;
            //}
            //else
            //{
            //    lbl_Pin1.Visible = true;
            //    lbl_Pin1FailCount.Visible = true;
            //    lbl_Pin1.Location = new Point(lbl_Orient.Location.X, intPositionY);
            //    lbl_Pin1FailCount.Location = new Point(lbl_OrientFailCount.Location.X, intPositionY);
            //    intPositionY += 17;
            //}

            //if ((m_smCustomOption.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            //{
            //    lbl_Mark.Visible = false;
            //    lbl_MarkFailCount.Visible = false;
            //}
            //else
            //{
            //    lbl_Mark.Location = new Point(lbl_Orient.Location.X, intPositionY);
            //    lbl_MarkFailCount.Location = new Point(lbl_OrientFailCount.Location.X, intPositionY);
            //    intPositionY += 17;
            //}

            //if ((m_smCustomOption.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            //{
            //    lbl_Package.Visible = false;
            //    lbl_PackageFailCount.Visible = false;
            //    lbl_Empty.Visible = false;
            //    lbl_EmptyFailCount.Visible = false;
            //}
            //else
            //{
            //    if (!m_smVisionInfo.g_blnViewColorImage)
            //    {
            //        lbl_Empty.Visible = false;
            //        lbl_EmptyFailCount.Visible = false;
            //    }

            //    lbl_Package.Location = new Point(lbl_Package.Location.X, intPositionY);
            //    lbl_PackageFailCount.Location = new Point(lbl_PackageFailCount.Location.X, intPositionY);
            //    intPositionY += 17;
            //    lbl_Empty.Location = new Point(lbl_Empty.Location.X, intPositionY);
            //    lbl_EmptyFailCount.Location = new Point(lbl_EmptyFailCount.Location.X, intPositionY);
            //    intPositionY += 17;

            //    if (m_smVisionInfo.g_blnViewColorImage)
            //    {
            //        lbl_Orient.Visible = lbl_OrientFailCount.Visible = true;
            //        lbl_Orient.Location = new Point(lbl_Orient.Location.X, intPositionY);
            //        lbl_OrientFailCount.Location = new Point(lbl_OrientFailCount.Location.X, intPositionY);
            //        intPositionY += 17;
            //    }
            //}

            //if ((m_smCustomOption.g_intWantMark & (0x01 << m_smVisionInfo.g_intVisionPos)) == 0)
            //{
            //    lbl_Mark.Visible = false;
            //    lbl_MarkFailCount.Visible = false;
            //}

            //if ((m_smCustomOption.g_intWantOrient & (0x01 << m_smVisionInfo.g_intVisionPos)) == 0)
            //{
            //    lbl_Orient.Visible = false;
            //    lbl_OrientFailCount.Visible = false;
            //}

            //if ((m_smCustomOption.g_intWantPackage & (0x01 << m_smVisionInfo.g_intVisionPos)) == 0)
            //{
            //    lbl_Package.Visible = false;
            //    lbl_PackageFailCount.Visible = false;
            //}

            if (m_smVisionInfo.g_intUnitsOnImage == 1)
            {
                lbl_ResultStatus1.Size = new Size(129, 45);
                lbl_ResultStatus1.Location = new Point(181, 29);

                lbl_Unit1.Visible = false;
                lbl_ResultStatus2.Visible = false;
                lbl_Unit2.Visible = false;
            }

            //2021-10-27 ZJYEOH : Orient also hide for InPocket Vision
            //if ((m_smCustomOption.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                pnl_Orient.Visible = false;
            }

            if ((m_smCustomOption.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                pnl_Mark.Visible = false;
            }

            if ((m_smCustomOption.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                pnl_Lead.Visible = false;
            }

            if ((m_smCustomOption.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                pnl_Package.Visible = false;
                pnl_PackageDefect.Visible = false;
                pnl_PackageColorDefect.Visible = false;

                //if (!m_smVisionInfo.g_blnViewColorImage)
                {
                    pnl_EmptyUnit.Visible = false;
                }
            }
            else
            {
                if (m_smVisionInfo.g_arrPackage.Count > m_smVisionInfo.g_intSelectedUnit && (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x1000) > 0)
                {
                    pnl_Package.Visible = true;
                }
                else
                {
                    if (m_smVisionInfo.g_intPackageFailureTotal == 0) //if (m_intPackageFailCount == 0)
                    {
                        pnl_Package.Visible = false;
                    }
                }

                if (m_smVisionInfo.g_arrPackage.Count > m_smVisionInfo.g_intSelectedUnit && m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantInspectPackage())
                {
                    pnl_PackageDefect.Visible = true;
                }
                else
                {
                    if (m_smVisionInfo.g_intPkgDefectFailureTotal == 0) //if (m_intPackageDefectFailCount == 0)
                    {
                        pnl_PackageDefect.Visible = false;
                    }
                }

                if ((m_smCustomOption.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                {
                    pnl_PackageColorDefect.Visible = false;
                }
                else
                {
                    if (m_smVisionInfo.g_arrPackage.Count > m_smVisionInfo.g_intSelectedUnit && m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask > 0)
                    {
                        pnl_PackageColorDefect.Visible = true;
                    }
                    else
                    {
                        if (m_smVisionInfo.g_intPkgColorDefectFailureTotal == 0) //if (m_intPackageColorDefectFailCount == 0)
                        {
                            pnl_PackageColorDefect.Visible = false;
                        }
                    }
                }

                //if (!m_smVisionInfo.g_blnViewColorImage)
                {
                    pnl_EmptyUnit.Visible = false;
                }
            }

            if (m_smVisionInfo.g_blnWantPin1)
            {
                pnl_Pin1.Visible = true;
            }
            else
            {
                //only remove when fail count is 0, if fail count not 0 will remain
                if (m_smVisionInfo.g_intPin1FailureTotal == 0) //if (m_intPin1FailCount == 0)
                {
                    pnl_Pin1.Visible = false;
                }
            }

            if (m_smVisionInfo.g_blnWantGauge)
            {
                pnl_EdgeNotFound.Visible = true;
            }
            else
            {
                //only remove when fail count is 0, if fail count not 0 will remain
                if (m_smVisionInfo.g_intEdgeNotFoundFailureTotal == 0) //if (m_intEdgeNotFoundFailCount == 0)
                {
                    pnl_EdgeNotFound.Visible = false;
                }
            }

            if (m_smVisionInfo.g_blnWantCheckUnitSitProper)
            {
                pnl_Position.Visible = true;
            }
            else
            {
                //only remove when fail count is 0, if fail count not 0 will remain
                if (m_smVisionInfo.g_intPositionFailureTotal == 0) //if (m_intPositionFailCount == 0)
                {
                    pnl_Position.Visible = false;
                }
            }

            if (m_smVisionInfo.g_intNoTemplateFailureTotal != 0) //if (m_intNoTemplateFailCount != 0)
            {
                pnl_NoTemplate.Visible = true;
            }
            else
            {
                pnl_NoTemplate.Visible = false;
            }
        }

        private void btn_Fail_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intForceFailCounter = Convert.ToInt32(textBox1.Text);
        }

        public void SetMultiViewGUI(bool blnSet)
        {
            if (blnSet)
            {
                this.Size = new Size(380, 285);
                //pnl_Detail.Location = new Point(181, 128);
                this.AutoScroll = true;
            }
            else
            {
                this.Size = new Size(319, 569);
                //pnl_Detail.Location = new Point(5, 231);
                this.AutoScroll = false;
            }
        }

        private void label15_Click(object sender, EventArgs e)
        {
            m_smProductionInfo.m_blnTrackON = !m_smProductionInfo.m_blnTrackON;
            SRMMessageBox.Show("Track " + m_smProductionInfo.m_blnTrackON);
        }

        public void DisableField()
        {
            // 2021 07 20 - CCENG:  Change Bypass button to always display at any user right level. 
            //                      When user press the Bypass button, the login form will display if below user right level. 
            //                      Same like postseal bypass button view method.
            // 2020 10 29 - Applicable for New method of User Right Access Setting.mdb
            //string strChild1 = "ByPass";

            //if (m_smProductionInfo.g_intUserGroup > m_smCustomOption.objNewUserRight.GetInPocketChild1Group(m_smVisionInfo.g_strVisionName, strChild1, m_smVisionInfo.g_intVisionNameNo))
            //    btn_ByPassUnit.Visible = false;
            //else
            {
                if (m_smProductionInfo.g_blnWantBuypassIPM) //display only check box is checked in option
                    btn_ByPassUnit.Visible = true;
                else
                    btn_ByPassUnit.Visible = false;
            }

            //string strChild1 = "ByPass Button";
            //string strChild2 = "InPocket ByPass Unit";
            //if (m_smProductionInfo.g_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //    btn_ByPassUnit.Visible = false;
            //else
            //{
            //    if (m_smProductionInfo.g_blnWantBuypassIPM) //display only check box is checked in option
            //        btn_ByPassUnit.Visible = true;
            //    else
            //        btn_ByPassUnit.Visible = false;
            //}
            //btn_ByPassUnit.Visible = false;  // 2020 01 09 - Change true to false. Never display bypass button in IPM
        }

        private void EnableButton(bool blnEnable)
        {
            btn_ByPassUnit.Enabled = blnEnable;
            btn_Reset.Enabled = blnEnable;
        }

        public void LoadTemplateImage()
        {
            //return;

            string strRecipe = m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex];
            string strPath, strEmptyPath = "", strLeadPath = "";
            if (((m_smCustomOption.g_intWantMark & (0x01 << m_smVisionInfo.g_intVisionPos)) > 0) && !m_smVisionInfo.g_blnWantSkipMark)
            {
                strPath = m_smProductionInfo.g_strRecipePath + strRecipe +
                            "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\Template" + m_smVisionInfo.g_intSelectedGroup + "_";

            }
            else if ((m_smCustomOption.g_intWantOrient & (0x01 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                strPath = m_smProductionInfo.g_strRecipePath + strRecipe +
                            "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Orient\\Template\\Template";
            }
            else if ((m_smCustomOption.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                strPath = m_smProductionInfo.g_strRecipePath + strRecipe +
                            "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Package\\Template\\Template0.bmp";

                if (System.IO.File.Exists(strPath))
                {
                    pnl_template.AutoScroll = false;
                    pic_Learn1.Visible = true;
                    pic_Learn1.Size = new Size(180, 180);
                    pic_Learn1.BringToFront();
                    pic_Learn1.Load(strPath);
                    lbl_Temp1.BringToFront();
                }

                return;
            }
            else
                return;

            if (m_smVisionInfo.g_blnWantCheckEmpty)
            {
                strEmptyPath = m_smProductionInfo.g_strRecipePath + strRecipe +
                            "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Positioning\\Template\\EmptyTemplate0.bmp";
            }
           
            pnl_template.AutoScroll = false;
            pic_Learn1.Visible = false;
            pic_Learn2.Visible = false;
            pic_Learn3.Visible = false;
            pic_Learn4.Visible = false;
            pic_Learn5.Visible = false;
            pic_Learn6.Visible = false;
            pic_Learn7.Visible = false;
            pic_Learn8.Visible = false;
            if (pic_Learn1.Image != null)
            {
                pic_Learn1.Image.Dispose();
                pic_Learn1.Image = null;
            }
            if (pic_Learn2.Image != null)
            {
                pic_Learn2.Image.Dispose();
                pic_Learn2.Image = null;
            }
            if (pic_Learn3.Image != null)
            {
                pic_Learn3.Image.Dispose();
                pic_Learn3.Image = null;
            }
            if (pic_Learn4.Image != null)
            {
                pic_Learn4.Image.Dispose();
                pic_Learn4.Image = null;
            }
            if (pic_Learn5.Image != null)
            {
                pic_Learn5.Image.Dispose();
                pic_Learn5.Image = null;
            }
            if (pic_Learn6.Image != null)
            {
                pic_Learn6.Image.Dispose();
                pic_Learn6.Image = null;
            }
            if (pic_Learn7.Image != null)
            {
                pic_Learn7.Image.Dispose();
                pic_Learn7.Image = null;
            }
            if (pic_Learn8.Image != null)
            {
                pic_Learn8.Image.Dispose();
                pic_Learn8.Image = null;
            }
            lbl_Temp1.Visible = false;
            lbl_Temp2.Visible = false;
            lbl_Temp3.Visible = false;
            lbl_Temp4.Visible = false;
            lbl_Temp5.Visible = false;
            lbl_Temp6.Visible = false;
            lbl_Temp7.Visible = false;
            lbl_Temp8.Visible = false;

            Point[] pPicLocation = { new Point(0, 0), new Point(90, 0), new Point(0, 93), new Point(90, 93), new Point(0, 186), new Point(90, 186), new Point(0, 279), new Point(90, 279),
                new Point(0, 372), new Point(90, 372), new Point(0, 465), new Point(90, 465), new Point(0, 558) };
            Point[] pLbLLocation = { new Point(6, 6), new Point(96, 6), new Point(6, 99), new Point(96, 99), new Point(6, 192), new Point(96, 192), new Point(6, 285), new Point(96, 285),
                new Point(6, 378), new Point(96, 378), new Point(6, 471), new Point(96, 471), new Point(6, 564) };
            if (m_smVisionInfo.g_blnWantMultiTemplates)
            {
             
                int intCount = 0;
                pnl_template.AutoScroll = true;
                for (int i = 0; i < m_smVisionInfo.g_intTotalTemplates; i++)
                { 
                    //More than 4 template enable auto scroll
                    if (i == 4)
                        pnl_template.AutoScroll = true;
                    
                    switch (i)
                    {
                        case 0:
                            if (!pic_Learn1.Visible)
                            {
                                pic_Learn1.Visible = true;
                                pic_Learn1.Size = new Size(91, 94);

                                if ((m_smCustomOption.g_intWantOCR & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                                    lbl_Temp1.Visible = true;
                            }
                            if (File.Exists(strPath + "0.bmp"))
                            {
                                // 2018 08 09 - CCENG: Use FileStream with FileAccess.Read to load image. pic_Learn1.Load will lock the physical image and cause the image file cannot be overwrited during learn new image.
                                FileStream fileStream = new FileStream(strPath + "0.bmp", FileMode.Open, FileAccess.Read);
                                try
                                {
                                    pic_Learn1.Image = Image.FromStream(fileStream);
                                    fileStream.Close();
                                }
                                catch (Exception ex)
                                {
                                    STTrackLog.WriteLine("Vision1Page.cs > LoadTemplateImage() > ex = " + ex.ToString());
                                    fileStream.Close();
                                }
                            }
                            if ((m_smVisionInfo.g_intTemplateMask & 0x01) > 0)
                            {
                                lbl_Temp1.Visible = true;
                                pic_Learn1.Visible = true;
                                lbl_Temp1.BringToFront();
                                lbl_Temp1.ForeColor = Color.Lime;
                                intCount++;
                            }
                            else
                            {
                                lbl_Temp1.Visible = false;
                                pic_Learn1.Visible = false;
                            }
                            break;
                        case 1:
                            if (!pic_Learn2.Visible)
                            {
                                pic_Learn2.Visible = true;
                                if ((m_smCustomOption.g_intWantOCR & (0x01 << m_smVisionInfo.g_intVisionPos)) == 0)
                                    lbl_Temp2.Visible = true;
                            }
                            if (File.Exists(strPath + "1.bmp"))
                            {
                                FileStream fileStream = new FileStream(strPath + "1.bmp", FileMode.Open, FileAccess.Read);
                                try
                                {
                                    pic_Learn2.Image = Image.FromStream(fileStream);
                                    fileStream.Close();
                                }
                                catch (Exception ex)
                                {
                                    STTrackLog.WriteLine("Vision1Page.cs > LoadTemplateImage() > ex = " + ex.ToString());
                                    fileStream.Close();
                                }
                            }
                            if ((m_smVisionInfo.g_intTemplateMask & 0x02) > 0)
                            {
                                lbl_Temp2.Visible = true;
                                pic_Learn2.Visible = true;
                                lbl_Temp2.BringToFront();
                                lbl_Temp2.ForeColor = Color.Lime;
                                if (intCount == 0)
                                {
                                    lbl_Temp2.Location = pLbLLocation[0];
                                    pic_Learn2.Location = pPicLocation[0];
                                }
                                else
                                {
                                    lbl_Temp2.Location = pLbLLocation[1];
                                    pic_Learn2.Location = pPicLocation[1];
                                }
                                intCount++;
                            }
                            else
                            {
                                lbl_Temp2.Visible = false;
                                pic_Learn2.Visible = false;
                            }
                            break;
                        case 2:
                            if (!pic_Learn3.Visible)
                            {
                                pic_Learn3.Visible = true;
                                if ((m_smCustomOption.g_intWantOCR & (0x01 << m_smVisionInfo.g_intVisionPos)) == 0)
                                    lbl_Temp3.Visible = true;
                            }
                            if (File.Exists(strPath + "2.bmp"))
                            {
                                FileStream fileStream = new FileStream(strPath + "2.bmp", FileMode.Open, FileAccess.Read);
                                try
                                {
                                    pic_Learn3.Image = Image.FromStream(fileStream);
                                    fileStream.Close();
                                }
                                catch (Exception ex)
                                {
                                    STTrackLog.WriteLine("Vision1Page.cs > LoadTemplateImage() > ex = " + ex.ToString());
                                    fileStream.Close();
                                }
                            }

                            if ((m_smVisionInfo.g_intTemplateMask & 0x04) > 0)
                            {
                                lbl_Temp3.Visible = true;
                                pic_Learn3.Visible = true;
                                lbl_Temp3.BringToFront();
                                lbl_Temp3.ForeColor = Color.Lime;

                                if (intCount == 0)
                                {
                                    lbl_Temp3.Location = pLbLLocation[0];
                                    pic_Learn3.Location = pPicLocation[0];
                                }
                                else if (intCount == 1)
                                {
                                    lbl_Temp3.Location = pLbLLocation[1];
                                    pic_Learn3.Location = pPicLocation[1];
                                }
                                else
                                {
                                    lbl_Temp3.Location = pLbLLocation[2];
                                    pic_Learn3.Location = pPicLocation[2];
                                }
                                intCount++;
                            }
                            else
                            {
                                lbl_Temp3.Visible = false;
                                pic_Learn3.Visible = false;
                            }
                            break;
                        case 3:
                            if (!pic_Learn4.Visible)
                            {
                                pic_Learn4.Visible = true;
                                if ((m_smCustomOption.g_intWantOCR & (0x01 << m_smVisionInfo.g_intVisionPos)) == 0)
                                    lbl_Temp4.Visible = true;
                            }
                            if (File.Exists(strPath + "3.bmp"))
                            {
                                FileStream fileStream = new FileStream(strPath + "3.bmp", FileMode.Open, FileAccess.Read);
                                try
                                {
                                    pic_Learn4.Image = Image.FromStream(fileStream);
                                    fileStream.Close();
                                }
                                catch (Exception ex)
                                {
                                    STTrackLog.WriteLine("Vision1Page.cs > LoadTemplateImage() > ex = " + ex.ToString());
                                    fileStream.Close();
                                }
                            }

                            if ((m_smVisionInfo.g_intTemplateMask & 0x08) > 0)
                            {
                                lbl_Temp4.Visible = true;
                                pic_Learn4.Visible = true;
                                lbl_Temp4.BringToFront();
                                lbl_Temp4.ForeColor = Color.Lime;

                                if (intCount == 0)
                                {
                                    lbl_Temp4.Location = pLbLLocation[0];
                                    pic_Learn4.Location = pPicLocation[0];
                                }
                                else if (intCount == 1)
                                {
                                    lbl_Temp4.Location = pLbLLocation[1];
                                    pic_Learn4.Location = pPicLocation[1];
                                }
                                else if (intCount == 2)
                                {
                                    lbl_Temp4.Location = pLbLLocation[2];
                                    pic_Learn4.Location = pPicLocation[2];
                                }
                                else
                                {
                                    lbl_Temp4.Location = pLbLLocation[3];
                                    pic_Learn4.Location = pPicLocation[3];
                                }
                                intCount++;
                            }
                            else
                            {
                                lbl_Temp4.Visible = false;
                                pic_Learn4.Visible = false;
                            }
                            break;
                        case 4:
                            if (!pic_Learn5.Visible)
                            {
                                pic_Learn5.Visible = true;
                                if ((m_smCustomOption.g_intWantOCR & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                                    lbl_Temp5.Visible = true;
                            }
                            if (File.Exists(strPath + "4.bmp"))
                            {
                                FileStream fileStream = new FileStream(strPath + "4.bmp", FileMode.Open, FileAccess.Read);
                                try
                                {
                                    pic_Learn5.Image = Image.FromStream(fileStream);
                                    fileStream.Close();
                                }
                                catch (Exception ex)
                                {
                                    STTrackLog.WriteLine("Vision1Page.cs > LoadTemplateImage() > ex = " + ex.ToString());
                                    fileStream.Close();
                                }
                            }

                            if ((m_smVisionInfo.g_intTemplateMask & 0x10) > 0)
                            {
                                lbl_Temp5.Visible = true;
                                pic_Learn5.Visible = true;
                                lbl_Temp5.BringToFront();
                                lbl_Temp5.ForeColor = Color.Lime;

                                if (intCount == 0)
                                {
                                    lbl_Temp5.Location = pLbLLocation[0];
                                    pic_Learn5.Location = pPicLocation[0];
                                }
                                else if (intCount == 1)
                                {
                                    lbl_Temp5.Location = pLbLLocation[1];
                                    pic_Learn5.Location = pPicLocation[1];
                                }
                                else if (intCount == 2)
                                {
                                    lbl_Temp5.Location = pLbLLocation[2];
                                    pic_Learn5.Location = pPicLocation[2];
                                }
                                else if (intCount == 3)
                                {
                                    lbl_Temp5.Location = pLbLLocation[3];
                                    pic_Learn5.Location = pPicLocation[3];
                                }
                                else
                                {
                                    lbl_Temp5.Location = pLbLLocation[4];
                                    pic_Learn5.Location = pPicLocation[4];
                                }
                                intCount++;
                            }
                            else
                            {
                                lbl_Temp5.Visible = false;
                                pic_Learn5.Visible = false;
                            }
                            break;
                        case 5:
                            if (!pic_Learn6.Visible)
                            {
                                pic_Learn6.Visible = true;
                                if ((m_smCustomOption.g_intWantOCR & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                                    lbl_Temp6.Visible = true;
                            }
                            if (File.Exists(strPath + "5.bmp"))
                            {
                                FileStream fileStream = new FileStream(strPath + "5.bmp", FileMode.Open, FileAccess.Read);
                                try
                                {
                                    pic_Learn6.Image = Image.FromStream(fileStream);
                                    fileStream.Close();
                                }
                                catch (Exception ex)
                                {
                                    STTrackLog.WriteLine("Vision1Page.cs > LoadTemplateImage() > ex = " + ex.ToString());
                                    fileStream.Close();
                                }
                            }

                            if ((m_smVisionInfo.g_intTemplateMask & 0x20) > 0)
                            {
                                lbl_Temp6.Visible = true;
                                pic_Learn6.Visible = true;
                                lbl_Temp6.BringToFront();
                                lbl_Temp6.ForeColor = Color.Lime;

                                if (intCount == 0)
                                {
                                    lbl_Temp6.Location = pLbLLocation[0];
                                    pic_Learn6.Location = pPicLocation[0];
                                }
                                else if (intCount == 1)
                                {
                                    lbl_Temp6.Location = pLbLLocation[1];
                                    pic_Learn6.Location = pPicLocation[1];
                                }
                                else if (intCount == 2)
                                {
                                    lbl_Temp6.Location = pLbLLocation[2];
                                    pic_Learn6.Location = pPicLocation[2];
                                }
                                else if (intCount == 3)
                                {
                                    lbl_Temp6.Location = pLbLLocation[3];
                                    pic_Learn6.Location = pPicLocation[3];
                                }
                                else if (intCount == 4)
                                {
                                    lbl_Temp6.Location = pLbLLocation[4];
                                    pic_Learn6.Location = pPicLocation[4];
                                }
                                else
                                {
                                    lbl_Temp6.Location = pLbLLocation[5];
                                    pic_Learn6.Location = pPicLocation[5];
                                }
                                intCount++;
                            }
                            else
                            {
                                lbl_Temp6.Visible = false;
                                pic_Learn6.Visible = false;
                            }
                            break;
                        case 6:
                            if (!pic_Learn7.Visible)
                            {
                                pic_Learn7.Visible = true;
                                if ((m_smCustomOption.g_intWantOCR & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                                    lbl_Temp7.Visible = true;
                            }
                            if (File.Exists(strPath + "6.bmp"))
                            {
                                FileStream fileStream = new FileStream(strPath + "6.bmp", FileMode.Open, FileAccess.Read);
                                try
                                {
                                    pic_Learn7.Image = Image.FromStream(fileStream);
                                    fileStream.Close();
                                }
                                catch (Exception ex)
                                {
                                    STTrackLog.WriteLine("Vision1Page.cs > LoadTemplateImage() > ex = " + ex.ToString());
                                    fileStream.Close();
                                }
                            }

                            if ((m_smVisionInfo.g_intTemplateMask & 0x40) > 0)
                            {
                                lbl_Temp7.Visible = true;
                                pic_Learn7.Visible = true;
                                lbl_Temp7.BringToFront();
                                lbl_Temp7.ForeColor = Color.Lime;

                                if (intCount == 0)
                                {
                                    lbl_Temp7.Location = pLbLLocation[0];
                                    pic_Learn7.Location = pPicLocation[0];
                                }
                                else if (intCount == 1)
                                {
                                    lbl_Temp7.Location = pLbLLocation[1];
                                    pic_Learn7.Location = pPicLocation[1];
                                }
                                else if (intCount == 2)
                                {
                                    lbl_Temp7.Location = pLbLLocation[2];
                                    pic_Learn7.Location = pPicLocation[2];
                                }
                                else if (intCount == 3)
                                {
                                    lbl_Temp7.Location = pLbLLocation[3];
                                    pic_Learn7.Location = pPicLocation[3];
                                }
                                else if (intCount == 4)
                                {
                                    lbl_Temp7.Location = pLbLLocation[4];
                                    pic_Learn7.Location = pPicLocation[4];
                                }
                                else if (intCount == 5)
                                {
                                    lbl_Temp7.Location = pLbLLocation[5];
                                    pic_Learn7.Location = pPicLocation[5];
                                }
                                else
                                {
                                    lbl_Temp7.Location = pLbLLocation[6];
                                    pic_Learn7.Location = pPicLocation[6];
                                }
                                intCount++;
                            }
                            else
                            {
                                lbl_Temp7.Visible = false;
                                pic_Learn7.Visible = false;
                            }
                            break;
                        case 7:
                            if (!pic_Learn8.Visible)
                            {
                                pic_Learn8.Visible = true;
                                if ((m_smCustomOption.g_intWantOCR & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                                    lbl_Temp8.Visible = true;
                            }
                            if (File.Exists(strPath + "7.bmp"))
                            {
                                FileStream fileStream = new FileStream(strPath + "7.bmp", FileMode.Open, FileAccess.Read);
                                try
                                {
                                    pic_Learn8.Image = Image.FromStream(fileStream);
                                    fileStream.Close();
                                }
                                catch (Exception ex)
                                {
                                    STTrackLog.WriteLine("Vision1Page.cs > LoadTemplateImage() > ex = " + ex.ToString());
                                    fileStream.Close();
                                }
                            }

                            if ((m_smVisionInfo.g_intTemplateMask & 0x80) > 0)
                            {
                                lbl_Temp8.Visible = true;
                                pic_Learn8.Visible = true;
                                lbl_Temp8.BringToFront();
                                lbl_Temp8.ForeColor = Color.Lime;

                                if (intCount == 0)
                                {
                                    lbl_Temp8.Location = pLbLLocation[0];
                                    pic_Learn8.Location = pPicLocation[0];
                                }
                                else if (intCount == 1)
                                {
                                    lbl_Temp8.Location = pLbLLocation[1];
                                    pic_Learn8.Location = pPicLocation[1];
                                }
                                else if (intCount == 2)
                                {
                                    lbl_Temp8.Location = pLbLLocation[2];
                                    pic_Learn8.Location = pPicLocation[2];
                                }
                                else if (intCount == 3)
                                {
                                    lbl_Temp8.Location = pLbLLocation[3];
                                    pic_Learn8.Location = pPicLocation[3];
                                }
                                else if (intCount == 4)
                                {
                                    lbl_Temp8.Location = pLbLLocation[4];
                                    pic_Learn8.Location = pPicLocation[4];
                                }
                                else if (intCount == 5)
                                {
                                    lbl_Temp8.Location = pLbLLocation[5];
                                    pic_Learn8.Location = pPicLocation[5];
                                }
                                else if (intCount == 6)
                                {
                                    lbl_Temp8.Location = pLbLLocation[6];
                                    pic_Learn8.Location = pPicLocation[6];
                                }
                                else
                                {
                                    lbl_Temp8.Location = pLbLLocation[7];
                                    pic_Learn8.Location = pPicLocation[7];
                                }
                                intCount++;
                            }
                            else
                            {
                                lbl_Temp8.Visible = false;
                                pic_Learn8.Visible = false;
                            }
                            break;
                    }
                }
            }
            else
            {
                pnl_template.AutoScroll = false;
                pic_Learn1.Visible = true;
                pic_Learn1.Size = new Size(180, 180);
                if (File.Exists(strPath + "0.bmp"))
                {
                    FileStream fileStream = new FileStream(strPath + "0.bmp", FileMode.Open, FileAccess.Read);
                    pic_Learn1.Image = Image.FromStream(fileStream);
                    fileStream.Close();

                    //pic_Learn1.Load(strPath + "0.bmp");   // 2018 08 09 - CCENG: Use FileStream with FileAccess.Read to load image. pic_Learn1.Load will lock the physical image and cause the image file cannot be overwrited during learn new image.
                }
            }

            if (m_smVisionInfo.g_blnWantCheckEmpty)
            {
                if (m_smVisionInfo.g_intTotalTemplates == 4)
                    pnl_template.AutoScroll = true;

                if (!pic_LearnEmpty.Visible)
                {
                    pic_LearnEmpty.Visible = true;
                    lbl_TempEmpty.Visible = true;

                }

                if (File.Exists(strEmptyPath))
                {
                    FileStream fileStream = new FileStream(strEmptyPath, FileMode.Open, FileAccess.Read);
                    pic_LearnEmpty.Image = Image.FromStream(fileStream);
                    fileStream.Close();
                    lbl_TempEmpty.ForeColor = Color.Lime;
                    pic_Learn1.Size = new Size(91, 94);
                }
                else
                {
                    pic_LearnEmpty.Visible = false;
                    lbl_TempEmpty.Visible = false;
                    lbl_TempEmpty.ForeColor = Color.Red;
                }
                
                if (m_smVisionInfo.g_intTotalTemplates == 0)
                {
                    lbl_TempEmpty.Location = pLbLLocation[0];
                    pic_LearnEmpty.Location = pPicLocation[0];
                }
                else if (m_smVisionInfo.g_intTotalTemplates == 1)
                {
                    lbl_TempEmpty.Location = pLbLLocation[1];
                    pic_LearnEmpty.Location = pPicLocation[1];
                }
                else if (m_smVisionInfo.g_intTotalTemplates == 2)
                {
                    lbl_TempEmpty.Location = pLbLLocation[2];
                    pic_LearnEmpty.Location = pPicLocation[2];
                }
                else if (m_smVisionInfo.g_intTotalTemplates == 3)
                {
                    lbl_TempEmpty.Location = pLbLLocation[3];
                    pic_LearnEmpty.Location = pPicLocation[3];
                }
                else if (m_smVisionInfo.g_intTotalTemplates == 4)
                {
                    lbl_TempEmpty.Location = pLbLLocation[4];
                    pic_LearnEmpty.Location = pPicLocation[4];
                }
                else if (m_smVisionInfo.g_intTotalTemplates == 5)
                {
                    lbl_TempEmpty.Location = pLbLLocation[5];
                    pic_LearnEmpty.Location = pPicLocation[5];
                }
                else if (m_smVisionInfo.g_intTotalTemplates == 6)
                {
                    lbl_TempEmpty.Location = pLbLLocation[6];
                    pic_LearnEmpty.Location = pPicLocation[6];
                }
                else if (m_smVisionInfo.g_intTotalTemplates == 7)
                {
                    lbl_TempEmpty.Location = pLbLLocation[7];
                    pic_LearnEmpty.Location = pPicLocation[7];
                }
                else
                {
                    lbl_TempEmpty.Location = pLbLLocation[8];
                    pic_LearnEmpty.Location = pPicLocation[8];
                }
            }

            if ((m_smCustomOption.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                if (m_smVisionInfo.g_intTotalTemplates == 4)
                    pnl_template.AutoScroll = true;
                else if (m_smVisionInfo.g_intTotalTemplates == 3 && m_smVisionInfo.g_blnWantCheckEmpty)
                    pnl_template.AutoScroll = true;

                if (!pic_LearnLeadTop.Visible && m_smVisionInfo.g_arrLead[1].ref_blnSelected)
                {
                    pic_LearnLeadTop.Visible = true;
                    lbl_TempLeadTop.Visible = true;
                }
                if (!pic_LearnLeadRight.Visible && m_smVisionInfo.g_arrLead[2].ref_blnSelected)
                {
                    pic_LearnLeadRight.Visible = true;
                    lbl_TempLeadRight.Visible = true;
                }
                if (!pic_LearnLeadBottom.Visible && m_smVisionInfo.g_arrLead[3].ref_blnSelected)
                {
                    pic_LearnLeadBottom.Visible = true;
                    lbl_TempLeadBottom.Visible = true;
                }
                if (!pic_LearnLeadLeft.Visible && m_smVisionInfo.g_arrLead[4].ref_blnSelected)
                {
                    pic_LearnLeadLeft.Visible = true;
                    lbl_TempLeadLeft.Visible = true;
                }

                strLeadPath = m_smProductionInfo.g_strRecipePath + strRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead\\Template\\UnitTemplate1.bmp";

                if (File.Exists(strLeadPath) && m_smVisionInfo.g_arrLead[1].ref_blnSelected)
                {
                    FileStream fileStream = new FileStream(strLeadPath, FileMode.Open, FileAccess.Read);
                    pic_LearnLeadTop.Image = Image.FromStream(fileStream);
                    fileStream.Close();
                    lbl_TempLeadTop.ForeColor = Color.Lime;
                }
                else
                {
                    pic_LearnLeadTop.Visible = false;
                    lbl_TempLeadTop.Visible = false;
                    lbl_TempLeadTop.ForeColor = Color.Red;
                }

                strLeadPath = m_smProductionInfo.g_strRecipePath + strRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead\\Template\\UnitTemplate2.bmp";

                if (File.Exists(strLeadPath) && m_smVisionInfo.g_arrLead[2].ref_blnSelected)
                {
                    FileStream fileStream = new FileStream(strLeadPath, FileMode.Open, FileAccess.Read);
                    pic_LearnLeadRight.Image = Image.FromStream(fileStream);
                    fileStream.Close();
                    lbl_TempLeadRight.ForeColor = Color.Lime;
                }
                else
                {
                    pic_LearnLeadRight.Visible = false;
                    lbl_TempLeadRight.Visible = false;
                    lbl_TempLeadRight.ForeColor = Color.Red;
                }

                strLeadPath = m_smProductionInfo.g_strRecipePath + strRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead\\Template\\UnitTemplate3.bmp";

                if (File.Exists(strLeadPath) && m_smVisionInfo.g_arrLead[3].ref_blnSelected)
                {
                    FileStream fileStream = new FileStream(strLeadPath, FileMode.Open, FileAccess.Read);
                    pic_LearnLeadBottom.Image = Image.FromStream(fileStream);
                    fileStream.Close();
                    lbl_TempLeadBottom.ForeColor = Color.Lime;
                }
                else
                {
                    pic_LearnLeadBottom.Visible = false;
                    lbl_TempLeadBottom.Visible = false;
                    lbl_TempLeadBottom.ForeColor = Color.Red;
                }

                strLeadPath = m_smProductionInfo.g_strRecipePath + strRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead\\Template\\UnitTemplate4.bmp";

                if (File.Exists(strLeadPath) && m_smVisionInfo.g_arrLead[4].ref_blnSelected)
                {
                    FileStream fileStream = new FileStream(strLeadPath, FileMode.Open, FileAccess.Read);
                    pic_LearnLeadLeft.Image = Image.FromStream(fileStream);
                    fileStream.Close();
                    lbl_TempLeadLeft.ForeColor = Color.Lime;
                }
                else
                {
                    pic_LearnLeadLeft.Visible = false;
                    lbl_TempLeadLeft.Visible = false;
                    lbl_TempLeadLeft.ForeColor = Color.Red;
                }

                int intCountEmpty = 0;
                if (m_smVisionInfo.g_blnWantCheckEmpty)
                    intCountEmpty = 1;

                int intIndex = m_smVisionInfo.g_intTotalTemplates;

                if (lbl_TempLeadTop.ForeColor == Color.Lime)
                {
                    lbl_TempLeadTop.Location = pLbLLocation[intIndex + intCountEmpty];
                    pic_LearnLeadTop.Location = pPicLocation[intIndex + intCountEmpty];
                    intIndex++;
                }
                if (lbl_TempLeadRight.ForeColor == Color.Lime)
                {
                    lbl_TempLeadRight.Location = pLbLLocation[intIndex + intCountEmpty];
                    pic_LearnLeadRight.Location = pPicLocation[intIndex + intCountEmpty];
                    intIndex++;
                }
                if (lbl_TempLeadBottom.ForeColor == Color.Lime)
                {
                    lbl_TempLeadBottom.Location = pLbLLocation[intIndex + intCountEmpty];
                    pic_LearnLeadBottom.Location = pPicLocation[intIndex + intCountEmpty];
                    intIndex++;
                }
                if (lbl_TempLeadLeft.ForeColor == Color.Lime)
                {
                    lbl_TempLeadLeft.Location = pLbLLocation[intIndex + intCountEmpty];
                    pic_LearnLeadLeft.Location = pPicLocation[intIndex + intCountEmpty];
                    intIndex++;
                }

            }
        }

        private void ResetCount()
        {
            //m_smVisionInfo.g_intMarkFailureTotal = 0;
            //m_smVisionInfo.g_intOrientFailureTotal = 0;
            //m_smVisionInfo.g_intPin1FailureTotal = 0;
            //m_smVisionInfo.g_intPackageFailureTotal = 0;
            //m_smVisionInfo.g_intLeadFailureTotal = 0;
            //m_smVisionInfo.g_intPassTotal = 0;
            //m_smVisionInfo.g_intTestedTotal = 0;
            //m_smVisionInfo.g_intPassImageCount = 0;
            //m_smVisionInfo.g_intFailImageCount = 0;
            //m_smVisionInfo.g_intLowYieldUnitCount = 0;

            //m_smVisionInfo.VS_AT_UpdateQuantity = true;

            lbl_PassCount.Text = "0";
            lbl_FailCount.Text = "0";
            lbl_TestedTotal.Text = "0";
            lbl_OrientFailCount.Text = "0";
            lbl_MarkFailCount.Text = "0";
            lbl_LeadFailCount.Text = "0";
            lbl_PackageFailCount.Text = "0";
            lbl_EmptyFailCount.Text = "0";
            lbl_Pin1FailCount.Text = "0";
            lbl_PositionFailCount.Text = "0";
            lbl_NoTemplateFailCount.Text = "0";
            lbl_PackageDefectFailCount.Text = "0";
            lbl_PackageColorDefectFailCount.Text = "0";
            lbl_EdgeNotFoundFailCount.Text = "0";
            lbl_MarkFailPercent.Text = "0.00";
            lbl_LeadFailPercent.Text = "0.00";
            lbl_OrientFailPercent.Text = "0.00";
            lbl_Pin1FailPercent.Text = "0.00";
            lbl_EmptyFailPercent.Text = "0.00";
            lbl_PackageFailPercent.Text = "0.00";
            lbl_PositionFailPercent.Text = "0.00";
            lbl_NoTemplateFailPercent.Text = "0.00";
            lbl_PackageDefectFailPercent.Text = "0.00";
            lbl_PackageColorDefectFailPercent.Text = "0.00";
            lbl_EdgeNotFoundFailPercent.Text = "0.00";


            lbl_Yield.Text = "0.00";
            lbl_Yield.BackColor = Color.White;
        }

        private void UpdateInfo()
        {
            m_strResult = m_smVisionInfo.g_strResult;
            if (m_strResult != m_strResultPrev)
            {
                switch (m_strResult)
                {
                    case "Pass":
                        lbl_ResultStatus1.BackColor = Color.Lime;
                        lbl_ResultStatus1.Text = m_strResult;
                        break;
                    case "Fail":
                        lbl_ResultStatus1.BackColor = Color.Red;
                        lbl_ResultStatus1.Text = m_strResult;
                        break;
                    case "Empty":
                        lbl_ResultStatus1.BackColor = Color.Lime;
                        lbl_ResultStatus1.Text = m_strResult;
                        break;
                    case "NoEmpty":
                        lbl_ResultStatus1.BackColor = Color.Red;
                        lbl_ResultStatus1.Text = m_strResult;
                        break;
                    case "----":
                        lbl_ResultStatus1.BackColor = SystemColors.InactiveCaptionText;
                        lbl_ResultStatus1.Text = "----";
                        break;
                }

                m_strResultPrev = m_strResult;
            }

            if (m_smVisionInfo.g_intUnitsOnImage > 1)
            {
                m_strResult2 = m_smVisionInfo.g_strResult2;
                if (m_strResult2 != m_strResult2Prev)
                {
                    switch (m_strResult2)
                    {
                        case "Pass":
                            lbl_ResultStatus2.BackColor = Color.Lime;
                            lbl_ResultStatus2.Text = m_strResult2;
                            break;
                        case "Fail":
                            lbl_ResultStatus2.BackColor = Color.Red;
                            lbl_ResultStatus2.Text = m_strResult2;
                            break;
                        case "----":
                            lbl_ResultStatus2.BackColor = Color.White;
                            lbl_ResultStatus2.Text = "----";
                            break;

                    }

                    m_strResult2Prev = m_strResult2;
                }
            }

            m_intPassCount = m_smVisionInfo.g_intPassTotal;
            if (m_intPassCount != m_intPassCountPrev)
            {
                lbl_PassCount.Text = m_intPassCount.ToString();
                m_intPassCountPrev = m_intPassCount;
            }

            m_intFailCount = m_smVisionInfo.g_intTestedTotal - m_smVisionInfo.g_intPassTotal;
            if (m_intFailCount != m_intFailCountPrev)
            {
                lbl_FailCount.Text = m_intFailCount.ToString();
                m_intFailCountPrev = m_intFailCount;
            }

            m_intTestedTotal = m_smVisionInfo.g_intTestedTotal;
            if (m_intTestedTotal != m_intTestedTotalPrev)
            {
                lbl_TestedTotal.Text = m_intTestedTotal.ToString();

                if (m_intTestedTotal != 0)
                {
                    float fYield = (m_intPassCount / (float)m_intTestedTotal) * 100;
                    lbl_Yield.Text = fYield.ToString("f2");

                    if (fYield <= m_smVisionInfo.g_fLowYield)
                        lbl_Yield.BackColor = Color.Red;
                    else
                        lbl_Yield.BackColor = Color.White;
                }
                else
                {
                    lbl_Yield.Text = "0.00";
                    lbl_Yield.BackColor = Color.White;
                }

                m_intTestedTotalPrev = m_intTestedTotal;
            }

            m_intMarkFailCount = m_smVisionInfo.g_intMarkFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fMarkFailPercent = (m_intMarkFailCount / (float)m_intTestedTotal) * 100;
                if (fMarkFailPercent > 100)
                    fMarkFailPercent = 100;
                lbl_MarkFailPercent.Text = fMarkFailPercent.ToString("f2");
            }
            else
                lbl_MarkFailPercent.Text = "0.00";

            if (m_intMarkFailCount != m_intMarkFailCountPrev)
            {
                lbl_MarkFailCount.BackColor = Color.Red;
                lbl_MarkFailPercent.BackColor = Color.Red;
                lbl_MarkFailCount.Text = m_intMarkFailCount.ToString();
                m_intMarkFailCountPrev = m_intMarkFailCount;
            }
            else
            {
                lbl_MarkFailCount.BackColor = Color.White;
                lbl_MarkFailPercent.BackColor = Color.White;
            }

            m_intLeadFailCount = m_smVisionInfo.g_intLeadFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fLeadFailPercent = (m_intLeadFailCount / (float)m_intTestedTotal) * 100;
                if (fLeadFailPercent > 100)
                    fLeadFailPercent = 100;
                lbl_LeadFailPercent.Text = fLeadFailPercent.ToString("f2");
            }
            else
                lbl_LeadFailPercent.Text = "0.00";

            if (m_intLeadFailCount != m_intLeadFailCountPrev)
            {
                lbl_LeadFailCount.BackColor = Color.Red;
                lbl_LeadFailPercent.BackColor = Color.Red;
                lbl_LeadFailCount.Text = m_intLeadFailCount.ToString();
                m_intLeadFailCountPrev = m_intLeadFailCount;
            }
            else
            {
                lbl_LeadFailCount.BackColor = Color.White;
                lbl_LeadFailPercent.BackColor = Color.White;
            }

            m_intOrientFailCount = m_smVisionInfo.g_intOrientFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fOrientFailPercent = (m_intOrientFailCount / (float)m_intTestedTotal) * 100;
                if (fOrientFailPercent > 100)
                    fOrientFailPercent = 100;
                lbl_OrientFailPercent.Text = fOrientFailPercent.ToString("f2");
            }
            else
                lbl_OrientFailPercent.Text = "0.00";

            if (m_intOrientFailCount != m_intOrientFailCountPrev)
            {
                lbl_OrientFailCount.BackColor = Color.Red;
                lbl_OrientFailPercent.BackColor = Color.Red;
                lbl_OrientFailCount.Text = m_intOrientFailCount.ToString();
                m_intOrientFailCountPrev = m_intOrientFailCount;
            }
            else
            {
                lbl_OrientFailCount.BackColor = Color.White;
                lbl_OrientFailPercent.BackColor = Color.White;
            }

            m_intPin1FailCount = m_smVisionInfo.g_intPin1FailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fPin1FailPercent = (m_intPin1FailCount / (float)m_intTestedTotal) * 100;
                if (fPin1FailPercent > 100)
                    fPin1FailPercent = 100;
                lbl_Pin1FailPercent.Text = fPin1FailPercent.ToString("f2");
            }
            else
                lbl_Pin1FailPercent.Text = "0.00";

            if (m_intPin1FailCount != m_intPin1FailCountPrev)
            {
                lbl_Pin1FailCount.BackColor = Color.Red;
                lbl_Pin1FailPercent.BackColor = Color.Red;
                lbl_Pin1FailCount.Text = m_intPin1FailCount.ToString();
                m_intPin1FailCountPrev = m_intPin1FailCount;
            }
            else
            {
                lbl_Pin1FailCount.BackColor = Color.White;
                lbl_Pin1FailPercent.BackColor = Color.White;
            }

            m_intEmptyUnitCount = m_smVisionInfo.g_intCheckPresenceFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fEmptyFailPercent = (m_intEmptyUnitCount / (float)m_intTestedTotal) * 100;
                if (fEmptyFailPercent > 100)
                    fEmptyFailPercent = 100;
                lbl_EmptyFailPercent.Text = fEmptyFailPercent.ToString("f2");
            }
            else
                lbl_EmptyFailPercent.Text = "0.00";

            if (m_intEmptyUnitCount != m_intEmptyUnitCountPrev)
            {
                lbl_EmptyFailCount.BackColor = Color.Red;
                lbl_EmptyFailPercent.BackColor = Color.Red;
                lbl_EmptyFailCount.Text = m_intEmptyUnitCount.ToString();
                m_intEmptyUnitCountPrev = m_intEmptyUnitCount;
            }
            else
            {
                lbl_EmptyFailCount.BackColor = Color.White;
                lbl_EmptyFailPercent.BackColor = Color.White;
            }

            m_intPackageFailCount = m_smVisionInfo.g_intPackageFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fPackageFailPercent = (m_intPackageFailCount / (float)m_intTestedTotal) * 100;
                if (fPackageFailPercent > 100)
                    fPackageFailPercent = 100;
                lbl_PackageFailPercent.Text = fPackageFailPercent.ToString("f2");
            }
            else
                lbl_PackageFailPercent.Text = "0.00";

            if (m_intPackageFailCount != m_intPackageFailCountPrev)
            {
                lbl_PackageFailCount.BackColor = Color.Red;
                lbl_PackageFailPercent.BackColor = Color.Red;
                lbl_PackageFailCount.Text = m_intPackageFailCount.ToString();
                m_intPackageFailCountPrev = m_intPackageFailCount;
            }
            else
            {
                lbl_PackageFailCount.BackColor = Color.White;
                lbl_PackageFailPercent.BackColor = Color.White;
            }

            m_intPackageDefectFailCount = m_smVisionInfo.g_intPkgDefectFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fPackageDefectFailPercent = (m_intPackageDefectFailCount / (float)m_intTestedTotal) * 100;
                if (fPackageDefectFailPercent > 100)
                    fPackageDefectFailPercent = 100;
                lbl_PackageDefectFailPercent.Text = fPackageDefectFailPercent.ToString("f2");
            }
            else
                lbl_PackageDefectFailPercent.Text = "0.00";

            if (m_intPackageDefectFailCount != m_intPackageDefectFailCountPrev)
            {
                lbl_PackageDefectFailCount.BackColor = Color.Red;
                lbl_PackageDefectFailPercent.BackColor = Color.Red;
                lbl_PackageDefectFailCount.Text = m_intPackageDefectFailCount.ToString();
                m_intPackageDefectFailCountPrev = m_intPackageDefectFailCount;
            }
            else
            {
                lbl_PackageDefectFailCount.BackColor = Color.White;
                lbl_PackageDefectFailPercent.BackColor = Color.White;
            }

            m_intPackageColorDefectFailCount = m_smVisionInfo.g_intPkgColorDefectFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fPackageColorDefectFailPercent = (m_intPackageColorDefectFailCount / (float)m_intTestedTotal) * 100;
                if (fPackageColorDefectFailPercent > 100)
                    fPackageColorDefectFailPercent = 100;
                lbl_PackageColorDefectFailPercent.Text = fPackageColorDefectFailPercent.ToString("f2");
            }
            else
                lbl_PackageColorDefectFailPercent.Text = "0.00";

            if (m_intPackageColorDefectFailCount != m_intPackageColorDefectFailCountPrev)
            {
                lbl_PackageColorDefectFailCount.BackColor = Color.Red;
                lbl_PackageColorDefectFailPercent.BackColor = Color.Red;
                lbl_PackageColorDefectFailCount.Text = m_intPackageColorDefectFailCount.ToString();
                m_intPackageColorDefectFailCountPrev = m_intPackageColorDefectFailCount;
            }
            else
            {
                lbl_PackageColorDefectFailCount.BackColor = Color.White;
                lbl_PackageColorDefectFailPercent.BackColor = Color.White;
            }

            m_intEdgeNotFoundFailCount = m_smVisionInfo.g_intEdgeNotFoundFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fEdgeNotFoundFailPercent = (m_intEdgeNotFoundFailCount / (float)m_intTestedTotal) * 100;
                if (fEdgeNotFoundFailPercent > 100)
                    fEdgeNotFoundFailPercent = 100;
                lbl_EdgeNotFoundFailPercent.Text = fEdgeNotFoundFailPercent.ToString("f2");
            }
            else
                lbl_EdgeNotFoundFailPercent.Text = "0.00";

            if (m_intEdgeNotFoundFailCount != m_intEdgeNotFoundFailCountPrev)
            {
                lbl_EdgeNotFoundFailCount.BackColor = Color.Red;
                lbl_EdgeNotFoundFailPercent.BackColor = Color.Red;
                lbl_EdgeNotFoundFailCount.Text = m_intEdgeNotFoundFailCount.ToString();
                m_intEdgeNotFoundFailCountPrev = m_intEdgeNotFoundFailCount;
            }
            else
            {
                lbl_EdgeNotFoundFailCount.BackColor = Color.White;
                lbl_EdgeNotFoundFailPercent.BackColor = Color.White;
            }

            m_intPositionFailCount = m_smVisionInfo.g_intPositionFailureTotal;
            if (m_intTestedTotal != 0)
            {
                float fPositionFailPercent = (m_intPositionFailCount / (float)m_intTestedTotal) * 100;
                if (fPositionFailPercent > 100)
                    fPositionFailPercent = 100;
                lbl_PositionFailPercent.Text = fPositionFailPercent.ToString("f2");
            }
            else
                lbl_PositionFailPercent.Text = "0.00";

            if (m_intPositionFailCount != m_intPositionFailCountPrev)
            {
                lbl_PositionFailCount.BackColor = Color.Red;
                lbl_PositionFailPercent.BackColor = Color.Red;
                lbl_PositionFailCount.Text = m_intPositionFailCount.ToString();
                m_intPositionFailCountPrev = m_intPositionFailCount;
            }
            else
            {
                lbl_PositionFailCount.BackColor = Color.White;
                lbl_PositionFailPercent.BackColor = Color.White;
            }

            //-------------------------------------Special cases------------------------------------------------
            m_intNoTemplateFailCount = m_smVisionInfo.g_intNoTemplateFailureTotal;
            if (m_intNoTemplateFailCount != 0)
            {
                pnl_NoTemplate.Visible = true;
            }
            else
            {
                pnl_NoTemplate.Visible = false;
            }

            if (m_intTestedTotal != 0)
            {
                float fNoTemplateFailPercent = (m_intNoTemplateFailCount / (float)m_intTestedTotal) * 100;
                if (fNoTemplateFailPercent > 100)
                    fNoTemplateFailPercent = 100;
                lbl_NoTemplateFailPercent.Text = fNoTemplateFailPercent.ToString("f2");
            }
            else
                lbl_NoTemplateFailPercent.Text = "0.00";

            if (m_intNoTemplateFailCount != m_intNoTemplateFailCountPrev)
            {
                lbl_NoTemplateFailCount.BackColor = Color.Red;
                lbl_NoTemplateFailPercent.BackColor = Color.Red;
                lbl_NoTemplateFailCount.Text = m_intNoTemplateFailCount.ToString();
                m_intNoTemplateFailCountPrev = m_intNoTemplateFailCount;
            }
            else
            {
                lbl_NoTemplateFailCount.BackColor = Color.White;
                lbl_NoTemplateFailPercent.BackColor = Color.White;
            }

            if (m_smVisionInfo.g_blnNoGrabTime)
            {
                lbl_GrabDelay.Text = "0";
                lbl_GrabTime.Text = "0";
                lbl_ProcessTime.Text = m_smVisionInfo.g_objTotalTime.Duration.ToString("f2");
            }
            else
            {
                lbl_GrabDelay.Text = m_smVisionInfo.g_intCameraGrabDelay.ToString();
                lbl_GrabTime.Text = (Math.Max(0, m_smVisionInfo.g_objGrabTime.Duration - m_smVisionInfo.g_intCameraGrabDelay)).ToString("f0"); //29-05-2019 ZJYEOH : Need to minus grab delay as it included inside Grab time 
                lbl_ProcessTime.Text = (Math.Max(0, m_smVisionInfo.g_objTotalTime.Duration - m_smVisionInfo.g_objGrabTime.Duration)).ToString("f2");
            }
            lbl_TotalTime.Text = m_smVisionInfo.g_objTotalTime.Duration.ToString("f2");
        }

        private void UpdateLotNo()
        {
            //lbl_LotID.Text = m_smProductionInfo.g_strLotID;
            lbl_LotID.Text = m_smProductionInfo.g_arrSingleLotID[m_smVisionInfo.g_intVisionIndex];
            lbl_OperatorID.Text = m_smProductionInfo.g_strOperatorID;
            UpdateInfo();
        }


        private void btn_ByPassUnit_Click(object sender, EventArgs e)
        {
            try
            {
                STTrackLog.WriteLine("Vision4Page > btn_ByPassUnit_Click 1");

                string strChild1 = "ByPass";
                m_smVisionInfo.VM_PR_ByPassUnit = false;

                STTrackLog.WriteLine("Vision4Page > btn_ByPassUnit_Click. 2");
                int intUserGroup = 5;
                LoginForm objLogin = new LoginForm(m_smProductionInfo);
                // 2020 02 17 - JBTAN: Login as higher level to bypass alarm
                if (objLogin.ShowDialog(this) == DialogResult.OK)
                {
                    intUserGroup = objLogin.ref_intUserGroup;
                    STTrackLog.WriteLine("Vision4Page > btn_ByPassUnit_Click. 4. intUserGroup=" + intUserGroup.ToString());
                }

                do
                {
                    STTrackLog.WriteLine("Vision4Page > btn_ByPassUnit_Click. 5");
                    int ittt = m_smCustomOption.objNewUserRight.GetInPocketChild1Group(m_smVisionInfo.g_strVisionName, strChild1, m_smVisionInfo.g_intVisionNameNo);
                    if (intUserGroup > m_smCustomOption.objNewUserRight.GetInPocketChild1Group(m_smVisionInfo.g_strVisionName, strChild1, m_smVisionInfo.g_intVisionNameNo))
                    {
                        STTrackLog.WriteLine("Vision4Page > btn_ByPassUnit_Click. 6");
                        SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                        if (intUserGroup == 5)
                        {
                            STTrackLog.WriteLine("Vision4Page > btn_ByPassUnit_Click. 7");
                            objLogin.Dispose();
                            return;
                        }
                        else
                        {
                            if (objLogin.ShowDialog(this) != DialogResult.OK)
                            {
                                STTrackLog.WriteLine("Vision4Page > btn_ByPassUnit_Click. 8");
                                objLogin.Dispose();
                                return;
                            }
                            else
                                intUserGroup = objLogin.ref_intUserGroup;
                        }
                    }
                    else
                    {
                        STTrackLog.WriteLine("Vision4Page > btn_ByPassUnit_Click. 5");
                        if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        {
                            STTrackLog.WriteLine("Vision4Page > btn_ByPassUnit_Click. 10");

                            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);

                            m_smVisionInfo.VM_PR_ByPassUnit = true;
                            objLogin.Dispose();
                            return;
                        }
                        else
                        {
                            STTrackLog.WriteLine("Vision4Page > btn_ByPassUnit_Click. 11");
                            objLogin.Dispose();
                            return;
                        }
                    }

                    System.Threading.Thread.Sleep(1);
                } while (m_smVisionInfo.VM_PR_ByPassUnit == false);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("Vision4Page > btn_ByPassUnit_Click Exception = " + ex.ToString());
                SRMMessageBox.Show("Vision4Page > btn_ByPassUnit_Click Exception = " + ex.ToString());
            }

            //if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            //{
            //    m_smVisionInfo.VM_PR_ByPassUnit = true;
            //}
            //else
            //{
            //    m_smVisionInfo.VM_PR_ByPassUnit = false;
            //}
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            ResetCount();
            ResetCounterBackColor();
            m_smProductionInfo.VM_TH_UpdateCount = true;
        }




        private void timer_Live_Tick(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_intMachineStatus == 2 && m_intMachineStatusPrev != 2)
            {
                EnableButton(false);
                m_intMachineStatusPrev = 2;
            }
            else if (m_intMachineStatusPrev != m_smVisionInfo.g_intMachineStatus)
            {
                EnableButton(true);
                m_intMachineStatusPrev = m_smVisionInfo.g_intMachineStatus;
            }

            if (m_smVisionInfo.g_blnResetGUIDisplayCount)
            {
                m_smVisionInfo.g_blnResetGUIDisplayCount = false;
                ResetCount();
                ResetCounterBackColor();
                m_smProductionInfo.VM_TH_UpdateCount = true;
            }

            if (m_smVisionInfo.PR_VM_UpdateQuantity)
            {
                UpdateInfo();
                m_smVisionInfo.PR_VM_UpdateQuantity = false;
            }

            if (m_smVisionInfo.PG_VM_LoadTemplate)
            {
                LoadTemplateImage();
                m_smVisionInfo.PG_VM_LoadTemplate = false;
            }

            if (m_smVisionInfo.AT_VM_UpdateProduction)
            {
                UpdateLotNo();
                LoadTemplateImage();
                UpdateInfo(); 
                m_smVisionInfo.AT_VM_UpdateProduction = false;
            }
        }
        
        private void txt_UB_TextChanged(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intTemporaryValue = Convert.ToInt32(txt_UB.Text);
        }

        private void srmInputBox1_TextChanged(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intTemporaryValue2 = Convert.ToInt32(txt_Delay.Text);
        }

        private void chk_ForceXtozero_Click(object sender, EventArgs e)
        {
            if (chk_ForceXtozero.Checked)
                m_smVisionInfo.g_intForceZero = 1;
            else
                m_smVisionInfo.g_intForceZero = 0;
        }

        private void chk_ForceYtozero_Click(object sender, EventArgs e)
        {
            if (chk_ForceYtozero.Checked)
                m_smVisionInfo.g_intForceYZero = 1;
            else
                m_smVisionInfo.g_intForceYZero = 0;

        }
     

    }
}