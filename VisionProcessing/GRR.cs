using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Common;

namespace VisionProcessing
{
    public class GRR
    {
        #region Member Variables

        private int m_intMaxPart = 0;
        private int m_intMaxOp = 0;
        private int m_intMaxTrial = 0;
        
        private int m_intPartCount = 0;
        private int m_intOperatorCount = 0;
        private int m_intTrialCount = 0;
        private int m_intGroupLength = 0;
        private int m_intPartLength = 0;
        private int m_intOperatorLength = 0;
        private int m_intTrialLength = 0;
        private int m_intPartSaveCount = 0;
        private int m_intOperatorSaveCount = 0;
        private int m_intTrialSaveCount = 0;
        private int m_intGRRMode = 0;

        private bool m_blnStart = false;
        
        private float[][] m_fLSL;         //D1: Item, D2: Group
        private float[][] m_fUSL;

        private float[][][][][] m_fData;  //D1: Item, D2: Group, D3: Part, D4: Operator, D5: Trial

        private double m_dEV = 0;
        private double m_dAV = 0;
        private double m_dRR = 0;
        private double m_dPV = 0;
        private double m_dNDC = 0; // number of distinct categories // must >= 5 to pass
        private double m_dPercentEV = 0;
        private double m_dPercentAV = 0;
        private double m_dPercentRR = 0;
        private double m_dPercentPV = 0;
        private double m_dTV = 0;
        private double m_dPTRatio;
        private double m_dTolerance = 1;  // Temporary
        private double m_dRPart = 0;
        private double m_dUCLRange = 0;
        private double m_dAverageRBar = 0;
        private double m_dRangeAverage = 0;
        private double[] m_dXBar;
        private double[] m_dSumXOpe;
        private double[] m_dAveXOpe;
        private double[] m_dAveROpe;
        private double[] m_dSumRangeColumn;
        private double[][] m_dRange;
        private double[][] m_dSumTrialColumn;

        private string m_strLotID;
        private string m_strOpeID;
        private string m_strRecipeID;
        private string m_strMachineID;
        private string m_strVisionID;
        private string[] m_strItems;
        private string[] m_strGroupName;

        private DateTime m_startDateTime = new DateTime();
        #endregion

        #region Properties

        public int ref_intMaxPart { set { m_intMaxPart = value; } get { return m_intMaxPart; } }
        public int ref_intMaxOp { set { m_intMaxOp = value; } get { return m_intMaxOp; } }
        public int ref_intMaxTrial { set { m_intMaxTrial = value; } get { return m_intMaxTrial; } }

        public int ref_intPartCount { set { m_intPartCount = value; } get { return m_intPartCount; } }
        public int ref_intOperatorCount { set { m_intOperatorCount = value; } get { return m_intOperatorCount; } }
        public int ref_intTrialCount { set { m_intTrialCount = value; } get { return m_intTrialCount; } }
        public int ref_intGRRMode { set { m_intGRRMode = value; } get { return m_intGRRMode; } }
        public int ref_intGroupLength { set { m_intGroupLength = value; } get { return m_intGroupLength; } }
        public int ref_intItemsLength { get { return m_strItems.Length; } }

        public double ref_dRPart { get { return m_dRPart; } }
        public double ref_dAverageRBar { get { return m_dAverageRBar; } }
        public double ref_dUCLRange { get { return m_dUCLRange; } }
        public double ref_dRangeAverage { get { return m_dRangeAverage; } }
        public double ref_dEV { get { return m_dEV; } }
        public double ref_dAV { get { return m_dAV; } }
        public double ref_dRR { get { return m_dRR; } }
        public double ref_dPV { get { return m_dRR; } }
        public double ref_dPercentEV { get { return m_dPercentEV; } }
        public double ref_dPercentAV { get { return m_dPercentAV; } }
        public double ref_dPercentRR { get { return m_dPercentRR; } }
        public double ref_dPercentPV { get { return m_dPercentPV; } }
        public double ref_dTolerance { get { return m_dTolerance; } }
        public double ref_dTV { get { return m_dTV; } }
        public double ref_dPTRatio { get { return m_dPTRatio; } }

        #endregion


        public GRR(string[] strItems)
        {
            m_strItems = strItems;
            m_fUSL = new float[m_strItems.Length][];
            m_fLSL = new float[m_strItems.Length][];
            m_fData = new float[m_strItems.Length][][][][];
        }


        /// <summary>
        /// Init parameter of GRR 
        /// </summary>
        /// <param name="strLotID">Lot Name</param>
        /// <param name="strOpeID">Operator ID</param>
        /// <param name="strRecipeID">Recipe Name</param>
        /// <param name="strMachineID">Machine Name</param>
        /// <param name="strVisionID">Vision Name</param>
        /// <param name="intGroupLength">Tested Unit count on each images</param>
        /// <param name="intPartLength"></param>
        /// <param name="intOperatorLength"></param>
        /// <param name="intTrialLength"></param>
        public void Init(string strLotID, string strOpeID, string strRecipeID, string strMachineID, string strVisionID, 
                         int intGroupLength, int intPartLength, int intOperatorLength, int intTrialLength)
        {
            m_strLotID = strLotID;
            m_strOpeID = strOpeID;
            m_strRecipeID = strRecipeID;
            m_strMachineID = strMachineID;
            m_strVisionID = strVisionID;
            m_intGroupLength = intGroupLength;
            m_intMaxPart = intPartLength;
            m_intMaxOp = intOperatorLength;
            m_intMaxTrial = intTrialLength;

            Reset();

            for (int x = 0; x < m_fData.Length; x++)
            {
                m_fData[x] = new float[m_intGroupLength][][][];
                m_fLSL[x] = new float[m_intGroupLength];
                m_fUSL[x] = new float[m_intGroupLength];
            }

            m_strGroupName = new string[m_intGroupLength];

            SetTestCounter(intPartLength, intOperatorLength, intTrialLength);
        }
        /// <summary>
        /// Load GRR report from file
        /// File is loaded from "Current Directory...\GRRReport\LotID\VisionID\GRR_VisionID_StartDateTime.xml"
        /// </summary>
        /// <param name="strPath">Load file location</param>
        public void Load(string strPath)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection("Lot");
            m_strLotID = objFile.GetValueAsString("LotID", "No Found");
            m_strOpeID = objFile.GetValueAsString("OperatorID", "No Found");
            m_strRecipeID = objFile.GetValueAsString("RecipeID", "No Found");
            m_strMachineID = objFile.GetValueAsString("MachineID", "No Found");
            m_strVisionID = objFile.GetValueAsString("VisionID", "No Found");
            m_startDateTime = Convert.ToDateTime(objFile.GetValueAsString("GRRStartTime", "No Found"));

            int x, y, i, j, k;

            objFile.GetFirstSection("Format");
            m_intGroupLength = objFile.GetValueAsInt("Group", 0);
            m_intPartLength = objFile.GetValueAsInt("Part", 0);
            m_intOperatorLength = objFile.GetValueAsInt("Operator", 0);
            m_intTrialLength = objFile.GetValueAsInt("Trial", 0);

            objFile.GetFirstSection("Items");
            int intItemsLength = objFile.GetSecondSectionCount();
            m_strItems = new string[intItemsLength];
            m_fUSL = new float[intItemsLength][];
            m_fLSL = new float[intItemsLength][];
            for (x = 0; x < intItemsLength; x++)
            {
                m_fLSL[x] = new float[m_intGroupLength];
                m_fUSL[x] = new float[m_intGroupLength];
                m_strGroupName = new string[m_intGroupLength];
                for (y = 0; y < m_intGroupLength; y++)
                {
                    objFile.GetSecondSection("Items" + x);
                    m_strItems[x] = objFile.GetValueAsString("Name", " No Found", 2);
                    m_fLSL[x][y] = objFile.GetValueAsFloat("LSL", 0, 2);
                    m_fUSL[x][y] = objFile.GetValueAsFloat("USL", 1, 2);
                    m_strGroupName[y] = objFile.GetValueAsString("GroupName", "", 2);

                }
            }

            m_fData = new float[intItemsLength][][][][];
            int intGroupLength;
            int intPartLength;
            int intOperatorLength;
            int intTrialLength;
            for (x = 0; x < intItemsLength; x++)
            {
                objFile.GetFirstSection("Items" + x);
                intGroupLength = objFile.GetSecondSectionCount();
                m_fData[x] = new float[intGroupLength][][][];

                for (y = 0; y < intGroupLength; y++)
                {
                    objFile.GetSecondSection("Group" + y);
                    intPartLength = objFile.GetThirdSectionCount();
                    m_fData[x][y] = new float[intPartLength][][];

                    for (i = 0; i < intPartLength; i++)
                    {
                        objFile.GetThirdSection("Part" + i);
                        intOperatorLength = objFile.GetFourthSectionCount();
                        m_fData[x][y][i] = new float[intOperatorLength][];

                        for (j = 0; j < intOperatorLength; j++)
                        {
                            objFile.GetFourthSection("Operator" + j);
                            intTrialLength = objFile.GetFifthSectionCount();
                            m_fData[x][y][i][j] = new float[intTrialLength];

                            for (k = 0; k < intTrialLength; k++)
                            {
                                m_fData[x][y][i][j][k] = objFile.GetValueAsFloat("Trial" + k, 999, 4);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Set GRR test counter to next index
        /// </summary>
        public void NextIndex()
        {
            // GRR Static Mode: Trial -> Operator -> Part
            if (m_intGRRMode == 0)
            {
                if (m_intTrialCount >= m_intTrialLength)
                {
                    m_intTrialCount = 0;
                    if (m_intOperatorCount >= m_intOperatorLength)
                        m_intOperatorCount = 0;
                }

                m_intTrialCount++;
                if (m_intTrialCount == 1)
                {
                    m_intOperatorCount++;
                    if (m_intOperatorCount == 1)
                    {
                        m_intPartCount++;
                    }
                }

                m_intTrialSaveCount++;
                if (m_intTrialSaveCount >= m_intTrialLength)
                {
                    m_intTrialSaveCount = 0;
                    m_intOperatorSaveCount++;
                    if (m_intOperatorSaveCount >= m_intOperatorLength)
                    {
                        m_intTrialSaveCount = 0;
                        m_intOperatorSaveCount = 0;
                        m_intPartSaveCount++;
                        if (m_intPartSaveCount >= m_intPartLength)
                        {
                            SaveReport();
                        }
                    }
                }
            }
            else // GRR Dynamic Mode: Part -> Trial -> Operator
            {
                if (m_intPartCount >= m_intPartLength)
                {
                    m_intPartCount = 0;
                    if (m_intTrialCount >= m_intTrialLength)
                        m_intTrialCount = 0;
                }

                m_intPartCount++;
                if (m_intPartCount == 1)
                {
                    m_intTrialCount++;
                    if (m_intTrialCount == 1)
                    {
                        m_intOperatorCount++;
                    }
                }

                m_intPartSaveCount++;
                if (m_intPartSaveCount >= m_intPartLength)
                {
                    m_intPartSaveCount = 0;
                    m_intTrialSaveCount++;
                    if (m_intTrialSaveCount >= m_intTrialLength)
                    {
                        m_intPartSaveCount = 0;
                        m_intTrialSaveCount = 0;
                        m_intOperatorSaveCount++;
                        if (m_intOperatorSaveCount >= m_intOperatorLength)
                        {
                            SaveReport();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Record GRR data
        /// </summary>
        /// <param name="intItemIndex">Item index</param>
        /// <param name="intGroupIndex">Group index</param>
        /// <param name="fValue">GRR data</param>
        public bool Record(int intItemIndex, int intGroupIndex, float fValue)
        {
            if (!m_blnStart)
            {
                Reset();
                m_blnStart = true;
            }


            if (m_fData.Length > intItemIndex)
            {
                if (m_fData[intItemIndex].Length > intGroupIndex)
                {
                    if (m_fData[intItemIndex][intGroupIndex].Length == 0)
                    {
                        SRMMessageBox.Show("Part, Operator and Trial setting should not set to 0.", "GRR", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                        return false;
                    }

                    if (m_fData[intItemIndex][intGroupIndex].Length > m_intPartSaveCount)
                    {
                        if (m_fData[intItemIndex][intGroupIndex][m_intPartSaveCount].Length > m_intOperatorSaveCount)
                        {
                            if (m_fData[intItemIndex][intGroupIndex][m_intPartSaveCount][m_intOperatorSaveCount].Length > m_intTrialSaveCount)
                            {
                                m_fData[intItemIndex][intGroupIndex][m_intPartSaveCount][m_intOperatorSaveCount][m_intTrialSaveCount] = fValue;
                                return true;
                            }
                        }
                    }
                }
            }
            SRMMessageBox.Show("GRR Storage full. Please reset GRR storage before record new data again.", "GRR", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
            return false;
        }

        public bool Record(int intItemIndex, int intGroupIndex, string strGroupName, float fValue)
        {
            if (!m_blnStart)
            {
                Reset();
                m_blnStart = true;
            }


            if (m_fData.Length > intItemIndex)
            {
                if (m_fData[intItemIndex].Length > intGroupIndex)
                {
                    if (m_fData[intItemIndex][intGroupIndex].Length == 0)
                    {
                        SRMMessageBox.Show("Part, Operator and Trial setting should not set to 0.", "GRR", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                        return false;
                    }

                    if (m_strGroupName[intGroupIndex] != strGroupName)
                        m_strGroupName[intGroupIndex] = strGroupName;

                    if (m_fData[intItemIndex][intGroupIndex].Length > m_intPartSaveCount)
                    {
                        if (m_fData[intItemIndex][intGroupIndex][m_intPartSaveCount].Length > m_intOperatorSaveCount)
                        {
                            if (m_fData[intItemIndex][intGroupIndex][m_intPartSaveCount][m_intOperatorSaveCount].Length > m_intTrialSaveCount)
                            {
                                m_fData[intItemIndex][intGroupIndex][m_intPartSaveCount][m_intOperatorSaveCount][m_intTrialSaveCount] = fValue;
                                return true;
                            }
                        }
                    }
                }
            }
            SRMMessageBox.Show("GRR Storage full. Please reset GRR storage before record new data again.", "GRR", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
            return false;
        }

        /// <summary>
        /// Reset all GRR data and counter
        /// </summary>
        public void Reset()
        {
            m_intPartCount = m_intOperatorCount = m_intTrialCount = 0;
            m_intPartSaveCount = m_intOperatorSaveCount = m_intTrialSaveCount = 0;
            m_dNDC = m_dEV = m_dAV = m_dRR = m_dPV = 0;
            m_dPercentEV = m_dPercentAV = m_dPercentRR = m_dPercentPV = 0;

            m_startDateTime = DateTime.Now;
        }
        /// <summary>
        /// Save GRR report to file
        /// File is saved at "Current Directory...\GRRReport\LotID\VisionID\GRR_VisionID_StartDateTime.xml"
        /// </summary>
        public void SaveReport()
        {
            CheckFolderDirectory();
            //DeviceEdit objDeviceEdit = new DeviceEdit(1,"test", "123yeoh");
            //STDeviceEdit.CopySettingFile(AppDomain.CurrentDomain.BaseDirectory + "GRRReport\\" +
            //    m_strLotID + "\\" + m_strVisionID + "\\","GRR_" + m_strVisionID + "_" + m_startDateTime.ToString("yyyyMMddHHmmss") + ".xml");
            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "GRRReport\\" +
                m_strLotID + "\\" + m_strVisionID + "\\GRR_" + m_strVisionID + "_" + m_startDateTime.ToString("yyyyMMddHHmmss") + ".xml");

            objFile.WriteSectionElement("Lot", false);
            objFile.WriteElement1Value("LotID", m_strLotID, "Lot ID", true);
            objFile.WriteElement1Value("OperatorID", m_strOpeID, "Operator ID", true);
            objFile.WriteElement1Value("RecipeID", m_strRecipeID, "Recipe ID", true);
            objFile.WriteElement1Value("MachineID", m_strMachineID, "Machine ID", true);
            objFile.WriteElement1Value("VisionID", m_strVisionID, "Vision Station Name", true);
            objFile.WriteElement1Value("GRRStartTime", m_startDateTime.ToString("yyyy/MM/dd HH:mm:ss"), "GRR Creation Time", true);
          
            // Save value
            int x, y, i, j, k;
            objFile.WriteSectionElement("Items", true);
            for (x = 0; x < m_strItems.Length; x++)
            {
                objFile.WriteElement1Value("Items" + x, "", "Items No", true);
                objFile.WriteElement2Value("Name", m_strItems[x], "Operator Name", true);
                for (y = 0; y < m_intGroupLength; y++)
                {
                    objFile.WriteElement2Value("Group" + y, "", "Group3 No", true);
                    objFile.WriteElement3Value("GroupName", m_strGroupName[y], "Group Name", true);
                    objFile.WriteElement3Value("LSL", m_fLSL[x][y].ToString("f4"), "Lower Specification Limit", true);
                    objFile.WriteElement3Value("USL", m_fUSL[x][y].ToString("f4"), "Upper Specification Limit", true);
                }
            }

            objFile.WriteSectionElement("Format", false);
            objFile.WriteElement1Value("Group", m_intGroupLength, "Group Counter", true);
            objFile.WriteElement1Value("Part", m_intPartLength, "Part Counter", true);
            objFile.WriteElement1Value("Operator", m_intOperatorLength, "Operator Counter", true);
            objFile.WriteElement1Value("Trial", m_intTrialLength, "Trial Counter", true);

            for (x = 0; x < m_fData.Length; x++)
            {
                objFile.WriteSectionElement("Items" + x, true);
                for (y = 0; y < m_intGroupLength; y++)
                {
                    objFile.WriteElement1Value("Group" + y, "", "Group No.", true);

                    // Calculate GRR
                    CalculateGRR(x, y);

                    for (i = 0; i < m_intPartLength; i++)
                    {
                        objFile.WriteElement2Value("Part" + i, "", "Part No.", true);
                        for (j = 0; j < m_intOperatorLength; j++)
                        {
                            objFile.WriteElement3Value("Operator" + j, "", "Operator ID", true);
                            for (k = 0; k < m_intTrialLength; k++)
                            {
                                objFile.WriteElement4Value("Trial" + k, m_fData[x][y][i][j][k].ToString("f4"), "Trial No", true); // "F3"
                            }
                            objFile.WriteElement4Value("Range" + j, m_dRange[i][j].ToString("f4"), "Range No.", true);// "F3"
                        }
                        objFile.WriteElement3Value("XBar" + i, m_dXBar[i].ToString("f4"), "XBar No.", true);// "F3"
                    }

                    objFile.WriteElement2Value("Total", "", "Total", true);
                    for (j = 0; j < m_intOperatorLength; j++)
                    {
                        objFile.WriteElement3Value("Operator" + j, "", "Operator ID", true);
                        for (k = 0; k < m_intTrialLength; k++)
                        {
                            objFile.WriteElement4Value("TotalTrial" + k, m_dSumTrialColumn[j][k].ToString("f4"), "Total Trial " + k, true);// "F3"
                        }
                        objFile.WriteElement3Value("TotalRange" + j, m_dSumRangeColumn[j].ToString("f4"), "Total Range " + j, true);// "F3"
                        objFile.WriteElement3Value("TotalTrial" + j, m_dSumXOpe[j].ToString("f4"), "Total Trial " + j, true);// "F3"
                        objFile.WriteElement3Value("AverageTrial" + j, m_dAveXOpe[j].ToString("f4"), "Average Trial " + j, true);// "F3"
                        objFile.WriteElement3Value("AverageXBar" + j, m_dAveROpe[j].ToString("f4"), "Average XBar " + j, true);// "F3"
                    }

                    objFile.WriteElement2Value("R-Part", m_dRPart.ToString("F5"), "R-Part",true);
                    objFile.WriteElement2Value("UCL-Range", m_dUCLRange.ToString("F5"), "UCL-Range", true);
                    objFile.WriteElement2Value("RangeAverage", m_dRangeAverage.ToString("F5"), "Range Average", true);
                    objFile.WriteElement2Value("AverageR-Bar", m_dAverageRBar.ToString("F5"), "AverageR-Bar", true);

                    objFile.WriteElement2Value("EV", m_dEV.ToString("F5"), "EV", true);
                    objFile.WriteElement2Value("AV", m_dAV.ToString("F5"), "AV", true);
                    objFile.WriteElement2Value("RR", m_dRR.ToString("F5"), "PR", true);
                    objFile.WriteElement2Value("PV", m_dPV.ToString("F5"), "PV", true);

                    objFile.WriteElement2Value("NDC", m_dNDC.ToString("F5"), "NDC", true);

                    objFile.WriteElement2Value("PercentEV", m_dPercentEV.ToString("F2"), "PercentEV", true);
                    objFile.WriteElement2Value("PercentAV", m_dPercentAV.ToString("F2"), "PercentAV", true);
                    objFile.WriteElement2Value("PercentRR", m_dPercentRR.ToString("F2"), "PercentRR", true);
                    objFile.WriteElement2Value("PercentPV", m_dPercentPV.ToString("F2"), "PercentPV", true);

                    objFile.WriteElement2Value("Tolerance", m_dTolerance.ToString("f4"), "Tolerance", true);// "F3"
                    objFile.WriteElement2Value("TV", m_dTV.ToString("f4"), "TV", true);// "F3"
                    objFile.WriteElement2Value("PT", m_dPTRatio.ToString("f4"), "PT", true);// "F3"
                }
            }
            objFile.WriteEndElement();
            //STDeviceEdit.XMLChangesTracing(  "Pad-GRR", AppDomain.CurrentDomain.BaseDirectory + "GRRReport\\" +
            //    m_strLotID + "\\" + m_strVisionID + "\\","GRR_" + m_strVisionID + "_" + m_startDateTime.ToString("yyyyMMddHHmmss") + ".xml");
        }
        /// <summary>
        /// Set group/unit counter. 
        /// E.g Set group counter to 2 if want to save GRR data for current unit and next unit.
        /// </summary>
        /// <param name="intGroupLength">Group/unit counter</param>
        public void SetGroupCounter(int intGroupLength)
        {
            if (m_fData.Length == 0)
                return;

            m_intGroupLength = intGroupLength;            
        }
        /// <summary>
        /// Set GRR specification 
        /// </summary>
        /// <param name="intItemIndex">Item index. E.g Index 0 = position-x, index 1 = position-y</param>
        /// <param name="intGroupIndex">Group index. E.g Index 0 = current unit, index 2 = next unit</param>
        /// <param name="fLSL">LSL value</param>
        /// <param name="fUSL">USL value</param>
        public void SetSpecification(int intItemIndex, int intGroupIndex, float fLSL, float fUSL)
        {
            if ((intItemIndex >= m_fLSL.Length) || (intItemIndex >= m_fUSL.Length))
                return;

            if ((intGroupIndex >= m_fLSL[intItemIndex].Length) || (intGroupIndex >= m_fUSL[intItemIndex].Length))
                return;

            m_fLSL[intItemIndex][intGroupIndex] = fLSL;
            m_fUSL[intItemIndex][intGroupIndex] = fUSL;
        }
        /// <summary>
        /// Set GRR Part, Operator and Trial counter setting
        /// </summary>
        /// <param name="intPartLength">Sample count</param>
        /// <param name="intOperatorLength">Operator count</param>
        /// <param name="intTrialLength">Trial count</param>
        public void SetTestCounter(int intPartLength, int intOperatorLength, int intTrialLength)
        {
            m_intPartLength = intPartLength;
            m_intOperatorLength = intOperatorLength;
            m_intTrialLength = intTrialLength;

            int x, y, i, j ,k;
            for (x = 0; x < m_fData.Length; x++)
            {
                for (y = 0; y < m_fData[x].Length; y++)
                {
                    m_fData[x][y] = new float[m_intPartLength][][];
                    for (i = 0; i < m_intPartLength; i++)
                    {
                        m_fData[x][y][i] = new float[m_intOperatorLength][];

                        for (j = 0; j < m_intOperatorLength; j++)
                        {
                            m_fData[x][y][i][j] = new float[m_intTrialLength];

                            for (k = 0; k < m_intTrialLength; k++)
                                m_fData[x][y][i][j][k] = -999;
                        }

                    }
                }
            }
        }



        /// <summary>
        /// Start calculate GRR
        /// </summary>
        /// <param name="intItemIndex"></param>
        /// <param name="intGroupIndex"></param>
        private void CalculateGRR(int intItemIndex, int intGroupIndex)
        {
            #region Init
            int i, j, k;

            double[] dSum = new double[m_intOperatorLength];

            // Temp
            double dValue = 0;
            double dSumX = 0;
            double dSumXX = 0;
            double dMinAve = 0;
            double dMaxAve = 0;
            double dMinXBar = 0;
            double dMaxXBar = 0;
            double dMinValue = 0;
            double dMaxValue = 0;
            double dSumTrialRow = 0;

            m_dRange = new double[m_intPartLength][];  //trial max value - trial min value from each part->operator
            m_dXBar = new double[m_intPartLength];     //trial average value from each part  
            m_dSumRangeColumn = new double[m_intOperatorLength];
            m_dSumTrialColumn = new double[m_intOperatorLength][];
            m_dSumXOpe = new double[m_intOperatorLength];
            m_dAveXOpe = new double[m_intOperatorLength];
            m_dAveROpe = new double[m_intOperatorLength];

            for (i = 0; i < m_intPartLength; i++)
            {
                m_dRange[i] = new double[m_intOperatorLength];
            }

            for (i = 0; i < m_intOperatorLength; i++)
            {
                m_dSumTrialColumn[i] = new double[m_intTrialLength];
                for (j = 0; j < m_intTrialLength; j++)
                {
                    m_dSumTrialColumn[i][j] = 0;
                }

                m_dSumXOpe[i] = 0;
                m_dAveXOpe[i] = 0;
                m_dAveROpe[i] = 0;
                m_dSumRangeColumn[i] = 0;
            }
            #endregion

            #region Get range, X-Bar, sum trial each column, sum range each column
            // Scan part
            for (i = 0; i < m_fData[intItemIndex][intGroupIndex].Length; i++)
            {
                dSumTrialRow = 0;

                // Scan operator
                for (j = 0; j < m_fData[intItemIndex][intGroupIndex][i].Length; j++)
                {
                    dMinValue = 0;
                    dMaxValue = 0;

                    // Scan trial
                    for (k = 0; k < m_fData[intItemIndex][intGroupIndex][i][j].Length; k++)
                    {
                        dValue = Math.Abs(m_fData[intItemIndex][intGroupIndex][i][j][k]);

                        // Get min max trial each Part->Operator
                        if ((k == 0) || (dValue < dMinValue))
                            dMinValue = dValue;
                        if ((k == 0) || (dValue > dMaxValue))
                            dMaxValue = dValue;

                        // Get sum trial each Part
                        dSumTrialRow += dValue;

                        // *Get sum trial each column
                        m_dSumTrialColumn[j][k] += dValue;

                        // Get sum trial
                        dSumX += dValue;

                        // Get double sum trial
                        dSumXX += (dValue * dValue);
                    }

                    // *Get range each Part->Operator
                    m_dRange[i][j] = dMaxValue - dMinValue;

                    // *Get sum range each column
                    m_dSumRangeColumn[j] += m_dRange[i][j];
                }

                // *Get X-Bar each Part
                m_dXBar[i] = dSumTrialRow / (m_intOperatorLength * m_intTrialLength);

                // Get min max X-Bar column
                if ((i == 0) || (m_dXBar[i] < dMinXBar))
                    dMinXBar = m_dXBar[i];
                if ((i == 0) || (m_dXBar[i] > dMaxXBar))
                    dMaxXBar = m_dXBar[i];
            }
            #endregion

            #region Get sum trial each operator, average trial, average X-Bar
            for (i = 0; i < m_intOperatorLength; i++)
            {
                for (j = 0; j < m_intTrialLength; j++)
                {
                    m_dSumXOpe[i] += m_dSumTrialColumn[i][j];
                }
                m_dAveXOpe[i] = m_dSumXOpe[i] / (m_intPartLength * m_intTrialLength);
                m_dAveROpe[i] = m_dSumRangeColumn[i] / m_intPartLength;

            }
            #endregion

            double dAveXAll = 0;
            for (i = 0; i < m_intOperatorLength; i++)
            {
                dAveXAll += m_dAveXOpe[i];
            }
            dAveXAll = dAveXAll / m_intOperatorLength;


            #region Get R-Part, UCL-Range, Range of Average, Average of R-Bar

            // Get R-Part
            m_dRPart = dMaxXBar - dMinXBar;

            // Get Average of R-Bar
            m_dAverageRBar = 0;
            for (i = 0; i < m_intOperatorLength; i++)
            {
                m_dAverageRBar += m_dSumRangeColumn[i];
            }
            m_dAverageRBar = m_dAverageRBar / (m_intPartLength * m_intOperatorLength);

            //22-01-2019 ZJYEOH : Some fixed values inside the formula is now follow the K value according to AIAG MSA manual revision fourth (Chapter 3 section B Page 120)

            // Get UCL-Range
            // 2020-12-11 ZJYEOH : constant D4 from table R Chart
            if (m_intTrialLength == 2)
                m_dUCLRange = m_dAverageRBar * (3.267); 
            else if (m_intTrialLength == 3)
                //m_dUCLRange = 0.1;
                m_dUCLRange = m_dAverageRBar * (2.574);

            // Get Range of Average
            for (i = 0; i < m_intOperatorLength; i++)
            {
                dValue = m_dSumXOpe[i] / (m_intPartLength * m_intTrialLength);
                if ((i == 0) || (dValue < dMinAve))
                    dMinAve = dValue;

                if ((i == 0) || (dValue > dMaxAve))
                    dMaxAve = dValue;
            }
            m_dRangeAverage = dMaxAve - dMinAve;
            #endregion

            #region Get Repeatability, Reproducibility, R&R, Part Variation

            // Repeatability
            if (m_intTrialLength == 2)
                m_dEV = m_dAverageRBar * 0.8862; //(4.56)
            else if (m_intTrialLength == 3)
                m_dEV = m_dAverageRBar * 0.5908; //(3.05)
            else if (m_intTrialLength == 4)
                m_dEV = m_dAverageRBar * 0.4857; //(2.5)

            // Reproducibility
            if (m_intOperatorLength == 2)
                dValue = Math.Pow(m_dRangeAverage * 0.7071, 2) - (Math.Pow(m_dEV, 2) / (m_intPartLength * m_intTrialLength)); //(3.65)
            else if (m_intOperatorLength == 3)
                dValue = Math.Pow(m_dRangeAverage * 0.5231, 2) - (Math.Pow(m_dEV, 2) / (m_intPartLength * m_intTrialLength)); //(2.7)
            else if (m_intOperatorLength == 4)
                dValue = Math.Pow(m_dRangeAverage * 0.4467, 2) - (Math.Pow(m_dEV, 2) / (m_intPartLength * m_intTrialLength)); //(2.5)

            if (dValue > 0)
                m_dAV = Math.Sqrt(dValue);
            else
                m_dAV = 0;

            // R&R
            m_dRR = Math.Sqrt(Math.Pow(m_dEV, 2) + Math.Pow(m_dAV, 2));

            // Part Variation
            if (m_intPartLength == 2)
                m_dPV = m_dRPart * (1 / 1.41421); //(3.65);
            else if (m_intPartLength == 3)
                m_dPV = m_dRPart * (1 / 1.91155); //(2.7);
            else if (m_intPartLength == 4)
                m_dPV = m_dRPart * (1 / 2.23887); //(2.5);
            else if (m_intPartLength == 5)
                m_dPV = m_dRPart * (1 / 2.48124); //(2.08);
            else if (m_intPartLength == 6)
                m_dPV = m_dRPart * (1 / 2.67253); //(1.93);
            else if (m_intPartLength == 7)
                m_dPV = m_dRPart * (1 / 2.82981); //(1.82);
            else if (m_intPartLength == 8)
                m_dPV = m_dRPart * (1 / 2.96288); //(1.74);
            else if (m_intPartLength == 9)
                m_dPV = m_dRPart * (1 / 3.07794); //(1.67);
            else if (m_intPartLength == 10)
                m_dPV = m_dRPart * (1 / 3.17905); //(1.62);
            else if (m_intPartLength == 11)
                m_dPV = m_dRPart * (1 / 3.26909);
            else if (m_intPartLength == 12)
                m_dPV = m_dRPart * (1 / 3.35016);
            else if (m_intPartLength == 13)
                m_dPV = m_dRPart * (1 / 3.42378);
            else if (m_intPartLength == 14)
                m_dPV = m_dRPart * (1 / 3.49116);
            else if (m_intPartLength == 15)
                m_dPV = m_dRPart * (1 / 3.55333);
            else if (m_intPartLength == 16)
                m_dPV = m_dRPart * (1 / 3.61071);
            else if (m_intPartLength == 17)
                m_dPV = m_dRPart * (1 / 3.66422);
            else if (m_intPartLength == 18)
                m_dPV = m_dRPart * (1 / 3.71424);
            else if (m_intPartLength == 19)
                m_dPV = m_dRPart * (1 / 3.76118);
            else if (m_intPartLength >= 20)
                m_dPV = m_dRPart * (1 / 3.80537);
            #endregion

            if (m_dRR > 0)
            {

            }
            #region Get LSL-USL. Total Variation, PT Ratio
            // Total Variation (TV)
            // Calculate PT Ratio only if specification is defined
            if (m_fLSL[intItemIndex][intGroupIndex] == m_fUSL[intItemIndex][intGroupIndex])
            {
                // Use study variation if specification tolerance no defined
                m_dTolerance = -999;
                m_dTV = Math.Sqrt(Math.Pow(m_dRR, 2) + Math.Pow(m_dPV, 2));
                m_dPTRatio = -999;
            }
            else
            {
                // Specification tolerance
                m_dTolerance = m_fUSL[intItemIndex][intGroupIndex] - m_fLSL[intItemIndex][intGroupIndex];

                // Use specification tolerance if defined
                //m_dTV = m_dTolerance;
                m_dTV = Math.Sqrt(Math.Pow(m_dRR, 2) + Math.Pow(m_dPV, 2));

                // PT Ratio formula used here is reversed compare to standard PT Ratio
                m_dPTRatio = (m_fUSL[intItemIndex][intGroupIndex] - m_fLSL[intItemIndex][intGroupIndex]) / m_dRR; 
            }

            #endregion

            #region Get Percent of Repeatability, Reproducibility, R&R and Part Variation
            // Percent of Repeatability
            //2020-12-15 ZJYEOH : Formula of percentage tolerance = measurement * 6 / (USL - LSL) * 100, where constant value 6 means cover 99.73% of process measurements, can change to 5.15 which 99% of process measurements if our GRR not good, only can choose between 6 or 5.15
            //if (m_dTV == 0)
            if (m_fLSL[intItemIndex][intGroupIndex] == m_fUSL[intItemIndex][intGroupIndex])
                m_dPercentEV = 0;
            else
                //m_dPercentEV = m_dEV / m_dTV * 100;
                //m_dPercentEV = m_dEV / m_dTV / m_dRPart;
                m_dPercentEV = m_dEV * 6 / m_dTolerance * 100;

            // Percent of Reproducibility
            //if (m_dTV == 0)
            if (m_fLSL[intItemIndex][intGroupIndex] == m_fUSL[intItemIndex][intGroupIndex])
                m_dPercentAV = 0;
            else
                //m_dPercentAV = m_dAV / m_dTV * 100;
                //m_dPercentAV = m_dAV / m_dTV / m_dRPart;
                m_dPercentAV = m_dAV * 6 / m_dTolerance * 100;

            // Percent of R&R
            //if (m_dTV == 0)
            if (m_fLSL[intItemIndex][intGroupIndex] == m_fUSL[intItemIndex][intGroupIndex])
                m_dPercentRR = 0;
            else
                //m_dPercentRR = m_dRR / m_dTV * 100;
                //m_dPercentRR = m_dRR / m_dTV * 100 / (dAveXAll * 10);
                m_dPercentRR = m_dRR * 6 / m_dTolerance * 100;

            // Percent of Part Variation
            //if (m_dTV == 0)
            if (m_fLSL[intItemIndex][intGroupIndex] == m_fUSL[intItemIndex][intGroupIndex])
                m_dPercentPV = 0;
            else
                //m_dPercentPV = m_dPV / m_dTV * 100;
                //m_dPercentPV = m_dPV / m_dTV / m_dRPart;
                m_dPercentPV = m_dPV * 6 / m_dTolerance * 100;

            m_dNDC = 1.41 * (m_dPV / m_dRR);
            #endregion
        }
        /// <summary>
        /// Check is save GRR report directory exist?
        /// Create the directory if no exist
        /// </summary>
        private void CheckFolderDirectory()
        {
            // Check GRRReport folder
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "GRRReport"))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "GRRReport");
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "GRRReport\\" + m_strLotID))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "GRRReport\\" + m_strLotID);
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "GRRReport\\" + m_strLotID + "\\" + m_strVisionID))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "GRRReport\\" + m_strLotID + "\\" + m_strVisionID);
        }

    }
}
