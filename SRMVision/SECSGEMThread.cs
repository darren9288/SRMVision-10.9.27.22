using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using SharedMemory;
using Common;
using System.IO;
using VisionProcessing;

namespace SRMVision
{
    public class SECSGEMThread
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
        private string m_strSECSGEMFileName = "SECSGEM(Temp).xml";
        private bool m_blnBottomInited = false;
        private bool m_blnUnitPresentInited = false;
        private bool m_blnMarkOrientInited = false;
        private bool m_blnInPocketInited = false;
        private bool m_blnCoplanPadInited = false;
        private bool m_blnSealInited = false;
        private bool m_blnSECSGEMFileExist = false;
        private int m_intSECSGEMMaxNoOfCoplanPad = 0;

        #endregion

        public SECSGEMThread(ProductionInfo smProductionInfo, VisionInfo[] smVSInfo, CustomOption smCustomizeInfo)
        {
            m_smProductionInfo = smProductionInfo;
            m_smVSInfo = smVSInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            LoadMaxNoOfCoplanPadInfo();

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
                    if (m_smCustomizeInfo.g_blnWantSECSGEM)
                    {
                        if (m_smProductionInfo.g_blnSECSGEMSInit)
                        {
                            m_smProductionInfo.g_blnSECSGEMSInit = false;
                            DeleteTempSECSGEMFile();
                            CheckSECSGEMFileExist();

                            Thread.Sleep(50);

                            m_strSECSGEMFileName = "SECSGEM(Temp).xml";

                            m_blnBottomInited = false;
                            m_blnUnitPresentInited = false;
                            m_blnMarkOrientInited = false;
                            m_blnCoplanPadInited = false;
                            m_blnInPocketInited = false;
                            m_blnSealInited = false;
                            for (int i = 0; i < m_smVSInfo.Length; i++)
                            {
                                if (m_smVSInfo[i] == null)
                                    continue;

                                UpdateSECSGEMFile(m_smVSInfo[i]);
                            }

                            //If current vision does not use either of the vision station, also need to init the value as NA
                            if (!m_blnBottomInited)
                            {
                                m_blnBottomInited = true;
                                string strPath = m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\" + m_strSECSGEMFileName;
                                string strVisionName = "Bottom";
                                VisionInfo smVisionInfo = new VisionInfo();

                                SaveYieldSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                                SaveGeneralSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                                SaveGaugeM4LSetting_SECSGEM(strPath, "MOGaugeM4L", smVisionInfo.g_arrOrientGaugeM4L, strVisionName);

                                SaveOrientSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                                SaveCalibrationSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
                                //ROI.LoadFile(strFolderPath + "Positioning\\ROI.xml", smVisionInfo.g_arrPositioningROIs);
                                LGauge.SaveFile_SECSGEM(strPath, smVisionInfo.g_arrPositioningGauges, strVisionName, 4, m_blnSECSGEMFileExist);

                                if (smVisionInfo.g_objPositioning != null)
                                    smVisionInfo.g_objPositioning.SavePosition_SECSGEM(strPath, "Position", strVisionName);
                                else
                                {
                                    if (!m_blnSECSGEMFileExist)
                                    {
                                        XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
                                        objFile.WriteRootElement("SECSGEMData");

                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionImageIndex", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyImageIndex", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinBorderScore", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinEmptyScore", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_AngleLimit", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionXTolerance", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionYTolerance", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieWidth", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieHeight", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_Method", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PRSMode", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue2", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateX", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateY", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipThreshold", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipAreaLimit", "NA");
                                        //Empty
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyThreshold", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyWhiteArea", "NA");
                                        //PH
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHThreshold", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHBlackArea", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHMinArea", "NA");

                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_MinScore", "NA");
                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionX", "NA");
                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionY", "NA");
                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PosTolerance", "NA");
                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_Direction", "NA");

                                        objFile.WriteEndElement();
                                    }
                                }
                            }

                            if (!m_blnUnitPresentInited)
                            {
                                m_blnUnitPresentInited = true;
                                string strPath = m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\" + m_strSECSGEMFileName;
                                string strVisionName = "UnitPresent";
                                VisionInfo smVisionInfo = new VisionInfo();

                                //ROI.LoadFile(strFolderPath + "CheckPresent\\ROI.xml", smVisionInfo.g_arrPositioningROIs);
                                if (smVisionInfo.g_objUnitPresent != null)
                                    smVisionInfo.g_objUnitPresent.SaveUnitPresent_SECSGEM(strPath, "UnitPresentSetting", strVisionName);
                                else
                                {
                                    if (!m_blnSECSGEMFileExist)
                                    {
                                        XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
                                        objFile.WriteRootElement("SECSGEMData");

                                        // Rectangle gauge template measurement result
                                        objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_WhiteOnBlack", "NA");
                                        objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_ThresholdValue", "NA");
                                        objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_FilterMinArea", "NA");
                                        objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_FilterMaxArea", "NA");
                                        objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_TotalTemplateBlobObject", "NA");
                                        objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_HalfPitch", "NA");
                                        objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_UnitROICountX", "NA");
                                        objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_UnitROICountY", "NA");

                                        //for (int i = 0; i < 10; i++)
                                        //{
                                        //    //objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i, "");
                                        //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_LimitCenterX", "NA");
                                        //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_LimitCenterY", "NA");
                                        //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_Width", "NA");
                                        //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_Height", "NA");
                                        //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_Area", "NA");
                                        //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_StartX", "NA");
                                        //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_StartY", "NA");
                                        //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_EndX", "NA");
                                        //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_EndY", "NA");
                                        //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_MinArea", "NA");
                                        //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_MinOffSet", "NA");
                                        //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_ROIStartX", "NA");
                                        //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_ROIStartY", "NA");
                                        //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_ROIWidth", "NA");
                                        //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_ROIHeight", "NA");
                                        //}

                                        objFile.WriteEndElement();
                                    }
                                }
                            }

                            if (!m_blnMarkOrientInited)
                            {
                                m_blnMarkOrientInited = true;
                                string strPath = m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\" + m_strSECSGEMFileName;
                                string strVisionName = "MarkOrient";
                                VisionInfo smVisionInfo = new VisionInfo();

                                SaveYieldSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                                SaveGeneralSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                                //orient
                                SaveGaugeM4LSetting_SECSGEM(strPath, "MOGaugeM4L", smVisionInfo.g_arrOrientGaugeM4L, strVisionName);

                                SaveOrientSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                                //ROI.LoadFile(strFolderPath + "Mark\\ROI.xml", smVisionInfo.g_arrMarkROIs);

                                //Mark
                                SaveMarkSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                                SaveGaugeM4LSetting_SECSGEM(strPath, "MOGaugeM4L", smVisionInfo.g_arrMarkGaugeM4L, strVisionName);

                                SaveControlSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                                //Pkg
                                SaveCalibrationSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
                                //ROI.LoadFile(strFolderPath + "Package\\ROI.xml", smVisionInfo.g_arrPackageROIs);

                                SaveGaugeM4LSetting_SECSGEM(strPath, "PkgGaugeM4L", smVisionInfo.g_arrPackageGaugeM4L, strVisionName);

                                SaveGaugeM4LSetting_SECSGEM(strPath, "PkgGauge2M4L", smVisionInfo.g_arrPackageGauge2M4L, strVisionName);

                                SavePackageSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                                //if (smVisionInfo.g_arrPolygon_Package != null)
                                //    LoadPolygonSetting_Package(strFolderPath + "Package\\Template\\Polygon.xml");

                                //Lead
                                //LoadROISetting(strFolderPath + "Lead\\ROI.xml", smVisionInfo.g_arrLeadROIs, smVisionInfo.g_arrLead.Length);
                                SaveLeadSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                                //Position
                                //ROI.LoadFile(strFolderPath + "Positioning\\ROI.xml", smVisionInfo.g_arrPositioningROIs);
                                LGauge.SaveFile_SECSGEM(strPath, smVisionInfo.g_arrPositioningGauges, strVisionName, 4, m_blnSECSGEMFileExist);

                                if (smVisionInfo.g_objPositioning != null)
                                    smVisionInfo.g_objPositioning.SavePosition_SECSGEM(strPath, "Position", strVisionName);
                                else
                                {
                                    if (!m_blnSECSGEMFileExist)
                                    {
                                        XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
                                        objFile.WriteRootElement("SECSGEMData");

                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionImageIndex", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyImageIndex", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinBorderScore", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinEmptyScore", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_AngleLimit", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionXTolerance", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionYTolerance", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieWidth", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieHeight", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_Method", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PRSMode", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue2", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateX", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateY", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipThreshold", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipAreaLimit", "NA");
                                        //Empty
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyThreshold", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyWhiteArea", "NA");
                                        //PH
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHThreshold", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHBlackArea", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHMinArea", "NA");

                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_MinScore", "NA");
                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionX", "NA");
                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionY", "NA");
                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PosTolerance", "NA");
                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_Direction", "NA");

                                        objFile.WriteEndElement();
                                    }
                                }
                            }

                            if (!m_blnCoplanPadInited)
                            {
                                m_blnCoplanPadInited = true;
                                string strPath = m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\" + m_strSECSGEMFileName;
                                string strVisionName = "CoplanPad";
                                VisionInfo smVisionInfo = new VisionInfo();

                                SaveYieldSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                                //Pad
                                SaveCalibrationSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                                //LoadROISetting(strFolderPath + "Pad\\ROI.xml", smVisionInfo.g_arrPadROIs, smVisionInfo.g_arrPad.Length);

                                SavePadSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                                SaveControlSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                                //Lead3D
                                //LoadROISetting(strFolderPath + "Lead3D\\ROI.xml", smVisionInfo.g_arrLeadROIs, smVisionInfo.g_arrLead3D.Length);
                                SaveLead3DSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                                //Position
                                //ROI.LoadFile(strFolderPath + "Positioning\\ROI.xml", smVisionInfo.g_arrPositioningROIs);
                                LGauge.SaveFile_SECSGEM(strPath, smVisionInfo.g_arrPositioningGauges, strVisionName, 12, m_blnSECSGEMFileExist);

                                if (smVisionInfo.g_objPositioning != null)
                                    smVisionInfo.g_objPositioning.SavePosition_SECSGEM(strPath, "Position", strVisionName);
                                else
                                {
                                    if (!m_blnSECSGEMFileExist)
                                    {
                                        XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
                                        objFile.WriteRootElement("SECSGEMData");

                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionImageIndex", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyImageIndex", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinBorderScore", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinEmptyScore", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_AngleLimit", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionXTolerance", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionYTolerance", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieWidth", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieHeight", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_Method", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PRSMode", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue2", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateX", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateY", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipThreshold", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipAreaLimit", "NA");
                                        //Empty
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyThreshold", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyWhiteArea", "NA");
                                        //PH
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHThreshold", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHBlackArea", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHMinArea", "NA");

                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_MinScore", "NA");
                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionX", "NA");
                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionY", "NA");
                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PosTolerance", "NA");
                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_Direction", "NA");

                                        objFile.WriteEndElement();
                                    }
                                }
                            }

                            if (!m_blnInPocketInited)
                            {
                                m_blnInPocketInited = true;
                                string strPath = m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\" + m_strSECSGEMFileName;
                                string strVisionName = "InPocket";
                                VisionInfo smVisionInfo = new VisionInfo();

                                SaveYieldSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                                //Orient
                                SaveGeneralSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                                SaveGaugeM4LSetting_SECSGEM(strPath, "MOGaugeM4L", smVisionInfo.g_arrOrientGaugeM4L, strVisionName);

                                SaveOrientSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                                //Mark
                                //ROI.LoadFile(strFolderPath + "Mark\\ROI.xml", smVisionInfo.g_arrMarkROIs);
                                SaveMarkSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                                SaveGaugeM4LSetting_SECSGEM(strPath, "MOGaugeM4L", smVisionInfo.g_arrMarkGaugeM4L, strVisionName);

                                SaveControlSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                                //Package
                                SaveCalibrationSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
                                //ROI.LoadFile(strFolderPath + "Package\\ROI.xml", smVisionInfo.g_arrPackageROIs);

                                SaveGaugeM4LSetting_SECSGEM(strPath, "PkgGaugeM4L", smVisionInfo.g_arrPackageGaugeM4L, strVisionName);

                                SaveGaugeM4LSetting_SECSGEM(strPath, "PkgGauge2M4L", smVisionInfo.g_arrPackageGauge2M4L, strVisionName);

                                SavePackageSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                                //if (smVisionInfo.g_arrPolygon_Package != null)
                                //    LoadPolygonSetting_Package(strFolderPath + "Package\\Template\\Polygon.xml");

                                //Lead
                                //LoadROISetting(strFolderPath + "Lead\\ROI.xml", smVisionInfo.g_arrLeadROIs, smVisionInfo.g_arrLead.Length);
                                SaveLeadSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                                //Position
                                //ROI.LoadFile(strFolderPath + "Positioning\\ROI.xml", smVisionInfo.g_arrPositioningROIs);
                                LGauge.SaveFile_SECSGEM(strPath, smVisionInfo.g_arrPositioningGauges, strVisionName, 4, m_blnSECSGEMFileExist);

                                if (smVisionInfo.g_objPositioning != null)
                                    smVisionInfo.g_objPositioning.SavePosition_SECSGEM(strPath, "Position", strVisionName);
                                else
                                {
                                    if (!m_blnSECSGEMFileExist)
                                    {
                                        XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
                                        objFile.WriteRootElement("SECSGEMData");

                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionImageIndex", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyImageIndex", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinBorderScore", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinEmptyScore", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_AngleLimit", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionXTolerance", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionYTolerance", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieWidth", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieHeight", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_Method", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PRSMode", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue2", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateX", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateY", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipThreshold", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipAreaLimit", "NA");
                                        //Empty
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyThreshold", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyWhiteArea", "NA");
                                        //PH
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHThreshold", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHBlackArea", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHMinArea", "NA");

                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_MinScore", "NA");
                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionX", "NA");
                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionY", "NA");
                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PosTolerance", "NA");
                                        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_Direction", "NA");

                                        objFile.WriteEndElement();
                                    }
                                }
                            }

                            if (!m_blnSealInited)
                            {
                                m_blnSealInited = true;
                                string strPath = m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\" + m_strSECSGEMFileName;
                                string strVisionName = "Seal";
                                VisionInfo smVisionInfo = new VisionInfo();

                                SaveYieldSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                                SaveCalibrationSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                                SaveSealGeneralSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                                SaveSealSettings_SECSGEM(smVisionInfo, strPath, strVisionName);
                                //ROI.LoadFile(strFolderPath + "Seal\\ROI.xml", smVisionInfo.g_arrSealROIs);
                                LGauge.SaveFile_SECSGEM(strPath, smVisionInfo.g_arrSealGauges, strVisionName, 50, m_blnSECSGEMFileExist);

                                if (smVisionInfo.g_objSeal != null)
                                    smVisionInfo.g_objSeal.SaveSeal_SECSGEM(strPath, "SealSettings", strVisionName, smVisionInfo.g_fCalibPixelX);
                                else
                                {
                                    if (!m_blnSECSGEMFileExist)
                                    {
                                        XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
                                        objFile.WriteRootElement("SECSGEMData");

                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_BuildObjectLength", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_Seal1Threshold", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_Seal2Threshold", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_OverHeatThreshold", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_DistanceThreshold", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_Seal1MinArea", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_Seal2MinArea", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_OverHeatMinArea", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_SealHoleMinArea1", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_SealHoleMinArea2", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_MinBrokenWidth", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_PositionCenterX", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_PositionCenterY", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthLowerTolerance", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthLowerTolerance1", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthLowerTolerance2", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthUpperTolerance", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthUpperTolerance1", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthUpperTolerance2", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_DistanceMinTolerance", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_DistanceMaxTolerance", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_ShiftPositionTolerance", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_SealScoreTolerance", "NA");

                                        for (int i = 0; i < 2; i++)
                                        {
                                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_TemplateSealArea" + i, "NA");
                                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_SealAreaTolerance" + i, "NA");
                                        }
                                        for (int i = 0; i < 3; i++)
                                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_TemplateSealLineWidth" + i, "NA");

                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_PocketMinScore", "NA");
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_MarkMinScore", "NA");

                                        // Grab image index
                                        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_GrabImageIndexCount", "NA");
                                        for (int j = 0; j < 5; j++)
                                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_GrabImageIndex" + j.ToString(), "NA");

                                        objFile.WriteEndElement();
                                    }
                                }
                            }

                            m_strSECSGEMFileName = "SECSGEM.xml";

                            if (File.Exists(m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\" + m_strSECSGEMFileName))
                                File.Delete(m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\" + m_strSECSGEMFileName);

                            if (File.Exists(m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\SECSGEM(Temp).xml"))
                                File.Move(m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\SECSGEM(Temp).xml", m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\" + m_strSECSGEMFileName);

                            m_intSECSGEMMaxNoOfCoplanPad = m_smCustomizeInfo.g_intSECSGEMMaxNoOfCoplanPad;
                            SaveMaxNoOfCoplanPadInfo();
                            m_blnSECSGEMFileExist = true;
                        }
                        else
                        {
                            for (int i = 0; i < m_smVSInfo.Length; i++)
                            {
                                if (m_smVSInfo[i] == null)
                                    continue;

                                //Update individual vision
                                if (m_smVSInfo[i].g_blnUpdateSECSGEMFile)
                                {
                                    m_smVSInfo[i].g_blnUpdateSECSGEMFile = false;
                                    UpdateSECSGEMFile(m_smVSInfo[i]);
                                }
                            }
                        }

                    }
                    Thread.Sleep(1);
                }
            }
            finally
            {
                SetStopped();
            }

        }

        private void LoadMaxNoOfCoplanPadInfo()
        {
            XmlParser objFileHandle = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFileHandle.GetFirstSection("SECSGEM");
            m_intSECSGEMMaxNoOfCoplanPad = objFileHandle.GetValueAsInt("SECSGEMMaxNoOfCoplanPadCurrent", 10);
        }

        private void SaveMaxNoOfCoplanPadInfo()
        {
            XmlParser objFileHandle = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFileHandle.WriteSectionElement("SECSGEM");
            objFileHandle.WriteElement1Value("SECSGEMMaxNoOfCoplanPadCurrent", m_intSECSGEMMaxNoOfCoplanPad);
            objFileHandle.WriteEndElement();
        }

        private void SaveGeneralSetting_SECSGEM(VisionInfo smVisionInfo, string strPath, string strVisionName)
        {
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");
            objFile.WriteElementValue(strVisionName + "_TemplateCounting" + "_TotalUnits", smVisionInfo.g_intUnitsOnImage);
            objFile.WriteElementValue(strVisionName + "_TemplateCounting" + "_TotalGroups", smVisionInfo.g_intTotalGroup);
            objFile.WriteElementValue(strVisionName + "_TemplateCounting" + "_TotalTemplates", smVisionInfo.g_intTotalTemplates);
            objFile.WriteElementValue(strVisionName + "_TemplateCounting" + "_TemplateMask", smVisionInfo.g_intTemplateMask);
            objFile.WriteElementValue(strVisionName + "_TemplateCounting" + "_TemplatePriority", smVisionInfo.g_intTemplatePriority);

            switch (smVisionInfo.g_strVisionName)
            {
                case "Orient":
                case "BottomOrient":
                    break;
                case "Mark":
                case "MarkOrient":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                case "MOLi":
                case "Package":
                    break;
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                case "IPMLi":
                case "IPMLiPkg":
                default:
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckEmpty", smVisionInfo.g_blnWantCheckEmpty);
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantUseEmptyPattern", smVisionInfo.g_blnWantUseEmptyPattern);
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantUseEmptyThreshold", smVisionInfo.g_blnWantUseEmptyThreshold);
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantUnitPRFindGauge", smVisionInfo.g_blnWantUseUnitPRFindGauge);
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckUnitSitProper", smVisionInfo.g_blnWantCheckUnitSitProper);
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_UseAutoRepalceCounter", smVisionInfo.g_blnUseAutoRepalceCounter);
                    break;
            }

            objFile.WriteEndElement();
        }

        private void SaveYieldSetting_SECSGEM(VisionInfo smVisionInfo, string strPath, string strVisionName)
        {
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");
            objFile.WriteElementValue(strVisionName + "_Yield" + "_StopLowYield", smVisionInfo.g_blnStopLowYield);
            objFile.WriteElementValue(strVisionName + "_Yield" + "_StopContinuousPass", smVisionInfo.g_blnStopContinuousPass);
            objFile.WriteElementValue(strVisionName + "_Yield" + "_StopContinuousFail", smVisionInfo.g_blnStopContinuousFail);
            objFile.WriteElementValue(strVisionName + "_Yield" + "_LowYield", smVisionInfo.g_fLowYield);
            objFile.WriteElementValue(strVisionName + "_Yield" + "_MinUnitCheck", smVisionInfo.g_intMinUnitCheck);
            objFile.WriteElementValue(strVisionName + "_Yield" + "_MinPassUnit", smVisionInfo.g_intMinPassUnit);
            objFile.WriteElementValue(strVisionName + "_Yield" + "_MinFailUnit", smVisionInfo.g_intMinFailUnit);
            objFile.WriteEndElement();
        }

        private void SaveGaugeM4LSetting_SECSGEM(string strPath, string strSectionName, List<RectGaugeM4L> arrGauge, string strVisionName)
        {
            if (arrGauge != null)
            {
                for (int j = 0; j < arrGauge.Count; j++)
                {
                    arrGauge[j].SaveRectGauge4L_SECSGEM(strPath, strSectionName + j, strVisionName);
                }
            }
            else
            {
                if (!m_blnSECSGEMFileExist)
                {
                    XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
                    objFile.WriteRootElement("SECSGEMData");

                    objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_GainValue", "NA");
                    objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_UnitSizeWidth", "NA");
                    objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_UnitSizeHeight", "NA");
                    objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_Gauge4LCenterPointX", "NA");
                    objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_Gauge4LCenterPointY", "NA");
                    objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_InToOut", "NA");

                    for (int i = 0; i < 12; i++)
                    {
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_CenterX", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_CenterY", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_Length", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_Angle", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_Tolerance", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_TransType", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_TransChoice", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_Threshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_Thickness", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_MinAmp", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_MinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_Filter", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_SamplingStep", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_FilteringThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_FilteringPasses", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeMinScore", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeMaxAngle", "NA");
                    }

                    //for (int i = 0; i < 4; i++)
                    //{
                    //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection0" + "_EdgeROI_" + i + "_PositionX", "NA");
                    //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection0" + "_EdgeROI_" + i + "_PositionY", "NA");
                    //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection0" + "_EdgeROI_" + i + "_Width", "NA");
                    //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection0" + "_EdgeROI_" + i + "_Height", "NA");
                    //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection0" + "_EdgeROI_" + i + "_CenterX", "NA");
                    //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection0" + "_EdgeROI_" + i + "_CenterY", "NA");
                    //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection0" + "_EdgeROI_" + i + "_DontCareROICount", "NA");
                    //}

                    objFile.WriteEndElement();
                }
            }
        }

        private void SaveOrientSettings_SECSGEM(VisionInfo smVisionInfo, string strPath, string strVisionName)
        {
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");

            if (smVisionInfo.g_arrOrients != null)
            {
                ////units on image
                //for (int j = 0; j < 2; j++)
                //{
                if (smVisionInfo.g_arrOrients.Count > 0)
                {
                    if (smVisionInfo.g_arrOrients[0].Count > 0)
                    {
                        //objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_TemplateCenterX", smVisionInfo.g_arrOrients[0][0].ref_fTemplateX);
                        //objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_TemplateCenterY", smVisionInfo.g_arrOrients[0][0].ref_fTemplateY);
                        objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_MaxX", smVisionInfo.g_arrOrients[0][0].ref_fXTolerance);
                        objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_MaxY", smVisionInfo.g_arrOrients[0][0].ref_fYTolerance);
                        objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_MinScore", smVisionInfo.g_arrOrients[0][0].ref_fMinScore);
                        objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_MaxAngle", smVisionInfo.g_arrOrients[0][0].ref_fAngleTolerance);
                        objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_SubMatcherCount", smVisionInfo.g_arrOrientROIs[0].Count - 2);
                        objFile.WriteElementValue(strVisionName + "_OrientAdvSetting_Template0" + "_Direction", smVisionInfo.g_arrOrients[0][0].ref_intDirections);

                        smVisionInfo.g_arrOrients[0][0].SaveOrient_SECSGEM(strPath, "Orient_General", strVisionName);
                    }
                    else
                    {
                        if (!m_blnSECSGEMFileExist)
                        {
                            //objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_TemplateCenterX", "NA");
                            //objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_TemplateCenterY", "NA");
                            objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_MaxX", "NA");
                            objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_MaxY", "NA");
                            objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_MinScore", "NA");
                            objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_MaxAngle", "NA");
                            objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_SubMatcherCount", "NA");
                            objFile.WriteElementValue(strVisionName + "_OrientAdvSetting_Template0" + "_Direction", "NA");

                            objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_RotatedAngle", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_CorrectAngleMethod", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_EmptyThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_EmptyMaxArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_MatcherOffSetCenterX", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_MatcherOffSetCenterY", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_UnitSurfaceOffsetX", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_UnitSurfaceOffsetY", "NA");
                        }
                    }
                }
                else
                {
                    if (!m_blnSECSGEMFileExist)
                    {
                        //objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_TemplateCenterX", "NA");
                        //objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_TemplateCenterY", "NA");
                        objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_MaxX", "NA");
                        objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_MaxY", "NA");
                        objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_MinScore", "NA");
                        objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_MaxAngle", "NA");
                        objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_SubMatcherCount", "NA");
                        objFile.WriteElementValue(strVisionName + "_OrientAdvSetting_Template0" + "_Direction", "NA");

                        objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_RotatedAngle", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_CorrectAngleMethod", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_EmptyThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_EmptyMaxArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_MatcherOffSetCenterX", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_MatcherOffSetCenterY", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_UnitSurfaceOffsetX", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_UnitSurfaceOffsetY", "NA");
                    }
                }
                //}
            }
            else
            {
                if (!m_blnSECSGEMFileExist)
                {
                    //for (int j = 0; j < 2; j++)
                    ////{
                    //objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_TemplateCenterX", "NA");
                    //objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_TemplateCenterY", "NA");
                    objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_MaxX", "NA");
                    objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_MaxY", "NA");
                    objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_MinScore", "NA");
                    objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_MaxAngle", "NA");
                    objFile.WriteElementValue(strVisionName + "_Orient_Template0" + "_SubMatcherCount", "NA");
                    objFile.WriteElementValue(strVisionName + "_OrientAdvSetting_Template0" + "_Direction", "NA");

                    objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_RotatedAngle", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_CorrectAngleMethod", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_EmptyThreshold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_EmptyMaxArea", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_MatcherOffSetCenterX", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_MatcherOffSetCenterY", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_UnitSurfaceOffsetX", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "Orient_General" + "_UnitSurfaceOffsetY", "NA");
                    //}
                }
            }

            objFile.WriteEndElement();

            if (smVisionInfo.g_arrPin1 != null)
            {
                ////units on image
                //for (int u = 0; u < 2; u++)
                //{
                if (smVisionInfo.g_arrPin1.Count > 0)
                    smVisionInfo.g_arrPin1[0].SaveTemplate_SECSGEM(strPath, "_Pin1Settings", strVisionName, m_blnSECSGEMFileExist);
                else
                {
                    if (!m_blnSECSGEMFileExist)
                    {
                        objFile.WriteElementValue(strVisionName + "_Pin1Settings_Pin1TemplateNum", "NA");

                        //Max template is 8
                        for (int j = 0; j < 8; j++)
                        {
                            objFile.WriteElementValue(strVisionName + "_Pin1Settings_Pin1Template" + j + "_WantCheckPin1", "NA");
                            objFile.WriteElementValue(strVisionName + "_Pin1Settings_Pin1Template" + j + "_OffsetRefPosX", "NA");
                            objFile.WriteElementValue(strVisionName + "_Pin1Settings_Pin1Template" + j + "_OffsetRefPosY", "NA");
                            objFile.WriteElementValue(strVisionName + "_Pin1Settings_Pin1Template" + j + "_MinScore", "NA");
                        }

                        //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIStartX", "NA");
                        //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIStartY", "NA");
                        //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIWidth", "NA");
                        //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIHeight", "NA");

                        //if (m_objPin1ROI.ref_ROI != null)
                        //{
                        //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIStartX", "NA");
                        //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIStartY", "NA");
                        //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIWidth", "NA");
                        //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIHeight", "NA");
                        //}
                    }
                }
                //}
            }
            else
            {
                if (!m_blnSECSGEMFileExist)
                {
                    ////units on image
                    //for (int u = 0; u < 2; u++)
                    //{
                    objFile.WriteElementValue(strVisionName + "_Pin1Settings_Pin1TemplateNum", "NA");

                    //Max template is 8
                    for (int j = 0; j < 8; j++)
                    {
                        objFile.WriteElementValue(strVisionName + "_Pin1Settings_Pin1Template" + j + "_WantCheckPin1", "NA");
                        objFile.WriteElementValue(strVisionName + "_Pin1Settings_Pin1Template" + j + "_OffsetRefPosX", "NA");
                        objFile.WriteElementValue(strVisionName + "_Pin1Settings_Pin1Template" + j + "_OffsetRefPosY", "NA");
                        objFile.WriteElementValue(strVisionName + "_Pin1Settings_Pin1Template" + j + "_MinScore", "NA");
                    }

                    //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIStartX", "NA");
                    //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIStartY", "NA");
                    //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIWidth", "NA");
                    //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIHeight", "NA");

                    //if (m_objPin1ROI.ref_ROI != null)
                    //{
                    //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIStartX", "NA");
                    //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIStartY", "NA");
                    //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIWidth", "NA");
                    //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIHeight", "NA");
                    //}
                    //}
                }
            }
        }

        private void SaveMarkSettings_SECSGEM(VisionInfo smVisionInfo, string strPath, string strVisionName)
        {
            //Save SECSGEM
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");

            if (smVisionInfo.g_arrMarks != null)
            {
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WhiteOnBlack", smVisionInfo.g_blnWhiteOnBlack);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantMultiGroups", smVisionInfo.g_blnWantMultiGroups);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantBuildTexts", smVisionInfo.g_blnWantBuildTexts);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantMultiTemplates", smVisionInfo.g_blnWantMultiTemplates);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantSetTemplateBasedOnBinInfo", smVisionInfo.g_blnWantSetTemplateBasedOnBinInfo);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantSet1ToAll", smVisionInfo.g_blnWantSet1ToAll);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantSkipMark", smVisionInfo.g_blnWantSkipMark);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantPin1", smVisionInfo.g_blnWantPin1);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantGaugeMeasureMarkDimension", smVisionInfo.g_blnWantGauge);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantClearMarkTemplateWhenNewLot", smVisionInfo.g_blnWantClearMarkTemplateWhenNewLot);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckNoMark", smVisionInfo.g_blnWantCheckNoMark);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckContourOnMark", smVisionInfo.g_blnWantCheckContourOnMark); 
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantMark2DCode", smVisionInfo.g_blnWantMark2DCode);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantDontCareAreaMark", smVisionInfo.g_blnWantDontCareArea_Mark);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_CodeType", smVisionInfo.g_int2DCodeType);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_DefaultMarkScore", smVisionInfo.g_intDefaultMarkScore);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_MinMarkScore", smVisionInfo.g_intMinMarkScore);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_MaxMarkTemplate", smVisionInfo.g_intMaxMarkTemplate);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_NoMarkMaximumBlob", smVisionInfo.g_arrMarks[0].ref_fNoMarkMaximumBlobArea);

                //for (int u = 0; u < 2; u++)
                //{
                    if (smVisionInfo.g_arrMarks.Count > 0)
                        smVisionInfo.g_arrMarks[0].SaveTemplate_SECSGEM(strPath, "_MarkSettings", strVisionName, m_blnSECSGEMFileExist);
                    else
                    {
                    if (!m_blnSECSGEMFileExist)
                    {
                        objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_CheckMark", "NA");
                        objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_FailOptionMask", "NA");
                        objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_MinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_MaxArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_CharDilateHalfWidth", "NA");
                        objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_CharErodeHalfWidth", "NA");
                        objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_LearnMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_TemplateMask", "NA");
                        objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_TemplatePriority", "NA");
                        objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_WhiteOnBlack", "NA");

                        for (int i = 0; i < 1; i++)
                        {
                            objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_RefChar", "NA");
                            objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_TemplateNum", "NA");

                            // Max no of template is 8
                            for (int j = 0; j < 8; j++)
                            {
                                // Save tolerance settting
                                //objFile.WriteElementValue(strVisionName + "_Template" + j, "");
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_FailOptionMask", "NA");
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_BrokenSize", "NA");
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharShiftXY", "NA");
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_ExtraMinArea", "NA");
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_GroupExtraMinArea", "NA");
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_ExcessMinArea", "NA");
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_MissingMinArea", "NA");
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TextMinScore", "NA");
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_Threshold", "NA");
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_UnCheckAreaBottom", "NA");
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_UnCheckAreaLeft", "NA");
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_UnCheckAreaRight", "NA");
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_UnCheckAreaTop", "NA");
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharNum", "NA");

                                // Save Char Score Setting
                                //Prepare at least 10 character to save in secsgem, if less than 10 will save as "NA"
                                int intCharNo = 10;
                                for (int k = 0; k < intCharNo; k++)
                                {
                                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharSetting" + k, "NA");
                                }

                                //objFile.WriteElement2Value(strVisionName + "_MaxExcessArea", "");

                                for (int k = 0; k < intCharNo; k++)
                                {
                                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_MaxExcessArea" + k, "NA");
                                }

                                //objFile.WriteElement2Value(strVisionName + "_MaxBrokenArea", "");
                                for (int k = 0; k < intCharNo; k++)
                                {
                                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_MaxBrokenArea" + k, "NA");
                                }

                                //objFile.WriteElement2Value(strVisionName + "_CharShiftX", "");
                                for (int k = 0; k < intCharNo; k++)
                                {
                                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharShiftX" + k, "NA");
                                }

                                //objFile.WriteElement2Value(strVisionName + "_CharShiftY", "");
                                for (int k = 0; k < intCharNo; k++)
                                {
                                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharShiftY" + k, "NA");
                                }

                                //objFile.WriteElement2Value(strVisionName + "_TextCharOffsetX", "");
                                for (int k = 0; k < intCharNo; k++)
                                {
                                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TextCharOffsetX" + k, "NA");
                                }

                                //objFile.WriteElement2Value(strVisionName + "_TextCharOffsetY", "");
                                for (int k = 0; k < intCharNo; k++)
                                {
                                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TextCharOffsetY" + k, "NA");
                                }

                                //objFile.WriteElement2Value(strVisionName + "_EnableMark", "");
                                for (int k = 0; k < intCharNo; k++)
                                {
                                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_EnableMark" + k, "NA");
                                }


                                //// Save Char Represent Number
                                ////objFile.WriteElement2Value(strVisionName + "_CharNo", "");
                                //for (int k = 0; k < intCharNo; k++)
                                //{
                                //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharNo" + k, "NA");
                                //}

                                //objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TextNum", "NA");

                                //objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TemplateCharOffSetNum", "NA");

                                //for (int k = 0; k < intCharNo; k++)
                                //{
                                //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharOffSetX" + k, "NA");
                                //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharOffSetY" + k, "NA");

                                //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharStartX" + k, "NA");
                                //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharStartY" + k, "NA");

                                //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharEndX" + k, "NA");
                                //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharEndY" + k, "NA");

                                //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharWidth" + k, "NA");
                                //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharHeight" + k, "NA");
                                //}

                                //objFile.WriteElementValue(strVisionName + "_Group" + i + "_Template" + j + "_TextROI", "");
                                //for (int tx = 0; tx < intNumTexts; tx++)
                                //{
                                //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TextROIStartX" + tx, m_arrTemplateSetting[i][j].arrTemplateTextROI[tx].ref_ROIPositionX);
                                //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TextROIStartY" + tx, m_arrTemplateSetting[i][j].arrTemplateTextROI[tx].ref_ROIPositionY);
                                //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TextROIWidth" + tx, m_arrTemplateSetting[i][j].arrTemplateTextROI[tx].ref_ROIWidth);
                                //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TextROIHeight" + tx, m_arrTemplateSetting[i][j].arrTemplateTextROI[tx].ref_ROIHeight);
                                //}
                            }
                        }
                        //}
                    }
                }
            }
            else
            {
                if (!m_blnSECSGEMFileExist)
                {
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WhiteOnBlack", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantMultiGroups", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantBuildTexts", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantMultiTemplates", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantSetTemplateBasedOnBinInfo", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantSet1ToAll", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantSkipMark", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantPin1", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantGaugeMeasureMarkDimension", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantClearMarkTemplateWhenNewLot", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckNoMark", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantMark2DCode", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantDontCareAreaMark", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_CodeType", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_DefaultMarkScore", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_MinMarkScore", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_MaxMarkTemplate", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_NoMarkMaximumBlob", "NA");

                    //for (int u = 0; u < 2; u++)
                    //{
                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_CheckMark", "NA");
                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_FailOptionMask", "NA");
                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_MinArea", "NA");
                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_MaxArea", "NA");
                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_CharDilateHalfWidth", "NA");
                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_CharErodeHalfWidth", "NA");
                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_LearnMinArea", "NA");
                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_TemplateMask", "NA");
                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_TemplatePriority", "NA");
                    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_WhiteOnBlack", "NA");

                    for (int i = 0; i < 1; i++)
                    {
                        objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_RefChar", "NA");
                        objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_TemplateNum", "NA");

                        // Max no of template is 8
                        for (int j = 0; j < 8; j++)
                        {
                            // Save tolerance settting
                            //objFile.WriteElementValue(strVisionName + "_Template" + j, "");
                            objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_FailOptionMask", "NA");
                            objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_BrokenSize", "NA");
                            objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharShiftXY", "NA");
                            objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_ExtraMinArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_GroupExtraMinArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_ExcessMinArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_MissingMinArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TextMinScore", "NA");
                            objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_Threshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_UnCheckAreaBottom", "NA");
                            objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_UnCheckAreaLeft", "NA");
                            objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_UnCheckAreaRight", "NA");
                            objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_UnCheckAreaTop", "NA");
                            objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharNum", "NA");

                            // Save Char Score Setting
                            //Prepare at least 10 character to save in secsgem, if less than 10 will save as "NA"
                            int intCharNo = 10;
                            for (int k = 0; k < intCharNo; k++)
                            {
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharSetting" + k, "NA");
                            }

                            //objFile.WriteElement2Value(strVisionName + "_MaxExcessArea", "");

                            for (int k = 0; k < intCharNo; k++)
                            {
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_MaxExcessArea" + k, "NA");
                            }

                            //objFile.WriteElement2Value(strVisionName + "_MaxBrokenArea", "");
                            for (int k = 0; k < intCharNo; k++)
                            {
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_MaxBrokenArea" + k, "NA");
                            }

                            //objFile.WriteElement2Value(strVisionName + "_CharShiftX", "");
                            for (int k = 0; k < intCharNo; k++)
                            {
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharShiftX" + k, "NA");
                            }

                            //objFile.WriteElement2Value(strVisionName + "_CharShiftY", "");
                            for (int k = 0; k < intCharNo; k++)
                            {
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharShiftY" + k, "NA");
                            }

                            //objFile.WriteElement2Value(strVisionName + "_TextCharOffsetX", "");
                            for (int k = 0; k < intCharNo; k++)
                            {
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TextCharOffsetX" + k, "NA");
                            }

                            //objFile.WriteElement2Value(strVisionName + "_TextCharOffsetY", "");
                            for (int k = 0; k < intCharNo; k++)
                            {
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TextCharOffsetY" + k, "NA");
                            }

                            //objFile.WriteElement2Value(strVisionName + "_EnableMark", "");
                            for (int k = 0; k < intCharNo; k++)
                            {
                                objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_EnableMark" + k, "NA");
                            }


                            //// Save Char Represent Number
                            ////objFile.WriteElement2Value(strVisionName + "_CharNo", "");
                            //for (int k = 0; k < intCharNo; k++)
                            //{
                            //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharNo" + k, "NA");
                            //}

                            //objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TextNum", "NA");

                            //objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TemplateCharOffSetNum", "NA");

                            //for (int k = 0; k < intCharNo; k++)
                            //{
                            //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharOffSetX" + k, "NA");
                            //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharOffSetY" + k, "NA");

                            //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharStartX" + k, "NA");
                            //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharStartY" + k, "NA");

                            //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharEndX" + k, "NA");
                            //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharEndY" + k, "NA");

                            //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharWidth" + k, "NA");
                            //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_CharHeight" + k, "NA");
                            //}

                            //objFile.WriteElementValue(strVisionName + "_Group" + i + "_Template" + j + "_TextROI", "");
                            //for (int tx = 0; tx < intNumTexts; tx++)
                            //{
                            //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TextROIStartX" + tx, m_arrTemplateSetting[i][j].arrTemplateTextROI[tx].ref_ROIPositionX);
                            //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TextROIStartY" + tx, m_arrTemplateSetting[i][j].arrTemplateTextROI[tx].ref_ROIPositionY);
                            //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TextROIWidth" + tx, m_arrTemplateSetting[i][j].arrTemplateTextROI[tx].ref_ROIWidth);
                            //    objFile.WriteElementValue(strVisionName + "_MarkSettings" + "_Group" + i + "_Template" + j + "_TextROIHeight" + tx, m_arrTemplateSetting[i][j].arrTemplateTextROI[tx].ref_ROIHeight);
                            //}
                        }
                    }
                    //}
                }
            }

            objFile.WriteEndElement();
        }

        private void SavePackageSettings_SECSGEM(VisionInfo smVisionInfo, string strPath, string strVisionName)
        {
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");

            if (smVisionInfo.g_arrPackage != null)
            {
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantShowGRR", smVisionInfo.g_blnWantShowGRR);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantUseSideLightGauge", smVisionInfo.g_blnWantUseSideLightGauge);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckVoidOnMark", smVisionInfo.g_blnWantCheckVoidOnMarkArea);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantUseDetailThresholdPackage", smVisionInfo.g_blnWantUseDetailThreshold_Package);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckVoidOnMark_SideLight0", smVisionInfo.g_blnWantCheckVoidOnMarkArea_SideLight[0]);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckVoidOnMark_SideLight1", smVisionInfo.g_blnWantCheckVoidOnMarkArea_SideLight[1]);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckVoidOnMark_SideLight2", smVisionInfo.g_blnWantCheckVoidOnMarkArea_SideLight[2]);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantDontCareAreaPackage", smVisionInfo.g_blnWantDontCareArea_Package);

                //for (int u = 0; u < 2; u++)
                //{
                if (smVisionInfo.g_arrPackage.Count > 0)
                {
                    smVisionInfo.g_arrPackage[0].SavePackage_SECSGEM(strPath, "PkgSettings", strVisionName, smVisionInfo.g_fCalibPixelX, smVisionInfo.g_fCalibPixelY);
                }
                else
                {
                    if (!m_blnSECSGEMFileExist)
                    {
                        // Min area
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_MarkViewMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PkgViewMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_CrackViewMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_ChipView1MinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_ChipView2MinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_VoidViewMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_MoldFlashMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_BrightFieldMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_DarkFieldMinArea", "NA");
                        // Threshold
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PackageViewThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_MarkViewHighThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_MarkViewLowThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_CrackViewThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_ChipView1Threshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_ChipView2Threshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_VoidViewThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_MoldFlashThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_CrackViewLowThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_CrackViewHighThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_BrightFieldLowThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_BrightFieldHighThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_DarkFieldLowThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_DarkFieldHighThreshold", "NA");

                        // Template Unit size tolerance
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UnitWidthMin", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UnitWidthMax", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UnitHeightMin", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UnitHeightMax", "NA");

                        //Unit Sitting Tolerance
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_SittingWidthMin", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_SittingWidthMax", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_SittingHeightMin", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_SittingHeightMax", "NA");

                        // Setting
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromEdge", "NA"); //04-03-2019 ZJYeoh : remove conversion(/ fCalibPixelPerMM) to avoid rounding problem 
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromRight", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromBottom", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromLeft", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromEdge_Mold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromRight_Mold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromBottom_Mold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromLeft_Mold", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromEdge_Chip", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromRight_Chip", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromBottom_Chip", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromLeft_Chip", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromEdge_Dark", "NA"); //04-03-2019 ZJYeoh : remove conversion(/ fCalibPixelPerMM) to avoid rounding problem 
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromRight_Dark", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromBottom_Dark", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromLeft_Dark", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromEdge_Chip_Dark", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromRight_Chip_Dark", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromBottom_Chip_Dark", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromLeft_Chip_Dark", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UnitSizeTolerance", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_FailMask", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UseOtherGaugeMeasurePackage", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_SeperateCrackDefectSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UseLinkFunction", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_LinkTolerance", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_SeperateChippedOffDefectSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_SeperateVoidDefectSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_SeperateMoldFlashDefectSetting", "NA");

                        // Unit Surface Offset
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UnitSurfaceOffsetX", "NA");
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UnitSurfaceOffsetY", "NA");

                        // Defect Horizontal Tolerance
                        for (int i = 0; i < 10; i++)
                            objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_DefectHorizontalValue" + i.ToString(), "NA");

                        // Defect Vertical Tolerance
                        for (int i = 0; i < 10; i++)
                            objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_DefectVerticalValue" + i.ToString(), "NA");

                        // Defect Area Toelerance
                        for (int i = 0; i < 10; i++)
                            objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_DefectAreaValue" + i.ToString(), "NA");

                        // Defect Total Area Toelerance
                        for (int i = 0; i < 10; i++)
                            objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_DefectTotalAreaValue" + i.ToString(), "NA");

                        // Want Defect Length
                        for (int i = 0; i < 10; i++)
                            objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_WantDefectLength" + i.ToString(), "NA");

                        // Want Defect Length
                        for (int i = 0; i < 10; i++)
                            objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_WantDefectArea" + i.ToString(), "NA");

                        // Want Inspect Package
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_WantInspectPackage", "NA");

                        // Grab image index
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_GrabImageIndexCount", "NA");

                        for (int j = 0; j < 5; j++)
                        {
                            objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_GrabImageIndex" + j.ToString(), "NA");
                        }
                    }
                }
                //}
            }
            else
            {
                if (!m_blnSECSGEMFileExist)
                {
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantShowGRR", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantUseSideLightGauge", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckVoidOnMark", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantUseDetailThresholdPackage", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantDontCareAreaPackage", "NA");

                    //for (int u = 0; u < 2; u++)
                    //{
                    // Min area
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_MarkViewMinArea", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PkgViewMinArea", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_CrackViewMinArea", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_ChipView1MinArea", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_ChipView2MinArea", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_VoidViewMinArea", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_MoldFlashMinArea", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_BrightFieldMinArea", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_DarkFieldMinArea", "NA");
                    // Threshold
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PackageViewThreshold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_MarkViewHighThreshold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_MarkViewLowThreshold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_CrackViewThreshold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_ChipView1Threshold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_ChipView2Threshold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_VoidViewThreshold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_MoldFlashThreshold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_CrackViewLowThreshold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_CrackViewHighThreshold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_BrightFieldLowThreshold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_BrightFieldHighThreshold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_DarkFieldLowThreshold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_DarkFieldHighThreshold", "NA");

                    // Template Unit size tolerance
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UnitWidthMin", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UnitWidthMax", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UnitHeightMin", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UnitHeightMax", "NA");

                    //Unit Sitting Tolerance
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_SittingWidthMin", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_SittingWidthMax", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_SittingHeightMin", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_SittingHeightMax", "NA");

                    // Setting
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromEdge", "NA"); //04-03-2019 ZJYeoh : remove conversion(/ fCalibPixelPerMM) to avoid rounding problem 
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromRight", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromBottom", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromLeft", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromEdge_Mold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromRight_Mold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromBottom_Mold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromLeft_Mold", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromEdge_Chip", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromRight_Chip", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromBottom_Chip", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromLeft_Chip", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromEdge_Dark", "NA"); //04-03-2019 ZJYeoh : remove conversion(/ fCalibPixelPerMM) to avoid rounding problem 
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromRight_Dark", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromBottom_Dark", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromLeft_Dark", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromEdge_Chip_Dark", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromRight_Chip_Dark", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromBottom_Chip_Dark", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_PixelFromLeft_Chip_Dark", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UnitSizeTolerance", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_FailMask", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UseOtherGaugeMeasurePackage", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_SeperateCrackDefectSetting", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UseLinkFunction", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_LinkTolerance", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_SeperateChippedOffDefectSetting", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_SeperateVoidDefectSetting", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_SeperateMoldFlashDefectSetting", "NA");

                    // Unit Surface Offset
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UnitSurfaceOffsetX", "NA");
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_UnitSurfaceOffsetY", "NA");

                    // Defect Horizontal Tolerance
                    for (int i = 0; i < 10; i++)
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_DefectHorizontalValue" + i.ToString(), "NA");

                    // Defect Vertical Tolerance
                    for (int i = 0; i < 10; i++)
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_DefectVerticalValue" + i.ToString(), "NA");

                    // Defect Area Toelerance
                    for (int i = 0; i < 10; i++)
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_DefectAreaValue" + i.ToString(), "NA");

                    // Defect Total Area Toelerance
                    for (int i = 0; i < 10; i++)
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_DefectTotalAreaValue" + i.ToString(), "NA");

                    // Want Defect Length
                    for (int i = 0; i < 10; i++)
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_WantDefectLength" + i.ToString(), "NA");

                    // Want Defect Length
                    for (int i = 0; i < 10; i++)
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_WantDefectArea" + i.ToString(), "NA");

                    // Want Inspect Package
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_WantInspectPackage", "NA");

                    // Grab image index
                    objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_GrabImageIndexCount", "NA");

                    for (int j = 0; j < 5; j++)
                    {
                        objFile.WriteElementValue(strVisionName + "_" + "PkgSettings" + "_GrabImageIndex" + j.ToString(), "NA");
                    }
                    //}
                }
            }

            objFile.WriteEndElement();
        }

        private void SaveControlSettings_SECSGEM(VisionInfo smVisionInfo, string strPath, string strVisionName)
        {
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");

            objFile.WriteElementValue(strVisionName + "_ControlSetting" + "_ControlMask", smVisionInfo.g_intOptionControlMask);
            objFile.WriteElementValue(strVisionName + "_ControlSetting" + "_ControlMask2", smVisionInfo.g_intOptionControlMask2);
            objFile.WriteElementValue(strVisionName + "_ControlSetting" + "_ControlMask3", smVisionInfo.g_intOptionControlMask3);
            objFile.WriteElementValue(strVisionName + "_ControlSetting" + "_ControlMask4", smVisionInfo.g_intOptionControlMask4);
            objFile.WriteElementValue(strVisionName + "_ControlSetting" + "_ControlMask5", smVisionInfo.g_intOptionControlMask5);
            objFile.WriteElementValue(strVisionName + "_ControlSetting" + "_PkgControlMask", smVisionInfo.g_intPkgOptionControlMask);
            objFile.WriteElementValue(strVisionName + "_ControlSetting" + "_PkgControlMask2", smVisionInfo.g_intPkgOptionControlMask2);
            objFile.WriteElementValue(strVisionName + "_ControlSetting" + "_LeadControlMask", smVisionInfo.g_intLeadOptionControlMask);
            objFile.WriteEndElement();
        }

        private void SavePadSetting_SECSGEM(VisionInfo smVisionInfo, string strPath, string strVisionName)
        {
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");

            if (smVisionInfo.g_arrPad != null)
            {
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckPackage", smVisionInfo.g_blnCheckPackage);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantUseDetailThresholdPadPackage", smVisionInfo.g_blnWantUseDetailThreshold_PadPackage);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_SeperateCrackDefectSetting", smVisionInfo.g_arrPad[0].ref_blnSeperateCrackDefectSetting);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_SeperateForeignMaterialDefectSetting", smVisionInfo.g_arrPad[0].ref_blnSeperateForeignMaterialDefectSetting);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_SeperateChippedOffDefectSetting", smVisionInfo.g_arrPad[0].ref_blnSeperateChippedOffDefectSetting);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_SeperateMoldFlashDefectSetting", smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckPad", smVisionInfo.g_blnCheckPad);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheck4Sides", smVisionInfo.g_blnCheck4Sides);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantShowGRR", smVisionInfo.g_blnWantShowGRR);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckCPK", smVisionInfo.g_blnCPKON);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_CheckAllPadCPK", smVisionInfo.g_blnRecordAllPadCPKEvenIfFail);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_PadCPKCount", smVisionInfo.g_intCPKTestCount);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantPin1", smVisionInfo.g_blnWantPin1);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckPH", smVisionInfo.g_blnWantCheckPH);

                for (int i = 0; i < 5; i++)
                {
                    string strSectionName = "";
                    if (i == 0)
                        strSectionName = "CenterROI";
                    else if (i == 1)
                        strSectionName = "TopROI";
                    else if (i == 2)
                        strSectionName = "RightROI";
                    else if (i == 3)
                        strSectionName = "BottomROI";
                    else if (i == 4)
                        strSectionName = "LeftROI";

                    if (smVisionInfo.g_arrPad.Length > i)
                    {
                        smVisionInfo.g_arrPad[i].SavePad_SECSGEM(strPath, strSectionName, strVisionName, m_blnSECSGEMFileExist, m_smCustomizeInfo.g_intSECSGEMMaxNoOfCoplanPad);
                        smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SaveRectGauge4L_SECSGEM(strPath, strSectionName, strVisionName);
                    }
                    else
                    {
                        if (!m_blnSECSGEMFileExist)
                        {
                            //Pad
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_AngleTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_XTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_YTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ThresholdValue", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ImageMerge2ThresholdValue", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_InterPadThresholdValue", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_SurfaceThresholdValue", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_SurfaceLowThresholdValue", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ExtraPadMinArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_TotalExtraPadMinArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ExcessPadMinArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ExtraPadSetLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ExcessPadSetLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_SmearLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_MinArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_InspectionMinArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_InspectionImageMerge2MinArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_SurfaceMinArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_Image2SurfaceMinArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_MoldFlashMinArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_VoidMinArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_CrackMinArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BrightFieldMinArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_DarkFieldMinArea", "NA");

                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadLabelRefCorner", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadLabelDirection", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadStartX", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadStartY", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadEndX", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadEndY", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_FailMask", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_FailPkgMask", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantInspectPackage", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WhiteOnBlack", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_DefaultPixelTolerance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadSizeHalfWidthTolerance", m_intPadSizeHalfWidthTolerance);  // 2019 08 27 - CCENG : No longer use. Replaced by m_intMPErodeHalfWidth and m_intMPDilateHalfWidth
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_TightSettingThresholdTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_TightSettingTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_SensitivityOnPadMethod", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_SensitivityOnPadValue", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_EmptyMinScore", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_UseEmtpyUnitThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantGaugeMeasurePkgSize", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_MeasureCenterPkgSizeUsingSidePkg", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_UseDetailDefectCriteria", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_EmptyAreaLimit", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_EmptyThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_EmptyAreaColorTransition", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadPRSScore", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_UnitAngleLimit", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PosToleranceX", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PosToleranceY", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ImageGain", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GaugeSizeImageIndex", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_CheckPadDimensionImageIndex", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadROISizeTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadROISizeToleranceADV", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantTightSetting", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantConsiderPadImage2", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantPRUnitLocationBeforeGauge", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantUseGaugeMeasureDimension", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantUseClosestSizeDefineTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantAutoGauge", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantRotateSidePadImage", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantUseGroupToleranceSetting", "NA");

                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantCheckExtraPadLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantCheckExtraPadArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantCheckBrokenPadLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantCheckBrokenPadArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantCheckSmearPadLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantCheckExcessPadLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantCheckExcessPadArea", "NA");

                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeaturesCount", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeaturesCount", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGapCount", "NA");

                            for (int v = 0; v < m_smCustomizeInfo.g_intSECSGEMMaxNoOfCoplanPad; v++)
                            {
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_NoID", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GroupNo", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Area", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LengthMode", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_StartX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_StartY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_EndX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_EndY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fStartX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fStartY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fEndX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fEndY", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Direction", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PadSide", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_SmearSide", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GravityCenterX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GravityCenterY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LimitCenterX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LimitCenterY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Width", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Height", "NA");

                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthStart1", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightStart1", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthEnd1", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightEnd1", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthStart2", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightStart2", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthEnd2", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightEnd2", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthStart3", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightStart3", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthEnd3", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightEnd3", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthStart4", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightStart4", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthEnd4", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightEnd4", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthStart5", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightStart5", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthEnd5", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightEnd5", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthStart6", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightStart6", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthEnd6", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightEnd6", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthStart7", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightStart7", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthEnd7", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightEnd7", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LineCount", "NA");

                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_FeretWidth", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_FeretHeight", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_FeretCenterX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_FeretCenterY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_FeretAngle", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_FeretLengthMode", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_FeretActivated", "NA");

                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_P1X", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_P1Y", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_P2X", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_P2Y", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_P3X", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_P3Y", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_P4X", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_P4Y", "NA");

                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Selected", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Enable", "NA");

                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LeftDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_RightDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TopDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BottomDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_XDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_YDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Pitch", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Gap", "NA");

                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_OffSet", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinArea", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxArea", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinWidth", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxWidth", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinLength", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxLength", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinPitch", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxPitch", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinGap", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxGap", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxHole", "NA");   // == MaxBroken
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxBrokenLength", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxExcess", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxSmearLength", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_WidthOffset", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_HeightOffset", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PitchOffset", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GapOffset", "NA");

                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinLine3", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxLine3", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinLine4", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxLine4", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinLine5", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxLine5", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinLine6", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxLine6", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinLine7", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxLine7", "NA");
                            }

                            //for (int q = 0; q < 10; q++)
                            //{
                            //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_FromPadNo", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_ToPadNo", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_MinPitch", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_MaxPitch", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_MinGap", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_MaxGap", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_Gap", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_Pitch", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_StartX", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_StartY", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_EndX", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_EndY", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_Direction", "NA");
                            //}

                            // Save Group BlobsFeatures
                            for (int j = 0; j < 15; j++)
                            {
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_NoID", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_GroupNo", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_Area", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_LengthMode", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_ContourX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_ContourY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_StartX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_StartY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_EndX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_EndY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_fStartX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_fStartY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_fEndX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_fEndY", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_Direction", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_PadSide", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_SmearSide", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_GravityCenterX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_GravityCenterY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_LimitCenterX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_LimitCenterY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_Width", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_Height", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_Selected", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_Enable", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_LeftDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_RightDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_TopDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_BottomDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_XDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_YDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_Pitch", "NA");
                                //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_Gap", "NA");

                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_OffSet", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinArea", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxArea", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinWidth", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxWidth", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinLength", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxLength", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinPitch", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxPitch", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinGap", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxGap", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxHole", "NA");   // == MaxBroken
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxBrokenLength", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxExcess", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxSmearLength", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_WidthOffset", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_HeightOffset", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_PitchOffset", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_GapOffset", "NA");

                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinLine3", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxLine3", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinLine4", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxLine4", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinLine5", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxLine5", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinLine6", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxLine6", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinLine7", "NA");
                                objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxLine7", "NA");
                            }

                            // Save Package Setting
                            //objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PackageSetting", "");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_ChipStartPixelFromEdge", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_ChipStartPixelFromRight", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_ChipStartPixelFromBottom", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_ChipStartPixelFromLeft", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_MoldStartPixelFromEdge", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_MoldStartPixelFromRight", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_MoldStartPixelFromBottom", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_MoldStartPixelFromLeft", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgStartPixelFromEdge", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgStartPixelFromRight", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgStartPixelFromBottom", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgStartPixelFromLeft", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgImage1HighPadThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgImage1LowPadThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgImage1HighSurfaceThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgImage1LowSurfaceThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgImage1Gain", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgImage2HighPadThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgImage2LowPadThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgImage3HighPadThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgImage3LowPadThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgImage1MoldFlashThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgImage2VoidThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgImage2HighCrackThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgImage2LowCrackThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PkgImage1ChippedThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_BrightFieldLowThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_BrightFieldHighThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_DarkFieldLowThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_DarkFieldHighThreshold", "NA");

                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_UnitWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_UnitHeight", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_UnitThickness", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_UnitWidthMin", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_UnitWidthMax", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_UnitHeightMin", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_UnitHeightMax", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_UnitThicknessMin", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_UnitThicknessMax", "NA");

                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_UnitSizeLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_CrackLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_ScratchLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_ScratchArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_ChipArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_ContaminationLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_SolderMeltLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_VoidLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_VoidArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_CrackLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_CrackArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_MoldFlashLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_InCompletePlateLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_PadBrokenLength", "NA");

                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_BrightLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_BrightWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_BrightArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_BrightTotalArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_DarkLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_DarkWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_DarkArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_DarkTotalArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_BrightChippedOffArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_DarkChippedOffArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_DarkVerticalCrack", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_DarkHorizontalCrack", "NA");

                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_MPErodeHalfWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _PackageSetting_" + strSectionName + "_MPDilateHalfWidth", "NA");

                            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GoldenDataSetCount", "NA");

                            for (int k = 0; k < 3; k++)
                            {
                                objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GoldenDataSet" + k.ToString() + "_GoldenDataUsed", "NA");
                                objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GoldenDataSet" + k.ToString() + "_GoldenDataCount", "NA");

                                for (int l = 0; l < 5; l++)
                                {
                                    //if (m_arrGoldenData[i][j].Count > 0)
                                    objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GoldenDataSet" + k.ToString() + "_Width" + l.ToString(), "NA");
                                    //if (m_arrGoldenData[i][j].Count > 1)
                                    objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GoldenDataSet" + k.ToString() + "_Length" + l.ToString(), "NA");
                                    //if (m_arrGoldenData[i][j].Count > 2)
                                    objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GoldenDataSet" + k.ToString() + "_Pitch" + l.ToString(), "NA");
                                    //if (m_arrGoldenData[i][j].Count > 3)
                                    objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GoldenDataSet" + k.ToString() + "_Gap" + l.ToString(), "NA");
                                }
                            }

                            //Gauge
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_UnitSizeWidth", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_UnitSizeHeight", "NA");

                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_Gauge4LCenterPointX", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_Gauge4LCenterPointY", "NA");

                            for (int j = 0; j < 12; j++)
                            {
                                // Rectangle gauge position setting
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_CenterX", "NA");
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_CenterY", "NA");
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_Length", "NA");
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_Angle", "NA");
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_Tolerance", "NA");

                                // Rectangle gauge measurement setting
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_TransType", "NA");
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_TransChoice", "NA");
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_Threshold", "NA");
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_Thickness", "NA");
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_MinAmp", "NA");
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_MinArea", "NA");
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_Filter", "NA");

                                // Rectangle gauge fitting setting
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_SamplingStep", "NA");
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_FilteringThreshold", "NA");
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_FilteringPasses", "NA");

                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_GaugeMinScore", "NA");
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_GaugeMaxAngle", "NA");
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_GaugeImageMode", "NA");

                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_GaugeTiltAngle", "NA");
                                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_IntoOut", "NA");
                            }

                            ////objFile.WriteSectionElement("PadDirection" + m_intDirection, blnNewSection);
                            //for (int k = 0; k < 4; k++)
                            //{
                            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + i + "_EdgeROI_" + k + "_PositionX", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + i + "_EdgeROI_" + k + "_PositionY", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + i + "_EdgeROI_" + k + "_Width", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + i + "_EdgeROI_" + k + "_Height", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + i + "_EdgeROI_" + k + "_CenterX", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + i + "_EdgeROI_" + k + "_CenterY", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + i + "_EdgeROI_" + k + "_DontCareROICount", "NA");
                            //}
                        }
                    }
                }

                if (smVisionInfo.g_arrPin1 != null)
                {
                    //units on image
                    //for (int u = 0; u < 2; u++)
                    //{
                    if (smVisionInfo.g_arrPin1.Count > 0)
                        smVisionInfo.g_arrPin1[0].SaveTemplate_SECSGEM(strPath, "_Pin1Settings", strVisionName, m_blnSECSGEMFileExist);
                    else
                    {
                        if (!m_blnSECSGEMFileExist)
                        {
                            objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1TemplateNum", "NA");

                            //Max template is 8
                            for (int j = 0; j < 8; j++)
                            {
                                objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1Template" + j + "_WantCheckPin1", "NA");
                                objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1Template" + j + "_OffsetRefPosX", "NA");
                                objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1Template" + j + "_OffsetRefPosY", "NA");
                                objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1Template" + j + "_MinScore", "NA");
                            }

                            //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIStartX", "NA");
                            //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIStartY", "NA");
                            //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIWidth", "NA");
                            //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIHeight", "NA");

                            //if (m_objPin1ROI.ref_ROI != null)
                            //{
                            //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIStartX", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIStartY", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIWidth", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIHeight", "NA");
                            //}
                        }
                    }
                    //}
                }
                else
                {
                    ////units on image
                    //for (int u = 0; u < 2; u++)
                    //{
                    if (!m_blnSECSGEMFileExist)
                    {
                        objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1TemplateNum", "NA");

                        //Max template is 8
                        for (int j = 0; j < 8; j++)
                        {
                            objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1Template" + j + "_WantCheckPin1", "NA");
                            objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1Template" + j + "_OffsetRefPosX", "NA");
                            objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1Template" + j + "_OffsetRefPosY", "NA");
                            objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1Template" + j + "_MinScore", "NA");
                        }

                        //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIStartX", "NA");
                        //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIStartY", "NA");
                        //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIWidth", "NA");
                        //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIHeight", "NA");

                        //if (m_objPin1ROI.ref_ROI != null)
                        //{
                        //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIStartX", "NA");
                        //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIStartY", "NA");
                        //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIWidth", "NA");
                        //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIHeight", "NA");
                        //}
                        //}
                    }
                }
            }
            else
            {
                if (!m_blnSECSGEMFileExist)
                {
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckPackage", "NA");
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantUseDetailThresholdPadPackage", "NA");
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_SeperateCrackDefectSetting", "NA");
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_SeperateChippedOffDefectSetting", "NA");
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_SeperateMoldFlashDefectSetting", "NA");
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckPad", "NA");
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheck4Sides", "NA");
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantShowGRR", "NA");
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckCPK", "NA");
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_CheckAllPadCPK", "NA");
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_PadCPKCount", "NA");
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantPin1", "NA");
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckPH", "NA");

                    for (int i = 0; i < 5; i++)
                    {
                        string strSectionName = "";
                        if (i == 0)
                            strSectionName = "CenterROI";
                        else if (i == 1)
                            strSectionName = "TopROI";
                        else if (i == 2)
                            strSectionName = "RightROI";
                        else if (i == 3)
                            strSectionName = "BottomROI";
                        else if (i == 4)
                            strSectionName = "LeftROI";

                        //Pad
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_AngleTolerance", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_XTolerance", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_YTolerance", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ThresholdValue", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ImageMerge2ThresholdValue", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_InterPadThresholdValue", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_SurfaceThresholdValue", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_SurfaceLowThresholdValue", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ExtraPadMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_TotalExtraPadMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ExcessPadMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ExtraPadSetLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ExcessPadSetLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_SmearLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_MinArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_InspectionMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_InspectionImageMerge2MinArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_SurfaceMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_Image2SurfaceMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_MoldFlashMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_VoidMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_CrackMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BrightFieldMinArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_DarkFieldMinArea", "NA");

                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadLabelRefCorner", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadLabelDirection", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadStartX", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadStartY", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadEndX", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadEndY", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_FailMask", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_FailPkgMask", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantInspectPackage", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WhiteOnBlack", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_DefaultPixelTolerance", "NA");
                        //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadSizeHalfWidthTolerance", m_intPadSizeHalfWidthTolerance);  // 2019 08 27 - CCENG : No longer use. Replaced by m_intMPErodeHalfWidth and m_intMPDilateHalfWidth
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_TightSettingThresholdTolerance", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_TightSettingTolerance", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_SensitivityOnPadMethod", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_SensitivityOnPadValue", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_EmptyMinScore", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_UseEmtpyUnitThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantGaugeMeasurePkgSize", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_MeasureCenterPkgSizeUsingSidePkg", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_UseDetailDefectCriteria", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_EmptyAreaLimit", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_EmptyThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_EmptyAreaColorTransition", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadPRSScore", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_UnitAngleLimit", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PosToleranceX", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PosToleranceY", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ImageGain", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GaugeSizeImageIndex", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_CheckPadDimensionImageIndex", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadROISizeTolerance", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadROISizeToleranceADV", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantTightSetting", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantConsiderPadImage2", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantPRUnitLocationBeforeGauge", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantUseGaugeMeasureDimension", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantUseClosestSizeDefineTolerance", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantAutoGauge", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantRotateSidePadImage", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantUseGroupToleranceSetting", "NA");

                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantCheckExtraPadLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantCheckExtraPadArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantCheckBrokenPadLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantCheckBrokenPadArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantCheckSmearPadLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantCheckExcessPadLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_WantCheckExcessPadArea", "NA");

                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeaturesCount", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeaturesCount", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGapCount", "NA");

                        for (int v = 0; v < m_smCustomizeInfo.g_intSECSGEMMaxNoOfCoplanPad; v++)
                        {
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_NoID", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GroupNo", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Area", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LengthMode", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_StartX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_StartY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_EndX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_EndY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fStartX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fStartY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fEndX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fEndY", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Direction", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PadSide", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_SmearSide", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GravityCenterX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GravityCenterY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LimitCenterX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LimitCenterY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Width", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Height", "NA");

                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthStart1", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightStart1", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthEnd1", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightEnd1", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthStart2", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightStart2", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthEnd2", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightEnd2", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthStart3", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightStart3", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthEnd3", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightEnd3", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthStart4", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightStart4", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthEnd4", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightEnd4", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthStart5", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightStart5", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthEnd5", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightEnd5", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthStart6", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightStart6", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthEnd6", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightEnd6", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthStart7", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightStart7", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentWidthEnd7", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PercentHeightEnd7", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LineCount", "NA");

                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_FeretWidth", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_FeretHeight", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_FeretCenterX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_FeretCenterY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_FeretAngle", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_FeretLengthMode", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_FeretActivated", "NA");

                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_P1X", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_P1Y", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_P2X", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_P2Y", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_P3X", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_P3Y", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_P4X", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_P4Y", "NA");

                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Selected", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Enable", "NA");

                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LeftDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_RightDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TopDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BottomDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_XDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_YDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Pitch", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Gap", "NA");

                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_OffSet", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinPitch", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxPitch", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinGap", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxGap", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxHole", "NA");   // == MaxBroken
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxBrokenLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxExcess", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxSmearLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_WidthOffset", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_HeightOffset", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_PitchOffset", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GapOffset", "NA");

                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinLine3", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxLine3", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinLine4", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxLine4", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinLine5", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxLine5", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinLine6", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxLine6", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinLine7", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxLine7", "NA");
                        }

                        //for (int q = 0; q < 10; q++)
                        //{
                        //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_FromPadNo", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_ToPadNo", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_MinPitch", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_MaxPitch", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_MinGap", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_MaxGap", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_Gap", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_Pitch", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_StartX", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_StartY", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_EndX", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_EndY", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PitchGap" + q + "_Direction", "NA");
                        //}

                        // Save Group BlobsFeatures
                        for (int j = 0; j < m_smCustomizeInfo.g_intSECSGEMMaxNoOfCoplanPad; j++)
                        {
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_NoID", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_GroupNo", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_Area", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_LengthMode", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_ContourX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_ContourY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_StartX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_StartY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_EndX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_EndY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_fStartX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_fStartY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_fEndX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_fEndY", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_Direction", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_PadSide", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_SmearSide", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_GravityCenterX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_GravityCenterY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_LimitCenterX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_LimitCenterY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_Width", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_Height", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_Selected", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_Enable", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_LeftDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_RightDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_TopDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_BottomDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_XDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_YDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_Pitch", "NA");
                            //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_Gap", "NA");

                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_OffSet", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxArea", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinPitch", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxPitch", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinGap", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxGap", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxHole", "NA");   // == MaxBroken
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxBrokenLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxExcess", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxSmearLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_WidthOffset", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_HeightOffset", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_PitchOffset", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_GapOffset", "NA");

                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinLine3", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxLine3", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinLine4", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxLine4", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinLine5", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxLine5", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinLine6", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxLine6", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MinLine7", "NA");
                            objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_GroupBlobsFeatures" + j + "_MaxLine7", "NA");
                        }

                        // Save Package Setting
                        //objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PackageSetting", "");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ChipStartPixelFromEdge", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ChipStartPixelFromRight", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ChipStartPixelFromBottom", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ChipStartPixelFromLeft", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_MoldStartPixelFromEdge", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_MoldStartPixelFromRight", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_MoldStartPixelFromBottom", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_MoldStartPixelFromLeft", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgStartPixelFromEdge", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgStartPixelFromRight", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgStartPixelFromBottom", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgStartPixelFromLeft", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgImage1HighPadThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgImage1LowPadThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgImage1HighSurfaceThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgImage1LowSurfaceThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgImage1Gain", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgImage2HighPadThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgImage2LowPadThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgImage3HighPadThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgImage3LowPadThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgImage1MoldFlashThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgImage2VoidThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgImage2HighCrackThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgImage2LowCrackThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PkgImage1ChippedThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BrightFieldLowThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BrightFieldHighThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_DarkFieldLowThreshold", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_DarkFieldHighThreshold", "NA");

                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_UnitWidth", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_UnitHeight", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_UnitThickness", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_UnitWidthMin", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_UnitWidthMax", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_UnitHeightMin", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_UnitHeightMax", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_UnitThicknessMin", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_UnitThicknessMax", "NA");

                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_UnitSizeLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_CrackLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ScratchLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ScratchArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ChipArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_ContaminationLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_SolderMeltLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_VoidLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_VoidArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_CrackLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_CrackArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_MoldFlashLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_InCompletePlateLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_PadBrokenLength", "NA");

                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BrightLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BrightWidth", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BrightArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BrightTotalArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_DarkLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_DarkWidth", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_DarkArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_DarkTotalArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_BrightChippedOffArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_DarkChippedOffArea", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_DarkVerticalCrack", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_DarkHorizontalCrack", "NA");

                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_MPErodeHalfWidth", "NA");
                        objFile.WriteElementValue(strVisionName + " _PadSetting_" + strSectionName + "_MPDilateHalfWidth", "NA");

                        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GoldenDataSetCount", "NA");

                        for (int k = 0; k < 3; k++)
                        {
                            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GoldenDataSet" + k.ToString() + "_GoldenDataUsed", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GoldenDataSet" + k.ToString() + "_GoldenDataCount", "NA");

                            for (int l = 0; l < 5; l++)
                            {
                                //if (m_arrGoldenData[i][j].Count > 0)
                                objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GoldenDataSet" + k.ToString() + "_Width" + l.ToString(), "NA");
                                //if (m_arrGoldenData[i][j].Count > 1)
                                objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GoldenDataSet" + k.ToString() + "_Length" + l.ToString(), "NA");
                                //if (m_arrGoldenData[i][j].Count > 2)
                                objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GoldenDataSet" + k.ToString() + "_Pitch" + l.ToString(), "NA");
                                //if (m_arrGoldenData[i][j].Count > 3)
                                objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GoldenDataSet" + k.ToString() + "_Gap" + l.ToString(), "NA");
                            }
                        }

                        //Gauge
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_UnitSizeWidth", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_UnitSizeHeight", "NA");

                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_Gauge4LCenterPointX", "NA");
                        objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_Gauge4LCenterPointY", "NA");

                        for (int j = 0; j < 12; j++)
                        {
                            // Rectangle gauge position setting
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_CenterX", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_CenterY", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_Length", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_Angle", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_Tolerance", "NA");

                            // Rectangle gauge measurement setting
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_TransType", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_TransChoice", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_Threshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_Thickness", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_MinAmp", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_MinArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_Filter", "NA");

                            // Rectangle gauge fitting setting
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_SamplingStep", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_FilteringThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_FilteringPasses", "NA");

                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_GaugeMinScore", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_GaugeMaxAngle", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_GaugeImageMode", "NA");

                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_GaugeTiltAngle", "NA");
                            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + j + "_IntoOut", "NA");
                        }

                        ////objFile.WriteSectionElement("PadDirection" + m_intDirection, blnNewSection);
                        //for (int k = 0; k < 4; k++)
                        //{
                        //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + i + "_EdgeROI_" + k + "_PositionX", "NA");
                        //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + i + "_EdgeROI_" + k + "_PositionY", "NA");
                        //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + i + "_EdgeROI_" + k + "_Width", "NA");
                        //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + i + "_EdgeROI_" + k + "_Height", "NA");
                        //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + i + "_EdgeROI_" + k + "_CenterX", "NA");
                        //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + i + "_EdgeROI_" + k + "_CenterY", "NA");
                        //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + i + "_EdgeROI_" + k + "_DontCareROICount", "NA");
                        //}
                    }
                }

                if (smVisionInfo.g_arrPin1 != null)
                {
                    ////units on image
                    //for (int u = 0; u < 2; u++)
                    //{
                    if (smVisionInfo.g_arrPin1.Count > 0)
                        smVisionInfo.g_arrPin1[0].SaveTemplate_SECSGEM(strPath, "_Pin1Settings", strVisionName, m_blnSECSGEMFileExist);
                    else
                    {
                        if (!m_blnSECSGEMFileExist)
                        {
                            objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1TemplateNum", "NA");

                            //Max template is 8
                            for (int j = 0; j < 8; j++)
                            {
                                objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1Template" + j + "_WantCheckPin1", "NA");
                                objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1Template" + j + "_OffsetRefPosX", "NA");
                                objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1Template" + j + "_OffsetRefPosY", "NA");
                                objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1Template" + j + "_MinScore", "NA");
                            }

                            //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIStartX", "NA");
                            //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIStartY", "NA");
                            //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIWidth", "NA");
                            //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIHeight", "NA");

                            //if (m_objPin1ROI.ref_ROI != null)
                            //{
                            //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIStartX", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIStartY", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIWidth", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIHeight", "NA");
                            //}
                        }
                    }
                    //}
                }
                else
                {
                    if (!m_blnSECSGEMFileExist)
                    {
                        //units on image
                        for (int u = 0; u < 2; u++)
                        {
                            objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1TemplateNum", "NA");

                            //Max template is 8
                            for (int j = 0; j < 8; j++)
                            {
                                objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1Template" + j + "_WantCheckPin1", "NA");
                                objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1Template" + j + "_OffsetRefPosX", "NA");
                                objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1Template" + j + "_OffsetRefPosY", "NA");
                                objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1Template" + j + "_MinScore", "NA");
                            }

                            //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIStartX", "NA");
                            //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIStartY", "NA");
                            //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIWidth", "NA");
                            //objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_SearchROIHeight", "NA");

                            //if (m_objPin1ROI.ref_ROI != null)
                            //{
                            //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIStartX", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIStartY", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIWidth", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_Pin1Settings" + "_Pin1ROIHeight", "NA");
                            //}
                        }
                    }
                }
            }

            objFile.WriteEndElement();
        }

        private void SaveLeadSetting_SECSGEM(VisionInfo smVisionInfo, string strPath, string strVisionName)
        {
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");

            if (smVisionInfo.g_arrLead != null)
            {
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckLead", smVisionInfo.g_blnCheckLead);

                for (int i = 0; i < 5; i++)
                {
                    // Load Lead Template Setting
                    string strSectionName = "";
                    if (i == 0)
                        strSectionName = "SearchROI";
                    else if (i == 1)
                        strSectionName = "TopROI";
                    else if (i == 2)
                        strSectionName = "RightROI";
                    else if (i == 3)
                        strSectionName = "BottomROI";
                    else if (i == 4)
                        strSectionName = "LeftROI";

                    if (smVisionInfo.g_arrLead.Length > i)
                    {
                        smVisionInfo.g_arrLead[i].SaveLead_SECSGEM(strPath, strSectionName, strVisionName, m_blnSECSGEMFileExist, m_smCustomizeInfo.g_intSECSGEMMaxNoOfCoplanPad);
                        smVisionInfo.g_arrLead[i].ref_objPointGauge.SavePointGauge_SECSGEM(strPath, strSectionName, strVisionName);
                    }
                    else
                    {
                        if (!m_blnSECSGEMFileExist)
                        {
                            //Lead
                            // Save LeadSetting
                            //objFile.WriteElement1Value("LeadSetting", "");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ImageViewNo", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ThresholdValue", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadDirection", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_NumberOfLead", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSelected", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_MinArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_FailMask", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadStartX", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadStartY", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadEndX", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadEndY", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ReferencePointStartX", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ReferencePointStartY", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ReferencePointEndX", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ReferencePointEndY", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadMinSpanStart", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadMaxSpanStart", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadMinSpanEnd", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadMaxSpanEnd", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ExtraLeadSetArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_TotalExtraLeadSetArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ExtraLeadSetLength", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_TemplateLeadMinSpanLimit", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_TemplateLeadMaxSpanLimit", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_WantUseGaugeMeasureLeadDimension", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_WantCheckExtraLeadLength", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_WantCheckExtraLeadArea", "NA");

                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ClockWise", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_Lead1", "NA");


                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_BlobsFeaturesCount", "NA");
                            for (int v = 0; v < m_smCustomizeInfo.g_intSECSGEMMaxNoOfCoplanPad; v++)
                            {
                                //objFile.WriteElement1Value("BlobsFeatures" + v, "");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_NoID", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GroupNo", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Area", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LengthMode", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourLength", "NA");

                                //for (int j = 0; j < 10; j++)
                                //{
                                //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourX" + j.ToString(), "NA");
                                //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourY" + j.ToString(), "NA");
                                //}

                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_StartX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_StartY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_EndX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_EndY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fStartX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fStartY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fEndX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fEndY", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Direction", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LeadSide", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GravityCenterX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GravityCenterY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LimitCenterX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LimitCenterY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Width", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Height", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseWidth", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseHeight", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseCenterX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseCenterY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipWidth", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipHeight", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipCenterX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipCenterY", "NA");

                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Selected", "NA");

                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LeftDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_RightDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TopDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BottomDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_XDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_YDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Pitch", "NA");

                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_OffSet", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinWidth", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxWidth", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinHeight", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxHeight", "NA");

                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinBaseWidth", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxBaseWidth", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinBaseHeight", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxBaseHeight", "NA");

                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinTipWidth", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxTipWidth", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinTipHeight", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxTipHeight", "NA");

                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinPitch", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxPitch", "NA");

                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Variance", "NA");

                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseInwardOffset", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipInwardOffset", "NA");

                            }

                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_PitchGapCount", "NA");
                            //for (int q = 0; q < 10; q++)
                            //{
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_FromLeadNo", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_ToLeadNo", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_MinPitch", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_MaxPitch", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_Pitch", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_StartX", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_StartY", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_EndX", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_EndY", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_Direction", "NA");
                            //}

                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitWidth", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitHeight", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitThickness", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitWidthMin", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitWidthMax", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitHeightMin", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitHeightMax", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitThicknessMin", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitThicknessMax", "NA");

                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitSizeLength", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_CrackLength", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ScratchLength", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ContaminationLength", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_SolderMeltLength", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_VoidLength", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_MoldFlashLength", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_InCompletePlateLength", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadBrokenLength", "NA");

                            //Gauge
                            // Point gauge position setting
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_CenterX", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_CenterY", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Angle", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Tolerance", "NA");

                            // Rectangle gauge measurement setting
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_TransType", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_TransChoice", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Threshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Thickness", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_MinAmp", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_MinArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Filter", "NA");
                        }
                    }
                }
            }
            else
            {
                if (!m_blnSECSGEMFileExist)
                {
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckLead", "NA");

                    for (int i = 0; i < 5; i++)
                    {
                        // Load Lead Template Setting
                        string strSectionName = "";
                        if (i == 0)
                            strSectionName = "SearchROI";
                        else if (i == 1)
                            strSectionName = "TopROI";
                        else if (i == 2)
                            strSectionName = "RightROI";
                        else if (i == 3)
                            strSectionName = "BottomROI";
                        else if (i == 4)
                            strSectionName = "LeftROI";

                        //Lead
                        // Save LeadSetting
                        //objFile.WriteElement1Value("LeadSetting", "");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ImageViewNo", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ThresholdValue", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadDirection", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_NumberOfLead", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSelected", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_MinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_FailMask", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadStartX", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadStartY", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadEndX", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadEndY", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ReferencePointStartX", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ReferencePointStartY", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ReferencePointEndX", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ReferencePointEndY", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadMinSpanStart", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadMaxSpanStart", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadMinSpanEnd", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadMaxSpanEnd", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ExtraLeadSetArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_TotalExtraLeadSetArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ExtraLeadSetLength", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_TemplateLeadMinSpanLimit", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_TemplateLeadMaxSpanLimit", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_WantUseGaugeMeasureLeadDimension", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_WantCheckExtraLeadLength", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_WantCheckExtraLeadArea", "NA");

                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ClockWise", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_Lead1", "NA");


                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_BlobsFeaturesCount", "NA");
                        for (int v = 0; v < m_smCustomizeInfo.g_intSECSGEMMaxNoOfCoplanPad; v++)
                        {
                            //objFile.WriteElement1Value("BlobsFeatures" + v, "");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_NoID", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GroupNo", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Area", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LengthMode", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourLength", "NA");

                            //for (int j = 0; j < 10; j++)
                            //{
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourX" + j.ToString(), "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourY" + j.ToString(), "NA");
                            //}

                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_StartX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_StartY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_EndX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_EndY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fStartX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fStartY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fEndX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fEndY", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Direction", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LeadSide", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GravityCenterX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GravityCenterY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LimitCenterX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LimitCenterY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Width", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Height", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseWidth", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseHeight", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseCenterX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseCenterY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipWidth", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipHeight", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipCenterX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipCenterY", "NA");

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Selected", "NA");

                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LeftDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_RightDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TopDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BottomDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_XDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_YDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Pitch", "NA");

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_OffSet", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinHeight", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxHeight", "NA");

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinBaseWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxBaseWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinBaseHeight", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxBaseHeight", "NA");

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinTipWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxTipWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinTipHeight", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxTipHeight", "NA");

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinPitch", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxPitch", "NA");

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Variance", "NA");

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseInwardOffset", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipInwardOffset", "NA");

                        }

                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_PitchGapCount", "NA");
                        //for (int q = 0; q < 10; q++)
                        //{
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_FromLeadNo", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_ToLeadNo", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_MinPitch", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_MaxPitch", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_Pitch", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_StartX", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_StartY", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_EndX", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_EndY", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_Direction", "NA");
                        //}

                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitWidth", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitHeight", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitThickness", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitWidthMin", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitWidthMax", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitHeightMin", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitHeightMax", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitThicknessMin", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitThicknessMax", "NA");

                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_UnitSizeLength", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_CrackLength", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ScratchLength", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ContaminationLength", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_SolderMeltLength", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_VoidLength", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_MoldFlashLength", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_InCompletePlateLength", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadBrokenLength", "NA");

                        //Gauge
                        // Point gauge position setting
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_CenterX", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_CenterY", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Angle", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Tolerance", "NA");

                        // Rectangle gauge measurement setting
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_TransType", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_TransChoice", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Threshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Thickness", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_MinAmp", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_MinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Filter", "NA");

                        //Gauge
                        // Point gauge position setting
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_CenterX", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_CenterY", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Angle", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Tolerance", "NA");

                        // Rectangle gauge measurement setting
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_TransType", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_TransChoice", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Threshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Thickness", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_MinAmp", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_MinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Filter", "NA");
                    }
                }
            }

            objFile.WriteEndElement();
        }

        private void SaveLead3DSetting_SECSGEM(VisionInfo smVisionInfo, string strPath, string strVisionName)
        {
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");

            if (smVisionInfo.g_arrLead3D != null)
            {
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckLead", smVisionInfo.g_blnCheckLead);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckPH", smVisionInfo.g_blnWantCheckPH);

                for (int i = 0; i < 5; i++)
                {
                    // Load Lead Template Setting
                    string strSectionName = "";
                    if (i == 0)
                        strSectionName = "CenterROI";
                    else if (i == 1)
                        strSectionName = "TopROI";
                    else if (i == 2)
                        strSectionName = "RightROI";
                    else if (i == 3)
                        strSectionName = "BottomROI";
                    else if (i == 4)
                        strSectionName = "LeftROI";

                    if (smVisionInfo.g_arrLead3D.Length > i)
                    {
                        smVisionInfo.g_arrLead3D[i].SaveLead3D_SECSGEM(strPath, strSectionName, strVisionName, m_blnSECSGEMFileExist, m_smCustomizeInfo.g_intSECSGEMMaxNoOfCoplanPad);
                        smVisionInfo.g_arrLead3D[i].ref_objPointGauge.SavePointGauge_SECSGEM(strPath, strSectionName, strVisionName);
                    }
                    else
                    {
                        if (!m_blnSECSGEMFileExist)
                        {
                            //Lead3D
                            // Save LeadSetting
                            //objFile.WriteElementValue("LeadSetting", "");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ThresholdValue", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadDirection", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_NumberOfLead", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_NumberOfLead_Top", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_NumberOfLead_Bottom", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_NumberOfLead_Left", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_NumberOfLead_Right", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSelected", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_MinArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_FailMask", "NA");
                            //objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadStartX", "NA");
                            //objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadStartY", "NA");
                            //objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadEndX", "NA");
                            //objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadEndY", "NA");
                            //objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ReferencePointStartX", "NA");
                            //objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ReferencePointStartY", "NA");
                            //objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ReferencePointEndX", "NA");
                            //objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ReferencePointEndY", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_BaseOffset", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_TipOffset", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ClockWise", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_Lead1", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_BaseLineTrimFromEdge", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_BaseLineSteps", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_WhiteOnBlack", "NA");

                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_CornerSearchingTolerance_Top", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_CornerSearchingTolerance_Right", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_CornerSearchingTolerance_Bottom", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_CornerSearchingTolerance_Left", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_CornerSearchingLength", "NA");

                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_PitchVarianceLeftMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_PitchVarianceRightMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_PitchVarianceTopMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_PitchVarianceBottomMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_StandOffVarianceUnitMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSweepVarianceLeftMinSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSweepVarianceRightMinSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSweepVarianceTopMinSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSweepVarianceBottomMinSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSweepVarianceLeftMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSweepVarianceRightMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSweepVarianceTopMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSweepVarianceBottomMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LengthVarianceLeftMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LengthVarianceRightMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LengthVarianceTopMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LengthVarianceBottomMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_StandOffVarianceLeftMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_StandOffVarianceRightMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_StandOffVarianceTopMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_StandOffVarianceBottomMaxSetting", "NA");
                            objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_StandOffVarianceUnitMaxSetting", "NA");

                            // Save BlobsFeatures
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeaturesCount", "NA");
                            for (int v = 0; v < m_smCustomizeInfo.g_intSECSGEMMaxNoOfCoplanPad; v++)
                            {
                                //objFile.WriteElementValue("BlobsFeatures" + v, "");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_NoID", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GroupNo", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Area", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LengthMode", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourY", "NA");

                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourLength", "NA");

                                //for (int j = 0; j < 10; j++)
                                //{
                                //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourX" + j.ToString(), "NA");
                                //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourY" + j.ToString(), "NA");
                                //}

                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_StartX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_StartY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_EndX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_EndY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fStartX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fStartY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fEndX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fEndY", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Direction", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LeadSide", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GravityCenterX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GravityCenterY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LimitCenterX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LimitCenterY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Width", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Height", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseWidth", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseLength", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseCenterX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseCenterY", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipWidth", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipHeight", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipCenterX", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipCenterY", "NA");

                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Selected", "NA");

                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LeftDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_RightDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TopDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BottomDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_XDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_YDistance", "NA");
                                //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Pitch", "NA");

                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_OffSet", "NA");

                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinBaseWidth", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxBaseWidth", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinBaseLength", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxBaseLength", "NA");

                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinTipWidth", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxTipWidth", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinTipHeight", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxTipHeight", "NA");

                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinPitch", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxPitch", "NA");

                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinStandOff", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxStandOff", "NA");

                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinSolderPadLength", "NA");
                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxSolderPadLength", "NA");

                                objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxCoplan", "NA");
                            }

                            //for (int q = 0; q < 10; q++)
                            //{
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_FromLeadNo", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_ToLeadNo", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_MinPitch", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_MaxPitch", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_Pitch", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_StartX", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_StartY", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_EndX", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_EndY", "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_Direction", "NA");
                            //}

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitHeight", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitThickness", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitWidthMin", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitWidthMax", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitHeightMin", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitHeightMax", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitThicknessMin", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitThicknessMax", "NA");

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitSizeLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_CrackLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_ScratchLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_ContaminationLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_SolderMeltLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_VoidLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_MoldFlashLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_InCompletePlateLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_LeadBrokenLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_MPTolerance", "NA");

                            //Gauge
                            // Point gauge position setting
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_CenterX", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_CenterY", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Angle", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Tolerance", "NA");

                            // Rectangle gauge measurement setting
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_TransType", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_TransChoice", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Threshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Thickness", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_MinAmp", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_MinArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Filter", "NA");
                        }
                    }
                }
            }
            else
            {
                if (!m_blnSECSGEMFileExist)
                {
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantCheckLead", "NA");

                    for (int i = 0; i < 5; i++)
                    {
                        // Load Lead Template Setting
                        string strSectionName = "";
                        if (i == 0)
                            strSectionName = "CenterROI";
                        else if (i == 1)
                            strSectionName = "TopROI";
                        else if (i == 2)
                            strSectionName = "RightROI";
                        else if (i == 3)
                            strSectionName = "BottomROI";
                        else if (i == 4)
                            strSectionName = "LeftROI";

                        //Lead3D
                        // Save LeadSetting
                        //objFile.WriteElementValue("LeadSetting", "");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ThresholdValue", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadDirection", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_NumberOfLead", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_NumberOfLead_Top", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_NumberOfLead_Bottom", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_NumberOfLead_Left", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_NumberOfLead_Right", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSelected", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_MinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_FailMask", "NA");
                        //objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadStartX", "NA");
                        //objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadStartY", "NA");
                        //objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadEndX", "NA");
                        //objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadEndY", "NA");
                        //objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ReferencePointStartX", "NA");
                        //objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ReferencePointStartY", "NA");
                        //objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ReferencePointEndX", "NA");
                        //objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ReferencePointEndY", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_BaseOffset", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_TipOffset", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_ClockWise", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_Lead1", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_BaseLineTrimFromEdge", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_BaseLineSteps", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_WhiteOnBlack", "NA");

                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_CornerSearchingTolerance_Top", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_CornerSearchingTolerance_Right", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_CornerSearchingTolerance_Bottom", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_CornerSearchingTolerance_Left", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_CornerSearchingLength", "NA");

                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_PitchVarianceLeftMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_PitchVarianceRightMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_PitchVarianceTopMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_PitchVarianceBottomMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_StandOffVarianceUnitMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSweepVarianceLeftMinSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSweepVarianceRightMinSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSweepVarianceTopMinSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSweepVarianceBottomMinSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSweepVarianceLeftMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSweepVarianceRightMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSweepVarianceTopMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LeadSweepVarianceBottomMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LengthVarianceLeftMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LengthVarianceRightMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LengthVarianceTopMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_LengthVarianceBottomMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_StandOffVarianceLeftMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_StandOffVarianceRightMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_StandOffVarianceTopMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_StandOffVarianceBottomMaxSetting", "NA");
                        objFile.WriteElementValue(strVisionName + "_LeadSetting_" + strSectionName + "_StandOffVarianceUnitMaxSetting", "NA");

                        // Save BlobsFeatures
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeaturesCount", "NA");
                        for (int v = 0; v < m_smCustomizeInfo.g_intSECSGEMMaxNoOfCoplanPad; v++)
                        {
                            //objFile.WriteElementValue("BlobsFeatures" + v, "");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_NoID", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GroupNo", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Area", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LengthMode", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourY", "NA");

                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourLength", "NA");

                            //for (int j = 0; j < 10; j++)
                            //{
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourX" + j.ToString(), "NA");
                            //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_ContourY" + j.ToString(), "NA");
                            //}

                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_StartX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_StartY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_EndX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_EndY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fStartX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fStartY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fEndX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_fEndY", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Direction", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LeadSide", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GravityCenterX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_GravityCenterY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LimitCenterX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LimitCenterY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Width", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Height", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseWidth", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseLength", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseCenterX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BaseCenterY", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipWidth", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipHeight", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipCenterX", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TipCenterY", "NA");

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Selected", "NA");

                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_LeftDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_RightDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_TopDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_BottomDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_XDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_YDistance", "NA");
                            //objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_Pitch", "NA");

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_OffSet", "NA");

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinBaseWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxBaseWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinBaseLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxBaseLength", "NA");

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinTipWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxTipWidth", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinTipHeight", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxTipHeight", "NA");

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinPitch", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxPitch", "NA");

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinStandOff", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxStandOff", "NA");

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MinSolderPadLength", "NA");
                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxSolderPadLength", "NA");

                            objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_BlobsFeatures" + v + "_MaxCoplan", "NA");
                        }

                        //for (int q = 0; q < 10; q++)
                        //{
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_FromLeadNo", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_ToLeadNo", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_MinPitch", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_MaxPitch", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_Pitch", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_StartX", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_StartY", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_EndX", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_EndY", "NA");
                        //    objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_PitchGap" + q + "_Direction", "NA");
                        //}

                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitWidth", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitHeight", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitThickness", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitWidthMin", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitWidthMax", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitHeightMin", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitHeightMax", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitThicknessMin", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitThicknessMax", "NA");

                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_UnitSizeLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_CrackLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_ScratchLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_ContaminationLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_SolderMeltLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_VoidLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_MoldFlashLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_InCompletePlateLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_LeadBrokenLength", "NA");
                        objFile.WriteElementValue(strVisionName + " _LeadSetting_" + strSectionName + "_MPTolerance", "NA");

                        //Gauge
                        // Point gauge position setting
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_CenterX", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_CenterY", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Angle", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Tolerance", "NA");

                        // Rectangle gauge measurement setting
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_TransType", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_TransChoice", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Threshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Thickness", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_MinAmp", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_MinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Filter", "NA");
                    }
                }
            }
            objFile.WriteEndElement();
        }

        private void SaveSealGeneralSetting_SECSGEM(VisionInfo smVisionInfo, string strPath, string strVisionName)
        {
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");
            objFile.WriteElementValue(strVisionName + "_TemplateCounting" + "_TotalPocketTemplates", smVisionInfo.g_intPocketTemplateTotal);
            objFile.WriteElementValue(strVisionName + "_TemplateCounting" + "_TotalMarkTemplates", smVisionInfo.g_intMarkTemplateTotal);
            objFile.WriteElementValue(strVisionName + "_TemplateCounting" + "_PocketTemplateMask", smVisionInfo.g_intPocketTemplateMask);
            objFile.WriteElementValue(strVisionName + "_TemplateCounting" + "_MarkTemplateMask", smVisionInfo.g_intMarkTemplateMask);
            objFile.WriteEndElement();
        }

        private void SaveSealSettings_SECSGEM(VisionInfo smVisionInfo, string strPath, string strVisionName)
        {
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");

            if (smVisionInfo.g_objSeal != null)
            {
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_Direction", smVisionInfo.g_objSeal.ref_intDirections);
                //objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_PackageName", smVisionInfo.g_objSeal.ref_strPackageName);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantSkipOrient", smVisionInfo.g_objSeal.ref_blnWantSkipOrient);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantSkipSprocketHole", smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantSkipSprocketHoleDiameterAndDefect", smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantSkipSprocketHoleBrokenAndRoundness", smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness);
                objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_CheckMarkMethod", smVisionInfo.g_objSeal.ref_intCheckMarkMethod);

                switch (smVisionInfo.g_objSeal.ref_intTapePocketPitch)
                {
                    case 4:
                        objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_PocketPitch", 0);
                        break;
                    case 8:
                        objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_PocketPitch", 1);
                        break;
                    case 12:
                        objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_PocketPitch", 2);
                        break;
                    case 3:
                        objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_PocketPitch", 3);
                        break;
                }
            }
            else
            {
                if (!m_blnSECSGEMFileExist)
                {
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_Direction", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_PackageName", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_WantSkipOrient", "NA");
                    objFile.WriteElementValue(strVisionName + "_AdvSetting" + "_PocketPitch", "NA");
                }
            }
            objFile.WriteEndElement();
        }

        private void SaveCalibrationSetting_SECSGEM(VisionInfo smVisionInfo, string strPath, string strVisionName)
        {
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");
            objFile.WriteElementValue(strVisionName + "_Calibrate" + "_PixelX", smVisionInfo.g_fCalibPixelX);
            objFile.WriteElementValue(strVisionName + "_Calibrate" + "_PixelY", smVisionInfo.g_fCalibPixelY);
            objFile.WriteElementValue(strVisionName + "_Calibrate" + "_PixelZ", smVisionInfo.g_fCalibPixelZ);
            objFile.WriteElementValue(strVisionName + "_Calibrate" + "_OffSetX", smVisionInfo.g_fCalibOffSetX);
            objFile.WriteElementValue(strVisionName + "_Calibrate" + "_OffSetY", smVisionInfo.g_fCalibOffSetY);
            objFile.WriteElementValue(strVisionName + "_Calibrate" + "_OffSetZ", smVisionInfo.g_fCalibOffSetZ);
            objFile.WriteEndElement();
        }

        private void DeleteTempSECSGEMFile()
        {
            if (File.Exists(m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\SECSGEM(Temp).xml"))
                File.Delete(m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\SECSGEM(Temp).xml");
        }

        private void CheckSECSGEMFileExist()
        {
            if (File.Exists(m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\SECSGEM.xml") && m_smCustomizeInfo.g_intSECSGEMMaxNoOfCoplanPad == m_intSECSGEMMaxNoOfCoplanPad)
            {
                m_blnSECSGEMFileExist = true;
                //If the SECSGEM.xml already exist, copy to SECSGEM(Temp).xml
                File.Copy(m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\SECSGEM.xml", m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\SECSGEM(Temp).xml");
            }
            else
                m_blnSECSGEMFileExist = false;
        }

        private void UpdateSECSGEMFile(VisionInfo smVisionInfo)
        {
            string strPath = m_smCustomizeInfo.g_strSECSGEMSharedFolderPath + "\\" + m_strSECSGEMFileName;
            string strVisionName;
            switch (smVisionInfo.g_strVisionName)
            {
                case "BottomPosition":
                case "BottomPositionOrient":
                    strVisionName = "Bottom";
                    m_blnBottomInited = true;
                    SaveYieldSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                    SaveGeneralSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                    SaveGaugeM4LSetting_SECSGEM(strPath, "MOGaugeM4L", smVisionInfo.g_arrOrientGaugeM4L, strVisionName);

                    SaveOrientSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                    SaveCalibrationSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
                    //ROI.LoadFile(strFolderPath + "Positioning\\ROI.xml", smVisionInfo.g_arrPositioningROIs);
                    LGauge.SaveFile_SECSGEM(strPath, smVisionInfo.g_arrPositioningGauges, strVisionName, 4, m_blnSECSGEMFileExist);

                    if (smVisionInfo.g_objPositioning != null)
                        smVisionInfo.g_objPositioning.SavePosition_SECSGEM(strPath, "Position", strVisionName);
                    else
                    {
                        if (!m_blnSECSGEMFileExist)
                        {
                            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
                            objFile.WriteRootElement("SECSGEMData");

                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionImageIndex", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyImageIndex", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinBorderScore", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinEmptyScore", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_AngleLimit", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionXTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionYTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieWidth", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieHeight", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_Method", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PRSMode", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue2", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateX", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateY", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipAreaLimit", "NA");
                            //Empty
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyWhiteArea", "NA");
                            //PH
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHBlackArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHMinArea", "NA");

                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_MinScore", "NA");
                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionX", "NA");
                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionY", "NA");
                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PosTolerance", "NA");
                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_Direction", "NA");

                            objFile.WriteEndElement();
                        }
                    }
                    break;
                case "UnitPresent":
                    strVisionName = "UnitPresent";
                    m_blnUnitPresentInited = true;
                    //ROI.LoadFile(strFolderPath + "CheckPresent\\ROI.xml", smVisionInfo.g_arrPositioningROIs);
                    if (smVisionInfo.g_objUnitPresent != null)
                        smVisionInfo.g_objUnitPresent.SaveUnitPresent_SECSGEM(strPath, "UnitPresentSetting", strVisionName);
                    else
                    {
                        if (!m_blnSECSGEMFileExist)
                        {
                            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
                            objFile.WriteRootElement("SECSGEMData");

                            // Rectangle gauge template measurement result
                            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_WhiteOnBlack", "NA");
                            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_ThresholdValue", "NA");
                            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_FilterMinArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_FilterMaxArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_TotalTemplateBlobObject", "NA");
                            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_HalfPitch", "NA");
                            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_UnitROICountX", "NA");
                            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_UnitROICountY", "NA");

                            //for (int i = 0; i < 10; i++)
                            //{
                            //    //objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i, "");
                            //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_LimitCenterX", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_LimitCenterY", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_Width", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_Height", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_Area", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_StartX", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_StartY", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_EndX", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_EndY", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_MinArea", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_MinOffSet", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_ROIStartX", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_ROIStartY", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_ROIWidth", "NA");
                            //    objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_ROIHeight", "NA");
                            //}

                            objFile.WriteEndElement();
                        }
                    }

                    break;
                case "Mark":
                case "MarkOrient":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                case "MOLi":
                case "Package":
                case "Orient":
                case "BottomOrient":
                    strVisionName = "MarkOrient";
                    m_blnMarkOrientInited = true;
                    SaveYieldSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                    SaveGeneralSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                    //orient
                    SaveGaugeM4LSetting_SECSGEM(strPath, "MOGaugeM4L", smVisionInfo.g_arrOrientGaugeM4L, strVisionName);

                    SaveOrientSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                    //ROI.LoadFile(strFolderPath + "Mark\\ROI.xml", smVisionInfo.g_arrMarkROIs);

                    //Mark
                    SaveMarkSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                    SaveGaugeM4LSetting_SECSGEM(strPath, "MOGaugeM4L", smVisionInfo.g_arrMarkGaugeM4L, strVisionName);

                    SaveControlSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                    //Pkg
                    SaveCalibrationSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
                    //ROI.LoadFile(strFolderPath + "Package\\ROI.xml", smVisionInfo.g_arrPackageROIs);

                    SaveGaugeM4LSetting_SECSGEM(strPath, "PkgGaugeM4L", smVisionInfo.g_arrPackageGaugeM4L, strVisionName);

                    SaveGaugeM4LSetting_SECSGEM(strPath, "PkgGauge2M4L", smVisionInfo.g_arrPackageGauge2M4L, strVisionName);

                    SavePackageSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                    //if (smVisionInfo.g_arrPolygon_Package != null)
                    //    LoadPolygonSetting_Package(strFolderPath + "Package\\Template\\Polygon.xml");

                    //Lead
                    //LoadROISetting(strFolderPath + "Lead\\ROI.xml", smVisionInfo.g_arrLeadROIs, smVisionInfo.g_arrLead.Length);
                    SaveLeadSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                    //Position
                    //ROI.LoadFile(strFolderPath + "Positioning\\ROI.xml", smVisionInfo.g_arrPositioningROIs);
                    LGauge.SaveFile_SECSGEM(strPath, smVisionInfo.g_arrPositioningGauges, strVisionName, 4, m_blnSECSGEMFileExist);

                    if (smVisionInfo.g_objPositioning != null)
                        smVisionInfo.g_objPositioning.SavePosition_SECSGEM(strPath, "Position", strVisionName);
                    else
                    {
                        if (!m_blnSECSGEMFileExist)
                        {
                            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
                            objFile.WriteRootElement("SECSGEMData");

                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionImageIndex", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyImageIndex", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinBorderScore", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinEmptyScore", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_AngleLimit", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionXTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionYTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieWidth", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieHeight", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_Method", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PRSMode", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue2", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateX", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateY", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipAreaLimit", "NA");
                            //Empty
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyWhiteArea", "NA");
                            //PH
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHBlackArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHMinArea", "NA");

                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_MinScore", "NA");
                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionX", "NA");
                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionY", "NA");
                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PosTolerance", "NA");
                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_Direction", "NA");

                            objFile.WriteEndElement();
                        }
                    }

                    break;
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                case "IPMLi":
                case "IPMLiPkg":
                    strVisionName = "InPocket";
                    m_blnInPocketInited = true;
                    SaveYieldSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                    //Orient
                    SaveGeneralSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                    SaveGaugeM4LSetting_SECSGEM(strPath, "MOGaugeM4L", smVisionInfo.g_arrOrientGaugeM4L, strVisionName);

                    SaveOrientSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                    //Mark
                    //ROI.LoadFile(strFolderPath + "Mark\\ROI.xml", smVisionInfo.g_arrMarkROIs);
                    SaveMarkSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                    SaveGaugeM4LSetting_SECSGEM(strPath, "MOGaugeM4L", smVisionInfo.g_arrMarkGaugeM4L, strVisionName);

                    SaveControlSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                    //Package
                    SaveCalibrationSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
                    //ROI.LoadFile(strFolderPath + "Package\\ROI.xml", smVisionInfo.g_arrPackageROIs);

                    SaveGaugeM4LSetting_SECSGEM(strPath, "PkgGaugeM4L", smVisionInfo.g_arrPackageGaugeM4L, strVisionName);

                    SaveGaugeM4LSetting_SECSGEM(strPath, "PkgGauge2M4L", smVisionInfo.g_arrPackageGauge2M4L, strVisionName);

                    SavePackageSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                    //if (smVisionInfo.g_arrPolygon_Package != null)
                    //    LoadPolygonSetting_Package(strFolderPath + "Package\\Template\\Polygon.xml");

                    //Lead
                    //LoadROISetting(strFolderPath + "Lead\\ROI.xml", smVisionInfo.g_arrLeadROIs, smVisionInfo.g_arrLead.Length);
                    SaveLeadSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                    //Position
                    //ROI.LoadFile(strFolderPath + "Positioning\\ROI.xml", smVisionInfo.g_arrPositioningROIs);
                    LGauge.SaveFile_SECSGEM(strPath, smVisionInfo.g_arrPositioningGauges, strVisionName, 4, m_blnSECSGEMFileExist);

                    if (smVisionInfo.g_objPositioning != null)
                        smVisionInfo.g_objPositioning.SavePosition_SECSGEM(strPath, "Position", strVisionName);
                    else
                    {
                        if (!m_blnSECSGEMFileExist)
                        {
                            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
                            objFile.WriteRootElement("SECSGEMData");

                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionImageIndex", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyImageIndex", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinBorderScore", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinEmptyScore", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_AngleLimit", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionXTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionYTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieWidth", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieHeight", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_Method", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PRSMode", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue2", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateX", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateY", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipAreaLimit", "NA");
                            //Empty
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyWhiteArea", "NA");
                            //PH
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHBlackArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHMinArea", "NA");

                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_MinScore", "NA");
                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionX", "NA");
                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionY", "NA");
                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PosTolerance", "NA");
                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_Direction", "NA");

                            objFile.WriteEndElement();
                        }
                    }
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "PadPos":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                case "Li3D":
                case "Li3DPkg":
                    strVisionName = "CoplanPad";
                    m_blnCoplanPadInited = true;
                    SaveYieldSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                    //Pad
                    SaveCalibrationSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                    //LoadROISetting(strFolderPath + "Pad\\ROI.xml", smVisionInfo.g_arrPadROIs, smVisionInfo.g_arrPad.Length);

                    SavePadSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                    SaveControlSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

                    //Lead3D
                    //LoadROISetting(strFolderPath + "Lead3D\\ROI.xml", smVisionInfo.g_arrLeadROIs, smVisionInfo.g_arrLead3D.Length);
                    SaveLead3DSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                    //Position
                    //ROI.LoadFile(strFolderPath + "Positioning\\ROI.xml", smVisionInfo.g_arrPositioningROIs);
                    LGauge.SaveFile_SECSGEM(strPath, smVisionInfo.g_arrPositioningGauges, strVisionName, 12, m_blnSECSGEMFileExist);

                    if (smVisionInfo.g_objPositioning != null)
                        smVisionInfo.g_objPositioning.SavePosition_SECSGEM(strPath, "Position", strVisionName);
                    else
                    {
                        if (!m_blnSECSGEMFileExist)
                        {
                            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
                            objFile.WriteRootElement("SECSGEMData");

                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionImageIndex", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyImageIndex", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinBorderScore", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinEmptyScore", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_AngleLimit", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionXTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionYTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieWidth", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieHeight", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_Method", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PRSMode", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue2", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateX", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateY", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipAreaLimit", "NA");
                            //Empty
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyWhiteArea", "NA");
                            //PH
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHBlackArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHMinArea", "NA");

                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_MinScore", "NA");
                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionX", "NA");
                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionY", "NA");
                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PosTolerance", "NA");
                            objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_Direction", "NA");

                            objFile.WriteEndElement();
                        }
                    }
                    break;
                case "Seal":
                    strVisionName = "Seal";
                    m_blnSealInited = true;
                    SaveYieldSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                    SaveCalibrationSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                    SaveSealGeneralSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

                    SaveSealSettings_SECSGEM(smVisionInfo, strPath, strVisionName);
                    //ROI.LoadFile(strFolderPath + "Seal\\ROI.xml", smVisionInfo.g_arrSealROIs);
                    LGauge.SaveFile_SECSGEM(strPath, smVisionInfo.g_arrSealGauges, strVisionName, 50, m_blnSECSGEMFileExist);

                    if (smVisionInfo.g_objSeal != null)
                        smVisionInfo.g_objSeal.SaveSeal_SECSGEM(strPath, "SealSettings", strVisionName, smVisionInfo.g_fCalibPixelX);
                    else
                    {
                        if (!m_blnSECSGEMFileExist)
                        {
                            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
                            objFile.WriteRootElement("SECSGEMData");

                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_BuildObjectLength", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_Seal1Threshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_Seal2Threshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_OverHeatThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_DistanceThreshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_Seal1MinArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_Seal2MinArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_OverHeatMinArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_SealHoleMinArea1", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_SealHoleMinArea2", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_MinBrokenWidth", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_PositionCenterX", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_PositionCenterY", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthLowerTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthLowerTolerance1", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthLowerTolerance2", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthUpperTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthUpperTolerance1", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthUpperTolerance2", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_DistanceMinTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_DistanceMaxTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_ShiftPositionTolerance", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_SealScoreTolerance", "NA");

                            for (int i = 0; i < 2; i++)
                            {
                                objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_TemplateSealArea" + i, "NA");
                                objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_SealAreaTolerance" + i, "NA");
                            }
                            for (int i = 0; i < 3; i++)
                                objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_TemplateSealLineWidth" + i, "NA");

                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_PocketMinScore", "NA");
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_MarkMinScore", "NA");

                            // Grab image index
                            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_GrabImageIndexCount", "NA");
                            for (int j = 0; j < 5; j++)
                                objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_GrabImageIndex" + j.ToString(), "NA");

                            objFile.WriteEndElement();
                        }
                    }

                    break;
            }

            //SaveYieldSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
            ////Orient settings
            //if ((m_smCustomizeInfo.g_intWantOrient & (1 << smVisionInfo.g_intVisionPos)) > 0)
            //{
            //    SaveGeneralSetting_SECSGEM(smVisionInfo, strPath, strVisionName);

            //    SaveGaugeM4LSetting_SECSGEM(strPath, "MOGaugeM4L", smVisionInfo.g_arrOrientGaugeM4L, strVisionName);

            //    SaveOrientSettings_SECSGEM(smVisionInfo, strPath, strVisionName);
            //}

            ////Mark settings
            //if ((m_smCustomizeInfo.g_intWantMark & (1 << smVisionInfo.g_intVisionPos)) > 0)
            //{
            //    SaveGeneralSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
            //    //ROI.LoadFile(strFolderPath + "Mark\\ROI.xml", smVisionInfo.g_arrMarkROIs);

            //    SaveMarkSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

            //    SaveGaugeM4LSetting_SECSGEM(strPath, "MOGaugeM4L", smVisionInfo.g_arrMarkGaugeM4L, strVisionName);

            //    SaveControlSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

            //    //if (smVisionInfo.g_arrPolygon_Mark != null)
            //    //    LoadPolygonSetting_Mark(strFolderPath + "Mark\\Template\\Polygon.xml");
            //}

            //if ((m_smCustomizeInfo.g_intWantPackage & (1 << smVisionInfo.g_intVisionPos)) > 0)
            //{
            //    SaveCalibrationSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
            //    //ROI.LoadFile(strFolderPath + "Package\\ROI.xml", smVisionInfo.g_arrPackageROIs);

            //    SaveGaugeM4LSetting_SECSGEM(strPath, "PkgGaugeM4L", smVisionInfo.g_arrPackageGaugeM4L, strVisionName);

            //    SaveGaugeM4LSetting_SECSGEM(strPath, "PkgGauge2M4L", smVisionInfo.g_arrPackageGauge2M4L, strVisionName);

            //    SavePackageSettings_SECSGEM(smVisionInfo, strPath, strVisionName);

            //    //if (smVisionInfo.g_arrPolygon_Package != null)
            //    //    LoadPolygonSetting_Package(strFolderPath + "Package\\Template\\Polygon.xml");

            //    SaveControlSettings_SECSGEM(smVisionInfo, strPath, strVisionName);
            //}

            ////Pad settings //5s pad settings
            //if ((m_smCustomizeInfo.g_intWantPad & (1 << smVisionInfo.g_intVisionPos)) > 0 || (m_smCustomizeInfo.g_intWantPad5S & (1 << smVisionInfo.g_intVisionPos)) > 0)
            //{
            //    SaveCalibrationSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
            //    //LoadROISetting(strFolderPath + "Pad\\ROI.xml", smVisionInfo.g_arrPadROIs, smVisionInfo.g_arrPad.Length);
            //    SavePadSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
            //    SaveControlSettings_SECSGEM(smVisionInfo, strPath, strVisionName);
            //}

            ////Lead settings
            //if ((m_smCustomizeInfo.g_intWantLead & (1 << smVisionInfo.g_intVisionPos)) > 0)
            //{
            //    SaveCalibrationSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
            //    //LoadROISetting(strFolderPath + "Lead\\ROI.xml", smVisionInfo.g_arrLeadROIs, smVisionInfo.g_arrLead.Length);
            //    SaveLeadSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
            //    SaveControlSettings_SECSGEM(smVisionInfo, strPath, strVisionName);
            //}

            ////Lead3D settings
            //if ((m_smCustomizeInfo.g_intWantLead3D & (1 << smVisionInfo.g_intVisionPos)) > 0)
            //{
            //    SaveCalibrationSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
            //    //LoadROISetting(strFolderPath + "Lead3D\\ROI.xml", smVisionInfo.g_arrLeadROIs, smVisionInfo.g_arrLead3D.Length);
            //    SaveLead3DSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
            //    SaveControlSettings_SECSGEM(smVisionInfo, strPath, strVisionName);
            //}

            ////Seal settings
            //if ((m_smCustomizeInfo.g_intWantSeal & (1 << smVisionInfo.g_intVisionPos)) > 0)
            //{
            //    SaveCalibrationSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
            //    SaveSealGeneralSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
            //    SaveSealSettings_SECSGEM(smVisionInfo, strPath, strVisionName);
            //    //ROI.LoadFile(strFolderPath + "Seal\\ROI.xml", smVisionInfo.g_arrSealROIs);
            //    LGauge.SaveFile_SECSGEM(strPath, smVisionInfo.g_arrSealGauges, strVisionName);

            //    if (smVisionInfo.g_objSeal != null)
            //        smVisionInfo.g_objSeal.SaveSeal_SECSGEM(strPath, "SealSettings", strVisionName, smVisionInfo.g_fCalibPixelX);
            //    else
            //    {
            //        XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            //        objFile.WriteRootElement("SECSGEMData");

            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_BuildObjectLength", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_Seal1Threshold", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_Seal2Threshold", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_OverHeatThreshold", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_DistanceThreshold", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_Seal1MinArea", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_Seal2MinArea", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_OverHeatMinArea", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_SealHoleMinArea1", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_SealHoleMinArea2", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_MinBrokenWidth", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_PositionCenterX", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_PositionCenterY", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthLowerTolerance", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthLowerTolerance1", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthLowerTolerance2", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthUpperTolerance", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthUpperTolerance1", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_WidthUpperTolerance2", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_DistanceMinTolerance", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_DistanceMaxTolerance", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_ShiftPositionTolerance", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_SealScoreTolerance", "NA");

            //        for (int i = 0; i < 2; i++)
            //        {
            //            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_TemplateSealArea" + i, "NA");
            //            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_SealAreaTolerance" + i, "NA");
            //        }
            //        for (int i = 0; i < 3; i++)
            //            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_TemplateSealLineWidth" + i, "NA");

            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_PocketMinScore", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_MarkMinScore", "NA");

            //        // Grab image index
            //        objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_GrabImageIndexCount", "NA");
            //        for (int j = 0; j < 5; j++)
            //            objFile.WriteElementValue(strVisionName + "_" + "SealSettings" + "_GrabImageIndex" + j.ToString(), "NA");

            //        objFile.WriteEndElement();
            //    }
            //}

            //if ((m_smCustomizeInfo.g_intWantPositioning & (1 << smVisionInfo.g_intVisionPos)) > 0)
            //{
            //    SaveCalibrationSetting_SECSGEM(smVisionInfo, strPath, strVisionName);
            //    //ROI.LoadFile(strFolderPath + "Positioning\\ROI.xml", smVisionInfo.g_arrPositioningROIs);
            //    LGauge.SaveFile_SECSGEM(strPath, smVisionInfo.g_arrPositioningGauges, strVisionName);

            //    if(smVisionInfo.g_objPositioning != null)
            //        smVisionInfo.g_objPositioning.SavePosition_SECSGEM(strPath, "Position", strVisionName);
            //    else
            //    {
            //        XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            //        objFile.WriteRootElement("SECSGEMData");

            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionImageIndex", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyImageIndex", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinBorderScore", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinEmptyScore", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_AngleLimit", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionXTolerance", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionYTolerance", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieWidth", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieHeight", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_Method", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PRSMode", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue2", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateX", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateY", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipThreshold", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipAreaLimit", "NA");
            //        //Empty
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyThreshold", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyWhiteArea", "NA");
            //        //PH
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHThreshold", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHBlackArea", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHMinArea", "NA");

            //        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_MinScore", "NA");
            //        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionX", "NA");
            //        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionY", "NA");
            //        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PosTolerance", "NA");
            //        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_Direction", "NA");

            //        objFile.WriteEndElement();
            //    }
            //}

            //if ((m_smCustomizeInfo.g_intWantCheckPresent & (1 << smVisionInfo.g_intVisionPos)) > 0)
            //{

            //    //ROI.LoadFile(strFolderPath + "CheckPresent\\ROI.xml", smVisionInfo.g_arrPositioningROIs);
            //    if (smVisionInfo.g_objUnitPresent != null)
            //        smVisionInfo.g_objUnitPresent.SaveUnitPresent_SECSGEM(strPath, "UnitPresentSetting", strVisionName);
            //    else
            //    {
            //        XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            //        objFile.WriteRootElement("SECSGEMData");

            //        // Rectangle gauge template measurement result
            //        objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_WhiteOnBlack", "NA");
            //        objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_ThresholdValue", "NA");
            //        objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_FilterMinArea", "NA");
            //        objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_FilterMaxArea", "NA");
            //        objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_TotalTemplateBlobObject", "NA");
            //        objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_HalfPitch", "NA");
            //        objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_UnitROICountX", "NA");
            //        objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_UnitROICountY", "NA");

            //        for (int i = 0; i < 10; i++)
            //        {
            //            //objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i, "");
            //            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_LimitCenterX", "NA");
            //            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_LimitCenterY", "NA");
            //            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_Width", "NA");
            //            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_Height", "NA");
            //            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_Area", "NA");
            //            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_StartX", "NA");
            //            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_StartY", "NA");
            //            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_EndX", "NA");
            //            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_EndY", "NA");
            //            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_MinArea", "NA");
            //            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_MinOffSet", "NA");
            //            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_ROIStartX", "NA");
            //            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_ROIStartY", "NA");
            //            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_ROIWidth", "NA");
            //            objFile.WriteElementValue(strVisionName + "_UnitPresentSetting" + "_BlobObject" + i + "_ROIHeight", "NA");
            //        }

            //        objFile.WriteEndElement();
            //    }
            //}
            //if (smVisionInfo.g_blnWantCheckPH)
            //{
            //    //ROI.LoadFile(strFolderPath + "Positioning\\PHROI.xml", smVisionInfo.g_arrPHROIs);
            //    if (smVisionInfo.g_objPositioning != null)
            //        smVisionInfo.g_objPositioning.SavePosition_SECSGEM(strPath, "Position", strVisionName);
            //    else
            //    {
            //        XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            //        objFile.WriteRootElement("SECSGEMData");

            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionImageIndex", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyImageIndex", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinBorderScore", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinEmptyScore", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_AngleLimit", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionXTolerance", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionYTolerance", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieWidth", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieHeight", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_Method", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PRSMode", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue2", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateX", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateY", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipThreshold", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipAreaLimit", "NA");
            //        //Empty
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyThreshold", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyWhiteArea", "NA");
            //        //PH
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHThreshold", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHBlackArea", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHMinArea", "NA");

            //        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_MinScore", "NA");
            //        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionX", "NA");
            //        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionY", "NA");
            //        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PosTolerance", "NA");
            //        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_Direction", "NA");

            //        objFile.WriteEndElement();
            //    }
            //}

            //if (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM"))
            //{
            //    if (smVisionInfo.g_objPositioning != null)
            //        smVisionInfo.g_objPositioning.SavePosition_SECSGEM(strPath, "Position", strVisionName);
            //    else
            //    {
            //        XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            //        objFile.WriteRootElement("SECSGEMData");

            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionImageIndex", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyImageIndex", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinBorderScore", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_MinEmptyScore", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_AngleLimit", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionXTolerance", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PositionYTolerance", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieWidth", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_DieHeight", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_Method", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PRSMode", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_GainValue2", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateX", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_CompensateY", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipThreshold", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_FlipAreaLimit", "NA");
            //        //Empty
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyThreshold", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_EmptyWhiteArea", "NA");
            //        //PH
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHThreshold", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHBlackArea", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + "Position" + "_PHMinArea", "NA");

            //        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_MinScore", "NA");
            //        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionX", "NA");
            //        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PositionY", "NA");
            //        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_PosTolerance", "NA");
            //        objFile.WriteElement1Value(strVisionName + "_" + "PositionOrient" + "_Direction", "NA");

            //        objFile.WriteEndElement();
            //    }

            //    //if (File.Exists(strFolderPath + "Positioning\\" + "ROI.xml"))
            //    //    ROI.LoadFile(strFolderPath + "Positioning\\ROI.xml", smVisionInfo.g_arrPositioningROIs);
            //}
        }

    }
}
