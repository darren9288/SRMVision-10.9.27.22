namespace VisionProcessForm
{
    partial class ColorGuideLine
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
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_Save = new SRMControl.SRMButton();
            this.btn_Remove = new SRMControl.SRMButton();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.ColorPanel = new System.Windows.Forms.Panel();
            this.panel_ColorBar = new System.Windows.Forms.Panel();
            this.tabCtrl_Lean = new SRMControl.SRMTabControl();
            this.tp_Step1 = new System.Windows.Forms.TabPage();
            this.tp_Step2 = new System.Windows.Forms.TabPage();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.tp_Step3 = new System.Windows.Forms.TabPage();
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.srmLabel7 = new SRMControl.SRMLabel();
            this.srmLabel6 = new SRMControl.SRMLabel();
            this.txt_BlueMax = new SRMControl.SRMInputBox();
            this.txt_GreenMax = new SRMControl.SRMInputBox();
            this.txt_RedMax = new SRMControl.SRMInputBox();
            this.lbl_Lightness = new SRMControl.SRMLabel();
            this.lbl_Saturation = new SRMControl.SRMLabel();
            this.txt_GreenMin = new SRMControl.SRMInputBox();
            this.txt_BlueMin = new SRMControl.SRMInputBox();
            this.lbl_Hue = new SRMControl.SRMLabel();
            this.txt_RedMin = new SRMControl.SRMInputBox();
            this.btn_Redo = new SRMControl.SRMButton();
            this.btn_Previous = new SRMControl.SRMButton();
            this.btn_Next = new SRMControl.SRMButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cbo_Action = new SRMControl.SRMComboBox();
            this.tabCtrl_Lean.SuspendLayout();
            this.tp_Step1.SuspendLayout();
            this.tp_Step2.SuspendLayout();
            this.tp_Step3.SuspendLayout();
            this.SuspendLayout();
            // 
            // srmLabel5
            // 
            this.srmLabel5.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.srmLabel5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel5.Location = new System.Drawing.Point(15, 26);
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.Size = new System.Drawing.Size(211, 57);
            this.srmLabel5.TabIndex = 163;
            this.srmLabel5.Text = "Drag ROI to desired area to get color range";
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Cancel.Location = new System.Drawing.Point(164, 413);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(80, 34);
            this.btn_Cancel.TabIndex = 120;
            this.btn_Cancel.Text = "Close";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Save.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Save.Location = new System.Drawing.Point(6, 363);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(80, 34);
            this.btn_Save.TabIndex = 121;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_Remove
            // 
            this.btn_Remove.Location = new System.Drawing.Point(172, 363);
            this.btn_Remove.Name = "btn_Remove";
            this.btn_Remove.Size = new System.Drawing.Size(75, 34);
            this.btn_Remove.TabIndex = 165;
            this.btn_Remove.Text = "Remove";
            this.btn_Remove.UseVisualStyleBackColor = true;
            this.btn_Remove.Click += new System.EventHandler(this.btn_Remove_Click);
            // 
            // srmLabel2
            // 
            this.srmLabel2.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.srmLabel2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel2.Location = new System.Drawing.Point(6, 12);
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.Size = new System.Drawing.Size(283, 18);
            this.srmLabel2.TabIndex = 163;
            this.srmLabel2.Text = "Set each color (R,G,B) range";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // ColorPanel
            // 
            this.ColorPanel.BackColor = System.Drawing.Color.Black;
            this.ColorPanel.Location = new System.Drawing.Point(6, 6);
            this.ColorPanel.Name = "ColorPanel";
            this.ColorPanel.Size = new System.Drawing.Size(238, 301);
            this.ColorPanel.TabIndex = 166;
            this.ColorPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorPanel_Paint);
            this.ColorPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorPanel_MouseClick);
            // 
            // panel_ColorBar
            // 
            this.panel_ColorBar.BackColor = System.Drawing.Color.Black;
            this.panel_ColorBar.Location = new System.Drawing.Point(6, 111);
            this.panel_ColorBar.Name = "panel_ColorBar";
            this.panel_ColorBar.Size = new System.Drawing.Size(235, 246);
            this.panel_ColorBar.TabIndex = 164;
            // 
            // tabCtrl_Lean
            // 
            this.tabCtrl_Lean.Controls.Add(this.tp_Step1);
            this.tabCtrl_Lean.Controls.Add(this.tp_Step2);
            this.tabCtrl_Lean.Controls.Add(this.tp_Step3);
            this.tabCtrl_Lean.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.tabCtrl_Lean.Location = new System.Drawing.Point(-4, -22);
            this.tabCtrl_Lean.Name = "tabCtrl_Lean";
            this.tabCtrl_Lean.SelectedIndex = 0;
            this.tabCtrl_Lean.Size = new System.Drawing.Size(258, 429);
            this.tabCtrl_Lean.TabIndex = 124;
            this.tabCtrl_Lean.TabStop = false;
            // 
            // tp_Step1
            // 
            this.tp_Step1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Step1.Controls.Add(this.srmLabel5);
            this.tp_Step1.Location = new System.Drawing.Point(4, 22);
            this.tp_Step1.Name = "tp_Step1";
            this.tp_Step1.Size = new System.Drawing.Size(250, 403);
            this.tp_Step1.TabIndex = 4;
            this.tp_Step1.Text = "1";
            // 
            // tp_Step2
            // 
            this.tp_Step2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Step2.Controls.Add(this.cbo_Action);
            this.tp_Step2.Controls.Add(this.srmLabel4);
            this.tp_Step2.Controls.Add(this.srmLabel3);
            this.tp_Step2.Controls.Add(this.btn_Remove);
            this.tp_Step2.Controls.Add(this.ColorPanel);
            this.tp_Step2.Location = new System.Drawing.Point(4, 22);
            this.tp_Step2.Name = "tp_Step2";
            this.tp_Step2.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Step2.Size = new System.Drawing.Size(250, 403);
            this.tp_Step2.TabIndex = 0;
            this.tp_Step2.Text = "2";
            // 
            // srmLabel4
            // 
            this.srmLabel4.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.srmLabel4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel4.Location = new System.Drawing.Point(6, 338);
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.Size = new System.Drawing.Size(238, 15);
            this.srmLabel4.TabIndex = 168;
            this.srmLabel4.Text = "- Click another time to unselect it";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel3
            // 
            this.srmLabel3.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.srmLabel3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel3.Location = new System.Drawing.Point(6, 310);
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.Size = new System.Drawing.Size(238, 29);
            this.srmLabel3.TabIndex = 167;
            this.srmLabel3.Text = "- Click once to select it which will be drawn in red box ";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tp_Step3
            // 
            this.tp_Step3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Step3.Controls.Add(this.srmLabel8);
            this.tp_Step3.Controls.Add(this.srmLabel7);
            this.tp_Step3.Controls.Add(this.srmLabel6);
            this.tp_Step3.Controls.Add(this.txt_BlueMax);
            this.tp_Step3.Controls.Add(this.txt_GreenMax);
            this.tp_Step3.Controls.Add(this.txt_RedMax);
            this.tp_Step3.Controls.Add(this.lbl_Lightness);
            this.tp_Step3.Controls.Add(this.lbl_Saturation);
            this.tp_Step3.Controls.Add(this.txt_GreenMin);
            this.tp_Step3.Controls.Add(this.txt_BlueMin);
            this.tp_Step3.Controls.Add(this.lbl_Hue);
            this.tp_Step3.Controls.Add(this.txt_RedMin);
            this.tp_Step3.Controls.Add(this.btn_Redo);
            this.tp_Step3.Controls.Add(this.srmLabel2);
            this.tp_Step3.Controls.Add(this.btn_Save);
            this.tp_Step3.Controls.Add(this.panel_ColorBar);
            this.tp_Step3.Location = new System.Drawing.Point(4, 22);
            this.tp_Step3.Name = "tp_Step3";
            this.tp_Step3.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Step3.Size = new System.Drawing.Size(250, 403);
            this.tp_Step3.TabIndex = 1;
            this.tp_Step3.Text = "3";
            // 
            // srmLabel8
            // 
            this.srmLabel8.AutoSize = true;
            this.srmLabel8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel8.Location = new System.Drawing.Point(132, 88);
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.Size = new System.Drawing.Size(10, 13);
            this.srmLabel8.TabIndex = 180;
            this.srmLabel8.Text = "-";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel7
            // 
            this.srmLabel7.AutoSize = true;
            this.srmLabel7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel7.Location = new System.Drawing.Point(132, 62);
            this.srmLabel7.Name = "srmLabel7";
            this.srmLabel7.Size = new System.Drawing.Size(10, 13);
            this.srmLabel7.TabIndex = 179;
            this.srmLabel7.Text = "-";
            this.srmLabel7.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel6
            // 
            this.srmLabel6.AutoSize = true;
            this.srmLabel6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel6.Location = new System.Drawing.Point(132, 35);
            this.srmLabel6.Name = "srmLabel6";
            this.srmLabel6.Size = new System.Drawing.Size(10, 13);
            this.srmLabel6.TabIndex = 178;
            this.srmLabel6.Text = "-";
            this.srmLabel6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_BlueMax
            // 
            this.txt_BlueMax.BackColor = System.Drawing.Color.White;
            this.txt_BlueMax.DecimalPlaces = 0;
            this.txt_BlueMax.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_BlueMax.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_BlueMax.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_BlueMax.ForeColor = System.Drawing.Color.Black;
            this.txt_BlueMax.InputType = SRMControl.InputType.Number;
            this.txt_BlueMax.Location = new System.Drawing.Point(148, 85);
            this.txt_BlueMax.Name = "txt_BlueMax";
            this.txt_BlueMax.NormalBackColor = System.Drawing.Color.White;
            this.txt_BlueMax.Size = new System.Drawing.Size(55, 20);
            this.txt_BlueMax.TabIndex = 177;
            this.txt_BlueMax.Text = "125";
            this.txt_BlueMax.TextChanged += new System.EventHandler(this.txt_ColorTolerance_TextChanged);
            // 
            // txt_GreenMax
            // 
            this.txt_GreenMax.BackColor = System.Drawing.Color.White;
            this.txt_GreenMax.DecimalPlaces = 0;
            this.txt_GreenMax.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_GreenMax.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_GreenMax.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_GreenMax.ForeColor = System.Drawing.Color.Black;
            this.txt_GreenMax.InputType = SRMControl.InputType.Number;
            this.txt_GreenMax.Location = new System.Drawing.Point(148, 59);
            this.txt_GreenMax.Name = "txt_GreenMax";
            this.txt_GreenMax.NormalBackColor = System.Drawing.Color.White;
            this.txt_GreenMax.Size = new System.Drawing.Size(55, 20);
            this.txt_GreenMax.TabIndex = 176;
            this.txt_GreenMax.Text = "125";
            this.txt_GreenMax.TextChanged += new System.EventHandler(this.txt_ColorTolerance_TextChanged);
            // 
            // txt_RedMax
            // 
            this.txt_RedMax.BackColor = System.Drawing.Color.White;
            this.txt_RedMax.DecimalPlaces = 0;
            this.txt_RedMax.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_RedMax.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_RedMax.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_RedMax.ForeColor = System.Drawing.Color.Black;
            this.txt_RedMax.InputType = SRMControl.InputType.Number;
            this.txt_RedMax.Location = new System.Drawing.Point(148, 33);
            this.txt_RedMax.Name = "txt_RedMax";
            this.txt_RedMax.NormalBackColor = System.Drawing.Color.White;
            this.txt_RedMax.Size = new System.Drawing.Size(55, 20);
            this.txt_RedMax.TabIndex = 175;
            this.txt_RedMax.Text = "125";
            this.txt_RedMax.TextChanged += new System.EventHandler(this.txt_ColorTolerance_TextChanged);
            // 
            // lbl_Lightness
            // 
            this.lbl_Lightness.AutoSize = true;
            this.lbl_Lightness.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Lightness.Location = new System.Drawing.Point(14, 88);
            this.lbl_Lightness.Name = "lbl_Lightness";
            this.lbl_Lightness.Size = new System.Drawing.Size(28, 13);
            this.lbl_Lightness.TabIndex = 168;
            this.lbl_Lightness.Text = "Blue";
            this.lbl_Lightness.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Saturation
            // 
            this.lbl_Saturation.AutoSize = true;
            this.lbl_Saturation.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Saturation.Location = new System.Drawing.Point(14, 63);
            this.lbl_Saturation.Name = "lbl_Saturation";
            this.lbl_Saturation.Size = new System.Drawing.Size(36, 13);
            this.lbl_Saturation.TabIndex = 170;
            this.lbl_Saturation.Text = "Green";
            this.lbl_Saturation.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_GreenMin
            // 
            this.txt_GreenMin.BackColor = System.Drawing.Color.White;
            this.txt_GreenMin.DecimalPlaces = 0;
            this.txt_GreenMin.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_GreenMin.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_GreenMin.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_GreenMin.ForeColor = System.Drawing.Color.Black;
            this.txt_GreenMin.InputType = SRMControl.InputType.Number;
            this.txt_GreenMin.Location = new System.Drawing.Point(72, 59);
            this.txt_GreenMin.Name = "txt_GreenMin";
            this.txt_GreenMin.NormalBackColor = System.Drawing.Color.White;
            this.txt_GreenMin.Size = new System.Drawing.Size(55, 20);
            this.txt_GreenMin.TabIndex = 171;
            this.txt_GreenMin.Text = "125";
            this.txt_GreenMin.TextChanged += new System.EventHandler(this.txt_ColorTolerance_TextChanged);
            // 
            // txt_BlueMin
            // 
            this.txt_BlueMin.BackColor = System.Drawing.Color.White;
            this.txt_BlueMin.DecimalPlaces = 0;
            this.txt_BlueMin.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_BlueMin.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_BlueMin.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_BlueMin.ForeColor = System.Drawing.Color.Black;
            this.txt_BlueMin.InputType = SRMControl.InputType.Number;
            this.txt_BlueMin.Location = new System.Drawing.Point(72, 85);
            this.txt_BlueMin.Name = "txt_BlueMin";
            this.txt_BlueMin.NormalBackColor = System.Drawing.Color.White;
            this.txt_BlueMin.Size = new System.Drawing.Size(55, 20);
            this.txt_BlueMin.TabIndex = 169;
            this.txt_BlueMin.Text = "125";
            this.txt_BlueMin.TextChanged += new System.EventHandler(this.txt_ColorTolerance_TextChanged);
            // 
            // lbl_Hue
            // 
            this.lbl_Hue.AutoSize = true;
            this.lbl_Hue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Hue.Location = new System.Drawing.Point(14, 36);
            this.lbl_Hue.Name = "lbl_Hue";
            this.lbl_Hue.Size = new System.Drawing.Size(27, 13);
            this.lbl_Hue.TabIndex = 173;
            this.lbl_Hue.Text = "Red";
            this.lbl_Hue.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_RedMin
            // 
            this.txt_RedMin.BackColor = System.Drawing.Color.White;
            this.txt_RedMin.DecimalPlaces = 0;
            this.txt_RedMin.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_RedMin.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_RedMin.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_RedMin.ForeColor = System.Drawing.Color.Black;
            this.txt_RedMin.InputType = SRMControl.InputType.Number;
            this.txt_RedMin.Location = new System.Drawing.Point(72, 33);
            this.txt_RedMin.Name = "txt_RedMin";
            this.txt_RedMin.NormalBackColor = System.Drawing.Color.White;
            this.txt_RedMin.Size = new System.Drawing.Size(55, 20);
            this.txt_RedMin.TabIndex = 174;
            this.txt_RedMin.Text = "125";
            this.txt_RedMin.TextChanged += new System.EventHandler(this.txt_ColorTolerance_TextChanged);
            // 
            // btn_Redo
            // 
            this.btn_Redo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Redo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Redo.Location = new System.Drawing.Point(101, 363);
            this.btn_Redo.Name = "btn_Redo";
            this.btn_Redo.Size = new System.Drawing.Size(102, 34);
            this.btn_Redo.TabIndex = 167;
            this.btn_Redo.Text = "Redo";
            this.btn_Redo.UseVisualStyleBackColor = true;
            this.btn_Redo.Click += new System.EventHandler(this.btn_Redo_Click);
            // 
            // btn_Previous
            // 
            this.btn_Previous.Enabled = false;
            this.btn_Previous.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Previous.Location = new System.Drawing.Point(3, 415);
            this.btn_Previous.Name = "btn_Previous";
            this.btn_Previous.Size = new System.Drawing.Size(61, 30);
            this.btn_Previous.TabIndex = 126;
            this.btn_Previous.Text = "<<";
            this.btn_Previous.UseVisualStyleBackColor = true;
            this.btn_Previous.Click += new System.EventHandler(this.btn_Previous_Click);
            // 
            // btn_Next
            // 
            this.btn_Next.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Next.Location = new System.Drawing.Point(66, 415);
            this.btn_Next.Name = "btn_Next";
            this.btn_Next.Size = new System.Drawing.Size(61, 30);
            this.btn_Next.TabIndex = 125;
            this.btn_Next.Text = ">>";
            this.btn_Next.UseVisualStyleBackColor = true;
            this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cbo_Action
            // 
            this.cbo_Action.BackColor = System.Drawing.Color.White;
            this.cbo_Action.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Action.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Action.FormattingEnabled = true;
            this.cbo_Action.Items.AddRange(new object[] {
            "Affect 1 Column",
            "Affect Whole Column",
            "Affect Whole Row"});
            this.cbo_Action.Location = new System.Drawing.Point(3, 371);
            this.cbo_Action.Name = "cbo_Action";
            this.cbo_Action.NormalBackColor = System.Drawing.Color.White;
            this.cbo_Action.Size = new System.Drawing.Size(155, 21);
            this.cbo_Action.TabIndex = 169;
            // 
            // ColorGuideLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(249, 503);
            this.ControlBox = false;
            this.Controls.Add(this.btn_Previous);
            this.Controls.Add(this.btn_Next);
            this.Controls.Add(this.tabCtrl_Lean);
            this.Controls.Add(this.btn_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ColorGuideLine";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Color Guideline";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ColorGuideLine_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ColorGuideLine_FormClosing);
            this.tabCtrl_Lean.ResumeLayout(false);
            this.tp_Step1.ResumeLayout(false);
            this.tp_Step2.ResumeLayout(false);
            this.tp_Step3.ResumeLayout(false);
            this.tp_Step3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMLabel srmLabel5;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_Save;
        private SRMControl.SRMButton btn_Remove;
        private SRMControl.SRMLabel srmLabel2;
        private System.Windows.Forms.Panel ColorPanel;
        private System.Windows.Forms.Panel panel_ColorBar;
        private SRMControl.SRMTabControl tabCtrl_Lean;
        private System.Windows.Forms.TabPage tp_Step1;
        private System.Windows.Forms.TabPage tp_Step2;
        private System.Windows.Forms.TabPage tp_Step3;
        private SRMControl.SRMButton btn_Redo;
        private SRMControl.SRMButton btn_Previous;
        private SRMControl.SRMButton btn_Next;
        private System.Windows.Forms.Timer timer1;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMLabel lbl_Lightness;
        private SRMControl.SRMLabel lbl_Saturation;
        private SRMControl.SRMInputBox txt_GreenMin;
        private SRMControl.SRMInputBox txt_BlueMin;
        private SRMControl.SRMLabel lbl_Hue;
        private SRMControl.SRMInputBox txt_RedMin;
        private SRMControl.SRMInputBox txt_BlueMax;
        private SRMControl.SRMInputBox txt_GreenMax;
        private SRMControl.SRMInputBox txt_RedMax;
        private SRMControl.SRMLabel srmLabel8;
        private SRMControl.SRMLabel srmLabel7;
        private SRMControl.SRMLabel srmLabel6;
        private SRMControl.SRMComboBox cbo_Action;
    }
}