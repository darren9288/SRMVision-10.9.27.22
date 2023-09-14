using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.CrystalReports;
using CrystalDecisions.Shared;
using Common;
using SharedMemory;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace History
{
    public partial class LotHistory : Form
    {
        #region Member Variables
        private SRMWaitingFormThread m_thWaitingFormThread;
        private string m_strFilterText = "";
        private string m_strFilterColumn = "";
        private string m_strFilterMatchField = "";
        private string m_strFilterVisionModule = "";
        private string m_strFilterCategory = "";
        private string m_strFilterMethod = "Monthly";
        private string m_strFilterViewDaily = "";
        private string m_strFilterViewWeekly = "";

        private bool m_blnInitDone = false;
        private bool m_blnRefillLot = false;
        private int m_intParameterMask = 0x0C;
        private int m_intParameterMask_Lead3D = 0x06;
        private int m_intROIMask = 0x1F;
        private string m_strSelectedFile = "";
        private string[] m_strParameter = new string[2];
        private string[] m_strParameter_Lead3D = new string[2];
        private string[] m_strROI = new string[5];
        private ProductionInfo m_smProductionInfo;
        private VisionInfo[] m_smVSInfo;
        private FileSorting m_objTimeComparer = new FileSorting();
        private CustomOption m_smCustomizeInfo;

        //private DataSet m_dsData = new DataSet();
        private ReportDocument m_report = new ReportDocument();
        private XmlParser m_objFile;

        private DataTable m_dtGRR = new DataTable("GRR");
        private DataTable m_dtData = new DataTable("GRRData");
        private DataTable m_dtOperator = new DataTable("Operator");

        private DataTable m_dtCPKGroup = new DataTable("CPKGroup");
        private DataTable m_dtCPK = new DataTable("CPK");
        private int m_intSelectedFilter = 1;
        #endregion

        public LotHistory(VisionInfo[] smVisionInfo, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            this.Dock = DockStyle.None;

            m_smProductionInfo = smProductionInfo;
            m_smVSInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;

            InitializeComponent();

            m_strParameter[0] = "Width";
            m_strParameter[1] = "Length";
            m_strParameter_Lead3D[0] = "Width";
            m_strParameter_Lead3D[1] = "Length";
            m_strROI[0] = "Middle";
            m_strROI[1] = "Top";
            m_strROI[2] = "Right";
            m_strROI[3] = "Bottom";
            m_strROI[4] = "Left";
            FillVisionModule();
            InitGRRTable();
            InitCPKTable();
            FillMonthComboBox();
            //FillLotComboBox();
            //FillDeviceLogDatagrid();

            m_blnInitDone = true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="strParameter"></param>
        /// <returns></returns>
        private bool CheckParameterSelected(string strParameter)
        {
            for (int i = 0; i < m_strParameter.Length; i++)
            {
                if (strParameter == m_strParameter[i])
                    return true;
            }
            return false;
        }

        private bool CheckParameterSelected_Lead3D(string strParameter)
        {
            for (int i = 0; i < m_strParameter_Lead3D.Length; i++)
            {
                if (strParameter == m_strParameter_Lead3D[i])
                    return true;
            }
            return false;
        }

        private bool CheckROISelected(string strROI)
        {
            for (int i = 0; i < m_strROI.Length; i++)
            {
                if (strROI.Substring(0, m_strROI[i].Length) == m_strROI[i])
                    return true;
            }
            return false;
        }



        /// <summary>
        /// Gather and display lot summary in crystal report
        /// </summary>
        private void DisplayLotSummary()
        {
            if (cbo_Lot.SelectedIndex < 0)
                return;
            m_report = new ReportDocument();
            string strSelectedLot = cbo_Lot.SelectedItem.ToString();
            int fromStart = strSelectedLot.IndexOf("From", 0);
            string strLotID = strSelectedLot.Substring(0, fromStart - 1);
            string strLotStartTime = strSelectedLot.Substring(fromStart + 7);
            string strLotTime = strLotStartTime.Substring(0, 4) + strLotStartTime.Substring(5, 2) + strLotStartTime.Substring(8, 2) +
                strLotStartTime.Substring(11, 2) + strLotStartTime.Substring(14, 2) + strLotStartTime.Substring(17, 2);

            m_strSelectedFile = AppDomain.CurrentDomain.BaseDirectory + "LotReport\\" + strLotID + "_" + strLotTime + ".xml";
            if (File.Exists(m_strSelectedFile))
            {
                m_objFile = new XmlParser(m_strSelectedFile);
                int intVisionNo = m_objFile.GetFirstSectionCount() - 1;

                DataTable dtVision = new DataTable("Vision");
                dtVision.Columns.Add("VisionName");
                dtVision.Columns.Add("Total");
                dtVision.Columns.Add("Pass");
                dtVision.Columns.Add("Fail");
                dtVision.Columns.Add("Yield");

                DataTable dtFail = new DataTable("Fail");
                dtFail.Columns.Add("VisionName");
                dtFail.Columns.Add("Feature");
                dtFail.Columns.Add("Count");

                for (int i = 0; i < intVisionNo; i++)
                {
                    m_objFile.GetFirstSection("VisionFeature" + i);

                    string strName = m_objFile.GetValueAsString("VisionName", "");
                    int intTotal = m_objFile.GetValueAsInt("Total", 0);
                    int intPass = m_objFile.GetValueAsInt("Pass", 0);
                    int intFail = intTotal - intPass;
                    float fYield = 0;
                    if (intTotal > 0)
                        fYield = (intPass / (float)intTotal )*100;

                    DataRow dr = dtVision.NewRow();
                    dr["VisionName"] = strName;
                    dr["Total"] = intTotal.ToString();
                    dr["Pass"] = intPass.ToString();
                    dr["Fail"] = intFail.ToString();
                    dr["Yield"] = fYield.ToString("f2");
                    dtVision.Rows.Add(dr);

                    m_objFile.GetSecondSection("Fail");
                    int intChildCount = m_objFile.GetThirdSectionCount();
                    for (int j = 0; j < intChildCount; j++)
                    {
                        string strNode = m_objFile.GetThirdSectionElement("Fail", j);
                        dr = dtFail.NewRow();
                        dr["VisionName"] = strName;
                        dr["Feature"] = strNode;
                        dr["Count"] = m_objFile.GetValueAsString(strNode, "0", 2);
                        dtFail.Rows.Add(dr);
                    }
                }

                m_report.Load(AppDomain.CurrentDomain.BaseDirectory + "\\Report\\LotlyReport.rpt");
                m_report.Database.Tables["Vision"].SetDataSource(dtVision);
                m_report.Database.Tables["FailDetail"].SetDataSource(dtFail);

                SetParameterValue();
                HistoryCR.ReportSource = m_report;
            }
            else
                HistoryCR.ReportSource = null;
            
            FillGRRComboBox(strLotID + "_" + strLotTime);
            FillCPKComboBox(strLotID + "_" + strLotTime);

            //2019-09-27 ZJYEOH : Decide cbo_VisionModule.Enabled here, so that able to control GRR or CPK 
            if (cbo_GRR.Enabled || cbo_CPK.Enabled)
                cbo_VisionModule.Enabled = true;
            else
                cbo_VisionModule.Enabled = false;
        }

        /// <summary>
        /// Gather and display GRR in crystal report
        /// </summary>
        private void DisplayGRRReport()
        {
            if (cbo_GRR.SelectedIndex < 0)
                return;

            m_dtGRR.Rows.Clear();
            m_dtData.Rows.Clear();
            m_dtOperator.Rows.Clear();
            m_report = new ReportDocument();
            
            // Get information from selected lot
            string strSelectedLot = cbo_Lot.SelectedItem.ToString();
            int fromStart = strSelectedLot.IndexOf("From", 0);
            string strLotID = strSelectedLot.Substring(0, fromStart - 1);
            string strLotStartTime = strSelectedLot.Substring(fromStart + 7);
            string strLotTime = strLotStartTime.Substring(0, 4) + strLotStartTime.Substring(5, 2) + strLotStartTime.Substring(8, 2) +
                strLotStartTime.Substring(11, 2) + strLotStartTime.Substring(14, 2) + strLotStartTime.Substring(17, 2);

            // Get information from selected GRR
            string strSelectedGRR = cbo_GRR.SelectedItem.ToString();
            fromStart = strSelectedGRR.IndexOf("From", 0);
            string strGRRID = strSelectedGRR.Substring(0, fromStart - 1);
            string strVisionName = strGRRID.Substring(4);
            string strGRRStartTime = strSelectedGRR.Substring(fromStart + 7);
            string strGRRTime = strGRRStartTime.Substring(0, 4) + strGRRStartTime.Substring(5, 2) + strGRRStartTime.Substring(8, 2) +
                strGRRStartTime.Substring(11, 2) + strGRRStartTime.Substring(14, 2) + strGRRStartTime.Substring(17, 2);

            m_strSelectedFile = AppDomain.CurrentDomain.BaseDirectory + "GRRReport\\" + strLotID + "_" + strLotTime + "\\" + strVisionName + "\\" + strGRRID + "_" + strGRRTime + ".xml";
            m_objFile = new XmlParser(m_strSelectedFile);

            m_objFile.GetFirstSection("Format");
            int intMaxGroup = m_objFile.GetValueAsInt("Group", 0);
            int intMaxPart = m_objFile.GetValueAsInt("Part", 0);
            int intMaxOperator = m_objFile.GetValueAsInt("Operator", 0);
            int intMaxTrial = m_objFile.GetValueAsInt("Trial", 0);

            string[] strItems;
            string[] strGroupName = null;
            float[][] fUSL;
            float[][] fLSL;
            m_objFile.GetFirstSection("Items");
            int intItemsLength = m_objFile.GetSecondSectionCount();
            strItems = new string[intItemsLength];
            fUSL = new float[intItemsLength][];
            fLSL = new float[intItemsLength][];
            for (int x = 0; x < intItemsLength; x++)
            {
                m_objFile.GetSecondSection("Items" + x);
                strItems[x] = m_objFile.GetValueAsString("Name", "No Found", 2);
                fLSL[x] = new float[intMaxGroup];
                fUSL[x] = new float[intMaxGroup];
                if (x == 0)
                    strGroupName = new string[intMaxGroup];
                for (int y = 0; y < intMaxGroup; y++)
                {
                    m_objFile.GetThirdSection("Group" + y);
                    fLSL[x][y] = m_objFile.GetValueAsFloat("LSL", 0, 3);
                    fUSL[x][y] = m_objFile.GetValueAsFloat("USL", 0, 3);
                    if (x == 0)
                        strGroupName[y] = m_objFile.GetValueAsString("GroupName", "", 3);
                }
            }

            int intID = 0;
            for (int x = 0; x < intItemsLength; x++)
            {
                if (!CheckParameterSelected(strItems[x]))
                    continue;

                m_objFile.GetFirstSection("Items" + x);

                for (int y = 0; y < intMaxGroup; y++)
                {
                    m_objFile.GetSecondSection("Group" + y);
                                        
                    for (int i = 0; i < intMaxPart; i++)
                    {
                        DataRow drData = m_dtData.NewRow();
                        m_objFile.GetThirdSection("Part" + i);
                        drData["ID"] = intID;
                        drData["Parts"] = i + 1;

                        int intColumn = 1;
                        for (int j = 0; j < intMaxOperator; j++)
                        {
                            switch (j)
                            {
                                case 1:
                                    intColumn = 5;
                                    break;
                                case 2:
                                    intColumn = 9;
                                    break;
                            }
                            m_objFile.GetFourthSection("Operator" + j);

                            for (int k = 0; k < intMaxTrial; k++)
                            {
                                drData["Column" + (intColumn+k)] = m_objFile.GetValueAsString("Trial" + k, "0", 4);                              
                            }
                            drData["Column" + (intColumn+3)] = m_objFile.GetValueAsString("Range" + j, "0", 4);
                        }
                        drData["XBar"] = m_objFile.GetValueAsString("XBar" + i, "0", 3);
                        m_dtData.Rows.Add(drData);
                    }

                    DataRow drTotalData = m_dtData.NewRow();
                    DataRow dr = m_dtOperator.NewRow();
                    dr["ID"] = drTotalData["ID"] = intID;
                    drTotalData["Parts"] = "Total";
                    m_objFile.GetThirdSection("Total");
                    int intCol = 1;
                    for (int j = 0; j < intMaxOperator; j++)
                    {
                        string str = "A";
                         switch (j)
                        {                        
                            case 1:
                                intCol = 5;
                                str = "B";
                                break;
                            case 2:
                                intCol = 9;
                                str = "C";
                                break;
                        }
                        dr["Op" + str] = "Operator " + str;

                        m_objFile.GetFourthSection("Operator" + j);
                        for (int k = 0; k < intMaxTrial; k++)
                        {
                            dr["Column" + (intCol + k)] = "Trial" + (k + 1);
                            drTotalData["Column" + (intCol + k)] = m_objFile.GetValueAsString("TotalTrial" + k, "0", 4);
                        }
                        drTotalData["Column" + (intCol + 3)] = m_objFile.GetValueAsString("TotalRange" + j, "0", 3);
                        dr["Column" + (intCol + 3)] = "Range";
                        dr["Sum" + str] = m_objFile.GetValueAsString("TotalTrial" + j, "0", 3);
                        dr["Average" + str] = m_objFile.GetValueAsString("AverageTrial" + j, "0", 3);
                        dr["RBar" + str] = m_objFile.GetValueAsString("AverageXBar" + j, "0", 3); 
                    }
                    m_dtData.Rows.Add(drTotalData);
                    m_dtOperator.Rows.Add(dr);

                    DataRow drGRR = m_dtGRR.NewRow();
                    drGRR["ID"] = intID;
                    drGRR["Parameter"] = strItems[x];
                    if (y >= strGroupName.Length || strGroupName[y] == "")
                        drGRR["GroupNo"] = "Pad" + (y + 1);
                    else
                        drGRR["GroupNo"] = strGroupName[y];
                    drGRR["Specification"] = fLSL[x][y] + "-" + fUSL[x][y];
                    drGRR["R-Part"] = m_objFile.GetValueAsString("R-Part", "0", 2);
                    drGRR["UCL-Range"] = m_objFile.GetValueAsString("UCL-Range", "0", 2);
                    drGRR["RangeOfAverages"] = m_objFile.GetValueAsString("RangeAverage", "0", 2);
                    drGRR["AverageOfRBar"] = m_objFile.GetValueAsString("AverageR-Bar", "0", 2);
                    drGRR["RepeatabilityInUnit"] = m_objFile.GetValueAsString("EV", "0", 2);
                    drGRR["RepeatabilityInPercent"] = m_objFile.GetValueAsString("PercentEV", "0", 2);
                    drGRR["ReproducilityInUnit"] = m_objFile.GetValueAsString("AV", "0", 2);
                    drGRR["ReproducilityInPercent"] = m_objFile.GetValueAsString("PercentAV", "0", 2);
                    drGRR["R&RInUnit"] = m_objFile.GetValueAsString("RR", "0", 2);
                    drGRR["R&RInPercent"] = m_objFile.GetValueAsString("PercentRR", "0", 2);
                    drGRR["PartVariationInUnit"] = m_objFile.GetValueAsString("PV", "0", 2);
                    drGRR["PartVariationInPercent"] = m_objFile.GetValueAsString("PercentPV", "0", 2);
                    drGRR["Tolerance"] = m_objFile.GetValueAsString("Tolerance", "0", 2);
                    drGRR["TotalVariation"] = m_objFile.GetValueAsString("TV", "0", 2);
                    drGRR["Ratio"] = m_objFile.GetValueAsString("PT", "0", 2);
                    drGRR["ndc"] = m_objFile.GetValueAsString("NDC", "0", 2);
                    m_dtGRR.Rows.Add(drGRR);
                    intID++;
                }               
            }

            m_report.Load(AppDomain.CurrentDomain.BaseDirectory + "Report\\GRRReport.rpt");
            m_report.Database.Tables["GRR"].SetDataSource(m_dtGRR);
            m_report.Database.Tables["GRRData"].SetDataSource(m_dtData);
            m_report.Database.Tables["Operator"].SetDataSource(m_dtOperator);

            SetGRRParameterValue();

            HistoryCR.Zoom(125);
            HistoryCR.ReportSource = m_report;
        }

        private void DisplayGRRReport_Lead3D()
        {
            if (cbo_GRR.SelectedIndex < 0)
                return;

            m_dtGRR.Rows.Clear();
            m_dtData.Rows.Clear();
            m_dtOperator.Rows.Clear();
            m_report = new ReportDocument();

            // Get information from selected lot
            string strSelectedLot = cbo_Lot.SelectedItem.ToString();
            int fromStart = strSelectedLot.IndexOf("From", 0);
            string strLotID = strSelectedLot.Substring(0, fromStart - 1);
            string strLotStartTime = strSelectedLot.Substring(fromStart + 7);
            string strLotTime = strLotStartTime.Substring(0, 4) + strLotStartTime.Substring(5, 2) + strLotStartTime.Substring(8, 2) +
                strLotStartTime.Substring(11, 2) + strLotStartTime.Substring(14, 2) + strLotStartTime.Substring(17, 2);

            // Get information from selected GRR
            string strSelectedGRR = cbo_GRR.SelectedItem.ToString();
            fromStart = strSelectedGRR.IndexOf("From", 0);
            string strGRRID = strSelectedGRR.Substring(0, fromStart - 1);
            string strVisionName = strGRRID.Substring(4);
            string strGRRStartTime = strSelectedGRR.Substring(fromStart + 7);
            string strGRRTime = strGRRStartTime.Substring(0, 4) + strGRRStartTime.Substring(5, 2) + strGRRStartTime.Substring(8, 2) +
                strGRRStartTime.Substring(11, 2) + strGRRStartTime.Substring(14, 2) + strGRRStartTime.Substring(17, 2);

            m_strSelectedFile = AppDomain.CurrentDomain.BaseDirectory + "GRRReport\\" + strLotID + "_" + strLotTime + "\\" + strVisionName + "\\" + strGRRID + "_" + strGRRTime + ".xml";
            m_objFile = new XmlParser(m_strSelectedFile);

            m_objFile.GetFirstSection("Format");
            int intMaxGroup = m_objFile.GetValueAsInt("Group", 0);
            int intMaxPart = m_objFile.GetValueAsInt("Part", 0);
            int intMaxOperator = m_objFile.GetValueAsInt("Operator", 0);
            int intMaxTrial = m_objFile.GetValueAsInt("Trial", 0);

            string[] strItems;
            string[] strGroupName = null;
            float[][] fUSL;
            float[][] fLSL;
            m_objFile.GetFirstSection("Items");
            int intItemsLength = m_objFile.GetSecondSectionCount();
            strItems = new string[intItemsLength];
            fUSL = new float[intItemsLength][];
            fLSL = new float[intItemsLength][];
            for (int x = 0; x < intItemsLength; x++)
            {
                m_objFile.GetSecondSection("Items" + x);
                strItems[x] = m_objFile.GetValueAsString("Name", "No Found", 2);
                fLSL[x] = new float[intMaxGroup];
                fUSL[x] = new float[intMaxGroup];
                if (x == 0)
                    strGroupName = new string[intMaxGroup];
                for (int y = 0; y < intMaxGroup; y++)
                {
                    m_objFile.GetThirdSection("Group" + y);
                    fLSL[x][y] = m_objFile.GetValueAsFloat("LSL", 0, 3);
                    fUSL[x][y] = m_objFile.GetValueAsFloat("USL", 0, 3);
                    if (x == 0)
                        strGroupName[y] = m_objFile.GetValueAsString("GroupName", "", 3);
                }
            }

            int intID = 0;
            for (int x = 0; x < intItemsLength; x++)
            {
                if (!CheckParameterSelected_Lead3D(strItems[x]))
                    continue;

                m_objFile.GetFirstSection("Items" + x);

                for (int y = 0; y < intMaxGroup; y++)
                {
                    m_objFile.GetSecondSection("Group" + y);

                    for (int i = 0; i < intMaxPart; i++)
                    {
                        DataRow drData = m_dtData.NewRow();
                        m_objFile.GetThirdSection("Part" + i);
                        drData["ID"] = intID;
                        drData["Parts"] = i + 1;

                        int intColumn = 1;
                        for (int j = 0; j < intMaxOperator; j++)
                        {
                            switch (j)
                            {
                                case 1:
                                    intColumn = 5;
                                    break;
                                case 2:
                                    intColumn = 9;
                                    break;
                            }
                            m_objFile.GetFourthSection("Operator" + j);

                            for (int k = 0; k < intMaxTrial; k++)
                            {
                                drData["Column" + (intColumn + k)] = m_objFile.GetValueAsString("Trial" + k, "0", 4);
                            }
                            drData["Column" + (intColumn + 3)] = m_objFile.GetValueAsString("Range" + j, "0", 4);
                        }
                        drData["XBar"] = m_objFile.GetValueAsString("XBar" + i, "0", 3);
                        m_dtData.Rows.Add(drData);
                    }

                    DataRow drTotalData = m_dtData.NewRow();
                    DataRow dr = m_dtOperator.NewRow();
                    dr["ID"] = drTotalData["ID"] = intID;
                    drTotalData["Parts"] = "Total";
                    m_objFile.GetThirdSection("Total");
                    int intCol = 1;
                    for (int j = 0; j < intMaxOperator; j++)
                    {
                        string str = "A";
                        switch (j)
                        {
                            case 1:
                                intCol = 5;
                                str = "B";
                                break;
                            case 2:
                                intCol = 9;
                                str = "C";
                                break;
                        }
                        dr["Op" + str] = "Operator " + str;

                        m_objFile.GetFourthSection("Operator" + j);
                        for (int k = 0; k < intMaxTrial; k++)
                        {
                            dr["Column" + (intCol + k)] = "Trial" + (k + 1);
                            drTotalData["Column" + (intCol + k)] = m_objFile.GetValueAsString("TotalTrial" + k, "0", 4);
                        }
                        drTotalData["Column" + (intCol + 3)] = m_objFile.GetValueAsString("TotalRange" + j, "0", 3);
                        dr["Column" + (intCol + 3)] = "Range";
                        dr["Sum" + str] = m_objFile.GetValueAsString("TotalTrial" + j, "0", 3);
                        dr["Average" + str] = m_objFile.GetValueAsString("AverageTrial" + j, "0", 3);
                        dr["RBar" + str] = m_objFile.GetValueAsString("AverageXBar" + j, "0", 3);
                    }
                    m_dtData.Rows.Add(drTotalData);
                    m_dtOperator.Rows.Add(dr);

                    DataRow drGRR = m_dtGRR.NewRow();
                    drGRR["ID"] = intID;
                    drGRR["Parameter"] = strItems[x];
                    if (y >= strGroupName.Length || strGroupName[y] == "")
                        drGRR["GroupNo"] = "Pad" + (y + 1);
                    else
                        drGRR["GroupNo"] = strGroupName[y];
                    drGRR["Specification"] = fLSL[x][y] + "-" + fUSL[x][y];
                    drGRR["R-Part"] = m_objFile.GetValueAsString("R-Part", "0", 2);
                    drGRR["UCL-Range"] = m_objFile.GetValueAsString("UCL-Range", "0", 2);
                    drGRR["RangeOfAverages"] = m_objFile.GetValueAsString("RangeAverage", "0", 2);
                    drGRR["AverageOfRBar"] = m_objFile.GetValueAsString("AverageR-Bar", "0", 2);
                    drGRR["RepeatabilityInUnit"] = m_objFile.GetValueAsString("EV", "0", 2);
                    drGRR["RepeatabilityInPercent"] = m_objFile.GetValueAsString("PercentEV", "0", 2);
                    drGRR["ReproducilityInUnit"] = m_objFile.GetValueAsString("AV", "0", 2);
                    drGRR["ReproducilityInPercent"] = m_objFile.GetValueAsString("PercentAV", "0", 2);
                    drGRR["R&RInUnit"] = m_objFile.GetValueAsString("RR", "0", 2);
                    drGRR["R&RInPercent"] = m_objFile.GetValueAsString("PercentRR", "0", 2);
                    drGRR["PartVariationInUnit"] = m_objFile.GetValueAsString("PV", "0", 2);
                    drGRR["PartVariationInPercent"] = m_objFile.GetValueAsString("PercentPV", "0", 2);
                    drGRR["Tolerance"] = m_objFile.GetValueAsString("Tolerance", "0", 2);
                    drGRR["TotalVariation"] = m_objFile.GetValueAsString("TV", "0", 2);
                    drGRR["Ratio"] = m_objFile.GetValueAsString("PT", "0", 2);
                    m_dtGRR.Rows.Add(drGRR);
                    intID++;
                }
            }

            m_report.Load(AppDomain.CurrentDomain.BaseDirectory + "Report\\GRRReport.rpt");
            m_report.Database.Tables["GRR"].SetDataSource(m_dtGRR);
            m_report.Database.Tables["GRRData"].SetDataSource(m_dtData);
            m_report.Database.Tables["Operator"].SetDataSource(m_dtOperator);

            SetGRRParameterValue();

            HistoryCR.Zoom(125);
            HistoryCR.ReportSource = m_report;
        }

        /// <summary>
        /// Gather and display lot summary in crystal report
        /// </summary>
        private void DisplayCPKReport()
        {
            if (cbo_CPK.SelectedIndex < 0)
                return;

            m_dtCPK.Rows.Clear();
            m_dtCPKGroup.Rows.Clear();
            m_report = new ReportDocument();
            m_report.Load(AppDomain.CurrentDomain.BaseDirectory + "Report\\CPKReport.rpt");
            // Get information from selected lot
            string strSelectedLot = cbo_Lot.SelectedItem.ToString();
            int fromStart = strSelectedLot.IndexOf("From", 0);
            string strLotID = strSelectedLot.Substring(0, fromStart - 1);
            string strLotStartTime = strSelectedLot.Substring(fromStart + 7);
            string strLotTime = strLotStartTime.Substring(0, 4) + strLotStartTime.Substring(5, 2) + strLotStartTime.Substring(8, 2) +
                strLotStartTime.Substring(11, 2) + strLotStartTime.Substring(14, 2) + strLotStartTime.Substring(17, 2);

            // Get information from selected CPK
            string strSelectedCPK = cbo_CPK.SelectedItem.ToString();
            fromStart = strSelectedCPK.IndexOf("From", 0);
            string strCPKID = strSelectedCPK.Substring(0, fromStart - 1);
            string strVisionName = strCPKID.Substring(4);
            string strCPKStartTime = strSelectedCPK.Substring(fromStart + 7);
            string strCPKTime = strCPKStartTime.Substring(0, 4) + strCPKStartTime.Substring(5, 2) + strCPKStartTime.Substring(8, 2) +
                strCPKStartTime.Substring(11, 2) + strCPKStartTime.Substring(14, 2) + strCPKStartTime.Substring(17, 2);

            m_strSelectedFile = AppDomain.CurrentDomain.BaseDirectory + "CPKReport\\" + strLotID + "_" + strLotTime + "\\" + strVisionName + "\\" + strCPKID + "_" + strCPKTime + ".xml";

            if (File.Exists(m_strSelectedFile))
            {
                m_objFile = new XmlParser(m_strSelectedFile);
                m_objFile.GetFirstSection("Format");
                int intMaxGroup = m_objFile.GetValueAsInt("Group", 0);
                int intPartCount = m_objFile.GetValueAsInt("Part", 0);

                int intID = 0;
                string strGroupName = "";
                string strGroupNameFirstCharPrev = "";
                m_objFile.GetFirstSection("Group");
                for (int x = 0; x < intMaxGroup; x++)
                {
                    m_objFile.GetSecondSection("Group" + x);
                    strGroupName = m_objFile.GetValueAsString("GroupName", "No Found", 2);
                    if (!CheckROISelected(strGroupName))
                        continue;

                    DataRow drCPKGroup = m_dtCPKGroup.NewRow();
                    drCPKGroup["ID"] = intID;
                    drCPKGroup["Column1"] = strGroupName;

                    if (strGroupNameFirstCharPrev != strGroupName.Substring(0, strGroupName.LastIndexOf(' ')))
                    {
                        strGroupNameFirstCharPrev = strGroupName.Substring(0, strGroupName.LastIndexOf(' '));
                        drCPKGroup["Column2"] = "-------------------------------------------------------------------------------------" + strGroupNameFirstCharPrev + "------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------";
                    }

                    m_dtCPKGroup.Rows.Add(drCPKGroup);
                    int intItemsLength = m_objFile.GetThirdSectionCount() - 1;
                    for (int y = 0; y < intItemsLength; y++)
                    {
                        m_objFile.GetThirdSection("Items" + y);
                        DataRow drCPK = m_dtCPK.NewRow();
                        drCPK["ID"] = intID;
                        drCPK["Column1"] = m_objFile.GetValueAsString("Name", "No Found", 3);
                        drCPK["Column2"] = m_objFile.GetValueAsString("LSL", "0", 3);

                        //nominal
                        if (m_objFile.GetValueAsString("LSL", "0", 3) != "NA")
                            drCPK["Column3"] = ((Convert.ToSingle(m_objFile.GetValueAsString("LSL", "0", 3)) + Convert.ToSingle(m_objFile.GetValueAsString("USL", "0", 3))) / 2).ToString("f3");
                        else
                            drCPK["Column3"] = "NA";

                        drCPK["Column4"] = m_objFile.GetValueAsString("USL", "0", 3);
                        drCPK["Column5"] = m_objFile.GetValueAsString("MinValue", "0", 3);
                        drCPK["Column6"] = m_objFile.GetValueAsString("Mean", "0", 3);
                        drCPK["Column7"] = m_objFile.GetValueAsString("MaxValue", "0", 3);
                        drCPK["Column8"] = m_objFile.GetValueAsString("Sigma", "0", 3);

                        //3 sigma
                        if (m_objFile.GetValueAsString("Sigma", "0", 3) != "NA")
                            drCPK["Column9"] = (Convert.ToSingle(m_objFile.GetValueAsString("Sigma", "0", 3)) * 3).ToString("f5");
                        else
                            drCPK["Column9"] = "NA";

                        drCPK["Column10"] = m_objFile.GetValueAsString("CPResult", "0", 3);
                        drCPK["Column11"] = m_objFile.GetValueAsString("CPKResult", "0", 3);
                        m_dtCPK.Rows.Add(drCPK);
                    }
                    intID++;
                }

                m_report.Database.Tables["CPKGroup"].SetDataSource(m_dtCPKGroup);
                m_report.Database.Tables["CPK"].SetDataSource(m_dtCPK);

                m_objFile.GetFirstSection("VisionFeature");
                int intTotal = m_objFile.GetValueAsInt("Total", 0);
                int intPass = m_objFile.GetValueAsInt("Pass", 0);
                int intFail = intTotal - intPass;
                float fYield = 0;
                if (intTotal > 0)
                    fYield = (intPass / (float)intTotal) * 100;

                if (intPartCount != intTotal)
                    m_report.SetParameterValue("Total", intTotal.ToString() + " (Incomplete)");
                else
                    m_report.SetParameterValue("Total", intTotal.ToString());

                m_report.SetParameterValue("Pass", intPass.ToString());
                m_report.SetParameterValue("Fail", intFail.ToString());
                m_report.SetParameterValue("Yield", fYield.ToString() + "%");

                m_objFile.GetSecondSection("Fail");
                m_report.SetParameterValue("Offset", m_objFile.GetValueAsInt("Offset", 0, 2).ToString());
                m_report.SetParameterValue("Area", m_objFile.GetValueAsInt("Area", 0, 2).ToString());
                m_report.SetParameterValue("Width", m_objFile.GetValueAsInt("Width", 0, 2).ToString());
                m_report.SetParameterValue("Length", m_objFile.GetValueAsInt("Length", 0, 2).ToString());
                m_report.SetParameterValue("Pitch", m_objFile.GetValueAsInt("Pitch", 0, 2).ToString());
                m_report.SetParameterValue("Gap", m_objFile.GetValueAsInt("Gap", 0, 2).ToString());
                m_report.SetParameterValue("BrokenArea", m_objFile.GetValueAsInt("BrokenArea", 0, 2).ToString());
                m_report.SetParameterValue("BrokenLength", m_objFile.GetValueAsInt("BrokenLength", 0, 2).ToString());
                m_report.SetParameterValue("Excess", m_objFile.GetValueAsInt("Excess", 0, 2).ToString());
                m_report.SetParameterValue("Smear", m_objFile.GetValueAsInt("Smear", 0, 2).ToString());

                SetCPKParameterValue();

                HistoryCR.Zoom(125);
                HistoryCR.ReportSource = m_report;
            }
            else
                HistoryCR.ReportSource = null;
        }

        /// <summary>
        /// Fill in and display current month device edit log into data grid
        /// </summary>
        private void FillDeviceLogDatagrid()
        {
            if (cbo_Month.SelectedIndex == -1)
                return;

            string strSelectedMonth = cbo_Month.SelectedItem.ToString() + ".mdb";

            if (File.Exists(m_smProductionInfo.g_strHistoryDataLocation + "Data\\"+ strSelectedMonth))
            {
                if (STDeviceEdit2.m_dsDeviceEditLog == null)
                    STDeviceEdit2.InitDeviceEdit(m_smProductionInfo);

                STDeviceEdit2.GetDeviceEditLogDataSet(strSelectedMonth);
                if (m_smProductionInfo.g_blnWantEditLog)
                    dgd_DeviceEdit.DataSource = STDeviceEdit2.m_dsDeviceEditLog.Tables[0];

                bool bln_match = false;
                List<string> compare2 = new List<string>();
                List<string> strDelete = new List<string>(dgd_DeviceEdit.Rows.Count);
                int counter2 = 0;

                //start filter lot id
                if (m_blnRefillLot)
                {
                    for (int j = 0; j < cbo_Lot.Items.Count; j++)
                    {
                        bln_match = false;
                        for (int i = 0; i < dgd_DeviceEdit.Rows.Count; i++)
                        {
                            if (dgd_DeviceEdit.Rows[dgd_DeviceEdit.Rows.Count - 1].Cells[6].Value.ToString() == cbo_Lot.Items[j].ToString().Substring(0, cbo_Lot.Items[j].ToString().IndexOf(':') - 6) && compare2.Count == 0 && counter2 == 0) //check continuos lot
                            {
                                int counter = 0;
                                string compare = "";
                                string temp = cbo_Lot.Items[j].ToString().Substring(0, cbo_Lot.Items[j].ToString().IndexOf(':') - 6);
                                List<string> compare3 = new List<string>();

                                do
                                {
                                    if (temp.Equals(cbo_Lot.Items[counter].ToString().Substring(0, cbo_Lot.Items[counter].ToString().IndexOf(':') - 6)))
                                    {
                                        compare3.Add(cbo_Lot.Items[counter].ToString());

                                        if (compare == "")
                                        {
                                            compare = cbo_Lot.Items[counter].ToString().Substring(cbo_Lot.Items[counter].ToString().IndexOf('/') + 1, 2);
                                            counter2++;
                                        }
                                        else
                                        {
                                            if (Int16.Parse(cbo_Lot.Items[counter].ToString().Substring(cbo_Lot.Items[counter].ToString().IndexOf('/') + 1, 2)) < Int16.Parse(compare))
                                            {
                                                compare = cbo_Lot.Items[counter].ToString().Substring(cbo_Lot.Items[counter].ToString().IndexOf('/') + 1, 2);
                                                counter2++;
                                            }
                                            else
                                                counter2++;
                                        }
                                    }

                                    counter++;
                                } while (counter != cbo_Lot.Items.Count);


                                if (counter2 > 1)
                                {
                                    if (Int16.Parse(compare) != Int16.Parse(strSelectedMonth.Substring(5, 2)) - 1)
                                    {
                                        foreach (string s in compare3)
                                        {
                                            if (Int16.Parse(s.Substring(s.IndexOf('/') + 1, 2)) != Int16.Parse(strSelectedMonth.Substring(5, 2)) - 1)
                                            {
                                                if (Int16.Parse(s.Substring(s.IndexOf('/') + 1, 2)) == Int16.Parse(strSelectedMonth.Substring(5, 2)))
                                                    continue;
                                                else
                                                {
                                                    strDelete.Add(s);
                                                    compare2.Add(s);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                    bln_match = true;
                            }

                            if (dgd_DeviceEdit.Rows[i].Cells[6].Value.ToString() == cbo_Lot.Items[j].ToString().Substring(0, cbo_Lot.Items[j].ToString().IndexOf(':') - 6))
                            {
                                bln_match = true;
                                bool bln_match2 = false;

                                foreach (string s in compare2)
                                {
                                    if (s == cbo_Lot.Items[j].ToString()) //if already delete skip
                                    {
                                        bln_match2 = true;
                                        break;
                                    }
                                }

                                //string temp = dgd_DeviceEdit.Rows[i].Cells[7].Value.ToString().Substring(dgd_DeviceEdit.Rows[i].Cells[7].Value.ToString().IndexOf('/') + 1, 2);
                                //string temp2 = cbo_Lot.Items[j].ToString().Substring(cbo_Lot.Items[j].ToString().IndexOf('/') + 1, 2);

                                DateTime dtTemp = Convert.ToDateTime(dgd_DeviceEdit.Rows[i].Cells[7].Value);
                                string temp = dtTemp.Month.ToString("00");
                                int intIndexOf2 = cbo_Lot.Items[j].ToString().IndexOf(':') + 1;
                                if (intIndexOf2 < 0 || intIndexOf2 >= cbo_Lot.Items[j].ToString().Length)
                                {
                                    strDelete.Add(cbo_Lot.Items[j].ToString());
                                    break;
                                }
                                string temp2 = Convert.ToDateTime(cbo_Lot.Items[j].ToString().Substring(intIndexOf2)).Month.ToString("00");

                                if (temp.Contains("/"))
                                      temp = "0" + temp.Substring(0, 1);

                                if (compare2.Count != 0)
                                {
                                    if (compare2[0].Substring(0, compare2[0].IndexOf(':') - 6) == cbo_Lot.Items[j].ToString().Substring(0, cbo_Lot.Items[j].ToString().IndexOf(':') - 6) && !bln_match2) //if same id with deleted 
                                    {
                                        if (Int16.Parse(temp2) == Int16.Parse(strSelectedMonth.Substring(5, 2)) - 1 || Int16.Parse(temp2) == Int16.Parse(strSelectedMonth.Substring(5, 2))) //if equal to tis month lot or continuos lot from previous
                                            bln_match = true;
                                        else
                                        {
                                            if (temp != temp2) // check date if lot date is next month or further
                                            {
                                                strDelete.Add(cbo_Lot.Items[j].ToString());
                                                break;
                                            }
                                        }
                                    }
                                    else //if not same id direct check date
                                    {
                                        if (temp != temp2) // check date if lot date is next month or further
                                        {
                                            strDelete.Add(cbo_Lot.Items[j].ToString());
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (counter2 == 1)
                                        break;

                                    if (temp != temp2) // check date if lot date is next month or further
                                    {
                                        strDelete.Add(cbo_Lot.Items[j].ToString());
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (i == dgd_DeviceEdit.Rows.Count - 1)
                                {
                                    if (bln_match)
                                        continue;
                                    else
                                        strDelete.Add(cbo_Lot.Items[j].ToString());
                                }
                            }

                            //if (dgd_DeviceEdit.Rows[i].Cells[6].Value.ToString() == cbo_Lot.Items[j].ToString().Substring(0, cbo_Lot.Items[j].ToString().IndexOf(':') - 6))
                            //{
                            //    bln_match = true;
                            //    if (dgd_DeviceEdit.Rows[i].Cells[2].Value.ToString().Contains("New Lot")) 
                            //    {
                            //        string temp = dgd_DeviceEdit.Rows[i].Cells[7].Value.ToString().Substring(dgd_DeviceEdit.Rows[i].Cells[7].Value.ToString().IndexOf('/') + 1, 2);
                            //        string temp2 = cbo_Lot.Items[j].ToString().Substring(cbo_Lot.Items[j].ToString().IndexOf('/') + 1, 2);

                            //        if (temp.Contains("/"))
                            //            temp = "0" + temp.Substring(0, 1);

                            //        if (temp != temp2) //
                            //        {
                            //            strDelete.Add(cbo_Lot.Items[j].ToString());
                            //            break;
                            //        }
                            //    }

                        }
                    }

                    for (int i = 0; i < strDelete.Count; i++)
                    {
                        for (int j = 0; j < cbo_Lot.Items.Count; j++)
                        {
                            if (cbo_Lot.Items[j].ToString().Equals(strDelete[i]))
                            {
                                cbo_Lot.Items.Remove(strDelete[i]);
                                j--;
                            }
                        }
                    }

                    m_blnRefillLot = false;

                    if (cbo_Lot.Items.Count == 0) //if no lot set to filter by month to prevent error
                    {
                        m_intSelectedFilter = 2;
                        return;
                    }
                    else
                        cbo_Lot.SelectedIndex = 0;
                }

                //DeviceEdit objDeviceEdit = new DeviceEdit(m_smProductionInfo);
                //objDeviceEdit.GetDeviceEditLogDataSet(strSelectedMonth);
                //if (m_smProductionInfo.g_blnWantEditLog)
                //    dgd_DeviceEdit.DataSource = objDeviceEdit.m_dsDeviceEditLog.Tables[0];
                //objDeviceEdit.Dispose();


                //STDeviceEdit.ReloadDeviceEditLogDataSet(strSelectedMonth);
                //if (m_smProductionInfo.g_blnWantEditLog)
                //    dgd_DeviceEdit.DataSource = STDeviceEdit.m_dsDeviceEditLog.Tables[0];

            }
            else
                dgd_DeviceEdit.DataSource = null;
        }

        /// <summary>
        /// Fill in lot selection into combo box
        /// </summary>
        private void FillLotComboBox()
        {
            if (cbo_Month.SelectedIndex == -1)
                return;

            cbo_Lot.Items.Clear();

            ArrayList arrFileList = new ArrayList();
            string[] strLotList = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "LotReport", "*.xml");
            foreach (string strLot in strLotList)
            {
                //string strMonth = cbo_Month.SelectedItem.ToString().Substring(0, 4) + cbo_Month.SelectedItem.ToString().Substring(5, 2);   cxlim: take out tis will filter lot id later
                //if (strLot.IndexOf("_" + strMonth) > 0)
                    arrFileList.Add(new DirectoryInfo(strLot));
            }
            if (arrFileList.Count > 0)
            {
                //sort the folder lost until the latest new folder is at the first
                arrFileList.Sort(m_objTimeComparer);

                for (int i = 0; i < arrFileList.Count; i++)
                {
                    string[] strFilePart = ((DirectoryInfo)arrFileList[i]).Name.Split('_');
                    strFilePart[1] = strFilePart[1].Substring(0, strFilePart[1].Length - 4);
                    string strDate = strFilePart[1].Substring(0, 4) + "/" + strFilePart[1].Substring(4, 2) + "/" +
                        strFilePart[1].Substring(6, 2) + " " + strFilePart[1].Substring(8, 2) + ":" +
                        strFilePart[1].Substring(10, 2) + ":" + strFilePart[1].Substring(12, 2);
                    string strFileName = strFilePart[0] + " From : " + strDate;

                    cbo_Lot.Items.Add(strFileName);
                }                
            }

            if (!m_smProductionInfo.g_blnEndLotStatus)
            {
                string strTemp = m_smProductionInfo.g_strLotStartTime.Substring(0, 4) + "/" + m_smProductionInfo.g_strLotStartTime.Substring(4, 2) + "/" +
                          m_smProductionInfo.g_strLotStartTime.Substring(6, 2) + " " + m_smProductionInfo.g_strLotStartTime.Substring(8, 2) + ":" +
                          m_smProductionInfo.g_strLotStartTime.Substring(10, 2) + ":" + m_smProductionInfo.g_strLotStartTime.Substring(12, 2);
                cbo_Lot.Items.Insert(0, m_smProductionInfo.g_strLotID + " From : " + strTemp);
            }

            if (cbo_Lot.Items.Count > 0)
            {
                //cbo_Lot.SelectedIndex = 0;
                cbo_Lot.Enabled = true;
            }
        }

        /// <summary>
        /// Fill in month selection into combo box
        /// </summary>
        private void FillMonthComboBox()
        {
            string[] strHistoryList = Directory.GetFiles(m_smProductionInfo.g_strHistoryDataLocation + "Data", "*.mdb");
            FileSorting objTimeComparing = new FileSorting();
            Array.Sort(strHistoryList, objTimeComparing.CompareCreateDescending);
            foreach (string str in strHistoryList)
            {
                string strFile = Path.GetFileNameWithoutExtension(str);
                if (strFile != "History")
                {
                    cbo_Month.Items.Add(strFile);
                }
            }

            string[] strLotList = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "LotReport", "*.xml");
            Array.Sort(strLotList, objTimeComparing.CompareCreateDescending);
            foreach (string strLot in strLotList)
            {
                string strMonth = strLot.Substring(strLot.LastIndexOf('_') + 1, 4) + "-" + strLot.Substring(strLot.LastIndexOf('_') + 5, 2);
                if (cbo_Month.Items.IndexOf(strMonth) < 0)
                {
                    cbo_Month.Items.Add(strMonth);
                }
            }
           
            if (cbo_Month.Items.Count > 0)
                cbo_Month.SelectedIndex = 0;
            else
                cbo_Month.Enabled = false;
        }

        /// <summary>
        /// Fill in GRR selection into combo box
        /// </summary>
        /// <param name="strLotID"></param>
        private void FillGRRComboBox(string strLotID)
        {
            if (cbo_Lot.SelectedIndex == -1)
                return;

            cbo_GRR.Items.Clear();
            string strPath = AppDomain.CurrentDomain.BaseDirectory + "GRRReport\\" + strLotID + "\\";
            if (Directory.Exists(strPath) && Directory.GetDirectories(strPath).Length > 0)
            {
                cbo_GRR.Enabled = true;
                //cbo_VisionModule.Enabled = true;
                radioBtn_GRR.Enabled = true;
                btn_GRRDisplaySetting.Enabled = true;
                btn_Lead3DGRRDisplaySetting.Enabled = true;
            }
            else
            {
                cbo_GRR.Enabled = false;
                //cbo_VisionModule.Enabled = false;
                radioBtn_GRR.Enabled = false;
                btn_GRRDisplaySetting.Enabled = false;
                btn_Lead3DGRRDisplaySetting.Enabled = false;
            }

            ArrayList arrFileList = new ArrayList();
            if (Directory.Exists(strPath + cbo_VisionModule.SelectedItem.ToString()))
            {
                string[] strGRRList = Directory.GetFiles(strPath + cbo_VisionModule.SelectedItem.ToString(), "*.xml");
              
                foreach (string strGRR in strGRRList)
                {
                    string strMonth = cbo_Month.SelectedItem.ToString().Substring(0, 4) + cbo_Month.SelectedItem.ToString().Substring(5, 2);
                    if (strGRR.IndexOf("_" + strMonth) > 0)
                        arrFileList.Add(new DirectoryInfo(strGRR));
                }                
            }

            if (arrFileList.Count > 0)
            {
                //sort the folder lost until the latest new folder is at the first
                arrFileList.Sort(m_objTimeComparer);

                for (int i = 0; i < arrFileList.Count; i++)
                {
                    string[] strFilePart = ((DirectoryInfo)arrFileList[i]).Name.Split('_');
                    int intLastIndex = strFilePart.Length - 1;
                    string strDate = strFilePart[intLastIndex].Substring(0, strFilePart[intLastIndex].Length - 4);
                    strDate = strDate.Substring(0, 4) + "/" + strDate.Substring(4, 2) + "/" +
                        strDate.Substring(6, 2) + " " + strDate.Substring(8, 2) + ":" +
                        strDate.Substring(10, 2) + ":" + strDate.Substring(12, 2);
                    string strFileName = ((DirectoryInfo)arrFileList[i]).Name.Substring(0, ((DirectoryInfo)arrFileList[i]).Name.Length - strFilePart[intLastIndex].Length - 1) + " From : " + strDate;

                    cbo_GRR.Items.Add(strFileName);
                }

                cbo_GRR.SelectedIndex = 0;               
            }
            else
            {             
                cbo_GRR.SelectedIndex = -1;
                radioBtn_Lot.Checked = true;
                radioBtn_GRR.Checked = false;                           
            }
        }

        /// <summary>
        /// Fill in GRR selection into combo box
        /// </summary>
        /// <param name="strLotID"></param>
        private void FillCPKComboBox(string strLotID)
        {
            if (cbo_Lot.SelectedIndex == -1)
                return;

            cbo_CPK.Items.Clear();
            string strPath = AppDomain.CurrentDomain.BaseDirectory + "CPKReport\\" + strLotID + "\\";
            if (Directory.Exists(strPath) && Directory.GetDirectories(strPath).Length > 0)
            {
                cbo_CPK.Enabled = true;
                //cbo_VisionModule.Enabled = true;
                radioBtn_CPK.Enabled = true;
                btn_CPKDisplaySetting.Enabled = true;
            }
            else
            {
                cbo_CPK.Enabled = false;
                //cbo_VisionModule.Enabled = false;
                radioBtn_CPK.Enabled = false;
                btn_CPKDisplaySetting.Enabled = false;
            }

            ArrayList arrFileList = new ArrayList();
            if (Directory.Exists(strPath + cbo_VisionModule.SelectedItem.ToString()))
            {
                string[] strCPKList = Directory.GetFiles(strPath + cbo_VisionModule.SelectedItem.ToString(), "*.xml");

                foreach (string strCPK in strCPKList)
                {
                    string strMonth = cbo_Month.SelectedItem.ToString().Substring(0, 4) + cbo_Month.SelectedItem.ToString().Substring(5, 2);
                    if (strCPK.IndexOf("_" + strMonth) > 0)
                        arrFileList.Add(new DirectoryInfo(strCPK));
                }
            }

            if (arrFileList.Count > 0)
            {
                //sort the folder lost until the latest new folder is at the first
                arrFileList.Sort(m_objTimeComparer);

                for (int i = 0; i < arrFileList.Count; i++)
                {
                    string[] strFilePart = ((DirectoryInfo)arrFileList[i]).Name.Split('_');
                    int intLastIndex = strFilePart.Length - 1;
                    string strDate = strFilePart[intLastIndex].Substring(0, strFilePart[intLastIndex].Length - 4);
                    strDate = strDate.Substring(0, 4) + "/" + strDate.Substring(4, 2) + "/" +
                        strDate.Substring(6, 2) + " " + strDate.Substring(8, 2) + ":" +
                        strDate.Substring(10, 2) + ":" + strDate.Substring(12, 2);
                    string strFileName = ((DirectoryInfo)arrFileList[i]).Name.Substring(0, ((DirectoryInfo)arrFileList[i]).Name.Length - strFilePart[intLastIndex].Length - 1) + " From : " + strDate;

                    cbo_CPK.Items.Add(strFileName);
                }

                cbo_CPK.SelectedIndex = 0;
            }
            else
            {
                cbo_CPK.SelectedIndex = -1;
                radioBtn_Lot.Checked = true;
                radioBtn_CPK.Checked = false;
            }
        }

        /// <summary>
        /// Fill in what kind of vision module is available on this machine
        /// </summary>
        private void FillVisionModule()
        {
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] != null)
                {
                    cbo_VisionModule.Items.Add(m_smVSInfo[i].g_strVisionName);
                }
            }

            if(cbo_VisionModule.Items.Count > 0)
                cbo_VisionModule.SelectedIndex = 0;
        }

        private void InitGRRTable()
        {
            m_dtGRR.Columns.Add("ID");
            m_dtGRR.Columns.Add("Parameter");
            m_dtGRR.Columns.Add("GroupNo");
            m_dtGRR.Columns.Add("Specification");
            m_dtGRR.Columns.Add("R-Part");
            m_dtGRR.Columns.Add("UCL-Range");
            m_dtGRR.Columns.Add("RangeOfAverages");
            m_dtGRR.Columns.Add("AverageOfRBar");
            m_dtGRR.Columns.Add("RepeatabilityInUnit");
            m_dtGRR.Columns.Add("RepeatabilityInPercent");
            m_dtGRR.Columns.Add("ReproducilityInUnit");
            m_dtGRR.Columns.Add("ReproducilityInPercent");
            m_dtGRR.Columns.Add("R&RInUnit");
            m_dtGRR.Columns.Add("R&RInPercent");
            m_dtGRR.Columns.Add("PartVariationInUnit");
            m_dtGRR.Columns.Add("PartVariationInPercent");
            m_dtGRR.Columns.Add("Tolerance");
            m_dtGRR.Columns.Add("TotalVariation");
            m_dtGRR.Columns.Add("Ratio");
            m_dtGRR.Columns.Add("ndc");

            m_dtData.Columns.Add("ID");
            m_dtData.Columns.Add("Parts");
            m_dtData.Columns.Add("Column1");
            m_dtData.Columns.Add("Column2");
            m_dtData.Columns.Add("Column3");
            m_dtData.Columns.Add("Column4");
            m_dtData.Columns.Add("Column5");
            m_dtData.Columns.Add("Column6");
            m_dtData.Columns.Add("Column7");
            m_dtData.Columns.Add("Column8");
            m_dtData.Columns.Add("Column9");
            m_dtData.Columns.Add("Column10");
            m_dtData.Columns.Add("Column11");
            m_dtData.Columns.Add("Column12");
            m_dtData.Columns.Add("XBar");
            
            m_dtOperator.Columns.Add("ID");
            m_dtOperator.Columns.Add("SumA");
            m_dtOperator.Columns.Add("SumB");
            m_dtOperator.Columns.Add("SumC");
            m_dtOperator.Columns.Add("AverageA");
            m_dtOperator.Columns.Add("AverageB");
            m_dtOperator.Columns.Add("AverageC");
            m_dtOperator.Columns.Add("RBarA");
            m_dtOperator.Columns.Add("RBarB");
            m_dtOperator.Columns.Add("RBarC");
            m_dtOperator.Columns.Add("Column1");
            m_dtOperator.Columns.Add("Column2");
            m_dtOperator.Columns.Add("Column3");
            m_dtOperator.Columns.Add("Column4");
            m_dtOperator.Columns.Add("Column5");
            m_dtOperator.Columns.Add("Column6");
            m_dtOperator.Columns.Add("Column7");
            m_dtOperator.Columns.Add("Column8");
            m_dtOperator.Columns.Add("Column9");
            m_dtOperator.Columns.Add("Column10");
            m_dtOperator.Columns.Add("Column11");
            m_dtOperator.Columns.Add("Column12");
            m_dtOperator.Columns.Add("OpA");
            m_dtOperator.Columns.Add("OpB");
            m_dtOperator.Columns.Add("OpC");
        }


        private void InitCPKTable()
        {
            m_dtCPKGroup.Columns.Add("ID");
            m_dtCPKGroup.Columns.Add("Column1");
            m_dtCPKGroup.Columns.Add("Column2");

            m_dtCPK.Columns.Add("ID");
            m_dtCPK.Columns.Add("Column1");
            m_dtCPK.Columns.Add("Column2");
            m_dtCPK.Columns.Add("Column3");
            m_dtCPK.Columns.Add("Column4");
            m_dtCPK.Columns.Add("Column5");
            m_dtCPK.Columns.Add("Column6");
            m_dtCPK.Columns.Add("Column7");
            m_dtCPK.Columns.Add("Column8");
            m_dtCPK.Columns.Add("Column9");
            m_dtCPK.Columns.Add("Column10");
            m_dtCPK.Columns.Add("Column11");
        }

        /// <summary>
        /// Set GRR parameter
        /// </summary>
        private void SetGRRParameterValue()
        {
            m_objFile.GetFirstSection("Lot");
            m_report.SetParameterValue("MachineID", m_objFile.GetValueAsString("MachineID", "SRM"));
            m_report.SetParameterValue("LotID", m_objFile.GetValueAsString("LotID", "SRM"));
            m_report.SetParameterValue("DateTime", m_objFile.GetValueAsString("GRRStartTime", DateTime.Now.ToString()));
            m_report.SetParameterValue("OperatorID", m_objFile.GetValueAsString("OperatorID", "Op"));
            m_report.SetParameterValue("RecipeID", m_objFile.GetValueAsString("RecipeID", "Default"));
        }

        /// <summary>
        /// Set CPK parameter
        /// </summary>
        private void SetCPKParameterValue()
        {
            m_objFile.GetFirstSection("Lot");
            m_report.SetParameterValue("MachineID", m_objFile.GetValueAsString("MachineID", "SRM"));
            m_report.SetParameterValue("LotID", m_objFile.GetValueAsString("LotID", "SRM"));
            m_report.SetParameterValue("DateTime", m_objFile.GetValueAsString("CPKStartTime", DateTime.Now.ToString()));
            m_report.SetParameterValue("OperatorID", m_objFile.GetValueAsString("OperatorID", "Op"));
            m_report.SetParameterValue("RecipeID", m_objFile.GetValueAsString("RecipeID", "Default"));
        }

        /// <summary>
        /// Set lot summary parameter value
        /// </summary>
        private void SetParameterValue()
        {
            m_objFile.GetFirstSection("Lot");
            m_report.SetParameterValue("MachineID", m_objFile.GetValueAsString("MachineID", "SRM"));
            m_report.SetParameterValue("LotID", m_objFile.GetValueAsString("LotID", "SRM"));
            m_report.SetParameterValue("LotStartTime", m_objFile.GetValueAsString("LotStartTime", DateTime.Now.ToString()));
            m_report.SetParameterValue("LotStopTime", m_objFile.GetValueAsString("LotStopTime", DateTime.Now.ToString()));
            m_report.SetParameterValue("OperatorID", m_objFile.GetValueAsString("OperatorID", "Op"));
            m_report.SetParameterValue("RecipeID", m_objFile.GetValueAsString("RecipeID", "Default"));
        }




        private void cbo_Lot_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayLotSummary();
           
            tabCtrl_History.Focus();
            if (tabCtrl_History.SelectedTab.Name == "tp_DeviceLog")
            {
                //FillDeviceLogDatagrid();
                if (m_blnRefillLot)
                    return;

                UpdateFilter();
            }
        }

        private void cbo_Month_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_blnRefillLot = true;
            FillLotComboBox();
            if (tabCtrl_History.SelectedTab.Name == "tp_DeviceLog")
            {
                FillDeviceLogDatagrid();
            }
        }

        private void cbo_GRR_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioBtn_GRR.Checked)
            {
                if (cbo_VisionModule.SelectedItem.ToString().Contains("Lead3D"))
                    DisplayGRRReport_Lead3D();
                else
                    DisplayGRRReport();          
            }         

            tabCtrl_History.Focus();
        }

        private void cbo_VisionModule_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            string strSelectedLot = cbo_Lot.SelectedItem.ToString();
            int fromStart = strSelectedLot.IndexOf("From", 0);
            string strLotID = strSelectedLot.Substring(0, fromStart - 1);
            string strLotStartTime = strSelectedLot.Substring(fromStart + 7);
            string strLotTime = strLotStartTime.Substring(0, 4) + strLotStartTime.Substring(5, 2) + strLotStartTime.Substring(8, 2) +
                strLotStartTime.Substring(11, 2) + strLotStartTime.Substring(14, 2) + strLotStartTime.Substring(17, 2);
            
            FillGRRComboBox(strLotID + "_" + strLotTime);
            FillCPKComboBox(strLotID + "_" + strLotTime);

            //2019-09-27 ZJYEOH : Decide cbo_VisionModule.Enabled here, so that able to control GRR or CPK 
            if (cbo_GRR.Enabled || cbo_CPK.Enabled)
                cbo_VisionModule.Enabled = true;
            else
                cbo_VisionModule.Enabled = false;

            if (cbo_VisionModule.SelectedItem.ToString().Contains("Lead3D") || cbo_VisionModule.SelectedItem.ToString().Contains("Li3D"))
            {
                btn_Lead3DGRRDisplaySetting.BringToFront();
            }
            else
            {
                btn_Lead3DGRRDisplaySetting.SendToBack();
            }
        }



        private void radioBtn_Lot_Click(object sender, EventArgs e)
        {
            radioBtn_Lot.Checked = true;
            radioBtn_GRR.Checked = false;
            radioBtn_CPK.Checked = false;
            DisplayLotSummary();
        }

        private void radioBtn_GRR_Click(object sender, EventArgs e)
        {
            radioBtn_CPK.Checked = false;
            radioBtn_Lot.Checked = false;
            btn_Lead3DGRRDisplaySetting.Enabled = true;
            btn_GRRDisplaySetting.Enabled = true;
            if (cbo_VisionModule.SelectedItem.ToString().Contains("Lead3D"))
                DisplayGRRReport_Lead3D();
            else
                DisplayGRRReport();
        }

       

        private void btn_Close_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void btn_GRRDisplaySetting_Click(object sender, EventArgs e)
        {
            GRRDisplaySettingForm objGRRDisplaySettingForm = new GRRDisplaySettingForm(m_intParameterMask);
            if (objGRRDisplaySettingForm.ShowDialog() == DialogResult.OK)
            {
                m_intParameterMask = objGRRDisplaySettingForm.ref_intParameterMask;
                m_strParameter = objGRRDisplaySettingForm.ref_strParameter;
                if (radioBtn_GRR.Checked)
                    DisplayGRRReport();
            }
        }

        private void sbtn_CPKDisplaySetting_Click(object sender, EventArgs e)
        {
            CPKDisplaySettingForm objCPKDisplaySettingForm = new CPKDisplaySettingForm(m_intROIMask);
            if (objCPKDisplaySettingForm.ShowDialog() == DialogResult.OK)
            {
                m_intROIMask = objCPKDisplaySettingForm.ref_intROIMask;
                m_strROI = objCPKDisplaySettingForm.ref_strROI;
                if (radioBtn_CPK.Checked)
                    DisplayCPKReport();
            }
        }


        private void LotHistory_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }

        private void LotHistory_FormClosing(object sender, FormClosingEventArgs e)
        {
            Enabled = false;
        }

        private void tabCtrl_History_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabCtrl_History.SelectedTab.Name)
            {
                case "tp_DeviceLog":
                    lbl_Notice.Visible = true;
                    radioBtn_Lot.Visible = false;
                    UpdateFilter();
                    radioBtn_FilterByMonth.Visible = true;
                    radioBtn_FilterByLot.Visible = true;
                    btn_DeviceEditLogDisplaySetting.Visible = true;
                    btn_SavetoPDF.Visible = true;
                    cbo_Month.Visible = label1.Visible = true;
                    break;
                default:
                    lbl_Notice.Visible = false;
                    radioBtn_Lot.Visible = true;
                    radioBtn_Lot.Checked = true;
                    radioBtn_FilterByMonth.Visible = false;
                    radioBtn_FilterByLot.Visible = false;
                    btn_DeviceEditLogDisplaySetting.Visible = false;
                    btn_SavetoPDF.Visible = false;

                    cbo_Month.Visible = label1.Visible = false; // 2021 09 18 - CCENG: Currently lot ID will display all lot ID without month filtering. Cbo_Month is not affecting Lot ID combo box list, so can hide this month combo box.
                    m_blnRefillLot = true;
                    FillLotComboBox();
                    break;
            }
        }

        private void radioBtn_CPK_Click(object sender, EventArgs e)
        {
            radioBtn_CPK.Checked = true;
            radioBtn_Lot.Checked = false;
            radioBtn_GRR.Checked = false;
            btn_CPKDisplaySetting.Enabled = true;
            DisplayCPKReport();
        }

        private void cbo_CPK_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioBtn_CPK.Checked)
            {
                DisplayCPKReport();
            }

            tabCtrl_History.Focus();
        }

        private void tabCtrl_History_TabIndexChanged(object sender, EventArgs e)
        {
            switch (tabCtrl_History.SelectedTab.Name)
            {
                case "tp_DeviceLog":
                    m_blnRefillLot = true;
                    FillDeviceLogDatagrid();
                    radioBtn_FilterByMonth.Enabled = true;
                    radioBtn_FilterByLot.Enabled = true;
                    btn_DeviceEditLogDisplaySetting.Visible = true;
                    break;
                default:
                    radioBtn_FilterByMonth.Enabled = false;
                    radioBtn_FilterByLot.Enabled = false;
                    btn_DeviceEditLogDisplaySetting.Visible = false;
                    break;
            }
        }

        private void dgd_DeviceEdit_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3)
            {
                m_strFilterText = dgd_DeviceEdit.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                string strSelectedMonth = cbo_Month.SelectedItem.ToString();

                DeviceEditLogFilterForm objFilterForm = new DeviceEditLogFilterForm(m_strFilterText, m_strFilterColumn, m_strFilterMatchField, m_strFilterVisionModule, m_strFilterCategory, m_strFilterMethod,m_strFilterViewDaily,m_strFilterViewWeekly, strSelectedMonth,  cbo_VisionModule.Items.Count);
                if (objFilterForm.ShowDialog() == DialogResult.OK)
                {
                    StartWaiting("Filtering...");
                    //FillDeviceLogDatagrid();
                    UpdateFilter();
                    m_strFilterText = objFilterForm.ref_strFilterText;
                    m_strFilterColumn = objFilterForm.ref_strFilterColumn;
                    m_strFilterMatchField = objFilterForm.ref_strFilterMatchField;
                    m_strFilterVisionModule = objFilterForm.ref_strFilterVisionModule;
                    m_strFilterCategory = objFilterForm.ref_strFilterCategory;
                    m_strFilterMethod = objFilterForm.ref_strFilterMethod;
                    m_strFilterViewDaily = objFilterForm.ref_strFilterViewDaily;
                    m_strFilterViewWeekly = objFilterForm.ref_strFilterViewWeekly;

                    ViewOrderBy();
                    FilterWithKeyword();
                    lbl_Filter.Visible = true;
                    btn_RemoveFilter.Visible = true;
                    StopWaiting();
                }
                objFilterForm.Close();
                objFilterForm.Dispose();
            }
            else
            {
                if (dgd_DeviceEdit.Rows[e.RowIndex].Cells[5].Value.ToString().Contains(".bmp") && dgd_DeviceEdit.Rows[e.RowIndex].Cells[2].Value.ToString().Contains("Learn Template"))
                {
                    string Description = dgd_DeviceEdit.Rows[e.RowIndex].Cells[3].Value.ToString();
                    DateTime LogDateTime = Convert.ToDateTime(dgd_DeviceEdit.Rows[e.RowIndex].Cells[7].Value.ToString());

                    string ImageNameOld = dgd_DeviceEdit.Rows[e.RowIndex].Cells[4].Value.ToString();
                    string ImageNameNew = dgd_DeviceEdit.Rows[e.RowIndex].Cells[5].Value.ToString();
                    TemplateImageDisplayForm objForm = new TemplateImageDisplayForm(LogDateTime, Description, ImageNameOld, ImageNameNew, m_smProductionInfo.g_strHistoryDataLocation);
                    objForm.ShowDialog();
                    objForm.Close();
                    objForm.Dispose();
                }
            }
          
        }

        private void radioBtn_Filter_Click(object sender, EventArgs e)
        {
            if (radioBtn_FilterByLot.Checked)
            {
                m_intSelectedFilter = 1;
            }
            else if (radioBtn_FilterByMonth.Checked)
            {
                m_intSelectedFilter = 2;
            }
            UpdateFilter();
        }
        private void UpdateFilter()
        {
            if (m_intSelectedFilter == 1)
            {
                FillDeviceLogDatagrid();

                if(m_intSelectedFilter == 2)
                {
                    radioBtn_FilterByLot.Checked = false;
                    radioBtn_FilterByMonth.Checked = true;
                    return;
                }

                radioBtn_FilterByLot.Checked = true;
                radioBtn_FilterByMonth.Checked = false;

                if (cbo_Lot.Items.Count == 0) //no lot report no nid filter
                    return;

                string strSelectedLot = cbo_Lot.SelectedItem.ToString();
                int fromStart = strSelectedLot.IndexOf("From", 0);
                string strLotID = strSelectedLot.Substring(0, fromStart - 1);
                string strLotStartTime = strSelectedLot.Substring(fromStart + 7);
                DateTime dtStartDate = Convert.ToDateTime(strLotStartTime);
                DateTime dtEndDate = DateTime.Now;
                if (cbo_Lot.SelectedIndex != 0)
                {
                    string strSelectedLot2 = cbo_Lot.Items[cbo_Lot.SelectedIndex -1].ToString();
                    int fromStart2 = strSelectedLot2.IndexOf("From", 0);
                    string strLotStartTime2 = strSelectedLot2.Substring(fromStart2 + 7);
                    dtEndDate = Convert.ToDateTime(strLotStartTime2);
                }
                for (int i = 0; i < dgd_DeviceEdit.Rows.Count; i++)
                {
                    if (dgd_DeviceEdit.Rows[i].Cells[6].Value.ToString() != strLotID)
                    {
                        dgd_DeviceEdit.Rows.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        DateTime dtRowDateTime = Convert.ToDateTime(dgd_DeviceEdit.Rows[i].Cells[7].Value);  

                        if (dtRowDateTime < dtStartDate)// || dtRowDateTime >= dtEndDate)
                        {
                            dgd_DeviceEdit.Rows.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            else if (m_intSelectedFilter == 2)
            {
                radioBtn_FilterByMonth.Checked = true;
                radioBtn_FilterByLot.Checked = false;
                FillDeviceLogDatagrid();
            }
        }

        private void btn_Lead3DGRRDisplaySetting_Click(object sender, EventArgs e)
        {
            GRRLead3DDisplaySettingForm objGRRLead3DDisplaySettingForm = new GRRLead3DDisplaySettingForm(m_intParameterMask_Lead3D);
            if (objGRRLead3DDisplaySettingForm.ShowDialog() == DialogResult.OK)
            {
                m_intParameterMask_Lead3D = objGRRLead3DDisplaySettingForm.ref_intParameterMask;
                m_strParameter_Lead3D = objGRRLead3DDisplaySettingForm.ref_strParameter;
                if (radioBtn_GRR.Checked)
                    DisplayGRRReport_Lead3D();
            }
        }

        private void btn_DeviceEditLogDisplaySetting_Click(object sender, EventArgs e)
        {
            string strSelectedMonth = cbo_Month.SelectedItem.ToString();

            DeviceEditLogFilterForm objFilterForm = new DeviceEditLogFilterForm(m_strFilterText, m_strFilterColumn, m_strFilterMatchField, m_strFilterVisionModule, m_strFilterCategory, m_strFilterMethod, m_strFilterViewDaily, m_strFilterViewWeekly, strSelectedMonth, cbo_VisionModule.Items.Count);
            if (objFilterForm.ShowDialog() == DialogResult.OK)
            {
                StartWaiting("Filtering...");
                //FillDeviceLogDatagrid();
                UpdateFilter();
                m_strFilterText = objFilterForm.ref_strFilterText;
                m_strFilterColumn = objFilterForm.ref_strFilterColumn;
                m_strFilterMatchField = objFilterForm.ref_strFilterMatchField;
                m_strFilterVisionModule = objFilterForm.ref_strFilterVisionModule;
                m_strFilterCategory = objFilterForm.ref_strFilterCategory;
                m_strFilterMethod = objFilterForm.ref_strFilterMethod;
                m_strFilterViewDaily = objFilterForm.ref_strFilterViewDaily;
                m_strFilterViewWeekly = objFilterForm.ref_strFilterViewWeekly;

                ViewOrderBy();
                FilterWithKeyword();
                lbl_Filter.Visible = true;
                btn_RemoveFilter.Visible = true;
                StopWaiting();
            }
            objFilterForm.Close();
            objFilterForm.Dispose();
        }
        private void StartWaiting(string StrMessage)
        {
            m_thWaitingFormThread = new SRMWaitingFormThread();
            m_thWaitingFormThread.SetStartSplash(StrMessage);
            this.Enabled = false;
        }

        private void StopWaiting()
        {
            m_thWaitingFormThread.SetStopSplash();
            this.Enabled = true;
        }

        private void ViewOrderBy()
        {
            bool blnMatched = false;
            int Totaldays = 0;

            int diff = 0;
            int addon = 0;
            string strSelectedMonth = cbo_Month.SelectedItem.ToString();
            DateTime first = new DateTime(Int16.Parse(strSelectedMonth.Substring(0, 4)), 1, 1);

            if(first.DayOfWeek != DayOfWeek.Sunday)
            {
                addon += (7 -(int)first.DayOfWeek);
            }

            for (int i = 0; i < Int16.Parse(strSelectedMonth.Substring(5)); i++)
            {
                int month = i + 1;
                Totaldays += DateTime.DaysInMonth(Int16.Parse(strSelectedMonth.Substring(0, 4)), month);
            }
            
            int days = DateTime.DaysInMonth(Int16.Parse(strSelectedMonth.Substring(0, 4)), Int16.Parse(strSelectedMonth.Substring(5)));

            switch (m_strFilterMethod)
            {
                case "Weekly":
                    for (int i = 0; i < dgd_DeviceEdit.Rows.Count; i++)
                    {
                        blnMatched = false;
                        DateTime dt = Convert.ToDateTime(dgd_DeviceEdit.Rows[i].Cells[7].Value.ToString());
                        int intday = dt.Day;

                        if((Int16.Parse(m_strFilterViewWeekly) * 7) + addon > Totaldays)
                        {
                            DateTime date = new DateTime(Int16.Parse(strSelectedMonth.Substring(0, 4)), Int16.Parse(strSelectedMonth.Substring(5)), days);
                            if(date.DayOfWeek != DayOfWeek.Sunday)
                            {
                                diff = days - (int)date.DayOfWeek;
                            }
                        }
                        else
                        {
                            int temp = Math.Abs(Totaldays - ((Int16.Parse(m_strFilterViewWeekly) * 7) + addon));

                            if (temp > days)
                                diff = temp - days;
                            else
                                diff = days - temp;
                        }

                        DateTime d = new DateTime(Int16.Parse(strSelectedMonth.Substring(0, 4)), Int16.Parse(strSelectedMonth.Substring(5)), diff);

                        switch(d.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                if (intday >= diff && intday < diff + 7)
                                    blnMatched = true;
                                break;
                            case DayOfWeek.Monday:
                                if (intday >= diff - 1 && intday < diff + 6)
                                    blnMatched = true;
                                break;
                            case DayOfWeek.Tuesday:
                                if (intday >= diff - 2 && intday < diff + 5)
                                    blnMatched = true;
                                break;
                            case DayOfWeek.Wednesday:
                                if (intday >= diff - 3 && intday < diff + 4)
                                    blnMatched = true;
                                break;
                            case DayOfWeek.Thursday:
                                if (intday >= diff - 4 && intday < diff + 3)
                                    blnMatched = true;
                                break;
                            case DayOfWeek.Friday:
                                if (intday >= diff - 5 && intday < diff + 2)
                                    blnMatched = true;
                                break;
                            case DayOfWeek.Saturday:
                                if (intday >= diff - 6 && intday < diff + 1)
                                    blnMatched = true;
                                break;
                        }

                        if (!blnMatched)
                        {
                            dgd_DeviceEdit.Rows.RemoveAt(i);
                            i--;
                        }
                    }
                    break;
                case "Daily":
                    for (int i = 0; i < dgd_DeviceEdit.Rows.Count; i++)
                    {
                        blnMatched = false;
                        DateTime dt = Convert.ToDateTime(dgd_DeviceEdit.Rows[i].Cells[7].Value.ToString());
                        int intday = dt.Day;

                        days = Int16.Parse(m_strFilterViewDaily);

                        if (intday ==  days)
                            blnMatched = true;

                        if (!blnMatched)
                        {
                            dgd_DeviceEdit.Rows.RemoveAt(i);
                            i--;
                        }

                    }

                    break;
            }

        }

        private void FilterWithKeyword()
        {
            switch (m_strFilterColumn)
            {
                case "All":
                    for (int i = 0; i < dgd_DeviceEdit.Rows.Count; i++)
                    {
                        bool blnMatched = false;
                        for (int j = 0; j < 4; j++)
                        {
                            if (m_strFilterMatchField == "Whole Field")
                            {
                                if (dgd_DeviceEdit.Rows[i].Cells[j].Value.ToString().ToLower() == m_strFilterText.ToLower())
                                {
                                    if (m_strFilterCategory == "Any Category")
                                        blnMatched = true;
                                    else
                                    {
                                        if (dgd_DeviceEdit.Rows[i].Cells[j].Value.ToString().ToLower().Contains(m_strFilterCategory.ToLower()))
                                            blnMatched = true;
                                    }
                                }
                            }
                            else
                            {
                                if (dgd_DeviceEdit.Rows[i].Cells[j].Value.ToString().ToLower().Contains(m_strFilterText.ToLower()))
                                {
                                    if (m_strFilterCategory == "Any Category")
                                        blnMatched = true;
                                    else
                                    {
                                        if (dgd_DeviceEdit.Rows[i].Cells[j].Value.ToString().ToLower().Contains(m_strFilterCategory.ToLower()))
                                            blnMatched = true;
                                    }
                                }
                            }

                        }

                        if (blnMatched)
                        {
                            if (m_strFilterVisionModule != "All Vision")
                                if (!dgd_DeviceEdit.Rows[i].Cells[2].Value.ToString().ToLower().Contains(m_strFilterVisionModule.ToLower()))
                                    blnMatched = false;
                        }

                        if (!blnMatched)
                        {
                            dgd_DeviceEdit.Rows.RemoveAt(i);
                            i--;
                        }
                    }
                    break;
                case "User":
                    for (int i = 0; i < dgd_DeviceEdit.Rows.Count; i++)
                    {
                        bool blnMatched = false;

                        if (m_strFilterMatchField == "Whole Field")
                        {
                            if (dgd_DeviceEdit.Rows[i].Cells[0].Value.ToString().ToLower() == m_strFilterText.ToLower())
                            {
                                if (m_strFilterCategory == "Any Category")
                                    blnMatched = true;
                                else
                                {
                                    if (dgd_DeviceEdit.Rows[i].Cells[0].Value.ToString().ToLower().Contains(m_strFilterCategory.ToLower()))
                                        blnMatched = true;
                                }
                            }
                        }
                        else
                        {
                            if (dgd_DeviceEdit.Rows[i].Cells[0].Value.ToString().ToLower().Contains(m_strFilterText.ToLower()))
                            {
                                if (m_strFilterCategory == "Any Category")
                                    blnMatched = true;
                                else
                                {
                                    if (dgd_DeviceEdit.Rows[i].Cells[0].Value.ToString().ToLower().Contains(m_strFilterCategory.ToLower()))
                                        blnMatched = true;
                                }
                            }
                        }

                        if (blnMatched)
                        {
                            if (m_strFilterVisionModule != "All Vision")
                                if (!dgd_DeviceEdit.Rows[i].Cells[0].Value.ToString().ToLower().Contains(m_strFilterVisionModule.ToLower()))
                                    blnMatched = false;
                        }

                        if (!blnMatched)
                        {
                            dgd_DeviceEdit.Rows.RemoveAt(i);
                            i--;
                        }
                    }
                    break;
                case "Group":
                    for (int i = 0; i < dgd_DeviceEdit.Rows.Count; i++)
                    {
                        bool blnMatched = false;

                        if (m_strFilterMatchField == "Whole Field")
                        {
                            if (dgd_DeviceEdit.Rows[i].Cells[1].Value.ToString().ToLower() == m_strFilterText.ToLower())
                            {
                                if (m_strFilterCategory == "Any Category")
                                    blnMatched = true;
                                else
                                {
                                    if (dgd_DeviceEdit.Rows[i].Cells[1].Value.ToString().ToLower().Contains(m_strFilterCategory.ToLower()))
                                        blnMatched = true;
                                }
                            }
                        }
                        else
                        {
                            if (dgd_DeviceEdit.Rows[i].Cells[1].Value.ToString().ToLower().Contains(m_strFilterText.ToLower()))
                            {
                                if (m_strFilterCategory == "Any Category")
                                    blnMatched = true;
                                else
                                {
                                    if (dgd_DeviceEdit.Rows[i].Cells[1].Value.ToString().ToLower().Contains(m_strFilterCategory.ToLower()))
                                        blnMatched = true;
                                }
                            }
                        }

                        if (blnMatched)
                        {
                            if (m_strFilterVisionModule != "All Vision")
                                if (!dgd_DeviceEdit.Rows[i].Cells[1].Value.ToString().ToLower().Contains(m_strFilterVisionModule.ToLower()))
                                    blnMatched = false;
                        }

                        if (!blnMatched)
                        {
                            dgd_DeviceEdit.Rows.RemoveAt(i);
                            i--;
                        }
                    }
                    break;
                case "Module":
                    for (int i = 0; i < dgd_DeviceEdit.Rows.Count; i++)
                    {
                        bool blnMatched = false;

                        if (m_strFilterMatchField == "Whole Field")
                        {
                            if (dgd_DeviceEdit.Rows[i].Cells[2].Value.ToString().ToLower() == m_strFilterText.ToLower())
                            {
                                if (m_strFilterCategory == "Any Category")
                                    blnMatched = true;
                                else
                                {
                                    if (dgd_DeviceEdit.Rows[i].Cells[2].Value.ToString().ToLower().Contains(m_strFilterCategory.ToLower()))
                                        blnMatched = true;
                                }
                            }
                        }
                        else
                        {
                            if (dgd_DeviceEdit.Rows[i].Cells[2].Value.ToString().ToLower().Contains(m_strFilterText.ToLower()))
                            {
                                if (m_strFilterCategory == "Any Category")
                                    blnMatched = true;
                                else
                                {
                                    if (dgd_DeviceEdit.Rows[i].Cells[2].Value.ToString().ToLower().Contains(m_strFilterCategory.ToLower()))
                                        blnMatched = true;
                                }
                            }
                        }

                        if (blnMatched)
                        {
                            if (m_strFilterVisionModule != "All Vision")
                                if (!dgd_DeviceEdit.Rows[i].Cells[2].Value.ToString().ToLower().Contains(m_strFilterVisionModule.ToLower()))
                                    blnMatched = false;
                        }

                        if (!blnMatched)
                        {
                            dgd_DeviceEdit.Rows.RemoveAt(i);
                            i--;
                        }
                    }
                    break;
                case "Description":
                    for (int i = 0; i < dgd_DeviceEdit.Rows.Count; i++)
                    {
                        bool blnMatched = false;

                        if (m_strFilterMatchField == "Whole Field")
                        {
                            if (dgd_DeviceEdit.Rows[i].Cells[3].Value.ToString().ToLower() == m_strFilterText.ToLower())
                            {
                                if (m_strFilterCategory == "Any Category")
                                    blnMatched = true;
                                else
                                {
                                    if (dgd_DeviceEdit.Rows[i].Cells[3].Value.ToString().ToLower().Contains(m_strFilterCategory.ToLower()))
                                        blnMatched = true;
                                }
                            }
                        }
                        else
                        {
                            if (dgd_DeviceEdit.Rows[i].Cells[3].Value.ToString().ToLower().Contains(m_strFilterText.ToLower()))
                            {
                                if (m_strFilterCategory == "Any Category")
                                    blnMatched = true;
                                else
                                {
                                    if (dgd_DeviceEdit.Rows[i].Cells[3].Value.ToString().ToLower().Contains(m_strFilterCategory.ToLower()))
                                        blnMatched = true;
                                }
                            }
                        }

                        if (blnMatched)
                        {
                            if (m_strFilterVisionModule != "All Vision")
                                if (!dgd_DeviceEdit.Rows[i].Cells[3].Value.ToString().ToLower().Contains(m_strFilterVisionModule.ToLower()))
                                    blnMatched = false;
                        }

                        if (!blnMatched)
                        {
                            dgd_DeviceEdit.Rows.RemoveAt(i);
                            i--;
                        }
                    }
                    break;
            
            }
        }

        private void btn_RemoveFilter_Click(object sender, EventArgs e)
        {
            StartWaiting("Removing Filter...");
            FillDeviceLogDatagrid();
            UpdateFilter();
            lbl_Filter.Visible = false;
            btn_RemoveFilter.Visible = false;
            StopWaiting();
        }

        private void btn_SavetoPDF_Click(object sender, EventArgs e)
        {
            if (dgd_DeviceEdit.Rows.Count > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PDF (*.pdf)|*.pdf";
                sfd.FileName = "Edit Log.pdf";
                bool fileError = false;
                string strSelectedMonth = cbo_Month.SelectedItem.ToString();

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(sfd.FileName))
                    {
                        try
                        {
                            File.Delete(sfd.FileName);
                        }
                        catch (IOException ex)
                        {
                            fileError = true;
                            MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                        }
                    }
                    if (!fileError)
                    {
                        try
                        {
                            float[] width = {30, 50, 90, 100, 60, 60, 100,90};
                            iTextSharp.text.Font fontH1 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 7, 1);
                            iTextSharp.text.Font Title = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, 1);
                            iTextSharp.text.Font fontH2 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 8, 1);
                            PdfPTable pdfTable = new PdfPTable(dgd_DeviceEdit.Columns.Count);
                            pdfTable.SetWidths(width);
                            pdfTable.DefaultCell.Padding = 3;
                            pdfTable.WidthPercentage = 100;
                            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

                            PdfPCell[] temp = new PdfPCell[8];

                            for(int j=0;j<11;j++)
                            {
                                switch(j)
                                {
                                    case 0:
                                        temp[0] = new PdfPCell(new Phrase("Device Edit Log for Machine " + m_smCustomizeInfo.g_strMachineID));
                                        temp[0].HorizontalAlignment = Element.ALIGN_CENTER;
                                        break;
                                    case 2:
                                        temp[0] = new PdfPCell(new Phrase("Date: " + strSelectedMonth, fontH2));
                                        break;
                                    case 3:
                                        temp[0] = new PdfPCell(new Phrase("Order By: " + m_strFilterMethod, fontH2));
                                        break;
                                    case 4:
                                        if (m_strFilterMethod == "Weekly")
                                            temp[0] = new PdfPCell(new Phrase("Week: " + m_strFilterViewWeekly, fontH2));
                                        else if (m_strFilterMethod == "Daily")
                                            temp[0] = new PdfPCell(new Phrase("Week: " + m_strFilterViewWeekly, fontH2));
                                        else
                                            temp[0] = new PdfPCell(new Phrase(" "));
                                        break;
                                    case 5:
                                        if (m_strFilterMethod == "Daily")
                                            temp[0] = new PdfPCell(new Phrase("Day: " + m_strFilterViewDaily, fontH2));
                                        else
                                            temp[0] = new PdfPCell(new Phrase(" "));
                                        break;
                                    case 6:
                                        temp[0] = new PdfPCell(new Phrase("Category: " + m_strFilterCategory, fontH2));
                                        break;
                                    case 7:
                                        temp[0] = new PdfPCell(new Phrase("Vision Module: " + m_strFilterVisionModule, fontH2));
                                        break;
                                    case 8:
                                        temp[0] = new PdfPCell(new Phrase("Match Field: " + m_strFilterMatchField, fontH2));
                                        break;
                                    case 9:
                                        temp[0] = new PdfPCell(new Phrase("Column: " + m_strFilterColumn, fontH2));
                                        break;
                                    default:
                                        temp[0] = new PdfPCell(new Phrase(" "));
                                        break;
                                }

                                for (int i = 0; i < temp.Length; i++)
                                {
                                    temp[0].Colspan = 8;
                                    temp[i].Border = 0;
                                    pdfTable.AddCell(temp[i]);
                                    break;
                                }
                            }

                            if (m_strFilterMethod == "Monthly")
                            {
                                pdfTable.DeleteRow(4); 
                            }

                            foreach (DataGridViewColumn column in dgd_DeviceEdit.Columns)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, fontH2));
                                pdfTable.AddCell(cell);
                            }

                            foreach (DataGridViewRow row in dgd_DeviceEdit.Rows)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    Phrase c = new Phrase(cell.Value.ToString(),fontH1);
                                    pdfTable.AddCell(c);
                                }
                            }

                            using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                            {
                                iTextSharp.text.Document pdfDoc = new Document(PageSize.A4, 10f, 20f, 20f, 10f);
                                iTextSharp.text.pdf.PdfWriter.GetInstance(pdfDoc, stream);
                                pdfDoc.Open();
                                pdfDoc.Add(pdfTable);
                                pdfDoc.Close();
                                stream.Close();
                            }

                            MessageBox.Show("Data Exported Successfully !!!", "Info");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error :" + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No Record To Export !!!", "Info");
            }
        }

        private void btn_FolderStored_Click(object sender, EventArgs e)
        {
            DisplayImageAndDataSizeForm objForm = new DisplayImageAndDataSizeForm(m_smVSInfo, m_smProductionInfo);
            objForm.ShowDialog();
        }
    }

}