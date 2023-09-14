using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common
{
    public class TrackLog
    {
        #region Member Variables

        private string m_strFilePath = "";
        private string m_strFileName = "";

        #endregion

        public TrackLog()
        {
            m_strFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\TrackLog\\";
            if (!Directory.Exists(m_strFilePath))
                Directory.CreateDirectory(m_strFilePath);
        }

        /// <summary>
        /// record down the error
        /// </summary>
        /// <param name="strFolderPath">folder path to save file</param>
        public TrackLog(string strFolderPath)
        {
            m_strFilePath = strFolderPath;
            if (!Directory.Exists(m_strFilePath))
                Directory.CreateDirectory(m_strFilePath);
        }

        public TrackLog(string strFolderPath, string strFileName)
        {
            m_strFilePath = strFolderPath;
            if (!Directory.Exists(m_strFilePath))
                Directory.CreateDirectory(m_strFilePath);

            m_strFileName = strFileName;
        }


        public void WriteLine(string strMessage)
        {
            DateTime dt = DateTime.Now;
            string strFileDirectory;
            if (m_strFileName == "")
                strFileDirectory = m_strFilePath + dt.ToString("yyyy-MM") + ".srm";
            else 
                strFileDirectory = m_strFilePath + m_strFileName + ".srm";               

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

        public void WriteLine_NoDate(string strMessage)
        {
            DateTime dt = DateTime.Now;
            string strFileDirectory;
            if (m_strFileName == "")
                strFileDirectory = m_strFilePath + dt.ToString("yyyy-MM") + ".srm";
            else
                strFileDirectory = m_strFilePath + m_strFileName + ".srm";

            FileInfo fi = new FileInfo(strFileDirectory);
            if (!fi.Exists)
            {
                //Create a file to write to.
                StreamWriter sw = fi.CreateText();
                sw.Close();
            }

            using (StreamWriter sw = fi.AppendText())
            {
                sw.WriteLine(strMessage + "\n");
                sw.Close();
            }
        }
    }
}
