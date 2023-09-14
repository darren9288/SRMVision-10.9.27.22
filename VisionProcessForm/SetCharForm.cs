using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;

namespace VisionProcessForm
{
    public partial class SetCharForm : Form
    {
        #region Member Variables
        #endregion

        #region Properties
        public char ref_cCharacter { get { return Convert.ToChar(txt_Character.Text); }}
        public int ref_intClass { get { return cbo_Class.SelectedIndex; } }
        #endregion

        public SetCharForm()
        {
            InitializeComponent();

            cbo_Class.SelectedIndex = 0;
        }



        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (txt_Character.Text.Length == 0)
            {
                SRMMessageBox.Show("Character TextBox cannot be empty!");
                return;
            }

            if (txt_Character.Text.Length > 1)
            {
                SRMMessageBox.Show("Please insert 1 character only in Character TextBox!");
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void txt_Character_TextChanged(object sender, EventArgs e)
        {
            if (txt_Character.Text.Length == 0)
                return;

            char cFirstChar = Convert.ToChar(txt_Character.Text.Substring(0, 1));

            if (char.IsWhiteSpace(cFirstChar))
            {
                txt_Character.Text = "";
                return;
            }

            if (char.IsDigit(cFirstChar))
                cbo_Class.SelectedIndex = 0;
            else if (char.IsUpper(cFirstChar))
                cbo_Class.SelectedIndex = 1;
            else if (char.IsLower(cFirstChar))
                cbo_Class.SelectedIndex = 2;
            else
                cbo_Class.SelectedIndex = 3;
        }
    }
}