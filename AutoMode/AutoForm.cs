using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using TD.Eyefinder;
using Common;
using SharedMemory;
using VisionModule;
using VisionProcessing;
using ImageAcquisition;
using CrystalDecisions.CrystalReports.Engine;

namespace AutoMode
{
    public partial class AutoForm : Form
    {
        #region Constant Variables

        private const int SC_SIZE = 0xF000;
        private const int SC_MOVE = 0xF010;
        private const int SC_MINIMIZE = 0xF020;
        private const int SC_RESTORE = 0xF120;

        #endregion

        #region Members Variables
        private bool m_blnWantSaveAnotherLot = false;
        private int m_intVisionInvolved = 0;

        private bool m_blnBlockProductionCheckStatus = false;
        private bool m_blnInitDone = false;
        private bool m_blnDeviceReady = false;
        private bool m_blnGroupGRR = false;
        private bool m_blnchecktypo = false;
        private bool[] m_blnImageLoadDisable = new bool[10];
        private bool m_blnDebugRunning = false;
        private int m_intSelectedVisionStation = -999;
        private int m_intUserGroup = 5;
        private string m_strSelectedRecipePrev = "";
        private string[] m_arrSelectedRecipePrev = new string[10];
        private bool m_blnUseServerRecipePrev = false;
        private string[] m_arrStrLoadImagePathPrev = new string[10];
        private int m_intVisionCount = 0;

        private FileSorting m_objTimeComparer = new FileSorting();
        private TrackLog m_objTrack = new TrackLog();

        private AutoDisplayAllPage m_objDisplay1Page;
        private AutoDisplayAllPage m_objDisplay2Page;
        private AutoDisplayAllPage m_objDisplay3Page;
        private VisionPage[] m_objVisionPage = new VisionPage[10];
        private VisionComThread[] m_smVSComThread;
        private TCPIPIO[] m_smVSTCPIPIO;
        private RS232 m_thCOMMPort;
        private IDSuEyeCamera m_objIDSCamera;
        private ReportDocument m_report = new ReportDocument();
        private SRMWaitingFormThread m_thWaitingFormThread;
        private UserRight m_objUserRight = new UserRight();

        #endregion

        #region Shared Memory Variables

        private Thread m_Thread;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo[] m_smVSInfo;

        //private VisionInfo m_smStation1 = new VisionInfo();
        //private VisionInfo m_smStation2 = new VisionInfo();
        //private VisionInfo m_smStation3 = new VisionInfo();
        //private VisionInfo m_smStation4 = new VisionInfo();
        //private VisionInfo m_smStation5 = new VisionInfo();
        //private VisionInfo m_smStation6 = new VisionInfo();
        //private VisionInfo m_smStation7 = new VisionInfo();
        //private VisionInfo m_smStation8 = new VisionInfo();
        //private VisionInfo m_smStation9 = new VisionInfo();
        //private VisionInfo m_smStation10 = new VisionInfo();

        #endregion

        public AutoForm(CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, VisionInfo[] smVSInfo,
            int intUserGroup, VisionComThread[] smVSComThread, RS232 thCOMMPort, TCPIPIO[] smVSTCPIPIO)
        {
            InitializeComponent();

            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_smVSComThread = smVSComThread;
            m_smVSTCPIPIO = smVSTCPIPIO;
            m_smVSInfo = smVSInfo;
            m_thCOMMPort = thCOMMPort;
            m_intUserGroup = intUserGroup;

            if (m_intSelectedVisionStation == -999)
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subKey = key.CreateSubKey("SVG\\AutoMode");
                m_intSelectedVisionStation = Convert.ToInt32(subKey.GetValue("SelectedPane", 0));
            }

            m_blnDeviceReady = true;

            STTrackLog.WriteLine("AutoForm 1");
            if (!IsDeviceReady())
            {
                AbortState();
                m_blnDeviceReady = false;
            }

            STTrackLog.WriteLine("AutoForm 2");
            ReadRegistry();

            STTrackLog.WriteLine("AutoForm 3");
            GetVisionName();
            STTrackLog.WriteLine("AutoForm 4");
            CustomizeGUI();
            STTrackLog.WriteLine("AutoForm 5");
            SetCustomView2();
            STTrackLog.WriteLine("AutoForm 6");
            CreateProductionImageDirectory();
            STTrackLog.WriteLine("AutoForm 7");
            UpdateNewLotButtonGUI();
            STTrackLog.WriteLine("AutoForm 8");
            m_blnInitDone = true;
            STTrackLog.WriteLine("AutoForm 9");
        }


        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message message)
        {
            // Listen for operating system messages.
            switch (message.WParam.ToInt32() & 0xfff0)
            {
                case SC_SIZE:
                    break;
                case SC_MOVE:
                    break;
                case SC_MINIMIZE:
                    break;
                case SC_RESTORE:
                    break;
                default:
                    base.WndProc(ref message);
                    break;
            }
        }

        /// <summary>
        /// Check to ensure the selected device no is available on system
        /// </summary>
        /// <returns>true if available and otherwise false</returns>
        private bool IsDeviceReady()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            string strSelectedRecipe = (string)subkey1.GetValue("SelectedRecipeID", "Default");

            m_smProductionInfo.g_strRecipeID = strSelectedRecipe;
            for (int i = 0; i < 10; i++)
            {
                m_smProductionInfo.g_arrSingleRecipeID[i] = (string)subkey1.GetValue("SingleRecipeID" + i.ToString(), "");
                if (m_smProductionInfo.g_arrSingleRecipeID[i] == "")
                    m_smProductionInfo.g_arrSingleRecipeID[i] = m_smProductionInfo.g_strRecipeID;
            }

            string strDirectoryName = "";
            string strTargetDirectory = "";
            bool blnUseServerRecipe = false;
            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
            {
                if (NetworkTransfer.IsConnectionPass(m_smCustomizeInfo.g_strHostIP))
                {
                    if (Directory.Exists(m_smCustomizeInfo.g_strDeviceUploadDir))
                    {
                        blnUseServerRecipe = true;
                        strTargetDirectory = m_smCustomizeInfo.g_strDeviceUploadDir + "\\DeviceNo";
                    }
                    else
                    {
                        SRMMessageBox.Show("Recipe server path does not exist!", "Device No Setting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
                else
                {
                    SRMMessageBox.Show("Fail to connect to recipe server!", "Device No Setting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else
            {
                strTargetDirectory = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo";
            }

            string[] strDirectoryEntries = Directory.GetDirectories(strTargetDirectory);
            int intDirectoryLength = strTargetDirectory.Length + 1;

            if (strDirectoryEntries.Length == 0)
            {
                SRMMessageBox.Show("All Device No. Not Available! Please Contact SRM", "Device No Setting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
                List<bool> isSelected = new List<bool>(new bool[10]);
                foreach (string strDirectory in strDirectoryEntries)
                {
                    strDirectoryName = strDirectory.Remove(0, intDirectoryLength);
                    for (int i = 0; i < m_smVSInfo.Length; i++)
                    {
                        if (m_smVSInfo[i] == null || isSelected[i])
                            continue;

                        if (strDirectoryName == m_smProductionInfo.g_arrSingleRecipeID[i])
                        {
                            isSelected[i] = true;
                        }
                    }
                }

                int intCount = 0;
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                    {
                        isSelected.RemoveAt(intCount);

                    }
                    else
                        intCount++;
                }

                if (isSelected.Contains(false))
                {
                    SRMMessageBox.Show("Device No. Selected Is Not Available", "Device No Setting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            bool blnSameRecipe = true;
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (m_smProductionInfo.g_arrSingleRecipeID[i] != m_arrSelectedRecipePrev[i])
                {
                    blnSameRecipe = false;
                    break;
                }
            }

            // 2019 07 03 - JBTAN: dont have to load recipe to tempRecipe every time, only load if:
            //1. init and load from server
            //2. switch from load from local to load from server
            //3. change recipe during load from server
            if (blnUseServerRecipe && !m_blnUseServerRecipePrev || blnUseServerRecipe && (!blnSameRecipe))
            {
                StartWaiting("Loading Recipe...");
                // Delete previous TempRecipe, then transfer selected recipe from server to TempRecipe, finally set the tempRecipe path to DeviceNoPath 
                DeleteFilesAndFoldersRecursively(AppDomain.CurrentDomain.BaseDirectory + "TempRecipe");
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "TempRecipe");
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    if (m_smProductionInfo.g_arrSingleRecipeID[i] != m_arrSelectedRecipePrev[i])
                        NetworkTransfer.TransferFile_MappedNetwork(strTargetDirectory + "\\" + strSelectedRecipe, AppDomain.CurrentDomain.BaseDirectory + "TempRecipe\\" + strSelectedRecipe);
                }
                StopWaiting();
                m_smProductionInfo.g_strRecipePath = AppDomain.CurrentDomain.BaseDirectory + "TempRecipe\\";
            }
            else
            {
                m_smProductionInfo.g_strRecipePath = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\";
            }
            
            if (m_intSelectedVisionStation <= 0)
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                {
                    if (blnUseServerRecipe)
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Server)";
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Local)";
                }
                else
                    lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID;
            }
            else
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                {
                    if (blnUseServerRecipe)
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Server)";
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Local)";
                }
                else
                    lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1];
            }

            m_smProductionInfo.g_strLotID = (string)subkey1.GetValue("LotNo", "SRM");
            for (int i = 0; i < 10; i++)
            {
                m_smProductionInfo.g_arrSingleLotID[i] = (string)subkey1.GetValue("SingleLotNo" + i.ToString(), "");
                if (m_smProductionInfo.g_arrSingleLotID[i] == "")
                    m_smProductionInfo.g_arrSingleLotID[i] = m_smProductionInfo.g_strLotID;

                m_smProductionInfo.g_arrIsNewLot[i] = (bool)subkey1.GetValue("IsNewLot" + i.ToString(), false).Equals("True");
            }
            m_smProductionInfo.g_strOperatorID = (string)subkey1.GetValue("OperatorID", "Op");
            m_smProductionInfo.g_strLotStartTime = (string)subkey1.GetValue("LotStartTime", "0");
            // if registry LotStartTime no exist, use current time and set it to registry so that datetime is compatible
            if (m_smProductionInfo.g_strLotStartTime == "0")
            {
                m_smProductionInfo.g_strLotStartTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                subkey1.SetValue("LotStartTime", m_smProductionInfo.g_strLotStartTime);
            }
            return true;
        }



        /// <summary>
        /// Prevent user to start production because device no is unavailable
        /// </summary>
        private void AbortState()
        {
            btn_ProductionAll.Enabled = false;
            btn_ProductionAll.Checked = false;
            btn_ProductionAll.Text = GetLanguageText("Prod All");
            btn_Production.Enabled = false;
            btn_Production.Checked = false;
            btn_Production.Text = GetLanguageText("Production");
            btn_NewLot.Enabled = true;
            btn_Exit.Enabled = true;

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null) continue;
                m_smVSInfo[i].g_intMachineStatus = 1;
            }
        }

        private string GetLanguageText(string strText)
        {
            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
            {
                string strResult = "";
                switch (strText)
                {
                    case "Prod All":
                        strResult = "全部生产";
                        break;
                    case "Production":
                        strResult = "生产";
                        break;
                    case "Stop":
                        strResult = "停止";
                        break;
                    case "Live":
                        strResult = "连续攫取";
                        break;
                    case "Stop Live":
                        strResult = "停止攫取";
                        break;
                    case "Stop All":
                        strResult = "全部停止";
                        break;
                    case "Recipe":
                        strResult = "菜单";
                        break;
                    case "New Lot":
                        strResult = "新批次";
                        break;
                    case "End Lot":
                        strResult = "结束批次";
                        break;
                }

                return strResult;
            }
            //else if (m_smCustomizeInfo.g_intLanguageCulture == 4)
            //{

            //}
            else
                return strText;
        }

        /// <summary>
        /// Add suitable page into tab page control such as vision 1, vision 2, Vision info, ... and so on.
        /// </summary>
        private void AddAutoTabPage()
        {
            int intVisionCount = 0;

            if ((m_smCustomizeInfo.g_intVisionMask & 0x01) > 0)
            {
                int XX = tabPage_Vision1.Width;
                int YY = tabPage_Vision1.Height;
                m_objVisionPage[0].TopLevel = false;
                tabPage_Vision1.Controls.Add(m_objVisionPage[0]);
                tabPage_Vision1.Controls[0].Show();
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x02) > 0)
            {
                m_objVisionPage[1].TopLevel = false;
                tabPage_Vision2.Controls.Add(m_objVisionPage[1]);
                tabPage_Vision2.Controls[0].Show();
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x04) > 0)
            {
                m_objVisionPage[2].TopLevel = false;
                tabPage_Vision3.Controls.Add(m_objVisionPage[2]);
                tabPage_Vision3.Controls[0].Show();
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x08) > 0)
            {
                m_objVisionPage[3].TopLevel = false;
                tabPage_Vision4.Controls.Add(m_objVisionPage[3]);
                tabPage_Vision4.Controls[0].Show();
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x10) > 0)
            {
                m_objVisionPage[4].TopLevel = false;
                tabPage_Vision5.Controls.Add(m_objVisionPage[4]);
                tabPage_Vision5.Controls[0].Show();
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x20) > 0)
            {
                m_objVisionPage[5].TopLevel = false;
                tabPage_Vision6.Controls.Add(m_objVisionPage[5]);
                tabPage_Vision6.Controls[0].Show();
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x40) > 0)
            {
                m_objVisionPage[6].TopLevel = false;
                tabPage_Vision7.Controls.Add(m_objVisionPage[6]);
                tabPage_Vision7.Controls[0].Show();
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x80) > 0)
            {
                m_objVisionPage[7].TopLevel = false;
                tabPage_Vision8.Controls.Add(m_objVisionPage[7]);
                tabPage_Vision8.Controls[0].Show();
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x100) > 0)
            {
                m_objVisionPage[8].TopLevel = false;
                tabPage_Vision9.Controls.Add(m_objVisionPage[8]);
                tabPage_Vision9.Controls[0].Show();
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x200) > 0)
            {
                m_objVisionPage[9].TopLevel = false;
                tabPage_Vision10.Controls.Add(m_objVisionPage[9]);
                tabPage_Vision10.Controls[0].Show();
                intVisionCount++;
            }

            m_objDisplay1Page.TopLevel = false;
            tabPage_Display1.Controls.Add(m_objDisplay1Page);
            tabPage_Display1.Controls[0].Show();

            //if (intVisionCount > 4)
            //{
            //    m_objDisplay2Page.TopLevel = false;
            //    tabPage_Display2.Controls.Add(m_objDisplay2Page);
            //    tabPage_Display2.Controls[0].Show();
            //}
            //if (intVisionCount > 8)
            //{
            //    m_objDisplay3Page.TopLevel = false;
            //    tabPage_Display3.Controls.Add(m_objDisplay3Page);
            //    tabPage_Display3.Controls[0].Show();
            //}
        }

        /// <summary>
        /// Change each vision mode and display correct machine status
        /// </summary>
        /// <param name="intVisionID">vision ID : 0= Wafer Ring, 1 = Bottom Die,...</param>
        /// <param name="intMachineStatus">1 = idle, 2 = production</param>
        private void ChangeSPC_LowYieldIndicator(int intVisionID, int intMachineStatus)
        {

        }

        private void ChangeSPC(int intVisionID, int intMachineStatus)
        {
            switch (intVisionID)
            {
                case 0:
                    if (intMachineStatus == 1 && lbl_Vision1Status.Text != "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision1.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision1Status.Text != "Idle") lbl_Vision1Status.Text = "Idle";
                                //lbl_Vision1Status.BackColor = Color.Yellow;
                                btn_Vision1Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision1.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision1Status.Text != "Idle") lbl_Vision1Status.Text = "Idle";
                                //lbl_Vision1Status.BackColor = Color.Yellow;
                                btn_Vision1Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision1.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision1Status.Text != "Idle") lbl_Vision1Status.Text = "Idle";
                            //lbl_Vision1Status.BackColor = Color.Yellow;
                            btn_Vision1Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 1 && lbl_Vision1Status.Text == "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision1.LargeImage = imageList_AutoForm.Images[5];
                                if (lbl_Vision1Status.Text != "Idle") lbl_Vision1Status.Text = "Idle";
                                //lbl_Vision1Status.BackColor = Color.Yellow;
                                btn_Vision1Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision1.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision1Status.Text != "Idle") lbl_Vision1Status.Text = "Idle";
                                //lbl_Vision1Status.BackColor = Color.Yellow;
                                btn_Vision1Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision1.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision1Status.Text != "Idle") lbl_Vision1Status.Text = "Idle";
                            //lbl_Vision1Status.BackColor = Color.Yellow;
                            btn_Vision1Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision1Status.Text != "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision1.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision1Status.Text = "Operating";
                                //lbl_Vision1Status.BackColor = Color.Red;
                                btn_Vision1Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision1.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision1Status.Text = "Operating";
                                //lbl_Vision1Status.BackColor = Color.Lime;
                                btn_Vision1Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision1.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision1Status.Text = "Operating";
                            //lbl_Vision1Status.BackColor = Color.Lime;
                            btn_Vision1Tab.ImageIndex = 1;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision1Status.Text == "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision1.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision1Status.Text = "Operating";
                                //lbl_Vision1Status.BackColor = Color.Red;
                                btn_Vision1Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision1.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision1Status.Text = "Operating";
                                //lbl_Vision1Status.BackColor = Color.Lime;
                                btn_Vision1Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision1.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision1Status.Text = "Operating";
                            //lbl_Vision1Status.BackColor = Color.Lime;
                            btn_Vision1Tab.ImageIndex = 1;

                        }
                    }
                    break;
                case 1:
                    if (intMachineStatus == 1 && lbl_Vision2Status.Text != "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision2.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision2Status.Text != "Idle") lbl_Vision2Status.Text = "Idle";
                                //lbl_Vision2Status.BackColor = Color.Yellow;
                                btn_Vision2Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision2.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision2Status.Text != "Idle") lbl_Vision2Status.Text = "Idle";
                                //lbl_Vision2Status.BackColor = Color.Yellow;
                                btn_Vision2Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision2.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision2Status.Text != "Idle") lbl_Vision2Status.Text = "Idle";
                            //lbl_Vision2Status.BackColor = Color.Yellow;
                            btn_Vision2Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 1 && lbl_Vision2Status.Text == "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision2.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision2Status.Text != "Idle") lbl_Vision2Status.Text = "Idle";
                                //lbl_Vision2Status.BackColor = Color.Yellow;
                                btn_Vision2Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision2.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision2Status.Text != "Idle") lbl_Vision2Status.Text = "Idle";
                                //lbl_Vision2Status.BackColor = Color.Yellow;
                                btn_Vision2Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision2.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision2Status.Text != "Idle") lbl_Vision2Status.Text = "Idle";
                            //lbl_Vision2Status.BackColor = Color.Yellow;
                            btn_Vision2Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision2Status.Text != "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision2.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision2Status.Text = "Operating";
                                //lbl_Vision2Status.BackColor = Color.Red;
                                btn_Vision2Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision2.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision2Status.Text = "Operating";
                                //lbl_Vision2Status.BackColor = Color.Lime;
                                btn_Vision2Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision2.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision2Status.Text = "Operating";
                            //lbl_Vision2Status.BackColor = Color.Lime;
                            btn_Vision2Tab.ImageIndex = 1;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision2Status.Text == "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision2.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision2Status.Text = "Operating";
                                //lbl_Vision2Status.BackColor = Color.Red;
                                btn_Vision2Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision2.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision2Status.Text = "Operating";
                                //lbl_Vision2Status.BackColor = Color.Lime;
                                btn_Vision2Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision2.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision2Status.Text = "Operating";
                            //lbl_Vision2Status.BackColor = Color.Lime;
                            btn_Vision2Tab.ImageIndex = 1;

                        }
                    }
                    break;
                case 2:
                    if (intMachineStatus == 1 && lbl_Vision3Status.Text != "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision3.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision3Status.Text != "Idle") lbl_Vision3Status.Text = "Idle";
                                //lbl_Vision3Status.BackColor = Color.Yellow;
                                btn_Vision3Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision3.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision3Status.Text != "Idle") lbl_Vision3Status.Text = "Idle";
                                //lbl_Vision3Status.BackColor = Color.Yellow;
                                btn_Vision3Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision3.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision3Status.Text != "Idle") lbl_Vision3Status.Text = "Idle";
                            //lbl_Vision3Status.BackColor = Color.Yellow;
                            btn_Vision3Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 1 && lbl_Vision3Status.Text == "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision3.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision3Status.Text != "Idle") lbl_Vision3Status.Text = "Idle";
                                //lbl_Vision3Status.BackColor = Color.Yellow;
                                btn_Vision3Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision3.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision3Status.Text != "Idle") lbl_Vision3Status.Text = "Idle";
                                //lbl_Vision3Status.BackColor = Color.Yellow;
                                btn_Vision3Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision3.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision3Status.Text != "Idle") lbl_Vision3Status.Text = "Idle";
                            //lbl_Vision3Status.BackColor = Color.Yellow;
                            btn_Vision3Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision3Status.Text != "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision3.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision3Status.Text = "Operating";
                                //lbl_Vision3Status.BackColor = Color.Red;
                                btn_Vision3Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision3.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision3Status.Text = "Operating";
                                //lbl_Vision3Status.BackColor = Color.Lime;
                                btn_Vision3Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision3.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision3Status.Text = "Operating";
                            //lbl_Vision3Status.BackColor = Color.Lime;
                            btn_Vision3Tab.ImageIndex = 1;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision3Status.Text == "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision3.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision3Status.Text = "Operating";
                                //lbl_Vision3Status.BackColor = Color.Red;
                                btn_Vision3Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision3.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision3Status.Text = "Operating";
                                //lbl_Vision3Status.BackColor = Color.Lime;
                                btn_Vision3Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision3.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision3Status.Text = "Operating";
                            //lbl_Vision3Status.BackColor = Color.Lime;
                            btn_Vision3Tab.ImageIndex = 1;

                        }
                    }
                    break;
                case 3:
                    if (intMachineStatus == 1 && lbl_Vision4Status.Text != "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision4.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision4Status.Text != "Idle") lbl_Vision4Status.Text = "Idle";
                                //lbl_Vision4Status.BackColor = Color.Yellow;
                                btn_Vision4Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision4.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision4Status.Text != "Idle") lbl_Vision4Status.Text = "Idle";
                                //lbl_Vision4Status.BackColor = Color.Yellow;
                                btn_Vision4Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision4.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision4Status.Text != "Idle") lbl_Vision4Status.Text = "Idle";
                            //lbl_Vision4Status.BackColor = Color.Yellow;
                            btn_Vision4Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 1 && lbl_Vision4Status.Text == "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision4.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision4Status.Text != "Idle") lbl_Vision4Status.Text = "Idle";
                                //lbl_Vision4Status.BackColor = Color.Yellow;
                                btn_Vision4Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision4.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision4Status.Text != "Idle") lbl_Vision4Status.Text = "Idle";
                                //lbl_Vision4Status.BackColor = Color.Yellow;
                                btn_Vision4Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision4.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision4Status.Text != "Idle") lbl_Vision4Status.Text = "Idle";
                            //lbl_Vision4Status.BackColor = Color.Yellow;
                            btn_Vision4Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision4Status.Text != "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision4.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision4Status.Text = "Operating";
                                //lbl_Vision4Status.BackColor = Color.Red;
                                btn_Vision4Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision4.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision4Status.Text = "Operating";
                                //lbl_Vision4Status.BackColor = Color.Lime;
                                btn_Vision4Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision4.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision4Status.Text = "Operating";
                            //lbl_Vision4Status.BackColor = Color.Lime;
                            btn_Vision4Tab.ImageIndex = 1;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision4Status.Text == "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision4.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision4Status.Text = "Operating";
                                //lbl_Vision4Status.BackColor = Color.Red;
                                btn_Vision4Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision4.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision4Status.Text = "Operating";
                                //lbl_Vision4Status.BackColor = Color.Lime;
                                btn_Vision4Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision4.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision4Status.Text = "Operating";
                            //lbl_Vision4Status.BackColor = Color.Lime;
                            btn_Vision4Tab.ImageIndex = 1;

                        }
                    }
                    break;
                case 4:
                    if (intMachineStatus == 1 && lbl_Vision5Status.Text != "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision5.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision5Status.Text != "Idle") lbl_Vision5Status.Text = "Idle";
                                //lbl_Vision5Status.BackColor = Color.Yellow;
                                btn_Vision5Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision5.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision5Status.Text != "Idle") lbl_Vision5Status.Text = "Idle";
                                //lbl_Vision5Status.BackColor = Color.Yellow;
                                btn_Vision5Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision5.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision5Status.Text != "Idle") lbl_Vision5Status.Text = "Idle";
                            //lbl_Vision5Status.BackColor = Color.Yellow;
                            btn_Vision5Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 1 && lbl_Vision5Status.Text == "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision5.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision5Status.Text != "Idle") lbl_Vision5Status.Text = "Idle";
                                //lbl_Vision5Status.BackColor = Color.Yellow;
                                btn_Vision5Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision5.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision5Status.Text != "Idle") lbl_Vision5Status.Text = "Idle";
                                //lbl_Vision5Status.BackColor = Color.Yellow;
                                btn_Vision5Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision5.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision5Status.Text != "Idle") lbl_Vision5Status.Text = "Idle";
                            //lbl_Vision5Status.BackColor = Color.Yellow;
                            btn_Vision5Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision5Status.Text != "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision5.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision5Status.Text = "Operating";
                                //lbl_Vision5Status.BackColor = Color.Red;
                                btn_Vision5Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision5.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision5Status.Text = "Operating";
                                //lbl_Vision5Status.BackColor = Color.Lime;
                                btn_Vision5Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision5.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision5Status.Text = "Operating";
                            //lbl_Vision5Status.BackColor = Color.Lime;
                            btn_Vision5Tab.ImageIndex = 1;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision5Status.Text == "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision5.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision5Status.Text = "Operating";
                                //lbl_Vision5Status.BackColor = Color.Red;
                                btn_Vision5Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision5.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision5Status.Text = "Operating";
                                //lbl_Vision5Status.BackColor = Color.Lime;
                                btn_Vision5Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision5.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision5Status.Text = "Operating";
                            //lbl_Vision5Status.BackColor = Color.Lime;
                            btn_Vision5Tab.ImageIndex = 1;

                        }
                    }
                    break;
                case 5:
                    if (intMachineStatus == 1 && lbl_Vision6Status.Text != "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision6.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision6Status.Text != "Idle") lbl_Vision6Status.Text = "Idle";
                                //lbl_Vision6Status.BackColor = Color.Yellow;
                                btn_Vision6Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision6.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision6Status.Text != "Idle") lbl_Vision6Status.Text = "Idle";
                                //lbl_Vision6Status.BackColor = Color.Yellow;
                                btn_Vision6Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision6.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision6Status.Text != "Idle") lbl_Vision6Status.Text = "Idle";
                            //lbl_Vision6Status.BackColor = Color.Yellow;
                            btn_Vision6Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 1 && lbl_Vision6Status.Text == "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision6.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision6Status.Text != "Idle") lbl_Vision6Status.Text = "Idle";
                                //lbl_Vision6Status.BackColor = Color.Yellow;
                                btn_Vision6Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision6.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision6Status.Text != "Idle") lbl_Vision6Status.Text = "Idle";
                                //lbl_Vision6Status.BackColor = Color.Yellow;
                                btn_Vision6Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision6.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision6Status.Text != "Idle") lbl_Vision6Status.Text = "Idle";
                            //lbl_Vision6Status.BackColor = Color.Yellow;
                            btn_Vision6Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision6Status.Text != "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision6.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision6Status.Text = "Operating";
                                //lbl_Vision6Status.BackColor = Color.Red;
                                btn_Vision6Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision6.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision6Status.Text = "Operating";
                                //lbl_Vision6Status.BackColor = Color.Lime;
                                btn_Vision6Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision6.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision6Status.Text = "Operating";
                            //lbl_Vision6Status.BackColor = Color.Lime;
                            btn_Vision6Tab.ImageIndex = 1;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision6Status.Text == "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision6.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision6Status.Text = "Operating";
                                //lbl_Vision6Status.BackColor = Color.Red;
                                btn_Vision6Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision6.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision6Status.Text = "Operating";
                                //lbl_Vision6Status.BackColor = Color.Lime;
                                btn_Vision6Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision6.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision6Status.Text = "Operating";
                            //lbl_Vision6Status.BackColor = Color.Lime;
                            btn_Vision6Tab.ImageIndex = 1;

                        }
                    }
                    break;
                case 6:
                    if (intMachineStatus == 1 && lbl_Vision7Status.Text != "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision7.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision7Status.Text != "Idle") lbl_Vision7Status.Text = "Idle";
                                //lbl_Vision7Status.BackColor = Color.Yellow;
                                btn_Vision7Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision7.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision7Status.Text != "Idle") lbl_Vision7Status.Text = "Idle";
                                //lbl_Vision7Status.BackColor = Color.Yellow;
                                btn_Vision7Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision7.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision7Status.Text != "Idle") lbl_Vision7Status.Text = "Idle";
                            //lbl_Vision7Status.BackColor = Color.Yellow;
                            btn_Vision7Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 1 && lbl_Vision7Status.Text == "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision7.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision7Status.Text != "Idle") lbl_Vision7Status.Text = "Idle";
                                //lbl_Vision7Status.BackColor = Color.Yellow;
                                btn_Vision7Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision7.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision7Status.Text != "Idle") lbl_Vision7Status.Text = "Idle";
                                //lbl_Vision7Status.BackColor = Color.Yellow;
                                btn_Vision7Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision7.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision7Status.Text != "Idle") lbl_Vision7Status.Text = "Idle";
                            //lbl_Vision7Status.BackColor = Color.Yellow;
                            btn_Vision7Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision7Status.Text != "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision7.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision7Status.Text = "Operating";
                                //lbl_Vision7Status.BackColor = Color.Red;
                                btn_Vision7Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision7.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision7Status.Text = "Operating";
                                //lbl_Vision7Status.BackColor = Color.Lime;
                                btn_Vision7Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision7.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision7Status.Text = "Operating";
                            //lbl_Vision7Status.BackColor = Color.Lime;
                            btn_Vision7Tab.ImageIndex = 1;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision7Status.Text == "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision7.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision7Status.Text = "Operating";
                                //lbl_Vision7Status.BackColor = Color.Red;
                                btn_Vision7Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision7.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision7Status.Text = "Operating";
                                //lbl_Vision7Status.BackColor = Color.Lime;
                                btn_Vision7Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision7.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision7Status.Text = "Operating";
                            //lbl_Vision7Status.BackColor = Color.Lime;
                            btn_Vision7Tab.ImageIndex = 1;

                        }
                    }
                    break;
                case 7:
                    if (intMachineStatus == 1 && lbl_Vision8Status.Text != "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision8.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision8Status.Text != "Idle") lbl_Vision8Status.Text = "Idle";
                                //lbl_Vision8Status.BackColor = Color.Yellow;
                                btn_Vision8Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision8.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision8Status.Text != "Idle") lbl_Vision8Status.Text = "Idle";
                                //lbl_Vision8Status.BackColor = Color.Yellow;
                                btn_Vision8Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision8.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision8Status.Text != "Idle") lbl_Vision8Status.Text = "Idle";
                            //lbl_Vision8Status.BackColor = Color.Yellow;
                            btn_Vision8Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 1 && lbl_Vision8Status.Text == "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision8.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision8Status.Text != "Idle") lbl_Vision8Status.Text = "Idle";
                                //lbl_Vision8Status.BackColor = Color.Yellow;
                                btn_Vision8Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision8.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision8Status.Text != "Idle") lbl_Vision8Status.Text = "Idle";
                                //lbl_Vision8Status.BackColor = Color.Yellow;
                                btn_Vision8Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision8.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision8Status.Text != "Idle") lbl_Vision8Status.Text = "Idle";
                            //lbl_Vision8Status.BackColor = Color.Yellow;
                            btn_Vision8Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision8Status.Text != "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision8.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision8Status.Text = "Operating";
                                //lbl_Vision8Status.BackColor = Color.Red;
                                btn_Vision8Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision8.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision8Status.Text = "Operating";
                                //lbl_Vision8Status.BackColor = Color.Lime;
                                btn_Vision8Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision8.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision8Status.Text = "Operating";
                            //lbl_Vision8Status.BackColor = Color.Lime;
                            btn_Vision8Tab.ImageIndex = 1;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision8Status.Text == "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision8.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision8Status.Text = "Operating";
                                //lbl_Vision8Status.BackColor = Color.Red;
                                btn_Vision8Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision8.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision8Status.Text = "Operating";
                                //lbl_Vision8Status.BackColor = Color.Lime;
                                btn_Vision8Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision8.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision8Status.Text = "Operating";
                            //lbl_Vision8Status.BackColor = Color.Lime;
                            btn_Vision8Tab.ImageIndex = 1;

                        }
                    }
                    break;
                case 8:
                    if (intMachineStatus == 1 && lbl_Vision9Status.Text != "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision9.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision9Status.Text != "Idle") lbl_Vision9Status.Text = "Idle";
                                //lbl_Vision9Status.BackColor = Color.Yellow;
                                btn_Vision9Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision9.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision9Status.Text != "Idle") lbl_Vision9Status.Text = "Idle";
                                //lbl_Vision9Status.BackColor = Color.Yellow;
                                btn_Vision9Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision9.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision9Status.Text != "Idle") lbl_Vision9Status.Text = "Idle";
                            //lbl_Vision9Status.BackColor = Color.Yellow;
                            btn_Vision9Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 1 && lbl_Vision9Status.Text == "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision9.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision9Status.Text != "Idle") lbl_Vision9Status.Text = "Idle";
                                //lbl_Vision9Status.BackColor = Color.Yellow;
                                btn_Vision9Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision9.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision9Status.Text != "Idle") lbl_Vision9Status.Text = "Idle";
                                //lbl_Vision9Status.BackColor = Color.Yellow;
                                btn_Vision9Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision9.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision9Status.Text != "Idle") lbl_Vision9Status.Text = "Idle";
                            //lbl_Vision9Status.BackColor = Color.Yellow;
                            btn_Vision9Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision9Status.Text != "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision9.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision9Status.Text = "Operating";
                                //lbl_Vision9Status.BackColor = Color.Red;
                                btn_Vision9Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision9.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision9Status.Text = "Operating";
                                //lbl_Vision9Status.BackColor = Color.Lime;
                                btn_Vision9Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision9.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision9Status.Text = "Operating";
                            //lbl_Vision9Status.BackColor = Color.Lime;
                            btn_Vision9Tab.ImageIndex = 1;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision9Status.Text == "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision9.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision9Status.Text = "Operating";
                                //lbl_Vision9Status.BackColor = Color.Red;
                                btn_Vision9Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision9.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision9Status.Text = "Operating";
                                //lbl_Vision9Status.BackColor = Color.Lime;
                                btn_Vision9Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision9.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision9Status.Text = "Operating";
                            //lbl_Vision9Status.BackColor = Color.Lime;
                            btn_Vision9Tab.ImageIndex = 1;

                        }
                    }
                    break;
                case 9:
                    if (intMachineStatus == 1 && lbl_Vision10Status.Text != "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision10.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision10Status.Text != "Idle") lbl_Vision10Status.Text = "Idle";
                                //lbl_Vision10Status.BackColor = Color.Yellow;
                                btn_Vision10Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision10.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision10Status.Text != "Idle") lbl_Vision10Status.Text = "Idle";
                                //lbl_Vision10Status.BackColor = Color.Yellow;
                                btn_Vision10Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision10.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision10Status.Text != "Idle") lbl_Vision10Status.Text = "Idle";
                            //lbl_Vision10Status.BackColor = Color.Yellow;
                            btn_Vision10Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 1 && lbl_Vision10Status.Text == "Idle")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision10.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision10Status.Text != "Idle") lbl_Vision10Status.Text = "Idle";
                                //lbl_Vision10Status.BackColor = Color.Yellow;
                                btn_Vision10Tab.ImageIndex = 6;

                            }
                            else
                            {
                                //NavigationPane_Vision10.LargeImage = imageList_AutoForm.Images[0];
                                if (lbl_Vision10Status.Text != "Idle") lbl_Vision10Status.Text = "Idle";
                                //lbl_Vision10Status.BackColor = Color.Yellow;
                                btn_Vision10Tab.ImageIndex = 0;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision10.LargeImage = imageList_AutoForm.Images[0];
                            if (lbl_Vision10Status.Text != "Idle") lbl_Vision10Status.Text = "Idle";
                            //lbl_Vision10Status.BackColor = Color.Yellow;
                            btn_Vision10Tab.ImageIndex = 0;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision10Status.Text != "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision10.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision10Status.Text = "Operating";
                                //lbl_Vision10Status.BackColor = Color.Red;
                                btn_Vision10Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision10.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision10Status.Text = "Operating";
                                //lbl_Vision10Status.BackColor = Color.Lime;
                                btn_Vision10Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision10.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision10Status.Text = "Operating";
                            //lbl_Vision10Status.BackColor = Color.Lime;
                            btn_Vision10Tab.ImageIndex = 1;

                        }
                    }
                    else if (intMachineStatus == 2 && lbl_Vision10Status.Text == "Operating")
                    {
                        if (m_smVSInfo[intVisionID].g_intTestedTotal != 0)
                        {
                            float fYield = (m_smVSInfo[intVisionID].g_intPassTotal / (float)m_smVSInfo[intVisionID].g_intTestedTotal) * 100;

                            if (m_smVSInfo[intVisionID].g_strVisionName != "Barcode" && fYield <= m_smVSInfo[intVisionID].g_fLowYield)
                            {
                                //NavigationPane_Vision10.LargeImage = imageList_AutoForm.Images[5];
                                lbl_Vision10Status.Text = "Operating";
                                //lbl_Vision10Status.BackColor = Color.Red;
                                btn_Vision10Tab.ImageIndex = 5;

                            }
                            else
                            {
                                //NavigationPane_Vision10.LargeImage = imageList_AutoForm.Images[1];
                                lbl_Vision10Status.Text = "Operating";
                                //lbl_Vision10Status.BackColor = Color.Lime;
                                btn_Vision10Tab.ImageIndex = 1;

                            }
                        }
                        else
                        {
                            //NavigationPane_Vision10.LargeImage = imageList_AutoForm.Images[1];
                            lbl_Vision10Status.Text = "Operating";
                            //lbl_Vision10Status.BackColor = Color.Lime;
                            btn_Vision10Tab.ImageIndex = 1;

                        }
                    }
                    break;
            }

            //switch (intVisionID)
            //{
            //    case 0:
            //        if (intMachineStatus == 1 && lbl_Vision1Status.Text != "Idle")
            //        {
            //            NavigationPane_Vision1.LargeImage = imageList_AutoForm.Images[0];
            //          if (lbl_Vision1Status.Text != "Idle") lbl_Vision1Status.Text = "Idle";
            //            //lbl_Vision1Status.BackColor = Color.Yellow;
            //            btn_Vision1Tab.ImageIndex = 0;
            //        }
            //        else if (intMachineStatus == 2 && lbl_Vision1Status.Text != "Operating")
            //        {
            //            NavigationPane_Vision1.LargeImage = imageList_AutoForm.Images[1];
            //            lbl_Vision1Status.Text = "Operating";
            //            //lbl_Vision1Status.BackColor = Color.Lime;
            //            btn_Vision1Tab.ImageIndex = 1;
            //        }
            //        break;
            //    case 1:
            //        if (intMachineStatus == 1 && lbl_Vision2Status.Text != "Idle")
            //        {
            //            //NavigationPane_Vision2.LargeImage = imageList_AutoForm.Images[0];
            //          if (lbl_Vision2Status.Text != "Idle") lbl_Vision2Status.Text = "Idle";
            //            //lbl_Vision2Status.BackColor = Color.Yellow;
            //            btn_Vision2Tab.ImageIndex = 0;
            //        }
            //        else if (intMachineStatus == 2 && lbl_Vision2Status.Text != "Operating")
            //        {
            //            //NavigationPane_Vision2.LargeImage = imageList_AutoForm.Images[1];
            //            lbl_Vision2Status.Text = "Operating";
            //            //lbl_Vision2Status.BackColor = Color.Lime;
            //            btn_Vision2Tab.ImageIndex = 0;
            //        }
            //        break;
            //    case 2:
            //        if (intMachineStatus == 1 && lbl_Vision3Status.Text != "Idle")
            //        {
            //            //NavigationPane_Vision3.LargeImage = imageList_AutoForm.Images[0];
            //          if (lbl_Vision3Status.Text != "Idle") lbl_Vision3Status.Text = "Idle";
            //            //lbl_Vision3Status.BackColor = Color.Yellow;
            //            btn_Vision3Tab.ImageIndex = 0;
            //        }
            //        else if (intMachineStatus == 2 && lbl_Vision3Status.Text != "Operating")
            //        {
            //            //NavigationPane_Vision3.LargeImage = imageList_AutoForm.Images[1];
            //            lbl_Vision3Status.Text = "Operating";
            //            //lbl_Vision3Status.BackColor = Color.Lime;
            //            btn_Vision3Tab.ImageIndex = 0;
            //        }
            //        break;
            //    case 3:
            //        if (intMachineStatus == 1 && lbl_Vision4Status.Text != "Idle")
            //        {
            //            //NavigationPane_Vision4.LargeImage = imageList_AutoForm.Images[0];
            //          if (lbl_Vision4Status.Text != "Idle") lbl_Vision4Status.Text = "Idle";
            //            //lbl_Vision4Status.BackColor = Color.Yellow;
            //            btn_Vision4Tab.ImageIndex = 0;
            //        }
            //        else if (intMachineStatus == 2 && lbl_Vision4Status.Text != "Operating")
            //        {
            //            //NavigationPane_Vision4.LargeImage = imageList_AutoForm.Images[1];
            //            lbl_Vision4Status.Text = "Operating";
            //            //lbl_Vision4Status.BackColor = Color.Lime;
            //            btn_Vision4Tab.ImageIndex = 0;
            //        }
            //        break;
            //    case 4:
            //        if (intMachineStatus == 1 && lbl_Vision5Status.Text != "Idle")
            //        {
            //            //NavigationPane_Vision5.LargeImage = imageList_AutoForm.Images[0];
            //          if (lbl_Vision5Status.Text != "Idle") lbl_Vision5Status.Text = "Idle";
            //            //lbl_Vision5Status.BackColor = Color.Yellow;
            //            btn_Vision5Tab.ImageIndex = 0;
            //        }
            //        else if (intMachineStatus == 2 && lbl_Vision5Status.Text != "Operating")
            //        {
            //            //NavigationPane_Vision5.LargeImage = imageList_AutoForm.Images[1];
            //            lbl_Vision5Status.Text = "Operating";
            //            //lbl_Vision5Status.BackColor = Color.Lime;
            //            btn_Vision5Tab.ImageIndex = 0;
            //        }
            //        break;
            //    case 5:
            //        if (intMachineStatus == 1 && lbl_Vision6Status.Text != "Idle")
            //        {
            //            //NavigationPane_Vision6.LargeImage = imageList_AutoForm.Images[0];
            //          if (lbl_Vision6Status.Text != "Idle") lbl_Vision6Status.Text = "Idle";
            //            //lbl_Vision6Status.BackColor = Color.Yellow;
            //            btn_Vision6Tab.ImageIndex = 0;
            //        }
            //        else if (intMachineStatus == 2 && lbl_Vision6Status.Text != "Operating")
            //        {
            //            //NavigationPane_Vision6.LargeImage = imageList_AutoForm.Images[1];
            //            lbl_Vision6Status.Text = "Operating";
            //            //lbl_Vision6Status.BackColor = Color.Lime;
            //            btn_Vision6Tab.ImageIndex = 0;
            //        }
            //        break;
            //    case 6:
            //        if (intMachineStatus == 1 && lbl_Vision7Status.Text != "Idle")
            //        {
            //            //NavigationPane_Vision7.LargeImage = imageList_AutoForm.Images[0];
            //          if (lbl_Vision7Status.Text != "Idle") lbl_Vision7Status.Text = "Idle";
            //            //lbl_Vision7Status.BackColor = Color.Yellow;
            //            btn_Vision7Tab.ImageIndex = 0;
            //        }
            //        else if (intMachineStatus == 2 && lbl_Vision7Status.Text != "Operating")
            //        {
            //            //NavigationPane_Vision7.LargeImage = imageList_AutoForm.Images[1];
            //            lbl_Vision7Status.Text = "Operating";
            //            //lbl_Vision7Status.BackColor = Color.Lime;
            //            btn_Vision7Tab.ImageIndex = 0;
            //        }
            //        break;
            //    case 7:
            //        if (intMachineStatus == 1 && lbl_Vision8Status.Text != "Idle")
            //        {
            //            //NavigationPane_Vision8.LargeImage = imageList_AutoForm.Images[0];
            //          if (lbl_Vision8Status.Text != "Idle") lbl_Vision8Status.Text = "Idle";
            //            //lbl_Vision8Status.BackColor = Color.Yellow;
            //            btn_Vision8Tab.ImageIndex = 0;
            //        }
            //        else if (intMachineStatus == 2 && lbl_Vision8Status.Text != "Operating")
            //        {
            //            //NavigationPane_Vision8.LargeImage = imageList_AutoForm.Images[1];
            //            lbl_Vision8Status.Text = "Operating";
            //            //lbl_Vision8Status.BackColor = Color.Lime;
            //            btn_Vision8Tab.ImageIndex = 0;
            //        }
            //        break;
            //    case 8:
            //        if (intMachineStatus == 1 && lbl_Vision9Status.Text != "Idle")
            //        {
            //            //NavigationPane_Vision9.LargeImage = imageList_AutoForm.Images[0];
            //          if (lbl_Vision9Status.Text != "Idle") lbl_Vision9Status.Text = "Idle";
            //            //lbl_Vision9Status.BackColor = Color.Yellow;
            //            btn_Vision9Tab.ImageIndex = 0;
            //        }
            //        else if (intMachineStatus == 2 && lbl_Vision9Status.Text != "Operating")
            //        {
            //            //NavigationPane_Vision9.LargeImage = imageList_AutoForm.Images[1];
            //            lbl_Vision9Status.Text = "Operating";
            //            //lbl_Vision9Status.BackColor = Color.Lime;
            //            btn_Vision9Tab.ImageIndex = 0;
            //        }
            //        break;
            //    case 9:
            //        if (intMachineStatus == 1 && lbl_Vision10Status.Text != "Idle")
            //        {
            //            //NavigationPane_Vision10.LargeImage = imageList_AutoForm.Images[0];
            //          if (lbl_Vision10Status.Text != "Idle") lbl_Vision10Status.Text = "Idle";
            //            //lbl_Vision10Status.BackColor = Color.Yellow;
            //            btn_Vision10Tab.ImageIndex = 0;
            //        }
            //        else if (intMachineStatus == 2 && lbl_Vision10Status.Text != "Operating")
            //        {
            //            //NavigationPane_Vision10.LargeImage = imageList_AutoForm.Images[1];
            //            lbl_Vision10Status.Text = "Operating";
            //            //lbl_Vision10Status.BackColor = Color.Lime;
            //            btn_Vision10Tab.ImageIndex = 0;
            //        }
            //        break;
            //}
        }

        /// <summary>
        /// Copy directory and all files inside the directory from source path to destination path
        /// </summary>
        /// <param name="strSource">source path</param>
        /// <param name="strDestination">destination path</param>
        private void CopyDirectory(string strSource, string strDestination)
        {
            if (strDestination[strDestination.Length - 1] != Path.DirectorySeparatorChar)
                strDestination += Path.DirectorySeparatorChar;

            Directory.CreateDirectory(strDestination);

            String[] strFiles = Directory.GetFileSystemEntries(strSource);
            foreach (string strElement in strFiles)
            {
                // Sub directories
                if (Directory.Exists(strElement))
                    CopyDirectory(strElement, strDestination + Path.GetFileName(strElement));
                // Files in directory
                else
                    File.Copy(strElement, strDestination + Path.GetFileName(strElement), true);
            }
        }

        /// <summary>
        /// Create new lot without end lot feature
        /// </summary>
        private void CreateNewLot()
        {
            m_smProductionInfo.g_blnEndLotStatus = false;
            //Save end lot status to registry
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey1 = key.CreateSubKey("SVG\\AutoMode");
            subKey1.SetValue("EndLotStatus", m_smProductionInfo.g_blnEndLotStatus);
            SaveLotFile();
            SaveReportToServer();
            SaveReportToHandler();
            CreateProductionImageDirectory();

            if (m_intSelectedVisionStation <= 0)
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                {
                    if (m_blnUseServerRecipePrev)
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Server)";
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Local)";
                }
                else
                    lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID;
            }
            else
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                {
                    if (m_blnUseServerRecipePrev)
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Server)";
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Local)";
                }
                else
                    lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1];
            }

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (m_smVSInfo[i].g_blnWantClearMarkTemplateWhenNewLot)
                {
                    m_objVisionPage[i].DeleteMarkTemplateWhenNewLot();
                }

                if (m_smVSInfo[i].g_blnWantClearSealTemplateWhenNewLot)
                {
                    m_objVisionPage[i].DeleteSealTemplateWhenNewLot();
                }

                if (m_smVSInfo[i].g_blnWantLoadRefTolWhenNewLot)
                {
                    if (m_smVSInfo[i].g_strBrowsePath == "")
                    {
                        SRMMessageBox.Show("Load Rolerance Reference File fail. Please set Tolerance Reference file directory first.");
                    }
                    else if (!File.Exists(m_smVSInfo[i].g_strBrowsePath + "\\" + m_smProductionInfo.g_strRecipeID + ".stol"))
                    {
                        SRMMessageBox.Show("Load Rolerance Reference File fail. Tolerance Reference file no exist.");
                    }
                    else
                        m_objVisionPage[i].LoadToleranceReference(m_smVSInfo[i].g_strBrowsePath + "\\" + m_smProductionInfo.g_strRecipeID + ".stol");
                }

                //If want CPK, reset CPK counter when new lot
                if (m_smVSInfo[i].g_blnCPKON)
                {
                    //Re-init CPK
                    m_smVSInfo[i].g_blnReInitCPK = false;
                }

                m_smVSInfo[i].g_intPassTotal = 0;
                m_smVSInfo[i].g_intMarkFailureTotal = 0;
                m_smVSInfo[i].g_intOrientFailureTotal = 0;
                m_smVSInfo[i].g_intPin1FailureTotal = 0;
                m_smVSInfo[i].g_intLeadFailureTotal = 0;
                m_smVSInfo[i].g_intPackageFailureTotal = 0;
                m_smVSInfo[i].g_intPadFailureTotal = 0;
                m_smVSInfo[i].g_intBarcodeFailureTotal = 0;
                m_smVSInfo[i].g_intSealFailureTotal = 0;
                m_smVSInfo[i].g_intSealDistanceFailureTotal = 0;
                m_smVSInfo[i].g_intSealOverHeatFailureTotal = 0;
                m_smVSInfo[i].g_intSealBrokenAreaFailureTotal = 0;
                m_smVSInfo[i].g_intSealBrokenGapFailureTotal = 0;
                m_smVSInfo[i].g_intSealSprocketHoleFailureTotal = 0;
                m_smVSInfo[i].g_intSealSprocketHoleDiameterFailureTotal = 0;
                m_smVSInfo[i].g_intSealSprocketHoleDefectFailureTotal = 0;
                m_smVSInfo[i].g_intSealSprocketHoleBrokenFailureTotal = 0;
                m_smVSInfo[i].g_intSealSprocketHoleRoundnessFailureTotal = 0;
                m_smVSInfo[i].g_intSealEdgeStraightnessFailureTotal = 0;
                m_smVSInfo[i].g_intCheckPresenceFailureTotal = 0;
                m_smVSInfo[i].g_intPositionFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadOffsetFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadAreaFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadDimensionFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadBrokenFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadExcessFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadSmearFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadStandOffFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadSpanFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadContaminationFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadMissingFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadOffsetFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadAreaFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadDimensionFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadPitchGapFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadBrokenFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadExcessFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadSmearFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadEdgeLimitFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadStandOffFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadEdgeDistanceFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadSpanFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadContaminationFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadMissingFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadColorDefectFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPkgDefectFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal = 0;
                m_smVSInfo[i].g_intSidePkgDefectFailureTotal = 0;
                m_smVSInfo[i].g_intSidePkgDimensionFailureTotal = 0;
                m_smVSInfo[i].g_intLeadOffsetFailureTotal = 0;
                m_smVSInfo[i].g_intLeadSkewFailureTotal = 0;
                m_smVSInfo[i].g_intLeadWidthFailureTotal = 0;
                m_smVSInfo[i].g_intLeadLengthFailureTotal = 0;
                m_smVSInfo[i].g_intLeadLengthVarianceFailureTotal = 0;
                m_smVSInfo[i].g_intLeadPitchGapFailureTotal = 0;
                m_smVSInfo[i].g_intLeadPitchVarianceFailureTotal = 0;
                m_smVSInfo[i].g_intLeadStandOffFailureTotal = 0;
                m_smVSInfo[i].g_intLeadStandOffVarianceFailureTotal = 0;
                m_smVSInfo[i].g_intLeadCoplanFailureTotal = 0;
                m_smVSInfo[i].g_intLeadAGVFailureTotal = 0;
                m_smVSInfo[i].g_intLeadSpanFailureTotal = 0;
                m_smVSInfo[i].g_intLeadSweepsFailureTotal = 0;
                m_smVSInfo[i].g_intLeadUnCutTiebarFailureTotal = 0;
                m_smVSInfo[i].g_intLeadMissingFailureTotal = 0;
                m_smVSInfo[i].g_intLeadContaminationFailureTotal = 0;
                m_smVSInfo[i].g_intLeadPkgDefectFailureTotal = 0;
                m_smVSInfo[i].g_intLeadPkgDimensionFailureTotal = 0;
                m_smVSInfo[i].g_intNoTemplateFailureTotal = 0;
                m_smVSInfo[i].g_intPkgDefectFailureTotal = 0;
                m_smVSInfo[i].g_intPkgColorDefectFailureTotal = 0;
                m_smVSInfo[i].g_intEdgeNotFoundFailureTotal = 0;
                m_smVSInfo[i].g_intAngleFailureTotal = 0;
                m_smVSInfo[i].g_intLowYieldUnitCount = 0;
                m_smVSInfo[i].g_intContinuousPassUnitCount = 0;
                m_smVSInfo[i].g_intContinuousFailUnitCount = 0;
                m_smVSInfo[i].g_intTestedTotal = 0;
                m_smVSInfo[i].g_intPassImageCount = 0;
                m_smVSInfo[i].g_intFailImageCount = 0;
                m_smVSInfo[i].g_intTotalImageCount = 0;
                m_smVSInfo[i].AT_VP_NewLot = true;
                m_smVSInfo[i].AT_VM_UpdateProductionDisplayAll = true;

                // 2020 03 27 - JBTAN: Save Vision Reset Count info
                m_smVSInfo[i].g_intVisionResetCount = 0;
                subKey1.SetValue("VS" + (i + 1) + "ResetCount", m_smVSInfo[i].g_intVisionResetCount);
                subKey1.SetValue("VS" + (i + 1) + "ResetCountTime", m_smProductionInfo.g_strLotStartTime);
            }

            m_smProductionInfo.VM_TH_UpdateCount = true;
            UpdateNewLotButtonGUI();
        }

        /// <summary>
        /// Create new lot without end lot on particular vision use by TCPIP or RS232
        /// </summary>
        /// <param name="intVision">vision no</param>
        private void CreateNewLot(int intVision)
        {
            try
            {
                m_smProductionInfo.g_blnEndLotStatus = false;
                //Save end lot status to registry
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subKey1 = key.CreateSubKey("SVG\\AutoMode");
                subKey1.SetValue("EndLotStatus", m_smProductionInfo.g_blnEndLotStatus);
                SaveLotFile();
                SaveReportToServer();
                SaveReportToHandler();
                CreateProductionImageDirectory();

                if (m_intSelectedVisionStation <= 0)
                {
                    if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    {
                        if (m_blnUseServerRecipePrev)
                            lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Server)";
                        else
                            lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Local)";
                    }
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID;
                }
                else
                {
                    if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    {
                        if (m_blnUseServerRecipePrev)
                            lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Server)";
                        else
                            lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Local)";
                    }
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1];
                }

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    //New lot on particular vision or all vision
                    if ((intVision & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) > 0 || intVision == 0 || m_smProductionInfo.g_arrVisionIDIndex[i] == 0)
                    {
                        if (m_smVSInfo[i].g_blnWantClearMarkTemplateWhenNewLot)
                        {
                            m_objVisionPage[i].DeleteMarkTemplateWhenNewLot();
                        }

                        if (m_smVSInfo[i].g_blnWantClearSealTemplateWhenNewLot)
                        {
                            m_objVisionPage[i].DeleteSealTemplateWhenNewLot();
                        }

                        if (m_smVSInfo[i].g_blnWantLoadRefTolWhenNewLot)
                        {
                            if (m_smVSInfo[i].g_strBrowsePath == "")
                            {
                                SRMMessageBox.Show("Load Rolerance Reference File fail. Please set Tolerance Reference file directory first.");
                            }
                            else if (!File.Exists(m_smVSInfo[i].g_strBrowsePath + "\\" + m_smProductionInfo.g_strRecipeID + ".stol"))
                            {
                                SRMMessageBox.Show("Load Rolerance Reference File fail. Tolerance Reference file no exist.");
                            }
                            else
                                m_objVisionPage[i].LoadToleranceReference(m_smVSInfo[i].g_strBrowsePath + "\\" + m_smProductionInfo.g_strRecipeID + ".stol");
                        }

                        //If want CPK, reset CPK counter when new lot
                        if (m_smVSInfo[i].g_blnCPKON)
                        {
                            //Re-init CPK
                            m_smVSInfo[i].g_blnReInitCPK = false;
                        }

                        m_smVSInfo[i].g_intPassTotal = 0;
                        m_smVSInfo[i].g_intMarkFailureTotal = 0;
                        m_smVSInfo[i].g_intOrientFailureTotal = 0;
                        m_smVSInfo[i].g_intPin1FailureTotal = 0;
                        m_smVSInfo[i].g_intLeadFailureTotal = 0;
                        m_smVSInfo[i].g_intPackageFailureTotal = 0;
                        m_smVSInfo[i].g_intPadFailureTotal = 0;
                        m_smVSInfo[i].g_intBarcodeFailureTotal = 0;
                        m_smVSInfo[i].g_intSealFailureTotal = 0;
                        m_smVSInfo[i].g_intSealDistanceFailureTotal = 0;
                        m_smVSInfo[i].g_intSealOverHeatFailureTotal = 0;
                        m_smVSInfo[i].g_intSealBrokenAreaFailureTotal = 0;
                        m_smVSInfo[i].g_intSealBrokenGapFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleDiameterFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleBrokenFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleRoundnessFailureTotal = 0;
                        m_smVSInfo[i].g_intSealEdgeStraightnessFailureTotal = 0;
                        m_smVSInfo[i].g_intCheckPresenceFailureTotal = 0;
                        m_smVSInfo[i].g_intPositionFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadOffsetFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadAreaFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadBrokenFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadExcessFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadSmearFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadStandOffFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadSpanFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadContaminationFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadMissingFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadOffsetFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadAreaFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadPitchGapFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadBrokenFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadExcessFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadSmearFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadEdgeLimitFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadStandOffFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadEdgeDistanceFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadSpanFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadContaminationFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadMissingFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadColorDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePkgDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadOffsetFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadSkewFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadWidthFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadLengthFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadLengthVarianceFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPitchGapFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPitchVarianceFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadStandOffFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadStandOffVarianceFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadCoplanFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadAGVFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadSpanFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadSweepsFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadUnCutTiebarFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadMissingFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadContaminationFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPkgDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intNoTemplateFailureTotal = 0;
                        m_smVSInfo[i].g_intPkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intPkgColorDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intEdgeNotFoundFailureTotal = 0;
                        m_smVSInfo[i].g_intAngleFailureTotal = 0;
                        m_smVSInfo[i].g_intLowYieldPass = 0;
                        m_smVSInfo[i].g_intLowYieldUnitCount = 0;
                        m_smVSInfo[i].g_intContinuousPassUnitCount = 0;
                        m_smVSInfo[i].g_intContinuousFailUnitCount = 0;
                        m_smVSInfo[i].g_intTestedTotal = 0;
                        m_smVSInfo[i].g_intPassImageCount = 0;
                        m_smVSInfo[i].g_intFailImageCount = 0;
                        m_smVSInfo[i].g_intTotalImageCount = 0;
                        m_smVSInfo[i].AT_VM_UpdateProduction = true;
                        m_smVSInfo[i].AT_VM_UpdateProductionDisplayAll = true;

                        // 2019 08 16 - JBTAN: not use
                        //m_smVSInfo[i].g_intLotlyPassTotal = 0;
                        //m_smVSInfo[i].g_intLotlyTestedTotal = 0;
                        //m_smVSInfo[i].g_intLotlyMarkFailureTotal = 0;
                        //m_smVSInfo[i].g_intLotlyOrientFailureTotal = 0;
                        //m_smVSInfo[i].g_intLotlyLeadFailureTotal = 0;
                        //m_smVSInfo[i].g_intLotlyPackageFailureTotal = 0;
                        //m_smVSInfo[i].g_intLotlyPadFailureTotal = 0;
                        //m_smVSInfo[i].g_intLotlySealFailureTotal = 0;
                        //m_smVSInfo[i].g_intLotlyCheckPresenceFailureTotal = 0;
                        m_smVSInfo[i].AT_VP_NewLot = true;

                        // 2020 03 27 - JBTAN: Save Vision Reset Count info
                        m_smVSInfo[i].g_intVisionResetCount = 0;
                        subKey1.SetValue("VS" + (i + 1) + "ResetCount", m_smVSInfo[i].g_intVisionResetCount);
                        subKey1.SetValue("VS" + (i + 1) + "ResetCountTime", m_smProductionInfo.g_strLotStartTime);
                    }
                }

                m_smProductionInfo.VM_TH_UpdateCount = true;
                UpdateNewLotButtonGUI();

                //

                //RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                //RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                //string strRecipePrev = subkey1.GetValue("RecipeIDPrev", "Default").ToString();
                //string strLotPrev = subkey1.GetValue("LotNoPrev", "SRM").ToString();

                //STDeviceEdit.SaveDeviceEditLog("New Lot", "Create New Lot", strLotPrev, m_smProductionInfo.g_strLotID);
                //if (strRecipePrev != m_smProductionInfo.g_strRecipeID)
                //    STDeviceEdit.SaveDeviceEditLog("", "Change Recipe", strRecipePrev, m_smProductionInfo.g_strRecipeID);

                m_smProductionInfo.g_blnNewLotPass = true;
            }
            catch (Exception ex)
            {
                m_smProductionInfo.g_blnNewLotPass = false;
                m_objTrack.WriteLine("AutoForm (COMCreateNewLot) : " + ex.ToString());
                SRMMessageBox.Show("Fail to Create New Lot Due to " + ex.ToString(), "SRM Vision");
            }
            finally
            {
                m_smProductionInfo.AT_CO_NewLotDone = true;
            }
        }

        /// <summary>
        /// Create new lot with end lot feature
        /// </summary>
        private void ProcessNewLot()
        {
            m_smProductionInfo.g_blnEndLotStatus = false;
            //Save end lot status to registry
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey1 = key.CreateSubKey("SVG\\AutoMode");
            subKey1.SetValue("EndLotStatus", m_smProductionInfo.g_blnEndLotStatus);
            //SaveLotFile();
            //SaveReportToServer();
            CreateProductionImageDirectory();

            if (m_intSelectedVisionStation <= 0)
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                {
                    if (m_blnUseServerRecipePrev)
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Server)";
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Local)";
                }
                else
                    lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID;
            }
            else
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                {
                    if (m_blnUseServerRecipePrev)
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Server)";
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Local)";
                }
                else
                    lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1];
            }

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (m_smVSInfo[i].g_blnWantClearMarkTemplateWhenNewLot)
                {
                    m_objVisionPage[i].DeleteMarkTemplateWhenNewLot();
                }

                if (m_smVSInfo[i].g_blnWantClearSealTemplateWhenNewLot)
                {
                    m_objVisionPage[i].DeleteSealTemplateWhenNewLot();
                }

                if (m_smVSInfo[i].g_blnWantLoadRefTolWhenNewLot)
                {
                    if (m_smVSInfo[i].g_strBrowsePath == "")
                    {
                        SRMMessageBox.Show("Load Rolerance Reference File fail. Please set Tolerance Reference file directory first.");
                    }
                    else if (!File.Exists(m_smVSInfo[i].g_strBrowsePath + "\\" + m_smProductionInfo.g_strRecipeID + ".stol"))
                    {
                        SRMMessageBox.Show("Load Rolerance Reference File fail. Tolerance Reference file no exist.");
                    }
                    else
                        m_objVisionPage[i].LoadToleranceReference(m_smVSInfo[i].g_strBrowsePath + "\\" + m_smProductionInfo.g_strRecipeID + ".stol");
                }

                //If want CPK, reset CPK counter when new lot
                if (m_smVSInfo[i].g_blnCPKON)
                {
                    //Re-init CPK
                    m_smVSInfo[i].g_blnReInitCPK = false;
                }

                m_smVSInfo[i].g_intPassTotal = 0;
                m_smVSInfo[i].g_intMarkFailureTotal = 0;
                m_smVSInfo[i].g_intOrientFailureTotal = 0;
                m_smVSInfo[i].g_intPin1FailureTotal = 0;
                m_smVSInfo[i].g_intLeadFailureTotal = 0;
                m_smVSInfo[i].g_intPackageFailureTotal = 0;
                m_smVSInfo[i].g_intPadFailureTotal = 0;
                m_smVSInfo[i].g_intBarcodeFailureTotal = 0;
                m_smVSInfo[i].g_intSealFailureTotal = 0;
                m_smVSInfo[i].g_intSealDistanceFailureTotal = 0;
                m_smVSInfo[i].g_intSealOverHeatFailureTotal = 0;
                m_smVSInfo[i].g_intSealBrokenAreaFailureTotal = 0;
                m_smVSInfo[i].g_intSealBrokenGapFailureTotal = 0;
                m_smVSInfo[i].g_intSealSprocketHoleFailureTotal = 0;
                m_smVSInfo[i].g_intSealSprocketHoleDiameterFailureTotal = 0;
                m_smVSInfo[i].g_intSealSprocketHoleDefectFailureTotal = 0;
                m_smVSInfo[i].g_intSealSprocketHoleBrokenFailureTotal = 0;
                m_smVSInfo[i].g_intSealSprocketHoleRoundnessFailureTotal = 0;
                m_smVSInfo[i].g_intSealEdgeStraightnessFailureTotal = 0;
                m_smVSInfo[i].g_intCheckPresenceFailureTotal = 0;
                m_smVSInfo[i].g_intPositionFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadOffsetFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadAreaFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadDimensionFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadBrokenFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadExcessFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadSmearFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadStandOffFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadSpanFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadContaminationFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadMissingFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadOffsetFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadAreaFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadDimensionFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadPitchGapFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadBrokenFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadExcessFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadSmearFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadEdgeLimitFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadStandOffFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadEdgeDistanceFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadSpanFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadContaminationFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadMissingFailureTotal = 0;
                m_smVSInfo[i].g_intSidePadColorDefectFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPkgDefectFailureTotal = 0;
                m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal = 0;
                m_smVSInfo[i].g_intSidePkgDefectFailureTotal = 0;
                m_smVSInfo[i].g_intSidePkgDimensionFailureTotal = 0;
                m_smVSInfo[i].g_intLeadOffsetFailureTotal = 0;
                m_smVSInfo[i].g_intLeadSkewFailureTotal = 0;
                m_smVSInfo[i].g_intLeadWidthFailureTotal = 0;
                m_smVSInfo[i].g_intLeadLengthFailureTotal = 0;
                m_smVSInfo[i].g_intLeadLengthVarianceFailureTotal = 0;
                m_smVSInfo[i].g_intLeadPitchGapFailureTotal = 0;
                m_smVSInfo[i].g_intLeadPitchVarianceFailureTotal = 0;
                m_smVSInfo[i].g_intLeadStandOffFailureTotal = 0;
                m_smVSInfo[i].g_intLeadStandOffVarianceFailureTotal = 0;
                m_smVSInfo[i].g_intLeadCoplanFailureTotal = 0;
                m_smVSInfo[i].g_intLeadAGVFailureTotal = 0;
                m_smVSInfo[i].g_intLeadSpanFailureTotal = 0;
                m_smVSInfo[i].g_intLeadSweepsFailureTotal = 0;
                m_smVSInfo[i].g_intLeadUnCutTiebarFailureTotal = 0;
                m_smVSInfo[i].g_intLeadMissingFailureTotal = 0;
                m_smVSInfo[i].g_intLeadContaminationFailureTotal = 0;
                m_smVSInfo[i].g_intLeadPkgDefectFailureTotal = 0;
                m_smVSInfo[i].g_intLeadPkgDimensionFailureTotal = 0;
                m_smVSInfo[i].g_intNoTemplateFailureTotal = 0;
                m_smVSInfo[i].g_intPkgDefectFailureTotal = 0;
                m_smVSInfo[i].g_intPkgColorDefectFailureTotal = 0;
                m_smVSInfo[i].g_intEdgeNotFoundFailureTotal = 0;
                m_smVSInfo[i].g_intAngleFailureTotal = 0;
                m_smVSInfo[i].g_intLowYieldUnitCount = 0;
                m_smVSInfo[i].g_intContinuousPassUnitCount = 0;
                m_smVSInfo[i].g_intContinuousFailUnitCount = 0;
                m_smVSInfo[i].g_intTestedTotal = 0;
                m_smVSInfo[i].g_intPassImageCount = 0;
                m_smVSInfo[i].g_intFailImageCount = 0;
                m_smVSInfo[i].g_intTotalImageCount = 0;
                m_smVSInfo[i].AT_VP_NewLot = true;
                m_smVSInfo[i].AT_VM_UpdateProductionDisplayAll = true;
                m_smVSInfo[i].g_blnWantClearSaveImageInfo = true; // Clear all save pass fail image information before new lot, to prevent delete wrong image and over save image

                // 2020 03 27 - JBTAN: Save Vision Reset Count info
                m_smVSInfo[i].g_intVisionResetCount = 0;
                subKey1.SetValue("VS" + (i + 1) + "ResetCount", m_smVSInfo[i].g_intVisionResetCount);
                subKey1.SetValue("VS" + (i + 1) + "ResetCountTime", m_smProductionInfo.g_strLotStartTime);
            }

            m_smProductionInfo.VM_TH_UpdateCount = true;
            UpdateNewLotButtonGUI();
        }

        /// <summary>
        /// Create new lot with end lot on particular vision use by TCPIP or RS232
        /// </summary>
        private void ProcessNewLot(int intVision)
        {
            try
            {
                m_smProductionInfo.g_blnEndLotStatus = false;
                //Save end lot status to registry
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subKey1 = key.CreateSubKey("SVG\\AutoMode");
                subKey1.SetValue("EndLotStatus", m_smProductionInfo.g_blnEndLotStatus);
                //SaveLotFile();
                //SaveReportToServer();
                CreateProductionImageDirectory();

                if (m_intSelectedVisionStation <= 0)
                {
                    if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    {
                        if (m_blnUseServerRecipePrev)
                            lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Server)";
                        else
                            lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Local)";
                    }
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID;
                }
                else
                {
                    if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    {
                        if (m_blnUseServerRecipePrev)
                            lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Server)";
                        else
                            lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Local)";
                    }
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1];
                }

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    //New lot on particular vision or all vision
                    if ((intVision & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) > 0 || intVision == 0 || m_smProductionInfo.g_arrVisionIDIndex[i] == 0)
                    {
                        if (m_smVSInfo[i].g_blnWantClearMarkTemplateWhenNewLot)
                        {
                            m_objVisionPage[i].DeleteMarkTemplateWhenNewLot();
                        }

                        if (m_smVSInfo[i].g_blnWantClearSealTemplateWhenNewLot)
                        {
                            m_objVisionPage[i].DeleteSealTemplateWhenNewLot();
                        }

                        if (m_smVSInfo[i].g_blnWantLoadRefTolWhenNewLot)
                        {
                            if (m_smVSInfo[i].g_strBrowsePath == "")
                            {
                                SRMMessageBox.Show("Load Rolerance Reference File fail. Please set Tolerance Reference file directory first.");
                            }
                            else if (!File.Exists(m_smVSInfo[i].g_strBrowsePath + "\\" + m_smProductionInfo.g_strRecipeID + ".stol"))
                            {
                                SRMMessageBox.Show("Load Rolerance Reference File fail. Tolerance Reference file no exist.");
                            }
                            else
                                m_objVisionPage[i].LoadToleranceReference(m_smVSInfo[i].g_strBrowsePath + "\\" + m_smProductionInfo.g_strRecipeID + ".stol");
                        }

                        //If want CPK, reset CPK counter when new lot
                        if (m_smVSInfo[i].g_blnCPKON)
                        {
                            //Re-init CPK
                            m_smVSInfo[i].g_blnReInitCPK = false;
                        }

                        m_smVSInfo[i].g_intPassTotal = 0;
                        m_smVSInfo[i].g_intMarkFailureTotal = 0;
                        m_smVSInfo[i].g_intOrientFailureTotal = 0;
                        m_smVSInfo[i].g_intPin1FailureTotal = 0;
                        m_smVSInfo[i].g_intLeadFailureTotal = 0;
                        m_smVSInfo[i].g_intPackageFailureTotal = 0;
                        m_smVSInfo[i].g_intPadFailureTotal = 0;
                        m_smVSInfo[i].g_intBarcodeFailureTotal = 0;
                        m_smVSInfo[i].g_intSealFailureTotal = 0;
                        m_smVSInfo[i].g_intSealDistanceFailureTotal = 0;
                        m_smVSInfo[i].g_intSealOverHeatFailureTotal = 0;
                        m_smVSInfo[i].g_intSealBrokenAreaFailureTotal = 0;
                        m_smVSInfo[i].g_intSealBrokenGapFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleDiameterFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleBrokenFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleRoundnessFailureTotal = 0;
                        m_smVSInfo[i].g_intSealEdgeStraightnessFailureTotal = 0;
                        m_smVSInfo[i].g_intCheckPresenceFailureTotal = 0;
                        m_smVSInfo[i].g_intPositionFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadOffsetFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadAreaFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadBrokenFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadExcessFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadSmearFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadStandOffFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadSpanFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadContaminationFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadMissingFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadOffsetFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadAreaFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadPitchGapFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadBrokenFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadExcessFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadSmearFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadEdgeLimitFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadStandOffFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadEdgeDistanceFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadSpanFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadContaminationFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadMissingFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadColorDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePkgDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadOffsetFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadSkewFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadWidthFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadLengthFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadLengthVarianceFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPitchGapFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPitchVarianceFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadStandOffFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadStandOffVarianceFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadCoplanFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadAGVFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadSpanFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadSweepsFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadUnCutTiebarFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadMissingFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadContaminationFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPkgDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intNoTemplateFailureTotal = 0;
                        m_smVSInfo[i].g_intPkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intPkgColorDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intEdgeNotFoundFailureTotal = 0;
                        m_smVSInfo[i].g_intAngleFailureTotal = 0;
                        m_smVSInfo[i].g_intLowYieldUnitCount = 0;
                        m_smVSInfo[i].g_intContinuousPassUnitCount = 0;
                        m_smVSInfo[i].g_intContinuousFailUnitCount = 0;
                        m_smVSInfo[i].g_intTestedTotal = 0;
                        m_smVSInfo[i].g_intPassImageCount = 0;
                        m_smVSInfo[i].g_intFailImageCount = 0;
                        m_smVSInfo[i].g_intTotalImageCount = 0;
                        m_smVSInfo[i].AT_VP_NewLot = true;
                        m_smVSInfo[i].AT_VM_UpdateProductionDisplayAll = true;
                        m_smVSInfo[i].g_blnWantClearSaveImageInfo = true; // Clear all save pass fail image information before new lot, to prevent delete wrong image and over save image

                        // 2020 03 27 - JBTAN: Save Vision Reset Count info
                        m_smVSInfo[i].g_intVisionResetCount = 0;
                        subKey1.SetValue("VS" + (i + 1) + "ResetCount", m_smVSInfo[i].g_intVisionResetCount);
                        subKey1.SetValue("VS" + (i + 1) + "ResetCountTime", m_smProductionInfo.g_strLotStartTime);
                    }
                }
                m_smProductionInfo.VM_TH_UpdateCount = true;
                UpdateNewLotButtonGUI();

                m_smProductionInfo.g_blnNewLotPass = true;
            }
            catch (Exception ex)
            {
                m_smProductionInfo.g_blnNewLotPass = false;
                m_objTrack.WriteLine("AutoForm (COMCreateNewLot) : " + ex.ToString());
                SRMMessageBox.Show("Fail to Create New Lot Due to " + ex.ToString(), "SRM Vision");
            }
            finally
            {
                m_smProductionInfo.AT_CO_NewLotDone = true;
            }
        }

        /// <summary>
        /// End lot feature
        /// </summary>
        private void ProcessEndLot()
        {
            m_smProductionInfo.g_blnEndLotStatus = true;
            //Save end lot status to registry
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey1 = key.CreateSubKey("SVG\\AutoMode");
            subKey1.SetValue("EndLotStatus", m_smProductionInfo.g_blnEndLotStatus);
            subKey1.SetValue("LotNoPrev", m_smProductionInfo.g_strLotID);
            subKey1.SetValue("OperatorIDPrev", m_smProductionInfo.g_strOperatorID);
            subKey1.SetValue("RecipeIDPrev", m_smProductionInfo.g_strRecipeID);
            subKey1.SetValue("LotStartTimePrev", m_smProductionInfo.g_strLotStartTime);

            //for (int i = 0; i < 10; i++)
            //{
            //    subKey1.SetValue("SingleRecipeIDPrev" + i.ToString(), m_smProductionInfo.g_arrSingleRecipeID[i]);
            //}

            string strLotEndTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            // 2019 08 20 - JBTAN: set to lot start time because when save lot file use "LotStartTime" as the previous lot end time
            // will not affect next lot time because it will be set again when user click new lot
            subKey1.SetValue("LotStartTime", strLotEndTime);

            SaveLotFile();
            SaveReportToServer();
            SaveReportToHandler();

            if (m_blnWantSaveAnotherLot)
            {
                m_blnWantSaveAnotherLot = false;
                SaveLotFile(m_intVisionInvolved);
                SaveReportToServer();
                SaveReportToHandler();
                m_intVisionInvolved = 0;
            }

            //CreateProductionImageDirectory();
            //if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
            //{
            //    if (m_blnUseServerRecipePrev)
            //        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Server)";
            //    else
            //        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Local)";
            //}
            //else
            //    lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID;

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                //if (m_smVSInfo[i].g_blnWantClearMarkTemplateWhenNewLot)
                //{
                //    m_objVisionPage[i].DeleteMarkTemplateWhenNewLot();
                //}

                //If want CPK, reset CPK counter when end lot to create CPK report if CPK report is incomplete
                if (m_smVSInfo[i].g_blnCPKON)
                {
                    //Re-init CPK
                    m_smVSInfo[i].g_blnReInitCPK = false;
                }

                //m_smVSInfo[i].g_intPassTotal = 0;
                //m_smVSInfo[i].g_intMarkFailureTotal = 0;
                //m_smVSInfo[i].g_intOrientFailureTotal = 0;
                //m_smVSInfo[i].g_intPin1FailureTotal = 0;
                //m_smVSInfo[i].g_intLeadFailureTotal = 0;
                //m_smVSInfo[i].g_intPackageFailureTotal = 0;
                //m_smVSInfo[i].g_intPadFailureTotal = 0;
                //m_smVSInfo[i].g_intSealFailureTotal = 0;
                //m_smVSInfo[i].g_intCheckPresenceFailureTotal = 0;
                //m_smVSInfo[i].g_intPositionFailureTotal = 0;
                //m_smVSInfo[i].g_intLowYieldUnitCount = 0;
                //m_smVSInfo[i].g_intTestedTotal = 0;
                //m_smVSInfo[i].g_intPassImageCount = 0;
                //m_smVSInfo[i].g_intFailImageCount = 0;
                //m_smVSInfo[i].AT_VP_NewLot = true;
                //m_smVSInfo[i].AT_VM_UpdateProductionDisplayAll = true;
            }

            //m_smProductionInfo.VM_TH_UpdateCount = true;
            UpdateNewLotButtonGUI();
        }

        /// <summary>
        /// End lot on particular vision use by TCPIP or RS232 (currently handler side will only send End lot for all vision)
        /// </summary>
        /// <param name="intVision">vision no</param>
        private void ProcessEndLot(int intVision)
        {
            try
            {
                m_smProductionInfo.g_blnEndLotStatus = true;
                //Save end lot status to registry
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subKey1 = key.CreateSubKey("SVG\\AutoMode");
                subKey1.SetValue("EndLotStatus", m_smProductionInfo.g_blnEndLotStatus);
                subKey1.SetValue("LotNoPrev", m_smProductionInfo.g_strLotID);
                subKey1.SetValue("OperatorIDPrev", m_smProductionInfo.g_strOperatorID);
                subKey1.SetValue("RecipeIDPrev", m_smProductionInfo.g_strRecipeID);
                subKey1.SetValue("LotStartTimePrev", m_smProductionInfo.g_strLotStartTime);

                //for (int i = 0; i < 10; i++)
                //{
                //    subKey1.SetValue("SingleRecipeIDPrev" + i.ToString(), m_smProductionInfo.g_arrSingleRecipeID[i]);
                //}

                string strLotEndTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                // 2019 08 20 - JBTAN: set to lot start time because when save lot file use "LotStartTime" as the previous lot end time
                // will not affect next lot time because it will be set again when user click new lot
                subKey1.SetValue("LotStartTime", strLotEndTime);

                SaveLotFile();
                SaveReportToServer();
                SaveReportToHandler();

                if (m_blnWantSaveAnotherLot)
                {
                    m_blnWantSaveAnotherLot = false;
                    SaveLotFile(m_intVisionInvolved);
                    SaveReportToServer();
                    SaveReportToHandler();
                    m_intVisionInvolved = 0;
                }

                //CreateProductionImageDirectory();
                //if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                //{
                //    if (m_blnUseServerRecipePrev)
                //        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Server)";
                //    else
                //        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Local)";
                //}
                //else
                //    lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID;

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    //End lot on particular vision or all vision
                    if ((intVision & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) > 0 || intVision == 0 || m_smProductionInfo.g_arrVisionIDIndex[i] == 0)
                    {

                        //If want CPK, reset CPK counter when end lot to create CPK report if CPK report is incomplete
                        if (m_smVSInfo[i].g_blnCPKON)
                        {
                            //Re-init CPK
                            m_smVSInfo[i].g_blnReInitCPK = false;
                        }

                        //m_smVSInfo[i].g_intPassTotal = 0;
                        //m_smVSInfo[i].g_intMarkFailureTotal = 0;
                        //m_smVSInfo[i].g_intOrientFailureTotal = 0;
                        //m_smVSInfo[i].g_intPin1FailureTotal = 0;
                        //m_smVSInfo[i].g_intLeadFailureTotal = 0;
                        //m_smVSInfo[i].g_intPackageFailureTotal = 0;
                        //m_smVSInfo[i].g_intPadFailureTotal = 0;
                        //m_smVSInfo[i].g_intSealFailureTotal = 0;
                        //m_smVSInfo[i].g_intCheckPresenceFailureTotal = 0;
                        //m_smVSInfo[i].g_intPositionFailureTotal = 0;
                        //m_smVSInfo[i].g_intLowYieldUnitCount = 0;
                        //m_smVSInfo[i].g_intTestedTotal = 0;
                        //m_smVSInfo[i].g_intPassImageCount = 0;
                        //m_smVSInfo[i].g_intFailImageCount = 0;
                        //m_smVSInfo[i].AT_VP_NewLot = true;
                        //m_smVSInfo[i].AT_VM_UpdateProductionDisplayAll = true;
                    }
                }

                //m_smProductionInfo.VM_TH_UpdateCount = true;
                m_smProductionInfo.g_blnEndLotPass = true;
                UpdateNewLotButtonGUI();
            }
            catch (Exception ex)
            {
                m_smProductionInfo.g_blnEndLotPass = false;
                m_objTrack.WriteLine("AutoForm (COMCreateEndLot) : " + ex.ToString());
                SRMMessageBox.Show("Fail to End Lot Due to " + ex.ToString(), "SRM Vision");
            }
            finally
            {
                m_smProductionInfo.AT_CO_EndLotDone = true;
            }
        }

        /// <summary>
        /// Create new lot without end lot on particular vision use by TCPIP or RS232
        /// </summary>
        /// <param name="intVision">vision no</param>
        private void CreateRerunLot(int intVision)
        {
            try
            {
                m_smProductionInfo.g_blnEndLotStatus = false;
                //Save end lot status to registry
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subKey1 = key.CreateSubKey("SVG\\AutoMode");
                subKey1.SetValue("EndLotStatus", m_smProductionInfo.g_blnEndLotStatus);
                SaveLotFile();
                SaveReportToServer();
                SaveReportToHandler();
                CreateProductionImageDirectory();

                if (m_intSelectedVisionStation <= 0)
                {
                    if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    {
                        if (m_blnUseServerRecipePrev)
                            lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Server)";
                        else
                            lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Local)";
                    }
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID;
                }
                else
                {
                    if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    {
                        if (m_blnUseServerRecipePrev)
                            lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Server)";
                        else
                            lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Local)";
                    }
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1];
                }

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    //New lot on particular vision or all vision
                    if ((intVision & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) > 0 || intVision == 0 || m_smProductionInfo.g_arrVisionIDIndex[i] == 0)
                    {
                        m_smVSInfo[i].g_intPassTotal = m_smVSInfo[i].PR_CO_PassQuantity;
                        m_smVSInfo[i].g_intMarkFailureTotal = 0;
                        m_smVSInfo[i].g_intOrientFailureTotal = 0;
                        m_smVSInfo[i].g_intPin1FailureTotal = 0;
                        m_smVSInfo[i].g_intLeadFailureTotal = 0;
                        m_smVSInfo[i].g_intPackageFailureTotal = 0;
                        m_smVSInfo[i].g_intPadFailureTotal = 0;
                        m_smVSInfo[i].g_intBarcodeFailureTotal = 0;
                        m_smVSInfo[i].g_intSealFailureTotal = 0;
                        m_smVSInfo[i].g_intSealDistanceFailureTotal = 0;
                        m_smVSInfo[i].g_intSealOverHeatFailureTotal = 0;
                        m_smVSInfo[i].g_intSealBrokenAreaFailureTotal = 0;
                        m_smVSInfo[i].g_intSealBrokenGapFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleDiameterFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleBrokenFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleRoundnessFailureTotal = 0;
                        m_smVSInfo[i].g_intSealEdgeStraightnessFailureTotal = 0;
                        m_smVSInfo[i].g_intCheckPresenceFailureTotal = 0;
                        m_smVSInfo[i].g_intPositionFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadOffsetFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadAreaFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadBrokenFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadExcessFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadSmearFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadStandOffFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadSpanFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadContaminationFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadMissingFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadOffsetFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadAreaFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadPitchGapFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadBrokenFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadExcessFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadSmearFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadEdgeLimitFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadStandOffFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadEdgeDistanceFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadSpanFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadContaminationFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadMissingFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadColorDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePkgDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadOffsetFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadSkewFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadWidthFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadLengthFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadLengthVarianceFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPitchGapFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPitchVarianceFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadStandOffFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadStandOffVarianceFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadCoplanFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadAGVFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadSpanFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadSweepsFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadUnCutTiebarFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadMissingFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadContaminationFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPkgDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intNoTemplateFailureTotal = 0;
                        m_smVSInfo[i].g_intPkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intPkgColorDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intEdgeNotFoundFailureTotal = 0;
                        m_smVSInfo[i].g_intAngleFailureTotal = 0;
                        m_smVSInfo[i].g_intLowYieldPass = 0;
                        m_smVSInfo[i].g_intLowYieldUnitCount = 0;
                        m_smVSInfo[i].g_intContinuousPassUnitCount = 0;
                        m_smVSInfo[i].g_intContinuousFailUnitCount = 0;
                        m_smVSInfo[i].g_intTestedTotal = m_smVSInfo[i].PR_CO_PassQuantity;
                        m_smVSInfo[i].g_intPassImageCount = 0;
                        m_smVSInfo[i].g_intFailImageCount = 0;
                        m_smVSInfo[i].g_intTotalImageCount = 0;
                        m_smVSInfo[i].AT_VM_UpdateProduction = true;
                        m_smVSInfo[i].AT_VM_UpdateProductionDisplayAll = true;

                        // 2019 08 16 - JBTAN: not use
                        //m_smVSInfo[i].g_intLotlyPassTotal = 0;
                        //m_smVSInfo[i].g_intLotlyTestedTotal = 0;
                        //m_smVSInfo[i].g_intLotlyMarkFailureTotal = 0;
                        //m_smVSInfo[i].g_intLotlyOrientFailureTotal = 0;
                        //m_smVSInfo[i].g_intLotlyLeadFailureTotal = 0;
                        //m_smVSInfo[i].g_intLotlyPackageFailureTotal = 0;
                        //m_smVSInfo[i].g_intLotlyPadFailureTotal = 0;
                        //m_smVSInfo[i].g_intLotlySealFailureTotal = 0;
                        //m_smVSInfo[i].g_intLotlyCheckPresenceFailureTotal = 0;
                        m_smVSInfo[i].AT_VP_NewLot = true;
                    }
                }

                m_smProductionInfo.VM_TH_UpdateCount = true;
                UpdateNewLotButtonGUI();

                //

                //RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                //RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                //string strRecipePrev = subkey1.GetValue("RecipeIDPrev", "Default").ToString();
                //string strLotPrev = subkey1.GetValue("LotNoPrev", "SRM").ToString();

                //STDeviceEdit.SaveDeviceEditLog("New Lot", "Create New Lot", strLotPrev, m_smProductionInfo.g_strLotID);
                //if (strRecipePrev != m_smProductionInfo.g_strRecipeID)
                //    STDeviceEdit.SaveDeviceEditLog("", "Change Recipe", strRecipePrev, m_smProductionInfo.g_strRecipeID);

                m_smProductionInfo.g_blnRerunLotPass = true;
            }
            catch (Exception ex)
            {
                m_smProductionInfo.g_blnRerunLotPass = false;
                m_objTrack.WriteLine("AutoForm (COMCreateRerunLot) : " + ex.ToString());
                SRMMessageBox.Show("Fail to Create New Lot Due to " + ex.ToString(), "SRM Vision");
            }
            finally
            {
                m_smProductionInfo.AT_CO_RerunLotDone = true;
            }
        }

        private void ProcessRerunLot(int intVision)
        {
            try
            {
                m_smProductionInfo.g_blnEndLotStatus = false;
                //Save end lot status to registry
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subKey1 = key.CreateSubKey("SVG\\AutoMode");
                subKey1.SetValue("EndLotStatus", m_smProductionInfo.g_blnEndLotStatus);
                //SaveLotFile();
                //SaveReportToServer();
                CreateProductionImageDirectory();

                if (m_intSelectedVisionStation <= 0)
                {
                    if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    {
                        if (m_blnUseServerRecipePrev)
                            lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Server)";
                        else
                            lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Local)";
                    }
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID;
                }
                else
                {
                    if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    {
                        if (m_blnUseServerRecipePrev)
                            lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Server)";
                        else
                            lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Local)";
                    }
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1];
                }

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    //New lot on particular vision or all vision
                    if ((intVision & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) > 0 || intVision == 0 || m_smProductionInfo.g_arrVisionIDIndex[i] == 0)
                    {
                        m_smVSInfo[i].g_intPassTotal = m_smVSInfo[i].PR_CO_PassQuantity;
                        m_smVSInfo[i].g_intMarkFailureTotal = 0;
                        m_smVSInfo[i].g_intOrientFailureTotal = 0;
                        m_smVSInfo[i].g_intPin1FailureTotal = 0;
                        m_smVSInfo[i].g_intLeadFailureTotal = 0;
                        m_smVSInfo[i].g_intPackageFailureTotal = 0;
                        m_smVSInfo[i].g_intPadFailureTotal = 0;
                        m_smVSInfo[i].g_intBarcodeFailureTotal = 0;
                        m_smVSInfo[i].g_intSealFailureTotal = 0;
                        m_smVSInfo[i].g_intSealDistanceFailureTotal = 0;
                        m_smVSInfo[i].g_intSealOverHeatFailureTotal = 0;
                        m_smVSInfo[i].g_intSealBrokenAreaFailureTotal = 0;
                        m_smVSInfo[i].g_intSealBrokenGapFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleDiameterFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleBrokenFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleRoundnessFailureTotal = 0;
                        m_smVSInfo[i].g_intSealEdgeStraightnessFailureTotal = 0;
                        m_smVSInfo[i].g_intCheckPresenceFailureTotal = 0;
                        m_smVSInfo[i].g_intPositionFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadOffsetFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadAreaFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadBrokenFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadExcessFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadSmearFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadStandOffFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadSpanFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadContaminationFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadMissingFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadOffsetFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadAreaFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadPitchGapFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadBrokenFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadExcessFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadSmearFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadEdgeLimitFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadStandOffFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadEdgeDistanceFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadSpanFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadContaminationFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadMissingFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadColorDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePkgDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadOffsetFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadSkewFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadWidthFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadLengthFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadLengthVarianceFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPitchGapFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPitchVarianceFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadStandOffFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadStandOffVarianceFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadCoplanFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadAGVFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadSpanFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadSweepsFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadUnCutTiebarFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadMissingFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadContaminationFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPkgDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intNoTemplateFailureTotal = 0;
                        m_smVSInfo[i].g_intPkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intPkgColorDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intEdgeNotFoundFailureTotal = 0;
                        m_smVSInfo[i].g_intAngleFailureTotal = 0;
                        m_smVSInfo[i].g_intLowYieldUnitCount = 0;
                        m_smVSInfo[i].g_intContinuousPassUnitCount = 0;
                        m_smVSInfo[i].g_intContinuousFailUnitCount = 0;
                        m_smVSInfo[i].g_intTestedTotal = m_smVSInfo[i].PR_CO_PassQuantity;
                        m_smVSInfo[i].g_intPassImageCount = 0;
                        m_smVSInfo[i].g_intFailImageCount = 0;
                        m_smVSInfo[i].g_intTotalImageCount = 0;
                        m_smVSInfo[i].AT_VP_NewLot = true;
                        m_smVSInfo[i].AT_VM_UpdateProductionDisplayAll = true;
                    }
                }
                m_smProductionInfo.VM_TH_UpdateCount = true;
                UpdateNewLotButtonGUI();

                m_smProductionInfo.g_blnRerunLotPass = true;
            }
            catch (Exception ex)
            {
                m_smProductionInfo.g_blnRerunLotPass = false;
                m_objTrack.WriteLine("AutoForm (COMCreateRerunLot) : " + ex.ToString());
                SRMMessageBox.Show("Fail to Create New Lot Due to " + ex.ToString(), "SRM Vision");
            }
            finally
            {
                m_smProductionInfo.AT_CO_RerunLotDone = true;
            }
        }

        /// <summary>
        /// Create reject image and vision station directory
        /// </summary>
        private void CreateProductionImageDirectory()
        {
            //string strPath = m_smCustomizeInfo.g_strSaveImageLocation + m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime;
            //if (!Directory.Exists(strPath))
            //    Directory.CreateDirectory(strPath);

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                string strPath = m_smVSInfo[i].g_strSaveImageLocation + m_smProductionInfo.g_arrSingleLotID[i] + "_" + m_smProductionInfo.g_strLotStartTime;
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                string strVisionImageFolderName = m_smVSInfo[i].g_strVisionFolderName + "(" + m_smVSInfo[i].g_strVisionDisplayName + " " + m_smVSInfo[i].g_strVisionNameRemark + ")" + "_" + m_smProductionInfo.g_strLotStartTime;

                if (!Directory.Exists(strPath + "\\" + strVisionImageFolderName))
                    Directory.CreateDirectory(strPath + "\\" + strVisionImageFolderName);
                if (!Directory.Exists(strPath + "\\" + strVisionImageFolderName + "\\Pass"))
                    Directory.CreateDirectory(strPath + "\\" + strVisionImageFolderName + "\\Pass");

                // 2019 09 06 - JBTAN: reject folder will be created later if test fail
                //switch (m_smVSInfo[i].g_strVisionName)
                //{
                //    case "Orient":
                //    case "BottomOrient":
                //    case "Mark":
                //    case "MarkOrient":
                //    case "MarkPkg":
                //    case "MOPkg":
                //    case "MOLiPkg":
                //    case "MOLi":
                //    case "Package":
                //        if ((m_smCustomizeInfo.g_intWantOrient & (0x01 << i)) > 0 && !Directory.Exists(strPath + "\\" + strVisionImageFolderName + "\\Orient"))
                //            Directory.CreateDirectory(strPath + "\\" + strVisionImageFolderName + "\\Orient");
                //        if ((m_smCustomizeInfo.g_intWantMark & (0x01 << i)) > 0 && !Directory.Exists(strPath + "\\" + strVisionImageFolderName + "\\Mark"))
                //            Directory.CreateDirectory(strPath + "\\" + strVisionImageFolderName + "\\Mark");
                //        break;
                //    case "UnitPresent":
                //    case "BottomPosition":
                //    case "BottomPositionOrient":
                //    case "TapePocketPosition":
                //        if (!Directory.Exists(strPath + "\\" + strVisionImageFolderName + "\\Fail"))
                //            Directory.CreateDirectory(strPath + "\\" + strVisionImageFolderName + "\\Fail");
                //        break;
                //    case "Pad":
                //    case "PadPos":
                //    case "PadPkg":
                //    case "PadPkgPos":
                //    case "Pad5S":
                //    case "Pad5SPos":
                //    case "Pad5SPkg":
                //    case "Pad5SPkgPos":
                //case "Li3D":
                //case "Li3DPkg":
                //        if (!Directory.Exists(strPath + "\\" + strVisionImageFolderName + "\\Fail"))
                //            Directory.CreateDirectory(strPath + "\\" + strVisionImageFolderName + "\\Fail");
                //        break;
                //    case "InPocket":
                //    case "InPocketPkg":
                //        if ((m_smCustomizeInfo.g_intWantOrient & (0x01 << i)) > 0 && !Directory.Exists(strPath + "\\" + strVisionImageFolderName + "\\Orient"))
                //            Directory.CreateDirectory(strPath + "\\" + strVisionImageFolderName + "\\Orient");
                //        if ((m_smCustomizeInfo.g_intWantMark & (0x01 << i)) > 0 && !Directory.Exists(strPath + "\\" + strVisionImageFolderName + "\\Mark"))
                //            Directory.CreateDirectory(strPath + "\\" + strVisionImageFolderName + "\\Mark");
                //        if (!Directory.Exists(strPath + "\\" + strVisionImageFolderName + "\\Position")) // 19-02-2019: ZJYEOH Create directory to save empty template
                //            Directory.CreateDirectory(strPath + "\\" + strVisionImageFolderName + "\\Position");
                //        break;
                //    case "InPocketPkgPos":
                //        if ((m_smCustomizeInfo.g_intWantOrient & (0x01 << i)) > 0 && !Directory.Exists(strPath + "\\" + strVisionImageFolderName + "\\Orient"))
                //            Directory.CreateDirectory(strPath + "\\" + strVisionImageFolderName + "\\Orient");
                //        if ((m_smCustomizeInfo.g_intWantMark & (0x01 << i)) > 0 && !Directory.Exists(strPath + "\\" + strVisionImageFolderName + "\\Mark"))
                //            Directory.CreateDirectory(strPath + "\\" + strVisionImageFolderName + "\\Mark");
                //        if ((m_smCustomizeInfo.g_intWantPositioning & (0x01 << i)) > 0 && !Directory.Exists(strPath + "\\" + strVisionImageFolderName + "\\Position"))
                //            Directory.CreateDirectory(strPath + "\\" + strVisionImageFolderName + "\\Position");
                //        break;
                //    //Post Seal
                //    case "Seal":
                //        if ((m_smCustomizeInfo.g_intWantSeal & (0x01 << i)) > 0 && !Directory.Exists(strPath + "\\" + strVisionImageFolderName + "\\Seal"))
                //            Directory.CreateDirectory(strPath + "\\" + strVisionImageFolderName + "\\Seal");
                //        break;
                //    default:
                //        SRMMessageBox.Show("CreateProductionImageDirectory() --> There is no such vision module name " + m_smVSInfo[i].g_strVisionName + " in this SRMVision software version.");
                //        break;
                //}
            }
        }

        /// <summary>
        /// Add selected vision system into navigation pane.
        /// </summary>
        private void CustomizeGUI()
        {
            int intVisionCount = 0;

            Pnl_TabPageButton.Controls.Add(btn_Display1Tab);

            // New
            if ((m_smCustomizeInfo.g_intVisionMask & 0x01) > 0)
            {
                btn_Vision1Tab.Text = m_smVSInfo[0].g_strVisionDisplayName + " " + m_smVSInfo[0].g_strVisionNameRemark;
                if (btn_Vision1Tab.Text.Length > 7)
                    btn_Vision1Tab.Width += (btn_Vision1Tab.Text.Length - 7) * 5;

                Pnl_TabPageButton.Controls.Add(btn_Vision1Tab);
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x02) > 0)
            {
                btn_Vision2Tab.Text = m_smVSInfo[1].g_strVisionDisplayName + " " + m_smVSInfo[1].g_strVisionNameRemark;
                if (btn_Vision2Tab.Text.Length > 7)
                    btn_Vision2Tab.Width += (btn_Vision2Tab.Text.Length - 7) * 5;

                Pnl_TabPageButton.Controls.Add(btn_Vision2Tab);
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x04) > 0)
            {
                btn_Vision3Tab.Text = m_smVSInfo[2].g_strVisionDisplayName + " " + m_smVSInfo[2].g_strVisionNameRemark;
                if (btn_Vision3Tab.Text.Length > 7)
                    btn_Vision3Tab.Width += (btn_Vision3Tab.Text.Length - 7) * 5;

                Pnl_TabPageButton.Controls.Add(btn_Vision3Tab);
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x08) > 0)
            {
                btn_Vision4Tab.Text = m_smVSInfo[3].g_strVisionDisplayName + " " + m_smVSInfo[3].g_strVisionNameRemark;
                if (btn_Vision4Tab.Text.Length > 7)
                    btn_Vision4Tab.Width += (btn_Vision4Tab.Text.Length - 7) * 5;

                Pnl_TabPageButton.Controls.Add(btn_Vision4Tab);
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x10) > 0)
            {
                btn_Vision5Tab.Text = m_smVSInfo[4].g_strVisionDisplayName + " " + m_smVSInfo[4].g_strVisionNameRemark;
                if (btn_Vision5Tab.Text.Length > 7)
                    btn_Vision5Tab.Width += (btn_Vision5Tab.Text.Length - 7) * 5;

                Pnl_TabPageButton.Controls.Add(btn_Vision5Tab);
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x20) > 0)
            {
                btn_Vision6Tab.Text = m_smVSInfo[5].g_strVisionDisplayName + " " + m_smVSInfo[5].g_strVisionNameRemark;
                if (btn_Vision6Tab.Text.Length > 7)
                    btn_Vision6Tab.Width += (btn_Vision6Tab.Text.Length - 7) * 5;

                Pnl_TabPageButton.Controls.Add(btn_Vision6Tab);
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x40) > 0)
            {
                btn_Vision7Tab.Text = m_smVSInfo[6].g_strVisionDisplayName + " " + m_smVSInfo[6].g_strVisionNameRemark;
                if (btn_Vision7Tab.Text.Length > 7)
                    btn_Vision7Tab.Width += (btn_Vision7Tab.Text.Length - 7) * 5;

                Pnl_TabPageButton.Controls.Add(btn_Vision7Tab);
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x80) > 0)
            {
                btn_Vision8Tab.Text = m_smVSInfo[7].g_strVisionDisplayName + " " + m_smVSInfo[7].g_strVisionNameRemark;
                if (btn_Vision8Tab.Text.Length > 7)
                    btn_Vision8Tab.Width += (btn_Vision8Tab.Text.Length - 7) * 5;

                Pnl_TabPageButton.Controls.Add(btn_Vision8Tab);
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x100) > 0)
            {
                btn_Vision9Tab.Text = m_smVSInfo[8].g_strVisionDisplayName + " " + m_smVSInfo[8].g_strVisionNameRemark;
                if (btn_Vision9Tab.Text.Length > 7)
                    btn_Vision9Tab.Width += (btn_Vision9Tab.Text.Length - 7) * 5;

                Pnl_TabPageButton.Controls.Add(btn_Vision9Tab);
                intVisionCount++;
            }

            if ((m_smCustomizeInfo.g_intVisionMask & 0x200) > 0)
            {
                btn_Vision10Tab.Text = m_smVSInfo[9].g_strVisionDisplayName + " " + m_smVSInfo[9].g_strVisionNameRemark;
                if (btn_Vision10Tab.Text.Length > 7)
                    btn_Vision10Tab.Width += (btn_Vision10Tab.Text.Length - 7) * 5;

                Pnl_TabPageButton.Controls.Add(btn_Vision10Tab);
                intVisionCount++;
            }

            m_intVisionCount = intVisionCount;

            Pnl_TabPageButton.Controls.Add(pnl_Header);

            //// Old
            //if ((m_smCustomizeInfo.g_intVisionMask & 0x200) > 0)
            //{
            //    //NavigationPane_Vision10.Text = m_smVSInfo[9].g_strVisionDisplayName + " " + m_smVSInfo[9].g_strVisionNameRemark;
            //    intVisionCount++;
            //}
            //else
            //    //NavigationPane_Vision10.Listed = false;

            //if ((m_smCustomizeInfo.g_intVisionMask & 0x100) > 0)
            //{
            //    //NavigationPane_Vision9.Text = m_smVSInfo[8].g_strVisionDisplayName + " " + m_smVSInfo[8].g_strVisionNameRemark;
            //    intVisionCount++;
            //}
            //else
            //    //NavigationPane_Vision9.Listed = false;

            //if ((m_smCustomizeInfo.g_intVisionMask & 0x80) > 0)
            //{
            //    //NavigationPane_Vision8.Text = m_smVSInfo[7].g_strVisionDisplayName + " " + m_smVSInfo[7].g_strVisionNameRemark;
            //    intVisionCount++;
            //}
            //else
            //    //NavigationPane_Vision8.Listed = false;

            //if ((m_smCustomizeInfo.g_intVisionMask & 0x40) > 0)
            //{
            //    //NavigationPane_Vision7.Text = m_smVSInfo[6].g_strVisionName + " " + m_smVSInfo[6].g_strVisionNameRemark;
            //    intVisionCount++;
            //}
            //else
            //    //NavigationPane_Vision7.Listed = false;

            //if ((m_smCustomizeInfo.g_intVisionMask & 0x20) > 0)
            //{
            //    //NavigationPane_Vision6.Text = m_smVSInfo[5].g_strVisionDisplayName + " " + m_smVSInfo[5].g_strVisionNameRemark;
            //    intVisionCount++;
            //}
            //else
            //    //NavigationPane_Vision6.Listed = false;

            //if ((m_smCustomizeInfo.g_intVisionMask & 0x10) > 0)
            //{
            //    //NavigationPane_Vision5.Text = m_smVSInfo[4].g_strVisionDisplayName + " " + m_smVSInfo[4].g_strVisionNameRemark;
            //    intVisionCount++;
            //}
            //else
            //    //NavigationPane_Vision5.Listed = false;

            //if ((m_smCustomizeInfo.g_intVisionMask & 0x08) > 0)
            //{
            //    //NavigationPane_Vision4.Text = m_smVSInfo[3].g_strVisionDisplayName + " " + m_smVSInfo[3].g_strVisionNameRemark;
            //    intVisionCount++;
            //}
            //else
            //    //NavigationPane_Vision4.Listed = false;

            //if ((m_smCustomizeInfo.g_intVisionMask & 0x04) > 0)
            //{
            //    //NavigationPane_Vision3.Text = m_smVSInfo[2].g_strVisionDisplayName + " " + m_smVSInfo[2].g_strVisionNameRemark;
            //    intVisionCount++;
            //}
            //else
            //    //NavigationPane_Vision3.Listed = false;

            //if ((m_smCustomizeInfo.g_intVisionMask & 0x02) > 0)
            //{
            //    //NavigationPane_Vision2.Text = m_smVSInfo[1].g_strVisionDisplayName + " " + m_smVSInfo[1].g_strVisionNameRemark;
            //    intVisionCount++;
            //}
            //else
            //    //NavigationPane_Vision2.Listed = false;

            //if ((m_smCustomizeInfo.g_intVisionMask & 0x01) > 0)
            //{
            //    NavigationPane_Vision1.Text = m_smVSInfo[0].g_strVisionDisplayName + " " + m_smVSInfo[0].g_strVisionNameRemark;
            //    intVisionCount++;
            //}
            //else
            //    NavigationPane_Vision1.Listed = false;

            //NavigationPane_Display2.Listed = false;
            //NavigationPane_Display3.Listed = false;

            int intVisionModuleCount = 0;
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] != null)
                    intVisionModuleCount++;
            }

            if (intVisionModuleCount <= 4)
                btn_DisplayConfig.Visible = false;
        }

        private void UpdateNewLotButtonGUI()
        {
            if (m_smCustomizeInfo.g_blnOnlyNewLot)
            {
                btn_NewLot.Image = imageList_NewLot.Images[1];
                btn_NewLot.Text = GetLanguageText("New Lot");
            }
            else
            {
                if (!m_smProductionInfo.g_blnEndLotStatus)
                {
                    btn_NewLot.Image = imageList_NewLot.Images[0];
                    btn_NewLot.Text = GetLanguageText("End Lot");
                }
                else
                {
                    btn_NewLot.Image = imageList_NewLot.Images[1];
                    btn_NewLot.Text = GetLanguageText("New Lot");
                }
            }
        }

        /// <summary>
        /// Enable menu button
        /// </summary>

        /// <summary>
        /// Enable Menu Button
        /// </summary>
        private void EnableMenuButton()
        {
            switch (m_intSelectedVisionStation)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    if (m_smVSInfo[m_intSelectedVisionStation - 1] != null && m_smVSInfo[m_intSelectedVisionStation - 1].g_intMachineStatus == 2)
                        IndividualAutoMode(false, m_intSelectedVisionStation - 1);
                    else
                        IndividualAutoMode(true, m_intSelectedVisionStation - 1);
                    break;
                default:
                    bool blnEnableResetCount = true;
                    for (int i = 0; i < m_smVSInfo.Length; i++)
                    {
                        if (m_smVSInfo[i] == null)
                            continue;

                        if (m_smVSInfo[i].g_intMachineStatus == 2)
                        {
                            blnEnableResetCount = false;
                            break;
                        }
                    }

                    if (blnEnableResetCount)
                        btn_ResetCounter.Enabled = true;
                    else
                        btn_ResetCounter.Enabled = false;

                    btn_Production.Checked = false;
                    btn_Production.Enabled = false;
                    btn_Production.Text = GetLanguageText("Production");
                    btn_Grab.Enabled = false;
                    //btn_Live.Checked = false;
                    btn_Live.Enabled = false;
                    //btn_Live.Text = "Live";
                    btn_LoadImage.Enabled = false;
                    btn_LoadImage2.Enabled = false;
                    btn_SaveImage.Enabled = false;
                    btn_ClearErrorMessage.Enabled = false;
                    btn_SaveErrorMessage.Enabled = false;
                    btn_ImagePrev.Enabled = false;
                    btn_ImageNext.Enabled = false;
                    btn_GRR.Enabled = false;
                    btn_GRR.Checked = false;
                    btn_Template.Enabled = false;
                    btn_Exit.Enabled = true;
                    btn_NewLot.Enabled = true;
                    for (int i = 0; i < m_smVSInfo.Length; i++)
                    {
                        if (m_smVSInfo[i] == null)
                            continue;

                        if (m_smVSInfo[i].g_intMachineStatus == 2)
                        {
                            btn_Exit.Enabled = false;
                            btn_NewLot.Enabled = false;
                            break;
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Customize vision station's function menu toolbar 
        /// </summary>
        /// <param name="smVisionInfo">vision info</param>
        /// <param name="intVisionNo">vision no</param>
        private void EnableLiveImage(VisionInfo smVisionInfo, int intVisionNo)
        {
            //if (smVisionInfo.g_strVisionName != NavigationBar_Auto.SelectedPane.Text)
            //    return;

            bool blnEnable = smVisionInfo.AT_PR_StartLiveImage;

            if (blnEnable)
                btn_Live.Text = GetLanguageText("Stop Live");
            else
                btn_Live.Text = GetLanguageText("Live");

            btn_Live.Checked = blnEnable;
            btn_SaveImage.Enabled = (!blnEnable && smVisionInfo.g_intMachineStatus == 1);

            if (m_blnImageLoadDisable[intVisionNo] && smVisionInfo.g_intMachineStatus == 1)
            {
                blnEnable = true;
                btn_Live.Enabled = false;
            }
            else if (smVisionInfo.g_intMachineStatus == 1)
                btn_Live.Enabled = true;

            bool blnStatus = (!blnEnable && smVisionInfo.g_intMachineStatus == 1);
            btn_Grab.Enabled = blnStatus;
            btn_LoadImage.Enabled = blnStatus;
            btn_LoadImage2.Enabled = blnStatus;
            if (smVisionInfo.g_arrImageFiles.Count > 1)
            {
                btn_ImageNext.Enabled = blnStatus;
                btn_ImagePrev.Enabled = blnStatus;
            }
            else
            {
                btn_ImageNext.Enabled = false;
                btn_ImagePrev.Enabled = false;
            }
            btn_Template.Enabled = blnStatus;
        }

        /// <summary>
        /// Get Individual Vision Name
        /// </summary>
        private void GetVisionName()
        {
            int intVisionCount = 0;

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                intVisionCount++;

                if (m_smVSInfo[i].g_strCameraModel == "IDS")
                {
                    if (m_objIDSCamera == null)
                        m_objIDSCamera = new IDSuEyeCamera();

                    m_objVisionPage[i] = new VisionPage(m_smCustomizeInfo, m_smProductionInfo, m_smVSInfo[i], m_blnDeviceReady, m_smVSComThread[i], m_thCOMMPort, m_objIDSCamera, m_smVSTCPIPIO[i]);
                }
                else
                {
                    m_objVisionPage[i] = new VisionPage(m_smCustomizeInfo, m_smProductionInfo, m_smVSInfo[i], m_blnDeviceReady, m_smVSComThread[i], m_thCOMMPort, m_smVSTCPIPIO[i]);
                }


                //SetVisionStation(intVisionCount, m_smVSInfo[i]);
            }

            m_objDisplay1Page = new AutoDisplayAllPage(m_smCustomizeInfo, m_smProductionInfo, m_smVSInfo);

            //m_objDisplay1Page = new AutoDisplayAllPage(m_smCustomizeInfo, m_smStation1, m_smStation2, m_smStation3, m_smStation4);

            //if (intVisionCount > 4)
            //    m_objDisplay2Page = new AutoDisplayAllPage(m_smCustomizeInfo, m_smStation5, m_smStation6, m_smStation7, m_smStation8);

            //if (intVisionCount > 8)
            //    m_objDisplay3Page = new AutoDisplayAllPage(m_smCustomizeInfo, m_smStation9, m_smStation10, new VisionInfo(), new VisionInfo());
        }

        /// <summary>
        /// Customize vision station's individual auto mode design
        /// </summary>
        /// <param name="blnEnable">true = enable function menu toolbar buttons, false = disable function menu toolbar buttons</param>
        /// <param name="intVisionNo">vision no</param>
        private void IndividualAutoMode(bool blnEnable, int intVisionNo)
        {
            bool blnEnable2 = true;

            // if one of the visions is in production mode, disable exit button
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;
                if (m_smVSInfo[i].g_intMachineStatus == 2)
                {
                    blnEnable2 = false;
                    break;
                }
            }

            if (blnEnable && blnEnable2)       // if got ant vision station is in production mode, disable user to exit autoform and create new lot
            {
                btn_NewLot.Enabled = true;
                btn_Exit.Enabled = true;
            }
            else
            {
                btn_NewLot.Enabled = false;
                btn_Exit.Enabled = false;
            }

            btn_Live.Enabled = blnEnable;
            btn_SaveErrorMessage.Enabled = blnEnable;
            btn_ClearErrorMessage.Enabled = blnEnable;

            string strParent = "Bottom Menu";
            string strChild1 = "Reset Counter";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild1Group(strParent, strChild1))
            {
                btn_ResetCounter.Enabled = false;
            }
            else
            {
                btn_ResetCounter.Enabled = blnEnable;
            }

            if (m_smVSInfo[m_intSelectedVisionStation - 1] != null)
                btn_GRR.Enabled = (m_smVSInfo[m_intSelectedVisionStation - 1].g_intMachineStatus == 1);

            if (intVisionNo < m_smVSInfo.Length && intVisionNo > -1)
            {
                btn_Production.Checked = !blnEnable;
                btn_Production.Enabled = true;
                if (btn_Production.Checked)
                {

                    btn_Production.Text = GetLanguageText("Stop");
                }
                else
                {
                    btn_Production.Text = GetLanguageText("Production");
                }
            }
            else
            {
                btn_Production.Checked = false;
                btn_Production.Enabled = false;
                btn_Production.Text = GetLanguageText("Production");
            }


            if (!blnEnable)
            {
                if (m_smVSInfo[m_intSelectedVisionStation - 1] != null && m_smVSInfo[m_intSelectedVisionStation - 1].AT_PR_StartLiveImage)
                {
                    btn_Live.Text = GetLanguageText("Stop Live");
                    btn_Live.Checked = true;
                }
                else
                {
                    btn_Live.Text = GetLanguageText("Live");
                    btn_Live.Checked = false;
                }

                btn_Grab.Enabled = false;
                btn_SaveImage.Enabled = false;
                btn_LoadImage.Enabled = false;
                btn_LoadImage2.Enabled = false;
                btn_ImageNext.Enabled = false;
                btn_ImagePrev.Enabled = false;
            }
            else if (intVisionNo < m_smVSInfo.Length && intVisionNo > -1)
                EnableLiveImage(m_smVSInfo[intVisionNo], intVisionNo);
        }

        /// <summary>
        /// Load general settings and update global interface
        /// </summary>
        /// <param name="strRecipeID">Selected Recipe File Name</param>
        private bool LoadRecipe(string strSelectedRecipe)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");

            string strDirectoryName = "";
            string strTargetDirectory = "";
            bool blnUseServerRecipe = false;
            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
            {
                if (NetworkTransfer.IsConnectionPass(m_smCustomizeInfo.g_strHostIP))
                {
                    if (Directory.Exists(m_smCustomizeInfo.g_strDeviceUploadDir))
                    {
                        blnUseServerRecipe = true;
                        strTargetDirectory = m_smCustomizeInfo.g_strDeviceUploadDir + "\\DeviceNo";
                    }
                    else
                    {
                        SRMMessageBox.Show("Recipe server path does not exist!", "Device No Setting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
                else
                {
                    SRMMessageBox.Show("Fail to connect to recipe server!", "Device No Setting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else
            {
                strTargetDirectory = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo";
            }

            string[] strDirectoryEntries = Directory.GetDirectories(strTargetDirectory);
            int intDirectoryLength = strTargetDirectory.Length + 1;

            if (strDirectoryEntries.Length == 0)
            {
                SRMMessageBox.Show("All Device No. Not Available! Please Contact SRM", "Device No Setting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
                List<bool> isSelected = new List<bool>(new bool[10]);
                foreach (string strDirectory in strDirectoryEntries)
                {
                    strDirectoryName = strDirectory.Remove(0, intDirectoryLength);
                    for (int i = 0; i < m_smVSInfo.Length; i++)
                    {
                        if (m_smVSInfo[i] == null || isSelected[i])
                            continue;

                        if (strDirectoryName == m_smProductionInfo.g_arrSingleRecipeID[i])
                        {
                            isSelected[i] = true;
                        }
                    }
                }

                int intCount = 0;
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                    {
                        isSelected.RemoveAt(intCount);

                    }
                    else
                        intCount++;
                }

                if (isSelected.Contains(false))
                {
                    SRMMessageBox.Show("Device No. Selected Is Not Available", "Device No Setting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            bool blnSameRecipe = true;
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (m_smProductionInfo.g_arrSingleRecipeID[i] != m_arrSelectedRecipePrev[i])
                {
                    blnSameRecipe = false;
                    break;
                }
            }

            // 2019 07 03 - JBTAN: dont have to load recipe to tempRecipe every time, only load if:
            //1. init and load from server
            //2. switch from load from local to load from server
            //3. change recipe during load from server
            if (blnUseServerRecipe && !m_blnUseServerRecipePrev || blnUseServerRecipe && (!blnSameRecipe))
            {
                StartWaiting("Loading Recipe...");
                // Delete previous TempRecipe, then transfer selected recipe from server to TempRecipe, finally set the tempRecipe path to DeviceNoPath 
                DeleteFilesAndFoldersRecursively(AppDomain.CurrentDomain.BaseDirectory + "TempRecipe");
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "TempRecipe");
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    if (m_smProductionInfo.g_arrSingleRecipeID[i] != m_arrSelectedRecipePrev[i])
                        NetworkTransfer.TransferFile_MappedNetwork(strTargetDirectory + "\\" + m_smProductionInfo.g_arrSingleRecipeID[i], AppDomain.CurrentDomain.BaseDirectory + "TempRecipe\\" + m_smProductionInfo.g_arrSingleRecipeID[i]);
                }
                StopWaiting();
                m_smProductionInfo.g_strRecipePath = AppDomain.CurrentDomain.BaseDirectory + "TempRecipe\\";
            }
            else
            {
                m_smProductionInfo.g_strRecipePath = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\";
            }

            m_smProductionInfo.g_strRecipeID = strSelectedRecipe;

            if (m_intSelectedVisionStation <= 0)
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                {
                    if (blnUseServerRecipe)
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Server)";
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Local)";
                }
                else
                    lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID;
            }
            else
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                {
                    if (blnUseServerRecipe)
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Server)";
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Local)";
                }
                else
                    lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1];
            }

            m_smProductionInfo.g_strLotID = (string)subkey1.GetValue("LotNo", "SRM");
            for (int i = 0; i < 10; i++)
            {
                m_smProductionInfo.g_arrSingleLotID[i] = (string)subkey1.GetValue("SingleLotNo" + i.ToString(), "");
                if (m_smProductionInfo.g_arrSingleLotID[i] == "")
                    m_smProductionInfo.g_arrSingleLotID[i] = m_smProductionInfo.g_strLotID;

                m_smProductionInfo.g_arrIsNewLot[i] = (bool)subkey1.GetValue("IsNewLot" + i.ToString(), false).Equals("True");
            }
            m_smProductionInfo.g_strOperatorID = (string)subkey1.GetValue("OperatorID", "Op");
            m_smProductionInfo.g_strLotStartTime = (string)subkey1.GetValue("LotStartTime", "0");
            // if registry LotStartTime no exist, use current time and set it to registry so that datetime is compatible
            if (m_smProductionInfo.g_strLotStartTime == "0")
            {
                m_smProductionInfo.g_strLotStartTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                subkey1.SetValue("LotStartTime", m_smProductionInfo.g_strLotStartTime);
            }
            return true;
        }

        /// <summary>
        /// Stop production
        /// </summary>
        private void ProductionStopMode()
        {
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null) continue;
                m_smVSInfo[i].g_intMachineStatus = 1;
            }

            //31-07-2019 ZJYEOH : To change the vision Tab Image when auto stop production
            btn_Vision1Tab.ImageIndex = 0;

            btn_Vision2Tab.ImageIndex = 0;

            btn_Vision3Tab.ImageIndex = 0;

            btn_Vision4Tab.ImageIndex = 0;

            btn_Vision5Tab.ImageIndex = 0;

            btn_Vision6Tab.ImageIndex = 0;

            btn_Vision7Tab.ImageIndex = 0;

            btn_Vision8Tab.ImageIndex = 0;

            btn_Vision9Tab.ImageIndex = 0;

            btn_Vision10Tab.ImageIndex = 0;

            if (m_smCustomizeInfo.g_blnSavePassImage || m_smCustomizeInfo.g_blnSaveFailImage)
            {
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    if (m_smVSInfo[i].g_intMachineStatus == 2)
                        continue;

                    VisionInfo smVisionInfo = m_smVSInfo[i];

                    string strPath = smVisionInfo.g_strSaveImageLocation +
                             m_smProductionInfo.g_arrSingleLotID[i] + "_" + m_smProductionInfo.g_strLotStartTime;

                    //string strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")";

                    string strVisionImageFolderName;
                    if (smVisionInfo.g_intVisionResetCount == 0)
                        strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")" + "_" + m_smProductionInfo.g_strLotStartTime;
                    else
                        strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")" + "_" + smVisionInfo.g_strVisionResetCountTime;

                    string Path = strPath + "\\" + strVisionImageFolderName;

                    if (Directory.Exists(Path))
                    {
                        string[] dir = Directory.GetDirectories(Path);
                        bool FirstTime = true;
                        foreach (string dirs in dir)
                        {
                            string[] Files = Directory.GetFiles(dirs);

                            //foreach (string imageName in Files)
                            //{
                            if (Files.Length > 0)
                            {
                                if (FirstTime)
                                {
                                    smVisionInfo.g_arrImageFiles.Clear();
                                    smVisionInfo.g_intFileIndex = -1;
                                }
                                LoadCurrentLotImages_AfterStopProduction(dirs + "\\", FirstTime, i);
                                FirstTime = false;
                            }
                            //}
                        }
                    }
                    SortAccordingToDateTime(ref m_smVSInfo[i].g_arrImageFiles);
                }

            }

            EnableMenuButton();
        }

        private void ReadRegistry()
        {
            RegistryKey Key = Registry.LocalMachine.OpenSubKey("Software");
            RegistryKey subKey1 = Key.OpenSubKey("SVG\\AutoMode");

            m_smProductionInfo.g_blnViewCrosshair = chk_Crosshair.Checked = (bool)subKey1.GetValue("ViewCrosshair", true).Equals("True");
            m_smProductionInfo.g_blnViewSearchROI = chk_SearchROI.Checked = (bool)subKey1.GetValue("ViewROI", true).Equals("True");
            m_smProductionInfo.g_blnViewPosCrosshair = chk_PositionCrosshair.Checked = (bool)subKey1.GetValue("ViewPosCrosshair", true).Equals("True");

            m_smProductionInfo.g_blnViewCrosshair = (bool)subKey1.GetValue("ViewCrosshair", true).Equals("True");
            m_smProductionInfo.g_blnViewPosCrosshair = (bool)subKey1.GetValue("ViewPosCrosshair", true).Equals("True");
            m_smProductionInfo.g_blnViewInspection = (bool)subKey1.GetValue("ViewInspection", true).Equals("True");
            m_smProductionInfo.g_blnViewPadResult = (bool)subKey1.GetValue("ViewPadResult", true).Equals("True");
            m_smProductionInfo.g_blnViewPackageResult = (bool)subKey1.GetValue("ViewPackageResult", true).Equals("True");
            m_smProductionInfo.g_blnViewPocketPositionResult = (bool)subKey1.GetValue("ViewPocketPositionResult", true).Equals("True");
            m_smProductionInfo.g_blnViewPadROIToleranceROI = (bool)subKey1.GetValue("ViewPadROIToleranceROI", true).Equals("True");
            m_smProductionInfo.g_blnViewMarkROI = (bool)subKey1.GetValue("ViewMarkROI", true).Equals("True");
            m_smProductionInfo.g_blnViewLeadDontCareROI = (bool)subKey1.GetValue("ViewLeadDontCareROI", true).Equals("True");
            m_smProductionInfo.g_blnViewPadInspectionArea = (bool)subKey1.GetValue("ViewPadInspectionArea", true).Equals("True");
            m_smProductionInfo.g_blnViewBarcodeInspectionArea = (bool)subKey1.GetValue("ViewBarcodeInspectionArea", true).Equals("True");
            m_smProductionInfo.g_blnViewBarcodePatternInspectionArea = (bool)subKey1.GetValue("ViewBarcodePatternInspectionArea", true).Equals("True");
            m_smProductionInfo.g_blnViewSearchROI = (bool)subKey1.GetValue("ViewROI", true).Equals("True");
            m_smProductionInfo.g_blnViewROITool = (bool)subKey1.GetValue("ViewROITool", true).Equals("True");
            m_smProductionInfo.g_blnViewROIDetails = (bool)subKey1.GetValue("ViewROIDetails", false).Equals("True");
            m_smProductionInfo.g_blnViewPositionXY = (bool)subKey1.GetValue("ViewPositionXY", true).Equals("True");
            m_smProductionInfo.g_blnViewGrayPixel = (bool)subKey1.GetValue("ViewGrayPixel", true).Equals("True");
            m_smProductionInfo.g_blnView5SRuler = (bool)subKey1.GetValue("View5SRuler", true).Equals("True");

            // Hide Mark Checkbox if mark vision is not used.

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                m_smProductionInfo.g_blnViewInspection = chk_Inspect.Checked = (bool)subKey1.GetValue("ViewInspection", true).Equals("True");
            }

            m_smProductionInfo.g_arrDisplayVisionModule[0] = Convert.ToInt32(subKey1.GetValue("SelectedVisionModuleDisplay1", 0));
            m_smProductionInfo.g_arrDisplayVisionModule[1] = Convert.ToInt32(subKey1.GetValue("SelectedVisionModuleDisplay2", 1));
            m_smProductionInfo.g_arrDisplayVisionModule[2] = Convert.ToInt32(subKey1.GetValue("SelectedVisionModuleDisplay3", 2));
            m_smProductionInfo.g_arrDisplayVisionModule[3] = Convert.ToInt32(subKey1.GetValue("SelectedVisionModuleDisplay4", 3));
            m_smProductionInfo.g_arrDisplayVisionModule[4] = Convert.ToInt32(subKey1.GetValue("SelectedVisionModuleDisplay5", 4));
            m_smProductionInfo.g_arrDisplayVisionModule[5] = Convert.ToInt32(subKey1.GetValue("SelectedVisionModuleDisplay6", 5));
            m_smProductionInfo.g_arrDisplayVisionModule[6] = Convert.ToInt32(subKey1.GetValue("SelectedVisionModuleDisplay7", 6));
            m_smProductionInfo.g_arrDisplayVisionModule[7] = Convert.ToInt32(subKey1.GetValue("SelectedVisionModuleDisplay8", 7));

            m_smProductionInfo.g_blnEndLotStatus = (bool)subKey1.GetValue("EndLotStatus", true).Equals("True");

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                m_smVSInfo[i].g_intSelectedImage = Convert.ToInt32(subKey1.GetValue("CurrentSelectedImage" + i.ToString(), 0));

                m_smVSInfo[i].g_intProductionViewImage = m_smVSInfo[i].g_intSelectedImage;

                m_smVSInfo[i].g_strLastImageFolder = subKey1.GetValue("VS" + (i + 1) + "LastImageFolder", "").ToString();
                m_smVSInfo[i].g_strLastImageName = subKey1.GetValue("VS" + (i + 1) + "LastImageName", "").ToString();

                // Tracking menu
                m_smVSInfo[i].g_blnSaveImageAfterGrab = Convert.ToBoolean(subKey1.GetValue("VS" + (i + 1) + "Tracking_SaveImageAfterGrab", false));
                m_smVSInfo[i].g_blnTrackIO = Convert.ToBoolean(subKey1.GetValue("VS" + (i + 1) + "Tracking_IO", false));
                m_smVSInfo[i].g_blnTrackSaveImageFile = Convert.ToBoolean(subKey1.GetValue("VS" + (i + 1) + "Tracking_saveImageFileName", false));

                if (i != (m_intSelectedVisionStation - 1))
                    continue;
            }
        }

        /// <summary>
        /// Reset current counter
        /// </summary>
        /// <param name="intVision">vision no</param>
        private void ResetCurrentCount(int intVision, bool blnTCPIP)
        {
            int i = 0;
            try
            {
                for (i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    bool blnReset = false;
                    if (blnTCPIP)
                    {
                        if ((intVision & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) > 0 || intVision == 0 || m_smProductionInfo.g_arrVisionIDIndex[i] == 0)
                            blnReset = true;
                    }
                    else
                    {
                        if ((intVision & (1 << i)) > 0)
                            blnReset = true;
                    }

                    if (blnReset)
                    {
                        m_smVSInfo[i].g_intPassTotal = 0;
                        m_smVSInfo[i].g_intMarkFailureTotal = 0;
                        m_smVSInfo[i].g_intOrientFailureTotal = 0;
                        m_smVSInfo[i].g_intPin1FailureTotal = 0;
                        m_smVSInfo[i].g_intLeadFailureTotal = 0;
                        m_smVSInfo[i].g_intPackageFailureTotal = 0;
                        m_smVSInfo[i].g_intPadFailureTotal = 0;
                        m_smVSInfo[i].g_intBarcodeFailureTotal = 0;
                        m_smVSInfo[i].g_intSealFailureTotal = 0;
                        m_smVSInfo[i].g_intSealDistanceFailureTotal = 0;
                        m_smVSInfo[i].g_intSealOverHeatFailureTotal = 0;
                        m_smVSInfo[i].g_intSealBrokenAreaFailureTotal = 0;
                        m_smVSInfo[i].g_intSealBrokenGapFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleDiameterFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleBrokenFailureTotal = 0;
                        m_smVSInfo[i].g_intSealSprocketHoleRoundnessFailureTotal = 0;
                        m_smVSInfo[i].g_intSealEdgeStraightnessFailureTotal = 0;
                        m_smVSInfo[i].g_intCheckPresenceFailureTotal = 0;
                        m_smVSInfo[i].g_intPositionFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadOffsetFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadAreaFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadBrokenFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadExcessFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadSmearFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadStandOffFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadSpanFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadContaminationFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadMissingFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadOffsetFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadAreaFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadPitchGapFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadBrokenFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadExcessFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadSmearFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadEdgeLimitFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadStandOffFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadEdgeDistanceFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadSpanFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadContaminationFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadMissingFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePadColorDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intSidePkgDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadOffsetFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadSkewFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadWidthFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadLengthFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadLengthVarianceFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPitchGapFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPitchVarianceFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadStandOffFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadStandOffVarianceFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadCoplanFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadAGVFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadSpanFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadSweepsFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadUnCutTiebarFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadMissingFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadContaminationFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intLeadPkgDimensionFailureTotal = 0;
                        m_smVSInfo[i].g_intNoTemplateFailureTotal = 0;
                        m_smVSInfo[i].g_intPkgDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intPkgColorDefectFailureTotal = 0;
                        m_smVSInfo[i].g_intEdgeNotFoundFailureTotal = 0;
                        m_smVSInfo[i].g_intAngleFailureTotal = 0;
                        m_smVSInfo[i].g_intLowYieldPass = 0;
                        m_smVSInfo[i].g_intLowYieldUnitCount = 0;
                        m_smVSInfo[i].g_intContinuousPassUnitCount = 0;
                        m_smVSInfo[i].g_intContinuousFailUnitCount = 0;
                        m_smVSInfo[i].g_intTestedTotal = 0;
                        m_smVSInfo[i].g_intPassImageCount = 0;
                        m_smVSInfo[i].g_intFailImageCount = 0;
                        m_smVSInfo[i].g_intTotalImageCount = 0;
                        m_smVSInfo[i].g_blnWantClearSaveImageInfo = true; // Clear all save pass fail image information before new lot, to prevent delete wrong image and over save image

                        //m_smVSInfo[i].PR_VM_UpdateQuantity = true;
                        m_smVSInfo[i].VS_AT_UpdateQuantity = true;
                    }
                }

                m_smProductionInfo.VM_TH_UpdateCount = true;
                m_smProductionInfo.g_blnResetPass = true;
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Unable to reset " + m_smVSInfo[i].g_strVisionName + " current count", "SRM Vision");
                m_objTrack.WriteLine("AutoForm (ResetCurrentCount): " + ex.ToString());
                m_smProductionInfo.g_blnResetPass = false;
            }
            finally
            {
                m_smProductionInfo.AT_CO_ResetCountDone = true;
            }
        }

        private void Get2DCode(int intVision)
        {
            int i = 0;
            try
            {
                for (i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    if ((intVision & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) > 0 || intVision == 0 || m_smProductionInfo.g_arrVisionIDIndex[i] == 0)
                    {
                        m_smProductionInfo.AT_CO_DataMatrixCode = m_smVSInfo[i].g_arrMarks[0].Get2DCodeResult();
                        break;
                    }
                }

                m_smProductionInfo.g_bln2DCodePass = true;
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Unable to get 2DCode " + m_smVSInfo[i].g_strVisionName + " current count", "SRM Vision");
                m_objTrack.WriteLine("AutoForm (Get2DCode): " + ex.ToString());
                m_smProductionInfo.g_bln2DCodePass = false;
            }
            finally
            {
                m_smProductionInfo.AT_CO_Get2DCodeDone = true;
            }
        }

        /// <summary>
        /// Save lot file
        /// </summary>
        private void SaveLotFile()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            string strLot = subkey1.GetValue("LotNoPrev", "SRM").ToString();
            string strLotStartTime = subkey1.GetValue("LotStartTimePrev", DateTime.Now.ToString("yyyyMMddHHmmss")).ToString();

            string strLotStopTime = subkey1.GetValue("LotStartTime", DateTime.Now.ToString("yyyyMMddHHmmss")).ToString();
            strLotStopTime = strLotStopTime.Substring(0, 4) + "/" + strLotStopTime.Substring(4, 2) + "/" +
                strLotStopTime.Substring(6, 2) + " " + strLotStopTime.Substring(8, 2) + ":" + strLotStopTime.Substring(10, 2) +
                ":" + strLotStopTime.Substring(12, 2);

            bool blnAllSingleLotIDSame = true;
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (((m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) == 0 && m_smProductionInfo.g_intVisionInvolved != 0) && m_smProductionInfo.g_arrVisionIDIndex[i] != 0)
                    continue;

                if (m_smProductionInfo.g_arrIsNewLot[i])
                    blnAllSingleLotIDSame = false;

                //m_smProductionInfo.g_arrIsNewLot[i] = false;
                //subkey1.SetValue("IsNewLot" + i.ToString(), m_smProductionInfo.g_arrIsNewLot[i]);
            }
            if (strLotStartTime.Contains("/"))
                strLotStartTime = strLotStartTime.Remove('/');
            if (strLotStartTime.Contains(":"))
                strLotStartTime = strLotStartTime.Remove(':');
            XmlParser objFile;
            string strLotPath = AppDomain.CurrentDomain.BaseDirectory + "LotReport\\" + strLot + "_" + strLotStartTime + ".xml";
            string strLotPathPrev = subkey1.GetValue("LotReportPathPrev", strLotPath).ToString();

            if (!blnAllSingleLotIDSame)
            {
                bool blnRecord = false;
                if (!File.Exists(strLotPathPrev))
                    blnRecord = true;

                objFile = new XmlParser(strLotPathPrev);
                if (blnRecord)
                {
                    strLotStartTime = strLotStartTime.Substring(0, 4) + "/" + strLotStartTime.Substring(4, 2) + "/" +
                  strLotStartTime.Substring(6, 2) + " " + strLotStartTime.Substring(8, 2) + ":" + strLotStartTime.Substring(10, 2) +
                  ":" + strLotStartTime.Substring(12, 2);

                    objFile.WriteSectionElement("Lot");
                    objFile.WriteElement1Value("LotID", strLot);
                    objFile.WriteElement1Value("OperatorID", subkey1.GetValue("OperatorIDPrev", "Op").ToString());
                    objFile.WriteElement1Value("RecipeID", subkey1.GetValue("RecipeIDPrev", "Default").ToString());
                    objFile.WriteElement1Value("LotStartTime", strLotStartTime);
                    objFile.WriteElement1Value("LotStopTime", strLotStopTime);
                    objFile.WriteElement1Value("MachineID", m_smCustomizeInfo.g_strMachineID);

                    subkey1.SetValue("LotReportPathPrev", strLotPathPrev);

                }
                else
                {
                    objFile.WriteSectionElement("Lot");
                    objFile.WriteElement1Value("LotStopTime", strLotStopTime);

                    bool blnAllNewLot = true;
                    for (int i = 0; i < m_smVSInfo.Length; i++)
                    {
                        if (m_smVSInfo[i] == null)
                            continue;

                        if (!m_smProductionInfo.g_arrIsNewLot[i])
                            blnAllNewLot = false;

                    }

                    if (blnAllNewLot && subkey1.GetValue("LotReportPathPrev", "----").ToString() != "----")
                        subkey1.DeleteValue("LotReportPathPrev");

                    if (blnAllNewLot)
                    {
                        for (int i = 0; i < m_smVSInfo.Length; i++)
                        {
                            if (m_smVSInfo[i] == null)
                                continue;

                            m_smProductionInfo.g_arrIsNewLot[i] = false;
                            subkey1.SetValue("IsNewLot" + i.ToString(), m_smProductionInfo.g_arrIsNewLot[i]);
                        }
                    }
                }

            }
            else
            {
                objFile = new XmlParser(strLotPath);

                strLotStartTime = strLotStartTime.Substring(0, 4) + "/" + strLotStartTime.Substring(4, 2) + "/" +
                strLotStartTime.Substring(6, 2) + " " + strLotStartTime.Substring(8, 2) + ":" + strLotStartTime.Substring(10, 2) +
                ":" + strLotStartTime.Substring(12, 2);

                objFile.WriteSectionElement("Lot");
                objFile.WriteElement1Value("LotID", strLot);
                objFile.WriteElement1Value("OperatorID", subkey1.GetValue("OperatorIDPrev", "Op").ToString());
                objFile.WriteElement1Value("RecipeID", subkey1.GetValue("RecipeIDPrev", "Default").ToString());
                objFile.WriteElement1Value("LotStartTime", strLotStartTime);
                objFile.WriteElement1Value("LotStopTime", strLotStopTime);
                objFile.WriteElement1Value("MachineID", m_smCustomizeInfo.g_strMachineID);

                if (subkey1.GetValue("LotReportPathPrev", "----").ToString() != "----")
                    subkey1.DeleteValue("LotReportPathPrev");

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    m_smProductionInfo.g_arrIsNewLot[i] = false;
                    subkey1.SetValue("IsNewLot" + i.ToString(), m_smProductionInfo.g_arrIsNewLot[i]);
                }
            }


            m_blnWantSaveAnotherLot = false;
            int intVisionCount = 0;
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (((m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) == 0 && m_smProductionInfo.g_intVisionInvolved != 0) && m_smProductionInfo.g_arrVisionIDIndex[i] != 0)
                    continue;

                if (m_smProductionInfo.g_intVisionInvolved == 0 && m_smProductionInfo.g_arrIsNewLot[i])
                {
                    m_intVisionInvolved |= (1 << i);
                    m_blnWantSaveAnotherLot = true;
                    m_smProductionInfo.g_arrIsNewLot[i] = false;
                    subkey1.SetValue("IsNewLot" + i.ToString(), m_smProductionInfo.g_arrIsNewLot[i]);
                    continue;
                }

                if (m_smVSInfo[i].g_strVisionName == "Barcode") // 2020-11-26 ZJYEOH : Barcode dont have counter
                    continue;

                int intPos = (1 << i);
                objFile.WriteSectionElement("VisionFeature" + i);//intVisionCount
                //if (m_smVSInfo[i].g_intVisionSameCount == 0)
                //    objFile.WriteElement1Value("VisionName", m_smVSInfo[i].g_strVisionName);
                //else
                //    objFile.WriteElement1Value("VisionName", m_smVSInfo[i].g_strVisionName + (m_smVSInfo[i].g_intVisionIndex + 1));
                //objFile.WriteElement1Value("VisionName", m_smVSInfo[i].g_strVisionName + m_smVSInfo[i].g_strVisionNameRemark);

                objFile.WriteElement1Value("VisionName", m_smVSInfo[i].g_strVisionName + m_smVSInfo[i].g_strVisionNameNo);

                objFile.WriteElement1Value("Total", m_smVSInfo[i].g_intTestedTotal);
                objFile.WriteElement1Value("Pass", m_smVSInfo[i].g_intPassTotal);
                objFile.WriteElement1Value("Fail", "");
                if ((m_smCustomizeInfo.g_intWantMark & intPos) > 0)
                    objFile.WriteElement2Value("Mark", m_smVSInfo[i].g_intMarkFailureTotal);
                if ((m_smCustomizeInfo.g_intWantOrient & intPos) > 0)
                    objFile.WriteElement2Value("Orient", m_smVSInfo[i].g_intOrientFailureTotal);
                if (m_smVSInfo[i].g_blnWantPin1)
                    objFile.WriteElement2Value("Pin1", m_smVSInfo[i].g_intPin1FailureTotal);
                if ((m_smCustomizeInfo.g_intWantLead3D & intPos) > 0)
                {
                    objFile.WriteElement2Value("Lead", m_smVSInfo[i].g_intLeadFailureTotal);

                    int intFailMask = m_smVSInfo[i].g_arrLead3D[0].ref_intFailOptionMask;

                    if ((intFailMask & 0x20000) > 0 || m_smVSInfo[i].g_intLeadOffsetFailureTotal > 0)
                        objFile.WriteElement2Value("LeadOffset", m_smVSInfo[i].g_intLeadOffsetFailureTotal);
                    if ((intFailMask & 0x100) > 0 || m_smVSInfo[i].g_intLeadSkewFailureTotal > 0)
                        objFile.WriteElement2Value("LeadSkew", m_smVSInfo[i].g_intLeadSkewFailureTotal);
                    if ((intFailMask & 0x40) > 0 || m_smVSInfo[i].g_intLeadWidthFailureTotal > 0)
                        objFile.WriteElement2Value("LeadWidth", m_smVSInfo[i].g_intLeadWidthFailureTotal);
                    if ((intFailMask & 0x80) > 0 || m_smVSInfo[i].g_intLeadLengthFailureTotal > 0)
                        objFile.WriteElement2Value("LeadLength", m_smVSInfo[i].g_intLeadLengthFailureTotal);
                    if ((intFailMask & 0x800) > 0 || m_smVSInfo[i].g_intLeadLengthVarianceFailureTotal > 0)
                        objFile.WriteElement2Value("LeadLengthVariance", m_smVSInfo[i].g_intLeadLengthVarianceFailureTotal);
                    if ((intFailMask & 0x200) > 0 || m_smVSInfo[i].g_intLeadPitchGapFailureTotal > 0)
                        objFile.WriteElement2Value("LeadPitchGap", m_smVSInfo[i].g_intLeadPitchGapFailureTotal);
                    if ((intFailMask & 0x2000) > 0 || m_smVSInfo[i].g_intLeadPitchVarianceFailureTotal > 0)
                        objFile.WriteElement2Value("LeadPitchVariance", m_smVSInfo[i].g_intLeadPitchVarianceFailureTotal);
                    if ((intFailMask & 0x01) > 0 || m_smVSInfo[i].g_intLeadStandOffFailureTotal > 0)
                        objFile.WriteElement2Value("LeadStandOff", m_smVSInfo[i].g_intLeadStandOffFailureTotal);
                    if ((intFailMask & 0x100) > 0 || m_smVSInfo[i].g_intLeadStandOffVarianceFailureTotal > 0)
                        objFile.WriteElement2Value("LeadStandOffVariance", m_smVSInfo[i].g_intLeadStandOffVarianceFailureTotal);
                    if ((intFailMask & 0x02) > 0 || m_smVSInfo[i].g_intLeadCoplanFailureTotal > 0)
                        objFile.WriteElement2Value("LeadCoplan", m_smVSInfo[i].g_intLeadCoplanFailureTotal);
                    if ((intFailMask & 0x40000) > 0 || m_smVSInfo[i].g_intLeadAGVFailureTotal > 0)
                        objFile.WriteElement2Value("LeadAGV", m_smVSInfo[i].g_intLeadAGVFailureTotal);
                    if ((intFailMask & 0x1000) > 0 || m_smVSInfo[i].g_intLeadSpanFailureTotal > 0)
                        objFile.WriteElement2Value("LeadSpan", m_smVSInfo[i].g_intLeadSpanFailureTotal);
                    if ((intFailMask & 0x04) > 0 || m_smVSInfo[i].g_intLeadSweepsFailureTotal > 0)
                        objFile.WriteElement2Value("LeadSweeps", m_smVSInfo[i].g_intLeadSweepsFailureTotal);
                    if ((intFailMask & 0x10) > 0 || m_smVSInfo[i].g_intLeadUnCutTiebarFailureTotal > 0)
                        objFile.WriteElement2Value("LeadUnCutTiebar", m_smVSInfo[i].g_intLeadUnCutTiebarFailureTotal);
                    if (m_smVSInfo[i].g_intLeadMissingFailureTotal > 0)
                        objFile.WriteElement2Value("LeadMissing", m_smVSInfo[i].g_intLeadMissingFailureTotal);
                    if ((((intFailMask & 0x8000) > 0) || ((intFailMask & 0x10000) > 0)) || m_smVSInfo[i].g_intLeadContaminationFailureTotal > 0)
                        objFile.WriteElement2Value("LeadContamination", m_smVSInfo[i].g_intLeadContaminationFailureTotal);

                    if ((m_smCustomizeInfo.g_intWantPackage & intPos) > 0)
                    {
                        if (m_smVSInfo[i].g_arrLead3D[0].GetWantInspectPackage() || m_smVSInfo[i].g_intLeadPkgDefectFailureTotal > 0)
                            objFile.WriteElement2Value("LeadPkgDefect", m_smVSInfo[i].g_intLeadPkgDefectFailureTotal);
                        if ((m_smVSInfo[i].g_arrLead3D[0].ref_intFailPkgOptionMask & 0x01) > 0 || m_smVSInfo[i].g_intLeadPkgDimensionFailureTotal > 0)
                            objFile.WriteElement2Value("LeadPkgDimension", m_smVSInfo[i].g_intLeadPkgDimensionFailureTotal);
                    }
                }

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

                if (((m_smCustomizeInfo.g_intWantPad & intPos) > 0))
                {
                    objFile.WriteElement2Value("Pad", m_smVSInfo[i].g_intPadFailureTotal);

                    int intFailMask = m_smVSInfo[i].g_arrPad[0].ref_intFailOptionMask;
                    if ((intFailMask & 0x100) > 0 || m_smVSInfo[i].g_intCenterPadOffsetFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadOffset", m_smVSInfo[i].g_intCenterPadOffsetFailureTotal);
                    if ((intFailMask & 0x20) > 0 || m_smVSInfo[i].g_intCenterPadAreaFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadArea", m_smVSInfo[i].g_intCenterPadAreaFailureTotal);
                    if ((intFailMask & 0xC0) > 0 || m_smVSInfo[i].g_intCenterPadDimensionFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadDimension", m_smVSInfo[i].g_intCenterPadDimensionFailureTotal);
                    if ((intFailMask & 0x600) > 0 || m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadPitchGap", m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal);
                    if ((intFailMask & 0x18) > 0 || m_smVSInfo[i].g_intCenterPadBrokenFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadBroken", m_smVSInfo[i].g_intCenterPadBrokenFailureTotal);
                    if ((intFailMask & 0x800) > 0 || m_smVSInfo[i].g_intCenterPadExcessFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadExcess", m_smVSInfo[i].g_intCenterPadExcessFailureTotal);
                    if ((intFailMask & 0x2000) > 0 || m_smVSInfo[i].g_intCenterPadSmearFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadSmear", m_smVSInfo[i].g_intCenterPadSmearFailureTotal);
                    if ((intFailMask & 0x4000) > 0 || m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadEdgeLimit", m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal);
                    if ((intFailMask & 0x8000) > 0 || m_smVSInfo[i].g_intCenterPadStandOffFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadStandOff", m_smVSInfo[i].g_intCenterPadStandOffFailureTotal);
                    if ((intFailMask & 0x10000) > 0 || m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadEdgeDistance", m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal);
                    if ((intFailMask & 0x20000) > 0 || (intFailMask & 0x40000) > 0 || m_smVSInfo[i].g_intCenterPadSpanFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadSpan", m_smVSInfo[i].g_intCenterPadSpanFailureTotal);

                    if (m_smVSInfo[i].g_intCenterPadMissingFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadMissing", m_smVSInfo[i].g_intCenterPadMissingFailureTotal);

                    if (((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0) && m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadColorDefect", m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal);

                    if (m_smVSInfo[i].g_intPositionFailureTotal > 0)
                        objFile.WriteElement2Value("Position", m_smVSInfo[i].g_intPositionFailureTotal);

                    if ((m_smCustomizeInfo.g_intWantPackage & intPos) > 0)
                    {
                        if (m_smVSInfo[i].g_arrPad[0].GetWantInspectPackage() || m_smVSInfo[i].g_intCenterPkgDefectFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPkgDefect", m_smVSInfo[i].g_intCenterPkgDefectFailureTotal);
                        if ((m_smVSInfo[i].g_arrPad[0].ref_intFailPkgOptionMask & 0x01) > 0 || m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPkgDimension", m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal);
                    }
                    else
                    {
                        if ((intFailMask & 0x1001) > 0 || m_smVSInfo[i].g_intCenterPadContaminationFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadContamination", m_smVSInfo[i].g_intCenterPadContaminationFailureTotal);
                    }

                    //objFile.WriteElement1Value("Statistic", "");
                }

                if (((m_smCustomizeInfo.g_intWantPad5S & intPos) > 0))
                {
                    objFile.WriteElement2Value("Pad", m_smVSInfo[i].g_intPadFailureTotal);

                    int intFailMask = m_smVSInfo[i].g_arrPad[0].ref_intFailOptionMask;
                    if ((intFailMask & 0x100) > 0 || m_smVSInfo[i].g_intCenterPadOffsetFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadOffset", m_smVSInfo[i].g_intCenterPadOffsetFailureTotal);
                    if ((intFailMask & 0x20) > 0 || m_smVSInfo[i].g_intCenterPadAreaFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadArea", m_smVSInfo[i].g_intCenterPadAreaFailureTotal);
                    if ((intFailMask & 0xC0) > 0 || m_smVSInfo[i].g_intCenterPadDimensionFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadDimension", m_smVSInfo[i].g_intCenterPadDimensionFailureTotal);
                    if ((intFailMask & 0x600) > 0 || m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadPitchGap", m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal);
                    if ((intFailMask & 0x18) > 0 || m_smVSInfo[i].g_intCenterPadBrokenFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadBroken", m_smVSInfo[i].g_intCenterPadBrokenFailureTotal);
                    if ((intFailMask & 0x800) > 0 || m_smVSInfo[i].g_intCenterPadExcessFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadExcess", m_smVSInfo[i].g_intCenterPadExcessFailureTotal);
                    if ((intFailMask & 0x2000) > 0 || m_smVSInfo[i].g_intCenterPadSmearFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadSmear", m_smVSInfo[i].g_intCenterPadSmearFailureTotal);
                    if ((intFailMask & 0x4000) > 0 || m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadEdgeLimit", m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal);
                    if ((intFailMask & 0x8000) > 0 || m_smVSInfo[i].g_intCenterPadStandOffFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadStandOff", m_smVSInfo[i].g_intCenterPadStandOffFailureTotal);
                    if ((intFailMask & 0x10000) > 0 || m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadEdgeDistance", m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal);
                    if ((intFailMask & 0x20000) > 0 || (intFailMask & 0x40000) > 0 || m_smVSInfo[i].g_intCenterPadSpanFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadSpan", m_smVSInfo[i].g_intCenterPadSpanFailureTotal);

                    if (m_smVSInfo[i].g_intCenterPadMissingFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadMissing", m_smVSInfo[i].g_intCenterPadMissingFailureTotal);

                    if (((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0) && m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal > 0)
                        objFile.WriteElement2Value("CenterPadColorDefect", m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal);

                    int intSideFailMask = m_smVSInfo[i].g_arrPad[1].ref_intFailOptionMask;
                    if ((intSideFailMask & 0x100) > 0 || m_smVSInfo[i].g_intSidePadOffsetFailureTotal > 0)
                        objFile.WriteElement2Value("SidePadOffset", m_smVSInfo[i].g_intSidePadOffsetFailureTotal);
                    if ((intSideFailMask & 0x20) > 0 || m_smVSInfo[i].g_intSidePadAreaFailureTotal > 0)
                        objFile.WriteElement2Value("SidePadArea", m_smVSInfo[i].g_intSidePadAreaFailureTotal);
                    if ((intSideFailMask & 0xC0) > 0 || m_smVSInfo[i].g_intSidePadDimensionFailureTotal > 0)
                        objFile.WriteElement2Value("SidePadDimension", m_smVSInfo[i].g_intSidePadDimensionFailureTotal);
                    if ((intSideFailMask & 0x600) > 0 || m_smVSInfo[i].g_intSidePadPitchGapFailureTotal > 0)
                        objFile.WriteElement2Value("SidePadPitchGap", m_smVSInfo[i].g_intSidePadPitchGapFailureTotal);
                    if ((intSideFailMask & 0x18) > 0 || m_smVSInfo[i].g_intSidePadBrokenFailureTotal > 0)
                        objFile.WriteElement2Value("SidePadBroken", m_smVSInfo[i].g_intSidePadBrokenFailureTotal);
                    if ((intSideFailMask & 0x800) > 0 || m_smVSInfo[i].g_intSidePadExcessFailureTotal > 0)
                        objFile.WriteElement2Value("SidePadExcess", m_smVSInfo[i].g_intSidePadExcessFailureTotal);
                    if ((intSideFailMask & 0x2000) > 0 || m_smVSInfo[i].g_intSidePadSmearFailureTotal > 0)
                        objFile.WriteElement2Value("SidePadSmear", m_smVSInfo[i].g_intSidePadSmearFailureTotal);
                    if ((intSideFailMask & 0x4000) > 0 || m_smVSInfo[i].g_intSidePadEdgeLimitFailureTotal > 0)
                        objFile.WriteElement2Value("SidePadEdgeLimit", m_smVSInfo[i].g_intSidePadEdgeLimitFailureTotal);
                    if ((intSideFailMask & 0x8000) > 0 || m_smVSInfo[i].g_intSidePadStandOffFailureTotal > 0)
                        objFile.WriteElement2Value("SidePadStandOff", m_smVSInfo[i].g_intSidePadStandOffFailureTotal);
                    if ((intSideFailMask & 0x10000) > 0 || m_smVSInfo[i].g_intSidePadEdgeDistanceFailureTotal > 0)
                        objFile.WriteElement2Value("SidePadEdgeDistance", m_smVSInfo[i].g_intSidePadEdgeDistanceFailureTotal);
                    if ((intSideFailMask & 0x20000) > 0 || (intSideFailMask & 0x40000) > 0 || m_smVSInfo[i].g_intSidePadSpanFailureTotal > 0)
                        objFile.WriteElement2Value("SidePadSpan", m_smVSInfo[i].g_intSidePadSpanFailureTotal);

                    if (m_smVSInfo[i].g_intSidePadMissingFailureTotal > 0)
                        objFile.WriteElement2Value("SidePadMissing", m_smVSInfo[i].g_intSidePadMissingFailureTotal);

                    if (((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0) && m_smVSInfo[i].g_intSidePadColorDefectFailureTotal > 0)
                        objFile.WriteElement2Value("SidePadColorDefect", m_smVSInfo[i].g_intSidePadColorDefectFailureTotal);

                    if (m_smVSInfo[i].g_intPositionFailureTotal > 0)
                        objFile.WriteElement2Value("Position", m_smVSInfo[i].g_intPositionFailureTotal);

                    if ((m_smCustomizeInfo.g_intWantPackage & intPos) > 0)
                    {
                        if (m_smVSInfo[i].g_arrPad[0].GetWantInspectPackage() || m_smVSInfo[i].g_intCenterPkgDefectFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPkgDefect", m_smVSInfo[i].g_intCenterPkgDefectFailureTotal);
                        if ((m_smVSInfo[i].g_arrPad[0].ref_intFailPkgOptionMask & 0x01) > 0 || m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPkgDimension", m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal);

                        if (m_smVSInfo[i].g_arrPad[1].GetWantInspectPackage() || m_smVSInfo[i].g_intSidePkgDefectFailureTotal > 0)
                            objFile.WriteElement2Value("SidePkgDefect", m_smVSInfo[i].g_intSidePkgDefectFailureTotal);
                        if ((m_smVSInfo[i].g_arrPad[1].ref_intFailPkgOptionMask & 0x01) > 0 || m_smVSInfo[i].g_intSidePkgDimensionFailureTotal > 0)
                            objFile.WriteElement2Value("SidePkgDimension", m_smVSInfo[i].g_intSidePkgDimensionFailureTotal);
                    }
                    else
                    {
                        if ((intFailMask & 0x1001) > 0 || m_smVSInfo[i].g_intCenterPadContaminationFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadContamination", m_smVSInfo[i].g_intCenterPadContaminationFailureTotal);

                        if ((intSideFailMask & 0x1001) > 0 || m_smVSInfo[i].g_intSidePadContaminationFailureTotal > 0)
                            objFile.WriteElement2Value("SidePadContamination", m_smVSInfo[i].g_intSidePadContaminationFailureTotal);
                    }

                    //objFile.WriteElement1Value("Statistic", "");
                }

                if ((m_smCustomizeInfo.g_intWantBottom & intPos) > 0)
                {
                    objFile.WriteElement2Value("Position", m_smVSInfo[i].g_intPositionFailureTotal);
                    objFile.WriteElement2Value("Angle", m_smVSInfo[i].g_intAngleFailureTotal);
                }

                if ((m_smCustomizeInfo.g_intWantSeal & intPos) > 0)
                {
                    objFile.WriteElement2Value("SealWidth", m_smVSInfo[i].g_intSealFailureTotal);
                    objFile.WriteElement2Value("SealDistance", m_smVSInfo[i].g_intSealDistanceFailureTotal);
                    objFile.WriteElement2Value("SealOverHeat", m_smVSInfo[i].g_intSealOverHeatFailureTotal);
                    objFile.WriteElement2Value("SealBrokenArea", m_smVSInfo[i].g_intSealBrokenAreaFailureTotal);
                    objFile.WriteElement2Value("SealBrokenGap", m_smVSInfo[i].g_intSealBrokenGapFailureTotal);
                    objFile.WriteElement2Value("SealSprocketHole", m_smVSInfo[i].g_intSealSprocketHoleFailureTotal);
                    objFile.WriteElement2Value("SealSprocketHoleDiameter", m_smVSInfo[i].g_intSealSprocketHoleDiameterFailureTotal);
                    objFile.WriteElement2Value("SealSprocketHoleDefect", m_smVSInfo[i].g_intSealSprocketHoleDefectFailureTotal);
                    objFile.WriteElement2Value("SealSprocketHoleBroken", m_smVSInfo[i].g_intSealSprocketHoleBrokenFailureTotal);
                    objFile.WriteElement2Value("SealEdgeStraightness", m_smVSInfo[i].g_intSealEdgeStraightnessFailureTotal);
                    objFile.WriteElement2Value("SealSprocketHoleRoundness", m_smVSInfo[i].g_intSealSprocketHoleRoundnessFailureTotal);
                    objFile.WriteElement2Value("Position", m_smVSInfo[i].g_intPositionFailureTotal);
                    objFile.WriteElement2Value("CheckPresence", m_smVSInfo[i].g_intCheckPresenceFailureTotal);
                    objFile.WriteElement2Value("Orient", m_smVSInfo[i].g_intOrientFailureTotal);
                }

                if ((m_smVSInfo[i].g_blnWantGauge) || m_smVSInfo[i].g_intEdgeNotFoundFailureTotal > 0)
                    objFile.WriteElement2Value("EdgeNotFound", m_smVSInfo[i].g_intEdgeNotFoundFailureTotal);

                if (m_smVSInfo[i].g_intNoTemplateFailureTotal > 0)
                    objFile.WriteElement2Value("NoTemplate", m_smVSInfo[i].g_intNoTemplateFailureTotal);

                intVisionCount++;
            }

            objFile.WriteEndElement();

        }
        private void SaveLotFile(int intVisionInvolved)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            string strLot = subkey1.GetValue("LotNoPrev", "SRM").ToString();
            string strLotStartTime = subkey1.GetValue("LotStartTimePrev", DateTime.Now.ToString("yyyyMMddHHmmss")).ToString();

            string strLotStopTime = subkey1.GetValue("LotStartTime", DateTime.Now.ToString("yyyyMMddHHmmss")).ToString();
            strLotStopTime = strLotStopTime.Substring(0, 4) + "/" + strLotStopTime.Substring(4, 2) + "/" +
                strLotStopTime.Substring(6, 2) + " " + strLotStopTime.Substring(8, 2) + ":" + strLotStopTime.Substring(10, 2) +
                ":" + strLotStopTime.Substring(12, 2);

            bool blnAllSingleLotIDSame = true;
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (((m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) == 0 && m_smProductionInfo.g_intVisionInvolved != 0) && m_smProductionInfo.g_arrVisionIDIndex[i] != 0)
                    continue;

                if (m_smProductionInfo.g_arrIsNewLot[i])
                    blnAllSingleLotIDSame = false;

                //m_smProductionInfo.g_arrIsNewLot[i] = false;
                //subkey1.SetValue("IsNewLot" + i.ToString(), m_smProductionInfo.g_arrIsNewLot[i]);
            }
            if (strLotStartTime.Contains("/"))
                strLotStartTime = strLotStartTime.Remove('/');
            if (strLotStartTime.Contains(":"))
                strLotStartTime = strLotStartTime.Remove(':');
            XmlParser objFile;
            string strLotPath = AppDomain.CurrentDomain.BaseDirectory + "LotReport\\" + strLot + "_" + strLotStartTime + ".xml";
            string strLotPathPrev = subkey1.GetValue("LotReportPathPrev", strLotPath).ToString();

            if (!blnAllSingleLotIDSame)
            {
                bool blnRecord = false;
                if (!File.Exists(strLotPathPrev))
                    blnRecord = true;

                objFile = new XmlParser(strLotPathPrev);
                if (blnRecord)
                {
                    strLotStartTime = strLotStartTime.Substring(0, 4) + "/" + strLotStartTime.Substring(4, 2) + "/" +
                   strLotStartTime.Substring(6, 2) + " " + strLotStartTime.Substring(8, 2) + ":" + strLotStartTime.Substring(10, 2) +
                   ":" + strLotStartTime.Substring(12, 2);

                    objFile.WriteSectionElement("Lot");
                    objFile.WriteElement1Value("LotID", strLot);
                    objFile.WriteElement1Value("OperatorID", subkey1.GetValue("OperatorIDPrev", "Op").ToString());
                    objFile.WriteElement1Value("RecipeID", subkey1.GetValue("RecipeIDPrev", "Default").ToString());
                    objFile.WriteElement1Value("LotStartTime", strLotStartTime);
                    objFile.WriteElement1Value("LotStopTime", strLotStopTime);
                    objFile.WriteElement1Value("MachineID", m_smCustomizeInfo.g_strMachineID);

                    subkey1.SetValue("LotReportPathPrev", strLotPathPrev);

                }
                else
                {
                    objFile.WriteSectionElement("Lot");
                    objFile.WriteElement1Value("LotStopTime", strLotStopTime);

                    bool blnAllNewLot = true;
                    for (int i = 0; i < m_smVSInfo.Length; i++)
                    {
                        if (m_smVSInfo[i] == null)
                            continue;

                        if (!m_smProductionInfo.g_arrIsNewLot[i])
                            blnAllNewLot = false;

                    }

                    if (blnAllNewLot && subkey1.GetValue("LotReportPathPrev", "----").ToString() != "----")
                        subkey1.DeleteValue("LotReportPathPrev");

                    if (blnAllNewLot)
                    {
                        for (int i = 0; i < m_smVSInfo.Length; i++)
                        {
                            if (m_smVSInfo[i] == null)
                                continue;

                            m_smProductionInfo.g_arrIsNewLot[i] = false;
                            subkey1.SetValue("IsNewLot" + i.ToString(), m_smProductionInfo.g_arrIsNewLot[i]);
                        }
                    }

                }

            }
            else
            {
                objFile = new XmlParser(strLotPath);

                strLotStartTime = strLotStartTime.Substring(0, 4) + "/" + strLotStartTime.Substring(4, 2) + "/" +
                strLotStartTime.Substring(6, 2) + " " + strLotStartTime.Substring(8, 2) + ":" + strLotStartTime.Substring(10, 2) +
                ":" + strLotStartTime.Substring(12, 2);

                objFile.WriteSectionElement("Lot");
                objFile.WriteElement1Value("LotID", strLot);
                objFile.WriteElement1Value("OperatorID", subkey1.GetValue("OperatorIDPrev", "Op").ToString());
                objFile.WriteElement1Value("RecipeID", subkey1.GetValue("RecipeIDPrev", "Default").ToString());
                objFile.WriteElement1Value("LotStartTime", strLotStartTime);
                objFile.WriteElement1Value("LotStopTime", strLotStopTime);
                objFile.WriteElement1Value("MachineID", m_smCustomizeInfo.g_strMachineID);

                if (subkey1.GetValue("LotReportPathPrev", "----").ToString() != "----")
                    subkey1.DeleteValue("LotReportPathPrev");

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    m_smProductionInfo.g_arrIsNewLot[i] = false;
                    subkey1.SetValue("IsNewLot" + i.ToString(), m_smProductionInfo.g_arrIsNewLot[i]);
                }
            }

            int intVisionCount = 0;
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (((m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) == 0 && m_smProductionInfo.g_intVisionInvolved != 0) && m_smProductionInfo.g_arrVisionIDIndex[i] != 0)
                    continue;

                if (m_smProductionInfo.g_intVisionInvolved == 0 && m_smProductionInfo.g_arrIsNewLot[i])
                {
                    m_smProductionInfo.g_arrIsNewLot[i] = false;
                    subkey1.SetValue("IsNewLot" + i.ToString(), m_smProductionInfo.g_arrIsNewLot[i]);
                    continue;
                }

                if (m_smVSInfo[i].g_strVisionName == "Barcode") // 2020-11-26 ZJYEOH : Barcode dont have counter
                    continue;

                int intPos = (1 << i);
                objFile.WriteSectionElement("VisionFeature" + i);//intVisionCount
                //if (m_smVSInfo[i].g_intVisionSameCount == 0)
                //    objFile.WriteElement1Value("VisionName", m_smVSInfo[i].g_strVisionName);
                //else
                //    objFile.WriteElement1Value("VisionName", m_smVSInfo[i].g_strVisionName + (m_smVSInfo[i].g_intVisionIndex + 1));
                //objFile.WriteElement1Value("VisionName", m_smVSInfo[i].g_strVisionName + m_smVSInfo[i].g_strVisionNameRemark);

                objFile.WriteElement1Value("VisionName", m_smVSInfo[i].g_strVisionName + m_smVSInfo[i].g_strVisionNameNo);

                if (((intVisionInvolved & (1 << i)) > 0))
                {
                    objFile.WriteElement1Value("Total", m_smVSInfo[i].g_intTestedTotal);
                    objFile.WriteElement1Value("Pass", m_smVSInfo[i].g_intPassTotal);
                    objFile.WriteElement1Value("Fail", "");
                    if ((m_smCustomizeInfo.g_intWantMark & intPos) > 0)
                        objFile.WriteElement2Value("Mark", m_smVSInfo[i].g_intMarkFailureTotal);
                    if ((m_smCustomizeInfo.g_intWantOrient & intPos) > 0)
                        objFile.WriteElement2Value("Orient", m_smVSInfo[i].g_intOrientFailureTotal);
                    if (m_smVSInfo[i].g_blnWantPin1)
                        objFile.WriteElement2Value("Pin1", m_smVSInfo[i].g_intPin1FailureTotal);
                    if ((m_smCustomizeInfo.g_intWantLead3D & intPos) > 0)
                    {
                        objFile.WriteElement2Value("Lead", m_smVSInfo[i].g_intLeadFailureTotal);

                        int intFailMask = m_smVSInfo[i].g_arrLead3D[0].ref_intFailOptionMask;

                        if ((intFailMask & 0x20000) > 0 || m_smVSInfo[i].g_intLeadOffsetFailureTotal > 0)
                            objFile.WriteElement2Value("LeadOffset", m_smVSInfo[i].g_intLeadOffsetFailureTotal);
                        if ((intFailMask & 0x100) > 0 || m_smVSInfo[i].g_intLeadSkewFailureTotal > 0)
                            objFile.WriteElement2Value("LeadSkew", m_smVSInfo[i].g_intLeadSkewFailureTotal);
                        if ((intFailMask & 0x40) > 0 || m_smVSInfo[i].g_intLeadWidthFailureTotal > 0)
                            objFile.WriteElement2Value("LeadWidth", m_smVSInfo[i].g_intLeadWidthFailureTotal);
                        if ((intFailMask & 0x80) > 0 || m_smVSInfo[i].g_intLeadLengthFailureTotal > 0)
                            objFile.WriteElement2Value("LeadLength", m_smVSInfo[i].g_intLeadLengthFailureTotal);
                        if ((intFailMask & 0x800) > 0 || m_smVSInfo[i].g_intLeadLengthVarianceFailureTotal > 0)
                            objFile.WriteElement2Value("LeadLengthVariance", m_smVSInfo[i].g_intLeadLengthVarianceFailureTotal);
                        if ((intFailMask & 0x200) > 0 || m_smVSInfo[i].g_intLeadPitchGapFailureTotal > 0)
                            objFile.WriteElement2Value("LeadPitchGap", m_smVSInfo[i].g_intLeadPitchGapFailureTotal);
                        if ((intFailMask & 0x2000) > 0 || m_smVSInfo[i].g_intLeadPitchVarianceFailureTotal > 0)
                            objFile.WriteElement2Value("LeadPitchVariance", m_smVSInfo[i].g_intLeadPitchVarianceFailureTotal);
                        if ((intFailMask & 0x01) > 0 || m_smVSInfo[i].g_intLeadStandOffFailureTotal > 0)
                            objFile.WriteElement2Value("LeadStandOff", m_smVSInfo[i].g_intLeadStandOffFailureTotal);
                        if ((intFailMask & 0x100) > 0 || m_smVSInfo[i].g_intLeadStandOffVarianceFailureTotal > 0)
                            objFile.WriteElement2Value("LeadStandOffVariance", m_smVSInfo[i].g_intLeadStandOffVarianceFailureTotal);
                        if ((intFailMask & 0x02) > 0 || m_smVSInfo[i].g_intLeadCoplanFailureTotal > 0)
                            objFile.WriteElement2Value("LeadCoplan", m_smVSInfo[i].g_intLeadCoplanFailureTotal);
                        if ((intFailMask & 0x40000) > 0 || m_smVSInfo[i].g_intLeadAGVFailureTotal > 0)
                            objFile.WriteElement2Value("LeadAGV", m_smVSInfo[i].g_intLeadAGVFailureTotal);
                        if ((intFailMask & 0x1000) > 0 || m_smVSInfo[i].g_intLeadSpanFailureTotal > 0)
                            objFile.WriteElement2Value("LeadSpan", m_smVSInfo[i].g_intLeadSpanFailureTotal);
                        if ((intFailMask & 0x04) > 0 || m_smVSInfo[i].g_intLeadSweepsFailureTotal > 0)
                            objFile.WriteElement2Value("LeadSweeps", m_smVSInfo[i].g_intLeadSweepsFailureTotal);
                        if ((intFailMask & 0x10) > 0 || m_smVSInfo[i].g_intLeadUnCutTiebarFailureTotal > 0)
                            objFile.WriteElement2Value("LeadUnCutTiebar", m_smVSInfo[i].g_intLeadUnCutTiebarFailureTotal);
                        if (m_smVSInfo[i].g_intLeadMissingFailureTotal > 0)
                            objFile.WriteElement2Value("LeadMissing", m_smVSInfo[i].g_intLeadMissingFailureTotal);
                        if ((((intFailMask & 0x8000) > 0) || ((intFailMask & 0x10000) > 0)) || m_smVSInfo[i].g_intLeadContaminationFailureTotal > 0)
                            objFile.WriteElement2Value("LeadContamination", m_smVSInfo[i].g_intLeadContaminationFailureTotal);

                        if ((m_smCustomizeInfo.g_intWantPackage & intPos) > 0)
                        {
                            if (m_smVSInfo[i].g_arrLead3D[0].GetWantInspectPackage() || m_smVSInfo[i].g_intLeadPkgDefectFailureTotal > 0)
                                objFile.WriteElement2Value("LeadPkgDefect", m_smVSInfo[i].g_intLeadPkgDefectFailureTotal);
                            if ((m_smVSInfo[i].g_arrLead3D[0].ref_intFailPkgOptionMask & 0x01) > 0 || m_smVSInfo[i].g_intLeadPkgDimensionFailureTotal > 0)
                                objFile.WriteElement2Value("LeadPkgDimension", m_smVSInfo[i].g_intLeadPkgDimensionFailureTotal);
                        }
                    }

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

                    if (((m_smCustomizeInfo.g_intWantPad & intPos) > 0))
                    {
                        objFile.WriteElement2Value("Pad", m_smVSInfo[i].g_intPadFailureTotal);

                        int intFailMask = m_smVSInfo[i].g_arrPad[0].ref_intFailOptionMask;
                        if ((intFailMask & 0x100) > 0 || m_smVSInfo[i].g_intCenterPadOffsetFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadOffset", m_smVSInfo[i].g_intCenterPadOffsetFailureTotal);
                        if ((intFailMask & 0x20) > 0 || m_smVSInfo[i].g_intCenterPadAreaFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadArea", m_smVSInfo[i].g_intCenterPadAreaFailureTotal);
                        if ((intFailMask & 0xC0) > 0 || m_smVSInfo[i].g_intCenterPadDimensionFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadDimension", m_smVSInfo[i].g_intCenterPadDimensionFailureTotal);
                        if ((intFailMask & 0x600) > 0 || m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadPitchGap", m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal);
                        if ((intFailMask & 0x18) > 0 || m_smVSInfo[i].g_intCenterPadBrokenFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadBroken", m_smVSInfo[i].g_intCenterPadBrokenFailureTotal);
                        if ((intFailMask & 0x800) > 0 || m_smVSInfo[i].g_intCenterPadExcessFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadExcess", m_smVSInfo[i].g_intCenterPadExcessFailureTotal);
                        if ((intFailMask & 0x2000) > 0 || m_smVSInfo[i].g_intCenterPadSmearFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadSmear", m_smVSInfo[i].g_intCenterPadSmearFailureTotal);
                        if ((intFailMask & 0x4000) > 0 || m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadEdgeLimit", m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal);
                        if ((intFailMask & 0x8000) > 0 || m_smVSInfo[i].g_intCenterPadStandOffFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadStandOff", m_smVSInfo[i].g_intCenterPadStandOffFailureTotal);
                        if ((intFailMask & 0x10000) > 0 || m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadEdgeDistance", m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal);
                        if ((intFailMask & 0x20000) > 0 || (intFailMask & 0x40000) > 0 || m_smVSInfo[i].g_intCenterPadSpanFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadSpan", m_smVSInfo[i].g_intCenterPadSpanFailureTotal);

                        if (m_smVSInfo[i].g_intCenterPadMissingFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadMissing", m_smVSInfo[i].g_intCenterPadMissingFailureTotal);

                        if (((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0) && m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadColorDefect", m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal);

                        if (m_smVSInfo[i].g_intPositionFailureTotal > 0)
                            objFile.WriteElement2Value("Position", m_smVSInfo[i].g_intPositionFailureTotal);

                        if ((m_smCustomizeInfo.g_intWantPackage & intPos) > 0)
                        {
                            if (m_smVSInfo[i].g_arrPad[0].GetWantInspectPackage() || m_smVSInfo[i].g_intCenterPkgDefectFailureTotal > 0)
                                objFile.WriteElement2Value("CenterPkgDefect", m_smVSInfo[i].g_intCenterPkgDefectFailureTotal);
                            if ((m_smVSInfo[i].g_arrPad[0].ref_intFailPkgOptionMask & 0x01) > 0 || m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal > 0)
                                objFile.WriteElement2Value("CenterPkgDimension", m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal);
                        }
                        else
                        {
                            if ((intFailMask & 0x1001) > 0 || m_smVSInfo[i].g_intCenterPadContaminationFailureTotal > 0)
                                objFile.WriteElement2Value("CenterPadContamination", m_smVSInfo[i].g_intCenterPadContaminationFailureTotal);
                        }

                        //objFile.WriteElement1Value("Statistic", "");
                    }

                    if (((m_smCustomizeInfo.g_intWantPad5S & intPos) > 0))
                    {
                        objFile.WriteElement2Value("Pad", m_smVSInfo[i].g_intPadFailureTotal);

                        int intFailMask = m_smVSInfo[i].g_arrPad[0].ref_intFailOptionMask;
                        if ((intFailMask & 0x100) > 0 || m_smVSInfo[i].g_intCenterPadOffsetFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadOffset", m_smVSInfo[i].g_intCenterPadOffsetFailureTotal);
                        if ((intFailMask & 0x20) > 0 || m_smVSInfo[i].g_intCenterPadAreaFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadArea", m_smVSInfo[i].g_intCenterPadAreaFailureTotal);
                        if ((intFailMask & 0xC0) > 0 || m_smVSInfo[i].g_intCenterPadDimensionFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadDimension", m_smVSInfo[i].g_intCenterPadDimensionFailureTotal);
                        if ((intFailMask & 0x600) > 0 || m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadPitchGap", m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal);
                        if ((intFailMask & 0x18) > 0 || m_smVSInfo[i].g_intCenterPadBrokenFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadBroken", m_smVSInfo[i].g_intCenterPadBrokenFailureTotal);
                        if ((intFailMask & 0x800) > 0 || m_smVSInfo[i].g_intCenterPadExcessFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadExcess", m_smVSInfo[i].g_intCenterPadExcessFailureTotal);
                        if ((intFailMask & 0x2000) > 0 || m_smVSInfo[i].g_intCenterPadSmearFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadSmear", m_smVSInfo[i].g_intCenterPadSmearFailureTotal);
                        if ((intFailMask & 0x4000) > 0 || m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadEdgeLimit", m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal);
                        if ((intFailMask & 0x8000) > 0 || m_smVSInfo[i].g_intCenterPadStandOffFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadStandOff", m_smVSInfo[i].g_intCenterPadStandOffFailureTotal);
                        if ((intFailMask & 0x10000) > 0 || m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadEdgeDistance", m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal);
                        if ((intFailMask & 0x20000) > 0 || (intFailMask & 0x40000) > 0 || m_smVSInfo[i].g_intCenterPadSpanFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadSpan", m_smVSInfo[i].g_intCenterPadSpanFailureTotal);

                        if (m_smVSInfo[i].g_intCenterPadMissingFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadMissing", m_smVSInfo[i].g_intCenterPadMissingFailureTotal);

                        if (((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0) && m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal > 0)
                            objFile.WriteElement2Value("CenterPadColorDefect", m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal);

                        int intSideFailMask = m_smVSInfo[i].g_arrPad[1].ref_intFailOptionMask;
                        if ((intSideFailMask & 0x100) > 0 || m_smVSInfo[i].g_intSidePadOffsetFailureTotal > 0)
                            objFile.WriteElement2Value("SidePadOffset", m_smVSInfo[i].g_intSidePadOffsetFailureTotal);
                        if ((intSideFailMask & 0x20) > 0 || m_smVSInfo[i].g_intSidePadAreaFailureTotal > 0)
                            objFile.WriteElement2Value("SidePadArea", m_smVSInfo[i].g_intSidePadAreaFailureTotal);
                        if ((intSideFailMask & 0xC0) > 0 || m_smVSInfo[i].g_intSidePadDimensionFailureTotal > 0)
                            objFile.WriteElement2Value("SidePadDimension", m_smVSInfo[i].g_intSidePadDimensionFailureTotal);
                        if ((intSideFailMask & 0x600) > 0 || m_smVSInfo[i].g_intSidePadPitchGapFailureTotal > 0)
                            objFile.WriteElement2Value("SidePadPitchGap", m_smVSInfo[i].g_intSidePadPitchGapFailureTotal);
                        if ((intSideFailMask & 0x18) > 0 || m_smVSInfo[i].g_intSidePadBrokenFailureTotal > 0)
                            objFile.WriteElement2Value("SidePadBroken", m_smVSInfo[i].g_intSidePadBrokenFailureTotal);
                        if ((intSideFailMask & 0x800) > 0 || m_smVSInfo[i].g_intSidePadExcessFailureTotal > 0)
                            objFile.WriteElement2Value("SidePadExcess", m_smVSInfo[i].g_intSidePadExcessFailureTotal);
                        if ((intSideFailMask & 0x2000) > 0 || m_smVSInfo[i].g_intSidePadSmearFailureTotal > 0)
                            objFile.WriteElement2Value("SidePadSmear", m_smVSInfo[i].g_intSidePadSmearFailureTotal);
                        if ((intSideFailMask & 0x4000) > 0 || m_smVSInfo[i].g_intSidePadEdgeLimitFailureTotal > 0)
                            objFile.WriteElement2Value("SidePadEdgeLimit", m_smVSInfo[i].g_intSidePadEdgeLimitFailureTotal);
                        if ((intSideFailMask & 0x8000) > 0 || m_smVSInfo[i].g_intSidePadStandOffFailureTotal > 0)
                            objFile.WriteElement2Value("SidePadStandOff", m_smVSInfo[i].g_intSidePadStandOffFailureTotal);
                        if ((intSideFailMask & 0x10000) > 0 || m_smVSInfo[i].g_intSidePadEdgeDistanceFailureTotal > 0)
                            objFile.WriteElement2Value("SidePadEdgeDistance", m_smVSInfo[i].g_intSidePadEdgeDistanceFailureTotal);
                        if ((intSideFailMask & 0x20000) > 0 || (intSideFailMask & 0x40000) > 0 || m_smVSInfo[i].g_intSidePadSpanFailureTotal > 0)
                            objFile.WriteElement2Value("SidePadSpan", m_smVSInfo[i].g_intSidePadSpanFailureTotal);

                        if (m_smVSInfo[i].g_intSidePadMissingFailureTotal > 0)
                            objFile.WriteElement2Value("SidePadMissing", m_smVSInfo[i].g_intSidePadMissingFailureTotal);

                        if (((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0) && m_smVSInfo[i].g_intSidePadColorDefectFailureTotal > 0)
                            objFile.WriteElement2Value("SidePadColorDefect", m_smVSInfo[i].g_intSidePadColorDefectFailureTotal);

                        if (m_smVSInfo[i].g_intPositionFailureTotal > 0)
                            objFile.WriteElement2Value("Position", m_smVSInfo[i].g_intPositionFailureTotal);

                        if ((m_smCustomizeInfo.g_intWantPackage & intPos) > 0)
                        {
                            if (m_smVSInfo[i].g_arrPad[0].GetWantInspectPackage() || m_smVSInfo[i].g_intCenterPkgDefectFailureTotal > 0)
                                objFile.WriteElement2Value("CenterPkgDefect", m_smVSInfo[i].g_intCenterPkgDefectFailureTotal);
                            if ((m_smVSInfo[i].g_arrPad[0].ref_intFailPkgOptionMask & 0x01) > 0 || m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal > 0)
                                objFile.WriteElement2Value("CenterPkgDimension", m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal);

                            if (m_smVSInfo[i].g_arrPad[1].GetWantInspectPackage() || m_smVSInfo[i].g_intSidePkgDefectFailureTotal > 0)
                                objFile.WriteElement2Value("SidePkgDefect", m_smVSInfo[i].g_intSidePkgDefectFailureTotal);
                            if ((m_smVSInfo[i].g_arrPad[1].ref_intFailPkgOptionMask & 0x01) > 0 || m_smVSInfo[i].g_intSidePkgDimensionFailureTotal > 0)
                                objFile.WriteElement2Value("SidePkgDimension", m_smVSInfo[i].g_intSidePkgDimensionFailureTotal);
                        }
                        else
                        {
                            if ((intFailMask & 0x1001) > 0 || m_smVSInfo[i].g_intCenterPadContaminationFailureTotal > 0)
                                objFile.WriteElement2Value("CenterPadContamination", m_smVSInfo[i].g_intCenterPadContaminationFailureTotal);

                            if ((intSideFailMask & 0x1001) > 0 || m_smVSInfo[i].g_intSidePadContaminationFailureTotal > 0)
                                objFile.WriteElement2Value("SidePadContamination", m_smVSInfo[i].g_intSidePadContaminationFailureTotal);
                        }

                        //objFile.WriteElement1Value("Statistic", "");
                    }

                    if ((m_smCustomizeInfo.g_intWantBottom & intPos) > 0)
                    {
                        objFile.WriteElement2Value("Position", m_smVSInfo[i].g_intPositionFailureTotal);
                        objFile.WriteElement2Value("Angle", m_smVSInfo[i].g_intAngleFailureTotal);
                    }

                    if ((m_smCustomizeInfo.g_intWantSeal & intPos) > 0)
                    {
                        objFile.WriteElement2Value("SealWidth", m_smVSInfo[i].g_intSealFailureTotal);
                        objFile.WriteElement2Value("SealDistance", m_smVSInfo[i].g_intSealDistanceFailureTotal);
                        objFile.WriteElement2Value("SealOverHeat", m_smVSInfo[i].g_intSealOverHeatFailureTotal);
                        objFile.WriteElement2Value("SealBrokenArea", m_smVSInfo[i].g_intSealBrokenAreaFailureTotal);
                        objFile.WriteElement2Value("SealBrokenGap", m_smVSInfo[i].g_intSealBrokenGapFailureTotal);
                        objFile.WriteElement2Value("SealSprocketHole", m_smVSInfo[i].g_intSealSprocketHoleFailureTotal);
                        objFile.WriteElement2Value("SealSprocketHoleDiameter", m_smVSInfo[i].g_intSealSprocketHoleDiameterFailureTotal);
                        objFile.WriteElement2Value("SealSprocketHoleDefect", m_smVSInfo[i].g_intSealSprocketHoleDefectFailureTotal);
                        objFile.WriteElement2Value("SealSprocketHoleBroken", m_smVSInfo[i].g_intSealSprocketHoleBrokenFailureTotal);
                        objFile.WriteElement2Value("SealEdgeStraightness", m_smVSInfo[i].g_intSealEdgeStraightnessFailureTotal);
                        objFile.WriteElement2Value("SealSprocketHoleRoundness", m_smVSInfo[i].g_intSealSprocketHoleRoundnessFailureTotal);
                        objFile.WriteElement2Value("Position", m_smVSInfo[i].g_intPositionFailureTotal);
                        objFile.WriteElement2Value("CheckPresence", m_smVSInfo[i].g_intCheckPresenceFailureTotal);
                        objFile.WriteElement2Value("Orient", m_smVSInfo[i].g_intOrientFailureTotal);
                    }

                    if ((m_smVSInfo[i].g_blnWantGauge) || m_smVSInfo[i].g_intEdgeNotFoundFailureTotal > 0)
                        objFile.WriteElement2Value("EdgeNotFound", m_smVSInfo[i].g_intEdgeNotFoundFailureTotal);

                    if (m_smVSInfo[i].g_intNoTemplateFailureTotal > 0)
                        objFile.WriteElement2Value("NoTemplate", m_smVSInfo[i].g_intNoTemplateFailureTotal);
                }
                else
                {
                    objFile.WriteElement1Value("Total", 0);
                    objFile.WriteElement1Value("Pass", 0);
                    objFile.WriteElement1Value("Fail", "");
                    if ((m_smCustomizeInfo.g_intWantMark & intPos) > 0)
                        objFile.WriteElement2Value("Mark", 0);
                    if ((m_smCustomizeInfo.g_intWantOrient & intPos) > 0)
                        objFile.WriteElement2Value("Orient", 0);
                    if (m_smVSInfo[i].g_blnWantPin1)
                        objFile.WriteElement2Value("Pin1", 0);
                    if ((m_smCustomizeInfo.g_intWantLead3D & intPos) > 0)
                    {
                        objFile.WriteElement2Value("Lead", 0);

                        int intFailMask = m_smVSInfo[i].g_arrLead3D[0].ref_intFailOptionMask;

                        if ((intFailMask & 0x20000) > 0)
                            objFile.WriteElement2Value("LeadOffset", 0);
                        if ((intFailMask & 0x100) > 0)
                            objFile.WriteElement2Value("LeadSkew", 0);
                        if ((intFailMask & 0x40) > 0)
                            objFile.WriteElement2Value("LeadWidth", 0);
                        if ((intFailMask & 0x80) > 0)
                            objFile.WriteElement2Value("LeadLength", 0);
                        if ((intFailMask & 0x800) > 0)
                            objFile.WriteElement2Value("LeadLengthVariance", 0);
                        if ((intFailMask & 0x200) > 0)
                            objFile.WriteElement2Value("LeadPitchGap", 0);
                        if ((intFailMask & 0x2000) > 0)
                            objFile.WriteElement2Value("LeadPitchVariance", 0);
                        if ((intFailMask & 0x01) > 0)
                            objFile.WriteElement2Value("LeadStandOff", 0);
                        if ((intFailMask & 0x100) > 0)
                            objFile.WriteElement2Value("LeadStandOffVariance", 0);
                        if ((intFailMask & 0x02) > 0)
                            objFile.WriteElement2Value("LeadCoplan", 0);
                        if ((intFailMask & 0x40000) > 0)
                            objFile.WriteElement2Value("LeadAGV", 0);
                        if ((intFailMask & 0x1000) > 0)
                            objFile.WriteElement2Value("LeadSpan", 0);
                        if ((intFailMask & 0x04) > 0)
                            objFile.WriteElement2Value("LeadSweeps", 0);
                        if ((intFailMask & 0x10) > 0)
                            objFile.WriteElement2Value("LeadUnCutTiebar", 0);

                        objFile.WriteElement2Value("LeadMissing", 0);
                        if ((((intFailMask & 0x8000) > 0) || ((intFailMask & 0x10000) > 0)))
                            objFile.WriteElement2Value("LeadContamination", 0);

                        if ((m_smCustomizeInfo.g_intWantPackage & intPos) > 0)
                        {
                            if (m_smVSInfo[i].g_arrLead3D[0].GetWantInspectPackage())
                                objFile.WriteElement2Value("LeadPkgDefect", 0);
                            if ((m_smVSInfo[i].g_arrLead3D[0].ref_intFailPkgOptionMask & 0x01) > 0)
                                objFile.WriteElement2Value("LeadPkgDimension", 0);
                        }
                    }

                    if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                        objFile.WriteElement2Value("Lead", 0);

                    if ((m_smCustomizeInfo.g_intWantPackage & intPos) > 0 && ((m_smCustomizeInfo.g_intWantPad & intPos) == 0) && ((m_smCustomizeInfo.g_intWantPad5S & intPos) == 0))
                    {
                        objFile.WriteElement2Value("Package", 0);
                        objFile.WriteElement2Value("PackageDefect", 0);
                        if (m_smVSInfo[i].g_blnViewColorImage)
                        {
                            objFile.WriteElement2Value("CheckPresence", 0);
                            objFile.WriteElement2Value("Orient", 0);
                        }
                    }

                    if (((m_smCustomizeInfo.g_intWantPositioning & intPos) > 0) && ((m_smCustomizeInfo.g_intWantPad & intPos) == 0))
                        objFile.WriteElement2Value("Positioning", 0);

                    if (((m_smCustomizeInfo.g_intWantPad & intPos) > 0))
                    {
                        objFile.WriteElement2Value("Pad", 0);

                        int intFailMask = m_smVSInfo[i].g_arrPad[0].ref_intFailOptionMask;
                        if ((intFailMask & 0x100) > 0)
                            objFile.WriteElement2Value("CenterPadOffset", 0);
                        if ((intFailMask & 0x20) > 0)
                            objFile.WriteElement2Value("CenterPadArea", 0);
                        if ((intFailMask & 0xC0) > 0)
                            objFile.WriteElement2Value("CenterPadDimension", 0);
                        if ((intFailMask & 0x600) > 0)
                            objFile.WriteElement2Value("CenterPadPitchGap", 0);
                        if ((intFailMask & 0x18) > 0)
                            objFile.WriteElement2Value("CenterPadBroken", 0);
                        if ((intFailMask & 0x800) > 0)
                            objFile.WriteElement2Value("CenterPadExcess", 0);
                        if ((intFailMask & 0x2000) > 0)
                            objFile.WriteElement2Value("CenterPadSmear", 0);
                        if ((intFailMask & 0x4000) > 0)
                            objFile.WriteElement2Value("CenterPadEdgeLimit", 0);
                        if ((intFailMask & 0x8000) > 0)
                            objFile.WriteElement2Value("CenterPadStandOff", 0);
                        if ((intFailMask & 0x10000) > 0)
                            objFile.WriteElement2Value("CenterPadEdgeDistance", 0);
                        if ((intFailMask & 0x20000) > 0)
                            objFile.WriteElement2Value("CenterPadSpanX", 0);
                        if ((intFailMask & 0x40000) > 0)
                            objFile.WriteElement2Value("CenterPadSpanY", 0);

                        objFile.WriteElement2Value("CenterPadMissing", 0);

                        if (((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0))
                            objFile.WriteElement2Value("CenterPadColorDefect", 0);

                        objFile.WriteElement2Value("Position", 0);

                        if ((m_smCustomizeInfo.g_intWantPackage & intPos) > 0)
                        {
                            if (m_smVSInfo[i].g_arrPad[0].GetWantInspectPackage())
                                objFile.WriteElement2Value("CenterPkgDefect", 0);
                            if ((m_smVSInfo[i].g_arrPad[0].ref_intFailPkgOptionMask & 0x01) > 0)
                                objFile.WriteElement2Value("CenterPkgDimension", 0);
                        }
                        else
                        {
                            if ((intFailMask & 0x1001) > 0)
                                objFile.WriteElement2Value("CenterPadContamination", 0);
                        }

                        //objFile.WriteElement1Value("Statistic", "");
                    }

                    if (((m_smCustomizeInfo.g_intWantPad5S & intPos) > 0))
                    {
                        objFile.WriteElement2Value("Pad", 0);

                        int intFailMask = m_smVSInfo[i].g_arrPad[0].ref_intFailOptionMask;
                        if ((intFailMask & 0x100) > 0)
                            objFile.WriteElement2Value("CenterPadOffset", 0);
                        if ((intFailMask & 0x20) > 0)
                            objFile.WriteElement2Value("CenterPadArea", 0);
                        if ((intFailMask & 0xC0) > 0)
                            objFile.WriteElement2Value("CenterPadDimension", 0);
                        if ((intFailMask & 0x600) > 0)
                            objFile.WriteElement2Value("CenterPadPitchGap", 0);
                        if ((intFailMask & 0x18) > 0)
                            objFile.WriteElement2Value("CenterPadBroken", 0);
                        if ((intFailMask & 0x800) > 0)
                            objFile.WriteElement2Value("CenterPadExcess", 0);
                        if ((intFailMask & 0x2000) > 0)
                            objFile.WriteElement2Value("CenterPadSmear", 0);
                        if ((intFailMask & 0x4000) > 0)
                            objFile.WriteElement2Value("CenterPadEdgeLimit", 0);
                        if ((intFailMask & 0x8000) > 0)
                            objFile.WriteElement2Value("CenterPadStandOff", 0);
                        if ((intFailMask & 0x10000) > 0)
                            objFile.WriteElement2Value("CenterPadEdgeDistance", 0);
                        if ((intFailMask & 0x20000) > 0)
                            objFile.WriteElement2Value("CenterPadSpanX", 0);
                        if ((intFailMask & 0x40000) > 0)
                            objFile.WriteElement2Value("CenterPadSpanY", 0);

                        objFile.WriteElement2Value("CenterPadMissing", m_smVSInfo[i].g_intCenterPadMissingFailureTotal);

                        if (((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0))
                            objFile.WriteElement2Value("CenterPadColorDefect", 0);

                        int intSideFailMask = m_smVSInfo[i].g_arrPad[1].ref_intFailOptionMask;
                        if ((intSideFailMask & 0x100) > 0)
                            objFile.WriteElement2Value("SidePadOffset", 0);
                        if ((intSideFailMask & 0x20) > 0)
                            objFile.WriteElement2Value("SidePadArea", 0);
                        if ((intSideFailMask & 0xC0) > 0)
                            objFile.WriteElement2Value("SidePadDimension", 0);
                        if ((intSideFailMask & 0x600) > 0)
                            objFile.WriteElement2Value("SidePadPitchGap", 0);
                        if ((intSideFailMask & 0x18) > 0)
                            objFile.WriteElement2Value("SidePadBroken", 0);
                        if ((intSideFailMask & 0x800) > 0)
                            objFile.WriteElement2Value("SidePadExcess", 0);
                        if ((intSideFailMask & 0x2000) > 0)
                            objFile.WriteElement2Value("SidePadSmear", 0);
                        if ((intSideFailMask & 0x4000) > 0)
                            objFile.WriteElement2Value("SidePadEdgeLimit", 0);
                        if ((intSideFailMask & 0x8000) > 0)
                            objFile.WriteElement2Value("SidePadStandOff", 0);
                        if ((intSideFailMask & 0x10000) > 0)
                            objFile.WriteElement2Value("SidePadEdgeDistance", 0);
                        if ((intSideFailMask & 0x20000) > 0)
                            objFile.WriteElement2Value("SidePadSpanX", 0);
                        if ((intSideFailMask & 0x40000) > 0)
                            objFile.WriteElement2Value("SidePadSpanY", 0);

                        objFile.WriteElement2Value("SidePadMissing", 0);

                        if (((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0))
                            objFile.WriteElement2Value("SidePadColorDefect", 0);

                        objFile.WriteElement2Value("Position", 0);

                        if ((m_smCustomizeInfo.g_intWantPackage & intPos) > 0)
                        {
                            if (m_smVSInfo[i].g_arrPad[0].GetWantInspectPackage())
                                objFile.WriteElement2Value("CenterPkgDefect", 0);
                            if ((m_smVSInfo[i].g_arrPad[0].ref_intFailPkgOptionMask & 0x01) > 0)
                                objFile.WriteElement2Value("CenterPkgDimension", 0);

                            if (m_smVSInfo[i].g_arrPad[1].GetWantInspectPackage())
                                objFile.WriteElement2Value("SidePkgDefect", 0);
                            if ((m_smVSInfo[i].g_arrPad[1].ref_intFailPkgOptionMask & 0x01) > 0)
                                objFile.WriteElement2Value("SidePkgDimension", 0);
                        }
                        else
                        {
                            if ((intFailMask & 0x1001) > 0)
                                objFile.WriteElement2Value("CenterPadContamination", 0);

                            if ((intSideFailMask & 0x1001) > 0)
                                objFile.WriteElement2Value("SidePadContamination", 0);
                        }

                        //objFile.WriteElement1Value("Statistic", "");
                    }

                    if ((m_smCustomizeInfo.g_intWantBottom & intPos) > 0)
                    {
                        objFile.WriteElement2Value("Position", 0);
                        objFile.WriteElement2Value("Angle", 0);
                    }

                    if ((m_smCustomizeInfo.g_intWantSeal & intPos) > 0)
                    {
                        objFile.WriteElement2Value("SealWidth", 0);
                        objFile.WriteElement2Value("SealDistance", 0);
                        objFile.WriteElement2Value("SealOverHeat", 0);
                        objFile.WriteElement2Value("SealBrokenArea", 0);
                        objFile.WriteElement2Value("SealBrokenGap", 0);
                        objFile.WriteElement2Value("SealSprocketHole", 0);
                        objFile.WriteElement2Value("SealSprocketHoleDiameter", 0);
                        objFile.WriteElement2Value("SealSprocketHoleDefect", 0);
                        objFile.WriteElement2Value("SealSprocketHoleBroken", 0);
                        objFile.WriteElement2Value("SealSprocketHoleRoundness", 0);
                        objFile.WriteElement2Value("Position", 0);
                        objFile.WriteElement2Value("CheckPresence", 0);
                        objFile.WriteElement2Value("Orient", 0);
                    }

                    if ((m_smVSInfo[i].g_blnWantGauge))
                        objFile.WriteElement2Value("EdgeNotFound", 0);

                    if (m_smVSInfo[i].g_intNoTemplateFailureTotal > 0)
                        objFile.WriteElement2Value("NoTemplate", 0);
                }
                intVisionCount++;
            }

            objFile.WriteEndElement();

        }

        private void SaveReportToServer()
        {
            if (!m_smCustomizeInfo.g_blnConfigShowNetwork || !m_smCustomizeInfo.g_blnWantNetwork || !Directory.Exists(m_smCustomizeInfo.g_strVisionLotReportUploadDir))
                return;

            m_report = new ReportDocument();
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            string strLot = subkey1.GetValue("LotNoPrev", "SRM").ToString();
            string strLotStartTime = subkey1.GetValue("LotStartTimePrev", DateTime.Now.ToString("yyyyMMddHHmmss")).ToString();
            string strSelectedFile = AppDomain.CurrentDomain.BaseDirectory + "LotReport\\" + strLot + "_" + strLotStartTime + ".xml";

            string strLotPathPrev = subkey1.GetValue("LotReportPathPrev", strSelectedFile).ToString();
            if (File.Exists(strLotPathPrev))
                strSelectedFile = strLotPathPrev;

            if (File.Exists(strSelectedFile))
            {
                XmlParser objFile = new XmlParser(strSelectedFile);
                int intVisionNo = objFile.GetFirstSectionCount() - 1;

                if (intVisionNo != m_intVisionCount) // 2021-03-20 ZJYEOH : If count not tally meaning still not fully end lot
                    return;

                DataTable dtVision = new DataTable("Vision");
                dtVision.Columns.Add("VisionName");
                dtVision.Columns.Add("Total");
                dtVision.Columns.Add("Pass");
                dtVision.Columns.Add("Fail");
                dtVision.Columns.Add("Yield");

                DataTable dtFail = new DataTable("Fail");
                dtFail.Columns.Add("VisionName");
                dtFail.Columns.Add("Feature");
                dtFail.Columns.Add("Count");

                for (int i = 0; i < intVisionNo; i++)
                {
                    objFile.GetFirstSection("VisionFeature" + i);

                    string strName = objFile.GetValueAsString("VisionName", "");
                    int intTotal = objFile.GetValueAsInt("Total", 0);
                    int intPass = objFile.GetValueAsInt("Pass", 0);
                    int intFail = intTotal - intPass;
                    float fYield = 0;
                    if (intTotal > 0)
                        fYield = (intPass / (float)intTotal) * 100;

                    DataRow dr = dtVision.NewRow();
                    dr["VisionName"] = strName;
                    dr["Total"] = intTotal.ToString();
                    dr["Pass"] = intPass.ToString();
                    dr["Fail"] = intFail.ToString();
                    dr["Yield"] = fYield.ToString("f2");
                    dtVision.Rows.Add(dr);

                    objFile.GetSecondSection("Fail");
                    int intChildCount = objFile.GetThirdSectionCount();
                    for (int j = 0; j < intChildCount; j++)
                    {
                        string strNode = objFile.GetThirdSectionElement("Fail", j);
                        dr = dtFail.NewRow();
                        dr["VisionName"] = strName;
                        dr["Feature"] = strNode;
                        dr["Count"] = objFile.GetValueAsString(strNode, "0", 2);
                        dtFail.Rows.Add(dr);
                    }
                }

                m_report.Load(AppDomain.CurrentDomain.BaseDirectory + "\\Report\\LotlyReport.rpt");
                m_report.Database.Tables["Vision"].SetDataSource(dtVision);
                m_report.Database.Tables["FailDetail"].SetDataSource(dtFail);

                objFile.GetFirstSection("Lot");
                m_report.SetParameterValue("MachineID", objFile.GetValueAsString("MachineID", "SRM"));
                m_report.SetParameterValue("LotID", objFile.GetValueAsString("LotID", "SRM"));
                m_report.SetParameterValue("LotStartTime", objFile.GetValueAsString("LotStartTime", DateTime.Now.ToString()));
                m_report.SetParameterValue("LotStopTime", objFile.GetValueAsString("LotStopTime", DateTime.Now.ToString()));
                m_report.SetParameterValue("OperatorID", objFile.GetValueAsString("OperatorID", "Op"));
                m_report.SetParameterValue("RecipeID", objFile.GetValueAsString("RecipeID", "Default"));

                m_report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, m_smCustomizeInfo.g_strVisionLotReportUploadDir + "\\" + strLot + "_" + DateTime.Now.ToString("yyyyMMddHHmmss").ToString() + ".pdf");
            }
            else
            {
                SRMMessageBox.Show("Save current lot report path does not exist. Fail to save current lot report.",
                                    "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void SaveReportToHandler()
        {
            if (!m_smCustomizeInfo.g_blnConfigShowTCPIC || !m_smCustomizeInfo.g_blnWantExportReport || !Directory.Exists(m_smCustomizeInfo.g_strExportReportDir))
                return;

            m_report = new ReportDocument();
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            string strLot = subkey1.GetValue("LotNoPrev", "SRM").ToString();
            string strLotStartTime = subkey1.GetValue("LotStartTimePrev", DateTime.Now.ToString("yyyyMMddHHmmss")).ToString();
            string strSelectedFile = AppDomain.CurrentDomain.BaseDirectory + "LotReport\\" + strLot + "_" + strLotStartTime + ".xml";

            string strLotPathPrev = subkey1.GetValue("LotReportPathPrev", strSelectedFile).ToString();
            if (File.Exists(strLotPathPrev))
                strSelectedFile = strLotPathPrev;

            if (File.Exists(strSelectedFile))
            {
                XmlParser objFile = new XmlParser(strSelectedFile);
                int intVisionNo = objFile.GetFirstSectionCount() - 1;

                if (intVisionNo != m_intVisionCount) // 2021-03-20 ZJYEOH : If count not tally meaning still not fully end lot
                    return;

                DataTable dtVision = new DataTable("Vision");
                dtVision.Columns.Add("VisionName");
                dtVision.Columns.Add("Total");
                dtVision.Columns.Add("Pass");
                dtVision.Columns.Add("Fail");
                dtVision.Columns.Add("Yield");

                DataTable dtFail = new DataTable("Fail");
                dtFail.Columns.Add("VisionName");
                dtFail.Columns.Add("Feature");
                dtFail.Columns.Add("Count");

                for (int i = 0; i < intVisionNo; i++)
                {
                    objFile.GetFirstSection("VisionFeature" + i);

                    string strName = objFile.GetValueAsString("VisionName", "");
                    int intTotal = objFile.GetValueAsInt("Total", 0);
                    int intPass = objFile.GetValueAsInt("Pass", 0);
                    int intFail = intTotal - intPass;
                    float fYield = 0;
                    if (intTotal > 0)
                        fYield = (intPass / (float)intTotal) * 100;

                    DataRow dr = dtVision.NewRow();
                    dr["VisionName"] = strName;
                    dr["Total"] = intTotal.ToString();
                    dr["Pass"] = intPass.ToString();
                    dr["Fail"] = intFail.ToString();
                    dr["Yield"] = fYield.ToString("f2");
                    dtVision.Rows.Add(dr);

                    objFile.GetSecondSection("Fail");
                    int intChildCount = objFile.GetThirdSectionCount();
                    for (int j = 0; j < intChildCount; j++)
                    {
                        string strNode = objFile.GetThirdSectionElement("Fail", j);
                        dr = dtFail.NewRow();
                        dr["VisionName"] = strName;
                        dr["Feature"] = strNode;
                        dr["Count"] = objFile.GetValueAsString(strNode, "0", 2);
                        dtFail.Rows.Add(dr);
                    }
                }

                m_report.Load(AppDomain.CurrentDomain.BaseDirectory + "\\Report\\LotlyReport.rpt");
                m_report.Database.Tables["Vision"].SetDataSource(dtVision);
                m_report.Database.Tables["FailDetail"].SetDataSource(dtFail);

                objFile.GetFirstSection("Lot");
                m_report.SetParameterValue("MachineID", objFile.GetValueAsString("MachineID", "SRM"));
                m_report.SetParameterValue("LotID", objFile.GetValueAsString("LotID", "SRM"));
                m_report.SetParameterValue("LotStartTime", objFile.GetValueAsString("LotStartTime", DateTime.Now.ToString()));
                m_report.SetParameterValue("LotStopTime", objFile.GetValueAsString("LotStopTime", DateTime.Now.ToString()));
                m_report.SetParameterValue("OperatorID", objFile.GetValueAsString("OperatorID", "Op"));
                m_report.SetParameterValue("RecipeID", objFile.GetValueAsString("RecipeID", "Default"));

                switch (m_smCustomizeInfo.g_intExportReportFormat)
                {
                    case 0:
                        m_report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Text, m_smCustomizeInfo.g_strExportReportDir + "\\" + strLot + "_" + DateTime.Now.ToString("yyyyMMddHHmmss").ToString() + ".txt");
                        break;
                    case 1:
                        m_report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, m_smCustomizeInfo.g_strExportReportDir + "\\" + strLot + "_" + DateTime.Now.ToString("yyyyMMddHHmmss").ToString() + ".pdf");
                        break;
                    case 2:
                        m_report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel, m_smCustomizeInfo.g_strExportReportDir + "\\" + strLot + "_" + DateTime.Now.ToString("yyyyMMddHHmmss").ToString() + ".xls");
                        break;
                }
            }
            else
            {
                SRMMessageBox.Show("Save current lot report path does not exist. Fail to save current lot report.",
                                    "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Customize GUI based on user access level
        /// </summary>
        private void SetCustomView()
        {
            UserRight objUserRight = new UserRight();
            string strChild1 = "AutoMode";
            string strChild2 = "";
            bool blnGroup1 = false;
            bool blnGroup2 = false;

            strChild2 = "New Lot";
            if (m_intUserGroup > objUserRight.GetGroupLevel2(strChild1, strChild2))
                btn_NewLot.Visible = false;
            else
            {
                btn_NewLot.Visible = true;
                blnGroup1 = true;
            }

            strChild2 = "Reset Counter";
            if (m_intUserGroup > objUserRight.GetGroupLevel2(strChild1, strChild2))
                btn_ResetCounter.Visible = false;
            else
            {
                btn_ResetCounter.Visible = true;
                blnGroup1 = true;
            }

            strChild2 = "Grab Image";
            if (m_intUserGroup > objUserRight.GetGroupLevel2(strChild1, strChild2))
                btn_Grab.Visible = false;
            else
            {
                btn_Grab.Visible = true;
                blnGroup1 = true;
            }

            strChild2 = "Live Image";
            if (m_intUserGroup > objUserRight.GetGroupLevel2(strChild1, strChild2))
                btn_Live.Visible = false;
            else
            {
                btn_Live.Visible = true;

                if (!blnGroup1)
                    blnGroup1 = true;
            }
            strChild2 = "Load Image";
            if (m_intUserGroup > objUserRight.GetGroupLevel2(strChild1, strChild2))
            {
                btn_LoadImage.Visible = false;
                btn_LoadImage2.Visible = false;
                btn_ImageNext.Visible = false;
                btn_ImagePrev.Visible = false;
            }
            else
            {
                btn_LoadImage2.Visible = true;
                btn_ImageNext.Visible = true;
                btn_ImagePrev.Visible = true;

                if (!blnGroup1)
                    blnGroup1 = true;
            }

            strChild2 = "Save Image";
            if (m_intUserGroup > objUserRight.GetGroupLevel2(strChild1, strChild2))
                btn_SaveImage.Visible = false;
            else
            {
                btn_SaveImage.Visible = true;

                if (!blnGroup1)
                    blnGroup1 = true;
            }

            strChild2 = "GRR Page";
            if (m_intUserGroup > objUserRight.GetGroupLevel2(strChild1, strChild2))
            {
                btn_GRR.Visible = false;
            }
            else
            {
                btn_GRR.Visible = true;

                m_blnGroupGRR = true;
                blnGroup1 = true;
            }

            if (!blnGroup1)
                TS_Separator1.Visible = false;
            if (!blnGroup2)
                TS_Separator2.Visible = false;
        }
        private void SetCustomView2()
        {
            //NewUserRight objUserRight = new NewUserRight(false);
            string strParent = "Bottom Menu";
            string strChild1 = "";
            bool blnGroup1 = false;
            bool blnGroup2 = false;

            strChild1 = "Production All";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild1Group(strParent, strChild1))
                btn_ProductionAll.Visible = false;
            else
            {
                btn_ProductionAll.Visible = true;
                blnGroup1 = true;
            }

            strChild1 = "Production";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild1Group(strParent, strChild1))
                btn_Production.Visible = false;
            else
            {
                btn_Production.Visible = true;
                blnGroup1 = true;
            }

            strChild1 = "New Lot";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild1Group(strParent, strChild1))
                btn_NewLot.Visible = false;
            else
            {
                btn_NewLot.Visible = true;
                blnGroup1 = true;
            }

            strChild1 = "Reset Counter";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild1Group(strParent, strChild1))
                btn_ResetCounter.Visible = false;
            else
            {
                btn_ResetCounter.Visible = true;
                blnGroup1 = true;
            }

            strChild1 = "Grab Image";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild1Group(strParent, strChild1))
                btn_Grab.Visible = false;
            else
            {
                btn_Grab.Visible = true;
                blnGroup1 = true;
            }

            strChild1 = "Live Image";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild1Group(strParent, strChild1))
                btn_Live.Visible = false;
            else
            {
                btn_Live.Visible = true;

                if (!blnGroup1)
                    blnGroup1 = true;
            }

            strChild1 = "Load Image";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild1Group(strParent, strChild1))
            {
                btn_LoadImage.Visible = false;
                btn_LoadImage2.Visible = false;
                btn_ImageNext.Visible = false;
                btn_ImagePrev.Visible = false;
            }
            else
            {
                btn_LoadImage2.Visible = true;
                btn_ImageNext.Visible = true;
                btn_ImagePrev.Visible = true;

                if (!blnGroup1)
                    blnGroup1 = true;
            }

            strChild1 = "Save Image";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild1Group(strParent, strChild1))
                btn_SaveImage.Visible = false;
            else
            {
                btn_SaveImage.Visible = true;

                if (!blnGroup1)
                    blnGroup1 = true;
            }

            strChild1 = "Image Previous";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild1Group(strParent, strChild1))
                btn_ImagePrev.Visible = false;
            else
            {
                btn_ImagePrev.Visible = true;

                if (!blnGroup1)
                    blnGroup1 = true;
            }

            strChild1 = "Image Next";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild1Group(strParent, strChild1))
                btn_ImageNext.Visible = false;
            else
            {
                btn_ImageNext.Visible = true;

                if (!blnGroup1)
                    blnGroup1 = true;
            }

            strChild1 = "GRR Page";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild1Group(strParent, strChild1))
            {
                btn_GRR.Visible = false;
            }
            else
            {
                btn_GRR.Visible = true;

                m_blnGroupGRR = true;
                blnGroup1 = true;
            }

            //strParent = "Save Error";
            //if (m_intUserGroup > objUserRight.GetBottomMenuChild1Group(strParent, strChild1))
            //{
            //    btn_SaveErrorMessage.Visible = false;
            //}
            //else
            //{
            //    btn_SaveErrorMessage.Visible = true;

            //    if (!blnGroup1)
            //        blnGroup1 = true;
            //}

            //strParent = "Clear Error";
            //if (m_intUserGroup > objUserRight.GetBottomMenuChild1Group(strParent, strChild1))
            //{
            //    btn_ClearErrorMessage.Visible = false;
            //}
            //else
            //{
            //    btn_ClearErrorMessage.Visible = true;

            //    if (!blnGroup1)
            //        blnGroup1 = true;
            //}

            strChild1 = "Template";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild1Group(strParent, strChild1))
            {
                btn_Template.Visible = false;
            }
            else
            {
                btn_Template.Visible = true;

                if (!blnGroup1)
                    blnGroup1 = true;
            }

            strChild1 = "Exit";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild1Group(strParent, strChild1))
            {
                btn_Exit.Visible = false;
            }
            else
            {
                btn_Exit.Visible = true;

                if (!blnGroup1)
                    blnGroup1 = true;
            }

            if (!blnGroup1)
                TS_Separator1.Visible = false;
            if (!blnGroup2)
                TS_Separator2.Visible = false;

            // Child 2

            strChild1 = "Load Image";
            string strChild2 = "";

            strChild2 = "Load Template Image";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild2Group(strChild1, strChild2))
                btn_TemplateImages.Visible = false;
            else
            {
                btn_TemplateImages.Visible = true;
            }

            // 2020 08 14 - no need this button anymore. User never used it.
            //strChild2 = "Last Test Image";
            //if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild2Group(strChild1, strChild2))
            //    btn_LastImage.Visible = false;
            //else
            //{
            //    btn_LastImage.Visible = true;
            //}

            strChild2 = "Current Lot Test Images";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild2Group(strChild1, strChild2))
                btn_CurrentLotImage.Visible = false;
            else
            {
                btn_CurrentLotImage.Visible = true;
            }

            strChild2 = "Lot Test Images List";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild2Group(strChild1, strChild2))
                btn_PreviousLotImage.Visible = false;
            else
            {
                btn_PreviousLotImage.Visible = true;
            }

            strChild1 = "Save Image";
            strChild2 = "Save Image";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild2Group(strChild1, strChild2))
                btn_SaveImage2.Visible = false;
            else
            {
                btn_SaveImage2.Visible = true;
            }

            strChild2 = "Save Option";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild2Group(strChild1, strChild2))
                btn_SaveOption.Visible = false;
            else
            {
                btn_SaveOption.Visible = true;
            }
        }
        /// <summary>
        /// Set vision station 
        /// </summary>
        /// <param name="intCount">vision station count</param>
        /// <param name="smVisionInfo">vision info</param>
        //private void SetVisionStation(int intCount, VisionInfo smVisionInfo)
        //{
        //    switch (intCount)
        //    {
        //        case 1:
        //            m_smStation1 = smVisionInfo;
        //            break;
        //        case 2:
        //            m_smStation2 = smVisionInfo;
        //            break;
        //        case 3:
        //            m_smStation3 = smVisionInfo;
        //            break;
        //        case 4:
        //            m_smStation4 = smVisionInfo;
        //            break;
        //        case 5:
        //            m_smStation5 = smVisionInfo;
        //            break;
        //        case 6:
        //            m_smStation6 = smVisionInfo;
        //            break;
        //        case 7:
        //            m_smStation7 = smVisionInfo;
        //            break;
        //        case 8:
        //            m_smStation8 = smVisionInfo;
        //            break;
        //        case 9:
        //            m_smStation9 = smVisionInfo;
        //            break;
        //        case 10:
        //            m_smStation10 = smVisionInfo;
        //            break;
        //    }
        //}

        private string CheckControlSetting(bool blnPressProductionAll)
        {
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (!blnPressProductionAll)
                {
                    if (i != (m_intSelectedVisionStation - 1))
                        continue;
                }

                if (m_smVSInfo[i] == null)
                    continue;

                int intPos = (1 << i);
                switch (m_smVSInfo[i].g_strVisionName)
                {
                    case "BottomOrient":
                        if (!CheckOrientControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;
                        break;
                    case "Mark":
                    case "MarkOrient":
                    case "MOLi":
                    case "InPocket":
                    case "IPMLi":
                        if (!CheckMarkControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;

                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                        {
                            if (!CheckLeadControlSetting(m_smVSInfo[i]))
                                return m_smVSInfo[i].g_strVisionDisplayName;
                        }
                        break;
                    case "MarkPkg":
                    case "MOPkg":
                    case "MOLiPkg":
                    case "InPocketPkg":
                    case "InPocketPkgPos":
                    case "IPMLiPkg":

                        if (!CheckOrientationControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;

                        if (!CheckMarkControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;

                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                        {
                            if (!CheckLeadControlSetting(m_smVSInfo[i]))
                                return m_smVSInfo[i].g_strVisionDisplayName;
                        }

                        if (!CheckPackageControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;

                        if ((m_smCustomizeInfo.g_intUseColorCamera & intPos) > 0)
                        {
                            if (!CheckColorPackageControlSetting(m_smVSInfo[i]))
                                return m_smVSInfo[i].g_strVisionDisplayName;
                        }
                        break;
                    case "Package":
                        if (!CheckPackageControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;

                        if ((m_smCustomizeInfo.g_intUseColorCamera & intPos) > 0)
                        {
                            if (!CheckColorPackageControlSetting(m_smVSInfo[i]))
                                return m_smVSInfo[i].g_strVisionDisplayName;
                        }
                        break;
                    case "BottomOrientPad":
                    case "BottomOPadPkg":
                        if (!CheckOrientControlSetting(m_smVSInfo[i]) || !CheckPadControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;

                        if ((m_smCustomizeInfo.g_intUseColorCamera & intPos) > 0)
                        {
                            if (!CheckCenterColorPadControlSetting(m_smVSInfo[i]))
                                return m_smVSInfo[i].g_strVisionDisplayName;
                        }
                        break;
                    case "Pad":
                    case "PadPos":
                        if (!CheckPadControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;

                        if ((m_smCustomizeInfo.g_intUseColorCamera & intPos) > 0)
                        {
                            if (!CheckCenterColorPadControlSetting(m_smVSInfo[i]))
                                return m_smVSInfo[i].g_strVisionDisplayName;
                        }
                        break;
                    case "PadPkg":
                    case "PadPkgPos":
                        if (!CheckPadControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;

                        if (!CheckCenterPackagePadControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;

                        if ((m_smCustomizeInfo.g_intUseColorCamera & intPos) > 0)
                        {
                            if (!CheckCenterColorPadControlSetting(m_smVSInfo[i]))
                                return m_smVSInfo[i].g_strVisionDisplayName;
                        }
                        break;
                    case "Pad5S":
                    case "Pad5SPos":
                        if (!CheckPadControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;

                        if (!CheckSidePadControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;

                        if ((m_smCustomizeInfo.g_intUseColorCamera & intPos) > 0)
                        {
                            if (!CheckCenterColorPadControlSetting(m_smVSInfo[i]))
                                return m_smVSInfo[i].g_strVisionDisplayName;
                            if (!CheckSideColorPadControlSetting(m_smVSInfo[i]))
                                return m_smVSInfo[i].g_strVisionDisplayName;
                        }
                        break;
                    case "Pad5SPkg":
                    case "Pad5SPkgPos":
                        if (!CheckPadControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;

                        if (!CheckSidePadControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;

                        if (!CheckCenterPackagePadControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;

                        if (!CheckSidePackagePadControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;

                        if ((m_smCustomizeInfo.g_intUseColorCamera & intPos) > 0)
                        {
                            if (!CheckCenterColorPadControlSetting(m_smVSInfo[i]))
                                return m_smVSInfo[i].g_strVisionDisplayName;
                            if (!CheckSideColorPadControlSetting(m_smVSInfo[i]))
                                return m_smVSInfo[i].g_strVisionDisplayName;
                        }
                        break;
                    case "Li3D":
                        if (!CheckLead3DControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;
                        break;
                    case "Li3DPkg":
                        if (!CheckLead3DControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;

                        if (!CheckLead3DPkgControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;
                        break;
                    case "Seal":
                        if (!CheckSealControlSetting(m_smVSInfo[i]))
                            return m_smVSInfo[i].g_strVisionDisplayName;
                        break;
                        break;
                    case "UnitPresent":
                        break;
                }
            }
            return "";
        }
        private bool CheckSealControlSetting(VisionInfo smVisionInfo)
        {
            //------------------------------------Seal--------------------------------

            for (int u = 0; u < smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (((smVisionInfo.g_intOptionControlMask & 0x01) > 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x01) == 0))
                    return false;

                if (((smVisionInfo.g_intOptionControlMask & 0x02) > 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x02) == 0))
                    return false;

                if (((smVisionInfo.g_intOptionControlMask & 0x04) > 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x04) == 0))
                    return false;

                if (((smVisionInfo.g_intOptionControlMask & 0x08) > 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x08) == 0))
                    return false;

                if (((smVisionInfo.g_intOptionControlMask & 0x10) > 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x10) == 0))
                    return false;

                if (((smVisionInfo.g_intOptionControlMask & 0x20) > 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x20) == 0))
                    return false;

                if (((smVisionInfo.g_intOptionControlMask & 0x40) > 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x40) == 0))
                    return false;

                if (((smVisionInfo.g_intOptionControlMask & 0x80) > 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x80) == 0))
                    return false;

                if ((((smVisionInfo.g_intOptionControlMask & 0x100) > 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x100) == 0)) && !smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole)
                    return false;

                if ((((smVisionInfo.g_intOptionControlMask & 0x200) > 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x200) == 0)) && !smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
                    return false;

                if ((((smVisionInfo.g_intOptionControlMask & 0x400) > 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x400) == 0)) && !smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
                    return false;

                if ((((smVisionInfo.g_intOptionControlMask & 0x800) > 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x800) == 0)) && !smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
                    return false;

                if ((((smVisionInfo.g_intOptionControlMask & 0x1000) > 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x1000) == 0)) && !smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
                    return false;

                if ((((smVisionInfo.g_intOptionControlMask & 0x2000) > 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x2000) == 0)) && smVisionInfo.g_objSeal.ref_blnWantCheckSealEdgeStraightness)
                    return false;
            }


            return true;
        }
        private bool CheckOrientControlSetting(VisionInfo smVisionInfo)
        {
            //------------------------------------Orient--------------------------------

            for (int u = 0; u < smVisionInfo.g_intUnitsOnImage; u++)
            {
                if ((smVisionInfo.g_intOptionControlMask & 0x01) > 0 && !smVisionInfo.g_arrOrients[u][0].ref_blnWantCheckOrientAngleTolerance)
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x02) > 0 && !smVisionInfo.g_arrOrients[u][0].ref_blnWantCheckOrientXTolerance)
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x04) > 0 && !smVisionInfo.g_arrOrients[u][0].ref_blnWantCheckOrientYTolerance)
                    return false;
            }


            return true;
        }
        private bool CheckOrientPadControlSetting(VisionInfo smVisionInfo)
        {
            //------------------------------------OrientPad--------------------------------

            if (smVisionInfo.g_objPadOrient != null)
            {
                if ((smVisionInfo.g_intOptionControlMask & 0x8000000) > 0 && !smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientAngleTolerance)
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x10000000) > 0 && !smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientXTolerance)
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x20000000) > 0 && !smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientYTolerance)
                    return false;
            }


            return true;
        }

        private bool CheckOrientationControlSetting(VisionInfo smVisionInfo)
        {
            //------------------------------------OrientPad--------------------------------

            if (smVisionInfo.g_arrOrients != null && smVisionInfo.g_arrOrients.Count > 0 && smVisionInfo.g_arrOrients[0].Count > 0)
            {

                if ((smVisionInfo.g_intOptionControlMask & 0x40000000) > 0 && !smVisionInfo.g_arrOrients[0][0].ref_blnWantCheckOrientation)
                    return false;
            }

            return true;
        }

        private bool CheckMarkControlSetting(VisionInfo smVisionInfo)
        {
            //------------------------------------Mark--------------------------------

            for (int u = 0; u < smVisionInfo.g_intUnitsOnImage; u++)
            {
                int intFailMask = smVisionInfo.g_arrMarks[u].GetFailOptionMask(smVisionInfo.g_intSelectedGroup, smVisionInfo.g_intSelectedTemplate);

                if ((smVisionInfo.g_intOptionControlMask & 0x01) > 0 && !smVisionInfo.g_arrMarks[u].ref_blnCheckMark)
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x02) > 0 && ((intFailMask & 0x01) == 0 || !smVisionInfo.g_arrMarks[u].ref_blnCheckMark))
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x04) > 0 && ((intFailMask & 0x02) == 0 || !smVisionInfo.g_arrMarks[u].ref_blnCheckMark))
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x08) > 0 && ((intFailMask & 0x04) == 0 || !smVisionInfo.g_arrMarks[u].ref_blnCheckMark))
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x10) > 0 && ((intFailMask & 0x08) == 0 || !smVisionInfo.g_arrMarks[u].ref_blnCheckMark))
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x20) > 0 && ((intFailMask & 0x10) == 0 || !smVisionInfo.g_arrMarks[u].ref_blnCheckMark))
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x40) > 0 && ((intFailMask & 0x20) == 0 || !smVisionInfo.g_arrMarks[u].ref_blnCheckMark) && smVisionInfo.g_blnWantCheckMarkBroken)
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x80) > 0 && ((intFailMask & 0x40) == 0 || !smVisionInfo.g_arrMarks[u].ref_blnCheckMark))
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x100) > 0 && ((intFailMask & 0x80) == 0 || !smVisionInfo.g_arrMarks[u].ref_blnCheckMark))
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x200) > 0 && ((intFailMask & 0x100) == 0 || !smVisionInfo.g_arrMarks[u].ref_blnCheckMark) && smVisionInfo.g_blnWantCheckMarkTotalExcess)
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x400) > 0 && ((intFailMask & 0x200) == 0 || !smVisionInfo.g_arrMarks[u].ref_blnCheckMark) && smVisionInfo.g_blnWantCheckMarkAverageGrayValue)
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x2000) > 0 && ((intFailMask & 0x2000) == 0 || !smVisionInfo.g_arrMarks[u].ref_blnCheckMark) && smVisionInfo.g_blnWantCheckMarkAngle)
                    return false;
            }
            //------------------------------------Pin1--------------------------------

            if (smVisionInfo.g_blnWantPin1)
            {
                if ((smVisionInfo.g_intOptionControlMask & 0x800) > 0 && !smVisionInfo.g_arrPin1[0].getWantCheckPin1(smVisionInfo.g_intSelectedTemplate))
                    return false;
            }

            return true;
        }

        private bool CheckLeadControlSetting(VisionInfo smVisionInfo)
        {
            //------------------------------------Lead--------------------------------

            for (int i = 0; i < smVisionInfo.g_arrLead.Length; i++)
            {
                if (!smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;

                int intFailMask = smVisionInfo.g_arrLead[i].ref_intFailOptionMask;

                if ((smVisionInfo.g_intLeadOptionControlMask & 0x40000) > 0 && !smVisionInfo.g_arrLead[i].GetWantInspectLead())
                    return false;

                if ((smVisionInfo.g_intLeadOptionControlMask & 0xC0) > 0 && (intFailMask & 0xC0) == 0)
                    return false;

                if ((smVisionInfo.g_intLeadOptionControlMask & 0x100) > 0 && (intFailMask & 0x100) == 0)
                    return false;

                if ((smVisionInfo.g_intLeadOptionControlMask & 0x20000) > 0 && (intFailMask & 0x8000) == 0)// && !smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance) //2020-08-12 ZJYEOH : only check tip offset control setting if no use package to base method
                    return false;

                if ((smVisionInfo.g_intLeadOptionControlMask & 0x800) > 0 && (intFailMask & 0x800) == 0)
                    return false;

                if ((smVisionInfo.g_intLeadOptionControlMask & 0x600) > 0 && (intFailMask & 0x600) == 0)
                    return false;

                if ((smVisionInfo.g_intLeadOptionControlMask & 0x1000) > 0 && (intFailMask & 0x1000) == 0)
                    return false;

                if ((smVisionInfo.g_intLeadOptionControlMask & 0x2000) > 0 && (intFailMask & 0x2000) == 0)
                    return false;

                if ((smVisionInfo.g_intLeadOptionControlMask & 0x10000) > 0 && (intFailMask & 0x4000) == 0 && smVisionInfo.g_arrLead[0].ref_blnWantUseAverageGrayValueMethod)
                    return false;

                if ((smVisionInfo.g_intLeadOptionControlMask & 0x4000) > 0 && !smVisionInfo.g_arrLead[i].ref_blnWantCheckExtraLeadArea)
                    return false;

                if ((smVisionInfo.g_intLeadOptionControlMask & 0x8000) > 0 && !smVisionInfo.g_arrLead[i].ref_blnWantCheckExtraLeadLength)
                    return false;

                if ((smVisionInfo.g_intLeadOptionControlMask & 0x80000) > 0 && (intFailMask & 0x10000) == 0 && smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead)
                    return false;

                if ((smVisionInfo.g_intLeadOptionControlMask & 0x100000) > 0 && (intFailMask & 0x20000) == 0 && smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead)
                    return false;

            }
            return true;
        }
        private bool CheckColorPackageControlSetting(VisionInfo smVisionInfo)
        {
            //------------------------------------Color Package--------------------------------

            int intFailMask = smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x01) > 0 && (intFailMask & 0x01) == 0 && smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x02) > 0 && (intFailMask & 0x02) == 0 && smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x04) > 0 && (intFailMask & 0x04) == 0 && smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 1)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x08) > 0 && (intFailMask & 0x08) == 0 && smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 1)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x10) > 0 && (intFailMask & 0x10) == 0 && smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x20) > 0 && (intFailMask & 0x20) == 0 && smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x40) > 0 && (intFailMask & 0x40) == 0 && smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x80) > 0 && (intFailMask & 0x80) == 0 && smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x100) > 0 && (intFailMask & 0x100) == 0 && smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x200) > 0 && (intFailMask & 0x200) == 0 && smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4)
                return false;

            return true;
        }
        private bool CheckPackageControlSetting(VisionInfo smVisionInfo)
        {
            //------------------------------------Package--------------------------------
            if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnUseDetailDefectCriteria)
            {
                if ((smVisionInfo.g_intPkgOptionControlMask & 0x01) > 0 && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage())
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x02) > 0 && (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x1000) == 0)
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x04) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam(0) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x08) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam(2) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x10) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam(4) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x20) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam(5) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x40) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam(5) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x80) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam(0) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x100) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam(1) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x200) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam(2) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x400) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam(3) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x800) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam(4) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                    return false;
            }
            else
            {
                if ((smVisionInfo.g_intPkgOptionControlMask & 0x1000) > 0 && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage())
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x2000) > 0 && (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x1000) == 0)
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x2000000) > 0 && (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x2000) == 0)
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x4000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Bright) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x8000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Bright) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x10000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x20000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                    return false;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
                {
                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x4000000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark2) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                        return false;

                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x8000000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark2) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                        return false;
                }

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting)
                {
                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x10000000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark3) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                        return false;

                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x20000000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark3) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                        return false;
                }

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
                {
                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x100000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark4) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                        return false;

                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x200000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark4) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                        return false;
                }


                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                {
                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x40000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Crack) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                        return false;

                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x80000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Crack) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                        return false;
                }

                //if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateVoidDefectSetting)
                //{
                //    if ((smVisionInfo.g_intPkgOptionControlMask & 0x100000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Void) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                //        return false;


                //    if ((smVisionInfo.g_intPkgOptionControlMask & 0x200000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Void) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                //        return false;
                //}

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                {
                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x400000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.MoldFlash) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                        return false;
                }

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
                {
                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x800000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.ChipBright) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                        return false;

                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x40000000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.ChipBright) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                        return false;

                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x1000000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.ChipDark) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                        return false;

                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x80000000) > 0 && (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.ChipDark) || !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()))
                        return false;

                }
            }
            return true;
        }
        private bool CheckCenterColorPadControlSetting(VisionInfo smVisionInfo)
        {
            //------------------------------------Color Pad--------------------------------

            int intFailMask = smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x01) > 0 && (intFailMask & 0x01) == 0 && smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x02) > 0 && (intFailMask & 0x02) == 0 && smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x04) > 0 && (intFailMask & 0x04) == 0 && smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 1)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x08) > 0 && (intFailMask & 0x08) == 0 && smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 1)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x10) > 0 && (intFailMask & 0x10) == 0 && smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x20) > 0 && (intFailMask & 0x20) == 0 && smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x40) > 0 && (intFailMask & 0x40) == 0 && smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x80) > 0 && (intFailMask & 0x80) == 0 && smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x100) > 0 && (intFailMask & 0x100) == 0 && smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4)
                return false;

            if ((smVisionInfo.g_intOptionControlMask2 & 0x200) > 0 && (intFailMask & 0x200) == 0 && smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4)
                return false;

            return true;
        }
        private bool CheckSideColorPadControlSetting(VisionInfo smVisionInfo)
        {
            //------------------------------------Color Pad--------------------------------
            int intControlMask = 0;
            for (int i = 1; i < smVisionInfo.g_arrPad.Length; i++)
            {
                int intFailMask = smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask;
      
                if(i == 1)
                    intControlMask = smVisionInfo.g_intOptionControlMask2;
                if (i == 2)
                    intControlMask = smVisionInfo.g_intOptionControlMask3;
                if (i == 3)
                    intControlMask = smVisionInfo.g_intOptionControlMask4;
                if (i == 4)
                    intControlMask = smVisionInfo.g_intOptionControlMask5;

                if ((intControlMask & 0x1000) > 0 && (intFailMask & 0x01) == 0 && smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 0)
                    return false;

                if ((intControlMask & 0x2000) > 0 && (intFailMask & 0x02) == 0 && smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 0)
                    return false;

                if ((intControlMask & 0x4000) > 0 && (intFailMask & 0x04) == 0 && smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1)
                    return false;

                if ((intControlMask & 0x8000) > 0 && (intFailMask & 0x08) == 0 && smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1)
                    return false;

                if ((intControlMask & 0x10000) > 0 && (intFailMask & 0x10) == 0 && smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2)
                    return false;

                if ((intControlMask & 0x20000) > 0 && (intFailMask & 0x20) == 0 && smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2)
                    return false;

                if ((intControlMask & 0x40000) > 0 && (intFailMask & 0x40) == 0 && smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3)
                    return false;

                if ((intControlMask & 0x80000) > 0 && (intFailMask & 0x80) == 0 && smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3)
                    return false;

                if ((intControlMask & 0x100000) > 0 && (intFailMask & 0x100) == 0 && smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4)
                    return false;

                if ((intControlMask & 0x200000) > 0 && (intFailMask & 0x200) == 0 && smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4)
                    return false;
            }

            return true;
        }
        private bool CheckPadControlSetting(VisionInfo smVisionInfo)
        {
            //------------------------------------Pad--------------------------------

            int intFailMask = smVisionInfo.g_arrPad[0].ref_intFailOptionMask;

            if ((smVisionInfo.g_intOptionControlMask & 0x4000000) > 0 && !smVisionInfo.g_blnCheckPad)
            {
                if ((m_smCustomizeInfo.g_intWantOrient & (1 << smVisionInfo.g_intVisionPos)) > 0)
                    return false;
            }

            if ((smVisionInfo.g_intOptionControlMask & 0x01) > 0 && (intFailMask & 0x20) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x02) > 0 && (intFailMask & 0xC0) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x04) > 0 && (intFailMask & 0x100) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x20) > 0 && !smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadArea)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x40) > 0 && !smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadLength)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x80) > 0 && (intFailMask & 0x600) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x100) > 0 && (intFailMask & 0x800) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x200) > 0 && (intFailMask & 0x2000) == 0)
                return false;

            if (((smVisionInfo.g_intOptionControlMask & 0x800000) > 0 && (intFailMask & 0x4000) == 0) && smVisionInfo.g_arrPad[0].ref_blnWantEdgeLimit_Pad)
                return false;

            if (((smVisionInfo.g_intOptionControlMask & 0x2000000) > 0 && (intFailMask & 0x8000) == 0) && smVisionInfo.g_arrPad[0].ref_blnWantStandOff_Pad)
                return false;

            if (((smVisionInfo.g_intOptionControlMask & 0x10000000) > 0 && (intFailMask & 0x10000) == 0) && smVisionInfo.g_arrPad[0].ref_blnWantEdgeDistance_Pad)
                return false;

            if (((smVisionInfo.g_intOptionControlMask & 0x40000000) > 0 && (intFailMask & 0x20000) == 0) && smVisionInfo.g_arrPad[0].ref_blnWantSpan_Pad)
                return false;

            if (((smVisionInfo.g_intOptionControlMask & 0x80000000) > 0 && (intFailMask & 0x40000) == 0) && smVisionInfo.g_arrPad[0].ref_blnWantSpan_Pad)
                return false;

            if ((smVisionInfo.g_strVisionName != "Pad5SPkg" && smVisionInfo.g_strVisionName != "PadPkg") && smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize)
            {
                //------------------------------------Foreign Material--------------------------------

                if ((smVisionInfo.g_intOptionControlMask & 0x08) > 0 && !smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadArea)
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x10) > 0 && !smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadLength)
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x400) > 0 && (intFailMask & 0x1000) == 0)
                    return false;
            }

            //------------------------------------Pin1--------------------------------

            if (smVisionInfo.g_blnWantPin1)
            {
                if ((smVisionInfo.g_intOptionControlMask & 0x800) > 0 && !smVisionInfo.g_arrPin1[0].getWantCheckPin1(smVisionInfo.g_intSelectedTemplate))
                    return false;
            }
            return true;
        }

        private bool CheckSidePadControlSetting(VisionInfo smVisionInfo)
        {
            //------------------------------------Pad--------------------------------

            int intFailMask = smVisionInfo.g_arrPad[1].ref_intFailOptionMask;

            if ((smVisionInfo.g_intOptionControlMask & 0x1000) > 0 && (intFailMask & 0x20) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x2000) > 0 && (intFailMask & 0xC0) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x4000) > 0 && (intFailMask & 0x100) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x20000) > 0 && !smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadArea)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x40000) > 0 && !smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadLength)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x80000) > 0 && (intFailMask & 0x600) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x100000) > 0 && (intFailMask & 0x800) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x200000) > 0 && (intFailMask & 0x2000) == 0)
                return false;

            if (((smVisionInfo.g_intOptionControlMask & 0x1000000) > 0 && (intFailMask & 0x4000) == 0) && smVisionInfo.g_arrPad[1].ref_blnWantEdgeLimit_Pad)
                return false;

            if (((smVisionInfo.g_intOptionControlMask & 0x8000000) > 0 && (intFailMask & 0x8000) == 0) && smVisionInfo.g_arrPad[1].ref_blnWantStandOff_Pad)
                return false;

            if (((smVisionInfo.g_intOptionControlMask & 0x20000000) > 0 && (intFailMask & 0x10000) == 0) && smVisionInfo.g_arrPad[1].ref_blnWantEdgeDistance_Pad)
                return false;

            if (((smVisionInfo.g_intOptionControlMask & 0x100000000) > 0 && (intFailMask & 0x20000) == 0) && smVisionInfo.g_arrPad[1].ref_blnWantSpan_Pad)
                return false;

            if (((smVisionInfo.g_intOptionControlMask & 0x200000000) > 0 && (intFailMask & 0x40000) == 0) && smVisionInfo.g_arrPad[1].ref_blnWantSpan_Pad)
                return false;

            if ((smVisionInfo.g_strVisionName != "Pad5SPkg" && smVisionInfo.g_strVisionName != "PadPkg") && smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize)
            {
                //------------------------------------Foreign Material--------------------------------

                if ((smVisionInfo.g_intOptionControlMask & 0x8000) > 0 && !smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadArea)
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x10000) > 0 && !smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadLength)
                    return false;

                if ((smVisionInfo.g_intOptionControlMask & 0x400000) > 0 && (intFailMask & 0x1000) == 0)
                    return false;
            }
            return true;
        }

        private bool CheckCenterPackagePadControlSetting(VisionInfo smVisionInfo)
        {
            //------------------------------------Pad--------------------------------

            int intFailMask = smVisionInfo.g_arrPad[0].ref_intFailPkgOptionMask;
            if (smVisionInfo.g_arrPad[0].ref_blnUseDetailDefectCriteria)
            {
                if ((smVisionInfo.g_intPkgOptionControlMask & 0x01) > 0 && !smVisionInfo.g_arrPad[0].GetWantInspectPackage())
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x02) > 0 && (intFailMask & 0x01) == 0)
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x04) > 0 && ((intFailMask & 0x04) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x08) > 0 && ((intFailMask & 0x08) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x10) > 0 && ((intFailMask & 0x10) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x20) > 0 && ((intFailMask & 0x20) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x40) > 0 && ((intFailMask & 0x40) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x80) > 0 && ((intFailMask & 0x1000) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x100) > 0 && ((intFailMask & 0x80) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x200) > 0 && ((intFailMask & 0x100) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x400) > 0 && ((intFailMask & 0x200) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x800) > 0 && ((intFailMask & 0x400) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x1000) > 0 && ((intFailMask & 0x800) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                    return false;
            }
            else
            {
                if ((smVisionInfo.g_intPkgOptionControlMask & 0x2000) > 0 && !smVisionInfo.g_arrPad[0].GetWantInspectPackage())
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x4000) > 0 && (intFailMask & 0x01) == 0)
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x8000) > 0 && ((intFailMask & 0x10000) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x10000) > 0 && ((intFailMask & 0x20000) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x20000) > 0 && ((intFailMask & 0x40000) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x40000) > 0 && ((intFailMask & 0x80000) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                    return false;

                if (smVisionInfo.g_arrPad[0].ref_blnSeperateCrackDefectSetting)
                {
                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x80000) > 0 && ((intFailMask & 0x800) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                        return false;

                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x100000) > 0 && ((intFailMask & 0x400) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                        return false;
                }

                if (smVisionInfo.g_arrPad[0].ref_blnSeperateChippedOffDefectSetting)
                {
                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x200000) > 0 && ((intFailMask & 0x100000) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                        return false;

                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x400000) > 0 && ((intFailMask & 0x200000) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                        return false;
                }

                if (smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting)
                {
                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x800000) > 0 && ((intFailMask & 0x80) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                        return false;

                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x4000000) > 0 && ((intFailMask & 0x1000000) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                        return false;
                }

                if (smVisionInfo.g_arrPad[0].ref_blnSeperateForeignMaterialDefectSetting)
                {
                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x1000000) > 0 && ((intFailMask & 0x400000) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                        return false;

                    if ((smVisionInfo.g_intPkgOptionControlMask & 0x2000000) > 0 && ((intFailMask & 0x800000) == 0 || !smVisionInfo.g_arrPad[0].GetWantInspectPackage()))
                        return false;
                }
            }

            return true;
        }

        private bool CheckSidePackagePadControlSetting(VisionInfo smVisionInfo)
        {
            //------------------------------------Pad--------------------------------

            int intFailMask = smVisionInfo.g_arrPad[1].ref_intFailPkgOptionMask;
            if (smVisionInfo.g_arrPad[1].ref_blnUseDetailDefectCriteria)
            {
                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x01) > 0 && !smVisionInfo.g_arrPad[1].GetWantInspectPackage())
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x02) > 0 && (intFailMask & 0x01) == 0)
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x04) > 0 && ((intFailMask & 0x04) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x08) > 0 && ((intFailMask & 0x08) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x10) > 0 && ((intFailMask & 0x10) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x20) > 0 && ((intFailMask & 0x20) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x40) > 0 && ((intFailMask & 0x40) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x80) > 0 && ((intFailMask & 0x1000) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x100) > 0 && ((intFailMask & 0x80) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x200) > 0 && ((intFailMask & 0x100) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x400) > 0 && ((intFailMask & 0x200) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x800) > 0 && ((intFailMask & 0x400) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x1000) > 0 && ((intFailMask & 0x800) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                    return false;
            }
            else
            {
                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x2000) > 0 && !smVisionInfo.g_arrPad[1].GetWantInspectPackage())
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x4000) > 0 && (intFailMask & 0x01) == 0)
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x8000) > 0 && ((intFailMask & 0x10000) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x10000) > 0 && ((intFailMask & 0x20000) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x20000) > 0 && ((intFailMask & 0x40000) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x40000) > 0 && ((intFailMask & 0x80000) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                    return false;

                if (smVisionInfo.g_arrPad[1].ref_blnSeperateCrackDefectSetting)
                {
                    if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x80000) > 0 && ((intFailMask & 0x800) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                        return false;

                    if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x100000) > 0 && ((intFailMask & 0x400) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                        return false;
                }

                if (smVisionInfo.g_arrPad[1].ref_blnSeperateChippedOffDefectSetting)
                {
                    if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x200000) > 0 && ((intFailMask & 0x100000) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                        return false;

                    if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x400000) > 0 && ((intFailMask & 0x200000) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                        return false;
                }

                if (smVisionInfo.g_arrPad[1].ref_blnSeperateMoldFlashDefectSetting)
                {
                    if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x800000) > 0 && ((intFailMask & 0x80) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                        return false;

                    if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x4000000) > 0 && ((intFailMask & 0x1000000) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                        return false;
                }
                
                if (smVisionInfo.g_arrPad[1].ref_blnSeperateForeignMaterialDefectSetting)
                {
                    if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x1000000) > 0 && ((intFailMask & 0x400000) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                        return false;

                    if ((smVisionInfo.g_intPkgOptionControlMask2 & 0x2000000) > 0 && ((intFailMask & 0x800000) == 0 || !smVisionInfo.g_arrPad[1].GetWantInspectPackage()))
                        return false;
                }
            }

            return true;
        }

        private bool CheckLead3DControlSetting(VisionInfo smVisionInfo)
        {
            //------------------------------------Lead3D--------------------------------

            int intFailMask = smVisionInfo.g_arrLead3D[0].ref_intFailOptionMask;

            if ((smVisionInfo.g_intOptionControlMask & 0x01) > 0 && (intFailMask & 0x40) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x02) > 0 && (intFailMask & 0x80) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x04) > 0 && (intFailMask & 0x800) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x08) > 0 && (intFailMask & 0x200) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x10) > 0 && (intFailMask & 0x2000) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x20) > 0 && (intFailMask & 0x01) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x40) > 0 && (intFailMask & 0x4000) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x80) > 0 && (intFailMask & 0x02) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x100) > 0 && (intFailMask & 0x1000) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x200) > 0 && (intFailMask & 0x04) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x400) > 0 && (intFailMask & 0x08) == 0)
                return false;

            //2020-07-29 ZJYEOH : Temporary Hide Un-Cut Tiebar
            //if ((smVisionInfo.g_intOptionControlMask & 0x800) > 0 && (intFailMask & 0x10) == 0)
            //    return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x1000) > 0 && !smVisionInfo.g_arrLead3D[0].ref_blnWantCheckExtraLeadArea)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x2000) > 0 && !smVisionInfo.g_arrLead3D[0].ref_blnWantCheckExtraLeadLength)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x4000) > 0 && (intFailMask & 0x10000) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x8000) > 0 && (intFailMask & 0x100) == 0)// && !smVisionInfo.g_arrLead3D[0].ref_blnWantUsePkgToBaseTolerance) //2020-07-28 ZJYEOH : only check tip offset control setting if no use package to base method
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x10000) > 0 && (intFailMask & 0x20000) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x20000) > 0 && (intFailMask & 0x40000) == 0 && smVisionInfo.g_arrLead3D[0].ref_blnWantUseAverageGrayValueMethod)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x40000) > 0 && (intFailMask & 0x100000) == 0)
                return false;

            if ((smVisionInfo.g_intOptionControlMask & 0x80000) > 0 && (intFailMask & 0x200000) == 0)
                return false;

            return true;
        }

        private bool CheckLead3DPkgControlSetting(VisionInfo smVisionInfo)
        {
            //------------------------------------Lead3D--------------------------------

            int intFailMask = smVisionInfo.g_arrLead3D[0].ref_intFailPkgOptionMask;

            if ((smVisionInfo.g_intPkgOptionControlMask & 0x2000) > 0 && !smVisionInfo.g_arrLead3D[0].GetWantInspectPackage())
                return false;

            if ((smVisionInfo.g_intPkgOptionControlMask & 0x4000) > 0 && (intFailMask & 0x01) == 0)
                return false;

            if ((smVisionInfo.g_intPkgOptionControlMask & 0x8000) > 0 && ((intFailMask & 0x10000) == 0 || !smVisionInfo.g_arrLead3D[0].GetWantInspectPackage()))
                return false;

            if ((smVisionInfo.g_intPkgOptionControlMask & 0x10000) > 0 && ((intFailMask & 0x20000) == 0 || !smVisionInfo.g_arrLead3D[0].GetWantInspectPackage()))
                return false;

            if ((smVisionInfo.g_intPkgOptionControlMask & 0x20000) > 0 && ((intFailMask & 0x40000) == 0 || !smVisionInfo.g_arrLead3D[0].GetWantInspectPackage()))
                return false;

            if ((smVisionInfo.g_intPkgOptionControlMask & 0x40000) > 0 && ((intFailMask & 0x80000) == 0 || !smVisionInfo.g_arrLead3D[0].GetWantInspectPackage()))
                return false;

            if (smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
            {
                if ((smVisionInfo.g_intPkgOptionControlMask & 0x80000) > 0 && ((intFailMask & 0x800) == 0 || !smVisionInfo.g_arrLead3D[0].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x100000) > 0 && ((intFailMask & 0x400) == 0 || !smVisionInfo.g_arrLead3D[0].GetWantInspectPackage()))
                    return false;
            }

            if (smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting)
            {
                if ((smVisionInfo.g_intPkgOptionControlMask & 0x200000) > 0 && ((intFailMask & 0x100000) == 0 || !smVisionInfo.g_arrLead3D[0].GetWantInspectPackage()))
                    return false;

                if ((smVisionInfo.g_intPkgOptionControlMask & 0x400000) > 0 && ((intFailMask & 0x200000) == 0 || !smVisionInfo.g_arrLead3D[0].GetWantInspectPackage()))
                    return false;
            }

            if (smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
            {
                if ((smVisionInfo.g_intPkgOptionControlMask & 0x800000) > 0 && ((intFailMask & 0x80) == 0 || !smVisionInfo.g_arrLead3D[0].GetWantInspectPackage()))
                    return false;
            }


            return true;
        }

        private void btn_ClearErrorMessage_Click(object sender, EventArgs e)
        {
            m_objVisionPage[m_intSelectedVisionStation - 1].ClearErrorMessage();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            string strErrorMessage = "";
            List<int> temp = new List<int>();
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                // 2020 03 13 - JBTAN: offline page will auto close when user click production
                if (m_smVSInfo[i].VM_AT_OfflinePageView)
                {
                    m_objVisionPage[i].HideOfflinePage();
                }

                if (m_smVSInfo[i].VM_AT_SettingInDialog)// || m_smVSInfo[i].VM_AT_OfflinePageView)
                {
                    strErrorMessage += "- " + m_smVSInfo[i].g_strVisionDisplayName + "\n";
                    temp.Add(i);
                    //return;
                }
            }

            if (strErrorMessage.Length > 0)
            {
                SRMMessageBox.Show("Please close below vision’s setting form and test form before exit :\n" + strErrorMessage,
                                    "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                switch(temp[0])
                {
                    case 0:
                        m_intSelectedVisionStation = 1;
                        btn_Vision1Tab.PerformClick();
                        break;
                    case 1:
                        m_intSelectedVisionStation = 2;
                        btn_Vision2Tab.PerformClick();
                        break;
                    case 2:
                        m_intSelectedVisionStation = 3;
                        btn_Vision3Tab.PerformClick();
                        break;
                    case 3:
                        m_intSelectedVisionStation = 4;
                        btn_Vision4Tab.PerformClick();
                        break;
                    case 4:
                        m_intSelectedVisionStation = 5;
                        btn_Vision5Tab.PerformClick();
                        break;
                    case 5:
                        m_intSelectedVisionStation = 6;
                        btn_Vision6Tab.PerformClick();
                        break;
                    case 6:
                        m_intSelectedVisionStation = 7;
                        btn_Vision7Tab.PerformClick();
                        break;
                    case 7:
                        m_intSelectedVisionStation = 8;
                        btn_Vision8Tab.PerformClick();
                        break;
                }
                return;
            }


            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                m_smVSInfo[i].AT_PR_StartLiveImage = false;

                if (m_objVisionPage[i] != null)
                    m_objVisionPage[i].PauseThread();
            }

            if (m_objDisplay1Page != null)
                m_objDisplay1Page.PauseThread();
            if (m_objDisplay2Page != null)
                m_objDisplay2Page.PauseThread();
            if (m_objDisplay3Page != null)
                m_objDisplay3Page.PauseThread();

            //if (m_objIDSCamera != null)
            //    m_objIDSCamera.Dispose();

            if (m_intSelectedVisionStation > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Exit Auto Mode", "Pressed Exit Button", "", "", m_smProductionInfo.g_strLotID);
            }
            else
                STDeviceEdit.SaveDeviceEditLog("DisplayAll>Exit Auto Mode", "Pressed Exit Button", "", "", m_smProductionInfo.g_strLotID);


            if (btn_Live.Checked)
            {
                btn_Live.Checked = false;
                btn_Live.Text = GetLanguageText("Live");
                btn_Grab.Enabled = true;
                btn_LoadImage.Enabled = true;
                btn_LoadImage2.Enabled = true;
                btn_Template.Enabled = true;
            }

            m_smProductionInfo.AT_ALL_InAuto = false;
            this.Enabled = false;
            this.Hide();
        }

        private void btn_Grab_Click(object sender, EventArgs e)
        {
            if (m_intSelectedVisionStation - 1 < 0)
            {
                for (int i = 0; i < m_intVisionCount; i++)
                {
                    m_smVSInfo[i].AT_PR_GrabImage = true;
                }
            }
            else
            {
                //
                //STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + "-Grab Image", "Grab Image", m_smVSInfo[m_intSelectedVisionStation - 1].AT_PR_GrabImage.ToString(), "True", m_smProductionInfo.g_strLotID);
                //
                m_smVSInfo[m_intSelectedVisionStation - 1].AT_PR_GrabImage = true;
            }
        }

        private void btn_GRR_Click(object sender, EventArgs e)
        {
            //btn_GRR.Checked = !btn_GRR.Checked;
            btn_GRR.Checked = !m_smVSInfo[m_intSelectedVisionStation - 1].g_blnGRRON;
            
            if (!m_smVSInfo[m_intSelectedVisionStation - 1].g_blnGRRON)//(btn_GRR.Checked)
            {
                // Make sure all dialog form close
                if (m_smVSInfo[m_intSelectedVisionStation - 1].VM_AT_SettingInDialog || m_smVSInfo[m_intSelectedVisionStation - 1].VM_AT_OfflinePageView)
                {
                    btn_GRR.Checked = false;
                    SRMMessageBox.Show("Please close any setting form and test form before running GRR",
                        "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Stop camera live
                if (m_smVSInfo[m_intSelectedVisionStation - 1].AT_PR_StartLiveImage)
                {
                    //STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + "-Live Image", "Live Image", m_smVSInfo[m_intSelectedVisionStation - 1].AT_PR_StartLiveImage.ToString(), "False", m_smProductionInfo.g_strLotID);
                    m_smVSInfo[m_intSelectedVisionStation - 1].AT_PR_StartLiveImage = false;
                    m_smVSInfo[m_intSelectedVisionStation - 1].AT_PR_TriggerLiveImage = true;
                }

                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Run GRR", "GRR Status", m_smVSInfo[m_intSelectedVisionStation - 1].g_blnGRRON.ToString(), "True", m_smProductionInfo.g_strLotID);
                // Set GRR status ON
                m_smVSInfo[m_intSelectedVisionStation - 1].g_blnGRRON = true;
            }
            else
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Stop GRR", "GRR Status", m_smVSInfo[m_intSelectedVisionStation - 1].g_blnGRRON.ToString(), "False", m_smProductionInfo.g_strLotID);
                // Set GRR Status OFF
                m_smVSInfo[m_intSelectedVisionStation - 1].g_blnGRRON = false;
            }
            
        }

        private void btn_ImageNext_Click(object sender, EventArgs e)
        {
            try
            {
                string strFileName;
                VisionInfo smVisionInfo = m_smVSInfo[m_intSelectedVisionStation - 1];

                if (smVisionInfo.VM_AT_BlockImageUpdate)    // Block image update when inspection is in progress
                    return;
                //string PreviousFileName = smVisionInfo.g_arrImageFiles[smVisionInfo.g_intFileIndex].ToString();
                //string[] PreviousSplit = PreviousFileName.Split('\\');

                if (++smVisionInfo.g_intFileIndex == smVisionInfo.g_arrImageFiles.Count)
                    smVisionInfo.g_intFileIndex = 0;

                strFileName = smVisionInfo.g_arrImageFiles[smVisionInfo.g_intFileIndex].ToString();
                string[] CurrentSplit = strFileName.Split('\\');
                //
                //STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + "-Next Image", "Image File Name", PreviousSplit[PreviousSplit.Length - 1], CurrentSplit[PreviousSplit.Length - 1], m_smProductionInfo.g_strLotID);
                //
                //Image objImage = Image.FromFile(strFileName);                     // 2020 08 08 - CCENG: No use

                if (!smVisionInfo.g_blnViewColorImage)
                {
                    smVisionInfo.g_arrImages[0].LoadImage(strFileName);
                    for (int i = 1; i < smVisionInfo.g_arrImages.Count; i++)
                    {
                        string strDirPath = Path.GetDirectoryName(strFileName);
                        string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + i.ToString() + ".BMP";

                        if (File.Exists(strPkgView))
                            smVisionInfo.g_arrImages[i].LoadImage(strPkgView);
                        else
                            smVisionInfo.g_arrImages[i].LoadImage(strFileName);
                    }
                }
                else
                {
                    smVisionInfo.g_arrColorImages[0].LoadImage(strFileName);
                    smVisionInfo.g_arrColorImages[0].ConvertColorToMono(ref smVisionInfo.g_arrImages, 0);
                    for (int i = 1; i < smVisionInfo.g_arrColorImages.Count; i++)
                    {
                        string strDirPath = Path.GetDirectoryName(strFileName);
                        string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + i.ToString() + ".BMP";

                        if (File.Exists(strPkgView))
                            smVisionInfo.g_arrColorImages[i].LoadImage(strPkgView);
                        else
                            smVisionInfo.g_arrColorImages[i].LoadImage(strFileName);

                        smVisionInfo.g_arrColorImages[i].ConvertColorToMono(ref smVisionInfo.g_arrImages, i);
                    }
                    
                }

                smVisionInfo.g_blnLoadFile = true;

                if (smVisionInfo.AT_VM_ManualTestMode)
                {
                    smVisionInfo.MN_PR_StartTest = true;
                    smVisionInfo.VM_AT_BlockImageUpdate = true;
                }
                else
                {
                    smVisionInfo.AT_PR_AttachImagetoROI = true;
                    smVisionInfo.g_blnClearResult = true;
                }

                if (smVisionInfo.g_bImageStatisticAnalysisON)
                {
                    smVisionInfo.g_bImageStatisticAnalysisUpdateInfo = true;
                }

                if (smVisionInfo.g_bPocketPositionMeanStatisticAnalysisON)
                {
                    smVisionInfo.g_bPocketPositionMeanStatisticAnalysisUpdateInfo = true;
                }
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Fail to load next image: " + ex.ToString());
            }
        }

        private void btn_ImagePrev_Click(object sender, EventArgs e)
        {
            try
            {
                string strFileName;
                VisionInfo smVisionInfo = m_smVSInfo[m_intSelectedVisionStation - 1];

                if (smVisionInfo.VM_AT_BlockImageUpdate)    // Block image update when inspection is in progress
                    return;

                //string PreviousFileName = smVisionInfo.g_arrImageFiles[smVisionInfo.g_intFileIndex].ToString();
                //string[] PreviousSplit = PreviousFileName.Split('\\');
                if (--smVisionInfo.g_intFileIndex < 0)
                    smVisionInfo.g_intFileIndex = smVisionInfo.g_arrImageFiles.Count - 1;

                strFileName = smVisionInfo.g_arrImageFiles[smVisionInfo.g_intFileIndex].ToString();
                //string[] CurrentSplit = strFileName.Split('\\');
                //
                //STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + "-Previous Image", "Image File Name", PreviousSplit[PreviousSplit.Length - 1], CurrentSplit[PreviousSplit.Length - 1], m_smProductionInfo.g_strLotID);
                //
                //Image objImage = Image.FromFile(strFileName);                         // 2020 08 08 - CCENG: No use


                if (!smVisionInfo.g_blnViewColorImage)
                {
                    smVisionInfo.g_arrImages[0].LoadImage(strFileName);
                    for (int i = 1; i < smVisionInfo.g_arrImages.Count; i++)
                    {
                        string strDirPath = Path.GetDirectoryName(strFileName);
                        string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + i.ToString() + ".BMP";

                        if (File.Exists(strPkgView))
                            smVisionInfo.g_arrImages[i].LoadImage(strPkgView);
                        else
                            smVisionInfo.g_arrImages[i].LoadImage(strFileName);
                    }
                }
                else
                {
                    smVisionInfo.g_arrColorImages[0].LoadImage(strFileName);
                    smVisionInfo.g_arrColorImages[0].ConvertColorToMono(ref smVisionInfo.g_arrImages, 0);
                    for (int i = 1; i < smVisionInfo.g_arrColorImages.Count; i++)
                    {
                        string strDirPath = Path.GetDirectoryName(strFileName);
                        string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + i.ToString() + ".BMP";

                        if (File.Exists(strPkgView))
                            smVisionInfo.g_arrColorImages[i].LoadImage(strPkgView);
                        else
                            smVisionInfo.g_arrColorImages[i].LoadImage(strFileName);

                        smVisionInfo.g_arrColorImages[i].ConvertColorToMono(ref smVisionInfo.g_arrImages, i);
                    }
                    
                }

                smVisionInfo.g_blnLoadFile = true;

                if (smVisionInfo.AT_VM_ManualTestMode)
                {
                    smVisionInfo.MN_PR_StartTest = true;
                    smVisionInfo.VM_AT_BlockImageUpdate = true;
                }
                else
                {
                    smVisionInfo.AT_PR_AttachImagetoROI = true;
                    smVisionInfo.g_blnClearResult = true;
                }

                if (smVisionInfo.g_bImageStatisticAnalysisON)
                {
                    smVisionInfo.g_bImageStatisticAnalysisUpdateInfo = true;
                }

                if (smVisionInfo.g_bPocketPositionMeanStatisticAnalysisON)
                {
                    smVisionInfo.g_bPocketPositionMeanStatisticAnalysisUpdateInfo = true;
                }

            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Fail to load previous image: " + ex.ToString());
            }
        }

        private void btn_Live_Click(object sender, EventArgs e)
        {
            btn_Live.Checked = !btn_Live.Checked;

            bool blnLive = false;
            if (btn_Live.Checked)
                blnLive = true;

            int intVisionNo = m_intSelectedVisionStation - 1;
            if (intVisionNo < 0)
            {
                for (int i = 0; i < m_intVisionCount; i++)
                {
                    m_smVSInfo[i].AT_PR_StartLiveImage = blnLive;
                    EnableLiveImage(m_smVSInfo[i], i);
                }
            }
            else
            {

                //
                //STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[intVisionNo].g_strVisionFolderName + "-Live Image", "Live Image", m_smVSInfo[intVisionNo].AT_PR_StartLiveImage.ToString(), blnLive.ToString(), m_smProductionInfo.g_strLotID);
                //
                m_smVSInfo[intVisionNo].AT_PR_StartLiveImage = blnLive;
                EnableLiveImage(m_smVSInfo[intVisionNo], intVisionNo);
            }
        }

        private void btn_LoadImage_Click(object sender, EventArgs e)
        {
            VisionInfo smVisionInfo = m_smVSInfo[m_intSelectedVisionStation - 1];

            string strPath = smVisionInfo.g_strSaveImageLocation +
             m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime;

            //string strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")";

            string strVisionImageFolderName;
            if (smVisionInfo.g_intVisionResetCount == 0)
                strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")" + "_" + m_smProductionInfo.g_strLotStartTime;
            else
                strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")" + "_" + smVisionInfo.g_strVisionResetCountTime;

            dlg_ImageFile.InitialDirectory = strPath + "\\" + strVisionImageFolderName + "\\";

            if (dlg_ImageFile.ShowDialog() == DialogResult.OK)
            {
                string strFileName = dlg_ImageFile.FileName;

                // Filter the string "_Image#"
                if (strFileName.ToLower().LastIndexOf("_image1.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image2.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image3.bmp") > 0
                    || strFileName.ToLower().LastIndexOf("_image4.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image5.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image6.bmp") > 0)
                {
                    strFileName = strFileName.Substring(0, strFileName.IndexOf("_Image")) + ".bmp";
                }

                string strDirectoryName = Path.GetDirectoryName(strFileName);
                int i = 0;
                smVisionInfo.g_arrImageFiles.Clear();
                smVisionInfo.g_intFileIndex = -1;
                bool blnEnable = false;

                string[] strImageFiles = Directory.GetFiles(strDirectoryName, "*.bmp");

                // Sorting
                //string[] strImageFiles2 = new string[strImageFiles.Length];
                //for (int j = 0; j < 100; j++)
                //{
                //    strImageFiles2[j] = strImageFiles[j];
                //}
                // Array.Sort(strImageFiles, m_objTimeComparer.CompareCreateAscending);

                int intImageLimit = 30;
                int intTotalLoadImageLimit = strImageFiles.Length;
                bool blnLoadAllImage = true;
                bool blnCheckSkipImage = false;
                if ((strImageFiles.Length / smVisionInfo.g_arrImages.Count) > intImageLimit)
                {
                    if (SRMMessageBox.Show("Too many images in this folder. Do you want to load all the images? \nPress No will load " + intImageLimit.ToString() + " images only.", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        intTotalLoadImageLimit = intImageLimit;
                        blnLoadAllImage = false;
                        blnCheckSkipImage = true;
                    }
                }

                NumericComparer ns = new NumericComparer();
                Array.Sort(strImageFiles, ns);

                //
                //STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + "-Load Image", "Image File Name", "", strImageFiles[0], m_smProductionInfo.g_strLotID);
                //
                int intSkipImageCount = 0;
                foreach (string strName in strImageFiles)
                {
                    if (strName.IndexOf("_Image") >= 0)
                        continue;

                    try
                    {
                        if (!blnLoadAllImage && blnCheckSkipImage)
                        {
                            if (strName.ToLower() == strFileName.ToLower())
                                blnCheckSkipImage = false;
                            else
                            {
                                if (((int)Math.Ceiling((float)strImageFiles.Length / smVisionInfo.g_arrImages.Count) - intSkipImageCount) > intTotalLoadImageLimit)
                                {
                                    intSkipImageCount++;
                                    continue;
                                }
                                else
                                    blnCheckSkipImage = false;
                            }
                        }

                        Image objImage = Image.FromFile(strName);

                        if (PixelFormat.Format8bppIndexed == objImage.PixelFormat &&
                            objImage.Width == smVisionInfo.g_intCameraResolutionWidth &&
                            objImage.Height == smVisionInfo.g_intCameraResolutionHeight)
                        {
                            smVisionInfo.g_arrImageFiles.Add(strName);
                            if (strName.ToLower() == strFileName.ToLower())
                            {
                                smVisionInfo.g_intFileIndex = i;
                                smVisionInfo.g_arrImages[0].LoadImage(strFileName);

                                for (int x = 1; x < smVisionInfo.g_arrImages.Count; x++)
                                {
                                    string strDirPath = Path.GetDirectoryName(strFileName);
                                    string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + x.ToString() + ".BMP";

                                    if (File.Exists(strPkgView))
                                        smVisionInfo.g_arrImages[x].LoadImage(strPkgView);
                                    else
                                        smVisionInfo.g_arrImages[x].LoadImage(strFileName);
                                }

                                smVisionInfo.AT_PR_AttachImagetoROI = true;
                                smVisionInfo.g_blnLoadFile = true;
                            }
                            i++;
                        }
                        else if (smVisionInfo.g_blnViewColorImage && objImage.Width == smVisionInfo.g_intCameraResolutionWidth && objImage.Height == smVisionInfo.g_intCameraResolutionHeight)
                        {
                            smVisionInfo.g_arrImageFiles.Add(strName);
                            if (strName.ToLower() == strFileName.ToLower())
                            {
                                smVisionInfo.g_intFileIndex = i;
                                smVisionInfo.g_arrColorImages[0].LoadImage(strFileName);
                                smVisionInfo.g_arrColorImages[0].ConvertColorToMono(ref smVisionInfo.g_arrImages, 0);
                                for (int x = 1; x < smVisionInfo.g_arrColorImages.Count; x++)
                                {
                                    string strDirPath = Path.GetDirectoryName(strFileName);
                                    string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + x.ToString() + ".BMP";

                                    if (File.Exists(strPkgView))
                                        smVisionInfo.g_arrColorImages[x].LoadImage(strPkgView);
                                    else
                                        smVisionInfo.g_arrColorImages[x].LoadImage(strFileName);
                                    smVisionInfo.g_arrColorImages[x].ConvertColorToMono(ref smVisionInfo.g_arrImages, x);
                                }

                                smVisionInfo.AT_PR_AttachImagetoROI = true;
                                smVisionInfo.g_blnLoadFile = true;
                            }
                            i++;
                        }

                        objImage.Dispose();

                        if (i >= intTotalLoadImageLimit)
                            break;
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (i > 1)
                    blnEnable = true;
                if (smVisionInfo.g_intFileIndex == -1)
                {
                    SRMMessageBox.Show("Invalid File Format", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    smVisionInfo.g_blnLoadFile = false;
                }

                smVisionInfo.g_blnViewRotatedImage = false;
                btn_ImageNext.Enabled = blnEnable;
                btn_ImagePrev.Enabled = blnEnable;
            }

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void btn_NewLot_Click(object sender, EventArgs e)
        {
            string strErrorMessage = "";
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                //2020-06-02 ZJYEOH : Only force user to close setting form, offline test form will auto hide
                if (m_smVSInfo[i].VM_AT_SettingInDialog)// || m_smVSInfo[i].VM_AT_OfflinePageView)
                {
                    strErrorMessage += "- " + m_smVSInfo[i].g_strVisionDisplayName + "\n";
                }

                if (m_smVSInfo[i].VM_AT_OfflinePageView)
                {
                    m_objVisionPage[i].HideOfflinePage();
                }
            }

            if (strErrorMessage.Length > 0)
            {
                SRMMessageBox.Show("Please close below visions' setting form and test form before create new lot / end lot :\n" + strErrorMessage,
                                    "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (m_smCustomizeInfo.g_blnOnlyNewLot)
            {
                //only new lot
                Cursor.Current = Cursors.WaitCursor;
                NewLotForm objForm = new NewLotForm(m_smProductionInfo, m_smCustomizeInfo);
                if (objForm.ShowDialog() == DialogResult.OK)
                {
                    if (!m_smProductionInfo.g_blnEndLotStatus)
                    {
                        
                        if (m_intSelectedVisionStation > 0)
                            STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + "> New Lot", "New Lot", "", "", m_smProductionInfo.g_strLotID);
                        else
                            STDeviceEdit.SaveDeviceEditLog("DisplayAll> New Lot", "New Lot", "", "", m_smProductionInfo.g_strLotID);
                        
                        CreateNewLot();
                    }
                    else
                    {
                        
                        if (m_intSelectedVisionStation > 0)
                            STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + "> New Lot", "New Lot", "", "", m_smProductionInfo.g_strLotID);
                        else
                            STDeviceEdit.SaveDeviceEditLog("DisplayAll> New Lot", "New Lot", "", "", m_smProductionInfo.g_strLotID);
                        
                        ProcessNewLot();
                    }
                    if (!btn_ProductionAll.Enabled)
                        btn_ProductionAll.Enabled = true;
                }

                objForm.Dispose();
            }
            else
            {
                //new lot and end lot
                if (m_smProductionInfo.g_blnEndLotStatus)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    NewLotForm objForm = new NewLotForm(m_smProductionInfo, m_smCustomizeInfo);
                    if (objForm.ShowDialog() == DialogResult.OK)
                    {
                        
                        if (m_intSelectedVisionStation > 0)
                            STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + "> New Lot", "New Lot", "", "", m_smProductionInfo.g_strLotID);
                        else
                            STDeviceEdit.SaveDeviceEditLog("DisplayAll> New Lot", "New Lot", "", "", m_smProductionInfo.g_strLotID);
                        
                        ProcessNewLot();
                        if (!btn_ProductionAll.Enabled)
                            btn_ProductionAll.Enabled = true;
                    }

                    objForm.Dispose();
                }
                else
                {
                    if (SRMMessageBox.Show("Are you sure you want to end the lot?", "End Lot", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                    {
                        m_smProductionInfo.g_intVisionInvolved = 0;
                        if (m_intSelectedVisionStation > 0)
                            STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + "> End Lot", "End Lot", "", "", m_smProductionInfo.g_strLotID);
                        else
                            STDeviceEdit.SaveDeviceEditLog("DisplayAll> End Lot", "End Lot", "", "", m_smProductionInfo.g_strLotID);
                        
                        ProcessEndLot();
                    }
                }
            }
        }

        private void btn_Production_Click(object sender, EventArgs e)
        {
            btn_Production.Checked = !btn_Production.Checked;

            
            if (btn_Production.Checked)
            {
                btn_Production.Text = GetLanguageText("Stop");

                m_smVSInfo[m_intSelectedVisionStation - 1].g_arrblnImageRotated[0] = false;
                m_smVSInfo[m_intSelectedVisionStation - 1].g_arrblnImageRotated[1] = false;
                m_smVSInfo[m_intSelectedVisionStation - 1].g_arrblnImageRotated[2] = false;
                m_smVSInfo[m_intSelectedVisionStation - 1].g_arrblnImageRotated[3] = false;
                m_smVSInfo[m_intSelectedVisionStation - 1].g_arrblnImageRotated[4] = false;
                m_smVSInfo[m_intSelectedVisionStation - 1].g_arrblnImageRotated[5] = false;
                m_smVSInfo[m_intSelectedVisionStation - 1].g_arrblnImageRotated[6] = false;

                if (m_smVSInfo[m_intSelectedVisionStation - 1].AT_PR_StartLiveImage)
                {
                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Live Image", "Live Image", m_smVSInfo[m_intSelectedVisionStation - 1].AT_PR_StartLiveImage.ToString(), "False", m_smProductionInfo.g_strLotID);
                    btn_Production.Checked = false;
                    btn_Production.Text = GetLanguageText("Production");
                    m_smVSInfo[m_intSelectedVisionStation - 1].AT_PR_StartLiveImage = false;
                    m_smVSInfo[m_intSelectedVisionStation - 1].AT_PR_TriggerLiveImage = true;
                }

                //Check end lot status here
                if (m_smProductionInfo.g_blnEndLotStatus)
                {
                    btn_Production.Checked = false;
                    btn_Production.Text = GetLanguageText("Production");
                    SRMMessageBox.Show("Please start new lot before running production.",
                        "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2020 03 13 - JBTAN: offline page will auto close when user click production
                if (m_smVSInfo[m_intSelectedVisionStation - 1].VM_AT_OfflinePageView)
                {
                    m_objVisionPage[m_intSelectedVisionStation - 1].HideOfflinePage();
                }

                if (m_smVSInfo[m_intSelectedVisionStation - 1].VM_AT_SettingInDialog)// || m_smVSInfo[m_intSelectedVisionStation - 1].VM_AT_OfflinePageView)
                {
                    btn_Production.Checked = false;
                    btn_Production.Text = GetLanguageText("Production");
                    SRMMessageBox.Show("Please close vision's setting form and test form before running production",
                        "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ////Check Template here
                //if (m_smVSInfo[m_intSelectedVisionStation - 1].VM_AT_TemplateNotLearn)
                //{
                //    btn_Production.Checked = false;
                //    btn_Production.Text = GetLanguageText("Production");
                //    SRMMessageBox.Show("Please make sure mark template learnt before running production.",
                //        "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}

                string strFailVisionName = CheckControlSetting(false);
                if (strFailVisionName != "")
                {
                    btn_Production.Checked = false;
                    btn_Production.Text = GetLanguageText("Production");
                    //SRMMessageBox.Show("Please make sure " + strFailVisionName + " inspection option (*) are selected before running production.",
                    //    "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SRMMessageBox.Show(LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, "Please make sure inspection option (*) are selected before running production.\n -" + strFailVisionName),
                     "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subKey = key.CreateSubKey("SVG\\Visions");

                // 2021 01 25 - CCENG: Add lower() so that able to handler upper and lower case of "TRUE" string.
                //if (!CheckFailOptionSecureSetting(false, m_intSelectedVisionStation, (bool)subKey.GetValue("SecureOnOff", true).Equals("True")))
                if (!CheckFailOptionSecureSetting(false, m_intSelectedVisionStation, (bool)subKey.GetValue("SecureOnOff", false).ToString().ToLower().Equals("true")))
                {
                    btn_Production.Checked = false;
                    btn_Production.Text = GetLanguageText("Production");
                    return;
                }

                if (m_smProductionInfo.g_blnWantRecipeVerification)
                {
                    if (m_smVSInfo[m_intSelectedVisionStation - 1].AT_PR_StartLiveImage)
                    {
                        m_smVSInfo[m_intSelectedVisionStation - 1].AT_PR_StartLiveImage = false;
                        m_smVSInfo[m_intSelectedVisionStation - 1].AT_PR_TriggerLiveImage = true;
                    }
                    m_smVSInfo[m_intSelectedVisionStation - 1].AT_VM_ManualTestMode = true;
                    
                    m_smVSInfo[m_intSelectedVisionStation - 1].PR_MN_TestDone = false;
                    m_smVSInfo[m_intSelectedVisionStation - 1].MN_PR_StartTest_Verification = true;
                    WaitEventDone(ref m_smVSInfo[m_intSelectedVisionStation - 1].PR_MN_TestDone, true, 10000, "Recipe Verification");

                    RecipeVerificationForm objForm = new RecipeVerificationForm(m_smProductionInfo, m_smVSInfo, m_smCustomizeInfo, m_intSelectedVisionStation - 1);

                    if (objForm.ShowDialog() == DialogResult.Cancel)
                    {
                        btn_Production.Checked = false;
                        btn_Production.Text = GetLanguageText("Production");
                        objForm.Close();
                        objForm.Dispose();
                        return;
                    }
                    objForm.Close();
                    objForm.Dispose();
                }

                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + "> Start Production", "Machine Status", m_smVSInfo[m_intSelectedVisionStation - 1].g_intMachineStatus.ToString(), "2", m_smProductionInfo.g_strLotID);
                m_smVSInfo[m_intSelectedVisionStation - 1].g_intMachineStatus = 2;

                //NavigationBar_Auto.SelectedPane.LargeImage = imageList_AutoForm.Images[1];
                //NavigationBar_Auto.SelectedPane.Controls[0].Text = "Operating";
                //NavigationBar_Auto.SelectedPane.Controls[0].BackColor = Color.Lime;

                // 2020 04 18 - the image index will be changed in timer ChangeSPC function
                //switch (m_intSelectedVisionStation)
                //{
                //    case 1:
                //        btn_Vision1Tab.ImageIndex = 1;
                //        break;
                //    case 2:
                //        btn_Vision2Tab.ImageIndex = 1;
                //        break;
                //    case 3:
                //        btn_Vision3Tab.ImageIndex = 1;
                //        break;
                //    case 4:
                //        btn_Vision4Tab.ImageIndex = 1;
                //        break;
                //    case 5:
                //        btn_Vision5Tab.ImageIndex = 1;
                //        break;
                //    case 6:
                //        btn_Vision6Tab.ImageIndex = 1;
                //        break;
                //    case 7:
                //        btn_Vision7Tab.ImageIndex = 1;
                //        break;
                //    case 8:
                //        btn_Vision8Tab.ImageIndex = 1;
                //        break;
                //    case 9:
                //        btn_Vision9Tab.ImageIndex = 1;
                //        break;
                //    case 10:
                //        btn_Vision10Tab.ImageIndex = 1;
                //        break;
                //}
                IndividualAutoMode(false, m_intSelectedVisionStation - 1);
                bool blnAllProduction = true;
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    if (m_smVSInfo[i].g_intMachineStatus != 2 && blnAllProduction)
                    {
                        blnAllProduction = false;
                        break;
                    }
                }
                if (blnAllProduction && !btn_ProductionAll.Checked)
                {
                    btn_ProductionAll.Checked = true;
                    btn_ProductionAll.Text = GetLanguageText("Stop All");
                }
            }
            else
            {
                btn_Production.Text = GetLanguageText("Production");
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + "> Stop Production", "Machine Status", m_smVSInfo[m_intSelectedVisionStation - 1].g_intMachineStatus.ToString(), "1", m_smProductionInfo.g_strLotID);
                m_smVSInfo[m_intSelectedVisionStation - 1].g_intMachineStatus = 1;

                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subKey = key.CreateSubKey("SVG\\AutoMode");

                string LastImageFolder = subKey.GetValue("LastImageFolder", "").ToString();
                string LastImageName = subKey.GetValue("LastImageName", "").ToString();

                subKey.SetValue("VS" + (m_intSelectedVisionStation) + "LastImageFolder", m_smVSInfo[m_intSelectedVisionStation - 1].g_strLastImageFolder);
                subKey.SetValue("VS" + (m_intSelectedVisionStation) + "LastImageName", m_smVSInfo[m_intSelectedVisionStation - 1].g_strLastImageName);

                //

                //if (LastImageFolder!= m_smVSInfo[m_intSelectedVisionStation - 1].g_strLastImageFolder)
                //    STDeviceEdit.SaveDeviceEditLog("VS" + (m_intSelectedVisionStation).ToString(), "LastImageFolder", LastImageFolder, m_smVSInfo[m_intSelectedVisionStation - 1].g_strLastImageFolder);
                //if (LastImageName != m_smVSInfo[m_intSelectedVisionStation - 1].g_strLastImageName)
                //    STDeviceEdit.SaveDeviceEditLog("VS" + (m_intSelectedVisionStation).ToString(), "LastImageName", LastImageName, m_smVSInfo[m_intSelectedVisionStation - 1].g_strLastImageName);

                // 2020 04 18 - the image index will be changed in timer ChangeSPC function
                //switch (m_intSelectedVisionStation)
                //{
                //    case 1:
                //        btn_Vision1Tab.ImageIndex = 0;
                //        break;
                //    case 2:
                //        btn_Vision2Tab.ImageIndex = 0;
                //        break;
                //    case 3:
                //        btn_Vision3Tab.ImageIndex = 0;
                //        break;
                //    case 4:
                //        btn_Vision4Tab.ImageIndex = 0;
                //        break;
                //    case 5:
                //        btn_Vision5Tab.ImageIndex = 0;
                //        break;
                //    case 6:
                //        btn_Vision6Tab.ImageIndex = 0;
                //        break;
                //    case 7:
                //        btn_Vision7Tab.ImageIndex = 0;
                //        break;
                //    case 8:
                //        btn_Vision8Tab.ImageIndex = 0;
                //        break;
                //    case 9:
                //        btn_Vision9Tab.ImageIndex = 0;
                //        break;
                //    case 10:
                //        btn_Vision10Tab.ImageIndex = 0;
                //        break;
                //}

                IndividualAutoMode(true, m_intSelectedVisionStation - 1);

                if (btn_ProductionAll.Checked)
                {
                    btn_ProductionAll.Checked = false;
                    btn_ProductionAll.Text = GetLanguageText("Prod All");
                }

                if (m_smCustomizeInfo.g_blnSavePassImage || m_smCustomizeInfo.g_blnSaveFailImage)
                {
                    VisionInfo smVisionInfo = m_smVSInfo[m_intSelectedVisionStation - 1];

                    if (!smVisionInfo.g_blnDebugRUN)
                    {
                        string strPath = smVisionInfo.g_strSaveImageLocation +
                             m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime;

                        //string strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")";

                        string strVisionImageFolderName;
                        if (smVisionInfo.g_intVisionResetCount == 0)
                            strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")" + "_" + m_smProductionInfo.g_strLotStartTime;
                        else
                            strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")" + "_" + smVisionInfo.g_strVisionResetCountTime;

                        string Path = strPath + "\\" + strVisionImageFolderName;

                        if (Directory.Exists(Path))
                        {
                            string[] dir = Directory.GetDirectories(Path);
                            bool FirstTime = true;
                            foreach (string dirs in dir)
                            {
                                string[] Files = Directory.GetFiles(dirs);

                                //foreach (string imageName in Files)
                                //{
                                if (Files.Length > 0)
                                {
                                    if (FirstTime)
                                    {
                                        smVisionInfo.g_arrImageFiles.Clear();
                                        smVisionInfo.g_intFileIndex = -1;
                                    }
                                    LoadCurrentLotImages_AfterStopProduction(dirs + "\\", FirstTime, m_intSelectedVisionStation - 1);
                                    FirstTime = false;
                                }
                                //}
                            }
                        }

                        SortAccordingToDateTime(ref m_smVSInfo[m_intSelectedVisionStation - 1].g_arrImageFiles);
                    }
                }
            }
            
        }
        private void SortAccordingToDateTime(ref ArrayList FileNames)
        {
            List<DateTime> dt = new List<DateTime>(FileNames.Count);
            List<DateTime> Sortdt = new List<DateTime>(FileNames.Count);
            foreach (string strName in FileNames)
            {
                dt.Add(Directory.GetCreationTime(strName));
                //Sortdt.Add(Directory.GetCreationTime(strName));
            }
            for (int i = 0; i < dt.Count; i++)
            {
                int intIndex = Sortdt.Count;
                for (int j = 0; j < Sortdt.Count; j++)
                {
                    if (DateTime.Compare(Sortdt[j], dt[i]) > 0)
                    {
                        intIndex = j;
                        break;
                    }
                }

                string str = FileNames[i].ToString();
                FileNames.RemoveAt(i);
                FileNames.Insert(intIndex, str);
                Sortdt.Insert(intIndex, dt[i]);
            }
        }

        private void btn_ProductionAll_Click(object sender, EventArgs e)
        {
            m_blnBlockProductionCheckStatus = true;
            btn_ProductionAll.Checked = !btn_ProductionAll.Checked;

            
            if (btn_ProductionAll.Checked)
            {
                btn_ProductionAll.Text = GetLanguageText("Stop All");

                //Check end lot status here
                if (m_smProductionInfo.g_blnEndLotStatus)
                {
                    btn_Production.Checked = false;
                    btn_Production.Text = GetLanguageText("Production");
                    SRMMessageBox.Show("Please start new lot before running production.",
                        "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    m_blnBlockProductionCheckStatus = false;
                    return;
                }

                string strErrorMessage = "";
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    m_smVSInfo[i].g_arrblnImageRotated[0] = false;
                    m_smVSInfo[i].g_arrblnImageRotated[1] = false;
                    m_smVSInfo[i].g_arrblnImageRotated[2] = false;
                    m_smVSInfo[i].g_arrblnImageRotated[3] = false;
                    m_smVSInfo[i].g_arrblnImageRotated[4] = false;
                    m_smVSInfo[i].g_arrblnImageRotated[5] = false;
                    m_smVSInfo[i].g_arrblnImageRotated[6] = false;

                    // 2020 03 13 - JBTAN: offline page will auto close when user click production
                    if (m_smVSInfo[i].VM_AT_OfflinePageView)
                    {
                        m_objVisionPage[i].HideOfflinePage();
                    }

                    if (m_smVSInfo[i].VM_AT_SettingInDialog)// || m_smVSInfo[i].VM_AT_OfflinePageView)
                    {
                        strErrorMessage += "- " + m_smVSInfo[i].g_strVisionDisplayName + "\n";
                    }

                    ////Check Template here
                    //if (m_smVSInfo[i].VM_AT_TemplateNotLearn)
                    //{
                    //    btn_Production.Checked = false;
                    //    btn_Production.Text = GetLanguageText("Production");
                    //    SRMMessageBox.Show("Please make sure mark template learnt before running production.",
                    //        "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    return;
                    //}
                }

                if (strErrorMessage.Length > 0)
                {
                    btn_ProductionAll.Checked = false;
                    btn_ProductionAll.Text = GetLanguageText("Prod All");
                    SRMMessageBox.Show("Please close below visions' setting form and test form before running production :\n" + strErrorMessage,
                                        "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    m_blnBlockProductionCheckStatus = false;
                    return;
                }

                string strFailVisionName = CheckControlSetting(true);
                if (strFailVisionName != "")
                {
                    btn_ProductionAll.Checked = false;
                    btn_ProductionAll.Text = GetLanguageText("Prod All");
                    //SRMMessageBox.Show("Please make sure " + strFailVisionName + " inspection option (*) are selected before running production.",
                    //    "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SRMMessageBox.Show(LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, "Please make sure inspection option (*) are selected before running production.\n -") + strFailVisionName,
                    "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    m_blnBlockProductionCheckStatus = false;
                    return;
                }

                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subKey = key.CreateSubKey("SVG\\Visions");

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {

                    if (m_smVSInfo[i] == null)
                        continue;

                    // 2021 01 25 - CCENG: Add lower() so that able to handler upper and lower case of "TRUE" string.
                    //if (!CheckFailOptionSecureSetting(true, i, (bool)subKey.GetValue("SecureOnOff", true).Equals("True")))
                    if (!CheckFailOptionSecureSetting(true, i, (bool)subKey.GetValue("SecureOnOff", false).ToString().ToLower().Equals("true")))
                    {
                        btn_ProductionAll.Checked = false;
                        btn_ProductionAll.Text = GetLanguageText("Prod All");
                        m_blnBlockProductionCheckStatus = false;
                        return;
                    }
                }


                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    if (m_smVSInfo[i].AT_PR_StartLiveImage)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[i].g_strVisionFolderName + ">Live Image", "Live Image", m_smVSInfo[i].AT_PR_StartLiveImage.ToString(), "False", m_smProductionInfo.g_strLotID);
                        m_smVSInfo[i].AT_PR_StartLiveImage = false;
                        m_smVSInfo[i].AT_PR_TriggerLiveImage = true;
                    }
                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[i].g_strVisionFolderName + "> Start All Production", "Machine Status", m_smVSInfo[i].g_intMachineStatus.ToString(), "2", m_smProductionInfo.g_strLotID);
                    m_smVSInfo[i].g_intMachineStatus = 2;
                }

                //NavigationPane_Vision1.LargeImage = //NavigationPane_Vision2.LargeImage = //NavigationPane_Vision3.LargeImage =
                //NavigationPane_Vision4.LargeImage = //NavigationPane_Vision5.LargeImage = //NavigationPane_Vision6.LargeImage =
                //NavigationPane_Vision7.LargeImage = //NavigationPane_Vision8.LargeImage = //NavigationPane_Vision9.LargeImage =
                //NavigationPane_Vision10.LargeImage = imageList_AutoForm.Images[1];

                // 2020 04 18 - the image index will be changed in timer ChangeSPC function
                //btn_Vision1Tab.ImageIndex = btn_Vision2Tab.ImageIndex = btn_Vision3Tab.ImageIndex = btn_Vision4Tab.ImageIndex =
                //btn_Vision5Tab.ImageIndex = btn_Vision6Tab.ImageIndex = btn_Vision7Tab.ImageIndex = btn_Vision8Tab.ImageIndex =
                //btn_Vision9Tab.ImageIndex = btn_Vision10Tab.ImageIndex = 1;

                lbl_Vision1Status.Text = "Operating";
                //lbl_Vision1Status.BackColor = Color.Lime;
                lbl_Vision2Status.Text = "Operating";
                //lbl_Vision2Status.BackColor = Color.Lime;
                lbl_Vision3Status.Text = "Operating";
                //lbl_Vision3Status.BackColor = Color.Lime;
                lbl_Vision4Status.Text = "Operating";
                //lbl_Vision4Status.BackColor = Color.Lime;
                lbl_Vision5Status.Text = "Operating";
                //lbl_Vision5Status.BackColor = Color.Lime;
                lbl_Vision6Status.Text = "Operating";
                //lbl_Vision6Status.BackColor = Color.Lime;
                lbl_Vision7Status.Text = "Operating";
                //lbl_Vision7Status.BackColor = Color.Lime;
                lbl_Vision8Status.Text = "Operating";
                //lbl_Vision8Status.BackColor = Color.Lime;
                lbl_Vision9Status.Text = "Operating";
                //lbl_Vision9Status.BackColor = Color.Lime;
                lbl_Vision10Status.Text = "Operating";
                //lbl_Vision10Status.BackColor = Color.Lime;
            }
            else
            {
                btn_ProductionAll.Text = GetLanguageText("Prod All");

                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subKey = key.CreateSubKey("SVG\\AutoMode");

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[i].g_strVisionFolderName + "> Stop All Production", "Machine Status", m_smVSInfo[i].g_intMachineStatus.ToString(), "1", m_smProductionInfo.g_strLotID);
                    m_smVSInfo[i].g_intMachineStatus = 1;

                    string LastImageFolder = subKey.GetValue("LastImageFolder", "").ToString();
                    string LastImageName = subKey.GetValue("LastImageName", "").ToString();

                    subKey.SetValue("VS" + (i + 1) + "LastImageFolder", m_smVSInfo[i].g_strLastImageFolder);
                    subKey.SetValue("VS" + (i + 1) + "LastImageName", m_smVSInfo[i].g_strLastImageName);

                    //

                    //if (LastImageFolder != m_smVSInfo[i].g_strLastImageFolder)
                    //    STDeviceEdit.SaveDeviceEditLog("VS" + (i + 1).ToString(), "LastImageFolder", LastImageFolder, m_smVSInfo[i].g_strLastImageFolder);
                    //if (LastImageName != m_smVSInfo[i].g_strLastImageName)
                    //    STDeviceEdit.SaveDeviceEditLog("VS" + (i + 1).ToString(), "LastImageName", LastImageName, m_smVSInfo[i].g_strLastImageName);

                }

                //NavigationPane_Vision1.LargeImage = imageList_AutoForm.Images[0];
                //NavigationPane_Vision2.LargeImage = imageList_AutoForm.Images[0];
                //NavigationPane_Vision3.LargeImage = imageList_AutoForm.Images[0];
                //NavigationPane_Vision4.LargeImage = imageList_AutoForm.Images[0];
                //NavigationPane_Vision5.LargeImage = imageList_AutoForm.Images[0];
                //NavigationPane_Vision6.LargeImage = imageList_AutoForm.Images[0];
                //NavigationPane_Vision7.LargeImage = imageList_AutoForm.Images[0];
                //NavigationPane_Vision8.LargeImage = imageList_AutoForm.Images[0];
                //NavigationPane_Vision9.LargeImage = imageList_AutoForm.Images[0];
                //NavigationPane_Vision10.LargeImage = imageList_AutoForm.Images[0];

                // 2020 04 18 - the image index will be changed in timer ChangeSPC function
                //btn_Vision1Tab.ImageIndex = btn_Vision2Tab.ImageIndex = btn_Vision3Tab.ImageIndex = btn_Vision4Tab.ImageIndex =
                //btn_Vision5Tab.ImageIndex = btn_Vision6Tab.ImageIndex = btn_Vision7Tab.ImageIndex = btn_Vision8Tab.ImageIndex =
                //btn_Vision9Tab.ImageIndex = btn_Vision10Tab.ImageIndex = 0;

                if (lbl_Vision1Status.Text != "Idle") lbl_Vision1Status.Text = "Idle";
                //lbl_Vision1Status.BackColor = Color.Yellow;
                if (lbl_Vision2Status.Text != "Idle") lbl_Vision2Status.Text = "Idle";
                //lbl_Vision2Status.BackColor = Color.Yellow;
                if (lbl_Vision3Status.Text != "Idle") lbl_Vision3Status.Text = "Idle";
                //lbl_Vision3Status.BackColor = Color.Yellow;
                if (lbl_Vision4Status.Text != "Idle") lbl_Vision4Status.Text = "Idle";
                //lbl_Vision4Status.BackColor = Color.Yellow;
                if (lbl_Vision5Status.Text != "Idle") lbl_Vision5Status.Text = "Idle";
                //lbl_Vision5Status.BackColor = Color.Yellow;
                if (lbl_Vision6Status.Text != "Idle") lbl_Vision6Status.Text = "Idle";
                //lbl_Vision6Status.BackColor = Color.Yellow;
                if (lbl_Vision7Status.Text != "Idle") lbl_Vision7Status.Text = "Idle";
                //lbl_Vision7Status.BackColor = Color.Yellow;
                if (lbl_Vision8Status.Text != "Idle") lbl_Vision8Status.Text = "Idle";
                //lbl_Vision8Status.BackColor = Color.Yellow;
                if (lbl_Vision9Status.Text != "Idle") lbl_Vision9Status.Text = "Idle";
                //lbl_Vision9Status.BackColor = Color.Yellow;
                if (lbl_Vision10Status.Text != "Idle") lbl_Vision10Status.Text = "Idle";
                //lbl_Vision10Status.BackColor = Color.Yellow;
            }

            

            if (m_smCustomizeInfo.g_blnSavePassImage || m_smCustomizeInfo.g_blnSaveFailImage)
            {
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    if (m_smVSInfo[i].g_intMachineStatus == 2)
                        continue;

                    VisionInfo smVisionInfo = m_smVSInfo[i];

                    string strPath = smVisionInfo.g_strSaveImageLocation +
                             m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime;

                    //string strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")";

                    string strVisionImageFolderName;
                    if (smVisionInfo.g_intVisionResetCount == 0)
                        strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")" + "_" + m_smProductionInfo.g_strLotStartTime;
                    else
                        strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")" + "_" + smVisionInfo.g_strVisionResetCountTime;

                    string Path = strPath + "\\" + strVisionImageFolderName;

                    if (Directory.Exists(Path))
                    {
                        string[] dir = Directory.GetDirectories(Path);
                        bool FirstTime = true;
                        foreach (string dirs in dir)
                        {
                            string[] Files = Directory.GetFiles(dirs);

                            //foreach (string imageName in Files)
                            //{
                            if (Files.Length > 0)
                            {
                                if (FirstTime)
                                {
                                    smVisionInfo.g_arrImageFiles.Clear();
                                    smVisionInfo.g_intFileIndex = -1;
                                }
                                LoadCurrentLotImages_AfterStopProduction(dirs + "\\", FirstTime, i);
                                FirstTime = false;
                            }
                            //}
                        }
                    }
                    SortAccordingToDateTime(ref m_smVSInfo[i].g_arrImageFiles);
                }

            }
            EnableMenuButton();
            m_blnBlockProductionCheckStatus = false;
        }

        private void btn_SaveErrorMessage_Click(object sender, EventArgs e)
        {
            if (dlg_SaveTextFile.ShowDialog() == DialogResult.OK)
            {
                m_objVisionPage[m_intSelectedVisionStation - 1].SaveErrorMessage(dlg_SaveImageFile.FileName);
            }
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }
        private bool WantSaveImageAccordingMergeType(int intImageIndex)
        {
            switch (m_smVSInfo[m_intSelectedVisionStation - 1].g_intImageMergeType)
            {
                case 0:
                    return true;
                    break;
                case 1:
                    if (intImageIndex == 1)
                        return false;
                    break;
                case 2:
                    if ((intImageIndex == 1) || (intImageIndex == 2))
                        return false;
                    break;
                case 3:
                    if ((intImageIndex == 1) || (intImageIndex == 3))
                        return false;
                    break;
                case 4:
                    if ((intImageIndex == 1) || (intImageIndex == 2) || (intImageIndex == 4))
                        return false;
                    break;
            }

            return true;
        }
        private void btn_SaveImage_Click(object sender, EventArgs e)
        {
            if (dlg_SaveImageFile.ShowDialog() == DialogResult.OK)
            {
                if (m_blnchecktypo)
                {
                    dlg_SaveImageFile.Reset();
                    dlg_SaveImageFile.Filter = "Image Files(*.BMP) | *.BMP";
                    dlg_SaveImageFile.DefaultExt = "bmp";
                    btn_SaveImage_Click(sender, e);
                }
                else
                {
                    VisionInfo smVisionInfo = m_smVSInfo[m_intSelectedVisionStation - 1];

                    if (smVisionInfo.g_blnViewColorImage)
                        smVisionInfo.g_arrColorImages[0].SaveImage(dlg_SaveImageFile.FileName);
                    else
                        smVisionInfo.g_arrImages[0].SaveImage(dlg_SaveImageFile.FileName);
                    //
                    //STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + "-Save Image", smVisionInfo.g_strVisionFolderName, "", dlg_SaveImageFile.FileName, m_smProductionInfo.g_strLotID);
                    //
                    string strDirPath = Path.GetDirectoryName(dlg_SaveImageFile.FileName);
                    string strFileName;
                    string strExtension = Path.GetExtension(dlg_SaveImageFile.FileName);
                    for (int i = 1; i < smVisionInfo.g_arrImages.Count; i++)
                    {
                        if (!WantSaveImageAccordingMergeType(i))
                            continue;

                        strFileName = Path.GetFileNameWithoutExtension(dlg_SaveImageFile.FileName) + "_Image" + i.ToString();
                        if (smVisionInfo.g_blnViewColorImage)
                            smVisionInfo.g_arrColorImages[i].SaveImage(strDirPath + "\\" + strFileName + strExtension);
                        else
                            smVisionInfo.g_arrImages[i].SaveImage(strDirPath + "\\" + strFileName + strExtension);
                    }

                    Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                }
            }
        }



        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey1 = key.CreateSubKey("SVG\\AutoMode");

            switch (((SRMControl.SRMCheckBox)sender).Name)
            {
                case "chk_Crosshair":
                    m_smProductionInfo.g_blnViewCrosshair = chk_Crosshair.Checked;
                    subKey1.SetValue("ViewCrosshair", chk_Crosshair.Checked);
                    break;
                case "chk_Inspect":
                    m_smProductionInfo.g_blnViewInspection = chk_Inspect.Checked;

                    subKey1.SetValue("ViewInspection", chk_Inspect.Checked);
                    break;
                case "chk_SearchROI":
                    m_smProductionInfo.g_blnViewSearchROI = chk_SearchROI.Checked;
                    subKey1.SetValue("ViewROI", chk_SearchROI.Checked);
                    break;
                case "chk_PositionCrosshair":
                    m_smProductionInfo.g_blnViewPosCrosshair = chk_PositionCrosshair.Checked;
                    subKey1.SetValue("ViewPosCrosshair", chk_PositionCrosshair.Checked);
                    break;
            }

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                m_smVSInfo[i].ALL_VM_UpdatePictureBox = true;
            }
        }

        private void AutoNavigationBar_SelectedPaneChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            // disable timer when leave a vision page to reduce CPU usage
            switch (m_intSelectedVisionStation)
            {
                case 0:
                    m_objDisplay1Page.ActivateTimer(false);
                    break;
                case -1:
                    if (m_objDisplay2Page != null)
                        m_objDisplay2Page.ActivateTimer(false);
                    break;
                case -2:
                    if (m_objDisplay3Page != null)
                        m_objDisplay3Page.ActivateTimer(false);
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    if (m_objVisionPage[m_intSelectedVisionStation - 1] == null)
                        m_intSelectedVisionStation = 1;
                    m_objVisionPage[m_intSelectedVisionStation - 1].ActivateTimer(false);
                    break;
            }

            if (NavigationBar_Auto.SelectedPane.Name == "NavigationPane_Display1")
            {
                tabCtrl_Auto.SelectedTab = tabPage_Display1;
                m_intSelectedVisionStation = 0;
                m_objDisplay1Page.ActivateTimer(true);
            }
            else if (NavigationBar_Auto.SelectedPane.Name == "NavigationPane_Display2")
            {
                tabCtrl_Auto.SelectedTab = tabPage_Display2;
                m_intSelectedVisionStation = -1;
                m_objDisplay2Page.ActivateTimer(true);
            }
            else if (NavigationBar_Auto.SelectedPane.Name == "NavigationPane_Display3")
            {
                tabCtrl_Auto.SelectedTab = tabPage_Display3;
                m_intSelectedVisionStation = -2;
                m_objDisplay3Page.ActivateTimer(true);
            }
            else if (NavigationBar_Auto.SelectedPane.Name == "NavigationPane_Vision1")
            {
                tabCtrl_Auto.SelectedTab = tabPage_Vision1;
                m_intSelectedVisionStation = 1;
                m_objVisionPage[0].ActivateTimer(true);
            }
            else if (NavigationBar_Auto.SelectedPane.Name == "NavigationPane_Vision2")
            {
                if (m_objVisionPage[1] != null)
                {
                    tabCtrl_Auto.SelectedTab = tabPage_Vision2;
                    m_intSelectedVisionStation = 2;
                    m_objVisionPage[1].ActivateTimer(true);
                }
            }
            else if (NavigationBar_Auto.SelectedPane.Name == "NavigationPane_Vision3")
            {
                tabCtrl_Auto.SelectedTab = tabPage_Vision3;
                m_intSelectedVisionStation = 3;
                if (m_objVisionPage[2] != null)
                    m_objVisionPage[2].ActivateTimer(true);
            }
            else if (NavigationBar_Auto.SelectedPane.Name == "NavigationPane_Vision4")
            {
                tabCtrl_Auto.SelectedTab = tabPage_Vision4;
                m_intSelectedVisionStation = 4;
                if (m_objVisionPage[3] != null)
                    m_objVisionPage[3].ActivateTimer(true);
            }
            else if (NavigationBar_Auto.SelectedPane.Name == "NavigationPane_Vision5")
            {
                tabCtrl_Auto.SelectedTab = tabPage_Vision5;
                m_intSelectedVisionStation = 5;
                if (m_objVisionPage[4] != null)
                    m_objVisionPage[4].ActivateTimer(true);
            }
            else if (NavigationBar_Auto.SelectedPane.Name == "NavigationPane_Vision6")
            {
                tabCtrl_Auto.SelectedTab = tabPage_Vision6;
                m_intSelectedVisionStation = 6;
                if (m_objVisionPage[5] != null)
                    m_objVisionPage[5].ActivateTimer(true);
            }
            else if (NavigationBar_Auto.SelectedPane.Name == "NavigationPane_Vision7")
            {
                tabCtrl_Auto.SelectedTab = tabPage_Vision7;
                m_intSelectedVisionStation = 7;
                if (m_objVisionPage[6] != null)
                    m_objVisionPage[6].ActivateTimer(true);
            }
            else if (NavigationBar_Auto.SelectedPane.Name == "NavigationPane_Vision8")
            {
                tabCtrl_Auto.SelectedTab = tabPage_Vision8;
                m_intSelectedVisionStation = 8;
                if (m_objVisionPage[7] != null)
                    m_objVisionPage[7].ActivateTimer(true);
            }
            else if (NavigationBar_Auto.SelectedPane.Name == "NavigationPane_Vision9")
            {
                tabCtrl_Auto.SelectedTab = tabPage_Vision9;
                m_intSelectedVisionStation = 9;
                if (m_objVisionPage[8] != null)
                    m_objVisionPage[8].ActivateTimer(true);
            }
            else if (NavigationBar_Auto.SelectedPane.Name == "NavigationPane_Vision10")
            {
                tabCtrl_Auto.SelectedTab = tabPage_Vision10;
                m_intSelectedVisionStation = 10;
                if (m_objVisionPage[9] != null)
                    m_objVisionPage[9].ActivateTimer(true);
            }

            if (m_intSelectedVisionStation > 0)
            {
                if (m_smVSInfo[m_intSelectedVisionStation - 1] != null)
                {
                    if (m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionName == "Barcode")
                        m_smProductionInfo.g_blnInBarCodePage = true;
                    else
                        m_smProductionInfo.g_blnInBarCodePage = false;
                    m_smProductionInfo.g_blnInDisplayAllPage = false;
                }
            }
            else
            {
                m_smProductionInfo.g_blnInBarCodePage = false;
                m_smProductionInfo.g_blnInDisplayAllPage = true;
            }
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\AutoMode");

            //int SelectedPane = (int)subKey.GetValue("SelectedPane", "");

            //subKey.SetValue("SelectedPane", m_intSelectedVisionStation);

            //if (SelectedPane != m_intSelectedVisionStation)
            //{
            //    
            //    STDeviceEdit.SaveDeviceEditLog("SelectedPane", "SelectedPane Changed", SelectedPane.ToString(), m_intSelectedVisionStation.ToString(), m_smProductionInfo.g_strLotID);
            //    
            //}

            EnableMenuButton();

            if ((m_intSelectedVisionStation - 1) >= 0 && m_smVSInfo[m_intSelectedVisionStation - 1] != null)
            {
                switch (m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionName)
                {
                    case "MarkPkg":
                    case "MOLiPkg":
                    case "MOPkg":
                    case "InPocketPkg":
                    case "IPMLiPkg":
                        if (m_smVSInfo[m_intSelectedVisionStation - 1].g_blnWantShowGRR)
                            btn_GRR.Visible = true;
                        else
                            btn_GRR.Visible = false;
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
                        if (m_smVSInfo[m_intSelectedVisionStation - 1].g_blnWantShowGRR)
                            btn_GRR.Visible = true;
                        else
                            btn_GRR.Visible = false;
                        break;
                    case "Li3D":
                    case "Li3DPkg":
                        if (m_smVSInfo[m_intSelectedVisionStation - 1].g_blnWantShowGRR)
                            btn_GRR.Visible = true;
                        else
                            btn_GRR.Visible = false;
                        break;
                    default:
                        btn_GRR.Visible = false;
                        break;

                }
            }

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (i != (m_intSelectedVisionStation - 1))
                    continue;

                if (m_smVSInfo[i].g_intImageMergeType != 0)
                {
                    int intViewImageCount = ImageDrawing.GetImageViewCount(m_smVSInfo[i].g_intVisionIndex);
                    
                    if (cbo_ViewImage.Items.Count != intViewImageCount)
                    {
                        cbo_ViewImage.Items.Clear();
                        for (int j = 0; j < intViewImageCount; j++)
                        {
                            cbo_ViewImage.Items.Add("Image " + (j + 1));
                        }
                        cbo_ViewImage.SelectedIndex = 0;
                    }

                    if (intViewImageCount > 1)
                    {
                        cbo_ViewImage.Visible = true;

                        if (m_smVSInfo[i].g_intSelectedImage < cbo_ViewImage.Items.Count)
                            cbo_ViewImage.SelectedIndex = m_smVSInfo[i].g_intSelectedImage;
                        else if (intViewImageCount == m_smVSInfo[i].g_intSelectedImage)
                            cbo_ViewImage.SelectedIndex = m_smVSInfo[i].g_intSelectedImage - 1;
                        else
                            cbo_ViewImage.SelectedIndex = 0;
                    }
                    else
                        cbo_ViewImage.Visible = false;
                }
                else
                {
                    if (!m_smVSInfo[i].g_blnWantCheckPH && !(m_smVSInfo[i].g_strVisionName.Contains("Pad") || m_smVSInfo[i].g_strVisionName.Contains("BottomOrientPad") || m_smVSInfo[i].g_strVisionName.Contains("Li3D")))
                    {
                        if (cbo_ViewImage.Items.Contains("Image PH"))
                            cbo_ViewImage.Items.Remove("Image PH");
                    }
                    if (!m_smVSInfo[i].g_blnWantCheckEmpty && !(m_smVSInfo[i].g_strVisionName.Contains("InPocket") || m_smVSInfo[i].g_strVisionName.Contains("IPM")))
                    {
                        if (cbo_ViewImage.Items.Contains("Image Empty"))
                            cbo_ViewImage.Items.Remove("Image Empty");
                    }
                    if (cbo_ViewImage.Items.Count != m_smVSInfo[i].g_arrImages.Count)
                    {
                        cbo_ViewImage.Items.Clear();
                        for (int j = 0; j < m_smVSInfo[i].g_arrImages.Count; j++)
                        {
                            cbo_ViewImage.Items.Add("Image " + (j + 1));
                        }
                        cbo_ViewImage.SelectedIndex = 0;
                    }

                    if (m_smVSInfo[i].g_arrImages.Count > 1)
                    {
                        cbo_ViewImage.Visible = true;

                        if (m_smVSInfo[i].g_intSelectedImage < cbo_ViewImage.Items.Count)
                            cbo_ViewImage.SelectedIndex = m_smVSInfo[i].g_intSelectedImage;
                        else
                            cbo_ViewImage.SelectedIndex = 0;
                    }
                    else
                        cbo_ViewImage.Visible = false;
                }
                //Add Image PH item
                if (m_smVSInfo[i].g_blnWantCheckPH && (m_smVSInfo[i].g_strVisionName.Contains("Pad") || m_smVSInfo[i].g_strVisionName.Contains("BottomOrientPad") || m_smVSInfo[i].g_strVisionName.Contains("Li3D")))
                {
                    if (!cbo_ViewImage.Items.Contains("Image PH"))
                        cbo_ViewImage.Items.Add("Image PH");
                    if (m_smVSInfo[i].g_intSelectedImage == 0 && m_smVSInfo[i].g_blnViewPHImage == true)
                        cbo_ViewImage.SelectedIndex = cbo_ViewImage.Items.Count - 1;
                }
                else
                {
                    if (cbo_ViewImage.Items.Contains("Image PH"))
                        cbo_ViewImage.Items.Remove("Image PH");
                }
                //Add Image Empty item
                if (m_smVSInfo[i].g_blnWantCheckEmpty && (m_smVSInfo[i].g_strVisionName.Contains("InPocket") || m_smVSInfo[i].g_strVisionName.Contains("IPM")))
                {
                    if (!cbo_ViewImage.Items.Contains("Image Empty"))
                        cbo_ViewImage.Items.Add("Image Empty");
                    if (m_smVSInfo[i].g_intSelectedImage == 0 && m_smVSInfo[i].g_blnViewEmptyImage == true)
                        cbo_ViewImage.SelectedIndex = cbo_ViewImage.Items.Count - 1;
                }
                else
                {
                    if (cbo_ViewImage.Items.Contains("Image Empty"))
                        cbo_ViewImage.Items.Remove("Image Empty");
                }
            }

        }

        private void AutoSizeForm()
        {
            // Set AutoForm size to fill in whole window screen
            Rectangle objScreenRect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            //if (objScreenRect.Width == 1920 && objScreenRect.Height == 1080)
            //{
            //    this.Size = new Size(objScreenRect.Width - 4, objScreenRect.Height - 144);
            //    tabCtrl_Auto.Size = new Size(objScreenRect.Width - 165, objScreenRect.Height - 166);
            //}
            //else
            {
                this.Size = new Size(objScreenRect.Width - 4, objScreenRect.Height - 109);
                tabCtrl_Auto.Size = new Size(objScreenRect.Width, objScreenRect.Height - 156);
            }

            Pnl_TabPageButton.Width = objScreenRect.Width - 4;


            int intTotalWidth = 0;
            for (int i = 0; i < m_intVisionCount; i++)
            {
                switch (i)
                {
                    case 0:
                        intTotalWidth += btn_Vision1Tab.Width;
                        break;
                    case 1:
                        intTotalWidth += btn_Vision2Tab.Width;
                        break;
                    case 2:
                        intTotalWidth += btn_Vision3Tab.Width;
                        break;
                    case 3:
                        intTotalWidth += btn_Vision4Tab.Width;
                        break;
                    case 4:
                        intTotalWidth += btn_Vision5Tab.Width;
                        break;
                    case 5:
                        intTotalWidth += btn_Vision6Tab.Width;
                        break;
                    case 6:
                        intTotalWidth += btn_Vision7Tab.Width;
                        break;
                    case 7:
                        intTotalWidth += btn_Vision8Tab.Width;
                        break;
                    case 8:
                        intTotalWidth += btn_Vision9Tab.Width;
                        break;
                    case 9:
                        intTotalWidth += btn_Vision10Tab.Width;
                        break;
                }
            }
            pnl_Header.Width = Pnl_TabPageButton.Width - (intTotalWidth + btn_Display1Tab.Width);

            //cbo_ViewImage.Location = new Point(pnl_Header.Width - cbo_ViewImage.Width, cbo_ViewImage.Location.Y);
        }

        private void AutoForm_Load(object sender, EventArgs e)
        {
            STTrackLog.WriteLine("AutoForm_Load 1");

            AutoSizeForm();

            STTrackLog.WriteLine("AutoForm_Load 2");
            AddAutoTabPage();

            STTrackLog.WriteLine("AutoForm_Load 3");
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\AutoMode");
            m_intSelectedVisionStation = Convert.ToInt32(subKey.GetValue("SelectedPane", 0));

            STTrackLog.WriteLine("AutoForm_Load 4");
            // If this vision station is unavailable, then set the default selected vision as display All page
            try
            {
                STTrackLog.WriteLine("AutoForm_Load 5");

                switch (m_intSelectedVisionStation)
                {
                    case 0:
                        btn_Display1Tab.BackColor = Color.FromArgb(255, 192, 128);
                        tabCtrl_Auto.SelectedTab = tabPage_Display1;
                        m_objDisplay1Page.ActivateTimer(true);
                        break;
                    case -1:
                        btn_Display2Tab.BackColor = Color.FromArgb(255, 192, 128);
                        tabCtrl_Auto.SelectedTab = tabPage_Display2;
                        m_objDisplay2Page.ActivateTimer(true);
                        break;
                    case -2:
                        btn_Display3Tab.BackColor = Color.FromArgb(255, 192, 128);
                        tabCtrl_Auto.SelectedTab = tabPage_Display3;
                        m_objDisplay3Page.ActivateTimer(true);
                        break;
                    case 1:
                        btn_Vision1Tab.BackColor = Color.FromArgb(255, 192, 128);
                        tabCtrl_Auto.SelectedTab = tabPage_Vision1;
                        m_objVisionPage[0].ActivateTimer(true);
                        break;
                    case 2:
                        btn_Vision2Tab.BackColor = Color.FromArgb(255, 192, 128);
                        tabCtrl_Auto.SelectedTab = tabPage_Vision2;
                        m_objVisionPage[1].ActivateTimer(true);
                        break;
                    case 3:
                        btn_Vision3Tab.BackColor = Color.FromArgb(255, 192, 128);
                        tabCtrl_Auto.SelectedTab = tabPage_Vision3;
                        m_objVisionPage[2].ActivateTimer(true);
                        break;
                    case 4:
                        btn_Vision4Tab.BackColor = Color.FromArgb(255, 192, 128);
                        tabCtrl_Auto.SelectedTab = tabPage_Vision4;
                        m_objVisionPage[3].ActivateTimer(true);
                        break;
                    case 5:
                        btn_Vision5Tab.BackColor = Color.FromArgb(255, 192, 128);
                        tabCtrl_Auto.SelectedTab = tabPage_Vision5;
                        m_objVisionPage[4].ActivateTimer(true);
                        break;
                    case 6:
                        btn_Vision6Tab.BackColor = Color.FromArgb(255, 192, 128);
                        tabCtrl_Auto.SelectedTab = tabPage_Vision6;
                        m_objVisionPage[5].ActivateTimer(true);
                        break;
                    case 7:
                        btn_Vision7Tab.BackColor = Color.FromArgb(255, 192, 128);
                        tabCtrl_Auto.SelectedTab = tabPage_Vision7;
                        m_objVisionPage[6].ActivateTimer(true);
                        break;
                    case 8:
                        btn_Vision8Tab.BackColor = Color.FromArgb(255, 192, 128);
                        tabCtrl_Auto.SelectedTab = tabPage_Vision8;
                        m_objVisionPage[7].ActivateTimer(true);
                        break;
                    case 9:
                        btn_Vision9Tab.BackColor = Color.FromArgb(255, 192, 128);
                        tabCtrl_Auto.SelectedTab = tabPage_Vision9;
                        m_objVisionPage[8].ActivateTimer(true);
                        break;
                    case 10:
                        btn_Vision10Tab.BackColor = Color.FromArgb(255, 192, 128);
                        tabCtrl_Auto.SelectedTab = tabPage_Vision10;
                        m_objVisionPage[9].ActivateTimer(true);
                        break;
                }

                if (m_intSelectedVisionStation > 0)
                {
                    if (m_smVSInfo[m_intSelectedVisionStation - 1] != null)
                    {
                        if (m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionName == "Barcode")
                            m_smProductionInfo.g_blnInBarCodePage = true;
                        else
                            m_smProductionInfo.g_blnInBarCodePage = false;
                        m_smProductionInfo.g_blnInDisplayAllPage = false;
                    }
                }
                else
                {
                    m_smProductionInfo.g_blnInBarCodePage = false;
                    m_smProductionInfo.g_blnInDisplayAllPage = true;
                }
                STTrackLog.WriteLine("AutoForm_Load 5");
                EnableMenuButton();
                STTrackLog.WriteLine("AutoForm_Load 6");
                if ((m_intSelectedVisionStation - 1) >= 0 && m_smVSInfo[m_intSelectedVisionStation - 1] != null)
                {
                    switch (m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionName)
                    {
                        case "MarkPkg":
                        case "MOLiPkg":
                        case "MOPkg":
                        case "InPocketPkg":
                        case "IPMLiPkg":
                            if (m_smVSInfo[m_intSelectedVisionStation - 1].g_blnWantShowGRR)
                                btn_GRR.Visible = true;
                            else
                                btn_GRR.Visible = false;
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
                            if (m_smVSInfo[m_intSelectedVisionStation - 1].g_blnWantShowGRR)
                                btn_GRR.Visible = true;
                            else
                                btn_GRR.Visible = false;
                            break;
                        case "Li3D":
                        case "Li3DPkg":
                            if (m_smVSInfo[m_intSelectedVisionStation - 1].g_blnWantShowGRR)
                                btn_GRR.Visible = true;
                            else
                                btn_GRR.Visible = false;
                            break;
                        default:
                            btn_GRR.Visible = false;
                            break;
                    }
                }

                STTrackLog.WriteLine("AutoForm_Load 7");
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    if (i != (m_intSelectedVisionStation - 1))
                        continue;

                    if (m_smVSInfo[i].g_intImageMergeType != 0)
                    {
                        int intViewImageCount = ImageDrawing.GetImageViewCount(m_smVSInfo[i].g_intVisionIndex);

                        if (cbo_ViewImage.Items.Count != intViewImageCount)
                        {
                            cbo_ViewImage.Items.Clear();
                            for (int j = 0; j < intViewImageCount; j++)
                            {
                                cbo_ViewImage.Items.Add("Image " + (j + 1));
                            }
                            cbo_ViewImage.SelectedIndex = 0;
                        }

                        if (intViewImageCount == 2)
                        {
                            cbo_ViewImage.Visible = true;

                            if (m_smVSInfo[i].g_intSelectedImage <= 2)
                                cbo_ViewImage.SelectedIndex = 0;
                            else if (m_smVSInfo[i].g_intSelectedImage == 3)
                                cbo_ViewImage.SelectedIndex = 1;
                            else
                                cbo_ViewImage.SelectedIndex = 0;
                        }
                        else if (intViewImageCount == 3)
                        {
                            cbo_ViewImage.Visible = true;

                            if (m_smVSInfo[i].g_intSelectedImage <= 1)
                                cbo_ViewImage.SelectedIndex = 0;
                            else if (m_smVSInfo[i].g_intSelectedImage == 2)
                                cbo_ViewImage.SelectedIndex = 1;
                            else if (m_smVSInfo[i].g_intSelectedImage == 3)
                                cbo_ViewImage.SelectedIndex = 2;
                            else
                                cbo_ViewImage.SelectedIndex = 0;
                        }
                        else if (intViewImageCount == 4)
                        {
                            cbo_ViewImage.Visible = true;

                            if (m_smVSInfo[i].g_intSelectedImage <= 1)
                                cbo_ViewImage.SelectedIndex = 0;
                            else if (m_smVSInfo[i].g_intSelectedImage == 2)
                                cbo_ViewImage.SelectedIndex = 1;
                            else if (m_smVSInfo[i].g_intSelectedImage == 3)
                                cbo_ViewImage.SelectedIndex = 2;
                            else if (m_smVSInfo[i].g_intSelectedImage == 4)
                                cbo_ViewImage.SelectedIndex = 3;
                            else
                                cbo_ViewImage.SelectedIndex = 0;
                        }
                        else if (intViewImageCount == 5)
                        {
                            cbo_ViewImage.Visible = true;

                            if (m_smVSInfo[i].g_intSelectedImage <= 1)
                                cbo_ViewImage.SelectedIndex = 0;
                            else if (m_smVSInfo[i].g_intSelectedImage == 2)
                                cbo_ViewImage.SelectedIndex = 1;
                            else if (m_smVSInfo[i].g_intSelectedImage == 3)
                                cbo_ViewImage.SelectedIndex = 2;
                            else if (m_smVSInfo[i].g_intSelectedImage == 4)
                                cbo_ViewImage.SelectedIndex = 3;
                            else if (m_smVSInfo[i].g_intSelectedImage == 5)
                                cbo_ViewImage.SelectedIndex = 4;
                            else
                                cbo_ViewImage.SelectedIndex = 0;
                        }
                        else if (intViewImageCount == 6)
                        {
                            cbo_ViewImage.Visible = true;

                            if (m_smVSInfo[i].g_intSelectedImage <= 1)
                                cbo_ViewImage.SelectedIndex = 0;
                            else if (m_smVSInfo[i].g_intSelectedImage == 2)
                                cbo_ViewImage.SelectedIndex = 1;
                            else if (m_smVSInfo[i].g_intSelectedImage == 3)
                                cbo_ViewImage.SelectedIndex = 2;
                            else if (m_smVSInfo[i].g_intSelectedImage == 4)
                                cbo_ViewImage.SelectedIndex = 3;
                            else if (m_smVSInfo[i].g_intSelectedImage == 5)
                                cbo_ViewImage.SelectedIndex = 4;
                            else if (m_smVSInfo[i].g_intSelectedImage == 6)
                                cbo_ViewImage.SelectedIndex = 5;
                            else
                                cbo_ViewImage.SelectedIndex = 0;
                        }
                        else
                        {
                            m_smVSInfo[i].g_intSelectedImage = 0;
                            subKey.SetValue("CurrentSelectedImage" + i.ToString(), m_smVSInfo[i].g_intSelectedImage);
                            cbo_ViewImage.Visible = false;
                        }
                    }
                    else
                    {
                        if (!m_smVSInfo[i].g_blnWantCheckPH && !(m_smVSInfo[i].g_strVisionName.Contains("Pad") || m_smVSInfo[i].g_strVisionName.Contains("BottomOrientPad") || m_smVSInfo[i].g_strVisionName.Contains("Li3D")))
                        {
                            if (cbo_ViewImage.Items.Contains("Image PH"))
                                cbo_ViewImage.Items.Remove("Image PH");
                        }
                        if (!m_smVSInfo[i].g_blnWantCheckEmpty && !(m_smVSInfo[i].g_strVisionName.Contains("InPocket") || m_smVSInfo[i].g_strVisionName.Contains("IPM")))
                        {
                            if (cbo_ViewImage.Items.Contains("Image Empty"))
                                cbo_ViewImage.Items.Remove("Image Empty");
                        }

                        if (cbo_ViewImage.Items.Count != m_smVSInfo[i].g_arrImages.Count)
                        {
                            cbo_ViewImage.Items.Clear();
                            for (int j = 0; j < m_smVSInfo[i].g_arrImages.Count; j++)
                            {
                                cbo_ViewImage.Items.Add("Image " + (j + 1));
                            }
                            cbo_ViewImage.SelectedIndex = 0;
                        }

                        if (m_smVSInfo[i].g_arrImages.Count > 1)
                        {
                            cbo_ViewImage.Visible = true;

                            if (m_smVSInfo[i].g_intSelectedImage < cbo_ViewImage.Items.Count)
                                cbo_ViewImage.SelectedIndex = m_smVSInfo[i].g_intSelectedImage;
                            else
                                cbo_ViewImage.SelectedIndex = 0;
                        }
                        else
                            cbo_ViewImage.Visible = false;
                    }

                    //Add Image PH item
                    if (m_smVSInfo[i].g_blnWantCheckPH && (m_smVSInfo[i].g_strVisionName.Contains("Pad") || m_smVSInfo[i].g_strVisionName.Contains("BottomOrientPad") || m_smVSInfo[i].g_strVisionName.Contains("Li3D")))
                    {
                        if (!cbo_ViewImage.Items.Contains("Image PH"))
                            cbo_ViewImage.Items.Add("Image PH");
                        if (m_smVSInfo[i].g_intSelectedImage == 0 && m_smVSInfo[i].g_blnViewPHImage == true)
                            cbo_ViewImage.SelectedIndex = cbo_ViewImage.Items.Count - 1;
                    }
                    else
                    {
                        if (cbo_ViewImage.Items.Contains("Image PH"))
                            cbo_ViewImage.Items.Remove("Image PH");
                    }
                    //Add Image Empty item
                    if (m_smVSInfo[i].g_blnWantCheckEmpty && (m_smVSInfo[i].g_strVisionName.Contains("InPocket") || m_smVSInfo[i].g_strVisionName.Contains("IPM")))
                    {
                        if (!cbo_ViewImage.Items.Contains("Image Empty"))
                            cbo_ViewImage.Items.Add("Image Empty");
                        if (m_smVSInfo[i].g_intSelectedImage == 0 && m_smVSInfo[i].g_blnViewEmptyImage == true)
                            cbo_ViewImage.SelectedIndex = cbo_ViewImage.Items.Count - 1;
                    }
                    else
                    {
                        if (cbo_ViewImage.Items.Contains("Image Empty"))
                            cbo_ViewImage.Items.Remove("Image Empty");
                    }
                }

                STTrackLog.WriteLine("AutoForm_Load 6");
            }
            catch
            {
                STTrackLog.WriteLine("AutoForm_Load 7");
                m_intSelectedVisionStation = -1;
                //NavigationBar_Auto.SelectedPane = //NavigationPane_Vision2;
                tabCtrl_Auto.SelectedTab = tabPage_Display1;
            }

            STTrackLog.WriteLine("AutoForm_Load 8");

            ProductionStopMode();

            STTrackLog.WriteLine("AutoForm_Load 9");
            timer_Auto.Enabled = true;
            m_smProductionInfo.AT_ALL_InAuto = true;
            Cursor.Current = Cursors.Default;

            STTrackLog.WriteLine("AutoForm_Load 10");
        }



        private void timer_Auto_Tick(object sender, EventArgs e)
        {
            if (m_smProductionInfo.g_blnRefreshingON)
            {
                timer_Auto.Stop();
                this.Enabled = false;
                this.Close();
                return;
            }

            if (m_intSelectedVisionStation > 0)
            {
                if (m_smVSInfo[m_intSelectedVisionStation - 1].g_blnWantShowGRR)
                    btn_GRR.Visible = true;
                else
                {
                    btn_GRR.Visible = false;
                    // Set GRR Status OFF when button not visible
                    m_smVSInfo[m_intSelectedVisionStation - 1].g_blnGRRON = false;
                }
            }
            if (m_smProductionInfo.PR_AT_StopProduction)
            {
                m_smProductionInfo.PR_AT_StopProduction = false;
                ProductionStopMode();
            }

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (m_smVSInfo[i].AT_PR_TriggerLiveImage)
                {
                    EnableLiveImage(m_smVSInfo[i], i);
                    m_smVSInfo[i].AT_PR_TriggerLiveImage = false;
                }

                if (m_smVSInfo[i].VM_AT_DisableImageLoad != m_blnImageLoadDisable[i])
                {
                    m_blnImageLoadDisable[i] = m_smVSInfo[i].VM_AT_DisableImageLoad;
                    EnableLiveImage(m_smVSInfo[i], i);
                }

                if (m_smVSInfo[i].g_intMachineStatus == 1)
                {
                    if (btn_ProductionAll.Checked && !m_blnBlockProductionCheckStatus)
                    {
                        btn_ProductionAll.Checked = false;
                        btn_ProductionAll.Text = GetLanguageText("Prod All");
                    }
                    if (i == (m_intSelectedVisionStation - 1) && btn_Production.Checked)
                    {
                        EnableMenuButton();
                    }
                    ChangeSPC(i, 1);
                }

                if (m_smVSInfo[i].g_intMachineStatus == 2)
                {
                    ChangeSPC(i, m_smVSInfo[i].g_intMachineStatus);
                }

                if (m_smVSInfo[i].VM_AT_ReloadRecipe)
                {
                    StartWaiting("Loading Recipe...");
                    for (int j = 0; j < m_smVSInfo.Length; j++)
                    {
                        if (m_smVSInfo[j] == null)
                            continue;
                        
                        if (i != j && m_smVSInfo[j].g_strDeviceFolderName == m_smVSInfo[i].g_strDeviceFolderName)
                        {
                            //string strPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_strRecipeID + "\\";
                            //for (int k = 0; k < m_smVSInfo[i].g_arrEditedDir.Count; k++)
                            //{
                            //    CopyDirectory(strPath + m_smVSInfo[i].g_strVisionName + "\\" + m_smVSInfo[i].g_arrEditedDir[k],
                            //        strPath + m_smVSInfo[j].g_strVisionName + "\\" + m_smVSInfo[i].g_arrEditedDir[k]);
                            //}
                            m_objVisionPage[i].ReadFromXML(m_smProductionInfo.g_arrSingleRecipeID[i], false);
                            m_objVisionPage[i].ReInitCamera();//2021-03-25 ZJYEOH : Need to load camera setting too
                            m_objVisionPage[i].SetCustomView_Visible(m_blnDeviceReady);
                            m_objVisionPage[i].CustomizeGUI();
                            m_smVSInfo[j].AT_VM_ReloadRecipe = true;
                        }
                    }
                    StopWaiting();
                    m_smVSInfo[i].VM_AT_ReloadRecipe = false;
                }

                //if (m_smVSInfo[i].AT_VM_ManualTestMode == true)
                if (m_smVSInfo[i].g_blnUpdateImageNoComboBox)   // Allow to update combo box when necessary
                {
                    m_smVSInfo[i].g_blnUpdateImageNoComboBox = false;

                    if (m_smVSInfo[i].g_intImageMergeType != 0)
                    {
                        int intViewImageCount = ImageDrawing.GetImageViewCount(m_smVSInfo[i].g_intVisionIndex);

                        if (cbo_ViewImage.Items.Count != intViewImageCount)
                        {
                            cbo_ViewImage.Items.Clear();
                            for (int j = 0; j < intViewImageCount; j++)
                            {
                                cbo_ViewImage.Items.Add("Image " + (j + 1));
                            }
                            cbo_ViewImage.SelectedIndex = 0;
                        }

                        if (intViewImageCount > 1)
                        {
                            VisibleViewImageComboBox();

                            switch (m_smVSInfo[i].g_intImageMergeType)
                            {
                                case 1: // Merge grab 1 and 2
                                    {
                                        int intImageViewSelectedIndex = 0;

                                        if (m_smVSInfo[i].g_intSelectedImage == 0 || m_smVSInfo[i].g_intSelectedImage == 1)
                                            intImageViewSelectedIndex = 0;
                                        else
                                            intImageViewSelectedIndex = m_smVSInfo[i].g_intSelectedImage - 1;

                                        if (intImageViewSelectedIndex < cbo_ViewImage.Items.Count)
                                            cbo_ViewImage.SelectedIndex = intImageViewSelectedIndex;
                                        else
                                            cbo_ViewImage.SelectedIndex = 0;
                                    }
                                    break;
                                case 2: // Merge grab 1, 2 and 3
                                    {
                                        int intImageViewSelectedIndex = 0;

                                        if (m_smVSInfo[i].g_intSelectedImage == 0 || m_smVSInfo[i].g_intSelectedImage == 1 || m_smVSInfo[i].g_intSelectedImage == 2)
                                            intImageViewSelectedIndex = 0;
                                        else
                                            intImageViewSelectedIndex = m_smVSInfo[i].g_intSelectedImage - 2;

                                        if (intImageViewSelectedIndex < cbo_ViewImage.Items.Count)
                                            cbo_ViewImage.SelectedIndex = intImageViewSelectedIndex;
                                        else
                                            cbo_ViewImage.SelectedIndex = 0;
                                    }
                                    break;
                                case 3: // Merge grab 1 and 2, Merge grab 3 and 4
                                    {
                                        int intImageViewSelectedIndex = 0;

                                        if (m_smVSInfo[i].g_intSelectedImage == 0 || m_smVSInfo[i].g_intSelectedImage == 1)
                                            intImageViewSelectedIndex = 0;
                                        else if (m_smVSInfo[i].g_intSelectedImage == 2 || m_smVSInfo[i].g_intSelectedImage == 3)
                                            intImageViewSelectedIndex = 1;
                                        else
                                            intImageViewSelectedIndex = m_smVSInfo[i].g_intSelectedImage - 2;

                                        if (intImageViewSelectedIndex < cbo_ViewImage.Items.Count)
                                            cbo_ViewImage.SelectedIndex = intImageViewSelectedIndex;
                                        else
                                            cbo_ViewImage.SelectedIndex = 0;
                                    }
                                    break;
                                case 4: // Merge grab 1 & 2 & 3, Merge grab 4 and 5
                                    {
                                        int intImageViewSelectedIndex = 0;

                                        if (m_smVSInfo[i].g_intSelectedImage == 0 || m_smVSInfo[i].g_intSelectedImage == 1 || m_smVSInfo[i].g_intSelectedImage == 2)
                                            intImageViewSelectedIndex = 0;
                                        else if (m_smVSInfo[i].g_intSelectedImage == 3 || m_smVSInfo[i].g_intSelectedImage == 4)
                                            intImageViewSelectedIndex = 1;
                                        else
                                            intImageViewSelectedIndex = m_smVSInfo[i].g_intSelectedImage - 3;   // should be -3, not -2 bcos index start with 0

                                        if (intImageViewSelectedIndex >= 0 && intImageViewSelectedIndex < cbo_ViewImage.Items.Count)
                                            cbo_ViewImage.SelectedIndex = intImageViewSelectedIndex;
                                        else
                                            cbo_ViewImage.SelectedIndex = 0;
                                    }
                                    break;
                            }

                            //if (m_smVSInfo[i].g_intSelectedImage < cbo_ViewImage.Items.Count)
                            //    cbo_ViewImage.SelectedIndex = m_smVSInfo[i].g_intSelectedImage;
                            //else if (intViewImageCount == m_smVSInfo[i].g_intSelectedImage)
                            //    cbo_ViewImage.SelectedIndex = m_smVSInfo[i].g_intSelectedImage - 1;
                            //else
                            //    cbo_ViewImage.SelectedIndex = 0;
                        }
                        else
                            cbo_ViewImage.Visible = false;
                    }
                    else
                    {
                        if (!m_smVSInfo[i].g_blnWantCheckPH && !(m_smVSInfo[i].g_strVisionName.Contains("Pad") || m_smVSInfo[i].g_strVisionName.Contains("BottomOrientPad") || m_smVSInfo[i].g_strVisionName.Contains("Li3D")))
                        {
                            if (cbo_ViewImage.Items.Contains("Image PH"))
                                cbo_ViewImage.Items.Remove("Image PH");
                        }
                        if (!m_smVSInfo[i].g_blnWantCheckEmpty && !(m_smVSInfo[i].g_strVisionName.Contains("InPocket") || m_smVSInfo[i].g_strVisionName.Contains("IPM")))
                        {
                            if (cbo_ViewImage.Items.Contains("Image Empty"))
                                cbo_ViewImage.Items.Remove("Image Empty");
                        }
                        if (cbo_ViewImage.Items.Count != m_smVSInfo[i].g_arrImages.Count)
                        {
                            cbo_ViewImage.Items.Clear();
                            for (int j = 0; j < m_smVSInfo[i].g_arrImages.Count; j++)
                            {
                                cbo_ViewImage.Items.Add("Image " + (j + 1));
                            }
                            cbo_ViewImage.SelectedIndex = 0;
                        }

                        if (m_smVSInfo[i].g_arrImages.Count > 1)
                        {
                            VisibleViewImageComboBox();

                            if (m_smVSInfo[i].g_intSelectedImage < cbo_ViewImage.Items.Count)
                                cbo_ViewImage.SelectedIndex = m_smVSInfo[i].g_intSelectedImage;
                            else
                                cbo_ViewImage.SelectedIndex = 0;
                        }
                        else
                            cbo_ViewImage.Visible = false;
                    }
                    //Add Image PH item
                    if (m_smVSInfo[i].g_blnWantCheckPH && (m_smVSInfo[i].g_strVisionName.Contains("Pad") || m_smVSInfo[i].g_strVisionName.Contains("BottomOrientPad") || m_smVSInfo[i].g_strVisionName.Contains("Li3D")))
                    {
                        if (!cbo_ViewImage.Items.Contains("Image PH"))
                            cbo_ViewImage.Items.Add("Image PH");
                        if (m_smVSInfo[i].g_intSelectedImage == 0 && m_smVSInfo[i].g_blnViewPHImage == true)
                            cbo_ViewImage.SelectedIndex = cbo_ViewImage.Items.Count - 1;

                    }
                    else
                    {
                        if (cbo_ViewImage.Items.Contains("Image PH"))
                            cbo_ViewImage.Items.Remove("Image PH");
                    }
                    //Add Image Empty item
                    if (m_smVSInfo[i].g_blnWantCheckEmpty && (m_smVSInfo[i].g_strVisionName.Contains("InPocket") || m_smVSInfo[i].g_strVisionName.Contains("IPM")))
                    {
                        if (!cbo_ViewImage.Items.Contains("Image Empty"))
                            cbo_ViewImage.Items.Add("Image Empty");
                        if (m_smVSInfo[i].g_intSelectedImage == 0 && m_smVSInfo[i].g_blnViewEmptyImage == true)
                            cbo_ViewImage.SelectedIndex = cbo_ViewImage.Items.Count - 1;
                    }
                    else
                    {
                        if (cbo_ViewImage.Items.Contains("Image Empty"))
                            cbo_ViewImage.Items.Remove("Image Empty");
                    }
                }

                if (m_smVSInfo[i].g_blnCPKON)
                {
                    if (!m_smVSInfo[i].g_blnInitCPK)
                    {
                        // init CPK when enter software, will load temp data from file to check is CPK recorded halfway from previous run
                        m_smVSInfo[i].g_blnInitCPK = true;
                        m_smVSInfo[i].g_blnReInitCPK = true;
                        m_objVisionPage[i].InitCPK(true);
                    }

                    if (!m_smVSInfo[i].g_blnReInitCPK)
                    {
                        //re-init if Setting change or learn pad (no load temp data, will clear all and start from 0)
                        m_smVSInfo[i].g_blnReInitCPK = true;
                        m_objVisionPage[i].InitCPK(false);
                    }
                }

                if (m_smVSInfo[i].g_blnResetCount)
                {
                    m_smVSInfo[i].g_blnResetCount = false;
                    ResetCurrentCount(1 << i, false);
                }
            }

            if (m_smCustomizeInfo.g_intSelectedVision > 0)
            {
                switch (m_smCustomizeInfo.g_intSelectedVision)
                {
                    case 1:
                        btn_Vision1Tab.PerformClick();
                        break;
                    case 2:
                        btn_Vision2Tab.PerformClick();
                        break;
                    case 3:
                        btn_Vision3Tab.PerformClick();
                        break;
                    case 4:
                        btn_Vision4Tab.PerformClick();
                        break;
                    case 5:
                        btn_Vision5Tab.PerformClick();
                        break;
                    case 6:
                        btn_Vision6Tab.PerformClick();
                        break;
                    case 7:
                        btn_Vision7Tab.PerformClick();
                        break;
                    case 8:
                        btn_Vision8Tab.PerformClick();
                        break;
                    case 9:
                        btn_Vision9Tab.PerformClick();
                        break;
                    case 10:
                        btn_Vision10Tab.PerformClick();
                        break;
                }
                m_smCustomizeInfo.g_intSelectedVision = 0;
            }
            if (m_smProductionInfo.CO_AT_ResetCount)
            {
                m_smProductionInfo.CO_AT_ResetCount = false;
                ResetCurrentCount(m_smProductionInfo.g_intVisionInvolved, true);
            }

            if (m_smProductionInfo.CO_AT_Get2DCode)
            {
                m_smProductionInfo.CO_AT_Get2DCode = false;
                Get2DCode(m_smProductionInfo.g_intVisionInvolved);
            }

            if (m_smProductionInfo.CO_AT_LoadRecipe)
            {
                m_smProductionInfo.CO_AT_LoadRecipe = false;

                if (!LoadRecipe(m_smProductionInfo.g_strRecipeID))
                {
                    m_smProductionInfo.AT_CO_LoadRecipeDone = true;
                    m_smProductionInfo.g_blnLoadRecipePass = false;
                    return;
                }

                try
                {
                    for (int i = 0; i < m_smVSInfo.Length; i++)
                    {
                        if (m_smVSInfo[i] == null)
                            continue;

                        if ((m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) > 0 || m_smProductionInfo.g_intVisionInvolved == 0 || m_smProductionInfo.g_arrVisionIDIndex[i] == 0)
                        {
                            m_objVisionPage[i].ReadFromXML(m_smProductionInfo.g_arrSingleRecipeID[i], true);
                            m_objVisionPage[i].ReInitCamera();//2021-03-25 ZJYEOH : Need to load camera setting too
                            m_objVisionPage[i].SetCustomView_Visible(m_blnDeviceReady);
                            m_objVisionPage[i].CustomizeGUI();
                            m_strSelectedRecipePrev = m_smProductionInfo.g_strRecipeID;
                            m_arrSelectedRecipePrev[i] = m_smProductionInfo.g_arrSingleRecipeID[i];
                            m_smVSInfo[i].AT_VP_NewLot = true;
                            m_smVSInfo[i].AT_VM_UpdateProductionDisplayAll = true;
                            m_smVSInfo[i].PG_VM_LoadTemplate = true;
                        }
                    }
                    m_smProductionInfo.g_blnLoadRecipePass = true;
                    m_smProductionInfo.g_blnSECSGEMSInit = true;
                }
                catch (Exception ex)
                {
                    m_objTrack.WriteLine("AutoForm (COMLoadRecipe): " + ex.ToString());
                    m_smProductionInfo.g_blnLoadRecipePass = false;
                }
                finally
                {
                    m_smProductionInfo.AT_CO_LoadRecipeDone = true;
                }
            }

            if (m_smProductionInfo.CO_AT_NewLot)
            {
                m_smProductionInfo.CO_AT_NewLot = false;
                if (!m_smProductionInfo.g_blnEndLotStatus)
                    CreateNewLot(m_smProductionInfo.g_intVisionInvolved);
                else
                    ProcessNewLot(m_smProductionInfo.g_intVisionInvolved);

                if (!btn_ProductionAll.Enabled)
                    btn_ProductionAll.Enabled = true;
            }

            if (m_smProductionInfo.CO_AT_EndLot)
            {
                m_smProductionInfo.CO_AT_EndLot = false;
                if (!m_smProductionInfo.g_blnEndLotStatus)
                {
                    ProcessEndLot(m_smProductionInfo.g_intVisionInvolved);
                    if (!btn_ProductionAll.Enabled)
                        btn_ProductionAll.Enabled = true;
                }
                else
                {
                    //2021-11-16 ZJYEOH : no need to do anything when already end lot
                    m_smProductionInfo.g_blnEndLotPass = true;//false
                    //SRMMessageBox.Show("Current lot has already ended previously!", "SRM Vision");
                    m_smProductionInfo.AT_CO_EndLotDone = true;
                }
            }

            if (m_smProductionInfo.CO_AT_RerunLot)
            {
                m_smProductionInfo.CO_AT_RerunLot = false;
                if (!m_smProductionInfo.g_blnEndLotStatus)
                    CreateRerunLot(m_smProductionInfo.g_intVisionInvolved);
                else
                    ProcessRerunLot(m_smProductionInfo.g_intVisionInvolved);

                if (!btn_ProductionAll.Enabled)
                    btn_ProductionAll.Enabled = true;
            }

            if (m_smProductionInfo.g_blnViewMultipleImageOff)
            {
                m_smProductionInfo.g_blnViewMultipleImageOff = false;

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    m_smVSInfo[i].g_blnSetMultipleImageViewOff = true;
                }
            }

            //if (m_intSelectedVisionStation > 1)
            //{
            //    if (!m_smVSInfo[m_intSelectedVisionStation - 1].g_blncboImageView)
            //    {
            //        cbo_ViewImage.Visible = false;

            //    }
            //    else
            //        cbo_ViewImage.Visible = true;
            //}

            VisibleViewImageComboBox();

        }

        private void btn_DisplayConfig_Click(object sender, EventArgs e)
        {
            SelectVisionDisplay objSelectedVisionDisplay = new SelectVisionDisplay(m_smVSInfo, m_smProductionInfo.g_arrDisplayVisionModule);

            if (objSelectedVisionDisplay.ShowDialog() == DialogResult.OK)
            {
                m_smProductionInfo.g_arrDisplayVisionModule = objSelectedVisionDisplay.ref_arrSelectedVisionModules;

                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subKey1 = key.CreateSubKey("SVG\\AutoMode");

                subKey1.SetValue("SelectedVisionModuleDisplay1", m_smProductionInfo.g_arrDisplayVisionModule[0]);
                subKey1.SetValue("SelectedVisionModuleDisplay2", m_smProductionInfo.g_arrDisplayVisionModule[1]);
                subKey1.SetValue("SelectedVisionModuleDisplay3", m_smProductionInfo.g_arrDisplayVisionModule[2]);
                subKey1.SetValue("SelectedVisionModuleDisplay4", m_smProductionInfo.g_arrDisplayVisionModule[3]);
                subKey1.SetValue("SelectedVisionModuleDisplay5", m_smProductionInfo.g_arrDisplayVisionModule[4]);
                subKey1.SetValue("SelectedVisionModuleDisplay6", m_smProductionInfo.g_arrDisplayVisionModule[5]);
                subKey1.SetValue("SelectedVisionModuleDisplay7", m_smProductionInfo.g_arrDisplayVisionModule[6]);
                subKey1.SetValue("SelectedVisionModuleDisplay8", m_smProductionInfo.g_arrDisplayVisionModule[7]);
            }


            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                m_smVSInfo[i].ALL_VM_UpdatePictureBox = true;
                m_smVSInfo[i].VS_AT_UpdateQuantity = true;
            }
        }

        private void cbo_ViewImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 2019 03 29 - CCENG: cbo_ViewImage_SelectedIndexChanged will be replaced by cbo_ViewImage_SelectionChangeCommitted

            //RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            //RegistryKey subKey1 = key.CreateSubKey("SVG\\AutoMode");

            //for (int i = 0; i < m_smVSInfo.Length; i++)
            //{
            //    if (m_smVSInfo[i] == null)
            //        continue;

            //    if (i != (m_intSelectedVisionStation - 1))
            //        continue;

            //    if (m_smVSInfo[i].g_intImageMergeType != 0)
            //    {
            //        if (m_smVSInfo[i].g_intImageMergeType == 1)     // Merge grab 1 center and grab 2 side 
            //        {
            //            if (m_smVSInfo[i].g_arrImages.Count <= cbo_ViewImage.SelectedIndex + 1)
            //                m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 0;  // User select View Image 1
            //            else if (cbo_ViewImage.SelectedIndex == 1)
            //                m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 2;  // User select View Image 2
            //            else if (cbo_ViewImage.SelectedIndex == 2)
            //                m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 3;
            //            else
            //                m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 0;
            //        }
            //        else
            //        {
            //            if (m_smVSInfo[i].g_arrImages.Count <= cbo_ViewImage.SelectedIndex + 2)
            //                m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 0;
            //            else if (cbo_ViewImage.SelectedIndex == 1)
            //                m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 3;
            //            else
            //                m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 0;
            //        }
            //    }
            //    else
            //    {
            //        if (m_smVSInfo[i].g_arrImages.Count <= cbo_ViewImage.SelectedIndex)
            //            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 0;
            //        else
            //            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = cbo_ViewImage.SelectedIndex;
            //    }

            //    m_smVSInfo[i].ALL_VM_UpdatePictureBox = true;
            //    m_smVSInfo[i].VS_AT_UpdateQuantity = true;

            //    subKey1.SetValue("CurrentSelectedImage" + i.ToString(), m_smVSInfo[i].g_intSelectedImage);

            //    break;
            //}
        }

        private void UpdateSelectImageList()
        {

        }

        private void btn_Template_Click(object sender, EventArgs e)
        {
            //
            //STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + "-View Template", "Pressed View Template Button", "", "", m_smProductionInfo.g_strLotID);
            //
            TemplateImageForm objTemplateImageForm = new TemplateImageForm(m_smVSInfo[m_intSelectedVisionStation - 1], m_smProductionInfo.g_strRecipePath, m_smProductionInfo.g_strRecipeID);
            objTemplateImageForm.ShowDialog();
        }

        private void btn_LoadImage2_ButtonClick(object sender, EventArgs e)
        {
            LoadImages("", "");
        }

        private void LoadImages(string strPath, string strDlgFileName)
        {
            VisionInfo smVisionInfo = m_smVSInfo[m_intSelectedVisionStation - 1];

            if (strPath != "")
            {
                if (Directory.Exists(strPath))
                {
                    dlg_ImageFile.Reset();
                    dlg_ImageFile.InitialDirectory = strPath;
                }
            }
            else
            {
                if (m_arrStrLoadImagePathPrev[m_intSelectedVisionStation - 1] != "" && m_arrStrLoadImagePathPrev[m_intSelectedVisionStation - 1] != null)
                {
                    dlg_ImageFile.Reset();
                    dlg_ImageFile.InitialDirectory = m_arrStrLoadImagePathPrev[m_intSelectedVisionStation - 1];
                }
                else
                {
                    //    string strPath1 = smVisionInfo.g_strSaveImageLocation +
                    //         m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime;

                    //    string strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")";

                    //    dlg_ImageFile.InitialDirectory = strPath1 + "\\" + strVisionImageFolderName + "\\";

                }
            }

            dlg_ImageFile.FileName = strDlgFileName;

            if (dlg_ImageFile.ShowDialog() == DialogResult.OK)
            {
                string strFileName = dlg_ImageFile.FileName;

                // Filter the string "_Image#"
                if (strFileName.ToLower().LastIndexOf("_image1.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image2.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image3.bmp") > 0 
                    || strFileName.ToLower().LastIndexOf("_image4.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image5.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image6.bmp") > 0)
                {
                    strFileName = strFileName.Substring(0, strFileName.IndexOf("_Image")) + ".bmp";
                }

                string strDirectoryName = Path.GetDirectoryName(strFileName);
                int i = 0;
                smVisionInfo.g_arrImageFiles.Clear();
                smVisionInfo.g_intFileIndex = -1;
                bool blnEnable = false;

                string[] strImageFiles = Directory.GetFiles(strDirectoryName, "*.bmp");

                // Sorting
                //string[] strImageFiles2 = new string[strImageFiles.Length];
                //for (int j = 0; j < 100; j++)
                //{
                //    strImageFiles2[j] = strImageFiles[j];
                //}
                // Array.Sort(strImageFiles, m_objTimeComparer.CompareCreateAscending);

                int intImageLimit = 30;
                int intTotalLoadImageLimit = strImageFiles.Length;
                bool blnLoadAllImage = true;
                bool blnCheckSkipImage = false;
                if ((strImageFiles.Length / smVisionInfo.g_arrImages.Count) > intImageLimit)
                {
                    if (SRMMessageBox.Show("Too many images in this folder. Do you want to load all the images? \nPress No will load " + intImageLimit.ToString() + " images only.", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        intTotalLoadImageLimit = intImageLimit;
                        blnLoadAllImage = false;
                        blnCheckSkipImage = true;
                    }
                }

                NumericComparer ns = new NumericComparer();
                Array.Sort(strImageFiles, ns);


                int intSkipImageCount = 0;
                long longFirstValidFileLength = 0;
                foreach (string strName in strImageFiles)
                {
                    if (strName.IndexOf("_Image") >= 0)
                        continue;

                    try
                    {
                        if (!blnLoadAllImage && blnCheckSkipImage)
                        {
                            if (strName.ToLower() == strFileName.ToLower())
                                blnCheckSkipImage = false;
                            else
                            {
                                if (((int)Math.Ceiling((float)strImageFiles.Length / smVisionInfo.g_arrImages.Count) - intSkipImageCount) > intTotalLoadImageLimit)
                                {
                                    intSkipImageCount++;
                                    continue;
                                }
                                else
                                    blnCheckSkipImage = false;
                            }
                        }

                        bool blnValidImage = false;
                        if (longFirstValidFileLength == 0)
                        {
                            Image objImage = Image.FromFile(strName);

                            if (smVisionInfo.g_blnViewColorImage)
                            {
                                if (objImage.Width == smVisionInfo.g_intCameraResolutionWidth && objImage.Height == smVisionInfo.g_intCameraResolutionHeight)
                                {
                                    blnValidImage = true;
                                }
                            }
                            else
                            {
                                // 2019 04 12-CCENG: if loaded image is PixelFormat.Format24bppRgb (color image), Euresys will auto convert it to mono color during loading image.
                                if ((PixelFormat.Format8bppIndexed == objImage.PixelFormat || PixelFormat.Format24bppRgb == objImage.PixelFormat) &&
                                    objImage.Width == smVisionInfo.g_intCameraResolutionWidth &&
                                    objImage.Height == smVisionInfo.g_intCameraResolutionHeight)
                                {
                                    blnValidImage = true;
                                }
                            }

                            objImage.Dispose();
                        }
                        else
                        {
                            long fileLength = new System.IO.FileInfo(strName).Length;
                            //if (longFirstValidFileLength == fileLength)
                            //{
                            //    blnValidImage = true;
                            //}
                            Image objImage = Image.FromFile(strName);
                            if (objImage.Width == smVisionInfo.g_intCameraResolutionWidth && objImage.Height == smVisionInfo.g_intCameraResolutionHeight)
                            {
                                blnValidImage = true;
                            }
                            objImage.Dispose();
                        }
                        if (blnValidImage)
                        {
                            if (longFirstValidFileLength == 0)
                            {
                                longFirstValidFileLength = new System.IO.FileInfo(strName).Length;
                            }

                            if (!smVisionInfo.g_blnViewColorImage)
                            {
                                smVisionInfo.g_arrImageFiles.Add(strName);
                                if (strName.ToLower() == strFileName.ToLower())
                                {
                                    smVisionInfo.g_intFileIndex = i;
                                    smVisionInfo.g_arrImages[0].LoadImage(strFileName);
                                    //smVisionInfo.g_arrImages[0].SaveImage("D:\\TS\\imagetest.bmp");

                                    for (int x = 1; x < smVisionInfo.g_arrImages.Count; x++)
                                    {
                                        string strDirPath = Path.GetDirectoryName(strFileName);
                                        string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + x.ToString() + ".BMP";

                                        if (File.Exists(strPkgView))
                                            smVisionInfo.g_arrImages[x].LoadImage_CopyToTempFolderFirst(strPkgView);
                                        else
                                            smVisionInfo.g_arrImages[x].LoadImage_CopyToTempFolderFirst(strFileName);
                                    }

                                    smVisionInfo.AT_PR_AttachImagetoROI = true;
                                    smVisionInfo.g_blnLoadFile = true;
                                }
                                i++;
                            }
                            else if (smVisionInfo.g_blnViewColorImage)
                            {
                                smVisionInfo.g_arrImageFiles.Add(strName);
                                if (strName.ToLower() == strFileName.ToLower())
                                {
                                    smVisionInfo.g_intFileIndex = i;
                                    smVisionInfo.g_arrColorImages[0].LoadImage(strFileName);
                                    smVisionInfo.g_arrColorImages[0].ConvertColorToMono(ref smVisionInfo.g_arrImages, 0);
                                    for (int x = 1; x < smVisionInfo.g_arrColorImages.Count; x++)
                                    {
                                        string strDirPath = Path.GetDirectoryName(strFileName);
                                        string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + x.ToString() + ".BMP";

                                        if (File.Exists(strPkgView))
                                            smVisionInfo.g_arrColorImages[x].LoadImage(strPkgView);
                                        else
                                            smVisionInfo.g_arrColorImages[x].LoadImage(strFileName);

                                        smVisionInfo.g_arrColorImages[x].ConvertColorToMono(ref smVisionInfo.g_arrImages, x);
                                    }
                                    
                                    smVisionInfo.AT_PR_AttachImagetoROI = true;
                                    smVisionInfo.g_blnLoadFile = true;
                                }
                                i++;
                            }

                            if (i >= intTotalLoadImageLimit)
                                break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (i > 1)
                    blnEnable = true;
                if (smVisionInfo.g_intFileIndex == -1)
                {
                    SRMMessageBox.Show("Invalid File Format", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    smVisionInfo.g_blnLoadFile = false;
                }
                else
                {
                    string[] LoadImageName = strImageFiles[0].Split('\\');
                    //
                    //STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + "-Load Image", "Image File Name", "", LoadImageName[LoadImageName.Length - 1], m_smProductionInfo.g_strLotID);
                    //
                }
                smVisionInfo.g_blnClearResult = true;
                smVisionInfo.g_blnViewRotatedImage = false;
                btn_ImageNext.Enabled = blnEnable;
                btn_ImagePrev.Enabled = blnEnable;

                m_arrStrLoadImagePathPrev[m_intSelectedVisionStation - 1] = strDirectoryName;
            }

            // 2020 09 29 - CCENG: when set the load template image, current lot and previous lot path as default
            //Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            if (smVisionInfo.g_bImageStatisticAnalysisON)
            {
                smVisionInfo.g_bImageStatisticAnalysisUpdateInfo = true;
            }

            if (smVisionInfo.g_bPocketPositionMeanStatisticAnalysisON)
            {
                smVisionInfo.g_bPocketPositionMeanStatisticAnalysisUpdateInfo = true;
            }

        }

        private void LoadCurrentLotImages_AfterStopProduction(string strFileName, bool blnFirstTime, int selectedVision)
        {
            if (strFileName.Length == 0)
                return;

            VisionInfo smVisionInfo = m_smVSInfo[selectedVision];

            // Filter the string "_Image#"
            if (strFileName.ToLower().LastIndexOf("_image1.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image2.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image3.bmp") > 0
                || strFileName.ToLower().LastIndexOf("_image4.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image5.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image6.bmp") > 0)
            {
                strFileName = strFileName.Substring(0, strFileName.IndexOf("_Image")) + ".bmp";
            }

            string strDirectoryName = Path.GetDirectoryName(strFileName);
            int i = 0;
            //if (blnFirstTime)
            //    smVisionInfo.g_arrImageFiles.Clear();
            //smVisionInfo.g_intFileIndex = -1;
            bool blnEnable = false;

            string[] strImageFiles = Directory.GetFiles(strDirectoryName, "*.bmp");

            // Sorting
            //string[] strImageFiles2 = new string[strImageFiles.Length];
            //for (int j = 0; j < 100; j++)
            //{
            //    strImageFiles2[j] = strImageFiles[j];
            //}
            // Array.Sort(strImageFiles, m_objTimeComparer.CompareCreateAscending);

            int intImageLimit = 30;
            int intTotalLoadImageLimit = strImageFiles.Length;
            bool blnLoadAllImage = true;
            bool blnCheckSkipImage = false;
            //if ((strImageFiles.Length / smVisionInfo.g_arrImages.Count) > intImageLimit)
            //{
            //   // if (SRMMessageBox.Show("Too many images in this folder. Do you want to load all the images? \nPress No will load " + intImageLimit.ToString() + " images only.", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            //    {
            //        intTotalLoadImageLimit = intImageLimit;
            //        blnLoadAllImage = false;
            //        blnCheckSkipImage = true;
            //    }
            //}

            NumericComparer ns = new NumericComparer();
            Array.Sort(strImageFiles, ns);

            int intSkipImageCount = 0;
            long longFirstValidFileLength = 0;
            foreach (string strName in strImageFiles)
            {
                if (strName.IndexOf("_Image") >= 0)
                    continue;

                try
                {
                    if (!blnLoadAllImage && blnCheckSkipImage)
                    {
                        if (strName.ToLower() == strFileName.ToLower())
                            blnCheckSkipImage = false;
                        else
                        {
                            if (((int)Math.Ceiling((float)strImageFiles.Length / smVisionInfo.g_arrImages.Count) - intSkipImageCount) > intTotalLoadImageLimit)
                            {
                                intSkipImageCount++;
                                continue;
                            }
                            else
                                blnCheckSkipImage = false;
                        }
                    }

                    bool blnValidImage = false;
                    if (longFirstValidFileLength == 0)
                    {
                        if (File.Exists(strName))
                        {
                            Image objImage = Image.FromFile(strName);

                            if (smVisionInfo.g_blnViewColorImage)
                            {
                                if (objImage.Width == smVisionInfo.g_intCameraResolutionWidth && objImage.Height == smVisionInfo.g_intCameraResolutionHeight)
                                {
                                    blnValidImage = true;
                                }
                            }
                            else
                            {
                                // 2019 04 12-CCENG: if loaded image is PixelFormat.Format24bppRgb (color image), Euresys will auto convert it to mono color during loading image.
                                if ((PixelFormat.Format8bppIndexed == objImage.PixelFormat || PixelFormat.Format24bppRgb == objImage.PixelFormat) &&
                                    objImage.Width == smVisionInfo.g_intCameraResolutionWidth &&
                                    objImage.Height == smVisionInfo.g_intCameraResolutionHeight)
                                {
                                    blnValidImage = true;
                                }
                            }

                            objImage.Dispose();
                        }
                    }
                    else
                    {
                        if (File.Exists(strName))
                        {
                            long fileLength = new System.IO.FileInfo(strName).Length;
                            if (longFirstValidFileLength == fileLength)
                            {
                                blnValidImage = true;
                            }
                        }
                    }
                    if (blnValidImage)
                    {
                        if (longFirstValidFileLength == 0)
                        {
                            longFirstValidFileLength = new System.IO.FileInfo(strName).Length;
                        }

                        if (!smVisionInfo.g_blnViewColorImage)
                        {
                            smVisionInfo.g_arrImageFiles.Add(strName);
                            //if (strName.ToLower() == strFileName.ToLower())
                            {
                                smVisionInfo.g_intFileIndex = i;
                                //    smVisionInfo.g_arrImages[0].LoadImage(strFileName);
                                //    //smVisionInfo.g_arrImages[0].SaveImage("D:\\TS\\imagetest.bmp");

                                //    for (int x = 1; x < smVisionInfo.g_arrImages.Count; x++)
                                //    {
                                //        string strDirPath = Path.GetDirectoryName(strFileName);
                                //        string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + x.ToString() + ".BMP";

                                //        if (File.Exists(strPkgView))
                                //            smVisionInfo.g_arrImages[x].LoadImage(strPkgView);
                                //        else
                                //            smVisionInfo.g_arrImages[x].LoadImage(strFileName);
                                //    }

                                //    smVisionInfo.AT_PR_AttachImagetoROI = true;
                                //    smVisionInfo.g_blnLoadFile = true;
                            }
                            i++;
                        }
                        else if (smVisionInfo.g_blnViewColorImage)
                        {
                            smVisionInfo.g_arrImageFiles.Add(strName);
                            //if (strName.ToLower() == strFileName.ToLower())
                            {
                                smVisionInfo.g_intFileIndex = i;
                                //    smVisionInfo.g_arrColorImages[0].LoadImage(strFileName);

                                //    for (int x = 1; x < smVisionInfo.g_arrColorImages.Count; x++)
                                //    {
                                //        string strDirPath = Path.GetDirectoryName(strFileName);
                                //        string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + x.ToString() + ".BMP";

                                //        if (File.Exists(strPkgView))
                                //            smVisionInfo.g_arrColorImages[x].LoadImage(strPkgView);
                                //        else
                                //            smVisionInfo.g_arrColorImages[x].LoadImage(strFileName);
                                //    }

                                //    smVisionInfo.AT_PR_AttachImagetoROI = true;
                                //    smVisionInfo.g_blnLoadFile = true;
                            }
                            i++;
                        }

                        if (i >= intTotalLoadImageLimit)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    STTrackLog.WriteLine("AutoForm > LoadCurrentLotImages_AfterStopProduction > Ex=" + ex.ToString());
                    continue;
                }
            }

            if (i > 1)
                blnEnable = true;
            //if (smVisionInfo.g_intFileIndex == -1)
            //{
            //    SRMMessageBox.Show("Invalid File Format", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    smVisionInfo.g_blnLoadFile = false;
            //}

            smVisionInfo.g_blnClearResult = true;
            smVisionInfo.g_blnViewRotatedImage = false;
            btn_ImageNext.Enabled = blnEnable;
            btn_ImagePrev.Enabled = blnEnable;

            m_arrStrLoadImagePathPrev[selectedVision] = strDirectoryName;
        }

        private void LoadImages(string strFileName)
        {
            if (strFileName.Length == 0)
                return;

            VisionInfo smVisionInfo = m_smVSInfo[m_intSelectedVisionStation - 1];

            // Filter the string "_Image#"
            if (strFileName.ToLower().LastIndexOf("_image1.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image2.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image3.bmp") > 0
                || strFileName.ToLower().LastIndexOf("_image4.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image5.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image6.bmp") > 0)
            {
                strFileName = strFileName.Substring(0, strFileName.IndexOf("_Image")) + ".bmp";
            }

            string strDirectoryName = Path.GetDirectoryName(strFileName);
            int i = 0;
            smVisionInfo.g_arrImageFiles.Clear();
            smVisionInfo.g_intFileIndex = -1;
            bool blnEnable = false;

            string[] strImageFiles;
            strImageFiles = Directory.GetFiles(strDirectoryName, "*.bmp");

            //2021-01-28 ZJYEOH : Filter out all images with different name
            string ImageFileName;
            for (int j = 0; j < strImageFiles.Length; j++)
            {
                ImageFileName = strFileName.Substring(strFileName.LastIndexOf('\\') + 1, strFileName.Length - strFileName.LastIndexOf('\\') - 5);
                if (!strImageFiles[j].Contains(ImageFileName))
                {
                    strImageFiles[j] = "_Image";
                }
            }

            // Sorting
            //string[] strImageFiles2 = new string[strImageFiles.Length];
            //for (int j = 0; j < 100; j++)
            //{
            //    strImageFiles2[j] = strImageFiles[j];
            //}
            // Array.Sort(strImageFiles, m_objTimeComparer.CompareCreateAscending);

            int intImageLimit = 30;
            int intTotalLoadImageLimit = strImageFiles.Length;
            bool blnLoadAllImage = true;
            bool blnCheckSkipImage = false;
            if ((strImageFiles.Length / smVisionInfo.g_arrImages.Count) > intImageLimit)
            {
                if (SRMMessageBox.Show("Too many images in this folder. Do you want to load all the images? \nPress No will load " + intImageLimit.ToString() + " images only.", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    intTotalLoadImageLimit = intImageLimit;
                    blnLoadAllImage = false;
                    blnCheckSkipImage = true;
                }
            }

            NumericComparer ns = new NumericComparer();
            Array.Sort(strImageFiles, ns);

            int intSkipImageCount = 0;
            long longFirstValidFileLength = 0;
            foreach (string strName in strImageFiles)
            {
                if (strName.IndexOf("_Image") >= 0)
                    continue;

                try
                {
                    if (!blnLoadAllImage && blnCheckSkipImage)
                    {
                        if (strName.ToLower() == strFileName.ToLower())
                            blnCheckSkipImage = false;
                        else
                        {
                            if (((int)Math.Ceiling((float)strImageFiles.Length / smVisionInfo.g_arrImages.Count) - intSkipImageCount) > intTotalLoadImageLimit)
                            {
                                intSkipImageCount++;
                                continue;
                            }
                            else
                                blnCheckSkipImage = false;
                        }
                    }

                    bool blnValidImage = false;
                    if (longFirstValidFileLength == 0)
                    {
                        string strDestination = "C:\\Template_Temporary.bmp";
                        File.Copy(strName, strDestination, true);
                        Image objImage = Image.FromFile(strDestination);

                        if (smVisionInfo.g_blnViewColorImage)
                        {
                            if (objImage.Width == smVisionInfo.g_intCameraResolutionWidth && objImage.Height == smVisionInfo.g_intCameraResolutionHeight)
                            {
                                blnValidImage = true;
                            }
                        }
                        else
                        {
                            // 2019 04 12-CCENG: if loaded image is PixelFormat.Format24bppRgb (color image), Euresys will auto convert it to mono color during loading image.
                            if ((PixelFormat.Format8bppIndexed == objImage.PixelFormat || PixelFormat.Format24bppRgb == objImage.PixelFormat) &&
                                objImage.Width == smVisionInfo.g_intCameraResolutionWidth &&
                                objImage.Height == smVisionInfo.g_intCameraResolutionHeight)
                            {
                                blnValidImage = true;
                            }
                        }
                        objImage.Dispose();

                    }
                    else
                    {
                        long fileLength = new System.IO.FileInfo(strName).Length;
                        if (longFirstValidFileLength == fileLength)
                        {
                            blnValidImage = true;
                        }
                    }
                    if (blnValidImage)
                    {
                        if (longFirstValidFileLength == 0)
                        {
                            longFirstValidFileLength = new System.IO.FileInfo(strName).Length;
                        }

                        if (!smVisionInfo.g_blnViewColorImage)
                        {
                            smVisionInfo.g_arrImageFiles.Add(strName);
                            if (strName.ToLower() == strFileName.ToLower())
                            {
                                smVisionInfo.g_intFileIndex = i;
                                smVisionInfo.g_arrImages[0].LoadImage_CopyToTempFolderFirst(strFileName);
                                //smVisionInfo.g_arrImages[0].SaveImage("D:\\TS\\imagetest.bmp");

                                for (int x = 1; x < smVisionInfo.g_arrImages.Count; x++)
                                {
                                    string strDirPath = Path.GetDirectoryName(strFileName);
                                    string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + x.ToString() + ".BMP";

                                    if (File.Exists(strPkgView))
                                        smVisionInfo.g_arrImages[x].LoadImage_CopyToTempFolderFirst(strPkgView);
                                    else
                                        smVisionInfo.g_arrImages[x].LoadImage_CopyToTempFolderFirst(strFileName);
                                }

                                smVisionInfo.AT_PR_AttachImagetoROI = true;
                                smVisionInfo.g_blnLoadFile = true;
                            }
                            i++;
                        }
                        else if (smVisionInfo.g_blnViewColorImage)
                        {
                            smVisionInfo.g_arrImageFiles.Add(strName);
                            if (strName.ToLower() == strFileName.ToLower())
                            {
                                smVisionInfo.g_intFileIndex = i;
                                smVisionInfo.g_arrColorImages[0].LoadImage(strFileName);
                                smVisionInfo.g_arrColorImages[0].ConvertColorToMono(ref smVisionInfo.g_arrImages, 0);
                                for (int x = 1; x < smVisionInfo.g_arrColorImages.Count; x++)
                                {
                                    string strDirPath = Path.GetDirectoryName(strFileName);
                                    string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + x.ToString() + ".BMP";

                                    if (File.Exists(strPkgView))
                                        smVisionInfo.g_arrColorImages[x].LoadImage(strPkgView);
                                    else
                                        smVisionInfo.g_arrColorImages[x].LoadImage(strFileName);

                                    smVisionInfo.g_arrColorImages[x].ConvertColorToMono(ref smVisionInfo.g_arrImages, x);
                                }
                                
                                smVisionInfo.AT_PR_AttachImagetoROI = true;
                                smVisionInfo.g_blnLoadFile = true;
                            }
                            i++;
                        }

                        if (i >= intTotalLoadImageLimit)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            if (i > 1)
                blnEnable = true;
            if (smVisionInfo.g_intFileIndex == -1)
            {
                SRMMessageBox.Show("Invalid File Format", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                smVisionInfo.g_blnLoadFile = false;
            }
            else
            {
                string[] LoadImageName = strImageFiles[0].Split('\\');
                //
                //STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + "-Load Image", "Image File Name", "", LoadImageName[LoadImageName.Length - 1], m_smProductionInfo.g_strLotID);
                //
            }
            smVisionInfo.g_blnClearResult = true;
            smVisionInfo.g_blnViewRotatedImage = false;
            btn_ImageNext.Enabled = blnEnable;
            btn_ImagePrev.Enabled = blnEnable;

            // 2020 08 14 - no need to record template image path
            //m_arrStrLoadImagePathPrev[m_intSelectedVisionStation - 1] = strDirectoryName;
        }

        private void LoadImages(string strFileName, string[] arrFileName)
        {
            if (strFileName.Length == 0)
                return;

            VisionInfo smVisionInfo = m_smVSInfo[m_intSelectedVisionStation - 1];

            // Filter the string "_Image#"
            if (strFileName.ToLower().LastIndexOf("_image1.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image2.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image3.bmp") > 0
                || strFileName.ToLower().LastIndexOf("_image4.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image5.bmp") > 0 || strFileName.ToLower().LastIndexOf("_image6.bmp") > 0)
            {
                strFileName = strFileName.Substring(0, strFileName.IndexOf("_Image")) + ".bmp";
            }

            string strDirectoryName = Path.GetDirectoryName(strFileName);
            int i = 0;
            smVisionInfo.g_arrImageFiles.Clear();
            smVisionInfo.g_intFileIndex = -1;
            bool blnEnable = false;

            string[] strImageFiles = arrFileName;
            //if (intLoadType == 0)
            //{
            //    DirectoryInfo strParentDirectoryName = Directory.GetParent(strDirectoryName);
            //    strImageFiles = Directory.GetFiles(strParentDirectoryName.FullName, "*.bmp", SearchOption.AllDirectories);
            //}
            //else if (intLoadType == 1)
            //{
            //    DirectoryInfo strParentDirectoryName = Directory.GetParent(strDirectoryName);
            //    strImageFiles = Directory.GetFiles(strParentDirectoryName.FullName, "*.bmp", SearchOption.AllDirectories);

            //    List<string> arrImageFiles = new List<string>();
            //    for (int j = 0; j < strImageFiles.Length; j++)
            //    {
            //        if (strImageFiles[j].Contains("\\Pass\\"))
            //            continue;
            //        arrImageFiles.Add(strImageFiles[j]);
            //    }

            //    strImageFiles = arrImageFiles.ToArray();
            //}
            //else
            //    strImageFiles = Directory.GetFiles(strDirectoryName, "*.bmp");

            // Sorting
            //string[] strImageFiles2 = new string[strImageFiles.Length];
            //for (int j = 0; j < 100; j++)
            //{
            //    strImageFiles2[j] = strImageFiles[j];
            //}
            // Array.Sort(strImageFiles, m_objTimeComparer.CompareCreateAscending);

            int intImageLimit = 30;
            int intTotalLoadImageLimit = strImageFiles.Length;
            bool blnLoadAllImage = true;
            bool blnCheckSkipImage = false;
            if ((strImageFiles.Length / smVisionInfo.g_arrImages.Count) > intImageLimit)
            {
                if (SRMMessageBox.Show("Too many images in this folder. Do you want to load all the images? \nPress No will load " + intImageLimit.ToString() + " images only.", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    intTotalLoadImageLimit = intImageLimit;
                    blnLoadAllImage = false;
                    blnCheckSkipImage = true;
                }
            }

            //NumericComparer ns = new NumericComparer();
            //Array.Sort(strImageFiles, ns);

            int intSkipImageCount = 0;
            long longFirstValidFileLength = 0;
            foreach (string strName in strImageFiles)
            {
                if (strName.IndexOf("_Image") >= 0)
                    continue;

                try
                {
                    if (!blnLoadAllImage && blnCheckSkipImage)
                    {
                        if (strName.ToLower() == strFileName.ToLower())
                            blnCheckSkipImage = false;
                        else
                        {
                            if (((int)Math.Ceiling((float)strImageFiles.Length / smVisionInfo.g_arrImages.Count) - intSkipImageCount) > intTotalLoadImageLimit)
                            {
                                intSkipImageCount++;
                                continue;
                            }
                            else
                                blnCheckSkipImage = false;
                        }
                    }

                    bool blnValidImage = false;
                    if (longFirstValidFileLength == 0)
                    {
                        Image objImage = Image.FromFile(strName);

                        if (smVisionInfo.g_blnViewColorImage)
                        {
                            if (objImage.Width == smVisionInfo.g_intCameraResolutionWidth && objImage.Height == smVisionInfo.g_intCameraResolutionHeight)
                            {
                                blnValidImage = true;
                            }
                        }
                        else
                        {
                            // 2019 04 12-CCENG: if loaded image is PixelFormat.Format24bppRgb (color image), Euresys will auto convert it to mono color during loading image.
                            if ((PixelFormat.Format8bppIndexed == objImage.PixelFormat || PixelFormat.Format24bppRgb == objImage.PixelFormat) &&
                                objImage.Width == smVisionInfo.g_intCameraResolutionWidth &&
                                objImage.Height == smVisionInfo.g_intCameraResolutionHeight)
                            {
                                blnValidImage = true;
                            }
                        }

                        objImage.Dispose();
                    }
                    else
                    {
                        long fileLength = new System.IO.FileInfo(strName).Length;
                        if (longFirstValidFileLength == fileLength)
                        {
                            blnValidImage = true;
                        }
                    }
                    if (blnValidImage)
                    {
                        if (longFirstValidFileLength == 0)
                        {
                            longFirstValidFileLength = new System.IO.FileInfo(strName).Length;
                        }

                        if (!smVisionInfo.g_blnViewColorImage)
                        {
                            smVisionInfo.g_arrImageFiles.Add(strName);
                            if (strName.ToLower() == strFileName.ToLower())
                            {
                                smVisionInfo.g_intFileIndex = i;
                                smVisionInfo.g_arrImages[0].LoadImage(strFileName);
                                //smVisionInfo.g_arrImages[0].SaveImage("D:\\TS\\imagetest.bmp");

                                for (int x = 1; x < smVisionInfo.g_arrImages.Count; x++)
                                {
                                    string strDirPath = Path.GetDirectoryName(strFileName);
                                    string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + x.ToString() + ".BMP";

                                    if (File.Exists(strPkgView))
                                        smVisionInfo.g_arrImages[x].LoadImage(strPkgView);
                                    else
                                        smVisionInfo.g_arrImages[x].LoadImage(strFileName);
                                }

                                smVisionInfo.AT_PR_AttachImagetoROI = true;
                                smVisionInfo.g_blnLoadFile = true;
                            }
                            i++;
                        }
                        else if (smVisionInfo.g_blnViewColorImage)
                        {
                            smVisionInfo.g_arrImageFiles.Add(strName);
                            if (strName.ToLower() == strFileName.ToLower())
                            {
                                smVisionInfo.g_intFileIndex = i;
                                smVisionInfo.g_arrColorImages[0].LoadImage(strFileName);
                                smVisionInfo.g_arrColorImages[0].ConvertColorToMono(ref smVisionInfo.g_arrImages, 0);
                                for (int x = 1; x < smVisionInfo.g_arrColorImages.Count; x++)
                                {
                                    string strDirPath = Path.GetDirectoryName(strFileName);
                                    string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + x.ToString() + ".BMP";

                                    if (File.Exists(strPkgView))
                                        smVisionInfo.g_arrColorImages[x].LoadImage(strPkgView);
                                    else
                                        smVisionInfo.g_arrColorImages[x].LoadImage(strFileName);
                                    smVisionInfo.g_arrColorImages[x].ConvertColorToMono(ref smVisionInfo.g_arrImages, x);
                                }
                                
                                smVisionInfo.AT_PR_AttachImagetoROI = true;
                                smVisionInfo.g_blnLoadFile = true;
                            }
                            i++;
                        }

                        if (i >= intTotalLoadImageLimit)
                            break;
                    }
                }
                catch
                {
                    continue;
                }
            }

            if (i > 1)
                blnEnable = true;
            if (smVisionInfo.g_intFileIndex == -1)
            {
                SRMMessageBox.Show("Invalid File Format", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                smVisionInfo.g_blnLoadFile = false;
            }
            else
            {
                string[] LoadImageName = strImageFiles[0].Split('\\');
                //
                //STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + "-Load Image", "Image File Name", "", LoadImageName[LoadImageName.Length - 1], m_smProductionInfo.g_strLotID);
                //
            }
            smVisionInfo.g_blnClearResult = true;
            smVisionInfo.g_blnViewRotatedImage = false;
            btn_ImageNext.Enabled = blnEnable;
            btn_ImagePrev.Enabled = blnEnable;

            //m_arrStrLoadImagePathPrev[m_intSelectedVisionStation - 1] = strDirectoryName;
        }

        private void btn_TemplateImages_Click(object sender, EventArgs e)
        {
            VisionInfo smVisionInfo = m_smVSInfo[m_intSelectedVisionStation - 1];

            string strPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1];
            LoadRecipeImageForm objForm = new LoadRecipeImageForm(m_smCustomizeInfo, smVisionInfo, m_smProductionInfo, strPath + "\\" + smVisionInfo.g_strVisionFolderName + "\\", smVisionInfo.g_strVisionName);
            if (objForm.ShowDialog() == DialogResult.OK)
            {
                string strFileName = objForm.ref_strSelectedImagePath;
                LoadImages(strFileName);
            }
            objForm.Dispose();
            //LoadImages(strPath + "\\" + smVisionInfo.g_strVisionFolderName + "\\", "");
        }

        private void btn_LastImage_Click(object sender, EventArgs e)
        {
            VisionInfo smVisionInfo = m_smVSInfo[m_intSelectedVisionStation - 1];

            LoadImages(smVisionInfo.g_strLastImageFolder, smVisionInfo.g_strLastImageName);
        }

        private void btn_CurrentLotImage_Click(object sender, EventArgs e)
        {
            VisionInfo smVisionInfo = m_smVSInfo[m_intSelectedVisionStation - 1];

            string strPath = smVisionInfo.g_strSaveImageLocation +
                     m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime;

            //string strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")";

            string strVisionImageFolderName;
            if (smVisionInfo.g_intVisionResetCount == 0)
                strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")" + "_" + m_smProductionInfo.g_strLotStartTime;
            else
                strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")" + "_" + smVisionInfo.g_strVisionResetCountTime;

            LoadImageForm objForm = new LoadImageForm(m_smCustomizeInfo, smVisionInfo, m_smProductionInfo, strPath + "\\" + strVisionImageFolderName);
            if (objForm.ShowDialog() == DialogResult.OK)
            {
                string strFileName = objForm.ref_strSelectedImagePath;
                string[] arrFileNames = objForm.ref_arrSelectedImageList.ToArray();
                LoadImages(strFileName, arrFileNames);
            }
            objForm.Dispose();
        }

        private void btn_PreviousLotImage_Click(object sender, EventArgs e)
        {
            VisionInfo smVisionInfo = m_smVSInfo[m_intSelectedVisionStation - 1];

            //RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            //RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");

            //string strLotNoPrev = subkey1.GetValue("LotNoPrev", "SRM").ToString();
            //string strLotStartTimePrev = subkey1.GetValue("LotStartTimePrev", DateTime.Now.ToString("yyyyMMddHHmmss")).ToString();

            //string strPath = smVisionInfo.g_strSaveImageLocation +
            //         strLotNoPrev + "_" + strLotStartTimePrev;

            string strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")";
            PreviousLotImageFolderList objPreviousLotImageFolderList = new PreviousLotImageFolderList(smVisionInfo.g_strSaveImageLocation, strVisionImageFolderName);
            if (objPreviousLotImageFolderList.ShowDialog() == DialogResult.OK)
            {
                string strFolderPath = objPreviousLotImageFolderList.ref_strSelectedFolderPath;
                LoadImageForm objForm = new LoadImageForm(m_smCustomizeInfo, smVisionInfo, m_smProductionInfo, strFolderPath);
                if (objForm.ShowDialog() == DialogResult.OK)
                {
                    string strFileName = objForm.ref_strSelectedImagePath;
                    string[] arrFileNames = objForm.ref_arrSelectedImageList.ToArray();
                    LoadImages(strFileName, arrFileNames);
                }
                objForm.Dispose();
            }
            objPreviousLotImageFolderList.Dispose();
        }
        
        public void StartThread(bool blnInit)
        {
            STTrackLog.WriteLine("AutoForm StartThread 1");
            if (m_strSelectedRecipePrev != "")
            {
                m_intUserGroup = m_smProductionInfo.g_intUserGroup;
                m_blnDeviceReady = IsDeviceReady();

                ReadRegistry();

                bool blnRecipeChanged = false;
                for (int i = 0; i < m_objVisionPage.Length; i++)
                {
                    if (m_objVisionPage[i] == null)
                        continue;

                    //Handle situation when server and local have same recipe name
                    if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork && !m_blnUseServerRecipePrev)
                    {
                        m_objVisionPage[i].VisionPageReinitialization(m_blnDeviceReady, true);
                        blnRecipeChanged = true;
                    }
                    else if ((!m_smCustomizeInfo.g_blnConfigShowNetwork || !m_smCustomizeInfo.g_blnWantNetwork) && m_blnUseServerRecipePrev)
                    {
                        m_objVisionPage[i].VisionPageReinitialization(m_blnDeviceReady, true);
                        blnRecipeChanged = true;
                    }
                    else if ((m_arrSelectedRecipePrev[i] == m_smProductionInfo.g_arrSingleRecipeID[i]) && m_smProductionInfo.g_blnRecipeImported[i]) // 2020-03-31 ZJYEOH : reinit vision page if imported recipe
                    {
                        m_objVisionPage[i].UpdateGlobalSharingFile(m_smProductionInfo.g_arrSingleRecipeID[i]); //2021-08-18 ZJYEOH : Need to update all global sharing file as user might copy recipe manually into SVG folder and just change recipe when software running

                        m_objVisionPage[i].VisionPageReinitialization(true, true);
                        blnRecipeChanged = true;
                        m_smProductionInfo.g_blnRecipeImported[i] = false;
                    }
                    else if ((m_arrSelectedRecipePrev[i] == m_smProductionInfo.g_arrSingleRecipeID[i]) && m_smVSInfo[i].g_blnWantReloadRecipe) // 2020-03-31 ZJYEOH : reinit vision page if imported recipe
                    {
                        m_objVisionPage[i].UpdateGlobalSharingFile(m_smProductionInfo.g_arrSingleRecipeID[i]); //2021-08-18 ZJYEOH : Need to update all global sharing file as user might copy recipe manually into SVG folder and just change recipe when software running

                        m_objVisionPage[i].VisionPageReinitialization(true, true);
                        blnRecipeChanged = false;
                        m_smVSInfo[i].g_blnWantReloadRecipe = false;
                    }
                    else
                    {
                        if (m_arrSelectedRecipePrev[i] != m_smProductionInfo.g_arrSingleRecipeID[i])
                            m_objVisionPage[i].UpdateGlobalSharingFile(m_smProductionInfo.g_arrSingleRecipeID[i]); //2021-08-18 ZJYEOH : Need to update all global sharing file as user might copy recipe manually into SVG folder and just change recipe when software running

                        m_objVisionPage[i].VisionPageReinitialization(m_blnDeviceReady, m_arrSelectedRecipePrev[i] != m_smProductionInfo.g_arrSingleRecipeID[i]);
                        blnRecipeChanged = (m_arrSelectedRecipePrev[i] != m_smProductionInfo.g_arrSingleRecipeID[i]);
                    }
                }

                m_strSelectedRecipePrev = m_smProductionInfo.g_strRecipeID;

                for (int i = 0; i < 10; i++)
                    m_arrSelectedRecipePrev[i] = m_smProductionInfo.g_arrSingleRecipeID[i];

                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    m_blnUseServerRecipePrev = true;
                else
                    m_blnUseServerRecipePrev = false;

                if (blnRecipeChanged)
                    m_smProductionInfo.g_blnSECSGEMSInit = true;
            }
            else
            {
                m_strSelectedRecipePrev = m_smProductionInfo.g_strRecipeID;

                for (int i = 0; i < 10; i++)
                    m_arrSelectedRecipePrev[i] = m_smProductionInfo.g_arrSingleRecipeID[i];

                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    m_blnUseServerRecipePrev = true;
                else
                    m_blnUseServerRecipePrev = false;

                m_smProductionInfo.g_blnSECSGEMSInit = true;
            }
            STTrackLog.WriteLine("AutoForm StartThread 2");

            SetCustomView2();
            STTrackLog.WriteLine("AutoForm StartThread 3");

            //string strChild1 = "AutoMode";
            //string strChild2 = "Reset Counter";
            //if (m_smProductionInfo.g_intUserGroup > m_objUserRight.GetGroupLevel2(strChild1, strChild2))
            //{
            //    btn_ResetCounter.Enabled = false;
            //}
            //else
            //{
            //    btn_ResetCounter.Enabled = true;
            //}

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (m_objVisionPage[i] != null)
                    m_objVisionPage[i].StartThread();
            }
            STTrackLog.WriteLine("AutoForm StartThread 4");

            if (!blnInit)
            {
                switch (m_intSelectedVisionStation)
                {
                    case 0:
                        m_objDisplay1Page.ActivateTimer(true);
                        break;
                    case -1:
                        m_objDisplay2Page.ActivateTimer(true);
                        break;
                    case -2:
                        m_objDisplay3Page.ActivateTimer(true);
                        break;
                    case 1:
                        m_objVisionPage[0].ActivateTimer(true);
                        break;
                    case 2:
                        m_objVisionPage[1].ActivateTimer(true);
                        break;
                    case 3:
                        m_objVisionPage[2].ActivateTimer(true);
                        break;
                    case 4:
                        m_objVisionPage[3].ActivateTimer(true);
                        break;
                    case 5:
                        m_objVisionPage[4].ActivateTimer(true);
                        break;
                    case 6:
                        m_objVisionPage[5].ActivateTimer(true);
                        break;
                    case 7:
                        m_objVisionPage[6].ActivateTimer(true);
                        break;
                    case 8:
                        m_objVisionPage[7].ActivateTimer(true);
                        break;
                    case 9:
                        m_objVisionPage[8].ActivateTimer(true);
                        break;
                    case 10:
                        m_objVisionPage[9].ActivateTimer(true);
                        break;
                }
            }
            STTrackLog.WriteLine("AutoForm StartThread 5");

            if (m_intSelectedVisionStation > 0)
            {
                if (m_smVSInfo[m_intSelectedVisionStation - 1] != null)
                {
                    if (m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionName == "Barcode")
                        m_smProductionInfo.g_blnInBarCodePage = true;
                    else
                        m_smProductionInfo.g_blnInBarCodePage = false;
                    m_smProductionInfo.g_blnInDisplayAllPage = false;
                }
            }
            else
            {
                m_smProductionInfo.g_blnInBarCodePage = false;
                m_smProductionInfo.g_blnInDisplayAllPage = true;
            }
            STTrackLog.WriteLine("AutoForm StartThread 6");

            if (!blnInit)   // 2021 06 17 - CCENG: During first init, DisplayAll timer no need to start thread here bcos the timer will start at AutoForm_Load
            {
                if (m_objDisplay1Page != null)
                    m_objDisplay1Page.StartThread();
                if (m_objDisplay2Page != null)
                    m_objDisplay2Page.StartThread();
                if (m_objDisplay3Page != null)
                    m_objDisplay3Page.StartThread();
            }

            STTrackLog.WriteLine("AutoForm StartThread 7");

            if (m_smProductionInfo.g_blnAllRunFromCenter)
            {
                if (!m_blnDebugRunning)
                {
                    //List<string> arrThreadNameBF = new List<string>();
                    //List<string> arrThreadNameAF = new List<string>();
                    //arrThreadNameBF = ProcessTh.GetThreadsName("SRMVision");

                    m_Thread = new Thread(new ThreadStart(DebuRun));
                    m_Thread.IsBackground = true;
                    m_Thread.Priority = ThreadPriority.Lowest;
                    m_Thread.Start();

                    //Thread.Sleep(500);
                    //arrThreadNameAF = ProcessTh.GetThreadsName("SRMVision");
                    //ProcessTh.GetDifferentThreadsName(arrThreadNameAF, arrThreadNameBF, "1", 0x02);
                }
            }
            STTrackLog.WriteLine("AutoForm StartThread 8");

            UpdateNewLotButtonGUI();
            STTrackLog.WriteLine("AutoForm StartThread 9");

            m_smProductionInfo.AT_ALL_InAuto = true;
        } 

        private void DeleteFilesAndFoldersRecursively(string target_dir)
        {
            if (Directory.Exists(target_dir))
            {
                foreach (string file in Directory.GetFiles(target_dir))
                {
                    File.Delete(file);
                }

                foreach (string subDir in Directory.GetDirectories(target_dir))
                {
                    DeleteFilesAndFoldersRecursively(subDir);
                }

                Thread.Sleep(1); // This makes the difference between whether it works or not. Sleep(0) is not enough.
                Directory.Delete(target_dir);
            }
        }

        public void StopThread()
        {
            m_smProductionInfo.g_blnAllRunFromCenter = false;

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (m_objVisionPage[i] != null)
                    m_objVisionPage[i].StopThread();
            }
        }

        private void DebuRun()
        {
            m_blnDebugRunning = true;
            bool blnAllSOV = false;
            while (m_smProductionInfo.g_blnAllRunFromCenter)
            {
                try
                {
                    if (blnAllSOV)
                    {
                        bool blnAllFinishTest = true;
                        for (int i = 0; i < m_smVSInfo.Length; i++)
                        {
                            if (m_smVSInfo[i] == null)
                                continue;

                            if (m_smVSInfo[i].g_intMachineStatus == 2)
                            {
                                if (m_smVSInfo[i].g_blnDebugRUN)
                                {
                                    blnAllFinishTest = false;
                                    break;
                                }
                            }
                        }

                        if (blnAllFinishTest)
                        {
                            blnAllSOV = false;
                        }
                    }
                    else
                    {
                        bool blnFound = false;
                        for (int i = 0; i < m_smVSInfo.Length; i++)
                        {
                            if (m_smVSInfo[i] == null)
                                continue;

                            if (m_smVSInfo[i].g_intMachineStatus == 2)
                            {
                                m_smVSInfo[i].g_blnDebugRUN = true;
                                blnFound = true;
                            }
                        }

                        if (blnFound)
                            blnAllSOV = true;
                    }
                }
                catch
                {

                }

                Thread.Sleep(1);
            }

            Thread.Sleep(100);  // Sleep to make sure recall for this thread is not happen again.
            m_blnDebugRunning = false;
        }

        private void StartWaiting(string StrMessage)
        {
            m_thWaitingFormThread = new SRMWaitingFormThread();
            m_thWaitingFormThread.SetStartSplash(StrMessage);
            this.Enabled = false;
        }

        private void StopWaiting()
        {
            m_thWaitingFormThread.SetStopSplash();
            this.Enabled = true;
        }

        private void VisibleViewImageComboBox()
        {
            if (m_intSelectedVisionStation >= 1)    // 2020 09 12 - CCENG: Change from > to >=. Bcos if using >, Vision 1 Image Index combobox will be hidden when switch from all page to vision 1
            {
                if (m_smVSInfo[m_intSelectedVisionStation - 1].VM_AT_SettingInDialog)
                {
                    if (cbo_ViewImage.Visible)
                        cbo_ViewImage.Visible = false;

                }
                else
                {
                    if (!cbo_ViewImage.Visible)
                        if (cbo_ViewImage.Items.Count > 1)
                            cbo_ViewImage.Visible = true;
                }
            }

        }

        private void btn_VisionTab_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            // disable timer when leave a vision page to reduce CPU usage
            switch (m_intSelectedVisionStation)
            {
                case 0:
                    m_objDisplay1Page.ActivateTimer(false);
                    break;
                case -1:
                    if (m_objDisplay2Page != null)
                        m_objDisplay2Page.ActivateTimer(false);
                    break;
                case -2:
                    if (m_objDisplay3Page != null)
                        m_objDisplay3Page.ActivateTimer(false);
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    if (m_objVisionPage[m_intSelectedVisionStation - 1] == null)
                        m_intSelectedVisionStation = 1;
                    m_objVisionPage[m_intSelectedVisionStation - 1].ActivateTimer(false);
                    break;
            }

            btn_Display1Tab.BackColor = Color.FromArgb(210, 230, 255);
            btn_Display2Tab.BackColor = Color.FromArgb(210, 230, 255);
            btn_Display3Tab.BackColor = Color.FromArgb(210, 230, 255);
            btn_Vision1Tab.BackColor = Color.FromArgb(210, 230, 255);
            btn_Vision2Tab.BackColor = Color.FromArgb(210, 230, 255);
            btn_Vision3Tab.BackColor = Color.FromArgb(210, 230, 255);
            btn_Vision4Tab.BackColor = Color.FromArgb(210, 230, 255);
            btn_Vision5Tab.BackColor = Color.FromArgb(210, 230, 255);
            btn_Vision6Tab.BackColor = Color.FromArgb(210, 230, 255);
            btn_Vision7Tab.BackColor = Color.FromArgb(210, 230, 255);
            btn_Vision8Tab.BackColor = Color.FromArgb(210, 230, 255);
            btn_Vision9Tab.BackColor = Color.FromArgb(210, 230, 255);
            btn_Vision10Tab.BackColor = Color.FromArgb(210, 230, 255);
            //
            if (sender == btn_Display1Tab)
            {
                //STDeviceEdit.SaveDeviceEditLog("DisplayAll-Change Vision Station", "Pressed Vision Station Tab", GetTabButtonText(m_intSelectedVisionStation), GetTabButtonText(0), m_smProductionInfo.g_strLotID);
                btn_Display1Tab.BackColor = Color.FromArgb(255, 192, 128);
                tabCtrl_Auto.SelectedTab = tabPage_Display1;
                m_intSelectedVisionStation = 0;
                cbo_ViewImage.Visible = false;
                m_objDisplay1Page.ActivateTimer(true);
            }
            else if (sender == btn_Display2Tab)
            {
                //STDeviceEdit.SaveDeviceEditLog("DisplayAll-Change Vision Station", "Pressed Vision Station Tab", GetTabButtonText(m_intSelectedVisionStation), GetTabButtonText(-1), m_smProductionInfo.g_strLotID);
                btn_Display2Tab.BackColor = Color.FromArgb(255, 192, 128);
                tabCtrl_Auto.SelectedTab = tabPage_Display2;
                m_intSelectedVisionStation = -1;
                cbo_ViewImage.Visible = false;
                m_objDisplay2Page.ActivateTimer(true);
            }
            else if (sender == btn_Display3Tab)
            {
                //STDeviceEdit.SaveDeviceEditLog("DisplayAll-Change Vision Station", "Pressed Vision Station Tab", GetTabButtonText(m_intSelectedVisionStation), GetTabButtonText(-2), m_smProductionInfo.g_strLotID);
                btn_Display3Tab.BackColor = Color.FromArgb(255, 192, 128);
                tabCtrl_Auto.SelectedTab = tabPage_Display3;
                m_intSelectedVisionStation = -2;
                cbo_ViewImage.Visible = false;
                m_objDisplay3Page.ActivateTimer(true);
            }
            else if (sender == btn_Vision1Tab)
            {
                //STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[0].g_strVisionFolderName + "-Change Vision Station", "Pressed Vision Station Tab", GetTabButtonText(m_intSelectedVisionStation), GetTabButtonText(1), m_smProductionInfo.g_strLotID);
                btn_Vision1Tab.BackColor = Color.FromArgb(255, 192, 128);
                tabCtrl_Auto.SelectedTab = tabPage_Vision1;
                m_intSelectedVisionStation = 1;
                VisibleViewImageComboBox();
                m_objVisionPage[0].ActivateTimer(true);
            }
            else if (sender == btn_Vision2Tab)
            {
                //STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[1].g_strVisionFolderName + "-Change Vision Station", "Pressed Vision Station Tab", GetTabButtonText(m_intSelectedVisionStation), GetTabButtonText(2), m_smProductionInfo.g_strLotID);
                btn_Vision2Tab.BackColor = Color.FromArgb(255, 192, 128);
                VisibleViewImageComboBox();
                if (m_objVisionPage[1] != null)
                {
                    tabCtrl_Auto.SelectedTab = tabPage_Vision2;
                    m_intSelectedVisionStation = 2;
                    m_objVisionPage[1].ActivateTimer(true);
                }
            }
            else if (sender == btn_Vision3Tab)
            {
                //STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[2].g_strVisionFolderName + "-Change Vision Station", "Pressed Vision Station Tab", GetTabButtonText(m_intSelectedVisionStation), GetTabButtonText(3), m_smProductionInfo.g_strLotID);
                btn_Vision3Tab.BackColor = Color.FromArgb(255, 192, 128);
                tabCtrl_Auto.SelectedTab = tabPage_Vision3;
                m_intSelectedVisionStation = 3;
                VisibleViewImageComboBox();
                if (m_objVisionPage[2] != null)
                    m_objVisionPage[2].ActivateTimer(true);
            }
            else if (sender == btn_Vision4Tab)
            {
                //STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[3].g_strVisionFolderName + "-Change Vision Station", "Pressed Vision Station Tab", GetTabButtonText(m_intSelectedVisionStation), GetTabButtonText(4), m_smProductionInfo.g_strLotID);
                btn_Vision4Tab.BackColor = Color.FromArgb(255, 192, 128);
                tabCtrl_Auto.SelectedTab = tabPage_Vision4;
                m_intSelectedVisionStation = 4;
                VisibleViewImageComboBox();
                if (m_objVisionPage[3] != null)
                    m_objVisionPage[3].ActivateTimer(true);
            }
            else if (sender == btn_Vision5Tab)
            {
                //STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[4].g_strVisionFolderName + "-Change Vision Station", "Pressed Vision Station Tab", GetTabButtonText(m_intSelectedVisionStation), GetTabButtonText(5), m_smProductionInfo.g_strLotID);
                btn_Vision5Tab.BackColor = Color.FromArgb(255, 192, 128);
                tabCtrl_Auto.SelectedTab = tabPage_Vision5;
                m_intSelectedVisionStation = 5;
                VisibleViewImageComboBox();
                if (m_objVisionPage[4] != null)
                    m_objVisionPage[4].ActivateTimer(true);
            }
            else if (sender == btn_Vision6Tab)
            {
                //STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[5].g_strVisionFolderName + "-Change Vision Station", "Pressed Vision Station Tab", GetTabButtonText(m_intSelectedVisionStation), GetTabButtonText(6), m_smProductionInfo.g_strLotID);
                btn_Vision6Tab.BackColor = Color.FromArgb(255, 192, 128);
                tabCtrl_Auto.SelectedTab = tabPage_Vision6;
                m_intSelectedVisionStation = 6;
                VisibleViewImageComboBox();
                if (m_objVisionPage[5] != null)
                    m_objVisionPage[5].ActivateTimer(true);
            }
            else if (sender == btn_Vision7Tab)
            {
                //STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[6].g_strVisionFolderName + "-Change Vision Station", "Pressed Vision Station Tab", GetTabButtonText(m_intSelectedVisionStation), GetTabButtonText(7), m_smProductionInfo.g_strLotID);
                btn_Vision7Tab.BackColor = Color.FromArgb(255, 192, 128);
                tabCtrl_Auto.SelectedTab = tabPage_Vision7;
                m_intSelectedVisionStation = 7;
                VisibleViewImageComboBox();
                if (m_objVisionPage[6] != null)
                    m_objVisionPage[6].ActivateTimer(true);
            }
            else if (sender == btn_Vision8Tab)
            {
                //STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[7].g_strVisionFolderName + "-Change Vision Station", "Pressed Vision Station Tab", GetTabButtonText(m_intSelectedVisionStation), GetTabButtonText(8), m_smProductionInfo.g_strLotID);
                btn_Vision8Tab.BackColor = Color.FromArgb(255, 192, 128);
                tabCtrl_Auto.SelectedTab = tabPage_Vision8;
                m_intSelectedVisionStation = 8;
                VisibleViewImageComboBox();
                if (m_objVisionPage[7] != null)
                    m_objVisionPage[7].ActivateTimer(true);
            }
            else if (sender == btn_Vision9Tab)
            {
                //STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[8].g_strVisionFolderName + "-Change Vision Station", "Pressed Vision Station Tab", GetTabButtonText(m_intSelectedVisionStation), GetTabButtonText(9), m_smProductionInfo.g_strLotID);
                btn_Vision9Tab.BackColor = Color.FromArgb(255, 192, 128);
                tabCtrl_Auto.SelectedTab = tabPage_Vision9;
                m_intSelectedVisionStation = 9;
                VisibleViewImageComboBox();
                if (m_objVisionPage[8] != null)
                    m_objVisionPage[8].ActivateTimer(true);
            }
            else if (sender == btn_Vision10Tab)
            {
                //STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[9].g_strVisionFolderName + "-Change Vision Station", "Pressed Vision Station Tab", GetTabButtonText(m_intSelectedVisionStation), GetTabButtonText(10), m_smProductionInfo.g_strLotID);
                btn_Vision10Tab.BackColor = Color.FromArgb(255, 192, 128);
                tabCtrl_Auto.SelectedTab = tabPage_Vision10;
                m_intSelectedVisionStation = 10;
                VisibleViewImageComboBox();
                if (m_objVisionPage[9] != null)
                    m_objVisionPage[9].ActivateTimer(true);
            }
            //
            if (m_intSelectedVisionStation > 0)
            {
                if (m_smVSInfo[m_intSelectedVisionStation - 1] != null)
                {
                    if (m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionName == "Barcode")
                        m_smProductionInfo.g_blnInBarCodePage = true;
                    else
                        m_smProductionInfo.g_blnInBarCodePage = false;
                    m_smProductionInfo.g_blnInDisplayAllPage = false;
                }
            }
            else
            {
                m_smProductionInfo.g_blnInBarCodePage = false;
                m_smProductionInfo.g_blnInDisplayAllPage = true;
            }
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\AutoMode");
            subKey.SetValue("SelectedPane", m_intSelectedVisionStation);

            if (m_intSelectedVisionStation <= 0)
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                {
                    if (m_blnUseServerRecipePrev)
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Server)";
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID + " (Local)";
                }
                else
                    lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_strRecipeID;
            }
            else
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                {
                    if (m_blnUseServerRecipePrev)
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Server)";
                    else
                        lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1] + " (Local)";
                }
                else
                    lbl_Header.Text = GetLanguageText("Recipe") + " : " + m_smProductionInfo.g_arrSingleRecipeID[m_intSelectedVisionStation - 1];
            }

            EnableMenuButton();

            if ((m_intSelectedVisionStation - 1) >= 0 && m_smVSInfo[m_intSelectedVisionStation - 1] != null)
            {
                switch (m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionName)
                {
                    case "MarkPkg":
                    case "MOLiPkg":
                    case "MOPkg":
                    case "InPocketPkg":
                    case "IPMLiPkg":
                        if (m_smVSInfo[m_intSelectedVisionStation - 1].g_blnWantShowGRR)
                            btn_GRR.Visible = true;
                        else
                            btn_GRR.Visible = false;
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
                        if (m_smVSInfo[m_intSelectedVisionStation - 1].g_blnWantShowGRR)
                            btn_GRR.Visible = true;
                        else
                            btn_GRR.Visible = false;
                        break;
                    case "Li3D":
                    case "Li3DPkg":
                        if (m_smVSInfo[m_intSelectedVisionStation - 1].g_blnWantShowGRR)
                            btn_GRR.Visible = true;
                        else
                            btn_GRR.Visible = false;
                        break;
                    default:
                        btn_GRR.Visible = false;
                        break;
                }
            }

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (i != (m_intSelectedVisionStation - 1))
                    continue;

                if (m_smVSInfo[i].g_intImageMergeType != 0)
                {
                    int intViewImageCount = ImageDrawing.GetImageViewCount(m_smVSInfo[i].g_intVisionIndex);

                    if (intViewImageCount <= 0)
                        intViewImageCount = 1;

                    if (cbo_ViewImage.Items.Count != intViewImageCount)
                    {
                        cbo_ViewImage.Items.Clear();
                        for (int j = 0; j < intViewImageCount; j++)
                        {
                            cbo_ViewImage.Items.Add("Image " + (j + 1));
                        }
                        cbo_ViewImage.SelectedIndex = 0;
                    }

                    //if (intViewImageCount == 2)
                    //{
                    //    VisibleViewImageComboBox();

                    //    if (m_smVSInfo[i].g_intSelectedImage <= 1)
                    //        cbo_ViewImage.SelectedIndex = 0;
                    //    else if (m_smVSInfo[i].g_intSelectedImage == 2)
                    //        cbo_ViewImage.SelectedIndex = 1;
                    //    else
                    //        cbo_ViewImage.SelectedIndex = 0;
                    //}
                    //else if (intViewImageCount == 3)
                    //{
                    //    VisibleViewImageComboBox();

                    //    if (m_smVSInfo[i].g_intSelectedImage <= 1)
                    //        cbo_ViewImage.SelectedIndex = 0;
                    //    else if (m_smVSInfo[i].g_intSelectedImage == 2)
                    //        cbo_ViewImage.SelectedIndex = 1;
                    //    else if (m_smVSInfo[i].g_intSelectedImage == 3)
                    //        cbo_ViewImage.SelectedIndex = 2;
                    //    else
                    //        cbo_ViewImage.SelectedIndex = 0;
                    //}
                    //else if (intViewImageCount == 4)
                    //{
                    //    VisibleViewImageComboBox();

                    //    if (m_smVSInfo[i].g_intSelectedImage <= 1)
                    //        cbo_ViewImage.SelectedIndex = 0;
                    //    else if (m_smVSInfo[i].g_intSelectedImage == 2)
                    //        cbo_ViewImage.SelectedIndex = 1;
                    //    else if (m_smVSInfo[i].g_intSelectedImage == 3)
                    //        cbo_ViewImage.SelectedIndex = 2;
                    //    else if (m_smVSInfo[i].g_intSelectedImage == 4)
                    //        cbo_ViewImage.SelectedIndex = 3;
                    //    else
                    //        cbo_ViewImage.SelectedIndex = 0;
                    //}
                    //else if (intViewImageCount == 5)
                    //{
                    //    VisibleViewImageComboBox();

                    //    if (m_smVSInfo[i].g_intSelectedImage <= 1)
                    //        cbo_ViewImage.SelectedIndex = 0;
                    //    else if (m_smVSInfo[i].g_intSelectedImage == 2)
                    //        cbo_ViewImage.SelectedIndex = 1;
                    //    else if (m_smVSInfo[i].g_intSelectedImage == 3)
                    //        cbo_ViewImage.SelectedIndex = 2;
                    //    else if (m_smVSInfo[i].g_intSelectedImage == 4)
                    //        cbo_ViewImage.SelectedIndex = 3;
                    //    else if (m_smVSInfo[i].g_intSelectedImage == 5)
                    //        cbo_ViewImage.SelectedIndex = 4;
                    //    else
                    //        cbo_ViewImage.SelectedIndex = 0;
                    //}
                    //else if (intViewImageCount == 6)
                    //{
                    //    VisibleViewImageComboBox();

                    //    if (m_smVSInfo[i].g_intSelectedImage <= 1)
                    //        cbo_ViewImage.SelectedIndex = 0;
                    //    else if (m_smVSInfo[i].g_intSelectedImage == 2)
                    //        cbo_ViewImage.SelectedIndex = 1;
                    //    else if (m_smVSInfo[i].g_intSelectedImage == 3)
                    //        cbo_ViewImage.SelectedIndex = 2;
                    //    else if (m_smVSInfo[i].g_intSelectedImage == 4)
                    //        cbo_ViewImage.SelectedIndex = 3;
                    //    else if (m_smVSInfo[i].g_intSelectedImage == 5)
                    //        cbo_ViewImage.SelectedIndex = 4;
                    //    else if (m_smVSInfo[i].g_intSelectedImage == 6)
                    //        cbo_ViewImage.SelectedIndex = 5;
                    //    else
                    //        cbo_ViewImage.SelectedIndex = 0;
                    //}
                    //else
                    //{
                    //    m_smVSInfo[i].g_intSelectedImage = 0;
                    //    subKey.SetValue("CurrentSelectedImage" + i.ToString(), m_smVSInfo[i].g_intSelectedImage);
                    //    cbo_ViewImage.Visible = false;
                    //}

                    if (intViewImageCount > 1 || intViewImageCount <= 6)
                    {
                        VisibleViewImageComboBox();

                        int intSelectedIndex = ImageDrawing.GetImageViewNo(m_smVSInfo[i].g_intSelectedImage, m_smVSInfo[i].g_intVisionIndex);

                        if (cbo_ViewImage.Items.Count > intSelectedIndex)
                            cbo_ViewImage.SelectedIndex = intSelectedIndex;
                        else
                            cbo_ViewImage.SelectedIndex = 0;
                    }
                    else
                    {
                        m_smVSInfo[i].g_intSelectedImage = 0;
                        subKey.SetValue("CurrentSelectedImage" + i.ToString(), m_smVSInfo[i].g_intSelectedImage);
                        cbo_ViewImage.Visible = false;
                    }
                }
                else
                {
                    if (!m_smVSInfo[i].g_blnWantCheckPH && !(m_smVSInfo[i].g_strVisionName.Contains("Pad") || m_smVSInfo[i].g_strVisionName.Contains("BottomOrientPad") || m_smVSInfo[i].g_strVisionName.Contains("Li3D")))
                    {
                        if (cbo_ViewImage.Items.Contains("Image PH"))
                            cbo_ViewImage.Items.Remove("Image PH");
                    }
                    if (!m_smVSInfo[i].g_blnWantCheckEmpty && !(m_smVSInfo[i].g_strVisionName.Contains("InPocket") || m_smVSInfo[i].g_strVisionName.Contains("IPM")))
                    {
                        if (cbo_ViewImage.Items.Contains("Image Empty"))
                            cbo_ViewImage.Items.Remove("Image Empty");
                    }
                    if (cbo_ViewImage.Items.Count != m_smVSInfo[i].g_arrImages.Count)
                    {
                        cbo_ViewImage.Items.Clear();
                        for (int j = 0; j < m_smVSInfo[i].g_arrImages.Count; j++)
                        {
                            cbo_ViewImage.Items.Add("Image " + (j + 1));
                        }
                        cbo_ViewImage.SelectedIndex = 0;
                    }

                    if (m_smVSInfo[i].g_arrImages.Count > 1)
                    {
                        VisibleViewImageComboBox();

                        if (m_smVSInfo[i].g_intSelectedImage < cbo_ViewImage.Items.Count)
                            cbo_ViewImage.SelectedIndex = m_smVSInfo[i].g_intSelectedImage;
                        else
                            cbo_ViewImage.SelectedIndex = 0;
                    }
                    else
                        cbo_ViewImage.Visible = false;

                }
                //Add Image PH item
                if (m_smVSInfo[i].g_blnWantCheckPH && (m_smVSInfo[i].g_strVisionName.Contains("Pad") || m_smVSInfo[i].g_strVisionName.Contains("BottomOrientPad") || m_smVSInfo[i].g_strVisionName.Contains("Li3D")))
                {
                    if (!cbo_ViewImage.Items.Contains("Image PH"))
                        cbo_ViewImage.Items.Add("Image PH");
                    if (m_smVSInfo[i].g_intSelectedImage == 0 && m_smVSInfo[i].g_blnViewPHImage == true)
                        cbo_ViewImage.SelectedIndex = cbo_ViewImage.Items.Count - 1;
                }
                else
                {
                    if (cbo_ViewImage.Items.Contains("Image PH"))
                        cbo_ViewImage.Items.Remove("Image PH");
                }
                //Add Image Empty item
                if (m_smVSInfo[i].g_blnWantCheckEmpty && (m_smVSInfo[i].g_strVisionName.Contains("InPocket") || m_smVSInfo[i].g_strVisionName.Contains("IPM")))
                {
                    if (!cbo_ViewImage.Items.Contains("Image Empty"))
                        cbo_ViewImage.Items.Add("Image Empty");
                    if (m_smVSInfo[i].g_intSelectedImage == 0 && m_smVSInfo[i].g_blnViewEmptyImage == true)
                        cbo_ViewImage.SelectedIndex = cbo_ViewImage.Items.Count - 1;
                }
                else
                {
                    if (cbo_ViewImage.Items.Contains("Image Empty"))
                        cbo_ViewImage.Items.Remove("Image Empty");
                }
            }

        }

        private void AutoForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.S))
            {
                //Switch between view single image and view multiple image when "S" is press
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    // 2020 03 23 - JBTAN: dont allow press "S" when display all
                    if (i != (m_intSelectedVisionStation - 1))
                        continue;

                    if (m_smVSInfo[m_intSelectedVisionStation - 1].VM_AT_SettingInDialog || m_smVSInfo[m_intSelectedVisionStation - 1].VM_AT_OfflinePageView)
                    {
                        return;
                    }

                    if (i == (m_intSelectedVisionStation - 1))  // Affecting on selected vision module only.
                        m_smVSInfo[i].g_blnSetMultipleImageViewOnOff = !m_smVSInfo[i].g_blnSetMultipleImageViewOnOff;
                }
            }
        }

        private void cbo_ViewImage_SelectionChangeCommitted(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey1 = key.CreateSubKey("SVG\\AutoMode");

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (i != (m_intSelectedVisionStation - 1))
                    continue;

                if (m_smVSInfo[i].g_intImageMergeType != 0)
                {
                    if (m_smVSInfo[i].g_intImageMergeType == 1)     // Merge grab 1 center and grab 2 side 
                    {
                        if (m_smVSInfo[i].g_arrImages.Count <= cbo_ViewImage.SelectedIndex + 1)
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 0;  // User select View Image 1
                        else if (cbo_ViewImage.SelectedIndex == 1)
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 2;  // User select View Image 2
                        else if (cbo_ViewImage.SelectedIndex == 2)
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 3;
                        else if (cbo_ViewImage.SelectedIndex == 3)
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 4;
                        else if (cbo_ViewImage.SelectedIndex == 4)
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 5;
                        else if (cbo_ViewImage.SelectedIndex == 5)
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 6;
                        else
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 0;
                    }
                    else if (m_smVSInfo[i].g_intImageMergeType == 3)     // Merge grab 1 center and grab 2 side , Merge grab 3 center and grab 4 side
                    {
                        if (cbo_ViewImage.SelectedIndex == 0) // View Image 1
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 0;
                        else if (cbo_ViewImage.SelectedIndex == 1) // View Image 2
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 2;
                        else if (cbo_ViewImage.SelectedIndex == 2) // View Image 3
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 4;
                        else if (cbo_ViewImage.SelectedIndex == 3)
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 5;
                        else if (cbo_ViewImage.SelectedIndex == 4)
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 6;
                        else
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 0;
                    }
                    else if (m_smVSInfo[i].g_intImageMergeType == 4)     // Merge grab 1 center and grab 2 side TL and grab 3 side BR, Merge grab 4 center and grab 5 side
                    {
                        if (cbo_ViewImage.SelectedIndex == 0) // View Image 1
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 0;
                        else if (cbo_ViewImage.SelectedIndex == 1) // View Image 2
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 3;
                        else if (cbo_ViewImage.SelectedIndex == 2)
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 5;
                        else if (cbo_ViewImage.SelectedIndex == 3)
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 6;
                        else
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 0;
                    }
                    else
                    {
                        if (m_smVSInfo[i].g_arrImages.Count <= cbo_ViewImage.SelectedIndex + 2)
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 0;
                        else if (cbo_ViewImage.SelectedIndex == 1)
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 3;
                        else if (cbo_ViewImage.SelectedIndex == 2)
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 4;
                        else if (cbo_ViewImage.SelectedIndex == 3)
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 5;
                        else if (cbo_ViewImage.SelectedIndex == 4)
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 6;
                        else
                            m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 0;
                    }
                }
                else
                {
                    if (m_smVSInfo[i].g_arrImages.Count <= cbo_ViewImage.SelectedIndex)
                        m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = 0;
                    else
                        m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage = cbo_ViewImage.SelectedIndex;
                }
                if (m_smVSInfo[i].g_blnWantCheckPH)
                {
                    if (cbo_ViewImage.SelectedIndex == cbo_ViewImage.Items.Count - 1)
                    {
                        m_smVSInfo[i].g_intSelectedImage = 0;
                        m_smVSInfo[i].g_blnViewPHImage = true;
                    }
                    else
                    {
                        m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage;
                        m_smVSInfo[i].g_blnViewPHImage = false;
                    }
                }
                if (m_smVSInfo[i].g_blnWantCheckEmpty)
                {
                    if (cbo_ViewImage.SelectedIndex == cbo_ViewImage.Items.Count - 1)
                    {
                        m_smVSInfo[i].g_intSelectedImage = 0;
                        m_smVSInfo[i].g_blnViewEmptyImage = true;
                    }
                    else
                    {
                        m_smVSInfo[i].g_intSelectedImage = m_smVSInfo[i].g_intProductionViewImage;
                        m_smVSInfo[i].g_blnViewEmptyImage = false;
                    }
                }

                m_smVSInfo[i].ALL_VM_UpdatePictureBox = true;
                m_smVSInfo[i].VS_AT_UpdateQuantity = true;

                subKey1.SetValue("CurrentSelectedImage" + i.ToString(), m_smVSInfo[i].g_intSelectedImage);

                break;
            }
        }

        private void btn_SaveOption_Click(object sender, EventArgs e)
        {
            SaveOptionForm objForm = new SaveOptionForm(m_smVSInfo[m_intSelectedVisionStation - 1], m_smProductionInfo);
            objForm.ShowDialog();
        }

        private void btn_ResetCounter_Click(object sender, EventArgs e)
        {
            ResetCountForm objForm = new ResetCountForm(m_smProductionInfo, m_smVSInfo, m_smCustomizeInfo);
            objForm.ShowDialog();

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey1 = key.CreateSubKey("SVG\\AutoMode");

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                // 2020 03 27 - JBTAN: Save Vision Reset Count info
                subKey1.SetValue("VS" + (i + 1) + "ResetCount", m_smVSInfo[i].g_intVisionResetCount);
                subKey1.SetValue("VS" + (i + 1) + "ResetCountTime", m_smVSInfo[i].g_strVisionResetCountTime);
            }
        }

        private string GetTabButtonText(int intSelectedTab)
        {
            switch (intSelectedTab)
            {
                case 0:
                    return btn_Display1Tab.Text;
                case -1:
                    return btn_Display2Tab.Text;
                case -2:
                    return btn_Display3Tab.Text;
                case 1:
                    return btn_Vision1Tab.Text;
                case 2:
                    return btn_Vision2Tab.Text;
                case 3:
                    return btn_Vision3Tab.Text;
                case 4:
                    return btn_Vision4Tab.Text;
                case 5:
                    return btn_Vision5Tab.Text;
                case 6:
                    return btn_Vision6Tab.Text;
                case 7:
                    return btn_Vision7Tab.Text;
                case 8:
                    return btn_Vision8Tab.Text;
                case 9:
                    return btn_Vision9Tab.Text;
                case 10:
                    return btn_Vision10Tab.Text;
            }
            return "";
        }

        private bool CheckFailOptionSecureSetting(bool blnPressProductionAll, int intCurentVisionNo, bool blnSecureOn)
        {
            if (!blnSecureOn)
                return true;

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (!blnPressProductionAll)
                {
                    if (i != (m_intSelectedVisionStation - 1))
                        continue;
                }
                else
                {
                    if (i != intCurentVisionNo)
                        continue;
                }

                if (m_smVSInfo[i] == null)
                    continue;
                
                int intPos = (1 << i);
                switch (m_smVSInfo[i].g_strVisionName)
                {
                    case "BottomOrient":
                        bool blnOrientAllUnChecked = false, blnOrientNotTally = false;
                        CheckOrientOptionSecureSetting(m_smVSInfo[i], i, ref blnOrientAllUnChecked, ref blnOrientNotTally);
                        if (blnOrientAllUnChecked && blnOrientNotTally)
                        {
                            if (!blnSecureOn)
                                return true;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Orient inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Orient Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateOrientSecureOption(i);
                                return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Orient Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnOrientAllUnChecked)
                        {
                            if (!blnSecureOn)
                                return true;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Orient inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Orient Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateOrientSecureOption(i);
                                return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Orient Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnOrientNotTally)
                        {
                            if (!blnSecureOn)
                                return true;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Orient inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Orient Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateOrientSecureOption(i);
                                return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Orient Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        break;
                    case "Mark":
                    case "MarkOrient":
                    case "MOLi":
                    case "InPocket":
                    case "IPMLi":
                        List<bool> blnMarkAllUnChecked = new List<bool>();
                        List<bool> blnMarkNotTally = new List<bool>();
                        List<bool> blnMarkParentUnChecked = new List<bool>();
                        List<bool> blnMarkPin1NotTally = new List<bool>();
                        for (int v = 0; v < m_smVSInfo[i].g_intTotalTemplates; v++)
                        {
                            blnMarkAllUnChecked.Add(false);
                            blnMarkNotTally.Add(false);
                            blnMarkParentUnChecked.Add(false);
                            blnMarkPin1NotTally.Add(false);
                        }
                        CheckMarkOptionSecureSetting(m_smVSInfo[i], i, ref blnMarkAllUnChecked, ref blnMarkNotTally, ref blnMarkParentUnChecked, ref blnMarkPin1NotTally);
                        for (int v = 0; v < m_smVSInfo[i].g_intTotalTemplates; v++)
                        {
                            if (blnMarkAllUnChecked[v] && blnMarkNotTally[v])
                            {
                                if (blnMarkPin1NotTally[v])
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Mark and Pin1 inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkPin1SecureOption(i, v);
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Mark inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                            }
                            else if (blnMarkParentUnChecked[v] && blnMarkNotTally[v])
                            {
                                if (blnMarkPin1NotTally[v])
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Mark and Pin1 inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkPin1SecureOption(i, v);
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Mark in inspection option is UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                            }
                            else if (blnMarkAllUnChecked[v])
                            {
                                if (blnMarkPin1NotTally[v])
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Mark inspection option are UnChecked and Pin1 inspection option is not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkPin1SecureOption(i, v);
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Mark inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                            }
                            else if (blnMarkParentUnChecked[v])
                            {
                                if (blnMarkPin1NotTally[v])
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Mark inspection option is UnChecked and Pin1 inspection option is not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkPin1SecureOption(i, v);
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Mark in inspection option is UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                            }
                            else if (blnMarkNotTally[v])
                            {
                                if (blnMarkPin1NotTally[v])
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Mark and Pin 1 inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkPin1SecureOption(i, v);
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Mark inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLeadOnly;
                                        else
                                            return true;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                            }
                            else if (blnMarkPin1NotTally[v])
                            {
                                if (!blnSecureOn)
                                {
                                    if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                        goto CheckLeadOnly;
                                    else
                                        return true;
                                }

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Pin 1 inspection option is not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateMarkPin1SecureOption(i, v);
                                    if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                        goto CheckLeadOnly;
                                    else
                                        return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                        }

                    CheckLeadOnly:
                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                        {
                            List<bool> blnLeadAllUnChecked = new List<bool>();
                            List<bool> blnLeadNotTally = new List<bool>();
                            bool blnLeadParentUnChecked = false;
                            for (int v = 0; v < m_smVSInfo[i].g_arrLead.Length; v++)
                            {
                                blnLeadAllUnChecked.Add(false);
                                blnLeadNotTally.Add(false);
                            }
                            CheckLeadOptionSecureSetting(m_smVSInfo[i], i, ref blnLeadAllUnChecked, ref blnLeadNotTally, ref blnLeadParentUnChecked);
                            for (int v = 0; v < m_smVSInfo[i].g_arrLead.Length; v++)
                            {
                                if (!m_smVSInfo[i].g_arrLead[v].ref_blnSelected)
                                    continue;

                                string strSectionName = "";
                                if (v == 0)
                                    strSectionName = "SearchROI";
                                else if (v == 1)
                                    strSectionName = "TopROI";
                                else if (v == 2)
                                    strSectionName = "RightROI";
                                else if (v == 3)
                                    strSectionName = "BottomROI";
                                else if (v == 4)
                                    strSectionName = "LeftROI";

                                if (blnLeadAllUnChecked[v] && blnLeadNotTally[v])
                                {
                                    if (!blnSecureOn)
                                        return true;

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + " : " + strSectionName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " All Lead inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateLeadSecureOption(i, v);
                                        return true;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else if (blnLeadParentUnChecked && blnLeadNotTally[v])
                                {
                                    if (!blnSecureOn)
                                        return true;

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + " : " + strSectionName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " Inspect Lead is UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateLeadSecureOption(i, v);
                                        return true;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else if (blnLeadAllUnChecked[v])
                                {
                                    if (!blnSecureOn)
                                        return true;

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + " : " + strSectionName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " All Lead inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateLeadSecureOption(i, v);
                                        return true;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else if (blnLeadParentUnChecked)
                                {
                                    if (!blnSecureOn)
                                        return true;

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + " : " + strSectionName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " Inspect Lead is UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateLeadSecureOption(i, v);
                                        return true;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else if (blnLeadNotTally[v])
                                {
                                    if (!blnSecureOn)
                                        return true;

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + " : " + strSectionName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " Lead inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateLeadSecureOption(i, v);
                                        return true;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                            }
                        }
                        break;
                    case "MarkPkg":
                    case "MOPkg":
                    case "MOLiPkg":
                    case "InPocketPkg":
                    case "InPocketPkgPos":
                    case "IPMLiPkg":
                        List<bool> blnMarkPkgAllUnChecked = new List<bool>();
                        List<bool> blnMarkPkgNotTally = new List<bool>();
                        List<bool> blnMarkPkgParentUnChecked = new List<bool>();
                        List<bool> blnMarkPkgPin1NotTally = new List<bool>();
                        for (int v = 0; v < m_smVSInfo[i].g_intTotalTemplates; v++)
                        {
                            blnMarkPkgAllUnChecked.Add(false);
                            blnMarkPkgNotTally.Add(false);
                            blnMarkPkgParentUnChecked.Add(false);
                            blnMarkPkgPin1NotTally.Add(false);
                        }
                        CheckMarkOptionSecureSetting(m_smVSInfo[i], i, ref blnMarkPkgAllUnChecked, ref blnMarkPkgNotTally, ref blnMarkPkgParentUnChecked, ref blnMarkPkgPin1NotTally);
                        for (int v = 0; v < m_smVSInfo[i].g_intTotalTemplates; v++)
                        {
                            if (blnMarkPkgAllUnChecked[v] && blnMarkPkgNotTally[v])
                            {
                                if (blnMarkPkgPin1NotTally[v])
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Mark and Pin 1 inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkPin1SecureOption(i, v);
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Mark inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                            }
                            else if (blnMarkPkgParentUnChecked[v] && blnMarkPkgNotTally[v])
                            {
                                if (blnMarkPkgPin1NotTally[v])
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Mark and Pin 1 inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkPin1SecureOption(i, v);
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Mark in inspection option is UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                            }
                            else if (blnMarkPkgAllUnChecked[v])
                            {
                                if (blnMarkPkgPin1NotTally[v])
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Mark inspection option are UnChecked and Pin 1 inspection option is not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkPin1SecureOption(i, v);
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Mark inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                            }
                            else if (blnMarkPkgParentUnChecked[v])
                            {
                                if (blnMarkPkgPin1NotTally[v])
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Mark inspection option are UnChecked and Pin 1 inspection option is not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkPin1SecureOption(i, v);
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Mark in inspection option is UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                            }
                            else if (blnMarkPkgNotTally[v])
                            {
                                if (blnMarkPkgPin1NotTally[v])
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Mark and Pin 1 inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkPin1SecureOption(i, v);
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (!blnSecureOn)
                                    {
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Mark inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateMarkSecureOption(i, v);
                                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                            goto CheckLead;
                                        else
                                            goto CheckPackage;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                            }
                            else if (blnMarkPkgPin1NotTally[v])
                            {
                                if (!blnSecureOn)
                                {
                                    if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                        goto CheckLead;
                                    else
                                        goto CheckPackage;
                                }

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " - Template ") + (v + 1).ToString() + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Pin 1 inspection option is not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateMarkPin1SecureOption(i, v);
                                    if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                                        goto CheckLead;
                                    else
                                        goto CheckPackage;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Mark Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }

                        }

                    CheckLead:
                        if ((m_smCustomizeInfo.g_intWantLead & intPos) > 0)
                        {
                            List<bool> blnLeadAllUnChecked = new List<bool>();
                            List<bool> blnLeadNotTally = new List<bool>();
                            bool blnLeadParentUnChecked = false;
                            for (int v = 0; v < m_smVSInfo[i].g_arrLead.Length; v++)
                            {
                                blnLeadAllUnChecked.Add(false);
                                blnLeadNotTally.Add(false);
                            }
                            CheckLeadOptionSecureSetting(m_smVSInfo[i], i, ref blnLeadAllUnChecked, ref blnLeadNotTally, ref blnLeadParentUnChecked);
                            for (int v = 0; v < m_smVSInfo[i].g_arrLead.Length; v++)
                            {
                                if (!m_smVSInfo[i].g_arrLead[v].ref_blnSelected)
                                    continue;

                                string strSectionName = "";
                                if (v == 0)
                                    strSectionName = "SearchROI";
                                else if (v == 1)
                                    strSectionName = "TopROI";
                                else if (v == 2)
                                    strSectionName = "RightROI";
                                else if (v == 3)
                                    strSectionName = "BottomROI";
                                else if (v == 4)
                                    strSectionName = "LeftROI";

                                if (blnLeadAllUnChecked[v] && blnLeadNotTally[v])
                                {
                                    if (!blnSecureOn)
                                        goto CheckPackage;

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + " : " + strSectionName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " All Lead inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateLeadSecureOption(i, v);
                                        goto CheckPackage;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else if (blnLeadParentUnChecked && blnLeadNotTally[v])
                                {
                                    if (!blnSecureOn)
                                        goto CheckPackage;

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + " : " + strSectionName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " Inspect Lead is UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateLeadSecureOption(i, v);
                                        goto CheckPackage;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else if (blnLeadAllUnChecked[v])
                                {
                                    if (!blnSecureOn)
                                        goto CheckPackage;

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + " : " + strSectionName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " All Lead inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateLeadSecureOption(i, v);
                                        goto CheckPackage;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else if (blnLeadParentUnChecked)
                                {
                                    if (!blnSecureOn)
                                        goto CheckPackage;

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + " : " + strSectionName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " Inspect Lead is UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateLeadSecureOption(i, v);
                                        goto CheckPackage;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                                else if (blnLeadNotTally[v])
                                {
                                    if (!blnSecureOn)
                                        goto CheckPackage;

                                    if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + " : " + strSectionName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " Lead inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        UpdateLeadSecureOption(i, v);
                                        goto CheckPackage;
                                    }
                                    else
                                    {
                                        
                                        STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                        
                                        return false;
                                    }
                                }
                            }
                        }

                    CheckPackage:
                        bool blnPackageAllUnChecked = false, blnPackageNotTally = false, blnPackageParentUnChecked = false;
                        CheckPackageOptionSecureSetting(m_smVSInfo[i], i, ref blnPackageAllUnChecked, ref blnPackageNotTally, ref blnPackageParentUnChecked);
                        if (blnPackageAllUnChecked && blnPackageNotTally)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckMOPackageColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Package inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdatePackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckMOPackageColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnPackageParentUnChecked && blnPackageNotTally)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckMOPackageColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Package Size and Check Package Defect in inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdatePackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckMOPackageColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnPackageAllUnChecked)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckMOPackageColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Package inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdatePackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckMOPackageColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnPackageParentUnChecked)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckMOPackageColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Package Size and Check Package Defect in inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdatePackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckMOPackageColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnPackageNotTally)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckMOPackageColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Package inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdatePackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckMOPackageColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        CheckMOPackageColor:
                        if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                        {
                            bool blnPackageColorAllUnChecked = false, blnPackageColorNotTally = false;
                            CheckColorPackageOptionSecureSetting(m_smVSInfo[i], i, ref blnPackageColorAllUnChecked, ref blnPackageColorNotTally);
                            if (blnPackageColorAllUnChecked && blnPackageColorNotTally)
                            {
                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Package inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {

                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);

                                    UpdateColorPackageSecureOption(i);
                                    return true;
                                }
                                else
                                {

                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);

                                    return false;
                                }

                            }
                            else if (blnPackageColorAllUnChecked)
                            {

                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Package inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {

                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);

                                    UpdateColorPackageSecureOption(i);
                                    return true;
                                }
                                else
                                {

                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);

                                    return false;
                                }

                            }
                            else if (blnPackageColorNotTally)
                            {

                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Color Package inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {

                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);

                                    UpdateColorPackageSecureOption(i);
                                    return true;
                                }
                                else
                                {

                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);

                                    return false;
                                }

                            }
                        }
                        break;
                    case "Package":
                        bool blnPackageOnlyAllUnChecked = false, blnPackageOnlyNotTally = false, blnPackageOnlyParentUnChecked = false;
                        CheckPackageOptionSecureSetting(m_smVSInfo[i], i, ref blnPackageOnlyAllUnChecked, ref blnPackageOnlyNotTally, ref blnPackageOnlyParentUnChecked);
                        if (blnPackageOnlyAllUnChecked && blnPackageOnlyNotTally)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPackageColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Package inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdatePackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPackageColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnPackageOnlyParentUnChecked && blnPackageOnlyNotTally)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPackageColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Package Size and Check Package Defect in inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdatePackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPackageColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnPackageOnlyAllUnChecked)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPackageColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Package inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdatePackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPackageColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnPackageOnlyParentUnChecked)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPackageColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Package Size and Check Package Defect in inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdatePackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPackageColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnPackageOnlyNotTally)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPackageColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Package inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdatePackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPackageColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        CheckPackageColor:
                        if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                        {
                            bool blnPackageColorAllUnChecked = false, blnPackageColorNotTally = false;
                            CheckColorPackageOptionSecureSetting(m_smVSInfo[i], i, ref blnPackageColorAllUnChecked, ref blnPackageColorNotTally);
                            if (blnPackageColorAllUnChecked && blnPackageColorNotTally)
                            {
                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Package inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {

                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);

                                    UpdateColorPackageSecureOption(i);
                                    return true;
                                }
                                else
                                {

                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);

                                    return false;
                                }

                            }
                            else if (blnPackageColorAllUnChecked)
                            {

                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Package inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {

                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);

                                    UpdateColorPackageSecureOption(i);
                                    return true;
                                }
                                else
                                {

                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);

                                    return false;
                                }

                            }
                            else if (blnPackageColorNotTally)
                            {

                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Color Package inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {

                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);

                                    UpdateColorPackageSecureOption(i);
                                    return true;
                                }
                                else
                                {

                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);

                                    return false;
                                }

                            }
                        }
                        break;
                    case "BottomOrientPad":
                    case "BottomOPadPkg":
                        bool blnOrientPadAllUnChecked = false, blnOrientPadNotTally = false;
                        CheckOrientPadOptionSecureSetting(m_smVSInfo[i], i, ref blnOrientPadAllUnChecked, ref blnOrientPadNotTally);
                        if (blnOrientPadAllUnChecked && blnOrientPadNotTally)
                        {
                            if (!blnSecureOn)
                                goto CheckOPad;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All OrientPad inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">OrientPad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateOrientPadSecureOption(i);
                                goto CheckOPad;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">OrientPad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnOrientPadAllUnChecked)
                        {
                            if (!blnSecureOn)
                                goto CheckOPad;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All OrientPad inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">OrientPad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateOrientPadSecureOption(i);
                                goto CheckOPad;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">OrientPad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnOrientPadNotTally)
                        {
                            if (!blnSecureOn)
                                goto CheckOPad;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : OrientPad inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">OrientPad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateOrientPadSecureOption(i);
                                goto CheckOPad;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">OrientPad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }

                    CheckOPad:
                        bool blnOPadAllUnChecked = false, blnOPadNotTally = false, blnOPadPin1NotTally = false;
                        CheckPadOptionSecureSetting(m_smVSInfo[i], i, ref blnOPadAllUnChecked, ref blnOPadNotTally, ref blnOPadPin1NotTally);
                        if (blnOPadAllUnChecked && blnOPadNotTally)
                        {
                            if (blnOPadPin1NotTally)
                            {
                                if (!blnSecureOn)
                                {
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckOPadColor;
                                    else
                                        return true;
                                }

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Pad and Pin 1 inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdatePadPin1SecureOption(i);
                                    UpdateCenterPadSecureOption(i);

                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckOPadColor;
                                    else
                                        return true;

                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                            else
                            {
                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Pad inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterPadSecureOption(i);

                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckOPadColor;
                                    else
                                        return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                        }
                        else if (blnOPadAllUnChecked)
                        {
                            if (blnOPadPin1NotTally)
                            {
                                if (!blnSecureOn)
                                {
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckOPadColor;
                                    else
                                        return true;
                                }

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Pad and Pin 1 inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdatePadPin1SecureOption(i);
                                    UpdateCenterPadSecureOption(i);

                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckOPadColor;
                                    else
                                        return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                            else
                            {
                                if (!blnSecureOn)
                                {
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckOPadColor;
                                    else
                                        return true;
                                }

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Pad inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterPadSecureOption(i);

                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckOPadColor;
                                    else
                                        return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                        }
                        else if (blnOPadNotTally)
                        {
                            if (blnOPadPin1NotTally)
                            {
                                if (!blnSecureOn)
                                {
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckOPadColor;
                                    else
                                        return true;
                                }

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Pad and Pin 1 inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdatePadPin1SecureOption(i);
                                    UpdateCenterPadSecureOption(i);

                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckOPadColor;
                                    else
                                        return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                            else
                            {
                                if (!blnSecureOn)
                                {
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckOPadColor;
                                    else
                                        return true;
                                }

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Pad inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterPadSecureOption(i);

                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckOPadColor;
                                    else
                                        return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                        }
                        else if (blnOPadPin1NotTally)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckOPadColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Pin 1 inspection option is not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdatePadPin1SecureOption(i);

                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckOPadColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                    CheckOPadColor:
                        if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                        {
                            bool blnOPadColorAllUnChecked = false, blnOPadColorNotTally = false;
                            CheckCenterColorPadOptionSecureSetting(m_smVSInfo[i], i, ref blnOPadColorAllUnChecked, ref blnOPadColorNotTally);
                            if (blnOPadColorAllUnChecked && blnOPadColorNotTally)
                            {
                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Pad inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterColorPadSecureOption(i);
                                    return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                            else if (blnOPadColorAllUnChecked)
                            {

                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Pad inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterColorPadSecureOption(i);
                                    return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                            else if (blnOPadColorNotTally)
                            {

                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Color Pad inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterColorPadSecureOption(i);
                                    return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                        }
                        break;
                    case "Pad":
                    case "PadPos":
                        bool blnPadAllUnChecked = false, blnPadNotTally = false, blnPadPin1NotTally = false;
                        CheckPadOptionSecureSetting(m_smVSInfo[i], i, ref blnPadAllUnChecked, ref blnPadNotTally, ref blnPadPin1NotTally);
                        if (blnPadAllUnChecked && blnPadNotTally)
                        {
                            if (blnPadPin1NotTally)
                            {
                                if (!blnSecureOn)
                                {
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckPadColor;
                                    else
                                        return true;
                                }

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Pad and Pin 1 inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdatePadPin1SecureOption(i);
                                    UpdateCenterPadSecureOption(i);
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckPadColor;
                                    else
                                        return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                            else
                            {
                                if (!blnSecureOn)
                                {
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckPadColor;
                                    else
                                        return true;
                                }

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Pad inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterPadSecureOption(i);
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckPadColor;
                                    else
                                        return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                        }
                        else if (blnPadAllUnChecked)
                        {
                            if (blnPadPin1NotTally)
                            {
                                if (!blnSecureOn)
                                {
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckPadColor;
                                    else
                                        return true;
                                }

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Pad and Pin 1 inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdatePadPin1SecureOption(i);
                                    UpdateCenterPadSecureOption(i);
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckPadColor;
                                    else
                                        return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                            else
                            {
                                if (!blnSecureOn)
                                {
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckPadColor;
                                    else
                                        return true;
                                }

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Pad inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterPadSecureOption(i);
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckPadColor;
                                    else
                                        return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                        }
                        else if (blnPadNotTally)
                        {
                            if (blnPadPin1NotTally)
                            {
                                if (!blnSecureOn)
                                {
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckPadColor;
                                    else
                                        return true;
                                }

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Pad and Pin 1 inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdatePadPin1SecureOption(i);
                                    UpdateCenterPadSecureOption(i);
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckPadColor;
                                    else
                                        return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                            else
                            {
                                if (!blnSecureOn)
                                {
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckPadColor;
                                    else
                                        return true;
                                }

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Pad inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterPadSecureOption(i);
                                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                        goto CheckPadColor;
                                    else
                                        return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                        }
                        else if (blnPadPin1NotTally)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPadColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Pin 1 inspection option is not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdatePadPin1SecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPadColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                    CheckPadColor:
                        if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                        {
                            bool blnPadColorAllUnChecked = false, blnPadColorNotTally = false;
                            CheckCenterColorPadOptionSecureSetting(m_smVSInfo[i], i, ref blnPadColorAllUnChecked, ref blnPadColorNotTally);
                            if (blnPadColorAllUnChecked && blnPadColorNotTally)
                            {
                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Pad inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterColorPadSecureOption(i);
                                    return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                            else if (blnPadColorAllUnChecked)
                            {

                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Pad inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterColorPadSecureOption(i);
                                    return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                            else if (blnPadColorNotTally)
                            {

                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Color Pad inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterColorPadSecureOption(i);
                                    return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                        }
                        break;
                    case "PadPkg":
                    case "PadPkgPos":
                        bool blnPadPkgAllUnChecked = false, blnPadPkgNotTally = false, blnPadPkgPin1NotTally = false;
                        CheckPadOptionSecureSetting(m_smVSInfo[i], i, ref blnPadPkgAllUnChecked, ref blnPadPkgNotTally, ref blnPadPkgPin1NotTally);
                        if (blnPadPkgAllUnChecked && blnPadPkgNotTally)
                        {
                            if (blnPadPkgPin1NotTally)
                            {
                                if (!blnSecureOn)
                                    goto CheckPadPackageOnly;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Pad and Pin 1 inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdatePadPin1SecureOption(i);
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckPadPackageOnly;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                            else
                            {
                                if (!blnSecureOn)
                                    goto CheckPadPackageOnly;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Pad inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckPadPackageOnly;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                        }
                        else if (blnPadPkgAllUnChecked)
                        {
                            if (blnPadPkgPin1NotTally)
                            {
                                if (!blnSecureOn)
                                    goto CheckPadPackageOnly;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Pad and Pin 1 inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdatePadPin1SecureOption(i);
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckPadPackageOnly;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                            else
                            {
                                if (!blnSecureOn)
                                    goto CheckPadPackageOnly;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Pad inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckPadPackageOnly;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                        }
                        else if (blnPadPkgNotTally)
                        {
                            if (blnPadPkgPin1NotTally)
                            {
                                if (!blnSecureOn)
                                    goto CheckPadPackageOnly;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Pad and Pin 1 inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdatePadPin1SecureOption(i);
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckPadPackageOnly;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                            else
                            {
                                if (!blnSecureOn)
                                    goto CheckPadPackageOnly;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Pad inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckPadPackageOnly;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                        }
                        else if (blnPadPkgPin1NotTally)
                        {
                            if (!blnSecureOn)
                                goto CheckPadPackageOnly;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Pin 1 inspection option is not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdatePadPin1SecureOption(i);
                                goto CheckPadPackageOnly;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }

                    CheckPadPackageOnly:
                        bool blnPadPackageAllUnChecked = false, blnPadPackageNotTally = false, blnPadPackageParentUnChecked = false;
                        CheckCenterPadPackageOptionSecureSetting(m_smVSInfo[i], i, ref blnPadPackageAllUnChecked, ref blnPadPackageNotTally, ref blnPadPackageParentUnChecked);
                        if (blnPadPackageAllUnChecked && blnPadPackageNotTally)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPadPkgColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All PadPackage inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateCenterPadPackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPadPkgColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnPadPackageParentUnChecked && blnPadPackageNotTally)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPadPkgColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check PadPackage Size and Check Package Defect in inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateCenterPadPackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPadPkgColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnPadPackageAllUnChecked)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPadPkgColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All PadPackage inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateCenterPadPackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPadPkgColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnPadPackageParentUnChecked)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPadPkgColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check PadPackage Size and Check Package Defect in inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateCenterPadPackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPadPkgColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnPadPackageNotTally)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPadPkgColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : PadPackage inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateCenterPadPackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPadPkgColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                    CheckPadPkgColor:
                        if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                        {
                            bool blnPadPkgColorAllUnChecked = false, blnPadPkgColorNotTally = false;
                            CheckCenterColorPadOptionSecureSetting(m_smVSInfo[i], i, ref blnPadPkgColorAllUnChecked, ref blnPadPkgColorNotTally);
                            if (blnPadPkgColorAllUnChecked && blnPadPkgColorNotTally)
                            {
                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Pad inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterColorPadSecureOption(i);
                                    return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                            else if (blnPadPkgColorAllUnChecked)
                            {

                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Pad inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterColorPadSecureOption(i);
                                    return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                            else if (blnPadPkgColorNotTally)
                            {

                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Color Pad inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterColorPadSecureOption(i);
                                    return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                        }
                        break;
                    case "Pad5S":
                    case "Pad5SPos":
                        bool blnCenterPadAllUnChecked = false, blnCenterPadNotTally = false, blnCenterPadPin1NotTally = false;
                        CheckPadOptionSecureSetting(m_smVSInfo[i], i, ref blnCenterPadAllUnChecked, ref blnCenterPadNotTally, ref blnCenterPadPin1NotTally);
                        if (blnCenterPadAllUnChecked && blnCenterPadNotTally)
                        {
                            if (blnCenterPadPin1NotTally)
                            {
                                if (!blnSecureOn)
                                    goto CheckSidePadOnly;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Center Pad and Pin 1 inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdatePadPin1SecureOption(i);
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckSidePadOnly;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                            else
                            {
                                if (!blnSecureOn)
                                    goto CheckSidePadOnly;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Center Pad inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckSidePadOnly;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                        }
                        else if (blnCenterPadAllUnChecked)
                        {
                            if (blnCenterPadPin1NotTally)
                            {
                                if (!blnSecureOn)
                                    goto CheckSidePadOnly;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Center Pad and Pin 1 inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdatePadPin1SecureOption(i);
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckSidePadOnly;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                            else
                            {
                                if (!blnSecureOn)
                                    goto CheckSidePadOnly;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Center Pad inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckSidePadOnly;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                        }
                        else if (blnCenterPadNotTally)
                        {
                            if (blnCenterPadPin1NotTally)
                            {
                                if (!blnSecureOn)
                                    goto CheckSidePadOnly;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Center Pad and Pin 1 inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdatePadPin1SecureOption(i);
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckSidePadOnly;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                            else
                            {
                                if (!blnSecureOn)
                                    goto CheckSidePadOnly;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Center Pad inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckSidePadOnly;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                        }
                        else if (blnCenterPadPin1NotTally)
                        {
                            if (!blnSecureOn)
                                goto CheckSidePadOnly;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Pin 1 inspection option is not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdatePadPin1SecureOption(i);
                                goto CheckSidePadOnly;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }

                    CheckSidePadOnly:
                        bool blnSidePadAllUnChecked = false, blnSidePadNotTally = false;
                        CheckSidePadOptionSecureSetting(m_smVSInfo[i], i, ref blnSidePadAllUnChecked, ref blnSidePadNotTally);
                        if (blnSidePadAllUnChecked && blnSidePadNotTally)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPad5SColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Side Pad inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateSidePadSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPad5SColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnSidePadAllUnChecked)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPad5SColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Side Pad inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateSidePadSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPad5SColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnSidePadNotTally)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPad5SColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Side Pad inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateSidePadSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPad5SColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                    CheckPad5SColor:
                        if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                        {
                            bool blnPad5SColorAllUnChecked = false, blnPad5SColorNotTally = false;
                            CheckCenterColorPadOptionSecureSetting(m_smVSInfo[i], i, ref blnPad5SColorAllUnChecked, ref blnPad5SColorNotTally);
                            if (blnPad5SColorAllUnChecked && blnPad5SColorNotTally)
                            {
                                if (!blnSecureOn)
                                    goto CheckPad5SColorSide;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Pad inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterColorPadSecureOption(i);
                                    goto CheckPad5SColorSide;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                            else if (blnPad5SColorAllUnChecked)
                            {

                                if (!blnSecureOn)
                                    goto CheckPad5SColorSide;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Pad inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterColorPadSecureOption(i);
                                    goto CheckPad5SColorSide;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                            else if (blnPad5SColorNotTally)
                            {

                                if (!blnSecureOn)
                                    goto CheckPad5SColorSide;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Color Pad inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterColorPadSecureOption(i);
                                    goto CheckPad5SColorSide;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                        CheckPad5SColorSide:
                            bool blnSidePad5SColorAllUnChecked = false, blnSidePad5SColorNotTally = false;
                            CheckSideColorPadOptionSecureSetting(m_smVSInfo[i], i, ref blnSidePad5SColorAllUnChecked, ref blnSidePad5SColorNotTally);
                            if (blnSidePad5SColorAllUnChecked && blnSidePad5SColorNotTally)
                            {
                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Pad inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateSideColorPadSecureOption(i);
                                    return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                            else if (blnSidePad5SColorAllUnChecked)
                            {

                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Pad inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateSideColorPadSecureOption(i);
                                    return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                            else if (blnSidePad5SColorNotTally)
                            {

                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Color Pad inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateSideColorPadSecureOption(i);
                                    return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                        }
                        break;
                    case "Pad5SPkg":
                    case "Pad5SPkgPos":
                        bool blnCenterPad5SAllUnChecked = false, blnCenterPad5SNotTally = false, blnCenterPad5SPin1NotTally = false;
                        CheckPadOptionSecureSetting(m_smVSInfo[i], i, ref blnCenterPad5SAllUnChecked, ref blnCenterPad5SNotTally, ref blnCenterPad5SPin1NotTally);
                        if (blnCenterPad5SAllUnChecked && blnCenterPad5SNotTally)
                        {
                            if (blnCenterPad5SPin1NotTally)
                            {
                                if (!blnSecureOn)
                                    goto CheckSidePad;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Center Pad and Pin 1 inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdatePadPin1SecureOption(i);
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckSidePad;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                            else
                            {
                                if (!blnSecureOn)
                                    goto CheckSidePad;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Center Pad inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckSidePad;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                        }
                        else if (blnCenterPad5SAllUnChecked)
                        {
                            if (blnCenterPad5SPin1NotTally)
                            {
                                if (!blnSecureOn)
                                    goto CheckSidePad;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Center Pad and Pin 1 inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdatePadPin1SecureOption(i);
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckSidePad;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                            else
                            {
                                if (!blnSecureOn)
                                    goto CheckSidePad;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Center Pad inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckSidePad;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                        }
                        else if (blnCenterPad5SNotTally)
                        {
                            if (blnCenterPad5SPin1NotTally)
                            {
                                if (!blnSecureOn)
                                    goto CheckSidePad;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Center Pad and Pin 1 inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdatePadPin1SecureOption(i);
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckSidePad;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                            else
                            {
                                if (!blnSecureOn)
                                    goto CheckSidePad;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Center Pad inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterPadSecureOption(i);
                                    goto CheckSidePad;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }
                            }
                        }
                        else if (blnCenterPad5SPin1NotTally)
                        {
                            if (!blnSecureOn)
                                goto CheckSidePad;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Pin 1 inspection option is not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdatePadPin1SecureOption(i);
                                goto CheckSidePad;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }

                    CheckSidePad:
                        bool blnSidePad5SAllUnChecked = false, blnSidePad5SNotTally = false;
                        CheckSidePadOptionSecureSetting(m_smVSInfo[i], i, ref blnSidePad5SAllUnChecked, ref blnSidePad5SNotTally);
                        if (blnSidePad5SAllUnChecked && blnSidePad5SNotTally)
                        {
                            if (!blnSecureOn)
                                goto CheckCenterPackage;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Side Pad inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateSidePadSecureOption(i);
                                goto CheckCenterPackage;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnSidePad5SAllUnChecked)
                        {
                            if (!blnSecureOn)
                                goto CheckCenterPackage;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Side Pad inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateSidePadSecureOption(i);
                                goto CheckCenterPackage;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnSidePad5SNotTally)
                        {
                            if (!blnSecureOn)
                                goto CheckCenterPackage;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Side Pad inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateSidePadSecureOption(i);
                                goto CheckCenterPackage;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }

                    CheckCenterPackage:
                        bool blnCenterPadPackageAllUnChecked = false, blnCenterPadPackageNotTally = false, blnCenterPadPackageParentUnChecked = false;
                        CheckCenterPadPackageOptionSecureSetting(m_smVSInfo[i], i, ref blnCenterPadPackageAllUnChecked, ref blnCenterPadPackageNotTally, ref blnCenterPadPackageParentUnChecked);
                        if (blnCenterPadPackageAllUnChecked && blnCenterPadPackageNotTally)
                        {
                            if (!blnSecureOn)
                                goto CheckSidePackage;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Center PadPackage inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateCenterPadPackageSecureOption(i);
                                goto CheckSidePackage;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnCenterPadPackageParentUnChecked && blnCenterPadPackageNotTally)
                        {
                            if (!blnSecureOn)
                                goto CheckSidePackage;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Center PadPackage Size and Check Package Defect in inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateCenterPadPackageSecureOption(i);
                                goto CheckSidePackage;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnCenterPadPackageAllUnChecked)
                        {
                            if (!blnSecureOn)
                                goto CheckSidePackage;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Center PadPackage inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateCenterPadPackageSecureOption(i);
                                goto CheckSidePackage;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnCenterPadPackageParentUnChecked)
                        {
                            if (!blnSecureOn)
                                goto CheckSidePackage;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Center PadPackage Size and Check Package Defect in inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateCenterPadPackageSecureOption(i);
                                goto CheckSidePackage;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnCenterPadPackageNotTally)
                        {
                            if (!blnSecureOn)
                                goto CheckSidePackage;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Center PadPackage inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateCenterPadPackageSecureOption(i);
                                goto CheckSidePackage;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Pad Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }

                    CheckSidePackage:
                        bool blnSidePadPackageAllUnChecked = false, blnSidePadPackageNotTally = false, blnSidePadPackageParentUnChecked = false;
                        CheckSidePadPackageOptionSecureSetting(m_smVSInfo[i], i, ref blnSidePadPackageAllUnChecked, ref blnSidePadPackageNotTally, ref blnSidePadPackageParentUnChecked);
                        if (blnSidePadPackageAllUnChecked && blnSidePadPackageNotTally)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPad5SPkgColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Side PadPackage inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateSidePadPackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPad5SPkgColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnSidePadPackageParentUnChecked && blnSidePadPackageNotTally)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPad5SPkgColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Side PadPackage Size and Check Package Defect in inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateSidePadPackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPad5SPkgColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnSidePadPackageAllUnChecked)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPad5SPkgColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Side PadPackage inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateSidePadPackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPad5SPkgColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnSidePadPackageParentUnChecked)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPad5SPkgColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Side PadPackage Size and Check Package Defect in inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateSidePadPackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPad5SPkgColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnSidePadPackageNotTally)
                        {
                            if (!blnSecureOn)
                            {
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPad5SPkgColor;
                                else
                                    return true;
                            }

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Side PadPackage inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateSidePadPackageSecureOption(i);
                                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                                    goto CheckPad5SPkgColor;
                                else
                                    return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Pad Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                    CheckPad5SPkgColor:
                        if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                        {
                            bool blnPad5SPkgColorAllUnChecked = false, blnPad5SPkgColorNotTally = false;
                            CheckCenterColorPadOptionSecureSetting(m_smVSInfo[i], i, ref blnPad5SPkgColorAllUnChecked, ref blnPad5SPkgColorNotTally);
                            if (blnPad5SPkgColorAllUnChecked && blnPad5SPkgColorNotTally)
                            {
                                if (!blnSecureOn)
                                    goto CheckPad5SPkgColorSide;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Pad inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterColorPadSecureOption(i);
                                    goto CheckPad5SPkgColorSide;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                            else if (blnPad5SPkgColorAllUnChecked)
                            {

                                if (!blnSecureOn)
                                    goto CheckPad5SPkgColorSide;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Pad inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterColorPadSecureOption(i);
                                    goto CheckPad5SPkgColorSide;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                            else if (blnPad5SPkgColorNotTally)
                            {

                                if (!blnSecureOn)
                                    goto CheckPad5SPkgColorSide;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Color Pad inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateCenterColorPadSecureOption(i);
                                    goto CheckPad5SPkgColorSide;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Center Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                        CheckPad5SPkgColorSide:
                            bool blnSidePad5SPkgColorAllUnChecked = false, blnSidePad5SPkgColorNotTally = false;
                            CheckSideColorPadOptionSecureSetting(m_smVSInfo[i], i, ref blnSidePad5SPkgColorAllUnChecked, ref blnSidePad5SPkgColorNotTally);
                            if (blnSidePad5SPkgColorAllUnChecked && blnSidePad5SPkgColorNotTally)
                            {
                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Pad inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateSideColorPadSecureOption(i);
                                    return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                            else if (blnSidePad5SPkgColorAllUnChecked)
                            {

                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Color Pad inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateSideColorPadSecureOption(i);
                                    return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                            else if (blnSidePad5SPkgColorNotTally)
                            {

                                if (!blnSecureOn)
                                    return true;

                                if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Color Pad inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Color Pad Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    UpdateSideColorPadSecureOption(i);
                                    return true;
                                }
                                else
                                {
                                    
                                    STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Side Color Pad Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                    
                                    return false;
                                }

                            }
                        }
                        break;
                    case "Li3D":
                        bool blnLead3DAllUnChecked = false, blnLead3DNotTally = false;
                        CheckLead3DOptionSecureSetting(m_smVSInfo[i], i, ref blnLead3DAllUnChecked, ref blnLead3DNotTally);
                        if (blnLead3DAllUnChecked && blnLead3DNotTally)
                        {
                            if (!blnSecureOn)
                                return true;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Lead3D inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateLead3DSecureOption(i);
                                return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnLead3DAllUnChecked)
                        {
                            if (!blnSecureOn)
                                return true;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Lead3D inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateLead3DSecureOption(i);
                                return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnLead3DNotTally)
                        {
                            if (!blnSecureOn)
                                return true;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Lead3D inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateLead3DSecureOption(i);
                                return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        break;
                    case "Li3DPkg":
                        bool blnLead3DPkgAllUnChecked = false, blnLead3DPkgNotTally = false;
                        CheckLead3DOptionSecureSetting(m_smVSInfo[i], i, ref blnLead3DPkgAllUnChecked, ref blnLead3DPkgNotTally);
                        if (blnLead3DPkgAllUnChecked && blnLead3DPkgNotTally)
                        {
                            if (!blnSecureOn)
                                goto CheckLead3DPackage;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Lead3D inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateLead3DSecureOption(i);
                                goto CheckLead3DPackage;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnLead3DPkgAllUnChecked)
                        {
                            if (!blnSecureOn)
                                goto CheckLead3DPackage;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Lead3D inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateLead3DSecureOption(i);
                                goto CheckLead3DPackage;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnLead3DPkgNotTally)
                        {
                            if (!blnSecureOn)
                                goto CheckLead3DPackage;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Lead3D inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateLead3DSecureOption(i);
                                goto CheckLead3DPackage;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }

                    CheckLead3DPackage:
                        bool blnLead3DPackageAllUnChecked = false, blnLead3DPackageNotTally = false, blnLead3DPackageParentUnChecked = false;
                        CheckLead3DPkgOptionSecureSetting(m_smVSInfo[i], i, ref blnLead3DPackageAllUnChecked, ref blnLead3DPackageNotTally, ref blnLead3DPackageParentUnChecked);
                        if (blnLead3DPackageAllUnChecked && blnLead3DPackageNotTally)
                        {
                            if (!blnSecureOn)
                                return true;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Lead3D Package inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateLead3DPackageSecureOption(i);
                                return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnLead3DPackageParentUnChecked && blnLead3DPackageNotTally)
                        {
                            if (!blnSecureOn)
                                return true;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Lead3D Package Size and Check Package Defect in inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateLead3DPackageSecureOption(i);
                                return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnLead3DPackageAllUnChecked)
                        {
                            if (!blnSecureOn)
                                return true;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Lead3D Package inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateLead3DPackageSecureOption(i);
                                return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnLead3DPackageParentUnChecked)
                        {
                            if (!blnSecureOn)
                                return true;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Check Lead3D Package Size and Check Package Defect in inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateLead3DPackageSecureOption(i);
                                return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnLead3DPackageNotTally)
                        {
                            if (!blnSecureOn)
                                return true;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Lead3D Package inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Package Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateLead3DPackageSecureOption(i);
                                return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Lead3D Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        break;
                    case "Seal":
                        bool blnSealAllUnChecked = false, blnSealNotTally = false;
                        CheckSealOptionSecureSetting(m_smVSInfo[i], i, ref blnSealAllUnChecked, ref blnSealNotTally);
                        if (blnSealAllUnChecked && blnSealNotTally)
                        {
                            if (!blnSecureOn)
                                return true;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Seal inspection option are UnChecked and not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Seal Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateSealSecureOption(i);
                                return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Seal Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnSealAllUnChecked)
                        {
                            if (!blnSecureOn)
                                return true;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : All Seal inspection option are UnChecked. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Seal Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateSealSecureOption(i);
                                return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Seal Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        else if (blnSealNotTally)
                        {
                            if (!blnSecureOn)
                                return true;

                            if (SRMMessageBox.Show(m_smVSInfo[i].g_strVisionDisplayName + LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, " : Seal inspection option are not tally with previous inspection option. Continue Production?"), "Production", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Seal Fail Option Changes", "User Continue Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                UpdateSealSecureOption(i);
                                return true;
                            }
                            else
                            {
                                
                                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[m_intSelectedVisionStation - 1].g_strVisionFolderName + ">Seal Package Fail Option Changes", "User Cancel Production", "", "", m_smProductionInfo.g_strLotID);
                                
                                return false;
                            }
                        }
                        break;
                    case "UnitPresent":
                        break;
                }
            }
            return true;
        }

        private void UpdateOrientSecureOption(int i)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);
            int intOrientFailOptionMask = 0;
            for (int u = 0; u < m_smVSInfo[i].g_intUnitsOnImage; u++)
            {
                if (m_smVSInfo[i].g_arrOrients[u][0].ref_blnWantCheckOrientAngleTolerance)
                    intOrientFailOptionMask |= 0x01;
                else
                    intOrientFailOptionMask &= ~0x01;

                if (m_smVSInfo[i].g_arrOrients[u][0].ref_blnWantCheckOrientXTolerance)
                    intOrientFailOptionMask |= 0x02;
                else
                    intOrientFailOptionMask &= ~0x02;

                if (m_smVSInfo[i].g_arrOrients[u][0].ref_blnWantCheckOrientYTolerance)
                    intOrientFailOptionMask |= 0x04;
                else
                    intOrientFailOptionMask &= ~0x04;

                subKey1.SetValue("Secure-OrientFailOptionMask", intOrientFailOptionMask);
            }

        }
        private void UpdateOrientPadSecureOption(int i)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);
            int intOrientPadFailOptionMask = 0;
            for (int u = 0; u < m_smVSInfo[i].g_intUnitsOnImage; u++)
            {
                if (m_smVSInfo[i].g_objPadOrient.ref_blnWantCheckOrientAngleTolerance)
                    intOrientPadFailOptionMask |= 0x01;
                else
                    intOrientPadFailOptionMask &= ~0x01;

                if (m_smVSInfo[i].g_objPadOrient.ref_blnWantCheckOrientXTolerance)
                    intOrientPadFailOptionMask |= 0x02;
                else
                    intOrientPadFailOptionMask &= ~0x02;

                if (m_smVSInfo[i].g_objPadOrient.ref_blnWantCheckOrientYTolerance)
                    intOrientPadFailOptionMask |= 0x04;
                else
                    intOrientPadFailOptionMask &= ~0x04;

                subKey1.SetValue("Secure-OrientPadFailOptionMask", intOrientPadFailOptionMask);
            }

        }
        private void UpdateMarkSecureOption(int i, int v)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);
            for (int u = 0; u < m_smVSInfo[i].g_intUnitsOnImage; u++)
            {

                subKey1.SetValue("Secure-MarkFailOptionMask" + v.ToString(), m_smVSInfo[i].g_arrMarks[u].GetFailOptionMask(m_smVSInfo[i].g_intSelectedGroup, v));
                subKey1.SetValue("Secure-MarkFailOptionCheckMark", m_smVSInfo[i].g_arrMarks[u].ref_blnCheckMark);
            }

        }

        private void UpdateMarkPin1SecureOption(int i, int v)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);

            subKey1.SetValue("Secure-Pin1FailOption" + v.ToString(), m_smVSInfo[i].g_arrPin1[0].getWantCheckPin1(v));
        }

        private void UpdateLeadSecureOption(int i, int v)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);

            subKey1.SetValue("Secure-LeadFailOptionMask" + v.ToString(), m_smVSInfo[i].g_arrLead[v].ref_intFailOptionMask);
            subKey1.SetValue("Secure-LeadFailOptionCheckLead", m_smVSInfo[i].g_arrLead[v].GetWantInspectLead());
        }
        private void UpdateColorPackageSecureOption(int i)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);

            subKey1.SetValue("Secure-ColorPackageFailOptionMask", m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].ref_intFailColorOptionMask);
        }
        private void UpdatePackageSecureOption(int i)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);
            int intPackageFailMask = 0;
            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Bright))
                intPackageFailMask |= 0x01;
            else
                intPackageFailMask &= ~0x01;

            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Bright))
                intPackageFailMask |= 0x02;
            else
                intPackageFailMask &= ~0x02;

            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark))
                intPackageFailMask |= 0x04;
            else
                intPackageFailMask &= ~0x04;

            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark))
                intPackageFailMask |= 0x08;
            else
                intPackageFailMask &= ~0x08;

            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark2))
                intPackageFailMask |= 0x10;
            else
                intPackageFailMask &= ~0x10;

            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark2))
                intPackageFailMask |= 0x20;
            else
                intPackageFailMask &= ~0x20;

            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Crack))
                intPackageFailMask |= 0x40;
            else
                intPackageFailMask &= ~0x40;

            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Crack))
                intPackageFailMask |= 0x80;
            else
                intPackageFailMask &= ~0x80;

            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.MoldFlash))
                intPackageFailMask |= 0x100;
            else
                intPackageFailMask &= ~0x100;

            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.ChipBright))
                intPackageFailMask |= 0x200;
            else
                intPackageFailMask &= ~0x200;

            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.ChipDark))
                intPackageFailMask |= 0x400;
            else
                intPackageFailMask &= ~0x400;

            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.ChipBright))
                intPackageFailMask |= 0x800;
            else
                intPackageFailMask &= ~0x800;

            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.ChipDark))
                intPackageFailMask |= 0x1000;
            else
                intPackageFailMask &= ~0x1000;

            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark3))
                intPackageFailMask |= 0x2000;
            else
                intPackageFailMask &= ~0x2000;

            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark3))
                intPackageFailMask |= 0x4000;
            else
                intPackageFailMask &= ~0x4000;

            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark4))
                intPackageFailMask |= 0x8000;
            else
                intPackageFailMask &= ~0x8000;

            if (m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark4))
                intPackageFailMask |= 0x10000;
            else
                intPackageFailMask &= ~0x10000;

            subKey1.SetValue("Secure-PackageFailOptionMask", intPackageFailMask);
            subKey1.SetValue("Secure-PackageFailOptionCheckPackageSize", m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].ref_intFailMask);
            subKey1.SetValue("Secure-PackageFailOptionCheckPackageDefect", m_smVSInfo[i].g_arrPackage[m_smVSInfo[i].g_intSelectedUnit].GetWantInspectPackage());
        }
        private void UpdateCenterColorPadSecureOption(int i)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);

            subKey1.SetValue("Secure-CenterColorPadFailOptionMask", m_smVSInfo[i].g_arrPad[0].ref_intFailColorOptionMask);
        }
        private void UpdateSideColorPadSecureOption(int i)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);

            subKey1.SetValue("Secure-SideColorPadFailOptionMask_Top", m_smVSInfo[i].g_arrPad[1].ref_intFailColorOptionMask);
            subKey1.SetValue("Secure-SideColorPadFailOptionMask_Right", m_smVSInfo[i].g_arrPad[2].ref_intFailColorOptionMask);
            subKey1.SetValue("Secure-SideColorPadFailOptionMask_Bottom", m_smVSInfo[i].g_arrPad[3].ref_intFailColorOptionMask);
            subKey1.SetValue("Secure-SideColorPadFailOptionMask_Left", m_smVSInfo[i].g_arrPad[4].ref_intFailColorOptionMask);
        }
        private void UpdateCenterPadSecureOption(int i)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);

            subKey1.SetValue("Secure-CenterPadFailOptionMask", m_smVSInfo[i].g_arrPad[0].ref_intFailOptionMask);
            subKey1.SetValue("Secure-CenterPadFailOptionCheckCenterPadBrokenArea", m_smVSInfo[i].g_arrPad[0].ref_blnWantCheckBrokenPadArea);
            subKey1.SetValue("Secure-CenterPadFailOptionCheckCenterPadBrokenLength", m_smVSInfo[i].g_arrPad[0].ref_blnWantCheckBrokenPadLength);
            subKey1.SetValue("Secure-CenterPadFailOptionCheckCenterPadExtraArea", m_smVSInfo[i].g_arrPad[0].ref_blnWantCheckExtraPadArea);
            subKey1.SetValue("Secure-CenterPadFailOptionCheckCenterPadExtraLength", m_smVSInfo[i].g_arrPad[0].ref_blnWantCheckExtraPadLength);
            subKey1.SetValue("Secure-InspectCenterPad", m_smVSInfo[i].g_blnCheckPad);
        }

        private void UpdatePadPin1SecureOption(int i)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);

            subKey1.SetValue("Secure-PadPin1FailOption", m_smVSInfo[i].g_arrPin1[0].getWantCheckPin1(m_smVSInfo[i].g_intSelectedTemplate));
        }

        private void UpdateSidePadSecureOption(int i)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);

            subKey1.SetValue("Secure-SidePadFailOptionMask", m_smVSInfo[i].g_arrPad[1].ref_intFailOptionMask);
            subKey1.SetValue("Secure-SidePadFailOptionCheckSidePadBrokenArea", m_smVSInfo[i].g_arrPad[1].ref_blnWantCheckBrokenPadArea);
            subKey1.SetValue("Secure-SidePadFailOptionCheckSidePadBrokenLength", m_smVSInfo[i].g_arrPad[1].ref_blnWantCheckBrokenPadLength);
            subKey1.SetValue("Secure-SidePadFailOptionCheckSidePadExtraArea", m_smVSInfo[i].g_arrPad[1].ref_blnWantCheckExtraPadArea);
            subKey1.SetValue("Secure-SidePadFailOptionCheckSidePadExtraLength", m_smVSInfo[i].g_arrPad[1].ref_blnWantCheckExtraPadLength);
        }

        private void UpdateCenterPadPackageSecureOption(int i)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);

            subKey1.SetValue("Secure-CenterPadPackageFailOptionMask", m_smVSInfo[i].g_arrPad[0].ref_intFailPkgOptionMask);
            //subKey1.SetValue("Secure-CenterPadPackageFailOptionCheckPackageSize", m_smVSInfo[i].g_arrPad[0].ref_intFailOptionMask);
            subKey1.SetValue("Secure-CenterPadPackageFailOptionCheckPackageDefect", m_smVSInfo[i].g_arrPad[0].GetWantInspectPackage());
        }

        private void UpdateSidePadPackageSecureOption(int i)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);

            subKey1.SetValue("Secure-SidePadPackageFailOptionMask", m_smVSInfo[i].g_arrPad[1].ref_intFailPkgOptionMask);
            //subKey1.SetValue("Secure-SidePadPackageFailOptionCheckPackageSize", m_smVSInfo[i].g_arrPad[1].ref_intFailOptionMask);
            subKey1.SetValue("Secure-SidePadPackageFailOptionCheckPackageDefect", m_smVSInfo[i].g_arrPad[1].GetWantInspectPackage());
        }

        private void UpdateLead3DSecureOption(int i)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);

            subKey1.SetValue("Secure-Lead3DFailOptionMask", m_smVSInfo[i].g_arrLead3D[0].ref_intFailOptionMask);
            subKey1.SetValue("Secure-Lead3DFailOptionCheckLead3DExtraArea", m_smVSInfo[i].g_arrLead3D[0].ref_blnWantCheckExtraLeadArea);
            subKey1.SetValue("Secure-Lead3DFailOptionCheckLead3DExtraLength", m_smVSInfo[i].g_arrLead3D[0].ref_blnWantCheckExtraLeadLength);
        }

        private void UpdateLead3DPackageSecureOption(int i)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);

            subKey1.SetValue("Secure-Lead3DPackageFailOptionMask", m_smVSInfo[i].g_arrLead3D[0].ref_intFailPkgOptionMask);
            //subKey1.SetValue("Secure-Lead3DPackageFailOptionCheckPackageSize", m_smVSInfo[i].g_arrLead3D[0].ref_blnWantCheckExtraLeadArea);
            subKey1.SetValue("Secure-Lead3DPackageFailOptionCheckPackageDefect", m_smVSInfo[i].g_arrLead3D[0].GetWantInspectPackage());
        }

        private void UpdateSealSecureOption(int i)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            subKey1 = subKey.OpenSubKey(strVisionList[i], true);

            subKey1.SetValue("Secure-SealFailOptionMask", m_smVSInfo[i].g_objSeal.ref_intFailOptionMaskSeal);
        }

        private void CheckSealOptionSecureSetting(VisionInfo smVisionInfo, int intVisionNo, ref bool blnAllUnChecked, ref bool blnNotTally)
        {
            blnAllUnChecked = blnNotTally = false;
            //------------------------------------Seal--------------------------------
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            subKey1 = subKey.OpenSubKey(strVisionList[intVisionNo], true);

            string[] strsubkeyNames = subKey1.GetValueNames();
            bool blnFound = false;
            for (int i = 0; i < strsubkeyNames.Length; i++)
            {
                if (strsubkeyNames[i].Contains("Secure-SealFailOptionMask"))
                    blnFound = true;
            }
            if (!blnFound)
                subKey1.SetValue("Secure-SealFailOptionMask", smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal);

            int intSealFailOptionMask = Convert.ToInt32(subKey1.GetValue("Secure-SealFailOptionMask", 0));

            if (((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x01) == 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x02) == 0)
                && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x04) == 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x08) == 0)
                && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x10) == 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x20) == 0)
                && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x40) == 0) && ((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x80) == 0)
                && (((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x100) == 0) && !smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole)
                && (((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x200) == 0) && !smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
                && (((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x400) == 0) && !smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
                && (((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x800) == 0) && !smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
                && (((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x1000) == 0) && !smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
                && (((smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x2000) == 0) && smVisionInfo.g_objSeal.ref_blnWantCheckSealEdgeStraightness))
                blnAllUnChecked = true;

            if (intSealFailOptionMask != smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal)
            {
                
                STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Seal Fail Option", Convert.ToInt32(subKey1.GetValue("Secure-SealFailOptionMask", 0)).ToString(), smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString(), m_smProductionInfo.g_strLotID);
                
                blnNotTally = true;
            }

        }
        private void CheckOrientOptionSecureSetting(VisionInfo smVisionInfo, int intVisionNo, ref bool blnAllUnChecked, ref bool blnNotTally)
        {
            blnAllUnChecked = blnNotTally = false;
            //------------------------------------Orient--------------------------------
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            subKey1 = subKey.OpenSubKey(strVisionList[intVisionNo], true);

            string[] strsubkeyNames = subKey1.GetValueNames();
            bool blnFound = false;
            for (int i = 0; i < strsubkeyNames.Length; i++)
            {
                if (strsubkeyNames[i].Contains("Secure-OrientFailOptionMask"))
                    blnFound = true;
            }
            if (!blnFound)
            {
                int intFailOptionMask = 0;
                for (int u = 0; u < smVisionInfo.g_intUnitsOnImage; u++)
                {
                    if (smVisionInfo.g_arrOrients[u][0].ref_blnWantCheckOrientAngleTolerance)
                        intFailOptionMask |= 0x01;
                    else
                        intFailOptionMask &= ~0x01;

                    if (smVisionInfo.g_arrOrients[u][0].ref_blnWantCheckOrientXTolerance)
                        intFailOptionMask |= 0x02;
                    else
                        intFailOptionMask &= ~0x02;

                    if (smVisionInfo.g_arrOrients[u][0].ref_blnWantCheckOrientYTolerance)
                        intFailOptionMask |= 0x04;
                    else
                        intFailOptionMask &= ~0x04;

                    subKey1.SetValue("Secure-OrientFailOptionMask", intFailOptionMask);
                }
            }

            int intOrientFailOptionMask = Convert.ToInt32(subKey1.GetValue("Secure-OrientFailOptionMask", 0));

            for (int u = 0; u < smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (!smVisionInfo.g_arrOrients[u][0].ref_blnWantCheckOrientAngleTolerance && !smVisionInfo.g_arrOrients[u][0].ref_blnWantCheckOrientXTolerance && !smVisionInfo.g_arrOrients[u][0].ref_blnWantCheckOrientYTolerance)
                    blnAllUnChecked = true;

                if (((intOrientFailOptionMask & 0x01) > 0) != smVisionInfo.g_arrOrients[u][0].ref_blnWantCheckOrientAngleTolerance)
                    blnNotTally = true;

                if (((intOrientFailOptionMask & 0x02) > 0) != smVisionInfo.g_arrOrients[u][0].ref_blnWantCheckOrientXTolerance)
                    blnNotTally = true;

                if (((intOrientFailOptionMask & 0x04) > 0) != smVisionInfo.g_arrOrients[u][0].ref_blnWantCheckOrientYTolerance)
                    blnNotTally = true;

                if (blnNotTally)
                {
                    
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Orient Fail Option", Convert.ToInt32(subKey1.GetValue("Secure-OrientFailOptionMask", 0)).ToString(), intOrientFailOptionMask.ToString(), m_smProductionInfo.g_strLotID);
                    
                }
            }

        }
        private void CheckOrientPadOptionSecureSetting(VisionInfo smVisionInfo, int intVisionNo, ref bool blnAllUnChecked, ref bool blnNotTally)
        {
            blnAllUnChecked = blnNotTally = false;
            //------------------------------------OrientPad--------------------------------
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            subKey1 = subKey.OpenSubKey(strVisionList[intVisionNo], true);

            string[] strsubkeyNames = subKey1.GetValueNames();
            bool blnFound = false;
            for (int i = 0; i < strsubkeyNames.Length; i++)
            {
                if (strsubkeyNames[i].Contains("Secure-OrientPadFailOptionMask"))
                    blnFound = true;
            }
            if (!blnFound)
            {
                int intFailOptionMask = 0;
                if (smVisionInfo.g_objPadOrient != null)
                {
                    if (smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientAngleTolerance)
                        intFailOptionMask |= 0x8000000;
                    else
                        intFailOptionMask &= ~0x8000000;

                    if (smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientXTolerance)
                        intFailOptionMask |= 0x10000000;
                    else
                        intFailOptionMask &= ~0x10000000;

                    if (smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientYTolerance)
                        intFailOptionMask |= 0x20000000;
                    else
                        intFailOptionMask &= ~0x20000000;

                    subKey1.SetValue("Secure-OrientPadFailOptionMask", intFailOptionMask);
                }
            }

            int intOrientPadFailOptionMask = Convert.ToInt32(subKey1.GetValue("Secure-OrientPadFailOptionMask", 0));

            if (smVisionInfo.g_objPadOrient != null)
            {
                if (!smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientAngleTolerance && !smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientXTolerance && !smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientYTolerance)
                    blnAllUnChecked = true;

                if (((intOrientPadFailOptionMask & 0x8000000) > 0) != smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientAngleTolerance)
                    blnNotTally = true;

                if (((intOrientPadFailOptionMask & 0x10000000) > 0) != smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientXTolerance)
                    blnNotTally = true;

                if (((intOrientPadFailOptionMask & 0x20000000) > 0) != smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientYTolerance)
                    blnNotTally = true;

                if (blnNotTally)
                {
                    
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "OrientPad Fail Option", Convert.ToInt32(subKey1.GetValue("Secure-OrientPadFailOptionMask", 0)).ToString(), intOrientPadFailOptionMask.ToString(), m_smProductionInfo.g_strLotID);
                    
                }
            }

        }
        private void CheckMarkOptionSecureSetting(VisionInfo smVisionInfo, int intVisionNo, ref List<bool> blnAllUnChecked, ref List<bool> blnNotTally, ref List<bool> blnParentUnChecked, ref List<bool> blnPin1NotTally)
        {
            for (int v = 0; v < smVisionInfo.g_intTotalTemplates; v++)
            {
                blnAllUnChecked[v] = blnNotTally[v] = blnParentUnChecked[v] = blnPin1NotTally[v] = false;
            }
            //------------------------------------Mark--------------------------------
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            List<int> arrMarkFailOptionMask = new List<int>();
            List<bool> arrCheckMark = new List<bool>();
            subKey1 = subKey.OpenSubKey(strVisionList[intVisionNo], true);

            string[] strsubkeyNames = subKey1.GetValueNames();
            bool[] blnFoundMarkOption = new bool[smVisionInfo.g_intTotalTemplates], blnFoundPin1Option = new bool[smVisionInfo.g_intTotalTemplates];
            bool blnFoundCheckMark = false;
            for (int i = 0; i < strsubkeyNames.Length; i++)
            {
                for (int v = 0; v < smVisionInfo.g_intTotalTemplates; v++)
                {
                    if (strsubkeyNames[i].Contains("Secure-MarkFailOptionMask" + v.ToString()))
                        blnFoundMarkOption[v] = true;
                    if (strsubkeyNames[i].Contains("Secure-MarkFailOptionCheckMark"))
                        blnFoundCheckMark = true;
                    if (strsubkeyNames[i].Contains("Secure-Pin1FailOption" + v.ToString()))
                        blnFoundPin1Option[v] = true;
                }
            }
            for (int v = 0; v < smVisionInfo.g_intTotalTemplates; v++)
            {
                if (!blnFoundMarkOption[v])
                {
                    for (int u = 0; u < smVisionInfo.g_intUnitsOnImage; u++)
                    {
                        //subKey1.CreateSubKey("Secure-MarkFailOptionCheckMark");
                        subKey1.SetValue("Secure-MarkFailOptionMask" + v.ToString(), smVisionInfo.g_arrMarks[u].GetFailOptionMask(smVisionInfo.g_intSelectedGroup, v), RegistryValueKind.DWord);

                    }
                }
            }
            if (!blnFoundCheckMark)
            {
                for (int u = 0; u < smVisionInfo.g_intUnitsOnImage; u++)
                {
                    subKey1.SetValue("Secure-MarkFailOptionCheckMark", smVisionInfo.g_arrMarks[u].ref_blnCheckMark);
                }
            }
            for (int v = 0; v < smVisionInfo.g_intTotalTemplates; v++)
            {
                if (!blnFoundPin1Option[v])
                {
                    subKey1.SetValue("Secure-Pin1FailOption" + v.ToString(), smVisionInfo.g_arrPin1[0].getWantCheckPin1(v));

                }
            }
            for (int i = 0; i < smVisionInfo.g_intTotalTemplates; i++)
            {
                arrMarkFailOptionMask.Add(Convert.ToInt32(subKey1.GetValue("Secure-MarkFailOptionMask" + i.ToString(), 0)));
                arrCheckMark.Add((bool)subKey1.GetValue("Secure-MarkFailOptionCheckMark", true).Equals("True"));

            }
            
            for (int u = 0; u < smVisionInfo.g_intUnitsOnImage; u++)
            {
                for (int v = 0; v < smVisionInfo.g_intTotalTemplates; v++)
                {
                    int intFailMask = smVisionInfo.g_arrMarks[u].GetFailOptionMask(smVisionInfo.g_intSelectedGroup, v);

                    if (!smVisionInfo.g_arrMarks[u].ref_blnCheckMark)
                        blnParentUnChecked[v] = true;

                    if (((intFailMask & 0x01) == 0) && ((intFailMask & 0x02) == 0) && ((intFailMask & 0x04) == 0) && ((intFailMask & 0x08) == 0)
                     && ((intFailMask & 0x10) == 0) && ((intFailMask & 0x20) == 0 && smVisionInfo.g_blnWantCheckMarkBroken) && ((intFailMask & 0x40) == 0) && ((intFailMask & 0x80) == 0) 
                     && ((intFailMask & 0x2000) == 0 && smVisionInfo.g_blnWantCheckMarkAngle) && ((intFailMask & 0x100) == 0 && smVisionInfo.g_blnWantCheckMarkTotalExcess) && ((intFailMask & 0x200) == 0 && smVisionInfo.g_blnWantCheckMarkAverageGrayValue))
                        blnAllUnChecked[v] = true;

                    if ((arrMarkFailOptionMask[v] != intFailMask)
                        || (arrCheckMark[v] != smVisionInfo.g_arrMarks[u].ref_blnCheckMark))
                    {
                        if (arrMarkFailOptionMask[v] != intFailMask)
                            STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Mark Template " + (v + 1).ToString() + " Fail Option", Convert.ToInt32(subKey1.GetValue("Secure-MarkFailOptionMask" + v.ToString(), 0)).ToString(), smVisionInfo.g_arrMarks[u].GetFailOptionMask(smVisionInfo.g_intSelectedGroup, v).ToString(), m_smProductionInfo.g_strLotID);
                        if (arrCheckMark[v] != smVisionInfo.g_arrMarks[u].ref_blnCheckMark)
                            STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Mark Template " + (v + 1).ToString() + " Want Check Mark", ((bool)subKey1.GetValue("Secure-MarkFailOptionCheckMark", true).Equals("True")).ToString(), smVisionInfo.g_arrMarks[u].ref_blnCheckMark.ToString(), m_smProductionInfo.g_strLotID);

                        blnNotTally[v] = true;
                    }
                }
            }
            //------------------------------------Pin1--------------------------------
            List<bool> arrPin1FailOptionMask = new List<bool>();
            for (int i = 0; i < smVisionInfo.g_intTotalTemplates; i++)
                arrPin1FailOptionMask.Add((bool)subKey1.GetValue("Secure-Pin1FailOption" + i.ToString(), true).Equals("True"));
            if (smVisionInfo.g_blnWantPin1)
            {
                for (int v = 0; v < smVisionInfo.g_intTotalTemplates; v++)
                {
                    if (arrPin1FailOptionMask[v] != smVisionInfo.g_arrPin1[0].getWantCheckPin1(v))
                    {
                        STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Mark Template " + (v + 1).ToString() + " Want Check Pin 1", ((bool)subKey1.GetValue("Secure-Pin1FailOption" + v.ToString(), true).Equals("True")).ToString(), smVisionInfo.g_arrPin1[0].getWantCheckPin1(v).ToString(), m_smProductionInfo.g_strLotID);

                        blnPin1NotTally[v] = true;
                    }
                }
            }
            
        }

        private bool CheckLeadOptionSecureSetting(VisionInfo smVisionInfo, int intVisionNo, ref List<bool> blnAllUnChecked, ref List<bool> blnNotTally, ref bool blnParentUnChecked)
        {
            for (int v = 0; v < smVisionInfo.g_arrLead.Length; v++)
            {
                blnAllUnChecked[v] = blnNotTally[v] = false;
            }
            //------------------------------------Lead--------------------------------
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            subKey1 = subKey.OpenSubKey(strVisionList[intVisionNo], true);

            string[] strsubkeyNames = subKey1.GetValueNames();
            bool[] blnFoundLeadOption = new bool[smVisionInfo.g_arrLead.Length];
            bool blnFoundWantInspectLead = false;
            for (int i = 0; i < strsubkeyNames.Length; i++)
            {
                for (int v = 0; v < smVisionInfo.g_arrLead.Length; v++)
                {
                    if (strsubkeyNames[i].Contains("Secure-LeadFailOptionMask" + v.ToString()))
                        blnFoundLeadOption[v] = true;
                    if (strsubkeyNames[i].Contains("Secure-LeadFailOptionCheckLead"))
                        blnFoundWantInspectLead = true;
                }
            }
            for (int v = 0; v < smVisionInfo.g_arrLead.Length; v++)
            {
                if (!blnFoundLeadOption[v])
                {
                    subKey1.SetValue("Secure-LeadFailOptionMask" + v.ToString(), smVisionInfo.g_arrLead[v].ref_intFailOptionMask);
                }

                if (!blnFoundWantInspectLead)
                    subKey1.SetValue("Secure-LeadFailOptionCheckLead", smVisionInfo.g_arrLead[0].GetWantInspectLead());

            }

            for (int i = 0; i < smVisionInfo.g_arrLead.Length; i++)
            {
                if (!smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;
                int intLeadFailOptionMask = Convert.ToInt32(subKey1.GetValue("Secure-LeadFailOptionMask" + i.ToString(), 0));
                bool blnWantInspectLead = (bool)subKey1.GetValue("Secure-LeadFailOptionCheckLead", true).Equals("True");
                int intFailMask = smVisionInfo.g_arrLead[i].ref_intFailOptionMask;

                if (!smVisionInfo.g_arrLead[i].GetWantInspectLead())
                    blnParentUnChecked = true;

                if (!smVisionInfo.g_arrLead[i].GetWantInspectLead() && ((intFailMask & 0xC0) == 0) && ((intFailMask & 0x100) == 0) && ((intFailMask & 0x800) == 0) && ((intFailMask & 0x600) == 0) && ((intFailMask & 0x1000) == 0) && (((intFailMask & 0x4000) == 0) && smVisionInfo.g_arrLead[0].ref_blnWantUseAverageGrayValueMethod)
                    && ((intFailMask & 0x2000) == 0) && (((intFailMask & 0x10000) == 0) && ((intFailMask & 0x20000) == 0) && smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead) && !smVisionInfo.g_arrLead[i].ref_blnWantCheckExtraLeadArea && !smVisionInfo.g_arrLead[i].ref_blnWantCheckExtraLeadLength && ((intFailMask & 0x8000) == 0))// && !smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance))
                    blnAllUnChecked[i] = true;

                if ((blnWantInspectLead != smVisionInfo.g_arrLead[i].GetWantInspectLead()) || (intLeadFailOptionMask != intFailMask))
                {
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
                    
                    if (blnWantInspectLead != smVisionInfo.g_arrLead[i].GetWantInspectLead())
                        STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", " Inspect Lead", subKey1.GetValue("Secure-LeadFailOptionCheckLead", true).ToString(), smVisionInfo.g_arrLead[i].GetWantInspectLead().ToString(), m_smProductionInfo.g_strLotID);
                    else
                        STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Lead " + strSectionName + " Fail Option", Convert.ToInt32(subKey1.GetValue("Secure-LeadFailOptionMask" + i.ToString(), 0)).ToString(), smVisionInfo.g_arrLead[i].ref_intFailOptionMask.ToString(), m_smProductionInfo.g_strLotID);
                    
                    blnNotTally[i] = true;
                }
            }
            return true;
        }
        private void CheckColorPackageOptionSecureSetting(VisionInfo smVisionInfo, int intVisionNo, ref bool blnAllUnChecked, ref bool blnNotTally)
        {
            blnAllUnChecked = blnNotTally = false;
            //------------------------------------Color Package--------------------------------
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            subKey1 = subKey.OpenSubKey(strVisionList[intVisionNo], true);

            string[] strsubkeyNames = subKey1.GetValueNames();
            bool blnFoundFailOption = false;
            for (int i = 0; i < strsubkeyNames.Length; i++)
            {
                if (strsubkeyNames[i].Contains("Secure-ColorPackageFailOptionMask"))
                    blnFoundFailOption = true;

            }

            if (!blnFoundFailOption)
                subKey1.SetValue("Secure-ColorPackageFailOptionMask", smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask);

            int intColorPackageFailOptionMask = Convert.ToInt32(subKey1.GetValue("Secure-ColorPackageFailOptionMask", 0));

            int intFailMask = smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask;

            if ((((intFailMask & 0x01) == 0) && ((intFailMask & 0x02) == 0) && smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 0) &&
                (((intFailMask & 0x04) == 0) && ((intFailMask & 0x08) == 0) && smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 1) &&
                (((intFailMask & 0x10) == 0) && ((intFailMask & 0x20) == 0) && smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2) &&
                (((intFailMask & 0x40) == 0) && ((intFailMask & 0x80) == 0) && smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3) && 
                (((intFailMask & 0x100) == 0) && ((intFailMask & 0x200) == 0) && smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4))
                blnAllUnChecked = true;

            if (intColorPackageFailOptionMask != intFailMask)
            {

                if (intColorPackageFailOptionMask != intFailMask)
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Color Package Fail Option", Convert.ToInt32(subKey1.GetValue("Secure-ColorPackageFailOptionMask", 0)).ToString(), smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask.ToString(), m_smProductionInfo.g_strLotID);

                blnNotTally = true;
            }

        }
        private void CheckPackageOptionSecureSetting(VisionInfo smVisionInfo, int intVisionNo, ref bool blnAllUnChecked, ref bool blnNotTally, ref bool blnParentUnChecked)
        {
            blnAllUnChecked = blnNotTally = blnParentUnChecked = false;
            //------------------------------------Package--------------------------------
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            subKey1 = subKey.OpenSubKey(strVisionList[intVisionNo], true);

            string[] strsubkeyNames = subKey1.GetValueNames();
            bool blnFoundFailOption = false, blnFoundCheckSize = false, blnFoundCheckDefect = false;
            for (int i = 0; i < strsubkeyNames.Length; i++)
            {
                if (strsubkeyNames[i].Contains("Secure-PackageFailOptionMask"))
                    blnFoundFailOption = true;

                if (strsubkeyNames[i].Contains("Secure-PackageFailOptionCheckPackageSize"))
                    blnFoundCheckSize = true;

                if (strsubkeyNames[i].Contains("Secure-PackageFailOptionCheckPackageDefect"))
                    blnFoundCheckDefect = true;
            }
            if (!blnFoundFailOption)
            {
                int intPackageFailMask = 0;
                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Bright))
                    intPackageFailMask |= 0x01;
                else
                    intPackageFailMask &= ~0x01;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Bright))
                    intPackageFailMask |= 0x02;
                else
                    intPackageFailMask &= ~0x02;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark))
                    intPackageFailMask |= 0x04;
                else
                    intPackageFailMask &= ~0x04;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark))
                    intPackageFailMask |= 0x08;
                else
                    intPackageFailMask &= ~0x08;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark2))
                    intPackageFailMask |= 0x10;
                else
                    intPackageFailMask &= ~0x10;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark2))
                    intPackageFailMask |= 0x20;
                else
                    intPackageFailMask &= ~0x20;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Crack))
                    intPackageFailMask |= 0x40;
                else
                    intPackageFailMask &= ~0x40;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Crack))
                    intPackageFailMask |= 0x80;
                else
                    intPackageFailMask &= ~0x80;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.MoldFlash))
                    intPackageFailMask |= 0x100;
                else
                    intPackageFailMask &= ~0x100;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.ChipBright))
                    intPackageFailMask |= 0x200;
                else
                    intPackageFailMask &= ~0x200;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.ChipDark))
                    intPackageFailMask |= 0x400;
                else
                    intPackageFailMask &= ~0x400;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.ChipBright))
                    intPackageFailMask |= 0x800;
                else
                    intPackageFailMask &= ~0x800;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.ChipDark))
                    intPackageFailMask |= 0x1000;
                else
                    intPackageFailMask &= ~0x1000;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark3))
                    intPackageFailMask |= 0x2000;
                else
                    intPackageFailMask &= ~0x2000;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark3))
                    intPackageFailMask |= 0x4000;
                else
                    intPackageFailMask &= ~0x4000;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark4))
                    intPackageFailMask |= 0x8000;
                else
                    intPackageFailMask &= ~0x8000;

                if (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark4))
                    intPackageFailMask |= 0x10000;
                else
                    intPackageFailMask &= ~0x10000;

                subKey1.SetValue("Secure-PackageFailOptionMask", intPackageFailMask);
            }
            if (!blnFoundCheckSize)
            {
                subKey1.SetValue("Secure-PackageFailOptionCheckPackageSize", smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_intFailMask);
            }
            if (!blnFoundCheckDefect)
            {
                subKey1.SetValue("Secure-PackageFailOptionCheckPackageDefect", smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage());
            }
            int intPackageFailOptionMask = Convert.ToInt32(subKey1.GetValue("Secure-PackageFailOptionMask", 0));
            bool blnCheckPackageSize = (Convert.ToInt32(subKey1.GetValue("Secure-PackageFailOptionCheckPackageSize", 0)) & 0x1000) > 0;
            bool blnCheckPackageDefect = (bool)subKey1.GetValue("Secure-PackageFailOptionCheckPackageDefect", true).Equals("True");

            if (((smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x1000) == 0) && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage())
                blnParentUnChecked = true;

            if (!smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Bright) && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Bright)
                && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark) && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark)
                && (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark2) && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark2))
                && (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark3) && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark3))
                && (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark4) && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark4))
                && (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Crack) && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Crack))
                && (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.MoldFlash))
                && (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.ChipBright) && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.ChipDark)
                && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.ChipBright) && !smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.ChipDark))
                )
                blnAllUnChecked = true;

            if ((blnCheckPackageSize != ((smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x1000) > 0))
                || (blnCheckPackageDefect != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage())
                || (((intPackageFailOptionMask & 0x01) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Bright))
                || (((intPackageFailOptionMask & 0x02) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Bright))
                || (((intPackageFailOptionMask & 0x04) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark))
                || ((((intPackageFailOptionMask & 0x08) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark)))
                || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting && (((intPackageFailOptionMask & 0x10) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark2)))
                || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting && ((intPackageFailOptionMask & 0x20) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark2))
                || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting && (((intPackageFailOptionMask & 0x2000) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark3)))
                || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting && ((intPackageFailOptionMask & 0x4000) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark3))
                || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting && (((intPackageFailOptionMask & 0x8000) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark4)))
                || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting && ((intPackageFailOptionMask & 0x10000) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark4))
                || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting && (((intPackageFailOptionMask & 0x40) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Crack)))
                || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting && (((intPackageFailOptionMask & 0x80) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Crack)))
                || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting && (((intPackageFailOptionMask & 0x100) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.MoldFlash)))
                || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting && (((intPackageFailOptionMask & 0x200) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.ChipBright)))
                || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting && (((intPackageFailOptionMask & 0x400) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.ChipDark)))
                || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting && (((intPackageFailOptionMask & 0x800) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.ChipBright)))
                || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting && (((intPackageFailOptionMask & 0x1000) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.ChipDark)))
                )
            {
                
                if ((((intPackageFailOptionMask & 0x01) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Bright))
           || (((intPackageFailOptionMask & 0x02) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Bright))
           || (((intPackageFailOptionMask & 0x04) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark))
           || (((intPackageFailOptionMask & 0x08) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark))
           || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting && (((intPackageFailOptionMask & 0x10) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark2)))
           || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting && ((intPackageFailOptionMask & 0x20) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark2))
           || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting && (((intPackageFailOptionMask & 0x2000) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark3)))
           || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting && ((intPackageFailOptionMask & 0x4000) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark3))
           || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting && (((intPackageFailOptionMask & 0x8000) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark4)))
           || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting && ((intPackageFailOptionMask & 0x10000) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark4))
           || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting && (((intPackageFailOptionMask & 0x40) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Crack)))
           || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting && (((intPackageFailOptionMask & 0x80) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Crack)))
           || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting && (((intPackageFailOptionMask & 0x100) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.MoldFlash)))
           || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting && (((intPackageFailOptionMask & 0x200) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.ChipBright)))
           || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting && (((intPackageFailOptionMask & 0x400) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.ChipDark)))
           || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting && (((intPackageFailOptionMask & 0x800) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.ChipBright)))
           || (smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting && (((intPackageFailOptionMask & 0x1000) > 0) != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.ChipDark))))
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Package Fail Option", Convert.ToInt32(subKey1.GetValue("Secure-PackageFailOptionMask", 0)).ToString(), intPackageFailOptionMask.ToString(), m_smProductionInfo.g_strLotID);
                if (blnCheckPackageSize != ((smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x1000) > 0))
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Package Size Fail Option", Convert.ToInt32(subKey1.GetValue("Secure-PackageFailOptionCheckPackageSize", 0)).ToString(), smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].ref_intFailMask.ToString(), m_smProductionInfo.g_strLotID);
                if (blnCheckPackageDefect != smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage())
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Package Want Check Package Defect", ((bool)subKey1.GetValue("Secure-PackageFailOptionCheckPackageDefect", true).Equals("True")).ToString(), smVisionInfo.g_arrPackage[smVisionInfo.g_intSelectedUnit].GetWantInspectPackage().ToString(), m_smProductionInfo.g_strLotID);
                
                blnNotTally = true;
            }
        }
        private void CheckCenterColorPadOptionSecureSetting(VisionInfo smVisionInfo, int intVisionNo, ref bool blnAllUnChecked, ref bool blnNotTally)
        {
            blnAllUnChecked = blnNotTally = false;
            //------------------------------------Color Pad--------------------------------
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            subKey1 = subKey.OpenSubKey(strVisionList[intVisionNo], true);

            string[] strsubkeyNames = subKey1.GetValueNames();
            bool blnFoundFailOption = false;
            for (int i = 0; i < strsubkeyNames.Length; i++)
            {
                if (strsubkeyNames[i].Contains("Secure-CenterColorPadFailOptionMask"))
                    blnFoundFailOption = true;

            }

            if (!blnFoundFailOption)
                subKey1.SetValue("Secure-CenterColorPadFailOptionMask", smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask);

            int intCenterColorPadFailOptionMask = Convert.ToInt32(subKey1.GetValue("Secure-CenterColorPadFailOptionMask", 0));

            int intFailMask = smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask;

            if ((((intFailMask & 0x01) == 0) && ((intFailMask & 0x02) == 0) && smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 0) &&
                (((intFailMask & 0x04) == 0) && ((intFailMask & 0x08) == 0) && smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 1) &&
                (((intFailMask & 0x10) == 0) && ((intFailMask & 0x20) == 0) && smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2) &&
                (((intFailMask & 0x40) == 0) && ((intFailMask & 0x80) == 0) && smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3) &&
                (((intFailMask & 0x100) == 0) && ((intFailMask & 0x200) == 0) && smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4))
                blnAllUnChecked = true;

            if (intCenterColorPadFailOptionMask != intFailMask)
            {
                
                if (intCenterColorPadFailOptionMask != intFailMask)
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Center Color Pad Fail Option", Convert.ToInt32(subKey1.GetValue("Secure-CenterColorPadFailOptionMask", 0)).ToString(), smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask.ToString(), m_smProductionInfo.g_strLotID);
                
                blnNotTally = true;
            }

        }
        private void CheckSideColorPadOptionSecureSetting(VisionInfo smVisionInfo, int intVisionNo, ref bool blnAllUnChecked, ref bool blnNotTally)
        {
            blnAllUnChecked = blnNotTally = false;
            //------------------------------------Color Pad--------------------------------
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            subKey1 = subKey.OpenSubKey(strVisionList[intVisionNo], true);

            string[] strsubkeyNames = subKey1.GetValueNames();
            int intAllUnCheckedCounter = 0;
            for (int i = 1; i < smVisionInfo.g_arrPad.Length; i++)
            {
                bool blnFoundFailOption = false;
                string strDirection = "";
                if (i == 1)
                    strDirection = "Top";
                else if (i == 2)
                    strDirection = "Right";
                else if (i == 3)
                    strDirection = "Bottom";
                else if (i == 4)
                    strDirection = "Left";

                for (int j = 0; j < strsubkeyNames.Length; j++)
                {
                    if (strsubkeyNames[j].Contains("Secure-SideColorPadFailOptionMask_" + strDirection))
                        blnFoundFailOption = true;
                }

                if (!blnFoundFailOption)
                    subKey1.SetValue("Secure-SideColorPadFailOptionMask_" + strDirection, smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask);

                int intSideColorPadFailOptionMask = Convert.ToInt32(subKey1.GetValue("Secure-SideColorPadFailOptionMask_" + strDirection, 0));

                int intFailMask = smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask;

                if ((((intFailMask & 0x01) == 0) && ((intFailMask & 0x02) == 0) && smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 0) &&
                    (((intFailMask & 0x04) == 0) && ((intFailMask & 0x08) == 0) && smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1) &&
                    (((intFailMask & 0x10) == 0) && ((intFailMask & 0x20) == 0) && smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2) &&
                    (((intFailMask & 0x40) == 0) && ((intFailMask & 0x80) == 0) && smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3) &&
                    (((intFailMask & 0x100) == 0) && ((intFailMask & 0x200) == 0) && smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4))
                    intAllUnCheckedCounter++;

                if (intSideColorPadFailOptionMask != intFailMask)
                {

                    if (intSideColorPadFailOptionMask != intFailMask)
                        STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", strDirection + " Side Color Pad Fail Option", Convert.ToInt32(subKey1.GetValue("Secure-SideColorPadFailOptionMask_" + strDirection, 0)).ToString(), smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask.ToString(), m_smProductionInfo.g_strLotID);

                    blnNotTally = true;
                }
            }

            if(intAllUnCheckedCounter == smVisionInfo.g_arrPad.Length - 1)
                blnAllUnChecked = true;
        }
        private void CheckPadOptionSecureSetting(VisionInfo smVisionInfo, int intVisionNo, ref bool blnAllUnChecked, ref bool blnNotTally, ref bool blnPin1NotTally)
        {
            blnAllUnChecked = blnNotTally = blnPin1NotTally = false;
            //------------------------------------Pad--------------------------------
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            subKey1 = subKey.OpenSubKey(strVisionList[intVisionNo], true);

            string[] strsubkeyNames = subKey1.GetValueNames();
            bool blnFoundFailOption = false, blnFoundInspectPad = false, blnFoundCheckCenterPadBrokenArea = false, blnFoundCheckCenterPadBrokenLength = false, blnFoundCheckCenterPadExtraArea = false, blnFoundCheckCenterPadExtraLength = false, blnFoundPin1Option = false;
            for (int i = 0; i < strsubkeyNames.Length; i++)
            {
                if (strsubkeyNames[i].Contains("Secure-InspectCenterPad"))
                    blnFoundInspectPad = true;

                if (strsubkeyNames[i].Contains("Secure-CenterPadFailOptionMask"))
                    blnFoundFailOption = true;

                if (strsubkeyNames[i].Contains("Secure-CenterPadFailOptionCheckCenterPadBrokenArea"))
                    blnFoundCheckCenterPadBrokenArea = true;

                if (strsubkeyNames[i].Contains("Secure-CenterPadFailOptionCheckCenterPadBrokenLength"))
                    blnFoundCheckCenterPadBrokenLength = true;

                if (strsubkeyNames[i].Contains("Secure-CenterPadFailOptionCheckCenterPadExtraArea"))
                    blnFoundCheckCenterPadExtraArea = true;

                if (strsubkeyNames[i].Contains("Secure-CenterPadFailOptionCheckCenterPadExtraLength"))
                    blnFoundCheckCenterPadExtraLength = true;

                if (strsubkeyNames[i].Contains("Secure-PadPin1FailOption"))
                    blnFoundPin1Option = true;
            }
            if (!blnFoundInspectPad)
                subKey1.SetValue("Secure-InspectCenterPad", smVisionInfo.g_blnCheckPad);
            if (!blnFoundFailOption)
                subKey1.SetValue("Secure-CenterPadFailOptionMask", smVisionInfo.g_arrPad[0].ref_intFailOptionMask);
            if (!blnFoundCheckCenterPadBrokenArea)
                subKey1.SetValue("Secure-CenterPadFailOptionCheckCenterPadBrokenArea", smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadArea);
            if (!blnFoundCheckCenterPadBrokenLength)
                subKey1.SetValue("Secure-CenterPadFailOptionCheckCenterPadBrokenLength", smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadLength);
            if (!blnFoundCheckCenterPadExtraArea)
                subKey1.SetValue("Secure-CenterPadFailOptionCheckCenterPadExtraArea", smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadArea);
            if (!blnFoundCheckCenterPadExtraLength)
                subKey1.SetValue("Secure-CenterPadFailOptionCheckCenterPadExtraLength", smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadLength);
            if (!blnFoundPin1Option)
                subKey1.SetValue("Secure-PadPin1FailOption", smVisionInfo.g_arrPin1[0].getWantCheckPin1(smVisionInfo.g_intSelectedTemplate));

            bool blnInspectPad = (bool)subKey1.GetValue("Secure-InspectCenterPad", true).Equals("True");
            int intCenterPadFailOptionMask = Convert.ToInt32(subKey1.GetValue("Secure-CenterPadFailOptionMask", 0));
            bool blnCheckCenterPadBrokenArea = (bool)subKey1.GetValue("Secure-CenterPadFailOptionCheckCenterPadBrokenArea", true).Equals("True");
            bool blnCheckCenterPadBrokenLength = (bool)subKey1.GetValue("Secure-CenterPadFailOptionCheckCenterPadBrokenLength", true).Equals("True");
            bool blnCheckCenterPadExtraArea = (bool)subKey1.GetValue("Secure-CenterPadFailOptionCheckCenterPadExtraArea", true).Equals("True");
            bool blnCheckCenterPadExtraLength = (bool)subKey1.GetValue("Secure-CenterPadFailOptionCheckCenterPadExtraLength", true).Equals("True");

            int intFailMask = smVisionInfo.g_arrPad[0].ref_intFailOptionMask;

            if (((intFailMask & 0x20) == 0) && ((intFailMask & 0xC0) == 0) && ((intFailMask & 0x100) == 0) && !smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadArea && !smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadLength
                && ((intFailMask & 0x600) == 0) && ((intFailMask & 0x800) == 0) && ((intFailMask & 0x2000) == 0) && (((intFailMask & 0x4000) == 0) && smVisionInfo.g_arrPad[0].ref_blnWantEdgeLimit_Pad)
                && (((intFailMask & 0x8000) == 0) && smVisionInfo.g_arrPad[0].ref_blnWantStandOff_Pad) && (((intFailMask & 0x10000) == 0) && smVisionInfo.g_arrPad[0].ref_blnWantEdgeDistance_Pad)
                && (((intFailMask & 0x20000) == 0 && (intFailMask & 0x40000) == 0) && smVisionInfo.g_arrPad[0].ref_blnWantSpan_Pad)
                && (((smVisionInfo.g_strVisionName != "Pad5SPkg" && smVisionInfo.g_strVisionName != "PadPkg") || !smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize) && !smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadArea && !smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadLength && ((intFailMask & 0x1000) == 0))
                && ((!smVisionInfo.g_blnCheckPad) && ((m_smCustomizeInfo.g_intWantOrient & (1 << smVisionInfo.g_intVisionPos)) > 0)))
                blnAllUnChecked = true;
            
            if ((intCenterPadFailOptionMask != intFailMask) || (blnCheckCenterPadBrokenArea != smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadArea) || (blnCheckCenterPadBrokenLength != smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadLength)
                || (((smVisionInfo.g_strVisionName != "Pad5SPkg" && smVisionInfo.g_strVisionName != "PadPkg") || !smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize) && ((blnCheckCenterPadExtraArea != smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadArea) || (blnCheckCenterPadExtraLength != smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadLength)))
                || ((blnInspectPad != smVisionInfo.g_blnCheckPad) && ((m_smCustomizeInfo.g_intWantOrient & (1 << smVisionInfo.g_intVisionPos)) > 0)))
            {
                if (intCenterPadFailOptionMask != intFailMask)
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Center Pad Fail Option", Convert.ToInt32(subKey1.GetValue("Secure-CenterPadFailOptionMask", 0)).ToString(), smVisionInfo.g_arrPad[0].ref_intFailOptionMask.ToString(), m_smProductionInfo.g_strLotID);
                if (blnCheckCenterPadBrokenArea != smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadArea)
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Center Pad Want Check Broken Area", ((bool)subKey1.GetValue("Secure-CenterPadFailOptionCheckCenterPadBrokenArea", true).Equals("True")).ToString(), smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadArea.ToString(), m_smProductionInfo.g_strLotID);
                if (blnCheckCenterPadBrokenLength != smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadLength)
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Center Pad Want Check Broken Length", ((bool)subKey1.GetValue("Secure-CenterPadFailOptionCheckCenterPadBrokenLength", true).Equals("True")).ToString(), smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadLength.ToString(), m_smProductionInfo.g_strLotID);
                if (((smVisionInfo.g_strVisionName != "Pad5SPkg" && smVisionInfo.g_strVisionName != "PadPkg") || !smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize) && ((blnCheckCenterPadExtraArea != smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadArea)))
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Center Pad Want Check Extra Area", ((bool)subKey1.GetValue("Secure-CenterPadFailOptionCheckCenterPadExtraArea", true).Equals("True")).ToString(), smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadArea.ToString(), m_smProductionInfo.g_strLotID);
                if (((smVisionInfo.g_strVisionName != "Pad5SPkg" && smVisionInfo.g_strVisionName != "PadPkg") || !smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize) && ((blnCheckCenterPadExtraLength != smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadLength)))
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Center Pad Want Check Extra Length", ((bool)subKey1.GetValue("Secure-CenterPadFailOptionCheckCenterPadExtraLength", true).Equals("True")).ToString(), smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadLength.ToString(), m_smProductionInfo.g_strLotID);

                blnNotTally = true;
            }

            //------------------------------------Pin1--------------------------------
            bool blnPin1FailOptionMask = (bool)subKey1.GetValue("Secure-PadPin1FailOption", true).Equals("True");
            if (smVisionInfo.g_blnWantPin1)
            {
                if (blnPin1FailOptionMask != smVisionInfo.g_arrPin1[0].getWantCheckPin1(smVisionInfo.g_intSelectedTemplate))
                {
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Pad Want Check Pin 1", ((bool)subKey1.GetValue("Secure-PadPin1FailOption", true).Equals("True")).ToString(), smVisionInfo.g_arrPin1[0].getWantCheckPin1(smVisionInfo.g_intSelectedTemplate).ToString(), m_smProductionInfo.g_strLotID);

                    blnPin1NotTally = true;
                }
            }
            
        }

        private void CheckSidePadOptionSecureSetting(VisionInfo smVisionInfo, int intVisionNo, ref bool blnAllUnChecked, ref bool blnNotTally)
        {
            blnAllUnChecked = blnNotTally = false;
            //------------------------------------Pad--------------------------------
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            subKey1 = subKey.OpenSubKey(strVisionList[intVisionNo], true);

            string[] strsubkeyNames = subKey1.GetValueNames();
            bool blnFoundFailOption = false, blnFoundCheckSidePadBrokenArea = false, blnFoundCheckSidePadBrokenLength = false, blnFoundCheckSidePadExtraArea = false, blnFoundCheckSidePadExtraLength = false;
            for (int i = 0; i < strsubkeyNames.Length; i++)
            {
                if (strsubkeyNames[i].Contains("Secure-SidePadFailOptionMask"))
                    blnFoundFailOption = true;

                if (strsubkeyNames[i].Contains("Secure-SidePadFailOptionCheckSidePadBrokenArea"))
                    blnFoundCheckSidePadBrokenArea = true;

                if (strsubkeyNames[i].Contains("Secure-SidePadFailOptionCheckSidePadBrokenLength"))
                    blnFoundCheckSidePadBrokenLength = true;

                if (strsubkeyNames[i].Contains("Secure-SidePadFailOptionCheckSidePadExtraArea"))
                    blnFoundCheckSidePadExtraArea = true;

                if (strsubkeyNames[i].Contains("Secure-SidePadFailOptionCheckSidePadExtraLength"))
                    blnFoundCheckSidePadExtraLength = true;
            }
            if (!blnFoundFailOption)
                subKey1.SetValue("Secure-SidePadFailOptionMask", smVisionInfo.g_arrPad[1].ref_intFailOptionMask);
            if (!blnFoundCheckSidePadBrokenArea)
                subKey1.SetValue("Secure-SidePadFailOptionCheckSidePadBrokenArea", smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadArea);
            if (!blnFoundCheckSidePadBrokenLength)
                subKey1.SetValue("Secure-SidePadFailOptionCheckSidePadBrokenLength", smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadLength);
            if (!blnFoundCheckSidePadExtraArea)
                subKey1.SetValue("Secure-SidePadFailOptionCheckSidePadExtraArea", smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadArea);
            if (!blnFoundCheckSidePadExtraLength)
                subKey1.SetValue("Secure-SidePadFailOptionCheckSidePadExtraLength", smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadLength);

            int intSidePadFailOptionMask = Convert.ToInt32(subKey1.GetValue("Secure-SidePadFailOptionMask", 0));
            bool blnCheckSidePadBrokenArea = (bool)subKey1.GetValue("Secure-SidePadFailOptionCheckSidePadBrokenArea", true).Equals("True");
            bool blnCheckSidePadBrokenLength = (bool)subKey1.GetValue("Secure-SidePadFailOptionCheckSidePadBrokenLength", true).Equals("True");
            bool blnCheckSidePadExtraArea = (bool)subKey1.GetValue("Secure-SidePadFailOptionCheckSidePadExtraArea", true).Equals("True");
            bool blnCheckSidePadExtraLength = (bool)subKey1.GetValue("Secure-SidePadFailOptionCheckSidePadExtraLength", true).Equals("True");

            int intFailMask = smVisionInfo.g_arrPad[1].ref_intFailOptionMask;

            if (((intFailMask & 0x20) == 0) && ((intFailMask & 0xC0) == 0) && ((intFailMask & 0x100) == 0) && !smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadArea && !smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadLength
               && ((intFailMask & 0x600) == 0) && ((intFailMask & 0x800) == 0) && ((intFailMask & 0x2000) == 0) && (((intFailMask & 0x4000) == 0) && smVisionInfo.g_arrPad[1].ref_blnWantEdgeLimit_Pad)
               && (((intFailMask & 0x8000) == 0) && smVisionInfo.g_arrPad[1].ref_blnWantStandOff_Pad) && (((intFailMask & 0x10000) == 0) && smVisionInfo.g_arrPad[1].ref_blnWantEdgeDistance_Pad)
               && (((intFailMask & 0x20000) == 0 && (intFailMask & 0x40000) == 0) && smVisionInfo.g_arrPad[1].ref_blnWantSpan_Pad)
               && (((smVisionInfo.g_strVisionName != "Pad5SPkg" && smVisionInfo.g_strVisionName != "PadPkg") || !smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize) && !smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadArea && !smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadLength && ((intFailMask & 0x1000) == 0)))
                blnAllUnChecked = true;
            
            if ((intSidePadFailOptionMask != intFailMask) || (blnCheckSidePadBrokenArea != smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadArea) || (blnCheckSidePadBrokenLength != smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadLength)
                || (((smVisionInfo.g_strVisionName != "Pad5SPkg" && smVisionInfo.g_strVisionName != "PadPkg") || !smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize) && ((blnCheckSidePadExtraArea != smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadArea) || (blnCheckSidePadExtraLength != smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadLength))))
            {
                
                if (intSidePadFailOptionMask != intFailMask)
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Side Pad Fail Option", Convert.ToInt32(subKey1.GetValue("Secure-SidePadFailOptionMask", 0)).ToString(), smVisionInfo.g_arrPad[1].ref_intFailOptionMask.ToString(), m_smProductionInfo.g_strLotID);
                if (blnCheckSidePadBrokenArea != smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadArea)
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Side Pad Want Check Broken Area", ((bool)subKey1.GetValue("Secure-SidePadFailOptionCheckSidePadBrokenArea", true).Equals("True")).ToString(), smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadArea.ToString(), m_smProductionInfo.g_strLotID);
                if (blnCheckSidePadBrokenLength != smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadLength)
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Side Pad Want Check Broken Length", ((bool)subKey1.GetValue("Secure-SidePadFailOptionCheckSidePadBrokenLength", true).Equals("True")).ToString(), smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadLength.ToString(), m_smProductionInfo.g_strLotID);
                if (((smVisionInfo.g_strVisionName != "Pad5SPkg" && smVisionInfo.g_strVisionName != "PadPkg") || !smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize) && ((blnCheckSidePadExtraArea != smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadArea)))
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Side Pad Want Check Extra Area", ((bool)subKey1.GetValue("Secure-SidePadFailOptionCheckSidePadExtraArea", true).Equals("True")).ToString(), smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadArea.ToString(), m_smProductionInfo.g_strLotID);
                if (((smVisionInfo.g_strVisionName != "Pad5SPkg" && smVisionInfo.g_strVisionName != "PadPkg") || !smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize) && ((blnCheckSidePadExtraLength != smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadLength)))
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Side Pad Want Check Extra Length", ((bool)subKey1.GetValue("Secure-SidePadFailOptionCheckSidePadExtraLength", true).Equals("True")).ToString(), smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadLength.ToString(), m_smProductionInfo.g_strLotID);
                
                blnNotTally = true;
            }
        }

        private void CheckCenterPadPackageOptionSecureSetting(VisionInfo smVisionInfo, int intVisionNo, ref bool blnAllUnChecked, ref bool blnNotTally, ref bool blnParentUnChecked)
        {
            blnAllUnChecked = blnNotTally = blnParentUnChecked = false;
            //------------------------------------Pad--------------------------------
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            subKey1 = subKey.OpenSubKey(strVisionList[intVisionNo], true);

            string[] strsubkeyNames = subKey1.GetValueNames();
            bool blnFoundFailOption = false, blnFoundCheckCenterPadPackageDefect = false;
            for (int i = 0; i < strsubkeyNames.Length; i++)
            {
                if (strsubkeyNames[i].Contains("Secure-CenterPadPackageFailOptionMask"))
                    blnFoundFailOption = true;

                if (strsubkeyNames[i].Contains("Secure-CenterPadPackageFailOptionCheckPackageDefect"))
                    blnFoundCheckCenterPadPackageDefect = true;

            }
            if (!blnFoundFailOption)
                subKey1.SetValue("Secure-CenterPadPackageFailOptionMask", smVisionInfo.g_arrPad[0].ref_intFailPkgOptionMask);
            if (!blnFoundCheckCenterPadPackageDefect)
                subKey1.SetValue("Secure-CenterPadPackageFailOptionCheckPackageDefect", smVisionInfo.g_arrPad[0].GetWantInspectPackage());

            int intCenterPadPackageFailOptionMask = Convert.ToInt32(subKey1.GetValue("Secure-CenterPadPackageFailOptionMask", 0));
            //bool blnCheckCenterPadPackageSize = (bool)subKey1.GetValue("Secure-CenterPadPackageFailOptionCheckPackageSize", true).Equals("True");
            bool blnCheckCenterPadPackageDefect = (bool)subKey1.GetValue("Secure-CenterPadPackageFailOptionCheckPackageDefect", true).Equals("True");

            int intFailMask = smVisionInfo.g_arrPad[0].ref_intFailPkgOptionMask;

            if (((intFailMask & 0x01) == 0) && !smVisionInfo.g_arrPad[0].GetWantInspectPackage())
                blnParentUnChecked = true;

            if (!smVisionInfo.g_arrPad[0].GetWantInspectPackage() && ((intFailMask & 0x01) == 0) && ((intFailMask & 0x10000) == 0) && ((intFailMask & 0x20000) == 0) && ((intFailMask & 0x40000) == 0) && ((intFailMask & 0x80000) == 0)
                && (smVisionInfo.g_arrPad[0].ref_blnSeperateCrackDefectSetting && (((intFailMask & 0x800) == 0) && ((intFailMask & 0x400) == 0)))
                && (smVisionInfo.g_arrPad[0].ref_blnSeperateChippedOffDefectSetting && (((intFailMask & 0x100000) == 0) && ((intFailMask & 0x200000) == 0)))
                && (smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting && ((intFailMask & 0x80) == 0) && ((intFailMask & 0x1000000) == 0))
                && (smVisionInfo.g_arrPad[0].ref_blnSeperateForeignMaterialDefectSetting && (((intFailMask & 0x400000) == 0) && ((intFailMask & 0x800000) == 0))))
                blnAllUnChecked = true;

            if ((blnCheckCenterPadPackageDefect != smVisionInfo.g_arrPad[0].GetWantInspectPackage()) || (intCenterPadPackageFailOptionMask != intFailMask))
            {
                
                if (intCenterPadPackageFailOptionMask != intFailMask)
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Center Pad Package Fail Option", Convert.ToInt32(subKey1.GetValue("Secure-CenterPadPackageFailOptionMask", 0)).ToString(), smVisionInfo.g_arrPad[0].ref_intFailPkgOptionMask.ToString(), m_smProductionInfo.g_strLotID);
                if (blnCheckCenterPadPackageDefect != smVisionInfo.g_arrPad[0].GetWantInspectPackage())
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Center Pad Package Want Check Package Defect", ((bool)subKey1.GetValue("Secure-CenterPadPackageFailOptionCheckPackageDefect", true).Equals("True")).ToString(), smVisionInfo.g_arrPad[0].GetWantInspectPackage().ToString(), m_smProductionInfo.g_strLotID);
                
                blnNotTally = true;
            }
        }

        private void CheckSidePadPackageOptionSecureSetting(VisionInfo smVisionInfo, int intVisionNo, ref bool blnAllUnChecked, ref bool blnNotTally, ref bool blnParentUnChecked)
        {
            blnAllUnChecked = blnNotTally = blnParentUnChecked = false;
            //------------------------------------Pad--------------------------------
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            subKey1 = subKey.OpenSubKey(strVisionList[intVisionNo], true);

            string[] strsubkeyNames = subKey1.GetValueNames();
            bool blnFoundFailOption = false, blnFoundCheckSidePadPackageDefect = false;
            for (int i = 0; i < strsubkeyNames.Length; i++)
            {
                if (strsubkeyNames[i].Contains("Secure-SidePadPackageFailOptionMask"))
                    blnFoundFailOption = true;

                if (strsubkeyNames[i].Contains("Secure-SidePadPackageFailOptionCheckPackageDefect"))
                    blnFoundCheckSidePadPackageDefect = true;

            }
            if (!blnFoundFailOption)
                subKey1.SetValue("Secure-SidePadPackageFailOptionMask", smVisionInfo.g_arrPad[1].ref_intFailPkgOptionMask);
            if (!blnFoundCheckSidePadPackageDefect)
                subKey1.SetValue("Secure-SidePadPackageFailOptionCheckPackageDefect", smVisionInfo.g_arrPad[1].GetWantInspectPackage());

            int intSidePadPackageFailOptionMask = Convert.ToInt32(subKey1.GetValue("Secure-SidePadPackageFailOptionMask", 0));
            //bool blnCheckSidePadPackageSize = (bool)subKey1.GetValue("Secure-SidePadPackageFailOptionCheckPackageSize", true).Equals("True");
            bool blnCheckSidePadPackageDefect = (bool)subKey1.GetValue("Secure-SidePadPackageFailOptionCheckPackageDefect", true).Equals("True");

            int intFailMask = smVisionInfo.g_arrPad[1].ref_intFailPkgOptionMask;

            if (((intFailMask & 0x01) == 0) && !smVisionInfo.g_arrPad[1].GetWantInspectPackage())
                blnParentUnChecked = true;

            if (!smVisionInfo.g_arrPad[1].GetWantInspectPackage() && ((intFailMask & 0x01) == 0) && ((intFailMask & 0x10000) == 0) && ((intFailMask & 0x20000) == 0) && ((intFailMask & 0x40000) == 0) && ((intFailMask & 0x80000) == 0)
                && (smVisionInfo.g_arrPad[1].ref_blnSeperateCrackDefectSetting && (((intFailMask & 0x800) == 0) && ((intFailMask & 0x400) == 0)))
                && (smVisionInfo.g_arrPad[1].ref_blnSeperateChippedOffDefectSetting && (((intFailMask & 0x100000) == 0) && ((intFailMask & 0x200000) == 0)))
                && (smVisionInfo.g_arrPad[1].ref_blnSeperateMoldFlashDefectSetting && ((intFailMask & 0x80) == 0) && ((intFailMask & 0x1000000) == 0))
                  && (smVisionInfo.g_arrPad[1].ref_blnSeperateForeignMaterialDefectSetting && (((intFailMask & 0x400000) == 0) && ((intFailMask & 0x800000) == 0))))
                blnAllUnChecked = true;

            if ((blnCheckSidePadPackageDefect != smVisionInfo.g_arrPad[1].GetWantInspectPackage()) || (intSidePadPackageFailOptionMask != intFailMask))
            {
                
                if (intSidePadPackageFailOptionMask != intFailMask)
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Side Pad Package Fail Option", Convert.ToInt32(subKey1.GetValue("Secure-SidePadPackageFailOptionMask", 0)).ToString(), smVisionInfo.g_arrPad[1].ref_intFailPkgOptionMask.ToString(), m_smProductionInfo.g_strLotID);
                if (blnCheckSidePadPackageDefect != smVisionInfo.g_arrPad[1].GetWantInspectPackage())
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Side Pad Package Want Check Package Defect", ((bool)subKey1.GetValue("Secure-SidePadPackageFailOptionCheckPackageDefect", true).Equals("True")).ToString(), smVisionInfo.g_arrPad[1].GetWantInspectPackage().ToString(), m_smProductionInfo.g_strLotID);
                
                blnNotTally = true;
            }

        }

        private void CheckLead3DOptionSecureSetting(VisionInfo smVisionInfo, int intVisionNo, ref bool blnAllUnChecked, ref bool blnNotTally)
        {
            blnAllUnChecked = blnNotTally = false;
            //------------------------------------Lead3D--------------------------------
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            subKey1 = subKey.OpenSubKey(strVisionList[intVisionNo], true);

            string[] strsubkeyNames = subKey1.GetValueNames();
            bool blnFoundFailOption = false, blnFoundCheckLead3DExtraArea = false, blnFoundCheckLead3DExtraLength = false;
            for (int i = 0; i < strsubkeyNames.Length; i++)
            {
                if (strsubkeyNames[i].Contains("Secure-Lead3DFailOptionMask"))
                    blnFoundFailOption = true;

                if (strsubkeyNames[i].Contains("Secure-Lead3DFailOptionCheckLead3DExtraArea"))
                    blnFoundCheckLead3DExtraArea = true;

                if (strsubkeyNames[i].Contains("Secure-Lead3DFailOptionCheckLead3DExtraLength"))
                    blnFoundCheckLead3DExtraLength = true;
            }
            if (!blnFoundFailOption)
                subKey1.SetValue("Secure-Lead3DFailOptionMask", smVisionInfo.g_arrLead3D[0].ref_intFailOptionMask);
            if (!blnFoundCheckLead3DExtraArea)
                subKey1.SetValue("Secure-Lead3DFailOptionCheckLead3DExtraArea", smVisionInfo.g_arrLead3D[0].ref_blnWantCheckExtraLeadArea);
            if (!blnFoundCheckLead3DExtraLength)
                subKey1.SetValue("Secure-Lead3DFailOptionCheckLead3DExtraLength", smVisionInfo.g_arrLead3D[0].ref_blnWantCheckExtraLeadLength);

            int intLead3DFailOptionMask = Convert.ToInt32(subKey1.GetValue("Secure-Lead3DFailOptionMask", 0));
            bool blnCheckLead3DExtraArea = (bool)subKey1.GetValue("Secure-Lead3DFailOptionCheckLead3DExtraArea", true).Equals("True");
            bool blnCheckLead3DExtraLength = (bool)subKey1.GetValue("Secure-Lead3DFailOptionCheckLead3DExtraLength", true).Equals("True");


            int intFailMask = smVisionInfo.g_arrLead3D[0].ref_intFailOptionMask;

            if (((intFailMask & 0x40) == 0) && ((intFailMask & 0x80) == 0) && ((intFailMask & 0x800) == 0) && ((intFailMask & 0x200) == 0) && ((intFailMask & 0x2000) == 0) && ((intFailMask & 0x01) == 0)
                && ((intFailMask & 0x4000) == 0) && ((intFailMask & 0x02) == 0) && ((intFailMask & 0x1000) == 0) && ((intFailMask & 0x04) == 0) && ((intFailMask & 0x08) == 0) && ((intFailMask & 0x10) == 0)
             && !smVisionInfo.g_arrLead3D[0].ref_blnWantCheckExtraLeadArea && !smVisionInfo.g_arrLead3D[0].ref_blnWantCheckExtraLeadLength && ((intFailMask & 0x10000) == 0) && ((intFailMask & 0x100) == 0)// && !smVisionInfo.g_arrLead3D[0].ref_blnWantUsePkgToBaseTolerance) 
             && ((intFailMask & 0x20000) == 0) && (smVisionInfo.g_arrLead3D[0].ref_blnWantUseAverageGrayValueMethod && ((intFailMask & 0x40000) == 0)) && ((intFailMask & 0x100000) == 0) && ((intFailMask & 0x200000) == 0))
                blnAllUnChecked = true;

            if ((intLead3DFailOptionMask != intFailMask) || (blnCheckLead3DExtraArea != smVisionInfo.g_arrLead3D[0].ref_blnWantCheckExtraLeadArea) || (blnCheckLead3DExtraLength != smVisionInfo.g_arrLead3D[0].ref_blnWantCheckExtraLeadLength))
            {
                
                if (intLead3DFailOptionMask != intFailMask)
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Lead3D Fail Option", Convert.ToInt32(subKey1.GetValue("Secure-Lead3DFailOptionMask", 0)).ToString(), smVisionInfo.g_arrLead3D[0].ref_intFailOptionMask.ToString(), m_smProductionInfo.g_strLotID);
                if (blnCheckLead3DExtraArea != smVisionInfo.g_arrLead3D[0].ref_blnWantCheckExtraLeadArea)
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Lead3D Want Check Extra Area", ((bool)subKey1.GetValue("Secure-Lead3DFailOptionCheckLead3DExtraArea", true).Equals("True")).ToString(), smVisionInfo.g_arrLead3D[0].ref_blnWantCheckExtraLeadArea.ToString(), m_smProductionInfo.g_strLotID);
                if (blnCheckLead3DExtraLength != smVisionInfo.g_arrLead3D[0].ref_blnWantCheckExtraLeadLength)
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Lead3D Want Check Extra Length", ((bool)subKey1.GetValue("Secure-Lead3DFailOptionCheckLead3DExtraLength", true).Equals("True")).ToString(), smVisionInfo.g_arrLead3D[0].ref_blnWantCheckExtraLeadLength.ToString(), m_smProductionInfo.g_strLotID);
                
                blnNotTally = true;
            }

        }

        private void CheckLead3DPkgOptionSecureSetting(VisionInfo smVisionInfo, int intVisionNo, ref bool blnAllUnChecked, ref bool blnNotTally, ref bool blnParentUnChecked)
        {
            blnAllUnChecked = blnNotTally = blnParentUnChecked = false;
            //------------------------------------Lead3D--------------------------------
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            subKey1 = subKey.OpenSubKey(strVisionList[intVisionNo], true);

            string[] strsubkeyNames = subKey1.GetValueNames();
            bool blnFoundFailOption = false, blnFoundCheckLead3DPackageDefect = false;
            for (int i = 0; i < strsubkeyNames.Length; i++)
            {
                if (strsubkeyNames[i].Contains("Secure-Lead3DPackageFailOptionMask"))
                    blnFoundFailOption = true;

                if (strsubkeyNames[i].Contains("Secure-Lead3DPackageFailOptionCheckPackageDefect"))
                    blnFoundCheckLead3DPackageDefect = true;
            }
            if (!blnFoundFailOption)
                subKey1.SetValue("Secure-Lead3DPackageFailOptionMask", smVisionInfo.g_arrLead3D[0].ref_intFailPkgOptionMask);
            if (!blnFoundCheckLead3DPackageDefect)
                subKey1.SetValue("Secure-Lead3DPackageFailOptionCheckPackageDefect", smVisionInfo.g_arrLead3D[0].GetWantInspectPackage());

            int intLead3DPackageFailOptionMask = Convert.ToInt32(subKey1.GetValue("Secure-Lead3DPackageFailOptionMask", 0));
            //bool blnCheckLead3DPackageSize = (bool)subKey1.GetValue("Secure-Lead3DPackageFailOptionCheckPackageSize", true).Equals("True");
            bool blnCheckLead3DPackageDefect = (bool)subKey1.GetValue("Secure-Lead3DPackageFailOptionCheckPackageDefect", true).Equals("True");

            int intFailMask = smVisionInfo.g_arrLead3D[0].ref_intFailPkgOptionMask;

            if (((intFailMask & 0x01) == 0) && !smVisionInfo.g_arrLead3D[0].GetWantInspectPackage())
                blnParentUnChecked = true;

            if (!smVisionInfo.g_arrLead3D[0].GetWantInspectPackage() && ((intFailMask & 0x01) == 0) && ((intFailMask & 0x10000) == 0) && ((intFailMask & 0x20000) == 0) && ((intFailMask & 0x40000) == 0) && ((intFailMask & 0x80000) == 0)
                && (smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting && (((intFailMask & 0x800) == 0) && ((intFailMask & 0x400) == 0)))
                && (smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting && (((intFailMask & 0x100000) == 0) && ((intFailMask & 0x200000) == 0)))
                && (smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting && ((intFailMask & 0x80) == 0)))
                blnAllUnChecked = true;

            if ((blnCheckLead3DPackageDefect != smVisionInfo.g_arrLead3D[0].GetWantInspectPackage()) || (intLead3DPackageFailOptionMask != intFailMask))
            {
                
                if (intLead3DPackageFailOptionMask != intFailMask)
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Lead3D Package Fail Option", Convert.ToInt32(subKey1.GetValue("Secure-Lead3DPackageFailOptionMask", 0)).ToString(), smVisionInfo.g_arrLead3D[0].ref_intFailPkgOptionMask.ToString(), m_smProductionInfo.g_strLotID);
                if (blnCheckLead3DPackageDefect != smVisionInfo.g_arrLead3D[0].GetWantInspectPackage())
                    STDeviceEdit.SaveDeviceEditLog(smVisionInfo.g_strVisionFolderName + ">Fail Option Changes", "Lead3D Package Want Check Package Defect", ((bool)subKey1.GetValue("Secure-Lead3DPackageFailOptionCheckPackageDefect", true).Equals("True")).ToString(), smVisionInfo.g_arrLead3D[0].GetWantInspectPackage().ToString(), m_smProductionInfo.g_strLotID);
                
                blnNotTally = true;
            }
        }
        private void CheckTypo(object sender, CancelEventArgs e)
        {
            string temp = dlg_SaveImageFile.FileName.ToLower();
            m_blnchecktypo = false;

            if (temp.Contains(".jpg") || temp.Contains(".jpeg") || temp.Contains(".tif") || temp.Contains(".gif") || temp.Contains(".png"))
            {
                SRMMessageBox.Show("Image can only save as Bitmap. Please retype filename.");
                m_blnchecktypo = true;
                return;
            }
            else
                return;
        }
        private void WaitEventDone(ref bool bTriggerEvent, bool bBreakResult, int intTimeout, string strTrackName)
        {
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            while (true)
            {
                if (bTriggerEvent == bBreakResult)
                {
                    //STTrackLog.WriteLine("Time = " + timeout.Timing.ToString());
                    return;
                }

                if (timeout.Timing > intTimeout)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> Vision " + (m_intSelectedVisionStation - 1).ToString() + " time out 9 - " + strTrackName);
                    bTriggerEvent = bBreakResult;
                    break;
                }
                Thread.Sleep(1);    // 2018 10 01 - CCENG: Dun use Sleep(0) as it may cause other internal thread hang especially during waiting for grab image done. (Grab frame timeout happen)
            }

            timeout.Stop();
        }
    }
}