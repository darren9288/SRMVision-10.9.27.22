using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using SharedMemory;
using VisionProcessing;
using IOMode;
using Common;

namespace VisionModule
{
    public partial class Vision7Page : Form
    {
        #region Member Variables

        private int m_intMachineStatusPrev = 1;
        private int m_intUserGroup;
        // Count
        private int m_intPassCount = 0, m_intPassCountPrev = -1;
        private int m_intFailCount = 0, m_intFailCountPrev = -1;
        private int m_intTestedTotal = 0, m_intTestedTotalPrev = -1;
        private int m_intBarcodeFailCount = 0, m_inBarcodeFailCountPrev = -1;
        private int m_intNoTemplateFailCount = 0, m_intNoTemplateFailCountPrev = -1;

        private string m_strResult = "", m_strResultPrev = "";
        private string[] m_strBarcodeResult = new string[10] { "----", "----", "----", "----", "----", "----", "----", "----", "----", "----" };
        private string[] m_strBarcodeResultPrev = new string[10] { "----", "----", "----", "----", "----", "----", "----", "----", "----", "----" };
        private CustomOption m_smCustomOption;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo m_smVisionInfo;
        private UserRight m_objUserRight = new UserRight();
        #endregion
        public Vision7Page(CustomOption smCustomOption, ProductionInfo smProductionInfo, VisionInfo smVisionInfo)
        {
            InitializeComponent();
            m_smCustomOption = smCustomOption;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;

            //lbl_LotID.Text = m_smProductionInfo.g_strLotID;
            lbl_LotID.Text = m_smProductionInfo.g_arrSingleLotID[m_smVisionInfo.g_intVisionIndex];
            lbl_OperatorID.Text = m_smProductionInfo.g_strOperatorID;
            UpdateInfo();
            //ResetCounterBackColor();
            CustomizeGUI();
            LoadTemplateImage();
        }
        private void ResetCounterBackColor()
        {

            lbl_BarcodeFailCount.BackColor = Color.White;

            lbl_NoTemplateFailCount.BackColor = Color.White;

            lbl_BarcodeFailPercent.BackColor = Color.White;

            lbl_NoTemplateFailPercent.BackColor = Color.White;

        }
        public void ActivateTimer(bool blnEnable)
        {
            timer_Live.Enabled = blnEnable;
        }
        public void CustomizeGUI()
        {
            if ((m_smCustomOption.g_intWantBarcode & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                pnl_Barcode.Visible = true;
                lbl_BarcodeResult.Visible = true;
                lbl_TemplateBarcode.Visible = true;
                //if (m_smVisionInfo.g_objBarcode != null)
                //    lbl_TemplateBarcode.Text = m_smVisionInfo.g_objBarcode.ref_strTemplateCode;

                if (m_smVisionInfo.g_objBarcode != null)
                {
                    dgd_Result.Rows.Clear();
                    for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
                    {
                        dgd_Result.Rows.Add();
                        dgd_Result.Rows[i].Cells[0].Value = (i + 1).ToString();
                        dgd_Result.Rows[i].Cells[1].Value = m_smVisionInfo.g_objBarcode.ref_strTemplateCode[i];
                        dgd_Result.Rows[i].Cells[1].Style.BackColor = m_smVisionInfo.g_objBarcode.ref_Color[i];
                        dgd_Result.Rows[i].Cells[1].Style.SelectionBackColor = m_smVisionInfo.g_objBarcode.ref_Color[i];
                        dgd_Result.Rows[i].Cells[2].Value = "----";
                        dgd_Result.Rows[i].Cells[3].Value = "----";
                        m_strBarcodeResultPrev[i] = "----";
                        m_strBarcodeResult[i] = "----";
                    }

                    if (dgd_Result.RowCount == 0)
                    {
                        dgd_Result.Rows.Add();
                        dgd_Result.Rows[0].Cells[0].Value = (1).ToString();
                        dgd_Result.Rows[0].Cells[1].Value = "----";
                        m_smVisionInfo.g_objBarcode.ref_intTemplateCount = 1;
                        dgd_Result.Rows[0].Cells[1].Style.BackColor = m_smVisionInfo.g_objBarcode.ref_Color[0];
                        dgd_Result.Rows[0].Cells[1].Style.SelectionBackColor = m_smVisionInfo.g_objBarcode.ref_Color[0];
                        dgd_Result.Rows[0].Cells[2].Value = "----";
                        dgd_Result.Rows[0].Cells[3].Value = "----";
                        m_strBarcodeResultPrev[0] = "----";
                        m_strBarcodeResult[0] = "----";
                    }
                }
            }
            else
            {
                pnl_Barcode.Visible = false;
                lbl_BarcodeResult.Visible = false;
                lbl_TemplateBarcode.Visible = false;
            }

            lbl_TemplateBarcode.Size = new Size(lbl_TemplateBarcode.Size.Width, Math.Max(100, (int)Math.Round((lbl_TemplateBarcode.Text.Length / (lbl_TemplateBarcode.Size.Width / lbl_TemplateBarcode.Font.Size)) * lbl_TemplateBarcode.Font.Height)));

            lbl_PassCount.Text = "---";
            lbl_FailCount.Text = "---";
            lbl_TestedTotal.Text = "---";
            lbl_Yield.Text = "---";

            //if (m_intNoTemplateFailCount != 0)
            //{
            //    pnl_NoTemplate.Visible = true;
            //}
            //else
            //{
            //    pnl_NoTemplate.Visible = false;
            //}

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
        private void EnableButton(bool blnEnable)
        {
            btn_Reset.Enabled = blnEnable;
        }

        public void LoadTemplateImage()
        {
            //return;

            string strRecipe = m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex];
            string strPath;
            if (((m_smCustomOption.g_intWantBarcode & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
            {
                strPath = m_smProductionInfo.g_strRecipePath + strRecipe +
                            "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Barcode\\Template\\Template";

            }
            else
                return;

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

          
                pnl_template.AutoScroll = false;
                pic_Learn1.Visible = true;
                pic_Learn1.Size = new Size(150, 150);
                if (File.Exists(strPath + "0.bmp"))
                {
                    FileStream fileStream = new FileStream(strPath + "0.bmp", FileMode.Open, FileAccess.Read);
                    pic_Learn1.Image = Image.FromStream(fileStream);
                    fileStream.Close();

                    //pic_Learn1.Load(strPath + "0.bmp");   // 2018 08 09 - CCENG: Use FileStream with FileAccess.Read to load image. pic_Learn1.Load will lock the physical image and cause the image file cannot be overwrited during learn new image.
                }
            
        }
        private void ResetCount()
        {
            
            lbl_PassCount.Text = "0";
            lbl_FailCount.Text = "0";
            lbl_TestedTotal.Text = "0";
            lbl_BarcodeFailCount.Text = "0";
           
            lbl_NoTemplateFailCount.Text = "0";
           
            lbl_BarcodeFailPercent.Text = "0.00";
          
            lbl_NoTemplateFailPercent.Text = "0.00";
          
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
                        lbl_ResultStatus.BackColor = Color.Lime;
                        lbl_ResultStatus.Text = "Pass";
                        break;
                    case "Fail":
                        lbl_ResultStatus.BackColor = Color.Red;
                        lbl_ResultStatus.Text = "Fail";
                        break;
                    case "Idle":
                        lbl_ResultStatus.BackColor = Color.Yellow;
                        lbl_ResultStatus.Text = "Idle";
                        break;
                }

                m_strResultPrev = m_strResult;
            }

            //m_strBarcodeResult = m_smVisionInfo.g_objBarcode.ref_strResultCode;
            //if (m_strBarcodeResult != m_strBarcodeResultPrev)
            //{
            //    if (m_smVisionInfo.g_objBarcode.ref_blnTestDone && m_smVisionInfo.g_objBarcode.ref_blnCodeFound && m_smVisionInfo.g_objBarcode.ref_blnCodePassed)
            //    {
            //        lbl_BarcodeResult.BackColor = Color.Lime;
            //        lbl_BarcodeResult.Text = m_strBarcodeResult;
            //        m_strBarcodeResultPrev = m_strBarcodeResult;
            //    }
            //    else if (m_smVisionInfo.g_objBarcode.ref_blnTestDone && m_smVisionInfo.g_objBarcode.ref_blnCodeNotMatched)
            //    {
            //        lbl_BarcodeResult.BackColor = Color.Red;
            //        lbl_BarcodeResult.Text = m_strBarcodeResult;
            //        m_strBarcodeResultPrev = m_strBarcodeResult;
            //    }
            //    else if (m_smVisionInfo.g_objBarcode.ref_blnTestDone)
            //    {
            //        lbl_BarcodeResult.BackColor = Color.Red;
            //        lbl_BarcodeResult.Text = "----";
            //        m_strBarcodeResultPrev = m_strBarcodeResult;
            //    }
            //    else
            //    {
            //        //lbl_BarcodeResult.BackColor = Color.Yellow;
            //        //lbl_BarcodeResult.Text = "----";
            //        m_strBarcodeResultPrev = "----";
            //    }

            //}

            ////2020-08-31 ZJYEOH : Need to check the color status again because when fail then user go learn then the Prev Result will same as current result then above condition cannot enter
            //if (m_smVisionInfo.g_objBarcode.ref_blnTestDone && m_smVisionInfo.g_objBarcode.ref_blnCodeFound && m_smVisionInfo.g_objBarcode.ref_blnCodePassed)
            //{
            //    if (lbl_BarcodeResult.BackColor != Color.Lime)
            //        lbl_BarcodeResult.BackColor = Color.Lime;
            //}
            //else if (m_smVisionInfo.g_objBarcode.ref_blnTestDone && m_smVisionInfo.g_objBarcode.ref_blnCodeNotMatched)
            //{
            //    if (lbl_BarcodeResult.BackColor != Color.Red)
            //        lbl_BarcodeResult.BackColor = Color.Red;
            //}
            //else if (m_smVisionInfo.g_objBarcode.ref_blnTestDone)
            //{
            //    if (lbl_BarcodeResult.BackColor != Color.Red)
            //        lbl_BarcodeResult.BackColor = Color.Red;
            //}
            //else
            //{
            //    //if (lbl_BarcodeResult.BackColor != Color.Yellow)
            //    //    lbl_BarcodeResult.BackColor = Color.Yellow;
            //}

            for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
            {
                m_strBarcodeResult[i] = m_smVisionInfo.g_objBarcode.ref_strResultCode[i];
                if (m_strBarcodeResult[i] != m_strBarcodeResultPrev[i])
                {
                    if (m_smVisionInfo.g_objBarcode.ref_blnTestDone && m_smVisionInfo.g_objBarcode.ref_blnCodeFound[i] && m_smVisionInfo.g_objBarcode.ref_blnCodePassed[i])
                    {
                        if (m_strBarcodeResult[i] != "----")
                        {
                            if (Math.Abs(m_smVisionInfo.g_objBarcode.ref_fBarcodeAngle[i] - m_smVisionInfo.g_objBarcode.ref_fBarcodeOrientationAngle) > 45)
                            {
                                dgd_Result.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                dgd_Result.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                                dgd_Result.Rows[i].Cells[0].Style.BackColor = Color.Red;
                                dgd_Result.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                            }
                            else
                            {
                                dgd_Result.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                dgd_Result.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_Result.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                                dgd_Result.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                            }
                            dgd_Result.Rows[i].Cells[3].Value = m_smVisionInfo.g_objBarcode.ref_fBarcodeAngle[i];
                        }
                        else
                        {
                            dgd_Result.Rows[i].Cells[3].Value = "----";
                            dgd_Result.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_Result.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_Result.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                            dgd_Result.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                        }

                        //lbl_BarcodeResult.BackColor = Color.Lime;
                        //lbl_BarcodeResult.Text = m_strBarcodeResult[i];
                       
                        dgd_Result.Rows[i].Cells[2].Value = m_strBarcodeResult[i];
                        m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                    }
                    else if (m_smVisionInfo.g_objBarcode.ref_blnTestDone && m_smVisionInfo.g_objBarcode.ref_blnCodeNotMatched[i])
                    {
                        if (m_strBarcodeResult[i] != "----")
                        {
                            dgd_Result.Rows[i].Cells[3].Value = m_smVisionInfo.g_objBarcode.ref_fBarcodeAngle[i];
                        }
                        else
                        {
                            dgd_Result.Rows[i].Cells[3].Value = "----";
                        }

                        //lbl_BarcodeResult.BackColor = Color.Red;
                        //lbl_BarcodeResult.Text = m_strBarcodeResult[i];
                        dgd_Result.Rows[i].Cells[0].Style.BackColor = Color.Red;
                        dgd_Result.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                        dgd_Result.Rows[i].Cells[2].Value = m_strBarcodeResult[i];
                        m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                    }
                    else if (m_smVisionInfo.g_objBarcode.ref_blnTestDone)
                    {
                        dgd_Result.Rows[i].Cells[3].Value = "----";
                        //lbl_BarcodeResult.BackColor = Color.Red;
                        //lbl_BarcodeResult.Text = "----";
                        dgd_Result.Rows[i].Cells[0].Style.BackColor = Color.Red;
                        dgd_Result.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                        dgd_Result.Rows[i].Cells[2].Value = "----";
                        m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                    }
                    else
                    {
                        //lbl_BarcodeResult.BackColor = Color.Yellow;
                        //lbl_BarcodeResult.Text = "----";
                        m_strBarcodeResultPrev[i] = "----";
                    }

                }

                //2020-08-31 ZJYEOH : Need to check the color status again because when fail then user go learn then the Prev Result will same as current result then above condition cannot enter
                if (m_smVisionInfo.g_objBarcode.ref_blnTestDone && m_smVisionInfo.g_objBarcode.ref_blnCodeFound[i] && m_smVisionInfo.g_objBarcode.ref_blnCodePassed[i])
                {
                    if (m_strBarcodeResult[i] != "----")
                    {
                        if (Math.Abs(m_smVisionInfo.g_objBarcode.ref_fBarcodeAngle[i] - m_smVisionInfo.g_objBarcode.ref_fBarcodeOrientationAngle) > 45)
                        {
                            dgd_Result.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_Result.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_Result.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_Result.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        }
                        dgd_Result.Rows[i].Cells[3].Value = m_smVisionInfo.g_objBarcode.ref_fBarcodeAngle[i];
                    }
                    else
                    {
                        dgd_Result.Rows[i].Cells[3].Value = "----";
                        if (dgd_Result.Rows[i].Cells[3].Style.BackColor != Color.Lime)
                            dgd_Result.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        if (dgd_Result.Rows[i].Cells[3].Style.SelectionBackColor != Color.Lime)
                            dgd_Result.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    }

                    //if (lbl_BarcodeResult.BackColor != Color.Lime)
                    //    lbl_BarcodeResult.BackColor = Color.Lime;
                    if (dgd_Result.Rows[i].Cells[2].Style.BackColor != Color.Lime)
                        dgd_Result.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                    if (dgd_Result.Rows[i].Cells[2].Style.SelectionBackColor != Color.Lime)
                        dgd_Result.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                }
                else if (m_smVisionInfo.g_objBarcode.ref_blnTestDone && m_smVisionInfo.g_objBarcode.ref_blnCodeNotMatched[i])
                {
                    if (m_strBarcodeResult[i] != "----")
                    {
                        dgd_Result.Rows[i].Cells[3].Value = m_smVisionInfo.g_objBarcode.ref_fBarcodeAngle[i];
                    }
                    else
                    {
                        dgd_Result.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Result.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    }

                    //if (lbl_BarcodeResult.BackColor != Color.Red)
                    //    lbl_BarcodeResult.BackColor = Color.Red;
                    if (dgd_Result.Rows[i].Cells[2].Style.BackColor != Color.Red)
                        dgd_Result.Rows[i].Cells[2].Style.BackColor = Color.Red;
                    if (dgd_Result.Rows[i].Cells[2].Style.SelectionBackColor != Color.Red)
                        dgd_Result.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                }
                else if (m_smVisionInfo.g_objBarcode.ref_blnTestDone)
                {
                    //if (lbl_BarcodeResult.BackColor != Color.Red)
                    //    lbl_BarcodeResult.BackColor = Color.Red;
                    if (dgd_Result.Rows[i].Cells[2].Style.BackColor != Color.Red)
                        dgd_Result.Rows[i].Cells[2].Style.BackColor = Color.Red;
                    if (dgd_Result.Rows[i].Cells[2].Style.SelectionBackColor != Color.Red)
                        dgd_Result.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                }
                else
                {
                    //if (lbl_BarcodeResult.BackColor != Color.Yellow)
                    //    lbl_BarcodeResult.BackColor = Color.Yellow;
                }
            }
            lbl_BarcodeResult.Size = new Size(lbl_BarcodeResult.Size.Width, Math.Max(100, (int)Math.Round((lbl_BarcodeResult.Text.Length / (lbl_BarcodeResult.Size.Width / lbl_BarcodeResult.Font.Size)) * lbl_BarcodeResult.Font.Height)));
            
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


        private void btn_Reset_Click(object sender, EventArgs e)
        {
            //IO.ConnectUSBIOCard(9);

            //frmUSB8IO objIO = new frmUSB8IO();
            //objIO.ShowDialog();

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
                CustomizeGUI();
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

    }
}
