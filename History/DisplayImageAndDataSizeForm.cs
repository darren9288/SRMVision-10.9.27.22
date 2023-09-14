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
using System.Windows.Forms.DataVisualization.Charting;

namespace History
{
    public partial class DisplayImageAndDataSizeForm : Form
    {
        private ProductionInfo m_smProductionInfo;
        private VisionInfo[] m_smVSInfo;
        private FileSorting Sort = new FileSorting();
        private List<string> SaveLocation = new List<string>();
        private Point? prevPosition = null;
        private ToolTip tooltip = new ToolTip();

        public DisplayImageAndDataSizeForm(VisionInfo[] smVisionInfo, ProductionInfo smProductionInfo)
        {
            m_smVSInfo = smVisionInfo;
            m_smProductionInfo = smProductionInfo;
            InitializeComponent();
            UpdateImageInfo();
            UpdateDataFolderInfo();
            UpdateDeletedImageInfo();
            FillChart();

            for(int i=0;i<SaveLocation.Count;i++)
            {
                if (SaveLocation[i].Contains("SVG") || SaveLocation[i].Contains("bin"))
                    continue;

                cbo_ViewPartition.Items.Add(SaveLocation[i].Substring(0, 1));
            }

            cbo_ViewPartition.SelectedIndex = 0;
        }


        private void UpdateImageInfo()
        {
            int counter = 0;
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                bool bln_StoreFile = StoreImageFileSize(i);

                if (Directory.Exists(m_smVSInfo[i].g_strSaveImageLocation) && bln_StoreFile)
                {
                    string[] strDirectories = Directory.GetDirectories(m_smVSInfo[i].g_strSaveImageLocation);
                    Array.Sort(strDirectories, Sort.CompareCreateAscending);

                    for (int j=0; j < strDirectories.Length;j++)
                    {
                        dgd_ImageFolder.Rows.Add();
                        dgd_ImageFolder.Rows[counter].Cells[0].Value = strDirectories[j];
                        dgd_ImageFolder.Rows[counter].Cells[1].Value = m_smVSInfo[0].g_dbFileImageSize[j];
                        counter++;
                    }
                }
            }
        }


        private void UpdateDeletedImageInfo()
        {
            for(int i=0; i<m_smVSInfo[0].g_strDeleteImageFileName.Count;i++)
            {
                dgd_ImageDeleted.Rows.Add();
                dgd_ImageDeleted.Rows[i].Cells[0].Value = m_smVSInfo[0].g_strDeleteImageFileName[i];
                dgd_ImageDeleted.Rows[i].Cells[1].Value = m_smVSInfo[0].g_strDeleteImageDate[i];
            }
        }

        private void UpdateDataFolderInfo()
        {
            DirectoryInfo dl = new DirectoryInfo(m_smProductionInfo.g_strHistoryDataLocation + "Data");
            int counter = 0;
            m_smVSInfo[0].g_dbFileDataSize.Clear();
            string[] FileInside = Directory.GetDirectories(dl.FullName);
            Array.Sort(FileInside, Sort.CompareCreateDescending);

            for (int j = 0; j < FileInside.Length; j++)
            {
                m_smVSInfo[0].g_dbFileDataSize.Add(new List<double>());
                string[] FileInside2 = Directory.GetDirectories(FileInside[j]);
                Array.Sort(FileInside2, Sort.CompareCreateAscending);

                for (int k = 0; k < FileInside2.Length; k++)
                {
                    DirectoryInfo dll = new DirectoryInfo(FileInside2[k]);
                    double b = Math.Round((GetDiskSpace(dll) / Math.Pow(1024, 2)), 2);
                    m_smVSInfo[0].g_dbFileDataSize[j].Add(b);
                }
            }

            for (int i = 0; i < FileInside.Length; i++)
            {
                string[] FileInside2 = Directory.GetDirectories(FileInside[i]);
                Array.Sort(FileInside2, Sort.CompareCreateAscending);

                for (int j = 0; j < FileInside2.Length; j++)
                {
                    dgd_Data.Rows.Add();
                    dgd_Data.Rows[counter].Cells[0].Value = FileInside2[j];
                    dgd_Data.Rows[counter].Cells[1].Value = m_smVSInfo[0].g_dbFileDataSize[i][j];
                    counter++;
                }
            }
        }

        public bool StoreImageFileSize(int intVision)
        {
            if (Directory.Exists(m_smVSInfo[intVision].g_strSaveImageLocation))
            {
                if (SaveLocation.Count != 0)
                {
                    for (int i = 0; i < SaveLocation.Count; i++)
                    {
                        if (m_smVSInfo[intVision].g_strSaveImageLocation.Equals(SaveLocation[i]))
                            return false;
                    }
                }

                string[] strDirectories = Directory.GetDirectories(m_smVSInfo[intVision].g_strSaveImageLocation);
                m_smVSInfo[0].g_dbFileImageSize.Clear();
                Array.Sort(strDirectories, Sort.CompareCreateAscending);

                for (int i = 0; i < strDirectories.Length; i++)
                {
                    DirectoryInfo dl = new DirectoryInfo(strDirectories[i]);
                    double dByte = Math.Round(GetDiskSpace(dl) / Math.Pow(1024, 2),MidpointRounding.AwayFromZero);
                    m_smVSInfo[0].g_dbFileImageSize.Add(dByte);
                }
                SaveLocation.Add(m_smVSInfo[intVision].g_strSaveImageLocation);
                return true;
            }
            else
                return false;
        }

        public double GetDataFileSize(DriveInfo D)
        {
            DirectoryInfo dl = new DirectoryInfo("D:\\Data\\");

            string strParentDir = dl.FullName.Substring(0, 3);

            if (strParentDir.Equals(D.Name))
            {
                double dByte = GetDiskSpace(dl) / Math.Pow(1024, 3);
                return dByte;
            }
            return -1;
        }

        private void FillChart()
        {
            //chart title  
            Chart_Space.Titles.Add("Space Occupied");
            Chart_Space.Legends[0].Enabled = true;
            Chart_Space.ChartAreas[0].AxisY.Minimum = 0;
            Chart_Space.ChartAreas[0].AxisY.Maximum = 100;

            for (int i = 0; i < SaveLocation.Count; i++)
            {
                DirectoryInfo dl = new DirectoryInfo(SaveLocation[i]);
                string strParentDir = dl.FullName.Substring(0, 1);
                double dByte2 = Math.Round(GetDataFileSize(new DriveInfo(m_smProductionInfo.g_strHistoryDataLocation + "Data")),2);
                DriveInfo D_Drive = new DriveInfo(strParentDir);
                double dTotalByte = Math.Round(D_Drive.TotalSize / Math.Pow(1024, 3), 2);
                double dByte = Math.Round(GetDiskSpace(dl) / Math.Pow(1024, 3), 2); // change to Giga byte format -- 1024 = Kilo, 1024*1024 = Mega, 1024 * 1024 * 1024 = Giga
                double dOccupiedPercent = Math.Round(dByte / dTotalByte * 100, 2);
                double FreeSpace = Math.Round(D_Drive.AvailableFreeSpace / Math.Pow(1024, 3), 2);

                if (strParentDir == m_smProductionInfo.g_strHistoryDataLocation.Substring(0, 1))
                {
                    double dByte3 = Math.Round(dTotalByte - dByte - dByte2 - FreeSpace,2);
                    double dOccupiedPercent2 = Math.Round(dByte2 / dTotalByte * 100, 2);
                    double dOccupiedPercent3 = Math.Round(dByte3 / dTotalByte * 100, 2);
                    double dOccupiedPercent4 = Math.Round(FreeSpace / dTotalByte * 100, 2);

                    if (dOccupiedPercent < 5)
                        Chart_Space.Series[0].IsValueShownAsLabel = false;
                    else
                        Chart_Space.Series[0].IsValueShownAsLabel = true;

                    if (dOccupiedPercent2 < 5)
                        Chart_Space.Series[1].IsValueShownAsLabel = false;
                    else
                        Chart_Space.Series[1].IsValueShownAsLabel = true;

                    if (dOccupiedPercent3 < 5)
                        Chart_Space.Series[2].IsValueShownAsLabel = false;
                    else
                        Chart_Space.Series[2].IsValueShownAsLabel = true;

                    if (dOccupiedPercent4 < 5)
                        Chart_Space.Series[3].IsValueShownAsLabel = false;
                    else
                        Chart_Space.Series[3].IsValueShownAsLabel = true;

                    Chart_Space.Series[0].Points.AddXY(SaveLocation[i].Substring(0, 1), dOccupiedPercent /*+ "%" + "(" + dByte + "GB)"*/);
                    Chart_Space.Series[1].Points.AddXY(SaveLocation[i].Substring(0, 1), dOccupiedPercent2/* + "%" + "(" + dByte2 + "GB)"*/);
                    Chart_Space.Series[2].Points.AddXY(SaveLocation[i].Substring(0, 1), dOccupiedPercent3/* + "%" + "(" + dByte3 + "GB)"*/);
                    Chart_Space.Series[3].Points.AddXY(SaveLocation[i].Substring(0, 1), dOccupiedPercent4 /*+ "%" + "(" + FreeSpace + "GB)"*/);

                    dgd_DisplaySpaceUsed.Rows.Add();
                    dgd_DisplaySpaceUsed.Rows[i].Cells[0].Value = strParentDir;
                    dgd_DisplaySpaceUsed.Rows[i].Cells[1].Value = dByte + " GB" + " (" + dOccupiedPercent.ToString() + "%)";
                    dgd_DisplaySpaceUsed.Rows[i].Cells[2].Value = dByte2 + " GB" + " (" + dOccupiedPercent2.ToString() + "%)";
                    dgd_DisplaySpaceUsed.Rows[i].Cells[3].Value = dByte3 + " GB" + " (" + dOccupiedPercent3.ToString() + "%)";
                    dgd_DisplaySpaceUsed.Rows[i].Cells[4].Value = FreeSpace + " GB" + " (" + dOccupiedPercent4.ToString() + "%)";
                }
                else
                {
                    double dByte3 = Math.Round(dTotalByte - dByte - FreeSpace,2);
                    double dOccupiedPercent3 = Math.Round(dByte3 / dTotalByte * 100, 2);
                    double dOccupiedPercent4 = Math.Round(FreeSpace / dTotalByte * 100, 2);
                    Chart_Space.Series[2].IsValueShownAsLabel = true;
                    Chart_Space.Series[3].IsValueShownAsLabel = true;
                    Chart_Space.Series[0].Points.AddXY(SaveLocation[i].Substring(0, 1), dOccupiedPercent /*+ "%" + "(" + dByte + "GB)"*/);
                    Chart_Space.Series[2].Points.AddXY(SaveLocation[i].Substring(0, 1), dOccupiedPercent3/* + "%" + "(" + dByte3 + "GB)"*/);
                    Chart_Space.Series[3].Points.AddXY(SaveLocation[i].Substring(0, 1), dOccupiedPercent4 /*+ "%" + "(" + FreeSpace + "GB)"*/);

                    if (dOccupiedPercent < 5)
                        Chart_Space.Series[0].IsValueShownAsLabel = false;
                    else
                        Chart_Space.Series[0].IsValueShownAsLabel = true;

                    if (dOccupiedPercent3 < 5)
                        Chart_Space.Series[2].IsValueShownAsLabel = false;
                    else
                        Chart_Space.Series[2].IsValueShownAsLabel = true;

                    if (dOccupiedPercent4 < 5)
                        Chart_Space.Series[3].IsValueShownAsLabel = false;
                    else
                        Chart_Space.Series[3].IsValueShownAsLabel = true;

                    dgd_DisplaySpaceUsed.Rows.Add();
                    dgd_DisplaySpaceUsed.Rows[i].Cells[0].Value = strParentDir;
                    dgd_DisplaySpaceUsed.Rows[i].Cells[1].Value = dByte + " GB" + " (" + dOccupiedPercent.ToString() + "%)";
                    dgd_DisplaySpaceUsed.Rows[i].Cells[2].Value = "-";
                    dgd_DisplaySpaceUsed.Rows[i].Cells[3].Value = dByte3 + " GB" + " (" + dOccupiedPercent3.ToString() + "%)";
                    dgd_DisplaySpaceUsed.Rows[i].Cells[4].Value = FreeSpace + " GB" + " (" + dOccupiedPercent4.ToString() + "%)";
                }
            }
        }

        private double GetDiskSpace(DirectoryInfo Dir)
        {
            double Size = 0;  // in byte

            try
            {
                FileInfo[] fis = Dir.GetFiles();
                foreach (FileInfo fi in fis)
                {
                    Size += Convert.ToDouble(fi.Length);
                }
                // Add subdirectory sizes.
                DirectoryInfo[] dis = Dir.GetDirectories();
                foreach (DirectoryInfo di in dis)
                {
                    Size += GetDiskSpace(di);
                }
            }
            catch
            {
            }

            return Size;
        }

        private void Chart_Space_MouseMove(object sender, MouseEventArgs e)
        {
            var Position = e.Location;
            if (prevPosition.HasValue && Position == prevPosition.Value)
                return;
            tooltip.RemoveAll();
            prevPosition = Position;
            var results = Chart_Space.HitTest(Position.X, Position.Y, false,
                                            ChartElementType.DataPoint);
            foreach (var result in results)
            {
                if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    var prop = result.Object as DataPoint;
                    if (prop != null)
                    {
                        tooltip.Show("Value =" + prop.YValues[0], this.Chart_Space,
                                 Position.X, Position.Y - 15);
                    }
                }
            }
        }

        private void tabCtrl_History_Selected(object sender, TabControlEventArgs e)
        {
            if(tabCtrl_History.SelectedTab == tp_Graph)
            {
                dgd_DisplaySpaceUsed.Visible = true;
            }
            else
                dgd_DisplaySpaceUsed.Visible = false;
        }

        private void cbo_ViewPartition_SelectedValueChanged(object sender, EventArgs e)
        {
            dgd_ImageDeleted.Rows.Clear();
            UpdateDeletedImageInfo();

            if (dgd_ImageDeleted.RowCount != 0)
            {
                for (int i = 0; i < dgd_ImageDeleted.RowCount; i++)
                {
                    if (!(dgd_ImageDeleted.Rows[i].Cells[0].Value.ToString().Substring(0, 1).Equals(cbo_ViewPartition.SelectedItem.ToString())))
                    {
                        dgd_ImageDeleted.Rows.Remove(dgd_ImageDeleted.Rows[i]);
                        i--;
                    }
                    else
                        continue;
                }
            }
            else
                return;
        }
    }
}
