namespace VisionProcessForm
{
    partial class PadSubSettingForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PadSubSettingForm));
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.panel_SelectEdge = new System.Windows.Forms.Panel();
            this.srmLabel13 = new SRMControl.SRMLabel();
            this.chk_AutoSensitivity_Left = new SRMControl.SRMCheckBox();
            this.chk_AutoSensitivity_Bottom = new SRMControl.SRMCheckBox();
            this.chk_AutoSensitivity_Right = new SRMControl.SRMCheckBox();
            this.chk_AutoSensitivity_Top = new SRMControl.SRMCheckBox();
            this.chk_AutoSensitivity_Center = new SRMControl.SRMCheckBox();
            this.srmLabel10 = new SRMControl.SRMLabel();
            this.txt_SensitivityValue_Left = new SRMControl.SRMInputBox();
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.cbo_SensitivityOnPad_Left = new SRMControl.SRMComboBox();
            this.srmLabel9 = new SRMControl.SRMLabel();
            this.txt_SensitivityValue_Bottom = new SRMControl.SRMInputBox();
            this.srmLabel6 = new SRMControl.SRMLabel();
            this.cbo_SensitivityOnPad_Bottom = new SRMControl.SRMComboBox();
            this.srmLabel7 = new SRMControl.SRMLabel();
            this.txt_SensitivityValue_Right = new SRMControl.SRMInputBox();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.cbo_SensitivityOnPad_Right = new SRMControl.SRMComboBox();
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.txt_SensitivityValue_Top = new SRMControl.SRMInputBox();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.cbo_SensitivityOnPad_Top = new SRMControl.SRMComboBox();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.srmLabel11 = new SRMControl.SRMLabel();
            this.txt_SensitivityValue_Center = new SRMControl.SRMInputBox();
            this.srmLabel12 = new SRMControl.SRMLabel();
            this.cbo_SensitivityOnPad_Center = new SRMControl.SRMComboBox();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.ils_ImageListTree = new System.Windows.Forms.ImageList(this.components);
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.panel_SelectEdge.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // panel_SelectEdge
            // 
            resources.ApplyResources(this.panel_SelectEdge, "panel_SelectEdge");
            this.panel_SelectEdge.Controls.Add(this.srmLabel13);
            this.panel_SelectEdge.Controls.Add(this.chk_AutoSensitivity_Left);
            this.panel_SelectEdge.Controls.Add(this.chk_AutoSensitivity_Bottom);
            this.panel_SelectEdge.Controls.Add(this.chk_AutoSensitivity_Right);
            this.panel_SelectEdge.Controls.Add(this.chk_AutoSensitivity_Top);
            this.panel_SelectEdge.Controls.Add(this.chk_AutoSensitivity_Center);
            this.panel_SelectEdge.Controls.Add(this.srmLabel10);
            this.panel_SelectEdge.Controls.Add(this.txt_SensitivityValue_Left);
            this.panel_SelectEdge.Controls.Add(this.srmLabel8);
            this.panel_SelectEdge.Controls.Add(this.cbo_SensitivityOnPad_Left);
            this.panel_SelectEdge.Controls.Add(this.srmLabel9);
            this.panel_SelectEdge.Controls.Add(this.txt_SensitivityValue_Bottom);
            this.panel_SelectEdge.Controls.Add(this.srmLabel6);
            this.panel_SelectEdge.Controls.Add(this.cbo_SensitivityOnPad_Bottom);
            this.panel_SelectEdge.Controls.Add(this.srmLabel7);
            this.panel_SelectEdge.Controls.Add(this.txt_SensitivityValue_Right);
            this.panel_SelectEdge.Controls.Add(this.srmLabel4);
            this.panel_SelectEdge.Controls.Add(this.cbo_SensitivityOnPad_Right);
            this.panel_SelectEdge.Controls.Add(this.srmLabel5);
            this.panel_SelectEdge.Controls.Add(this.txt_SensitivityValue_Top);
            this.panel_SelectEdge.Controls.Add(this.srmLabel2);
            this.panel_SelectEdge.Controls.Add(this.cbo_SensitivityOnPad_Top);
            this.panel_SelectEdge.Controls.Add(this.srmLabel3);
            this.panel_SelectEdge.Controls.Add(this.srmLabel11);
            this.panel_SelectEdge.Controls.Add(this.txt_SensitivityValue_Center);
            this.panel_SelectEdge.Controls.Add(this.srmLabel12);
            this.panel_SelectEdge.Controls.Add(this.cbo_SensitivityOnPad_Center);
            this.panel_SelectEdge.Controls.Add(this.srmLabel1);
            this.panel_SelectEdge.Name = "panel_SelectEdge";
            // 
            // srmLabel13
            // 
            resources.ApplyResources(this.srmLabel13, "srmLabel13");
            this.srmLabel13.Name = "srmLabel13";
            this.srmLabel13.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // chk_AutoSensitivity_Left
            // 
            resources.ApplyResources(this.chk_AutoSensitivity_Left, "chk_AutoSensitivity_Left");
            this.chk_AutoSensitivity_Left.Checked = true;
            this.chk_AutoSensitivity_Left.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_AutoSensitivity_Left.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_AutoSensitivity_Left.Name = "chk_AutoSensitivity_Left";
            this.chk_AutoSensitivity_Left.Selected = false;
            this.chk_AutoSensitivity_Left.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_AutoSensitivity_Left.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_AutoSensitivity_Left.UseVisualStyleBackColor = true;
            // 
            // chk_AutoSensitivity_Bottom
            // 
            resources.ApplyResources(this.chk_AutoSensitivity_Bottom, "chk_AutoSensitivity_Bottom");
            this.chk_AutoSensitivity_Bottom.Checked = true;
            this.chk_AutoSensitivity_Bottom.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_AutoSensitivity_Bottom.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_AutoSensitivity_Bottom.Name = "chk_AutoSensitivity_Bottom";
            this.chk_AutoSensitivity_Bottom.Selected = false;
            this.chk_AutoSensitivity_Bottom.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_AutoSensitivity_Bottom.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_AutoSensitivity_Bottom.UseVisualStyleBackColor = true;
            // 
            // chk_AutoSensitivity_Right
            // 
            resources.ApplyResources(this.chk_AutoSensitivity_Right, "chk_AutoSensitivity_Right");
            this.chk_AutoSensitivity_Right.Checked = true;
            this.chk_AutoSensitivity_Right.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_AutoSensitivity_Right.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_AutoSensitivity_Right.Name = "chk_AutoSensitivity_Right";
            this.chk_AutoSensitivity_Right.Selected = false;
            this.chk_AutoSensitivity_Right.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_AutoSensitivity_Right.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_AutoSensitivity_Right.UseVisualStyleBackColor = true;
            // 
            // chk_AutoSensitivity_Top
            // 
            resources.ApplyResources(this.chk_AutoSensitivity_Top, "chk_AutoSensitivity_Top");
            this.chk_AutoSensitivity_Top.Checked = true;
            this.chk_AutoSensitivity_Top.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_AutoSensitivity_Top.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_AutoSensitivity_Top.Name = "chk_AutoSensitivity_Top";
            this.chk_AutoSensitivity_Top.Selected = false;
            this.chk_AutoSensitivity_Top.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_AutoSensitivity_Top.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_AutoSensitivity_Top.UseVisualStyleBackColor = true;
            this.chk_AutoSensitivity_Top.CheckedChanged += new System.EventHandler(this.srmCheckBox1_CheckedChanged);
            // 
            // chk_AutoSensitivity_Center
            // 
            resources.ApplyResources(this.chk_AutoSensitivity_Center, "chk_AutoSensitivity_Center");
            this.chk_AutoSensitivity_Center.Checked = true;
            this.chk_AutoSensitivity_Center.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_AutoSensitivity_Center.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_AutoSensitivity_Center.Name = "chk_AutoSensitivity_Center";
            this.chk_AutoSensitivity_Center.Selected = false;
            this.chk_AutoSensitivity_Center.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_AutoSensitivity_Center.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_AutoSensitivity_Center.UseVisualStyleBackColor = true;
            // 
            // srmLabel10
            // 
            resources.ApplyResources(this.srmLabel10, "srmLabel10");
            this.srmLabel10.Name = "srmLabel10";
            this.srmLabel10.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_SensitivityValue_Left
            // 
            resources.ApplyResources(this.txt_SensitivityValue_Left, "txt_SensitivityValue_Left");
            this.txt_SensitivityValue_Left.BackColor = System.Drawing.Color.White;
            this.txt_SensitivityValue_Left.DecimalPlaces = 0;
            this.txt_SensitivityValue_Left.DecMaxValue = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_SensitivityValue_Left.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_SensitivityValue_Left.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_SensitivityValue_Left.ForeColor = System.Drawing.Color.Black;
            this.txt_SensitivityValue_Left.InputType = SRMControl.InputType.Number;
            this.txt_SensitivityValue_Left.Name = "txt_SensitivityValue_Left";
            this.txt_SensitivityValue_Left.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel8
            // 
            resources.ApplyResources(this.srmLabel8, "srmLabel8");
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_SensitivityOnPad_Left
            // 
            resources.ApplyResources(this.cbo_SensitivityOnPad_Left, "cbo_SensitivityOnPad_Left");
            this.cbo_SensitivityOnPad_Left.BackColor = System.Drawing.Color.White;
            this.cbo_SensitivityOnPad_Left.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_SensitivityOnPad_Left.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_SensitivityOnPad_Left.FormattingEnabled = true;
            this.cbo_SensitivityOnPad_Left.Items.AddRange(new object[] {
            resources.GetString("cbo_SensitivityOnPad_Left.Items"),
            resources.GetString("cbo_SensitivityOnPad_Left.Items1"),
            resources.GetString("cbo_SensitivityOnPad_Left.Items2")});
            this.cbo_SensitivityOnPad_Left.Name = "cbo_SensitivityOnPad_Left";
            this.cbo_SensitivityOnPad_Left.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel9
            // 
            resources.ApplyResources(this.srmLabel9, "srmLabel9");
            this.srmLabel9.Name = "srmLabel9";
            this.srmLabel9.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_SensitivityValue_Bottom
            // 
            resources.ApplyResources(this.txt_SensitivityValue_Bottom, "txt_SensitivityValue_Bottom");
            this.txt_SensitivityValue_Bottom.BackColor = System.Drawing.Color.White;
            this.txt_SensitivityValue_Bottom.DecimalPlaces = 0;
            this.txt_SensitivityValue_Bottom.DecMaxValue = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_SensitivityValue_Bottom.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_SensitivityValue_Bottom.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_SensitivityValue_Bottom.ForeColor = System.Drawing.Color.Black;
            this.txt_SensitivityValue_Bottom.InputType = SRMControl.InputType.Number;
            this.txt_SensitivityValue_Bottom.Name = "txt_SensitivityValue_Bottom";
            this.txt_SensitivityValue_Bottom.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel6
            // 
            resources.ApplyResources(this.srmLabel6, "srmLabel6");
            this.srmLabel6.Name = "srmLabel6";
            this.srmLabel6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_SensitivityOnPad_Bottom
            // 
            resources.ApplyResources(this.cbo_SensitivityOnPad_Bottom, "cbo_SensitivityOnPad_Bottom");
            this.cbo_SensitivityOnPad_Bottom.BackColor = System.Drawing.Color.White;
            this.cbo_SensitivityOnPad_Bottom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_SensitivityOnPad_Bottom.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_SensitivityOnPad_Bottom.FormattingEnabled = true;
            this.cbo_SensitivityOnPad_Bottom.Items.AddRange(new object[] {
            resources.GetString("cbo_SensitivityOnPad_Bottom.Items"),
            resources.GetString("cbo_SensitivityOnPad_Bottom.Items1"),
            resources.GetString("cbo_SensitivityOnPad_Bottom.Items2")});
            this.cbo_SensitivityOnPad_Bottom.Name = "cbo_SensitivityOnPad_Bottom";
            this.cbo_SensitivityOnPad_Bottom.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel7
            // 
            resources.ApplyResources(this.srmLabel7, "srmLabel7");
            this.srmLabel7.Name = "srmLabel7";
            this.srmLabel7.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_SensitivityValue_Right
            // 
            resources.ApplyResources(this.txt_SensitivityValue_Right, "txt_SensitivityValue_Right");
            this.txt_SensitivityValue_Right.BackColor = System.Drawing.Color.White;
            this.txt_SensitivityValue_Right.DecimalPlaces = 0;
            this.txt_SensitivityValue_Right.DecMaxValue = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_SensitivityValue_Right.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_SensitivityValue_Right.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_SensitivityValue_Right.ForeColor = System.Drawing.Color.Black;
            this.txt_SensitivityValue_Right.InputType = SRMControl.InputType.Number;
            this.txt_SensitivityValue_Right.Name = "txt_SensitivityValue_Right";
            this.txt_SensitivityValue_Right.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel4
            // 
            resources.ApplyResources(this.srmLabel4, "srmLabel4");
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_SensitivityOnPad_Right
            // 
            resources.ApplyResources(this.cbo_SensitivityOnPad_Right, "cbo_SensitivityOnPad_Right");
            this.cbo_SensitivityOnPad_Right.BackColor = System.Drawing.Color.White;
            this.cbo_SensitivityOnPad_Right.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_SensitivityOnPad_Right.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_SensitivityOnPad_Right.FormattingEnabled = true;
            this.cbo_SensitivityOnPad_Right.Items.AddRange(new object[] {
            resources.GetString("cbo_SensitivityOnPad_Right.Items"),
            resources.GetString("cbo_SensitivityOnPad_Right.Items1"),
            resources.GetString("cbo_SensitivityOnPad_Right.Items2")});
            this.cbo_SensitivityOnPad_Right.Name = "cbo_SensitivityOnPad_Right";
            this.cbo_SensitivityOnPad_Right.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel5
            // 
            resources.ApplyResources(this.srmLabel5, "srmLabel5");
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_SensitivityValue_Top
            // 
            resources.ApplyResources(this.txt_SensitivityValue_Top, "txt_SensitivityValue_Top");
            this.txt_SensitivityValue_Top.BackColor = System.Drawing.Color.White;
            this.txt_SensitivityValue_Top.DecimalPlaces = 0;
            this.txt_SensitivityValue_Top.DecMaxValue = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_SensitivityValue_Top.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_SensitivityValue_Top.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_SensitivityValue_Top.ForeColor = System.Drawing.Color.Black;
            this.txt_SensitivityValue_Top.InputType = SRMControl.InputType.Number;
            this.txt_SensitivityValue_Top.Name = "txt_SensitivityValue_Top";
            this.txt_SensitivityValue_Top.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_SensitivityOnPad_Top
            // 
            resources.ApplyResources(this.cbo_SensitivityOnPad_Top, "cbo_SensitivityOnPad_Top");
            this.cbo_SensitivityOnPad_Top.BackColor = System.Drawing.Color.White;
            this.cbo_SensitivityOnPad_Top.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_SensitivityOnPad_Top.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_SensitivityOnPad_Top.FormattingEnabled = true;
            this.cbo_SensitivityOnPad_Top.Items.AddRange(new object[] {
            resources.GetString("cbo_SensitivityOnPad_Top.Items"),
            resources.GetString("cbo_SensitivityOnPad_Top.Items1"),
            resources.GetString("cbo_SensitivityOnPad_Top.Items2")});
            this.cbo_SensitivityOnPad_Top.Name = "cbo_SensitivityOnPad_Top";
            this.cbo_SensitivityOnPad_Top.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel11
            // 
            resources.ApplyResources(this.srmLabel11, "srmLabel11");
            this.srmLabel11.Name = "srmLabel11";
            this.srmLabel11.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_SensitivityValue_Center
            // 
            resources.ApplyResources(this.txt_SensitivityValue_Center, "txt_SensitivityValue_Center");
            this.txt_SensitivityValue_Center.BackColor = System.Drawing.Color.White;
            this.txt_SensitivityValue_Center.DecimalPlaces = 0;
            this.txt_SensitivityValue_Center.DecMaxValue = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_SensitivityValue_Center.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_SensitivityValue_Center.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_SensitivityValue_Center.ForeColor = System.Drawing.Color.Black;
            this.txt_SensitivityValue_Center.InputType = SRMControl.InputType.Number;
            this.txt_SensitivityValue_Center.Name = "txt_SensitivityValue_Center";
            this.txt_SensitivityValue_Center.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel12
            // 
            resources.ApplyResources(this.srmLabel12, "srmLabel12");
            this.srmLabel12.Name = "srmLabel12";
            this.srmLabel12.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_SensitivityOnPad_Center
            // 
            resources.ApplyResources(this.cbo_SensitivityOnPad_Center, "cbo_SensitivityOnPad_Center");
            this.cbo_SensitivityOnPad_Center.BackColor = System.Drawing.Color.White;
            this.cbo_SensitivityOnPad_Center.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_SensitivityOnPad_Center.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_SensitivityOnPad_Center.FormattingEnabled = true;
            this.cbo_SensitivityOnPad_Center.Items.AddRange(new object[] {
            resources.GetString("cbo_SensitivityOnPad_Center.Items"),
            resources.GetString("cbo_SensitivityOnPad_Center.Items1"),
            resources.GetString("cbo_SensitivityOnPad_Center.Items2")});
            this.cbo_SensitivityOnPad_Center.Name = "cbo_SensitivityOnPad_Center";
            this.cbo_SensitivityOnPad_Center.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // ils_ImageListTree
            // 
            this.ils_ImageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ils_ImageListTree.ImageStream")));
            this.ils_ImageListTree.TransparentColor = System.Drawing.Color.Transparent;
            this.ils_ImageListTree.Images.SetKeyName(0, "5S Center Pkg ROI.png");
            this.ils_ImageListTree.Images.SetKeyName(1, "5S Top Pkg ROI.png");
            this.ils_ImageListTree.Images.SetKeyName(2, "5S Right Pkg ROI.png");
            this.ils_ImageListTree.Images.SetKeyName(3, "5S Bottom Pkg ROI.png");
            this.ils_ImageListTree.Images.SetKeyName(4, "5S Left Pkg ROI.png");
            // 
            // Timer
            // 
            this.Timer.Enabled = true;
            // 
            // PadSubSettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.panel_SelectEdge);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PadSubSettingForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.panel_SelectEdge.ResumeLayout(false);
            this.panel_SelectEdge.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private System.Windows.Forms.Panel panel_SelectEdge;
        private System.Windows.Forms.ImageList ils_ImageListTree;
        private System.Windows.Forms.Timer Timer;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMLabel srmLabel11;
        private SRMControl.SRMInputBox txt_SensitivityValue_Center;
        private SRMControl.SRMLabel srmLabel12;
        private SRMControl.SRMComboBox cbo_SensitivityOnPad_Center;
        private SRMControl.SRMInputBox txt_SensitivityValue_Left;
        private SRMControl.SRMLabel srmLabel8;
        private SRMControl.SRMComboBox cbo_SensitivityOnPad_Left;
        private SRMControl.SRMLabel srmLabel9;
        private SRMControl.SRMInputBox txt_SensitivityValue_Bottom;
        private SRMControl.SRMLabel srmLabel6;
        private SRMControl.SRMComboBox cbo_SensitivityOnPad_Bottom;
        private SRMControl.SRMLabel srmLabel7;
        private SRMControl.SRMInputBox txt_SensitivityValue_Right;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMComboBox cbo_SensitivityOnPad_Right;
        private SRMControl.SRMLabel srmLabel5;
        private SRMControl.SRMInputBox txt_SensitivityValue_Top;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMComboBox cbo_SensitivityOnPad_Top;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMLabel srmLabel10;
        private SRMControl.SRMLabel srmLabel13;
        private SRMControl.SRMCheckBox chk_AutoSensitivity_Left;
        private SRMControl.SRMCheckBox chk_AutoSensitivity_Bottom;
        private SRMControl.SRMCheckBox chk_AutoSensitivity_Right;
        private SRMControl.SRMCheckBox chk_AutoSensitivity_Top;
        private SRMControl.SRMCheckBox chk_AutoSensitivity_Center;
    }
}