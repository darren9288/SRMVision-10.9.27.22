using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SharedMemory;
using Common;

namespace AutoMode
{
    public partial class TemplateImageForm : Form
    {
        private string m_strRecipeID;
        private string m_strRecipePath;
        private VisionInfo m_smVisionInfo;

        public TemplateImageForm(VisionInfo smVisionInfo, string strRecipePath, string strRecipeID)
        {
            InitializeComponent();

            m_smVisionInfo = smVisionInfo;
            m_strRecipeID = strRecipeID;
            m_strRecipePath = strRecipePath;
            LoadTemplateImages();
        }

        private void LoadTemplateImages()
        {
            // Delete all tabpage first
            tabCtrl_Templates.Controls.Clear();

            switch (m_smVisionInfo.g_strVisionName)
            {
                case "Barcode":
                    LoadBarcodeTemplateImage();
                    break;
                case "Orient":
                case "BottomOrient":
                case "BottomPosition":
                case "Mark":
                case "MarkOrient":
                case "MarkPkg":
                case "MOPkg":
                case "Package":
                    LoadMOPkgTemplateImage();
                    break;
                case "MOLiPkg":
                case "MOLi":
                    LoadMOPkgTemplateImage();
                    LoadLeadTemplateImage();
                    break;
                case "BottomPositionOrient":
                case "TapePocketPosition":
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                    LoadPadOrientTemplateImage();
                    LoadPadTemplateImage();
                    break;
                case "Pad":
                case "PadPos":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    LoadPadTemplateImage();
                    break;
                case "Li3D":
                case "Li3DPkg":
                    LoadLead3DTemplateImage();
                    break;
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                    LoadInPocketTemplateImage();
                    break;
                case "IPMLi":
                case "IPMLiPkg":
                    LoadInPocketTemplateImage();
                    LoadLeadTemplateImage();
                    break;
                case "Seal":
                    LoadSealTemplateImage();
                    break;
                case "UnitPresent":
                    LoadUnitPresentTemplateImage();
                    break;
                default:
                    SRMMessageBox.Show("TemplateImageForm.cs->LoadTemplateImages(): There is no such vision module name " + m_smVisionInfo.g_strVisionName + " in this SRMVision software version.");
                    break;
            }
        }
        private void LoadBarcodeTemplateImage()
        {
            string strFolderName = "";
            string strFileName = "";


            strFolderName = "Barcode";
            strFileName = "Template";


            string strFolderPath = m_strRecipePath + m_strRecipeID +
                "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + strFolderName + "\\Template\\";



            if (File.Exists(strFolderPath + strFileName + 0.ToString() + ".bmp"))
                AddExistImageTabPage(strFolderName + " " + (0 + 1), strFolderPath + strFileName + 0.ToString() + ".bmp");


            string strCalFolderPath = m_strRecipePath + m_strRecipeID +
    "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            // Calibration Image
            if (File.Exists(strCalFolderPath + "CalibrationImage.bmp"))
                AddExistImageTabPage("Calibration", strCalFolderPath + "CalibrationImage.bmp");

        }
        private void LoadMOPkgTemplateImage()
        {
            string strFolderName = "";
            string strFileName = "";
            string strFileName2 = "";
            for (int x = 0; x < 4; x++) // 4 type of module: Mark, Orient, Package, Position
            {
                switch (x)
                {
                    case 0:
                        strFolderName = "Orient";
                        strFileName = "Template";
                        strFileName2 = "SubTemplate";
                        break;
                    case 1:
                        strFolderName = "Mark";
                        strFileName = "Template" + m_smVisionInfo.g_intSelectedGroup + "_";
                        break;
                    case 2:
                        strFolderName = "Package";
                        strFileName = "Template";
                        break;
                    case 3:
                        strFolderName = "Positioning";
                        strFileName = "Template";
                        break;
                }

                string strFolderPath = m_strRecipePath + m_strRecipeID +
                    "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + strFolderName + "\\Template\\";

                for (int i = 0; i < m_smVisionInfo.g_intTotalTemplates; i++)
                {

                    if (x != 1)
                    {
                        if (File.Exists(strFolderPath + strFileName + i.ToString() + ".bmp"))
                        {
                            if (m_smVisionInfo.g_strVisionName.Contains("BottomPosition"))
                                AddExistImageTabPage("Position" + " " + (i + 1), strFolderPath + strFileName + i.ToString() + ".bmp");
                            else
                                AddExistImageTabPage(strFolderName + " " + (i + 1), strFolderPath + strFileName + i.ToString() + ".bmp");
                        }

                        if (File.Exists(strFolderPath + strFileName2 + i.ToString() + ".bmp") && !m_smVisionInfo.g_strVisionName.Contains("BottomPosition"))
                            AddExistImageTabPage(strFolderName + " " + (i + 1) + " Orient Pattern", strFolderPath + strFileName2 + i.ToString() + ".bmp");
                    }
                    else
                    {
                        //string[] strImageFiles = Directory.GetFiles(strFolderPath, "*_" + i.ToString() + "_Char?.bmp");   // 2020 11 13 - CCENG : ? take care for 1 digit only
                        //string[] strImageFiles = Directory.GetFiles(strFolderPath, "*_" + i.ToString() + "_Char*.bmp");
                        var strtemp = Directory.GetFiles(strFolderPath, "*_" + i.ToString() + "_Char*.bmp");
                        int counter = 0;
                        int counter2 = 0;
                        string[] strImageFiles = new string[strtemp.Length];
                        string[] tempHold = new string[strtemp.Length];

                        foreach (string str in strtemp)
                        {
                            int Compare = 10;
                            int index = str.IndexOf("Char");
                            string temp = str.Substring(index);
                            string[] temp2 = temp.Split('.');
                            string digit = "";

                            foreach (char a in temp2[0])
                            {
                                if (char.IsDigit(a))
                                    digit = string.Concat(digit, a);
                            }

                            if (counter == 0)
                            {
                                strImageFiles[counter] = str;
                                counter++;
                            }
                            else
                            {
                                if (Compare <= Int32.Parse(digit))
                                {
                                    tempHold[counter2] = str;
                                    counter2++;
                                }
                                else
                                {
                                    strImageFiles[counter] = str;
                                    counter++;
                                }
                            }
                        }

                        for (int k = 0; k < tempHold.Length; k++)
                        {
                            if (tempHold[k] != null)
                                strImageFiles[k + 10] = tempHold[k];
                            else
                                break;
                        }
                        if (strImageFiles.Length > 0)
                            AddMultipleExistImageTabPage(strFolderName + " " + (i + 1), strImageFiles);
                    }
                }
            }

            string strCalFolderPath = m_strRecipePath + m_strRecipeID +
    "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            // Calibration Image
            if (File.Exists(strCalFolderPath + "CalibrationImage.bmp"))
                AddExistImageTabPage("Calibration", strCalFolderPath + "CalibrationImage.bmp");

        }
        private void LoadPadOrientTemplateImage()
        {
            string strFolderName = "";
            string strFileName = "";
            string strFileName2 = "";


            strFolderName = "Orient";
            strFileName = "Template";
            strFileName2 = "SubTemplate";

            string strFolderPath = m_strRecipePath + m_strRecipeID +
                "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + strFolderName + "\\Template\\";

            if (File.Exists(strFolderPath + strFileName + "0.bmp"))
                AddExistImageTabPage(strFolderName + " " + (0 + 1), strFolderPath + strFileName + "0.bmp");

            if (File.Exists(strFolderPath + strFileName2 + "0.bmp"))
                AddExistImageTabPage(strFolderName + " " + (0 + 1) + " Orient Pattern", strFolderPath + strFileName2 + "0.bmp");

        }
        private void LoadPositionOrientTemplateImage()
        {
            string strCalFolderPath = m_strRecipePath + m_strRecipeID +
    "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            // Calibration Image
            if (File.Exists(strCalFolderPath + "CalibrationImage.bmp"))
                AddExistImageTabPage("Calibration", strCalFolderPath + "CalibrationImage.bmp");
        }
        
        private void LoadLead3DTemplateImage()
        {
            string strFolderName = "Lead3D";
            string strFileName = "UnitTemplate";

            string strFolderPath = m_strRecipePath + m_strRecipeID +
                "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + strFolderName + "\\Template\\";

            if (File.Exists(strFolderPath + strFileName + "0.bmp"))
                AddExistImageTabPage("Center Lead", strFolderPath + strFileName + "0.bmp");

            if (m_smVisionInfo.g_arrLead3D.Length > 1)
            {
                if (File.Exists(strFolderPath + strFileName + "1.bmp"))
                    AddExistImageTabPage("Top Lead", strFolderPath + strFileName + "1.bmp");

                if (File.Exists(strFolderPath + strFileName + "2.bmp"))
                    AddExistImageTabPage("Right Lead", strFolderPath + strFileName + "2.bmp");

                if (File.Exists(strFolderPath + strFileName + "3.bmp"))
                    AddExistImageTabPage("Bottom Lead", strFolderPath + strFileName + "3.bmp");

                if (File.Exists(strFolderPath + strFileName + "4.bmp"))
                    AddExistImageTabPage("Left Lead", strFolderPath + strFileName + "4.bmp");
            }

            // 2020 03 23 - JBTAN: show pad pin 1 template image
            if (m_smVisionInfo.g_arrPin1 != null)
            {
                string strPin1FolderPath = m_strRecipePath + m_strRecipeID +
                    "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + strFolderName + "\\Template\\";
                // Pin 1 Image
                if (File.Exists(strPin1FolderPath + "Pin1Template0.bmp"))
                    AddExistImageTabPage("Pin1", strPin1FolderPath + "Pin1Template0.bmp");
            }

            string strCalFolderPath = m_strRecipePath + m_strRecipeID +
                "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            // Calibration Image
            //if (File.Exists(strCalFolderPath + "CalibrationImage.bmp"))
            //    AddExistImageTabPage("Calibration", strCalFolderPath + "CalibrationImage.bmp");
            if (File.Exists(strCalFolderPath + "CalibrationImageHorizontal.bmp"))
                AddExistImageTabPage("Calibration Horizontal", strCalFolderPath + "CalibrationImageHorizontal.bmp");
            if (File.Exists(strCalFolderPath + "CalibrationImageVertical.bmp"))
                AddExistImageTabPage("Calibration Vertical", strCalFolderPath + "CalibrationImageVertical.bmp");
        }
        private void LoadLeadTemplateImage()
        {
            string strFolderName = "Lead";
            string strFileName = "UnitTemplate";

            string strFolderPath = m_strRecipePath + m_strRecipeID +
                "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + strFolderName + "\\Template\\";

            if (File.Exists(strFolderPath + strFileName + "1.bmp") && m_smVisionInfo.g_arrLead[1].ref_blnSelected)
                AddExistImageTabPage("Top Lead", strFolderPath + strFileName + "1.bmp");

            if (File.Exists(strFolderPath + strFileName + "2.bmp") && m_smVisionInfo.g_arrLead[2].ref_blnSelected)
                AddExistImageTabPage("Right Lead", strFolderPath + strFileName + "2.bmp");

            if (File.Exists(strFolderPath + strFileName + "3.bmp") && m_smVisionInfo.g_arrLead[3].ref_blnSelected)
                AddExistImageTabPage("Bottom Lead", strFolderPath + strFileName + "3.bmp");

            if (File.Exists(strFolderPath + strFileName + "4.bmp") && m_smVisionInfo.g_arrLead[4].ref_blnSelected)
                AddExistImageTabPage("Left Lead", strFolderPath + strFileName + "4.bmp");

        }
        private void LoadPadTemplateImage()
        {
            string strFolderName = "Pad";
            string strFileName = "UnitToleranceTemplate"; // 2021-07-12 ZJYEOH : Change UnitTemplate to UnitToleranceTemplate, so that load image with pad extend out package edge

            string strFolderPath = m_strRecipePath + m_strRecipeID +
                "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + strFolderName + "\\Template\\";

            if (File.Exists(strFolderPath + strFileName + "0.bmp"))
                AddExistImageTabPage("Center Pad", strFolderPath + strFileName + "0.bmp");

            if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
            {
                if (File.Exists(strFolderPath + strFileName + "1.bmp"))
                    AddExistImageTabPage("Top Pad", strFolderPath + strFileName + "1.bmp");

                if (File.Exists(strFolderPath + strFileName + "2.bmp"))
                    AddExistImageTabPage("Right Pad", strFolderPath + strFileName + "2.bmp");

                if (File.Exists(strFolderPath + strFileName + "3.bmp"))
                    AddExistImageTabPage("Bottom Pad", strFolderPath + strFileName + "3.bmp");

                if (File.Exists(strFolderPath + strFileName + "4.bmp"))
                    AddExistImageTabPage("Left Pad", strFolderPath + strFileName + "4.bmp");
            }

            // 2020 03 23 - JBTAN: show pad pin 1 template image
            if (m_smVisionInfo.g_arrPin1 != null)
            {
                string strPin1FolderPath = m_strRecipePath + m_strRecipeID +
                    "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + strFolderName + "\\Template\\";
                // Pin 1 Image
                if (File.Exists(strPin1FolderPath + "Pin1Template0.bmp"))
                    AddExistImageTabPage("Pin1", strPin1FolderPath + "Pin1Template0.bmp");
            }

            string strCalFolderPath = m_strRecipePath + m_strRecipeID +
                "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            // Calibration Image
            if (File.Exists(strCalFolderPath + "CalibrationImage.bmp"))
                AddExistImageTabPage("Calibration", strCalFolderPath + "CalibrationImage.bmp");
        }

        private void LoadInPocketTemplateImage()
        {
            string strFolderName = "";
            string strFileName = "";
            string strFileName2 = "";
            for (int x = 0; x < 4; x++) // 4 type of module: Mark, Orient, Package, Position
            {
                switch (x)
                {
                    case 0:
                        strFolderName = "Orient";
                        strFileName = "Template";
                        strFileName2 = "SubTemplate";
                        break;
                    case 1:
                        strFolderName = "Mark";
                        strFileName = "Template" + m_smVisionInfo.g_intSelectedGroup + "_";
                        break;
                    case 2:
                        strFolderName = "Package";
                        strFileName = "Template";
                        break;
                    case 3:
                        strFolderName = "Positioning";
                        strFileName = "Template";
                        break;
                }

                string strFolderPath = m_strRecipePath + m_strRecipeID +
                    "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + strFolderName + "\\Template\\";

                for (int i = 0; i < m_smVisionInfo.g_intTotalTemplates; i++)
                {

                    if (x != 1)
                    {
                        if (File.Exists(strFolderPath + strFileName + i.ToString() + ".bmp"))
                            AddExistImageTabPage(strFolderName + " " + (i + 1), strFolderPath + strFileName + i.ToString() + ".bmp");

                        if (File.Exists(strFolderPath + strFileName2 + i.ToString() + ".bmp"))
                            AddExistImageTabPage(strFolderName + " " + (i + 1) + " Orient Pattern", strFolderPath + strFileName2 + i.ToString() + ".bmp");
                    }
                    else
                    {
                        //string[] strImageFiles = Directory.GetFiles(strFolderPath, "*_" + i.ToString() + "_Char?.bmp"); // 2020 11 13 - CCENG : ? take care for 1 digit only
                        string[] strImageFiles = Directory.GetFiles(strFolderPath, "*_" + i.ToString() + "_Char*.bmp");
                        if (strImageFiles.Length > 0)
                            AddMultipleExistImageTabPage(strFolderName + " " + (i + 1), strImageFiles);
                    }
                }
            }

            string strCalFolderPath = m_strRecipePath + m_strRecipeID +
    "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            // Calibration Image
            if (File.Exists(strCalFolderPath + "CalibrationImage.bmp"))
                AddExistImageTabPage("Calibration", strCalFolderPath + "CalibrationImage.bmp");

        }

        private void LoadSealTemplateImage()
        {
            string strFolderName = "";
            string strFileName = "";
            string strFolderPath = "";
            for (int x = 0; x < 2; x++) // Pocket and mark
            {
                switch (x)
                {
                    case 0:
                        strFolderName = "Pocket";
                        strFileName = "PocketTemplate0_";
                        strFolderPath = m_strRecipePath + m_strRecipeID +
                                        "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Seal\\Template\\" + strFolderName + "\\";

                        for (int i = 0; i < m_smVisionInfo.g_intPocketTemplateTotal; i++)
                        {
                            if (File.Exists(strFolderPath + strFileName + i.ToString() + ".bmp"))
                                AddExistImageTabPage(strFolderName + " " + (i + 1), strFolderPath + strFileName + i.ToString() + ".bmp");
                        }
                        break;
                    case 1:
                        strFolderName = "Mark";
                        strFileName = "MarkTemplate0_";
                        strFolderPath = m_strRecipePath + m_strRecipeID +
                                        "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Seal\\Template\\" + strFolderName + "\\";

                        for (int i = 0; i < m_smVisionInfo.g_intMarkTemplateTotal; i++)
                        {
                            if (File.Exists(strFolderPath + strFileName + i.ToString() + ".bmp"))
                                AddExistImageTabPage(strFolderName + " " + (i + 1), strFolderPath + strFileName + i.ToString() + ".bmp");
                        }
                        break;
                }
            }
        }

        private void LoadUnitPresentTemplateImage()
        {
            string strPath = m_strRecipePath + m_strRecipeID + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\CheckPresent\\Template\\Template0.bmp";

            // Load Search ROI image
            if (File.Exists(strPath))
                AddExistImageTabPage("SearchROI", strPath);

        }

        private void AddExistImageTabPage(string strHeaderTabPageName, string strTemplateImagePath)
        {
            TabPage objTabPage = new TabPage();
            objTabPage.Size = new Size(509, 234);

            PictureBox pic_Template = new PictureBox();
            pic_Template.BackColor = Color.Black;
            pic_Template.Location = new Point(0, 0);
            pic_Template.Size = new Size(512, 480);

            objTabPage.Controls.Add(pic_Template);
            tabCtrl_Templates.Controls.Add(objTabPage);
            objTabPage.Text = strHeaderTabPageName;
            try
            {
                pic_Template.Load(strTemplateImagePath);
                if (pic_Template.Image.Size.Width > pic_Template.Size.Width ||
                    pic_Template.Image.Size.Height > pic_Template.Size.Height)
                    pic_Template.SizeMode = PictureBoxSizeMode.Zoom;
                else
                    pic_Template.SizeMode = PictureBoxSizeMode.Normal;
            }
            catch
            {

            }
        }

        private void AddMultipleExistImageTabPage(string strHeaderTabPageName, string[] strTemplateImagePath)
        {
            TabPage objTabPage = new TabPage();
            objTabPage.Size = new Size(509, 234);

            Panel pnl = new Panel();
            pnl.BackColor = Color.Black;
            pnl.Dock = DockStyle.Fill;
            pnl.AutoScroll = true;
            objTabPage.Controls.Add(pnl);

            //List<int> arrImageWidth = new List<int>();
            //List<int> arrImageHeight = new List<int>();
            //for (int i = 0; i < strTemplateImagePath.Length; i++)
            //{
            //    Image Image = Image.FromFile(strTemplateImagePath[i]);
            //    arrImageWidth.Add(Image.Width);
            //    arrImageHeight.Add(Image.Height);
            //    Image.Dispose();
            //    Image = null;
            //}

            Label[] lbl_No = new Label[strTemplateImagePath.Length];
            PictureBox[] pic_Template = new PictureBox[strTemplateImagePath.Length];

            lbl_No[0] = new Label();
            lbl_No[0].ForeColor = Color.Lime;
            lbl_No[0].BackColor = Color.Black;
            lbl_No[0].Font = new Font(lbl_No[0].Font.FontFamily, 16);
            lbl_No[0].Text = "1";
            lbl_No[0].Width = lbl_No[0].Height * lbl_No[0].Text.Length;
            lbl_No[0].Location = new Point(0, 0);
            pnl.Controls.Add(lbl_No[0]);

            pic_Template[0] = new PictureBox();
            pic_Template[0].BackColor = Color.Black;
            pic_Template[0].Location = new Point(25, 0);
            pnl.Controls.Add(pic_Template[0]);
            pic_Template[0].Load(strTemplateImagePath[0]);
            pic_Template[0].SizeMode = PictureBoxSizeMode.Normal;
            pic_Template[0].Size = new Size(pic_Template[0].Image.Width, pic_Template[0].Image.Height);

            for (int i = 1; i < strTemplateImagePath.Length; i++)
            {
                lbl_No[i] = new Label();
                lbl_No[i].ForeColor = Color.Lime;
                lbl_No[i].BackColor = Color.Black;
                lbl_No[i].Font = new Font(lbl_No[0].Font.FontFamily, 16);
                lbl_No[i].Text = (i + 1).ToString();
                lbl_No[i].Width = lbl_No[i].Height * lbl_No[i].Text.Length;

                if (pic_Template[i - 1].Image.Height > lbl_No[i].Height)
                    lbl_No[i].Location = new Point(0, pic_Template[i - 1].Location.Y + pic_Template[i - 1].Image.Height + 5);
                else
                    lbl_No[i].Location = new Point(0, pic_Template[i - 1].Location.Y + lbl_No[i].Height + 5);

                pnl.Controls.Add(lbl_No[i]);

                pic_Template[i] = new PictureBox();
                pic_Template[i].BackColor = Color.Black;
                pic_Template[i].Location = new Point(25 * lbl_No[i].Text.Length, lbl_No[i].Location.Y);
                pic_Template[i].Load(strTemplateImagePath[i]);
                pic_Template[i].SizeMode = PictureBoxSizeMode.Normal;
                pic_Template[i].Size = new Size(pic_Template[i].Image.Width, pic_Template[i].Image.Height);
                pnl.Controls.Add(pic_Template[i]);
            }

            tabCtrl_Templates.Controls.Add(objTabPage);
            objTabPage.Text = strHeaderTabPageName;
        }

        private void AddNoImageTabPage(string strHeaderTabPageName)
        {
            TabPage objTabPage = new TabPage();
            objTabPage.Size = new Size(509, 234);
            objTabPage.BackColor = Color.Black;

            PictureBox pic_Template = new PictureBox();
            pic_Template.BackColor = Color.Black;
            pic_Template.Location = new Point(0, 0);
            pic_Template.Size = new Size(512, 480);

            SRMControl.SRMLabel lbl_Message = new SRMControl.SRMLabel();
            lbl_Message.BackColor = Color.Black;
            lbl_Message.ForeColor = Color.White;
            lbl_Message.Location = new Point(199, 209);
            lbl_Message.Text = "No Template!";

            objTabPage.Controls.Add(lbl_Message);
            objTabPage.Controls.Add(pic_Template);
            tabCtrl_Templates.Controls.Add(objTabPage);
            objTabPage.Text = strHeaderTabPageName;
        }

        private void TemplateImageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();
        }
    }
}