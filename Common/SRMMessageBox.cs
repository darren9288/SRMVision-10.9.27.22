using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Common
{
    public class SRMMessageBox
    {
        public static DialogResult Show(string messageText, string messageCaption, MessageBoxButtons messageBoxButton, MessageBoxIcon messageBoxIcon, MessageBoxDefaultButton messageBoxDefaultButton)
        {
            MessageBoxEx messageDialog = new MessageBoxEx();
            messageDialog.Message = messageText;
            messageDialog.Text = messageCaption;
            messageDialog.Buttons = messageBoxButton;
            messageDialog.Icons = messageBoxIcon;
            messageDialog.DefaultButton = messageBoxDefaultButton;
            messageDialog.Activate();
            return messageDialog.ShowDialog();
         }

        public static DialogResult Show(string messageText, string messageCaption, MessageBoxButtons messageBoxButton, MessageBoxIcon messageBoxIcon)
        {
            MessageBoxEx messageDialog = new MessageBoxEx();
            messageDialog.Message = messageText;
            messageDialog.Text = messageCaption;
            messageDialog.Buttons = messageBoxButton;
            messageDialog.Icons = messageBoxIcon;
            messageDialog.DefaultButton = MessageBoxDefaultButton.Button1;
            messageDialog.Activate();
            return messageDialog.ShowDialog();
        }

        public static DialogResult Show(string messageText, string messageCaption, MessageBoxButtons messageBoxButton)
        {
            MessageBoxEx messageDialog = new MessageBoxEx();
            messageDialog.Message = messageText;
            messageDialog.Text = messageCaption;
            messageDialog.Buttons = messageBoxButton;
            messageDialog.Icons = MessageBoxIcon.None;
            
            messageDialog.DefaultButton = MessageBoxDefaultButton.Button1;
            messageDialog.Activate();
            return messageDialog.ShowDialog();
        }

        public static DialogResult Show(string messageText, string messageCaption)
        {
            MessageBoxEx messageDialog = new MessageBoxEx();
            messageDialog.Message = messageText;
            messageDialog.Text = messageCaption;
            messageDialog.Buttons = MessageBoxButtons.OK;
            messageDialog.Icons = MessageBoxIcon.None;
            messageDialog.DefaultButton = MessageBoxDefaultButton.Button1;
            messageDialog.Activate();
            return messageDialog.ShowDialog();
        }

        public static DialogResult Show(string messageText)
        {
            MessageBoxEx messageDialog = new MessageBoxEx();
            messageDialog.Message = messageText;
            messageDialog.Text = "SRM Message";
            messageDialog.Buttons = MessageBoxButtons.OK;
            messageDialog.Icons = MessageBoxIcon.None;
            messageDialog.DefaultButton = MessageBoxDefaultButton.Button1;
            messageDialog.Activate();
            return messageDialog.ShowDialog();
        }
     
    }
}
