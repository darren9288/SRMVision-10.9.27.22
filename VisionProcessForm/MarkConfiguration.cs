using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharedMemory;
using Common;
using VisionProcessing;


namespace VisionProcessForm
{
    public partial class MarkConfiguration : Form
    {
        #region Member Variables
        private int m_intNumTemplates;
        private int m_intSelectedGroup;
        private int m_intTemplateMask;
        private long m_intTemplatePriority;
        private string m_strVisionName;
        private string m_strSelectedRecipe;
        private string m_strRecipePath;
        #endregion

        #region Properties
        public int ref_intTemplateMask { get { return m_intTemplateMask; } }
        public long ref_intTemplatePriority { get { return m_intTemplatePriority; } }

        #endregion
        
        public MarkConfiguration(int intSelectedGroup, int intNumTemplates, int intTemplateMask, 
                long intTemplatePriority, string strVisionName, string strSelectedRecipe, string strRecipePath)
        {
            InitializeComponent();
            m_intNumTemplates = intNumTemplates;
            m_intSelectedGroup = intSelectedGroup;
            m_intTemplateMask = intTemplateMask;
            m_intTemplatePriority = intTemplatePriority;
            m_strVisionName = strVisionName;
            m_strSelectedRecipe = strSelectedRecipe;
            m_strRecipePath = strRecipePath;
            UpdateGUI();

        }

        private void UpdateGUI()
        {
            string strPath = m_strRecipePath + m_strSelectedRecipe +
                "\\" + m_strVisionName + "\\Mark\\Template\\Template" + m_intSelectedGroup;               

            if ((m_intTemplateMask & 0x01) > 0)
                chk_Template1.Checked = true;
            else
                chk_Template1.Checked = false;

            if ((m_intTemplateMask & 0x02) > 0)
                chk_Template2.Checked = true;
            else
                chk_Template2.Checked = false;

            if ((m_intTemplateMask & 0x04) > 0)
                chk_Template3.Checked = true;
            else
                chk_Template3.Checked = false;

            if ((m_intTemplateMask & 0x08) > 0)
                chk_Template4.Checked = true;
            else
                chk_Template4.Checked = false;

            if ((m_intTemplateMask & 0x10) > 0)
                chk_Template5.Checked = true;
            else
                chk_Template5.Checked = false;

            if ((m_intTemplateMask & 0x20) > 0)
                chk_Template6.Checked = true;
            else
                chk_Template6.Checked = false;

            if ((m_intTemplateMask & 0x40) > 0)
                chk_Template7.Checked = true;
            else
                chk_Template7.Checked = false;

            if ((m_intTemplateMask & 0x80) > 0)
                chk_Template8.Checked = true;
            else
                chk_Template8.Checked = false;

            for (int i = 0; i < m_intNumTemplates; i++)
            {
                if (i == 0)
                {
                    pic_Learn1.Load(strPath + "_0.bmp");
                    chk_Template1.Enabled = true;
                }
                if (i == 1)
                {
                    pic_Learn2.Load(strPath + "_1.bmp");
                    chk_Template2.Enabled = true;
                }
                if (i == 2)
                {
                    pic_Learn3.Load(strPath + "_2.bmp");
                    chk_Template3.Enabled = true;
                }
                if (i == 3)
                {
                    pic_Learn4.Load(strPath + "_3.bmp");
                    chk_Template4.Enabled = true;
                }
                if (i == 4)
                {
                    pic_Learn5.Load(strPath + "_4.bmp");
                    chk_Template5.Enabled = true;
                }
                if (i == 5)
                {
                    pic_Learn6.Load(strPath + "_5.bmp");
                    chk_Template6.Enabled = true;
                }
                if (i == 6)
                {
                    pic_Learn7.Load(strPath + "_6.bmp");
                    chk_Template7.Enabled = true;
                }
                if (i == 7)
                {
                    pic_Learn8.Load(strPath + "_7.bmp");
                    chk_Template8.Enabled = true;
                }

                int intLearnNo;
                if (m_intTemplatePriority == 0)
                    intLearnNo = i + 1;
                else
                {
                    intLearnNo = (int)((long)(m_intTemplatePriority & (0x0F << (0x04 * i))) >> (0x04 * i));
                    if (intLearnNo == 0)
                        intLearnNo = i + 1;
                }
                lst_learnNo.Items.Add("Template " + intLearnNo.ToString());
            }

            lst_learnNo.SelectedIndex = m_intNumTemplates - 1;
        }


        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void btn_Down_Click(object sender, EventArgs e)
        {
            if ((lst_learnNo.SelectedIndex < 0) || (lst_learnNo.SelectedIndex == lst_learnNo.Items.Count - 1))
                return;
            int intSelectedIndex = lst_learnNo.SelectedIndex;
            string strLearnNo = lst_learnNo.SelectedItem.ToString();
            lst_learnNo.Items.RemoveAt(intSelectedIndex);
            lst_learnNo.Items.Insert(intSelectedIndex + 1, strLearnNo);
            lst_learnNo.SelectedIndex = intSelectedIndex + 1;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            m_intTemplateMask = 0;
            if (chk_Template1.Checked)
                m_intTemplateMask |= 0x01;
            if (chk_Template2.Checked)
                m_intTemplateMask |= 0x02;
            if (chk_Template3.Checked)
                m_intTemplateMask |= 0x04;
            if (chk_Template4.Checked)
                m_intTemplateMask |= 0x08;
            if (chk_Template5.Checked)
                m_intTemplateMask |= 0x10;
            if (chk_Template6.Checked)
                m_intTemplateMask |= 0x20;
            if (chk_Template7.Checked)
                m_intTemplateMask |= 0x40;
            if (chk_Template8.Checked)
                m_intTemplateMask |= 0x80;

            m_intTemplatePriority = 0;
            for (int i = 0; i < lst_learnNo.Items.Count; i++)
            {
                int intLearnNo = Convert.ToInt32(lst_learnNo.Items[i].ToString().Substring(9));
                m_intTemplatePriority |= (intLearnNo << (4 * i));
            }

            Close();
            Dispose();
        }

        private void btn_Up_Click(object sender, EventArgs e)
        {
            if (lst_learnNo.SelectedIndex <= 0)
                return;
            int intSelectedIndex = lst_learnNo.SelectedIndex;
            string strLearnNo = lst_learnNo.SelectedItem.ToString();
            lst_learnNo.Items.RemoveAt(intSelectedIndex);
            lst_learnNo.Items.Insert(intSelectedIndex - 1, strLearnNo);
            lst_learnNo.SelectedIndex = intSelectedIndex - 1;
        }


        private void chk_Template1_Click(object sender, EventArgs e)
        {
            if (!chk_Template1.Checked && !chk_Template2.Checked &&
                !chk_Template3.Checked && !chk_Template4.Checked &&
                !chk_Template5.Checked && !chk_Template6.Checked &&
                !chk_Template7.Checked && !chk_Template8.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_Template1.Checked = true;
            }
        }

        private void chk_Template2_Click(object sender, EventArgs e)
        {
            if (!chk_Template1.Checked && !chk_Template2.Checked &&
                !chk_Template3.Checked && !chk_Template4.Checked &&
                !chk_Template5.Checked && !chk_Template6.Checked &&
                !chk_Template7.Checked && !chk_Template8.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_Template2.Checked = true;
            }
        }

        private void chk_Template3_Click(object sender, EventArgs e)
        {
            if (!chk_Template1.Checked && !chk_Template2.Checked &&
                !chk_Template3.Checked && !chk_Template4.Checked &&
                !chk_Template5.Checked && !chk_Template6.Checked &&
                !chk_Template7.Checked && !chk_Template8.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_Template3.Checked = true;
            }
        }

        private void chk_Template4_Click(object sender, EventArgs e)
        {
            if (!chk_Template1.Checked && !chk_Template2.Checked &&
                !chk_Template3.Checked && !chk_Template4.Checked &&
                !chk_Template5.Checked && !chk_Template6.Checked &&
                !chk_Template7.Checked && !chk_Template8.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_Template4.Checked = true;
            }
        }

        private void Testing()
        {
            Bitmap objBitMap = new Bitmap(100, 100);
            pic_Learn1.Image = objBitMap;
        }

        private void chk_Template5_Click(object sender, EventArgs e)
        {
            if (!chk_Template1.Checked && !chk_Template2.Checked &&
                !chk_Template3.Checked && !chk_Template4.Checked &&
                !chk_Template5.Checked && !chk_Template6.Checked &&
                !chk_Template7.Checked && !chk_Template8.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_Template5.Checked = true;
            }
        }

        private void chk_Template6_Click(object sender, EventArgs e)
        {
            if (!chk_Template1.Checked && !chk_Template2.Checked &&
                !chk_Template3.Checked && !chk_Template4.Checked &&
                !chk_Template5.Checked && !chk_Template6.Checked &&
                !chk_Template7.Checked && !chk_Template8.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_Template6.Checked = true;
            }
        }

        private void chk_Template7_Click(object sender, EventArgs e)
        {
            if (!chk_Template1.Checked && !chk_Template2.Checked &&
                !chk_Template3.Checked && !chk_Template4.Checked &&
                !chk_Template5.Checked && !chk_Template6.Checked &&
                !chk_Template7.Checked && !chk_Template8.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_Template7.Checked = true;
            }
        }

        private void chk_Template8_Click(object sender, EventArgs e)
        {
            if (!chk_Template1.Checked && !chk_Template2.Checked &&
                !chk_Template3.Checked && !chk_Template4.Checked &&
                !chk_Template5.Checked && !chk_Template6.Checked &&
                !chk_Template7.Checked && !chk_Template8.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_Template8.Checked = true;
            }
        }
    }
}