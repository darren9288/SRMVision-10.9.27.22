using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Common;
using System.IO;

namespace VisionProcessing
{
    public class CPK
    {
        #region Member Variables
        private double m_dSumX = 0;
        private double m_dSumXX = 0;

        private double m_dCPKResult = 0;
        private double m_dCPResult = 0;
        private bool m_blnIsCalculated = false;

        private bool m_blnFirstTimeSet = true;
        private float m_fMin = 0;
        private float m_fMax = 0;
        private double m_dMean = 0;       //Average
        private double m_dSigma = 0;
        private int m_intFrequency = 0;

        private double m_dUSL = 0;        // Upper Limit
        private double m_dLSL = 0;        // Lower Limit
        private double m_dUSLResult = 0;
        private double m_dLSLResult = 0;

        //-----------------------New variable--------------------------
        //D1: Item, D2: Group
        private double[][] m_fLSL;
        private double[][] m_fUSL;
        private float[][] m_fMinValue;
        private float[][] m_fMaxValue;
        private double[][] m_fMean;
        private double[][] m_fSigma;
        private double[][] m_fCPResult;
        private double[][] m_fCPKResult;
        private double[][] m_fSumX;
        private double[][] m_fSumXX;

        private string m_strLotID;
        private string m_strOpeID;
        private string m_strRecipeID;
        private string m_strMachineID;
        private string m_strVisionID;
        private string[] m_strItems;
        private string[] m_strGroupName;
        private int m_intGroupLength = 0; //Total number of Pad
        private int m_intPartLength = 0; //Total number of Unit need to record
        private int m_intTestedTotal = 0; //current Unit count
        private int m_intPassedTotal = 0; //current Unit count
        private int[] m_intFailCriteria;
        private bool m_blnFail = false;

        private DateTime m_startDateTime = new DateTime();
        //-----------------------New variable--------------------------

        #endregion

        #region Properties
        public double ref_dSumX { set { m_dSumX = value; } get { return m_dSumX; } }
        public double ref_dSumXX { set { m_dSumXX = value; } get { return m_dSumXX; } }
        public int ref_intFrequency { set { m_intFrequency = value; } get { return m_intFrequency; } }

        public double ref_dCPKResult { get { return m_dCPKResult; } }
        public double ref_dCPResult { get { return m_dCPResult; } }
        public bool ref_blnIsCalculated { get { return m_blnIsCalculated; } }

        public double ref_dMean { get { return m_dMean; } }
        public double ref_dSigma { get { return m_dSigma; } }

        public float ref_fMin { set { m_fMin = value; } get { return m_fMin; } }
        public float ref_fMax { set { m_fMax = value; } get { return m_fMax; } }
        public double ref_dUSL { set { m_dUSL = value; } get { return m_dUSL; } }
        public double ref_dLSL { set { m_dLSL = value; } get { return m_dLSL; } }
        public int ref_intTestedTotal { get { return m_intTestedTotal; } }
        #endregion

        public CPK(string[] strItems)
        {
            m_strItems = strItems;
        }

        /// <summary>
        /// Init parameter of CPK 
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
                         int intGroupLength, int intPartLength, bool init)
        {
            m_strLotID = strLotID;
            m_strOpeID = strOpeID;
            m_strRecipeID = strRecipeID;
            m_strMachineID = strMachineID;
            m_strVisionID = strVisionID;
            m_intGroupLength = intGroupLength;
            m_intPartLength = intPartLength;

            InitializeData();

            if (init)
            {
                //Load from temp file if init
                LoadTempPadData();
            }
            else
            {
                //Delete temp file
                string strSelectedFile = AppDomain.CurrentDomain.BaseDirectory + "CPKReport\\" + m_strLotID + "\\" + m_strVisionID + "\\Temp\\Temp.xml";
                if (File.Exists(strSelectedFile))
                {
                    File.Delete(strSelectedFile);
                }
            }
        }

        /// <summary>
        /// Set CPK test counter to next index
        /// </summary>
        public void NextIndex()
        {
            if (m_blnFail)
                m_blnFail = false;
            else
                m_intPassedTotal++;

            m_intTestedTotal++;

            //Save Temp Data
            SaveTempPadData();

            if (m_intTestedTotal >= m_intPartLength)
            {
                CalculateCPK(true);
                SaveReport();
            }
        }

        // Save if user new lot or re-learn pad before complete the CPK
        public void CreateIncompleteCPKReport()
        {
            if (m_intTestedTotal > 0 && m_intPartLength > m_intTestedTotal)
            {
                CalculateCPK(false);
                SaveReport();
            }
        }

        public bool Record(int intItemIndex, int intGroupIndex, string strGroupName, float fValue)
        {
            if (m_intGroupLength > intGroupIndex)
            {
                if (m_strItems.Length > intItemIndex)
                {
                    //if (m_fData[intGroupIndex][intItemIndex].Length == 0)
                    //{
                    //    SRMMessageBox.Show("Part should not set to 0.", "CPK", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                    //    return false;
                    //}

                    if (m_strGroupName[intGroupIndex] != strGroupName)
                        m_strGroupName[intGroupIndex] = strGroupName;

                    if (m_intPartLength > m_intTestedTotal)
                    {
                        if (fValue != -999)
                        {
                            m_fSumX[intGroupIndex][intItemIndex] += fValue;
                            m_fSumXX[intGroupIndex][intItemIndex] += fValue * fValue;

                            if (m_fMinValue[intGroupIndex][intItemIndex] > fValue || m_fMinValue[intGroupIndex][intItemIndex] == -1)
                                m_fMinValue[intGroupIndex][intItemIndex] = fValue;

                            if (m_fMaxValue[intGroupIndex][intItemIndex] < fValue || m_fMaxValue[intGroupIndex][intItemIndex] == -1)
                                m_fMaxValue[intGroupIndex][intItemIndex] = fValue;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public void RecordPassFail(long intFailMask)
        {
            if ((intFailMask & 0x01) > 0)
            {
                m_blnFail = true;
                m_intFailCriteria[0]++;
            }
            else if ((intFailMask & 0x02) > 0)
            {
                m_blnFail = true;
                m_intFailCriteria[1]++;
            }
            else if ((intFailMask & 0x04) > 0)
            {
                m_blnFail = true;
                m_intFailCriteria[2]++;
            }
            else if ((intFailMask & 0x08) > 0)
            {
                m_blnFail = true;
                m_intFailCriteria[3]++;
            }
            else if ((intFailMask & 0x10) > 0)
            {
                m_blnFail = true;
                m_intFailCriteria[4]++;
            }
            else if ((intFailMask & 0x20) > 0)
            {
                m_blnFail = true;
                m_intFailCriteria[5]++;
            }
            else if ((intFailMask & 0x40) > 0)
            {
                m_blnFail = true;
                m_intFailCriteria[6]++;
            }
            else if ((intFailMask & 0x80) > 0)
            {
                m_blnFail = true;
                m_intFailCriteria[7]++;
            }
            else if ((intFailMask & 0x100) > 0)
            {
                m_blnFail = true;
                m_intFailCriteria[8]++;
            }
            else if ((intFailMask & 0x200) > 0)
            {
                m_blnFail = true;
                m_intFailCriteria[9]++;
            }
        }

        /// <summary>
        /// Save CPK report to file
        /// File is saved at "Current Directory...\CPKReport\LotID\VisionID\CPK_VisionID_StartDateTime.xml"
        /// </summary>
        public void SaveReport()
        {
            CheckFolderDirectory();
            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "CPKReport\\" +
                m_strLotID + "\\" + m_strVisionID + "\\CPK_" + m_strVisionID + "_" + m_startDateTime.ToString("yyyyMMddHHmmss") + ".xml");

            objFile.WriteSectionElement("Lot", false);
            objFile.WriteElement1Value("LotID", m_strLotID, "Lot ID", true);
            objFile.WriteElement1Value("OperatorID", m_strOpeID, "Operator ID", true);
            objFile.WriteElement1Value("RecipeID", m_strRecipeID, "Recipe ID", true);
            objFile.WriteElement1Value("MachineID", m_strMachineID, "Machine ID", true);
            objFile.WriteElement1Value("VisionID", m_strVisionID, "Vision Station Name", true);
            objFile.WriteElement1Value("CPKStartTime", m_startDateTime.ToString("yyyy/MM/dd HH:mm:ss"), "CPK Creation Time", true);

            //Save Feature
            objFile.WriteSectionElement("VisionFeature", true);
            objFile.WriteElement1Value("Total", m_intTestedTotal);
            objFile.WriteElement1Value("Pass", m_intPassedTotal);

            objFile.WriteElement1Value("Fail", "");
            objFile.WriteElement2Value("Offset", m_intFailCriteria[0]);
            objFile.WriteElement2Value("Area", m_intFailCriteria[1]);
            objFile.WriteElement2Value("Width", m_intFailCriteria[2]);
            objFile.WriteElement2Value("Length", m_intFailCriteria[3]);
            objFile.WriteElement2Value("Pitch", m_intFailCriteria[4]);
            objFile.WriteElement2Value("Gap", m_intFailCriteria[5]);
            objFile.WriteElement2Value("BrokenArea", m_intFailCriteria[6]);
            objFile.WriteElement2Value("BrokenLength", m_intFailCriteria[7]);
            objFile.WriteElement2Value("Excess", m_intFailCriteria[8]);
            objFile.WriteElement2Value("Smear", m_intFailCriteria[9]);

            //Save Format
            objFile.WriteSectionElement("Format", false);
            objFile.WriteElement1Value("Group", m_intGroupLength, "Group Counter", true);
            objFile.WriteElement1Value("Part", m_intPartLength, "Part Counter", true);

            // Save value
            int x, y;
            objFile.WriteSectionElement("Group", true);
            for (x = 0; x < m_intGroupLength; x++)
            {
                objFile.WriteElement1Value("Group" + x, "", "Group No", true);
                objFile.WriteElement2Value("GroupName", m_strGroupName[x], "Group Name", true);
                for (y = 0; y < m_strItems.Length; y++)
                {
                    objFile.WriteElement2Value("Items" + y, "", "Items No", true);
                    objFile.WriteElement3Value("Name", m_strItems[y], "Items Name", true);

                    if (m_fLSL[x][y] == -1)
                        objFile.WriteElement3Value("LSL", "NA", "Lower Specification Limit", true);
                    else
                        objFile.WriteElement3Value("LSL", m_fLSL[x][y].ToString("f3"), "Lower Specification Limit", true);

                    if (m_fUSL[x][y] == -1)
                        objFile.WriteElement3Value("USL", "NA", "Upper Specification Limit", true);
                    else
                        objFile.WriteElement3Value("USL", m_fUSL[x][y].ToString("f3"), "Upper Specification Limit", true);

                    if (m_fMinValue[x][y] == -1)
                        objFile.WriteElement3Value("MinValue", "NA", "Minimum Value", true);
                    else
                        objFile.WriteElement3Value("MinValue", m_fMinValue[x][y].ToString("f3"), "Minimum Value", true);

                    if (m_fMaxValue[x][y] == -1)
                        objFile.WriteElement3Value("MaxValue", "NA", "Maximum Value", true);
                    else
                        objFile.WriteElement3Value("MaxValue", m_fMaxValue[x][y].ToString("f3"), "Maximum Value", true);

                    if (m_fMean[x][y] == -1)
                        objFile.WriteElement3Value("Mean", "NA", "Average Value", true);
                    else
                        objFile.WriteElement3Value("Mean", m_fMean[x][y].ToString("f3"), "Average Value", true);

                    if (m_fSigma[x][y] == -1)
                        objFile.WriteElement3Value("Sigma", "NA", "Sigma Value", true);
                    else
                        objFile.WriteElement3Value("Sigma", m_fSigma[x][y].ToString("f5"), "Sigma Value", true);

                    if (m_fCPResult[x][y] == -1)
                        objFile.WriteElement3Value("CPResult", "NA", "CP Result", true);
                    else
                        objFile.WriteElement3Value("CPResult", m_fCPResult[x][y].ToString("f3"), "CP Result", true);

                    if (m_fCPKResult[x][y] == -1)
                        objFile.WriteElement3Value("CPKResult", "NA", "CPK Result", true);
                    else
                        objFile.WriteElement3Value("CPKResult", m_fCPKResult[x][y].ToString("f3"), "CPK Result", true);
                }
            }


            objFile.WriteEndElement();
            //STDeviceEdit.XMLChangesTracing(  "Pad-CPK", AppDomain.CurrentDomain.BaseDirectory + "CPKReport\\" +
            //    m_strLotID + "\\" + m_strVisionID + "\\","CPK_" + m_strVisionID + "_" + m_startDateTime.ToString("yyyyMMddHHmmss") + ".xml");
        }


        //Save Temp Pad data to prevent data lost if machine power off
        public void SaveTempPadData()
        {
            CheckFolderDirectory();
            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "CPKReport\\" +
                m_strLotID + "\\" + m_strVisionID + "\\Temp\\Temp.xml");


            objFile.WriteSectionElement("TestedDetail", true);
            objFile.WriteElement1Value("TestedCount", m_intTestedTotal, "Total Pad Tested", true);
            objFile.WriteElement1Value("PassedCount", m_intPassedTotal, "Total Pad Passed", true);
            objFile.WriteElement1Value("CPKStartTime", m_startDateTime.ToString("yyyy/MM/dd HH:mm:ss"), "CPK Creation Time", true);

            objFile.WriteSectionElement("Fail", true);
            objFile.WriteElement1Value("Offset", m_intFailCriteria[0]);
            objFile.WriteElement1Value("Area", m_intFailCriteria[1]);
            objFile.WriteElement1Value("Width", m_intFailCriteria[2]);
            objFile.WriteElement1Value("Length", m_intFailCriteria[3]);
            objFile.WriteElement1Value("Pitch", m_intFailCriteria[4]);
            objFile.WriteElement1Value("Gap", m_intFailCriteria[5]);
            objFile.WriteElement1Value("BrokenArea", m_intFailCriteria[6]);
            objFile.WriteElement1Value("BrokenLength", m_intFailCriteria[7]);
            objFile.WriteElement1Value("Excess", m_intFailCriteria[8]);
            objFile.WriteElement1Value("Smear", m_intFailCriteria[9]);

            // Save value
            int x, y;
            objFile.WriteSectionElement("Group", true);
            for (x = 0; x < m_intGroupLength; x++)
            {
                objFile.WriteElement1Value("Group" + x, "", "Group No", true);
                objFile.WriteElement2Value("GroupName", m_strGroupName[x], "Group Name", true);
                for (y = 0; y < m_strItems.Length; y++)
                {
                    objFile.WriteElement2Value("Items" + y, "", "Items No", true);
                    objFile.WriteElement3Value("Name", m_strItems[y], "Items Name", true);
                    objFile.WriteElement3Value("SumX", m_fSumX[x][y].ToString("f5"), "Temp Sum X", true);
                    objFile.WriteElement3Value("SumXX", m_fSumXX[x][y].ToString("f5"), "Temp Sum XX", true);
                    objFile.WriteElement3Value("MinValue", m_fMinValue[x][y].ToString("f3"), "Temp Minimum Value", true);
                    objFile.WriteElement3Value("MaxValue", m_fMaxValue[x][y].ToString("f3"), "Temp Maximum Value", true);
                }
            }
            objFile.WriteEndElement();
        }

        public void LoadTempPadData()
        {
            string strSelectedFile = AppDomain.CurrentDomain.BaseDirectory + "CPKReport\\" + m_strLotID + "\\" + m_strVisionID + "\\Temp\\Temp.xml";

            if (File.Exists(strSelectedFile))
            {
                XmlParser objFile = new XmlParser(strSelectedFile);
                objFile.GetFirstSection("TestedDetail");
                m_intTestedTotal = objFile.GetValueAsInt("TestedCount", 0);
                m_intPassedTotal = objFile.GetValueAsInt("PassedCount", 0);
                m_startDateTime = DateTime.ParseExact(objFile.GetValueAsString("CPKStartTime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                objFile.GetFirstSection("Fail");
                m_intFailCriteria[0] = objFile.GetValueAsInt("Offset", 0);
                m_intFailCriteria[1] = objFile.GetValueAsInt("Area", 0);
                m_intFailCriteria[2] = objFile.GetValueAsInt("Width", 0);
                m_intFailCriteria[3] = objFile.GetValueAsInt("Length", 0);
                m_intFailCriteria[4] = objFile.GetValueAsInt("Pitch", 0);
                m_intFailCriteria[5] = objFile.GetValueAsInt("Gap", 0);
                m_intFailCriteria[6] = objFile.GetValueAsInt("BrokenArea", 0);
                m_intFailCriteria[7] = objFile.GetValueAsInt("BrokenLength", 0);
                m_intFailCriteria[8] = objFile.GetValueAsInt("Excess", 0);
                m_intFailCriteria[9] = objFile.GetValueAsInt("Smear", 0);

                objFile.GetFirstSection("Group");
                for (int x = 0; x < m_intGroupLength; x++)
                {
                    objFile.GetSecondSection("Group" + x);
                    m_strGroupName[x] = objFile.GetValueAsString("GroupName", "No Found", 2);

                    for (int y = 0; y < m_strItems.Length; y++)
                    {
                        objFile.GetThirdSection("Items" + y);
                        m_fSumX[x][y] = Convert.ToDouble(objFile.GetValueAsString("SumX", "0", 3));
                        m_fSumXX[x][y] = Convert.ToDouble(objFile.GetValueAsString("SumXX", "0", 3));
                        m_fMinValue[x][y] = Convert.ToSingle(objFile.GetValueAsString("MinValue", "0", 3));
                        m_fMaxValue[x][y] = Convert.ToSingle(objFile.GetValueAsString("MaxValue", "0", 3));
                    }
                }
            }
        }


        //public void AppendObj(float fValue)
        //{
        //    if (m_blnFirstTimeSet)
        //    {
        //        m_fMin = fValue;
        //        m_fMax = fValue;
        //        m_blnFirstTimeSet = false;
        //    }
        //    else
        //    {
        //        if (fValue < m_fMin)
        //            m_fMin = fValue;
        //        if (fValue > m_fMax)
        //            m_fMax = fValue;
        //    }

        //    m_intFrequency++;

        //    m_dSumX += (double)fValue;
        //    m_dSumXX += ((double)fValue * (double)fValue);

        //    CalculateCPK();
        //    CalculateCP();
        //}     

        public void InitializeData()
        {
            m_strGroupName = new string[m_intGroupLength];
            m_fLSL = new double[m_intGroupLength][];
            m_fUSL = new double[m_intGroupLength][];
            m_fMinValue = new float[m_intGroupLength][];
            m_fMaxValue = new float[m_intGroupLength][];
            m_fMean = new double[m_intGroupLength][];
            m_fSigma = new double[m_intGroupLength][];
            m_fCPResult = new double[m_intGroupLength][];
            m_fCPKResult = new double[m_intGroupLength][];
            m_fSumX = new double[m_intGroupLength][];
            m_fSumXX = new double[m_intGroupLength][];
            m_intTestedTotal = 0;
            m_intPassedTotal = 0;
            m_intFailCriteria = new int[10];
            m_startDateTime = DateTime.Now;

            for (int x = 0; x < m_intGroupLength; x++)
            {
                m_fLSL[x] = new double[m_strItems.Length];
                m_fUSL[x] = new double[m_strItems.Length];

                m_fMinValue[x] = new float[m_strItems.Length];
                m_fMaxValue[x] = new float[m_strItems.Length];
                for (int y = 0; y < m_strItems.Length; y++)
                {
                    m_fMinValue[x][y] = -1;
                    m_fMaxValue[x][y] = -1;
                }

                m_fMean[x] = new double[m_strItems.Length];
                m_fSigma[x] = new double[m_strItems.Length];
                m_fCPResult[x] = new double[m_strItems.Length];
                m_fCPKResult[x] = new double[m_strItems.Length];
                m_fSumX[x] = new double[m_strItems.Length];
                m_fSumXX[x] = new double[m_strItems.Length];
            }
        }

        //public static PointF Origin(PointF[,] points, float pitchX, float pitchY, double windowX, double windowY)
        //{
        //    float sumX = 0; 
        //    float sumY = 0;
        //    int count = 0;

        //    for (int i = 0; i < points.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < points.GetLength(1); j++)
        //        {
        //            if (!points[i, j].IsEmpty)
        //            {
        //                sumX += points[i, j].X - i * pitchX;
        //                sumY += points[i, j].Y - j * pitchY;
        //                count++;
        //            }
        //        }
        //    }

        //    if (count <= 0)
        //        return PointF.Empty;

        //    float X = sumX / (float)count;
        //    float Y = sumY / (float)count;

        //    float[] errX = new float[points.GetLength(0)* points.GetLength(1)];
        //    float[] errY = new float[points.GetLength(0)* points.GetLength(1)];
        //    double[] errR = new double[points.GetLength(0) * points.GetLength(1)];
        //    double errR_max = double.MinValue;

        //    bool[,] cast = new bool[points.GetLength(0), points.GetLength(1)];
        //    int n = 0, n_errR_max = -1;

        //    for (int i = 0; i < points.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < points.GetLength(1); j++)
        //        {
        //            if (!points[i, j].IsEmpty)
        //            {
        //                n = i * points.GetLength(0) + j;
        //                errX[n] = points[i, j].X - (X + i * pitchX);
        //                errY[n] = points[i, j].Y - (Y + j * pitchY);
        //                errR[n] = Math.Sqrt(errX[n] * errX[n] + errY[n] * errY[n]);
        //                cast[i, j] = (errX[i * points.GetLength(0) + j] < windowX && errY[i * points.GetLength(0) + j] < windowY);
        //                if (errR[n] > errR_max)
        //                {
        //                    errR_max = errR[n];
        //                    n_errR_max = n;
        //                }
        //            }
        //            else
        //                cast[i, j] = false;
        //        }
        //    }

        //    cast[n_errR_max % points.GetLength(0), n_errR_max / points.GetLength(0)] = false;
        //    sumX = sumY = count = 0;

        //    for (int i = 0; i < points.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < points.GetLength(1); j++)
        //        {
        //            if (cast[i, j])
        //            {
        //                sumX += points[i, j].X - i * pitchX;
        //                sumY += points[i, j].Y - j * pitchY;
        //                count++;
        //            }
        //        }
        //    }

        //    if (count <= 0)
        //        return PointF.Empty;

        //    X = sumX / (float)count;
        //    Y = sumY / (float)count;

        //    return new PointF(X, Y);
        //}



        private void CalculateCPK(bool blnCompleteData)
        {
            double fUSLResult;
            double fLSLResult;

            int intUnitCount;
            if (blnCompleteData)
                intUnitCount = m_intPartLength;
            else
                intUnitCount = m_intTestedTotal;


            for (int x = 0; x < m_intGroupLength; x++)
            {
                for (int y = 0; y < m_strItems.Length; y++)
                {
                    if (m_fMinValue[x][y] != -1)
                    {
                        //Average
                        m_fMean[x][y] = m_fSumX[x][y] / intUnitCount;
                        //if ((m_fMean[x][y] <= m_fUSL[x][y]) && (m_fMean[x][y] >= m_fLSL[x][y]))
                        //{
                        double dValue = Math.Round((m_fSumXX[x][y] / (double)intUnitCount), 6, MidpointRounding.AwayFromZero);
                        double dValue1 = Math.Round(Math.Pow(m_fMean[x][y], 2), 6, MidpointRounding.AwayFromZero);
                        double dValue2 = dValue - dValue1;
                        if (dValue2 > 0)
                            m_fSigma[x][y] = Math.Sqrt(dValue2);
                        else
                            m_fSigma[x][y] = 0;

                        if (m_fSigma[x][y] != 0)
                        {
                            fUSLResult = (m_fUSL[x][y] - m_fMean[x][y]) / (3 * m_fSigma[x][y]);
                            fLSLResult = (m_fMean[x][y] - m_fLSL[x][y]) / (3 * m_fSigma[x][y]);
                            m_fCPKResult[x][y] = Math.Min(fUSLResult, fLSLResult);

                            m_fCPResult[x][y] = ((m_fUSL[x][y] - m_fLSL[x][y]) / (6 * m_fSigma[x][y]));
                        }
                        else
                        {
                            m_fCPResult[x][y] = -1;
                            m_fCPKResult[x][y] = -1;
                        }
                        //}
                    }
                    else
                    {
                        m_fLSL[x][y] = -1;
                        m_fUSL[x][y] = -1;
                        m_fMean[x][y] = -1;
                        m_fSigma[x][y] = -1;
                        m_fCPResult[x][y] = -1;
                        m_fCPKResult[x][y] = -1;
                    }
                }
            }
        }

        private void CalculateCP()
        {
            for (int x = 0; x < m_intGroupLength; x++)
            {
                for (int y = 0; y < m_strItems.Length; y++)
                {
                    if (m_fSigma[x][y] != 0)
                    {
                        m_fCPResult[x][y] = ((m_fUSL[x][y] - m_fLSL[x][y]) / (6 * m_fSigma[x][y]));
                    }
                    else
                    {
                        m_fCPResult[x][y] = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Set CPK specification 
        /// </summary>
        /// <param name="intItemIndex">Item index. E.g Index 0 = position-x, index 1 = position-y</param>
        /// <param name="intGroupIndex">Group index. E.g Index 0 = current unit, index 2 = next unit</param>
        /// <param name="fLSL">LSL value</param>
        /// <param name="fUSL">USL value</param>
        public void SetSpecification(int intItemIndex, int intGroupIndex, float fLSL, float fUSL)
        {
            if ((intGroupIndex >= m_fLSL.Length) || (intGroupIndex >= m_fUSL.Length))
                return;

            if ((intItemIndex >= m_fLSL[intGroupIndex].Length) || (intItemIndex >= m_fUSL[intGroupIndex].Length))
                return;

            m_fLSL[intGroupIndex][intItemIndex] = fLSL;
            m_fUSL[intGroupIndex][intItemIndex] = fUSL;
        }

        /// <summary>
        /// Check is save CPK report directory exist?
        /// Create the directory if no exist
        /// </summary>
        private void CheckFolderDirectory()
        {
            // Check CPKReport folder
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "CPKReport"))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "CPKReport");
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "CPKReport\\" + m_strLotID))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "CPKReport\\" + m_strLotID);
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "CPKReport\\" + m_strLotID + "\\" + m_strVisionID))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "CPKReport\\" + m_strLotID + "\\" + m_strVisionID);
        }
    }
}
