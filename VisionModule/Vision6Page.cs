using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharedMemory;
using Common;
using System.IO;
using User;
namespace VisionModule
{
    public partial class Vision6Page : Form
    {
        #region Member Variables

        private int m_intMachineStatusPrev = 1;

        // Count
        private int m_intPassCount = 0, m_intPassCountPrev = -1;
        private int m_intFailCount = 0, m_intFailCountPrev = -1;
        private int m_intSealFailCount = 0, m_intSealFailCountPrev = -1;
        private int m_intSealDistanceFailCount = 0, m_intSealDistanceFailCountPrev = -1;
        private int m_intSealOverHeatFailCount = 0, m_intSealOverHeatFailCountPrev = -1;
        private int m_intUnitPresenceFailCount = 0, m_intUnitPresenceFailCountPrev = -1;
        private int m_intUnitOrientationFailCount = 0, m_intUnitOrientationFailCountPrev = -1;
        private int m_intBrokenAreaFailCount = 0, m_intBrokenAreaFailCountPrev = -1;
        private int m_intBrokenGapFailCount = 0, m_intBrokenGapFailCountPrev = -1;
        private int m_intSprocketHoleFailCount = 0, m_intSprocketHoleFailCountPrev = -1;
        private int m_intSprocketHoleDiameterFailCount = 0, m_intSprocketHoleDiameterFailCountPrev = -1;
        private int m_intSprocketHoleDefectFailCount = 0, m_intSprocketHoleDefectFailCountPrev = -1;
        private int m_intSprocketHoleBrokenFailCount = 0, m_intSprocketHoleBrokenFailCountPrev = -1;
        private int m_intSprocketHoleRoundnessFailCount = 0, m_intSprocketHoleRoundnessFailCountPrev = -1;
        private int m_intSealEdgeStraightnessFailCount = 0, m_intSealEdgeStraightnessFailCountPrev = -1;
        private int m_intNoTemplateFailCount = 0, m_intNoTemplateFailCountPrev = -1;
        private int m_intPosNoFoundFailCount = 0, m_intPosNoFoundFailCountPrev = -1;
        private int m_intTestedTotal = 0, m_intTestedTotalPrev = -1;
        private string m_strResult = "", m_strResultPrev = "";
        private bool m_blnEnableByPass = false;

        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo m_smVisionInfo;

        private UserRight m_objUserRight = new UserRight();

        #endregion

        public Vision6Page(CustomOption smCustomOption, ProductionInfo smProductionInfo, VisionInfo smVisionInfo)
        {
            InitializeComponent();

            m_smCustomizeInfo = smCustomOption;
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
            
        }
        private void ResetCounterBackColor()
        {
            lbl_SealFailCount.BackColor = Color.White;
            lbl_SealFailPercent.BackColor = Color.White;
            lbl_NoTemplateFailCount.BackColor = Color.White;
            lbl_NoTemplateFailPercent.BackColor = Color.White;
            lbl_SealDistanceFailCount.BackColor = Color.White;
            lbl_SealDistanceFailPercent.BackColor = Color.White;
            lbl_OverHeatFailCount.BackColor = Color.White;
            lbl_OverHeatFailPercent.BackColor = Color.White;
            lbl_BrokenAreaFailCount.BackColor = Color.White;
            lbl_BrokenAreaFailPercent.BackColor = Color.White;
            lbl_BrokenGapFailCount.BackColor = Color.White;
            lbl_BrokenGapFailPercent.BackColor = Color.White;
            lbl_UnitPresenceFailCount.BackColor = Color.White;
            lbl_UnitPresenceFailPercent.BackColor = Color.White;
            lbl_OrientationFailCount.BackColor = Color.White;
            lbl_OrientationFailPercent.BackColor = Color.White;
            lbl_SprocketHoleFailCount.BackColor = Color.White;
            lbl_SprocketHoleFailPercent.BackColor = Color.White;
            lbl_SprocketHoleDiameterFailCount.BackColor = Color.White;
            lbl_SprocketHoleDiameterFailPercent.BackColor = Color.White;
            lbl_SprocketHoleDefectFailCount.BackColor = Color.White;
            lbl_SprocketHoleDefectFailPercent.BackColor = Color.White;
            lbl_SprocketHoleBrokenFailCount.BackColor = Color.White;
            lbl_SprocketHoleBrokenFailPercent.BackColor = Color.White;
            lbl_SprocketHoleRoundnessFailCount.BackColor = Color.White;
            lbl_SprocketHoleRoundnessFailPercent.BackColor = Color.White;
            lbl_PositionNotFoundFailCount.BackColor = Color.White;
            lbl_PositionNotFoundFailPercent.BackColor = Color.White;
            lbl_SealEdgeStraightnessFailCount.BackColor = Color.White;
            lbl_SealEdgeStraightnessFailPercent.BackColor = Color.White;
        }

        /// <summary>
        /// Make sure that only when form is focused, then enable timer.
        /// By this way, CPU usage can be reduced.
        /// </summary>
        public void ActivateTimer(bool blnEnable)
        {
            timer_Live.Enabled = blnEnable;
        }

        public void LoadTemplateImage()
        {
            string strRecipe = m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex];
            string strMarkPath = m_smProductionInfo.g_strRecipePath + strRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Seal\\Template\\Mark\\MarkTemplate0_";
            string strEmptyPath = m_smProductionInfo.g_strRecipePath + strRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Seal\\Template\\Pocket\\PocketTemplate0_";

            pnl_template.AutoScroll = false;
            pic_Learn1.Visible = false;
            pic_Learn2.Visible = false;
            pic_Learn3.Visible = false;
            pic_Learn4.Visible = false;
            pic_Learn5.Visible = false;
            pic_Learn6.Visible = false;
            pic_Learn7.Visible = false;
            pic_Learn8.Visible = false;
            pic_LearnEmpty1.Visible = false;
            pic_LearnEmpty2.Visible = false;
            pic_LearnEmpty3.Visible = false;
            pic_LearnEmpty4.Visible = false;
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
            if (pic_LearnEmpty1.Image != null)
            {
                pic_LearnEmpty1.Image.Dispose();
                pic_LearnEmpty1.Image = null;
            }
            if (pic_LearnEmpty2.Image != null)
            {
                pic_LearnEmpty2.Image.Dispose();
                pic_LearnEmpty2.Image = null;
            }
            if (pic_LearnEmpty3.Image != null)
            {
                pic_LearnEmpty3.Image.Dispose();
                pic_LearnEmpty3.Image = null;
            }
            if (pic_LearnEmpty4.Image != null)
            {
                pic_LearnEmpty4.Image.Dispose();
                pic_LearnEmpty4.Image = null;
            }
            lbl_Temp1.Visible = false;
            lbl_Temp2.Visible = false;
            lbl_Temp3.Visible = false;
            lbl_Temp4.Visible = false;
            lbl_Temp5.Visible = false;
            lbl_Temp6.Visible = false;
            lbl_Temp7.Visible = false;
            lbl_Temp8.Visible = false;
            lbl_TempEmpty1.Visible = false;
            lbl_TempEmpty2.Visible = false;
            lbl_TempEmpty3.Visible = false;
            lbl_TempEmpty4.Visible = false;

            Point[] pPicLocation = { new Point(0, 0), new Point(90, 0), new Point(0, 93), new Point(90, 93), new Point(0, 186), new Point(90, 186), new Point(0, 279), new Point(90, 279),
                                     new Point(0, 372), new Point(90, 372), new Point(0, 465), new Point(90, 465), new Point(0, 558), new Point(90, 558), new Point(0, 651), new Point(90, 651),
                                     new Point(0, 744), new Point(90, 744), new Point(0, 837), new Point(90, 837)};
            Point[] pLbLLocation = { new Point(6, 6), new Point(96, 6), new Point(6, 99), new Point(96, 99), new Point(6, 192), new Point(96, 192), new Point(6, 285), new Point(96, 285),
                                     new Point(6, 378), new Point(96, 378), new Point(6, 471), new Point(96, 471), new Point(6, 564), new Point(96, 564), new Point(6, 657), new Point(96, 657),
                                     new Point(0, 750), new Point(90, 750), new Point(0, 843), new Point(90, 843)};
            //if (m_smVisionInfo.g_blnWantMultiTemplates)
            //{

            int intCount = 0;
            pnl_template.AutoScroll = true;
            for (int i = 0; i < m_smVisionInfo.g_intMarkTemplateTotal; i++)
            {
                switch (i)
                {
                    case 0:
                        if (!pic_Learn1.Visible)
                        {
                            pic_Learn1.Visible = true;
                            pic_Learn1.Size = new Size(91, 94);

                        }
                        if (File.Exists(strMarkPath + "0.bmp"))
                        {
                            // 2018 08 09 - CCENG: Use FileStream with FileAccess.Read to load image. pic_Learn1.Load will lock the physical image and cause the image file cannot be overwrited during learn new image.
                            FileStream fileStream = new FileStream(strMarkPath + "0.bmp", FileMode.Open, FileAccess.Read);
                            pic_Learn1.Image = Image.FromStream(fileStream);
                            fileStream.Close();
                            //}
                            //if ((m_smVisionInfo.g_intMarkTemplateIndex & 0x01) > 0)
                            //{
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

                        }
                        if (File.Exists(strMarkPath + "1.bmp"))
                        {
                            FileStream fileStream = new FileStream(strMarkPath + "1.bmp", FileMode.Open, FileAccess.Read);
                            pic_Learn2.Image = Image.FromStream(fileStream);
                            fileStream.Close();

                            //}
                            //if ((m_smVisionInfo.g_intMarkTemplateIndex & 0x02) > 0)
                            //{
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

                        }
                        if (File.Exists(strMarkPath + "2.bmp"))
                        {
                            FileStream fileStream = new FileStream(strMarkPath + "2.bmp", FileMode.Open, FileAccess.Read);
                            pic_Learn3.Image = Image.FromStream(fileStream);
                            fileStream.Close();
                            //}

                            //if ((m_smVisionInfo.g_intMarkTemplateIndex & 0x04) > 0)
                            //{
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
                        }
                        if (File.Exists(strMarkPath + "3.bmp"))
                        {
                            FileStream fileStream = new FileStream(strMarkPath + "3.bmp", FileMode.Open, FileAccess.Read);
                            pic_Learn4.Image = Image.FromStream(fileStream);
                            fileStream.Close();
                            //}

                            //if ((m_smVisionInfo.g_intMarkTemplateIndex & 0x08) > 0)
                            //{
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
                        }
                        if (File.Exists(strMarkPath + "4.bmp"))
                        {
                            FileStream fileStream = new FileStream(strMarkPath + "4.bmp", FileMode.Open, FileAccess.Read);
                            pic_Learn5.Image = Image.FromStream(fileStream);
                            fileStream.Close();
                            //}

                            //if ((m_smVisionInfo.g_intMarkTemplateIndex & 0x08) > 0)
                            //{
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
                            else if (intCount == 4)
                            {
                                lbl_Temp5.Location = pLbLLocation[4];
                                pic_Learn5.Location = pPicLocation[4];
                            }
                            else if (intCount == 5)
                            {
                                lbl_Temp5.Location = pLbLLocation[5];
                                pic_Learn5.Location = pPicLocation[5];
                            }
                            else if (intCount == 6)
                            {
                                lbl_Temp5.Location = pLbLLocation[6];
                                pic_Learn5.Location = pPicLocation[6];
                            }
                            else
                            {
                                lbl_Temp5.Location = pLbLLocation[7];
                                pic_Learn5.Location = pPicLocation[7];
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
                        }
                        if (File.Exists(strMarkPath + "5.bmp"))
                        {
                            FileStream fileStream = new FileStream(strMarkPath + "5.bmp", FileMode.Open, FileAccess.Read);
                            pic_Learn6.Image = Image.FromStream(fileStream);
                            fileStream.Close();
                            //}

                            //if ((m_smVisionInfo.g_intMarkTemplateIndex & 0x08) > 0)
                            //{
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
                            else if (intCount == 5)
                            {
                                lbl_Temp6.Location = pLbLLocation[5];
                                pic_Learn6.Location = pPicLocation[5];
                            }
                            else if (intCount == 6)
                            {
                                lbl_Temp6.Location = pLbLLocation[6];
                                pic_Learn6.Location = pPicLocation[6];
                            }
                            else
                            {
                                lbl_Temp6.Location = pLbLLocation[7];
                                pic_Learn6.Location = pPicLocation[7];
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
                        }
                        if (File.Exists(strMarkPath + "6.bmp"))
                        {
                            FileStream fileStream = new FileStream(strMarkPath + "6.bmp", FileMode.Open, FileAccess.Read);
                            pic_Learn7.Image = Image.FromStream(fileStream);
                            fileStream.Close();
                            //}

                            //if ((m_smVisionInfo.g_intMarkTemplateIndex & 0x08) > 0)
                            //{
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
                            else if (intCount == 6)
                            {
                                lbl_Temp7.Location = pLbLLocation[6];
                                pic_Learn7.Location = pPicLocation[6];
                            }
                            else
                            {
                                lbl_Temp7.Location = pLbLLocation[7];
                                pic_Learn7.Location = pPicLocation[7];
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
                        }
                        if (File.Exists(strMarkPath + "7.bmp"))
                        {
                            FileStream fileStream = new FileStream(strMarkPath + "7.bmp", FileMode.Open, FileAccess.Read);
                            pic_Learn8.Image = Image.FromStream(fileStream);
                            fileStream.Close();
                            //}

                            //if ((m_smVisionInfo.g_intMarkTemplateIndex & 0x08) > 0)
                            //{
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

            for (int i = m_smVisionInfo.g_intMarkTemplateTotal; i < m_smVisionInfo.g_intPocketTemplateTotal + m_smVisionInfo.g_intMarkTemplateTotal; i++)
            {
                //More than 4 template enable auto scroll
                if (i == 4)
                    pnl_template.AutoScroll = true;

                switch (intCount - m_smVisionInfo.g_intMarkTemplateTotal)
                {
                    case 0:
                        if (!pic_LearnEmpty1.Visible)
                        {
                            pic_LearnEmpty1.Visible = true;
                            pic_LearnEmpty1.Size = new Size(91, 94);

                        }
                        if (File.Exists(strEmptyPath + "0.bmp"))
                        {
                            // 2018 08 09 - CCENG: Use FileStream with FileAccess.Read to load image. pic_Learn1.Load will lock the physical image and cause the image file cannot be overwrited during learn new image.
                            FileStream fileStream = new FileStream(strEmptyPath + "0.bmp", FileMode.Open, FileAccess.Read);
                            pic_LearnEmpty1.Image = Image.FromStream(fileStream);
                            fileStream.Close();
                            //}
                            //if ((m_smVisionInfo.g_intPocketTemplateIndex & 0x01) > 0)
                            //{
                            lbl_TempEmpty1.Visible = true;
                            pic_LearnEmpty1.Visible = true;
                            lbl_TempEmpty1.BringToFront();
                            lbl_TempEmpty1.ForeColor = Color.Lime;
                            if (intCount == 0)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[0];
                                pic_LearnEmpty1.Location = pPicLocation[0];
                            }
                            else if (intCount == 1)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[1];
                                pic_LearnEmpty1.Location = pPicLocation[1];
                            }
                            else if (intCount == 2)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[2];
                                pic_LearnEmpty1.Location = pPicLocation[2];
                            }
                            else if (intCount == 3)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[3];
                                pic_LearnEmpty1.Location = pPicLocation[3];
                            }
                            else if (intCount == 4)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[4];
                                pic_LearnEmpty1.Location = pPicLocation[4];
                            }
                            else if (intCount == 5)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[5];
                                pic_LearnEmpty1.Location = pPicLocation[5];
                            }
                            else if (intCount == 6)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[6];
                                pic_LearnEmpty1.Location = pPicLocation[6];
                            }
                            else if (intCount == 7)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[7];
                                pic_LearnEmpty1.Location = pPicLocation[7];
                            }
                            else if (intCount == 8)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[8];
                                pic_LearnEmpty1.Location = pPicLocation[8];
                            }
                            else if (intCount == 9)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[9];
                                pic_LearnEmpty1.Location = pPicLocation[9];
                            }
                            else if (intCount == 10)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[10];
                                pic_LearnEmpty1.Location = pPicLocation[10];
                            }
                            else
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[11];
                                pic_LearnEmpty1.Location = pPicLocation[11];
                            }
                            intCount++;
                        }
                        else
                        {
                            lbl_TempEmpty1.Visible = false;
                            pic_LearnEmpty1.Visible = false;
                        }
                        break;
                    case 1:
                        if (!pic_LearnEmpty2.Visible)
                        {
                            pic_LearnEmpty2.Visible = true;

                        }
                        if (File.Exists(strEmptyPath + "1.bmp"))
                        {
                            FileStream fileStream = new FileStream(strEmptyPath + "1.bmp", FileMode.Open, FileAccess.Read);
                            pic_LearnEmpty2.Image = Image.FromStream(fileStream);
                            fileStream.Close();

                            //}
                            //if ((m_smVisionInfo.g_intPocketTemplateIndex & 0x02) > 0)
                            //{
                            lbl_TempEmpty2.Visible = true;
                            pic_LearnEmpty2.Visible = true;
                            lbl_TempEmpty2.BringToFront();
                            lbl_TempEmpty2.ForeColor = Color.Lime;
                            if (intCount == 0)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[0];
                                pic_LearnEmpty2.Location = pPicLocation[0];
                            }
                            else if (intCount == 1)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[1];
                                pic_LearnEmpty2.Location = pPicLocation[1];
                            }
                            else if (intCount == 2)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[2];
                                pic_LearnEmpty2.Location = pPicLocation[2];
                            }
                            else if (intCount == 3)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[3];
                                pic_LearnEmpty2.Location = pPicLocation[3];
                            }
                            else if (intCount == 4)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[4];
                                pic_LearnEmpty2.Location = pPicLocation[4];
                            }
                            else if (intCount == 5)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[5];
                                pic_LearnEmpty2.Location = pPicLocation[5];
                            }
                            else if (intCount == 6)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[6];
                                pic_LearnEmpty2.Location = pPicLocation[6];
                            }
                            else if (intCount == 7)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[7];
                                pic_LearnEmpty2.Location = pPicLocation[7];
                            }
                            else if (intCount == 8)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[8];
                                pic_LearnEmpty2.Location = pPicLocation[8];
                            }
                            else if (intCount == 9)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[9];
                                pic_LearnEmpty2.Location = pPicLocation[9];
                            }
                            else if (intCount == 10)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[10];
                                pic_LearnEmpty2.Location = pPicLocation[10];
                            }
                            else
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[11];
                                pic_LearnEmpty2.Location = pPicLocation[11];
                            }
                            intCount++;
                        }
                        else
                        {
                            lbl_TempEmpty2.Visible = false;
                            pic_LearnEmpty2.Visible = false;
                        }
                        break;
                    case 2:
                        if (!pic_LearnEmpty3.Visible)
                        {
                            pic_LearnEmpty3.Visible = true;

                        }
                        if (File.Exists(strEmptyPath + "2.bmp"))
                        {
                            FileStream fileStream = new FileStream(strEmptyPath + "2.bmp", FileMode.Open, FileAccess.Read);
                            pic_LearnEmpty3.Image = Image.FromStream(fileStream);
                            fileStream.Close();
                            //}

                            //if ((m_smVisionInfo.g_intPocketTemplateIndex & 0x04) > 0)
                            //{
                            lbl_TempEmpty3.Visible = true;
                            pic_LearnEmpty3.Visible = true;
                            lbl_TempEmpty3.BringToFront();
                            lbl_TempEmpty3.ForeColor = Color.Lime;
                            if (intCount == 0)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[0];
                                pic_LearnEmpty3.Location = pPicLocation[0];
                            }
                            else if (intCount == 1)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[1];
                                pic_LearnEmpty3.Location = pPicLocation[1];
                            }
                            else if (intCount == 2)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[2];
                                pic_LearnEmpty3.Location = pPicLocation[2];
                            }
                            else if (intCount == 3)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[3];
                                pic_LearnEmpty3.Location = pPicLocation[3];
                            }
                            else if (intCount == 4)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[4];
                                pic_LearnEmpty3.Location = pPicLocation[4];
                            }
                            else if (intCount == 5)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[5];
                                pic_LearnEmpty3.Location = pPicLocation[5];
                            }
                            else if (intCount == 6)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[6];
                                pic_LearnEmpty3.Location = pPicLocation[6];
                            }
                            else if (intCount == 7)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[7];
                                pic_LearnEmpty3.Location = pPicLocation[7];
                            }
                            else if (intCount == 8)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[8];
                                pic_LearnEmpty3.Location = pPicLocation[8];
                            }
                            else if (intCount == 9)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[9];
                                pic_LearnEmpty3.Location = pPicLocation[9];
                            }
                            else if (intCount == 10)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[10];
                                pic_LearnEmpty3.Location = pPicLocation[10];
                            }
                            else
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[11];
                                pic_LearnEmpty3.Location = pPicLocation[11];
                            }
                            intCount++;
                        }
                        else
                        {
                            lbl_TempEmpty3.Visible = false;
                            pic_LearnEmpty3.Visible = false;
                        }
                        break;
                    case 3:
                        if (!pic_LearnEmpty4.Visible)
                        {
                            pic_LearnEmpty4.Visible = true;
                        }
                        if (File.Exists(strEmptyPath + "3.bmp"))
                        {
                            FileStream fileStream = new FileStream(strEmptyPath + "3.bmp", FileMode.Open, FileAccess.Read);
                            pic_LearnEmpty4.Image = Image.FromStream(fileStream);
                            fileStream.Close();
                            //}

                            //if ((m_smVisionInfo.g_intPocketTemplateIndex & 0x08) > 0)
                            //{
                            lbl_TempEmpty4.Visible = true;
                            pic_LearnEmpty4.Visible = true;
                            lbl_TempEmpty4.BringToFront();
                            lbl_TempEmpty4.ForeColor = Color.Lime;

                            if (intCount == 0)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[0];
                                pic_LearnEmpty4.Location = pPicLocation[0];
                            }
                            else if (intCount == 1)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[1];
                                pic_LearnEmpty4.Location = pPicLocation[1];
                            }
                            else if (intCount == 2)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[2];
                                pic_LearnEmpty4.Location = pPicLocation[2];
                            }
                            else if (intCount == 3)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[3];
                                pic_LearnEmpty4.Location = pPicLocation[3];
                            }
                            else if (intCount == 4)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[4];
                                pic_LearnEmpty4.Location = pPicLocation[4];
                            }
                            else if (intCount == 5)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[5];
                                pic_LearnEmpty4.Location = pPicLocation[5];
                            }
                            else if (intCount == 6)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[6];
                                pic_LearnEmpty4.Location = pPicLocation[6];
                            }
                           else if (intCount == 7)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[7];
                                pic_LearnEmpty4.Location = pPicLocation[7];
                            }
                            else if (intCount == 8)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[8];
                                pic_LearnEmpty4.Location = pPicLocation[8];
                            }
                            else if (intCount == 9)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[9];
                                pic_LearnEmpty4.Location = pPicLocation[9];
                            }
                            else if (intCount == 10)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[10];
                                pic_LearnEmpty4.Location = pPicLocation[10];
                            }
                            else
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[11];
                                pic_LearnEmpty4.Location = pPicLocation[11];
                            }
                            intCount++;
                        }
                        else
                        {
                            lbl_TempEmpty4.Visible = false;
                            pic_LearnEmpty4.Visible = false;
                        }
                        break;
                    case 4:
                        if (!pic_LearnEmpty1.Visible)
                        {
                            pic_LearnEmpty1.Visible = true;

                        }
                        if (File.Exists(strEmptyPath + "0.bmp"))
                        {
                            FileStream fileStream = new FileStream(strEmptyPath + "0.bmp", FileMode.Open, FileAccess.Read);
                            pic_LearnEmpty1.Image = Image.FromStream(fileStream);
                            fileStream.Close();
                            //}

                            //if ((m_smVisionInfo.g_intPocketTemplateIndex & 0x01) > 0)
                            //{
                            lbl_TempEmpty1.Visible = true;
                            pic_LearnEmpty1.Visible = true;
                            lbl_TempEmpty1.BringToFront();
                            lbl_TempEmpty1.ForeColor = Color.Lime;

                            if (intCount == 0)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[0];
                                pic_LearnEmpty1.Location = pPicLocation[0];
                            }
                            else if (intCount == 1)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[1];
                                pic_LearnEmpty1.Location = pPicLocation[1];
                            }
                            else if (intCount == 2)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[2];
                                pic_LearnEmpty1.Location = pPicLocation[2];
                            }
                            else if (intCount == 3)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[3];
                                pic_LearnEmpty1.Location = pPicLocation[3];
                            }
                            else if (intCount == 4)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[4];
                                pic_LearnEmpty1.Location = pPicLocation[4];
                            }
                            else if (intCount == 5)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[5];
                                pic_LearnEmpty1.Location = pPicLocation[5];
                            }
                            else if (intCount == 6)
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[6];
                                pic_LearnEmpty1.Location = pPicLocation[6];
                            }
                            else
                            {
                                lbl_TempEmpty1.Location = pLbLLocation[7];
                                pic_LearnEmpty1.Location = pPicLocation[7];
                            }
                            intCount++;
                        }
                        else
                        {
                            lbl_TempEmpty1.Visible = false;
                            pic_LearnEmpty1.Visible = false;
                        }
                        break;
                    case 5:
                        if (!pic_LearnEmpty2.Visible)
                        {
                            pic_LearnEmpty2.Visible = true;

                        }
                        if (File.Exists(strEmptyPath + "1.bmp"))
                        {
                            FileStream fileStream = new FileStream(strEmptyPath + "1.bmp", FileMode.Open, FileAccess.Read);
                            pic_LearnEmpty2.Image = Image.FromStream(fileStream);
                            fileStream.Close();
                            //}

                            //if ((m_smVisionInfo.g_intPocketTemplateIndex & 0x02) > 0)
                            //{
                            lbl_TempEmpty2.Visible = true;
                            pic_LearnEmpty2.Visible = true;
                            lbl_TempEmpty2.BringToFront();
                            lbl_TempEmpty2.ForeColor = Color.Lime;

                            if (intCount == 0)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[0];
                                pic_LearnEmpty2.Location = pPicLocation[0];
                            }
                            else if (intCount == 1)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[1];
                                pic_LearnEmpty2.Location = pPicLocation[1];
                            }
                            else if (intCount == 2)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[2];
                                pic_LearnEmpty2.Location = pPicLocation[2];
                            }
                            else if (intCount == 3)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[3];
                                pic_LearnEmpty2.Location = pPicLocation[3];
                            }
                            else if (intCount == 4)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[4];
                                pic_LearnEmpty2.Location = pPicLocation[4];
                            }
                            else if (intCount == 5)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[5];
                                pic_LearnEmpty2.Location = pPicLocation[5];
                            }
                            else if (intCount == 6)
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[6];
                                pic_LearnEmpty2.Location = pPicLocation[6];
                            }
                            else
                            {
                                lbl_TempEmpty2.Location = pLbLLocation[7];
                                pic_LearnEmpty2.Location = pPicLocation[7];
                            }
                            intCount++;
                        }
                        else
                        {
                            lbl_TempEmpty2.Visible = false;
                            pic_LearnEmpty2.Visible = false;
                        }
                        break;
                    case 6:
                        if (!pic_LearnEmpty3.Visible)
                        {
                            pic_LearnEmpty3.Visible = true;

                        }
                        if (File.Exists(strEmptyPath + "2.bmp"))
                        {
                            FileStream fileStream = new FileStream(strEmptyPath + "2.bmp", FileMode.Open, FileAccess.Read);
                            pic_LearnEmpty3.Image = Image.FromStream(fileStream);
                            fileStream.Close();
                            //}

                            //if ((m_smVisionInfo.g_intPocketTemplateIndex & 0x04) > 0)
                            //{
                            lbl_TempEmpty3.Visible = true;
                            pic_LearnEmpty3.Visible = true;
                            lbl_TempEmpty3.BringToFront();
                            lbl_TempEmpty3.ForeColor = Color.Lime;

                            if (intCount == 0)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[0];
                                pic_LearnEmpty3.Location = pPicLocation[0];
                            }
                            else if (intCount == 1)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[1];
                                pic_LearnEmpty3.Location = pPicLocation[1];
                            }
                            else if (intCount == 2)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[2];
                                pic_LearnEmpty3.Location = pPicLocation[2];
                            }
                            else if (intCount == 3)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[3];
                                pic_LearnEmpty3.Location = pPicLocation[3];
                            }
                            else if (intCount == 4)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[4];
                                pic_LearnEmpty3.Location = pPicLocation[4];
                            }
                            else if (intCount == 5)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[5];
                                pic_LearnEmpty3.Location = pPicLocation[5];
                            }
                            else if (intCount == 6)
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[6];
                                pic_LearnEmpty3.Location = pPicLocation[6];
                            }
                            else
                            {
                                lbl_TempEmpty3.Location = pLbLLocation[7];
                                pic_LearnEmpty3.Location = pPicLocation[7];
                            }
                            intCount++;
                        }
                        else
                        {
                            lbl_TempEmpty3.Visible = false;
                            pic_LearnEmpty3.Visible = false;
                        }
                        break;
                    case 7:
                        if (!pic_LearnEmpty4.Visible)
                        {
                            pic_LearnEmpty4.Visible = true;
                        }
                        if (File.Exists(strEmptyPath + "3.bmp"))
                        {
                            FileStream fileStream = new FileStream(strEmptyPath + "3.bmp", FileMode.Open, FileAccess.Read);
                            pic_LearnEmpty4.Image = Image.FromStream(fileStream);
                            fileStream.Close();
                            //}

                            //if ((m_smVisionInfo.g_intTemplateMask & 0x08) > 0)
                            //{
                            lbl_TempEmpty4.Visible = true;
                            pic_LearnEmpty4.Visible = true;
                            lbl_TempEmpty4.BringToFront();
                            lbl_TempEmpty4.ForeColor = Color.Lime;

                            if (intCount == 0)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[0];
                                pic_LearnEmpty4.Location = pPicLocation[0];
                            }
                            else if (intCount == 1)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[1];
                                pic_LearnEmpty4.Location = pPicLocation[1];
                            }
                            else if (intCount == 2)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[2];
                                pic_LearnEmpty4.Location = pPicLocation[2];
                            }
                            else if (intCount == 3)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[3];
                                pic_LearnEmpty4.Location = pPicLocation[3];
                            }
                            else if (intCount == 4)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[4];
                                pic_LearnEmpty4.Location = pPicLocation[4];
                            }
                            else if (intCount == 5)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[5];
                                pic_LearnEmpty4.Location = pPicLocation[5];
                            }
                            else if (intCount == 6)
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[6];
                                pic_LearnEmpty4.Location = pPicLocation[6];
                            }
                            else
                            {
                                lbl_TempEmpty4.Location = pLbLLocation[7];
                                pic_LearnEmpty4.Location = pPicLocation[7];
                            }
                            intCount++;
                        }
                        else
                        {
                            lbl_TempEmpty4.Visible = false;
                            pic_LearnEmpty4.Visible = false;
                        }
                        break;
                }
            }
            //}
            //else
            //{
            //    pnl_template.AutoScroll = false;
            //    pic_Learn1.Visible = true;
            //    pic_Learn1.Size = new Size(91, 94);
            //    if (File.Exists(strMarkPath + "0.bmp"))
            //    {
            //        FileStream fileStream = new FileStream(strMarkPath + "0.bmp", FileMode.Open, FileAccess.Read);
            //        pic_Learn1.Image = Image.FromStream(fileStream);
            //        fileStream.Close();

            //        //pic_Learn1.Load(strPath + "0.bmp");   // 2018 08 09 - CCENG: Use FileStream with FileAccess.Read to load image. pic_Learn1.Load will lock the physical image and cause the image file cannot be overwrited during learn new image.
            //    }

            //    pic_Learn2.Visible = true;
            //    pic_Learn2.Size = new Size(91, 94);
            //    if (File.Exists(strEmptyPath + "0.bmp"))
            //    {
            //        FileStream fileStream = new FileStream(strEmptyPath + "0.bmp", FileMode.Open, FileAccess.Read);
            //        pic_Learn2.Image = Image.FromStream(fileStream);
            //        fileStream.Close();

            //        //pic_Learn1.Load(strPath + "0.bmp");   // 2018 08 09 - CCENG: Use FileStream with FileAccess.Read to load image. pic_Learn1.Load will lock the physical image and cause the image file cannot be overwrited during learn new image.
            //    }
            //}

        }

        public void CustomizeGUI()
        {
            if ((m_smCustomizeInfo.g_intWantSeal & (0x01 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                //lbl_Seal.Visible = false;
                //lbl_SealFailCount.Visible = false;
                pnl_Seal.Visible = false;
                pnl_SealDistance.Visible = false;
                pnl_SealOverHeat.Visible = false;
                pnl_UnitPresence.Visible = false;
                pnl_Orientation.Visible = false;
                pnl_BrokenArea.Visible = false;
                pnl_BrokenGap.Visible = false;
                pnl_SprocketHole.Visible = false;
                pnl_SprocketHoleDiameter.Visible = false;
                pnl_SprocketHoleDefect.Visible = false;
                pnl_SprocketHoleBroken.Visible = false;
                pnl_SprocketHoleRoundness.Visible = false;
                pnl_SealEdgeStraightness.Visible = false;
            }
            else
            {
                if (m_smVisionInfo.g_objSeal != null)
                {
                    if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x01) == 0)
                        pnl_Seal.Visible = false;
                    else
                        pnl_Seal.Visible = true;

                    if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x02) == 0)
                        pnl_BrokenArea.Visible = false;
                    else
                        pnl_BrokenArea.Visible = true;

                    if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x08) == 0)
                        pnl_SealDistance.Visible = false;
                    else
                        pnl_SealDistance.Visible = true;

                    if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x10) == 0)
                        pnl_SealOverHeat.Visible = false;
                    else
                        pnl_SealOverHeat.Visible = true;

                    if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x20) == 0)
                        pnl_BrokenGap .Visible = false;
                    else
                        pnl_BrokenGap.Visible = true;

                    if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x40) == 0)
                        pnl_UnitPresence.Visible = false;
                    else
                        pnl_UnitPresence.Visible = true;

                    if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x80) == 0)
                        pnl_Orientation.Visible = false;
                    else
                        pnl_Orientation.Visible = true;

                    if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x100) == 0) || m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole)
                        pnl_SprocketHole.Visible = false;
                    else
                        pnl_SprocketHole.Visible = true;

                    if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x200) == 0) || m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
                        pnl_SprocketHoleDiameter.Visible = false;
                    else
                        pnl_SprocketHoleDiameter.Visible = true;

                    if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x400) == 0) || m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
                        pnl_SprocketHoleDefect.Visible = false;
                    else
                        pnl_SprocketHoleDefect.Visible = true;

                    if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x800) == 0) || m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
                        pnl_SprocketHoleBroken.Visible = false;
                    else
                        pnl_SprocketHoleBroken.Visible = true;

                    if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x1000) == 0) || m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
                        pnl_SprocketHoleRoundness.Visible = false;
                    else
                        pnl_SprocketHoleRoundness.Visible = true;

                    if (((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x2000) == 0) || !m_smVisionInfo.g_objSeal.ref_blnWantCheckSealEdgeStraightness)
                        pnl_SealEdgeStraightness.Visible = false;
                    else
                        pnl_SealEdgeStraightness.Visible = true;
                }
            }

            if (m_intNoTemplateFailCount != 0)
            {
                pnl_NoTemplate.Visible = true;
            }
            else
            {
                pnl_NoTemplate.Visible = false;
            }

            if (m_intPosNoFoundFailCount != 0)
            {
                pnl_PositionNoFound.Visible = true;
            }
            else
            {
                pnl_PositionNoFound.Visible = false;
            }
        }

        public void DisableField()
        {
            string strChild1 = "ByPass";

            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild1Group(m_smVisionInfo.g_strVisionName, strChild1, m_smVisionInfo.g_intVisionNameNo))
                m_blnEnableByPass = false;
            else
                m_blnEnableByPass = true;

            //string strChild1 = "ByPass Button";
            //string strChild2 = "Seal ByPass Unit";

            ////if (m_smProductionInfo.g_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            ////    btn_ByPassUnit.Visible = false;
            ////else
            ////    btn_ByPassUnit.Visible = true;

            //if (m_smProductionInfo.g_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //    m_blnEnableByPass = false;
            //else
            //    m_blnEnableByPass = true;
        }

        public void SetMultiViewGUI(bool blnSet)
        {
            if (blnSet)
            {
                this.Size = new Size(380, 285);
                //pnl_Detail.Location = new Point(181, 128);

            }
            else
            {
                this.Size = new Size(319, 569);
                //pnl_Detail.Location = new Point(5, 231);
            }
        }

        private void EnableButton(bool blnEnable)
        {
            btn_ByPassUnit.Enabled = blnEnable;
            btn_Reset.Enabled = blnEnable;
        }

        private void ResetCount()
        {
            //m_smVisionInfo.g_intPassTotal = 0;
            //m_smVisionInfo.g_intTestedTotal = 0;
            //m_smVisionInfo.g_intSealFailureTotal = 0;
            //m_smVisionInfo.g_intPassImageCount = 0;
            //m_smVisionInfo.g_intFailImageCount = 0;
            //m_smVisionInfo.g_intLowYieldUnitCount = 0;

            //m_smVisionInfo.VS_AT_UpdateQuantity = true;

            lbl_SealFailCount.Text = "0";
            lbl_PassCount.Text = "0";
            lbl_FailCount.Text = "0";
            lbl_TestedTotal.Text = "0";
            lbl_SealFailPercent.Text = "0.00";
            lbl_NoTemplateFailCount.Text = "0";
            lbl_NoTemplateFailPercent.Text = "0.00";

            lbl_Yield.Text = "0.00";
            lbl_Yield.BackColor = Color.White;
        }


        private void UpdateInfo()
        {
            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("Vision6Page UpdateInfo 1 > g_strResult =" + m_smVisionInfo.g_strResult);
            }

            m_strResult = m_smVisionInfo.g_strResult;
            if (m_strResult != m_strResultPrev)
            {
                switch (m_strResult)
                {
                    case "Pass":
                        if (m_smVisionInfo.g_blnTrackSealOption)
                        {
                            STTrackLog.WriteLine("Vision6Page UpdateInfo 1.1 > g_strResult =" + m_strResult);
                        }

                        lbl_ResultStatus1.BackColor = Color.Lime;
                        lbl_ResultStatus1.Text = m_strResult;
                        break;
                    case "Fail":
                    case "-90":
                    case "90":
                    case "180":
                        if (m_smVisionInfo.g_blnTrackSealOption)
                        {
                            STTrackLog.WriteLine("Vision6Page UpdateInfo 1.2 > g_strResult =" + m_strResult);
                        }
                        lbl_ResultStatus1.BackColor = Color.Red;
                        lbl_ResultStatus1.Text = m_strResult;
                        break;
                        lbl_ResultStatus1.BackColor = Color.Red;
                        lbl_ResultStatus1.Text = m_strResult;
                        break;
                    case "Empty":
                        if (m_smVisionInfo.g_blnTrackSealOption)
                        {
                            STTrackLog.WriteLine("Vision6Page UpdateInfo 1.3 > g_strResult =" + m_strResult);
                        }
                        lbl_ResultStatus1.BackColor = Color.Lime;
                        lbl_ResultStatus1.Text = m_strResult;
                        break;
                    case "NoEmpty":
                        if (m_smVisionInfo.g_blnTrackSealOption)
                        {
                            STTrackLog.WriteLine("Vision6Page UpdateInfo 1.4 > g_strResult =" + m_strResult);
                        }
                        lbl_ResultStatus1.BackColor = Color.Red;
                        lbl_ResultStatus1.Text = m_strResult;
                        break;
                    case "----":
                        if (m_smVisionInfo.g_blnTrackSealOption)
                        {
                            STTrackLog.WriteLine("Vision6Page UpdateInfo 1.5 > g_strResult =" + m_strResult);
                        }
                        lbl_ResultStatus1.BackColor = SystemColors.InactiveCaptionText;
                        lbl_ResultStatus1.Text = "----";
                        break;
                }

                m_strResultPrev = m_strResult;
            }

            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("Vision6Page UpdateInfo 2 > g_strResult =" + m_strResult);
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

            // ------------ Seal Width ----------------------------------------------------
            m_intSealFailCount = m_smVisionInfo.g_intSealFailureTotal;

            if (m_intTestedTotal != 0)
            {
                float fSealFailPercent = (m_intSealFailCount / (float)m_intTestedTotal) * 100;
                if (fSealFailPercent > 100)
                    fSealFailPercent = 100;
                lbl_SealFailPercent.Text = fSealFailPercent.ToString("f2");
            }
            else
                lbl_SealFailPercent.Text = "0.00";

            if (m_intSealFailCount != m_intSealFailCountPrev)
            {
                lbl_SealFailCount.BackColor = Color.Red;
                lbl_SealFailPercent.BackColor = Color.Red;
                lbl_SealFailCount.Text = m_intSealFailCount.ToString();
                m_intSealFailCountPrev = m_intSealFailCount;
            }
            else
            {
                lbl_SealFailCount.BackColor = Color.White;
                lbl_SealFailPercent.BackColor = Color.White;
            }

            // ------------ Seal Distance ----------------------------------------------------
            m_intSealDistanceFailCount = m_smVisionInfo.g_intSealDistanceFailureTotal;

            if (m_intTestedTotal != 0)
            {
                float fSealFailPercent = (m_intSealDistanceFailCount / (float)m_intTestedTotal) * 100;
                if (fSealFailPercent > 100)
                    fSealFailPercent = 100;
                lbl_SealDistanceFailPercent.Text = fSealFailPercent.ToString("f2");
            }
            else
                lbl_SealFailPercent.Text = "0.00";

            if (m_intSealDistanceFailCount != m_intSealDistanceFailCountPrev)
            {
                lbl_SealDistanceFailCount.BackColor = Color.Red;
                lbl_SealDistanceFailPercent.BackColor = Color.Red;
                lbl_SealDistanceFailCount.Text = m_intSealDistanceFailCount.ToString();
                m_intSealDistanceFailCountPrev = m_intSealDistanceFailCount;
            }
            else
            {
                lbl_SealDistanceFailCount.BackColor = Color.White;
                lbl_SealDistanceFailPercent.BackColor = Color.White;
            }

            // ------------ Seal Overheat ----------------------------------------------------
            m_intSealOverHeatFailCount = m_smVisionInfo.g_intSealOverHeatFailureTotal;

            if (m_intTestedTotal != 0)
            {
                float fSealOverHeatFailPercent = (m_intSealOverHeatFailCount / (float)m_intTestedTotal) * 100;
                if (fSealOverHeatFailPercent > 100)
                    fSealOverHeatFailPercent = 100;
                lbl_OverHeatFailPercent.Text = fSealOverHeatFailPercent.ToString("f2");
            }
            else
                lbl_OverHeatFailPercent.Text = "0.00";

            if (m_intSealOverHeatFailCount != m_intSealOverHeatFailCountPrev)
            {
                lbl_OverHeatFailCount.BackColor = Color.Red;
                lbl_OverHeatFailPercent.BackColor = Color.Red;
                lbl_OverHeatFailCount.Text = m_intSealOverHeatFailCount.ToString();
                m_intSealOverHeatFailCountPrev = m_intSealOverHeatFailCount;
            }
            else
            {
                lbl_OverHeatFailCount.BackColor = Color.White;
                lbl_OverHeatFailPercent.BackColor = Color.White;
            }

            // ------------ Seal Unit Presence ----------------------------------------------------
            m_intUnitPresenceFailCount = m_smVisionInfo.g_intCheckPresenceFailureTotal;

            if (m_intTestedTotal != 0)
            {
                float fUnitPresenceFailPercent = (m_intUnitPresenceFailCount / (float)m_intTestedTotal) * 100;
                if (fUnitPresenceFailPercent > 100)
                    fUnitPresenceFailPercent = 100;
                lbl_UnitPresenceFailPercent.Text = fUnitPresenceFailPercent.ToString("f2");
            }
            else
                lbl_UnitPresenceFailPercent.Text = "0.00";

            if (m_intUnitPresenceFailCount != m_intUnitPresenceFailCountPrev)
            {
                lbl_UnitPresenceFailCount.BackColor = Color.Red;
                lbl_UnitPresenceFailPercent.BackColor = Color.Red;
                lbl_UnitPresenceFailCount.Text = m_intUnitPresenceFailCount.ToString();
                m_intUnitPresenceFailCountPrev = m_intUnitPresenceFailCount;
            }
            else
            {
                lbl_UnitPresenceFailCount.BackColor = Color.White;
                lbl_UnitPresenceFailPercent.BackColor = Color.White;
            }

            // ------------ Seal Unit Orientation ----------------------------------------------------
            m_intUnitOrientationFailCount = m_smVisionInfo.g_intOrientFailureTotal;

            if (m_intTestedTotal != 0)
            {
                float fSealFailPercent = (m_intUnitOrientationFailCount / (float)m_intTestedTotal) * 100;
                if (fSealFailPercent > 100)
                    fSealFailPercent = 100;
                lbl_OrientationFailPercent.Text = fSealFailPercent.ToString("f2");
            }
            else
                lbl_OrientationFailPercent.Text = "0.00";

            if (m_intUnitOrientationFailCount != m_intUnitOrientationFailCountPrev)
            {
                lbl_OrientationFailCount.BackColor = Color.Red;
                lbl_OrientationFailPercent.BackColor = Color.Red;
                lbl_OrientationFailCount.Text = m_intUnitOrientationFailCount.ToString();
                m_intUnitOrientationFailCountPrev = m_intUnitOrientationFailCount;
            }
            else
            {
                lbl_OrientationFailCount.BackColor = Color.White;
                lbl_OrientationFailPercent.BackColor = Color.White;
            }

            // ------------ Seal Broken Area ----------------------------------------------------
            m_intBrokenAreaFailCount = m_smVisionInfo.g_intSealBrokenAreaFailureTotal;

            if (m_intTestedTotal != 0)
            {
                float fSealFailPercent = (m_intBrokenAreaFailCount / (float)m_intTestedTotal) * 100;
                if (fSealFailPercent > 100)
                    fSealFailPercent = 100;
                lbl_BrokenAreaFailPercent.Text = fSealFailPercent.ToString("f2");
            }
            else
                lbl_BrokenAreaFailPercent.Text = "0.00";

            if (m_intBrokenAreaFailCount != m_intBrokenAreaFailCountPrev)
            {
                lbl_BrokenAreaFailCount.BackColor = Color.Red;
                lbl_BrokenAreaFailPercent.BackColor = Color.Red;
                lbl_BrokenAreaFailCount.Text = m_intBrokenAreaFailCount.ToString();
                m_intBrokenAreaFailCountPrev = m_intBrokenAreaFailCount;
            }
            else
            {
                lbl_BrokenAreaFailCount.BackColor = Color.White;
                lbl_BrokenAreaFailPercent.BackColor = Color.White;
            }

            // ------------ Seal Broken Gap ----------------------------------------------------
            m_intBrokenGapFailCount = m_smVisionInfo.g_intSealBrokenGapFailureTotal;

            if (m_intTestedTotal != 0)
            {
                float fSealFailPercent = (m_intBrokenGapFailCount / (float)m_intTestedTotal) * 100;
                if (fSealFailPercent > 100)
                    fSealFailPercent = 100;
                lbl_BrokenGapFailPercent.Text = fSealFailPercent.ToString("f2");
            }
            else
                lbl_BrokenGapFailPercent.Text = "0.00";

            if (m_intBrokenGapFailCount != m_intBrokenGapFailCountPrev)
            {
                lbl_BrokenGapFailCount.BackColor = Color.Red;
                lbl_BrokenGapFailPercent.BackColor = Color.Red;
                lbl_BrokenGapFailCount.Text = m_intBrokenGapFailCount.ToString();
                m_intBrokenGapFailCountPrev = m_intBrokenGapFailCount;
            }
            else
            {
                lbl_BrokenGapFailCount.BackColor = Color.White;
                lbl_BrokenGapFailPercent.BackColor = Color.White;
            }

            // ------------ Seal Sprocket Hole Distance----------------------------------------------------
            m_intSprocketHoleFailCount = m_smVisionInfo.g_intSealSprocketHoleFailureTotal;

            if (m_intTestedTotal != 0)
            {
                float fSealFailPercent = (m_intSprocketHoleFailCount / (float)m_intTestedTotal) * 100;
                if (fSealFailPercent > 100)
                    fSealFailPercent = 100;
                lbl_SprocketHoleFailPercent.Text = fSealFailPercent.ToString("f2");
            }
            else
                lbl_SprocketHoleFailPercent.Text = "0.00";

            if (m_intSprocketHoleFailCount != m_intSprocketHoleFailCountPrev)
            {
                lbl_SprocketHoleFailCount.BackColor = Color.Red;
                lbl_SprocketHoleFailPercent.BackColor = Color.Red;
                lbl_SprocketHoleFailCount.Text = m_intSprocketHoleFailCount.ToString();
                m_intSprocketHoleFailCountPrev = m_intSprocketHoleFailCount;
            }
            else
            {
                lbl_SprocketHoleFailCount.BackColor = Color.White;
                lbl_SprocketHoleFailPercent.BackColor = Color.White;
            }

            // ------------ Seal Sprocket Hole Diameter----------------------------------------------------
            m_intSprocketHoleDiameterFailCount = m_smVisionInfo.g_intSealSprocketHoleDiameterFailureTotal;

            if (m_intTestedTotal != 0)
            {
                float fSealFailPercent = (m_intSprocketHoleDiameterFailCount / (float)m_intTestedTotal) * 100;
                if (fSealFailPercent > 100)
                    fSealFailPercent = 100;
                lbl_SprocketHoleDiameterFailPercent.Text = fSealFailPercent.ToString("f2");
            }
            else
                lbl_SprocketHoleDiameterFailPercent.Text = "0.00";

            if (m_intSprocketHoleDiameterFailCount != m_intSprocketHoleDiameterFailCountPrev)
            {
                lbl_SprocketHoleDiameterFailCount.BackColor = Color.Red;
                lbl_SprocketHoleDiameterFailPercent.BackColor = Color.Red;
                lbl_SprocketHoleDiameterFailCount.Text = m_intSprocketHoleDiameterFailCount.ToString();
                m_intSprocketHoleDiameterFailCountPrev = m_intSprocketHoleDiameterFailCount;
            }
            else
            {
                lbl_SprocketHoleDiameterFailCount.BackColor = Color.White;
                lbl_SprocketHoleDiameterFailPercent.BackColor = Color.White;
            }

            // ------------ Seal Sprocket Hole Defect----------------------------------------------------
            m_intSprocketHoleDefectFailCount = m_smVisionInfo.g_intSealSprocketHoleDefectFailureTotal;

            if (m_intTestedTotal != 0)
            {
                float fSealFailPercent = (m_intSprocketHoleDefectFailCount / (float)m_intTestedTotal) * 100;
                if (fSealFailPercent > 100)
                    fSealFailPercent = 100;
                lbl_SprocketHoleDefectFailPercent.Text = fSealFailPercent.ToString("f2");
            }
            else
                lbl_SprocketHoleDefectFailPercent.Text = "0.00";

            if (m_intSprocketHoleDefectFailCount != m_intSprocketHoleDefectFailCountPrev)
            {
                lbl_SprocketHoleDefectFailCount.BackColor = Color.Red;
                lbl_SprocketHoleDefectFailPercent.BackColor = Color.Red;
                lbl_SprocketHoleDefectFailCount.Text = m_intSprocketHoleDefectFailCount.ToString();
                m_intSprocketHoleDefectFailCountPrev = m_intSprocketHoleDefectFailCount;
            }
            else
            {
                lbl_SprocketHoleDefectFailCount.BackColor = Color.White;
                lbl_SprocketHoleDefectFailPercent.BackColor = Color.White;
            }

            // ------------ Seal Sprocket Hole Broken----------------------------------------------------
            m_intSprocketHoleBrokenFailCount = m_smVisionInfo.g_intSealSprocketHoleBrokenFailureTotal;

            if (m_intTestedTotal != 0)
            {
                float fSealFailPercent = (m_intSprocketHoleBrokenFailCount / (float)m_intTestedTotal) * 100;
                if (fSealFailPercent > 100)
                    fSealFailPercent = 100;
                lbl_SprocketHoleBrokenFailPercent.Text = fSealFailPercent.ToString("f2");
            }
            else
                lbl_SprocketHoleBrokenFailPercent.Text = "0.00";

            if (m_intSprocketHoleBrokenFailCount != m_intSprocketHoleBrokenFailCountPrev)
            {
                lbl_SprocketHoleBrokenFailCount.BackColor = Color.Red;
                lbl_SprocketHoleBrokenFailPercent.BackColor = Color.Red;
                lbl_SprocketHoleBrokenFailCount.Text = m_intSprocketHoleBrokenFailCount.ToString();
                m_intSprocketHoleBrokenFailCountPrev = m_intSprocketHoleBrokenFailCount;
            }
            else
            {
                lbl_SprocketHoleBrokenFailCount.BackColor = Color.White;
                lbl_SprocketHoleBrokenFailPercent.BackColor = Color.White;
            }

            // ------------ Seal Sprocket Hole Roundness----------------------------------------------------
            m_intSprocketHoleRoundnessFailCount = m_smVisionInfo.g_intSealSprocketHoleRoundnessFailureTotal;

            if (m_intTestedTotal != 0)
            {
                float fSealFailPercent = (m_intSprocketHoleRoundnessFailCount / (float)m_intTestedTotal) * 100;
                if (fSealFailPercent > 100)
                    fSealFailPercent = 100;
                lbl_SprocketHoleRoundnessFailPercent.Text = fSealFailPercent.ToString("f2");
            }
            else
                lbl_SprocketHoleRoundnessFailPercent.Text = "0.00";

            if (m_intSprocketHoleRoundnessFailCount != m_intSprocketHoleRoundnessFailCountPrev)
            {
                lbl_SprocketHoleRoundnessFailCount.BackColor = Color.Red;
                lbl_SprocketHoleRoundnessFailPercent.BackColor = Color.Red;
                lbl_SprocketHoleRoundnessFailCount.Text = m_intSprocketHoleRoundnessFailCount.ToString();
                m_intSprocketHoleRoundnessFailCountPrev = m_intSprocketHoleRoundnessFailCount;
            }
            else
            {
                lbl_SprocketHoleRoundnessFailCount.BackColor = Color.White;
                lbl_SprocketHoleRoundnessFailPercent.BackColor = Color.White;
            }

            // ------------ Seal Edge Straightness----------------------------------------------------
            m_intSealEdgeStraightnessFailCount = m_smVisionInfo.g_intSealEdgeStraightnessFailureTotal;

            if (m_intTestedTotal != 0)
            {
                float fSealFailPercent = (m_intSealEdgeStraightnessFailCount / (float)m_intTestedTotal) * 100;
                if (fSealFailPercent > 100)
                    fSealFailPercent = 100;
                lbl_SealEdgeStraightnessFailPercent.Text = fSealFailPercent.ToString("f2");
            }
            else
                lbl_SealEdgeStraightnessFailPercent.Text = "0.00";

            if (m_intSealEdgeStraightnessFailCount != m_intSealEdgeStraightnessFailCountPrev)
            {
                lbl_SealEdgeStraightnessFailCount.BackColor = Color.Red;
                lbl_SealEdgeStraightnessFailPercent.BackColor = Color.Red;
                lbl_SealEdgeStraightnessFailCount.Text = m_intSealEdgeStraightnessFailCount.ToString();
                m_intSealEdgeStraightnessFailCountPrev = m_intSealEdgeStraightnessFailCount;
            }
            else
            {
                lbl_SealEdgeStraightnessFailCount.BackColor = Color.White;
                lbl_SealEdgeStraightnessFailPercent.BackColor = Color.White;
            }

            // -------No Template  --------------------------------------------------------------------------
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

            // ------------ Position Not Found ----------------------------------------------------
            m_intPosNoFoundFailCount = m_smVisionInfo.g_intPositionFailureTotal;
            if (m_intPosNoFoundFailCount != 0)
            {
                pnl_PositionNoFound.Visible = true;
            }
            else
            {
                pnl_PositionNoFound.Visible = false;
            }

            if (m_intTestedTotal != 0)
            {
                float fPosNoFoundFailPercent = (m_intPosNoFoundFailCount / (float)m_intTestedTotal) * 100;
                if (fPosNoFoundFailPercent > 100)
                    fPosNoFoundFailPercent = 100;
                lbl_PositionNotFoundFailPercent.Text = fPosNoFoundFailPercent.ToString("f2");
            }
            else
                lbl_PositionNotFoundFailPercent.Text = "0.00";

            if (m_intPosNoFoundFailCount != m_intPosNoFoundFailCountPrev)
            {
                lbl_PositionNotFoundFailCount.BackColor = Color.Red;
                lbl_PositionNotFoundFailPercent.BackColor = Color.Red;
                lbl_PositionNotFoundFailCount.Text = m_intPosNoFoundFailCount.ToString();
                m_intPosNoFoundFailCountPrev = m_intPosNoFoundFailCount;
            }
            else
            {
                lbl_PositionNotFoundFailCount.BackColor = Color.White;
                lbl_PositionNotFoundFailPercent.BackColor = Color.White;
            }

            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("Vision6Page UpdateInfo 3 > g_strResult =" + m_strResult);
            }

            lbl_GrabDelay.Text = m_smVisionInfo.g_intCameraGrabDelay.ToString();
            lbl_GrabTime.Text = (Math.Max(0, m_smVisionInfo.g_objGrabTime.Duration - m_smVisionInfo.g_intCameraGrabDelay)).ToString("f0");
            lbl_ProcessTime.Text = (Math.Max(0, m_smVisionInfo.g_objTotalTime.Duration - m_smVisionInfo.g_objGrabTime.Duration)).ToString("f2");
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
                STTrackLog.WriteLine("btn_ByPassUnit_Click. 1");

                string strChild1 = "ByPass";
                string strChild2_other = "Other";
                string strChild2_SealDistance = "Seal Distance";
                string strChild2_SealBubble = "Seal Bubble";
                string strChild2_SealBrokenGap = "Seal Broken";
                string strChild2_SealWidth = "Seal Width";
                string strChild2_SealOverHeat = "Seal OverHeat";
                string strChild2_SealSprocketHole = "Seal Sprocket Hole Distance";
                string strChild2_SealSprocketHoleDiameter = "Seal Sprocket Hole Diameter";
                string strChild2_SealSprocketHoleDefect = "Seal Sprocket Hole Defect";
                string strChild2_SealSprocketHoleBroken = "Seal Sprocket Hole Broken";
                string strChild2_SealEdgeStraightness = "Seal Edge Straightness";
                string strChild2_SealSprocketHoleRoundness = "Seal Sprocket Hole Roundness";
                string strChild2_UnitPresent = "Check Unit Present";
                string strChild2_EmptyPocket = "Empty Pocket";
                string strChild2_UnitOrientation = "Check Unit Orientation";

                m_smVisionInfo.VM_PR_ByPassUnit = false;

                STTrackLog.WriteLine("btn_ByPassUnit_Click. 2");

                if (m_smVisionInfo.g_objSeal.ref_intSealFailMask == 0)
                {
                    STTrackLog.WriteLine("btn_ByPassUnit_Click. 3");
                    SRMMessageBox.Show("Sorry You can only bypass the fail unit.");

                    STTrackLog.WriteLine("btn_ByPassUnit_Click. 4");
                    return;
                }
                else
                {
                    STTrackLog.WriteLine("btn_ByPassUnit_Click. 5");

                    STTrackLog.WriteLine("btn_ByPassUnit_Click. 6");
                    int intUserGroup = 5;
                    LoginForm objLogin = new LoginForm(m_smProductionInfo);
                    // 2020 02 17 - JBTAN: Login as higher level to bypass alarm
                    if (objLogin.ShowDialog(this) == DialogResult.OK)
                    {
                        intUserGroup = objLogin.ref_intUserGroup;
                        STTrackLog.WriteLine("btn_ByPassUnit_Click. 7");
                    }

                    STTrackLog.WriteLine("btn_ByPassUnit_Click. 9. ref_intSealFailMask = " + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());

                    do
                    {

                        switch (m_smVisionInfo.g_objSeal.ref_intSealFailMask)
                        {
                            case 0x01:  // Fail position, position pattern no found. 
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 1");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_other, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 1a");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 1z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 1b");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 1c");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 2d");
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 1e");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;
                            //seal distance
                            case 0x02:
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 2");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_SealDistance, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 2a");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 2z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 2b");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 2c");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 2d");
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 2e");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;
                            //seal bubble
                            case 0x04:
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 4");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_SealBubble, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 4");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 4z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 4b");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 4c");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 4d");
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 4e");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;
                            //seal broken
                            case 0x08:
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 8");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_SealBrokenGap, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 8a");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 8z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 8b");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 8c");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 8d");
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 8e");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;
                            //seal width
                            case 0x10:    // 16
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 16");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_SealWidth, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 16a");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 16z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 16b");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 16c");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 16d");
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 16e");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;
                            //seal width insufficient
                            case 0x20:    //32
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 32");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_SealWidth, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 32b");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 32z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 32c");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 32d");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 32e");
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 32f");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;
                            //seal overheat
                            case 0x40:    // 64
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 64");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_SealOverHeat, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 64a");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 64z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 64b");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 64c");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 64d");
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 64e");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;
                            // Empty Pocket
                            case 0x100:
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 100");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_EmptyPocket, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 100a");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 100z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 100b");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 100c");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 100d");
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 100e");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;
                            //sprocket hole distance
                            case 0x200:   //  512
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512a");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_SealSprocketHole, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512b");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512c");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512d");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512e");
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512f");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break; 
                                //sprocket hole diameter
                            case 0x2000:   //  512
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512a");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_SealSprocketHoleDiameter, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512b");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512c");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512d");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512e");

                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);

                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512f");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;
                            //sprocket hole defect
                            case 0x4000:   //  512
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512a");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_SealSprocketHoleDefect, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512b");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512c");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512d");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512e");

                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);

                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512f");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;
                            //sprocket hole Broken
                            case 0x8000:   //  512
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512a");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_SealSprocketHoleBroken, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512b");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512c");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512d");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512e");

                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);

                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512f");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;
                            //sprocket hole Roundness
                            case 0x10000:   //  512
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512a");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_SealSprocketHoleRoundness, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512b");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512c");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512d");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512e");

                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);

                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512f");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;   
                                //Seal Edge Straightness
                            case 0x20000:   //  512
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512a");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_SealEdgeStraightness, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512b");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512c");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512d");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512e");

                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);

                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 512f");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;
                            case 0x080:
                            case 0x400:
                            case 0x480:  //1152
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 1152");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_UnitPresent, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 1152a");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 1152z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 1152b");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 1152c");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 1152d");
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 1152e");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;
                            case 0x800: // Tape Scratches
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 800");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_SealOverHeat, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 800a");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 800z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 800b");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 800c");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 800d");
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 800e");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;
                            case 0x1000:  //4096
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case 0x1000");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_UnitOrientation, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 0x1000a");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 0x1000z");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case 0x1000b");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case 0x1000c");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 0x1000d");
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case 0x1000e");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;
                            default:
                                STTrackLog.WriteLine("btn_ByPassUnit_Click. case default");
                                if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild1Group(m_smVisionInfo.g_strVisionName, strChild1, m_smVisionInfo.g_intVisionNameNo))
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case defaulta");
                                    SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");

                                    if (intUserGroup == 5)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case defaultz");
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        if (objLogin.ShowDialog(this) != DialogResult.OK)
                                        {
                                            STTrackLog.WriteLine("btn_ByPassUnit_Click. case defaultb");
                                            objLogin.Dispose();
                                            return;
                                        }
                                        else
                                            intUserGroup = objLogin.ref_intUserGroup;
                                    }
                                }
                                else
                                {
                                    STTrackLog.WriteLine("btn_ByPassUnit_Click. case defaultc");
                                    if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case defaultd");
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        m_smVisionInfo.VM_PR_ByPassUnit = true;
                                        objLogin.Dispose();
                                        return;
                                    }
                                    else
                                    {
                                        STTrackLog.WriteLine("btn_ByPassUnit_Click. case defaulte");
                                        objLogin.Dispose();
                                        return;
                                    }
                                }
                                break;
                        }

                        //check unit present
                        //if (!m_smVisionInfo.g_objSeal.ref_CheckUnitOrient && m_smVisionInfo.g_objSeal.ref_intSealFailMask == 128)
                        //{
                        //    if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_80))
                        //    {
                        //        SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");
                        //        if (objLogin.ShowDialog(this) != DialogResult.OK)
                        //        {
                        //            objLogin.Dispose();
                        //            return;
                        //        }
                        //        else
                        //            intUserGroup = objLogin.ref_intUserGroup;
                        //    }
                        //    else
                        //    {
                        //        if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        //        {
                        //            
                        //            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);
                        //            m_smVisionInfo.VM_PR_ByPassUnit = true;
                        //            objLogin.Dispose();
                        //            return;
                        //        }
                        //        else
                        //        {
                        //            objLogin.Dispose();
                        //            return;
                        //        }
                        //    }
                        //}
                        ////check unit orient
                        //else if (m_smVisionInfo.g_objSeal.ref_CheckUnitOrient && m_smVisionInfo.g_objSeal.ref_intSealFailMask == 128)
                        //{
                        //    if (intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(strChild1, strChild2_81))
                        //    {
                        //        SRMMessageBox.Show(" Sorry, You don't have privilege to by pass this unit. Please Login as higher level.");
                        //        if (objLogin.ShowDialog(this) != DialogResult.OK)
                        //        {
                        //            objLogin.Dispose();
                        //            return;
                        //        }
                        //        else
                        //            intUserGroup = objLogin.ref_intUserGroup;
                        //    }
                        //    else
                        //    {
                        //        if (SRMMessageBox.Show("Are you sure you want to by pass this unit?", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        //        {
                        //            
                        //            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Bypass", "Bypass Seal 1 time", "", "", m_smProductionInfo.g_strLotID);
                        //            m_smVisionInfo.VM_PR_ByPassUnit = true;
                        //            objLogin.Dispose();
                        //            return;
                        //        }
                        //        else
                        //        {
                        //            objLogin.Dispose();
                        //            return;
                        //        }
                        //    }
                        //}

                        System.Threading.Thread.Sleep(1);
                    } while (m_smVisionInfo.VM_PR_ByPassUnit == false);
                }
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("btn_ByPassUnit_Click Exception = " + ex.ToString());
                SRMMessageBox.Show("btn_ByPassUnit_Click Exception = " + ex.ToString());
            }
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
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("Vision6Page Call UpdateInfo A");
                }

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
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("Vision6Page Call UpdateInfo B");
                }

                UpdateLotNo();
                LoadTemplateImage();
                UpdateInfo();
                m_smVisionInfo.AT_VM_UpdateProduction = false;
            }
        }
        
    }
}