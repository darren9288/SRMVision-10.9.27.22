namespace SRMVision
{
    partial class AppConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppConfigForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabCtrl_Configuration = new SRMControl.SRMTabControl();
            this.tab_General = new System.Windows.Forms.TabPage();
            this.chk_GlobalSharingCameraData = new SRMControl.SRMCheckBox();
            this.cbo_MarkUnitDisplay = new SRMControl.SRMComboBox();
            this.srmLabel29 = new SRMControl.SRMLabel();
            this.chk_GlobalSharingCalibrationData = new SRMControl.SRMCheckBox();
            this.srmLabel21 = new SRMControl.SRMLabel();
            this.srmLabel20 = new SRMControl.SRMLabel();
            this.txt_AutoLogOutMinutes = new System.Windows.Forms.NumericUpDown();
            this.cbo_PreviousVersion = new SRMControl.SRMComboBox();
            this.chk_PreviousVersion = new SRMControl.SRMCheckBox();
            this.cbo_UnitDisplay = new SRMControl.SRMComboBox();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.txt_WrongFaceTole = new SRMControl.SRMInputBox();
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.txt_EmptyPocketTole = new SRMControl.SRMInputBox();
            this.txt_LowYield = new SRMControl.SRMInputBox();
            this.srmLabel6 = new SRMControl.SRMLabel();
            this.chk_StopLowYield = new SRMControl.SRMCheckBox();
            this.txt_MinUnitCheck = new SRMControl.SRMInputBox();
            this.group_PassImage = new SRMControl.SRMGroupBox();
            this.chk_SaveErrorMessge = new SRMControl.SRMCheckBox();
            this.txt_PassImagePics = new SRMControl.SRMInputBox();
            this.chk_SavePassImage = new SRMControl.SRMCheckBox();
            this.srmGroupBox3 = new SRMControl.SRMGroupBox();
            this.radioBtn_Last = new SRMControl.SRMRadioButton();
            this.radioBtn_First = new SRMControl.SRMRadioButton();
            this.chk_SaveFailImage = new SRMControl.SRMCheckBox();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.txt_FailImagePics = new SRMControl.SRMInputBox();
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.chk_DebugMode = new SRMControl.SRMCheckBox();
            this.srmLabel7 = new SRMControl.SRMLabel();
            this.tab_TCPIP = new System.Windows.Forms.TabPage();
            this.dgd_VisionList = new System.Windows.Forms.DataGridView();
            this.dcVisionName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dcUnits = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VisionID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grp_ExportLotReport = new SRMControl.SRMGroupBox();
            this.txt_IPAddress = new SRMControl.SRMInputBox();
            this.srmLabel26 = new SRMControl.SRMLabel();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.cbo_ReportFormat = new SRMControl.SRMComboBox();
            this.lbl_ReportPath = new SRMControl.SRMLabel();
            this.txt_TCPIPReportPath = new SRMControl.SRMTextBox();
            this.btn_ReportPathButton = new SRMControl.SRMButton();
            this.chk_ExportLotReport = new SRMControl.SRMCheckBox();
            this.srmLabel39 = new SRMControl.SRMLabel();
            this.txt_TCPIPRetriesCount = new SRMControl.SRMInputBox();
            this.srmLabel40 = new SRMControl.SRMLabel();
            this.srmLabel27 = new SRMControl.SRMLabel();
            this.txt_TCPIPTimeout = new SRMControl.SRMInputBox();
            this.srmLabel28 = new SRMControl.SRMLabel();
            this.chk_EnableTCPIP = new SRMControl.SRMCheckBox();
            this.txt_TCPIPPort = new SRMControl.SRMInputBox();
            this.srmLabel33 = new SRMControl.SRMLabel();
            this.tab_RS232 = new System.Windows.Forms.TabPage();
            this.srmLabel37 = new SRMControl.SRMLabel();
            this.txt_SerialPortRetriesCount = new SRMControl.SRMInputBox();
            this.srmLabel38 = new SRMControl.SRMLabel();
            this.srmLabel16 = new SRMControl.SRMLabel();
            this.txt_SerialPortTimeOut = new SRMControl.SRMInputBox();
            this.srmLabel36 = new SRMControl.SRMLabel();
            this.cbo_CommPort = new SRMControl.SRMComboBox();
            this.srmLabel35 = new SRMControl.SRMLabel();
            this.chk_SerialPortEnable = new SRMControl.SRMCheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbo_StopBits = new SRMControl.SRMComboBox();
            this.cbo_Parity = new SRMControl.SRMComboBox();
            this.cbo_DataBits = new SRMControl.SRMComboBox();
            this.srmLabel22 = new SRMControl.SRMLabel();
            this.srmLabel23 = new SRMControl.SRMLabel();
            this.srmLabel24 = new SRMControl.SRMLabel();
            this.cbo_BitsPerSecond = new SRMControl.SRMComboBox();
            this.srmLabel25 = new SRMControl.SRMLabel();
            this.tab_camera = new System.Windows.Forms.TabPage();
            this.cbo_7 = new SRMControl.SRMComboBox();
            this.cbo_6 = new SRMControl.SRMComboBox();
            this.cbo_5 = new SRMControl.SRMComboBox();
            this.cbo_Vision7 = new SRMControl.SRMComboBox();
            this.cbo_Vision6 = new SRMControl.SRMComboBox();
            this.cbo_Vision5 = new SRMControl.SRMComboBox();
            this.lbl_Vision7Status = new SRMControl.SRMLabel();
            this.lbl_Vision6Status = new SRMControl.SRMLabel();
            this.lbl_Vision5Status = new SRMControl.SRMLabel();
            this.lbl_Vision7 = new SRMControl.SRMLabel();
            this.lbl_Vision6 = new SRMControl.SRMLabel();
            this.lbl_Vision5 = new SRMControl.SRMLabel();
            this.cbo_4 = new SRMControl.SRMComboBox();
            this.cbo_3 = new SRMControl.SRMComboBox();
            this.cbo_2 = new SRMControl.SRMComboBox();
            this.cbo_1 = new SRMControl.SRMComboBox();
            this.btn_Connect = new SRMControl.SRMButton();
            this.cbo_Vision4 = new SRMControl.SRMComboBox();
            this.cbo_Vision3 = new SRMControl.SRMComboBox();
            this.cbo_Vision2 = new SRMControl.SRMComboBox();
            this.cbo_Vision1 = new SRMControl.SRMComboBox();
            this.lbl_Vision4Status = new SRMControl.SRMLabel();
            this.lbl_Vision3Status = new SRMControl.SRMLabel();
            this.lbl_Vision2Status = new SRMControl.SRMLabel();
            this.lbl_Vision1Status = new SRMControl.SRMLabel();
            this.lbl_Vision4 = new SRMControl.SRMLabel();
            this.lbl_Vision3 = new SRMControl.SRMLabel();
            this.lbl_Vision2 = new SRMControl.SRMLabel();
            this.lbl_Vision1 = new SRMControl.SRMLabel();
            this.srmLabel12 = new SRMControl.SRMLabel();
            this.srmLabel10 = new SRMControl.SRMLabel();
            this.srmLabel11 = new SRMControl.SRMLabel();
            this.tab_Network = new System.Windows.Forms.TabPage();
            this.chk_WantUseNetwork = new SRMControl.SRMCheckBox();
            this.chk_NetworkPasswordLimitCheckBox = new SRMControl.SRMCheckBox();
            this.txt_VisionLotReportUploadDirEditBox = new SRMControl.SRMTextBox();
            this.txt_NetworkPasswordEditBox = new SRMControl.SRMTextBox();
            this.txt_DeviceUploadDirEditBox = new SRMControl.SRMTextBox();
            this.btn_VisionLotReportUploadBrowseButton = new SRMControl.SRMButton();
            this.srmLabel13 = new SRMControl.SRMLabel();
            this.txt_NetworkUsernameEditBox = new SRMControl.SRMTextBox();
            this.txt_HostIPEditBox = new SRMControl.SRMTextBox();
            this.srmLabel9 = new SRMControl.SRMLabel();
            this.btn_DeviceUploadBrowseButton = new SRMControl.SRMButton();
            this.srmLabel14 = new SRMControl.SRMLabel();
            this.srmLabel15 = new SRMControl.SRMLabel();
            this.srmLabel17 = new SRMControl.SRMLabel();
            this.tab_SECSGEM = new System.Windows.Forms.TabPage();
            this.srmLabel19 = new SRMControl.SRMLabel();
            this.txt_MaxNoOfCoplanPad = new SRMControl.SRMInputBox();
            this.chk_WantUseSECSGEM = new SRMControl.SRMCheckBox();
            this.txt_SECSGEMSharedFolderPath = new SRMControl.SRMTextBox();
            this.btn_BrowseSECSGEMSharedFolderPath = new SRMControl.SRMButton();
            this.srmLabel18 = new SRMControl.SRMLabel();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.dlg_ReportFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.NetworkFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tabCtrl_Configuration.SuspendLayout();
            this.tab_General.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_AutoLogOutMinutes)).BeginInit();
            this.group_PassImage.SuspendLayout();
            this.srmGroupBox3.SuspendLayout();
            this.tab_TCPIP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_VisionList)).BeginInit();
            this.grp_ExportLotReport.SuspendLayout();
            this.tab_RS232.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tab_camera.SuspendLayout();
            this.tab_Network.SuspendLayout();
            this.tab_SECSGEM.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabCtrl_Configuration
            // 
            resources.ApplyResources(this.tabCtrl_Configuration, "tabCtrl_Configuration");
            this.tabCtrl_Configuration.Controls.Add(this.tab_General);
            this.tabCtrl_Configuration.Controls.Add(this.tab_TCPIP);
            this.tabCtrl_Configuration.Controls.Add(this.tab_RS232);
            this.tabCtrl_Configuration.Controls.Add(this.tab_camera);
            this.tabCtrl_Configuration.Controls.Add(this.tab_Network);
            this.tabCtrl_Configuration.Controls.Add(this.tab_SECSGEM);
            this.tabCtrl_Configuration.Name = "tabCtrl_Configuration";
            this.tabCtrl_Configuration.SelectedIndex = 0;
            this.tabCtrl_Configuration.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            // 
            // tab_General
            // 
            resources.ApplyResources(this.tab_General, "tab_General");
            this.tab_General.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tab_General.Controls.Add(this.chk_GlobalSharingCameraData);
            this.tab_General.Controls.Add(this.cbo_MarkUnitDisplay);
            this.tab_General.Controls.Add(this.srmLabel29);
            this.tab_General.Controls.Add(this.chk_GlobalSharingCalibrationData);
            this.tab_General.Controls.Add(this.srmLabel21);
            this.tab_General.Controls.Add(this.srmLabel20);
            this.tab_General.Controls.Add(this.txt_AutoLogOutMinutes);
            this.tab_General.Controls.Add(this.cbo_PreviousVersion);
            this.tab_General.Controls.Add(this.chk_PreviousVersion);
            this.tab_General.Controls.Add(this.cbo_UnitDisplay);
            this.tab_General.Controls.Add(this.srmLabel2);
            this.tab_General.Controls.Add(this.srmLabel1);
            this.tab_General.Controls.Add(this.txt_WrongFaceTole);
            this.tab_General.Controls.Add(this.srmLabel8);
            this.tab_General.Controls.Add(this.txt_EmptyPocketTole);
            this.tab_General.Controls.Add(this.txt_LowYield);
            this.tab_General.Controls.Add(this.srmLabel6);
            this.tab_General.Controls.Add(this.chk_StopLowYield);
            this.tab_General.Controls.Add(this.txt_MinUnitCheck);
            this.tab_General.Controls.Add(this.group_PassImage);
            this.tab_General.Controls.Add(this.chk_DebugMode);
            this.tab_General.Controls.Add(this.srmLabel7);
            this.tab_General.Name = "tab_General";
            // 
            // chk_GlobalSharingCameraData
            // 
            resources.ApplyResources(this.chk_GlobalSharingCameraData, "chk_GlobalSharingCameraData");
            this.chk_GlobalSharingCameraData.CheckedColor = System.Drawing.Color.Empty;
            this.chk_GlobalSharingCameraData.Name = "chk_GlobalSharingCameraData";
            this.chk_GlobalSharingCameraData.Selected = false;
            this.chk_GlobalSharingCameraData.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_GlobalSharingCameraData.UnCheckedColor = System.Drawing.Color.Empty;
            // 
            // cbo_MarkUnitDisplay
            // 
            resources.ApplyResources(this.cbo_MarkUnitDisplay, "cbo_MarkUnitDisplay");
            this.cbo_MarkUnitDisplay.BackColor = System.Drawing.Color.White;
            this.cbo_MarkUnitDisplay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_MarkUnitDisplay.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_MarkUnitDisplay.Items.AddRange(new object[] {
            resources.GetString("cbo_MarkUnitDisplay.Items"),
            resources.GetString("cbo_MarkUnitDisplay.Items1"),
            resources.GetString("cbo_MarkUnitDisplay.Items2"),
            resources.GetString("cbo_MarkUnitDisplay.Items3")});
            this.cbo_MarkUnitDisplay.Name = "cbo_MarkUnitDisplay";
            this.cbo_MarkUnitDisplay.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel29
            // 
            resources.ApplyResources(this.srmLabel29, "srmLabel29");
            this.srmLabel29.Name = "srmLabel29";
            this.srmLabel29.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // chk_GlobalSharingCalibrationData
            // 
            resources.ApplyResources(this.chk_GlobalSharingCalibrationData, "chk_GlobalSharingCalibrationData");
            this.chk_GlobalSharingCalibrationData.CheckedColor = System.Drawing.Color.Empty;
            this.chk_GlobalSharingCalibrationData.Name = "chk_GlobalSharingCalibrationData";
            this.chk_GlobalSharingCalibrationData.Selected = false;
            this.chk_GlobalSharingCalibrationData.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_GlobalSharingCalibrationData.UnCheckedColor = System.Drawing.Color.Empty;
            // 
            // srmLabel21
            // 
            resources.ApplyResources(this.srmLabel21, "srmLabel21");
            this.srmLabel21.Name = "srmLabel21";
            this.srmLabel21.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel20
            // 
            resources.ApplyResources(this.srmLabel20, "srmLabel20");
            this.srmLabel20.Name = "srmLabel20";
            this.srmLabel20.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_AutoLogOutMinutes
            // 
            resources.ApplyResources(this.txt_AutoLogOutMinutes, "txt_AutoLogOutMinutes");
            this.txt_AutoLogOutMinutes.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.txt_AutoLogOutMinutes.Name = "txt_AutoLogOutMinutes";
            // 
            // cbo_PreviousVersion
            // 
            resources.ApplyResources(this.cbo_PreviousVersion, "cbo_PreviousVersion");
            this.cbo_PreviousVersion.BackColor = System.Drawing.Color.White;
            this.cbo_PreviousVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_PreviousVersion.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_PreviousVersion.Items.AddRange(new object[] {
            resources.GetString("cbo_PreviousVersion.Items"),
            resources.GetString("cbo_PreviousVersion.Items1"),
            resources.GetString("cbo_PreviousVersion.Items2")});
            this.cbo_PreviousVersion.Name = "cbo_PreviousVersion";
            this.cbo_PreviousVersion.NormalBackColor = System.Drawing.Color.White;
            this.cbo_PreviousVersion.SelectedIndexChanged += new System.EventHandler(this.srmComboBox2_SelectedIndexChanged);
            // 
            // chk_PreviousVersion
            // 
            resources.ApplyResources(this.chk_PreviousVersion, "chk_PreviousVersion");
            this.chk_PreviousVersion.CheckedColor = System.Drawing.Color.Empty;
            this.chk_PreviousVersion.Name = "chk_PreviousVersion";
            this.chk_PreviousVersion.Selected = false;
            this.chk_PreviousVersion.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_PreviousVersion.UnCheckedColor = System.Drawing.Color.Empty;
            // 
            // cbo_UnitDisplay
            // 
            resources.ApplyResources(this.cbo_UnitDisplay, "cbo_UnitDisplay");
            this.cbo_UnitDisplay.BackColor = System.Drawing.Color.White;
            this.cbo_UnitDisplay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_UnitDisplay.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_UnitDisplay.Items.AddRange(new object[] {
            resources.GetString("cbo_UnitDisplay.Items"),
            resources.GetString("cbo_UnitDisplay.Items1"),
            resources.GetString("cbo_UnitDisplay.Items2")});
            this.cbo_UnitDisplay.Name = "cbo_UnitDisplay";
            this.cbo_UnitDisplay.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_WrongFaceTole
            // 
            resources.ApplyResources(this.txt_WrongFaceTole, "txt_WrongFaceTole");
            this.txt_WrongFaceTole.BackColor = System.Drawing.Color.White;
            this.txt_WrongFaceTole.DataType = SRMControl.SRMDataType.Int32;
            this.txt_WrongFaceTole.DecimalPlaces = 0;
            this.txt_WrongFaceTole.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_WrongFaceTole.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_WrongFaceTole.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_WrongFaceTole.ForeColor = System.Drawing.Color.Black;
            this.txt_WrongFaceTole.InputType = SRMControl.InputType.Number;
            this.txt_WrongFaceTole.Name = "txt_WrongFaceTole";
            this.txt_WrongFaceTole.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel8
            // 
            resources.ApplyResources(this.srmLabel8, "srmLabel8");
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_EmptyPocketTole
            // 
            resources.ApplyResources(this.txt_EmptyPocketTole, "txt_EmptyPocketTole");
            this.txt_EmptyPocketTole.BackColor = System.Drawing.Color.White;
            this.txt_EmptyPocketTole.DataType = SRMControl.SRMDataType.Int32;
            this.txt_EmptyPocketTole.DecimalPlaces = 0;
            this.txt_EmptyPocketTole.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_EmptyPocketTole.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_EmptyPocketTole.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_EmptyPocketTole.ForeColor = System.Drawing.Color.Black;
            this.txt_EmptyPocketTole.InputType = SRMControl.InputType.Number;
            this.txt_EmptyPocketTole.Name = "txt_EmptyPocketTole";
            this.txt_EmptyPocketTole.NormalBackColor = System.Drawing.Color.White;
            // 
            // txt_LowYield
            // 
            resources.ApplyResources(this.txt_LowYield, "txt_LowYield");
            this.txt_LowYield.BackColor = System.Drawing.Color.White;
            this.txt_LowYield.DataType = SRMControl.SRMDataType.Int32;
            this.txt_LowYield.DecimalPlaces = 1;
            this.txt_LowYield.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_LowYield.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_LowYield.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_LowYield.ForeColor = System.Drawing.Color.Black;
            this.txt_LowYield.InputType = SRMControl.InputType.Number;
            this.txt_LowYield.Name = "txt_LowYield";
            this.txt_LowYield.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel6
            // 
            resources.ApplyResources(this.srmLabel6, "srmLabel6");
            this.srmLabel6.Name = "srmLabel6";
            this.srmLabel6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // chk_StopLowYield
            // 
            resources.ApplyResources(this.chk_StopLowYield, "chk_StopLowYield");
            this.chk_StopLowYield.CheckedColor = System.Drawing.Color.Empty;
            this.chk_StopLowYield.Name = "chk_StopLowYield";
            this.chk_StopLowYield.Selected = false;
            this.chk_StopLowYield.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_StopLowYield.UnCheckedColor = System.Drawing.Color.Empty;
            // 
            // txt_MinUnitCheck
            // 
            resources.ApplyResources(this.txt_MinUnitCheck, "txt_MinUnitCheck");
            this.txt_MinUnitCheck.BackColor = System.Drawing.Color.White;
            this.txt_MinUnitCheck.DataType = SRMControl.SRMDataType.Int32;
            this.txt_MinUnitCheck.DecimalPlaces = 0;
            this.txt_MinUnitCheck.DecMaxValue = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.txt_MinUnitCheck.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MinUnitCheck.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MinUnitCheck.ForeColor = System.Drawing.Color.Black;
            this.txt_MinUnitCheck.InputType = SRMControl.InputType.Number;
            this.txt_MinUnitCheck.Name = "txt_MinUnitCheck";
            this.txt_MinUnitCheck.NormalBackColor = System.Drawing.Color.White;
            // 
            // group_PassImage
            // 
            resources.ApplyResources(this.group_PassImage, "group_PassImage");
            this.group_PassImage.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_PassImage.Controls.Add(this.chk_SaveErrorMessge);
            this.group_PassImage.Controls.Add(this.txt_PassImagePics);
            this.group_PassImage.Controls.Add(this.chk_SavePassImage);
            this.group_PassImage.Controls.Add(this.srmGroupBox3);
            this.group_PassImage.Controls.Add(this.chk_SaveFailImage);
            this.group_PassImage.Controls.Add(this.srmLabel3);
            this.group_PassImage.Controls.Add(this.txt_FailImagePics);
            this.group_PassImage.Controls.Add(this.srmLabel5);
            this.group_PassImage.Name = "group_PassImage";
            this.group_PassImage.TabStop = false;
            // 
            // chk_SaveErrorMessge
            // 
            resources.ApplyResources(this.chk_SaveErrorMessge, "chk_SaveErrorMessge");
            this.chk_SaveErrorMessge.CheckedColor = System.Drawing.Color.Empty;
            this.chk_SaveErrorMessge.Name = "chk_SaveErrorMessge";
            this.chk_SaveErrorMessge.Selected = false;
            this.chk_SaveErrorMessge.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SaveErrorMessge.UnCheckedColor = System.Drawing.Color.Empty;
            // 
            // txt_PassImagePics
            // 
            resources.ApplyResources(this.txt_PassImagePics, "txt_PassImagePics");
            this.txt_PassImagePics.BackColor = System.Drawing.Color.White;
            this.txt_PassImagePics.DataType = SRMControl.SRMDataType.Int32;
            this.txt_PassImagePics.DecimalPlaces = 0;
            this.txt_PassImagePics.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_PassImagePics.DecMinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_PassImagePics.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_PassImagePics.ForeColor = System.Drawing.Color.Black;
            this.txt_PassImagePics.InputType = SRMControl.InputType.Number;
            this.txt_PassImagePics.Name = "txt_PassImagePics";
            this.txt_PassImagePics.NormalBackColor = System.Drawing.Color.White;
            // 
            // chk_SavePassImage
            // 
            resources.ApplyResources(this.chk_SavePassImage, "chk_SavePassImage");
            this.chk_SavePassImage.CheckedColor = System.Drawing.Color.Empty;
            this.chk_SavePassImage.Name = "chk_SavePassImage";
            this.chk_SavePassImage.Selected = false;
            this.chk_SavePassImage.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SavePassImage.UnCheckedColor = System.Drawing.Color.Empty;
            // 
            // srmGroupBox3
            // 
            resources.ApplyResources(this.srmGroupBox3, "srmGroupBox3");
            this.srmGroupBox3.BorderColor = System.Drawing.Color.Transparent;
            this.srmGroupBox3.Controls.Add(this.radioBtn_Last);
            this.srmGroupBox3.Controls.Add(this.radioBtn_First);
            this.srmGroupBox3.Name = "srmGroupBox3";
            this.srmGroupBox3.TabStop = false;
            // 
            // radioBtn_Last
            // 
            resources.ApplyResources(this.radioBtn_Last, "radioBtn_Last");
            this.radioBtn_Last.Name = "radioBtn_Last";
            // 
            // radioBtn_First
            // 
            resources.ApplyResources(this.radioBtn_First, "radioBtn_First");
            this.radioBtn_First.Checked = true;
            this.radioBtn_First.Name = "radioBtn_First";
            this.radioBtn_First.TabStop = true;
            // 
            // chk_SaveFailImage
            // 
            resources.ApplyResources(this.chk_SaveFailImage, "chk_SaveFailImage");
            this.chk_SaveFailImage.Checked = true;
            this.chk_SaveFailImage.CheckedColor = System.Drawing.Color.Empty;
            this.chk_SaveFailImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_SaveFailImage.Name = "chk_SaveFailImage";
            this.chk_SaveFailImage.Selected = false;
            this.chk_SaveFailImage.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SaveFailImage.UnCheckedColor = System.Drawing.Color.Empty;
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_FailImagePics
            // 
            resources.ApplyResources(this.txt_FailImagePics, "txt_FailImagePics");
            this.txt_FailImagePics.BackColor = System.Drawing.Color.White;
            this.txt_FailImagePics.DataType = SRMControl.SRMDataType.Int32;
            this.txt_FailImagePics.DecimalPlaces = 0;
            this.txt_FailImagePics.DecMaxValue = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txt_FailImagePics.DecMinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_FailImagePics.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_FailImagePics.ForeColor = System.Drawing.Color.Black;
            this.txt_FailImagePics.InputType = SRMControl.InputType.Number;
            this.txt_FailImagePics.Name = "txt_FailImagePics";
            this.txt_FailImagePics.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel5
            // 
            resources.ApplyResources(this.srmLabel5, "srmLabel5");
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // chk_DebugMode
            // 
            resources.ApplyResources(this.chk_DebugMode, "chk_DebugMode");
            this.chk_DebugMode.CheckedColor = System.Drawing.Color.Empty;
            this.chk_DebugMode.Name = "chk_DebugMode";
            this.chk_DebugMode.Selected = false;
            this.chk_DebugMode.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_DebugMode.UnCheckedColor = System.Drawing.Color.Empty;
            // 
            // srmLabel7
            // 
            resources.ApplyResources(this.srmLabel7, "srmLabel7");
            this.srmLabel7.Name = "srmLabel7";
            this.srmLabel7.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tab_TCPIP
            // 
            resources.ApplyResources(this.tab_TCPIP, "tab_TCPIP");
            this.tab_TCPIP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tab_TCPIP.Controls.Add(this.dgd_VisionList);
            this.tab_TCPIP.Controls.Add(this.grp_ExportLotReport);
            this.tab_TCPIP.Controls.Add(this.chk_ExportLotReport);
            this.tab_TCPIP.Controls.Add(this.srmLabel39);
            this.tab_TCPIP.Controls.Add(this.txt_TCPIPRetriesCount);
            this.tab_TCPIP.Controls.Add(this.srmLabel40);
            this.tab_TCPIP.Controls.Add(this.srmLabel27);
            this.tab_TCPIP.Controls.Add(this.txt_TCPIPTimeout);
            this.tab_TCPIP.Controls.Add(this.srmLabel28);
            this.tab_TCPIP.Controls.Add(this.chk_EnableTCPIP);
            this.tab_TCPIP.Controls.Add(this.txt_TCPIPPort);
            this.tab_TCPIP.Controls.Add(this.srmLabel33);
            this.tab_TCPIP.Name = "tab_TCPIP";
            // 
            // dgd_VisionList
            // 
            resources.ApplyResources(this.dgd_VisionList, "dgd_VisionList");
            this.dgd_VisionList.AllowUserToAddRows = false;
            this.dgd_VisionList.AllowUserToDeleteRows = false;
            this.dgd_VisionList.AllowUserToResizeColumns = false;
            this.dgd_VisionList.AllowUserToResizeRows = false;
            this.dgd_VisionList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dgd_VisionList.BackgroundColor = System.Drawing.Color.AliceBlue;
            this.dgd_VisionList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_VisionList.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.LightSkyBlue;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgd_VisionList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgd_VisionList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgd_VisionList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dcVisionName,
            this.dcUnits,
            this.VisionID});
            this.dgd_VisionList.MultiSelect = false;
            this.dgd_VisionList.Name = "dgd_VisionList";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.LightSkyBlue;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgd_VisionList.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgd_VisionList.RowHeadersVisible = false;
            this.dgd_VisionList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dgd_VisionList.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgd_VisionList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            // 
            // dcVisionName
            // 
            this.dcVisionName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.dcVisionName, "dcVisionName");
            this.dcVisionName.Name = "dcVisionName";
            this.dcVisionName.ReadOnly = true;
            this.dcVisionName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // dcUnits
            // 
            this.dcUnits.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.NullValue = "1";
            this.dcUnits.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.dcUnits, "dcUnits");
            this.dcUnits.Name = "dcUnits";
            this.dcUnits.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // VisionID
            // 
            resources.ApplyResources(this.VisionID, "VisionID");
            this.VisionID.Name = "VisionID";
            // 
            // grp_ExportLotReport
            // 
            resources.ApplyResources(this.grp_ExportLotReport, "grp_ExportLotReport");
            this.grp_ExportLotReport.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.grp_ExportLotReport.Controls.Add(this.txt_IPAddress);
            this.grp_ExportLotReport.Controls.Add(this.srmLabel26);
            this.grp_ExportLotReport.Controls.Add(this.srmLabel4);
            this.grp_ExportLotReport.Controls.Add(this.cbo_ReportFormat);
            this.grp_ExportLotReport.Controls.Add(this.lbl_ReportPath);
            this.grp_ExportLotReport.Controls.Add(this.txt_TCPIPReportPath);
            this.grp_ExportLotReport.Controls.Add(this.btn_ReportPathButton);
            this.grp_ExportLotReport.Name = "grp_ExportLotReport";
            this.grp_ExportLotReport.TabStop = false;
            // 
            // txt_IPAddress
            // 
            resources.ApplyResources(this.txt_IPAddress, "txt_IPAddress");
            this.txt_IPAddress.BackColor = System.Drawing.Color.White;
            this.txt_IPAddress.DataType = SRMControl.SRMDataType.Int32;
            this.txt_IPAddress.DecimalPlaces = 0;
            this.txt_IPAddress.DecMaxValue = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.txt_IPAddress.DecMinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_IPAddress.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_IPAddress.ForeColor = System.Drawing.Color.Black;
            this.txt_IPAddress.Name = "txt_IPAddress";
            this.txt_IPAddress.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel26
            // 
            resources.ApplyResources(this.srmLabel26, "srmLabel26");
            this.srmLabel26.Name = "srmLabel26";
            this.srmLabel26.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel4
            // 
            resources.ApplyResources(this.srmLabel4, "srmLabel4");
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_ReportFormat
            // 
            resources.ApplyResources(this.cbo_ReportFormat, "cbo_ReportFormat");
            this.cbo_ReportFormat.BackColor = System.Drawing.Color.White;
            this.cbo_ReportFormat.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_ReportFormat.Items.AddRange(new object[] {
            resources.GetString("cbo_ReportFormat.Items"),
            resources.GetString("cbo_ReportFormat.Items1"),
            resources.GetString("cbo_ReportFormat.Items2")});
            this.cbo_ReportFormat.Name = "cbo_ReportFormat";
            this.cbo_ReportFormat.NormalBackColor = System.Drawing.Color.White;
            // 
            // lbl_ReportPath
            // 
            resources.ApplyResources(this.lbl_ReportPath, "lbl_ReportPath");
            this.lbl_ReportPath.Name = "lbl_ReportPath";
            this.lbl_ReportPath.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_TCPIPReportPath
            // 
            resources.ApplyResources(this.txt_TCPIPReportPath, "txt_TCPIPReportPath");
            this.txt_TCPIPReportPath.BackColor = System.Drawing.Color.White;
            this.txt_TCPIPReportPath.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_TCPIPReportPath.Name = "txt_TCPIPReportPath";
            this.txt_TCPIPReportPath.NormalBackColor = System.Drawing.Color.White;
            // 
            // btn_ReportPathButton
            // 
            resources.ApplyResources(this.btn_ReportPathButton, "btn_ReportPathButton");
            this.btn_ReportPathButton.Name = "btn_ReportPathButton";
            this.btn_ReportPathButton.Click += new System.EventHandler(this.btn_ReportPathButton_Click);
            // 
            // chk_ExportLotReport
            // 
            resources.ApplyResources(this.chk_ExportLotReport, "chk_ExportLotReport");
            this.chk_ExportLotReport.CheckedColor = System.Drawing.Color.Empty;
            this.chk_ExportLotReport.Name = "chk_ExportLotReport";
            this.chk_ExportLotReport.Selected = false;
            this.chk_ExportLotReport.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_ExportLotReport.UnCheckedColor = System.Drawing.Color.Empty;
            this.chk_ExportLotReport.Click += new System.EventHandler(this.chk_ExportLotReport_Click);
            // 
            // srmLabel39
            // 
            resources.ApplyResources(this.srmLabel39, "srmLabel39");
            this.srmLabel39.Name = "srmLabel39";
            this.srmLabel39.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_TCPIPRetriesCount
            // 
            resources.ApplyResources(this.txt_TCPIPRetriesCount, "txt_TCPIPRetriesCount");
            this.txt_TCPIPRetriesCount.BackColor = System.Drawing.Color.White;
            this.txt_TCPIPRetriesCount.DataType = SRMControl.SRMDataType.Int32;
            this.txt_TCPIPRetriesCount.DecimalPlaces = 0;
            this.txt_TCPIPRetriesCount.DecMaxValue = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.txt_TCPIPRetriesCount.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_TCPIPRetriesCount.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_TCPIPRetriesCount.ForeColor = System.Drawing.Color.Black;
            this.txt_TCPIPRetriesCount.InputType = SRMControl.InputType.Number;
            this.txt_TCPIPRetriesCount.Name = "txt_TCPIPRetriesCount";
            this.txt_TCPIPRetriesCount.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel40
            // 
            resources.ApplyResources(this.srmLabel40, "srmLabel40");
            this.srmLabel40.Name = "srmLabel40";
            this.srmLabel40.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel27
            // 
            resources.ApplyResources(this.srmLabel27, "srmLabel27");
            this.srmLabel27.Name = "srmLabel27";
            this.srmLabel27.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_TCPIPTimeout
            // 
            resources.ApplyResources(this.txt_TCPIPTimeout, "txt_TCPIPTimeout");
            this.txt_TCPIPTimeout.BackColor = System.Drawing.Color.White;
            this.txt_TCPIPTimeout.DataType = SRMControl.SRMDataType.Int32;
            this.txt_TCPIPTimeout.DecimalPlaces = 0;
            this.txt_TCPIPTimeout.DecMaxValue = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.txt_TCPIPTimeout.DecMinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_TCPIPTimeout.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_TCPIPTimeout.ForeColor = System.Drawing.Color.Black;
            this.txt_TCPIPTimeout.InputType = SRMControl.InputType.Number;
            this.txt_TCPIPTimeout.Name = "txt_TCPIPTimeout";
            this.txt_TCPIPTimeout.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel28
            // 
            resources.ApplyResources(this.srmLabel28, "srmLabel28");
            this.srmLabel28.Name = "srmLabel28";
            this.srmLabel28.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // chk_EnableTCPIP
            // 
            resources.ApplyResources(this.chk_EnableTCPIP, "chk_EnableTCPIP");
            this.chk_EnableTCPIP.CheckedColor = System.Drawing.Color.LawnGreen;
            this.chk_EnableTCPIP.Name = "chk_EnableTCPIP";
            this.chk_EnableTCPIP.Selected = false;
            this.chk_EnableTCPIP.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_EnableTCPIP.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_EnableTCPIP.Click += new System.EventHandler(this.chk_EnableTCPIP_Click);
            // 
            // txt_TCPIPPort
            // 
            resources.ApplyResources(this.txt_TCPIPPort, "txt_TCPIPPort");
            this.txt_TCPIPPort.BackColor = System.Drawing.Color.White;
            this.txt_TCPIPPort.DataType = SRMControl.SRMDataType.Int32;
            this.txt_TCPIPPort.DecimalPlaces = 0;
            this.txt_TCPIPPort.DecMaxValue = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.txt_TCPIPPort.DecMinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_TCPIPPort.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_TCPIPPort.ForeColor = System.Drawing.Color.Black;
            this.txt_TCPIPPort.InputType = SRMControl.InputType.Number;
            this.txt_TCPIPPort.Name = "txt_TCPIPPort";
            this.txt_TCPIPPort.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel33
            // 
            resources.ApplyResources(this.srmLabel33, "srmLabel33");
            this.srmLabel33.Name = "srmLabel33";
            this.srmLabel33.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tab_RS232
            // 
            resources.ApplyResources(this.tab_RS232, "tab_RS232");
            this.tab_RS232.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tab_RS232.Controls.Add(this.srmLabel37);
            this.tab_RS232.Controls.Add(this.txt_SerialPortRetriesCount);
            this.tab_RS232.Controls.Add(this.srmLabel38);
            this.tab_RS232.Controls.Add(this.srmLabel16);
            this.tab_RS232.Controls.Add(this.txt_SerialPortTimeOut);
            this.tab_RS232.Controls.Add(this.srmLabel36);
            this.tab_RS232.Controls.Add(this.cbo_CommPort);
            this.tab_RS232.Controls.Add(this.srmLabel35);
            this.tab_RS232.Controls.Add(this.chk_SerialPortEnable);
            this.tab_RS232.Controls.Add(this.groupBox2);
            this.tab_RS232.Name = "tab_RS232";
            // 
            // srmLabel37
            // 
            resources.ApplyResources(this.srmLabel37, "srmLabel37");
            this.srmLabel37.Name = "srmLabel37";
            this.srmLabel37.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_SerialPortRetriesCount
            // 
            resources.ApplyResources(this.txt_SerialPortRetriesCount, "txt_SerialPortRetriesCount");
            this.txt_SerialPortRetriesCount.BackColor = System.Drawing.Color.White;
            this.txt_SerialPortRetriesCount.DataType = SRMControl.SRMDataType.Int32;
            this.txt_SerialPortRetriesCount.DecimalPlaces = 0;
            this.txt_SerialPortRetriesCount.DecMaxValue = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.txt_SerialPortRetriesCount.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_SerialPortRetriesCount.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_SerialPortRetriesCount.ForeColor = System.Drawing.Color.Black;
            this.txt_SerialPortRetriesCount.InputType = SRMControl.InputType.Number;
            this.txt_SerialPortRetriesCount.Name = "txt_SerialPortRetriesCount";
            this.txt_SerialPortRetriesCount.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel38
            // 
            resources.ApplyResources(this.srmLabel38, "srmLabel38");
            this.srmLabel38.Name = "srmLabel38";
            this.srmLabel38.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel16
            // 
            resources.ApplyResources(this.srmLabel16, "srmLabel16");
            this.srmLabel16.Name = "srmLabel16";
            this.srmLabel16.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_SerialPortTimeOut
            // 
            resources.ApplyResources(this.txt_SerialPortTimeOut, "txt_SerialPortTimeOut");
            this.txt_SerialPortTimeOut.BackColor = System.Drawing.Color.White;
            this.txt_SerialPortTimeOut.DataType = SRMControl.SRMDataType.Int32;
            this.txt_SerialPortTimeOut.DecimalPlaces = 0;
            this.txt_SerialPortTimeOut.DecMaxValue = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.txt_SerialPortTimeOut.DecMinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_SerialPortTimeOut.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_SerialPortTimeOut.ForeColor = System.Drawing.Color.Black;
            this.txt_SerialPortTimeOut.InputType = SRMControl.InputType.Number;
            this.txt_SerialPortTimeOut.Name = "txt_SerialPortTimeOut";
            this.txt_SerialPortTimeOut.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel36
            // 
            resources.ApplyResources(this.srmLabel36, "srmLabel36");
            this.srmLabel36.Name = "srmLabel36";
            this.srmLabel36.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_CommPort
            // 
            resources.ApplyResources(this.cbo_CommPort, "cbo_CommPort");
            this.cbo_CommPort.BackColor = System.Drawing.Color.White;
            this.cbo_CommPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_CommPort.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_CommPort.Items.AddRange(new object[] {
            resources.GetString("cbo_CommPort.Items"),
            resources.GetString("cbo_CommPort.Items1"),
            resources.GetString("cbo_CommPort.Items2"),
            resources.GetString("cbo_CommPort.Items3"),
            resources.GetString("cbo_CommPort.Items4"),
            resources.GetString("cbo_CommPort.Items5"),
            resources.GetString("cbo_CommPort.Items6"),
            resources.GetString("cbo_CommPort.Items7"),
            resources.GetString("cbo_CommPort.Items8"),
            resources.GetString("cbo_CommPort.Items9")});
            this.cbo_CommPort.Name = "cbo_CommPort";
            this.cbo_CommPort.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel35
            // 
            resources.ApplyResources(this.srmLabel35, "srmLabel35");
            this.srmLabel35.Name = "srmLabel35";
            this.srmLabel35.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // chk_SerialPortEnable
            // 
            resources.ApplyResources(this.chk_SerialPortEnable, "chk_SerialPortEnable");
            this.chk_SerialPortEnable.CheckedColor = System.Drawing.Color.LawnGreen;
            this.chk_SerialPortEnable.Name = "chk_SerialPortEnable";
            this.chk_SerialPortEnable.Selected = false;
            this.chk_SerialPortEnable.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SerialPortEnable.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SerialPortEnable.Click += new System.EventHandler(this.chk_SerialPortEnable_Click);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.cbo_StopBits);
            this.groupBox2.Controls.Add(this.cbo_Parity);
            this.groupBox2.Controls.Add(this.cbo_DataBits);
            this.groupBox2.Controls.Add(this.srmLabel22);
            this.groupBox2.Controls.Add(this.srmLabel23);
            this.groupBox2.Controls.Add(this.srmLabel24);
            this.groupBox2.Controls.Add(this.cbo_BitsPerSecond);
            this.groupBox2.Controls.Add(this.srmLabel25);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // cbo_StopBits
            // 
            resources.ApplyResources(this.cbo_StopBits, "cbo_StopBits");
            this.cbo_StopBits.BackColor = System.Drawing.Color.White;
            this.cbo_StopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_StopBits.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_StopBits.Items.AddRange(new object[] {
            resources.GetString("cbo_StopBits.Items"),
            resources.GetString("cbo_StopBits.Items1"),
            resources.GetString("cbo_StopBits.Items2")});
            this.cbo_StopBits.Name = "cbo_StopBits";
            this.cbo_StopBits.NormalBackColor = System.Drawing.Color.White;
            // 
            // cbo_Parity
            // 
            resources.ApplyResources(this.cbo_Parity, "cbo_Parity");
            this.cbo_Parity.BackColor = System.Drawing.Color.White;
            this.cbo_Parity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Parity.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Parity.Items.AddRange(new object[] {
            resources.GetString("cbo_Parity.Items"),
            resources.GetString("cbo_Parity.Items1"),
            resources.GetString("cbo_Parity.Items2"),
            resources.GetString("cbo_Parity.Items3"),
            resources.GetString("cbo_Parity.Items4")});
            this.cbo_Parity.Name = "cbo_Parity";
            this.cbo_Parity.NormalBackColor = System.Drawing.Color.White;
            // 
            // cbo_DataBits
            // 
            resources.ApplyResources(this.cbo_DataBits, "cbo_DataBits");
            this.cbo_DataBits.BackColor = System.Drawing.Color.White;
            this.cbo_DataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_DataBits.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_DataBits.Items.AddRange(new object[] {
            resources.GetString("cbo_DataBits.Items"),
            resources.GetString("cbo_DataBits.Items1"),
            resources.GetString("cbo_DataBits.Items2"),
            resources.GetString("cbo_DataBits.Items3")});
            this.cbo_DataBits.Name = "cbo_DataBits";
            this.cbo_DataBits.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel22
            // 
            resources.ApplyResources(this.srmLabel22, "srmLabel22");
            this.srmLabel22.Name = "srmLabel22";
            this.srmLabel22.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel23
            // 
            resources.ApplyResources(this.srmLabel23, "srmLabel23");
            this.srmLabel23.Name = "srmLabel23";
            this.srmLabel23.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel24
            // 
            resources.ApplyResources(this.srmLabel24, "srmLabel24");
            this.srmLabel24.Name = "srmLabel24";
            this.srmLabel24.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_BitsPerSecond
            // 
            resources.ApplyResources(this.cbo_BitsPerSecond, "cbo_BitsPerSecond");
            this.cbo_BitsPerSecond.BackColor = System.Drawing.Color.White;
            this.cbo_BitsPerSecond.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_BitsPerSecond.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_BitsPerSecond.Items.AddRange(new object[] {
            resources.GetString("cbo_BitsPerSecond.Items"),
            resources.GetString("cbo_BitsPerSecond.Items1"),
            resources.GetString("cbo_BitsPerSecond.Items2"),
            resources.GetString("cbo_BitsPerSecond.Items3"),
            resources.GetString("cbo_BitsPerSecond.Items4"),
            resources.GetString("cbo_BitsPerSecond.Items5"),
            resources.GetString("cbo_BitsPerSecond.Items6"),
            resources.GetString("cbo_BitsPerSecond.Items7"),
            resources.GetString("cbo_BitsPerSecond.Items8"),
            resources.GetString("cbo_BitsPerSecond.Items9"),
            resources.GetString("cbo_BitsPerSecond.Items10"),
            resources.GetString("cbo_BitsPerSecond.Items11"),
            resources.GetString("cbo_BitsPerSecond.Items12")});
            this.cbo_BitsPerSecond.Name = "cbo_BitsPerSecond";
            this.cbo_BitsPerSecond.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel25
            // 
            resources.ApplyResources(this.srmLabel25, "srmLabel25");
            this.srmLabel25.Name = "srmLabel25";
            this.srmLabel25.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tab_camera
            // 
            resources.ApplyResources(this.tab_camera, "tab_camera");
            this.tab_camera.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tab_camera.Controls.Add(this.cbo_7);
            this.tab_camera.Controls.Add(this.cbo_6);
            this.tab_camera.Controls.Add(this.cbo_5);
            this.tab_camera.Controls.Add(this.cbo_Vision7);
            this.tab_camera.Controls.Add(this.cbo_Vision6);
            this.tab_camera.Controls.Add(this.cbo_Vision5);
            this.tab_camera.Controls.Add(this.lbl_Vision7Status);
            this.tab_camera.Controls.Add(this.lbl_Vision6Status);
            this.tab_camera.Controls.Add(this.lbl_Vision5Status);
            this.tab_camera.Controls.Add(this.lbl_Vision7);
            this.tab_camera.Controls.Add(this.lbl_Vision6);
            this.tab_camera.Controls.Add(this.lbl_Vision5);
            this.tab_camera.Controls.Add(this.cbo_4);
            this.tab_camera.Controls.Add(this.cbo_3);
            this.tab_camera.Controls.Add(this.cbo_2);
            this.tab_camera.Controls.Add(this.cbo_1);
            this.tab_camera.Controls.Add(this.btn_Connect);
            this.tab_camera.Controls.Add(this.cbo_Vision4);
            this.tab_camera.Controls.Add(this.cbo_Vision3);
            this.tab_camera.Controls.Add(this.cbo_Vision2);
            this.tab_camera.Controls.Add(this.cbo_Vision1);
            this.tab_camera.Controls.Add(this.lbl_Vision4Status);
            this.tab_camera.Controls.Add(this.lbl_Vision3Status);
            this.tab_camera.Controls.Add(this.lbl_Vision2Status);
            this.tab_camera.Controls.Add(this.lbl_Vision1Status);
            this.tab_camera.Controls.Add(this.lbl_Vision4);
            this.tab_camera.Controls.Add(this.lbl_Vision3);
            this.tab_camera.Controls.Add(this.lbl_Vision2);
            this.tab_camera.Controls.Add(this.lbl_Vision1);
            this.tab_camera.Controls.Add(this.srmLabel12);
            this.tab_camera.Controls.Add(this.srmLabel10);
            this.tab_camera.Controls.Add(this.srmLabel11);
            this.tab_camera.Name = "tab_camera";
            // 
            // cbo_7
            // 
            resources.ApplyResources(this.cbo_7, "cbo_7");
            this.cbo_7.BackColor = System.Drawing.Color.White;
            this.cbo_7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_7.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_7.Items.AddRange(new object[] {
            resources.GetString("cbo_7.Items")});
            this.cbo_7.Name = "cbo_7";
            this.cbo_7.NormalBackColor = System.Drawing.Color.White;
            // 
            // cbo_6
            // 
            resources.ApplyResources(this.cbo_6, "cbo_6");
            this.cbo_6.BackColor = System.Drawing.Color.White;
            this.cbo_6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_6.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_6.Items.AddRange(new object[] {
            resources.GetString("cbo_6.Items")});
            this.cbo_6.Name = "cbo_6";
            this.cbo_6.NormalBackColor = System.Drawing.Color.White;
            // 
            // cbo_5
            // 
            resources.ApplyResources(this.cbo_5, "cbo_5");
            this.cbo_5.BackColor = System.Drawing.Color.White;
            this.cbo_5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_5.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_5.Items.AddRange(new object[] {
            resources.GetString("cbo_5.Items")});
            this.cbo_5.Name = "cbo_5";
            this.cbo_5.NormalBackColor = System.Drawing.Color.White;
            // 
            // cbo_Vision7
            // 
            resources.ApplyResources(this.cbo_Vision7, "cbo_Vision7");
            this.cbo_Vision7.BackColor = System.Drawing.Color.White;
            this.cbo_Vision7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Vision7.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Vision7.Items.AddRange(new object[] {
            resources.GetString("cbo_Vision7.Items")});
            this.cbo_Vision7.Name = "cbo_Vision7";
            this.cbo_Vision7.NormalBackColor = System.Drawing.Color.White;
            this.cbo_Vision7.SelectedIndexChanged += new System.EventHandler(this.cbo_Vision7_SelectedIndexChanged);
            // 
            // cbo_Vision6
            // 
            resources.ApplyResources(this.cbo_Vision6, "cbo_Vision6");
            this.cbo_Vision6.BackColor = System.Drawing.Color.White;
            this.cbo_Vision6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Vision6.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Vision6.Items.AddRange(new object[] {
            resources.GetString("cbo_Vision6.Items")});
            this.cbo_Vision6.Name = "cbo_Vision6";
            this.cbo_Vision6.NormalBackColor = System.Drawing.Color.White;
            this.cbo_Vision6.SelectedIndexChanged += new System.EventHandler(this.cbo_Vision6_SelectedIndexChanged);
            // 
            // cbo_Vision5
            // 
            resources.ApplyResources(this.cbo_Vision5, "cbo_Vision5");
            this.cbo_Vision5.BackColor = System.Drawing.Color.White;
            this.cbo_Vision5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Vision5.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Vision5.Items.AddRange(new object[] {
            resources.GetString("cbo_Vision5.Items")});
            this.cbo_Vision5.Name = "cbo_Vision5";
            this.cbo_Vision5.NormalBackColor = System.Drawing.Color.White;
            this.cbo_Vision5.SelectedIndexChanged += new System.EventHandler(this.cbo_Vision5_SelectedIndexChanged);
            // 
            // lbl_Vision7Status
            // 
            resources.ApplyResources(this.lbl_Vision7Status, "lbl_Vision7Status");
            this.lbl_Vision7Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Vision7Status.ForeColor = System.Drawing.Color.Red;
            this.lbl_Vision7Status.Name = "lbl_Vision7Status";
            this.lbl_Vision7Status.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Vision6Status
            // 
            resources.ApplyResources(this.lbl_Vision6Status, "lbl_Vision6Status");
            this.lbl_Vision6Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Vision6Status.ForeColor = System.Drawing.Color.Red;
            this.lbl_Vision6Status.Name = "lbl_Vision6Status";
            this.lbl_Vision6Status.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Vision5Status
            // 
            resources.ApplyResources(this.lbl_Vision5Status, "lbl_Vision5Status");
            this.lbl_Vision5Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Vision5Status.ForeColor = System.Drawing.Color.Red;
            this.lbl_Vision5Status.Name = "lbl_Vision5Status";
            this.lbl_Vision5Status.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Vision7
            // 
            resources.ApplyResources(this.lbl_Vision7, "lbl_Vision7");
            this.lbl_Vision7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Vision7.Name = "lbl_Vision7";
            this.lbl_Vision7.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Vision6
            // 
            resources.ApplyResources(this.lbl_Vision6, "lbl_Vision6");
            this.lbl_Vision6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Vision6.Name = "lbl_Vision6";
            this.lbl_Vision6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Vision5
            // 
            resources.ApplyResources(this.lbl_Vision5, "lbl_Vision5");
            this.lbl_Vision5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Vision5.Name = "lbl_Vision5";
            this.lbl_Vision5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_4
            // 
            resources.ApplyResources(this.cbo_4, "cbo_4");
            this.cbo_4.BackColor = System.Drawing.Color.White;
            this.cbo_4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_4.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_4.Items.AddRange(new object[] {
            resources.GetString("cbo_4.Items")});
            this.cbo_4.Name = "cbo_4";
            this.cbo_4.NormalBackColor = System.Drawing.Color.White;
            // 
            // cbo_3
            // 
            resources.ApplyResources(this.cbo_3, "cbo_3");
            this.cbo_3.BackColor = System.Drawing.Color.White;
            this.cbo_3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_3.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_3.Items.AddRange(new object[] {
            resources.GetString("cbo_3.Items")});
            this.cbo_3.Name = "cbo_3";
            this.cbo_3.NormalBackColor = System.Drawing.Color.White;
            // 
            // cbo_2
            // 
            resources.ApplyResources(this.cbo_2, "cbo_2");
            this.cbo_2.BackColor = System.Drawing.Color.White;
            this.cbo_2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_2.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_2.Items.AddRange(new object[] {
            resources.GetString("cbo_2.Items")});
            this.cbo_2.Name = "cbo_2";
            this.cbo_2.NormalBackColor = System.Drawing.Color.White;
            // 
            // cbo_1
            // 
            resources.ApplyResources(this.cbo_1, "cbo_1");
            this.cbo_1.BackColor = System.Drawing.Color.White;
            this.cbo_1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_1.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_1.Items.AddRange(new object[] {
            resources.GetString("cbo_1.Items")});
            this.cbo_1.Name = "cbo_1";
            this.cbo_1.NormalBackColor = System.Drawing.Color.White;
            // 
            // btn_Connect
            // 
            resources.ApplyResources(this.btn_Connect, "btn_Connect");
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.UseVisualStyleBackColor = true;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // cbo_Vision4
            // 
            resources.ApplyResources(this.cbo_Vision4, "cbo_Vision4");
            this.cbo_Vision4.BackColor = System.Drawing.Color.White;
            this.cbo_Vision4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Vision4.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Vision4.Items.AddRange(new object[] {
            resources.GetString("cbo_Vision4.Items")});
            this.cbo_Vision4.Name = "cbo_Vision4";
            this.cbo_Vision4.NormalBackColor = System.Drawing.Color.White;
            this.cbo_Vision4.SelectedIndexChanged += new System.EventHandler(this.cbo_Vision4_SelectedIndexChanged);
            // 
            // cbo_Vision3
            // 
            resources.ApplyResources(this.cbo_Vision3, "cbo_Vision3");
            this.cbo_Vision3.BackColor = System.Drawing.Color.White;
            this.cbo_Vision3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Vision3.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Vision3.Items.AddRange(new object[] {
            resources.GetString("cbo_Vision3.Items")});
            this.cbo_Vision3.Name = "cbo_Vision3";
            this.cbo_Vision3.NormalBackColor = System.Drawing.Color.White;
            this.cbo_Vision3.SelectedIndexChanged += new System.EventHandler(this.cbo_Vision3_SelectedIndexChanged);
            // 
            // cbo_Vision2
            // 
            resources.ApplyResources(this.cbo_Vision2, "cbo_Vision2");
            this.cbo_Vision2.BackColor = System.Drawing.Color.White;
            this.cbo_Vision2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Vision2.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Vision2.Items.AddRange(new object[] {
            resources.GetString("cbo_Vision2.Items")});
            this.cbo_Vision2.Name = "cbo_Vision2";
            this.cbo_Vision2.NormalBackColor = System.Drawing.Color.White;
            this.cbo_Vision2.SelectedIndexChanged += new System.EventHandler(this.cbo_Vision2_SelectedIndexChanged);
            // 
            // cbo_Vision1
            // 
            resources.ApplyResources(this.cbo_Vision1, "cbo_Vision1");
            this.cbo_Vision1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Vision1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Vision1.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Vision1.Items.AddRange(new object[] {
            resources.GetString("cbo_Vision1.Items")});
            this.cbo_Vision1.Name = "cbo_Vision1";
            this.cbo_Vision1.NormalBackColor = System.Drawing.Color.White;
            this.cbo_Vision1.SelectedIndexChanged += new System.EventHandler(this.cbo_Vision1_SelectedIndexChanged);
            // 
            // lbl_Vision4Status
            // 
            resources.ApplyResources(this.lbl_Vision4Status, "lbl_Vision4Status");
            this.lbl_Vision4Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Vision4Status.ForeColor = System.Drawing.Color.Red;
            this.lbl_Vision4Status.Name = "lbl_Vision4Status";
            this.lbl_Vision4Status.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Vision3Status
            // 
            resources.ApplyResources(this.lbl_Vision3Status, "lbl_Vision3Status");
            this.lbl_Vision3Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Vision3Status.ForeColor = System.Drawing.Color.Red;
            this.lbl_Vision3Status.Name = "lbl_Vision3Status";
            this.lbl_Vision3Status.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Vision2Status
            // 
            resources.ApplyResources(this.lbl_Vision2Status, "lbl_Vision2Status");
            this.lbl_Vision2Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Vision2Status.ForeColor = System.Drawing.Color.Red;
            this.lbl_Vision2Status.Name = "lbl_Vision2Status";
            this.lbl_Vision2Status.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Vision1Status
            // 
            resources.ApplyResources(this.lbl_Vision1Status, "lbl_Vision1Status");
            this.lbl_Vision1Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Vision1Status.ForeColor = System.Drawing.Color.Red;
            this.lbl_Vision1Status.Name = "lbl_Vision1Status";
            this.lbl_Vision1Status.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Vision4
            // 
            resources.ApplyResources(this.lbl_Vision4, "lbl_Vision4");
            this.lbl_Vision4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Vision4.Name = "lbl_Vision4";
            this.lbl_Vision4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Vision3
            // 
            resources.ApplyResources(this.lbl_Vision3, "lbl_Vision3");
            this.lbl_Vision3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Vision3.Name = "lbl_Vision3";
            this.lbl_Vision3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Vision2
            // 
            resources.ApplyResources(this.lbl_Vision2, "lbl_Vision2");
            this.lbl_Vision2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Vision2.Name = "lbl_Vision2";
            this.lbl_Vision2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Vision1
            // 
            resources.ApplyResources(this.lbl_Vision1, "lbl_Vision1");
            this.lbl_Vision1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Vision1.Name = "lbl_Vision1";
            this.lbl_Vision1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel12
            // 
            resources.ApplyResources(this.srmLabel12, "srmLabel12");
            this.srmLabel12.Name = "srmLabel12";
            this.srmLabel12.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel10
            // 
            resources.ApplyResources(this.srmLabel10, "srmLabel10");
            this.srmLabel10.Name = "srmLabel10";
            this.srmLabel10.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel11
            // 
            resources.ApplyResources(this.srmLabel11, "srmLabel11");
            this.srmLabel11.Name = "srmLabel11";
            this.srmLabel11.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tab_Network
            // 
            resources.ApplyResources(this.tab_Network, "tab_Network");
            this.tab_Network.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tab_Network.Controls.Add(this.chk_WantUseNetwork);
            this.tab_Network.Controls.Add(this.chk_NetworkPasswordLimitCheckBox);
            this.tab_Network.Controls.Add(this.txt_VisionLotReportUploadDirEditBox);
            this.tab_Network.Controls.Add(this.txt_NetworkPasswordEditBox);
            this.tab_Network.Controls.Add(this.txt_DeviceUploadDirEditBox);
            this.tab_Network.Controls.Add(this.btn_VisionLotReportUploadBrowseButton);
            this.tab_Network.Controls.Add(this.srmLabel13);
            this.tab_Network.Controls.Add(this.txt_NetworkUsernameEditBox);
            this.tab_Network.Controls.Add(this.txt_HostIPEditBox);
            this.tab_Network.Controls.Add(this.srmLabel9);
            this.tab_Network.Controls.Add(this.btn_DeviceUploadBrowseButton);
            this.tab_Network.Controls.Add(this.srmLabel14);
            this.tab_Network.Controls.Add(this.srmLabel15);
            this.tab_Network.Controls.Add(this.srmLabel17);
            this.tab_Network.Name = "tab_Network";
            // 
            // chk_WantUseNetwork
            // 
            resources.ApplyResources(this.chk_WantUseNetwork, "chk_WantUseNetwork");
            this.chk_WantUseNetwork.CheckedColor = System.Drawing.Color.Empty;
            this.chk_WantUseNetwork.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chk_WantUseNetwork.Name = "chk_WantUseNetwork";
            this.chk_WantUseNetwork.Selected = false;
            this.chk_WantUseNetwork.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_WantUseNetwork.UnCheckedColor = System.Drawing.Color.Empty;
            // 
            // chk_NetworkPasswordLimitCheckBox
            // 
            resources.ApplyResources(this.chk_NetworkPasswordLimitCheckBox, "chk_NetworkPasswordLimitCheckBox");
            this.chk_NetworkPasswordLimitCheckBox.CheckedColor = System.Drawing.Color.Empty;
            this.chk_NetworkPasswordLimitCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chk_NetworkPasswordLimitCheckBox.Name = "chk_NetworkPasswordLimitCheckBox";
            this.chk_NetworkPasswordLimitCheckBox.Selected = false;
            this.chk_NetworkPasswordLimitCheckBox.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_NetworkPasswordLimitCheckBox.UnCheckedColor = System.Drawing.Color.Empty;
            // 
            // txt_VisionLotReportUploadDirEditBox
            // 
            resources.ApplyResources(this.txt_VisionLotReportUploadDirEditBox, "txt_VisionLotReportUploadDirEditBox");
            this.txt_VisionLotReportUploadDirEditBox.BackColor = System.Drawing.Color.White;
            this.txt_VisionLotReportUploadDirEditBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_VisionLotReportUploadDirEditBox.Name = "txt_VisionLotReportUploadDirEditBox";
            this.txt_VisionLotReportUploadDirEditBox.NormalBackColor = System.Drawing.Color.White;
            // 
            // txt_NetworkPasswordEditBox
            // 
            resources.ApplyResources(this.txt_NetworkPasswordEditBox, "txt_NetworkPasswordEditBox");
            this.txt_NetworkPasswordEditBox.BackColor = System.Drawing.Color.White;
            this.txt_NetworkPasswordEditBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_NetworkPasswordEditBox.Name = "txt_NetworkPasswordEditBox";
            this.txt_NetworkPasswordEditBox.NormalBackColor = System.Drawing.Color.White;
            this.txt_NetworkPasswordEditBox.UseSystemPasswordChar = true;
            // 
            // txt_DeviceUploadDirEditBox
            // 
            resources.ApplyResources(this.txt_DeviceUploadDirEditBox, "txt_DeviceUploadDirEditBox");
            this.txt_DeviceUploadDirEditBox.BackColor = System.Drawing.Color.White;
            this.txt_DeviceUploadDirEditBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_DeviceUploadDirEditBox.Name = "txt_DeviceUploadDirEditBox";
            this.txt_DeviceUploadDirEditBox.NormalBackColor = System.Drawing.Color.White;
            // 
            // btn_VisionLotReportUploadBrowseButton
            // 
            resources.ApplyResources(this.btn_VisionLotReportUploadBrowseButton, "btn_VisionLotReportUploadBrowseButton");
            this.btn_VisionLotReportUploadBrowseButton.Name = "btn_VisionLotReportUploadBrowseButton";
            this.btn_VisionLotReportUploadBrowseButton.UseVisualStyleBackColor = true;
            this.btn_VisionLotReportUploadBrowseButton.Click += new System.EventHandler(this.VisionLotReportUploadBrowseButton_Click);
            // 
            // srmLabel13
            // 
            resources.ApplyResources(this.srmLabel13, "srmLabel13");
            this.srmLabel13.Name = "srmLabel13";
            this.srmLabel13.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_NetworkUsernameEditBox
            // 
            resources.ApplyResources(this.txt_NetworkUsernameEditBox, "txt_NetworkUsernameEditBox");
            this.txt_NetworkUsernameEditBox.BackColor = System.Drawing.Color.White;
            this.txt_NetworkUsernameEditBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_NetworkUsernameEditBox.Name = "txt_NetworkUsernameEditBox";
            this.txt_NetworkUsernameEditBox.NormalBackColor = System.Drawing.Color.White;
            // 
            // txt_HostIPEditBox
            // 
            resources.ApplyResources(this.txt_HostIPEditBox, "txt_HostIPEditBox");
            this.txt_HostIPEditBox.BackColor = System.Drawing.Color.White;
            this.txt_HostIPEditBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_HostIPEditBox.Name = "txt_HostIPEditBox";
            this.txt_HostIPEditBox.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel9
            // 
            resources.ApplyResources(this.srmLabel9, "srmLabel9");
            this.srmLabel9.Name = "srmLabel9";
            this.srmLabel9.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_DeviceUploadBrowseButton
            // 
            resources.ApplyResources(this.btn_DeviceUploadBrowseButton, "btn_DeviceUploadBrowseButton");
            this.btn_DeviceUploadBrowseButton.Name = "btn_DeviceUploadBrowseButton";
            this.btn_DeviceUploadBrowseButton.UseVisualStyleBackColor = true;
            this.btn_DeviceUploadBrowseButton.Click += new System.EventHandler(this.DeviceUploadBrowseButton_Click);
            // 
            // srmLabel14
            // 
            resources.ApplyResources(this.srmLabel14, "srmLabel14");
            this.srmLabel14.Name = "srmLabel14";
            this.srmLabel14.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel15
            // 
            resources.ApplyResources(this.srmLabel15, "srmLabel15");
            this.srmLabel15.Name = "srmLabel15";
            this.srmLabel15.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel17
            // 
            resources.ApplyResources(this.srmLabel17, "srmLabel17");
            this.srmLabel17.Name = "srmLabel17";
            this.srmLabel17.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tab_SECSGEM
            // 
            resources.ApplyResources(this.tab_SECSGEM, "tab_SECSGEM");
            this.tab_SECSGEM.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tab_SECSGEM.Controls.Add(this.srmLabel19);
            this.tab_SECSGEM.Controls.Add(this.txt_MaxNoOfCoplanPad);
            this.tab_SECSGEM.Controls.Add(this.chk_WantUseSECSGEM);
            this.tab_SECSGEM.Controls.Add(this.txt_SECSGEMSharedFolderPath);
            this.tab_SECSGEM.Controls.Add(this.btn_BrowseSECSGEMSharedFolderPath);
            this.tab_SECSGEM.Controls.Add(this.srmLabel18);
            this.tab_SECSGEM.Name = "tab_SECSGEM";
            // 
            // srmLabel19
            // 
            resources.ApplyResources(this.srmLabel19, "srmLabel19");
            this.srmLabel19.Name = "srmLabel19";
            this.srmLabel19.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MaxNoOfCoplanPad
            // 
            resources.ApplyResources(this.txt_MaxNoOfCoplanPad, "txt_MaxNoOfCoplanPad");
            this.txt_MaxNoOfCoplanPad.BackColor = System.Drawing.Color.White;
            this.txt_MaxNoOfCoplanPad.DataType = SRMControl.SRMDataType.Int32;
            this.txt_MaxNoOfCoplanPad.DecimalPlaces = 0;
            this.txt_MaxNoOfCoplanPad.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_MaxNoOfCoplanPad.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MaxNoOfCoplanPad.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MaxNoOfCoplanPad.ForeColor = System.Drawing.Color.Black;
            this.txt_MaxNoOfCoplanPad.InputType = SRMControl.InputType.Number;
            this.txt_MaxNoOfCoplanPad.Name = "txt_MaxNoOfCoplanPad";
            this.txt_MaxNoOfCoplanPad.NormalBackColor = System.Drawing.Color.White;
            // 
            // chk_WantUseSECSGEM
            // 
            resources.ApplyResources(this.chk_WantUseSECSGEM, "chk_WantUseSECSGEM");
            this.chk_WantUseSECSGEM.CheckedColor = System.Drawing.Color.Empty;
            this.chk_WantUseSECSGEM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chk_WantUseSECSGEM.Name = "chk_WantUseSECSGEM";
            this.chk_WantUseSECSGEM.Selected = false;
            this.chk_WantUseSECSGEM.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_WantUseSECSGEM.UnCheckedColor = System.Drawing.Color.Empty;
            // 
            // txt_SECSGEMSharedFolderPath
            // 
            resources.ApplyResources(this.txt_SECSGEMSharedFolderPath, "txt_SECSGEMSharedFolderPath");
            this.txt_SECSGEMSharedFolderPath.BackColor = System.Drawing.Color.White;
            this.txt_SECSGEMSharedFolderPath.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_SECSGEMSharedFolderPath.Name = "txt_SECSGEMSharedFolderPath";
            this.txt_SECSGEMSharedFolderPath.NormalBackColor = System.Drawing.Color.White;
            // 
            // btn_BrowseSECSGEMSharedFolderPath
            // 
            resources.ApplyResources(this.btn_BrowseSECSGEMSharedFolderPath, "btn_BrowseSECSGEMSharedFolderPath");
            this.btn_BrowseSECSGEMSharedFolderPath.Name = "btn_BrowseSECSGEMSharedFolderPath";
            this.btn_BrowseSECSGEMSharedFolderPath.UseVisualStyleBackColor = true;
            this.btn_BrowseSECSGEMSharedFolderPath.Click += new System.EventHandler(this.btn_BrowseSECSGEMSharedFolderPath_Click);
            // 
            // srmLabel18
            // 
            resources.ApplyResources(this.srmLabel18, "srmLabel18");
            this.srmLabel18.Name = "srmLabel18";
            this.srmLabel18.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Name = "btn_Cancel";
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // dlg_ReportFolder
            // 
            resources.ApplyResources(this.dlg_ReportFolder, "dlg_ReportFolder");
            // 
            // NetworkFolderBrowserDialog
            // 
            resources.ApplyResources(this.NetworkFolderBrowserDialog, "NetworkFolderBrowserDialog");
            this.NetworkFolderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // AppConfigForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.tabCtrl_Configuration);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Name = "AppConfigForm";
            this.Load += new System.EventHandler(this.AppConfigForm_Load);
            this.tabCtrl_Configuration.ResumeLayout(false);
            this.tab_General.ResumeLayout(false);
            this.tab_General.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_AutoLogOutMinutes)).EndInit();
            this.group_PassImage.ResumeLayout(false);
            this.group_PassImage.PerformLayout();
            this.srmGroupBox3.ResumeLayout(false);
            this.tab_TCPIP.ResumeLayout(false);
            this.tab_TCPIP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_VisionList)).EndInit();
            this.grp_ExportLotReport.ResumeLayout(false);
            this.grp_ExportLotReport.PerformLayout();
            this.tab_RS232.ResumeLayout(false);
            this.tab_RS232.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tab_camera.ResumeLayout(false);
            this.tab_camera.PerformLayout();
            this.tab_Network.ResumeLayout(false);
            this.tab_Network.PerformLayout();
            this.tab_SECSGEM.ResumeLayout(false);
            this.tab_SECSGEM.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMTabControl tabCtrl_Configuration;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private System.Windows.Forms.TabPage tab_General;
        private SRMControl.SRMCheckBox chk_DebugMode;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMInputBox txt_WrongFaceTole;
        private SRMControl.SRMInputBox txt_EmptyPocketTole;
        private System.Windows.Forms.TabPage tab_TCPIP;
        private SRMControl.SRMButton btn_ReportPathButton;
        private SRMControl.SRMTextBox txt_TCPIPReportPath;
        private SRMControl.SRMLabel lbl_ReportPath;
        private SRMControl.SRMLabel srmLabel27;
        private SRMControl.SRMInputBox txt_TCPIPTimeout;
        private SRMControl.SRMLabel srmLabel28;
        private SRMControl.SRMCheckBox chk_EnableTCPIP;
        private SRMControl.SRMInputBox txt_TCPIPPort;
        private SRMControl.SRMLabel srmLabel33;
        private SRMControl.SRMCheckBox chk_ExportLotReport;
        private System.Windows.Forms.TabPage tab_RS232;
        private SRMControl.SRMLabel srmLabel37;
        private SRMControl.SRMInputBox txt_SerialPortRetriesCount;
        private SRMControl.SRMLabel srmLabel38;
        private SRMControl.SRMLabel srmLabel16;
        private SRMControl.SRMInputBox txt_SerialPortTimeOut;
        private SRMControl.SRMLabel srmLabel36;
        private SRMControl.SRMComboBox cbo_CommPort;
        private SRMControl.SRMLabel srmLabel35;
        private SRMControl.SRMCheckBox chk_SerialPortEnable;
        private System.Windows.Forms.GroupBox groupBox2;
        private SRMControl.SRMComboBox cbo_StopBits;
        private SRMControl.SRMComboBox cbo_Parity;
        private SRMControl.SRMComboBox cbo_DataBits;
        private SRMControl.SRMLabel srmLabel22;
        private SRMControl.SRMLabel srmLabel23;
        private SRMControl.SRMLabel srmLabel24;
        private SRMControl.SRMComboBox cbo_BitsPerSecond;
        private SRMControl.SRMLabel srmLabel25;
        private SRMControl.SRMGroupBox group_PassImage;
        private SRMControl.SRMGroupBox grp_ExportLotReport;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMComboBox cbo_ReportFormat;
        private System.Windows.Forms.FolderBrowserDialog dlg_ReportFolder;
        private SRMControl.SRMLabel srmLabel39;
        private SRMControl.SRMInputBox txt_TCPIPRetriesCount;
        private SRMControl.SRMLabel srmLabel40;
        private SRMControl.SRMInputBox txt_PassImagePics;
        private SRMControl.SRMCheckBox chk_SavePassImage;
        private SRMControl.SRMGroupBox srmGroupBox3;
        private SRMControl.SRMRadioButton radioBtn_Last;
        private SRMControl.SRMRadioButton radioBtn_First;
        private SRMControl.SRMCheckBox chk_SaveFailImage;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMInputBox txt_FailImagePics;
        private SRMControl.SRMLabel srmLabel5;
        private SRMControl.SRMCheckBox chk_StopLowYield;
        private SRMControl.SRMInputBox txt_MinUnitCheck;
        private SRMControl.SRMLabel srmLabel7;
        private SRMControl.SRMInputBox txt_LowYield;
        private SRMControl.SRMLabel srmLabel6;
        private System.Windows.Forms.DataGridView dgd_VisionList;
        private SRMControl.SRMLabel srmLabel8;
        private SRMControl.SRMComboBox cbo_UnitDisplay;
        private SRMControl.SRMCheckBox chk_PreviousVersion;
        private SRMControl.SRMComboBox cbo_PreviousVersion;
        private System.Windows.Forms.TabPage tab_camera;
        private SRMControl.SRMLabel lbl_Vision4;
        private SRMControl.SRMLabel lbl_Vision3;
        private SRMControl.SRMLabel lbl_Vision2;
        private SRMControl.SRMLabel lbl_Vision1;
        private SRMControl.SRMLabel srmLabel12;
        private SRMControl.SRMLabel srmLabel10;
        private SRMControl.SRMLabel srmLabel11;
        private SRMControl.SRMComboBox cbo_Vision4;
        private SRMControl.SRMComboBox cbo_Vision3;
        private SRMControl.SRMComboBox cbo_Vision2;
        private SRMControl.SRMComboBox cbo_Vision1;
        private SRMControl.SRMLabel lbl_Vision4Status;
        private SRMControl.SRMLabel lbl_Vision3Status;
        private SRMControl.SRMLabel lbl_Vision2Status;
        private SRMControl.SRMLabel lbl_Vision1Status;
        private SRMControl.SRMButton btn_Connect;
        private SRMControl.SRMComboBox cbo_4;
        private SRMControl.SRMComboBox cbo_3;
        private SRMControl.SRMComboBox cbo_2;
        private SRMControl.SRMComboBox cbo_1;
        private SRMControl.SRMComboBox cbo_Vision7;
        private SRMControl.SRMComboBox cbo_Vision6;
        private SRMControl.SRMComboBox cbo_Vision5;
        private SRMControl.SRMLabel lbl_Vision7Status;
        private SRMControl.SRMLabel lbl_Vision6Status;
        private SRMControl.SRMLabel lbl_Vision5Status;
        private SRMControl.SRMLabel lbl_Vision7;
        private SRMControl.SRMLabel lbl_Vision6;
        private SRMControl.SRMLabel lbl_Vision5;
        private SRMControl.SRMComboBox cbo_7;
        private SRMControl.SRMComboBox cbo_6;
        private SRMControl.SRMComboBox cbo_5;
        private System.Windows.Forms.TabPage tab_Network;
        private SRMControl.SRMTextBox txt_VisionLotReportUploadDirEditBox;
        private SRMControl.SRMButton btn_VisionLotReportUploadBrowseButton;
        private SRMControl.SRMLabel srmLabel13;
        private System.Windows.Forms.FolderBrowserDialog NetworkFolderBrowserDialog;
        private SRMControl.SRMCheckBox chk_SaveErrorMessge;
        private SRMControl.SRMCheckBox chk_WantUseNetwork;
        private SRMControl.SRMCheckBox chk_NetworkPasswordLimitCheckBox;
        private SRMControl.SRMTextBox txt_NetworkPasswordEditBox;
        private SRMControl.SRMTextBox txt_DeviceUploadDirEditBox;
        private SRMControl.SRMTextBox txt_NetworkUsernameEditBox;
        private SRMControl.SRMTextBox txt_HostIPEditBox;
        private SRMControl.SRMLabel srmLabel9;
        private SRMControl.SRMButton btn_DeviceUploadBrowseButton;
        private SRMControl.SRMLabel srmLabel14;
        private SRMControl.SRMLabel srmLabel15;
        private SRMControl.SRMLabel srmLabel17;
        private System.Windows.Forms.TabPage tab_SECSGEM;
        private SRMControl.SRMTextBox txt_SECSGEMSharedFolderPath;
        private SRMControl.SRMButton btn_BrowseSECSGEMSharedFolderPath;
        private SRMControl.SRMLabel srmLabel18;
        private SRMControl.SRMCheckBox chk_WantUseSECSGEM;
        private SRMControl.SRMLabel srmLabel19;
        private SRMControl.SRMInputBox txt_MaxNoOfCoplanPad;
        private SRMControl.SRMLabel srmLabel21;
        private SRMControl.SRMLabel srmLabel20;
        private System.Windows.Forms.NumericUpDown txt_AutoLogOutMinutes;
        private SRMControl.SRMInputBox txt_IPAddress;
        private SRMControl.SRMLabel srmLabel26;
        private SRMControl.SRMCheckBox chk_GlobalSharingCalibrationData;
        private SRMControl.SRMComboBox cbo_MarkUnitDisplay;
        private SRMControl.SRMLabel srmLabel29;
        private SRMControl.SRMCheckBox chk_GlobalSharingCameraData;
        private System.Windows.Forms.DataGridViewTextBoxColumn dcVisionName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dcUnits;
        private System.Windows.Forms.DataGridViewTextBoxColumn VisionID;
    }
}