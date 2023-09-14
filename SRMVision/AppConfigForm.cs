using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Common;
using SharedMemory;
using System.IO;
using ImageAcquisition;
using VisionProcessing;
using SharedMemory;

namespace SRMVision
{
      public struct Vision
    {
        public string ref_strVisionName;
        public int ref_intVisionNo;
    }


    public partial class AppConfigForm : Form
    {
        #region Member Variables
        private bool m_blnGlobalSharingCalibrationDataPrev = false;
        private bool m_blnGlobalSharingCameraDataPrev = false;

        private List<Vision> m_arrVisionList = new List<Vision>();
        XmlParser m_FileHandle = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo = new ProductionInfo();
        private VisionInfo[] m_smVSInfo;
        private TeliCameraAPI m_objTeliCamera = new TeliCameraAPI();
        private AVTVimba m_objAVTFireGrab = new AVTVimba();
        private List<int> m_arrCameraPortNoListAVT = new List<int>();
        private List<string> m_arrCameraIDListAVT = new List<string>();
        private List<string> m_arrCameraModelListAVT = new List<string>();
        private List<string> m_arrCameraSerialNumberListAVT = new List<string>();
        private List<int> m_arrCameraPortNoListTeli = new List<int>();
        private List<string> m_arrCameraIDListTeli = new List<string>();
        private List<string> m_arrCameraModelListTeli = new List<string>();
        private List<string> m_arrCameraSerialNUmberListTeli = new List<string>();
        private bool m_blnCameraIDMatched = false;
        private bool m_blnInitDone = false;
        private string Vision1Prev = "";
        private string Vision2Prev = "";
        private string Vision3Prev = "";
        private string Vision4Prev = "";
        private string Vision5Prev = "";
        private string Vision6Prev = "";
        private string Vision7Prev = "";
        private List<string> m_arrVisionName = new List<string>();
        #endregion

        public AppConfigForm(VisionInfo[] VSinfo, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVSInfo = VSinfo;
            InitAvailableVision();
            InitSetting();
            InitNetworkSetting();
            InitSECSGEMSetting();
            InitTCPSetting();
            InitSerialPortSetting();
            ClearItem();
            UpdateVisionGUI();
            //int count = 0;
            //for (int i = 0; i < m_smVSInfo.Length; i++)
            //{
            //    if (m_smVSInfo[i] == null)
            //    {
            //        count = i;
            //        break;
            //    }
            //}
            //for (int i = 0; i < count-1; i++)
            //{
            //    if (m_smVSInfo[i].g_strCameraModel == "AVT")
                    UpdateCameraGUI_AVT();
            //    else if (m_smVSInfo[i].g_strCameraModel == "Teli")
                    UpdateCameraGUI_Teli();
            //}
            m_blnInitDone = true;
        }
        private void ClearItem()
        {
            cbo_Vision1.Items.Clear();
            cbo_Vision2.Items.Clear();
            cbo_Vision3.Items.Clear();
            cbo_Vision4.Items.Clear();
            cbo_Vision5.Items.Clear();
            cbo_Vision6.Items.Clear();
            cbo_Vision7.Items.Clear();
            cbo_1.Items.Clear();
            cbo_2.Items.Clear();
            cbo_3.Items.Clear();
            cbo_4.Items.Clear();
            cbo_5.Items.Clear();
            cbo_6.Items.Clear();
            cbo_7.Items.Clear();

        }
        private void UpdateVisionGUI()
        {
            int count = 0;
            for (int i=0;i< m_smVSInfo.Length;i++)
            {
                if (m_smVSInfo[i] == null)
                {
                    count = i;
                    break;
                }
            }

            switch (count)
            {
                case 0:
                    RemoveVision1();
                    RemoveVision2();
                    RemoveVision3();
                    RemoveVision4();
                    RemoveVision5();
                    RemoveVision6();
                    RemoveVision7();
                    break;
                case 1:
                    RemoveVision2();
                    RemoveVision3();
                    RemoveVision4();
                    RemoveVision5();
                    RemoveVision6();
                    RemoveVision7();
                    break;
                case 2:
                    RemoveVision3();
                    RemoveVision4();
                    RemoveVision5();
                    RemoveVision6();
                    RemoveVision7();
                    break;
                case 3:
                    RemoveVision4();
                    RemoveVision5();
                    RemoveVision6();
                    RemoveVision7();
                    break;
                case 4: 
                    RemoveVision5();
                    RemoveVision6();
                    RemoveVision7();
                    break;
                case 5:
                    RemoveVision6();
                    RemoveVision7();
                    break;
                case 6:
                    RemoveVision7();
                    break;
                default:

                    break;
            }
        }

        private void RemoveVision1()
        {
            lbl_Vision1.Visible = false;
            lbl_Vision1Status.Visible = false;
            cbo_Vision1.Visible = false;
        }
        private void RemoveVision2()
        {
            lbl_Vision2.Visible = false;
            lbl_Vision2Status.Visible = false;
            cbo_Vision2.Visible = false;
        }
        private void RemoveVision3()
        {
            lbl_Vision3.Visible = false;
            lbl_Vision3Status.Visible = false;
            cbo_Vision3.Visible = false;
        }
        private void RemoveVision4()
        {
            lbl_Vision4.Visible = false;
            lbl_Vision4Status.Visible = false;
            cbo_Vision4.Visible = false;
        }
        private void RemoveVision5()
        {
            lbl_Vision5.Visible = false;
            lbl_Vision5Status.Visible = false;
            cbo_Vision5.Visible = false;
        }
        private void RemoveVision6()
        {
            lbl_Vision6.Visible = false;
            lbl_Vision6Status.Visible = false;
            cbo_Vision6.Visible = false;
        }
        private void RemoveVision7()
        {
            lbl_Vision7.Visible = false;
            lbl_Vision7Status.Visible = false;
            cbo_Vision7.Visible = false;
        }
        private void UpdateCameraGUI_AVT()
        {
            getCameraSerialNoAVT();
            updateConnectionStatusAVT();
        }
        private void UpdateCameraGUI_Teli()
        {
            getCameraSerialNoTeli();
            updateConnectionStatusTeli();
        }
        private void getCameraSerialNoAVT()
        {
            m_arrCameraPortNoListAVT.Clear();
            m_arrCameraIDListAVT.Clear();
            m_arrCameraModelListAVT.Clear();
            m_arrCameraSerialNumberListAVT.Clear();
            m_objAVTFireGrab.GetNodeList2(ref m_arrCameraPortNoListAVT, ref m_arrCameraIDListAVT, ref m_arrCameraModelListAVT, ref m_arrCameraSerialNumberListAVT);
           // m_objAVTFireGrab.GetNodeList2(ref m_arrCameraPortNoListAVT, ref m_arrCameraIDListAVT, ref m_arrCameraModelListAVT, ref m_arrCameraSerialNumberListAVT);
           // for (int i=0;i<m_arrCameraSerialNumberListAVT.Count;i++)
            //t.WriteLine( " PortNo "+m_arrCameraPortNoListAVT[i].ToString()+ " ID " + m_arrCameraIDListAVT[i]+ " Model " + m_arrCameraModelListAVT[i]+ " SerialNo " + m_arrCameraSerialNumberListAVT[i]);
        }
        private void getCameraSerialNoTeli()
        {
            m_objTeliCamera.GetNodeList(ref m_arrCameraPortNoListTeli, ref m_arrCameraIDListTeli, ref m_arrCameraModelListTeli);  
        }

        private void updateConnectionStatusAVT()
        {
            for (int j = 0; j < m_arrCameraSerialNumberListAVT.Count; j++)
            {
                m_blnCameraIDMatched = false;

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    if (m_smVSInfo[i].g_strVisionFolderName == "Vision1")
                    {
                        if (m_smVSInfo[i].g_strCameraSerialNo == m_arrCameraSerialNumberListAVT[j])
                        {
                            if (!OpenAndCloseCameraAVT(m_smVSInfo[i].g_strCameraSerialNo, m_smVSInfo[i].g_intCameraPortNo))//,true))
                            {
                                m_blnCameraIDMatched = false;
                                break;
                            }
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                lbl_Vision1Status.Text = "Connected";
                            else
                                lbl_Vision1Status.Text = "已连接";
                            lbl_Vision1Status.ForeColor = Color.Green;
                            cbo_Vision1.Items.Add(m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision1Status.Text);
                            cbo_Vision1.SelectedItem = m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision1Status.Text;
                            Vision1Prev = cbo_Vision1.SelectedItem.ToString();
                            cbo_1.Items.Add(m_smVSInfo[i].g_intCameraPortNo);
                            cbo_1.SelectedItem = m_smVSInfo[i].g_intCameraPortNo;
                           
                            m_blnCameraIDMatched = true;
                            break;
                        }


                    }
                    else if (m_smVSInfo[i].g_strVisionFolderName == "Vision2")
                    {
                        if (m_smVSInfo[i].g_strCameraSerialNo == m_arrCameraSerialNumberListAVT[j])
                        {
                            if (!OpenAndCloseCameraAVT(m_smVSInfo[i].g_strCameraSerialNo, m_smVSInfo[i].g_intCameraPortNo))//,true))
                            {
                                m_blnCameraIDMatched = false;
                                break;
                            }
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                lbl_Vision2Status.Text = "Connected";
                            else
                                lbl_Vision2Status.Text = "已连接";
                            lbl_Vision2Status.ForeColor = Color.Green;
                            cbo_Vision2.Items.Add(m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision2Status.Text);
                            cbo_Vision2.SelectedItem = m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision2Status.Text;
                            Vision2Prev = cbo_Vision2.SelectedItem.ToString();
                            cbo_2.Items.Add(m_smVSInfo[i].g_intCameraPortNo);
                            cbo_2.SelectedItem = m_smVSInfo[i].g_intCameraPortNo;
                            
                            m_blnCameraIDMatched = true;
                            break;
                        }
                    }
                    else if (m_smVSInfo[i].g_strVisionFolderName == "Vision3")
                    {
                        if (m_smVSInfo[i].g_strCameraSerialNo == m_arrCameraSerialNumberListAVT[j])
                        {
                            if (!OpenAndCloseCameraAVT(m_smVSInfo[i].g_strCameraSerialNo, m_smVSInfo[i].g_intCameraPortNo))//, true))
                            {
                                m_blnCameraIDMatched = false;
                                break;
                            }
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                lbl_Vision3Status.Text = "Connected";
                            else
                                lbl_Vision3Status.Text = "已连接";
                            lbl_Vision3Status.ForeColor = Color.Green;
                            cbo_Vision3.Items.Add(m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision3Status.Text);
                            cbo_Vision3.SelectedItem = m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision3Status.Text;
                            Vision3Prev = cbo_Vision3.SelectedItem.ToString();
                            cbo_3.Items.Add(m_smVSInfo[i].g_intCameraPortNo);
                            cbo_3.SelectedItem = m_smVSInfo[i].g_intCameraPortNo;
                            
                            m_blnCameraIDMatched = true;
                            break;
                        }


                    }
                    else if (m_smVSInfo[i].g_strVisionFolderName == "Vision4")
                    {
                        if (m_smVSInfo[i].g_strCameraSerialNo == m_arrCameraSerialNumberListAVT[j])
                        {
                            if (!OpenAndCloseCameraAVT(m_smVSInfo[i].g_strCameraSerialNo, m_smVSInfo[i].g_intCameraPortNo))//, true))
                            {
                                m_blnCameraIDMatched = false;
                                break;
                            }
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                lbl_Vision4Status.Text = "Connected";
                            else
                                lbl_Vision4Status.Text = "已连接";
                            lbl_Vision4Status.ForeColor = Color.Green;
                            cbo_Vision4.Items.Add(m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision4Status.Text);
                            cbo_Vision4.SelectedItem = m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision4Status.Text;
                            Vision4Prev = cbo_Vision4.SelectedItem.ToString();
                            cbo_4.Items.Add(m_smVSInfo[i].g_intCameraPortNo);
                            cbo_4.SelectedItem = m_smVSInfo[i].g_intCameraPortNo;
                            
                            m_blnCameraIDMatched = true;
                            break;
                        }


                    }
                    else if (m_smVSInfo[i].g_strVisionFolderName == "Vision5")
                    {
                        if (m_smVSInfo[i].g_strCameraSerialNo == m_arrCameraSerialNumberListAVT[j])
                        {
                            if (!OpenAndCloseCameraAVT(m_smVSInfo[i].g_strCameraSerialNo, m_smVSInfo[i].g_intCameraPortNo))//, true))
                            {
                                m_blnCameraIDMatched = false;
                                break;
                            }
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                lbl_Vision5Status.Text = "Connected";
                            else
                                lbl_Vision5Status.Text = "已连接";
                            lbl_Vision5Status.ForeColor = Color.Green;
                            cbo_Vision5.Items.Add(m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision5Status.Text);
                            cbo_Vision5.SelectedItem = m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision5Status.Text;
                            Vision5Prev = cbo_Vision5.SelectedItem.ToString();
                            cbo_5.Items.Add(m_smVSInfo[i].g_intCameraPortNo);
                            cbo_5.SelectedItem = m_smVSInfo[i].g_intCameraPortNo;

                            m_blnCameraIDMatched = true;
                            break;
                        }


                    }
                    else if (m_smVSInfo[i].g_strVisionFolderName == "Vision6")
                    {
                        if (m_smVSInfo[i].g_strCameraSerialNo == m_arrCameraSerialNumberListAVT[j])
                        {
                            if (!OpenAndCloseCameraAVT(m_smVSInfo[i].g_strCameraSerialNo, m_smVSInfo[i].g_intCameraPortNo))//, true))
                            {
                                m_blnCameraIDMatched = false;
                                break;
                            }
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                lbl_Vision6Status.Text = "Connected";
                            else
                                lbl_Vision6Status.Text = "已连接";
                            lbl_Vision6Status.ForeColor = Color.Green;
                            cbo_Vision6.Items.Add(m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision6Status.Text);
                            cbo_Vision6.SelectedItem = m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision6Status.Text;
                            Vision6Prev = cbo_Vision6.SelectedItem.ToString();
                            cbo_6.Items.Add(m_smVSInfo[i].g_intCameraPortNo);
                            cbo_6.SelectedItem = m_smVSInfo[i].g_intCameraPortNo;

                            m_blnCameraIDMatched = true;
                            break;
                        }


                    }
                    else if (m_smVSInfo[i].g_strVisionFolderName == "Vision7")
                    {
                        if (m_smVSInfo[i].g_strCameraSerialNo == m_arrCameraSerialNumberListAVT[j])
                        {
                            if (!OpenAndCloseCameraAVT(m_smVSInfo[i].g_strCameraSerialNo, m_smVSInfo[i].g_intCameraPortNo))//, true))
                            {
                                m_blnCameraIDMatched = false;
                                break;
                            }
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                lbl_Vision7Status.Text = "Connected";
                            else
                                lbl_Vision7Status.Text = "已连接";
                            lbl_Vision7Status.ForeColor = Color.Green;
                            cbo_Vision7.Items.Add(m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision7Status.Text);
                            cbo_Vision7.SelectedItem = m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision7Status.Text;
                            Vision7Prev = cbo_Vision7.SelectedItem.ToString();
                            cbo_7.Items.Add(m_smVSInfo[i].g_intCameraPortNo);
                            cbo_7.SelectedItem = m_smVSInfo[i].g_intCameraPortNo;

                            m_blnCameraIDMatched = true;
                            break;
                        }


                    }
                }

                if (!m_blnCameraIDMatched)
                {
                    //if the serial number does not match with either one of the serial number in the registry, add into combo box and then initialize the camera

                    //cbo_Vision1.Items.Add(m_arrCameraSerialNumberListAVT[j] + " (" + m_smVSInfo[j].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision1Status.Text);
                    //cbo_Vision2.Items.Add(m_arrCameraSerialNumberListAVT[j] + " (" + m_smVSInfo[j].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision2Status.Text);
                    //cbo_Vision3.Items.Add(m_arrCameraSerialNumberListAVT[j] + " (" + m_smVSInfo[j].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision3Status.Text);
                    //cbo_Vision4.Items.Add(m_arrCameraSerialNumberListAVT[j] + " (" + m_smVSInfo[j].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision4Status.Text);
                    //cbo_Vision5.Items.Add(m_arrCameraSerialNumberListAVT[j] + " (" + m_smVSInfo[j].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision5Status.Text);
                    //cbo_Vision6.Items.Add(m_arrCameraSerialNumberListAVT[j] + " (" + m_smVSInfo[j].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision6Status.Text);
                    //cbo_Vision7.Items.Add(m_arrCameraSerialNumberListAVT[j] + " (" + m_smVSInfo[j].g_strCameraModel + " - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision7Status.Text);
                    cbo_Vision1.Items.Add(m_arrCameraSerialNumberListAVT[j] + " (AVT - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision1Status.Text);
                    cbo_Vision2.Items.Add(m_arrCameraSerialNumberListAVT[j] + " (AVT - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision2Status.Text);
                    cbo_Vision3.Items.Add(m_arrCameraSerialNumberListAVT[j] + " (AVT - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision3Status.Text);
                    cbo_Vision4.Items.Add(m_arrCameraSerialNumberListAVT[j] + " (AVT - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision4Status.Text);
                    cbo_Vision5.Items.Add(m_arrCameraSerialNumberListAVT[j] + " (AVT - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision5Status.Text);
                    cbo_Vision6.Items.Add(m_arrCameraSerialNumberListAVT[j] + " (AVT - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision6Status.Text);
                    cbo_Vision7.Items.Add(m_arrCameraSerialNumberListAVT[j] + " (AVT - " + m_arrCameraModelListAVT[j] + ") - " + lbl_Vision7Status.Text);

                    cbo_5.Items.Add(m_arrCameraPortNoListAVT[j]);
                    cbo_6.Items.Add(m_arrCameraPortNoListAVT[j]);
                    cbo_7.Items.Add(m_arrCameraPortNoListAVT[j]);
                    cbo_1.Items.Add(m_arrCameraPortNoListAVT[j]);
                    cbo_2.Items.Add(m_arrCameraPortNoListAVT[j]);
                    cbo_3.Items.Add(m_arrCameraPortNoListAVT[j]);
                    cbo_4.Items.Add(m_arrCameraPortNoListAVT[j]);
                    m_objAVTFireGrab.InitializeCamera(m_arrCameraSerialNumberListAVT[j], m_arrCameraPortNoListAVT[j]);//,false);
                }
            }
        }
        private void updateConnectionStatusTeli()
        {
            for (int j = 0; j < m_arrCameraIDListTeli.Count; j++)
            {
                m_blnCameraIDMatched = false;

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    if (m_smVSInfo[i].g_strVisionFolderName == "Vision1") 
                    {
                        if (m_smVSInfo[i].g_strCameraSerialNo == m_arrCameraIDListTeli[j])
                        {
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                lbl_Vision1Status.Text = "Connected";
                            else
                                lbl_Vision1Status.Text = "已连接";
                            lbl_Vision1Status.ForeColor = Color.Green;
                            cbo_Vision1.Items.Add(m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel +" - "+ m_arrCameraModelListTeli[j]+") - "+lbl_Vision1Status.Text);
                            cbo_Vision1.SelectedItem = m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision1Status.Text;
                            Vision1Prev = cbo_Vision1.SelectedItem.ToString();
                            //string[] SerialNo = new string[2];
                            //if (cbo_Vision1.SelectedValue.ToString() != " " || cbo_Vision1.SelectedValue.ToString() != null)
                            //    SerialNo = cbo_Vision1.SelectedValue.ToString().Split('(');
                            //else SerialNo[0] = " ";
                            OpenAndCloseCameraTeli(m_smVSInfo[i].g_strCameraSerialNo, "");
                            m_blnCameraIDMatched = true;
                            break;
                        }

                      
                    }
                    else if (m_smVSInfo[i].g_strVisionFolderName == "Vision2")
                    {
                        if (m_smVSInfo[i].g_strCameraSerialNo == m_arrCameraIDListTeli[j])
                        {
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                lbl_Vision2Status.Text = "Connected";
                            else
                                lbl_Vision2Status.Text = "已连接";
                            lbl_Vision2Status.ForeColor = Color.Green;
                            cbo_Vision2.Items.Add(m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision2Status.Text);
                            cbo_Vision2.SelectedItem = m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision2Status.Text;
                            Vision2Prev = cbo_Vision2.SelectedItem.ToString();
                            //string[] SerialNo = new string[2];
                            //if (cbo_Vision2.SelectedValue.ToString() != " " || cbo_Vision2.SelectedValue.ToString() != null)
                            //    SerialNo = cbo_Vision2.SelectedValue.ToString().Split('(');
                            //else SerialNo[0] = " ";
                            OpenAndCloseCameraTeli(m_smVSInfo[i].g_strCameraSerialNo, ""); 
                            m_blnCameraIDMatched = true;
                            break;
                        }

                       
                    }
                    else if (m_smVSInfo[i].g_strVisionFolderName == "Vision3")
                    {
                        if (m_smVSInfo[i].g_strCameraSerialNo == m_arrCameraIDListTeli[j])
                        {
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                lbl_Vision3Status.Text = "Connected";
                            else
                                lbl_Vision3Status.Text = "已连接";
                            lbl_Vision3Status.ForeColor = Color.Green;
                            cbo_Vision3.Items.Add(m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision3Status.Text);
                            cbo_Vision3.SelectedItem = m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision3Status.Text;
                            Vision3Prev = cbo_Vision3.SelectedItem.ToString();
                            //string[] SerialNo = new string[2];
                            //if (cbo_Vision3.SelectedValue.ToString() != " " || cbo_Vision3.SelectedValue.ToString() != null)
                            //    SerialNo = cbo_Vision3.SelectedValue.ToString().Split('(');
                            //else SerialNo[0] = " ";
                            OpenAndCloseCameraTeli(m_smVSInfo[i].g_strCameraSerialNo, "");
                            m_blnCameraIDMatched = true;
                            break;
                        }

                      
                    }
                    else if (m_smVSInfo[i].g_strVisionFolderName == "Vision4")
                    {
                        if (m_smVSInfo[i].g_strCameraSerialNo == m_arrCameraIDListTeli[j])
                        {
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                lbl_Vision4Status.Text = "Connected";
                            else
                                lbl_Vision4Status.Text = "已连接";
                            lbl_Vision4Status.ForeColor = Color.Green;
                            cbo_Vision4.Items.Add(m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision4Status.Text);
                            cbo_Vision4.SelectedItem = m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision4Status.Text;
                            Vision4Prev = cbo_Vision4.SelectedItem.ToString();
                            //string[] SerialNo = new string[2];
                            //if (cbo_Vision4.SelectedValue.ToString() != " " || cbo_Vision4.SelectedValue.ToString() != null)
                            //    SerialNo = cbo_Vision4.SelectedValue.ToString().Split('(');
                            //else SerialNo[0] = " ";
                            OpenAndCloseCameraTeli(m_smVSInfo[i].g_strCameraSerialNo, "");
                            m_blnCameraIDMatched = true;
                            break;
                        }

                      
                    }
                    else if (m_smVSInfo[i].g_strVisionFolderName == "Vision5")
                    {
                        if (m_smVSInfo[i].g_strCameraSerialNo == m_arrCameraIDListTeli[j])
                        {
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                lbl_Vision5Status.Text = "Connected";
                            else
                                lbl_Vision5Status.Text = "已连接";
                            lbl_Vision5Status.ForeColor = Color.Green;
                            cbo_Vision5.Items.Add(m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision5Status.Text);
                            cbo_Vision5.SelectedItem = m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision5Status.Text;
                            Vision5Prev = cbo_Vision5.SelectedItem.ToString();
                            //string[] SerialNo = new string[2];
                            //if (cbo_Vision4.SelectedValue.ToString() != " " || cbo_Vision4.SelectedValue.ToString() != null)
                            //    SerialNo = cbo_Vision4.SelectedValue.ToString().Split('(');
                            //else SerialNo[0] = " ";
                            OpenAndCloseCameraTeli(m_smVSInfo[i].g_strCameraSerialNo, "");
                            m_blnCameraIDMatched = true;
                            break;
                        }


                    }
                    else if (m_smVSInfo[i].g_strVisionFolderName == "Vision6")
                    {
                        if (m_smVSInfo[i].g_strCameraSerialNo == m_arrCameraIDListTeli[j])
                        {
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                lbl_Vision6Status.Text = "Connected";
                            else
                                lbl_Vision6Status.Text = "已连接";
                            lbl_Vision6Status.ForeColor = Color.Green;
                            cbo_Vision6.Items.Add(m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision6Status.Text);
                            cbo_Vision6.SelectedItem = m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision6Status.Text;
                            Vision6Prev = cbo_Vision6.SelectedItem.ToString();
                            //string[] SerialNo = new string[2];
                            //if (cbo_Vision4.SelectedValue.ToString() != " " || cbo_Vision4.SelectedValue.ToString() != null)
                            //    SerialNo = cbo_Vision4.SelectedValue.ToString().Split('(');
                            //else SerialNo[0] = " ";
                            OpenAndCloseCameraTeli(m_smVSInfo[i].g_strCameraSerialNo, "");
                            m_blnCameraIDMatched = true;
                            break;
                        }


                    }
                    else if (m_smVSInfo[i].g_strVisionFolderName == "Vision7")
                    {
                        if (m_smVSInfo[i].g_strCameraSerialNo == m_arrCameraIDListTeli[j])
                        {
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                lbl_Vision7Status.Text = "Connected";
                            else
                                lbl_Vision7Status.Text = "已连接";
                            lbl_Vision7Status.ForeColor = Color.Green;
                            cbo_Vision7.Items.Add(m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision7Status.Text);
                            cbo_Vision7.SelectedItem = m_smVSInfo[i].g_strCameraSerialNo + " (" + m_smVSInfo[i].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision7Status.Text;
                            Vision7Prev = cbo_Vision7.SelectedItem.ToString();
                            //string[] SerialNo = new string[2];
                            //if (cbo_Vision4.SelectedValue.ToString() != " " || cbo_Vision4.SelectedValue.ToString() != null)
                            //    SerialNo = cbo_Vision4.SelectedValue.ToString().Split('(');
                            //else SerialNo[0] = " ";
                            OpenAndCloseCameraTeli(m_smVSInfo[i].g_strCameraSerialNo, "");
                            m_blnCameraIDMatched = true;
                            break;
                        }


                    }
                }

                if (!m_blnCameraIDMatched)
                {
                    //if the serial number does not match with either one of the serial number in the registry, add into combo box and then initialize the camera

                    //cbo_Vision1.Items.Add(m_arrCameraIDListTeli[j] + " (" + m_smVSInfo[j].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision1Status.Text);
                    //cbo_Vision2.Items.Add(m_arrCameraIDListTeli[j] + " (" + m_smVSInfo[j].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision2Status.Text);
                    //cbo_Vision3.Items.Add(m_arrCameraIDListTeli[j] + " (" + m_smVSInfo[j].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision3Status.Text);
                    //cbo_Vision4.Items.Add(m_arrCameraIDListTeli[j] + " (" + m_smVSInfo[j].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision4Status.Text);
                    //cbo_Vision5.Items.Add(m_arrCameraIDListTeli[j] + " (" + m_smVSInfo[j].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision5Status.Text);
                    //cbo_Vision6.Items.Add(m_arrCameraIDListTeli[j] + " (" + m_smVSInfo[j].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision6Status.Text);
                    //cbo_Vision7.Items.Add(m_arrCameraIDListTeli[j] + " (" + m_smVSInfo[j].g_strCameraModel + " - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision7Status.Text);
                    cbo_Vision1.Items.Add(m_arrCameraIDListTeli[j] + " (Teli - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision1Status.Text);
                    cbo_Vision2.Items.Add(m_arrCameraIDListTeli[j] + " (Teli - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision2Status.Text);
                    cbo_Vision3.Items.Add(m_arrCameraIDListTeli[j] + " (Teli - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision3Status.Text);
                    cbo_Vision4.Items.Add(m_arrCameraIDListTeli[j] + " (Teli - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision4Status.Text);
                    cbo_Vision5.Items.Add(m_arrCameraIDListTeli[j] + " (Teli - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision5Status.Text);
                    cbo_Vision6.Items.Add(m_arrCameraIDListTeli[j] + " (Teli - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision6Status.Text);
                    cbo_Vision7.Items.Add(m_arrCameraIDListTeli[j] + " (Teli - " + m_arrCameraModelListTeli[j] + ") - " + lbl_Vision7Status.Text);

                    m_objTeliCamera.InitializeCamera(m_arrCameraIDListTeli[j]);
                }
            }
        }
        private void OpenAndCloseCameraTeli(string SerialNo, string SerialNoPrev)
        {
            m_objTeliCamera.OpenCameraOnly(SerialNo);
            m_objTeliCamera.CloseCameraOnly(SerialNoPrev);

        }
        private bool OpenAndCloseCameraAVT(string SerialNo, int PortNo)//, bool display)
        {
           // m_objAVTFireGrab.OFFCamera();
           // m_objAVTFireGrab.StartCamera();

            if (!m_objAVTFireGrab.InitializeCamera(SerialNo, PortNo))// , display))//.VerifyCamera(SerialNo, PortNo,count))
                return false;
            
                return true;
            //m_objAVTFireGrab.CloseCameraOnly(SerialNoPrev);
        }
        private void cbo_Vision1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (cbo_Vision1.SelectedItem.ToString() == Vision1Prev)
                return;
            string[] SerialNo = new string[2];
            if (cbo_Vision1.SelectedItem.ToString() != " " && cbo_Vision1.SelectedItem.ToString() != null)
            {
                SerialNo = cbo_Vision1.SelectedItem.ToString().Split('(');
                SerialNo[0] = SerialNo[0].TrimEnd();
            }
            else SerialNo[0] = "";

            string[] SerialNoPrev = new string[2];
            SerialNoPrev = Vision1Prev.Split('(');
            SerialNoPrev[0] = SerialNoPrev[0].TrimEnd();

            if (SerialNo[1].Contains("Teli"))
                OpenAndCloseCameraTeli(SerialNo[0], SerialNoPrev[0]);
            else if (SerialNo[1].Contains("AVT"))
                OpenAndCloseCameraAVT(SerialNo[0], Convert.ToInt32(cbo_1.Items[cbo_Vision1.SelectedIndex].ToString()));//, false);

            Vision1Prev = cbo_Vision1.SelectedItem.ToString();
            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                lbl_Vision1Status.Text = "Not Connected";
            else
                lbl_Vision1Status.Text = "未连接";
            lbl_Vision1Status.ForeColor = Color.Red;

            for (int i = 0; i < cbo_Vision1.Items.Count; i++)
            {
                if (!cbo_Vision2.Items.Contains(cbo_Vision1.Items[i]))
                    cbo_Vision2.Items.Add(cbo_Vision1.Items[i]);

                if (!cbo_Vision3.Items.Contains(cbo_Vision1.Items[i]))
                    cbo_Vision3.Items.Add(cbo_Vision1.Items[i]);

                if (!cbo_Vision4.Items.Contains(cbo_Vision1.Items[i]))
                    cbo_Vision4.Items.Add(cbo_Vision1.Items[i]);

                if (!cbo_Vision5.Items.Contains(cbo_Vision1.Items[i]))
                    cbo_Vision5.Items.Add(cbo_Vision1.Items[i]);

                if (!cbo_Vision6.Items.Contains(cbo_Vision1.Items[i]))
                    cbo_Vision6.Items.Add(cbo_Vision1.Items[i]);

                if (!cbo_Vision7.Items.Contains(cbo_Vision1.Items[i]))
                    cbo_Vision7.Items.Add(cbo_Vision1.Items[i]);
            }

            if (cbo_Vision1.SelectedItem.ToString() == " "|| cbo_Vision1.SelectedItem.ToString() == "")
                return;

            cbo_Vision2.Items.Remove(cbo_Vision1.SelectedItem);
            cbo_Vision3.Items.Remove(cbo_Vision1.SelectedItem);
            cbo_Vision4.Items.Remove(cbo_Vision1.SelectedItem);
            cbo_Vision5.Items.Remove(cbo_Vision1.SelectedItem);
            cbo_Vision6.Items.Remove(cbo_Vision1.SelectedItem);
            cbo_Vision7.Items.Remove(cbo_Vision1.SelectedItem);
        }

        private void cbo_Vision2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (cbo_Vision2.SelectedItem.ToString() == Vision2Prev)
                return;
            string[] SerialNo = new string[2];
            if (cbo_Vision2.SelectedItem.ToString() != " " && cbo_Vision2.SelectedItem.ToString() != null)
            {
                SerialNo = cbo_Vision2.SelectedItem.ToString().Split('(');
                SerialNo[0] = SerialNo[0].TrimEnd();
            }
            else SerialNo[0] = "";

            string[] SerialNoPrev = new string[2];
            SerialNoPrev = Vision2Prev.Split('(');
            SerialNoPrev[0] = SerialNoPrev[0].TrimEnd();

           if(SerialNo[1].Contains("Teli"))
            OpenAndCloseCameraTeli(SerialNo[0], SerialNoPrev[0]);
           else if (SerialNo[1].Contains("AVT"))
                OpenAndCloseCameraAVT(SerialNo[0], Convert.ToInt32(cbo_2.Items[cbo_Vision2.SelectedIndex].ToString()));//, false);
            Vision2Prev = cbo_Vision2.SelectedItem.ToString();
            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                lbl_Vision2Status.Text = "Not Connected";
            else
                lbl_Vision2Status.Text = "未连接";
            lbl_Vision2Status.ForeColor = Color.Red;

            for (int i = 0; i < cbo_Vision2.Items.Count; i++)
            {
                if (!cbo_Vision1.Items.Contains(cbo_Vision2.Items[i]))
                    cbo_Vision1.Items.Add(cbo_Vision2.Items[i]);

                if (!cbo_Vision3.Items.Contains(cbo_Vision2.Items[i]))
                    cbo_Vision3.Items.Add(cbo_Vision2.Items[i]);

                if (!cbo_Vision4.Items.Contains(cbo_Vision2.Items[i]))
                    cbo_Vision4.Items.Add(cbo_Vision2.Items[i]);

                if (!cbo_Vision5.Items.Contains(cbo_Vision2.Items[i]))
                    cbo_Vision5.Items.Add(cbo_Vision2.Items[i]);

                if (!cbo_Vision6.Items.Contains(cbo_Vision2.Items[i]))
                    cbo_Vision6.Items.Add(cbo_Vision2.Items[i]);

                if (!cbo_Vision7.Items.Contains(cbo_Vision2.Items[i]))
                    cbo_Vision7.Items.Add(cbo_Vision2.Items[i]);

            }

            if (cbo_Vision2.SelectedItem.ToString() == " " || cbo_Vision2.SelectedItem.ToString() == "")
                return;

            cbo_Vision1.Items.Remove(cbo_Vision2.SelectedItem);
            cbo_Vision3.Items.Remove(cbo_Vision2.SelectedItem);
            cbo_Vision4.Items.Remove(cbo_Vision2.SelectedItem);
            cbo_Vision5.Items.Remove(cbo_Vision2.SelectedItem);
            cbo_Vision6.Items.Remove(cbo_Vision2.SelectedItem);
            cbo_Vision7.Items.Remove(cbo_Vision2.SelectedItem);
        }
        private void cbo_Vision3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (cbo_Vision3.SelectedItem.ToString() == Vision3Prev)
                return;

            string[] SerialNo = new string[2];
            if (cbo_Vision3.SelectedItem.ToString() != " " && cbo_Vision3.SelectedItem.ToString() != null)
            {
                SerialNo = cbo_Vision3.SelectedItem.ToString().Split('(');
                SerialNo[0] = SerialNo[0].TrimEnd();
            }
            else SerialNo[0] = "";

            string[] SerialNoPrev = new string[2];
            SerialNoPrev = Vision3Prev.Split('(');
            SerialNoPrev[0] = SerialNoPrev[0].TrimEnd();

            if (SerialNo[1].Contains("Teli"))
                OpenAndCloseCameraTeli(SerialNo[0], SerialNoPrev[0]);
            else if (SerialNo[1].Contains("AVT"))
                OpenAndCloseCameraAVT(SerialNo[0], Convert.ToInt32(cbo_3.Items[cbo_Vision3.SelectedIndex].ToString()));//, false);

            Vision3Prev = cbo_Vision3.SelectedItem.ToString();
            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                lbl_Vision3Status.Text = "Not Connected";
            else
                lbl_Vision3Status.Text = "未连接";
            lbl_Vision3Status.ForeColor = Color.Red;

            for (int i = 0; i < cbo_Vision3.Items.Count; i++)
            {
                if (!cbo_Vision2.Items.Contains(cbo_Vision3.Items[i]))
                    cbo_Vision2.Items.Add(cbo_Vision3.Items[i]);

                if (!cbo_Vision1.Items.Contains(cbo_Vision3.Items[i]))
                    cbo_Vision1.Items.Add(cbo_Vision3.Items[i]);

                if (!cbo_Vision4.Items.Contains(cbo_Vision3.Items[i]))
                    cbo_Vision4.Items.Add(cbo_Vision3.Items[i]);

                if (!cbo_Vision5.Items.Contains(cbo_Vision3.Items[i]))
                    cbo_Vision5.Items.Add(cbo_Vision3.Items[i]);

                if (!cbo_Vision6.Items.Contains(cbo_Vision3.Items[i]))
                    cbo_Vision6.Items.Add(cbo_Vision3.Items[i]);

                if (!cbo_Vision7.Items.Contains(cbo_Vision3.Items[i]))
                    cbo_Vision7.Items.Add(cbo_Vision3.Items[i]);
            }

            if (cbo_Vision3.SelectedItem.ToString() == " " || cbo_Vision3.SelectedItem.ToString() == "")
                return;

            cbo_Vision2.Items.Remove(cbo_Vision3.SelectedItem);
            cbo_Vision1.Items.Remove(cbo_Vision3.SelectedItem);
            cbo_Vision4.Items.Remove(cbo_Vision3.SelectedItem);
            cbo_Vision5.Items.Remove(cbo_Vision3.SelectedItem);
            cbo_Vision6.Items.Remove(cbo_Vision3.SelectedItem);
            cbo_Vision7.Items.Remove(cbo_Vision3.SelectedItem);
        }
        private void cbo_Vision4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (cbo_Vision4.SelectedItem.ToString() == Vision4Prev)
                return;

            string[] SerialNo = new string[2];
            if (cbo_Vision4.SelectedItem.ToString() != " " && cbo_Vision4.SelectedItem.ToString() != null)
            {
                SerialNo = cbo_Vision4.SelectedItem.ToString().Split('(');
                SerialNo[0] = SerialNo[0].TrimEnd();
            }
            else SerialNo[0] = "";

            string[] SerialNoPrev = new string[2];
            SerialNoPrev = Vision4Prev.Split('(');
            SerialNoPrev[0] = SerialNoPrev[0].TrimEnd();//SerialNo[0].TrimEnd();

            if (SerialNo[1].Contains("Teli"))
                OpenAndCloseCameraTeli(SerialNo[0], SerialNoPrev[0]);
            else if (SerialNo[1].Contains("AVT"))
                OpenAndCloseCameraAVT(SerialNo[0], Convert.ToInt32(cbo_4.Items[cbo_Vision4.SelectedIndex].ToString()));//, false);

            Vision4Prev = cbo_Vision4.SelectedItem.ToString();
            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                lbl_Vision4Status.Text = "Not Connected";
            else
                lbl_Vision4Status.Text = "未连接";
            lbl_Vision4Status.ForeColor = Color.Red;

            for (int i = 0; i < cbo_Vision4.Items.Count; i++)
            {
                if (!cbo_Vision2.Items.Contains(cbo_Vision4.Items[i]))
                    cbo_Vision2.Items.Add(cbo_Vision4.Items[i]);

                if (!cbo_Vision3.Items.Contains(cbo_Vision4.Items[i]))
                    cbo_Vision3.Items.Add(cbo_Vision4.Items[i]);

                if (!cbo_Vision1.Items.Contains(cbo_Vision4.Items[i]))
                    cbo_Vision1.Items.Add(cbo_Vision4.Items[i]);

                if (!cbo_Vision5.Items.Contains(cbo_Vision4.Items[i]))
                    cbo_Vision5.Items.Add(cbo_Vision4.Items[i]);

                if (!cbo_Vision6.Items.Contains(cbo_Vision4.Items[i]))
                    cbo_Vision6.Items.Add(cbo_Vision4.Items[i]);

                if (!cbo_Vision7.Items.Contains(cbo_Vision4.Items[i]))
                    cbo_Vision7.Items.Add(cbo_Vision4.Items[i]);
            }

            if (cbo_Vision4.SelectedItem.ToString() == " " || cbo_Vision4.SelectedItem.ToString() == "")
                return;

            cbo_Vision2.Items.Remove(cbo_Vision4.SelectedItem);
            cbo_Vision3.Items.Remove(cbo_Vision4.SelectedItem);
            cbo_Vision1.Items.Remove(cbo_Vision4.SelectedItem);
            cbo_Vision5.Items.Remove(cbo_Vision4.SelectedItem);
            cbo_Vision6.Items.Remove(cbo_Vision4.SelectedItem);
            cbo_Vision7.Items.Remove(cbo_Vision4.SelectedItem);

        }
        private void cbo_Vision5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (cbo_Vision5.SelectedItem.ToString() == Vision5Prev)
                return;

            string[] SerialNo = new string[2];
            if (cbo_Vision5.SelectedItem.ToString() != " " && cbo_Vision5.SelectedItem.ToString() != null)
            {
                SerialNo = cbo_Vision5.SelectedItem.ToString().Split('(');
                SerialNo[0] = SerialNo[0].TrimEnd();
            }
            else SerialNo[0] = "";

            string[] SerialNoPrev = new string[2];
            SerialNoPrev = Vision5Prev.Split('(');
            SerialNoPrev[0] = SerialNoPrev[0].TrimEnd();

            if (SerialNo[1].Contains("Teli"))
                OpenAndCloseCameraTeli(SerialNo[0], SerialNoPrev[0]);
            else if (SerialNo[1].Contains("AVT"))
                OpenAndCloseCameraAVT(SerialNo[0], Convert.ToInt32(cbo_5.Items[cbo_Vision5.SelectedIndex].ToString()));//, false);

            Vision5Prev = cbo_Vision5.SelectedItem.ToString();
            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                lbl_Vision5Status.Text = "Not Connected";
            else
                lbl_Vision5Status.Text = "未连接";
            lbl_Vision5Status.ForeColor = Color.Red;

            for (int i = 0; i < cbo_Vision5.Items.Count; i++)
            {
                if (!cbo_Vision2.Items.Contains(cbo_Vision5.Items[i]))
                    cbo_Vision2.Items.Add(cbo_Vision5.Items[i]);

                if (!cbo_Vision3.Items.Contains(cbo_Vision5.Items[i]))
                    cbo_Vision3.Items.Add(cbo_Vision5.Items[i]);

                if (!cbo_Vision1.Items.Contains(cbo_Vision5.Items[i]))
                    cbo_Vision1.Items.Add(cbo_Vision5.Items[i]);

                if (!cbo_Vision4.Items.Contains(cbo_Vision5.Items[i]))
                    cbo_Vision4.Items.Add(cbo_Vision5.Items[i]);

                if (!cbo_Vision6.Items.Contains(cbo_Vision5.Items[i]))
                    cbo_Vision6.Items.Add(cbo_Vision5.Items[i]);

                if (!cbo_Vision7.Items.Contains(cbo_Vision5.Items[i]))
                    cbo_Vision7.Items.Add(cbo_Vision5.Items[i]);

            }

            if (cbo_Vision5.SelectedItem.ToString() == " " || cbo_Vision5.SelectedItem.ToString() == "")
                return;

            cbo_Vision2.Items.Remove(cbo_Vision5.SelectedItem);
            cbo_Vision3.Items.Remove(cbo_Vision5.SelectedItem);
            cbo_Vision1.Items.Remove(cbo_Vision5.SelectedItem);
            cbo_Vision4.Items.Remove(cbo_Vision5.SelectedItem);
            cbo_Vision6.Items.Remove(cbo_Vision5.SelectedItem);
            cbo_Vision7.Items.Remove(cbo_Vision5.SelectedItem);
        }

        private void cbo_Vision6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (cbo_Vision6.SelectedItem.ToString() == Vision6Prev)
                return;

            string[] SerialNo = new string[2];
            if (cbo_Vision6.SelectedItem.ToString() != " " && cbo_Vision6.SelectedItem.ToString() != null)
            {
                SerialNo = cbo_Vision6.SelectedItem.ToString().Split('(');
                SerialNo[0] = SerialNo[0].TrimEnd();
            }
            else SerialNo[0] = "";

            string[] SerialNoPrev = new string[2];
            SerialNoPrev = Vision6Prev.Split('(');
            SerialNoPrev[0] = SerialNoPrev[0].TrimEnd();

            if (SerialNo[1].Contains("Teli"))
                OpenAndCloseCameraTeli(SerialNo[0], SerialNoPrev[0]);
            else if (SerialNo[1].Contains("AVT"))
                OpenAndCloseCameraAVT(SerialNo[0], Convert.ToInt32(cbo_6.Items[cbo_Vision6.SelectedIndex].ToString()));//, false);

            Vision6Prev = cbo_Vision6.SelectedItem.ToString();
            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                lbl_Vision6Status.Text = "Not Connected";
            else
                lbl_Vision6Status.Text = "未连接";
            lbl_Vision6Status.ForeColor = Color.Red;

            for (int i = 0; i < cbo_Vision6.Items.Count; i++)
            {
                if (!cbo_Vision2.Items.Contains(cbo_Vision6.Items[i]))
                    cbo_Vision2.Items.Add(cbo_Vision6.Items[i]);

                if (!cbo_Vision3.Items.Contains(cbo_Vision6.Items[i]))
                    cbo_Vision3.Items.Add(cbo_Vision6.Items[i]);

                if (!cbo_Vision1.Items.Contains(cbo_Vision6.Items[i]))
                    cbo_Vision1.Items.Add(cbo_Vision6.Items[i]);

                if (!cbo_Vision4.Items.Contains(cbo_Vision6.Items[i]))
                    cbo_Vision4.Items.Add(cbo_Vision6.Items[i]);

                if (!cbo_Vision5.Items.Contains(cbo_Vision6.Items[i]))
                    cbo_Vision5.Items.Add(cbo_Vision6.Items[i]);

                if (!cbo_Vision7.Items.Contains(cbo_Vision6.Items[i]))
                    cbo_Vision7.Items.Add(cbo_Vision6.Items[i]);

            }

            if (cbo_Vision6.SelectedItem.ToString() == " " || cbo_Vision6.SelectedItem.ToString() == "")
                return;

            cbo_Vision2.Items.Remove(cbo_Vision6.SelectedItem);
            cbo_Vision3.Items.Remove(cbo_Vision6.SelectedItem);
            cbo_Vision1.Items.Remove(cbo_Vision6.SelectedItem);
            cbo_Vision5.Items.Remove(cbo_Vision6.SelectedItem);
            cbo_Vision4.Items.Remove(cbo_Vision6.SelectedItem);
            cbo_Vision7.Items.Remove(cbo_Vision6.SelectedItem);
        }

        private void cbo_Vision7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (cbo_Vision7.SelectedItem.ToString() == Vision7Prev)
                return;

            string[] SerialNo = new string[2];
            if (cbo_Vision7.SelectedItem.ToString() != " " && cbo_Vision7.SelectedItem.ToString() != null)
            {
                SerialNo = cbo_Vision7.SelectedItem.ToString().Split('(');
                SerialNo[0] = SerialNo[0].TrimEnd();
            }
            else SerialNo[0] = "";

            string[] SerialNoPrev = new string[2];
            SerialNoPrev = Vision7Prev.Split('(');
            SerialNoPrev[0] = SerialNoPrev[0].TrimEnd();

            if (SerialNo[1].Contains("Teli"))
                OpenAndCloseCameraTeli(SerialNo[0], SerialNoPrev[0]);
            else if (SerialNo[1].Contains("AVT"))
                OpenAndCloseCameraAVT(SerialNo[0], Convert.ToInt32(cbo_7.Items[cbo_Vision7.SelectedIndex].ToString()));//, false);

            Vision7Prev = cbo_Vision7.SelectedItem.ToString();
            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                lbl_Vision7Status.Text = "Not Connected";
            else
                lbl_Vision7Status.Text = "未连接";
            lbl_Vision7Status.ForeColor = Color.Red;

            for (int i = 0; i < cbo_Vision7.Items.Count; i++)
            {
                if (!cbo_Vision2.Items.Contains(cbo_Vision7.Items[i]))
                    cbo_Vision2.Items.Add(cbo_Vision7.Items[i]);

                if (!cbo_Vision3.Items.Contains(cbo_Vision7.Items[i]))
                    cbo_Vision3.Items.Add(cbo_Vision7.Items[i]);

                if (!cbo_Vision1.Items.Contains(cbo_Vision7.Items[i]))
                    cbo_Vision1.Items.Add(cbo_Vision7.Items[i]);

                if (!cbo_Vision4.Items.Contains(cbo_Vision7.Items[i]))
                    cbo_Vision4.Items.Add(cbo_Vision7.Items[i]);

                if (!cbo_Vision5.Items.Contains(cbo_Vision7.Items[i]))
                    cbo_Vision5.Items.Add(cbo_Vision7.Items[i]);

                if (!cbo_Vision6.Items.Contains(cbo_Vision7.Items[i]))
                    cbo_Vision6.Items.Add(cbo_Vision7.Items[i]);

            }

            if (cbo_Vision7.SelectedItem.ToString() == " " || cbo_Vision7.SelectedItem.ToString() == "")
                return;

            cbo_Vision2.Items.Remove(cbo_Vision7.SelectedItem);
            cbo_Vision3.Items.Remove(cbo_Vision7.SelectedItem);
            cbo_Vision1.Items.Remove(cbo_Vision7.SelectedItem);
            cbo_Vision5.Items.Remove(cbo_Vision7.SelectedItem);
            cbo_Vision4.Items.Remove(cbo_Vision7.SelectedItem);
            cbo_Vision6.Items.Remove(cbo_Vision7.SelectedItem);
        }
        private void btn_Connect_Click(object sender, EventArgs e)
        {
            //  TrackLog t = new TrackLog();
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            bool blnConnectionFail = false;
            
            string m_CameraModel, m_SerialNo, m_CameraModelNo;
            // Get Vision Module name from Registry
            m_arrVisionName.Clear();
            string[] strVisionList = subKey.GetSubKeyNames();
            for (int i = 0; i < strVisionList.Length; i++)
            {
                m_arrVisionName.Add(strVisionList[i]);
            }

            RegistryKey subKey1;
            int intVisionPos = 0;
            for (int i = 0; i < m_arrVisionName.Count; i++)
            {
                subKey1 = subKey.CreateSubKey(m_arrVisionName[i].ToString());

                // Get vision module number
                intVisionPos = (Convert.ToInt32(m_arrVisionName[i].ToString().Substring(6))) % 10 - 1;  // == VSIndex


                if (m_smVSInfo[intVisionPos].g_strVisionFolderName == "Vision1")
                {
                    if (cbo_Vision1.SelectedItem != null)
                    {
                        if (cbo_Vision1.SelectedItem.ToString() != " " && cbo_Vision1.SelectedItem.ToString() != "")
                        {
                            m_CameraModel = subKey1.GetValue("CameraModel", "").ToString();
                            m_SerialNo = subKey1.GetValue("SerialNo", "").ToString();
                            m_CameraModelNo = subKey1.GetValue("CameraModelNo", "").ToString();

                            //if (m_smVSInfo[intVisionPos].g_strCameraModel == "AVT")
                            //{


                            //}
                            //else if (m_smVSInfo[intVisionPos].g_strCameraModel == "Teli")
                            //{
                            //t.WriteLine("Inside Vision1");
                            string[] SerialNo1 = new string[2];
                            // t.WriteLine(cbo_Vision1.SelectedItem.ToString());
                            SerialNo1 = cbo_Vision1.SelectedItem.ToString().Split('(');
                            SerialNo1[0] = SerialNo1[0].TrimEnd();
                            SerialNo1[1] = SerialNo1[1].Remove(SerialNo1[1].IndexOf(')'));

                            if (SerialNo1[1].Contains(m_CameraModelNo))
                            {
                                subKey1.SetValue("SerialNo", SerialNo1[0]);
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                    lbl_Vision1Status.Text = "Connected";
                                else
                                    lbl_Vision1Status.Text = "已连接";
                                lbl_Vision1Status.ForeColor = Color.Green;
                                m_smVSInfo[intVisionPos].g_strCameraSerialNo = SerialNo1[0];
                                if (cbo_Vision1.SelectedItem.ToString().Contains("AVT"))
                                {
                                    subKey1.SetValue("CameraModel", "AVT");
                                }
                                else if (cbo_Vision1.SelectedItem.ToString().Contains("Teli"))
                                {
                                    subKey1.SetValue("CameraModel", "Teli");
                                }
                                string NewItem = cbo_Vision1.SelectedItem.ToString();
                                if (cbo_Vision1.SelectedItem.ToString().Contains("Not Connected"))
                                {
                                    cbo_Vision1.Items.Remove(NewItem);
                                    NewItem = NewItem.Replace("Not Connected", "Connected");
                                    cbo_Vision1.Items.Add(NewItem);
                                    cbo_Vision1.SelectedItem = NewItem;
                                }
                                // t.WriteLine(SerialNo1[0]);
                                // }
                                if (m_CameraModel != subKey1.GetValue("CameraModel", "").ToString())
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[intVisionPos].g_strVisionFolderName + ">", "Camera Model changed", m_CameraModel, subKey1.GetValue("CameraModel", "").ToString(), m_smProductionInfo.g_strLotID);
                                if (m_SerialNo != subKey1.GetValue("SerialNo", "").ToString())
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[intVisionPos].g_strVisionFolderName + ">", "Camera Serial No changed", m_SerialNo, subKey1.GetValue("SerialNo", "").ToString(), m_smProductionInfo.g_strLotID);
                            }
                            else
                            {
                                SRMMessageBox.Show("Camera model is different, please connect correct model of camera. " + "Camera model set: " + m_CameraModelNo, "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                blnConnectionFail = true;
                            }
                        }
                        else
                        {
                            SRMMessageBox.Show("Vision1 serial No. cannot be blank.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            blnConnectionFail = true;
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Vision1 serial No. cannot be blank.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        blnConnectionFail = true;
                    }


                }

                if (m_smVSInfo[intVisionPos].g_strVisionFolderName == "Vision2")
                {
                    if (cbo_Vision2.SelectedItem != null)
                    {
                        if (cbo_Vision2.SelectedItem.ToString() != " " && cbo_Vision2.SelectedItem.ToString() != "")
                        {
                            m_CameraModel = subKey1.GetValue("CameraModel", "").ToString();
                            m_SerialNo = subKey1.GetValue("SerialNo", "").ToString();
                            m_CameraModelNo = subKey1.GetValue("CameraModelNo", "").ToString();

                            //if (m_smVSInfo[intVisionPos].g_strCameraModel == "AVT")
                            //{

                            //}
                            //else if (m_smVSInfo[intVisionPos].g_strCameraModel == "Teli")
                            //{
                            // t.WriteLine("Inside Vision2");
                            string[] SerialNo2 = new string[2];
                            // t.WriteLine(cbo_Vision2.SelectedItem.ToString());
                            SerialNo2 = cbo_Vision2.SelectedItem.ToString().Split('(');
                            SerialNo2[0] = SerialNo2[0].TrimEnd();
                            SerialNo2[1] = SerialNo2[1].Remove(SerialNo2[1].IndexOf(')'));

                            if (SerialNo2[1].Contains(m_CameraModelNo))
                            {
                                subKey1.SetValue("SerialNo", SerialNo2[0]);
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                    lbl_Vision2Status.Text = "Connected";
                                else
                                    lbl_Vision2Status.Text = "已连接";
                                lbl_Vision2Status.ForeColor = Color.Green;
                                m_smVSInfo[intVisionPos].g_strCameraSerialNo = SerialNo2[0];
                                if (cbo_Vision2.SelectedItem.ToString().Contains("AVT"))
                                {
                                    subKey1.SetValue("CameraModel", "AVT");
                                }
                                else if (cbo_Vision2.SelectedItem.ToString().Contains("Teli"))
                                {
                                    subKey1.SetValue("CameraModel", "Teli");
                                }
                                string NewItem = cbo_Vision2.SelectedItem.ToString();
                                if (cbo_Vision2.SelectedItem.ToString().Contains("Not Connected"))
                                {
                                    cbo_Vision2.Items.Remove(NewItem);
                                    NewItem = NewItem.Replace("Not Connected", "Connected");
                                    cbo_Vision2.Items.Add(NewItem);
                                    cbo_Vision2.SelectedItem = NewItem;
                                }
                                //  t.WriteLine(SerialNo2[0]);
                                // }
                                if (m_CameraModel != subKey1.GetValue("CameraModel", "").ToString())
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[intVisionPos].g_strVisionFolderName + ">", "Camera Model value changed", m_CameraModel, subKey1.GetValue("CameraModel", "").ToString(), m_smProductionInfo.g_strLotID);
                                if (m_SerialNo != subKey1.GetValue("SerialNo", "").ToString())
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[intVisionPos].g_strVisionFolderName + ">", "Camera Serial No value changed", m_SerialNo, subKey1.GetValue("SerialNo", "").ToString(), m_smProductionInfo.g_strLotID);
                            }
                            else
                            {
                                SRMMessageBox.Show("Camera model is different, please connect correct model of camera. " + "Camera model set: " + m_CameraModelNo, "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                blnConnectionFail = true;
                            }
                        }
                        else
                        {
                            SRMMessageBox.Show("Vision2 serial No. cannot be blank.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            blnConnectionFail = true;
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Vision2 serial No. cannot be blank.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        blnConnectionFail = true;
                    }


                }
                if (m_smVSInfo[intVisionPos].g_strVisionFolderName == "Vision3")
                {
                    if (cbo_Vision3.SelectedItem != null)
                    {

                        if (cbo_Vision3.SelectedItem.ToString() != " " && cbo_Vision3.SelectedItem.ToString() != "")
                        {
                            m_CameraModel = subKey1.GetValue("CameraModel", "").ToString();
                            m_SerialNo = subKey1.GetValue("SerialNo", "").ToString();
                            m_CameraModelNo = subKey1.GetValue("CameraModelNo", "").ToString();

                            //if (m_smVSInfo[intVisionPos].g_strCameraModel == "AVT")
                            //{

                            //}
                            //if (m_smVSInfo[intVisionPos].g_strCameraModel == "Teli")
                            //{
                            //  t.WriteLine("Inside Vision3");
                            string[] SerialNo3 = new string[2];
                            //  t.WriteLine(cbo_Vision3.SelectedItem.ToString());
                            SerialNo3 = cbo_Vision3.SelectedItem.ToString().Split('(');
                            SerialNo3[0] = SerialNo3[0].TrimEnd();
                            SerialNo3[1] = SerialNo3[1].Remove(SerialNo3[1].IndexOf(')'));

                            if (SerialNo3[1].Contains(m_CameraModelNo))
                            {
                                subKey1.SetValue("SerialNo", SerialNo3[0]);
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                    lbl_Vision3Status.Text = "Connected";
                                else
                                    lbl_Vision3Status.Text = "已连接";
                                lbl_Vision3Status.ForeColor = Color.Green;
                                m_smVSInfo[intVisionPos].g_strCameraSerialNo = SerialNo3[0];
                                if (cbo_Vision3.SelectedItem.ToString().Contains("AVT"))
                                {
                                    subKey1.SetValue("CameraModel", "AVT");
                                }
                                else if (cbo_Vision3.SelectedItem.ToString().Contains("Teli"))
                                {
                                    subKey1.SetValue("CameraModel", "Teli");
                                }
                                string NewItem = cbo_Vision3.SelectedItem.ToString();
                                if (cbo_Vision3.SelectedItem.ToString().Contains("Not Connected"))
                                {
                                    cbo_Vision3.Items.Remove(NewItem);
                                    NewItem = NewItem.Replace("Not Connected", "Connected");
                                    cbo_Vision3.Items.Add(NewItem);
                                    cbo_Vision3.SelectedItem = NewItem;
                                }
                                // t.WriteLine(SerialNo3[0]);
                                // }
                                if (m_CameraModel != subKey1.GetValue("CameraModel", "").ToString())
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[intVisionPos].g_strVisionFolderName + ">", "Camera Model value changed", m_CameraModel, subKey1.GetValue("CameraModel", "").ToString(), m_smProductionInfo.g_strLotID);
                                if (m_SerialNo != subKey1.GetValue("SerialNo", "").ToString())
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[intVisionPos].g_strVisionFolderName + ">", "Camera Serial No value changed", m_SerialNo, subKey1.GetValue("SerialNo", "").ToString(), m_smProductionInfo.g_strLotID);
                            }
                            else
                            {
                                SRMMessageBox.Show("Camera model is different, please connect correct model of camera. " + "Camera model set: " + m_CameraModelNo, "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                blnConnectionFail = true;
                            }
                        }
                        else
                        {
                            SRMMessageBox.Show("Vision3 serial No. cannot be blank.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            blnConnectionFail = true;
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Vision3 serial No. cannot be blank.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        blnConnectionFail = true;
                    }


                }
                if (m_smVSInfo[intVisionPos].g_strVisionFolderName == "Vision4")
                {
                    if (cbo_Vision4.SelectedItem != null)
                    {
                        if (cbo_Vision4.SelectedItem.ToString() != " " && cbo_Vision4.SelectedItem.ToString() != "")
                        {
                            m_CameraModel = subKey1.GetValue("CameraModel", "").ToString();
                            m_SerialNo = subKey1.GetValue("SerialNo", "").ToString();
                            m_CameraModelNo = subKey1.GetValue("CameraModelNo", "").ToString();
                            //if (m_smVSInfo[intVisionPos].g_strCameraModel == "AVT")
                            //{

                            //}
                            //if (m_smVSInfo[intVisionPos].g_strCameraModel == "Teli")
                            //{
                            //  t.WriteLine("Inside Vision4");
                            string[] SerialNo4 = new string[2];
                            // t.WriteLine(cbo_Vision4.SelectedItem.ToString());                         
                            SerialNo4 = cbo_Vision4.SelectedItem.ToString().Split('(');
                            SerialNo4[0] = SerialNo4[0].TrimEnd();
                            SerialNo4[1] = SerialNo4[1].Remove(SerialNo4[1].IndexOf(')'));

                            if (SerialNo4[1].Contains(m_CameraModelNo))
                            {
                                subKey1.SetValue("SerialNo", SerialNo4[0]);
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                    lbl_Vision4Status.Text = "Connected";
                                else
                                    lbl_Vision4Status.Text = "已连接";
                                lbl_Vision4Status.ForeColor = Color.Green;
                                m_smVSInfo[intVisionPos].g_strCameraSerialNo = SerialNo4[0];
                                if (cbo_Vision4.SelectedItem.ToString().Contains("AVT"))
                                {
                                    subKey1.SetValue("CameraModel", "AVT");
                                }
                                else if (cbo_Vision4.SelectedItem.ToString().Contains("Teli"))
                                {
                                    subKey1.SetValue("CameraModel", "Teli");
                                }
                                string NewItem = cbo_Vision4.SelectedItem.ToString();
                                if (cbo_Vision4.SelectedItem.ToString().Contains("Not Connected"))
                                {
                                    cbo_Vision4.Items.Remove(NewItem);
                                    NewItem = NewItem.Replace("Not Connected", "Connected");
                                    cbo_Vision4.Items.Add(NewItem);
                                    cbo_Vision4.SelectedItem = NewItem;
                                }
                                //  t.WriteLine(SerialNo4[0]);
                                // }
                                if (m_CameraModel != subKey1.GetValue("CameraModel", "").ToString())
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[intVisionPos].g_strVisionFolderName + ">", "Camera Model value changed", m_CameraModel, subKey1.GetValue("CameraModel", "").ToString(), m_smProductionInfo.g_strLotID);
                                if (m_SerialNo != subKey1.GetValue("SerialNo", "").ToString())
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[intVisionPos].g_strVisionFolderName + ">", "Camera Serial No value changed", m_SerialNo, subKey1.GetValue("SerialNo", "").ToString(), m_smProductionInfo.g_strLotID);
                            }
                            else
                            {
                                SRMMessageBox.Show("Camera model is different, please connect correct model of camera. " + "Camera model set: " + m_CameraModelNo, "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                blnConnectionFail = true;
                            }
                        }
                        else
                        {
                            SRMMessageBox.Show("Vision4 serial No. cannot be blank.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            blnConnectionFail = true;
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Vision4 serial No. cannot be blank.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        blnConnectionFail = true;
                    }

                }

                if (m_smVSInfo[intVisionPos].g_strVisionFolderName == "Vision5")
                {
                    if (cbo_Vision5.SelectedItem != null)
                    {
                        if (cbo_Vision5.SelectedItem.ToString() != " " && cbo_Vision5.SelectedItem.ToString() != "")
                        {
                            m_CameraModel = subKey1.GetValue("CameraModel", "").ToString();
                            m_SerialNo = subKey1.GetValue("SerialNo", "").ToString();
                            m_CameraModelNo = subKey1.GetValue("CameraModelNo", "").ToString();
                            //if (m_smVSInfo[intVisionPos].g_strCameraModel == "AVT")
                            //{

                            //}
                            //if (m_smVSInfo[intVisionPos].g_strCameraModel == "Teli")
                            //{
                            //  t.WriteLine("Inside Vision4");
                            string[] SerialNo5 = new string[2];
                            // t.WriteLine(cbo_Vision4.SelectedItem.ToString());                         
                            SerialNo5 = cbo_Vision5.SelectedItem.ToString().Split('(');
                            SerialNo5[0] = SerialNo5[0].TrimEnd();
                            SerialNo5[1] = SerialNo5[1].Remove(SerialNo5[1].IndexOf(')'));

                            if (SerialNo5[1].Contains(m_CameraModelNo))
                            {
                                subKey1.SetValue("SerialNo", SerialNo5[0]);
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                    lbl_Vision5Status.Text = "Connected";
                                else
                                    lbl_Vision5Status.Text = "已连接";
                                lbl_Vision5Status.ForeColor = Color.Green;
                                m_smVSInfo[intVisionPos].g_strCameraSerialNo = SerialNo5[0];
                                if (cbo_Vision5.SelectedItem.ToString().Contains("AVT"))
                                {
                                    subKey1.SetValue("CameraModel", "AVT");
                                }
                                else if (cbo_Vision5.SelectedItem.ToString().Contains("Teli"))
                                {
                                    subKey1.SetValue("CameraModel", "Teli");
                                }
                                string NewItem = cbo_Vision5.SelectedItem.ToString();
                                if (cbo_Vision5.SelectedItem.ToString().Contains("Not Connected"))
                                {
                                    cbo_Vision5.Items.Remove(NewItem);
                                    NewItem = NewItem.Replace("Not Connected", "Connected");
                                    cbo_Vision5.Items.Add(NewItem);
                                    cbo_Vision5.SelectedItem = NewItem;
                                }
                                //  t.WriteLine(SerialNo4[0]);
                                // }
                                if (m_CameraModel != subKey1.GetValue("CameraModel", "").ToString())
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[intVisionPos].g_strVisionFolderName + ">", "Camera Model value changed", m_CameraModel, subKey1.GetValue("CameraModel", "").ToString(), m_smProductionInfo.g_strLotID);
                                if (m_SerialNo != subKey1.GetValue("SerialNo", "").ToString())
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[intVisionPos].g_strVisionFolderName + ">", "Camera Serial No value changed", m_SerialNo, subKey1.GetValue("SerialNo", "").ToString(), m_smProductionInfo.g_strLotID);
                            }
                            else
                            {
                                SRMMessageBox.Show("Camera model is different, please connect correct model of camera. " + "Camera model set: " + m_CameraModelNo, "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                blnConnectionFail = true;
                            }
                        }
                        else
                        {
                            SRMMessageBox.Show("Vision5 serial No. cannot be blank.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            blnConnectionFail = true;
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Vision5 serial No. cannot be blank.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        blnConnectionFail = true;
                    }
                }

                if (m_smVSInfo[intVisionPos].g_strVisionFolderName == "Vision6")
                {
                    if (cbo_Vision6.SelectedItem != null)
                    {
                        if (cbo_Vision6.SelectedItem.ToString() != " " && cbo_Vision6.SelectedItem.ToString() != "")
                        {
                            m_CameraModel = subKey1.GetValue("CameraModel", "").ToString();
                            m_SerialNo = subKey1.GetValue("SerialNo", "").ToString();
                            m_CameraModelNo = subKey1.GetValue("CameraModelNo", "").ToString();
                            //if (m_smVSInfo[intVisionPos].g_strCameraModel == "AVT")
                            //{

                            //}
                            //if (m_smVSInfo[intVisionPos].g_strCameraModel == "Teli")
                            //{
                            //  t.WriteLine("Inside Vision4");
                            string[] SerialNo6 = new string[2];
                            // t.WriteLine(cbo_Vision4.SelectedItem.ToString());                         
                            SerialNo6 = cbo_Vision6.SelectedItem.ToString().Split('(');
                            SerialNo6[0] = SerialNo6[0].TrimEnd();
                            SerialNo6[1] = SerialNo6[1].Remove(SerialNo6[1].IndexOf(')'));

                            if (SerialNo6[1].Contains(m_CameraModelNo))
                            {
                                subKey1.SetValue("SerialNo", SerialNo6[0]);
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                    lbl_Vision6Status.Text = "Connected";
                                else
                                    lbl_Vision6Status.Text = "已连接";
                                lbl_Vision6Status.ForeColor = Color.Green;
                                m_smVSInfo[intVisionPos].g_strCameraSerialNo = SerialNo6[0];
                                if (cbo_Vision6.SelectedItem.ToString().Contains("AVT"))
                                {
                                    subKey1.SetValue("CameraModel", "AVT");
                                }
                                else if (cbo_Vision6.SelectedItem.ToString().Contains("Teli"))
                                {
                                    subKey1.SetValue("CameraModel", "Teli");
                                }
                                string NewItem = cbo_Vision6.SelectedItem.ToString();
                                if (cbo_Vision6.SelectedItem.ToString().Contains("Not Connected"))
                                {
                                    cbo_Vision6.Items.Remove(NewItem);
                                    NewItem = NewItem.Replace("Not Connected", "Connected");
                                    cbo_Vision6.Items.Add(NewItem);
                                    cbo_Vision6.SelectedItem = NewItem;
                                }
                                //  t.WriteLine(SerialNo4[0]);
                                // }
                                if (m_CameraModel != subKey1.GetValue("CameraModel", "").ToString())
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[intVisionPos].g_strVisionFolderName + ">", "Camera Model value changed", m_CameraModel, subKey1.GetValue("CameraModel", "").ToString(), m_smProductionInfo.g_strLotID);
                                if (m_SerialNo != subKey1.GetValue("SerialNo", "").ToString())
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[intVisionPos].g_strVisionFolderName + ">", "Camera Serial No value changed", m_SerialNo, subKey1.GetValue("SerialNo", "").ToString(), m_smProductionInfo.g_strLotID);
                            }
                            else
                            {
                                SRMMessageBox.Show("Camera model is different, please connect correct model of camera. " + "Camera model set: " + m_CameraModelNo, "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                blnConnectionFail = true;
                            }
                        }
                        else
                        {
                            SRMMessageBox.Show("Vision6 serial No. cannot be blank.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            blnConnectionFail = true;
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Vision6 serial No. cannot be blank.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        blnConnectionFail = true;
                    }
                }

                if (m_smVSInfo[intVisionPos].g_strVisionFolderName == "Vision7")
                {
                    if (cbo_Vision7.SelectedItem != null)
                    {
                        if (cbo_Vision7.SelectedItem.ToString() != " " && cbo_Vision7.SelectedItem.ToString() != "")
                        {
                            m_CameraModel = subKey1.GetValue("CameraModel", "").ToString();
                            m_SerialNo = subKey1.GetValue("SerialNo", "").ToString();
                            m_CameraModelNo = subKey1.GetValue("CameraModelNo", "").ToString();

                            //if (m_smVSInfo[intVisionPos].g_strCameraModel == "AVT")
                            //{

                            //}
                            //if (m_smVSInfo[intVisionPos].g_strCameraModel == "Teli")
                            //{
                            //  t.WriteLine("Inside Vision4");
                            string[] SerialNo7 = new string[2];
                            // t.WriteLine(cbo_Vision4.SelectedItem.ToString());                         
                            SerialNo7 = cbo_Vision7.SelectedItem.ToString().Split('(');
                            SerialNo7[0] = SerialNo7[0].TrimEnd();
                            SerialNo7[1] = SerialNo7[1].Remove(SerialNo7[1].IndexOf(')'));

                            if (SerialNo7[1].Contains(m_CameraModelNo))
                            {
                                subKey1.SetValue("SerialNo", SerialNo7[0]);
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                    lbl_Vision7Status.Text = "Connected";
                                else
                                    lbl_Vision7Status.Text = "已连接";
                                lbl_Vision7Status.ForeColor = Color.Green;
                                m_smVSInfo[intVisionPos].g_strCameraSerialNo = SerialNo7[0];
                                if (cbo_Vision7.SelectedItem.ToString().Contains("AVT"))
                                {
                                    subKey1.SetValue("CameraModel", "AVT");
                                }
                                else if (cbo_Vision7.SelectedItem.ToString().Contains("Teli"))
                                {
                                    subKey1.SetValue("CameraModel", "Teli");
                                }
                                string NewItem = cbo_Vision7.SelectedItem.ToString();
                                if (cbo_Vision7.SelectedItem.ToString().Contains("Not Connected"))
                                {
                                    cbo_Vision7.Items.Remove(NewItem);
                                    NewItem = NewItem.Replace("Not Connected", "Connected");
                                    cbo_Vision7.Items.Add(NewItem);
                                    cbo_Vision7.SelectedItem = NewItem;
                                }
                                //  t.WriteLine(SerialNo4[0]);
                                // }
                                if (m_CameraModel != subKey1.GetValue("CameraModel", "").ToString())
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[intVisionPos].g_strVisionFolderName + ">", "Camera Camera Model value changed", m_CameraModel, subKey1.GetValue("CameraModel", "").ToString(), m_smProductionInfo.g_strLotID);
                                if (m_SerialNo != subKey1.GetValue("SerialNo", "").ToString())
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[intVisionPos].g_strVisionFolderName + ">", "Camera Serial No value changed", m_SerialNo, subKey1.GetValue("SerialNo", "").ToString(), m_smProductionInfo.g_strLotID);
                            }
                            else
                            {
                                SRMMessageBox.Show("Camera model is different, please connect correct model of camera. " + "Camera model set: " + m_CameraModelNo, "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                blnConnectionFail = true;
                            }
                        }
                        else
                        {
                            SRMMessageBox.Show("Vision7 serial No. cannot be blank.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            blnConnectionFail = true;
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Vision7 serial No. cannot be blank.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        blnConnectionFail = true;
                    }
                }
            }
            
            if (!blnConnectionFail)
            {
                SRMMessageBox.Show("Camera serial connection successful. Please restart SRM.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            else
                return;
        }

        private bool CheckPortAvailable()
        {
            ArrayList arrPorts = new ArrayList();
            arrPorts.Add(txt_TCPIPPort.Text);
            for (int i = 0; i < dgd_VisionList.RowCount; i++)
            {
                string strPortNo = dgd_VisionList.Rows[i].Cells[1].Value.ToString();
                if (arrPorts.Contains(strPortNo))
                {
                    SRMMessageBox.Show("There are more than 2 Port sharing", "TCP/IP Comm", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }
                arrPorts.Add(strPortNo);
            }

            return true;
        }

        /// <summary>
        /// Add Vision Module into List
        /// </summary>
        private void InitAvailableVision()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            for (int i = 0; i < strVisionList.Length; i++)
            {
                subKey1 = subKey.OpenSubKey(strVisionList[i]);

                Vision objVision;
                objVision.ref_strVisionName = subKey1.GetValue("VisionName", strVisionList[i]).ToString();

                if (subKey1.GetValue("VisionNameNo", "").ToString() == "2")
                    objVision.ref_strVisionName += "2";

                int intStartIndex = strVisionList[i].IndexOf("Vision")+ 6;
                objVision.ref_intVisionNo = Convert.ToInt32(strVisionList[i].Substring(intStartIndex)) - 1;

                m_arrVisionList.Add(objVision);
            }
        }
        /// <summary>
        /// Load all RS232 settings and display on GUI
        /// </summary>
        private void InitSerialPortSetting()
        {
            if (m_smCustomizeInfo.g_blnConfigShowRS232)
            {
                m_FileHandle.GetFirstSection("SerialPort");

                txt_SerialPortTimeOut.Text = m_FileHandle.GetValueAsString("TimeOut", "100");
                txt_SerialPortRetriesCount.Text = m_FileHandle.GetValueAsString("RetriesCount", "0");
                cbo_BitsPerSecond.SelectedIndex = cbo_BitsPerSecond.FindStringExact(m_FileHandle.GetValueAsString("BitsPerSecond", "9600"));
                cbo_DataBits.SelectedIndex = cbo_DataBits.FindStringExact(m_FileHandle.GetValueAsString("DataBits", "8"));
                cbo_Parity.SelectedIndex = m_FileHandle.GetValueAsInt("Parity", 0);
                cbo_StopBits.SelectedIndex = m_FileHandle.GetValueAsInt("StopBits", 0);
                cbo_CommPort.SelectedIndex = cbo_CommPort.FindStringExact(m_FileHandle.GetValueAsString("CommPort", "COM2"));
                chk_SerialPortEnable.Checked = m_FileHandle.GetValueAsBoolean("Enable", false);
                
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                {
                    if (chk_SerialPortEnable.Checked)
                        chk_SerialPortEnable.Text = "Enabled";
                    else
                        chk_SerialPortEnable.Text = "Disabled";
                }
                else
                {
                    if (chk_SerialPortEnable.Checked)
                        chk_SerialPortEnable.Text = "启用";
                    else
                        chk_SerialPortEnable.Text = "禁用";
                }
            }
            else
            {
                tabCtrl_Configuration.TabPages.Remove(tab_RS232);
            }
        }
        /// <summary>
        /// Load all general settings and display on GUI
        /// </summary>
        private void InitSetting()
        {
            m_FileHandle.GetFirstSection("General");
            chk_DebugMode.Checked = m_FileHandle.GetValueAsBoolean("DebugMode", false);
            chk_PreviousVersion.Checked = m_FileHandle.GetValueAsBoolean("PreviousVersion", false);
            cbo_PreviousVersion.SelectedIndex = m_FileHandle.GetValueAsInt("PreviousVersionIndex", 0);
            //txt_PassImageCount.Text = m_FileHandle.GetValueAsString("PassImageCount", "1");
            chk_SavePassImage.Checked = m_FileHandle.GetValueAsBoolean("SavePassImage", false);
            chk_SaveFailImage.Checked = m_FileHandle.GetValueAsBoolean("SaveFailImage", true);
            txt_PassImagePics.Text = m_FileHandle.GetValueAsString("PassImagePics", "100");
            txt_FailImagePics.Text = m_FileHandle.GetValueAsString("FailImagePics", "100");
            if (m_FileHandle.GetValueAsInt("SaveImageMode", 0) == 0)
                radioBtn_First.Checked = true;
            else
                radioBtn_Last.Checked = true;
            chk_SaveErrorMessge.Checked = m_FileHandle.GetValueAsBoolean("SaveFailImageErrorMessage", false);
            txt_EmptyPocketTole.Text = m_FileHandle.GetValueAsString("EmptyPocketTole", "50");
            txt_WrongFaceTole.Text = m_FileHandle.GetValueAsString("WrongFaceTole", "50");
            chk_StopLowYield.Checked = m_FileHandle.GetValueAsBoolean("StopLowYield", false);
            m_blnGlobalSharingCalibrationDataPrev = chk_GlobalSharingCalibrationData.Checked = m_FileHandle.GetValueAsBoolean("GlobalSharingCalibrationData", false);
            m_blnGlobalSharingCameraDataPrev = chk_GlobalSharingCameraData.Checked = m_FileHandle.GetValueAsBoolean("GlobalSharingCameraData", false);
            txt_LowYield.Text = m_FileHandle.GetValueAsString("LowYield", "95");
            txt_MinUnitCheck.Text = m_FileHandle.GetValueAsString("MinUnitCheck", "1000");
            cbo_UnitDisplay.SelectedIndex = m_FileHandle.GetValueAsInt("UnitDisplay", 1) - 1;    // 1=mm(default) , 2=mil, 3=micron
            cbo_MarkUnitDisplay.SelectedIndex = m_FileHandle.GetValueAsInt("MarkUnitDisplay", 0);    // 0=pixel(default) , 1=mm, 2=mil, 3=micron
            txt_AutoLogOutMinutes.Value = m_FileHandle.GetValueAsInt("AutoLogOutMinutes", 5);
        }

        /// <summary>
        /// Load Network settings and display on GUI
        /// </summary>
        private void InitNetworkSetting()
        {
            if (m_smCustomizeInfo.g_blnConfigShowNetwork)
            {
                m_FileHandle.GetFirstSection("Network");
                chk_WantUseNetwork.Checked = m_FileHandle.GetValueAsBoolean("WantNetwork", false);
                chk_NetworkPasswordLimitCheckBox.Checked = m_FileHandle.GetValueAsBoolean("NetworkPasswordLimit", false);
                txt_HostIPEditBox.Text = m_FileHandle.GetValueAsString("NetworkHostIP", "");
                txt_NetworkUsernameEditBox.Text = m_FileHandle.GetValueAsString("NetworkUser", "");
                txt_NetworkPasswordEditBox.Text = m_FileHandle.GetValueAsString("NetworkPassword", "");
                txt_DeviceUploadDirEditBox.Text = m_FileHandle.GetValueAsString("DeviceUploadDir", "");
                txt_VisionLotReportUploadDirEditBox.Text = m_FileHandle.GetValueAsString("NetworkVisionLotReportUploadDir", "");
            }
            else
            {
                tabCtrl_Configuration.TabPages.Remove(tab_Network);
            }
        }

        private void InitSECSGEMSetting()
        {
            m_FileHandle.GetFirstSection("SECSGEM");
            chk_WantUseSECSGEM.Checked = m_FileHandle.GetValueAsBoolean("WantSECSGEM", false);
            txt_SECSGEMSharedFolderPath.Text = m_FileHandle.GetValueAsString("SECSGEMSharedFolderDir", "");
            txt_MaxNoOfCoplanPad.Text = m_FileHandle.GetValueAsString("SECSGEMMaxNoOfCoplanPad", "10");
        }

        /// <summary>
        /// Load TCPIP settings and display on GUI
        /// </summary>
        private void InitTCPSetting()
        {
            if (m_smCustomizeInfo.g_blnConfigShowTCPIC)
            {
                m_FileHandle.GetFirstSection("TCPIP");

                txt_TCPIPPort.Text = m_FileHandle.GetValueAsString("MainPort", "8080");
                chk_EnableTCPIP.Checked = m_FileHandle.GetValueAsBoolean("Enable", false);
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                {
                    if (chk_EnableTCPIP.Checked && chk_EnableTCPIP.Text != "Enabled")
                        chk_EnableTCPIP.Text = "Enabled";
                    else if (!chk_EnableTCPIP.Checked && chk_EnableTCPIP.Text != "Disabled")
                        chk_EnableTCPIP.Text = "Disabled";
                }
                else
                {
                    if (chk_EnableTCPIP.Checked && chk_EnableTCPIP.Text != "启用")
                        chk_EnableTCPIP.Text = "启用";
                    else if (!chk_EnableTCPIP.Checked && chk_EnableTCPIP.Text != "禁用")
                        chk_EnableTCPIP.Text = "禁用";
                }
                txt_TCPIPReportPath.Text = m_FileHandle.GetValueAsString("ReportAdd", "C:\\VsReport");
                txt_TCPIPTimeout.Text = m_FileHandle.GetValueAsString("TimeOut", "100");
                txt_TCPIPRetriesCount.Text = m_FileHandle.GetValueAsString("RetriesCount", "0");
                grp_ExportLotReport.Visible = chk_ExportLotReport.Checked = m_FileHandle.GetValueAsBoolean("ExportReport", false);
                cbo_ReportFormat.SelectedIndex = m_FileHandle.GetValueAsInt("ReportFormat", 0);
                txt_IPAddress.Text = m_FileHandle.GetValueAsString("IPAdd", "0.0.0.0");

                for (int i = 0; i < m_arrVisionList.Count; i++)
                {
                    string strVisionName = m_arrVisionList[i].ref_strVisionName;
                    
                    if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                        dgd_VisionList.Rows.Add(strVisionName,
                                                m_FileHandle.GetValueAsInt(strVisionName + "_TCPIPIO", 6010 + i),
                                                m_FileHandle.GetValueAsInt(strVisionName + "_VisionID", 0x01 << (m_smProductionInfo.g_arrVisionIDIndex[i])));
                    else
                        dgd_VisionList.Rows.Add(strVisionName,
                                                m_FileHandle.GetValueAsInt(strVisionName, 8081 + i),
                                                m_FileHandle.GetValueAsInt(strVisionName + "_VisionID", 0x01 << (m_smProductionInfo.g_arrVisionIDIndex[i])));
                }
            }
            else
            {
                tabCtrl_Configuration.TabPages.Remove(tab_TCPIP);
            }
        }
        /// <summary>
        /// Save all Settings in Option.xml
        /// </summary>
        private void WriteToFile()
        {
            
            STDeviceEdit.CopySettingFile(AppDomain.CurrentDomain.BaseDirectory, "Option.xml");

            m_FileHandle.WriteSectionElement("General");
            m_FileHandle.WriteElement1Value("DebugMode", chk_DebugMode.Checked);
            m_FileHandle.WriteElement1Value("PreviousVersion", chk_PreviousVersion.Checked);
            m_FileHandle.WriteElement1Value("PreviousVersionIndex", cbo_PreviousVersion.SelectedIndex);
            m_FileHandle.WriteElement1Value("SavePassImage", chk_SavePassImage.Checked);
            m_FileHandle.WriteElement1Value("SaveFailImage", chk_SaveFailImage.Checked);
            m_FileHandle.WriteElement1Value("PassImagePics", txt_PassImagePics.Text);
            m_FileHandle.WriteElement1Value("FailImagePics", txt_FailImagePics.Text);
            if (radioBtn_First.Checked)
                m_FileHandle.WriteElement1Value("SaveImageMode", 0);
            else
                m_FileHandle.WriteElement1Value("SaveImageMode", 1);
            m_FileHandle.WriteElement1Value("SaveFailImageErrorMessage", chk_SaveErrorMessge.Checked);
            m_FileHandle.WriteElement1Value("EmptyPocketTole", txt_EmptyPocketTole.Text);
            m_FileHandle.WriteElement1Value("WrongFaceTole", txt_WrongFaceTole.Text);
            m_FileHandle.WriteElement1Value("StopLowYield", chk_StopLowYield.Checked);
            m_FileHandle.WriteElement1Value("GlobalSharingCalibrationData", chk_GlobalSharingCalibrationData.Checked);
            m_FileHandle.WriteElement1Value("GlobalSharingCameraData", chk_GlobalSharingCameraData.Checked);
            m_FileHandle.WriteElement1Value("LowYield", txt_LowYield.Text);
            m_FileHandle.WriteElement1Value("MinUnitCheck", txt_MinUnitCheck.Text);
            m_FileHandle.WriteElement1Value("UnitDisplay", cbo_UnitDisplay.SelectedIndex + 1);     // 1=mm(default) , 2=mil, 3=micron
            m_FileHandle.WriteElement1Value("MarkUnitDisplay", cbo_MarkUnitDisplay.SelectedIndex);     // 0=pixel(default) , 1=mm, 2=mil, 3=micron
            m_FileHandle.WriteElement1Value("AutoLogOutMinutes", Convert.ToInt32(txt_AutoLogOutMinutes.Value));
            m_smProductionInfo.g_intAutoLogOutMinutes = Convert.ToInt32(txt_AutoLogOutMinutes.Value);

            m_smCustomizeInfo.g_intMarkUnitDisplay = cbo_MarkUnitDisplay.SelectedIndex;
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] != null)
                {
                    if ((m_smCustomizeInfo.g_intWantMark & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                    {
                        for (int u = 0; u < m_smVSInfo[i].g_intUnitsOnImage; u++)
                        {
                            m_smVSInfo[i].g_arrMarks[u].ref_intDisplayUnitMode = m_smCustomizeInfo.g_intMarkUnitDisplay;
                            m_smVSInfo[i].g_arrMarks[u].SetCalibrationData(
                                                             m_smVSInfo[i].g_fCalibPixelX,
                                                             m_smVSInfo[i].g_fCalibPixelY, m_smCustomizeInfo.g_intMarkUnitDisplay);
                        }
                    }
                }
            }


            m_FileHandle.WriteEndElement();
            STDeviceEdit.XMLChangesTracing("General", m_smProductionInfo.g_strLotID);
            STDeviceEdit.CopySettingFile(AppDomain.CurrentDomain.BaseDirectory, "Option.xml");

            m_FileHandle.WriteSectionElement("TCPIP");
            m_FileHandle.WriteElement1Value("MainPort", txt_TCPIPPort.Text);
            m_FileHandle.WriteElement1Value("Enable", chk_EnableTCPIP.Checked);
            m_FileHandle.WriteElement1Value("ReportAdd", txt_TCPIPReportPath.Text);
            m_FileHandle.WriteElement1Value("TimeOut", txt_TCPIPTimeout.Text);
            m_FileHandle.WriteElement1Value("RetriesCount", txt_TCPIPRetriesCount.Text);
            m_FileHandle.WriteElement1Value("ExportReport", chk_ExportLotReport.Checked);
            m_FileHandle.WriteElement1Value("ReportFormat", cbo_ReportFormat.SelectedIndex);
            m_FileHandle.WriteElement1Value("IPAdd", txt_IPAddress.Text);

            for (int i = 0; i < dgd_VisionList.RowCount; i++)
            {
                if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                {
                    m_FileHandle.WriteElement1Value(dgd_VisionList.Rows[i].Cells[0].Value.ToString() + "_TCPIPIO", dgd_VisionList.Rows[i].Cells[1].Value.ToString());
                    m_FileHandle.WriteElement1Value(dgd_VisionList.Rows[i].Cells[0].Value.ToString() + "_VisionID", dgd_VisionList.Rows[i].Cells[2].Value.ToString());
                }
                else
                {
                    m_FileHandle.WriteElement1Value(dgd_VisionList.Rows[i].Cells[0].Value.ToString(), dgd_VisionList.Rows[i].Cells[1].Value.ToString());
                    m_FileHandle.WriteElement1Value(dgd_VisionList.Rows[i].Cells[0].Value.ToString() + "_VisionID", dgd_VisionList.Rows[i].Cells[2].Value.ToString());
                }
            }

            m_FileHandle.WriteEndElement();
            STDeviceEdit.XMLChangesTracing("TCPIP", m_smProductionInfo.g_strLotID);
            for (int i = 0; i < dgd_VisionList.RowCount; i++)
            {
                STDeviceEdit.XMLChangesTracing(dgd_VisionList.Rows[i].Cells[0].Value.ToString(), m_smProductionInfo.g_strLotID);
            }
            STDeviceEdit.CopySettingFile(AppDomain.CurrentDomain.BaseDirectory, "Option.xml");

            if (cbo_CommPort.SelectedItem != null)
            {
                m_FileHandle.WriteSectionElement("SerialPort");
                m_FileHandle.WriteElement1Value("CommPort", cbo_CommPort.SelectedItem.ToString());
                m_FileHandle.WriteElement1Value("TimeOut", txt_SerialPortTimeOut.Text);
                m_FileHandle.WriteElement1Value("RetriesCount", txt_SerialPortRetriesCount.Text);
                m_FileHandle.WriteElement1Value("BitsPerSecond", cbo_BitsPerSecond.SelectedItem.ToString());
                m_FileHandle.WriteElement1Value("DataBits", cbo_DataBits.SelectedItem.ToString());
                m_FileHandle.WriteElement1Value("Parity", cbo_Parity.SelectedIndex);
                m_FileHandle.WriteElement1Value("StopBits", cbo_StopBits.SelectedIndex);
                m_FileHandle.WriteElement1Value("Enable", chk_SerialPortEnable.Checked);
            }
            m_FileHandle.WriteEndElement();
            STDeviceEdit.XMLChangesTracing("SerialPort", m_smProductionInfo.g_strLotID);

            m_FileHandle.WriteSectionElement("Network");
            m_FileHandle.WriteElement1Value("WantNetwork", chk_WantUseNetwork.Checked);
            m_FileHandle.WriteElement1Value("NetworkPasswordLimit", chk_NetworkPasswordLimitCheckBox.Checked);
            m_FileHandle.WriteElement1Value("NetworkHostIP", txt_HostIPEditBox.Text);
            m_FileHandle.WriteElement1Value("NetworkUser", txt_NetworkUsernameEditBox.Text);
            m_FileHandle.WriteElement1Value("NetworkPassword", txt_NetworkPasswordEditBox.Text);
            m_FileHandle.WriteElement1Value("DeviceUploadDir", txt_DeviceUploadDirEditBox.Text);
            m_FileHandle.WriteElement1Value("NetworkVisionLotReportUploadDir", txt_VisionLotReportUploadDirEditBox.Text);

            m_FileHandle.WriteSectionElement("SECSGEM");
            m_FileHandle.WriteElement1Value("WantSECSGEM", chk_WantUseSECSGEM.Checked);
            m_FileHandle.WriteElement1Value("SECSGEMSharedFolderDir", txt_SECSGEMSharedFolderPath.Text);
            m_FileHandle.WriteElement1Value("SECSGEMMaxNoOfCoplanPad", txt_MaxNoOfCoplanPad.Text);

            m_FileHandle.WriteEndElement();
            STDeviceEdit.XMLChangesTracing("Network", m_smProductionInfo.g_strLotID);
            //STDeviceEdit.CopySettingFile(AppDomain.CurrentDomain.BaseDirectory, "Option.xml");
            
        }

        public string GetNetworkFolders(FolderBrowserDialog oFolderBrowserDialog)
        {
            NetworkFolderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            NetworkFolderBrowserDialog.SelectedPath = "\\" + txt_HostIPEditBox.Text;
        
            if (NetworkFolderBrowserDialog.ShowDialog() == DialogResult.OK)
                return NetworkFolderBrowserDialog.SelectedPath;
            else
                return "";
        }




        private void chk_SerialPortEnable_Click(object sender, EventArgs e)
        {
            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
            {
                if (chk_SerialPortEnable.Checked)
                    chk_SerialPortEnable.Text = "Enabled";
                else
                    chk_SerialPortEnable.Text = "Disabled";
            }
            else
            {
                if (chk_SerialPortEnable.Checked)
                    chk_SerialPortEnable.Text = "启用";
                else
                    chk_SerialPortEnable.Text = "禁用";
            }
        }


        private void OkButton_Click(object sender, EventArgs e)
        {
           // m_objAVTFireGrab.OFFCamera();
            if ((int.Parse(txt_FailImagePics.Text) < 10) || (int.Parse(txt_FailImagePics.Text) > 10000))
            {
                SRMMessageBox.Show("Please insert 10 to 10000 pics fail images!",
                    "Application Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if ((int.Parse(txt_PassImagePics.Text) < 1) || (int.Parse(txt_PassImagePics.Text) > 500))
            {
                SRMMessageBox.Show("Please insert 1 to 500 pics pass images!",
                    "Application Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (m_smCustomizeInfo.g_blnConfigShowTCPIC)
            {
                if (!CheckPortAvailable())
                    return;

                if (chk_ExportLotReport.Checked)
                {
                    if (!Directory.Exists(txt_TCPIPReportPath.Text))
                    {
                        SRMMessageBox.Show("Lot Report Path doesn't exist!", "TCPIP Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }

                    if (!NetworkTransfer.IsConnectionPass(txt_IPAddress.Text))
                    {
                        SRMMessageBox.Show("Fail to ping IP address!", "TCPIP Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
            }

            if (chk_WantUseNetwork.Checked)
            {
                if (!Directory.Exists(txt_DeviceUploadDirEditBox.Text))
                {
                    SRMMessageBox.Show("Device Upload Path doesn't exist!", "Network Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                if (!Directory.Exists(txt_VisionLotReportUploadDirEditBox.Text))
                {
                    SRMMessageBox.Show("Vision Lot Report Path doesn't exist!", "Network Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                if (!NetworkTransfer.IsConnectionPass(txt_HostIPEditBox.Text))
                {
                    SRMMessageBox.Show("Fail to ping IP address!", "Network Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }

            if (chk_WantUseSECSGEM.Checked)
            {
                if (!Directory.Exists(txt_SECSGEMSharedFolderPath.Text))
                {
                    SRMMessageBox.Show("SECSGEM Shared Folder Path doesn't exist!", "Network Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }

            //2020-11-20 ZJYEOH : Copy current recipe calibration file to global file
            if (chk_GlobalSharingCalibrationData.Checked && !m_blnGlobalSharingCalibrationDataPrev)
            {
                if (SRMMessageBox.Show("Save Recipe " + m_smProductionInfo.g_strRecipeID + " Calibration Data as Global Sharing?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    for (int i = 0; i < m_smVSInfo.Length; i++)
                    {
                        if (m_smVSInfo[i] == null)
                            continue;

                        if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smProductionInfo.g_strRecipeID + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\Calibration.xml"))
                        {
                            File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smProductionInfo.g_strRecipeID + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\Calibration.xml",
                                AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml",
                                true);
                        }


                        //2020-11-25 ZJYEOH : Copy global calibration data to local folder
                        if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml"))
                        {
                            string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                            for (int j = 0; j < arrDirectory.Length; j++)
                            {
                                if (Directory.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                                {
                                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml", arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml", true);

                                    File.WriteAllText(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CalibrationSharingMode.txt", "1");
                                }
                            }
                        }

                    }
                }
                else
                {
                    for (int i = 0; i < m_smVSInfo.Length; i++)
                    {
                        if (m_smVSInfo[i] == null)
                            continue;
                        
                        if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml"))
                        {
                            SRMMessageBox.Show("No Global Calibration File Found! Please save Global Calibration File at least one time", "Error", MessageBoxButtons.OK);
                            return;
                        }

                    }

                    for (int i = 0; i < m_smVSInfo.Length; i++)
                    {
                        if (m_smVSInfo[i] == null)
                            continue;
                        
                        if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml"))
                        {
                            string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                            for (int j = 0; j < arrDirectory.Length; j++)
                            {
                                if (Directory.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                                {
                                    File.WriteAllText(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CalibrationSharingMode.txt", "1");
                                }
                            }
                        }

                    }
                }
            }
            else if (!chk_GlobalSharingCalibrationData.Checked)
            {
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;
                    
                    //2020-11-25 ZJYEOH : Copy global calibration data to local folder
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml"))
                    {
                        string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                        for (int j = 0; j < arrDirectory.Length; j++)
                        {
                            if (Directory.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                            {
                                File.WriteAllText(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CalibrationSharingMode.txt", "0");
                            }
                        }
                    }

                }
            }


            //2020-11-20 ZJYEOH : Copy current recipe Camera file to global file
            if (chk_GlobalSharingCameraData.Checked && !m_blnGlobalSharingCameraDataPrev)
            {
                if (SRMMessageBox.Show("Save Recipe " + m_smProductionInfo.g_strRecipeID + " Camera Data as Global Sharing?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    for (int i = 0; i < m_smVSInfo.Length; i++)
                    {
                        if (m_smVSInfo[i] == null)
                            continue;

                        if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smProductionInfo.g_strRecipeID + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\Camera.xml"))
                        {
                            File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smProductionInfo.g_strRecipeID + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\Camera.xml",
                                AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml",
                                true);
                        }


                        //2020-11-25 ZJYEOH : Copy global Camera data to local folder
                        if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml"))
                        {
                            string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                            for (int j = 0; j < arrDirectory.Length; j++)
                            {
                                if (Directory.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                                {
                                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml", arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml", true);

                                    File.WriteAllText(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CameraSharingMode.txt", "1");
                                }
                            }
                        }

                    }

                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smProductionInfo.g_strRecipeID + "\\Camera.xml"))
                    {
                        File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smProductionInfo.g_strRecipeID + "\\Camera.xml",
                            AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + "GlobalCamera.xml",
                            true);
                    }


                    //2020-11-25 ZJYEOH : Copy global Camera data to local folder
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\GlobalCamera.xml"))
                    {
                        string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                        for (int j = 0; j < arrDirectory.Length; j++)
                        {
                            if (Directory.Exists(arrDirectory[j] + "\\"))
                            {
                                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\GlobalCamera.xml", arrDirectory[j] + "\\GlobalCamera.xml", true);

                                File.WriteAllText(arrDirectory[j] + "\\" + "CameraSharingMode.txt", "1");
                            }
                        }
                    }
                }
                else
                {
                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\GlobalCamera.xml"))
                    {
                        SRMMessageBox.Show("No Global Camera File Found! Please save Global Camera File at least one time", "Error", MessageBoxButtons.OK);
                        return;
                    }

                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\GlobalCamera.xml"))
                    {
                        string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                        for (int j = 0; j < arrDirectory.Length; j++)
                        {
                            if (Directory.Exists(arrDirectory[j] + "\\"))
                            {
                                File.WriteAllText(arrDirectory[j] + "\\" + "CameraSharingMode.txt", "1");
                            }
                        }
                    }

                    for (int i = 0; i < m_smVSInfo.Length; i++)
                    {
                        if (m_smVSInfo[i] == null)
                            continue;
                        
                        if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml"))
                        {
                            string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                            for (int j = 0; j < arrDirectory.Length; j++)
                            {
                                if (Directory.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                                {
                                    File.WriteAllText(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CameraSharingMode.txt", "1");
                                }
                            }
                        }

                    }

                   
                }
            }
            else if (!chk_GlobalSharingCameraData.Checked)
            {
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    //2020-11-25 ZJYEOH : Copy global Camera data to local folder
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml"))
                    {
                        string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                        for (int j = 0; j < arrDirectory.Length; j++)
                        {
                            if (Directory.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                            {
                                File.WriteAllText(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CameraSharingMode.txt", "0");
                            }
                        }
                    }

                    //2020-11-25 ZJYEOH : Copy global Camera data to local folder
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\GlobalCamera.xml"))
                    {
                        string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                        for (int j = 0; j < arrDirectory.Length; j++)
                        {
                            if (Directory.Exists(arrDirectory[j] + "\\"))
                            {
                                File.WriteAllText(arrDirectory[j] + "\\" + "CameraSharingMode.txt", "0");
                            }
                        }
                    }

                }
            }

            WriteToFile();

            this.DialogResult = DialogResult.OK;
        }





        private void AppConfigForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }

        private void srmComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btn_ReportPathButton_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            string path = GetNetworkFolders(NetworkFolderBrowserDialog);
            if (path != "")
                txt_TCPIPReportPath.Text = path;
        }

        private void DeviceUploadBrowseButton_Click(object sender, EventArgs e)
        {
            string path = GetNetworkFolders(NetworkFolderBrowserDialog);
            if (path != "")
                txt_DeviceUploadDirEditBox.Text = path;
        }

        private void VisionLotReportUploadBrowseButton_Click(object sender, EventArgs e)
        {
            string path = GetNetworkFolders(NetworkFolderBrowserDialog);
            if (path != "")
                txt_VisionLotReportUploadDirEditBox.Text = path;
        }

        private void chk_EnableTCPIP_Click(object sender, EventArgs e)
        {
            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
            {
                if (chk_EnableTCPIP.Checked && chk_EnableTCPIP.Text != "Enabled")
                    chk_EnableTCPIP.Text = "Enabled";
                else if (!chk_EnableTCPIP.Checked && chk_EnableTCPIP.Text != "Disabled")
                    chk_EnableTCPIP.Text = "Disabled";
            }
            else
            {
                if (chk_EnableTCPIP.Checked && chk_EnableTCPIP.Text != "启用")
                    chk_EnableTCPIP.Text = "启用";
                else if (!chk_EnableTCPIP.Checked && chk_EnableTCPIP.Text != "禁用")
                    chk_EnableTCPIP.Text = "禁用";
            }
        }

        private void btn_BrowseSECSGEMSharedFolderPath_Click(object sender, EventArgs e)
        {
            string path = GetNetworkFolders(NetworkFolderBrowserDialog);
            if (path != "")
                txt_SECSGEMSharedFolderPath.Text = path;
        }

        private void chk_ExportLotReport_Click(object sender, EventArgs e)
        {
            grp_ExportLotReport.Visible = chk_ExportLotReport.Checked;
        }
        
    }
}