using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Common;
using SharedMemory;
using System.Runtime.InteropServices;


namespace SRMVision
{
    public class SaveRecipeThread
    {

        #region Member Variables

        private readonly object m_objStopLock = new object();
        private bool m_blnStopping = false;
        private bool m_blnStopped = false;
        private Thread m_thThread;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        private VisionInfo[] m_smVisionInfo;
        #endregion

        public SaveRecipeThread(ProductionInfo smProductionInfo, CustomOption smCustomizeInfo, VisionInfo[] smVisionInfo)
        {
            m_smVisionInfo = smVisionInfo;
            m_smProductionInfo = smProductionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_thThread = new Thread(new ThreadStart(UpdateProgress));
            m_thThread.IsBackground = true;
            m_thThread.Priority = ThreadPriority.Highest;
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

        private bool UpdateRecipeToServerOrLocal()
        {
            /*
            // Scenario 1: Current Recipe From Server Save to Local (When user select "Save to Local", software will set the uWantNetwork to 0 before local mode before trigger the CH_VS_SaveSetting event)
            // Scenario 2: Current Recipe Form Server Save to Server Same Name
            // Scenario 3: Current Recipe From Server Save to Server Different Name
            */

            if (NetworkTransfer.IsConnectionPass(m_smCustomizeInfo.g_strHostIP))
            {
                for (int i = 0; i < m_smVisionInfo.Length; i++)
                {
                    string strDeviceNoPath = m_smCustomizeInfo.g_strDeviceUploadDir + "\\DeviceNo\\" + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo[i].g_intVisionIndex];  // e.g 169.192.0.1\ServerFolder\Default

                    // Create Recipe Folder in server if no exist 
                    if (!Directory.Exists(strDeviceNoPath))
                    {
                        Directory.CreateDirectory(strDeviceNoPath);
                    }

                    // Make sure TempRecipe folder is exist
                    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "TempRecipe\\" + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo[i].g_intVisionIndex]))
                    {
                        return false;
                    }

                    // Transfer file from TempRecipe to Server Recipe
                    NetworkTransfer.TransferFile_MappedNetwork(AppDomain.CurrentDomain.BaseDirectory + "TempRecipe\\" + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo[i].g_intVisionIndex], strDeviceNoPath);
                    
                }
                return true;
            }
            else
            {
                return true;
            }
        }

        private void UpdateProgress()
        {
            try
            {
                while (!m_blnStopping)
                {

                    if (m_smProductionInfo.g_blnSaveRecipeToServer)
                    {
                        m_smProductionInfo.g_blnSaveRecipeToServer = false;
                        UpdateRecipeToServerOrLocal();
                    }

                    Thread.Sleep(10);
                }

            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("RecipeThread->UpdateProgress(): " + ex.ToString());
            }
            finally
            {
                SetStopped();
                m_thThread = null;
            }

            STTrackLog.WriteLine("End RecipeThread -> UpdateProgress()");
        }

    }




}
