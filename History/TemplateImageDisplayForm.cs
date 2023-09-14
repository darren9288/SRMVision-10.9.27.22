using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Common;
namespace History
{
    public partial class TemplateImageDisplayForm : Form
    {

        private string m_strOldImagePath, m_strNewImagePath;
        private string m_ImageNameOld, m_ImageNameNew;
        private string m_Description;
        private float m_ImageHeightOld, m_ImageHeightNew, m_ImageWidthOld, m_ImageWidthNew;
        public TemplateImageDisplayForm(DateTime LogDateTime, string Description, string ImageNameOld, string ImageNameNew, string strHistoryDataLocation)
        {
            InitializeComponent();
            string DT = LogDateTime.ToString().Replace(':', '.');
            string strImageFilePath = strHistoryDataLocation + "Data\\" + LogDateTime.ToString("yyyy-MM") + "\\" + DT + "\\" + Description.Replace('>', '-') + "\\";
            strImageFilePath = strImageFilePath.Replace('>', '-');
            m_ImageNameOld = ImageNameOld;
            m_ImageNameNew = ImageNameNew;
            m_Description = Description.Replace('>', '-');
            m_strOldImagePath = strImageFilePath + "Old\\";
            if (File.Exists(m_strOldImagePath + ImageNameOld))
            {
                m_ImageHeightOld = Image.FromFile(m_strOldImagePath + ImageNameOld).Height;
                m_ImageWidthOld = Image.FromFile(m_strOldImagePath + ImageNameOld).Width;
                pic_ImageOld.Image = Image.FromFile(m_strOldImagePath + ImageNameOld);
            }

            m_strNewImagePath = strImageFilePath + "New\\";
            if (File.Exists(m_strNewImagePath + ImageNameNew))
            {
                m_ImageHeightNew = Image.FromFile(m_strNewImagePath + ImageNameNew).Height;
                m_ImageWidthNew = Image.FromFile(m_strNewImagePath + ImageNameNew).Width;
                pic_ImageNew.Image = Image.FromFile(m_strNewImagePath + ImageNameNew);
            }

            if (Description.Contains("Mark") || Description.Contains("Seal Pocket"))
            {
                UpdateMarkGUI();
            }
            else //if (Description.Contains("Package"))
            {
                UpdateGUI_Only1Template();
            }

            pic_ImageOld.Size = new Size((int)(480 * (360.0 / 480) / (m_ImageHeightOld / m_ImageWidthOld)), 360);
            pnl_PictureBoxOld.Size = new Size(480, 360);
            pic_ImageNew.Size = new Size((int)(480 * (360.0 / 480) / (m_ImageHeightNew / m_ImageWidthNew)), 360);
            pnl_PictureBoxNew.Size = new Size(480, 360);
        }

        private void UpdateMarkGUI()
        {
            DirectoryInfo dirNew = new DirectoryInfo(m_strNewImagePath);
            if (!Directory.Exists(m_strNewImagePath))
            {
                SRMMessageBox.Show("Image path no exist! Path = " + m_strNewImagePath, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                STTrackLog.WriteLine("Image path no exist! Path = " + m_strNewImagePath);
                return;
            }

            FileInfo[] FNewInfo = dirNew.GetFiles();


            foreach (FileInfo ImageFile in FNewInfo)
            {
                if (!cbo_ViewImage.Items.Contains("Image 1"))
                    cbo_ViewImage.Items.Add("Image 1");

                if (ImageFile.Name.Contains("_Image1"))
                {
                    if (!cbo_ViewImage.Items.Contains("Image 2"))
                        cbo_ViewImage.Items.Add("Image 2");
                }

                if (ImageFile.Name.Contains("_Image2"))
                {
                    if (!cbo_ViewImage.Items.Contains("Image 3"))
                        cbo_ViewImage.Items.Add("Image 3");
                }

                if (ImageFile.Name.Contains("_Image3"))
                {
                    if (!cbo_ViewImage.Items.Contains("Image 4"))
                        cbo_ViewImage.Items.Add("Image 4");
                }

                if (ImageFile.Name.Contains("_Image4"))
                {
                    if (!cbo_ViewImage.Items.Contains("Image 5"))
                        cbo_ViewImage.Items.Add("Image 5");
                }

                char[] CharSplit = ImageFile.Name.ToCharArray();
                int intTemplateIndex = ImageFile.Name.IndexOf('_') + 1;
                if (CharSplit[intTemplateIndex] == '0')
                {
                    if (!cbo_TemplateNo.Items.Contains("1"))
                        cbo_TemplateNo.Items.Add("1");
                }

                if (CharSplit[intTemplateIndex] == '1')
                {
                    if (!cbo_TemplateNo.Items.Contains("2"))
                        cbo_TemplateNo.Items.Add("2");
                }

                if (CharSplit[intTemplateIndex] == '2')
                {
                    if (!cbo_TemplateNo.Items.Contains("3"))
                        cbo_TemplateNo.Items.Add("3");
                }
                if (CharSplit[intTemplateIndex] == '3')
                {
                    if (!cbo_TemplateNo.Items.Contains("4"))
                        cbo_TemplateNo.Items.Add("4");
                }
                if (CharSplit[intTemplateIndex] == '4')
                {
                    if (!cbo_TemplateNo.Items.Contains("5"))
                        cbo_TemplateNo.Items.Add("5");
                }

                if (CharSplit[intTemplateIndex] == '5')
                {
                    if (!cbo_TemplateNo.Items.Contains("6"))
                        cbo_TemplateNo.Items.Add("6");
                }

                if (CharSplit[intTemplateIndex] == '6')
                {
                    if (!cbo_TemplateNo.Items.Contains("7"))
                        cbo_TemplateNo.Items.Add("7");
                }
                if (CharSplit[intTemplateIndex] == '7')
                {
                    if (!cbo_TemplateNo.Items.Contains("8"))
                        cbo_TemplateNo.Items.Add("8");
                }

            }

            DirectoryInfo dirOld = new DirectoryInfo(m_strOldImagePath);
            FileInfo[] FOldInfo = dirOld.GetFiles();


            foreach (FileInfo ImageFile in FOldInfo)
            {
                if (!cbo_ViewImage.Items.Contains("Image 1"))
                    cbo_ViewImage.Items.Add("Image 1");

                if (ImageFile.Name.Contains("_Image1"))
                {
                    if (!cbo_ViewImage.Items.Contains("Image 2"))
                        cbo_ViewImage.Items.Add("Image 2");
                }

                if (ImageFile.Name.Contains("_Image2"))
                {
                    if (!cbo_ViewImage.Items.Contains("Image 3"))
                        cbo_ViewImage.Items.Add("Image 3");
                }

                if (ImageFile.Name.Contains("_Image3"))
                {
                    if (!cbo_ViewImage.Items.Contains("Image 4"))
                        cbo_ViewImage.Items.Add("Image 4");
                }

                if (ImageFile.Name.Contains("_Image4"))
                {
                    if (!cbo_ViewImage.Items.Contains("Image 5"))
                        cbo_ViewImage.Items.Add("Image 5");
                }

                char[] CharSplit = ImageFile.Name.ToCharArray();
                int intTemplateIndex = ImageFile.Name.IndexOf('_') + 1;

                if (CharSplit[intTemplateIndex] == '0')
                {
                    if (!cbo_TemplateNo.Items.Contains("1"))
                        cbo_TemplateNo.Items.Add("1");
                }

                if (CharSplit[intTemplateIndex] == '1')
                {
                    if (!cbo_TemplateNo.Items.Contains("2"))
                        cbo_TemplateNo.Items.Add("2");
                }

                if (CharSplit[intTemplateIndex] == '2')
                {
                    if (!cbo_TemplateNo.Items.Contains("3"))
                        cbo_TemplateNo.Items.Add("3");
                }
                if (CharSplit[intTemplateIndex] == '3')
                {
                    if (!cbo_TemplateNo.Items.Contains("4"))
                        cbo_TemplateNo.Items.Add("4");
                }
                if (CharSplit[intTemplateIndex] == '4')
                {
                    if (!cbo_TemplateNo.Items.Contains("5"))
                        cbo_TemplateNo.Items.Add("5");
                }

                if (CharSplit[intTemplateIndex] == '5')
                {
                    if (!cbo_TemplateNo.Items.Contains("6"))
                        cbo_TemplateNo.Items.Add("6");
                }

                if (CharSplit[intTemplateIndex] == '6')
                {
                    if (!cbo_TemplateNo.Items.Contains("7"))
                        cbo_TemplateNo.Items.Add("7");
                }
                if (CharSplit[intTemplateIndex] == '7')
                {
                    if (!cbo_TemplateNo.Items.Contains("8"))
                        cbo_TemplateNo.Items.Add("8");
                }

            }

            if (cbo_ViewImage.Items.Contains("Image 1"))
                cbo_ViewImage.SelectedItem = "Image 1";

            char[] splitTemplate = m_ImageNameNew.ToCharArray();
            int intsplitTemplateIndex = m_ImageNameNew.IndexOf('_') + 1;

            string a = (Convert.ToInt32(splitTemplate[intsplitTemplateIndex].ToString()) + 1).ToString();
            if (cbo_TemplateNo.Items.Contains((Convert.ToInt32(splitTemplate[intsplitTemplateIndex].ToString()) + 1).ToString()))
                cbo_TemplateNo.SelectedItem = (Convert.ToInt32(splitTemplate[intsplitTemplateIndex].ToString()) + 1).ToString();
        }
        private void UpdateGUI_Only1Template()
        {
            DirectoryInfo dirNew = new DirectoryInfo(m_strNewImagePath);
            FileInfo[] FNewInfo = dirNew.GetFiles();
            
            foreach (FileInfo ImageFile in FNewInfo)
            {
                if (!cbo_ViewImage.Items.Contains("Image 1"))
                    cbo_ViewImage.Items.Add("Image 1");

                if (ImageFile.Name.Contains("_Image1"))
                {
                    if (!cbo_ViewImage.Items.Contains("Image 2"))
                        cbo_ViewImage.Items.Add("Image 2");
                }

                if (ImageFile.Name.Contains("_Image2"))
                {
                    if (!cbo_ViewImage.Items.Contains("Image 3"))
                        cbo_ViewImage.Items.Add("Image 3");
                }

                if (ImageFile.Name.Contains("_Image3"))
                {
                    if (!cbo_ViewImage.Items.Contains("Image 4"))
                        cbo_ViewImage.Items.Add("Image 4");
                }

                if (ImageFile.Name.Contains("_Image4"))
                {
                    if (!cbo_ViewImage.Items.Contains("Image 5"))
                        cbo_ViewImage.Items.Add("Image 5");
                }
            }

            DirectoryInfo dirOld = new DirectoryInfo(m_strOldImagePath);
            FileInfo[] FOldInfo = dirOld.GetFiles();
            
            foreach (FileInfo ImageFile in FOldInfo)
            {
                if (!cbo_ViewImage.Items.Contains("Image 1"))
                    cbo_ViewImage.Items.Add("Image 1");

                if (ImageFile.Name.Contains("_Image1"))
                {
                    if (!cbo_ViewImage.Items.Contains("Image 2"))
                        cbo_ViewImage.Items.Add("Image 2");
                }

                if (ImageFile.Name.Contains("_Image2"))
                {
                    if (!cbo_ViewImage.Items.Contains("Image 3"))
                        cbo_ViewImage.Items.Add("Image 3");
                }

                if (ImageFile.Name.Contains("_Image3"))
                {
                    if (!cbo_ViewImage.Items.Contains("Image 4"))
                        cbo_ViewImage.Items.Add("Image 4");
                }

                if (ImageFile.Name.Contains("_Image4"))
                {
                    if (!cbo_ViewImage.Items.Contains("Image 5"))
                        cbo_ViewImage.Items.Add("Image 5");
                }
            }

            if (cbo_ViewImage.Items.Contains("Image 1"))
                cbo_ViewImage.SelectedItem = "Image 1";

            cbo_TemplateNo.Visible = false;
            lbl_Template.Visible = false;
        }
        private void cbo_TemplateNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ImageNameSub = m_ImageNameNew.Substring(0, m_ImageNameNew.IndexOf('_') + 1);

            if (cbo_ViewImage.SelectedItem.ToString() == "Image 1")
            {
                string ImageName = ImageNameSub + (Convert.ToInt32(cbo_TemplateNo.SelectedItem) - 1).ToString() + ".bmp";
                if (File.Exists(m_strOldImagePath + ImageName))
                    pic_ImageOld.Image = Image.FromFile(m_strOldImagePath + ImageName);
                else
                {
                    pic_ImageOld.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (File.Exists(m_strNewImagePath + ImageName))
                    pic_ImageNew.Image = Image.FromFile(m_strNewImagePath + ImageName);
                else
                {
                    pic_ImageNew.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }
            }
            if (cbo_ViewImage.SelectedItem.ToString() == "Image 2")
            {
                string ImageName = ImageNameSub + (Convert.ToInt32(cbo_TemplateNo.SelectedItem) - 1).ToString() + "_Image1" + ".bmp";
                if (File.Exists(m_strOldImagePath + ImageName))
                    pic_ImageOld.Image = Image.FromFile(m_strOldImagePath + ImageName);
                else
                {
                   pic_ImageOld.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory+"Misc\\Black.bmp" );
                }

                if (File.Exists(m_strNewImagePath + ImageName))
                    pic_ImageNew.Image = Image.FromFile(m_strNewImagePath + ImageName);
                else
                {
                    pic_ImageNew.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }
            }
            if (cbo_ViewImage.SelectedItem.ToString() == "Image 3")
            {
                string ImageName = ImageNameSub + (Convert.ToInt32(cbo_TemplateNo.SelectedItem) - 1).ToString() + "_Image2" + ".bmp";
                if (File.Exists(m_strOldImagePath + ImageName))
                    pic_ImageOld.Image = Image.FromFile(m_strOldImagePath + ImageName);
                else
                {
                    pic_ImageOld.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (File.Exists(m_strNewImagePath + ImageName))
                    pic_ImageNew.Image = Image.FromFile(m_strNewImagePath + ImageName);
                else
                {
                    pic_ImageNew.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }
            }

            if (cbo_ViewImage.SelectedItem.ToString() == "Image 4")
            {
                string ImageName = ImageNameSub + (Convert.ToInt32(cbo_TemplateNo.SelectedItem) - 1).ToString() + "_Image3" + ".bmp";
                if (File.Exists(m_strOldImagePath + ImageName))
                    pic_ImageOld.Image = Image.FromFile(m_strOldImagePath + ImageName);
                else
                {
                    pic_ImageOld.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (File.Exists(m_strNewImagePath + ImageName))
                    pic_ImageNew.Image = Image.FromFile(m_strNewImagePath + ImageName);
                else
                {
                    pic_ImageNew.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }
            }

            if (cbo_ViewImage.SelectedItem.ToString() == "Image 5")
            {
                string ImageName = ImageNameSub + (Convert.ToInt32(cbo_TemplateNo.SelectedItem) - 1).ToString() + "_Image4" + ".bmp";
                if (File.Exists(m_strOldImagePath + ImageName))
                    pic_ImageOld.Image = Image.FromFile(m_strOldImagePath + ImageName);
                else
                {
                    pic_ImageOld.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (File.Exists(m_strNewImagePath + ImageName))
                    pic_ImageNew.Image = Image.FromFile(m_strNewImagePath + ImageName);
                else
                {
                    pic_ImageNew.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }
            }
        }

        private void cbo_ViewImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbo_ViewImage.SelectedItem.ToString() == "Image 1")
            {
                string ImageName;
                if (m_Description.Contains("Package") || m_Description.Contains("Orient") || m_Description.Contains("Seal") || m_Description.Contains("Barcode"))
                    ImageName = "OriTemplate0.bmp";
                else if (m_Description.Contains("Pad") || m_Description.Contains("Lead") || m_Description.Contains("Lead3D") || m_Description.Contains("PadPkg"))
                    ImageName = "OriTemplate.bmp";
                else if (m_Description.Contains("PH"))
                    ImageName = "PHOriTemplate.bmp";
                else if (m_Description.Contains("Empty"))
                    ImageName = "EmptyOriTemplate.bmp";
                else
                    ImageName = "OriTemplate0_" + (Convert.ToInt32(cbo_TemplateNo.SelectedItem) - 1).ToString() + ".bmp";
                if (m_Description.Contains("Seal") && (m_Description.Contains("Mark") || m_Description.Contains("Pocket")))
                    ImageName = m_ImageNameNew.Substring(0, m_ImageNameNew.IndexOf('_') + 1) + (Convert.ToInt32(cbo_TemplateNo.SelectedItem) - 1).ToString() + ".bmp";

                if (File.Exists(m_strOldImagePath + ImageName))
                    pic_ImageOld.Image = Image.FromFile(m_strOldImagePath + ImageName);
                else
                {
                    pic_ImageOld.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (File.Exists(m_strNewImagePath + ImageName))
                    pic_ImageNew.Image = Image.FromFile(m_strNewImagePath + ImageName);
                else
                {
                    pic_ImageNew.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }
            }
            if (cbo_ViewImage.SelectedItem.ToString() == "Image 2")
            {
                string ImageName;
                if (m_Description.Contains("Package") || m_Description.Contains("Orient") || m_Description.Contains("Seal") || m_Description.Contains("Barcode"))
                    ImageName = "OriTemplate0_Image1.bmp";
                else if (m_Description.Contains("Pad") || m_Description.Contains("Lead") || m_Description.Contains("Lead3D") || m_Description.Contains("PadPkg"))
                    ImageName = "OriTemplate_Image1.bmp";
                else
                    ImageName = "OriTemplate0_" + (Convert.ToInt32(cbo_TemplateNo.SelectedItem) - 1).ToString() + "_Image1" + ".bmp";
                if (m_Description.Contains("Seal") && (m_Description.Contains("Mark") || m_Description.Contains("Pocket")))
                    ImageName = m_ImageNameNew.Substring(0, m_ImageNameNew.IndexOf('_') + 1) + (Convert.ToInt32(cbo_TemplateNo.SelectedItem) - 1).ToString() + "_Image1" + ".bmp";

                if (File.Exists(m_strOldImagePath + ImageName))
                    pic_ImageOld.Image = Image.FromFile(m_strOldImagePath + ImageName);
                else
                {
                    pic_ImageOld.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (File.Exists(m_strNewImagePath + ImageName))
                    pic_ImageNew.Image = Image.FromFile(m_strNewImagePath + ImageName);
                else
                {
                    pic_ImageNew.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }
            }
            if (cbo_ViewImage.SelectedItem.ToString() == "Image 3")
            {
                string ImageName;
                if (m_Description.Contains("Package") || m_Description.Contains("Orient") || m_Description.Contains("Seal") || m_Description.Contains("Barcode"))
                    ImageName = "OriTemplate0_Image2.bmp";
                else if (m_Description.Contains("Pad") || m_Description.Contains("Lead") || m_Description.Contains("Lead3D") || m_Description.Contains("PadPkg"))
                    ImageName = "OriTemplate_Image2.bmp";
                else
                    ImageName = "OriTemplate0_" + (Convert.ToInt32(cbo_TemplateNo.SelectedItem) - 1).ToString() + "_Image2" + ".bmp";
                if (m_Description.Contains("Seal") && (m_Description.Contains("Mark") || m_Description.Contains("Pocket")))
                    ImageName = m_ImageNameNew.Substring(0, m_ImageNameNew.IndexOf('_') + 1) + (Convert.ToInt32(cbo_TemplateNo.SelectedItem) - 1).ToString() + "_Image2" + ".bmp";

                if (File.Exists(m_strOldImagePath + ImageName))
                    pic_ImageOld.Image = Image.FromFile(m_strOldImagePath + ImageName);
                else
                {
                    pic_ImageOld.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (File.Exists(m_strNewImagePath + ImageName))
                    pic_ImageNew.Image = Image.FromFile(m_strNewImagePath + ImageName);
                else
                {
                    pic_ImageNew.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }
            }
            if (cbo_ViewImage.SelectedItem.ToString() == "Image 4")
            {
                string ImageName;
                if (m_Description.Contains("Package") || m_Description.Contains("Orient") || m_Description.Contains("Seal") || m_Description.Contains("Barcode"))
                    ImageName = "OriTemplate0_Image3.bmp";
                else if (m_Description.Contains("Pad") || m_Description.Contains("Lead") || m_Description.Contains("Lead3D") || m_Description.Contains("PadPkg"))
                    ImageName = "OriTemplate_Image3.bmp";
                else
                    ImageName = "OriTemplate0_" + (Convert.ToInt32(cbo_TemplateNo.SelectedItem) - 1).ToString() + "_Image3" + ".bmp";
                if (m_Description.Contains("Seal") && (m_Description.Contains("Mark") || m_Description.Contains("Pocket")))
                    ImageName = m_ImageNameNew.Substring(0, m_ImageNameNew.IndexOf('_') + 1) + (Convert.ToInt32(cbo_TemplateNo.SelectedItem) - 1).ToString() + "_Image3" + ".bmp";

                if (File.Exists(m_strOldImagePath + ImageName))
                    pic_ImageOld.Image = Image.FromFile(m_strOldImagePath + ImageName);
                else
                {
                    pic_ImageOld.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (File.Exists(m_strNewImagePath + ImageName))
                    pic_ImageNew.Image = Image.FromFile(m_strNewImagePath + ImageName);
                else
                {
                    pic_ImageNew.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }
            }
            if (cbo_ViewImage.SelectedItem.ToString() == "Image 5")
            {
                string ImageName;
                if (m_Description.Contains("Package") || m_Description.Contains("Orient") || m_Description.Contains("Seal") || m_Description.Contains("Barcode"))
                    ImageName = "OriTemplate0_Image4.bmp";
                else if (m_Description.Contains("Pad") || m_Description.Contains("Lead") || m_Description.Contains("Lead3D") || m_Description.Contains("PadPkg"))
                    ImageName = "OriTemplate_Image4.bmp";
                else
                    ImageName = "OriTemplate0_" + (Convert.ToInt32(cbo_TemplateNo.SelectedItem) - 1).ToString() + "_Image4" + ".bmp";
                if (m_Description.Contains("Seal") && (m_Description.Contains("Mark") || m_Description.Contains("Pocket")))
                    ImageName = m_ImageNameNew.Substring(0, m_ImageNameNew.IndexOf('_') + 1) + (Convert.ToInt32(cbo_TemplateNo.SelectedItem) - 1).ToString() + "_Image4" + ".bmp";

                if (File.Exists(m_strOldImagePath + ImageName))
                    pic_ImageOld.Image = Image.FromFile(m_strOldImagePath + ImageName);
                else
                {
                    pic_ImageOld.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (File.Exists(m_strNewImagePath + ImageName))
                    pic_ImageNew.Image = Image.FromFile(m_strNewImagePath + ImageName);
                else
                {
                    pic_ImageNew.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }
            }
        }
    }
}
