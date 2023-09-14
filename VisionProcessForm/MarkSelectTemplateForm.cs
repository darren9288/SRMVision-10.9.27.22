using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedMemory;
using Common;
using VisionProcessing;
using System.IO;
namespace VisionProcessForm
{
    public partial class MarkSelectTemplateForm : Form
    {
        #region Member Variables
        private int m_intNumTemplates;
        private int m_intSelectedTemplate;
        private int m_intSelectedGroup;
        private string m_strVisionName;
        private string m_strSelectedRecipe;
        private string m_strRecipePath;
        #endregion

        #region Properties
        public int ref_intSelectedTemplate { get { return m_intSelectedTemplate; } }

        #endregion

        public MarkSelectTemplateForm(int intSelectedGroup, int intNumTemplates, int intSelectedTemplate, string strVisionName, string strSelectedRecipe, string strRecipePath, bool blnWantPin1)
        {
            InitializeComponent();
            m_intSelectedTemplate = intSelectedTemplate;
            m_intNumTemplates = intNumTemplates;
            m_intSelectedGroup = intSelectedGroup;
            m_strVisionName = strVisionName;
            m_strSelectedRecipe = strSelectedRecipe;
            m_strRecipePath = strRecipePath;
            if (blnWantPin1)
                UpdatePin1GUI();
            else
                UpdateMarkOrientGUI();
        }

        private void UpdateMarkOrientGUI()
        {
            string strPath = m_strRecipePath + m_strSelectedRecipe +
                "\\" + m_strVisionName + "\\Mark\\Template\\Template" + m_intSelectedGroup;

            switch (m_intSelectedTemplate)
            {
                case 0:
                    radioBtn_Template1.Checked = true;
                    break;
                case 1:
                    radioBtn_Template2.Checked = true;
                    break;
                case 2:
                    radioBtn_Template3.Checked = true;
                    break;
                case 3:
                    radioBtn_Template4.Checked = true;
                    break;
                case 4:
                    radioBtn_Template5.Checked = true;
                    break;
                case 5:
                    radioBtn_Template6.Checked = true;
                    break;
                case 6:
                    radioBtn_Template7.Checked = true;
                    break;
                case 7:
                    radioBtn_Template8.Checked = true;
                    break;
            }

            for (int i = 0; i < m_intNumTemplates; i++)
            {
                if (i == 0 && File.Exists(strPath + "_0.bmp"))
                {
                    pic_Learn1.Load(strPath + "_0.bmp");
                    radioBtn_Template1.Enabled = true;
                }
                if (i == 1 && File.Exists(strPath + "_1.bmp"))
                {
                    pic_Learn2.Load(strPath + "_1.bmp");
                    radioBtn_Template2.Enabled = true;
                }
                if (i == 2 && File.Exists(strPath + "_2.bmp"))
                {
                    pic_Learn3.Load(strPath + "_2.bmp");
                    radioBtn_Template3.Enabled = true;
                }
                if (i == 3 && File.Exists(strPath + "_3.bmp"))
                {
                    pic_Learn4.Load(strPath + "_3.bmp");
                    radioBtn_Template4.Enabled = true;
                }
                if (i == 4 && File.Exists(strPath + "_4.bmp"))
                {
                    pic_Learn5.Load(strPath + "_4.bmp");
                    radioBtn_Template5.Enabled = true;
                }
                if (i == 5 && File.Exists(strPath + "_5.bmp"))
                {
                    pic_Learn6.Load(strPath + "_5.bmp");
                    radioBtn_Template6.Enabled = true;
                }
                if (i == 6 && File.Exists(strPath + "_6.bmp"))
                {
                    pic_Learn7.Load(strPath + "_6.bmp");
                    radioBtn_Template7.Enabled = true;
                }
                if (i == 7 && File.Exists(strPath + "_7.bmp"))
                {
                    pic_Learn8.Load(strPath + "_7.bmp");
                    radioBtn_Template8.Enabled = true;
                }
                
            }
            
        }

        private void UpdatePin1GUI()
        {
            string strPath = m_strRecipePath + m_strSelectedRecipe +
                "\\" + m_strVisionName + "\\Orient\\Template\\Pin1Template";

            switch (m_intSelectedTemplate)
            {
                case 0:
                    radioBtn_Template1.Checked = true;
                    break;
                case 1:
                    radioBtn_Template2.Checked = true;
                    break;
                case 2:
                    radioBtn_Template3.Checked = true;
                    break;
                case 3:
                    radioBtn_Template4.Checked = true;
                    break;
                case 4:
                    radioBtn_Template5.Checked = true;
                    break;
                case 5:
                    radioBtn_Template6.Checked = true;
                    break;
                case 6:
                    radioBtn_Template7.Checked = true;
                    break;
                case 7:
                    radioBtn_Template8.Checked = true;
                    break;
            }

            for (int i = 0; i < m_intNumTemplates; i++)
            {
                if (i == 0 && File.Exists(strPath + "0.bmp"))
                {
                    pic_Learn1.Load(strPath + "0.bmp");
                    radioBtn_Template1.Enabled = true;
                }
                if (i == 1 && File.Exists(strPath + "1.bmp"))
                {
                    pic_Learn2.Load(strPath + "1.bmp");
                    radioBtn_Template2.Enabled = true;
                }
                if (i == 2 && File.Exists(strPath + "2.bmp"))
                {
                    pic_Learn3.Load(strPath + "2.bmp");
                    radioBtn_Template3.Enabled = true;
                }
                if (i == 3 && File.Exists(strPath + "3.bmp"))
                {
                    pic_Learn4.Load(strPath + "3.bmp");
                    radioBtn_Template4.Enabled = true;
                }
                if (i == 4 && File.Exists(strPath + "4.bmp"))
                {
                    pic_Learn5.Load(strPath + "4.bmp");
                    radioBtn_Template5.Enabled = true;
                }
                if (i == 5 && File.Exists(strPath + "5.bmp"))
                {
                    pic_Learn6.Load(strPath + "5.bmp");
                    radioBtn_Template6.Enabled = true;
                }
                if (i == 6 && File.Exists(strPath + "6.bmp"))
                {
                    pic_Learn7.Load(strPath + "6.bmp");
                    radioBtn_Template7.Enabled = true;
                }
                if (i == 7 && File.Exists(strPath + "7.bmp"))
                {
                    pic_Learn8.Load(strPath + "7.bmp");
                    radioBtn_Template8.Enabled = true;
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
            if (radioBtn_Template1.Checked)
                m_intSelectedTemplate = 0;
            else if (radioBtn_Template2.Checked)
                m_intSelectedTemplate = 1;
            else if (radioBtn_Template3.Checked)
                m_intSelectedTemplate = 2;
            else if (radioBtn_Template4.Checked)
                m_intSelectedTemplate = 3;
            else if (radioBtn_Template5.Checked)
                m_intSelectedTemplate = 4;
            else if (radioBtn_Template6.Checked)
                m_intSelectedTemplate = 5;
            else if (radioBtn_Template7.Checked)
                m_intSelectedTemplate = 6;
            else if (radioBtn_Template8.Checked)
                m_intSelectedTemplate = 7;
            Close();
            Dispose();
        }
    }
}
