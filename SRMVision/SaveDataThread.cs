using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using SharedMemory;
using Common;

namespace SRMVision
{
    public class SaveDataThread
    {
        #region Member Variables
        // Thread handle
        private readonly object m_objStopLock = new object();
        private bool m_blnStopping = false;
        private bool m_blnStopped = false;
        private Thread m_thThread;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo[] m_smVSInfo;
        private CustomOption m_smCustomizeInfo;

        #endregion

        public SaveDataThread(ProductionInfo smProductionInfo, VisionInfo[] smVSInfo, CustomOption smCustomizeInfo)
        {
            m_smProductionInfo = smProductionInfo;
            m_smVSInfo = smVSInfo;
            m_smCustomizeInfo = smCustomizeInfo;

            m_thThread = new Thread(new ThreadStart(UpdateProgress));
            m_thThread.IsBackground = true;
            m_thThread.Priority = ThreadPriority.Lowest;
            m_thThread.Start();
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
        /// Tells the thread to stop, typically after completing its 
        /// current work item.
        /// </summary>
        public void StopThread()
        {
            lock (m_objStopLock)
            {
                m_blnStopping = true;
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

        private void UpdateProgress()
        {
            try
            {
                while (!m_blnStopping)
                {
                    if (m_smProductionInfo.VM_TH_SaveData)
                    {
                        Save2DCodeToXMLFile();
                        m_smProductionInfo.VM_TH_SaveData = false;
                    }

                    Thread.Sleep(1);
                }
            }
            finally
            {
                SetStopped();
            }

        }

        private void Save2DCodeToXMLFile()
        {
            #region Save Data

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                int intPos = (1 << i);

                switch (m_smVSInfo[i].g_strVisionName)
                {
                    case "Mark":
                    case "MarkOrient":
                    case "MOLi":
                    case "InPocket":
                    case "MarkPkg":
                    case "MOPkg":
                    case "MOLiPkg":
                    case "InPocketPkg":
                    case "InPocketPkgPos":
                    case "IPMLi":
                    case "IPMLiPkg":
                        RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                        RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                        string strLot = subkey1.GetValue("LotNo", "SRM").ToString();
                        string strStartTime = subkey1.GetValue("LotStartTime", DateTime.Now.ToString("yyyyMMddHHmmss")).ToString();
                        string strStopTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string strMachineID = m_smCustomizeInfo.g_strMachineID;

                        XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "2DReport\\" + strLot +
                            "_" + strStartTime + ".xml");

                        objFile.WriteSectionElement("Lot");
                        objFile.WriteElement1Value("LotID", strLot);
                        objFile.WriteElement1Value("StartTime", strStartTime);
                        objFile.WriteElement1Value("StopTime", strStopTime);
                        objFile.WriteElement1Value("MachineID", m_smCustomizeInfo.g_strMachineID);

                        objFile.WriteSectionElement("Summary");
                        objFile.WriteElement1Value("Total", m_smVSInfo[i].g_intTestedTotal);
                        objFile.WriteElement1Value("Pass", m_smVSInfo[i].g_intPassTotal);
                        objFile.WriteElement1Value("Fail", "");
                        if ((m_smCustomizeInfo.g_intWantMark & intPos) > 0)
                            objFile.WriteElement2Value("Mark", m_smVSInfo[i].g_intMarkFailureTotal);
                        if ((m_smCustomizeInfo.g_intWantOrient & intPos) > 0)
                            objFile.WriteElement2Value("Orient", m_smVSInfo[i].g_intOrientFailureTotal);
                        if (m_smVSInfo[i].g_blnWantPin1)
                            objFile.WriteElement2Value("Pin1", m_smVSInfo[i].g_intPin1FailureTotal);
                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                            objFile.WriteElement2Value("Lead", m_smVSInfo[i].g_intLeadFailureTotal);
                        if ((m_smCustomizeInfo.g_intWantPackage & intPos) > 0 && ((m_smCustomizeInfo.g_intWantPad & intPos) == 0) && ((m_smCustomizeInfo.g_intWantPad5S & intPos) == 0))
                        {
                            objFile.WriteElement2Value("Package", m_smVSInfo[i].g_intPackageFailureTotal);
                            objFile.WriteElement2Value("PackageDefect", m_smVSInfo[i].g_intPkgDefectFailureTotal);
                            if (m_smVSInfo[i].g_blnViewColorImage)
                            {
                                objFile.WriteElement2Value("PackageColorDefect", m_smVSInfo[i].g_intPkgColorDefectFailureTotal);
                                objFile.WriteElement2Value("CheckPresence", m_smVSInfo[i].g_intCheckPresenceFailureTotal);
                                objFile.WriteElement2Value("Orient", m_smVSInfo[i].g_intOrientFailureTotal);
                            }
                        }
                        if (((m_smCustomizeInfo.g_intWantPositioning & intPos) > 0) && ((m_smCustomizeInfo.g_intWantPad & intPos) == 0))
                            objFile.WriteElement2Value("Positioning", m_smVSInfo[i].g_intPositionFailureTotal);

                        objFile.WriteSectionElement("DeviceIDMap");

                        objFile.WriteElement1Value("MarkID", m_smVSInfo[i].g_arrMarks[0].Get2DCodeResult(), "Unit_No_" + m_smVSInfo[i].g_intTestedTotal.ToString(), false);
                        objFile.WriteEndElement();
                        return;
                    default:
                        break;
                }
            }
            #endregion
        }

    }
}
