using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Common;
using SharedMemory;
using VisionProcessing;

namespace AutoMode
{
    public partial class AutoDisplayAllPage : Form
    {
        #region Member Variables

        private bool m_blnSplitTime = false;

        private int m_intVS1PassCount = 0;
        private int m_intVS2PassCount = 0;
        private int m_intVS3PassCount = 0;
        private int m_intVS4PassCount = 0;
        private int m_intVS5PassCount = 0;
        private int m_intVS6PassCount = 0;
        private int m_intVS7PassCount = 0;

        private int m_intVS1FailCount = 0;
        private int m_intVS2FailCount = 0;
        private int m_intVS3FailCount = 0;
        private int m_intVS4FailCount = 0;
        private int m_intVS5FailCount = 0;
        private int m_intVS6FailCount = 0;
        private int m_intVS7FailCount = 0;

        private string[] m_strBarcodeResult = new string[10] { "----", "----", "----", "----", "----", "----", "----", "----", "----", "----" };
        private string[] m_strBarcodeResultPrev = new string[10] { "----", "----", "----", "----", "----", "----", "----", "----", "----", "----" };

        private int m_intVS1GrabDelay = 0, m_intVS1GrabDelayPrev = -1;
        private int m_intVS2GrabDelay = 0, m_intVS2GrabDelayPrev = -1;
        private int m_intVS3GrabDelay = 0, m_intVS3GrabDelayPrev = -1;
        private int m_intVS4GrabDelay = 0, m_intVS4GrabDelayPrev = -1;
        private int m_intVS5GrabDelay = 0, m_intVS5GrabDelayPrev = -1;
        private int m_intVS6GrabDelay = 0, m_intVS6GrabDelayPrev = -1;
        private int m_intVS7GrabDelay = 0, m_intVS7GrabDelayPrev = -1;

        private int m_intVS1TestedTotal = 0, m_intVS1TestedTotalPrev = -1;
        private int m_intVS2TestedTotal = 0, m_intVS2TestedTotalPrev = -1;
        private int m_intVS3TestedTotal = 0, m_intVS3TestedTotalPrev = -1;
        private int m_intVS4TestedTotal = 0, m_intVS4TestedTotalPrev = -1;
        private int m_intVS5TestedTotal = 0, m_intVS5TestedTotalPrev = -1;
        private int m_intVS6TestedTotal = 0, m_intVS6TestedTotalPrev = -1;
        private int m_intVS7TestedTotal = 0, m_intVS7TestedTotalPrev = -1;

        private float m_fVS1GrabTime = 0.0f, m_fVS1GrabTimePrev = -1.0f;
        private float m_fVS2GrabTime = 0.0f, m_fVS2GrabTimePrev = -1.0f;
        private float m_fVS3GrabTime = 0.0f, m_fVS3GrabTimePrev = -1.0f;
        private float m_fVS4GrabTime = 0.0f, m_fVS4GrabTimePrev = -1.0f;
        private float m_fVS5GrabTime = 0.0f, m_fVS5GrabTimePrev = -1.0f;
        private float m_fVS6GrabTime = 0.0f, m_fVS6GrabTimePrev = -1.0f;
        private float m_fVS7GrabTime = 0.0f, m_fVS7GrabTimePrev = -1.0f;

        private float m_fVS1ProcessTime = 0.0f, m_fVS1ProcessTimePrev = -1.0f;
        private float m_fVS2ProcessTime = 0.0f, m_fVS2ProcessTimePrev = -1.0f;
        private float m_fVS3ProcessTime = 0.0f, m_fVS3ProcessTimePrev = -1.0f;
        private float m_fVS4ProcessTime = 0.0f, m_fVS4ProcessTimePrev = -1.0f;
        private float m_fVS5ProcessTime = 0.0f, m_fVS5ProcessTimePrev = -1.0f;
        private float m_fVS6ProcessTime = 0.0f, m_fVS6ProcessTimePrev = -1.0f;
        private float m_fVS7ProcessTime = 0.0f, m_fVS7ProcessTimePrev = -1.0f;

        private float m_fVS1TotalTime = 0.0f, m_fVS1TotalTimePrev = -1.0f;
        private float m_fVS2TotalTime = 0.0f, m_fVS2TotalTimePrev = -1.0f;
        private float m_fVS3TotalTime = 0.0f, m_fVS3TotalTimePrev = -1.0f;
        private float m_fVS4TotalTime = 0.0f, m_fVS4TotalTimePrev = -1.0f;
        private float m_fVS5TotalTime = 0.0f, m_fVS5TotalTimePrev = -1.0f;
        private float m_fVS6TotalTime = 0.0f, m_fVS6TotalTimePrev = -1.0f;
        private float m_fVS7TotalTime = 0.0f, m_fVS7TotalTimePrev = -1.0f;

        private string m_strVS2ResultPrev = "", m_strVS2Result = "";
        private string m_strVS3ResultPrev = "", m_strVS3Result = "";
        private string m_strVS3Result2Prev = "", m_strVS3Result2 = "";
        private string m_strVS4ResultPrev = "", m_strVS4Result = "";
        private string m_strVS4Result2Prev = "", m_strVS4Result2 = "";
        private string m_strVS5ResultPrev = "", m_strVS5Result = "";
        private string m_strVS6ResultPrev = "", m_strVS6Result = "";
        private string m_strVS7ResultPrev = "", m_strVS7Result = "";

        //private int m_intFailCount1VS1 = 0, m_intFailCount1PrevVS1 = -1;
        //private int m_intFailCount2VS1 = 0, m_intFailCount2PrevVS1 = -1;
        //private int m_intFailCount3VS1 = 0, m_intFailCount3PrevVS1 = -1;
        //private int m_intFailCount4VS1 = 0, m_intFailCount4PrevVS1 = -1;
        //private int m_intFailCount5VS1 = 0, m_intFailCount5PrevVS1 = -1;

        //private int m_intFailCount1VS2 = 0, m_intFailCount1PrevVS2 = -1;
        //private int m_intFailCount2VS2 = 0, m_intFailCount2PrevVS2 = -1;
        //private int m_intFailCount3VS2 = 0, m_intFailCount3PrevVS2 = -1;
        //private int m_intFailCount4VS2 = 0, m_intFailCount4PrevVS2 = -1;
        //private int m_intFailCount5VS2 = 0, m_intFailCount5PrevVS2 = -1;

        //private int m_intFailCount1VS3 = 0, m_intFailCount1PrevVS3 = -1;
        //private int m_intFailCount2VS3 = 0, m_intFailCount2PrevVS3 = -1;

        //private int m_intFailCount3VS3 = 0, m_intFailCount3PrevVS3 = -1;
        //private int m_intFailCount4VS3 = 0, m_intFailCount4PrevVS3 = -1;

        //private int m_intFailCount5VS3 = 0, m_intFailCount5PrevVS3 = -1;

        //private int m_intFailCount1VS4 = 0, m_intFailCount1PrevVS4 = -1;

        //private int m_intFailCount2VS4 = 0, m_intFailCount2PrevVS4 = -1;
        //private int m_intFailCount3VS4 = 0, m_intFailCount3PrevVS4 = -1;
        //private int m_intFailCount4VS4 = 0, m_intFailCount4PrevVS4 = -1;
        //private int m_intFailCount5VS4 = 0, m_intFailCount5PrevVS4 = -1;

        private Graphics m_objVS1Graphics;
        private Graphics m_objVS2Graphics;
        private Graphics m_objVS3Graphics;
        private Graphics m_objVS4Graphics;
        private Graphics m_objVS5Graphics;
        private Graphics m_objVS6Graphics;
        private Graphics m_objVS7Graphics;

        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private ROI m_objDrawROI = new ROI();
        private CROI m_objDrawCROI = new CROI();
        #endregion

        #region Shared Memory Variables

        private VisionInfo[] m_smVSInfo;

        private int[] m_arrVisionIndex = new int[8];

        #endregion


        public AutoDisplayAllPage(CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, VisionInfo[] smVSInfo)
        {
            InitializeComponent();

            m_smCustomizeInfo = smCustomizeInfo;
            m_smVSInfo = smVSInfo;
            m_smProductionInfo = smProductionInfo;
            m_arrVisionIndex = m_smProductionInfo.g_arrDisplayVisionModule;

            m_objVS1Graphics = Graphics.FromHwnd(Station1Panel.Handle);
            m_objVS2Graphics = Graphics.FromHwnd(Station2Panel.Handle);
            m_objVS3Graphics = Graphics.FromHwnd(Station3Panel.Handle);
            m_objVS4Graphics = Graphics.FromHwnd(Station4Panel.Handle);
            m_objVS5Graphics = Graphics.FromHwnd(Station5Panel.Handle);
            m_objVS6Graphics = Graphics.FromHwnd(Station6Panel.Handle);
            m_objVS7Graphics = Graphics.FromHwnd(Station7Panel.Handle);

            UpdateGUI();
            UpdateVision1(true, true);
            UpdateVision2(true, true);
            UpdateVision3(true, true);
            UpdateVision4(true, true);
            UpdateVision5(true, true);
            UpdateVision6(true, true);
            UpdateVision7(true, true);
            //ResetCounterBackColor();
        }

        /// <summary>
        /// Make sure that only when form is focused, then enable timer.
        /// By this way, CPU usage can be reduced.
        /// </summary>
        /// <param name="blnEnable">true = start to tick timer, false = otherwise</param>
        public void ActivateTimer(bool blnEnable)
        {
            ResultTimer.Enabled = blnEnable;
        }

        //private void ResetCounterBackColor()
        //{
        //    lbl_FailCount1VS1.BackColor = Color.White;
        //    lbl_FailCount2VS1.BackColor = Color.White;
        //    lbl_FailCount3VS1.BackColor = Color.White;
        //    lbl_FailCount4VS1.BackColor = Color.White;
        //    lbl_FailCount5VS1.BackColor = Color.White;

        //    lbl_FailCount1VS2.BackColor = Color.White;
        //    lbl_FailCount2VS2.BackColor = Color.White;
        //    lbl_FailCount3VS2.BackColor = Color.White;
        //    lbl_FailCount4VS2.BackColor = Color.White;
        //    lbl_FailCount5VS2.BackColor = Color.White;

        //    lbl_FailCount1VS3.BackColor = Color.White;
        //    lbl_FailCount2VS3.BackColor = Color.White;
        //    lbl_FailCount3VS3.BackColor = Color.White;
        //    lbl_FailCount4VS3.BackColor = Color.White;
        //    lbl_FailCount5VS3.BackColor = Color.White;

        //    lbl_FailCount1VS4.BackColor = Color.White;
        //    lbl_FailCount2VS4.BackColor = Color.White;
        //    lbl_FailCount3VS4.BackColor = Color.White;
        //    lbl_FailCount4VS4.BackColor = Color.White;
        //    lbl_FailCount5VS4.BackColor = Color.White;
        //}


        /// <summary>
        /// Display all related vision name sequentially on each Header 
        /// if there is 2 results on 1 vision station, display second result tag as well
        /// </summary>
        private void UpdateGUI()
        {
            //If more than 4 vision
            if ((m_smCustomizeInfo.g_intVisionMask & 0x10) > 0)
            {
                //Vision 1
                pnl_V1Detail.Location = new Point(0, 0);
                panel1.Size = new Size(155, 181);
                HeaderV1.Location = new Point(161, 0);
                lbl_V1Result2.Size = new Size(49, 26);
                HeaderV1.Controls.Add(lbl_V1Result);
                lbl_V1Result.Location = new Point(163, 0);
                lbl_V1Result.Size = new Size(49, 26);
                pnl_Station1.Location = new Point(161, 26);
                pnl_Station1.Size = new Size(213, 160);
                Station1Panel.Size = new Size(213, 160);
                pnl_Station1Group.Size = new Size(376, 186);

                //Vision 2
                HeaderV2.Location = new Point(3, 0);
                lbl_V2Result2.Size = new Size(49, 26);
                HeaderV2.Controls.Add(lbl_V2Result);
                lbl_V2Result.Location = new Point(163, 0);
                lbl_V2Result.Size = new Size(49, 26);
                pnl_Station2.Location = new Point(3, 26);
                pnl_Station2.Size = new Size(213, 160);
                Station2Panel.Size = new Size(213, 160);
                panel2.Location = new Point(220, 3);
                panel2.Size = new Size(155, 181);
                pnl_V2Detail.Location = new Point(0, 0);
                pnl_Station2Group.Location = new Point(382, 0);
                pnl_Station2Group.Size = new Size(376, 186);

                //Vision 3
                pnl_V3Detail.Location = new Point(0, 0);
                panel3.Size = new Size(155, 181);
                HeaderV3.Location = new Point(161, 0);
                lbl_V3Result2.Size = new Size(49, 26);
                HeaderV3.Controls.Add(lbl_V3Result);
                lbl_V3Result.Location = new Point(163, 0);
                lbl_V3Result.Size = new Size(49, 26);
                pnl_Station3.Location = new Point(161, 26);
                pnl_Station3.Size = new Size(213, 160);
                Station3Panel.Size = new Size(213, 160);
                pnl_Station3Group.Location = new Point(0, 187);
                pnl_Station3Group.Size = new Size(376, 186);

                //Vision 4
                HeaderV4.Location = new Point(3, 0);
                lbl_V4Result2.Size = new Size(49, 26);
                HeaderV4.Controls.Add(lbl_V4Result);
                lbl_V4Result.Location = new Point(163, 0);
                lbl_V4Result.Size = new Size(49, 26);
                pnl_Station4.Location = new Point(3, 26);
                pnl_Station4.Size = new Size(213, 160);
                Station4Panel.Size = new Size(213, 160);
                panel4.Location = new Point(220, 3);
                panel4.Size = new Size(155, 181);
                pnl_V4Detail.Location = new Point(0, 0);
                pnl_Station4Group.Location = new Point(382, 187);
                pnl_Station4Group.Size = new Size(376, 186);

                //Vision 5
                pnl_Station5Group.Location = new Point(0, 374);

                //Vision 6
                if ((m_smCustomizeInfo.g_intVisionMask & 0x20) > 0)
                {
                    pnl_Station6Group.Location = new Point(382, 374);
                }
                else
                {
                    pnl_Station6Group.Visible = false;
                }
                
                //Vision 7
                if ((m_smCustomizeInfo.g_intVisionMask & 0x40) > 0)
                {
                    pnl_Station7Group.Location = new Point(760, 0);
                }
                else
                {
                    pnl_Station7Group.Visible = false;
                }
            }
            else
            {
                pnl_Station5Group.Visible = false;
                pnl_Station6Group.Visible = false;
                pnl_Station7Group.Visible = false;
            }

            if (m_smVSInfo[m_arrVisionIndex[0]] != null)
            {
                HeaderV1.Text = m_smVSInfo[m_arrVisionIndex[0]].g_strVisionDisplayName + " " + m_smVSInfo[m_arrVisionIndex[0]].g_strVisionNameRemark;
                if (m_smVSInfo[m_arrVisionIndex[0]].g_intUnitsOnImage == 1)
                    lbl_V1Result2.Visible = false;
                //lbl_LotIDVS1.Text = m_smProductionInfo.g_strLotID;
                //lbl_OperatorIDVS1.Text = m_smProductionInfo.g_strOperatorID;
            }
            if (m_smVSInfo[m_arrVisionIndex[1]] != null)
            {
                HeaderV2.Text = m_smVSInfo[m_arrVisionIndex[1]].g_strVisionDisplayName + " " + m_smVSInfo[m_arrVisionIndex[1]].g_strVisionNameRemark;
                if (m_smVSInfo[m_arrVisionIndex[1]].g_intUnitsOnImage == 1)
                    lbl_V2Result2.Visible = false;
                //lbl_LotIDVS2.Text = m_smProductionInfo.g_strLotID;
                //lbl_OperatorIDVS2.Text = m_smProductionInfo.g_strOperatorID;
            }
            if (m_smVSInfo[m_arrVisionIndex[2]] != null)
            {
                HeaderV3.Text = m_smVSInfo[m_arrVisionIndex[2]].g_strVisionDisplayName + " " + m_smVSInfo[m_arrVisionIndex[2]].g_strVisionNameRemark;
                if (m_smVSInfo[m_arrVisionIndex[2]].g_intUnitsOnImage == 1)
                    lbl_V3Result2.Visible = false;
                //lbl_LotIDVS3.Text = m_smProductionInfo.g_strLotID;
                //lbl_OperatorIDVS3.Text = m_smProductionInfo.g_strOperatorID;
            }
            if (m_smVSInfo[m_arrVisionIndex[3]] != null)
            {
                HeaderV4.Text = m_smVSInfo[m_arrVisionIndex[3]].g_strVisionDisplayName + " " + m_smVSInfo[m_arrVisionIndex[3]].g_strVisionNameRemark;
                if (m_smVSInfo[m_arrVisionIndex[3]].g_intUnitsOnImage == 1)
                    lbl_V4Result2.Visible = false;
                //lbl_LotIDVS4.Text = m_smProductionInfo.g_strLotID;
                //lbl_OperatorIDVS4.Text = m_smProductionInfo.g_strOperatorID;
            }
            if (m_smVSInfo[m_arrVisionIndex[4]] != null)
            {
                HeaderV5.Text = m_smVSInfo[m_arrVisionIndex[4]].g_strVisionDisplayName + " " + m_smVSInfo[m_arrVisionIndex[4]].g_strVisionNameRemark;
                if (m_smVSInfo[m_arrVisionIndex[4]].g_intUnitsOnImage == 1)
                    lbl_V5Result2.Visible = false;
                //lbl_LotIDVS3.Text = m_smProductionInfo.g_strLotID;
                //lbl_OperatorIDVS3.Text = m_smProductionInfo.g_strOperatorID;
            }
            if (m_smVSInfo[m_arrVisionIndex[5]] != null)
            {
                HeaderV6.Text = m_smVSInfo[m_arrVisionIndex[5]].g_strVisionDisplayName + " " + m_smVSInfo[m_arrVisionIndex[5]].g_strVisionNameRemark;
                if (m_smVSInfo[m_arrVisionIndex[5]].g_intUnitsOnImage == 1)
                    lbl_V6Result2.Visible = false;
                //lbl_LotIDVS4.Text = m_smProductionInfo.g_strLotID;
                //lbl_OperatorIDVS4.Text = m_smProductionInfo.g_strOperatorID;
            }
            if (m_smVSInfo[m_arrVisionIndex[6]] != null)
            {
                HeaderV7.Text = m_smVSInfo[m_arrVisionIndex[6]].g_strVisionDisplayName + " " + m_smVSInfo[m_arrVisionIndex[6]].g_strVisionNameRemark;
                if (m_smVSInfo[m_arrVisionIndex[6]].g_intUnitsOnImage == 1)
                    lbl_V7Result2.Visible = false;
                //lbl_LotIDVS4.Text = m_smProductionInfo.g_strLotID;
                //lbl_OperatorIDVS4.Text = m_smProductionInfo.g_strOperatorID;
            }
        }


        /// <summary>
        /// Display 1st/5th Vision Station latest test result, pass counter, total tested counter, 
        /// grab time, processing time, and grab delay time that is set in camera setting form
        /// </summary>
        private void UpdateVision1(bool blnReset, bool blnFirstTime)
        {
            if (m_smVSInfo[m_arrVisionIndex[0]] == null)
                return;

            float fPercentage = 0.0f;

            m_intVS1TestedTotal = m_smVSInfo[m_arrVisionIndex[0]].g_intTestedTotal;
            if (m_intVS1TestedTotal != m_intVS1TestedTotalPrev)
            {
                m_intVS1PassCount = m_smVSInfo[m_arrVisionIndex[0]].g_intPassTotal;
                lbl_VS1PassResult.Text = m_intVS1PassCount.ToString();

                if (m_intVS1TestedTotal - m_intVS1PassCount < 0)
                    m_intVS1FailCount = 0;
                else
                    m_intVS1FailCount = m_intVS1TestedTotal - m_intVS1PassCount;

                lbl_VS1FailResult.Text = m_intVS1FailCount.ToString();

                lbl_VS1TotalResult.Text = m_intVS1TestedTotal.ToString();

                if (m_intVS1TestedTotal != 0)
                {
                    fPercentage = (m_intVS1PassCount / (float)m_intVS1TestedTotal) * 100;
                    lbl_VS1YieldResult.Text = fPercentage.ToString("F2");
                }
                else
                    lbl_VS1YieldResult.Text = "0.00";

                m_intVS1TestedTotalPrev = m_intVS1TestedTotal;
            }

            if (m_smVSInfo[m_arrVisionIndex[0]].g_strVisionName == "Barcode")
            {
                //if (!pnl_V1Detail.Controls.Contains(lbl_BarcodeResult))
                //{
                //    pnl_V1Detail.Controls.Add(lbl_BarcodeResult);
                //    lbl_BarcodeResult.Location = new Point(lbl_Result1.Location.X, lbl_Result1.Location.Y + lbl_Result1.Size.Height);
                //    lbl_BarcodeResult.BringToFront();
                //}
                if (!pnl_V1Detail.Controls.Contains(dgd_BarcodeResult))
                {
                    pnl_V1Detail.Controls.Add(dgd_BarcodeResult);
                    dgd_BarcodeResult.Location = new Point(lbl_Result1.Location.X, lbl_Result1.Location.Y + lbl_Result1.Size.Height);
                    dgd_BarcodeResult.BringToFront();
                }
                lbl_VS1PassResult.Text = "---";
                lbl_VS1FailResult.Text = "---";
                lbl_VS1TotalResult.Text = "---";
                lbl_VS1YieldResult.Text = "---";

                //m_strBarcodeResult = m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_strResultCode;
                //if (m_strBarcodeResult != m_strBarcodeResultPrev)
                //{
                //    if (m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnCodeFound && m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnCodePassed)
                //    {
                //        lbl_BarcodeResult.BackColor = Color.Lime;
                //        lbl_BarcodeResult.Text = m_strBarcodeResult;
                //        m_strBarcodeResultPrev = m_strBarcodeResult;
                //    }
                //    else if (m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnCodeNotMatched)
                //    {
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //        lbl_BarcodeResult.Text = m_strBarcodeResult;
                //        m_strBarcodeResultPrev = m_strBarcodeResult;
                //    }
                //    else if (m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnTestDone)
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
                //if (m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnCodeFound && m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnCodePassed)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Lime)
                //        lbl_BarcodeResult.BackColor = Color.Lime;
                //}
                //else if (m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnCodeNotMatched)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Red)
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //}
                //else if (m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnTestDone)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Red)
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //}
                //else
                //{
                //    //if (lbl_BarcodeResult.BackColor != Color.Yellow)
                //    //    lbl_BarcodeResult.BackColor = Color.Yellow;
                //}

                if (blnFirstTime)
                {
                    dgd_BarcodeResult.Rows.Clear();
                    for (int i = 0; i < m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_intTemplateCount; i++)
                    {
                        dgd_BarcodeResult.Rows.Add();
                        dgd_BarcodeResult.Rows[i].Cells[0].Value = (i + 1).ToString();
                        dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
                        m_strBarcodeResultPrev[i] = "----";
                        m_strBarcodeResult[i] = "----";
                    }
                }

                //dgd_BarcodeResult.Rows.Clear();
                for (int i = 0; i < m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_intTemplateCount; i++)
                {
                    if (i >= dgd_BarcodeResult.RowCount)
                    {
                        dgd_BarcodeResult.Rows.Add();
                        dgd_BarcodeResult.Rows[i].Cells[0].Value = (i + 1).ToString();
                        dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
                    }

                    m_strBarcodeResult[i] = m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_strResultCode[i];
                    if (m_strBarcodeResult[i] != m_strBarcodeResultPrev[i])
                    {
                        if (m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnCodeFound[i] && m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnCodePassed[i])
                        {
                            if (m_strBarcodeResult[i] != "----")
                            {
                                if (Math.Abs(m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_fBarcodeAngle[i] - m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_fBarcodeOrientationAngle) > 45)
                                {
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                                }
                                else
                                {
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                                }
                            }
                            else
                            {
                                dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                                dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                            }
                            //dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                            //dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = m_strBarcodeResult[i];
                            m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                        }
                        else if (m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnCodeNotMatched[i])
                        {
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = m_strBarcodeResult[i];
                            m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                        }
                        else if (m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnTestDone)
                        {
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
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
                    if (m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnCodeFound[i] && m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnCodePassed[i])
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Lime)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Lime)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    }
                    else if (m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnCodeNotMatched[i])
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    }
                    else if (m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_blnTestDone)
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        //if (lbl_BarcodeResult.BackColor != Color.Yellow)
                        //    lbl_BarcodeResult.BackColor = Color.Yellow;
                    }
                }
                if (blnFirstTime && dgd_BarcodeResult.RowCount == 0)
                {
                    dgd_BarcodeResult.Rows.Add();
                    dgd_BarcodeResult.Rows[0].Cells[0].Value = (1).ToString();
                    m_smVSInfo[m_arrVisionIndex[0]].g_objBarcode.ref_intTemplateCount = 1;
                    dgd_BarcodeResult.Rows[0].Cells[1].Value = "----";
                }
            }

            if (m_smVSInfo[m_arrVisionIndex[0]].g_blnNoGrabTime)
                m_fVS1GrabTime = 0;
            else
                m_fVS1GrabTime = Math.Max(0, m_smVSInfo[m_arrVisionIndex[0]].g_objGrabTime.Duration - m_smVSInfo[m_arrVisionIndex[0]].g_intCameraGrabDelay);  //29-05-2019 ZJYEOH : Need to minus grab delay as it included inside Grab time 
            if (m_fVS1GrabTime != m_fVS1GrabTimePrev)
            {
                lbl_V1GrabResult.Text = m_fVS1GrabTime.ToString("F0");
                m_fVS1GrabTimePrev = m_fVS1GrabTime;
            }

            m_fVS1ProcessTime = (Math.Max(0, m_smVSInfo[m_arrVisionIndex[0]].g_objTotalTime.Duration - m_smVSInfo[m_arrVisionIndex[0]].g_objGrabTime.Duration)); //m_smVSInfo[m_arrVisionIndex[0]].g_objTransferTime.Duration + m_smVSInfo[m_arrVisionIndex[0]].g_objProcessTime.Duration;
            if (m_fVS1ProcessTime != m_fVS1ProcessTimePrev)
            {
                if (m_blnSplitTime)
                {
                    lbl_V1ProcessResult.Text = m_smVSInfo[m_arrVisionIndex[0]].g_objTransferTime.Duration.ToString("F2") + "|" +
                                                         m_smVSInfo[m_arrVisionIndex[0]].g_objProcessTime.Duration.ToString("F2");
                }
                else
                {
                    lbl_V1ProcessResult.Text = m_fVS1ProcessTime.ToString("F2");
                }

                m_fVS1ProcessTimePrev = m_fVS1ProcessTime;
            }

            m_fVS1TotalTime = m_smVSInfo[m_arrVisionIndex[0]].g_objTotalTime.Duration;
            if (m_fVS1TotalTime != m_fVS1TotalTimePrev)
            {
                lbl_V1TotalTime.Text = m_fVS1TotalTime.ToString("F2");

                m_fVS1TotalTimePrev = m_fVS1TotalTime;
            }


            if (m_smVSInfo[m_arrVisionIndex[0]].g_strResult == "Pass")
            {
                lbl_V1Result.BackColor = Color.Lime;

                if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVSInfo[m_arrVisionIndex[0]].g_intVisionPos)) > 0)
                {
                    switch (m_smVSInfo[m_arrVisionIndex[0]].g_intOrientResult[0])
                    {
                        case 0:
                            lbl_V1Result.Text = "0";
                            break;
                        case 1:
                            lbl_V1Result.Text = "-90";
                            break;
                        case 2:
                            lbl_V1Result.Text = "180";
                            break;
                        case 3:
                            lbl_V1Result.Text = "90";
                            break;
                        case 4:
                            lbl_V1Result.Text = "Invalid";
                            break;
                    }
                }
                else
                {
                    lbl_V1Result.Text = "Pass";
                }
            }
            else if (m_smVSInfo[m_arrVisionIndex[0]].g_strResult == "Fail")
            {
                lbl_V1Result.BackColor = Color.Red;
                lbl_V1Result.Text = m_smVSInfo[m_arrVisionIndex[0]].g_strResult;
            }
            else if (m_smVSInfo[m_arrVisionIndex[0]].g_strResult == "Idle")
            {
                lbl_V1Result.BackColor = Color.Yellow;
                lbl_V1Result.Text = m_smVSInfo[m_arrVisionIndex[0]].g_strResult;
            }

            if (m_smVSInfo[m_arrVisionIndex[0]].g_blnNoGrabTime)
                m_intVS1GrabDelay = 0;
            else
                m_intVS1GrabDelay = m_smVSInfo[m_arrVisionIndex[0]].g_intCameraGrabDelay;

            if (m_intVS1GrabDelay != m_intVS1GrabDelayPrev)
            {
                lbl_GrabDelay1.Text = m_intVS1GrabDelay.ToString();
                m_intVS1GrabDelayPrev = m_intVS1GrabDelay;
            }


            //switch(m_smVSInfo[m_arrVisionIndex[0]].g_strVisionName)
            //{
            //    case "Orient":
            //    case "BottomOrient":
            //    case "Mark":
            //    case "MarkOrient":
            //    case "MarkPkg":
            //    case "MOPkg":
            //    case "MOLiPkg":
            //    case "MOLi":
            //    case "Package":
            //        //Vision1
            //        lbl_FailName1VS1.Text = "Orient";
            //        m_intFailCount1VS1 = m_smVSInfo[m_arrVisionIndex[0]].g_intOrientFailureTotal;
            //        if (m_intFailCount1VS1 != m_intFailCount1PrevVS1 && !blnReset)
            //        {
            //            if (m_intFailCount1VS1 != 0)
            //                lbl_FailCount1VS1.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS1.BackColor = Color.White;
            //        }
            //        else
            //        {
            //            lbl_FailCount1VS1.BackColor = Color.White;
            //        }
            //        lbl_FailCount1VS1.Text = m_intFailCount1VS1.ToString();
            //        m_intFailCount1PrevVS1 = m_intFailCount1VS1;

            //        lbl_FailName2VS1.Text = "Mark";
            //        m_intFailCount2VS1 = m_smVSInfo[m_arrVisionIndex[0]].g_intMarkFailureTotal;
            //        if (m_intFailCount2VS1 != m_intFailCount2PrevVS1 && !blnReset)
            //        {
            //            if (m_intFailCount2VS1 != 0)
            //                lbl_FailCount2VS1.BackColor = Color.Red;
            //            else
            //                lbl_FailCount2VS1.BackColor = Color.White;
            //        }
            //        else
            //        {
            //            lbl_FailCount2VS1.BackColor = Color.White;
            //        }
            //        lbl_FailCount2VS1.Text = m_intFailCount2VS1.ToString();
            //        m_intFailCount2PrevVS1 = m_intFailCount2VS1;

            //        lbl_FailName3VS1.Text = "Package";
            //        m_intFailCount3VS1 = m_smVSInfo[m_arrVisionIndex[0]].g_intPackageFailureTotal;
            //        if (m_intFailCount3VS1 != m_intFailCount3PrevVS1 && !blnReset)
            //        {
            //            if (m_intFailCount3VS1 != 0)
            //                lbl_FailCount3VS1.BackColor = Color.Red;
            //            else
            //                lbl_FailCount3VS1.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount3VS1.BackColor = Color.White;
            //        lbl_FailCount3VS1.Text = m_intFailCount3VS1.ToString();
            //        m_intFailCount3PrevVS1 = m_intFailCount3VS1;

            //        lbl_FailName4VS1.Text = "Empty Unit";
            //        m_intFailCount4VS1 = m_smVSInfo[m_arrVisionIndex[0]].g_intCheckPresenceFailureTotal;
            //        if (m_intFailCount4VS1 != m_intFailCount4PrevVS1 && !blnReset)
            //        {
            //            if (m_intFailCount4VS1 != 0)
            //                lbl_FailCount4VS1.BackColor = Color.Red;
            //            else
            //                lbl_FailCount4VS1.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount4VS1.BackColor = Color.White;
            //        lbl_FailCount4VS1.Text = m_intFailCount4VS1.ToString();
            //        m_intFailCount4PrevVS1 = m_intFailCount4VS1;

            //        lbl_FailName5VS1.Text = "Pin1";
            //        m_intFailCount5VS1 = m_smVSInfo[m_arrVisionIndex[0]].g_intPin1FailureTotal;
            //        if (m_intFailCount5VS1 != m_intFailCount5PrevVS1 && !blnReset)
            //        {
            //            if (m_intFailCount5VS1 != 0)
            //                lbl_FailCount5VS1.BackColor = Color.Red;
            //            else
            //                lbl_FailCount5VS1.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount5VS1.BackColor = Color.White;
            //        lbl_FailCount5VS1.Text = m_intFailCount5VS1.ToString();
            //        m_intFailCount5PrevVS1 = m_intFailCount5VS1;

            //        break;
            //    case "UnitPresent":
            //    case "BottomPosition":
            //    case "BottomPositionOrient":
            //    case "TapePocketPosition":
            //        //Vision2
            //        lbl_FailName1VS1.Text = "Fail";
            //        m_intFailCount1VS1 = m_smVSInfo[m_arrVisionIndex[0]].g_intPositionFailureTotal;
            //        if (m_intFailCount1VS1 != m_intFailCount1PrevVS1 && !blnReset)
            //        {
            //            if (m_intFailCount1VS1 != 0)
            //                lbl_FailCount1VS1.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS1.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS1.BackColor = Color.White;
            //        lbl_FailCount1VS1.Text = m_intFailCount1VS1.ToString();
            //        m_intFailCount1PrevVS1 = m_intFailCount1VS1;

            //        lbl_FailName2VS1.Visible = false;
            //        lbl_FailCount2VS1.Visible = false;
            //        lbl_FailName3VS1.Visible = false;
            //        lbl_FailCount3VS1.Visible = false;
            //        lbl_FailName4VS1.Visible = false;
            //        lbl_FailCount4VS1.Visible = false;
            //        lbl_FailName5VS1.Visible = false;
            //        lbl_FailCount5VS1.Visible = false;
            //        break;
            //    case "Pad":
            //    case "PadPos":
            //    case "PadPkg":
            //    case "PadPkgPos":
            //    case "Pad5S":
            //    case "Pad5SPos":
            //    case "Pad5SPkg":
            //    case "Pad5SPkgPos":
            //    //Vision3
                    //case "Li3D":
                    //case "Li3DPkg":
            //        //Vision3Lead3D
            //        lbl_FailName1VS1.Text = "Empty Unit";
            //        m_intFailCount1VS1 = m_smVSInfo[m_arrVisionIndex[0]].g_intEmptyUnitFailureTotal;
            //        if (m_intFailCount1VS1 != m_intFailCount1PrevVS1 && !blnReset)
            //        {
            //            if (m_intFailCount1VS1 != 0)
            //                lbl_FailCount1VS1.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS1.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS1.BackColor = Color.White;
            //        lbl_FailCount1VS1.Text = m_intFailCount1VS1.ToString();
            //        m_intFailCount1PrevVS1 = m_intFailCount1VS1;


            //        lbl_FailName2VS1.Text = "Pad";
            //        m_intFailCount2VS1 = m_smVSInfo[m_arrVisionIndex[0]].g_intPadFailureTotal;
            //        if (m_intFailCount2VS1 != m_intFailCount2PrevVS1 && !blnReset)
            //        {
            //            if (m_intFailCount2VS1 != 0)
            //                lbl_FailCount2VS1.BackColor = Color.Red;
            //            else
            //                lbl_FailCount2VS1.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount2VS1.BackColor = Color.White;
            //        lbl_FailCount2VS1.Text = m_intFailCount2VS1.ToString();
            //        m_intFailCount2PrevVS1 = m_intFailCount2VS1;

            //        lbl_FailName3VS1.Text = "Position";
            //        m_intFailCount3VS1 = m_smVSInfo[m_arrVisionIndex[0]].g_intPositionFailureTotal;
            //        if (m_intFailCount3VS1 != m_intFailCount3PrevVS1 && !blnReset)
            //        {
            //            if (m_intFailCount3VS1 != 0)
            //                lbl_FailCount3VS1.BackColor = Color.Red;
            //            else
            //                lbl_FailCount3VS1.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount3VS1.BackColor = Color.White;
            //        lbl_FailCount3VS1.Text = m_intFailCount3VS1.ToString();
            //        m_intFailCount3PrevVS1 = m_intFailCount3VS1;

            //        lbl_FailName4VS1.Text = "Package";
            //        m_intFailCount4VS1 = m_smVSInfo[m_arrVisionIndex[0]].g_intPackageFailureTotal;
            //        if (m_intFailCount4VS1 != m_intFailCount4PrevVS1 && !blnReset)
            //        {
            //            if (m_intFailCount4VS1 != 0)
            //                lbl_FailCount4VS1.BackColor = Color.Red;
            //            else
            //                lbl_FailCount4VS1.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount4VS1.BackColor = Color.White;
            //        lbl_FailCount4VS1.Text = m_intFailCount4VS1.ToString();
            //        m_intFailCount4PrevVS1 = m_intFailCount4VS1;

            //        lbl_FailName5VS1.Text = "Pin1";
            //        m_intFailCount5VS1 = m_smVSInfo[m_arrVisionIndex[0]].g_intPin1FailureTotal;
            //        if (m_intFailCount5VS1 != m_intFailCount5PrevVS1 && !blnReset)
            //        {
            //            if (m_intFailCount5VS1 != 0)
            //                lbl_FailCount5VS1.BackColor = Color.Red;
            //            else
            //                lbl_FailCount5VS1.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount5VS1.BackColor = Color.White;
            //        lbl_FailCount5VS1.Text = m_intFailCount5VS1.ToString();
            //        m_intFailCount5PrevVS1 = m_intFailCount5VS1;
            //        break;
            //    case "InPocket":
            //    case "InPocketPkg":
            //    case "InPocketPkgPos":
            //    case "IPMLi":
            //    case "IPMLiPkg":
            //        //Vision4
            //        lbl_FailName1VS1.Text = "Orient";
            //        m_intFailCount1VS1 = m_smVSInfo[m_arrVisionIndex[0]].g_intOrientFailureTotal;
            //        if (m_intFailCount1VS1 != m_intFailCount1PrevVS1 && !blnReset)
            //        {
            //            if (m_intFailCount1VS1 != 0)
            //                lbl_FailCount1VS1.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS1.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS1.BackColor = Color.White;
            //        lbl_FailCount1VS1.Text = m_intFailCount1VS1.ToString();
            //        m_intFailCount1PrevVS1 = m_intFailCount1VS1;

            //        lbl_FailName2VS1.Text = "Mark";
            //        m_intFailCount2VS1 = m_smVSInfo[m_arrVisionIndex[0]].g_intMarkFailureTotal;
            //        if (m_intFailCount2VS1 != m_intFailCount2PrevVS1 && !blnReset)
            //        {
            //            if (m_intFailCount2VS1 != 0)
            //                lbl_FailCount2VS1.BackColor = Color.Red;
            //            else
            //                lbl_FailCount2VS1.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount2VS1.BackColor = Color.White;
            //        lbl_FailCount2VS1.Text = m_intFailCount2VS1.ToString();
            //        m_intFailCount2PrevVS1 = m_intFailCount2VS1;

            //        lbl_FailName3VS1.Text = "Package";
            //        m_intFailCount3VS1 = m_smVSInfo[m_arrVisionIndex[0]].g_intPackageFailureTotal;
            //        if (m_intFailCount3VS1 != m_intFailCount3PrevVS1 && !blnReset)
            //        {
            //            if (m_intFailCount3VS1 != 0)
            //                lbl_FailCount3VS1.BackColor = Color.Red;
            //            else
            //                lbl_FailCount3VS1.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount3VS1.BackColor = Color.White;
            //        lbl_FailCount3VS1.Text = m_intFailCount3VS1.ToString();
            //        m_intFailCount3PrevVS1 = m_intFailCount3VS1;

            //        lbl_FailName4VS1.Text = "Pin1";
            //        m_intFailCount4VS1 = m_smVSInfo[m_arrVisionIndex[0]].g_intPin1FailureTotal;
            //        if (m_intFailCount4VS1 != m_intFailCount4PrevVS1 && !blnReset)
            //        {
            //            if (m_intFailCount4VS1 != 0)
            //                lbl_FailCount4VS1.BackColor = Color.Red;
            //            else
            //                lbl_FailCount4VS1.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount4VS1.BackColor = Color.White;
            //        lbl_FailCount4VS1.Text = m_intFailCount4VS1.ToString();
            //        m_intFailCount4PrevVS1 = m_intFailCount4VS1;

            //        lbl_FailName5VS1.Visible = false;
            //        lbl_FailCount5VS1.Visible = false;
            //        break;
            //    case "Seal":
            //        //Vision6
            //        lbl_FailName1VS1.Text = "Seal";
            //        m_intFailCount1VS1 = m_smVSInfo[m_arrVisionIndex[0]].g_intSealFailureTotal;
            //        if (m_intFailCount1VS1 != m_intFailCount1PrevVS1 && !blnReset)
            //        {
            //            if (m_intFailCount1VS1 != 0)
            //                lbl_FailCount1VS1.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS1.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS1.BackColor = Color.White;
            //        lbl_FailCount1VS1.Text = m_intFailCount1VS1.ToString();
            //        m_intFailCount1PrevVS1 = m_intFailCount1VS1;

            //        lbl_FailName2VS1.Visible = false;
            //        lbl_FailCount2VS1.Visible = false;
            //        lbl_FailName3VS1.Visible = false;
            //        lbl_FailCount3VS1.Visible = false;
            //        lbl_FailName4VS1.Visible = false;
            //        lbl_FailCount4VS1.Visible = false;
            //        lbl_FailName5VS1.Visible = false;
            //        lbl_FailCount5VS1.Visible = false;
            //        break;

            //}

        }
        /// <summary>
        /// Display 2nd/6th Vision Station latest test result, pass counter, total tested counter, 
        /// grab time, processing time, and grab delay time that is set in camera setting form
        /// </summary>
        private void UpdateVision2(bool blnReset, bool blnFirstTime)
        {
            if (m_smVSInfo[m_arrVisionIndex[1]] == null)
                return;

            float fPercentage = 0.0f;

            m_intVS2TestedTotal = m_smVSInfo[m_arrVisionIndex[1]].g_intTestedTotal;
            if (m_intVS2TestedTotal != m_intVS2TestedTotalPrev)
            {
                m_intVS2PassCount = m_smVSInfo[m_arrVisionIndex[1]].g_intPassTotal;
                lbl_VS2PassResult.Text = m_intVS2PassCount.ToString();

                if (m_intVS2TestedTotal - m_intVS2PassCount < 0)
                    m_intVS2FailCount = 0;
                else
                    m_intVS2FailCount = m_intVS2TestedTotal - m_intVS2PassCount;

                lbl_VS2FailResult.Text = m_intVS2FailCount.ToString();

                lbl_VS2TotalResult.Text = m_intVS2TestedTotal.ToString();

                if (m_intVS2TestedTotal != 0)
                {
                    fPercentage = (m_intVS2PassCount / (float)m_intVS2TestedTotal) * 100;
                    lbl_VS2YieldResult.Text = fPercentage.ToString("F2");
                }
                else
                    lbl_VS2YieldResult.Text = "0.00";

                m_intVS2TestedTotalPrev = m_intVS2TestedTotal;
            }

            if (m_smVSInfo[m_arrVisionIndex[1]].g_strVisionName == "Barcode")
            {
                //if (!pnl_V2Detail.Controls.Contains(lbl_BarcodeResult))
                //{
                //    pnl_V2Detail.Controls.Add(lbl_BarcodeResult);
                //    lbl_BarcodeResult.Location = new Point(lbl_Result2.Location.X, lbl_Result2.Location.Y + lbl_Result2.Size.Height);
                //    lbl_BarcodeResult.BringToFront();
                //}
                if (!pnl_V2Detail.Controls.Contains(dgd_BarcodeResult))
                {
                    pnl_V2Detail.Controls.Add(dgd_BarcodeResult);
                    dgd_BarcodeResult.Location = new Point(lbl_Result2.Location.X, lbl_Result2.Location.Y + lbl_Result2.Size.Height);
                    dgd_BarcodeResult.BringToFront();
                }
                
                lbl_VS2PassResult.Text = "---";
                lbl_VS2FailResult.Text = "---";
                lbl_VS2TotalResult.Text = "---";
                lbl_VS2YieldResult.Text = "---";

                //m_strBarcodeResult = m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_strResultCode;
                //if (m_strBarcodeResult != m_strBarcodeResultPrev)
                //{
                //    if (m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnCodeFound && m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnCodePassed)
                //    {
                //        lbl_BarcodeResult.BackColor = Color.Lime;
                //        lbl_BarcodeResult.Text = m_strBarcodeResult;
                //        m_strBarcodeResultPrev = m_strBarcodeResult;
                //    }
                //    else if (m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnCodeNotMatched)
                //    {
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //        lbl_BarcodeResult.Text = m_strBarcodeResult;
                //        m_strBarcodeResultPrev = m_strBarcodeResult;
                //    }
                //    else if (m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnTestDone)
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
                //if (m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnCodeFound && m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnCodePassed)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Lime)
                //        lbl_BarcodeResult.BackColor = Color.Lime;
                //}
                //else if (m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnCodeNotMatched)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Red)
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //}
                //else if (m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnTestDone)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Red)
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //}
                //else
                //{
                //    //if (lbl_BarcodeResult.BackColor != Color.Yellow)
                //    //    lbl_BarcodeResult.BackColor = Color.Yellow;
                //}

                if (blnFirstTime)
                {
                    dgd_BarcodeResult.Rows.Clear();
                    for (int i = 0; i < m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_intTemplateCount; i++)
                    {
                        dgd_BarcodeResult.Rows.Add();
                        dgd_BarcodeResult.Rows[i].Cells[0].Value = (i + 1).ToString();
                        dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
                        m_strBarcodeResultPrev[i] = "----";
                        m_strBarcodeResult[i] = "----";
                    }
                }

                //dgd_BarcodeResult.Rows.Clear();
                for (int i = 0; i < m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_intTemplateCount; i++)
                {
                    if (i >= dgd_BarcodeResult.RowCount)
                    {
                        dgd_BarcodeResult.Rows.Add();
                        dgd_BarcodeResult.Rows[i].Cells[0].Value = (i + 1).ToString();
                        dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
                    }

                    m_strBarcodeResult[i] = m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_strResultCode[i];
                    if (m_strBarcodeResult[i] != m_strBarcodeResultPrev[i])
                    {
                        if (m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnCodeFound[i] && m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnCodePassed[i])
                        {
                            if (m_strBarcodeResult[i] != "----")
                            {
                                if (Math.Abs(m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_fBarcodeAngle[i] - m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_fBarcodeOrientationAngle) > 45)
                                {
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                                }
                                else
                                {
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                                }
                            }
                            else
                            {
                                dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                                dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                            }
                            //dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                            //dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = m_strBarcodeResult[i];
                            m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                        }
                        else if (m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnCodeNotMatched[i])
                        {
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = m_strBarcodeResult[i];
                            m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                        }
                        else if (m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnTestDone)
                        {
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
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
                    if (m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnCodeFound[i] && m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnCodePassed[i])
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Lime)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Lime)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    }
                    else if (m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnCodeNotMatched[i])
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    }
                    else if (m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_blnTestDone)
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        //if (lbl_BarcodeResult.BackColor != Color.Yellow)
                        //    lbl_BarcodeResult.BackColor = Color.Yellow;
                    }
                }
                if (blnFirstTime && dgd_BarcodeResult.RowCount == 0)
                {
                    dgd_BarcodeResult.Rows.Add();
                    dgd_BarcodeResult.Rows[0].Cells[0].Value = (1).ToString();
                    m_smVSInfo[m_arrVisionIndex[1]].g_objBarcode.ref_intTemplateCount = 1;
                    dgd_BarcodeResult.Rows[0].Cells[1].Value = "----";
                }
            }

            if (m_smVSInfo[m_arrVisionIndex[1]].g_blnNoGrabTime)
                m_fVS2GrabTime = 0;
            else
                m_fVS2GrabTime = Math.Max(0, m_smVSInfo[m_arrVisionIndex[1]].g_objGrabTime.Duration - m_smVSInfo[m_arrVisionIndex[1]].g_intCameraGrabDelay);  //29-05-2019 ZJYEOH : Need to minus grab delay as it included inside Grab time 
            if (m_fVS2GrabTime != m_fVS2GrabTimePrev)
            {
                lbl_V2GrabResult.Text = m_fVS2GrabTime.ToString("F0");
                m_fVS2GrabTimePrev = m_fVS2GrabTime;
            }

            m_fVS2ProcessTime = (Math.Max(0, m_smVSInfo[m_arrVisionIndex[1]].g_objTotalTime.Duration - m_smVSInfo[m_arrVisionIndex[1]].g_objGrabTime.Duration));// m_smVSInfo[m_arrVisionIndex[1]].g_objTransferTime.Duration + m_smVSInfo[m_arrVisionIndex[1]].g_objProcessTime.Duration;
            if (m_fVS2ProcessTime != m_fVS2ProcessTimePrev)
            {
                if (m_blnSplitTime)
                {
                    lbl_V2ProcessResult.Text = m_smVSInfo[m_arrVisionIndex[1]].g_objTransferTime.Duration.ToString("F2") + "|" +
                                                         m_smVSInfo[m_arrVisionIndex[1]].g_objProcessTime.Duration.ToString("F2");
                }
                else
                {
                    lbl_V2ProcessResult.Text = m_fVS2ProcessTime.ToString("F2");
                }

                m_fVS2ProcessTimePrev = m_fVS2ProcessTime;
            }

            m_fVS2TotalTime = m_smVSInfo[m_arrVisionIndex[1]].g_objTotalTime.Duration;
            if (m_fVS2TotalTime != m_fVS2TotalTimePrev)
            {
                lbl_V2TotalTime.Text = m_fVS2TotalTime.ToString("F2");

                m_fVS2TotalTimePrev = m_fVS2TotalTime;
            }

            m_strVS2Result = m_smVSInfo[m_arrVisionIndex[1]].g_strResult;
            if (m_strVS2Result != m_strVS2ResultPrev)
            {
                if (m_strVS2Result == "Pass" || m_strVS2Result == "Empty")
                    lbl_V2Result.BackColor = Color.Lime;
                else if (m_strVS2Result == "Fail" || m_strVS2Result == "NoEmpty")
                    lbl_V2Result.BackColor = Color.Red;
                else if (m_strVS2Result == "Idle")
                {
                    lbl_V2Result.BackColor = Color.Yellow;
                }

                lbl_V2Result.Text = m_strVS2Result;
                m_strVS2ResultPrev = m_strVS2Result;
            }

            if (m_smVSInfo[m_arrVisionIndex[1]].g_blnNoGrabTime)
                m_intVS2GrabDelay = 0;
            else
                m_intVS2GrabDelay = m_smVSInfo[m_arrVisionIndex[1]].g_intCameraGrabDelay;
            if (m_intVS2GrabDelay != m_intVS2GrabDelayPrev)
            {
                lbl_GrabDelay2.Text = m_intVS2GrabDelay.ToString();
                m_intVS2GrabDelayPrev = m_intVS2GrabDelay;
            }

            //switch (m_smVSInfo[m_arrVisionIndex[1]].g_strVisionName)
            //{
            //    case "Orient":
            //    case "BottomOrient":
            //    case "Mark":
            //    case "MarkOrient":
            //    case "MarkPkg":
            //    case "MOPkg":
            //    case "MOLiPkg":
            //    case "MOLi":
            //    case "Package":
            //        //Vision1
            //        lbl_FailName1VS2.Text = "Orient";
            //        m_intFailCount1VS2 = m_smVSInfo[m_arrVisionIndex[1]].g_intOrientFailureTotal;
            //        if (m_intFailCount1VS2 != m_intFailCount1PrevVS2 && !blnReset)
            //        {
            //            if (m_intFailCount1VS2 != 0)
            //                lbl_FailCount1VS2.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS2.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS2.BackColor = Color.White;
            //        lbl_FailCount1VS2.Text = m_intFailCount1VS2.ToString();
            //        m_intFailCount1PrevVS2 = m_intFailCount1VS2;

            //        lbl_FailName2VS2.Text = "Mark";
            //        m_intFailCount2VS2 = m_smVSInfo[m_arrVisionIndex[1]].g_intMarkFailureTotal;
            //        if (m_intFailCount2VS2 != m_intFailCount2PrevVS2 && !blnReset)
            //        {
            //            if (m_intFailCount2VS2 != 0)
            //                lbl_FailCount2VS2.BackColor = Color.Red;
            //            else
            //                lbl_FailCount2VS2.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount2VS2.BackColor = Color.White;
            //        lbl_FailCount2VS2.Text = m_intFailCount2VS2.ToString();
            //        m_intFailCount2PrevVS2 = m_intFailCount2VS2;

            //        lbl_FailName3VS2.Text = "Package";
            //        m_intFailCount3VS2 = m_smVSInfo[m_arrVisionIndex[1]].g_intPackageFailureTotal;
            //        if (m_intFailCount3VS2 != m_intFailCount3PrevVS2 && !blnReset)
            //        {
            //            if (m_intFailCount3VS2 != 0)
            //                lbl_FailCount3VS2.BackColor = Color.Red;
            //            else
            //                lbl_FailCount3VS2.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount3VS2.BackColor = Color.White;
            //        lbl_FailCount3VS2.Text = m_intFailCount3VS2.ToString();
            //        m_intFailCount3PrevVS2 = m_intFailCount3VS2;

            //        lbl_FailName4VS2.Text = "Empty Unit";
            //        m_intFailCount4VS2 = m_smVSInfo[m_arrVisionIndex[1]].g_intCheckPresenceFailureTotal;
            //        if (m_intFailCount4VS2 != m_intFailCount4PrevVS2 && !blnReset)
            //        {
            //            if (m_intFailCount4VS2 != 0)
            //                lbl_FailCount4VS2.BackColor = Color.Red;
            //            else
            //                lbl_FailCount4VS2.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount4VS2.BackColor = Color.White;
            //        lbl_FailCount4VS2.Text = m_intFailCount4VS2.ToString();
            //        m_intFailCount4PrevVS2 = m_intFailCount4VS2;

            //        lbl_FailName5VS2.Text = "Pin1";
            //        m_intFailCount5VS2 = m_smVSInfo[m_arrVisionIndex[1]].g_intPin1FailureTotal;
            //        if (m_intFailCount5VS2 != m_intFailCount5PrevVS2 && !blnReset)
            //        {
            //            if (m_intFailCount5VS2 != 0)
            //                lbl_FailCount5VS2.BackColor = Color.Red;
            //            else
            //                lbl_FailCount5VS2.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount5VS2.BackColor = Color.White;
            //        lbl_FailCount5VS2.Text = m_intFailCount5VS2.ToString();
            //        m_intFailCount5PrevVS2 = m_intFailCount5VS2;
            //        break;
            //    case "UnitPresent":
            //    case "BottomPosition":
            //    case "BottomPositionOrient":
            //    case "TapePocketPosition":
            //        //Vision2
            //        lbl_FailName1VS2.Text = "Fail";
            //        m_intFailCount1VS2 = m_smVSInfo[m_arrVisionIndex[1]].g_intPositionFailureTotal;
            //        if (m_intFailCount1VS2 != m_intFailCount1PrevVS2 && !blnReset)
            //        {
            //            if (m_intFailCount1VS2 != 0)
            //                lbl_FailCount1VS2.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS2.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS2.BackColor = Color.White;
            //        lbl_FailCount1VS2.Text = m_intFailCount1VS2.ToString();
            //        m_intFailCount1PrevVS2 = m_intFailCount1VS2;
            //        lbl_FailName2VS2.Visible = false;
            //        lbl_FailCount2VS2.Visible = false;
            //        lbl_FailName3VS2.Visible = false;
            //        lbl_FailCount3VS2.Visible = false;
            //        lbl_FailName4VS2.Visible = false;
            //        lbl_FailCount4VS2.Visible = false;
            //        lbl_FailName5VS2.Visible = false;
            //        lbl_FailCount5VS2.Visible = false;
            //        break;
            //    case "Pad":
            //    case "PadPos":
            //    case "PadPkg":
            //    case "PadPkgPos":
            //    case "Pad5S":
            //    case "Pad5SPos":
            //    case "Pad5SPkg":
            //    case "Pad5SPkgPos":
            //    //Vision3
            //case "Li3D":
            //case "Li3DPkg":
            //        //Vision3Lead3D
            //        lbl_FailName1VS2.Text = "Empty Unit";
            //        m_intFailCount1VS2 = m_smVSInfo[m_arrVisionIndex[1]].g_intEmptyUnitFailureTotal;
            //        if (m_intFailCount1VS2 != m_intFailCount1PrevVS2 && !blnReset)
            //        {
            //            if (m_intFailCount1VS2 != 0)
            //                lbl_FailCount1VS2.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS2.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS2.BackColor = Color.White;
            //        lbl_FailCount1VS2.Text = m_intFailCount1VS2.ToString();
            //        m_intFailCount1PrevVS2 = m_intFailCount1VS2;

            //        lbl_FailName2VS2.Text = "Pad";
            //        m_intFailCount2VS2 = m_smVSInfo[m_arrVisionIndex[1]].g_intPadFailureTotal;
            //        if (m_intFailCount2VS2 != m_intFailCount2PrevVS2 && !blnReset)
            //        {
            //            if (m_intFailCount2VS2 != 0)
            //                lbl_FailCount2VS2.BackColor = Color.Red;
            //            else
            //                lbl_FailCount2VS2.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount2VS2.BackColor = Color.White;
            //        lbl_FailCount2VS2.Text = m_intFailCount2VS2.ToString();
            //        m_intFailCount2PrevVS2 = m_intFailCount2VS2;

            //        lbl_FailName3VS2.Text = "Position";
            //        m_intFailCount3VS2 = m_smVSInfo[m_arrVisionIndex[1]].g_intPositionFailureTotal;
            //        if (m_intFailCount3VS2 != m_intFailCount3PrevVS2 && !blnReset)
            //        {
            //            if (m_intFailCount3VS2 != 0)
            //                lbl_FailCount3VS2.BackColor = Color.Red;
            //            else
            //                lbl_FailCount3VS2.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount3VS2.BackColor = Color.White;
            //        lbl_FailCount3VS2.Text = m_intFailCount3VS2.ToString();
            //        m_intFailCount3PrevVS2 = m_intFailCount3VS2;

            //        lbl_FailName4VS2.Text = "Package";
            //        m_intFailCount4VS2 = m_smVSInfo[m_arrVisionIndex[1]].g_intPackageFailureTotal;
            //        if (m_intFailCount4VS2 != m_intFailCount4PrevVS2 && !blnReset)
            //        {
            //            if (m_intFailCount4VS2 != 0)
            //                lbl_FailCount4VS2.BackColor = Color.Red;
            //            else
            //                lbl_FailCount4VS2.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount4VS2.BackColor = Color.White;
            //        lbl_FailCount4VS2.Text = m_intFailCount4VS2.ToString();
            //        m_intFailCount4PrevVS2 = m_intFailCount4VS2;

            //        lbl_FailName5VS2.Text = "Pin1";
            //        m_intFailCount5VS2 = m_smVSInfo[m_arrVisionIndex[1]].g_intPin1FailureTotal;
            //        if (m_intFailCount5VS2 != m_intFailCount5PrevVS2 && !blnReset)
            //        {
            //            if (m_intFailCount5VS2 != 0)
            //                lbl_FailCount5VS2.BackColor = Color.Red;
            //            else
            //                lbl_FailCount5VS2.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount5VS2.BackColor = Color.White;
            //        lbl_FailCount5VS2.Text = m_intFailCount5VS2.ToString();
            //        m_intFailCount5PrevVS2 = m_intFailCount5VS2;
            //        break;
            //    case "InPocket":
            //    case "InPocketPkg":
            //    case "InPocketPkgPos":
            //    case "IPMLi":
            //    case "IPMLiPkg":
            //        //Vision4
            //        lbl_FailName1VS2.Text = "Orient";
            //        m_intFailCount1VS2 = m_smVSInfo[m_arrVisionIndex[1]].g_intOrientFailureTotal;
            //        if (m_intFailCount1VS2 != m_intFailCount1PrevVS2 && !blnReset)
            //        {
            //            if (m_intFailCount1VS2 != 0)
            //                lbl_FailCount1VS2.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS2.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS2.BackColor = Color.White;
            //        lbl_FailCount1VS2.Text = m_intFailCount1VS2.ToString();
            //        m_intFailCount1PrevVS2 = m_intFailCount1VS2;

            //        lbl_FailName2VS2.Text = "Mark";
            //        m_intFailCount2VS2 = m_smVSInfo[m_arrVisionIndex[1]].g_intMarkFailureTotal;
            //        if (m_intFailCount2VS2 != m_intFailCount2PrevVS2 && !blnReset)
            //        {
            //            if (m_intFailCount2VS2 != 0)
            //                lbl_FailCount2VS2.BackColor = Color.Red;
            //            else
            //                lbl_FailCount2VS2.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount2VS2.BackColor = Color.White;
            //        lbl_FailCount2VS2.Text = m_intFailCount2VS2.ToString();
            //        m_intFailCount2PrevVS2 = m_intFailCount2VS2;

            //        lbl_FailName3VS2.Text = "Package";
            //        m_intFailCount3VS2 = m_smVSInfo[m_arrVisionIndex[1]].g_intPackageFailureTotal;
            //        if (m_intFailCount3VS2 != m_intFailCount3PrevVS2 && !blnReset)
            //        {
            //            if (m_intFailCount3VS2 != 0)
            //                lbl_FailCount3VS2.BackColor = Color.Red;
            //            else
            //                lbl_FailCount3VS2.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount3VS2.BackColor = Color.White;
            //        lbl_FailCount3VS2.Text = m_intFailCount3VS2.ToString();
            //        m_intFailCount3PrevVS2 = m_intFailCount3VS2;

            //        lbl_FailName4VS2.Text = "Pin1";
            //        m_intFailCount4VS2 = m_smVSInfo[m_arrVisionIndex[1]].g_intPin1FailureTotal;
            //        if (m_intFailCount4VS2 != m_intFailCount4PrevVS2 && !blnReset)
            //        {
            //            if (m_intFailCount4VS2 != 0)
            //                lbl_FailCount4VS2.BackColor = Color.Red;
            //            else
            //                lbl_FailCount4VS2.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount4VS2.BackColor = Color.White;
            //        lbl_FailCount4VS2.Text = m_intFailCount4VS2.ToString();
            //        m_intFailCount4PrevVS2 = m_intFailCount4VS2;
            //        lbl_FailName5VS2.Visible = false;
            //        lbl_FailCount5VS2.Visible = false;
            //        break;
            //    case "Seal":
            //        //Vision6
            //        lbl_FailName1VS2.Text = "Seal";
            //        m_intFailCount1VS2 = m_smVSInfo[m_arrVisionIndex[1]].g_intSealFailureTotal;
            //        if (m_intFailCount1VS2 != m_intFailCount1PrevVS2 && !blnReset)
            //        {
            //            if (m_intFailCount1VS2 != 0)
            //                lbl_FailCount1VS2.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS2.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS2.BackColor = Color.White;
            //        lbl_FailCount1VS2.Text = m_intFailCount1VS2.ToString();
            //        m_intFailCount1PrevVS2 = m_intFailCount1VS2;
            //        lbl_FailName2VS2.Visible = false;
            //        lbl_FailCount2VS2.Visible = false;
            //        lbl_FailName3VS2.Visible = false;
            //        lbl_FailCount3VS2.Visible = false;
            //        lbl_FailName4VS2.Visible = false;
            //        lbl_FailCount4VS2.Visible = false;
            //        lbl_FailName5VS2.Visible = false;
            //        lbl_FailCount5VS2.Visible = false;
            //        break;

            //}
        }
        /// <summary>
        /// Display 3rd/7th Vision Station latest test result, pass counter, total tested counter, 
        /// grab time, processing time, and grab delay time that is set in camera setting form
        /// </summary>
        private void UpdateVision3(bool blnReset, bool blnFirstTime)
        {
            if (m_smVSInfo[m_arrVisionIndex[2]] == null)
                return;

            float fPercentage = 0.0f;

            m_intVS3TestedTotal = m_smVSInfo[m_arrVisionIndex[2]].g_intTestedTotal;
            if (m_intVS3TestedTotal != m_intVS3TestedTotalPrev)
            {
                m_intVS3PassCount = m_smVSInfo[m_arrVisionIndex[2]].g_intPassTotal;
                lbl_VS3PassResult.Text = m_intVS3PassCount.ToString();

                if (m_intVS3TestedTotal - m_intVS3PassCount < 0)
                    m_intVS3FailCount = 0;
                else
                    m_intVS3FailCount = m_intVS3TestedTotal - m_intVS3PassCount;

                lbl_VS3FailResult.Text = m_intVS3FailCount.ToString();

                lbl_VS3TotalResult.Text = m_intVS3TestedTotal.ToString();

                if (m_intVS3TestedTotal != 0)
                {
                    fPercentage = (m_intVS3PassCount / (float)m_intVS3TestedTotal) * 100;
                    lbl_VS3YieldResult.Text = fPercentage.ToString("F2");
                }
                else
                    lbl_VS3YieldResult.Text = "0.00";

                m_intVS3TestedTotalPrev = m_intVS3TestedTotal;
            }

            if (m_smVSInfo[m_arrVisionIndex[2]].g_strVisionName == "Barcode")
            {
                //if (!pnl_V3Detail.Controls.Contains(lbl_BarcodeResult))
                //{
                //    pnl_V3Detail.Controls.Add(lbl_BarcodeResult);
                //    lbl_BarcodeResult.Location = new Point(lbl_Result3.Location.X, lbl_Result3.Location.Y + lbl_Result3.Size.Height);
                //    lbl_BarcodeResult.BringToFront();
                //}
                if (!pnl_V3Detail.Controls.Contains(dgd_BarcodeResult))
                {
                    pnl_V3Detail.Controls.Add(dgd_BarcodeResult);
                    dgd_BarcodeResult.Location = new Point(lbl_Result3.Location.X, lbl_Result3.Location.Y + lbl_Result3.Size.Height);
                    dgd_BarcodeResult.BringToFront();
                }
                
                lbl_VS3PassResult.Text = "---";
                lbl_VS3FailResult.Text = "---";
                lbl_VS3TotalResult.Text = "---";
                lbl_VS3YieldResult.Text = "---";

                //m_strBarcodeResult = m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_strResultCode;
                //if (m_strBarcodeResult != m_strBarcodeResultPrev)
                //{
                //    if (m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnCodeFound && m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnCodePassed)
                //    {
                //        lbl_BarcodeResult.BackColor = Color.Lime;
                //        lbl_BarcodeResult.Text = m_strBarcodeResult;
                //        m_strBarcodeResultPrev = m_strBarcodeResult;
                //    }
                //    else if (m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnCodeNotMatched)
                //    {
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //        lbl_BarcodeResult.Text = m_strBarcodeResult;
                //        m_strBarcodeResultPrev = m_strBarcodeResult;
                //    }
                //    else if (m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnTestDone)
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
                //if (m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnCodeFound && m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnCodePassed)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Lime)
                //        lbl_BarcodeResult.BackColor = Color.Lime;
                //}
                //else if (m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnCodeNotMatched)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Red)
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //}
                //else if (m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnTestDone)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Red)
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //}
                //else
                //{
                //    //if (lbl_BarcodeResult.BackColor != Color.Yellow)
                //    //    lbl_BarcodeResult.BackColor = Color.Yellow;
                //}

                if (blnFirstTime)
                {
                    dgd_BarcodeResult.Rows.Clear();
                    for (int i = 0; i < m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_intTemplateCount; i++)
                    {
                        dgd_BarcodeResult.Rows.Add();
                        dgd_BarcodeResult.Rows[i].Cells[0].Value = (i + 1).ToString();
                        dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
                        m_strBarcodeResultPrev[i] = "----";
                        m_strBarcodeResult[i] = "----";
                    }
                }

                //dgd_BarcodeResult.Rows.Clear();
                for (int i = 0; i < m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_intTemplateCount; i++)
                {
                    if (i >= dgd_BarcodeResult.RowCount)
                    {
                        dgd_BarcodeResult.Rows.Add();
                        dgd_BarcodeResult.Rows[i].Cells[0].Value = (i + 1).ToString();
                        dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
                    }

                    m_strBarcodeResult[i] = m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_strResultCode[i];
                    if (m_strBarcodeResult[i] != m_strBarcodeResultPrev[i])
                    {
                        if (m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnCodeFound[i] && m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnCodePassed[i])
                        {
                            if (m_strBarcodeResult[i] != "----")
                            {
                                if (Math.Abs(m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_fBarcodeAngle[i] - m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_fBarcodeOrientationAngle) > 45)
                                {
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                                }
                                else
                                {
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                                }
                            }
                            else
                            {
                                dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                                dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                            }
                            //dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                            //dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = m_strBarcodeResult[i];
                            m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                        }
                        else if (m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnCodeNotMatched[i])
                        {
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = m_strBarcodeResult[i];
                            m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                        }
                        else if (m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnTestDone)
                        {
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
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
                    if (m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnCodeFound[i] && m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnCodePassed[i])
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Lime)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Lime)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    }
                    else if (m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnCodeNotMatched[i])
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    }
                    else if (m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_blnTestDone)
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        //if (lbl_BarcodeResult.BackColor != Color.Yellow)
                        //    lbl_BarcodeResult.BackColor = Color.Yellow;
                    }
                }
                if (blnFirstTime && dgd_BarcodeResult.RowCount == 0)
                {
                    dgd_BarcodeResult.Rows.Add();
                    dgd_BarcodeResult.Rows[0].Cells[0].Value = (1).ToString();
                    m_smVSInfo[m_arrVisionIndex[2]].g_objBarcode.ref_intTemplateCount = 1;
                    dgd_BarcodeResult.Rows[0].Cells[1].Value = "----";
                }
            }

            if (m_smVSInfo[m_arrVisionIndex[2]].g_blnNoGrabTime)
                m_fVS3GrabTime = 0;
            else
                m_fVS3GrabTime = Math.Max(0, m_smVSInfo[m_arrVisionIndex[2]].g_objGrabTime.Duration - m_smVSInfo[m_arrVisionIndex[2]].g_intCameraGrabDelay);  //29-05-2019 ZJYEOH : Need to minus grab delay as it included inside Grab time 
            if (m_fVS3GrabTime != m_fVS3GrabTimePrev)
            {

                lbl_V3GrabResult.Text = m_fVS3GrabTime.ToString("F0");
                m_fVS3GrabTimePrev = m_fVS3GrabTime;
            }

            m_fVS3ProcessTime = (Math.Max(0, m_smVSInfo[m_arrVisionIndex[2]].g_objTotalTime.Duration - m_smVSInfo[m_arrVisionIndex[2]].g_objGrabTime.Duration)); //m_smVSInfo[m_arrVisionIndex[2]].g_objTransferTime.Duration + m_smVSInfo[m_arrVisionIndex[2]].g_objProcessTime.Duration;
            if (m_fVS3ProcessTime != m_fVS3ProcessTimePrev)
            {
                if (m_blnSplitTime)
                {
                    lbl_V3ProcessResult.Text = m_smVSInfo[m_arrVisionIndex[2]].g_objTransferTime.Duration.ToString("F2") + "|" +
                                                         m_smVSInfo[m_arrVisionIndex[2]].g_objProcessTime.Duration.ToString("F2");
                }
                else
                {
                    lbl_V3ProcessResult.Text = m_fVS3ProcessTime.ToString("F2");
                }

                m_fVS3ProcessTimePrev = m_fVS3ProcessTime;
            }

            m_fVS3TotalTime = m_smVSInfo[m_arrVisionIndex[2]].g_objTotalTime.Duration;
            if (m_fVS3TotalTime != m_fVS3TotalTimePrev)
            {
                lbl_V3TotalTime.Text = m_fVS3TotalTime.ToString("F2");

                m_fVS3TotalTimePrev = m_fVS3TotalTime;
            }

            m_strVS3Result = m_smVSInfo[m_arrVisionIndex[2]].g_strResult;
            if (m_strVS3Result != m_strVS3ResultPrev)
            {
                if (m_strVS3Result == "Pass" || m_strVS3Result == "Empty")
                    lbl_V3Result.BackColor = Color.Lime;
                else if (m_strVS3Result == "Fail" || m_strVS3Result == "NoEmpty")
                    lbl_V3Result.BackColor = Color.Red;
                else if (m_strVS3Result == "Idle")
                {
                    lbl_V3Result.BackColor = Color.Yellow;
                }

                lbl_V3Result.Text = m_strVS3Result;
                m_strVS3ResultPrev = m_strVS3Result;
            }

            m_strVS3Result2 = m_smVSInfo[m_arrVisionIndex[2]].g_strResult2;
            if (m_strVS3Result2 != m_strVS3Result2Prev)
            {
                lbl_V3Result2.Text = m_strVS3Result2;
                switch (m_strVS3Result2)
                {
                    case "Pass":
                    case "Empty":
                        lbl_V3Result2.BackColor = Color.Lime;
                        break;
                    case "Fail":
                    case "NoEmpty":
                        lbl_V3Result2.BackColor = Color.Red;
                        break;
                    case "Idle":
                        {
                            lbl_V3Result2.BackColor = Color.Yellow;
                        }
                        break;
                }
                m_strVS3Result2Prev = m_strVS3Result2;
            }

            if (m_smVSInfo[m_arrVisionIndex[2]].g_blnNoGrabTime)
                m_intVS3GrabDelay = 0;
            else
                m_intVS3GrabDelay = m_smVSInfo[m_arrVisionIndex[2]].g_intCameraGrabDelay;
            if (m_intVS3GrabDelay != m_intVS3GrabDelayPrev)
            {
                lbl_GrabDelay3.Text = m_intVS3GrabDelay.ToString();
                m_intVS3GrabDelayPrev = m_intVS3GrabDelay;
            }

            //switch (m_smVSInfo[m_arrVisionIndex[2]].g_strVisionName)
            //{
            //    case "Orient":
            //    case "BottomOrient":
            //    case "Mark":
            //    case "MarkOrient":
            //    case "MarkPkg":
            //    case "MOPkg":
            //    case "MOLiPkg":
            //    case "MOLi":
            //    case "Package":
            //        //Vision1
            //        lbl_FailName1VS3.Text = "Orient";
            //        m_intFailCount1VS3 = m_smVSInfo[m_arrVisionIndex[2]].g_intOrientFailureTotal;
            //        if (m_intFailCount1VS3 != m_intFailCount1PrevVS3 && !blnReset)
            //        {
            //            if (m_intFailCount1VS3 != 0)
            //                lbl_FailCount1VS3.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS3.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS3.BackColor = Color.White;
            //        lbl_FailCount1VS3.Text = m_intFailCount1VS3.ToString();
            //        m_intFailCount1PrevVS3 = m_intFailCount1VS3;

            //        lbl_FailName2VS3.Text = "Mark";
            //        m_intFailCount2VS3 = m_smVSInfo[m_arrVisionIndex[2]].g_intMarkFailureTotal;
            //        if (m_intFailCount2VS3 != m_intFailCount2PrevVS3 && !blnReset)
            //        {
            //            if (m_intFailCount2VS3 != 0)
            //                lbl_FailCount2VS3.BackColor = Color.Red;
            //            else
            //                lbl_FailCount2VS3.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount2VS3.BackColor = Color.White;
            //        lbl_FailCount2VS3.Text = m_intFailCount2VS3.ToString();
            //        m_intFailCount2PrevVS3 = m_intFailCount2VS3;

            //        lbl_FailName3VS3.Text = "Package";
            //        m_intFailCount3VS3 = m_smVSInfo[m_arrVisionIndex[2]].g_intPackageFailureTotal;
            //        if (m_intFailCount3VS3 != m_intFailCount3PrevVS3 && !blnReset)
            //        {
            //            if (m_intFailCount3VS3 != 0)
            //                lbl_FailCount3VS3.BackColor = Color.Red;
            //            else
            //                lbl_FailCount3VS3.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount3VS3.BackColor = Color.White;
            //        lbl_FailCount3VS3.Text = m_intFailCount3VS3.ToString();
            //        m_intFailCount3PrevVS3 = m_intFailCount3VS3;

            //        lbl_FailName4VS3.Text = "Empty Unit";
            //        m_intFailCount4VS3 = m_smVSInfo[m_arrVisionIndex[2]].g_intCheckPresenceFailureTotal;
            //        if (m_intFailCount4VS3 != m_intFailCount4PrevVS3 && !blnReset)
            //        {
            //            if (m_intFailCount4VS3 != 0)
            //                lbl_FailCount4VS3.BackColor = Color.Red;
            //            else
            //                lbl_FailCount4VS3.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount4VS3.BackColor = Color.White;
            //        lbl_FailCount4VS3.Text = m_intFailCount4VS3.ToString();
            //        m_intFailCount4PrevVS3 = m_intFailCount4VS3;

            //        lbl_FailName5VS3.Text = "Pin1";
            //        m_intFailCount5VS3 = m_smVSInfo[m_arrVisionIndex[2]].g_intPin1FailureTotal;
            //        if (m_intFailCount5VS3 != m_intFailCount5PrevVS3 && !blnReset)
            //        {
            //            if (m_intFailCount5VS3 != 0)
            //                lbl_FailCount5VS3.BackColor = Color.Red;
            //            else
            //                lbl_FailCount5VS3.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount5VS3.BackColor = Color.White;
            //        lbl_FailCount5VS3.Text = m_intFailCount5VS3.ToString();
            //        m_intFailCount5PrevVS3 = m_intFailCount5VS3;
            //        break;
            //    case "UnitPresent":
            //    case "BottomPosition":
            //    case "BottomPositionOrient":
            //    case "TapePocketPosition":
            //        //Vision2
            //        lbl_FailName1VS3.Text = "Fail";
            //        m_intFailCount1VS3 = m_smVSInfo[m_arrVisionIndex[2]].g_intPositionFailureTotal;
            //        if (m_intFailCount1VS3 != m_intFailCount1PrevVS3 && !blnReset)
            //        {
            //            if (m_intFailCount1VS3 != 0)
            //                lbl_FailCount1VS3.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS3.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS3.BackColor = Color.White;
            //        lbl_FailCount1VS3.Text = m_intFailCount1VS3.ToString();
            //        m_intFailCount1PrevVS3 = m_intFailCount1VS3;
            //        lbl_FailName2VS3.Visible = false;
            //        lbl_FailCount2VS3.Visible = false;
            //        lbl_FailName3VS3.Visible = false;
            //        lbl_FailCount3VS3.Visible = false;
            //        lbl_FailName4VS3.Visible = false;
            //        lbl_FailCount4VS3.Visible = false;
            //        lbl_FailName5VS3.Visible = false;
            //        lbl_FailCount5VS3.Visible = false;
            //        break;
            //    case "Pad":
            //    case "PadPos":
            //    case "PadPkg":
            //    case "PadPkgPos":
            //    case "Pad5S":
            //    case "Pad5SPos":
            //    case "Pad5SPkg":
            //    case "Pad5SPkgPos":
            //    //Vision3
            //case "Li3D":
            //case "Li3DPkg":
            //        //Vision3Lead3D
            //        lbl_FailName1VS3.Text = "Empty Unit";
            //        m_intFailCount1VS3 = m_smVSInfo[m_arrVisionIndex[2]].g_intEmptyUnitFailureTotal;
            //        if (m_intFailCount1VS3 != m_intFailCount1PrevVS3 && !blnReset)
            //        {
            //            if (m_intFailCount1VS3 != 0)
            //                lbl_FailCount1VS3.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS3.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS3.BackColor = Color.White;
            //        lbl_FailCount1VS3.Text = m_intFailCount1VS3.ToString();
            //        m_intFailCount1PrevVS3 = m_intFailCount1VS3;

            //        lbl_FailName2VS3.Text = "Pad";
            //        m_intFailCount2VS3 = m_smVSInfo[m_arrVisionIndex[2]].g_intPadFailureTotal;
            //        if (m_intFailCount2VS3 != m_intFailCount2PrevVS3 && !blnReset)
            //        {
            //            if (m_intFailCount2VS3 != 0)
            //                lbl_FailCount2VS3.BackColor = Color.Red;
            //            else
            //                lbl_FailCount2VS3.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount2VS3.BackColor = Color.White;
            //        lbl_FailCount2VS3.Text = m_intFailCount2VS3.ToString();
            //        m_intFailCount2PrevVS3 = m_intFailCount2VS3;

            //        lbl_FailName3VS3.Text = "Position";
            //        m_intFailCount3VS3 = m_smVSInfo[m_arrVisionIndex[2]].g_intPositionFailureTotal;
            //        if (m_intFailCount3VS3 != m_intFailCount3PrevVS3 && !blnReset)
            //        {
            //            if (m_intFailCount3VS3 != 0)
            //                lbl_FailCount3VS3.BackColor = Color.Red;
            //            else
            //                lbl_FailCount3VS3.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount3VS3.BackColor = Color.White;
            //        lbl_FailCount3VS3.Text = m_intFailCount3VS3.ToString();
            //        m_intFailCount3PrevVS3 = m_intFailCount3VS3;

            //        lbl_FailName4VS3.Text = "Package";
            //        m_intFailCount4VS3 = m_smVSInfo[m_arrVisionIndex[2]].g_intPackageFailureTotal;
            //        if (m_intFailCount4VS3 != m_intFailCount4PrevVS3 && !blnReset)
            //        {
            //            if (m_intFailCount4VS3 != 0)
            //                lbl_FailCount4VS3.BackColor = Color.Red;
            //            else
            //                lbl_FailCount4VS3.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount4VS3.BackColor = Color.White;
            //        lbl_FailCount4VS3.Text = m_intFailCount4VS3.ToString();
            //        m_intFailCount4PrevVS3 = m_intFailCount4VS3;

            //        lbl_FailName5VS3.Text = "Pin1";
            //        m_intFailCount5VS3 = m_smVSInfo[m_arrVisionIndex[2]].g_intPin1FailureTotal;
            //        if (m_intFailCount5VS3 != m_intFailCount5PrevVS3 && !blnReset)
            //        {
            //            if (m_intFailCount5VS3 != 0)
            //                lbl_FailCount5VS3.BackColor = Color.Red;
            //            else
            //                lbl_FailCount5VS3.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount5VS3.BackColor = Color.White;
            //        lbl_FailCount5VS3.Text = m_intFailCount5VS3.ToString();
            //        m_intFailCount5PrevVS3 = m_intFailCount5VS3;
            //        break;
            //    case "InPocket":
            //    case "InPocketPkg":
            //    case "InPocketPkgPos":
            //    case "IPMLi":
            //    case "IPMLiPkg":
            //        //Vision4
            //        lbl_FailName1VS3.Text = "Orient";
            //        m_intFailCount1VS3 = m_smVSInfo[m_arrVisionIndex[2]].g_intOrientFailureTotal;
            //        if (m_intFailCount1VS3 != m_intFailCount1PrevVS3 && !blnReset)
            //        {
            //            if (m_intFailCount1VS3 != 0)
            //                lbl_FailCount1VS3.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS3.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS3.BackColor = Color.White;
            //        lbl_FailCount1VS3.Text = m_intFailCount1VS3.ToString();
            //        m_intFailCount1PrevVS3 = m_intFailCount1VS3;

            //        lbl_FailName2VS3.Text = "Mark";
            //        m_intFailCount2VS3 = m_smVSInfo[m_arrVisionIndex[2]].g_intMarkFailureTotal;
            //        if (m_intFailCount2VS3 != m_intFailCount2PrevVS3 && !blnReset)
            //        {
            //            if (m_intFailCount2VS3 != 0)
            //                lbl_FailCount2VS3.BackColor = Color.Red;
            //            else
            //                lbl_FailCount2VS3.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount2VS3.BackColor = Color.White;
            //        lbl_FailCount2VS3.Text = m_intFailCount2VS3.ToString();
            //        m_intFailCount2PrevVS3 = m_intFailCount2VS3;
            //        lbl_FailName3VS3.Text = "Package";
            //        m_intFailCount3VS3 = m_smVSInfo[m_arrVisionIndex[2]].g_intPackageFailureTotal;
            //        if (m_intFailCount3VS3 != m_intFailCount3PrevVS3 && !blnReset)
            //        {
            //            if (m_intFailCount3VS3 != 0)
            //                lbl_FailCount3VS3.BackColor = Color.Red;
            //            else
            //                lbl_FailCount3VS3.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount3VS3.BackColor = Color.White;
            //        lbl_FailCount3VS3.Text = m_intFailCount3VS3.ToString();
            //        m_intFailCount3PrevVS3 = m_intFailCount3VS3;

            //        lbl_FailName4VS3.Text = "Pin1";
            //        m_intFailCount4VS3 = m_smVSInfo[m_arrVisionIndex[2]].g_intPin1FailureTotal;
            //        if (m_intFailCount4VS3 != m_intFailCount4PrevVS3 && !blnReset)
            //        {
            //            if (m_intFailCount4VS3 != 0)
            //                lbl_FailCount4VS3.BackColor = Color.Red;
            //            else
            //                lbl_FailCount4VS3.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount4VS3.BackColor = Color.White;
            //        lbl_FailCount4VS3.Text = m_intFailCount4VS3.ToString();
            //        m_intFailCount4PrevVS3 = m_intFailCount4VS3;
            //        lbl_FailName5VS3.Visible = false;
            //        lbl_FailCount5VS3.Visible = false;
            //        break;
            //    case "Seal":
            //        //Vision6
            //        lbl_FailName1VS3.Text = "Seal";
            //        m_intFailCount1VS3 = m_smVSInfo[m_arrVisionIndex[2]].g_intSealFailureTotal;
            //        if (m_intFailCount1VS3 != m_intFailCount1PrevVS3 && !blnReset)
            //        {
            //            if (m_intFailCount1VS3 != 0)
            //                lbl_FailCount1VS3.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS3.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS3.BackColor = Color.White;
            //        lbl_FailCount1VS3.Text = m_intFailCount1VS3.ToString();
            //        m_intFailCount1PrevVS3 = m_intFailCount1VS3;
            //        lbl_FailName2VS3.Visible = false;
            //        lbl_FailCount2VS3.Visible = false;
            //        lbl_FailName3VS3.Visible = false;
            //        lbl_FailCount3VS3.Visible = false;
            //        lbl_FailName4VS3.Visible = false;
            //        lbl_FailCount4VS3.Visible = false;
            //        lbl_FailName5VS3.Visible = false;
            //        lbl_FailCount5VS3.Visible = false;
            //        break;

            //}
        }
        /// <summary>
        /// Display 4th/8th Vision Station latest test result, pass counter, total tested counter, 
        /// grab time, processing time, and grab delay time that is set in camera setting form
        /// </summary>
        private void UpdateVision4(bool blnReset, bool blnFirstTime)
        {
            if (m_smVSInfo[m_arrVisionIndex[3]] == null)
                return;

            float fPercentage = 0.0f;

            m_intVS4TestedTotal = m_smVSInfo[m_arrVisionIndex[3]].g_intTestedTotal;
            if (m_intVS4TestedTotal != m_intVS4TestedTotalPrev)
            {
                m_intVS4PassCount = m_smVSInfo[m_arrVisionIndex[3]].g_intPassTotal;
                lbl_VS4PassResult.Text = m_intVS4PassCount.ToString();

                if (m_intVS4TestedTotal - m_intVS4PassCount < 0)
                    m_intVS4FailCount = 0;
                else
                    m_intVS4FailCount = m_intVS4TestedTotal - m_intVS4PassCount;

                lbl_VS4FailResult.Text = m_intVS4FailCount.ToString();

                lbl_VS4TotalResult.Text = m_intVS4TestedTotal.ToString();

                if (m_intVS4TestedTotal != 0)
                {
                    fPercentage = (m_intVS4PassCount / (float)m_intVS4TestedTotal) * 100;
                    lbl_VS4YieldResult.Text = fPercentage.ToString("f2");
                }
                else
                    lbl_VS4YieldResult.Text = "0.00";

                m_intVS4TestedTotalPrev = m_intVS4TestedTotal;
            }

            if (m_smVSInfo[m_arrVisionIndex[3]].g_strVisionName == "Barcode")
            {
                //if (!pnl_V4Detail.Controls.Contains(lbl_BarcodeResult))
                //{
                //    pnl_V4Detail.Controls.Add(lbl_BarcodeResult);
                //    lbl_BarcodeResult.Location = new Point(lbl_Result4.Location.X, lbl_Result4.Location.Y + lbl_Result4.Size.Height);
                //    lbl_BarcodeResult.BringToFront();
                //}
                if (!pnl_V4Detail.Controls.Contains(dgd_BarcodeResult))
                {
                    pnl_V4Detail.Controls.Add(dgd_BarcodeResult);
                    dgd_BarcodeResult.Location = new Point(lbl_Result4.Location.X, lbl_Result4.Location.Y + lbl_Result4.Size.Height);
                    dgd_BarcodeResult.BringToFront();
                }
                
                lbl_VS4PassResult.Text = "---";
                lbl_VS4FailResult.Text = "---";
                lbl_VS4TotalResult.Text = "---";
                lbl_VS4YieldResult.Text = "---";

                //m_strBarcodeResult = m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_strResultCode;
                //if (m_strBarcodeResult != m_strBarcodeResultPrev)
                //{
                //    if (m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnCodeFound && m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnCodePassed)
                //    {
                //        lbl_BarcodeResult.BackColor = Color.Lime;
                //        lbl_BarcodeResult.Text = m_strBarcodeResult;
                //        m_strBarcodeResultPrev = m_strBarcodeResult;
                //    }
                //    else if (m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnCodeNotMatched)
                //    {
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //        lbl_BarcodeResult.Text = m_strBarcodeResult;
                //        m_strBarcodeResultPrev = m_strBarcodeResult;
                //    }
                //    else if (m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnTestDone)
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
                //if (m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnCodeFound && m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnCodePassed)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Lime)
                //        lbl_BarcodeResult.BackColor = Color.Lime;
                //}
                //else if (m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnCodeNotMatched)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Red)
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //}
                //else if (m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnTestDone)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Red)
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //}
                //else
                //{
                //    //if (lbl_BarcodeResult.BackColor != Color.Yellow)
                //    //    lbl_BarcodeResult.BackColor = Color.Yellow;
                //}

                if (blnFirstTime)
                {
                    dgd_BarcodeResult.Rows.Clear();
                    for (int i = 0; i < m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_intTemplateCount; i++)
                    {
                        dgd_BarcodeResult.Rows.Add();
                        dgd_BarcodeResult.Rows[i].Cells[0].Value = (i + 1).ToString();
                        dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
                        m_strBarcodeResultPrev[i] = "----";
                        m_strBarcodeResult[i] = "----";
                    }
                }

                //dgd_BarcodeResult.Rows.Clear();
                for (int i = 0; i < m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_intTemplateCount; i++)
                {
                    if (i >= dgd_BarcodeResult.RowCount)
                    {
                        dgd_BarcodeResult.Rows.Add();
                        dgd_BarcodeResult.Rows[i].Cells[0].Value = (i + 1).ToString();
                        dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
                    }

                    m_strBarcodeResult[i] = m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_strResultCode[i];
                    if (m_strBarcodeResult[i] != m_strBarcodeResultPrev[i])
                    {
                        if (m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnCodeFound[i] && m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnCodePassed[i])
                        {
                            if (m_strBarcodeResult[i] != "----")
                            {
                                if (Math.Abs(m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_fBarcodeAngle[i] - m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_fBarcodeOrientationAngle) > 45)
                                {
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                                }
                                else
                                {
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                                }
                            }
                            else
                            {
                                dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                                dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                            }
                            //dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                            //dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = m_strBarcodeResult[i];
                            m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                        }
                        else if (m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnCodeNotMatched[i])
                        {
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = m_strBarcodeResult[i];
                            m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                        }
                        else if (m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnTestDone)
                        {
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
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
                    if (m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnCodeFound[i] && m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnCodePassed[i])
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Lime)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Lime)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    }
                    else if (m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnCodeNotMatched[i])
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    }
                    else if (m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_blnTestDone)
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        //if (lbl_BarcodeResult.BackColor != Color.Yellow)
                        //    lbl_BarcodeResult.BackColor = Color.Yellow;
                    }
                }
                if (blnFirstTime && dgd_BarcodeResult.RowCount == 0)
                {
                    dgd_BarcodeResult.Rows.Add();
                    dgd_BarcodeResult.Rows[0].Cells[0].Value = (1).ToString();
                    m_smVSInfo[m_arrVisionIndex[3]].g_objBarcode.ref_intTemplateCount = 1;
                    dgd_BarcodeResult.Rows[0].Cells[1].Value = "----";
                }
            }

            if (m_smVSInfo[m_arrVisionIndex[3]].g_blnNoGrabTime)
                m_intVS4GrabDelay = 0;
            else
                m_fVS4GrabTime = Math.Max(0, m_smVSInfo[m_arrVisionIndex[3]].g_objGrabTime.Duration - m_smVSInfo[m_arrVisionIndex[3]].g_intCameraGrabDelay);  //29-05-2019 ZJYEOH : Need to minus grab delay as it included inside Grab time 
            if (m_fVS4GrabTime != m_fVS4GrabTimePrev)
            {
                lbl_V4GrabResult.Text = m_fVS4GrabTime.ToString("F0");
                m_fVS4GrabTimePrev = m_fVS4GrabTime;
            }

            m_fVS4ProcessTime = (Math.Max(0, m_smVSInfo[m_arrVisionIndex[3]].g_objTotalTime.Duration - m_smVSInfo[m_arrVisionIndex[3]].g_objGrabTime.Duration));// m_smVSInfo[m_arrVisionIndex[3]].g_objTransferTime.Duration + m_smVSInfo[m_arrVisionIndex[3]].g_objProcessTime.Duration;
            if (m_fVS4ProcessTime != m_fVS4ProcessTimePrev)
            {
                if (m_blnSplitTime)
                {
                    lbl_V4ProcessResult.Text = m_smVSInfo[m_arrVisionIndex[3]].g_objTransferTime.Duration.ToString("F2") + "|" +
                                                         m_smVSInfo[m_arrVisionIndex[3]].g_objProcessTime.Duration.ToString("F2");
                }
                else
                {
                    lbl_V4ProcessResult.Text = m_fVS4ProcessTime.ToString("F2");
                }

                m_fVS4ProcessTimePrev = m_fVS4ProcessTime;
            }

            m_fVS4TotalTime = m_smVSInfo[m_arrVisionIndex[3]].g_objTotalTime.Duration;
            if (m_fVS4TotalTime != m_fVS4TotalTimePrev)
            {
                lbl_V4TotalTime.Text = m_fVS4TotalTime.ToString("F2");

                m_fVS4TotalTimePrev = m_fVS4TotalTime;
            }

            m_strVS4Result = m_smVSInfo[m_arrVisionIndex[3]].g_strResult;
            if (m_strVS4Result != m_strVS4ResultPrev)
            {
                lbl_V4Result.Text = m_strVS4Result;
                switch (m_strVS4Result)
                {
                    case "Pass":
                    case "Empty":
                        lbl_V4Result.BackColor = Color.Lime;
                        break;
                    case "Fail":
                    case "NoEmpty":
                        lbl_V4Result.BackColor = Color.Red;
                        break;
                    case "Idle":
                        lbl_V4Result.BackColor = Color.Yellow;
                        break;
                }
                m_strVS4ResultPrev = m_strVS4Result;
            }
            
            if (m_smVSInfo[m_arrVisionIndex[3]].g_strResult == "Pass")
            {
                if (!m_smVSInfo[m_arrVisionIndex[3]].g_strVisionName.Contains("InPocket") && !m_smVSInfo[m_arrVisionIndex[3]].g_strVisionName.Contains("IPM")) // 2020-02-19 ZJYEOH : Inpocket will not display orient result
                {
                    if (m_smVSInfo[m_arrVisionIndex[3]].g_intOrientResult != null)
                    {
                        switch (m_smVSInfo[m_arrVisionIndex[3]].g_intOrientResult[0])
                        {
                            case 0:
                                lbl_V4Result.Text = "0";
                                break;
                            case 1:
                                lbl_V4Result.Text = "-90";
                                break;
                            case 2:
                                lbl_V4Result.Text = "180";
                                break;
                            case 3:
                                lbl_V4Result.Text = "90";
                                break;
                            case 4:
                                lbl_V4Result.Text = "Invalid";
                                break;
                        }
                    }
                }
            }

            m_strVS4Result2 = m_smVSInfo[m_arrVisionIndex[3]].g_strResult2;
            if (m_strVS4Result2 != m_strVS4Result2Prev)
            {
                lbl_V4Result2.Text = m_strVS4Result2;
                switch (m_strVS4Result2)
                {
                    case "Pass":
                    case "Empty":
                        lbl_V4Result2.BackColor = Color.Lime;
                        break;
                    case "Fail":
                    case "NoEmpty":
                        lbl_V4Result2.BackColor = Color.Red;
                        break;
                    case "Idle":
                        {
                            lbl_V4Result2.BackColor = Color.Yellow;
                        }
                        break;
                }
                m_strVS4Result2Prev = m_strVS4Result2;
            }

            if (m_smVSInfo[m_arrVisionIndex[3]].g_blnNoGrabTime)
                m_intVS4GrabDelay = 0;
            else
                m_intVS4GrabDelay = m_smVSInfo[m_arrVisionIndex[3]].g_intCameraGrabDelay;
            if (m_intVS4GrabDelay != m_intVS4GrabDelayPrev)
            {
                lbl_GrabDelay4.Text = m_intVS4GrabDelay.ToString();
                m_intVS4GrabDelayPrev = m_intVS4GrabDelay;
            }

            //switch (m_smVSInfo[m_arrVisionIndex[3]].g_strVisionName)
            //{
            //    case "Orient":
            //    case "BottomOrient":
            //    case "Mark":
            //    case "MarkOrient":
            //    case "MarkPkg":
            //    case "MOPkg":
            //    case "MOLiPkg":
            //    case "MOLi":
            //    case "Package":
            //        //Vision1
            //        lbl_FailName1VS4.Text = "Orient";
            //        m_intFailCount1VS4 = m_smVSInfo[m_arrVisionIndex[3]].g_intOrientFailureTotal;
            //        if (m_intFailCount1VS4 != m_intFailCount1PrevVS4 && !blnReset)
            //        {
            //            if (m_intFailCount1VS4 != 0)
            //                lbl_FailCount1VS4.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS4.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS4.BackColor = Color.White;
            //        lbl_FailCount1VS4.Text = m_intFailCount1VS4.ToString();
            //        m_intFailCount1PrevVS4 = m_intFailCount1VS4;

            //        lbl_FailName2VS4.Text = "Mark";
            //        m_intFailCount2VS4 = m_smVSInfo[m_arrVisionIndex[3]].g_intMarkFailureTotal;
            //        if (m_intFailCount2VS4 != m_intFailCount2PrevVS4 && !blnReset)
            //        {
            //            if (m_intFailCount2VS4 != 0)
            //                lbl_FailCount2VS4.BackColor = Color.Red;
            //            else
            //                lbl_FailCount2VS4.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount2VS4.BackColor = Color.White;
            //        lbl_FailCount2VS4.Text = m_intFailCount2VS4.ToString();
            //        m_intFailCount2PrevVS4 = m_intFailCount2VS4;

            //        lbl_FailName3VS4.Text = "Package";
            //        m_intFailCount3VS4 = m_smVSInfo[m_arrVisionIndex[3]].g_intPackageFailureTotal;
            //        if (m_intFailCount3VS4 != m_intFailCount3PrevVS4 && !blnReset)
            //        {
            //            if (m_intFailCount3VS4 != 0)
            //                lbl_FailCount3VS4.BackColor = Color.Red;
            //            else
            //                lbl_FailCount3VS4.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount3VS4.BackColor = Color.White;
            //        lbl_FailCount3VS4.Text = m_intFailCount3VS4.ToString();
            //        m_intFailCount3PrevVS4 = m_intFailCount3VS4;

            //        lbl_FailName4VS4.Text = "Empty Unit";
            //        m_intFailCount4VS4 = m_smVSInfo[m_arrVisionIndex[3]].g_intCheckPresenceFailureTotal;
            //        if (m_intFailCount4VS4 != m_intFailCount4PrevVS4 && !blnReset)
            //        {
            //            if (m_intFailCount4VS4 != 0)
            //                lbl_FailCount4VS4.BackColor = Color.Red;
            //            else
            //                lbl_FailCount4VS4.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount4VS4.BackColor = Color.White;
            //        lbl_FailCount4VS4.Text = m_intFailCount4VS4.ToString();
            //        m_intFailCount4PrevVS4 = m_intFailCount4VS4;

            //        lbl_FailName5VS4.Text = "Pin1";
            //        m_intFailCount5VS4 = m_smVSInfo[m_arrVisionIndex[3]].g_intPin1FailureTotal;
            //        if (m_intFailCount5VS4 != m_intFailCount5PrevVS4 && !blnReset)
            //        {
            //            if (m_intFailCount5VS4 != 0)
            //                lbl_FailCount5VS4.BackColor = Color.Red;
            //            else
            //                lbl_FailCount5VS4.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount5VS4.BackColor = Color.White;
            //        lbl_FailCount5VS4.Text = m_intFailCount5VS4.ToString();
            //        m_intFailCount5PrevVS4 = m_intFailCount5VS4;
            //        break;
            //    case "UnitPresent":
            //    case "BottomPosition":
            //    case "BottomPositionOrient":
            //    case "TapePocketPosition":
            //        //Vision2
            //        lbl_FailName1VS4.Text = "Fail";
            //        m_intFailCount1VS4 = m_smVSInfo[m_arrVisionIndex[3]].g_intPositionFailureTotal;
            //        if (m_intFailCount1VS4 != m_intFailCount1PrevVS4 && !blnReset)
            //        {
            //            if (m_intFailCount1VS4 != 0)
            //                lbl_FailCount1VS4.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS4.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS4.BackColor = Color.White;
            //        lbl_FailCount1VS4.Text = m_intFailCount1VS4.ToString();
            //        m_intFailCount1PrevVS4 = m_intFailCount1VS4;
            //        lbl_FailName2VS4.Visible = false;
            //        lbl_FailCount2VS4.Visible = false;
            //        lbl_FailName3VS4.Visible = false;
            //        lbl_FailCount3VS4.Visible = false;
            //        lbl_FailName4VS4.Visible = false;
            //        lbl_FailCount4VS4.Visible = false;
            //        lbl_FailName5VS4.Visible = false;
            //        lbl_FailCount5VS4.Visible = false;
            //        break;
            //    case "Pad":
            //    case "PadPos":
            //    case "PadPkg":
            //    case "PadPkgPos":
            //    case "Pad5S":
            //    case "Pad5SPos":
            //    case "Pad5SPkg":
            //    case "Pad5SPkgPos":
            //    //Vision3
            //case "Li3D":
            //case "Li3DPkg":
            //        //Vision3Lead3D
            //        lbl_FailName1VS4.Text = "Empty Unit";
            //        m_intFailCount1VS4 = m_smVSInfo[m_arrVisionIndex[3]].g_intEmptyUnitFailureTotal;
            //        if (m_intFailCount1VS4 != m_intFailCount1PrevVS4 && !blnReset)
            //        {
            //            if (m_intFailCount1VS4 != 0)
            //                lbl_FailCount1VS4.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS4.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS4.BackColor = Color.White;
            //        lbl_FailCount1VS4.Text = m_intFailCount1VS4.ToString();
            //        m_intFailCount1PrevVS4 = m_intFailCount1VS4;

            //        lbl_FailName2VS4.Text = "Pad";
            //        m_intFailCount2VS4 = m_smVSInfo[m_arrVisionIndex[3]].g_intPadFailureTotal;
            //        if (m_intFailCount2VS4 != m_intFailCount2PrevVS4 && !blnReset)
            //        {
            //            if (m_intFailCount2VS4 != 0)
            //                lbl_FailCount2VS4.BackColor = Color.Red;
            //            else
            //                lbl_FailCount2VS4.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount2VS4.BackColor = Color.White;
            //        lbl_FailCount2VS4.Text = m_intFailCount2VS4.ToString();
            //        m_intFailCount2PrevVS4 = m_intFailCount2VS4;

            //        lbl_FailName3VS4.Text = "Position";
            //        m_intFailCount3VS4 = m_smVSInfo[m_arrVisionIndex[3]].g_intPositionFailureTotal;
            //        if (m_intFailCount3VS4 != m_intFailCount3PrevVS4 && !blnReset)
            //        {
            //            if (m_intFailCount3VS4 != 0)
            //                lbl_FailCount3VS4.BackColor = Color.Red;
            //            else
            //                lbl_FailCount3VS4.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount3VS4.BackColor = Color.White;
            //        lbl_FailCount3VS4.Text = m_intFailCount3VS4.ToString();
            //        m_intFailCount3PrevVS4 = m_intFailCount3VS4;

            //        lbl_FailName4VS4.Text = "Package";
            //        m_intFailCount4VS4 = m_smVSInfo[m_arrVisionIndex[3]].g_intPackageFailureTotal;
            //        if (m_intFailCount4VS4 != m_intFailCount4PrevVS4 && !blnReset)
            //        {
            //            if (m_intFailCount4VS4 != 0)
            //                lbl_FailCount4VS4.BackColor = Color.Red;
            //            else
            //                lbl_FailCount4VS4.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount4VS4.BackColor = Color.White;
            //        lbl_FailCount4VS4.Text = m_intFailCount4VS4.ToString();
            //        m_intFailCount4PrevVS4 = m_intFailCount4VS4;

            //        lbl_FailName5VS4.Text = "Pin1";
            //        m_intFailCount5VS4 = m_smVSInfo[m_arrVisionIndex[3]].g_intPin1FailureTotal;
            //        if (m_intFailCount5VS4 != m_intFailCount5PrevVS4 && !blnReset)
            //        {
            //            if (m_intFailCount5VS4 != 0)
            //                lbl_FailCount5VS4.BackColor = Color.Red;
            //            else
            //                lbl_FailCount5VS4.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount5VS4.BackColor = Color.White;
            //        lbl_FailCount5VS4.Text = m_intFailCount5VS4.ToString();
            //        m_intFailCount5PrevVS4 = m_intFailCount5VS4;
            //        break;
            //    case "InPocket":
            //    case "InPocketPkg":
            //    case "InPocketPkgPos":
            //    case "IPMLi":
            //    case "IPMLiPkg":
            //        //Vision4
            //        lbl_FailName1VS4.Text = "Orient";
            //        m_intFailCount1VS4 = m_smVSInfo[m_arrVisionIndex[3]].g_intOrientFailureTotal;
            //        if (m_intFailCount1VS4 != m_intFailCount1PrevVS4 && !blnReset)
            //        {
            //            if (m_intFailCount1VS4 != 0)
            //                lbl_FailCount1VS4.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS4.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS4.BackColor = Color.White;
            //        lbl_FailCount1VS4.Text = m_intFailCount1VS4.ToString();
            //        m_intFailCount1PrevVS4 = m_intFailCount1VS4;

            //        lbl_FailName2VS4.Text = "Mark";
            //        m_intFailCount2VS4 = m_smVSInfo[m_arrVisionIndex[3]].g_intMarkFailureTotal;
            //        if (m_intFailCount2VS4 != m_intFailCount2PrevVS4 && !blnReset)
            //        {
            //            if (m_intFailCount2VS4 != 0)
            //                lbl_FailCount2VS4.BackColor = Color.Red;
            //            else
            //                lbl_FailCount2VS4.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount2VS4.BackColor = Color.White;
            //        lbl_FailCount2VS4.Text = m_intFailCount2VS4.ToString();
            //        m_intFailCount2PrevVS4 = m_intFailCount2VS4;

            //        lbl_FailName3VS4.Text = "Package";
            //        m_intFailCount3VS4 = m_smVSInfo[m_arrVisionIndex[3]].g_intPackageFailureTotal;
            //        if (m_intFailCount3VS4 != m_intFailCount3PrevVS4 && !blnReset)
            //        {
            //            if (m_intFailCount3VS4 != 0)
            //                lbl_FailCount3VS4.BackColor = Color.Red;
            //            else
            //                lbl_FailCount3VS4.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount3VS4.BackColor = Color.White;
            //        lbl_FailCount3VS4.Text = m_intFailCount3VS4.ToString();
            //        m_intFailCount3PrevVS4 = m_intFailCount3VS4;

            //        lbl_FailName4VS4.Text = "Pin1";
            //        m_intFailCount4VS4 = m_smVSInfo[m_arrVisionIndex[3]].g_intPin1FailureTotal;
            //        if (m_intFailCount4VS4 != m_intFailCount4PrevVS4 && !blnReset)
            //        {
            //            if (m_intFailCount4VS4 != 0)
            //                lbl_FailCount4VS4.BackColor = Color.Red;
            //            else
            //                lbl_FailCount4VS4.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount4VS4.BackColor = Color.White;
            //        lbl_FailCount4VS4.Text = m_intFailCount4VS4.ToString();
            //        m_intFailCount4PrevVS4 = m_intFailCount4VS4;
            //        lbl_FailName5VS4.Visible = false;
            //        lbl_FailCount5VS4.Visible = false;
            //        break;
            //    case "Seal":
            //        //Vision6
            //        lbl_FailName1VS4.Text = "Seal";
            //        m_intFailCount1VS4 = m_smVSInfo[m_arrVisionIndex[3]].g_intSealFailureTotal;
            //        if (m_intFailCount1VS4 != m_intFailCount1PrevVS4 && !blnReset)
            //        {
            //            if (m_intFailCount1VS4 != 0)
            //                lbl_FailCount1VS4.BackColor = Color.Red;
            //            else
            //                lbl_FailCount1VS4.BackColor = Color.White;
            //        }
            //        else
            //            lbl_FailCount1VS4.BackColor = Color.White;
            //        lbl_FailCount1VS4.Text = m_intFailCount1VS4.ToString();
            //        m_intFailCount1PrevVS4 = m_intFailCount1VS4;
            //        lbl_FailName2VS4.Visible = false;
            //        lbl_FailCount2VS4.Visible = false;
            //        lbl_FailName3VS4.Visible = false;
            //        lbl_FailCount3VS4.Visible = false;
            //        lbl_FailName4VS4.Visible = false;
            //        lbl_FailCount4VS4.Visible = false;
            //        lbl_FailName5VS4.Visible = false;
            //        lbl_FailCount5VS4.Visible = false;
            //        break;

            //}
        }

        private void UpdateVision5(bool blnReset, bool blnFirstTime)
        {
            if (m_smVSInfo[m_arrVisionIndex[4]] == null)
                return;

            float fPercentage = 0.0f;

            m_intVS5TestedTotal = m_smVSInfo[m_arrVisionIndex[4]].g_intTestedTotal;
            if (m_intVS5TestedTotal != m_intVS5TestedTotalPrev)
            {
                m_intVS5PassCount = m_smVSInfo[m_arrVisionIndex[4]].g_intPassTotal;
                lbl_VS5PassResult.Text = m_intVS5PassCount.ToString();

                if (m_intVS5TestedTotal - m_intVS5PassCount < 0)
                    m_intVS5FailCount = 0;
                else
                    m_intVS5FailCount = m_intVS5TestedTotal - m_intVS5PassCount;

                lbl_VS5FailResult.Text = m_intVS5FailCount.ToString();

                lbl_VS5TotalResult.Text = m_intVS5TestedTotal.ToString();

                if (m_intVS5TestedTotal != 0)
                {
                    fPercentage = (m_intVS5PassCount / (float)m_intVS5TestedTotal) * 100;
                    lbl_VS5YieldResult.Text = fPercentage.ToString("F2");
                }
                else
                    lbl_VS5YieldResult.Text = "0.00";

                m_intVS5TestedTotalPrev = m_intVS5TestedTotal;
            }

            if (m_smVSInfo[m_arrVisionIndex[4]].g_strVisionName == "Barcode")
            {
                //if (!pnl_V5Detail.Controls.Contains(lbl_BarcodeResult))
                //{
                //    pnl_V5Detail.Controls.Add(lbl_BarcodeResult);
                //    lbl_BarcodeResult.Location = new Point(lbl_Result5.Location.X, lbl_Result5.Location.Y + lbl_Result5.Size.Height);
                //    lbl_BarcodeResult.BringToFront();
                //}
                if (!pnl_V5Detail.Controls.Contains(dgd_BarcodeResult))
                {
                    pnl_V5Detail.Controls.Add(dgd_BarcodeResult);
                    dgd_BarcodeResult.Location = new Point(lbl_Result5.Location.X, lbl_Result5.Location.Y + lbl_Result5.Size.Height);
                    dgd_BarcodeResult.BringToFront();
                }
                
                lbl_VS5PassResult.Text = "---";
                lbl_VS5FailResult.Text = "---";
                lbl_VS5TotalResult.Text = "---";
                lbl_VS5YieldResult.Text = "---";

                //m_strBarcodeResult = m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_strResultCode;
                //if (m_strBarcodeResult != m_strBarcodeResultPrev)
                //{
                //    if (m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnCodeFound && m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnCodePassed)
                //    {
                //        lbl_BarcodeResult.BackColor = Color.Lime;
                //        lbl_BarcodeResult.Text = m_strBarcodeResult;
                //        m_strBarcodeResultPrev = m_strBarcodeResult;
                //    }
                //    else if (m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnCodeNotMatched)
                //    {
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //        lbl_BarcodeResult.Text = m_strBarcodeResult;
                //        m_strBarcodeResultPrev = m_strBarcodeResult;
                //    }
                //    else if (m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnTestDone)
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
                //if (m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnCodeFound && m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnCodePassed)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Lime)
                //        lbl_BarcodeResult.BackColor = Color.Lime;
                //}
                //else if (m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnCodeNotMatched)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Red)
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //}
                //else if (m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnTestDone)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Red)
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //}
                //else
                //{
                //    //if (lbl_BarcodeResult.BackColor != Color.Yellow)
                //    //    lbl_BarcodeResult.BackColor = Color.Yellow;
                //}

                if (blnFirstTime)
                {
                    dgd_BarcodeResult.Rows.Clear();
                    for (int i = 0; i < m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_intTemplateCount; i++)
                    {
                        dgd_BarcodeResult.Rows.Add();
                        dgd_BarcodeResult.Rows[i].Cells[0].Value = (i + 1).ToString();
                        dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
                        m_strBarcodeResultPrev[i] = "----";
                        m_strBarcodeResult[i] = "----";
                    }
                }

                //dgd_BarcodeResult.Rows.Clear();
                for (int i = 0; i < m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_intTemplateCount; i++)
                {
                    if (i >= dgd_BarcodeResult.RowCount)
                    {
                        dgd_BarcodeResult.Rows.Add();
                        dgd_BarcodeResult.Rows[i].Cells[0].Value = (i + 1).ToString();
                        dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
                    }

                    m_strBarcodeResult[i] = m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_strResultCode[i];
                    if (m_strBarcodeResult[i] != m_strBarcodeResultPrev[i])
                    {
                        if (m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnCodeFound[i] && m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnCodePassed[i])
                        {
                            if (m_strBarcodeResult[i] != "----")
                            {
                                if (Math.Abs(m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_fBarcodeAngle[i] - m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_fBarcodeOrientationAngle) > 45)
                                {
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                                }
                                else
                                {
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                                }
                            }
                            else
                            {
                                dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                                dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                            }
                            //dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                            //dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = m_strBarcodeResult[i];
                            m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                        }
                        else if (m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnCodeNotMatched[i])
                        {
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = m_strBarcodeResult[i];
                            m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                        }
                        else if (m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnTestDone)
                        {
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
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
                    if (m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnCodeFound[i] && m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnCodePassed[i])
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Lime)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Lime)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    }
                    else if (m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnCodeNotMatched[i])
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    }
                    else if (m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_blnTestDone)
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        //if (lbl_BarcodeResult.BackColor != Color.Yellow)
                        //    lbl_BarcodeResult.BackColor = Color.Yellow;
                    }
                }
                if (blnFirstTime && dgd_BarcodeResult.RowCount == 0)
                {
                    dgd_BarcodeResult.Rows.Add();
                    dgd_BarcodeResult.Rows[0].Cells[0].Value = (1).ToString();
                    m_smVSInfo[m_arrVisionIndex[4]].g_objBarcode.ref_intTemplateCount = 1;
                    dgd_BarcodeResult.Rows[0].Cells[1].Value = "----";
                }
            }

            if (m_smVSInfo[m_arrVisionIndex[4]].g_blnNoGrabTime)
                m_fVS5GrabTime = 0;
            else
                m_fVS5GrabTime = Math.Max(0, m_smVSInfo[m_arrVisionIndex[4]].g_objGrabTime.Duration - m_smVSInfo[m_arrVisionIndex[4]].g_intCameraGrabDelay);  //29-05-2019 ZJYEOH : Need to minus grab delay as it included inside Grab time 
            if (m_fVS5GrabTime != m_fVS5GrabTimePrev)
            {
                lbl_V5GrabResult.Text = m_fVS5GrabTime.ToString("F0");
                m_fVS5GrabTimePrev = m_fVS5GrabTime;
            }

            m_fVS5ProcessTime = (Math.Max(0, m_smVSInfo[m_arrVisionIndex[4]].g_objTotalTime.Duration - m_smVSInfo[m_arrVisionIndex[4]].g_objGrabTime.Duration)); //m_smVSInfo[m_arrVisionIndex[4]].g_objTransferTime.Duration + m_smVSInfo[m_arrVisionIndex[4]].g_objProcessTime.Duration;
            if (m_fVS5ProcessTime != m_fVS5ProcessTimePrev)
            {
                if (m_blnSplitTime)
                {
                    lbl_V5ProcessResult.Text = m_smVSInfo[m_arrVisionIndex[4]].g_objTransferTime.Duration.ToString("F2") + "|" +
                                                         m_smVSInfo[m_arrVisionIndex[4]].g_objProcessTime.Duration.ToString("F2");
                }
                else
                {
                    lbl_V5ProcessResult.Text = m_fVS5ProcessTime.ToString("F2");
                }

                m_fVS5ProcessTimePrev = m_fVS5ProcessTime;
            }

            m_fVS5TotalTime = m_smVSInfo[m_arrVisionIndex[4]].g_objTotalTime.Duration;
            if (m_fVS5TotalTime != m_fVS5TotalTimePrev)
            {
                lbl_V5TotalTime.Text = m_fVS5TotalTime.ToString("F2");

                m_fVS5TotalTimePrev = m_fVS5TotalTime;
            }

            m_strVS5Result = m_smVSInfo[m_arrVisionIndex[4]].g_strResult;
            if (m_strVS5Result != m_strVS5ResultPrev)
            {
                if (m_strVS5Result == "Pass" || m_strVS5Result == "Empty")
                    lbl_V5Result.BackColor = Color.Lime;
                else if (m_strVS5Result == "Fail" || m_strVS5Result == "NoEmpty")
                    lbl_V5Result.BackColor = Color.Red;
                else if (m_strVS5Result == "Idle")
                    lbl_V5Result.BackColor = Color.Yellow;

                lbl_V5Result.Text = m_strVS5Result;
                m_strVS5ResultPrev = m_strVS5Result;
            }

            if (m_smVSInfo[m_arrVisionIndex[4]].g_blnNoGrabTime)
                m_intVS5GrabDelay = 0;
            else
                m_intVS5GrabDelay = m_smVSInfo[m_arrVisionIndex[4]].g_intCameraGrabDelay;

            if (m_intVS5GrabDelay != m_intVS5GrabDelayPrev)
            {
                lbl_GrabDelay5.Text = m_intVS5GrabDelay.ToString();
                m_intVS5GrabDelayPrev = m_intVS5GrabDelay;
            }
        }

        private void UpdateVision6(bool blnReset, bool blnFirstTime)
        {
            if (m_smVSInfo[m_arrVisionIndex[5]] == null)
                return;

            float fPercentage = 0.0f;

            m_intVS6TestedTotal = m_smVSInfo[m_arrVisionIndex[5]].g_intTestedTotal;
            if (m_intVS6TestedTotal != m_intVS6TestedTotalPrev)
            {
                m_intVS6PassCount = m_smVSInfo[m_arrVisionIndex[5]].g_intPassTotal;
                lbl_VS6PassResult.Text = m_intVS6PassCount.ToString();

                if (m_intVS6TestedTotal - m_intVS6PassCount < 0)
                    m_intVS6FailCount = 0;
                else
                    m_intVS6FailCount = m_intVS6TestedTotal - m_intVS6PassCount;

                lbl_VS6FailResult.Text = m_intVS6FailCount.ToString();

                lbl_VS6TotalResult.Text = m_intVS6TestedTotal.ToString();

                if (m_intVS6TestedTotal != 0)
                {
                    fPercentage = (m_intVS6PassCount / (float)m_intVS6TestedTotal) * 100;
                    lbl_VS6YieldResult.Text = fPercentage.ToString("F2");
                }
                else
                    lbl_VS6YieldResult.Text = "0.00";

                m_intVS6TestedTotalPrev = m_intVS6TestedTotal;
            }

            if (m_smVSInfo[m_arrVisionIndex[5]].g_strVisionName == "Barcode")
            {
                if (!pnl_V6Detail.Controls.Contains(lbl_BarcodeResult))
                {
                    pnl_V6Detail.Controls.Add(lbl_BarcodeResult);
                    lbl_BarcodeResult.Location = new Point(lbl_Result6.Location.X, lbl_Result6.Location.Y + lbl_Result6.Size.Height);
                    lbl_BarcodeResult.BringToFront();
                }
                if (!pnl_V6Detail.Controls.Contains(dgd_BarcodeResult))
                {
                    pnl_V6Detail.Controls.Add(dgd_BarcodeResult);
                    dgd_BarcodeResult.Location = new Point(lbl_Result6.Location.X, lbl_Result6.Location.Y + lbl_Result6.Size.Height);
                    dgd_BarcodeResult.BringToFront();
                }
                
                lbl_VS6PassResult.Text = "---";
                lbl_VS6FailResult.Text = "---";
                lbl_VS6TotalResult.Text = "---";
                lbl_VS6YieldResult.Text = "---";

                //m_strBarcodeResult = m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_strResultCode;
                //if (m_strBarcodeResult != m_strBarcodeResultPrev)
                //{
                //    if (m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnCodeFound && m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnCodePassed)
                //    {
                //        lbl_BarcodeResult.BackColor = Color.Lime;
                //        lbl_BarcodeResult.Text = m_strBarcodeResult;
                //        m_strBarcodeResultPrev = m_strBarcodeResult;
                //    }
                //    else if (m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnCodeNotMatched)
                //    {
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //        lbl_BarcodeResult.Text = m_strBarcodeResult;
                //        m_strBarcodeResultPrev = m_strBarcodeResult;
                //    }
                //    else if (m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnTestDone)
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
                //if (m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnCodeFound && m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnCodePassed)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Lime)
                //        lbl_BarcodeResult.BackColor = Color.Lime;
                //}
                //else if (m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnCodeNotMatched)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Red)
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //}
                //else if (m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnTestDone)
                //{
                //    if (lbl_BarcodeResult.BackColor != Color.Red)
                //        lbl_BarcodeResult.BackColor = Color.Red;
                //}
                //else
                //{
                //    //if (lbl_BarcodeResult.BackColor != Color.Yellow)
                //    //    lbl_BarcodeResult.BackColor = Color.Yellow;
                //}

                if (blnFirstTime)
                {
                    dgd_BarcodeResult.Rows.Clear();
                    for (int i = 0; i < m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_intTemplateCount; i++)
                    {
                        dgd_BarcodeResult.Rows.Add();
                        dgd_BarcodeResult.Rows[i].Cells[0].Value = (i + 1).ToString();
                        dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
                        m_strBarcodeResultPrev[i] = "----";
                        m_strBarcodeResult[i] = "----";
                    }
                }

                //dgd_BarcodeResult.Rows.Clear();
                for (int i = 0; i < m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_intTemplateCount; i++)
                {
                    if (i >= dgd_BarcodeResult.RowCount)
                    {
                        dgd_BarcodeResult.Rows.Add();
                        dgd_BarcodeResult.Rows[i].Cells[0].Value = (i + 1).ToString();
                        dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
                    }

                    m_strBarcodeResult[i] = m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_strResultCode[i];
                    if (m_strBarcodeResult[i] != m_strBarcodeResultPrev[i])
                    {
                        if (m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnCodeFound[i] && m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnCodePassed[i])
                        {
                            if (m_strBarcodeResult[i] != "----")
                            {
                                if (Math.Abs(m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_fBarcodeAngle[i] - m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_fBarcodeOrientationAngle) > 45)
                                {
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                                }
                                else
                                {
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                                }
                            }
                            else
                            {
                                dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                                dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                            }
                            //dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                            //dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = m_strBarcodeResult[i];
                            m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                        }
                        else if (m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnCodeNotMatched[i])
                        {
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = m_strBarcodeResult[i];
                            m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                        }
                        else if (m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnTestDone)
                        {
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
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
                    if (m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnCodeFound[i] && m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnCodePassed[i])
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Lime)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Lime)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    }
                    else if (m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnCodeNotMatched[i])
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    }
                    else if (m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_blnTestDone)
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        //if (lbl_BarcodeResult.BackColor != Color.Yellow)
                        //    lbl_BarcodeResult.BackColor = Color.Yellow;
                    }
                }
                if (blnFirstTime && dgd_BarcodeResult.RowCount == 0)
                {
                    dgd_BarcodeResult.Rows.Add();
                    dgd_BarcodeResult.Rows[0].Cells[0].Value = (1).ToString();
                    m_smVSInfo[m_arrVisionIndex[5]].g_objBarcode.ref_intTemplateCount = 1;
                    dgd_BarcodeResult.Rows[0].Cells[1].Value = "----";
                }
            }

            if (m_smVSInfo[m_arrVisionIndex[5]].g_blnNoGrabTime)
                m_fVS6GrabTime = 0;
            else
                m_fVS6GrabTime = Math.Max(0, m_smVSInfo[m_arrVisionIndex[5]].g_objGrabTime.Duration - m_smVSInfo[m_arrVisionIndex[5]].g_intCameraGrabDelay);  //29-05-2019 ZJYEOH : Need to minus grab delay as it included inside Grab time 
            if (m_fVS6GrabTime != m_fVS6GrabTimePrev)
            {
                lbl_V6GrabResult.Text = m_fVS6GrabTime.ToString("F0");
                m_fVS6GrabTimePrev = m_fVS6GrabTime;
            }

            m_fVS6ProcessTime = (Math.Max(0, m_smVSInfo[m_arrVisionIndex[5]].g_objTotalTime.Duration - m_smVSInfo[m_arrVisionIndex[5]].g_objGrabTime.Duration)); //m_smVSInfo[m_arrVisionIndex[5]].g_objTransferTime.Duration + m_smVSInfo[m_arrVisionIndex[5]].g_objProcessTime.Duration;
            if (m_fVS6ProcessTime != m_fVS6ProcessTimePrev)
            {
                if (m_blnSplitTime)
                {
                    lbl_V6ProcessResult.Text = m_smVSInfo[m_arrVisionIndex[5]].g_objTransferTime.Duration.ToString("F2") + "|" +
                                                         m_smVSInfo[m_arrVisionIndex[5]].g_objProcessTime.Duration.ToString("F2");
                }
                else
                {
                    lbl_V6ProcessResult.Text = m_fVS6ProcessTime.ToString("F2");
                }

                m_fVS6ProcessTimePrev = m_fVS6ProcessTime;
            }

            m_fVS6TotalTime = m_smVSInfo[m_arrVisionIndex[5]].g_objTotalTime.Duration;
            if (m_fVS6TotalTime != m_fVS6TotalTimePrev)
            {
                lbl_V6TotalTime.Text = m_fVS6TotalTime.ToString("F2");

                m_fVS6TotalTimePrev = m_fVS6TotalTime;
            }

            m_strVS6Result = m_smVSInfo[m_arrVisionIndex[5]].g_strResult;
            if (m_strVS6Result != m_strVS6ResultPrev)
            {
                if (m_strVS6Result == "Pass" || m_strVS6Result == "Empty")
                    lbl_V6Result.BackColor = Color.Lime;
                else if (m_strVS6Result == "Fail" || m_strVS6Result == "NoEmpty")
                    lbl_V6Result.BackColor = Color.Red;
                else if (m_strVS6Result == "Idle")
                    lbl_V6Result.BackColor = Color.Yellow;

                lbl_V6Result.Text = m_strVS6Result;
                m_strVS6ResultPrev = m_strVS6Result;
            }

            if (m_smVSInfo[m_arrVisionIndex[5]].g_blnNoGrabTime)
                m_intVS6GrabDelay = 0;
            else
                m_intVS6GrabDelay = m_smVSInfo[m_arrVisionIndex[5]].g_intCameraGrabDelay;

            if (m_intVS6GrabDelay != m_intVS6GrabDelayPrev)
            {
                lbl_GrabDelay6.Text = m_intVS6GrabDelay.ToString();
                m_intVS6GrabDelayPrev = m_intVS6GrabDelay;
            }
        }
        private void UpdateVision7(bool blnReset, bool blnFirstTime)
        {
            if (m_smVSInfo[m_arrVisionIndex[6]] == null)
                return;

            float fPercentage = 0.0f;

            m_intVS7TestedTotal = m_smVSInfo[m_arrVisionIndex[6]].g_intTestedTotal;
            if (m_intVS7TestedTotal != m_intVS7TestedTotalPrev)
            {
                m_intVS7PassCount = m_smVSInfo[m_arrVisionIndex[6]].g_intPassTotal;
                lbl_VS7PassResult.Text = m_intVS7PassCount.ToString();

                if (m_intVS7TestedTotal - m_intVS7PassCount < 0)
                    m_intVS7FailCount = 0;
                else
                    m_intVS7FailCount = m_intVS7TestedTotal - m_intVS7PassCount;

                lbl_VS7FailResult.Text = m_intVS7FailCount.ToString();

                lbl_VS7TotalResult.Text = m_intVS7TestedTotal.ToString();

                if (m_intVS7TestedTotal != 0)
                {
                    fPercentage = (m_intVS7PassCount / (float)m_intVS7TestedTotal) * 100;
                    lbl_VS7YieldResult.Text = fPercentage.ToString("F2");
                }
                else
                    lbl_VS7YieldResult.Text = "0.00";

                m_intVS7TestedTotalPrev = m_intVS7TestedTotal;
            }

            if (m_smVSInfo[m_arrVisionIndex[6]].g_strVisionName == "Barcode")
            {
                if (!pnl_V7Detail.Controls.Contains(lbl_BarcodeResult))
                {
                    pnl_V7Detail.Controls.Add(lbl_BarcodeResult);
                    lbl_BarcodeResult.Location = new Point(lbl_Result7.Location.X, lbl_Result7.Location.Y + lbl_Result7.Size.Height);
                    lbl_BarcodeResult.BringToFront();
                }
                if (!pnl_V7Detail.Controls.Contains(dgd_BarcodeResult))
                {
                    pnl_V7Detail.Controls.Add(dgd_BarcodeResult);
                    dgd_BarcodeResult.Location = new Point(lbl_Result7.Location.X, lbl_Result7.Location.Y + lbl_Result7.Size.Height);
                    dgd_BarcodeResult.BringToFront();
                }

                lbl_VS7PassResult.Text = "---";
                lbl_VS7FailResult.Text = "---";
                lbl_VS7TotalResult.Text = "---";
                lbl_VS7YieldResult.Text = "---";
                
                if (blnFirstTime)
                {
                    dgd_BarcodeResult.Rows.Clear();
                    for (int i = 0; i < m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_intTemplateCount; i++)
                    {
                        dgd_BarcodeResult.Rows.Add();
                        dgd_BarcodeResult.Rows[i].Cells[0].Value = (i + 1).ToString();
                        dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
                        m_strBarcodeResultPrev[i] = "----";
                        m_strBarcodeResult[i] = "----";
                    }
                }

                //dgd_BarcodeResult.Rows.Clear();
                for (int i = 0; i < m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_intTemplateCount; i++)
                {
                    if (i >= dgd_BarcodeResult.RowCount)
                    {
                        dgd_BarcodeResult.Rows.Add();
                        dgd_BarcodeResult.Rows[i].Cells[0].Value = (i + 1).ToString();
                        dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
                    }

                    m_strBarcodeResult[i] = m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_strResultCode[i];
                    if (m_strBarcodeResult[i] != m_strBarcodeResultPrev[i])
                    {
                        if (m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_blnCodeFound[i] && m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_blnCodePassed[i])
                        {
                            if (m_strBarcodeResult[i] != "----")
                            {
                                if (Math.Abs(m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_fBarcodeAngle[i] - m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_fBarcodeOrientationAngle) > 45)
                                {
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                                }
                                else
                                {
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                                    dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                                }
                            }
                            else
                            {
                                dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                                dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                            }
                            //dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                            //dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = m_strBarcodeResult[i];
                            m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                        }
                        else if (m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_blnCodeNotMatched[i])
                        {
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = m_strBarcodeResult[i];
                            m_strBarcodeResultPrev[i] = m_strBarcodeResult[i];
                        }
                        else if (m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_blnTestDone)
                        {
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.BackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                            dgd_BarcodeResult.Rows[i].Cells[1].Value = "----";
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
                    if (m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_blnCodeFound[i] && m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_blnCodePassed[i])
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Lime)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Lime)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    }
                    else if (m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_blnTestDone && m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_blnCodeNotMatched[i])
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    }
                    else if (m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_blnTestDone)
                    {
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        if (dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor != Color.Red)
                            dgd_BarcodeResult.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        //if (lbl_BarcodeResult.BackColor != Color.Yellow)
                        //    lbl_BarcodeResult.BackColor = Color.Yellow;
                    }
                }
                if (blnFirstTime && dgd_BarcodeResult.RowCount == 0)
                {
                    dgd_BarcodeResult.Rows.Add();
                    dgd_BarcodeResult.Rows[0].Cells[0].Value = (1).ToString();
                    m_smVSInfo[m_arrVisionIndex[6]].g_objBarcode.ref_intTemplateCount = 1;
                    dgd_BarcodeResult.Rows[0].Cells[1].Value = "----";
                }
            }

            if (m_smVSInfo[m_arrVisionIndex[6]].g_blnNoGrabTime)
                m_fVS7GrabTime = 0;
            else
                m_fVS7GrabTime = Math.Max(0, m_smVSInfo[m_arrVisionIndex[6]].g_objGrabTime.Duration - m_smVSInfo[m_arrVisionIndex[6]].g_intCameraGrabDelay);  //29-05-2019 ZJYEOH : Need to minus grab delay as it included inside Grab time 
            if (m_fVS7GrabTime != m_fVS7GrabTimePrev)
            {
                lbl_V7GrabResult.Text = m_fVS7GrabTime.ToString("F0");
                m_fVS7GrabTimePrev = m_fVS7GrabTime;
            }

            m_fVS7ProcessTime = (Math.Max(0, m_smVSInfo[m_arrVisionIndex[6]].g_objTotalTime.Duration - m_smVSInfo[m_arrVisionIndex[6]].g_objGrabTime.Duration)); //m_smVSInfo[m_arrVisionIndex[5]].g_objTransferTime.Duration + m_smVSInfo[m_arrVisionIndex[5]].g_objProcessTime.Duration;
            if (m_fVS7ProcessTime != m_fVS7ProcessTimePrev)
            {
                if (m_blnSplitTime)
                {
                    lbl_V7ProcessResult.Text = m_smVSInfo[m_arrVisionIndex[6]].g_objTransferTime.Duration.ToString("F2") + "|" +
                                                         m_smVSInfo[m_arrVisionIndex[6]].g_objProcessTime.Duration.ToString("F2");
                }
                else
                {
                    lbl_V7ProcessResult.Text = m_fVS7ProcessTime.ToString("F2");
                }

                m_fVS7ProcessTimePrev = m_fVS7ProcessTime;
            }

            m_fVS7TotalTime = m_smVSInfo[m_arrVisionIndex[6]].g_objTotalTime.Duration;
            if (m_fVS7TotalTime != m_fVS7TotalTimePrev)
            {
                lbl_V7TotalTime.Text = m_fVS7TotalTime.ToString("F2");

                m_fVS7TotalTimePrev = m_fVS7TotalTime;
            }

            m_strVS7Result = m_smVSInfo[m_arrVisionIndex[6]].g_strResult;
            if (m_strVS7Result != m_strVS7ResultPrev)
            {
                if (m_strVS7Result == "Pass" || m_strVS7Result == "Empty")
                    lbl_V7Result.BackColor = Color.Lime;
                else if (m_strVS7Result == "Fail" || m_strVS7Result == "NoEmpty")
                    lbl_V7Result.BackColor = Color.Red;
                else if (m_strVS7Result == "Idle")
                    lbl_V7Result.BackColor = Color.Yellow;

                lbl_V7Result.Text = m_strVS7Result;
                m_strVS7ResultPrev = m_strVS7Result;
            }

            if (m_smVSInfo[m_arrVisionIndex[6]].g_blnNoGrabTime)
                m_intVS7GrabDelay = 0;
            else
                m_intVS7GrabDelay = m_smVSInfo[m_arrVisionIndex[6]].g_intCameraGrabDelay;

            if (m_intVS7GrabDelay != m_intVS7GrabDelayPrev)
            {
                lbl_GrabDelay7.Text = m_intVS7GrabDelay.ToString();
                m_intVS7GrabDelayPrev = m_intVS7GrabDelay;
            }
        }
        /// <summary>
        /// Draw all grabed/loaded image for all available vision stations in 50% size of original size
        /// </summary>
        private void UpdatePictureBox()
        {
            if (m_smVSInfo[m_arrVisionIndex[0]] != null)
            {
                if (m_smVSInfo[m_arrVisionIndex[0]].VS_VM_UpdateSmallPictureBox || m_smVSInfo[m_arrVisionIndex[0]].ALL_VM_UpdatePictureBox)
                {
                    if (m_smVSInfo[m_arrVisionIndex[0]].g_intSelectedImage >= m_smVSInfo[m_arrVisionIndex[0]].g_arrImages.Count)
                        m_smVSInfo[m_arrVisionIndex[0]].g_intSelectedImage = 0;

                    if (m_smVSInfo[m_arrVisionIndex[0]].g_blnViewColorImage)
                    {
                        //m_smVSInfo[m_arrVisionIndex[0]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[0]].g_intSelectedImage].DrawZoomImage(m_objVS1Graphics, 0.5f);
                        if (m_smVSInfo[m_arrVisionIndex[0]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[0]].g_arrColorImages.Count)
                        {
                            float ScaleX = Station1Panel.Width / (float)m_smVSInfo[m_arrVisionIndex[0]].g_intCameraResolutionWidth;
                            float ScaleY = Station1Panel.Height / (float)m_smVSInfo[m_arrVisionIndex[0]].g_intCameraResolutionHeight;
                            float Scale = Math.Min(ScaleX, ScaleY);
                            //Scale = Math.Min(Scale, 1f);

                            Station1Panel.Size = new Size((int)(m_smVSInfo[m_arrVisionIndex[0]].g_intCameraResolutionWidth * Scale), (int)(m_smVSInfo[m_arrVisionIndex[0]].g_intCameraResolutionHeight * Scale));
                            Station1Panel.Location = new Point(pnl_Station1.Width / 2 - Station1Panel.Width / 2, pnl_Station1.Height / 2 - Station1Panel.Height / 2);
                            if (!m_smVSInfo[m_arrVisionIndex[0]].g_blnInspectionInProgress)
                            {
                                RedrawSubPanelImage(m_smVSInfo[m_arrVisionIndex[0]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[0]].g_intSelectedImage], m_objVS1Graphics, Station1Panel.Width, Station1Panel.Height);
                            }
                        }
                    }
                    else
                    {
                        if (m_smVSInfo[m_arrVisionIndex[0]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[0]].g_arrImages.Count)
                        {                        //m_smVSInfo[m_arrVisionIndex[0]].g_arrImages[m_smVSInfo[m_arrVisionIndex[0]].g_intSelectedImage].DrawZoomImage(m_objVS1Graphics, 0.5f);
                            float ScaleX = Station1Panel.Width / (float)m_smVSInfo[m_arrVisionIndex[0]].g_intCameraResolutionWidth;
                            float ScaleY = Station1Panel.Height / (float)m_smVSInfo[m_arrVisionIndex[0]].g_intCameraResolutionHeight;
                            float Scale = Math.Min(ScaleX, ScaleY);
                            //Scale = Math.Min(Scale, 1f);

                            Station1Panel.Size = new Size((int)(m_smVSInfo[m_arrVisionIndex[0]].g_intCameraResolutionWidth * Scale), (int)(m_smVSInfo[m_arrVisionIndex[0]].g_intCameraResolutionHeight * Scale));
                            Station1Panel.Location = new Point(pnl_Station1.Width / 2 - Station1Panel.Width / 2, pnl_Station1.Height / 2 - Station1Panel.Height / 2);
                            if (!m_smVSInfo[m_arrVisionIndex[0]].g_blnInspectionInProgress)
                            {
                                RedrawSubPanelImage(m_smVSInfo[m_arrVisionIndex[0]].g_arrImages[m_smVSInfo[m_arrVisionIndex[0]].g_intSelectedImage], m_objVS1Graphics, Station1Panel.Width, Station1Panel.Height);
                            }
                        }
                    }
                    m_smVSInfo[m_arrVisionIndex[0]].VS_VM_UpdateSmallPictureBox = false;
                }
            }

            if (m_smVSInfo[m_arrVisionIndex[1]] != null)
            {
                if (m_smVSInfo[m_arrVisionIndex[1]].VS_VM_UpdateSmallPictureBox || m_smVSInfo[m_arrVisionIndex[1]].ALL_VM_UpdatePictureBox)
                {
                    if (m_smVSInfo[m_arrVisionIndex[1]].g_intSelectedImage >= m_smVSInfo[m_arrVisionIndex[1]].g_arrImages.Count)
                        m_smVSInfo[m_arrVisionIndex[1]].g_intSelectedImage = 0;

                    if (m_smVSInfo[m_arrVisionIndex[1]].g_blnViewColorImage)
                    {
                        //m_smVSInfo[m_arrVisionIndex[1]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[1]].g_intSelectedImage].DrawZoomImage(m_objVS2Graphics, 0.5f);
                        if (m_smVSInfo[m_arrVisionIndex[1]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[1]].g_arrColorImages.Count)
                        {
                            float ScaleX = Station2Panel.Width / (float)m_smVSInfo[m_arrVisionIndex[1]].g_intCameraResolutionWidth;
                            float ScaleY = Station2Panel.Height / (float)m_smVSInfo[m_arrVisionIndex[1]].g_intCameraResolutionHeight;
                            float Scale = Math.Min(ScaleX, ScaleY);
                            //Scale = Math.Min(Scale, 1f);

                            Station2Panel.Size = new Size((int)(m_smVSInfo[m_arrVisionIndex[1]].g_intCameraResolutionWidth * Scale), (int)(m_smVSInfo[m_arrVisionIndex[1]].g_intCameraResolutionHeight * Scale));
                            Station2Panel.Location = new Point(pnl_Station2.Width / 2 - Station2Panel.Width / 2, pnl_Station2.Height / 2 - Station2Panel.Height / 2);
                            if (!m_smVSInfo[m_arrVisionIndex[1]].g_blnInspectionInProgress)
                            {
                                RedrawSubPanelImage(m_smVSInfo[m_arrVisionIndex[1]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[1]].g_intSelectedImage], m_objVS2Graphics, Station2Panel.Width, Station2Panel.Height);
                            }
                        }
                    }
                    else
                    {
                        if (m_smVSInfo[m_arrVisionIndex[1]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[1]].g_arrImages.Count)
                        {
                            float ScaleX = Station2Panel.Width / (float)m_smVSInfo[m_arrVisionIndex[1]].g_intCameraResolutionWidth;
                            float ScaleY = Station2Panel.Height / (float)m_smVSInfo[m_arrVisionIndex[1]].g_intCameraResolutionHeight;
                            float Scale = Math.Min(ScaleX, ScaleY);
                            //Scale = Math.Min(Scale, 1f);

                            Station2Panel.Size = new Size((int)(m_smVSInfo[m_arrVisionIndex[1]].g_intCameraResolutionWidth * Scale), (int)(m_smVSInfo[m_arrVisionIndex[1]].g_intCameraResolutionHeight * Scale));
                            Station2Panel.Location = new Point(pnl_Station2.Width / 2 - Station2Panel.Width / 2, pnl_Station2.Height / 2 - Station2Panel.Height / 2);
                            if (!m_smVSInfo[m_arrVisionIndex[1]].g_blnInspectionInProgress)
                            {
                                RedrawSubPanelImage(m_smVSInfo[m_arrVisionIndex[1]].g_arrImages[m_smVSInfo[m_arrVisionIndex[1]].g_intSelectedImage], m_objVS2Graphics, Station2Panel.Width, Station2Panel.Height);
                            }
                        }
                    }
                    //m_smVSInfo[m_arrVisionIndex[1]].g_arrImages[m_smVSInfo[m_arrVisionIndex[1]].g_intSelectedImage].DrawZoomImage(m_objVS2Graphics, 0.25f);

                    m_smVSInfo[m_arrVisionIndex[1]].VS_VM_UpdateSmallPictureBox = false;
                }
            }

            if (m_smVSInfo[m_arrVisionIndex[2]] != null)
            {
                if (m_smVSInfo[m_arrVisionIndex[2]].VS_VM_UpdateSmallPictureBox || m_smVSInfo[m_arrVisionIndex[2]].ALL_VM_UpdatePictureBox)
                {
                    if (m_smVSInfo[m_arrVisionIndex[2]].g_intSelectedImage >= m_smVSInfo[m_arrVisionIndex[2]].g_arrImages.Count)
                        m_smVSInfo[m_arrVisionIndex[2]].g_intSelectedImage = 0;

                    if (m_smVSInfo[m_arrVisionIndex[2]].g_blnViewColorImage)
                    {
                        //m_smVSInfo[m_arrVisionIndex[2]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[2]].g_intSelectedImage].DrawZoomImage(m_objVS3Graphics, 0.5f);
                        if (m_smVSInfo[m_arrVisionIndex[2]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[2]].g_arrColorImages.Count)
                        {
                            float ScaleX = Station3Panel.Width / (float)m_smVSInfo[m_arrVisionIndex[2]].g_intCameraResolutionWidth;
                            float ScaleY = Station3Panel.Height / (float)m_smVSInfo[m_arrVisionIndex[2]].g_intCameraResolutionHeight;
                            float Scale = Math.Min(ScaleX, ScaleY);
                            //Scale = Math.Min(Scale, 1f);

                            Station3Panel.Size = new Size((int)(m_smVSInfo[m_arrVisionIndex[2]].g_intCameraResolutionWidth * Scale), (int)(m_smVSInfo[m_arrVisionIndex[2]].g_intCameraResolutionHeight * Scale));
                            Station3Panel.Location = new Point(pnl_Station3.Width / 2 - Station3Panel.Width / 2, pnl_Station3.Height / 2 - Station3Panel.Height / 2);
                            if (!m_smVSInfo[m_arrVisionIndex[2]].g_blnInspectionInProgress)
                            {
                                RedrawSubPanelImage(m_smVSInfo[m_arrVisionIndex[2]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[2]].g_intSelectedImage], m_objVS3Graphics, Station3Panel.Width, Station3Panel.Height);
                            }
                        }
                    }
                    else
                    {
                        if (m_smVSInfo[m_arrVisionIndex[2]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[2]].g_arrImages.Count)
                        {
                            float ScaleX = Station3Panel.Width / (float)m_smVSInfo[m_arrVisionIndex[2]].g_intCameraResolutionWidth;
                            float ScaleY = Station3Panel.Height / (float)m_smVSInfo[m_arrVisionIndex[2]].g_intCameraResolutionHeight;
                            float Scale = Math.Min(ScaleX, ScaleY);
                            //Scale = Math.Min(Scale, 1f);

                            Station3Panel.Size = new Size((int)(m_smVSInfo[m_arrVisionIndex[2]].g_intCameraResolutionWidth * Scale), (int)(m_smVSInfo[m_arrVisionIndex[2]].g_intCameraResolutionHeight * Scale));
                            Station3Panel.Location = new Point(pnl_Station3.Width / 2 - Station3Panel.Width / 2, pnl_Station3.Height / 2 - Station3Panel.Height / 2);
                            if (!m_smVSInfo[m_arrVisionIndex[2]].g_blnInspectionInProgress)
                            {
                                RedrawSubPanelImage(m_smVSInfo[m_arrVisionIndex[2]].g_arrImages[m_smVSInfo[m_arrVisionIndex[2]].g_intSelectedImage], m_objVS3Graphics, Station3Panel.Width, Station3Panel.Height);
                            }
                        }
                    }
                    //m_smVSInfo[m_arrVisionIndex[2]].g_arrImages[m_smVSInfo[m_arrVisionIndex[2]].g_intSelectedImage].DrawZoomImage(m_objVS3Graphics, 0.25f);

                    m_smVSInfo[m_arrVisionIndex[2]].VS_VM_UpdateSmallPictureBox = false;
                }
            }

            if (m_smVSInfo[m_arrVisionIndex[3]] != null)
            {
                if (m_smVSInfo[m_arrVisionIndex[3]].VS_VM_UpdateSmallPictureBox || m_smVSInfo[m_arrVisionIndex[3]].ALL_VM_UpdatePictureBox)
                {
                    if (m_smVSInfo[m_arrVisionIndex[3]].g_intSelectedImage >= m_smVSInfo[m_arrVisionIndex[3]].g_arrImages.Count)
                        m_smVSInfo[m_arrVisionIndex[3]].g_intSelectedImage = 0;

                    if (m_smVSInfo[m_arrVisionIndex[3]].g_blnViewColorImage)
                    {
                        //m_smVSInfo[m_arrVisionIndex[3]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[3]].g_intSelectedImage].DrawZoomImage(m_objVS4Graphics, 0.5f);
                        if (m_smVSInfo[m_arrVisionIndex[3]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[3]].g_arrColorImages.Count)
                        {
                            float ScaleX = Station4Panel.Width / (float)m_smVSInfo[m_arrVisionIndex[3]].g_intCameraResolutionWidth;
                            float ScaleY = Station4Panel.Height / (float)m_smVSInfo[m_arrVisionIndex[3]].g_intCameraResolutionHeight;
                            float Scale = Math.Min(ScaleX, ScaleY);
                            //Scale = Math.Min(Scale, 1f);

                            Station4Panel.Size = new Size((int)(m_smVSInfo[m_arrVisionIndex[3]].g_intCameraResolutionWidth * Scale), (int)(m_smVSInfo[m_arrVisionIndex[3]].g_intCameraResolutionHeight * Scale));
                            Station4Panel.Location = new Point(pnl_Station4.Width / 2 - Station4Panel.Width / 2, pnl_Station4.Height / 2 - Station4Panel.Height / 2);
                            if (!m_smVSInfo[m_arrVisionIndex[3]].g_blnInspectionInProgress)
                            {
                                RedrawSubPanelImage(m_smVSInfo[m_arrVisionIndex[3]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[3]].g_intSelectedImage], m_objVS4Graphics, Station4Panel.Width, Station4Panel.Height);
                            }
                        }
                    }
                    else
                    {
                        if (m_smVSInfo[m_arrVisionIndex[3]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[3]].g_arrImages.Count)
                        {
                            float ScaleX = Station4Panel.Width / (float)m_smVSInfo[m_arrVisionIndex[3]].g_intCameraResolutionWidth;
                            float ScaleY = Station4Panel.Height / (float)m_smVSInfo[m_arrVisionIndex[3]].g_intCameraResolutionHeight;
                            float Scale = Math.Min(ScaleX, ScaleY);
                            //Scale = Math.Min(Scale, 1f);

                            Station4Panel.Size = new Size((int)(m_smVSInfo[m_arrVisionIndex[3]].g_intCameraResolutionWidth * Scale), (int)(m_smVSInfo[m_arrVisionIndex[3]].g_intCameraResolutionHeight * Scale));
                            Station4Panel.Location = new Point(pnl_Station4.Width / 2 - Station4Panel.Width / 2, pnl_Station4.Height / 2 - Station4Panel.Height / 2);
                            if (!m_smVSInfo[m_arrVisionIndex[3]].g_blnInspectionInProgress)
                            {
                                RedrawSubPanelImage(m_smVSInfo[m_arrVisionIndex[3]].g_arrImages[m_smVSInfo[m_arrVisionIndex[3]].g_intSelectedImage], m_objVS4Graphics, Station4Panel.Width, Station4Panel.Height);
                            }
                        }
                    }
                    m_smVSInfo[m_arrVisionIndex[3]].VS_VM_UpdateSmallPictureBox = false;
                }
            }

            if (m_smVSInfo[m_arrVisionIndex[4]] != null)
            {
                if (m_smVSInfo[m_arrVisionIndex[4]].VS_VM_UpdateSmallPictureBox || m_smVSInfo[m_arrVisionIndex[4]].ALL_VM_UpdatePictureBox)
                {
                    if (m_smVSInfo[m_arrVisionIndex[4]].g_intSelectedImage >= m_smVSInfo[m_arrVisionIndex[4]].g_arrImages.Count)
                        m_smVSInfo[m_arrVisionIndex[4]].g_intSelectedImage = 0;

                    if (m_smVSInfo[m_arrVisionIndex[4]].g_blnViewColorImage)
                    {
                        //m_smVSInfo[m_arrVisionIndex[4]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[4]].g_intSelectedImage].DrawZoomImage(m_objVS5Graphics, 0.5f);
                        if (m_smVSInfo[m_arrVisionIndex[4]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[4]].g_arrColorImages.Count)
                        {
                            float ScaleX = Station5Panel.Width / (float)m_smVSInfo[m_arrVisionIndex[4]].g_intCameraResolutionWidth;
                            float ScaleY = Station5Panel.Height / (float)m_smVSInfo[m_arrVisionIndex[4]].g_intCameraResolutionHeight;
                            float Scale = Math.Min(ScaleX, ScaleY);
                            //Scale = Math.Min(Scale, 1f);

                            Station5Panel.Size = new Size((int)(m_smVSInfo[m_arrVisionIndex[4]].g_intCameraResolutionWidth * Scale), (int)(m_smVSInfo[m_arrVisionIndex[4]].g_intCameraResolutionHeight * Scale));
                            Station5Panel.Location = new Point(pnl_Station5.Width / 2 - Station5Panel.Width / 2, pnl_Station5.Height / 2 - Station5Panel.Height / 2);
                            if (!m_smVSInfo[m_arrVisionIndex[4]].g_blnInspectionInProgress)
                            {
                                RedrawSubPanelImage(m_smVSInfo[m_arrVisionIndex[4]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[4]].g_intSelectedImage], m_objVS5Graphics, Station5Panel.Width, Station5Panel.Height);
                            }
                        }
                    }
                    else
                    {
                        if (m_smVSInfo[m_arrVisionIndex[4]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[4]].g_arrImages.Count)
                        {
                            float ScaleX = Station5Panel.Width / (float)m_smVSInfo[m_arrVisionIndex[4]].g_intCameraResolutionWidth;
                            float ScaleY = Station5Panel.Height / (float)m_smVSInfo[m_arrVisionIndex[4]].g_intCameraResolutionHeight;
                            float Scale = Math.Min(ScaleX, ScaleY);
                            //Scale = Math.Min(Scale, 1f);

                            Station5Panel.Size = new Size((int)(m_smVSInfo[m_arrVisionIndex[4]].g_intCameraResolutionWidth * Scale), (int)(m_smVSInfo[m_arrVisionIndex[4]].g_intCameraResolutionHeight * Scale));
                            Station5Panel.Location = new Point(pnl_Station5.Width / 2 - Station5Panel.Width / 2, pnl_Station5.Height / 2 - Station5Panel.Height / 2);
                            if (!m_smVSInfo[m_arrVisionIndex[4]].g_blnInspectionInProgress)
                            {
                                RedrawSubPanelImage(m_smVSInfo[m_arrVisionIndex[4]].g_arrImages[m_smVSInfo[m_arrVisionIndex[4]].g_intSelectedImage], m_objVS5Graphics, Station5Panel.Width, Station5Panel.Height);
                            }
                        }
                    }
                    m_smVSInfo[m_arrVisionIndex[4]].VS_VM_UpdateSmallPictureBox = false;
                }
            }

            if (m_smVSInfo[m_arrVisionIndex[5]] != null)
            {
                if (m_smVSInfo[m_arrVisionIndex[5]].VS_VM_UpdateSmallPictureBox || m_smVSInfo[m_arrVisionIndex[5]].ALL_VM_UpdatePictureBox)
                {
                    if (m_smVSInfo[m_arrVisionIndex[5]].g_intSelectedImage >= m_smVSInfo[m_arrVisionIndex[5]].g_arrImages.Count)
                        m_smVSInfo[m_arrVisionIndex[5]].g_intSelectedImage = 0;

                    if (m_smVSInfo[m_arrVisionIndex[5]].g_blnViewColorImage)
                    {
                        //m_smVSInfo[m_arrVisionIndex[5]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[5]].g_intSelectedImage].DrawZoomImage(m_objVS6Graphics, 0.5f);
                        if (m_smVSInfo[m_arrVisionIndex[5]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[5]].g_arrColorImages.Count)
                        {
                            float ScaleX = Station6Panel.Width / (float)m_smVSInfo[m_arrVisionIndex[5]].g_intCameraResolutionWidth;
                            float ScaleY = Station6Panel.Height / (float)m_smVSInfo[m_arrVisionIndex[5]].g_intCameraResolutionHeight;
                            float Scale = Math.Min(ScaleX, ScaleY);
                            //Scale = Math.Min(Scale, 1f);

                            Station6Panel.Size = new Size((int)(m_smVSInfo[m_arrVisionIndex[5]].g_intCameraResolutionWidth * Scale), (int)(m_smVSInfo[m_arrVisionIndex[5]].g_intCameraResolutionHeight * Scale));
                            Station6Panel.Location = new Point(pnl_Station6.Width / 2 - Station6Panel.Width / 2, pnl_Station6.Height / 2 - Station6Panel.Height / 2);
                            if (!m_smVSInfo[m_arrVisionIndex[5]].g_blnInspectionInProgress)
                            {
                                RedrawSubPanelImage(m_smVSInfo[m_arrVisionIndex[5]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[5]].g_intSelectedImage], m_objVS6Graphics, Station6Panel.Width, Station6Panel.Height);
                            }
                        }
                    }
                    else
                    {
                        if (m_smVSInfo[m_arrVisionIndex[5]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[5]].g_arrImages.Count)
                        {
                            float ScaleX = Station6Panel.Width / (float)m_smVSInfo[m_arrVisionIndex[5]].g_intCameraResolutionWidth;
                            float ScaleY = Station6Panel.Height / (float)m_smVSInfo[m_arrVisionIndex[5]].g_intCameraResolutionHeight;
                            float Scale = Math.Min(ScaleX, ScaleY);
                            //Scale = Math.Min(Scale, 1f);

                            Station6Panel.Size = new Size((int)(m_smVSInfo[m_arrVisionIndex[5]].g_intCameraResolutionWidth * Scale), (int)(m_smVSInfo[m_arrVisionIndex[5]].g_intCameraResolutionHeight * Scale));
                            Station6Panel.Location = new Point(pnl_Station6.Width / 2 - Station6Panel.Width / 2, pnl_Station6.Height / 2 - Station6Panel.Height / 2);
                            if (!m_smVSInfo[m_arrVisionIndex[5]].g_blnInspectionInProgress)
                            {
                                RedrawSubPanelImage(m_smVSInfo[m_arrVisionIndex[5]].g_arrImages[m_smVSInfo[m_arrVisionIndex[5]].g_intSelectedImage], m_objVS6Graphics, Station6Panel.Width, Station6Panel.Height);
                            }
                        }
                    }
                    m_smVSInfo[m_arrVisionIndex[5]].VS_VM_UpdateSmallPictureBox = false;
                }
            }

            if (m_smVSInfo[m_arrVisionIndex[6]] != null)
            {
                if (m_smVSInfo[m_arrVisionIndex[6]].VS_VM_UpdateSmallPictureBox || m_smVSInfo[m_arrVisionIndex[6]].ALL_VM_UpdatePictureBox)
                {
                    if (m_smVSInfo[m_arrVisionIndex[6]].g_intSelectedImage >= m_smVSInfo[m_arrVisionIndex[6]].g_arrImages.Count)
                        m_smVSInfo[m_arrVisionIndex[6]].g_intSelectedImage = 0;

                    if (m_smVSInfo[m_arrVisionIndex[6]].g_blnViewColorImage)
                    {
                        //m_smVSInfo[m_arrVisionIndex[6]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[6]].g_intSelectedImage].DrawZoomImage(m_objVS7Graphics, 0.5f);
                        if (m_smVSInfo[m_arrVisionIndex[6]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[6]].g_arrColorImages.Count)
                        {
                            float ScaleX = Station7Panel.Width / (float)m_smVSInfo[m_arrVisionIndex[6]].g_intCameraResolutionWidth;
                            float ScaleY = Station7Panel.Height / (float)m_smVSInfo[m_arrVisionIndex[6]].g_intCameraResolutionHeight;
                            float Scale = Math.Min(ScaleX, ScaleY);
                            //Scale = Math.Min(Scale, 1f);

                            Station7Panel.Size = new Size((int)(m_smVSInfo[m_arrVisionIndex[6]].g_intCameraResolutionWidth * Scale), (int)(m_smVSInfo[m_arrVisionIndex[6]].g_intCameraResolutionHeight * Scale));
                            Station7Panel.Location = new Point(pnl_Station7.Width / 2 - Station7Panel.Width / 2, pnl_Station7.Height / 2 - Station7Panel.Height / 2);
                            if (!m_smVSInfo[m_arrVisionIndex[6]].g_blnInspectionInProgress)
                            {
                                RedrawSubPanelImage(m_smVSInfo[m_arrVisionIndex[6]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[6]].g_intSelectedImage], m_objVS7Graphics, Station7Panel.Width, Station7Panel.Height);
                            }
                        }
                    }
                    else
                    {
                        if (m_smVSInfo[m_arrVisionIndex[6]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[6]].g_arrImages.Count)
                        {
                            float ScaleX = Station7Panel.Width / (float)m_smVSInfo[m_arrVisionIndex[6]].g_intCameraResolutionWidth;
                            float ScaleY = Station7Panel.Height / (float)m_smVSInfo[m_arrVisionIndex[6]].g_intCameraResolutionHeight;
                            float Scale = Math.Min(ScaleX, ScaleY);
                            //Scale = Math.Min(Scale, 1f);

                            Station7Panel.Size = new Size((int)(m_smVSInfo[m_arrVisionIndex[6]].g_intCameraResolutionWidth * Scale), (int)(m_smVSInfo[m_arrVisionIndex[6]].g_intCameraResolutionHeight * Scale));
                            Station7Panel.Location = new Point(pnl_Station7.Width / 2 - Station7Panel.Width / 2, pnl_Station7.Height / 2 - Station7Panel.Height / 2);
                            if (!m_smVSInfo[m_arrVisionIndex[6]].g_blnInspectionInProgress)
                            {
                                RedrawSubPanelImage(m_smVSInfo[m_arrVisionIndex[6]].g_arrImages[m_smVSInfo[m_arrVisionIndex[6]].g_intSelectedImage], m_objVS7Graphics, Station7Panel.Width, Station7Panel.Height);
                            }
                        }
                    }
                    m_smVSInfo[m_arrVisionIndex[6]].VS_VM_UpdateSmallPictureBox = false;
                }
            }
        }

        private void RedrawSubPanelImage(ImageDrawing objImage, Graphics objVSGraphics, int intPanelWidth, int intPanelHeight)
        {
            m_objDrawROI.AttachImage(objImage);
            m_objDrawROI.LoadROISetting(0, 0, objImage.ref_intImageWidth, objImage.ref_intImageHeight);
            m_objDrawROI.DrawZoomImage(objVSGraphics, intPanelWidth, intPanelHeight);
        }
        private void RedrawSubPanelImage(CImageDrawing objImage, Graphics objVSGraphics, int intPanelWidth, int intPanelHeight)
        {
            m_objDrawCROI.AttachImage(objImage);
            m_objDrawCROI.LoadROISetting(0, 0, objImage.ref_intImageWidth, objImage.ref_intImageHeight);
            m_objDrawCROI.DrawZoomImage(objVSGraphics, intPanelWidth, intPanelHeight);
        }

        private void Vision1Panel_Paint(object sender, PaintEventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[0]] == null)
                return;

            m_smVSInfo[m_arrVisionIndex[0]].VS_VM_UpdateSmallPictureBox = true;
        }

        private void Vision2Panel_Paint(object sender, PaintEventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[1]] == null)
                return;

            if (m_smVSInfo[m_arrVisionIndex[1]].g_strVisionName != "")
                m_smVSInfo[m_arrVisionIndex[1]].VS_VM_UpdateSmallPictureBox = true;
        }

        private void Vision3Panel_Paint(object sender, PaintEventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[2]] == null)
                return;

            if (m_smVSInfo[m_arrVisionIndex[2]].g_strVisionName != "")
                m_smVSInfo[m_arrVisionIndex[2]].VS_VM_UpdateSmallPictureBox = true;
        }

        private void Vision4Panel_Paint(object sender, PaintEventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[3]] == null)
                return;

            if (m_smVSInfo[m_arrVisionIndex[3]].g_strVisionName != "")
                m_smVSInfo[m_arrVisionIndex[3]].VS_VM_UpdateSmallPictureBox = true;
        }


        private void Station5Panel_Paint(object sender, PaintEventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[4]] == null)
                return;

            if (m_smVSInfo[m_arrVisionIndex[4]].g_strVisionName != "")
                m_smVSInfo[m_arrVisionIndex[4]].VS_VM_UpdateSmallPictureBox = true;
        }

        private void Station6Panel_Paint(object sender, PaintEventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[5]] == null)
                return;

            if (m_smVSInfo[m_arrVisionIndex[5]].g_strVisionName != "")
                m_smVSInfo[m_arrVisionIndex[5]].VS_VM_UpdateSmallPictureBox = true;
        }

        private void Station7Panel_Paint(object sender, PaintEventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[6]] == null)
                return;

            if (m_smVSInfo[m_arrVisionIndex[6]].g_strVisionName != "")
                m_smVSInfo[m_arrVisionIndex[6]].VS_VM_UpdateSmallPictureBox = true;
        }

        private void ResultTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < m_arrVisionIndex.Length; i++)
            {
                if (m_arrVisionIndex[i] != m_smProductionInfo.g_arrDisplayVisionModule[i])
                {
                    m_arrVisionIndex = m_smProductionInfo.g_arrDisplayVisionModule;
                    UpdateGUI();
                    break;
                }
            }


            UpdatePictureBox();

            if ((m_smVSInfo[m_arrVisionIndex[0]] != null) && m_smVSInfo[m_arrVisionIndex[0]].VS_AT_UpdateQuantity)
            {
                UpdateVision1(false, false);
                m_smVSInfo[m_arrVisionIndex[0]].VS_AT_UpdateQuantity = false;
            }

            if ((m_smVSInfo[m_arrVisionIndex[1]] != null) && m_smVSInfo[m_arrVisionIndex[1]].VS_AT_UpdateQuantity)
            {
                UpdateVision2(false, false);
                m_smVSInfo[m_arrVisionIndex[1]].VS_AT_UpdateQuantity = false;
            }

            if ((m_smVSInfo[m_arrVisionIndex[2]] != null) && m_smVSInfo[m_arrVisionIndex[2]].VS_AT_UpdateQuantity)
            {
                UpdateVision3(false, false);
                m_smVSInfo[m_arrVisionIndex[2]].VS_AT_UpdateQuantity = false;
            }

            if ((m_smVSInfo[m_arrVisionIndex[3]] != null) && m_smVSInfo[m_arrVisionIndex[3]].VS_AT_UpdateQuantity)
            {
                UpdateVision4(false, false);
                m_smVSInfo[m_arrVisionIndex[3]].VS_AT_UpdateQuantity = false;
            }

            if ((m_smVSInfo[m_arrVisionIndex[4]] != null) && m_smVSInfo[m_arrVisionIndex[4]].VS_AT_UpdateQuantity)
            {
                UpdateVision5(false, false);
                m_smVSInfo[m_arrVisionIndex[4]].VS_AT_UpdateQuantity = false;
            }

            if ((m_smVSInfo[m_arrVisionIndex[5]] != null) && m_smVSInfo[m_arrVisionIndex[5]].VS_AT_UpdateQuantity)
            {
                UpdateVision6(false, false);
                m_smVSInfo[m_arrVisionIndex[5]].VS_AT_UpdateQuantity = false;
            }

            if ((m_smVSInfo[m_arrVisionIndex[6]] != null) && m_smVSInfo[m_arrVisionIndex[6]].VS_AT_UpdateQuantity)
            {
                UpdateVision7(false, false);
                m_smVSInfo[m_arrVisionIndex[6]].VS_AT_UpdateQuantity = false;
            }

            if ((m_smVSInfo[m_arrVisionIndex[0]] != null) && m_smVSInfo[m_arrVisionIndex[0]].AT_VM_UpdateProductionDisplayAll)
            {
                UpdateGUI();
                UpdateVision1(true, false);
                m_smVSInfo[m_arrVisionIndex[0]].AT_VM_UpdateProductionDisplayAll = false;
            }

            if ((m_smVSInfo[m_arrVisionIndex[1]] != null) && m_smVSInfo[m_arrVisionIndex[1]].AT_VM_UpdateProductionDisplayAll)
            {
                UpdateGUI();
                UpdateVision2(true, false);
                m_smVSInfo[m_arrVisionIndex[1]].AT_VM_UpdateProductionDisplayAll = false;
            }

            if ((m_smVSInfo[m_arrVisionIndex[2]] != null) && m_smVSInfo[m_arrVisionIndex[2]].AT_VM_UpdateProductionDisplayAll)
            {
                UpdateGUI();
                UpdateVision3(true, false);
                m_smVSInfo[m_arrVisionIndex[2]].AT_VM_UpdateProductionDisplayAll = false;
            }

            if ((m_smVSInfo[m_arrVisionIndex[3]] != null) && m_smVSInfo[m_arrVisionIndex[3]].AT_VM_UpdateProductionDisplayAll)
            {
                UpdateGUI();
                UpdateVision4(true, false);
                m_smVSInfo[m_arrVisionIndex[3]].AT_VM_UpdateProductionDisplayAll = false;
            }

            if ((m_smVSInfo[m_arrVisionIndex[4]] != null) && m_smVSInfo[m_arrVisionIndex[4]].AT_VM_UpdateProductionDisplayAll)
            {
                UpdateGUI();
                UpdateVision5(true, false);
                m_smVSInfo[m_arrVisionIndex[4]].AT_VM_UpdateProductionDisplayAll = false;
            }

            if ((m_smVSInfo[m_arrVisionIndex[5]] != null) && m_smVSInfo[m_arrVisionIndex[5]].AT_VM_UpdateProductionDisplayAll)
            {
                UpdateGUI();
                UpdateVision6(true, false);
                m_smVSInfo[m_arrVisionIndex[5]].AT_VM_UpdateProductionDisplayAll = false;
            }

            if ((m_smVSInfo[m_arrVisionIndex[6]] != null) && m_smVSInfo[m_arrVisionIndex[6]].AT_VM_UpdateProductionDisplayAll)
            {
                UpdateGUI();
                UpdateVision7(true, false);
                m_smVSInfo[m_arrVisionIndex[6]].AT_VM_UpdateProductionDisplayAll = false;
            }
        }

        private void lbl_Process_Label(object sender, EventArgs e)
        {
            //m_blnSplitTime = !m_blnSplitTime;

            //m_fVS1ProcessTimePrev = m_fVS2ProcessTimePrev = m_fVS3ProcessTimePrev = m_fVS4ProcessTimePrev = -1.0f;

            //for (int i = 0; i < 4; i++)
            //{
            //    if (m_smVSInfo[m_arrVisionIndex[i]] == null)
            //        continue;

            //    m_smVSInfo[m_arrVisionIndex[i]].VS_AT_UpdateQuantity = true;
            //}
        }

        public void PauseThread()
        {
            ResultTimer.Stop();
        }

        public void StartThread()
        {
            m_objVS1Graphics = Graphics.FromHwnd(Station1Panel.Handle);
            m_objVS2Graphics = Graphics.FromHwnd(Station2Panel.Handle);
            m_objVS3Graphics = Graphics.FromHwnd(Station3Panel.Handle);
            m_objVS4Graphics = Graphics.FromHwnd(Station4Panel.Handle);
            m_objVS5Graphics = Graphics.FromHwnd(Station5Panel.Handle);
            m_objVS6Graphics = Graphics.FromHwnd(Station6Panel.Handle);
            m_objVS7Graphics = Graphics.FromHwnd(Station7Panel.Handle);

            ResultTimer.Start();
        }

        private void Station1Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[0]] != null)
            {
                if (m_smVSInfo[m_arrVisionIndex[0]].g_blnViewColorImage)
                {
                    if (!m_smProductionInfo.g_blnDisplayColorPixelInfo)
                        m_smProductionInfo.g_blnDisplayColorPixelInfo = true;
                }
                else
                {
                    if (m_smProductionInfo.g_blnDisplayColorPixelInfo)
                        m_smProductionInfo.g_blnDisplayColorPixelInfo = false;
                }

                m_smProductionInfo.g_intMousePositonX = (int)Math.Round(e.X * (m_smVSInfo[m_arrVisionIndex[0]].g_intCameraResolutionWidth / (float)Station1Panel.Size.Width), 0, MidpointRounding.AwayFromZero);
                m_smProductionInfo.g_intMousePositonY = (int)Math.Round(e.Y * (m_smVSInfo[m_arrVisionIndex[0]].g_intCameraResolutionHeight / (float)Station1Panel.Size.Height), 0, MidpointRounding.AwayFromZero);

                if (m_smVSInfo[m_arrVisionIndex[0]].g_blnViewRotatedImage)
                {
                    if (m_smVSInfo[m_arrVisionIndex[0]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[0]].g_arrRotatedImages.Count)
                        m_smProductionInfo.g_intMousePixel = m_smVSInfo[m_arrVisionIndex[0]].g_arrRotatedImages[m_smVSInfo[m_arrVisionIndex[0]].g_intSelectedImage].GetImageGrayPixel(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                    if (m_smVSInfo[m_arrVisionIndex[0]].g_blnViewColorImage && m_smVSInfo[m_arrVisionIndex[0]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[0]].g_arrColorRotatedImages.Count)
                        m_smProductionInfo.g_arrMouseRGBPixel = m_smVSInfo[m_arrVisionIndex[0]].g_arrColorRotatedImages[m_smVSInfo[m_arrVisionIndex[0]].g_intSelectedImage].GetRGBPixelValue(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                }
                else
                {
                    if (m_smVSInfo[m_arrVisionIndex[0]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[0]].g_arrImages.Count)
                        m_smProductionInfo.g_intMousePixel = m_smVSInfo[m_arrVisionIndex[0]].g_arrImages[m_smVSInfo[m_arrVisionIndex[0]].g_intSelectedImage].GetImageGrayPixel(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                    if (m_smVSInfo[m_arrVisionIndex[0]].g_blnViewColorImage && m_smVSInfo[m_arrVisionIndex[0]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[0]].g_arrColorImages.Count)
                        m_smProductionInfo.g_arrMouseRGBPixel = m_smVSInfo[m_arrVisionIndex[0]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[0]].g_intSelectedImage].GetRGBPixelValue(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                }
            }
        }
        private void Station2Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[1]] != null)
            {
                if (m_smVSInfo[m_arrVisionIndex[1]].g_blnViewColorImage)
                {
                    if (!m_smProductionInfo.g_blnDisplayColorPixelInfo)
                        m_smProductionInfo.g_blnDisplayColorPixelInfo = true;
                }
                else
                {
                    if (m_smProductionInfo.g_blnDisplayColorPixelInfo)
                        m_smProductionInfo.g_blnDisplayColorPixelInfo = false;
                }

                m_smProductionInfo.g_intMousePositonX = (int)Math.Round(e.X * (m_smVSInfo[m_arrVisionIndex[1]].g_intCameraResolutionWidth / (float)Station2Panel.Size.Width), 0, MidpointRounding.AwayFromZero);
                m_smProductionInfo.g_intMousePositonY = (int)Math.Round(e.Y * (m_smVSInfo[m_arrVisionIndex[1]].g_intCameraResolutionHeight / (float)Station2Panel.Size.Height), 0, MidpointRounding.AwayFromZero);

                if (m_smVSInfo[m_arrVisionIndex[1]].g_blnViewRotatedImage)
                {
                    if (m_smVSInfo[m_arrVisionIndex[1]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[1]].g_arrRotatedImages.Count)
                        m_smProductionInfo.g_intMousePixel = m_smVSInfo[m_arrVisionIndex[1]].g_arrRotatedImages[m_smVSInfo[m_arrVisionIndex[1]].g_intSelectedImage].GetImageGrayPixel(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                    if (m_smVSInfo[m_arrVisionIndex[1]].g_blnViewColorImage && m_smVSInfo[m_arrVisionIndex[1]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[1]].g_arrColorRotatedImages.Count)
                        m_smProductionInfo.g_arrMouseRGBPixel = m_smVSInfo[m_arrVisionIndex[1]].g_arrColorRotatedImages[m_smVSInfo[m_arrVisionIndex[1]].g_intSelectedImage].GetRGBPixelValue(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                }
                else
                {
                    if (m_smVSInfo[m_arrVisionIndex[1]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[1]].g_arrImages.Count)
                        m_smProductionInfo.g_intMousePixel = m_smVSInfo[m_arrVisionIndex[1]].g_arrImages[m_smVSInfo[m_arrVisionIndex[1]].g_intSelectedImage].GetImageGrayPixel(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                    if (m_smVSInfo[m_arrVisionIndex[1]].g_blnViewColorImage && m_smVSInfo[m_arrVisionIndex[1]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[1]].g_arrColorImages.Count)
                        m_smProductionInfo.g_arrMouseRGBPixel = m_smVSInfo[m_arrVisionIndex[1]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[1]].g_intSelectedImage].GetRGBPixelValue(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                }
            }
        }
        private void Station3Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[2]] != null)
            {
                if (m_smVSInfo[m_arrVisionIndex[2]].g_blnViewColorImage)
                {
                    if (!m_smProductionInfo.g_blnDisplayColorPixelInfo)
                        m_smProductionInfo.g_blnDisplayColorPixelInfo = true;
                }
                else
                {
                    if (m_smProductionInfo.g_blnDisplayColorPixelInfo)
                        m_smProductionInfo.g_blnDisplayColorPixelInfo = false;
                }

                m_smProductionInfo.g_intMousePositonX = (int)Math.Round(e.X * (m_smVSInfo[m_arrVisionIndex[2]].g_intCameraResolutionWidth / (float)Station3Panel.Size.Width), 0, MidpointRounding.AwayFromZero);
                m_smProductionInfo.g_intMousePositonY = (int)Math.Round(e.Y * (m_smVSInfo[m_arrVisionIndex[2]].g_intCameraResolutionHeight / (float)Station3Panel.Size.Height), 0, MidpointRounding.AwayFromZero);

                if (m_smVSInfo[m_arrVisionIndex[2]].g_blnViewRotatedImage)
                {
                    if (m_smVSInfo[m_arrVisionIndex[2]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[2]].g_arrRotatedImages.Count)
                        m_smProductionInfo.g_intMousePixel = m_smVSInfo[m_arrVisionIndex[2]].g_arrRotatedImages[m_smVSInfo[m_arrVisionIndex[2]].g_intSelectedImage].GetImageGrayPixel(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                    if (m_smVSInfo[m_arrVisionIndex[2]].g_blnViewColorImage && m_smVSInfo[m_arrVisionIndex[2]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[2]].g_arrColorRotatedImages.Count)
                        m_smProductionInfo.g_arrMouseRGBPixel = m_smVSInfo[m_arrVisionIndex[2]].g_arrColorRotatedImages[m_smVSInfo[m_arrVisionIndex[2]].g_intSelectedImage].GetRGBPixelValue(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                }
                else
                {
                    if (m_smVSInfo[m_arrVisionIndex[2]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[2]].g_arrImages.Count)
                        m_smProductionInfo.g_intMousePixel = m_smVSInfo[m_arrVisionIndex[2]].g_arrImages[m_smVSInfo[m_arrVisionIndex[2]].g_intSelectedImage].GetImageGrayPixel(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                    if (m_smVSInfo[m_arrVisionIndex[2]].g_blnViewColorImage && m_smVSInfo[m_arrVisionIndex[2]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[2]].g_arrColorImages.Count)
                        m_smProductionInfo.g_arrMouseRGBPixel = m_smVSInfo[m_arrVisionIndex[2]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[2]].g_intSelectedImage].GetRGBPixelValue(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                }
            }
        }
        private void Station4Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[3]] != null)
            {
                if (m_smVSInfo[m_arrVisionIndex[3]].g_blnViewColorImage)
                {
                    if (!m_smProductionInfo.g_blnDisplayColorPixelInfo)
                        m_smProductionInfo.g_blnDisplayColorPixelInfo = true;
                }
                else
                {
                    if (m_smProductionInfo.g_blnDisplayColorPixelInfo)
                        m_smProductionInfo.g_blnDisplayColorPixelInfo = false;
                }

                m_smProductionInfo.g_intMousePositonX = (int)Math.Round(e.X * (m_smVSInfo[m_arrVisionIndex[3]].g_intCameraResolutionWidth / (float)Station4Panel.Size.Width), 0, MidpointRounding.AwayFromZero);
                m_smProductionInfo.g_intMousePositonY = (int)Math.Round(e.Y * (m_smVSInfo[m_arrVisionIndex[3]].g_intCameraResolutionHeight / (float)Station4Panel.Size.Height), 0, MidpointRounding.AwayFromZero);

                if (m_smVSInfo[m_arrVisionIndex[3]].g_blnViewRotatedImage)
                {
                    if (m_smVSInfo[m_arrVisionIndex[3]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[3]].g_arrRotatedImages.Count)
                        m_smProductionInfo.g_intMousePixel = m_smVSInfo[m_arrVisionIndex[3]].g_arrRotatedImages[m_smVSInfo[m_arrVisionIndex[3]].g_intSelectedImage].GetImageGrayPixel(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                    if (m_smVSInfo[m_arrVisionIndex[3]].g_blnViewColorImage && m_smVSInfo[m_arrVisionIndex[3]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[3]].g_arrColorRotatedImages.Count)
                        m_smProductionInfo.g_arrMouseRGBPixel = m_smVSInfo[m_arrVisionIndex[3]].g_arrColorRotatedImages[m_smVSInfo[m_arrVisionIndex[3]].g_intSelectedImage].GetRGBPixelValue(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                }
                else
                {
                    if (m_smVSInfo[m_arrVisionIndex[3]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[3]].g_arrImages.Count)
                        m_smProductionInfo.g_intMousePixel = m_smVSInfo[m_arrVisionIndex[3]].g_arrImages[m_smVSInfo[m_arrVisionIndex[3]].g_intSelectedImage].GetImageGrayPixel(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                    if (m_smVSInfo[m_arrVisionIndex[3]].g_blnViewColorImage && m_smVSInfo[m_arrVisionIndex[3]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[3]].g_arrColorImages.Count)
                        m_smProductionInfo.g_arrMouseRGBPixel = m_smVSInfo[m_arrVisionIndex[3]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[3]].g_intSelectedImage].GetRGBPixelValue(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                }
            }
        }

        private void Station5Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[4]] != null)
            {
                if (m_smVSInfo[m_arrVisionIndex[4]].g_blnViewColorImage)
                {
                    if (!m_smProductionInfo.g_blnDisplayColorPixelInfo)
                        m_smProductionInfo.g_blnDisplayColorPixelInfo = true;
                }
                else
                {
                    if (m_smProductionInfo.g_blnDisplayColorPixelInfo)
                        m_smProductionInfo.g_blnDisplayColorPixelInfo = false;
                }

                m_smProductionInfo.g_intMousePositonX = (int)Math.Round(e.X * (m_smVSInfo[m_arrVisionIndex[4]].g_intCameraResolutionWidth / (float)Station5Panel.Size.Width), 0, MidpointRounding.AwayFromZero);
                m_smProductionInfo.g_intMousePositonY = (int)Math.Round(e.Y * (m_smVSInfo[m_arrVisionIndex[4]].g_intCameraResolutionHeight / (float)Station5Panel.Size.Height), 0, MidpointRounding.AwayFromZero);

                if (m_smVSInfo[m_arrVisionIndex[4]].g_blnViewRotatedImage)
                {
                    if (m_smVSInfo[m_arrVisionIndex[4]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[4]].g_arrRotatedImages.Count)
                        m_smProductionInfo.g_intMousePixel = m_smVSInfo[m_arrVisionIndex[4]].g_arrRotatedImages[m_smVSInfo[m_arrVisionIndex[4]].g_intSelectedImage].GetImageGrayPixel(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                    if (m_smVSInfo[m_arrVisionIndex[4]].g_blnViewColorImage && m_smVSInfo[m_arrVisionIndex[4]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[4]].g_arrColorRotatedImages.Count)
                        m_smProductionInfo.g_arrMouseRGBPixel = m_smVSInfo[m_arrVisionIndex[4]].g_arrColorRotatedImages[m_smVSInfo[m_arrVisionIndex[4]].g_intSelectedImage].GetRGBPixelValue(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                }
                else
                {
                    if (m_smVSInfo[m_arrVisionIndex[4]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[4]].g_arrImages.Count)
                        m_smProductionInfo.g_intMousePixel = m_smVSInfo[m_arrVisionIndex[4]].g_arrImages[m_smVSInfo[m_arrVisionIndex[4]].g_intSelectedImage].GetImageGrayPixel(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                    if (m_smVSInfo[m_arrVisionIndex[4]].g_blnViewColorImage && m_smVSInfo[m_arrVisionIndex[4]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[4]].g_arrColorImages.Count)
                        m_smProductionInfo.g_arrMouseRGBPixel = m_smVSInfo[m_arrVisionIndex[4]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[4]].g_intSelectedImage].GetRGBPixelValue(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                }
            }
        }

        private void Station6Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[5]] != null)
            {
                if (m_smVSInfo[m_arrVisionIndex[5]].g_blnViewColorImage)
                {
                    if (!m_smProductionInfo.g_blnDisplayColorPixelInfo)
                        m_smProductionInfo.g_blnDisplayColorPixelInfo = true;
                }
                else
                {
                    if (m_smProductionInfo.g_blnDisplayColorPixelInfo)
                        m_smProductionInfo.g_blnDisplayColorPixelInfo = false;
                }

                m_smProductionInfo.g_intMousePositonX = (int)Math.Round(e.X * (m_smVSInfo[m_arrVisionIndex[5]].g_intCameraResolutionWidth / (float)Station6Panel.Size.Width), 0, MidpointRounding.AwayFromZero);
                m_smProductionInfo.g_intMousePositonY = (int)Math.Round(e.Y * (m_smVSInfo[m_arrVisionIndex[5]].g_intCameraResolutionHeight / (float)Station6Panel.Size.Height), 0, MidpointRounding.AwayFromZero);

                if (m_smVSInfo[m_arrVisionIndex[5]].g_blnViewRotatedImage)
                {
                    if (m_smVSInfo[m_arrVisionIndex[5]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[5]].g_arrRotatedImages.Count)
                        m_smProductionInfo.g_intMousePixel = m_smVSInfo[m_arrVisionIndex[5]].g_arrRotatedImages[m_smVSInfo[m_arrVisionIndex[5]].g_intSelectedImage].GetImageGrayPixel(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                    if (m_smVSInfo[m_arrVisionIndex[5]].g_blnViewColorImage && m_smVSInfo[m_arrVisionIndex[5]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[5]].g_arrColorImages.Count)
                        m_smProductionInfo.g_arrMouseRGBPixel = m_smVSInfo[m_arrVisionIndex[5]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[5]].g_intSelectedImage].GetRGBPixelValue(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                }
                else
                {
                    if (m_smVSInfo[m_arrVisionIndex[5]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[5]].g_arrImages.Count)
                        m_smProductionInfo.g_intMousePixel = m_smVSInfo[m_arrVisionIndex[5]].g_arrImages[m_smVSInfo[m_arrVisionIndex[5]].g_intSelectedImage].GetImageGrayPixel(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                    if (m_smVSInfo[m_arrVisionIndex[5]].g_blnViewColorImage && m_smVSInfo[m_arrVisionIndex[5]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[5]].g_arrColorImages.Count)
                        m_smProductionInfo.g_arrMouseRGBPixel = m_smVSInfo[m_arrVisionIndex[5]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[5]].g_intSelectedImage].GetRGBPixelValue(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                }
            }
        }
        private void Station7Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[6]] != null)
            {
                if (m_smVSInfo[m_arrVisionIndex[6]].g_blnViewColorImage)
                {
                    if (!m_smProductionInfo.g_blnDisplayColorPixelInfo)
                        m_smProductionInfo.g_blnDisplayColorPixelInfo = true;
                }
                else
                {
                    if (m_smProductionInfo.g_blnDisplayColorPixelInfo)
                        m_smProductionInfo.g_blnDisplayColorPixelInfo = false;
                }

                m_smProductionInfo.g_intMousePositonX = (int)Math.Round(e.X * (m_smVSInfo[m_arrVisionIndex[6]].g_intCameraResolutionWidth / (float)Station7Panel.Size.Width), 0, MidpointRounding.AwayFromZero);
                m_smProductionInfo.g_intMousePositonY = (int)Math.Round(e.Y * (m_smVSInfo[m_arrVisionIndex[6]].g_intCameraResolutionHeight / (float)Station7Panel.Size.Height), 0, MidpointRounding.AwayFromZero);

                if (m_smVSInfo[m_arrVisionIndex[6]].g_blnViewRotatedImage)
                {
                    if (m_smVSInfo[m_arrVisionIndex[6]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[6]].g_arrRotatedImages.Count)
                        m_smProductionInfo.g_intMousePixel = m_smVSInfo[m_arrVisionIndex[6]].g_arrRotatedImages[m_smVSInfo[m_arrVisionIndex[6]].g_intSelectedImage].GetImageGrayPixel(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                    if (m_smVSInfo[m_arrVisionIndex[6]].g_blnViewColorImage && m_smVSInfo[m_arrVisionIndex[6]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[6]].g_arrColorImages.Count)
                        m_smProductionInfo.g_arrMouseRGBPixel = m_smVSInfo[m_arrVisionIndex[6]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[6]].g_intSelectedImage].GetRGBPixelValue(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                }
                else
                {
                    if (m_smVSInfo[m_arrVisionIndex[6]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[6]].g_arrImages.Count)
                        m_smProductionInfo.g_intMousePixel = m_smVSInfo[m_arrVisionIndex[6]].g_arrImages[m_smVSInfo[m_arrVisionIndex[6]].g_intSelectedImage].GetImageGrayPixel(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                    if (m_smVSInfo[m_arrVisionIndex[6]].g_blnViewColorImage && m_smVSInfo[m_arrVisionIndex[6]].g_intSelectedImage < m_smVSInfo[m_arrVisionIndex[6]].g_arrColorImages.Count)
                        m_smProductionInfo.g_arrMouseRGBPixel = m_smVSInfo[m_arrVisionIndex[6]].g_arrColorImages[m_smVSInfo[m_arrVisionIndex[6]].g_intSelectedImage].GetRGBPixelValue(m_smProductionInfo.g_intMousePositonX, m_smProductionInfo.g_intMousePositonY);
                }
            }
        }
        private void Station1Panel_Click(object sender, EventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[0]] != null)
            {
                m_smCustomizeInfo.g_intSelectedVision = 1;
            }
            else
                m_smCustomizeInfo.g_intSelectedVision = 0;
        }

        private void Station2Panel_Click(object sender, EventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[1]] != null)
            {
                m_smCustomizeInfo.g_intSelectedVision = 2;
            }
            else
                m_smCustomizeInfo.g_intSelectedVision = 0;
        }

        private void Station3Panel_Click(object sender, EventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[2]] != null)
            {
                m_smCustomizeInfo.g_intSelectedVision = 3;
            }
            else
                m_smCustomizeInfo.g_intSelectedVision = 0;
        }

        private void Station4Panel_Click(object sender, EventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[3]] != null)
            {
                m_smCustomizeInfo.g_intSelectedVision = 4;
            }
            else
                m_smCustomizeInfo.g_intSelectedVision = 0;
        }

        private void Station5Panel_Click(object sender, EventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[4]] != null)
            {
                m_smCustomizeInfo.g_intSelectedVision = 5;
            }
            else
                m_smCustomizeInfo.g_intSelectedVision = 0;
        }

        private void Station6Panel_Click(object sender, EventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[5]] != null)
            {
                m_smCustomizeInfo.g_intSelectedVision = 6;
            }
            else
                m_smCustomizeInfo.g_intSelectedVision = 0;
        }
        private void Station7Panel_Click(object sender, EventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[6]] != null)
            {
                m_smCustomizeInfo.g_intSelectedVision = 7;
            }
            else
                m_smCustomizeInfo.g_intSelectedVision = 0;
        }
    }
}