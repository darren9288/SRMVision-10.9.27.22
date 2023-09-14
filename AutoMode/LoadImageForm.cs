using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Common;
using SharedMemory;

namespace AutoMode
{
    public partial class LoadImageForm : Form
    {
        #region Member Variables

        string m_strPath;
        string[] m_strImageFiles;
        private bool m_blnInitDone = false;
        private string m_strSelectedImagePath;
        private List<string> m_arrSelectedImageList = new List<string>();
        //private bool m_blnLoadAllImage = false;
        //private int m_intLoadType = 0; //0 = all image folder, 1 = all fail image folder, 2 = only specific fail image folder eg. edgeNotFound
        private int m_intSelectedRowPrev = -1;

        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private bool m_blnSortByDescending = false;
        private bool m_blnUpdateErrorMessage = false;

        #endregion

        #region Properties

        public string ref_strSelectedImagePath { get { return m_strSelectedImagePath; } }
        public List<string> ref_arrSelectedImageList { get { return m_arrSelectedImageList; } }
        //public bool ref_blnLoadAllImage { get { return m_blnLoadAllImage; } }
        //public int ref_intLoadType { get { return m_intLoadType; } }

        #endregion

        public LoadImageForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, ProductionInfo smProductionInfo, string strPath)
        {
            InitializeComponent();
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_smProductionInfo = smProductionInfo;
            m_strPath = strPath;

            if (m_strPath != "")
            {
                if (Directory.Exists(m_strPath))
                {
                    m_strImageFiles = Directory.GetFiles(m_strPath, "*.bmp", SearchOption.AllDirectories);
                }
            }

            if (m_strImageFiles != null) // 2020-01-06 ZJYEOH : when m_strImageFiles is null, no need update GUI
            {
                UpdateGUI();
                updateImagePreview();
                CreateImageList();
                m_blnInitDone = true;
            }
        }

        private void CreateImageList()
        {
            dgd_ImageList.Rows.Clear();
            XmlParser objFile;
            for (int i = 0; i < m_strImageFiles.Length; i++)
            {
                dgd_ImageList.Rows.Add();
                dgd_ImageList.Rows[i].Cells[0].Value = Path.GetFileNameWithoutExtension(m_strImageFiles[i]);
                dgd_ImageList.Rows[i].Cells[1].Value = File.GetCreationTime(m_strImageFiles[i]).ToString("yy/MM/dd HH:mm:ss");
                dgd_ImageList.Rows[i].Cells[2].Value = m_strImageFiles[i];

                //When load image hide image 1 and image 2 first
                if (dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image2")
                    || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image4")
                    || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image6"))
                {
                    if (dgd_ImageList.Rows[i].Visible)
                        dgd_ImageList.Rows[i].Visible = false;
                }
                else
                {
                    if (File.Exists(Path.GetDirectoryName(m_strImageFiles[i]) + "\\" + dgd_ImageList.Rows[i].Cells[0].Value.ToString() + ".xml"))
                    {
                        objFile = new XmlParser(Path.GetDirectoryName(m_strImageFiles[i]) + "\\" + dgd_ImageList.Rows[i].Cells[0].Value.ToString() + ".xml");
                        objFile.GetFirstSection("ErrorMessage");
                        dgd_ImageList.Rows[i].Cells[3].Value = objFile.GetValueAsString("Message_" + dgd_ImageList.Rows[i].Cells[0].Value, "No Info.");
                    }
                    else
                    {
                        dgd_ImageList.Rows[i].Cells[3].Value = "No Info.";
                    }
                }
            }

            m_blnSortByDescending = false;
            RowComparer rc = new RowComparer(SortOrder.Ascending, 1);
            dgd_ImageList.Sort(rc);

            if (m_smVisionInfo.g_arrImages.Count >= 1 && dgd_ImageList.Rows.Count > 0)
                pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[0].Cells[2].Value.ToString());

            if (m_smVisionInfo.g_arrImages.Count >= 2 && dgd_ImageList.Rows.Count > 1)
            {
                if (dgd_ImageList.Rows[1].Cells[2].Value.ToString().Contains("Image1") || dgd_ImageList.Rows[1].Cells[2].Value.ToString().Contains("Image2") 
                    || dgd_ImageList.Rows[1].Cells[2].Value.ToString().Contains("Image3") || dgd_ImageList.Rows[1].Cells[2].Value.ToString().Contains("Image4") 
                    || dgd_ImageList.Rows[1].Cells[2].Value.ToString().Contains("Image5"))
                    pic_ImagePreview2.Image = Image.FromFile(dgd_ImageList.Rows[1].Cells[2].Value.ToString());
            }

            if (m_smVisionInfo.g_arrImages.Count >= 3 && dgd_ImageList.Rows.Count > 2)
            {
                if (dgd_ImageList.Rows[2].Cells[2].Value.ToString().Contains("Image1") || dgd_ImageList.Rows[2].Cells[2].Value.ToString().Contains("Image2")
                    || dgd_ImageList.Rows[2].Cells[2].Value.ToString().Contains("Image3") || dgd_ImageList.Rows[2].Cells[2].Value.ToString().Contains("Image4")
                    || dgd_ImageList.Rows[2].Cells[2].Value.ToString().Contains("Image5"))
                    pic_ImagePreview3.Image = Image.FromFile(dgd_ImageList.Rows[2].Cells[2].Value.ToString());
                else
                {
                    m_blnUpdateErrorMessage = true;
                    return;
                }
            }

            if (m_smVisionInfo.g_arrImages.Count >= 4 && dgd_ImageList.Rows.Count > 3)
            {
                if (dgd_ImageList.Rows[3].Cells[2].Value.ToString().Contains("Image1") || dgd_ImageList.Rows[3].Cells[2].Value.ToString().Contains("Image2")
                    || dgd_ImageList.Rows[3].Cells[2].Value.ToString().Contains("Image3") || dgd_ImageList.Rows[3].Cells[2].Value.ToString().Contains("Image4") 
                    || dgd_ImageList.Rows[3].Cells[2].Value.ToString().Contains("Image5"))
                    pic_ImagePreview4.Image = Image.FromFile(dgd_ImageList.Rows[3].Cells[2].Value.ToString());
                else
                {
                    m_blnUpdateErrorMessage = true;
                    return;
                }
            }

            if (m_smVisionInfo.g_arrImages.Count >= 5 && dgd_ImageList.Rows.Count > 4)
            {
                if (dgd_ImageList.Rows[4].Cells[2].Value.ToString().Contains("Image1") || dgd_ImageList.Rows[4].Cells[2].Value.ToString().Contains("Image2")
                    || dgd_ImageList.Rows[4].Cells[2].Value.ToString().Contains("Image3") || dgd_ImageList.Rows[4].Cells[2].Value.ToString().Contains("Image4")
                    || dgd_ImageList.Rows[4].Cells[2].Value.ToString().Contains("Image5"))
                    pic_ImagePreview5.Image = Image.FromFile(dgd_ImageList.Rows[4].Cells[2].Value.ToString());
                else
                {
                    m_blnUpdateErrorMessage = true;
                    return;
                }
            }

            if (m_smVisionInfo.g_arrImages.Count >= 6 && dgd_ImageList.Rows.Count > 5)
            {
                if (dgd_ImageList.Rows[5].Cells[2].Value.ToString().Contains("Image1") || dgd_ImageList.Rows[5].Cells[2].Value.ToString().Contains("Image2") 
                    || dgd_ImageList.Rows[5].Cells[2].Value.ToString().Contains("Image3") || dgd_ImageList.Rows[5].Cells[2].Value.ToString().Contains("Image4") 
                    || dgd_ImageList.Rows[5].Cells[2].Value.ToString().Contains("Image5"))
                    pic_ImagePreview6.Image = Image.FromFile(dgd_ImageList.Rows[5].Cells[2].Value.ToString());
                else
                {
                    m_blnUpdateErrorMessage = true;
                    return;
                }
            }
            m_blnUpdateErrorMessage = true;
        }

        public void UpdateGUI()
        {
            cbo_ImageSelection.Items.Clear();

            //Update cbo_ImageSelection
            if (!cbo_ImageSelection.Items.Contains("All"))
                cbo_ImageSelection.Items.Add("All");
            cbo_ImageSelection.SelectedIndex = 0;

            bool blnPassPresent = false;
            bool blnFailPresent = false;
            bool bln0DegreePresent = false;
            bool bln90DegreePresent = false;
            bool bln180DegreePresent = false;
            bool bln270DegreePresent = false;

            for (int i = 0; i < m_strImageFiles.Length; i++)
            {
                //if (m_strImageFiles[i].Contains("Pass"))
                blnPassPresent = true;

                //if (m_strImageFiles[i].Contains("Fail"))
                blnFailPresent = true;

                if (!(m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM")) && ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) == 0))
                {

                    if (m_strImageFiles[i].Contains("0Degree"))
                        bln0DegreePresent = true;

                    if (m_strImageFiles[i].Contains("90Degree"))
                        bln90DegreePresent = true;

                    if (m_strImageFiles[i].Contains("180Degree"))
                        bln180DegreePresent = true;

                    if (m_strImageFiles[i].Contains("270Degree"))
                        bln270DegreePresent = true;
                }
            }

            //if (blnPassPresent)
            //{
            if (!cbo_ImageSelection.Items.Contains("Pass"))
                cbo_ImageSelection.Items.Add("Pass");
            //}
            //else
            //{
            //    if (cbo_ImageSelection.Items.Contains("Pass"))
            //        cbo_ImageSelection.Items.Remove("Pass");
            //}

            //if (blnFailPresent)
            //{
            if (!cbo_ImageSelection.Items.Contains("Fail"))
                cbo_ImageSelection.Items.Add("Fail");
            //}
            //else
            //{
            //    if (cbo_ImageSelection.Items.Contains("Fail"))
            //        cbo_ImageSelection.Items.Remove("Fail");
            //}

            if (bln0DegreePresent)
            {
                if (!cbo_ImageSelection.Items.Contains("0Degree"))
                    cbo_ImageSelection.Items.Add("0Degree");
            }
            else
            {
                if (cbo_ImageSelection.Items.Contains("0Degree"))
                    cbo_ImageSelection.Items.Remove("0Degree");
            }

            if (bln90DegreePresent)
            {
                if (!cbo_ImageSelection.Items.Contains("90Degree"))
                    cbo_ImageSelection.Items.Add("90Degree");
            }
            else
            {
                if (cbo_ImageSelection.Items.Contains("90Degree"))
                    cbo_ImageSelection.Items.Remove("90Degree");
            }

            if (bln180DegreePresent)
            {
                if (!cbo_ImageSelection.Items.Contains("180Degree"))
                    cbo_ImageSelection.Items.Add("180Degree");
            }
            else
            {
                if (cbo_ImageSelection.Items.Contains("180Degree"))
                    cbo_ImageSelection.Items.Remove("180Degree");
            }

            if (bln270DegreePresent)
            {
                if (!cbo_ImageSelection.Items.Contains("270Degree"))
                    cbo_ImageSelection.Items.Add("270Degree");
            }
            else
            {
                if (cbo_ImageSelection.Items.Contains("270Degree"))
                    cbo_ImageSelection.Items.Remove("270Degree");
            }

            string[] arrFolderName = Directory.GetDirectories(m_strPath);

            for (int i = 0; i < arrFolderName.Length; i++)
            {
                string strFolderName = Path.GetFileName(arrFolderName[i]);

                if (!cbo_ImageSelection.Items.Contains(strFolderName))
                    cbo_ImageSelection.Items.Add(strFolderName);
            }

            //Update cbo_ImageNoSelection
            if (!cbo_ImageNoSelection.Items.Contains("View All"))
                cbo_ImageNoSelection.Items.Add("View All");
            if (!cbo_ImageNoSelection.Items.Contains("View Image 0"))
                cbo_ImageNoSelection.Items.Add("View Image 0");
            cbo_ImageNoSelection.SelectedIndex = 0;

            bool blnImage1Present = false;
            bool blnImage2Present = false;
            bool blnImage3Present = false;
            bool blnImage4Present = false;
            bool blnImage5Present = false;
            bool blnImage6Present = false;
            for (int i = 0; i < m_strImageFiles.Length; i++)
            {
                if (m_strImageFiles[i].Contains("_Image1"))
                    blnImage1Present = true;

                if (m_strImageFiles[i].Contains("_Image2"))
                    blnImage2Present = true;

                if (m_strImageFiles[i].Contains("_Image3"))
                    blnImage3Present = true;

                if (m_strImageFiles[i].Contains("_Image4"))
                    blnImage4Present = true;

                if (m_strImageFiles[i].Contains("_Image5"))
                    blnImage5Present = true;

                if (m_strImageFiles[i].Contains("_Image6"))
                    blnImage6Present = true;
            }

            if (blnImage1Present)
            {
                if (!cbo_ImageNoSelection.Items.Contains("View Image 1"))
                    cbo_ImageNoSelection.Items.Add("View Image 1");
            }
            else
            {
                if (cbo_ImageNoSelection.Items.Contains("View Image 1"))
                    cbo_ImageNoSelection.Items.Remove("View Image 1");
            }

            if (blnImage2Present)
            {
                if (!cbo_ImageNoSelection.Items.Contains("View Image 2"))
                    cbo_ImageNoSelection.Items.Add("View Image 2");
            }
            else
            {
                if (cbo_ImageNoSelection.Items.Contains("View Image 2"))
                    cbo_ImageNoSelection.Items.Remove("View Image 2");
            }

            if (blnImage3Present)
            {
                if (!cbo_ImageNoSelection.Items.Contains("View Image 3"))
                    cbo_ImageNoSelection.Items.Add("View Image 3");
            }
            else
            {
                if (cbo_ImageNoSelection.Items.Contains("View Image 3"))
                    cbo_ImageNoSelection.Items.Remove("View Image 3");
            }

            if (blnImage4Present)
            {
                if (!cbo_ImageNoSelection.Items.Contains("View Image 4"))
                    cbo_ImageNoSelection.Items.Add("View Image 4");
            }
            else
            {
                if (cbo_ImageNoSelection.Items.Contains("View Image 4"))
                    cbo_ImageNoSelection.Items.Remove("View Image 4");
            }

            if (blnImage5Present)
            {
                if (!cbo_ImageNoSelection.Items.Contains("View Image 5"))
                    cbo_ImageNoSelection.Items.Add("View Image 5");
            }
            else
            {
                if (cbo_ImageNoSelection.Items.Contains("View Image 5"))
                    cbo_ImageNoSelection.Items.Remove("View Image 5");
            }

            if (blnImage6Present)
            {
                if (!cbo_ImageNoSelection.Items.Contains("View Image 6"))
                    cbo_ImageNoSelection.Items.Add("View Image 6");
            }
            else
            {
                if (cbo_ImageNoSelection.Items.Contains("View Image 6"))
                    cbo_ImageNoSelection.Items.Remove("View Image 6");
            }
        }

        private void updateImagePreview()
        {
            if (cbo_ImageNoSelection.SelectedIndex == 0)
            {
                if (m_smVisionInfo.g_arrImages.Count == 1)
                {
                    pic_ImagePreview.Size = new Size(480, 360);
                    pic_ImagePreview2.Visible = false;
                    pic_ImagePreview3.Visible = false;
                    pic_ImagePreview4.Visible = false;
                    pic_ImagePreview5.Visible = false;
                    pic_ImagePreview6.Visible = false;
                }
                else if (m_smVisionInfo.g_arrImages.Count == 2)
                {
                    pic_ImagePreview.Size = new Size(240, 180);
                    pic_ImagePreview2.Visible = true;
                    pic_ImagePreview3.Visible = false;
                    pic_ImagePreview4.Visible = false;
                    pic_ImagePreview5.Visible = false;
                    pic_ImagePreview6.Visible = false;
                }
                else if (m_smVisionInfo.g_arrImages.Count == 3)
                {
                    pic_ImagePreview.Size = new Size(240, 180);
                    pic_ImagePreview2.Visible = true;
                    pic_ImagePreview3.Visible = true;
                    pic_ImagePreview4.Visible = false;
                    pic_ImagePreview5.Visible = false;
                    pic_ImagePreview6.Visible = false;
                }
                else if (m_smVisionInfo.g_arrImages.Count == 4)
                {
                    pic_ImagePreview.Size = new Size(240, 180);
                    pic_ImagePreview2.Visible = true;
                    pic_ImagePreview3.Visible = true;
                    pic_ImagePreview4.Visible = true;
                    pic_ImagePreview5.Visible = false;
                    pic_ImagePreview6.Visible = false;
                }
                else if (m_smVisionInfo.g_arrImages.Count == 5)
                {
                    pic_ImagePreview.Size = new Size(240, 180);
                    pic_ImagePreview2.Visible = true;
                    pic_ImagePreview3.Visible = true;
                    pic_ImagePreview4.Visible = true;
                    pic_ImagePreview5.Visible = true;
                    pic_ImagePreview6.Visible = false;
                }
                else if (m_smVisionInfo.g_arrImages.Count == 6)
                {
                    pic_ImagePreview.Size = new Size(240, 180);
                    pic_ImagePreview2.Visible = true;
                    pic_ImagePreview3.Visible = true;
                    pic_ImagePreview4.Visible = true;
                    pic_ImagePreview5.Visible = true;
                    pic_ImagePreview6.Visible = true;
                }
            }
            else
            {
                pic_ImagePreview.Size = new Size(480, 360);
                pic_ImagePreview2.Visible = false;
                pic_ImagePreview3.Visible = false;
                pic_ImagePreview4.Visible = false;
                pic_ImagePreview5.Visible = false;
                pic_ImagePreview6.Visible = false;
            }
        }

        private void updateImageList()
        {
            if (dgd_ImageList.RowCount < 1)
                return;

            dgd_ImageList.Enabled = false;

            if (cbo_ImageSelection.SelectedItem.ToString() == "All")
            {
                for (int i = dgd_ImageList.RowCount - 1; i >= 0; i--)
                {
                    if (dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image2")
                                || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image4")
                                || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image6"))
                    {
                        if (dgd_ImageList.Rows[i].Visible)
                            dgd_ImageList.Rows[i].Visible = false;

                        continue;
                    }

                    //switch (cbo_ImageNoSelection.SelectedItem.ToString())
                    //{
                    //case "View All":
                    //case "View Image 0":
                    //    if (dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image2")
                    //        || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image4")
                    //        || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image6"))
                    //    {
                    //        if (dgd_ImageList.Rows[i].Visible)
                    //            dgd_ImageList.Rows[i].Visible = false;

                    //        continue;
                    //    }
                    //    break;
                    //case "View Image 1":
                    //    if (!dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image1"))
                    //    {
                    //        if (dgd_ImageList.Rows[i].Visible)
                    //            dgd_ImageList.Rows[i].Visible = false;

                    //        continue;
                    //    }
                    //    break;
                    //case "View Image 2":
                    //    if (!dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image2"))
                    //    {
                    //        if (dgd_ImageList.Rows[i].Visible)
                    //            dgd_ImageList.Rows[i].Visible = false;

                    //        continue;
                    //    }
                    //    break;
                    //case "View Image 3":
                    //    if (!dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image3"))
                    //    {
                    //        if (dgd_ImageList.Rows[i].Visible)
                    //            dgd_ImageList.Rows[i].Visible = false;

                    //        continue;
                    //    }
                    //    break;
                    //case "View Image 4":
                    //    if (!dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image4"))
                    //    {
                    //        if (dgd_ImageList.Rows[i].Visible)
                    //            dgd_ImageList.Rows[i].Visible = false;

                    //        continue;
                    //    }
                    //    break;
                    //case "View Image 5":
                    //    if (!dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image5"))
                    //    {
                    //        if (dgd_ImageList.Rows[i].Visible)
                    //            dgd_ImageList.Rows[i].Visible = false;

                    //        continue;
                    //    }
                    //    break;
                    //}

                    if (!dgd_ImageList.Rows[i].Visible)
                        dgd_ImageList.Rows[i].Visible = true;
                }

                for (int j = 0; j < dgd_ImageList.RowCount; j++)
                {
                    if (dgd_ImageList.Rows[j].Visible)
                    {
                        //dgd_ImageList.FirstDisplayedScrollingRowIndex = j;

                        if (cbo_ImageNoSelection.SelectedItem.ToString() == "View All")
                        {
                            if (m_blnSortByDescending)
                            {
                                if (cbo_ImageNoSelection.SelectedIndex == 0 || cbo_ImageNoSelection.SelectedIndex == 1) // Select All View or View Image 0
                                    pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[2].Value.ToString());
                                else
                                    pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - (cbo_ImageNoSelection.SelectedIndex - 1)].Cells[2].Value.ToString());

                                if (pic_ImagePreview2.Visible && dgd_ImageList.SelectedRows[0].Index - 1 >= 0)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview2.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview2.Image = null;
                                    }
                                    else
                                        pic_ImagePreview2.Image = null;
                                }
                                else
                                    pic_ImagePreview2.Image = null;

                                if (pic_ImagePreview3.Visible && dgd_ImageList.SelectedRows[0].Index - 2 >= 0)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview3.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview3.Image = null;
                                    }
                                    else
                                        pic_ImagePreview3.Image = null;
                                }
                                else
                                    pic_ImagePreview3.Image = null;

                                if (pic_ImagePreview4.Visible && dgd_ImageList.SelectedRows[0].Index - 3 >= 0)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview4.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview4.Image = null;
                                    }
                                    else
                                        pic_ImagePreview4.Image = null;
                                }
                                else
                                    pic_ImagePreview4.Image = null;

                                if (pic_ImagePreview5.Visible && dgd_ImageList.SelectedRows[0].Index - 4 >= 0)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview5.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview5.Image = null;
                                    }
                                    else
                                        pic_ImagePreview5.Image = null;
                                }
                                else
                                    pic_ImagePreview5.Image = null;

                                if (pic_ImagePreview6.Visible && dgd_ImageList.SelectedRows[0].Index - 5 >= 0)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview6.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview6.Image = null;
                                    }
                                    else
                                        pic_ImagePreview6.Image = null;
                                }
                                else
                                    pic_ImagePreview6.Image = null;
                            }
                            else
                            {
                                if (cbo_ImageNoSelection.SelectedIndex == 0 || cbo_ImageNoSelection.SelectedIndex == 1) // Select All View or View Image 0
                                    pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[2].Value.ToString());
                                else
                                    pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + (cbo_ImageNoSelection.SelectedIndex - 1)].Cells[2].Value.ToString());

                                // 2020 03 24 - JBTAN: make sure index not out of range
                                if (pic_ImagePreview2.Visible && dgd_ImageList.Rows.Count > dgd_ImageList.SelectedRows[0].Index + 1)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview2.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview2.Image = null;
                                    }
                                    else
                                        pic_ImagePreview2.Image = null;
                                }
                                else
                                    pic_ImagePreview2.Image = null;

                                if (pic_ImagePreview3.Visible && dgd_ImageList.Rows.Count > dgd_ImageList.SelectedRows[0].Index + 2)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview3.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview3.Image = null;
                                    }
                                    else
                                        pic_ImagePreview3.Image = null;
                                }
                                else
                                    pic_ImagePreview3.Image = null;

                                if (pic_ImagePreview4.Visible && dgd_ImageList.Rows.Count > dgd_ImageList.SelectedRows[0].Index + 3)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview4.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview4.Image = null;
                                    }
                                    else
                                        pic_ImagePreview4.Image = null;
                                }
                                else
                                    pic_ImagePreview4.Image = null;

                                if (pic_ImagePreview5.Visible && dgd_ImageList.Rows.Count > dgd_ImageList.SelectedRows[0].Index + 4)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview5.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview5.Image = null;
                                    }
                                    else
                                        pic_ImagePreview5.Image = null;
                                }
                                else
                                    pic_ImagePreview5.Image = null;

                                if (pic_ImagePreview6.Visible && dgd_ImageList.Rows.Count > dgd_ImageList.SelectedRows[0].Index + 5)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview6.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview6.Image = null;
                                    }
                                    else
                                        pic_ImagePreview6.Image = null;
                                }
                                else
                                    pic_ImagePreview6.Image = null;
                            }

                        }
                        else
                        {
                            if (m_blnSortByDescending)
                                pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - (cbo_ImageNoSelection.SelectedIndex - 1)].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + (cbo_ImageNoSelection.SelectedIndex - 1)].Cells[2].Value.ToString());
                        }

                        break;
                    }
                }
            }
            else
            {
                for (int i = dgd_ImageList.RowCount - 1; i >= 0; i--)
                {
                    bool blnShow = true;
                    if (!dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains(cbo_ImageSelection.SelectedItem.ToString()))
                    {
                        if (cbo_ImageSelection.SelectedItem.ToString() != "Fail" || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("Pass"))
                        {
                            if (dgd_ImageList.Rows[i].Visible)
                            {
                                dgd_ImageList.Rows[i].Visible = false;
                            }

                            continue;
                        }
                    }

                    if (blnShow)
                    {
                        if (dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image2")
                                    || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image4")
                                    || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[i].Visible)
                                dgd_ImageList.Rows[i].Visible = false;

                            continue;
                        }
                        //switch (cbo_ImageNoSelection.SelectedItem.ToString())
                        //{
                        //    case "View All":
                        //    case "View Image 0":
                        //        if (dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image2")
                        //            || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image4")
                        //            || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image6"))
                        //        {
                        //            if (dgd_ImageList.Rows[i].Visible)
                        //                dgd_ImageList.Rows[i].Visible = false;

                        //            continue;
                        //        }
                        //        break;
                        //    case "View Image 1":
                        //        if (!dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image1"))
                        //        {
                        //            if (dgd_ImageList.Rows[i].Visible)
                        //                dgd_ImageList.Rows[i].Visible = false;

                        //            continue;
                        //        }
                        //        break;
                        //    case "View Image 2":
                        //        if (!dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image2"))
                        //        {
                        //            if (dgd_ImageList.Rows[i].Visible)
                        //                dgd_ImageList.Rows[i].Visible = false;

                        //            continue;
                        //        }
                        //        break;
                        //    case "View Image 3":
                        //        if (!dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image3"))
                        //        {
                        //            if (dgd_ImageList.Rows[i].Visible)
                        //                dgd_ImageList.Rows[i].Visible = false;

                        //            continue;
                        //        }
                        //        break;
                        //    case "View Image 4":
                        //        if (!dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image4"))
                        //        {
                        //            if (dgd_ImageList.Rows[i].Visible)
                        //                dgd_ImageList.Rows[i].Visible = false;

                        //            continue;
                        //        }
                        //        break;
                        //    case "View Image 5":
                        //        if (!dgd_ImageList.Rows[i].Cells[0].Value.ToString().Contains("_Image5"))
                        //        {
                        //            if (dgd_ImageList.Rows[i].Visible)
                        //                dgd_ImageList.Rows[i].Visible = false;

                        //            continue;
                        //        }
                        //        break;
                        //}

                        if (!dgd_ImageList.Rows[i].Visible)
                            dgd_ImageList.Rows[i].Visible = true;
                    }
                }

                for (int j = 0; j < dgd_ImageList.RowCount; j++)
                {
                    if (dgd_ImageList.Rows[j].Visible)
                    {
                        //dgd_ImageList.FirstDisplayedScrollingRowIndex = j;

                        if (cbo_ImageNoSelection.SelectedItem.ToString() == "View All")
                        {
                            if (m_blnSortByDescending)
                            {
                                if (cbo_ImageNoSelection.SelectedIndex == 0 || cbo_ImageNoSelection.SelectedIndex == 1) // Select All View or View Image 0
                                    pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[2].Value.ToString());
                                else
                                    pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - (cbo_ImageNoSelection.SelectedIndex - 1)].Cells[2].Value.ToString());

                                if (pic_ImagePreview2.Visible && dgd_ImageList.SelectedRows[0].Index - 1 >= 0)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview2.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview2.Image = null;
                                    }
                                    else
                                        pic_ImagePreview2.Image = null;
                                }
                                else
                                    pic_ImagePreview2.Image = null;

                                if (pic_ImagePreview3.Visible && dgd_ImageList.SelectedRows[0].Index - 2 >= 0)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview3.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview3.Image = null;
                                    }
                                    else
                                        pic_ImagePreview3.Image = null;
                                }
                                else
                                    pic_ImagePreview3.Image = null;

                                if (pic_ImagePreview4.Visible && dgd_ImageList.SelectedRows[0].Index - 3 >= 0)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview4.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview4.Image = null;
                                    }
                                    else
                                        pic_ImagePreview4.Image = null;
                                }
                                else
                                    pic_ImagePreview4.Image = null;

                                if (pic_ImagePreview5.Visible && dgd_ImageList.SelectedRows[0].Index - 4 >= 0)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview5.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview5.Image = null;
                                    }
                                    else
                                        pic_ImagePreview5.Image = null;
                                }
                                else
                                    pic_ImagePreview5.Image = null;

                                if (pic_ImagePreview6.Visible && dgd_ImageList.SelectedRows[0].Index - 5 >= 0)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview6.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview6.Image = null;
                                    }
                                    else
                                        pic_ImagePreview6.Image = null;
                                }
                                else
                                    pic_ImagePreview6.Image = null;
                            }
                            else
                            {
                                if (cbo_ImageNoSelection.SelectedIndex == 0 || cbo_ImageNoSelection.SelectedIndex == 1) // Select All View or View Image 0
                                    pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[2].Value.ToString());
                                else
                                    pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + (cbo_ImageNoSelection.SelectedIndex - 1)].Cells[2].Value.ToString());

                                // 2020 03 24 - JBTAN: make sure index not out of range
                                if (pic_ImagePreview2.Visible && dgd_ImageList.Rows.Count > dgd_ImageList.SelectedRows[0].Index + 1)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview2.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview2.Image = null;
                                    }
                                    else
                                        pic_ImagePreview2.Image = null;
                                }
                                else
                                    pic_ImagePreview2.Image = null;

                                if (pic_ImagePreview3.Visible && dgd_ImageList.Rows.Count > dgd_ImageList.SelectedRows[0].Index + 2)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview3.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview3.Image = null;
                                    }
                                    else
                                        pic_ImagePreview3.Image = null;
                                }
                                else
                                    pic_ImagePreview3.Image = null;

                                if (pic_ImagePreview4.Visible && dgd_ImageList.Rows.Count > dgd_ImageList.SelectedRows[0].Index + 3)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview4.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview4.Image = null;
                                    }
                                    else
                                        pic_ImagePreview4.Image = null;
                                }
                                else
                                    pic_ImagePreview4.Image = null;

                                if (pic_ImagePreview5.Visible && dgd_ImageList.Rows.Count > dgd_ImageList.SelectedRows[0].Index + 4)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview5.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview5.Image = null;
                                    }
                                    else
                                        pic_ImagePreview5.Image = null;
                                }
                                else
                                    pic_ImagePreview5.Image = null;

                                if (pic_ImagePreview6.Visible && dgd_ImageList.Rows.Count > dgd_ImageList.SelectedRows[0].Index + 5)
                                {
                                    if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image2")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image4")
                                        || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image6"))
                                    {
                                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                            pic_ImagePreview6.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[2].Value.ToString());
                                        else
                                            pic_ImagePreview6.Image = null;
                                    }
                                    else
                                        pic_ImagePreview6.Image = null;
                                }
                                else
                                    pic_ImagePreview6.Image = null;
                            }
                        }
                        else
                        {
                            if (m_blnSortByDescending)
                                pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - (cbo_ImageNoSelection.SelectedIndex - 1)].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + (cbo_ImageNoSelection.SelectedIndex - 1)].Cells[2].Value.ToString());
                        }

                        break;
                    }
                }
            }
            dgd_ImageList.Enabled = true;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (dgd_ImageList.RowCount < 1)
            {
                SRMMessageBox.Show("No image selected!");
                return;
            }
            int intRowIndex = 0;

            if (dgd_ImageList.CurrentCell != null)
                intRowIndex = dgd_ImageList.SelectedRows[0].Index;
            else
                intRowIndex = dgd_ImageList.FirstDisplayedScrollingRowIndex;

            m_strSelectedImagePath = dgd_ImageList.Rows[intRowIndex].Cells[2].Value.ToString();

            //if (cbo_ImageSelection.SelectedItem.ToString() == "All")
            //    //m_blnLoadAllImage = true;
            //    m_intLoadType = 0;
            //else if (cbo_ImageSelection.SelectedItem.ToString() == "Fail")
            //    m_intLoadType = 1;
            //else
            //    m_intLoadType = 2;

            if (m_strSelectedImagePath == "")
            {
                SRMMessageBox.Show("Cannot find selected image path!");
                return;
            }

            for (int i = 0; i < dgd_ImageList.RowCount; i++)
            {
                if (intRowIndex + i < dgd_ImageList.RowCount)
                {
                    if (dgd_ImageList.Rows[intRowIndex + i].Visible)
                        m_arrSelectedImageList.Add(dgd_ImageList.Rows[intRowIndex + i].Cells[2].Value.ToString());
                }
                else
                {
                    if (dgd_ImageList.Rows[intRowIndex + i - dgd_ImageList.RowCount].Visible)
                        m_arrSelectedImageList.Add(dgd_ImageList.Rows[intRowIndex + i - dgd_ImageList.RowCount].Cells[2].Value.ToString());
                }
            }

            this.DialogResult = DialogResult.OK;

            if (pic_ImagePreview.Image != null)
                pic_ImagePreview.Image.Dispose();
            if (pic_ImagePreview2.Image != null)
                pic_ImagePreview2.Image.Dispose();
            if (pic_ImagePreview3.Image != null)
                pic_ImagePreview3.Image.Dispose();
            if (pic_ImagePreview4.Image != null)
                pic_ImagePreview4.Image.Dispose();
            if (pic_ImagePreview5.Image != null)
                pic_ImagePreview5.Image.Dispose();
            if (pic_ImagePreview6.Image != null)
                pic_ImagePreview6.Image.Dispose();
            dgd_ImageList.Dispose();
            Close();
            Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            if (pic_ImagePreview.Image != null)
                pic_ImagePreview.Image.Dispose();
            if (pic_ImagePreview2.Image != null)
                pic_ImagePreview2.Image.Dispose();
            if (pic_ImagePreview3.Image != null)
                pic_ImagePreview3.Image.Dispose();
            if (pic_ImagePreview4.Image != null)
                pic_ImagePreview4.Image.Dispose();
            if (pic_ImagePreview5.Image != null)
                pic_ImagePreview5.Image.Dispose();
            if (pic_ImagePreview6.Image != null)
                pic_ImagePreview6.Image.Dispose();
            dgd_ImageList.Dispose();
            Close();
            Dispose();
        }

        private void cbo_ImageNoSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            updateImagePreview();
            updateImageList();
        }

        private void btn_Browse_Click(object sender, EventArgs e)
        {
            dlg_ImageFile.Reset();
            dlg_ImageFile.InitialDirectory = m_strPath;

            if (dlg_ImageFile.ShowDialog() == DialogResult.OK)
            {
                string strFileName = dlg_ImageFile.FileName;
                m_strPath = Path.GetDirectoryName(strFileName);
                m_strImageFiles = Directory.GetFiles(m_strPath, "*.bmp");
                UpdateGUI();
                updateImagePreview();
                CreateImageList();
            }
        }

        private void dgd_ImageList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (dgd_ImageList.Rows.Count == 0)
                return;

            if (e.RowIndex != -1)
            {
                //Display image
                if (m_blnSortByDescending)
                {
                    if (cbo_ImageNoSelection.SelectedIndex == 0 || cbo_ImageNoSelection.SelectedIndex == 1) // Select All View or View Image 0
                        pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex].Cells[2].Value.ToString());
                    else
                        pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex - (cbo_ImageNoSelection.SelectedIndex - 1)].Cells[2].Value.ToString());

                    // 2020 03 24 - JBTAN: make sure index not out of range
                    if (pic_ImagePreview2.Visible && e.RowIndex - 1 >= 0)
                    {
                        if (dgd_ImageList.Rows[e.RowIndex - 1].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[e.RowIndex - 1].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[e.RowIndex - 1].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[e.RowIndex - 1].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[e.RowIndex - 1].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[e.RowIndex - 1].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[e.RowIndex - 1].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[e.RowIndex - 1].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[e.RowIndex].Cells[0].Value.ToString())
                                pic_ImagePreview2.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex - 1].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview2.Image = null;
                        }
                        else
                            pic_ImagePreview2.Image = null;
                    }
                    else
                        pic_ImagePreview2.Image = null;

                    if (pic_ImagePreview3.Visible && e.RowIndex - 2 >= 0)
                    {
                        if (dgd_ImageList.Rows[e.RowIndex - 2].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[e.RowIndex - 2].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[e.RowIndex - 2].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[e.RowIndex - 2].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[e.RowIndex - 2].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[e.RowIndex - 2].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[e.RowIndex - 2].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[e.RowIndex - 2].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[e.RowIndex].Cells[0].Value.ToString())
                                pic_ImagePreview3.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex - 2].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview3.Image = null;
                        }
                        else
                            pic_ImagePreview3.Image = null;
                    }
                    else
                        pic_ImagePreview3.Image = null;

                    if (pic_ImagePreview4.Visible && e.RowIndex - 3 >= 0)
                    {
                        if (dgd_ImageList.Rows[e.RowIndex - 3].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[e.RowIndex - 3].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[e.RowIndex - 3].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[e.RowIndex - 3].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[e.RowIndex - 3].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[e.RowIndex - 3].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[e.RowIndex - 3].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[e.RowIndex - 3].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[e.RowIndex].Cells[0].Value.ToString())
                                pic_ImagePreview4.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex - 3].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview4.Image = null;
                        }
                        else
                            pic_ImagePreview4.Image = null;
                    }
                    else
                        pic_ImagePreview4.Image = null;

                    if (pic_ImagePreview5.Visible && e.RowIndex - 4 >= 0)
                    {
                        if (dgd_ImageList.Rows[e.RowIndex - 4].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[e.RowIndex - 4].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[e.RowIndex - 4].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[e.RowIndex - 4].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[e.RowIndex - 4].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[e.RowIndex - 4].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[e.RowIndex - 4].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[e.RowIndex - 4].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[e.RowIndex].Cells[0].Value.ToString())
                                pic_ImagePreview5.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex - 4].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview5.Image = null;
                        }
                        else
                            pic_ImagePreview5.Image = null;
                    }
                    else
                        pic_ImagePreview5.Image = null;

                    if (pic_ImagePreview6.Visible && e.RowIndex - 5 >= 0)
                    {
                        if (dgd_ImageList.Rows[e.RowIndex - 5].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[e.RowIndex - 5].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[e.RowIndex - 5].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[e.RowIndex - 5].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[e.RowIndex - 5].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[e.RowIndex - 5].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[e.RowIndex - 5].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[e.RowIndex - 5].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[e.RowIndex].Cells[0].Value.ToString())
                                pic_ImagePreview6.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex - 5].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview6.Image = null;
                        }
                        else
                            pic_ImagePreview6.Image = null;
                    }
                    else
                        pic_ImagePreview6.Image = null;

                }
                else
                {
                    if (cbo_ImageNoSelection.SelectedIndex == 0 || cbo_ImageNoSelection.SelectedIndex == 1) // Select All View or View Image 0
                        pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex].Cells[2].Value.ToString());
                    else
                        pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex + (cbo_ImageNoSelection.SelectedIndex - 1)].Cells[2].Value.ToString());

                    // 2020 03 24 - JBTAN: make sure index not out of range
                    if (pic_ImagePreview2.Visible && dgd_ImageList.Rows.Count > e.RowIndex + 1)
                    {
                        if (dgd_ImageList.Rows[e.RowIndex + 1].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[e.RowIndex + 1].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[e.RowIndex + 1].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[e.RowIndex + 1].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[e.RowIndex + 1].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[e.RowIndex + 1].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[e.RowIndex + 1].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[e.RowIndex + 1].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[e.RowIndex].Cells[0].Value.ToString())
                                pic_ImagePreview2.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex + 1].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview2.Image = null;
                        }
                        else
                            pic_ImagePreview2.Image = null;
                    }
                    else
                        pic_ImagePreview2.Image = null;

                    if (pic_ImagePreview3.Visible && dgd_ImageList.Rows.Count > e.RowIndex + 2)
                    {
                        if (dgd_ImageList.Rows[e.RowIndex + 2].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[e.RowIndex + 2].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[e.RowIndex + 2].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[e.RowIndex + 2].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[e.RowIndex + 2].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[e.RowIndex + 2].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[e.RowIndex + 2].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[e.RowIndex + 2].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[e.RowIndex].Cells[0].Value.ToString())
                                pic_ImagePreview3.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex + 2].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview3.Image = null;
                        }
                        else
                            pic_ImagePreview3.Image = null;
                    }
                    else
                        pic_ImagePreview3.Image = null;

                    if (pic_ImagePreview4.Visible && dgd_ImageList.Rows.Count > e.RowIndex + 3)
                    {
                        if (dgd_ImageList.Rows[e.RowIndex + 3].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[e.RowIndex + 3].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[e.RowIndex + 3].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[e.RowIndex + 3].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[e.RowIndex + 3].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[e.RowIndex + 3].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[e.RowIndex + 3].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[e.RowIndex + 3].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[e.RowIndex].Cells[0].Value.ToString())
                                pic_ImagePreview4.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex + 3].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview4.Image = null;
                        }
                        else
                            pic_ImagePreview4.Image = null;
                    }
                    else
                        pic_ImagePreview4.Image = null;

                    if (pic_ImagePreview5.Visible && dgd_ImageList.Rows.Count > e.RowIndex + 4)
                    {
                        if (dgd_ImageList.Rows[e.RowIndex + 4].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[e.RowIndex + 4].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[e.RowIndex + 4].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[e.RowIndex + 4].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[e.RowIndex + 4].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[e.RowIndex + 4].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[e.RowIndex + 4].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[e.RowIndex + 4].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[e.RowIndex].Cells[0].Value.ToString())
                                pic_ImagePreview5.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex + 4].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview5.Image = null;
                        }
                        else
                            pic_ImagePreview5.Image = null;
                    }
                    else
                        pic_ImagePreview5.Image = null;

                    if (pic_ImagePreview6.Visible && dgd_ImageList.Rows.Count > e.RowIndex + 5)
                    {
                        if (dgd_ImageList.Rows[e.RowIndex + 5].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[e.RowIndex + 5].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[e.RowIndex + 5].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[e.RowIndex + 5].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[e.RowIndex + 5].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[e.RowIndex + 5].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[e.RowIndex + 5].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[e.RowIndex + 5].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[e.RowIndex].Cells[0].Value.ToString())
                                pic_ImagePreview6.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex + 5].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview6.Image = null;
                        }
                        else
                            pic_ImagePreview6.Image = null;
                    }
                    else
                        pic_ImagePreview6.Image = null;
                }
            }
            else
            {
                //sort file name
                if (m_blnSortByDescending)
                {
                    m_blnSortByDescending = false;
                    RowComparer rc = new RowComparer(SortOrder.Ascending, e.ColumnIndex);
                    dgd_ImageList.Sort(rc);
                }
                else
                {
                    m_blnSortByDescending = true;
                    RowComparer rc = new RowComparer(SortOrder.Descending, e.ColumnIndex);
                    dgd_ImageList.Sort(rc);
                }
                for (int j = 0; j < dgd_ImageList.RowCount; j++)
                {
                    if (dgd_ImageList.Rows[j].Visible)
                    {
                        dgd_ImageList.FirstDisplayedScrollingRowIndex = j;
                        break;
                    }
                }
            }

            m_blnUpdateErrorMessage = true;
        }

        private void cbo_ImageSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            updateImageList();
        }

        private void dgd_ImageList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != (Keys.Down) && e.KeyData != (Keys.Up))
                return;

            if (!m_blnInitDone)
                return;

            if (dgd_ImageList.Rows.Count == 0)
                return;

            if (m_intSelectedRowPrev != dgd_ImageList.SelectedRows[0].Index)
            {
                m_intSelectedRowPrev = dgd_ImageList.SelectedRows[0].Index;


                if (m_blnSortByDescending)
                {
                    if (cbo_ImageNoSelection.SelectedIndex == 0 || cbo_ImageNoSelection.SelectedIndex == 1) // Select All View or View Image 0
                        pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[2].Value.ToString());
                    else
                        pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - (cbo_ImageNoSelection.SelectedIndex - 1)].Cells[2].Value.ToString());

                    if (pic_ImagePreview2.Visible && dgd_ImageList.SelectedRows[0].Index - 1 >= 0)
                    {
                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                pic_ImagePreview2.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 1].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview2.Image = null;
                        }
                        else
                            pic_ImagePreview2.Image = null;
                    }
                    else
                        pic_ImagePreview2.Image = null;

                    if (pic_ImagePreview3.Visible && dgd_ImageList.SelectedRows[0].Index - 2 >= 0)
                    {
                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                pic_ImagePreview3.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 2].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview3.Image = null;
                        }
                        else
                            pic_ImagePreview3.Image = null;
                    }
                    else
                        pic_ImagePreview3.Image = null;

                    if (pic_ImagePreview4.Visible && dgd_ImageList.SelectedRows[0].Index - 3 >= 0)
                    {
                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                pic_ImagePreview4.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 3].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview4.Image = null;
                        }
                        else
                            pic_ImagePreview4.Image = null;
                    }
                    else
                        pic_ImagePreview4.Image = null;

                    if (pic_ImagePreview5.Visible && dgd_ImageList.SelectedRows[0].Index - 4 >= 0)
                    {
                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                pic_ImagePreview5.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 4].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview5.Image = null;
                        }
                        else
                            pic_ImagePreview5.Image = null;
                    }
                    else
                        pic_ImagePreview5.Image = null;

                    if (pic_ImagePreview6.Visible && dgd_ImageList.SelectedRows[0].Index - 5 >= 0)
                    {
                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                pic_ImagePreview6.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index - 5].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview6.Image = null;
                        }
                        else
                            pic_ImagePreview6.Image = null;
                    }
                    else
                        pic_ImagePreview6.Image = null;
                }
                else
                {
                    if (cbo_ImageNoSelection.SelectedIndex == 0 || cbo_ImageNoSelection.SelectedIndex == 1) // Select All View or View Image 0
                        pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[2].Value.ToString());
                    else
                        pic_ImagePreview.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + (cbo_ImageNoSelection.SelectedIndex - 1)].Cells[2].Value.ToString());

                    // 2020 03 24 - JBTAN: make sure index not out of range
                    if (pic_ImagePreview2.Visible && dgd_ImageList.Rows.Count > dgd_ImageList.SelectedRows[0].Index + 1)
                    {
                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                pic_ImagePreview2.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 1].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview2.Image = null;
                        }
                        else
                            pic_ImagePreview2.Image = null;
                    }
                    else
                        pic_ImagePreview2.Image = null;

                    if (pic_ImagePreview3.Visible && dgd_ImageList.Rows.Count > dgd_ImageList.SelectedRows[0].Index + 2)
                    {
                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                pic_ImagePreview3.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 2].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview3.Image = null;
                        }
                        else
                            pic_ImagePreview3.Image = null;
                    }
                    else
                        pic_ImagePreview3.Image = null;

                    if (pic_ImagePreview4.Visible && dgd_ImageList.Rows.Count > dgd_ImageList.SelectedRows[0].Index + 3)
                    {
                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                pic_ImagePreview4.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 3].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview4.Image = null;
                        }
                        else
                            pic_ImagePreview4.Image = null;
                    }
                    else
                        pic_ImagePreview4.Image = null;

                    if (pic_ImagePreview5.Visible && dgd_ImageList.Rows.Count > dgd_ImageList.SelectedRows[0].Index + 4)
                    {
                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                pic_ImagePreview5.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 4].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview5.Image = null;
                        }
                        else
                            pic_ImagePreview5.Image = null;
                    }
                    else
                        pic_ImagePreview5.Image = null;

                    if (pic_ImagePreview6.Visible && dgd_ImageList.Rows.Count > dgd_ImageList.SelectedRows[0].Index + 5)
                    {
                        if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image1") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image2")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image3") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image4")
                            || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image5") || dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Contains("_Image6"))
                        {
                            if (dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().Substring(0, dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[0].Value.ToString().IndexOf("_Image")) == dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[0].Value.ToString())
                                pic_ImagePreview6.Image = Image.FromFile(dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index + 5].Cells[2].Value.ToString());
                            else
                                pic_ImagePreview6.Image = null;
                        }
                        else
                            pic_ImagePreview6.Image = null;
                    }
                    else
                        pic_ImagePreview6.Image = null;
                }
            }

            m_blnUpdateErrorMessage = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (dgd_ImageList.Rows.Count == 0)
                return;

            if (m_blnUpdateErrorMessage)
            {
                m_blnUpdateErrorMessage = false;
                // Draw the text

                lbl_ErrorMessge.Text = "";

                string strErrorMessage = dgd_ImageList.Rows[dgd_ImageList.SelectedRows[0].Index].Cells[3].Value.ToString();

                if (strErrorMessage != "No Info.")
                    lbl_ErrorMessge.ForeColor = Color.Red;
                else
                    lbl_ErrorMessge.ForeColor = Color.Black;

                string strTranslatedMsg = LanguageLibrary.Convert(strErrorMessage);

                string[] strResult = strTranslatedMsg.Split('*');
                string str = "";
                for (int i = 0; i < strResult.Length; i++)
                {
                    if (strResult[i] != "")
                    {
                        str += strResult[i] + "\n";
                    }
                }

                lbl_ErrorMessge.Text = str;
            }
        }
    }
}