using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedMemory;
using Common;
using System.IO;
using System.Collections;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.CSharp;

namespace VisionProcessForm
{
    public partial class ResultLogForm : Form
    {
        private int m_intUserGroup = 5;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private string m_strSelectedRecipe;
        private FileSorting m_objTimeComparer = new FileSorting();
        private CustomOption m_smCustomizeInfo;
        
        public ResultLogForm(ProductionInfo smProductionInfo, VisionInfo smVisionInfo, CustomOption smCustomizeInfo, string strSelectedRecipe)
        {
            InitializeComponent();

            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            
            m_smCustomizeInfo = smCustomizeInfo;
            m_intUserGroup = m_smProductionInfo.g_intUserGroup;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                      m_smVisionInfo.g_strVisionFolderName + "\\General.xml";
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection("ResultLog");
            txt_TotalResultCount.Text = objFile.GetValueAsInt("ResultLogMaxCount", 1000).ToString();
            chk_WantRecordResult.Checked = objFile.GetValueAsBoolean("WantRecordResult", false);

            FillLotComboBox();
            FillFileComboBox();
            DisableField2();
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild2 = "Result Log Page";
            string strChild3 = "Save Button";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_Save.Enabled = false;
            }
        }
        private int GetUserRightGroup_Child3(string Child2, string Child3)
        {
            //NewUserRight objUserRight = new NewUserRight(false);
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Orient":
                case "BottomOrient":
                    return m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(Child2, Child3);
                    break;
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                case "Package":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetMarkOrientChild3Group(Child2, Child3);
                    break;
                case "IPMLi":
                case "IPMLiPkg":
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                    return m_smCustomizeInfo.objNewUserRight.GetInPocketChild3Group(Child2, Child3, m_smVisionInfo.g_intVisionNameNo);
                    break;
                //case "BottomOrientPad":
                case "Pad":
                case "PadPos":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    return m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(Child2, Child3);
                    break;
                case "Li3D":
                case "Li3DPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(Child2, Child3);
                    break;
                case "Seal":
                    return m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(Child2, Child3, m_smVisionInfo.g_intVisionNameNo);
                    break;
                case "Barcode":
                    return m_smCustomizeInfo.objNewUserRight.GetBarcodeChild3Group(Child2, Child3);
                    break;
            }

            return 1;
        }
        private void FillLotComboBox()
        {
            cbo_Lot.Items.Clear();

            ArrayList arrFileList = new ArrayList();

            if (!Directory.Exists(m_smProductionInfo.g_strHistoryDataLocation + "ResultLog"))
                Directory.CreateDirectory(m_smProductionInfo.g_strHistoryDataLocation + "ResultLog");
            //string[] strLotList = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "ResultLog\\" + "Vision" + (m_smVisionInfo.g_intVisionIndex + 1).ToString() + "\\", "*.csv");
            string[] strLotList = Directory.GetDirectories(m_smProductionInfo.g_strHistoryDataLocation + "ResultLog\\");
            foreach (string strLot in strLotList)
            {
                arrFileList.Add(new DirectoryInfo(strLot));
            }
            if (arrFileList.Count > 0)
            {
                //sort the folder lost until the latest new folder is at the first
                arrFileList.Sort(m_objTimeComparer);

                for (int i = 0; i < arrFileList.Count; i++)
                {
                    string[] strFilePart = ((DirectoryInfo)arrFileList[i]).Name.Split('_');
                    //strFilePart[1] = strFilePart[1].Substring(0, strFilePart[1].Length - 4);
                    string strDate = strFilePart[1].Substring(0, 4) + "/" + strFilePart[1].Substring(4, 2) + "/" +
                        strFilePart[1].Substring(6, 2) + " " + strFilePart[1].Substring(8, 2) + ":" +
                        strFilePart[1].Substring(10, 2) + ":" + strFilePart[1].Substring(12, 2);
                    string strFileName = strFilePart[0] + " From : " + strDate;

                    cbo_Lot.Items.Add(strFileName);
                }
            }

            //if (!m_smProductionInfo.g_blnEndLotStatus)
            //{
            //    string strTemp = m_smProductionInfo.g_strLotStartTime.Substring(0, 4) + "/" + m_smProductionInfo.g_strLotStartTime.Substring(4, 2) + "/" +
            //              m_smProductionInfo.g_strLotStartTime.Substring(6, 2) + " " + m_smProductionInfo.g_strLotStartTime.Substring(8, 2) + ":" +
            //              m_smProductionInfo.g_strLotStartTime.Substring(10, 2) + ":" + m_smProductionInfo.g_strLotStartTime.Substring(12, 2);
            //    cbo_Lot.Items.Insert(0, m_smProductionInfo.g_strLotID + " From : " + strTemp);
            //}

            if (cbo_Lot.Items.Count > 0)
            {
                cbo_Lot.SelectedIndex = 0;
                cbo_Lot.Enabled = true;
            }
        }

        private void FillFileComboBox()
        {
            cbo_File.Items.Clear();

            if (cbo_Lot.SelectedIndex < 0)
                return;

            ArrayList arrFileList = new ArrayList();

            string strSelectedLot = cbo_Lot.SelectedItem.ToString();
            int fromStart = strSelectedLot.IndexOf("From", 0);
            string strLotID = strSelectedLot.Substring(0, fromStart - 1);
            string strLotStartTime = strSelectedLot.Substring(fromStart + 7);
            string strLotTime = strLotStartTime.Substring(0, 4) + strLotStartTime.Substring(5, 2) + strLotStartTime.Substring(8, 2) +
                strLotStartTime.Substring(11, 2) + strLotStartTime.Substring(14, 2) + strLotStartTime.Substring(17, 2);

            string[] strFileList = Directory.GetFiles(m_smProductionInfo.g_strHistoryDataLocation + "ResultLog\\" + strLotID + "_" + strLotTime + "\\", "*.txt");
            
            foreach (string strFile in strFileList)
            {
                arrFileList.Add(new DirectoryInfo(strFile));
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

                    string[] split = strFilePart[0].Split('(');
                    string[] split2 = split[1].Split(' ');
                    if (m_smVisionInfo.g_strVisionFolderName == split[0] && split2[0] == m_smVisionInfo.g_strVisionDisplayName)
                        cbo_File.Items.Add(strFileName);
                }
            }

            if (cbo_File.Items.Count > 0)
            {
                cbo_File.SelectedIndex = 0;
                cbo_File.Enabled = true;
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }



        private void SaveToCSV(DataGridView DGV)
        {
            string filename = "";
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV (*.csv)|*.csv";

            string[] split = cbo_File.SelectedItem.ToString().Split(')');

            sfd.FileName = "ResultLog_" + split[0] + ")_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(filename))
                {
                    try
                    {
                        File.Delete(filename);
                    }
                    catch (IOException ex)
                    {
                        SRMMessageBox.Show("Saving Fail." + ex.Message);
                    }
                }
                int columnCount = DGV.ColumnCount;
                string columnNames = "";
                string[] output = new string[DGV.RowCount + 1];
                for (int i = 0; i < columnCount; i++)
                {
                    if (i == columnCount - 1)
                        columnNames += DGV.Columns[i].HeaderText.ToString();
                    else
                        columnNames += DGV.Columns[i].HeaderText.ToString() + ",";
                }
                output[0] += columnNames;
                for (int i = 1; (i - 1) < DGV.RowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        if (j == columnCount - 1)
                            output[i] += DGV.Rows[i - 1].Cells[j].Value.ToString();
                        else
                            output[i] += DGV.Rows[i - 1].Cells[j].Value.ToString() + ",";
                    }
                }
                System.IO.File.WriteAllLines(sfd.FileName, output, System.Text.Encoding.UTF8);
                SRMMessageBox.Show("Result Saved Successfully.", "Save", MessageBoxButtons.OK);
            }
        }

        private void btn_SaveResult_Click(object sender, EventArgs e)
        {
            if (dgd_ResultLog.Rows.Count > 0)
            {
                SaveToExcel(dgd_ResultLog); //SaveToCSV(dgd_ResultLog);
            }
            else
            {
                SRMMessageBox.Show("No Result can be saved.", "Error", MessageBoxButtons.OK);
            }
        }
        private void copyAlltoClipboard(DataGridView DGV)
        {
            DGV.MultiSelect = true;
            DGV.SelectAll();
            DataObject dataObj = DGV.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
            DGV.MultiSelect = false;
        }
        private void SaveToExcel(DataGridView DGV)
        {
            string filename = "";
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Documents (*.xlsx)|*.xlsx";

            string[] split = cbo_File.SelectedItem.ToString().Split(')');

            sfd.FileName = "ResultLog_" + split[0] + ")_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(filename))
                {
                    try
                    {
                        File.Delete(filename);
                    }
                    catch (IOException ex)
                    {
                        SRMMessageBox.Show("Saving Fail." + ex.Message);
                    }
                }

                //File.Copy("C:\\Users\\zjyeoh\\Desktop\\Result Log Template.xlsx", sfd.FileName);

                copyAlltoClipboard(DGV);

                Microsoft.Office.Interop.Excel.Application xlexcel;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;
                xlexcel = new Excel.Application();
                //xlexcel.Visible = true;
                xlWorkBook = xlexcel.Workbooks.Add(misValue);
                //xlWorkBook = xlexcel.Workbooks.Open(@"C:\Users\zjyeoh\Desktop\Result Log Template.xlsx", 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                
                Excel.Range CR = (Excel.Range)xlWorkSheet.Cells[1, 1];
                CR.Select();
                xlWorkSheet.PasteSpecial(CR, false, false, Type.Missing, Type.Missing, Type.Missing, true);
                CR = xlWorkSheet.UsedRange;
                CR.AutoFormat(Microsoft.Office.Interop.Excel.XlRangeAutoFormat.xlRangeAutoFormatColor1);
                CR.EntireColumn.AutoFit();
                Microsoft.Office.Interop.Excel.Borders border = CR.Borders;
                border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;
                CR = (Excel.Range)xlWorkSheet.Cells[1, 1];
                CR.Select();
                xlWorkBook.SaveAs(sfd.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, misValue, misValue,
                            false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                            misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlexcel.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlexcel);

                SRMMessageBox.Show("Result Saved Successfully.", "Save", MessageBoxButtons.OK);
                
            }
        }
     
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                 m_smVisionInfo.g_strVisionFolderName + "\\General.xml";

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("ResultLog");
            
            objFileHandle.WriteElement1Value("ResultLogMaxCount", txt_TotalResultCount.Text);
            objFileHandle.WriteElement1Value("WantRecordResult", chk_WantRecordResult.Checked);

            objFileHandle.WriteEndElement();

            m_smVisionInfo.g_blnWantRecordResult = chk_WantRecordResult.Checked;
            m_smVisionInfo.g_intResultLogMaxCount = Convert.ToInt32(txt_TotalResultCount.Text);

            this.Close();
            this.Dispose();
        }

        private void cbo_Lot_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillFileComboBox();
            DisplayLotResultLog();
        }
        
        private void cbo_File_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayLotResultLog();
        }

        private void DisplayLotResultLog()
        {
            if (cbo_Lot.SelectedIndex < 0 || cbo_File.SelectedIndex < 0)
            {
                if (dgd_ResultLog.Columns.Count > 0)
                {
                    dgd_ResultLog.Columns.Clear();
                }
                return;
            }
            string strSelectedLot = cbo_Lot.SelectedItem.ToString();
            int fromStart = strSelectedLot.IndexOf("From", 0);
            string strLotID = strSelectedLot.Substring(0, fromStart - 1);
            string strLotStartTime = strSelectedLot.Substring(fromStart + 7);
            string strLotTime = strLotStartTime.Substring(0, 4) + strLotStartTime.Substring(5, 2) + strLotStartTime.Substring(8, 2) +
                strLotStartTime.Substring(11, 2) + strLotStartTime.Substring(14, 2) + strLotStartTime.Substring(17, 2);

            string strSelectedFile = cbo_File.SelectedItem.ToString();
            fromStart = strSelectedFile.IndexOf("From", 0);
            string strFileID = strSelectedFile.Substring(0, fromStart - 1);
            string strFileStartTime = strSelectedFile.Substring(fromStart + 7);
            string strFileTime = strFileStartTime.Substring(0, 4) + strFileStartTime.Substring(5, 2) + strFileStartTime.Substring(8, 2) +
                strFileStartTime.Substring(11, 2) + strFileStartTime.Substring(14, 2) + strFileStartTime.Substring(17, 2);

            string strSelectedFilePath = m_smProductionInfo.g_strHistoryDataLocation + "ResultLog\\" + strLotID + "_" + strLotTime + "\\" + strFileID + "_" + strFileTime + ".txt";
            if (File.Exists(strSelectedFilePath))
            {
                //dgd_ResultLog.Rows.Clear();
                dgd_ResultLog.Columns.Clear();


                DataTable dt = new DataTable();
                string[] lines = System.IO.File.ReadAllLines(strSelectedFilePath);
                List<string> HeaderList = new List<string>();
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string[] datas = lines[i].Split(',');
                        foreach (string data in datas)
                        {
                            string[] split = data.Split('=');

                            if (HeaderList.Contains(split[0]))
                                HeaderList.RemoveAt(HeaderList.LastIndexOf(split[0]));

                            HeaderList.Add(split[0]);
                          
                        }
                        
                    }
                }
                
                List<List<string>> DataList = new List<List<string>>();

                dt.Columns.Add(new DataColumn("Number"));
                HeaderList.RemoveAt(HeaderList.IndexOf("Date Time"));
                HeaderList.Insert(0, "Date Time");
                foreach (string header in HeaderList)
                {
                    dt.Columns.Add(new DataColumn(header));
                    DataList.Add(new List<string>());
                }
                
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        foreach (string header in HeaderList)
                        {
                            DataList[HeaderList.IndexOf(header)].Add("---");
                        }
                        string[] datas = lines[i].Split(',');
                        foreach (string data in datas)
                        {
                            string[] split = data.Split('=');
                            //if(split[0] == "Date Time")
                            //    DataList[0].Add(split[1]);
                            //else
                                DataList[HeaderList.IndexOf(split[0])][i] = split[1];
                        }

                    }
                }
                
                if (lines.Length > 0)
                {
                    //For Data
                    for (int i = 0; i < lines.Length; i++)
                    {
                        DataRow dr = dt.NewRow();
                        int columnIndex = 0;
                        dr[0] = i + 1;
                        foreach (string headerWord in HeaderList)
                        {
                            //string[] dataWords = DataList[columnIndex].ToArray();
                            dr[headerWord] = DataList[HeaderList.IndexOf(headerWord)][i];// dataWords[columnIndex++];
                        }
                        dt.Rows.Add(dr);
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    dgd_ResultLog.DataSource = dt;
                    //dgd_ResultLog.Columns[0].Width = 70;
                    //dgd_ResultLog.Columns[1].Width = 150;

                }

                if (dgd_ResultLog.Columns.Count > 0)
                {
                    dgd_ResultLog.Columns[0].Frozen = true;
                    dgd_ResultLog.Columns[0].DefaultCellStyle.BackColor = SystemColors.ControlLight;
                    dgd_ResultLog.Columns[0].DefaultCellStyle.ForeColor = SystemColors.WindowText;
                    dgd_ResultLog.Columns[0].DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                    dgd_ResultLog.Columns[0].DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
                }

            }
            else
            {
                //if (m_smVisionInfo.g_arrResultLogDateTime.Count == 0)
                if (dgd_ResultLog.Columns.Count > 0)
                    dgd_ResultLog.Columns.Clear();
            }


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

        private void ResultLogForm_Load(object sender, EventArgs e)
        {
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
        }

        private void ResultLogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Result Log Form Closed", "Exit Result Log Form", "", "", m_smProductionInfo.g_strLotID);
            
        }
    }
}
