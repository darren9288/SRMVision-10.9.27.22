using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using SharedMemory;
using System.IO;

namespace AutoMode
{
    public partial class LoadRecipeImageForm : Form
    {
        string m_strPath;
        List<string> m_strImageFiles = new List<string>();
        string[][] m_strImageFolder;
        List<string> m_strFolderName = new List<string>();
        private bool m_blnInitDone = false;
        private string m_strSelectedImagePath = "";
        private int m_intSelectedRowPrev = -1;
        private int m_intImageCount = 0;
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;

        public string ref_strSelectedImagePath { get { return m_strSelectedImagePath; } }

        public LoadRecipeImageForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, ProductionInfo smProductionInfo, string strPath, string VisionName)
        {
            InitializeComponent();
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_smProductionInfo = smProductionInfo;
            m_strPath = strPath;
            DirectoryInfo dir = new DirectoryInfo(m_strPath);
            DirectoryInfo[] dirs = dir.GetDirectories();
            m_strImageFolder = new string[dirs.Length][];
            if (m_strPath != "")
            {
                for (int i = 0; i < dirs.Length; i++)
                {
                    string dirName = dirs[i].ToString();
                    string TemplatePath = m_strPath + dirName + "\\Template";
                    if (Directory.Exists(TemplatePath))
                    {
                        DirectoryInfo Imagedir = new DirectoryInfo(TemplatePath);
                        FileInfo[] files = Imagedir.GetFiles("*.bmp", SearchOption.AllDirectories);
                        foreach (FileInfo file in files)
                        {
                            if (file.Name.Contains("OriTemplate") && !file.Name.Contains("Char"))
                            {
                                m_strImageFiles.Add(file.FullName);
                            }
                        }
                        m_strImageFolder[i] = m_strImageFiles.ToArray();
                        m_strImageFiles.Clear();
                    }
                }
            }
            UpdateGUI();
            CreateImageList();
            if (File.Exists(dir + "CalibrationImage.bmp"))
            {
                dgd_ImageList.Rows.Add();
                dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[0].Value = "CalibrationImage";
                dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[1].Value = File.GetCreationTime(dir + "CalibrationImage.bmp").ToString("yy/MM/dd HH:mm:ss");

                dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[2].Value = dir + "CalibrationImage.bmp";

                dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[7].Value = 1;
            }
            if (File.Exists(dir + "CalibrationImageHorizontal.bmp"))
            {
                dgd_ImageList.Rows.Add();
                dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[0].Value = "CalibrationImageHorizontal";
                dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[1].Value = File.GetCreationTime(dir + "CalibrationImageHorizontal.bmp").ToString("yy/MM/dd HH:mm:ss");

                dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[2].Value = dir + "CalibrationImageHorizontal.bmp";

                dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[7].Value = 1;
            }
            if (File.Exists(dir + "CalibrationImageVertical.bmp"))
            {
                dgd_ImageList.Rows.Add();
                dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[0].Value = "CalibrationImageVertical";
                dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[1].Value = File.GetCreationTime(dir + "CalibrationImageVertical.bmp").ToString("yy/MM/dd HH:mm:ss");

                dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[2].Value = dir + "CalibrationImageVertical.bmp";

                dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[7].Value = 1;
            }

            m_blnInitDone = true;
            if (dgd_ImageList.Rows.Count > 0)
            {
                m_intImageCount = Convert.ToInt32(dgd_ImageList.Rows[0].Cells[7].Value);
                ResizeImage(cbo_ImageNoSelection.SelectedIndex);

                if (dgd_ImageList.Rows[0].Cells[2].Value != null)
                {
                    if (dgd_ImageList.Rows[0].Cells[2].Value.ToString() != "")
                        pic_Image1.Image = Image.FromFile(dgd_ImageList.Rows[0].Cells[2].Value.ToString());
                }
                else
                {
                    pic_Image1.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (dgd_ImageList.Rows[0].Cells[3].Value != null)
                {
                    if (dgd_ImageList.Rows[0].Cells[3].Value.ToString() != "")
                        pic_Image2.Image = Image.FromFile(dgd_ImageList.Rows[0].Cells[3].Value.ToString());
                }
                else
                {
                    pic_Image2.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (dgd_ImageList.Rows[0].Cells[4].Value != null)
                {
                    if (dgd_ImageList.Rows[0].Cells[4].Value.ToString() != "")
                        pic_Image3.Image = Image.FromFile(dgd_ImageList.Rows[0].Cells[4].Value.ToString());
                }
                else
                {
                    pic_Image3.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (dgd_ImageList.Rows[0].Cells[5].Value != null)
                {
                    if (dgd_ImageList.Rows[0].Cells[5].Value.ToString() != "")
                        pic_Image4.Image = Image.FromFile(dgd_ImageList.Rows[0].Cells[5].Value.ToString());
                }
                else
                {
                    pic_Image4.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (dgd_ImageList.Rows[0].Cells[6].Value != null)
                {
                    if (dgd_ImageList.Rows[0].Cells[6].Value.ToString() != "")
                        pic_Image5.Image = Image.FromFile(dgd_ImageList.Rows[0].Cells[6].Value.ToString());
                }
                else
                {
                    pic_Image5.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }
            }
        }

        private void CreateImageList()
        {
            dgd_ImageList.Rows.Clear();
            for (int i = 0; i < m_strImageFolder.Length; i++)
            {
                if (m_strImageFolder[i] == null)
                    continue;

                int intTemplateCount = 1;
                for (int j = 0; j < m_strImageFolder[i].Length; j++)
                {
                    if (m_strImageFolder[i][j].Contains("_Image1") || m_strImageFolder[i][j].Contains("_Image2") || m_strImageFolder[i][j].Contains("_Image3") || m_strImageFolder[i][j].Contains("_Image4") || m_strImageFolder[i][j].Contains("_Image5") || m_strImageFolder[i][j].Contains("_Image6"))
                        continue;

                    if (Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Substring(Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Length - 1, 1) == "1")
                        intTemplateCount++;

                    if (Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Substring(Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Length - 1, 1) == "2")
                        intTemplateCount++;

                    if (Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Substring(Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Length - 1, 1) == "3")
                        intTemplateCount++;

                    if (Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Substring(Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Length - 1, 1) == "4")
                        intTemplateCount++;

                    if (Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Substring(Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Length - 1, 1) == "5")
                        intTemplateCount++;

                    if (Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Substring(Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Length - 1, 1) == "6")
                        intTemplateCount++;

                    if (Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Substring(Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Length - 1, 1) == "7")
                        intTemplateCount++;

                }

                if (m_strImageFolder[i].Length > 0)
                {
                    int intImageLength;
                    for (int k = 0; k < intTemplateCount; k++)
                    {
                        intImageLength = 0;
                        dgd_ImageList.Rows.Add();

                        if (m_smVisionInfo.g_strVisionName.Contains("BottomPosition"))
                        {
                            if (m_strFolderName[i].Equals("Orient"))
                                dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[0].Value = "Position" + " Template " + (k + 1).ToString();
                            else
                                dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[0].Value = m_strFolderName[i] + " Template " + (k + 1).ToString();
                        }
                        else
                            dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[0].Value = m_strFolderName[i] + " Template " + (k + 1).ToString();

                        dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[1].Value = File.GetCreationTime(m_strImageFolder[i][0]).ToString("yy/MM/dd HH:mm:ss");
                        for (int j = 0; j < m_strImageFolder[i].Length; j++)
                        {
                            if (m_strImageFolder[i][j].Contains("_Image1") || m_strImageFolder[i][j].Contains("_Image2") || m_strImageFolder[i][j].Contains("_Image3") || m_strImageFolder[i][j].Contains("_Image4") || m_strImageFolder[i][j].Contains("_Image5") || m_strImageFolder[i][j].Contains("_Image6"))
                            {
                                if (Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Substring(Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).IndexOf("_Image") - 1, 1) != "e")
                                    if (Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Substring(Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).IndexOf("_Image") - 1, 1) != k.ToString())
                                        continue;
                            }
                            else
                            {
                                if (Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Substring(Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Length - 1, 1) != "e")
                                    if (Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Substring(Path.GetFileNameWithoutExtension(m_strImageFolder[i][j]).Length - 1, 1) != k.ToString())
                                        continue;
                            }

                            if ((intImageLength + 2) < dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells.Count)
                                dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[intImageLength + 2].Value = m_strImageFolder[i][j];

                            intImageLength++;
                        }
                        dgd_ImageList.Rows[dgd_ImageList.Rows.Count - 1].Cells[7].Value = intImageLength;
                    }
                }
            }
        }
        public void UpdateGUI()
        {
            string[] arrFolderName = Directory.GetDirectories(m_strPath);

            for (int i = 0; i < arrFolderName.Length; i++)
            {
                m_strFolderName.Add(Path.GetFileName(arrFolderName[i]));
            }

            if (!cbo_ImageNoSelection.Items.Contains("View All"))
                cbo_ImageNoSelection.Items.Add("View All");
            if (!cbo_ImageNoSelection.Items.Contains("View Image 0"))
                cbo_ImageNoSelection.Items.Add("View Image 0");
            cbo_ImageNoSelection.SelectedIndex = 0;

            bool blnImage1Present = false;
            bool blnImage2Present = false;
            bool blnImage3Present = false;
            bool blnImage4Present = false;

            for (int i = 0; i < m_strImageFolder.Length; i++)
            {
                if (m_strImageFolder[i] == null)
                    continue;

                for (int j = 0; j < m_strImageFolder[i].Length; j++)
                {
                    if (m_strImageFolder[i][j].Contains("_Image1"))
                        blnImage1Present = true;

                    if (m_strImageFolder[i][j].Contains("_Image2"))
                        blnImage2Present = true;

                    if (m_strImageFolder[i][j].Contains("_Image3"))
                        blnImage3Present = true;

                    if (m_strImageFolder[i][j].Contains("_Image4"))
                        blnImage4Present = true;
                }
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
        }
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            // 2021 02 19 - CCENG: Make sure image is dispose to prevent the picture box lock the loaded folder.
            if (pic_Image1 != null && !pic_Image1.IsDisposed)
            {
                if (pic_Image1.Image != null)
                    pic_Image1.Image.Dispose();
                pic_Image1.Dispose();
                pic_Image1 = null;
            }
            if (pic_Image2 != null && !pic_Image2.IsDisposed)
            {
                if (pic_Image2.Image != null)
                    pic_Image2.Image.Dispose();
                pic_Image2.Dispose();
                pic_Image2 = null;
            }
            if (pic_Image3 != null && !pic_Image3.IsDisposed)
            {
                if (pic_Image3.Image != null)
                    pic_Image3.Image.Dispose();
                pic_Image3.Dispose();
                pic_Image3 = null;
            }
            if (pic_Image4 != null && !pic_Image4.IsDisposed)
            {
                if (pic_Image4.Image != null)
                    pic_Image4.Image.Dispose();
                pic_Image4.Dispose();
                pic_Image4 = null;
            }
            if (pic_Image5 != null && !pic_Image5.IsDisposed)
            {
                if (pic_Image5.Image != null)
                    pic_Image5.Image.Dispose();
                pic_Image5.Dispose();
                pic_Image5 = null;
            }
            if (pic_Image6 != null && !pic_Image6.IsDisposed)
            {
                if (pic_Image6.Image != null)
                    pic_Image6.Image.Dispose();
                pic_Image6.Dispose();
                pic_Image6 = null;
            }

            Close();
            Dispose();
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
                intRowIndex = dgd_ImageList.CurrentCell.RowIndex;
            else
                intRowIndex = dgd_ImageList.FirstDisplayedScrollingRowIndex;

            m_strSelectedImagePath = dgd_ImageList.Rows[intRowIndex].Cells[2].Value.ToString();

            if (m_strSelectedImagePath == "")
            {
                SRMMessageBox.Show("Cannot find selected image path!");
                return;
            }

            // 2021 02 19 - CCENG: Make sure image is dispose to prevent the picture box lock the loaded folder.
            if (pic_Image1 != null && !pic_Image1.IsDisposed)
            {
                if (pic_Image1.Image != null)
                    pic_Image1.Image.Dispose();
                pic_Image1.Dispose();
                pic_Image1 = null;
            }
            if (pic_Image2 != null && !pic_Image2.IsDisposed)
            {
                if (pic_Image2.Image != null)
                    pic_Image2.Image.Dispose();
                pic_Image2.Dispose();
                pic_Image2 = null;
            }
            if (pic_Image3 != null && !pic_Image3.IsDisposed)
            {
                if (pic_Image3.Image != null)
                    pic_Image3.Image.Dispose();
                pic_Image3.Dispose();
                pic_Image3 = null;
            }
            if (pic_Image4 != null && !pic_Image4.IsDisposed)
            {
                if (pic_Image4.Image != null)
                    pic_Image4.Image.Dispose();
                pic_Image4.Dispose();
                pic_Image4 = null;
            }
            if (pic_Image5 != null && !pic_Image5.IsDisposed)
            {
                if (pic_Image5.Image != null)
                    pic_Image5.Image.Dispose();
                pic_Image5.Dispose();
                pic_Image5 = null;
            }
            if (pic_Image6 != null && !pic_Image6.IsDisposed)
            {
                if (pic_Image6.Image != null)
                    pic_Image6.Image.Dispose();
                pic_Image6.Dispose();
                pic_Image6 = null;
            }

            this.DialogResult = DialogResult.OK;

            Close();
            Dispose();
        }

        private void cbo_ImageNoSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (dgd_ImageList.RowCount < 1)
                return;

            dgd_ImageList.Enabled = false;

            ResizeImage(cbo_ImageNoSelection.SelectedIndex);

            dgd_ImageList.Enabled = true;
        }

        private void dgd_ImageList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (e.RowIndex >= 0)
            {
                m_intImageCount = Convert.ToInt32(dgd_ImageList.Rows[e.RowIndex].Cells[7].Value);
                ResizeImage(cbo_ImageNoSelection.SelectedIndex);

                if (dgd_ImageList.Rows[e.RowIndex].Cells[2].Value != null)
                {
                    if (dgd_ImageList.Rows[e.RowIndex].Cells[2].Value.ToString() != "")
                        pic_Image1.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex].Cells[2].Value.ToString());
                }
                else
                {
                    pic_Image1.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (dgd_ImageList.Rows[e.RowIndex].Cells[3].Value != null)
                {
                    if (dgd_ImageList.Rows[e.RowIndex].Cells[3].Value.ToString() != "")
                        pic_Image2.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex].Cells[3].Value.ToString());
                }
                else
                {
                    pic_Image2.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (dgd_ImageList.Rows[e.RowIndex].Cells[4].Value != null)
                {
                    if (dgd_ImageList.Rows[e.RowIndex].Cells[4].Value.ToString() != "")
                        pic_Image3.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex].Cells[4].Value.ToString());
                }
                else
                {
                    pic_Image3.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (dgd_ImageList.Rows[e.RowIndex].Cells[5].Value != null)
                {
                    if (dgd_ImageList.Rows[e.RowIndex].Cells[5].Value.ToString() != "")
                        pic_Image4.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex].Cells[5].Value.ToString());
                }
                else
                {
                    pic_Image4.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }

                if (dgd_ImageList.Rows[e.RowIndex].Cells[6].Value != null)
                {
                    if (dgd_ImageList.Rows[e.RowIndex].Cells[6].Value.ToString() != "")
                        pic_Image5.Image = Image.FromFile(dgd_ImageList.Rows[e.RowIndex].Cells[6].Value.ToString());
                }
                else
                {
                    pic_Image5.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
                }
            }
        }

        private void ResizeImage(int SelectedDisplayMethod)
        {
            switch (SelectedDisplayMethod)
            {
                case 0:
                    if (m_intImageCount == 1)
                    {
                        pic_Image1.Visible = true;
                        pnl_PictureBox1.Visible = true;
                        pic_Image2.Visible = false;
                        pnl_PictureBox2.Visible = false;
                        pic_Image3.Visible = false;
                        pnl_PictureBox3.Visible = false;
                        pic_Image4.Visible = false;
                        pnl_PictureBox4.Visible = false;
                        pic_Image5.Visible = false;
                        pnl_PictureBox5.Visible = false;
                        pic_Image6.Visible = false;
                        pnl_PictureBox6.Visible = false;

                        pic_Image1.Size = new Size((int)(480 * (360.0 / 480) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 360);
                        pnl_PictureBox1.Size = new Size(480, 360);
                    }
                    if (m_intImageCount == 2)
                    {
                        pic_Image1.Visible = true;
                        pnl_PictureBox1.Visible = true;
                        pic_Image2.Visible = true;
                        pnl_PictureBox2.Visible = true;
                        pic_Image3.Visible = false;
                        pnl_PictureBox3.Visible = false;
                        pic_Image4.Visible = false;
                        pnl_PictureBox4.Visible = false;
                        pic_Image5.Visible = false;
                        pnl_PictureBox5.Visible = false;
                        pic_Image6.Visible = false;
                        pnl_PictureBox6.Visible = false;

                        pic_Image1.Size = new Size((int)(240 * (180.0 / 240) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 180);
                        pnl_PictureBox1.Size = new Size(240, 180);
                        pic_Image2.Size = new Size((int)(240 * (180.0 / 240) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 180);
                        pnl_PictureBox2.Size = new Size(240, 180);
                        pnl_PictureBox2.Location = new Point(pnl_PictureBox1.Location.X + pnl_PictureBox1.Size.Width, pnl_PictureBox1.Location.Y);
                    }
                    if (m_intImageCount == 3)
                    {
                        pic_Image1.Visible = true;
                        pnl_PictureBox1.Visible = true;
                        pic_Image2.Visible = true;
                        pnl_PictureBox2.Visible = true;
                        pic_Image3.Visible = true;
                        pnl_PictureBox3.Visible = true;
                        pic_Image4.Visible = false;
                        pnl_PictureBox4.Visible = false;
                        pic_Image5.Visible = false;
                        pnl_PictureBox5.Visible = false;
                        pic_Image6.Visible = false;
                        pnl_PictureBox6.Visible = false;

                        pic_Image1.Size = new Size((int)(240 * (180.0 / 240) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 180);
                        pnl_PictureBox1.Size = new Size(240, 180);
                        pic_Image2.Size = new Size((int)(240 * (180.0 / 240) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 180);
                        pnl_PictureBox2.Size = new Size(240, 180);
                        pnl_PictureBox2.Location = new Point(pnl_PictureBox1.Location.X + pnl_PictureBox1.Size.Width, pnl_PictureBox1.Location.Y);
                        pic_Image3.Size = new Size((int)(240 * (180.0 / 240) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 180);
                        pnl_PictureBox3.Size = new Size(240, 180);
                        pnl_PictureBox3.Location = new Point(pnl_PictureBox1.Location.X, pnl_PictureBox1.Location.Y + pnl_PictureBox1.Size.Height);
                    }
                    if (m_intImageCount == 4)
                    {
                        pic_Image1.Visible = true;
                        pnl_PictureBox1.Visible = true;
                        pic_Image2.Visible = true;
                        pnl_PictureBox2.Visible = true;
                        pic_Image3.Visible = true;
                        pnl_PictureBox3.Visible = true;
                        pic_Image4.Visible = true;
                        pnl_PictureBox4.Visible = true;
                        pic_Image5.Visible = false;
                        pnl_PictureBox5.Visible = false;
                        pic_Image6.Visible = false;
                        pnl_PictureBox6.Visible = false;

                        pic_Image1.Size = new Size((int)(240 * (180.0 / 240) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 180);
                        pnl_PictureBox1.Size = new Size(240, 180);
                        pic_Image2.Size = new Size((int)(240 * (180.0 / 240) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 180);
                        pnl_PictureBox2.Size = new Size(240, 180);
                        pnl_PictureBox2.Location = new Point(pnl_PictureBox1.Location.X + pnl_PictureBox1.Size.Width, pnl_PictureBox1.Location.Y);
                        pic_Image3.Size = new Size((int)(240 * (180.0 / 240) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 180);
                        pnl_PictureBox3.Size = new Size(240, 180);
                        pnl_PictureBox3.Location = new Point(pnl_PictureBox1.Location.X, pnl_PictureBox1.Location.Y + pnl_PictureBox1.Size.Height);
                        pic_Image4.Size = new Size((int)(240 * (180.0 / 240) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 180);
                        pnl_PictureBox4.Size = new Size(240, 180);
                        pnl_PictureBox4.Location = new Point(pnl_PictureBox3.Location.X + pnl_PictureBox3.Size.Width, pnl_PictureBox3.Location.Y);
                    }
                    if (m_intImageCount == 5)
                    {
                        pic_Image1.Visible = true;
                        pnl_PictureBox1.Visible = true;
                        pic_Image2.Visible = true;
                        pnl_PictureBox2.Visible = true;
                        pic_Image3.Visible = true;
                        pnl_PictureBox3.Visible = true;
                        pic_Image4.Visible = true;
                        pnl_PictureBox4.Visible = true;
                        pic_Image5.Visible = true;
                        pnl_PictureBox5.Visible = true;
                        pic_Image6.Visible = true;
                        pnl_PictureBox6.Visible = true;

                        pic_Image1.Size = new Size((int)(160 * (120.0 / 160) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 120);
                        pnl_PictureBox1.Size = new Size(160, 120);
                        pic_Image2.Size = new Size((int)(160 * (120.0 / 160) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 120);
                        pnl_PictureBox2.Size = new Size(160, 120);
                        pnl_PictureBox2.Location = new Point(pnl_PictureBox1.Location.X + pnl_PictureBox1.Size.Width, pnl_PictureBox1.Location.Y);
                        pic_Image3.Size = new Size((int)(160 * (120.0 / 160) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 120);
                        pnl_PictureBox3.Size = new Size(160, 120);
                        pnl_PictureBox3.Location = new Point(pnl_PictureBox2.Location.X + pnl_PictureBox2.Size.Width, pnl_PictureBox2.Location.Y);

                        pic_Image4.Size = new Size((int)(160 * (120.0 / 160) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 120);
                        pnl_PictureBox4.Size = new Size(160, 120);
                        pnl_PictureBox4.Location = new Point(pnl_PictureBox1.Location.X, pnl_PictureBox1.Location.Y + pnl_PictureBox1.Size.Height);
                        pic_Image5.Size = new Size((int)(160 * (120.0 / 160) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 120);
                        pnl_PictureBox5.Size = new Size(160, 120);
                        pnl_PictureBox5.Location = new Point(pnl_PictureBox4.Location.X + pnl_PictureBox4.Size.Width, pnl_PictureBox4.Location.Y);
                    }
                    break;
                case 1:
                    pic_Image1.Size = new Size((int)(480 * (360.0 / 480) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 360);
                    pnl_PictureBox1.Size = new Size(480, 360);

                    pic_Image1.Visible = true;
                    pnl_PictureBox1.Visible = true;
                    pic_Image2.Visible = false;
                    pnl_PictureBox2.Visible = false;
                    pic_Image3.Visible = false;
                    pnl_PictureBox3.Visible = false;
                    pic_Image4.Visible = false;
                    pnl_PictureBox4.Visible = false;
                    pic_Image5.Visible = false;
                    pnl_PictureBox5.Visible = false;
                    pic_Image6.Visible = false;
                    pnl_PictureBox6.Visible = false;
                    break;
                case 2:
                    pic_Image2.Size = new Size((int)(480 * (360.0 / 480) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 360);
                    pnl_PictureBox2.Size = new Size(480, 360);
                    pnl_PictureBox2.Location = new Point(pnl_PictureBox1.Location.X, pnl_PictureBox1.Location.Y);

                    pic_Image1.Visible = false;
                    pnl_PictureBox1.Visible = false;
                    pic_Image2.Visible = true;
                    pnl_PictureBox2.Visible = true;
                    pic_Image3.Visible = false;
                    pnl_PictureBox3.Visible = false;
                    pic_Image4.Visible = false;
                    pnl_PictureBox4.Visible = false;
                    pic_Image5.Visible = false;
                    pnl_PictureBox5.Visible = false;
                    pic_Image6.Visible = false;
                    pnl_PictureBox6.Visible = false;
                    break;
                case 3:
                    pic_Image3.Size = new Size((int)(480 * (360.0 / 480) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 360);
                    pnl_PictureBox3.Size = new Size(480, 360);
                    pnl_PictureBox3.Location = new Point(pnl_PictureBox1.Location.X, pnl_PictureBox1.Location.Y);

                    pic_Image1.Visible = false;
                    pnl_PictureBox1.Visible = false;
                    pic_Image2.Visible = false;
                    pnl_PictureBox2.Visible = false;
                    pic_Image3.Visible = true;
                    pnl_PictureBox3.Visible = true;
                    pic_Image4.Visible = false;
                    pnl_PictureBox4.Visible = false;
                    pic_Image5.Visible = false;
                    pnl_PictureBox5.Visible = false;
                    pic_Image6.Visible = false;
                    pnl_PictureBox6.Visible = false;
                    break;
                case 4:
                    pic_Image4.Size = new Size((int)(480 * (360.0 / 480) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 360);
                    pnl_PictureBox4.Size = new Size(480, 360);
                    pnl_PictureBox4.Location = new Point(pnl_PictureBox1.Location.X, pnl_PictureBox1.Location.Y);

                    pic_Image1.Visible = false;
                    pnl_PictureBox1.Visible = false;
                    pic_Image2.Visible = false;
                    pnl_PictureBox2.Visible = false;
                    pic_Image3.Visible = false;
                    pnl_PictureBox3.Visible = false;
                    pic_Image4.Visible = true;
                    pnl_PictureBox4.Visible = true;
                    pic_Image5.Visible = false;
                    pnl_PictureBox5.Visible = false;
                    pic_Image6.Visible = false;
                    pnl_PictureBox6.Visible = false;
                    break;
                case 5:
                    pic_Image5.Size = new Size((int)(480 * (360.0 / 480) / ((float)m_smVisionInfo.g_intCameraResolutionHeight / m_smVisionInfo.g_intCameraResolutionWidth)), 360);
                    pnl_PictureBox5.Size = new Size(480, 360);
                    pnl_PictureBox5.Location = new Point(pnl_PictureBox1.Location.X, pnl_PictureBox1.Location.Y);

                    pic_Image1.Visible = false;
                    pnl_PictureBox1.Visible = false;
                    pic_Image2.Visible = false;
                    pnl_PictureBox2.Visible = false;
                    pic_Image3.Visible = false;
                    pnl_PictureBox3.Visible = false;
                    pic_Image4.Visible = false;
                    pnl_PictureBox4.Visible = false;
                    pic_Image5.Visible = true;
                    pnl_PictureBox5.Visible = true;
                    pic_Image6.Visible = false;
                    pnl_PictureBox6.Visible = false;
                    break;
            }
        }
    }
}
