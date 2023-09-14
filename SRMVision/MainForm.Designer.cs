namespace SRMVision
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MenuToolStrip = new System.Windows.Forms.ToolStrip();
            this.btn_Auto = new System.Windows.Forms.ToolStripButton();
            this.btn_Recipe = new System.Windows.Forms.ToolStripButton();
            this.btn_IO = new System.Windows.Forms.ToolStripButton();
            this.btn_IOMap = new System.Windows.Forms.ToolStripButton();
            this.btn_Login = new System.Windows.Forms.ToolStripButton();
            this.btn_Diagnostic = new System.Windows.Forms.ToolStripButton();
            this.btn_User = new System.Windows.Forms.ToolStripButton();
            this.btn_History = new System.Windows.Forms.ToolStripButton();
            this.btn_Language = new System.Windows.Forms.ToolStripSplitButton();
            this.btn_ENGLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_CHSLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_CHTLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_Config = new System.Windows.Forms.ToolStripButton();
            this.btn_Option = new System.Windows.Forms.ToolStripButton();
            this.btn_OSK = new System.Windows.Forms.ToolStripButton();
            this.btn_AboutSRM = new System.Windows.Forms.ToolStripButton();
            this.btn_Quit = new System.Windows.Forms.ToolStripButton();
            this.StatusStrip_Main = new System.Windows.Forms.StatusStrip();
            this.TSlbl_PositionXTitle = new System.Windows.Forms.ToolStripStatusLabel();
            this.TSlbl_MousePositionX = new System.Windows.Forms.ToolStripStatusLabel();
            this.TSlbl_PositionYTitle = new System.Windows.Forms.ToolStripStatusLabel();
            this.TSlbl_MousePositionY = new System.Windows.Forms.ToolStripStatusLabel();
            this.TSlbl_PixelTitle = new System.Windows.Forms.ToolStripStatusLabel();
            this.TSlbl_Pixel = new System.Windows.Forms.ToolStripStatusLabel();
            this.TSlbl_RGBPixelTitle = new System.Windows.Forms.ToolStripStatusLabel();
            this.TSlbl_RGBPixel = new System.Windows.Forms.ToolStripStatusLabel();
            this.TSlbl_State = new System.Windows.Forms.ToolStripStatusLabel();
            this.TSlbl_User = new System.Windows.Forms.ToolStripStatusLabel();
            this.TSlbl_SoftwareVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.TSlbl_Date = new System.Windows.Forms.ToolStripStatusLabel();
            this.TSlbl_Time = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer_AutoLogOut = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.MenuToolStrip.SuspendLayout();
            this.StatusStrip_Main.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuToolStrip
            // 
            resources.ApplyResources(this.MenuToolStrip, "MenuToolStrip");
            this.MenuToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_Auto,
            this.btn_Recipe,
            this.btn_IO,
            this.btn_IOMap,
            this.btn_Login,
            this.btn_Diagnostic,
            this.btn_User,
            this.btn_History,
            this.btn_Language,
            this.btn_Config,
            this.btn_Option,
            this.btn_OSK,
            this.btn_AboutSRM,
            this.btn_Quit});
            this.MenuToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.MenuToolStrip.Name = "MenuToolStrip";
            // 
            // btn_Auto
            // 
            resources.ApplyResources(this.btn_Auto, "btn_Auto");
            this.btn_Auto.Name = "btn_Auto";
            this.btn_Auto.Click += new System.EventHandler(this.btn_Auto_Click);
            // 
            // btn_Recipe
            // 
            resources.ApplyResources(this.btn_Recipe, "btn_Recipe");
            this.btn_Recipe.Name = "btn_Recipe";
            this.btn_Recipe.Click += new System.EventHandler(this.btn_Recipe_Click);
            // 
            // btn_IO
            // 
            resources.ApplyResources(this.btn_IO, "btn_IO");
            this.btn_IO.Name = "btn_IO";
            this.btn_IO.Click += new System.EventHandler(this.btn_IO_Click);
            // 
            // btn_IOMap
            // 
            resources.ApplyResources(this.btn_IOMap, "btn_IOMap");
            this.btn_IOMap.Name = "btn_IOMap";
            this.btn_IOMap.Click += new System.EventHandler(this.btn_IOMap_Click);
            // 
            // btn_Login
            // 
            resources.ApplyResources(this.btn_Login, "btn_Login");
            this.btn_Login.Name = "btn_Login";
            this.btn_Login.Click += new System.EventHandler(this.btn_Login_Click);
            // 
            // btn_Diagnostic
            // 
            resources.ApplyResources(this.btn_Diagnostic, "btn_Diagnostic");
            this.btn_Diagnostic.Name = "btn_Diagnostic";
            this.btn_Diagnostic.Click += new System.EventHandler(this.btn_Diagnostic_Click);
            // 
            // btn_User
            // 
            resources.ApplyResources(this.btn_User, "btn_User");
            this.btn_User.Name = "btn_User";
            this.btn_User.Click += new System.EventHandler(this.btn_User_Click);
            // 
            // btn_History
            // 
            resources.ApplyResources(this.btn_History, "btn_History");
            this.btn_History.Name = "btn_History";
            this.btn_History.Click += new System.EventHandler(this.btn_History_Click);
            // 
            // btn_Language
            // 
            resources.ApplyResources(this.btn_Language, "btn_Language");
            this.btn_Language.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_ENGLanguage,
            this.btn_CHSLanguage,
            this.btn_CHTLanguage});
            this.btn_Language.Name = "btn_Language";
            // 
            // btn_ENGLanguage
            // 
            resources.ApplyResources(this.btn_ENGLanguage, "btn_ENGLanguage");
            this.btn_ENGLanguage.Name = "btn_ENGLanguage";
            this.btn_ENGLanguage.Click += new System.EventHandler(this.btn_Language_Click);
            // 
            // btn_CHSLanguage
            // 
            resources.ApplyResources(this.btn_CHSLanguage, "btn_CHSLanguage");
            this.btn_CHSLanguage.Name = "btn_CHSLanguage";
            this.btn_CHSLanguage.Click += new System.EventHandler(this.btn_Language_Click);
            // 
            // btn_CHTLanguage
            // 
            resources.ApplyResources(this.btn_CHTLanguage, "btn_CHTLanguage");
            this.btn_CHTLanguage.Name = "btn_CHTLanguage";
            this.btn_CHTLanguage.Click += new System.EventHandler(this.btn_Language_Click);
            // 
            // btn_Config
            // 
            resources.ApplyResources(this.btn_Config, "btn_Config");
            this.btn_Config.Name = "btn_Config";
            this.btn_Config.Click += new System.EventHandler(this.btn_Config_Click);
            // 
            // btn_Option
            // 
            resources.ApplyResources(this.btn_Option, "btn_Option");
            this.btn_Option.Name = "btn_Option";
            this.btn_Option.Click += new System.EventHandler(this.btn_Option_Click);
            // 
            // btn_OSK
            // 
            resources.ApplyResources(this.btn_OSK, "btn_OSK");
            this.btn_OSK.Name = "btn_OSK";
            this.btn_OSK.Click += new System.EventHandler(this.btn_OSK_Click);
            // 
            // btn_AboutSRM
            // 
            resources.ApplyResources(this.btn_AboutSRM, "btn_AboutSRM");
            this.btn_AboutSRM.Name = "btn_AboutSRM";
            this.btn_AboutSRM.Click += new System.EventHandler(this.btn_AboutSRM_Click);
            // 
            // btn_Quit
            // 
            resources.ApplyResources(this.btn_Quit, "btn_Quit");
            this.btn_Quit.Name = "btn_Quit";
            this.btn_Quit.Click += new System.EventHandler(this.btn_Quit_Click);
            // 
            // StatusStrip_Main
            // 
            resources.ApplyResources(this.StatusStrip_Main, "StatusStrip_Main");
            this.StatusStrip_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSlbl_PositionXTitle,
            this.TSlbl_MousePositionX,
            this.TSlbl_PositionYTitle,
            this.TSlbl_MousePositionY,
            this.TSlbl_PixelTitle,
            this.TSlbl_Pixel,
            this.TSlbl_RGBPixelTitle,
            this.TSlbl_RGBPixel,
            this.TSlbl_State,
            this.TSlbl_User,
            this.TSlbl_SoftwareVersion,
            this.TSlbl_Date,
            this.TSlbl_Time});
            this.StatusStrip_Main.Name = "StatusStrip_Main";
            this.StatusStrip_Main.ShowItemToolTips = true;
            // 
            // TSlbl_PositionXTitle
            // 
            resources.ApplyResources(this.TSlbl_PositionXTitle, "TSlbl_PositionXTitle");
            this.TSlbl_PositionXTitle.Name = "TSlbl_PositionXTitle";
            // 
            // TSlbl_MousePositionX
            // 
            resources.ApplyResources(this.TSlbl_MousePositionX, "TSlbl_MousePositionX");
            this.TSlbl_MousePositionX.Name = "TSlbl_MousePositionX";
            // 
            // TSlbl_PositionYTitle
            // 
            resources.ApplyResources(this.TSlbl_PositionYTitle, "TSlbl_PositionYTitle");
            this.TSlbl_PositionYTitle.Name = "TSlbl_PositionYTitle";
            // 
            // TSlbl_MousePositionY
            // 
            resources.ApplyResources(this.TSlbl_MousePositionY, "TSlbl_MousePositionY");
            this.TSlbl_MousePositionY.Name = "TSlbl_MousePositionY";
            // 
            // TSlbl_PixelTitle
            // 
            resources.ApplyResources(this.TSlbl_PixelTitle, "TSlbl_PixelTitle");
            this.TSlbl_PixelTitle.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.TSlbl_PixelTitle.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.TSlbl_PixelTitle.Name = "TSlbl_PixelTitle";
            // 
            // TSlbl_Pixel
            // 
            resources.ApplyResources(this.TSlbl_Pixel, "TSlbl_Pixel");
            this.TSlbl_Pixel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.TSlbl_Pixel.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.TSlbl_Pixel.Name = "TSlbl_Pixel";
            // 
            // TSlbl_RGBPixelTitle
            // 
            resources.ApplyResources(this.TSlbl_RGBPixelTitle, "TSlbl_RGBPixelTitle");
            this.TSlbl_RGBPixelTitle.Name = "TSlbl_RGBPixelTitle";
            // 
            // TSlbl_RGBPixel
            // 
            resources.ApplyResources(this.TSlbl_RGBPixel, "TSlbl_RGBPixel");
            this.TSlbl_RGBPixel.Name = "TSlbl_RGBPixel";
            // 
            // TSlbl_State
            // 
            resources.ApplyResources(this.TSlbl_State, "TSlbl_State");
            this.TSlbl_State.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.TSlbl_State.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.TSlbl_State.Name = "TSlbl_State";
            this.TSlbl_State.Spring = true;
            // 
            // TSlbl_User
            // 
            resources.ApplyResources(this.TSlbl_User, "TSlbl_User");
            this.TSlbl_User.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.TSlbl_User.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.TSlbl_User.Name = "TSlbl_User";
            // 
            // TSlbl_SoftwareVersion
            // 
            resources.ApplyResources(this.TSlbl_SoftwareVersion, "TSlbl_SoftwareVersion");
            this.TSlbl_SoftwareVersion.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.TSlbl_SoftwareVersion.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.TSlbl_SoftwareVersion.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.TSlbl_SoftwareVersion.Name = "TSlbl_SoftwareVersion";
            // 
            // TSlbl_Date
            // 
            resources.ApplyResources(this.TSlbl_Date, "TSlbl_Date");
            this.TSlbl_Date.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.TSlbl_Date.Name = "TSlbl_Date";
            // 
            // TSlbl_Time
            // 
            resources.ApplyResources(this.TSlbl_Time, "TSlbl_Time");
            this.TSlbl_Time.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.TSlbl_Time.Name = "TSlbl_Time";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timer_AutoLogOut
            // 
            this.timer_AutoLogOut.Tick += new System.EventHandler(this.timer_AutoLogOut_Tick);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.StatusStrip_Main);
            this.Controls.Add(this.MenuToolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MdiChildActivate += new System.EventHandler(this.MainForm_MdiChildActivate);
            this.MenuToolStrip.ResumeLayout(false);
            this.MenuToolStrip.PerformLayout();
            this.StatusStrip_Main.ResumeLayout(false);
            this.StatusStrip_Main.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip MenuToolStrip;
        private System.Windows.Forms.ToolStripButton btn_Auto;
        private System.Windows.Forms.ToolStripButton btn_IO;
        private System.Windows.Forms.ToolStripButton btn_Login;
        private System.Windows.Forms.ToolStripButton btn_User;
        private System.Windows.Forms.ToolStripButton btn_History;
        private System.Windows.Forms.ToolStripButton btn_Config;
        private System.Windows.Forms.ToolStripButton btn_Option;
        private System.Windows.Forms.ToolStripButton btn_AboutSRM;
        private System.Windows.Forms.ToolStripButton btn_Quit;
        private System.Windows.Forms.StatusStrip StatusStrip_Main;
        private System.Windows.Forms.ToolStripStatusLabel TSlbl_State;
        private System.Windows.Forms.ToolStripStatusLabel TSlbl_User;
        private System.Windows.Forms.ToolStripStatusLabel TSlbl_SoftwareVersion;
        private System.Windows.Forms.ToolStripStatusLabel TSlbl_Date;
        private System.Windows.Forms.ToolStripStatusLabel TSlbl_Time;
        private System.Windows.Forms.ToolStripSplitButton btn_Language;
        private System.Windows.Forms.ToolStripMenuItem btn_ENGLanguage;
        private System.Windows.Forms.ToolStripMenuItem btn_CHSLanguage;
        private System.Windows.Forms.ToolStripMenuItem btn_CHTLanguage;
        private System.Windows.Forms.ToolStripStatusLabel TSlbl_MousePositionX;
        private System.Windows.Forms.ToolStripStatusLabel TSlbl_MousePositionY;
        private System.Windows.Forms.ToolStripStatusLabel TSlbl_PositionXTitle;
        private System.Windows.Forms.ToolStripStatusLabel TSlbl_PositionYTitle;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripStatusLabel TSlbl_PixelTitle;
        private System.Windows.Forms.ToolStripStatusLabel TSlbl_Pixel;
        private System.Windows.Forms.ToolStripButton btn_Recipe;
        private System.Windows.Forms.ToolStripButton btn_Diagnostic;
        private System.Windows.Forms.ToolStripButton btn_IOMap;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.ToolStripButton btn_OSK;
        private System.Windows.Forms.Timer timer_AutoLogOut;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripStatusLabel TSlbl_RGBPixelTitle;
        private System.Windows.Forms.ToolStripStatusLabel TSlbl_RGBPixel;
    }
}

