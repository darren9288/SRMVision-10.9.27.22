using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
using Common;
using SharedMemory;
using System.Collections.Generic;

namespace SRMVision
{
    public partial class OptionForm : Form
    {
        #region Member Variables

        private VisionInfo[] m_smVSInfo;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomInfo;

        private int intOriV1BufferSize = 0,
            intOriV2BufferSize = 0,
            intOriV3BufferSize = 0,
            intOriV4BufferSize = 0,
            intOriV5BufferSize = 0,
            intOriV6BufferSize = 0,
            intOriV7BufferSize = 0,
            intOriV8BufferSize = 0;

        #endregion

        public OptionForm(VisionInfo[] smVSInfo, ProductionInfo smProductionInfo, CustomOption smCustomInfo)
        {
            m_smVSInfo = smVSInfo;
            m_smProductionInfo = smProductionInfo;
            m_smCustomInfo = smCustomInfo;

            InitializeComponent();
            UpdateSaveImageLocationComboBox();
            UpdateHistoryDataLocation();
            FillLightSourceSeq();

            cbo_DebugImageCountToUse.SelectedIndex = m_smProductionInfo.g_intDebugImageToUse;

            srmTabControl1.TabPages.Remove(tab_LightPort);  // 2019 07 08 - CCENG: Not need to show light control setting because will use LEDi controller only.
        }
        private void UpdateHistoryDataLocation()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                {
                    cbo_HistoryDataLocation.Items.Add(drive.Name);
                }
            }

            XmlParser fileHandle = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");

            fileHandle.GetFirstSection("General");
            string strSaveImageLocation = fileHandle.GetValueAsString("SaveHistoryDataLocation", "D:\\"); // 2020-03-30 ZJYEOH : Default History Data Save Location become D:\ so that it wont occupy so many space in SVG
            if (cbo_HistoryDataLocation.Items.Contains(strSaveImageLocation))
            {
                cbo_HistoryDataLocation.SelectedIndex = cbo_HistoryDataLocation.Items.IndexOf(strSaveImageLocation);
            }
            else
            {
                cbo_HistoryDataLocation.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// If LEDi is used, enabke LEDi group else do opposite function
        /// </summary>
        private void EnableTCOSIOControl()
        {
            if (chk_LEDiControl.Checked || chk_VTControl.Checked)
            {
                group_TCOSIOControl.Enabled = false;
            }
            else
            {
                group_TCOSIOControl.Enabled = true;
            }
        }
        /// <summary>
        /// Read Option.xml file to get available General settings
        /// </summary>
        private void LoadOption()
        {
            XmlParser fileHandle = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");

            fileHandle.GetFirstSection("General");
            txt_MachineID.Text = fileHandle.GetValueAsString("MachineID", "SRM Handler");
            chk_AutoStop.Checked = fileHandle.GetValueAsBoolean("AutoStop", false);
            chk_AutoLogout.Checked = fileHandle.GetValueAsBoolean("AutoLogout", false);
            chk_WantGauge.Checked = fileHandle.GetValueAsBoolean("WantGauge", true);
            chk_ShowSubLearnButton.Checked = fileHandle.GetValueAsBoolean("ShowSubLearnButton", false);
            chk_WantUseTCPIPAsIO.Visible = chk_ConfigShowTCPIP.Checked = fileHandle.GetValueAsBoolean("ConfigShowTCPIP", false);
            chk_WantUseTCPIPAsIO.Checked = fileHandle.GetValueAsBoolean("WantUseTCPIPAsIO", false);
            chk_ConfigShowRS232.Checked = fileHandle.GetValueAsBoolean("ConfigShowRS232", false);
            chk_ConfigShowNetwork.Checked = fileHandle.GetValueAsBoolean("ConfigShowNetwork", false);
            chk_ShareHandlerPC.Checked = fileHandle.GetValueAsBoolean("ShareHandlerPC", false);
            chk_UseUSBIOCard.Checked = fileHandle.GetValueAsBoolean("UseUSBIOCard", false);
            txt_MarkDefaultTolerance.Text = fileHandle.GetValueAsString("MarkDefaultTolerance", "75");
            txt_Width.Text = fileHandle.GetValueAsString("ResolutionWidth", "640");
            txt_Height.Text = fileHandle.GetValueAsString("ResolutionHeight", "480");
            chk_WantBypassIPM.Checked = fileHandle.GetValueAsBoolean("WantBuypassIPM", false);
            chk_WantNonRotateInspection.Checked = fileHandle.GetValueAsBoolean("WantNonRotateInspection", false);
            chk_WantEditLog.Checked = fileHandle.GetValueAsBoolean("WantEditLog", true);
            chk_BlankImageBeforeGrab.Checked = fileHandle.GetValueAsBoolean("BlankImageBeforeGrab", false);
            chk_DeleteImageDuringProduction.Checked = fileHandle.GetValueAsBoolean("DeleteImageDuringProduction", false);
            chk_ShowGUIForBelowUserRight.Checked = fileHandle.GetValueAsBoolean("ShowGUIForBelowUserRight", false);
            chk_WantRecipeVerification.Checked = fileHandle.GetValueAsBoolean("WantRecipeVerification", false);
            chk_WantShowWhiteBalance.Checked = fileHandle.GetValueAsBoolean("WantShowWhiteBalance", false);
            cbo_OrientIO.SelectedIndex = fileHandle.GetValueAsInt("OrientIO", 0);

            string strSaveImageLocation = fileHandle.GetValueAsString("V1SaveImageLocation", "D:\\");
            if (cbo_SaveImageLocation.Items.Contains(strSaveImageLocation))
            {
                cbo_SaveImageLocation.SelectedIndex = cbo_SaveImageLocation.Items.IndexOf(strSaveImageLocation);
            }
            else
            {
                if (cbo_SaveImageLocation.Items.Count != 0)
                    cbo_SaveImageLocation.SelectedIndex = 1;
            }

            string strV2SaveImageLocation = fileHandle.GetValueAsString("V2SaveImageLocation", "D:\\");
            if (cbo_V2SaveImageLocation.Items.Contains(strV2SaveImageLocation))
            {
                cbo_V2SaveImageLocation.SelectedIndex = cbo_V2SaveImageLocation.Items.IndexOf(strV2SaveImageLocation);
            }
            else
            {
                if (cbo_V2SaveImageLocation.Items.Count != 0)
                    cbo_V2SaveImageLocation.SelectedIndex = 1;
            }

            string strV3SaveImageLocation = fileHandle.GetValueAsString("V3SaveImageLocation", "D:\\");
            if (cbo_V3SaveImageLocation.Items.Contains(strV3SaveImageLocation))
            {
                cbo_V3SaveImageLocation.SelectedIndex = cbo_V3SaveImageLocation.Items.IndexOf(strV3SaveImageLocation);
            }
            else
            {
                if (cbo_V3SaveImageLocation.Items.Count != 0)
                    cbo_V3SaveImageLocation.SelectedIndex = 1;
            }

            string strV4SaveImageLocation = fileHandle.GetValueAsString("V4SaveImageLocation", "D:\\");
            if (cbo_V4SaveImageLocation.Items.Contains(strV4SaveImageLocation))
            {
                cbo_V4SaveImageLocation.SelectedIndex = cbo_V4SaveImageLocation.Items.IndexOf(strV4SaveImageLocation);
            }
            else
            {
                if (cbo_V4SaveImageLocation.Items.Count != 0)
                    cbo_V4SaveImageLocation.SelectedIndex = 1;
            }


            string strV5SaveImageLocation = fileHandle.GetValueAsString("V5SaveImageLocation", "D:\\");
            if (cbo_V5SaveImageLocation.Items.Contains(strV5SaveImageLocation))
            {
                cbo_V5SaveImageLocation.SelectedIndex = cbo_V5SaveImageLocation.Items.IndexOf(strV5SaveImageLocation);
            }
            else
            {
                if (cbo_V5SaveImageLocation.Items.Count != 0)
                    cbo_V5SaveImageLocation.SelectedIndex = 1;
            }

            string strV6SaveImageLocation = fileHandle.GetValueAsString("V6SaveImageLocation", "D:\\");
            if (cbo_V6SaveImageLocation.Items.Contains(strV6SaveImageLocation))
            {
                cbo_V6SaveImageLocation.SelectedIndex = cbo_V6SaveImageLocation.Items.IndexOf(strV6SaveImageLocation);
            }
            else
            {
                if (cbo_V6SaveImageLocation.Items.Count != 0)
                    cbo_V6SaveImageLocation.SelectedIndex = 1;
            }

            string strV7SaveImageLocation = fileHandle.GetValueAsString("V7SaveImageLocation", "D:\\");
            if (cbo_V7SaveImageLocation.Items.Contains(strV7SaveImageLocation))
            {
                cbo_V7SaveImageLocation.SelectedIndex = cbo_V7SaveImageLocation.Items.IndexOf(strV7SaveImageLocation);
            }
            else
            {
                if (cbo_V7SaveImageLocation.Items.Count != 0)
                    cbo_V7SaveImageLocation.SelectedIndex = 1;
            }

            string strV8SaveImageLocation = fileHandle.GetValueAsString("V8SaveImageLocation", "D:\\");
            if (cbo_V8SaveImageLocation.Items.Contains(strV8SaveImageLocation))
            {
                cbo_V8SaveImageLocation.SelectedIndex = cbo_V8SaveImageLocation.Items.IndexOf(strV8SaveImageLocation);
            }
            else
            {
                if (cbo_V8SaveImageLocation.Items.Count != 0)
                    cbo_V8SaveImageLocation.SelectedIndex = 1;
            }

            intOriV1BufferSize = fileHandle.GetValueAsInt("V1SaveImageBufferSize", 20);
            txt_SaveImageBufferSize.Text = intOriV1BufferSize.ToString();

            intOriV2BufferSize = fileHandle.GetValueAsInt("V2SaveImageBufferSize", 20);
            txt_V2SaveImageBufferSize.Text = intOriV2BufferSize.ToString();

            intOriV3BufferSize = fileHandle.GetValueAsInt("V3SaveImageBufferSize", 20);
            txt_V3SaveImageBufferSize.Text = intOriV3BufferSize.ToString();

            intOriV4BufferSize = fileHandle.GetValueAsInt("V4SaveImageBufferSize", 20);
            txt_V4SaveImageBufferSize.Text = intOriV4BufferSize.ToString();

            intOriV5BufferSize = fileHandle.GetValueAsInt("V5SaveImageBufferSize", 20);
            txt_V5SaveImageBufferSize.Text = intOriV5BufferSize.ToString();

            intOriV6BufferSize = fileHandle.GetValueAsInt("V6SaveImageBufferSize", 20);
            txt_V6SaveImageBufferSize.Text = intOriV6BufferSize.ToString();

            intOriV7BufferSize = fileHandle.GetValueAsInt("V7SaveImageBufferSize", 20);
            txt_V7SaveImageBufferSize.Text = intOriV7BufferSize.ToString();

            intOriV8BufferSize = fileHandle.GetValueAsInt("V8SaveImageBufferSize", 20);
            txt_V8SaveImageBufferSize.Text = intOriV8BufferSize.ToString();

            chk_MixController.Checked = fileHandle.GetValueAsBoolean("MixController", false);
            chk_OnlyNewLot.Checked = fileHandle.GetValueAsBoolean("OnlyNewLot", false);

            EnableTCOSIOControl();

            fileHandle.GetFirstSection("LightSource");
            txt_ReplyTimeOut.Text = fileHandle.GetValueAsString("RS232TimeOut", "100");
            cbo_BitsPerSecond.SelectedIndex = cbo_BitsPerSecond.FindStringExact(fileHandle.GetValueAsString("BitsPerSecond", "115200"));
            cbo_DataBits.SelectedIndex = cbo_DataBits.FindStringExact(fileHandle.GetValueAsString("DataBits", "7"));
            cbo_Parity.SelectedIndex = fileHandle.GetValueAsInt("Parity", 0);
            cbo_StopBits.SelectedIndex = fileHandle.GetValueAsInt("StopBits", 0);
            chk_LEDiControl.Checked = fileHandle.GetValueAsBoolean("LEDiControl", true);
            chk_VTControl.Checked = fileHandle.GetValueAsBoolean("VTControl", false);
            txt_MaximumLimit.Text = fileHandle.GetValueAsString("LEDiMaxLimit", "31");

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            for (int i = 0; i < strVisionList.Length; i++)
            {
                subKey1 = subKey.OpenSubKey(strVisionList[i]);

                string strTriggerMode;
                if (subKey1.GetValue("TriggerMode", "2").ToString() == "1")
                    strTriggerMode = "Integration Enable Inverted Yes";
                else if (subKey1.GetValue("TriggerMode", "2").ToString() == "2")
                    strTriggerMode = "Integration Enable Inverted No";
                else
                    strTriggerMode = "Trigger Off";

                string strLightControllerType;
                if (subKey1.GetValue("LightControllerType", "1").ToString() == "2")
                    strLightControllerType = "Sequential";
                else
                    strLightControllerType = "Normal";

                string strRotateFlip;
                if (subKey1.GetValue("RotateFlip", "0").ToString() == "1")
                    strRotateFlip = "Rotate 180";
                else if (subKey1.GetValue("RotateFlip", "0").ToString() == "2")
                    strRotateFlip = "Rotate 90";
                else if (subKey1.GetValue("RotateFlip", "0").ToString() == "3")
                    strRotateFlip = "Rotate -90";
                else if (subKey1.GetValue("RotateFlip", "0").ToString() == "4")
                    strRotateFlip = "Flip Horizontal";
                else if (subKey1.GetValue("RotateFlip", "0").ToString() == "5")
                    strRotateFlip = "Flip Vertical";
                else
                    strRotateFlip = "Original";

                string m_Color = "";

                if (Int16.Parse(subKey1.GetValue("ColorCamera", "0").ToString()) == 1)
                    m_Color = "Color";
                else
                    m_Color = "Mono";

                string strVisionName = subKey1.GetValue("VisionName", strVisionList[i]).ToString();
                if (subKey1.GetValue("VisionNameNo", "").ToString() == "2")
                    strVisionName += "2";

                string strGrabMode = "";

                if (Int16.Parse(subKey1.GetValue("GrabMode", "0").ToString()) == 0)
                    strGrabMode = "Normal";
                else if (Int16.Parse(subKey1.GetValue("GrabMode", "0").ToString()) == 1)
                    strGrabMode = "Grab All First";
                else
                    strGrabMode = "High Speed";

                dgd_VisionList.Rows.Add(subKey1.GetValue("VisionName", strVisionList[i]).ToString(),
                        subKey1.GetValue("CameraModel", "AVT").ToString(),
                        subKey1.GetValue("ImageUnits", "1").ToString(),
                        strTriggerMode,
                        subKey1.GetValue("Resolution", "640x480").ToString(),
                        strLightControllerType, "", "", strRotateFlip, subKey1.GetValue("CameraModelNo", "").ToString(), m_Color, strGrabMode);

                DataGridViewComboBoxCell dgvComboBox = new DataGridViewComboBoxCell();
                switch (subKey1.GetValue("VisionName", strVisionList[i]).ToString())
                {
                    case "Li3D":
                    case "Li3DPkg":
                        {
                            dgvComboBox.Items.Add("NoMerge");
                            dgvComboBox.Items.Add("MergeGrab1&2");
                            dgvComboBox.Items.Add("MergeAll");

                            string strImageMergeType;
                            if (subKey1.GetValue("ImageMerge", "0").ToString() == "0")
                                strImageMergeType = "NoMerge";
                            else if (subKey1.GetValue("ImageMerge", "0").ToString() == "1")
                                strImageMergeType = "MergeGrab1&2";
                            else
                                strImageMergeType = "MergeAll";

                            dgvComboBox.Value = strImageMergeType;
                            dgd_VisionList.Rows[i].Cells[6] = dgvComboBox;

                            dgvComboBox = new DataGridViewComboBoxCell();
                            dgvComboBox.Items.Add("Standard");
                            dgvComboBox.Items.Add("Pad");
                            dgvComboBox.Items.Add("Lead");
                            if (strImageMergeType == "MergeAll")
                            {
                                string strImageDisplayMode;
                                if (subKey1.GetValue("ImageDisplayMode", "0").ToString() == "0")
                                    strImageDisplayMode = "Standard";
                                else if (subKey1.GetValue("ImageDisplayMode", "0").ToString() == "1")
                                    strImageDisplayMode = "Pad";
                                else if (subKey1.GetValue("ImageDisplayMode", "0").ToString() == "2")
                                    strImageDisplayMode = "Lead";
                                else
                                    strImageDisplayMode = "Standard";
                                dgvComboBox.Value = strImageDisplayMode;

                            }
                            else
                            {
                                dgvComboBox.Value = "Standard";
                            }
                            dgd_VisionList.Rows[i].Cells[7] = dgvComboBox;
                        }
                        break;
                    case "Pad5S":
                    case "Pad5SPkg":
                        {
                            dgvComboBox.Items.Add("NoMerge");
                            dgvComboBox.Items.Add("MergeGrab1&2");
                            dgvComboBox.Items.Add("MergeAll");
                            dgvComboBox.Items.Add("MergeGrab1&2,Grab3&4");
                            dgvComboBox.Items.Add("MergeGrab1&2&3,Grab4&5");

                            string strImageMergeType;
                            if (subKey1.GetValue("ImageMerge", "0").ToString() == "0")
                                strImageMergeType = "NoMerge";
                            else if (subKey1.GetValue("ImageMerge", "0").ToString() == "1")
                                strImageMergeType = "MergeGrab1&2";
                            else if (subKey1.GetValue("ImageMerge", "0").ToString() == "2")
                                strImageMergeType = "MergeAll";
                            else if (subKey1.GetValue("ImageMerge", "0").ToString() == "3")
                                strImageMergeType = "MergeGrab1&2,Grab3&4";
                            else
                                strImageMergeType = "MergeGrab1&2&3,Grab4&5";

                            dgvComboBox.Value = strImageMergeType;
                            dgd_VisionList.Rows[i].Cells[6] = dgvComboBox;

                            dgvComboBox = new DataGridViewComboBoxCell();
                            dgvComboBox.Items.Add("Standard");
                            dgvComboBox.Items.Add("Pad");
                            dgvComboBox.Items.Add("Lead");
                            if (strImageMergeType == "MergeAll" || strImageMergeType == "MergeGrab1&2&3,Grab4&5")
                            {
                                string strImageDisplayMode;
                                if (subKey1.GetValue("ImageDisplayMode", "0").ToString() == "0")
                                    strImageDisplayMode = "Standard";
                                else if (subKey1.GetValue("ImageDisplayMode", "0").ToString() == "1")
                                    strImageDisplayMode = "Pad";
                                else if (subKey1.GetValue("ImageDisplayMode", "0").ToString() == "2")
                                    strImageDisplayMode = "Lead";
                                else
                                    strImageDisplayMode = "Standard";
                                dgvComboBox.Value = strImageDisplayMode;

                            }
                            else
                            {
                                dgvComboBox.Value = "Standard";
                            }
                            dgd_VisionList.Rows[i].Cells[7] = dgvComboBox;
                        }
                        break;
                    default:
                        dgvComboBox.Items.Add("NoMerge");
                        dgvComboBox.Value = dgvComboBox.Items[0];
                        dgd_VisionList.Rows[i].Cells[6] = dgvComboBox;

                        dgvComboBox = new DataGridViewComboBoxCell();
                        dgvComboBox.Items.Add("Standard");
                        dgvComboBox.Value = dgvComboBox.Items[0];
                        dgd_VisionList.Rows[i].Cells[7] = dgvComboBox;
                        break;
                }
            }

            if (m_smVSInfo[0] != null)
            {
                srmCheckBox1.Checked = m_smVSInfo[0].g_blnDebugRUN;
                srmInputBox1.Text = m_smVSInfo[0].g_intSleep.ToString();
            }

            chk_DebugRunFromCenter.Checked = m_smProductionInfo.g_blnAllRunFromCenter;
            chk_DebugRunWithoutGrabImage.Checked = m_smProductionInfo.g_blnAllRunWithoutGrabImage;
            chk_DebugRunGrabWithoutUseImage.Checked = m_smProductionInfo.g_blnAllRunGrabWithoutUseImage;
        }

        private void FillLightSourceSeq()
        {
            dgd_Sequence.Rows.Clear();
            for (int x = 0; x < m_smVSInfo.Length; x++)
            {
                //if (m_smVSInfo[x] == null || m_smVSInfo[x].g_arrLightSource.Count < 2)
                if (m_smVSInfo[x] == null)
                    continue;

                for (int i = 0; i < m_smVSInfo[x].g_arrLightSource.Count; i++)
                {
                    string strVisionName = m_smVSInfo[x].g_strVisionName.Replace(" ", "") + m_smVSInfo[x].g_strVisionNameNo;

                    LightSource objLight = m_smVSInfo[x].g_arrLightSource[i];
                    //dgd_Sequence.Rows.Add(m_smVSInfo[x].g_strVisionName.Replace(" ", ""), objLight.ref_strType, objLight.ref_intSeqNo);
                    dgd_Sequence.Rows.Add(strVisionName, objLight.ref_strType, objLight.ref_intSeqNo);
                }
            }
        }

        private void UpdateSaveImageLocationComboBox()
        {
            int intCount = 0;
            int intVisionQty = 0;

            DriveInfo[] driveInfo = DriveInfo.GetDrives();
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (intCount == 0)
                {
                    foreach (DriveInfo drive in driveInfo)
                    {
                        if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                        {
                            cbo_SaveImageLocation.Items.Add(drive.Name);
                        }
                    }

                    intCount++;

                    if (m_smVSInfo[i] == null)
                        continue;

                    lbl_SaveImageLocation.Visible = true;
                    //lbl_SaveImageLocation.Text = m_smVSInfo[i].g_strVisionName + " Save Image Location :";
                    cbo_SaveImageLocation.Visible = true;
                    lbl_SaveImageBufferSize.Visible = true;
                    txt_SaveImageBufferSize.Visible = true;

                    intVisionQty++;
                }
                else if (intCount == 1)
                {
                    foreach (DriveInfo drive in driveInfo)
                    {
                        if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                        {
                            cbo_V2SaveImageLocation.Items.Add(drive.Name);
                        }
                    }
                    intCount++;

                    if (m_smVSInfo[i] == null)
                        continue;

                    lbl_V2SaveImageLocation.Visible = true;
                    cbo_V2SaveImageLocation.Visible = true;
                    lbl_V2SaveImageBufferSize.Visible = true;
                    txt_V2SaveImageBufferSize.Visible = true;

                    intVisionQty++;
                }
                else if (intCount == 2)
                {
                    foreach (DriveInfo drive in driveInfo)
                    {
                        if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                        {
                            cbo_V3SaveImageLocation.Items.Add(drive.Name);
                        }
                    }
                    intCount++;

                    if (m_smVSInfo[i] == null)
                        continue;

                    lbl_V3SaveImageLocation.Visible = true;
                    cbo_V3SaveImageLocation.Visible = true;
                    lbl_V3SaveImageBufferSize.Visible = true;
                    txt_V3SaveImageBufferSize.Visible = true;

                    intVisionQty++;
                }
                else if (intCount == 3)
                {
                    foreach (DriveInfo drive in driveInfo)
                    {
                        if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                        {
                            cbo_V4SaveImageLocation.Items.Add(drive.Name);
                        }
                    }
                    intCount++;

                    if (m_smVSInfo[i] == null)
                        continue;

                    lbl_V4SaveImageLocation.Visible = true;
                    cbo_V4SaveImageLocation.Visible = true;
                    lbl_V4SaveImageBufferSize.Visible = true;
                    txt_V4SaveImageBufferSize.Visible = true;

                    intVisionQty++;
                }
                else if (intCount == 4)
                {
                    foreach (DriveInfo drive in driveInfo)
                    {
                        if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                        {
                            cbo_V5SaveImageLocation.Items.Add(drive.Name);
                        }
                    }
                    intCount++;

                    if (m_smVSInfo[i] == null)
                        continue;

                    lbl_V5SaveImageLocation.Visible = true;
                    cbo_V5SaveImageLocation.Visible = true;
                    lbl_V5SaveImageBufferSize.Visible = true;
                    txt_V5SaveImageBufferSize.Visible = true;

                    intVisionQty++;
                }
                else if (intCount == 5)
                {
                    foreach (DriveInfo drive in driveInfo)
                    {
                        if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                        {
                            cbo_V6SaveImageLocation.Items.Add(drive.Name);
                        }
                    }
                    intCount++;

                    if (m_smVSInfo[i] == null)
                        continue;

                    lbl_V6SaveImageLocation.Visible = true;
                    cbo_V6SaveImageLocation.Visible = true;
                    lbl_V6SaveImageBufferSize.Visible = true;
                    txt_V6SaveImageBufferSize.Visible = true;

                    intVisionQty++;
                }
                else if (intCount == 6)
                {
                    foreach (DriveInfo drive in driveInfo)
                    {
                        if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                        {
                            cbo_V7SaveImageLocation.Items.Add(drive.Name);
                        }
                    }
                    intCount++;

                    if (m_smVSInfo[i] == null)
                        continue;

                    lbl_V7SaveImageLocation.Visible = true;
                    cbo_V7SaveImageLocation.Visible = true;
                    lbl_V7SaveImageBufferSize.Visible = true;
                    txt_V7SaveImageBufferSize.Visible = true;

                    intVisionQty++;
                }
                else if (intCount == 7)
                {
                    foreach (DriveInfo drive in driveInfo)
                    {
                        if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                        {
                            cbo_V8SaveImageLocation.Items.Add(drive.Name);
                        }
                    }
                    if (m_smVSInfo[i] == null)
                        continue;

                    lbl_V8SaveImageLocation.Visible = true;
                    cbo_V8SaveImageLocation.Visible = true;
                    lbl_V8SaveImageBufferSize.Visible = true;
                    txt_V8SaveImageBufferSize.Visible = true;

                    intVisionQty++;
                }
            }
        }

        /// <summary>
        /// Write all settings into Option.xml
        /// </summary>
        private void WriteOption()
        {
            m_smProductionInfo.g_strHistoryDataLocation = cbo_HistoryDataLocation.SelectedItem.ToString();
            string path = AppDomain.CurrentDomain.BaseDirectory + "Option.xml";
            
            STDeviceEdit.CopySettingFile(AppDomain.CurrentDomain.BaseDirectory, "Option.xml");
            
            XmlParser fileHandle = new XmlParser(path);

            fileHandle.WriteSectionElement("General");
            fileHandle.WriteElement1Value("MachineID", txt_MachineID.Text);
            fileHandle.WriteElement1Value("AutoStop", chk_AutoStop.Checked);
            fileHandle.WriteElement1Value("AutoLogout", chk_AutoLogout.Checked);
            fileHandle.WriteElement1Value("WantGauge", chk_WantGauge.Checked);
            fileHandle.WriteElement1Value("ShareHandlerPC", chk_ShareHandlerPC.Checked);
            fileHandle.WriteElement1Value("ShowSubLearnButton", chk_ShowSubLearnButton.Checked);
            fileHandle.WriteElement1Value("ConfigShowTCPIP", chk_ConfigShowTCPIP.Checked);
            fileHandle.WriteElement1Value("WantUseTCPIPAsIO", chk_WantUseTCPIPAsIO.Checked); 
            fileHandle.WriteElement1Value("ConfigShowRS232", chk_ConfigShowRS232.Checked);
            fileHandle.WriteElement1Value("ConfigShowNetwork", chk_ConfigShowNetwork.Checked);
            fileHandle.WriteElement1Value("UseUSBIOCard", chk_UseUSBIOCard.Checked);
            fileHandle.WriteElement1Value("MarkDefaultTolerance", txt_MarkDefaultTolerance.Text);
            fileHandle.WriteElement1Value("ResolutionWidth", txt_Width.Text);
            fileHandle.WriteElement1Value("ResolutionHeight", txt_Height.Text);
            fileHandle.WriteElement1Value("SaveHistoryDataLocation", cbo_HistoryDataLocation.SelectedItem.ToString());
            fileHandle.WriteElement1Value("OrientIO", cbo_OrientIO.SelectedIndex.ToString());
            fileHandle.WriteElement1Value("V1SaveImageLocation", cbo_SaveImageLocation.SelectedItem.ToString());
            fileHandle.WriteElement1Value("V2SaveImageLocation", cbo_V2SaveImageLocation.SelectedItem.ToString());
            fileHandle.WriteElement1Value("V3SaveImageLocation", cbo_V3SaveImageLocation.SelectedItem.ToString());
            fileHandle.WriteElement1Value("V4SaveImageLocation", cbo_V4SaveImageLocation.SelectedItem.ToString());
            fileHandle.WriteElement1Value("V5SaveImageLocation", cbo_V5SaveImageLocation.SelectedItem.ToString());
            fileHandle.WriteElement1Value("V6SaveImageLocation", cbo_V6SaveImageLocation.SelectedItem.ToString());
            fileHandle.WriteElement1Value("V7SaveImageLocation", cbo_V7SaveImageLocation.SelectedItem.ToString());
            fileHandle.WriteElement1Value("V8SaveImageLocation", cbo_V8SaveImageLocation.SelectedItem.ToString());            
            fileHandle.WriteElement1Value("V1SaveImageBufferSize", txt_SaveImageBufferSize.Text.ToString());
            fileHandle.WriteElement1Value("V2SaveImageBufferSize", txt_V2SaveImageBufferSize.Text.ToString());
            fileHandle.WriteElement1Value("V3SaveImageBufferSize", txt_V3SaveImageBufferSize.Text.ToString());
            fileHandle.WriteElement1Value("V4SaveImageBufferSize", txt_V4SaveImageBufferSize.Text.ToString());
            fileHandle.WriteElement1Value("V5SaveImageBufferSize", txt_V5SaveImageBufferSize.Text.ToString());
            fileHandle.WriteElement1Value("V6SaveImageBufferSize", txt_V6SaveImageBufferSize.Text.ToString());
            fileHandle.WriteElement1Value("V7SaveImageBufferSize", txt_V7SaveImageBufferSize.Text.ToString());
            fileHandle.WriteElement1Value("V8SaveImageBufferSize", txt_V8SaveImageBufferSize.Text.ToString());
            fileHandle.WriteElement1Value("MixController", chk_MixController.Checked);
            fileHandle.WriteElement1Value("OnlyNewLot", chk_OnlyNewLot.Checked);
            fileHandle.WriteElement1Value("WantBuypassIPM", chk_WantBypassIPM.Checked);
            fileHandle.WriteElement1Value("WantNonRotateInspection", chk_WantNonRotateInspection.Checked);
            fileHandle.WriteElement1Value("WantEditLog", chk_WantEditLog.Checked);
            fileHandle.WriteElement1Value("BlankImageBeforeGrab", chk_BlankImageBeforeGrab.Checked);
            fileHandle.WriteElement1Value("DeleteImageDuringProduction", chk_DeleteImageDuringProduction.Checked);
            fileHandle.WriteElement1Value("ShowGUIForBelowUserRight", chk_ShowGUIForBelowUserRight.Checked);
            fileHandle.WriteElement1Value("WantRecipeVerification", chk_WantRecipeVerification.Checked);
            fileHandle.WriteElement1Value("WantShowWhiteBalance", chk_WantShowWhiteBalance.Checked);
            fileHandle.WriteEndElement();
            STDeviceEdit.XMLChangesTracing("General", m_smProductionInfo.g_strLotID);
            STDeviceEdit.CopySettingFile(AppDomain.CurrentDomain.BaseDirectory, "Option.xml");
            fileHandle.WriteSectionElement("LightSource");
            fileHandle.WriteElement1Value("RS232TimeOut", txt_ReplyTimeOut.Text);
            fileHandle.WriteElement1Value("BitsPerSecond", cbo_BitsPerSecond.SelectedItem.ToString());
            fileHandle.WriteElement1Value("DataBits", cbo_DataBits.SelectedItem.ToString());
            fileHandle.WriteElement1Value("Parity", cbo_Parity.SelectedIndex);
            fileHandle.WriteElement1Value("StopBits", cbo_StopBits.SelectedIndex);
            fileHandle.WriteElement1Value("LEDiControl", chk_LEDiControl.Checked);
            fileHandle.WriteElement1Value("VTControl", chk_VTControl.Checked);
            fileHandle.WriteElement1Value("LEDiMaxLimit", txt_MaximumLimit.Text);

            fileHandle.WriteEndElement();
            
            STDeviceEdit.XMLChangesTracing("LightSource", m_smProductionInfo.g_strLotID);

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            string m_CameraModel, m_ImageUnits, m_TriggerMode, m_Resolution, m_LightControllerType, m_ImageMerge, m_ImageDisplayMode, m_CameraModelNo, m_Color;
            for (int i = 0; i < strVisionList.Length; i++)
            {
                subKey1 = subKey.OpenSubKey(strVisionList[i], true);
                m_CameraModel = subKey1.GetValue("CameraModel","").ToString();
                m_ImageUnits = subKey1.GetValue("ImageUnits", "").ToString();
                m_CameraModelNo = subKey1.GetValue("CameraModelNo", "").ToString();
                subKey1.SetValue("CameraModel", dgd_VisionList.Rows[i].Cells[1].Value);
                subKey1.SetValue("ImageUnits", Convert.ToInt32(dgd_VisionList.Rows[i].Cells[2].Value));
                int intTriggerModdeIndex;
                switch (dgd_VisionList.Rows[i].Cells[3].Value.ToString())
                {
                    default:
                    case "Trigger Off":
                        intTriggerModdeIndex = 0;
                        break;
                    case "Integration Enable Inverted Yes":
                        intTriggerModdeIndex = 1;
                        break;
                    case "Integration Enable Inverted No":
                        intTriggerModdeIndex = 2;
                        break;
                }
                m_TriggerMode = subKey1.GetValue("TriggerMode", "").ToString();
                m_Resolution = subKey1.GetValue("Resolution", "").ToString();
                subKey1.SetValue("TriggerMode", intTriggerModdeIndex);
                subKey1.SetValue("Resolution", dgd_VisionList.Rows[i].Cells[4].Value);

                string[] strCameraResolution = dgd_VisionList.Rows[i].Cells[4].Value.ToString().Split('x');
                if (strCameraResolution.Length == 2)
                {
                    for (int x = 0; x < m_smVSInfo.Length; x++)
                    {
                        if (m_smVSInfo[x] == null)
                            continue;

                        if (m_smVSInfo[x].g_strVisionFolderName == strVisionList[i])
                        {
                            m_smVSInfo[x].g_intCameraResolutionWidth = Convert.ToInt32(strCameraResolution[0]);
                            m_smVSInfo[x].g_intCameraResolutionHeight = Convert.ToInt32(strCameraResolution[1]);
                        }
                    }
                }

                int intLightControllerType;
                switch (dgd_VisionList.Rows[i].Cells[5].Value.ToString())
                {
                    default:
                    case "Normal":
                        intLightControllerType = 1;
                        break;
                    case "Sequential":
                        intLightControllerType = 2;
                        break;
                }
                m_LightControllerType = subKey1.GetValue("LightControllerType", "").ToString();
                subKey1.SetValue("LightControllerType", intLightControllerType);

                int intImageMergeType;
                if (dgd_VisionList.Rows[i].Cells[6].Value.ToString() == "NoMerge")
                    intImageMergeType = 0;
                else if (dgd_VisionList.Rows[i].Cells[6].Value.ToString() == "MergeGrab1&2")
                    intImageMergeType = 1;
                else if (dgd_VisionList.Rows[i].Cells[6].Value.ToString() == "MergeAll")
                    intImageMergeType = 2;
                else if (dgd_VisionList.Rows[i].Cells[6].Value.ToString() == "MergeGrab1&2,Grab3&4")
                    intImageMergeType = 3;
                else
                    intImageMergeType = 4;

                m_ImageMerge = subKey1.GetValue("ImageMerge", "").ToString();
                subKey1.SetValue("ImageMerge", intImageMergeType);

                int intImageDisplayMode;
                if (dgd_VisionList.Rows[i].Cells[7].Value.ToString() == "Standard")
                    intImageDisplayMode = 0;
                else if (dgd_VisionList.Rows[i].Cells[7].Value.ToString() == "Pad")
                    intImageDisplayMode = 1;
                else if (dgd_VisionList.Rows[i].Cells[7].Value.ToString() == "Lead")
                    intImageDisplayMode = 2;
                else
                    intImageDisplayMode = 0;
                m_ImageDisplayMode = subKey1.GetValue("ImageDisplayMode", "").ToString();
                subKey1.SetValue("ImageDisplayMode", intImageDisplayMode);

                int intRotateFlip = 0;
                if (dgd_VisionList.Rows[i].Cells[8].Value.ToString() == "Rotate 180")
                    intRotateFlip = 1;
                else if (dgd_VisionList.Rows[i].Cells[8].Value.ToString() == "Rotate 90")
                    intRotateFlip = 2;
                else if (dgd_VisionList.Rows[i].Cells[8].Value.ToString() == "Rotate -90")
                    intRotateFlip = 3;
                else if (dgd_VisionList.Rows[i].Cells[8].Value.ToString() == "Flip Horizontal")
                    intRotateFlip = 4;
                else if (dgd_VisionList.Rows[i].Cells[8].Value.ToString() == "Flip Vertical")
                    intRotateFlip = 5;

                subKey1.SetValue("RotateFlip", intRotateFlip);
                subKey1.SetValue("CameraModelNo", dgd_VisionList.Rows[i].Cells[9].Value);

                if (dgd_VisionList.Rows[i].Cells[10].Value.ToString().Equals("Color"))
                    m_Color = 1.ToString();
                else
                    m_Color = 0.ToString();

                if (m_Color.Equals("1"))
                    subKey1.SetValue("ColorCamera", m_Color);
                else
                {
                    if (subKey1.GetValue("ColorCamera") != null)
                        subKey1.DeleteValue("ColorCamera");
                }

                string strGrabMode;
                if (dgd_VisionList.Rows[i].Cells[11].Value.ToString().Equals("Normal"))
                    strGrabMode = 0.ToString();
                else if (dgd_VisionList.Rows[i].Cells[11].Value.ToString().Equals("Grab All First"))
                    strGrabMode = 1.ToString();
                else
                    strGrabMode = 2.ToString();

                subKey1.SetValue("GrabMode", strGrabMode);
              
                if (m_ImageMerge != subKey1.GetValue("ImageMerge", "").ToString())
                STDeviceEdit.SaveDeviceEditLog("Camera", "ImageMerge value changed", m_ImageMerge, subKey1.GetValue("ImageMerge", "").ToString(), m_smProductionInfo.g_strLotID);
                if (m_ImageDisplayMode != subKey1.GetValue("ImageDisplayMode", "").ToString())
                    STDeviceEdit.SaveDeviceEditLog("Camera", "ImageDisplayMode value changed", m_ImageDisplayMode, subKey1.GetValue("ImageDisplayMode", "").ToString(), m_smProductionInfo.g_strLotID);
                if (m_CameraModel != subKey1.GetValue("CameraModel", "").ToString())
                    STDeviceEdit.SaveDeviceEditLog("Camera", "Camera Model changed", m_CameraModel, subKey1.GetValue("CameraModel", "").ToString(), m_smProductionInfo.g_strLotID);
                if (m_ImageUnits != subKey1.GetValue("ImageUnits", "").ToString())
                    STDeviceEdit.SaveDeviceEditLog("Camera", "ImageUnits value changed", m_ImageUnits, subKey1.GetValue("ImageUnits", "").ToString(), m_smProductionInfo.g_strLotID);
                if (m_TriggerMode != subKey1.GetValue("TriggerMode", "").ToString())
                    STDeviceEdit.SaveDeviceEditLog("Camera", "TriggerMode value changed", m_TriggerMode, subKey1.GetValue("TriggerMode", "").ToString(), m_smProductionInfo.g_strLotID);
                if (m_Resolution != subKey1.GetValue("Resolution", "").ToString())
                    STDeviceEdit.SaveDeviceEditLog("Camera", "Resolution value changed", m_Resolution, subKey1.GetValue("Resolution", "").ToString(), m_smProductionInfo.g_strLotID);
                if (m_LightControllerType != subKey1.GetValue("LightControllerType", "").ToString())
                    STDeviceEdit.SaveDeviceEditLog("Camera", "LightControllerType value changed", m_LightControllerType, subKey1.GetValue("LightControllerType", "").ToString(), m_smProductionInfo.g_strLotID);
            }
           
            
            STDeviceEdit.CopySettingFile(AppDomain.CurrentDomain.BaseDirectory, "DeviceNo\\Camera.xml");
            //Save Light Source Sequence
            //fileHandle = new XmlParser(m_smProductionInfo.g_strRecipePath + "Camera.xml");
            for (int i = 0; i < dgd_Sequence.Rows.Count; i++)
            {

                //fileHandle.WriteSectionElement(dgd_Sequence.Rows[i].Cells[0].Value.ToString(), false);
                //fileHandle.WriteElement1Value(dgd_Sequence.Rows[i].Cells[1].Value.ToString(), dgd_Sequence.Rows[i].Cells[2].Value.ToString());

                RegistryKey LightControlKey = key.CreateSubKey("SVG\\LightControl");

                LightControlKey.SetValue(dgd_Sequence.Rows[i].Cells[0].Value.ToString() + " - " + dgd_Sequence.Rows[i].Cells[1].Value.ToString(), dgd_Sequence.Rows[i].Cells[2].Value);
                

                bool blnFound = false;
                for (int m = 0; m < m_smVSInfo.Length; m++)
                {
                    if (m_smVSInfo[m] == null)
                        continue;

                    for (int n = 0; n < m_smVSInfo[m].g_arrLightSource.Count; n++)
                    {
                        LightSource objLight = m_smVSInfo[m].g_arrLightSource[n];
                        if ((m_smVSInfo[m].g_strVisionName.ToString() == dgd_Sequence.Rows[i].Cells[0].Value.ToString()) &&
                            (m_smVSInfo[m].g_arrLightSource[n].ref_strType == dgd_Sequence.Rows[i].Cells[1].Value.ToString()))
                        {
                            m_smVSInfo[m].g_arrLightSource[n].ref_intSeqNo = Convert.ToInt32(dgd_Sequence.Rows[i].Cells[2].Value);

                            if (blnFound)
                                break;
                        }
                    }
                    if (blnFound)
                        break;
                }
               
            }
            //fileHandle.WriteEndElement();
            for (int i = 0; i < dgd_Sequence.Rows.Count; i++)
            {
                STDeviceEdit.XMLChangesTracing(dgd_Sequence.Rows[i].Cells[0].Value.ToString(), m_smProductionInfo.g_strLotID);
            }
            
        }

        private void CheckIsBufferSizeChanged()
        {
            if (intOriV1BufferSize != Convert.ToInt32(txt_SaveImageBufferSize.Text.ToString())
                || intOriV2BufferSize != Convert.ToInt32(txt_V2SaveImageBufferSize.Text.ToString())
                || intOriV3BufferSize != Convert.ToInt32(txt_V3SaveImageBufferSize.Text.ToString())
                || intOriV4BufferSize != Convert.ToInt32(txt_V4SaveImageBufferSize.Text.ToString())
                || intOriV5BufferSize != Convert.ToInt32(txt_V5SaveImageBufferSize.Text.ToString())
                || intOriV6BufferSize != Convert.ToInt32(txt_V6SaveImageBufferSize.Text.ToString())
                || intOriV7BufferSize != Convert.ToInt32(txt_V7SaveImageBufferSize.Text.ToString())
                || intOriV8BufferSize != Convert.ToInt32(txt_V8SaveImageBufferSize.Text.ToString())
                )
                SRMMessageBox.Show("Please restart software for new save image buffer size to take effect.");
        }

        private void chk_LEDiControl_Click(object sender, EventArgs e)
        {
            chk_VTControl.Checked = false;
            EnableTCOSIOControl();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {     
            WriteOption();
            CheckIsBufferSizeChanged();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void OptionForm_Load(object sender, EventArgs e)
        {
            LoadOption();
        }

        private void srmCheckBox1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] != null)
                    m_smVSInfo[i].g_blnDebugRUN = srmCheckBox1.Checked;
            }
        }

        private void srmInputBox1_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] != null)
                    m_smVSInfo[i].g_intSleep = Convert.ToInt32(srmInputBox1.Text);
            }
        }

        private void chk_VTControl_Click(object sender, EventArgs e)
        {
            chk_LEDiControl.Checked = false;
            EnableTCOSIOControl();
        }

        private void chk_DebugRunFromCenter_Click(object sender, EventArgs e)
        {
            m_smProductionInfo.g_blnAllRunFromCenter = chk_DebugRunFromCenter.Checked;
        }

        private void chk_DebugRunWithoutGrabImage_Click(object sender, EventArgs e)
        {
            m_smProductionInfo.g_blnAllRunWithoutGrabImage = chk_DebugRunWithoutGrabImage.Checked;

            if (chk_DebugRunGrabWithoutUseImage.Checked)
                m_smProductionInfo.g_blnAllRunGrabWithoutUseImage = chk_DebugRunGrabWithoutUseImage.Checked = false;
        }

        private void chk_DebugRunGrabWithoutUseImage_Click(object sender, EventArgs e)
        {
            m_smProductionInfo.g_blnAllRunGrabWithoutUseImage = chk_DebugRunGrabWithoutUseImage.Checked;

            if (chk_DebugRunWithoutGrabImage.Checked)
                m_smProductionInfo.g_blnAllRunWithoutGrabImage = chk_DebugRunWithoutGrabImage.Checked = false;
        }

        private void chk_ConfigShowNetwork_Click(object sender, EventArgs e)
        {
            if (!chk_ConfigShowNetwork.Checked)
            {
                if (SRMMessageBox.Show("Off Network Config will disable save/load recipe from server and save lot report to server. Are you sure you want to continue?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    chk_ConfigShowNetwork.Checked = true;
                }
            }
        }

        private void cbo_DebugImageCountToUse_SelectedIndexChanged(object sender, EventArgs e)
        {

            m_smProductionInfo.g_intDebugImageToUse = cbo_DebugImageCountToUse.SelectedIndex;
            
        }

        private void dgd_VisionList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.ColumnIndex != 6 || e.RowIndex < 0)
                return;
            
            int intImageMergeType;
            if (dgd_VisionList.Rows[e.RowIndex].Cells[6].Value.ToString() == "NoMerge")
                intImageMergeType = 0;
            else if (dgd_VisionList.Rows[e.RowIndex].Cells[6].Value.ToString() == "MergeGrab1&2")
                intImageMergeType = 1;
            else if (dgd_VisionList.Rows[e.RowIndex].Cells[6].Value.ToString() == "MergeAll")
                intImageMergeType = 2;
            else if (dgd_VisionList.Rows[e.RowIndex].Cells[6].Value.ToString() == "MergeGrab1&2,Grab3&4")
                intImageMergeType = 3;
            else
                intImageMergeType = 4;

            DataGridViewComboBoxCell dgvComboBox = new DataGridViewComboBoxCell();
            if (intImageMergeType == 2 || intImageMergeType == 4)
            {
                dgvComboBox.Items.Add("Standard");
                dgvComboBox.Items.Add("Pad");
                dgvComboBox.Items.Add("Lead");
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
                RegistryKey subKey1;
                subKey1 = subKey.OpenSubKey("Vision" + (e.RowIndex + 1).ToString());
                string strImageDisplayMode;
                if (subKey1.GetValue("ImageDisplayMode", "0").ToString() == "0")
                    strImageDisplayMode = "Standard";
                else if (subKey1.GetValue("ImageDisplayMode", "0").ToString() == "1")
                    strImageDisplayMode = "Pad";
                else if (subKey1.GetValue("ImageDisplayMode", "0").ToString() == "2")
                    strImageDisplayMode = "Lead";
                else
                    strImageDisplayMode = "Standard";
                dgvComboBox.Value = strImageDisplayMode;
            }
            else
            {
                dgvComboBox.Items.Add("Standard");
                dgvComboBox.Value = "Standard";
            }
            dgd_VisionList.Rows[e.RowIndex].Cells[7] = dgvComboBox;
        }

        //cxlim 2020/12/11 : clear unwanted folder and vision
        //eg. Vision 1: Bottom Orient only nid orient folder and pad folder(from Orient Pad) inside need to clean
        private void ClearUnwantedFolder_Click(object sender, EventArgs e)
        {
            List<string> strFoldername = new List<string>();
            List<string[]> strFile = new List<string[]>();
            List<string> strWantedFile = new List<string>();
            List<string[]> strVisionUnwantedFolder = new List<string[]>();
            string filename = "";
            int index = -1;

            if (SRMMessageBox.Show("Are you sure you want to clear all the unnecessary files? Deleted Files will not be recover.", "Warning Message", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] != null) //store vision folder that currently machine needed
                        strFoldername.Add(m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[i] + "\\" + m_smVSInfo[i].g_strVisionFolderName);
                    else
                    {   //store unwanted vision folder that imported from other machine
                        strVisionUnwantedFolder.Add(Directory.GetDirectories(m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[i]));
                        break;
                    }
                }

                for (int i = 0; i < strFoldername.Count; i++)
                {
                    if (Directory.Exists(strFoldername[i])) //get folder name in vision
                        strFile.Add(Directory.GetDirectories(strFoldername[i]));
                    else
                        continue;
                }

                //Start Filtering and store wanted folder 
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    strWantedFile.Clear();
                    if (m_smVSInfo[i] != null)
                    {
                        for (int j = 0; j < strFile[i].Length; j++)
                        {
                            index = strFile[i][j].LastIndexOf("\\");
                            filename = strFile[i][j].Substring(index + 1);

                            if ((m_smCustomInfo.g_intWantMark & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                            {
                                if (filename.Contains("Mark"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }
                            }

                            if ((m_smCustomInfo.g_intWantOrient & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                            {
                                if (filename.Contains("Orient"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }
                            }

                            if ((m_smCustomInfo.g_intWantPackage & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                            {
                                if (filename.Contains("Package"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }
                            }

                            if ((m_smCustomInfo.g_intWantPad & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                            {
                                if (filename.Contains("Pad"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }

                                if (filename.Contains("Positioning"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }
                            }

                            if ((m_smCustomInfo.g_intWantPositioning & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                            {
                                if (filename.Contains("Positioning"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }

                                if (filename.Contains("Orient"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }
                            }

                            if ((m_smCustomInfo.g_intWantCheckPresent & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                            {
                                if (filename.Contains("CheckPresent"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }
                            }

                            if ((m_smCustomInfo.g_intWantLead3D & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                            {
                                if (filename.Contains("Lead3D"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }

                                if (filename.Contains("Positioning"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }

                                if (filename.Contains("System"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }
                            }

                            if ((m_smCustomInfo.g_intWantPad5S & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                            {
                                if (filename.Contains("Pad"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }

                                if (filename.Contains("Positioning"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }

                                if (filename.Contains("System"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }

                                if (m_smVSInfo[i].g_blnWantPin1)
                                {
                                    if (filename.Contains("Orient"))
                                    {
                                        strWantedFile.Add(strFile[i][j]);
                                        continue;
                                    }
                                }
                            }

                            if ((m_smCustomInfo.g_intWantLead & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                            {
                                if (filename.Contains("Lead"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }
                            }

                            if (m_smVSInfo[i].g_strVisionName == "InPocketPkg" || m_smVSInfo[i].g_strVisionName == "InPocketPkgPos" || m_smVSInfo[i].g_strVisionName == "IPMLiPkg")
                            {
                                if (filename.Contains("PocketPosition"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }

                                if (filename.Contains("Positioning"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }
                            }

                            if (m_smVSInfo[i].g_strVisionName == "IPMLi" || m_smVSInfo[i].g_strVisionName == "InPocket")
                            {
                                if (filename.Contains("Positioning"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }
                            }

                            if ((m_smCustomInfo.g_intWantSeal & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                            {
                                if (filename.Contains("Seal"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }
                            }


                            if ((m_smCustomInfo.g_intWantBarcode & (1 << m_smVSInfo[i].g_intVisionPos)) > 0)
                            {
                                if (filename.Contains("Barcode"))
                                {
                                    strWantedFile.Add(strFile[i][j]);
                                    continue;
                                }
                            }
                        }
                        //Delete folder which is unwanted 
                        for (int j = 0; j < strFile[i].Length; j++)
                        {
                            for (int k = 0; k < strWantedFile.Count; k++)
                            {
                                if (strWantedFile[k].Equals(strFile[i][j])) //if find wanted folder skip
                                    break;
                                else
                                {
                                    if (k == strWantedFile.Count - 1)  //if compare till last and not find the specific folder delete it
                                    {
                                        if (Directory.Exists(strFile[i][j]))
                                        {
                                            try
                                            {
                                                Directory.Delete(strFile[i][j], true);
                                            }
                                            catch
                                            {

                                            }
                                        }
                                    }
                                    else
                                        continue;
                                }
                            }
                        }

                    }
                    else
                    {
                        //clear unnessecary folder that remain innside recipe
                        if (i >= strVisionUnwantedFolder[0].Length)
                            break;

                        for (int j = 0; j < strVisionUnwantedFolder[0].Length; j++)
                        {
                            index = strVisionUnwantedFolder[0][j].LastIndexOf("\\");
                            filename = strVisionUnwantedFolder[0][j].Substring(index + 1);


                            if (filename.Contains("Vision"))
                            {
                                for (int k = 0; k < strFoldername.Count; k++)
                                {
                                    if (m_smVSInfo[k].g_strVisionFolderName.Equals(filename))
                                        break;
                                    else
                                    {
                                        if (k == strFoldername.Count - 1)
                                        {
                                            if (Directory.Exists(strVisionUnwantedFolder[0][j]))
                                                Directory.Delete(strVisionUnwantedFolder[0][j], true);
                                        }
                                        else
                                            continue;
                                    }
                                }
                            }
                            else
                            {
                                if (Directory.Exists(strVisionUnwantedFolder[0][j]))
                                    Directory.Delete(strVisionUnwantedFolder[0][j], true);
                            }

                        }
                    }
                }
            }
            else
                return;

            clearUnwantedFile(strFile, strFoldername); //clear unwanted file in folder after clear unnecessary folder 
        }

        //cxlim  29/12/2020 : clear unnecessary image file inside folder
        private void clearUnwantedFile(List<string[]> strfiles, List<string> Foldername)
        {
            List<string[]> FilesInside = new List<string[]>();
            List<string[]> FilesOutside = new List<string[]>();
            int counter = 0;

            for (int j = 0; j < strfiles.Count; j++)  //vision
            {
                FilesOutside.Clear();

                if (Directory.Exists((Foldername[counter])))
                    FilesOutside.Add(Directory.GetFiles(Foldername[counter], "*.bmp")); //file outside is calibration image

                for (int k = 0; k < strfiles[j].Length; k++) //get files inside folder
                {
                    FilesInside.Clear();

                    //start filtering (only take .bmp file)
                    if (strfiles[j][k].Contains("Orient"))
                    {
                        if (Directory.Exists(strfiles[j][k] + "\\Template\\"))
                        {
                            FilesInside.Add(Directory.GetFiles(strfiles[j][k] + "\\Template\\", "*.bmp"));
                        }
                    }

                    if (strfiles[j][k].Contains("Package"))
                    {
                        if (Directory.Exists(strfiles[j][k] + "\\Template\\"))
                        {
                            FilesInside.Add(Directory.GetFiles(strfiles[j][k] + "\\Template\\", "*.bmp"));
                        }
                    }

                    if (strfiles[j][k].Contains("Positioning"))
                    {
                        if (Directory.Exists(strfiles[j][k] + "\\Template\\"))
                        {
                            FilesInside.Add(Directory.GetFiles(strfiles[j][k] + "\\Template\\", "*.bmp"));
                        }
                    }

                    if (strfiles[j][k].Contains("Pad"))
                    {
                        if (Directory.Exists(strfiles[j][k] + "\\Template\\"))
                        {
                            FilesInside.Add(Directory.GetFiles(strfiles[j][k] + "\\Template\\", "*.bmp"));
                        }
                    }


                    if (FilesInside.Count != 0)
                    {
                        foreach (string str in FilesInside[0])
                        {
                            if (m_smVSInfo[counter].g_arrPadOrientROIs != null || m_smVSInfo[counter].g_arrPadROIs != null) //delete file for BottomOrient and PadOrient
                            {
                                if (m_smVSInfo[counter].g_arrPadOrientROIs.Count > 0 || m_smVSInfo[counter].g_arrPadROIs.Count > 0)
                                {
                                    if (strfiles[j][k].Contains("Orient") || strfiles[j][k].Contains("Package"))
                                    {
                                        if (str.Contains("OriTemplate0"))
                                            File.Delete(str);
                                    }
                                }
                                else
                                    continue;
                            }
                            else
                            {
                                if ((m_smCustomInfo.g_intWantPackage & (1 << m_smVSInfo[counter].g_intVisionPos)) > 0 && (m_smCustomInfo.g_intWantMark & (1 << m_smVSInfo[counter].g_intVisionPos)) > 0)  //delete for markPkg in 1st vision
                                    if (strfiles[j][k].Contains("Orient") || strfiles[j][k].Contains("Package"))
                                    {
                                        if (!str.Contains("OriTemplate0"))
                                            File.Delete(str);
                                    }
                            }


                            if ((m_smCustomInfo.g_intWantPad5S & (1 << m_smVSInfo[counter].g_intVisionPos)) > 0 || (m_smCustomInfo.g_intWantPad & (1 << m_smVSInfo[counter].g_intVisionPos)) > 0) //delete file in position in pad/pad5s
                            {
                                if (strfiles[j][k].Contains("Positioning")) //for pad5s or pad
                                {
                                    if (!str.Contains("PRS"))
                                        File.Delete(str);
                                }
                                else
                                {
                                    if (strfiles[j][k].Contains("Positioning")) //delete file for InPocket
                                    {
                                        if (str.Contains("PRS"))
                                            File.Delete(str);
                                    }
                                }

                                //delete unecessary pad template image 
                                if (strfiles[j][k].Contains("Pad") && m_smVSInfo[counter].g_arrImages.Count != 0 || strfiles[j][k].Contains("Package") && m_smVSInfo[counter].g_arrImages.Count != 0)
                                {
                                    if (m_smVSInfo[counter].g_arrImages.Count > 1 && m_smVSInfo[counter].g_intImageMergeType == 0)
                                    {
                                        if (str.Contains("OriTemplate_Image2") || str.Contains("OriTemplate_Image3") || str.Contains("OriTemplate_Image4"))
                                            File.Delete(str);
                                    }
                                    else if (m_smVSInfo[counter].g_arrImages.Count > 2 && m_smVSInfo[counter].g_intImageMergeType == 0 || m_smVSInfo[counter].g_arrImages.Count > 2 && m_smVSInfo[counter].g_intImageMergeType == 1 || m_smVSInfo[counter].g_arrImages.Count > 2 && m_smVSInfo[counter].g_intImageMergeType == 3)
                                    {
                                        if (str.Contains("OriTemplate_Image1") || str.Contains("OriTemplate_Image3") || str.Contains("OriTemplate_Image4"))
                                            File.Delete(str);
                                    }
                                    else if (m_smVSInfo[counter].g_arrImages.Count > 3 && m_smVSInfo[counter].g_intImageMergeType == 0 || m_smVSInfo[counter].g_arrImages.Count > 3 && m_smVSInfo[counter].g_intImageMergeType == 1 || m_smVSInfo[counter].g_arrImages.Count > 3 && m_smVSInfo[counter].g_intImageMergeType == 2 || m_smVSInfo[counter].g_arrImages.Count > 3 && m_smVSInfo[counter].g_intImageMergeType == 4)
                                    {
                                        if (str.Contains("OriTemplate_Image1") || str.Contains("OriTemplate_Image2") || str.Contains("OriTemplate_Image4"))
                                            File.Delete(str);
                                    }
                                    else
                                    {
                                        if (str.Contains("OriTemplate_Image1") || str.Contains("OriTemplate_Image2") || str.Contains("OriTemplate_Image3"))
                                            File.Delete(str);
                                    }
                                }
                            }
                            else
                                continue;

                        }
                    }

                    if ((m_smCustomInfo.g_intWantLead3D & (1 << m_smVSInfo[counter].g_intVisionPos)) > 0)
                    {
                        if (FilesOutside.Count != 0)
                        {
                            foreach (string str in FilesOutside[0])
                            {
                                int start = str.LastIndexOf("\\");
                                string temp = str.Substring(start + 1);

                                if (temp.Equals("CalibrationImageHorizontal.bmp"))
                                    continue;
                                else if (temp.Equals("CalibrationImageVertical.bmp"))
                                    continue;
                                else
                                    File.Delete(str);
                            }
                        }
                    }
                    else 
                    {
                        if (FilesOutside.Count != 0)
                        {
                            foreach (string str in FilesOutside[0])
                            {
                                int start = str.LastIndexOf("\\");
                                string temp = str.Substring(start + 1);

                                if (temp.Equals("CalibrationImageHorizontal.bmp"))
                                    File.Delete(str);
                                else if (temp.Equals("CalibrationImageVertical.bmp"))
                                    File.Delete(str);
                                else
                                    continue;
                            }
                        }

                    }
                }
                    counter++; // next vision
            }
        }

        private void chk_ConfigShowTCPIP_Click(object sender, EventArgs e)
        {
            chk_WantUseTCPIPAsIO.Visible = chk_ConfigShowTCPIP.Checked;
        }

        //private void txt_SaveImageBufferSize_TextChanged(object sender, EventArgs e)
        //{
        //    int intVisionSelected = Convert.ToInt32(((SRMControl.SRMInputBox)sender).Tag.ToString());
        //    int intBufferSize = Convert.ToInt32(((SRMControl.SRMInputBox)sender).Text.ToString());

        //    if (m_smVSInfo[intVisionSelected] != null)
        //        m_smVSInfo[intVisionSelected].g_intSaveImageBufferSize = intBufferSize;
        //}
    }
}