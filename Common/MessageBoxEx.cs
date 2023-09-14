using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Globalization;

namespace Common
{
    public partial class MessageBoxEx : Form
    {
        #region Constants

        private const int LEFT_PADDING = 12;
        private const int RIGHT_PADDING = 12;
        private const int TOP_PADDING = 20;
        private const int BOTTOM_PADDING = 12;
        private const int ITEM_PADDING = 12;
        
        private const int BUTTON_LEFT_PADDING = 4;
        private const int BUTTON_RIGHT_PADDING = 4;
        private const int BUTTON_TOP_PADDING = 4;
        private const int BUTTON_BOTTOM_PADDING = 4;
        private const int BUTTON_PADDING = 5;

        private const int MIN_BUTTON_HEIGHT = 23;
        private const int MIN_BUTTON_WIDTH = 74;

        private const int ICON_MESSAGE_PADDING = 15;

        #endregion

        #region Members

        private MessageBoxButtons m_buttons;
        private MessageBoxDefaultButton m_defaultButton;

        private int _maxLayoutWidth;
        private int _maxLayoutHeight;
        private int _maxWidth;
        private int _maxHeight;

        private Font font = new Font("Verdana", 10);
        private ArrayList _buttons = new ArrayList();
        private Hashtable _buttonControlsTable = new Hashtable();
        
        #endregion

        #region Properties

        public MessageBoxButtons Buttons
        {
            get
            {
                return m_buttons;
            }
            set
            {
                m_buttons = value;
                switch (m_buttons)
                {
                        case MessageBoxButtons.AbortRetryIgnore:
                        srmButton1.Text = GetTextLangauge("Abort");
                        srmButton1.DialogResult = DialogResult.Abort;
                        srmButton2.Text = GetTextLangauge("Retry");
                        srmButton2.DialogResult = DialogResult.Retry;
                        srmButton3.Text = GetTextLangauge("Ignore");
                        srmButton3.DialogResult = DialogResult.Ignore;
                        _buttons.Add(srmButton1);
                        _buttons.Add(srmButton2);
                        _buttons.Add(srmButton3);
                        break;
                    case MessageBoxButtons.OK:
                        srmButton1.Text = GetTextLangauge("OK");
                        srmButton1.DialogResult = DialogResult.OK;
                        srmButton2.Visible = false;
                        srmButton3.Visible = false;
                        _buttons.Add(srmButton1);
                        break;
                    case MessageBoxButtons.OKCancel:
                        srmButton1.Text = GetTextLangauge("OK");
                        srmButton1.DialogResult = DialogResult.OK;
                        srmButton2.Text = GetTextLangauge("Cancel");
                        srmButton2.DialogResult = DialogResult.Cancel;
                        srmButton3.Visible = false;
                        _buttons.Add(srmButton1);
                        _buttons.Add(srmButton2);
                        break;
                    case MessageBoxButtons.RetryCancel:
                        srmButton1.Text = GetTextLangauge("Retry");
                        srmButton1.DialogResult = DialogResult.Retry;
                        srmButton2.Text = GetTextLangauge("Cancel");
                        srmButton2.DialogResult = DialogResult.Cancel;
                        srmButton3.Visible = false;
                        _buttons.Add(srmButton1);
                        _buttons.Add(srmButton2);
                        break;
                    case MessageBoxButtons.YesNo:
                        srmButton1.Text = GetTextLangauge("Yes");
                        srmButton1.DialogResult = DialogResult.Yes;
                        srmButton2.Text = GetTextLangauge("No");
                        srmButton2.DialogResult = DialogResult.No;
                        srmButton3.Visible = false;
                        _buttons.Add(srmButton1);
                        _buttons.Add(srmButton2);
                        break;
                    case MessageBoxButtons.YesNoCancel:
                        srmButton1.Text = GetTextLangauge("Yes");
                        srmButton1.DialogResult = DialogResult.Yes;
                        srmButton2.Text = GetTextLangauge("No");
                        srmButton2.DialogResult = DialogResult.No;
                        srmButton3.Text = GetTextLangauge("Cancel");
                        srmButton3.DialogResult = DialogResult.Cancel;
                        _buttons.Add(srmButton1);
                        _buttons.Add(srmButton2);
                        _buttons.Add(srmButton3);
                        break;
                }
            }
        }

        public MessageBoxDefaultButton DefaultButton
        {
            get
            {
                return m_defaultButton;
            }
            set
            {
                m_defaultButton = value;
            }
        }

        public MessageBoxIcon Icons
        {
            set
            {
                switch (value)
                {
                    case MessageBoxIcon.Asterisk: //MessageBoxIcon.Information
                        pictureBox1.Image = SystemIcons.Asterisk.ToBitmap();
                        break;
                    case MessageBoxIcon.Error: //MessageBoxIcon.Hand & MessageBoxIcon.Stop
                        pictureBox1.Image = SystemIcons.Error.ToBitmap();
                        break;
                    case MessageBoxIcon.Exclamation: //MessageBoxIcon.Warning
                        pictureBox1.Image = SystemIcons.Exclamation.ToBitmap();
                        break;
                    case MessageBoxIcon.Question:
                        pictureBox1.Image = SystemIcons.Question.ToBitmap();
                        break;
                    case MessageBoxIcon.None:
                        pictureBox1.Visible = false;
                        lbl_Message.Location = new Point(pictureBox1.Location.X, pictureBox1.Location.Y);
                        break;
                }
            }
        }

        public string Message
        {
            get 
            {
                return lbl_Message.Text;
            }

            set
            {
                lbl_Message.Text = LanguageLibrary.Convert(value);
            }
        }

        public string Caption
        {
            get
            {
                return this.Text;
            }

            set
            {
                this.Text = GetTextLangauge(value);
            }
        }

        #endregion


        public MessageBoxEx()
        {
            InitializeComponent();
            _maxWidth = (int)(SystemInformation.WorkingArea.Width * 0.60);
            _maxHeight = (int)(SystemInformation.WorkingArea.Height * 0.90);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Size = new Size(_maxWidth, _maxHeight);
            //This is the rectangle in which all items will be layed out
            _maxLayoutWidth = this.ClientSize.Width - LEFT_PADDING - RIGHT_PADDING;
            _maxLayoutHeight = this.ClientSize.Height - TOP_PADDING - BOTTOM_PADDING;
            SetIconSizeAndVisibility();
            SetMessageSizeAndVisibility();
            SetOptimumSize();
            LayoutControls();
            CenterForm();
            base.OnLoad(e);

            switch (m_defaultButton)
            {
                case MessageBoxDefaultButton.Button1:
                    srmButton1.Select();
                    break;
                case MessageBoxDefaultButton.Button2:
                    srmButton2.Select();
                    break;
                case MessageBoxDefaultButton.Button3:
                    srmButton3.Select();
                    break;
            }
        }

        private void SetButtonLocation()
        {
            int buttonY = this.ClientSize.Height - BOTTOM_PADDING - srmButton1.Height;
            int buttonX;

            switch (m_buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                case MessageBoxButtons.YesNoCancel:
                    buttonX = this.Width / 2 - ITEM_PADDING - (int)(srmButton1.Width * 1.5);
                    srmButton1.Location = new Point(buttonX, buttonY);
                    buttonX = this.Width / 2 - srmButton1.Width / 2;
                    srmButton2.Location = new Point(buttonX, buttonY);
                    buttonX = this.Width / 2 + srmButton1.Width / 2 + ITEM_PADDING;
                    srmButton3.Location = new Point(buttonX, buttonY);
                    break;
                case MessageBoxButtons.OKCancel:
                case MessageBoxButtons.RetryCancel:
                case MessageBoxButtons.YesNo:
                    buttonX = this.Width / 2 - ITEM_PADDING - srmButton1.Width;
                    srmButton1.Location = new Point(buttonX, buttonY);
                    buttonX = this.Width / 2 + ITEM_PADDING;
                    srmButton2.Location = new Point(buttonX, buttonY);
                    break;
                case MessageBoxButtons.OK:
                    buttonX = this.Width / 2 - srmButton1.Width / 2;
                    srmButton1.Location = new Point(buttonX, buttonY);
                    break;
            }
        }

        private void SetMessageSizeAndVisibility()
        {
            if (lbl_Message.Text == null || lbl_Message.Text.Trim().Length == 0)
            {
                lbl_Message.Size = Size.Empty;
                lbl_Message.Visible = false;
            }
            else
            {
                int maxWidth = _maxLayoutWidth;
                if (pictureBox1.Size.Width != 0)
                {
                    maxWidth = maxWidth - (pictureBox1.Size.Width + ICON_MESSAGE_PADDING);
                }

                //We need to account for scroll bar width and height, otherwise for certains
                //kinds of text the scroll bar shows up unnecessarily
                maxWidth = maxWidth - SystemInformation.VerticalScrollBarWidth;
                Size messageRectSize = MeasureString(lbl_Message.Text, maxWidth);

                messageRectSize.Width += SystemInformation.VerticalScrollBarWidth;
                messageRectSize.Height = Math.Max(pictureBox1.Height, messageRectSize.Height) + SystemInformation.HorizontalScrollBarHeight;

                lbl_Message.Size = messageRectSize;
                lbl_Message.Visible = true;
            }
        }

        private Size MeasureString(string str, int maxWidth)
        {
            return MeasureString(str, maxWidth, font);
        }

        private Size MeasureString(string text, int width, Font font)
         {
             Graphics g = this.CreateGraphics();
             
             SizeF rectSizeF = g.MeasureString(text, font, width);
             g.Dispose();

             return new Size(Convert.ToInt32(Math.Ceiling(rectSizeF.Width)), Convert.ToInt32(Math.Ceiling(rectSizeF.Height)));               
         }

        private void SetOptimumSize()
        {
            int ncWidth = this.Width - this.ClientSize.Width;
            int ncHeight = this.Height - this.ClientSize.Height;

            int iconAndMessageRowWidth = lbl_Message.Width + ICON_MESSAGE_PADDING + pictureBox1.Width;
            int saveResponseRowWidth =  (int)(pictureBox1.Width / 2);
            int buttonsRowWidth = GetWidthOfAllButtons();
            int captionWidth = GetCaptionSize().Width;

            int maxItemWidth = Math.Max((saveResponseRowWidth), Math.Max(iconAndMessageRowWidth, buttonsRowWidth));
            int requiredWidth = LEFT_PADDING + maxItemWidth + RIGHT_PADDING + ncWidth;
            int requiredHeight = TOP_PADDING + Math.Max(lbl_Message.Height, pictureBox1.Height) + ITEM_PADDING +  ITEM_PADDING + GetButtonSize().Height + BOTTOM_PADDING + ncHeight;
           
            if (requiredHeight > _maxHeight)
           {
                lbl_Message.Height -= requiredHeight - _maxHeight;
            }

            int height = Math.Min(requiredHeight, _maxHeight);
            int width = Math.Min(requiredWidth, _maxWidth);
            this.Size = new Size(width, height);
        }

        private int GetWidthOfAllButtons()
        {
            Size buttonSize = GetButtonSize();
            int allButtonsWidth = buttonSize.Width * _buttons.Count + BUTTON_PADDING * (_buttons.Count - 1);

            return allButtonsWidth;
        }

        private Size GetCaptionSize()
        {
            Font captionFont = GetCaptionFont();
            if (captionFont == null)
            {
                //some error occured while determining system font
                captionFont = new Font("Tahoma", 8);
            }

            int availableWidth = _maxWidth - SystemInformation.CaptionButtonSize.Width - SystemInformation.Border3DSize.Width * 2;
            Size captionSize = MeasureString(this.Text, availableWidth, captionFont);

            captionSize.Width += SystemInformation.CaptionButtonSize.Width + SystemInformation.Border3DSize.Width * 2;
            return captionSize;
        }

        private Size GetButtonSize()
        {
            string longestButtonText = "cancel";
            //string longestButtonText = GetLongestButtonText();
            if (longestButtonText == null)
            {
                //TODO:Handle this case
            }

            Size buttonTextSize = MeasureString(longestButtonText, _maxLayoutWidth);
            Size buttonSize = new Size(buttonTextSize.Width + BUTTON_LEFT_PADDING + BUTTON_RIGHT_PADDING,
                buttonTextSize.Height + BUTTON_TOP_PADDING + BUTTON_BOTTOM_PADDING);

            if (buttonSize.Width < MIN_BUTTON_WIDTH)
                buttonSize.Width = MIN_BUTTON_WIDTH;
            if (buttonSize.Height < MIN_BUTTON_HEIGHT)
                buttonSize.Height = MIN_BUTTON_HEIGHT;

            return buttonSize;
        }

        /*private string GetLongestButtonText()
        {
            int maxLen = 0;
            string maxStr = null;
            foreach (MessageBoxEx button in _buttons)
            {
                if (button.Text != null && button.Text.Length > maxLen)
                {
                    maxLen = button.Text.Length;
                    maxStr = button.Text;
                }
            }
            return maxStr;
        }*/


        private Font GetCaptionFont()
        {
            NONCLIENTMETRICS ncm = new NONCLIENTMETRICS();
            ncm.cbSize = Marshal.SizeOf(typeof(NONCLIENTMETRICS));
            try
            {
                bool result = SystemParametersInfo(SPI_GETNONCLIENTMETRICS, ncm.cbSize, ref ncm, 0);

                if (result)
                {
                    return Font.FromLogFont(ncm.lfCaptionFont);
                }
                else
                {
                    int lastError = Marshal.GetLastWin32Error();
                    return null;
                }
            }
            catch (Exception /*ex*/)
            {
                //System.Console.WriteLine(ex.Message);
            }
            return null;
        }

        private string GetTextLangauge(string strText)
        {
            CultureInfo culture = CultureInfo.CurrentUICulture;
            string strResult = strText;

            switch (strText)
            {
                case "Abort":
                    {
                        if (culture.Name == "zh-CHS")
                            strResult = "退出";
                        else if (culture.Name == "zh-CHT")
                            strResult = "退出";
                        else
                            strResult = "Abort";
                    }
                    break;
                case "Retry":
                    {
                        if (culture.Name == "zh-CHS")
                            strResult = "重试";
                        else if (culture.Name == "zh-CHT")
                            strResult = "重試";
                        else
                            strResult = "Retry";
                    }
                    break;
                case "Ignore":
                    {
                        if (culture.Name == "zh-CHS")
                            strResult = "忽视";
                        else if (culture.Name == "zh-CHT")
                            strResult = "忽視";
                        else
                            strResult = "Ignore";
                    }
                    break;
                case "OK":
                    {
                        if (culture.Name == "zh-CHS")
                            strResult = "确定";
                        else if (culture.Name == "zh-CHT")
                            strResult = "確定";
                        else
                            strResult = "OK";
                    }
                    break;
                case "Cancel":
                    {
                        if (culture.Name == "zh-CHS")
                            strResult = "取消";
                        else if (culture.Name == "zh-CHT")
                            strResult = "取消";
                        else
                            strResult = "Cancel";
                    }
                    break;
                case "Yes":
                    {
                        if (culture.Name == "zh-CHS")
                            strResult = "是";
                        else if (culture.Name == "zh-CHT")
                            strResult = "是";
                        else
                            strResult = "Yes";
                    }
                    break;
                case "No":
                    {
                        if (culture.Name == "zh-CHS")
                            strResult = "否";
                        else if (culture.Name == "zh-CHT")
                            strResult = "否";
                        else
                            strResult = "No";
                    }
                    break;
            }

            return strResult;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct NONCLIENTMETRICS
        {
            public int cbSize;
            public int iBorderWidth;
            public int iScrollWidth;
            public int iScrollHeight;
            public int iCaptionWidth;
            public int iCaptionHeight;
            public LOGFONT lfCaptionFont;
            public int iSmCaptionWidth;
            public int iSmCaptionHeight;
            public LOGFONT lfSmCaptionFont;
            public int iMenuWidth;
            public int iMenuHeight;
            public LOGFONT lfMenuFont;
            public LOGFONT lfStatusFont;
            public LOGFONT lfMessageFont;
        }

        private const int SPI_GETNONCLIENTMETRICS = 41;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceSize;
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SystemParametersInfo(int uiAction, int uiParam,
            ref NONCLIENTMETRICS ncMetrics, int fWinIni);


        private void LayoutControls()
        {
            pictureBox1.Location = new Point(LEFT_PADDING, TOP_PADDING);
            lbl_Message.Location = new Point(LEFT_PADDING + pictureBox1.Width + ICON_MESSAGE_PADDING * (pictureBox1.Width == 0 ? 0 : 1), TOP_PADDING);

            Size buttonSize = GetButtonSize();
            int allButtonsWidth = GetWidthOfAllButtons();

            int firstButtonX = ((int)(this.ClientSize.Width - allButtonsWidth) / 2);
            int firstButtonY = this.ClientSize.Height - BOTTOM_PADDING - buttonSize.Height;
            Point nextButtonLocation = new Point(firstButtonX, firstButtonY);

            SetButtonLocation();
            nextButtonLocation.X += buttonSize.Width + BUTTON_PADDING;
        }

        private Button GetButton(MessageBoxEx button, Size size, Point location)
        {
            Button buttonCtrl = null;
            if (_buttonControlsTable.ContainsKey(button))
            {
                buttonCtrl = _buttonControlsTable[button] as Button;
                buttonCtrl.Size = size;
                buttonCtrl.Location = location;
            }
            return buttonCtrl;
        }

        /// <summary>
        /// Sets the size and visibility of the Icon
        /// </summary>
        private void SetIconSizeAndVisibility()
        {
            if (pictureBox1.Image == null)
            {
                pictureBox1.Visible = false;
                pictureBox1.Size = Size.Empty;
            }
            else
            {
                pictureBox1.Size = new Size(32, 32);
                pictureBox1.Visible = true;
            }
        }

        /// <summary>
        /// Centers the form on the screen
        /// </summary>
        private void CenterForm()
        {
            int x = (SystemInformation.WorkingArea.Width - this.Width) / 2;
            int y = (SystemInformation.WorkingArea.Height - this.Height) / 2;

            this.Location = new Point(x, y);
        }
        
    }
}