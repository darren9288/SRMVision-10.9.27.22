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
using Microsoft.Win32;
using Common;
using SharedMemory;

namespace AutoMode
{
    public partial class PreviousLotImageFolderList : Form
    {
        #region Member Variables

        string m_strSaveImagePath;
        string m_strVisionImageFolderName;
        string[] m_strLotFolder;
        string[] m_strImageFolder;
        string m_strSelectedImagePath;
        List<string[]> m_ImageQuantityFolder = new List<string[]>();
        #endregion

        #region Properties

        public string ref_strSelectedFolderPath { get { return m_strSelectedImagePath; } }
        #endregion

        public PreviousLotImageFolderList(string strSaveImagePath, string strVisionImageFolderName)
        {
            InitializeComponent();

            m_strSaveImagePath = strSaveImagePath;
            m_strVisionImageFolderName = strVisionImageFolderName;

            if (m_strSaveImagePath != "")
            {
                if (Directory.Exists(m_strSaveImagePath))
                {
                    m_strLotFolder = Directory.GetDirectories(m_strSaveImagePath);
                }
            }
            UpdateLotAvailable();
            UpdateFolderAvailable(-1);
        }

        /// <summary>
        /// Display all the available image folder
        /// </summary>
        private void UpdateLotAvailable()
        {
            dgd_ImageFolderList.Rows.Clear();
            for (int i = 0; i < m_strLotFolder.Length; i++)
            {
                dgd_ImageFolderList.Rows.Add();
                dgd_ImageFolderList.Rows[i].Cells[0].Value = Path.GetFileNameWithoutExtension(m_strLotFolder[i]);
                dgd_ImageFolderList.Rows[i].Cells[1].Value = File.GetCreationTime(m_strLotFolder[i]).ToString("yy/MM/dd HH:mm:ss");
                dgd_ImageFolderList.Rows[i].Cells[2].Value = m_strLotFolder[i];
            }

            RowComparer rc = new RowComparer(SortOrder.Descending, 1);
            dgd_ImageFolderList.Sort(rc);

            dgd_ImageFolderList.Rows[0].Selected = true;
        }

        /// <summary>
        /// Display all the available image folder
        /// </summary>
        private void UpdateFolderAvailable(int index)
        {
            if (index == -1)
            {
                if (Directory.Exists(dgd_ImageFolderList.Rows[dgd_ImageFolderList.SelectedRows[0].Index].Cells[2].Value.ToString()))
                {
                    m_strImageFolder = Directory.GetDirectories(dgd_ImageFolderList.Rows[dgd_ImageFolderList.SelectedRows[0].Index].Cells[2].Value.ToString());
                }
            }
            else
            {
                if (Directory.Exists(dgd_ImageFolderList.Rows[index].Cells[2].Value.ToString()))
                {
                    m_strImageFolder = Directory.GetDirectories(dgd_ImageFolderList.Rows[index].Cells[2].Value.ToString());
                }
            }

            lst_ImageFolderAvailable.Items.Clear();
            m_ImageQuantityFolder.Clear();

            if (m_strImageFolder.Length != 0)
            {
                foreach (string strDirectoryName in m_strImageFolder)
                {
                    if (strDirectoryName.Contains(m_strVisionImageFolderName))
                    {

                        lst_ImageFolderAvailable.Items.Add(Path.GetFileNameWithoutExtension(strDirectoryName));
                        m_ImageQuantityFolder.Add(Directory.GetDirectories(strDirectoryName));
                    }
                }
            }

            if (lst_ImageFolderAvailable.Items.Count == 0)
                lst_ImageFolderAvailable.Items.Add("Image Folder Not Available!");

            lst_ImageFolderAvailable.SelectedIndex = 0;
            FillImageFolder();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (lst_ImageFolderAvailable.SelectedIndex == -1)
            {
                SRMMessageBox.Show("Please Select A Folder to Load!", "Previous Lot Image Folder List", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (lst_ImageFolderAvailable.Text == "Image Folder Not Available!")
            {
                SRMMessageBox.Show("Image Folder Not Available!", "Previous Lot Image Folder List", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            m_strSelectedImagePath = dgd_ImageFolderList.Rows[dgd_ImageFolderList.SelectedRows[0].Index].Cells[2].Value.ToString() + "\\" + lst_ImageFolderAvailable.Text;
            string[] strImageFiles = Directory.GetFiles(m_strSelectedImagePath, "*.bmp", SearchOption.AllDirectories);

            if (strImageFiles.Length == 0)
            {
                SRMMessageBox.Show("No Image In Selected Folder!", "Previous Lot Image Folder List", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
            Dispose();
        }

        private void dgd_ImageFolderList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            UpdateFolderAvailable(-1);
        }

        private void lst_ImageFolderAvailable_MouseClick(object sender, MouseEventArgs e)
        {
            FillImageFolder();
        }
        private void FillImageFolder()
        {
            dgd_ImageQuantity.Rows.Clear();
            int[] b = new int[20];
            int j = 0;

            if (m_ImageQuantityFolder.Count == 0)
                return;

            if (lst_ImageFolderAvailable.SelectedIndex != -1)
            {
                foreach (string filename in m_ImageQuantityFolder[lst_ImageFolderAvailable.SelectedIndex])
                {
                    string[] temp = Directory.GetFiles(filename, "*.bmp");

                    for (int k = temp.Length - 1; k >= 0; k--)
                    {
                        if (temp.Length == 0)
                            break;

                        if (temp[k].Contains("Image3"))
                        {
                            temp = Remove(temp, k);
                            continue;
                        }

                        if (temp[k].Contains("Image2"))
                        {
                            temp = Remove(temp, k);
                            continue;
                        }

                        if (temp[k].Contains("Image1"))
                        {
                            temp = Remove(temp, k);
                            continue;
                        }
                    }
                    b[j] = temp.Length;
                    j++;
                }
            }
            for (int i = 0; i < m_ImageQuantityFolder[lst_ImageFolderAvailable.SelectedIndex].Length; i++)
            {
                dgd_ImageQuantity.Rows.Add();
                string temp2 = Path.GetFileName(m_ImageQuantityFolder[lst_ImageFolderAvailable.SelectedIndex][i]);
                dgd_ImageQuantity.Rows[i].Cells[0].Value = temp2;
                dgd_ImageQuantity.Rows[i].Cells[1].Value = b[i];
            }
        }

        private string[] Remove(string[] Array, int RemoveAt)
        {
            string[] newArray = new string[Array.Length - 1];

            int i = 0;
            int j = 0;
            while (i < Array.Length)
            {
                if (i != RemoveAt)
                {
                    newArray[j] = Array[i];
                    j++;
                }

                i++;
            }

            return newArray;
        }

        private void dgd_ImageFolderList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up) 
            {
                if (dgd_ImageFolderList.SelectedRows[0].Index - 1 < 0)
                    UpdateFolderAvailable(0);
                else
                UpdateFolderAvailable(dgd_ImageFolderList.SelectedRows[0].Index - 1);
            }
           else if(e.KeyCode == Keys.Down)
            {
                if(dgd_ImageFolderList.SelectedRows[0].Index + 1 >= dgd_ImageFolderList.Rows.Count)
                    UpdateFolderAvailable(dgd_ImageFolderList.Rows.Count - 1);
                else
                    UpdateFolderAvailable(dgd_ImageFolderList.SelectedRows[0].Index  + 1);
            }
        }
    }
}
