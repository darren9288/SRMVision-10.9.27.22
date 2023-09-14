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
using System.IO;


namespace VisionProcessForm
{
    public partial class SealConfiguration : Form
    {
        #region Member Variables
        private int m_intNumPocketTemplates;
        private int m_intNumMarkTemplates;
        private int m_intPocketTemplateMask;
        private int m_intMarkTemplateMask;
        private string m_strVisionName;
        private string m_strSelectedRecipe;
        private string m_strRecipePath;
        #endregion

        #region Properties
        public int ref_intPocketTemplateMask { get { return m_intPocketTemplateMask; } }
        public int ref_intMarkTemplateMask { get { return m_intMarkTemplateMask; } }

        #endregion

        public SealConfiguration(int intNumPocketTemplates, int intNumMarkTemplates, int intPocketTemplateMask, int intMarkTemplateMask, string strVisionName, string strRecipePath, string strSelectedRecipe)
        {
            InitializeComponent();

            m_intNumPocketTemplates = intNumPocketTemplates;
            m_intNumMarkTemplates = intNumMarkTemplates;
            m_intPocketTemplateMask = intPocketTemplateMask;
            m_intMarkTemplateMask = intMarkTemplateMask;
            m_strVisionName = strVisionName;
            m_strSelectedRecipe = strSelectedRecipe;
            m_strRecipePath = strRecipePath;
            UpdateGUI();

        }

        private void UpdateGUI()
        {
            string strPath = m_strRecipePath + m_strSelectedRecipe +
                "\\" + m_strVisionName + "\\Seal\\Template\\";
           
            //Pocket
            if ((m_intPocketTemplateMask & 0x01) > 0)
                chk_PocketTemplate1.Checked = true;
            else
                chk_PocketTemplate1.Checked = false;

            if ((m_intPocketTemplateMask & 0x02) > 0)
                chk_PocketTemplate2.Checked = true;
            else
                chk_PocketTemplate2.Checked = false;

            if ((m_intPocketTemplateMask & 0x04) > 0)
                chk_PocketTemplate3.Checked = true;
            else
                chk_PocketTemplate3.Checked = false;

            if ((m_intPocketTemplateMask & 0x08) > 0)
                chk_PocketTemplate4.Checked = true;
            else
                chk_PocketTemplate4.Checked = false;

            for (int i = 0; i < m_intNumPocketTemplates; i++)
            {
                if (i == 0)
                {
                    if (!File.Exists(strPath + "Pocket\\PocketTemplate0_0.bmp"))
                        return;
                    pic_LearnPocket1.Load(strPath + "Pocket\\PocketTemplate0_0.bmp");
                    chk_PocketTemplate1.Enabled = true;
                }
                if (i == 1)
                {
                    if (!File.Exists(strPath + "Pocket\\PocketTemplate0_1.bmp"))
                        return;
                    pic_LearnPocket2.Load(strPath + "Pocket\\PocketTemplate0_1.bmp");
                    chk_PocketTemplate2.Enabled = true;
                }
                if (i == 2)
                {
                    if (!File.Exists(strPath + "Pocket\\PocketTemplate0_2.bmp"))
                        return;
                    pic_LearnPocket3.Load(strPath + "Pocket\\PocketTemplate0_2.bmp");
                    chk_PocketTemplate3.Enabled = true;
                }
                if (i == 3)
                {
                    if (!File.Exists(strPath + "Pocket\\PocketTemplate0_3.bmp"))
                        return;
                    pic_LearnPocket4.Load(strPath + "Pocket\\PocketTemplate0_3.bmp");
                    chk_PocketTemplate4.Enabled = true;
                }
            }

            //Mark
            if ((m_intMarkTemplateMask & 0x01) > 0)
                chk_MarkTemplate1.Checked = true;
            else
                chk_MarkTemplate1.Checked = false;

            if ((m_intMarkTemplateMask & 0x02) > 0)
                chk_MarkTemplate2.Checked = true;
            else
                chk_MarkTemplate2.Checked = false;

            if ((m_intMarkTemplateMask & 0x04) > 0)
                chk_MarkTemplate3.Checked = true;
            else
                chk_MarkTemplate3.Checked = false;

            if ((m_intMarkTemplateMask & 0x08) > 0)
                chk_MarkTemplate4.Checked = true;
            else
                chk_MarkTemplate4.Checked = false;

            if ((m_intMarkTemplateMask & 0x10) > 0)
                chk_MarkTemplate5.Checked = true;
            else
                chk_MarkTemplate5.Checked = false;

            if ((m_intMarkTemplateMask & 0x20) > 0)
                chk_MarkTemplate6.Checked = true;
            else
                chk_MarkTemplate6.Checked = false;

            if ((m_intMarkTemplateMask & 0x40) > 0)
                chk_MarkTemplate7.Checked = true;
            else
                chk_MarkTemplate7.Checked = false;

            if ((m_intMarkTemplateMask & 0x80) > 0)
                chk_MarkTemplate8.Checked = true;
            else
                chk_MarkTemplate8.Checked = false;

            for (int i = 0; i < m_intNumMarkTemplates; i++)
            {
                if (i == 0)
                {
                    if (!File.Exists(strPath + "Mark\\MarkTemplate0_0.bmp"))
                        return;
                    pic_LearnMark1.Load(strPath + "Mark\\MarkTemplate0_0.bmp");
                    chk_MarkTemplate1.Enabled = true;
                }
                if (i == 1)
                {
                    if (!File.Exists(strPath + "Mark\\MarkTemplate0_1.bmp"))
                        return;
                    pic_LearnMark2.Load(strPath + "Mark\\MarkTemplate0_1.bmp");
                    chk_MarkTemplate2.Enabled = true;
                }
                if (i == 2)
                {
                    if (!File.Exists(strPath + "Mark\\MarkTemplate0_2.bmp"))
                        return;
                    pic_LearnMark3.Load(strPath + "Mark\\MarkTemplate0_2.bmp");
                    chk_MarkTemplate3.Enabled = true;
                }
                if (i == 3)
                {
                    if (!File.Exists(strPath + "Mark\\MarkTemplate0_3.bmp"))
                        return;
                    pic_LearnMark4.Load(strPath + "Mark\\MarkTemplate0_3.bmp");
                    chk_MarkTemplate4.Enabled = true;
                }
                if (i == 4)
                {
                    if (!File.Exists(strPath + "Mark\\MarkTemplate0_4.bmp"))
                        return;
                    pic_LearnMark5.Load(strPath + "Mark\\MarkTemplate0_4.bmp");
                    chk_MarkTemplate5.Enabled = true;
                }
                if (i == 5)
                {
                    if (!File.Exists(strPath + "Mark\\MarkTemplate0_5.bmp"))
                        return;
                    pic_LearnMark6.Load(strPath + "Mark\\MarkTemplate0_5.bmp");
                    chk_MarkTemplate6.Enabled = true;
                }
                if (i == 6)
                {
                    if (!File.Exists(strPath + "Mark\\MarkTemplate0_6.bmp"))
                        return;
                    pic_LearnMark7.Load(strPath + "Mark\\MarkTemplate0_6.bmp");
                    chk_MarkTemplate7.Enabled = true;
                }
                if (i == 7)
                {
                    if (!File.Exists(strPath + "Mark\\MarkTemplate0_7.bmp"))
                        return;
                    pic_LearnMark8.Load(strPath + "Mark\\MarkTemplate0_7.bmp");
                    chk_MarkTemplate8.Enabled = true;
                }
            }
        }


        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            m_intPocketTemplateMask = 0;
            if (chk_PocketTemplate1.Checked)
                m_intPocketTemplateMask |= 0x01;
            if (chk_PocketTemplate2.Checked)
                m_intPocketTemplateMask |= 0x02;
            if (chk_PocketTemplate3.Checked)
                m_intPocketTemplateMask |= 0x04;
            if (chk_PocketTemplate4.Checked)
                m_intPocketTemplateMask |= 0x08;

            m_intMarkTemplateMask = 0;
            if (chk_MarkTemplate1.Checked)
                m_intMarkTemplateMask |= 0x01;
            if (chk_MarkTemplate2.Checked)
                m_intMarkTemplateMask |= 0x02;
            if (chk_MarkTemplate3.Checked)
                m_intMarkTemplateMask |= 0x04;
            if (chk_MarkTemplate4.Checked)
                m_intMarkTemplateMask |= 0x08;
            if (chk_MarkTemplate5.Checked)
                m_intMarkTemplateMask |= 0x10;
            if (chk_MarkTemplate6.Checked)
                m_intMarkTemplateMask |= 0x20;
            if (chk_MarkTemplate7.Checked)
                m_intMarkTemplateMask |= 0x40;
            if (chk_MarkTemplate8.Checked)
                m_intMarkTemplateMask |= 0x80;

            Close();
            Dispose();
        }

        private void chk_Template1_Click(object sender, EventArgs e)
        {
            if (!chk_PocketTemplate1.Checked && !chk_PocketTemplate2.Checked &&
                !chk_PocketTemplate3.Checked && !chk_PocketTemplate4.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_PocketTemplate1.Checked = true;
            }
        }

        private void chk_Template2_Click(object sender, EventArgs e)
        {
            if (!chk_PocketTemplate1.Checked && !chk_PocketTemplate2.Checked &&
                !chk_PocketTemplate3.Checked && !chk_PocketTemplate4.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_PocketTemplate2.Checked = true;
            }
        }

        private void chk_Template3_Click(object sender, EventArgs e)
        {
            if (!chk_PocketTemplate1.Checked && !chk_PocketTemplate2.Checked &&
                !chk_PocketTemplate3.Checked && !chk_PocketTemplate4.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_PocketTemplate3.Checked = true;
            }
        }

        private void chk_Template4_Click(object sender, EventArgs e)
        {
            if (!chk_PocketTemplate1.Checked && !chk_PocketTemplate2.Checked &&
                !chk_PocketTemplate3.Checked && !chk_PocketTemplate4.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_PocketTemplate4.Checked = true;
            }
        }

        private void chk_MarkTemplate1_Click(object sender, EventArgs e)
        {
            if (!chk_MarkTemplate1.Checked && !chk_MarkTemplate2.Checked &&
                !chk_MarkTemplate3.Checked && !chk_MarkTemplate4.Checked &&
                !chk_MarkTemplate5.Checked && !chk_MarkTemplate6.Checked &&
                !chk_MarkTemplate7.Checked && !chk_MarkTemplate8.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_MarkTemplate1.Checked = true;
            }
        }

        private void chk_MarkTemplate2_Click(object sender, EventArgs e)
        {
            if (!chk_MarkTemplate1.Checked && !chk_MarkTemplate2.Checked &&
                !chk_MarkTemplate3.Checked && !chk_MarkTemplate4.Checked &&
                !chk_MarkTemplate5.Checked && !chk_MarkTemplate6.Checked &&
                !chk_MarkTemplate7.Checked && !chk_MarkTemplate8.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_MarkTemplate2.Checked = true;
            }
        }

        private void chk_MarkTemplate3_Click(object sender, EventArgs e)
        {
            if (!chk_MarkTemplate1.Checked && !chk_MarkTemplate2.Checked &&
                !chk_MarkTemplate3.Checked && !chk_MarkTemplate4.Checked &&
                !chk_MarkTemplate5.Checked && !chk_MarkTemplate6.Checked &&
                !chk_MarkTemplate7.Checked && !chk_MarkTemplate8.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_MarkTemplate3.Checked = true;
            }
        }

        private void chk_MarkTemplate4_Click(object sender, EventArgs e)
        {
            if (!chk_MarkTemplate1.Checked && !chk_MarkTemplate2.Checked &&
                !chk_MarkTemplate3.Checked && !chk_MarkTemplate4.Checked &&
                !chk_MarkTemplate5.Checked && !chk_MarkTemplate6.Checked &&
                !chk_MarkTemplate7.Checked && !chk_MarkTemplate8.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_MarkTemplate4.Checked = true;
            }
        }

        private void chk_MarkTemplate5_Click(object sender, EventArgs e)
        {
            if (!chk_MarkTemplate1.Checked && !chk_MarkTemplate2.Checked &&
                !chk_MarkTemplate3.Checked && !chk_MarkTemplate4.Checked &&
                !chk_MarkTemplate5.Checked && !chk_MarkTemplate6.Checked &&
                !chk_MarkTemplate7.Checked && !chk_MarkTemplate8.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_MarkTemplate5.Checked = true;
            }
        }

        private void chk_MarkTemplate6_Click(object sender, EventArgs e)
        {
            if (!chk_MarkTemplate1.Checked && !chk_MarkTemplate2.Checked &&
                !chk_MarkTemplate3.Checked && !chk_MarkTemplate4.Checked &&
                !chk_MarkTemplate5.Checked && !chk_MarkTemplate6.Checked &&
                !chk_MarkTemplate7.Checked && !chk_MarkTemplate8.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_MarkTemplate6.Checked = true;
            }
        }

        private void chk_MarkTemplate7_Click(object sender, EventArgs e)
        {
            if (!chk_MarkTemplate1.Checked && !chk_MarkTemplate2.Checked &&
                !chk_MarkTemplate3.Checked && !chk_MarkTemplate4.Checked &&
                !chk_MarkTemplate5.Checked && !chk_MarkTemplate6.Checked &&
                !chk_MarkTemplate7.Checked && !chk_MarkTemplate8.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_MarkTemplate7.Checked = true;
            }
        }

        private void chk_MarkTemplate8_Click(object sender, EventArgs e)
        {
            if (!chk_MarkTemplate1.Checked && !chk_MarkTemplate2.Checked &&
                 !chk_MarkTemplate3.Checked && !chk_MarkTemplate4.Checked &&
                 !chk_MarkTemplate5.Checked && !chk_MarkTemplate6.Checked &&
                 !chk_MarkTemplate7.Checked && !chk_MarkTemplate8.Checked)
            {
                SRMMessageBox.Show("At least one template must be selected!");
                chk_MarkTemplate8.Checked = true;
            }
        }
    }
}