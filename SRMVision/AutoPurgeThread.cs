using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Text;
using System.Threading;
using Common;
using SharedMemory;
using System.Threading.Tasks;

namespace SRMVision
{

    public class AutoPurgeThread
    {
        #region Members Variables

        // Thread handle
        private readonly object m_objStopLock = new object();
        private bool m_blnStopping = false;
        private bool m_blnStopped = false;
        private bool m_blnPauseThread = false;
        private bool[] m_blnContinue;
        private List<string> temp = new List<string>();

        private bool m_blnInit = true;
        private bool m_blnDelete = false;
        private ArrayList m_arrFolder = new ArrayList();
        private DirectoryInfo[] m_arrReject = new DirectoryInfo[8];
        private FileSorting m_objTimeComparer = new FileSorting();
        private Thread m_thAutoPurgeThread;
        private CustomOption m_smCustomizeInfo;
        private VisionInfo[] m_smVSInfo;
        private ProductionInfo m_smProductionInfo;
        #endregion

        public AutoPurgeThread(CustomOption smCustomizeInfo, VisionInfo[] smVSInfo,ProductionInfo smProductionInfo)
        {
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVSInfo = smVSInfo;
            m_smProductionInfo = smProductionInfo;
            LoadFromXML();
            m_blnContinue = new bool[m_smVSInfo.Length];

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                m_blnContinue[i] = false;
                m_arrReject[i] = new DirectoryInfo(m_smVSInfo[i].g_strSaveImageLocation);
            }

            //List<string> arrThreadNameBF = new List<string>();
            //List<string> arrThreadNameAF = new List<string>();
            //arrThreadNameBF = ProcessTh.GetThreadsName("SRMVision");

            m_thAutoPurgeThread = new Thread(new ThreadStart(UpdateProgress));
            m_thAutoPurgeThread.IsBackground = true;
            m_thAutoPurgeThread.Priority = ThreadPriority.Lowest;
            m_thAutoPurgeThread.Start();

            //Thread.Sleep(500);
            //arrThreadNameAF = ProcessTh.GetThreadsName("SRMVision");
            //ProcessTh.GetDifferentThreadsName(arrThreadNameAF, arrThreadNameBF, "4", 0x02);
        }



        /// <summary>
        /// Returns whether the worker thread has stopped.
        /// </summary>
        public bool IsThreadStopped
        {
            get
            {
                lock (m_objStopLock)
                {
                    return m_blnStopped;
                }
            }
        }
        /// <summary>
        /// Tells the thread to pause, typically after completing its current work item.
        /// </summary>
        public void PauseThread()
        {
            if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("A3 - AutoPurgeThread - PauseThread.");

            lock (m_objStopLock)
            {
                if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("A4 - AutoPurgeThread - m_blnPauseThread = true.");
                //m_blnPauseThread = true;
                m_blnPauseThread = !m_smProductionInfo.g_blnDeleteImageDuringProduction;
            }
        }
        /// <summary>
        /// Tells the thread to resume
        /// </summary>
        public void ResumeThread()
        {
            if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("A5 - AutoPurgeThread - ResumeThread.");
            lock (m_objStopLock)
            {
                if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("A6 - AutoPurgeThread - m_blnPauseThread = false;");
                m_blnPauseThread = false;
            }
        }
        /// <summary>
        /// Tells the thread to stop, typically after completing its current work item.
        /// </summary>
        public void StopThread()
        {
            lock (m_objStopLock)
            {
                m_blnStopping = true;
            }
        }


        /// <summary>
        /// Check the used space in this directory
        /// </summary>
        /// <param name="Dir">directory path and name</param>
        /// <returns>used space size in double format</returns>
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
        
        /// <summary>
        /// Delete rejected image folder according to descending sorting of file creation 
        /// </summary>
        private void DeleteImageFolder(int intVision)
        {
            string[] strDirectories = Directory.GetDirectories(m_smVSInfo[intVision].g_strSaveImageLocation);
            if (strDirectories.Length > 0)
            {
                //sort the folder lost until the latest new folder is at the first
                Array.Sort(strDirectories, m_objTimeComparer.CompareCreateDescending);
                m_arrFolder = new ArrayList();
                foreach (string strDirectory in strDirectories)
                {
                    m_arrFolder.Add(new DirectoryInfo(strDirectory));
                }
                
                DateTime d = DateTime.Now;
                m_smVSInfo[0].g_strDeleteImageDate.Add(d.ToString());
                m_smVSInfo[0].g_strDeleteImageFileName.Add(m_arrFolder[m_arrFolder.Count - 1].ToString());
                DeleteEldestFolder();
            }
        }

        /// <summary>
        /// Delete earliest created folder which is last one
        /// </summary>
        private void DeleteEldestFolder()
        {
            try
            {
                if (m_arrFolder.Count > 1)
                {
                    //TrackLog objTL = new TrackLog();
                    //objTL.WriteLine("DeleteEldestFolder count number: " + m_arrFolder[m_arrFolder.Count - 1] + " ---------- Now Count is:" + m_arrFolder.Count);
                    STTrackLog.WriteLine("DeleteEldestFolder count number: " + m_arrFolder[m_arrFolder.Count - 1] + " ---------- Now Count is:" + m_arrFolder.Count);
                    DirectoryInfo di = (DirectoryInfo)(m_arrFolder[m_arrFolder.Count - 1]);
                    di.Delete(true);                    
                    m_arrFolder.RemoveAt(m_arrFolder.Count - 1);
                }
            }
            catch
            {
                //TrackLog objTL = new TrackLog();
                //objTL.WriteLine("Fail deleted folder name is " + m_arrFolder[m_arrFolder.Count - 1]);
                STTrackLog.WriteLine("DeleteEldestFolder count number: " + m_arrFolder[m_arrFolder.Count - 1] + " ---------- Now Count is:" + m_arrFolder.Count);
            }
        }

        /// <summary>
        /// Called by the thread to indicate when it has stopped.
        /// </summary>
        private void SetStopped()
        {
            lock (m_objStopLock)
            {
                m_blnStopped = true;
            }
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

        //2021/9/15 cxlim:  Delete Condition - SaveImage Folder Occupied Percent larger than 65%
        private void UpdateProgress()
        {
            try
            {
                double dOccupiedPercent = 0;
                int intIntervalSleep = 1000;
                while (!m_blnStopping)
                {
                    bool bln_match = false;
                    string m_strPrevious = "";
                    int index = 0;

                    if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("B1 - m_blnStopping loop is working.");

                    if (!m_blnPauseThread)
                    {
                        if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("B2 - m_blnPauseThread loop is working.");
                        for (int i = 0; i < m_smVSInfo.Length; i++)
                        {
                            if (m_smVSInfo[i] == null)
                                continue;

                            if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("B3 - i = " + i.ToString());

                            double dByte = GetDiskSpace(m_arrReject[i]) / Math.Pow(1024, 3); // change to Giga byte format -- 1024 = Kilo, 1024*1024 = Mega, 1024 * 1024 * 1024 = Giga
                            string strParentDir = m_arrReject[i].FullName.Substring(0, 1);
                            DriveInfo D_Drive = new DriveInfo(strParentDir);
                            double dTotalByte = D_Drive.TotalSize / Math.Pow(1024, 3);
                            double dByte2 = GetDataFileSize(new DriveInfo(m_smProductionInfo.g_strHistoryDataLocation + "Data")); //get data folder size
                            dOccupiedPercent = Math.Round(dByte / dTotalByte * 100,2);
                            double dOccupiedPercent2 = Math.Round(dByte2 / dTotalByte * 100,2); //data folder occupied percent

                            if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("B4 - dByte = " + dByte.ToString());
                            if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("B4 - strParentDir = " + strParentDir.ToString());
                            if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("B4 - dTotalByte = " + dTotalByte.ToString());
                            if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("B4 - dByte2 = " + dByte2.ToString());
                            if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("B4 - dOccupiedPercent = " + dOccupiedPercent.ToString());
                            if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("B4 - dOccupiedPercent2 = " + dOccupiedPercent2.ToString());

                            // 2022 01 11 - CCENG: Increase the loop speed if HD is occupied more than 65%
                            if (dOccupiedPercent >= 65 || dOccupiedPercent2 > 25)
                                intIntervalSleep = 50;
                            else
                                intIntervalSleep = 1000;


                            if (temp.Count != 0)
                            {
                               for(int j=0; j<temp.Count;j++)
                                {
                                    if (temp[j] == m_arrReject[i].FullName)  //if already check for 65% will directly go for checking >60%
                                    {
                                        bln_match = true;
                                        m_blnDelete = true;
                                        break;
                                    }
                                }
                            }

                            if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("B5 - bln_match = " + bln_match.ToString() + ", m_blnDelete = " + m_blnDelete.ToString());

                            if (!m_blnDelete && dOccupiedPercent >= 65  && !bln_match|| !m_blnDelete && dOccupiedPercent2 > 25) // check > 65 %
                            {
                                STTrackLog.WriteLine("Condition > 65% ,dByte: " + dByte);
                                double dTotalFreeSpaceByte = D_Drive.TotalFreeSpace / Math.Pow(1024, 3);
                                STTrackLog.WriteLine(D_Drive.Name.ToString() + ": Free Space Before Delete ,dTotalFreeSpaceByte: " + dTotalFreeSpaceByte);

                                if(dOccupiedPercent >= 65) //when data folder size exceed no delete image
                                {
                                    DeleteImageFolder(i);
                                    m_blnDelete = true;
                                    temp.Add(m_arrReject[i].FullName);
                                }

                                if (!m_blnDelete && dOccupiedPercent2 > 25 && m_blnInit)  //only show 1 time to alert user
                                {
                                    SRMMessageBox.Show("Data File Need to Clear, Current Space Occupied: " + dOccupiedPercent2 + "%");
                                    m_blnInit = false;
                                }

                                DateTime d = DateTime.Now;
                                dTotalFreeSpaceByte = D_Drive.TotalFreeSpace / Math.Pow(1024, 3);
                                STTrackLog.WriteLine("Free Space After Delete ,dTotalFreeSpaceByte: " + dTotalFreeSpaceByte + ",Date Deleted: " + d.ToString() + "Drive Name: " + D_Drive.Name.ToString());

                                dByte = GetDiskSpace(m_arrReject[i]) / Math.Pow(1024, 3);
                                dOccupiedPercent = Math.Round(dByte / dTotalByte * 100, 2);
                                
                                if (dOccupiedPercent >= 60)  //if still > 60 after delete continue checking
                                {
                                    m_blnContinue[i] = true;
                                    STTrackLog.WriteLine(D_Drive.Name.ToString() + "set continue to true, i = " + i + " " + dOccupiedPercent);
                                }
                                else
                                {
                                    m_blnContinue[i] = false;
                                    STTrackLog.WriteLine(D_Drive.Name.ToString() + "set continue to false, i = " + i + " " + dOccupiedPercent);
                                }
                            }
                            else if(m_blnDelete && dOccupiedPercent >= 65 && !bln_match)   // check > 65 % if previous folder delete process occur
                            {
                                STTrackLog.WriteLine("Condition > 65% ,dByte: " + dByte);
                                double dTotalFreeSpaceByte = D_Drive.TotalFreeSpace / Math.Pow(1024, 3);
                                STTrackLog.WriteLine(D_Drive.Name.ToString() + ": Free Space Before Delete ,dTotalFreeSpaceByte: " + dTotalFreeSpaceByte);

                                DeleteImageFolder(i);

                                for (int j = 0; j < m_smVSInfo.Length; j++)  //check wether folder gt use same drive b4
                                {
                                    if (m_smVSInfo[j] == null)
                                        continue;

                                    if (m_arrReject[i].FullName.Substring(0, 1).Equals(m_arrReject[j].FullName.Substring(0, 1)))
                                    {
                                        if (i > j)
                                        {
                                            index = j;
                                            break;
                                        }
                                        else if (i == j)
                                            index = j;
                                    }
                                    else
                                        index = i;
                                }

                                DateTime d = DateTime.Now;
                                dTotalFreeSpaceByte = D_Drive.TotalFreeSpace / Math.Pow(1024, 3);
                                STTrackLog.WriteLine("Free Space After Delete ,dTotalFreeSpaceByte: " + dTotalFreeSpaceByte + ",Date Deleted: " + d.ToString() + " Drive Name: " + D_Drive.Name.ToString());
                                temp.Add(m_arrReject[i].FullName); // added for later checking

                                dByte = GetDiskSpace(m_arrReject[i]) / Math.Pow(1024, 3);
                                dOccupiedPercent = Math.Round(dByte / dTotalByte * 100, 2);

                                if (dOccupiedPercent >= 60) //continue if still > 60%
                                {
                                    m_blnContinue[index] = true;
                                    STTrackLog.WriteLine(D_Drive.Name.ToString() + "set continue to true, index = " + index + " " + dOccupiedPercent);
                                }
                                else
                                {
                                    m_blnContinue[index] = false;
                                    STTrackLog.WriteLine(D_Drive.Name.ToString() + "set continue to false, index = " + index + " " + dOccupiedPercent);
                                }
                            }
                            else if (m_blnDelete && dOccupiedPercent >= 60 && bln_match)  //check > 60%
                            {
                                STTrackLog.WriteLine("Condition > 60% , dByte: " + dByte);
                                double dTotalFreeSpaceByte = D_Drive.TotalFreeSpace / Math.Pow(1024, 3);
                                DateTime d = DateTime.Now;
                                STTrackLog.WriteLine(D_Drive.Name.ToString() + ": Free Space Before Delete ,dTotalFreeSpaceByte: " + dTotalFreeSpaceByte);

                                for (int j = 0; j < m_smVSInfo.Length; j++)  //check wether folder gt use same drive b4
                                {
                                    if (m_smVSInfo[j] == null)
                                        continue;

                                    if (m_arrReject[i].FullName.Substring(0, 1).Equals(m_arrReject[j].FullName.Substring(0, 1)))
                                    {
                                        if (i > j)
                                        {
                                            index = j;
                                            break;
                                        }
                                        else if (i == j)
                                            index = j;
                                    }
                                    else
                                        index = i;
                                }

                                if (m_strPrevious != m_arrReject[i].FullName) //if previous folder deleted in different drive compare to current, nid to update array to get current drive save image folder
                                    DeleteImageFolder(i);
                                else
                                {
                                    if (m_arrFolder.Count > 1) // if same then continue delete
                                    {
                                        m_smVSInfo[0].g_strDeleteImageDate.Add(d.ToString());
                                        m_smVSInfo[0].g_strDeleteImageFileName.Add(m_arrFolder[m_arrFolder.Count - 1].ToString());
                                    }
                                    DeleteEldestFolder();
                                }

                                dTotalFreeSpaceByte = D_Drive.TotalFreeSpace / Math.Pow(1024, 3);
                                STTrackLog.WriteLine("Free Space After Delete ,dTotalFreeSpaceByte: " + dTotalFreeSpaceByte + ",Date Deleted: " + d.ToString() + " Drive Name: " + D_Drive.Name.ToString());

                                dByte = GetDiskSpace(m_arrReject[i]) / Math.Pow(1024, 3);
                                dOccupiedPercent = Math.Round(dByte / dTotalByte * 100, 2);

                                if (dOccupiedPercent >= 60) //continue if still > 60%
                                {
                                    m_blnContinue[index] = true;
                                    STTrackLog.WriteLine(D_Drive.Name.ToString() + "set continue to true, index = " + index + " " + dOccupiedPercent);
                                }
                                else
                                {
                                    m_blnContinue[index] = false;
                                    STTrackLog.WriteLine(D_Drive.Name.ToString() + "set continue to false, index = " + index + " " + dOccupiedPercent);
                                }
                            }
                            else if (m_blnDelete)
                            {
                                if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("B6");

                                bool m_blnClear = true;
                                for(int j=0; j<m_smVSInfo.Length;j++) // check wether all drive is still under continue checking process
                                {
                                    if (m_blnContinue[j])
                                    {
                                        m_blnClear = false;
                                        STTrackLog.WriteLine(j + " still continue :" + m_blnContinue[j]);
                                        break; 
                                    }
                                    else
                                        continue;
                                }

                                if (m_blnClear) // if all folder in all drive occupied less than condition
                                {
                                    STTrackLog.WriteLine("Clear out array for compare folder");
                                    m_blnDelete = false;
                                    double dTotalFreeSpaceByte = D_Drive.TotalFreeSpace / Math.Pow(1024, 3);
                                    STTrackLog.WriteLine(D_Drive.Name.ToString() + ":Free Space After set m_blnDelete to false ,dTotalFreeSpaceByte: " + dTotalFreeSpaceByte);
                                    bln_match = false;

                                    temp.Clear(); // clear out array so next checking will start again from 65%

                                    if (m_smVSInfo[0].g_strDeleteImageFileName.Count != 0)
                                    {
                                        SaveToXML(); //save folder deleted to xml
                                    }
                                }
                            }

                            if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("B7");
                            m_strPrevious = m_arrReject[i].FullName; //store current checking folder
                        }

                        if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("B8");
                    }

                    if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("B9");

                    Thread.Sleep(intIntervalSleep);
                }
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("AutoPurgeThread UpdateProgress Exception: " + ex.ToString());
            }
            finally
            {
                SetStopped();
            }
        }

        private void LoadFromXML()
        {
            XmlParser fileHandle = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "DeletedHistory.xml");

            fileHandle.GetFirstSection("Deleted");
            int count = fileHandle.GetValueAsInt("Count", 0);

            for (int i = 0; i < count; i++)
            {
                m_smVSInfo[0].g_strDeleteImageFileName.Add(fileHandle.GetValueAsString("File Name_" + i, ""));
                m_smVSInfo[0].g_strDeleteImageDate.Add(fileHandle.GetValueAsString("DeletedTime_" + i, ""));
            }
        }

        private void SaveToXML()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "DeletedHistory.xml";
            XmlParser fileHandle = new XmlParser(path);
            fileHandle.WriteSectionElement("Deleted",true);
            fileHandle.WriteElement1Value("Count", m_smVSInfo[0].g_strDeleteImageFileName.Count);

            for (int i=0;i<m_smVSInfo[0].g_strDeleteImageFileName.Count;i++)
            {
                fileHandle.WriteElement1Value("File Name_" + i, m_smVSInfo[0].g_strDeleteImageFileName[i]);
                fileHandle.WriteElement1Value("DeletedTime_" + i, m_smVSInfo[0].g_strDeleteImageDate[i].ToString());
            }
            fileHandle.WriteEndElement();
        }
    }
}
