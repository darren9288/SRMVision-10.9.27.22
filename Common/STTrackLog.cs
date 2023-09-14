using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace Common
{
    public class STTrackLog
    {
        #region Member Variables

        private static int writerTimeouts = 5000;
        private static string m_strFilePath = DBCall.m_strSVGTrackLogPath + "TrackLog\\";
        private static string m_strReportFilePath = DBCall.m_strSVGTrackLogPath + "ReportLog\\";
        private static string m_strFileName = "";
        private static ReaderWriterLock rwl = new ReaderWriterLock();
        private static bool m_blnWriteOn = true; //Decide want to write trackLog or not
        private static object m_objLock = new object();

        #endregion

        public static void WriteLine(string strMessage)
        {
            if (!m_blnWriteOn)
                return;

            DateTime dt = DateTime.Now;
            string strFileDirectory;
            string strFilePath = m_strFilePath + dt.ToString("yyyy-MM-dd") + "\\";

            if (!System.IO.Directory.Exists(strFilePath))
            {
                System.IO.Directory.CreateDirectory(strFilePath);
                LogAutoPurge();
            }

            if (m_strFileName == "")
            {
                strFileDirectory = strFilePath + dt.ToString("yyyy-MM-dd HH") + ".srm";
            }
            else
                strFileDirectory = strFilePath + m_strFileName + ".srm";


            rwl.AcquireWriterLock(writerTimeouts);

            try
            {
                FileInfo fi = new FileInfo(strFileDirectory);

                if (!fi.Exists)
                {
                    //Create a file to write to.
                    StreamWriter sw = fi.CreateText();
                    sw.Close();
                }

                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine(dt.ToString("dd/MM/yy hh:mm:ss.ffff") + " - " + strMessage + "\n");
                    sw.Close();
                }
            }
            catch (ApplicationException)
            {
                // The writer lock request timed out.
                Interlocked.Increment(ref writerTimeouts);
            }
            finally
            {
                // Ensure that the lock is released.
                rwl.ReleaseWriterLock();
            }
        }
        public static void WriteLine(string strMessage, bool blnWantBreakLine)
        {
            if (!m_blnWriteOn)
                return;

            DateTime dt = DateTime.Now;
            string strFileDirectory;
            string strFilePath = m_strFilePath + dt.ToString("yyyy-MM-dd") + "\\";

            if (!System.IO.Directory.Exists(strFilePath))
            {
                System.IO.Directory.CreateDirectory(strFilePath);
                LogAutoPurge();
            }

            if (m_strFileName == "")
            {
                strFileDirectory = strFilePath + dt.ToString("yyyy-MM-dd HH") + ".srm";
            }
            else
                strFileDirectory = strFilePath + m_strFileName + ".srm";


            rwl.AcquireWriterLock(writerTimeouts);

            try
            {
                FileInfo fi = new FileInfo(strFileDirectory);

                if (!fi.Exists)
                {
                    //Create a file to write to.
                    StreamWriter sw = fi.CreateText();
                    sw.Close();
                }

                using (StreamWriter sw = fi.AppendText())
                {
                    if (blnWantBreakLine)
                    {
                        sw.WriteLine(strMessage + "\n");
                    }
                    else
                    {
                        sw.Write(strMessage);
                    }
                    sw.Close();
                }
            }
            catch (ApplicationException)
            {
                // The writer lock request timed out.
                Interlocked.Increment(ref writerTimeouts);
            }
            finally
            {
                // Ensure that the lock is released.
                rwl.ReleaseWriterLock();
            }
        }
        public static void Write_ForResultLog(string Path, string FileName, string strMessage)
        {
            if (!m_blnWriteOn)
                return;

            DateTime dt = DateTime.Now;
            string strFileDirectory;
            string strFilePath = Path;// m_strResultLogPath + Path;

            if (!System.IO.Directory.Exists(strFilePath))
            {
                System.IO.Directory.CreateDirectory(strFilePath);
            }

            strFileDirectory = strFilePath + "\\" + FileName + ".txt";


            rwl.AcquireWriterLock(writerTimeouts);

            try
            {
                FileInfo fi = new FileInfo(strFileDirectory);

                if (!fi.Exists)
                {
                    //Create a file to write to.
                    StreamWriter sw = fi.CreateText();
                    sw.Close();
                }

                using (StreamWriter sw = fi.AppendText())
                {
                    sw.Write(strMessage);
                    sw.Close();
                }
            }
            catch (ApplicationException)
            {
                // The writer lock request timed out.
                Interlocked.Increment(ref writerTimeouts);
            }
            finally
            {
                // Ensure that the lock is released.
                rwl.ReleaseWriterLock();
            }
        }
        public static void WriteLine_ForResultLog(string Path, string FileName, string strMessage)
        {
            if (!m_blnWriteOn)
                return;

            DateTime dt = DateTime.Now;
            string strFileDirectory;
            string strFilePath = Path;// m_strResultLogPath + Path;

            if (!System.IO.Directory.Exists(strFilePath))
            {
                System.IO.Directory.CreateDirectory(strFilePath);
            }

            strFileDirectory = strFilePath + "\\" + FileName + ".txt";


            rwl.AcquireWriterLock(writerTimeouts);

            try
            {
                FileInfo fi = new FileInfo(strFileDirectory);

                if (!fi.Exists)
                {
                    //Create a file to write to.
                    StreamWriter sw = fi.CreateText();
                    sw.Close();
                }

                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine(strMessage);
                    sw.Close();
                }
            }
            catch (ApplicationException)
            {
                // The writer lock request timed out.
                Interlocked.Increment(ref writerTimeouts);
            }
            finally
            {
                // Ensure that the lock is released.
                rwl.ReleaseWriterLock();
            }
        }

        public static void WriteLine_Report(string strMessage, string strFileName, bool blnWantDate)
        {
            DateTime dt = DateTime.Now;
            string strFileDirectory;
            if (m_strFileName == "")
                strFileDirectory = m_strReportFilePath + strFileName + ".srm";
            else
                strFileDirectory = m_strReportFilePath + m_strFileName + ".srm";

            rwl.AcquireWriterLock(writerTimeouts);
            try
            {
                FileInfo fi = new FileInfo(strFileDirectory);
                if (!fi.Exists)
                {
                    //Create a file to write to.
                    StreamWriter sw = fi.CreateText();
                    sw.Close();
                }
                using (StreamWriter sw = fi.AppendText())
                {
                    if (blnWantDate)
                    {
                        sw.WriteLine(dt.ToString("dd/MM/yy hh:mm:ss.ffff") + " - " + strMessage + "\n");
                    }
                    else
                    {
                        sw.WriteLine(strMessage + "\n");
                    }
                    sw.Close();
                }
            }
            catch (ApplicationException)
            {
                // The writer lock request timed out.
                Interlocked.Increment(ref writerTimeouts);
            }
            finally
            {
                // Ensure that the lock is released.
                rwl.ReleaseWriterLock();
            }
        }

        public static void WriteLine_Report(string strMessage, string strFolderName, string strFileName, bool blnWantDate)
        {
            DateTime dt = DateTime.Now;
            string strFileDirectory;
            if (m_strFileName == "")
                strFileDirectory = m_strReportFilePath + strFolderName + "\\" + strFileName + ".srm";
            else
                strFileDirectory = m_strReportFilePath + strFolderName + "\\" + m_strFileName + ".srm";

            if (!Directory.Exists(m_strReportFilePath + strFolderName + "\\"))
            {
                Directory.CreateDirectory(m_strReportFilePath + strFolderName + "\\");
            }

            rwl.AcquireWriterLock(writerTimeouts);
            try
            {
                FileInfo fi = new FileInfo(strFileDirectory);
                if (!fi.Exists)
                {
                    //Create a file to write to.
                    StreamWriter sw = fi.CreateText();
                    sw.Close();
                }
                using (StreamWriter sw = fi.AppendText())
                {
                    if (blnWantDate)
                    {
                        sw.WriteLine(dt.ToString("dd/MM/yy hh:mm:ss.ffff") + " - " + strMessage + "\n");
                    }
                    else
                    {
                        sw.WriteLine(strMessage + "\n");
                    }
                    sw.Close();
                }
            }
            catch (ApplicationException)
            {
                // The writer lock request timed out.
                Interlocked.Increment(ref writerTimeouts);
            }
            finally
            {
                // Ensure that the lock is released.
                rwl.ReleaseWriterLock();
            }
        }

        public static void LogAutoPurge()
        {
            DirectoryInfo Dir = new DirectoryInfo(m_strFilePath);
            DirectoryInfo[] DirList = Dir.GetDirectories();
            foreach (DirectoryInfo subDir in DirList)
            {
                if (subDir.CreationTime < DateTime.Now.AddYears(-1))
                    subDir.Delete(true);
            }
        }

        public static void WriteOnSetting(bool blnWantWriteOn)
        {
            m_blnWriteOn = blnWantWriteOn;
        }

        public static List<string> m_arrTrackingData = new List<string>();

        public static void ClearTrackingData()
        {
            m_arrTrackingData.Clear();
        }

        public static void WriteTrackingData()
        {
            TrackLog objTL = new TrackLog();

            for (int i = 0; i < m_arrTrackingData.Count; i++)
            {
                objTL.WriteLine(m_arrTrackingData[i]);
            }
        }

        public static void AddTrackingData(string strData)
        {
            lock (m_objLock)
            {
                m_arrTrackingData.Add(strData);

                if (m_arrTrackingData.Count > 3000)
                {
                    m_arrTrackingData.RemoveAt(0);
                }
            }
        }
    }
}
