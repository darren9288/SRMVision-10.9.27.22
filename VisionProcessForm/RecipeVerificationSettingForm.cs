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
using System.IO;
using VisionProcessing;
using System.Drawing.Imaging;

namespace VisionProcessForm
{
    public partial class RecipeVerificationSettingForm : Form
    {
        private int m_intUserGroup = 5;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private string m_strSelectedRecipe;
        private CustomOption m_smCustomizeInfo;
        private bool m_blnInitDone = false;
        private List<List<ImageDrawing>> m_arrImage = new List<List<ImageDrawing>>();
        private List<List<CImageDrawing>> m_arrColorImage = new List<List<CImageDrawing>>();
        private string m_strImagePath;
        private int m_intSelectedRowIndex = -1;
        private Graphics m_Graphic;
        private bool m_blnDrawFirstTime = true;

        public RecipeVerificationSettingForm(ProductionInfo smProductionInfo, VisionInfo smVisionInfo, CustomOption smCustomizeInfo, string strSelectedRecipe)
        {
            InitializeComponent();

            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            m_strSelectedRecipe = strSelectedRecipe;

            m_smCustomizeInfo = smCustomizeInfo;
            m_intUserGroup = m_smProductionInfo.g_intUserGroup;

            m_Graphic = Graphics.FromHwnd(pic_ImageNew.Handle);
            string strPath = smVisionInfo.g_strSaveImageLocation +
                     m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime;

            //string strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")";

            string strVisionImageFolderName;
            if (smVisionInfo.g_intVisionResetCount == 0)
                strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")" + "_" + m_smProductionInfo.g_strLotStartTime;
            else
                strVisionImageFolderName = smVisionInfo.g_strVisionFolderName + "(" + smVisionInfo.g_strVisionDisplayName + " " + smVisionInfo.g_strVisionNameRemark + ")" + "_" + smVisionInfo.g_strVisionResetCountTime;

            m_strImagePath = strPath + "\\" + strVisionImageFolderName;

            cbo_ViewImage.Items.Clear();
            cbo_ViewImage.Items.Add("Image 1");
            cbo_ViewImage.SelectedIndex = 0;
            for (int i = 1; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                cbo_ViewImage.Items.Add("Image " + (i + 1).ToString());
            }

            UpdateGUI();

            if (dgd_HandMade.RowCount > 0)
            {
                dgd_HandMade.Rows[0].Selected = true;
                dgd_HandMade.CurrentCell = dgd_HandMade.Rows[0].Cells[0];
            }

            if (dgd_SampleImage.RowCount > 0)
            {
                dgd_SampleImage.Rows[0].Selected = true;
                dgd_SampleImage.CurrentCell = dgd_SampleImage.Rows[0].Cells[0];
                m_intSelectedRowIndex = 0;
         
            }

            m_blnInitDone = true;
        }

        private void UpdateGUI()
        {
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "Orient":
                case "BottomOrient":

                    break;
                case "Mark":
                case "MarkOrient":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                case "MOLi":
               
                    dgd_HandMade.Rows.Add();
                    dgd_HandMade.Rows[dgd_HandMade.RowCount - 1].Cells[0].Value = dgd_HandMade.RowCount.ToString();
                    dgd_HandMade.Rows[dgd_HandMade.RowCount - 1].Cells[2].Value = "Excess Mark";
                    dgd_HandMade.Rows[dgd_HandMade.RowCount - 1].Cells[4].Value = (dgd_HandMade.Rows[dgd_HandMade.RowCount - 1].Cells[4] as DataGridViewComboBoxCell).Items[1];
                    dgd_HandMade.Rows[dgd_HandMade.RowCount - 1].Cells[6].Value = (m_smVisionInfo.g_arrMarks[0].ref_intFailOptionMask_ForPreTest & 0x01) > 0;
                   
                    dgd_HandMade.Rows.Add();
                    dgd_HandMade.Rows[dgd_HandMade.RowCount - 1].Cells[0].Value = dgd_HandMade.RowCount.ToString();
                    dgd_HandMade.Rows[dgd_HandMade.RowCount - 1].Cells[2].Value = "Extra Mark";
                    dgd_HandMade.Rows[dgd_HandMade.RowCount - 1].Cells[4].Value = (dgd_HandMade.Rows[dgd_HandMade.RowCount - 1].Cells[4] as DataGridViewComboBoxCell).Items[1];
                    dgd_HandMade.Rows[dgd_HandMade.RowCount - 1].Cells[6].Value = (m_smVisionInfo.g_arrMarks[0].ref_intFailOptionMask_ForPreTest & 0x02) > 0;
                    
                    dgd_HandMade.Rows.Add();
                    dgd_HandMade.Rows[dgd_HandMade.RowCount - 1].Cells[0].Value = dgd_HandMade.RowCount.ToString();
                    dgd_HandMade.Rows[dgd_HandMade.RowCount - 1].Cells[2].Value = "Missing Mark";
                    dgd_HandMade.Rows[dgd_HandMade.RowCount - 1].Cells[4].Value = (dgd_HandMade.Rows[dgd_HandMade.RowCount - 1].Cells[4] as DataGridViewComboBoxCell).Items[1];
                    dgd_HandMade.Rows[dgd_HandMade.RowCount - 1].Cells[6].Value = (m_smVisionInfo.g_arrMarks[0].ref_intFailOptionMask_ForPreTest & 0x10) > 0;

                    string strPath = "D:\\PreTest Image\\Recipe\\" + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
                    if (Directory.Exists(strPath))
                    {
                        string[] arrFiles = Directory.GetFiles(strPath, "*SampleImage*", SearchOption.AllDirectories);

                        LoadImages(arrFiles);

                        foreach (string dd in arrFiles)
                        {
                            if (dd.Contains("SampleImage") && !dd.Contains("_Image"))
                            {
                                dgd_SampleImage.Rows.Add();
                                dgd_SampleImage.Rows[dgd_SampleImage.RowCount - 1].Cells[0].Value = dgd_SampleImage.RowCount.ToString();
                                dgd_SampleImage.Rows[dgd_SampleImage.RowCount - 1].Cells[2].Value = dd;
                                dgd_SampleImage.Rows[dgd_SampleImage.RowCount - 1].Cells[4].Value = (dgd_SampleImage.Rows[dgd_SampleImage.RowCount - 1].Cells[4] as DataGridViewComboBoxCell).Items[m_smVisionInfo.g_arrPreTestExpectedResult[dgd_SampleImage.RowCount - 1]];
                                dgd_SampleImage.Rows[dgd_SampleImage.RowCount - 1].Cells[6].Value = m_smVisionInfo.g_arrPreTestInspect[dgd_SampleImage.RowCount - 1];
                            }
                        }

                    }
                    break;
                case "Package":

                    break;
                case "UnitPresent":
                case "BottomPosition":
                case "BottomPositionOrient":
                case "TapePocketPosition":

                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "PadPos":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":

                    break;
                case "Li3D":
                case "Li3DPkg":

                    break;
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                case "IPMLi":
                case "IPMLiPkg":

                    break;
                case "Seal":

                    break;
                case "Barcode":

                    break;
                default:
                    SRMMessageBox.Show("VisionPage() --> There is no such vision module name " + m_smVisionInfo.g_strVisionName + " in this SRMVision software version.");
                    break;

            }
        }

        private void SaveMarkSetting()
        {
            if (Convert.ToBoolean(dgd_HandMade.Rows[0].Cells[6].Value))
                m_smVisionInfo.g_arrMarks[0].ref_intFailOptionMask_ForPreTest |= 0x01;
            else
                m_smVisionInfo.g_arrMarks[0].ref_intFailOptionMask_ForPreTest &= ~0x01;

            if (Convert.ToBoolean(dgd_HandMade.Rows[1].Cells[6].Value))
                m_smVisionInfo.g_arrMarks[0].ref_intFailOptionMask_ForPreTest |= 0x02;
            else
                m_smVisionInfo.g_arrMarks[0].ref_intFailOptionMask_ForPreTest &= ~0x02;

            if (Convert.ToBoolean(dgd_HandMade.Rows[2].Cells[6].Value))
                m_smVisionInfo.g_arrMarks[0].ref_intFailOptionMask_ForPreTest |= 0x10;
            else
                m_smVisionInfo.g_arrMarks[0].ref_intFailOptionMask_ForPreTest &= ~0x10;

            m_smVisionInfo.g_arrMarks[0].SaveTemplate(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + "Mark\\Template\\", false);




            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                 m_smVisionInfo.g_strVisionFolderName + "\\General.xml";

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("PreTest");

            for (int i = 0; i < dgd_SampleImage.RowCount; i++)
            {
                int intExpectedResult = (dgd_SampleImage.Rows[i].Cells[4] as DataGridViewComboBoxCell).Items.IndexOf(dgd_SampleImage.Rows[i].Cells[4].Value);
                objFileHandle.WriteElement1Value("PreTestExpectedResult" + (i), intExpectedResult);

                bool blnInspect = Convert.ToBoolean(dgd_SampleImage.Rows[i].Cells[6].Value);
                objFileHandle.WriteElement1Value("PreTestInspect" + (i), blnInspect);
                
                m_smVisionInfo.g_arrPreTestExpectedResult[i] = intExpectedResult;
                m_smVisionInfo.g_arrPreTestInspect[i] = blnInspect;
            }
            objFileHandle.WriteEndElement();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            if (dgd_SampleImage.RowCount < 20)
            {
                dlg_ImageFile.Reset();
                dlg_ImageFile.InitialDirectory = m_strImagePath;

                if (dlg_ImageFile.ShowDialog() == DialogResult.OK)
                {
                    string strFileName = dlg_ImageFile.FileName;
                    m_strImagePath = Path.GetDirectoryName(strFileName);

                    bool blnValidImage = false;
                    LoadImages(strFileName, dgd_SampleImage.RowCount, ref blnValidImage);

                    if (blnValidImage)
                    {
                        string strDir = Path.GetDirectoryName(strFileName);
                        string strName = Path.GetFileNameWithoutExtension(strFileName);
                        string strExt = Path.GetExtension(strFileName);

                        if (strName.IndexOf("_Image") >= 0)
                        {
                            strFileName = strDir + "\\" + strName.ToString().Substring(0, strName.IndexOf("_Image")) + strExt;
                        }

                        dgd_SampleImage.Rows.Add();
                        dgd_SampleImage.Rows[dgd_SampleImage.RowCount - 1].Cells[0].Value = dgd_SampleImage.RowCount.ToString();
                        dgd_SampleImage.Rows[dgd_SampleImage.RowCount - 1].Cells[2].Value = strFileName;
                        dgd_SampleImage.Rows[dgd_SampleImage.RowCount - 1].Cells[4].Value = (dgd_SampleImage.Rows[dgd_SampleImage.RowCount - 1].Cells[4] as DataGridViewComboBoxCell).Items[0];
                        dgd_SampleImage.Rows[dgd_SampleImage.RowCount - 1].Cells[6].Value = false;

                        dgd_SampleImage.Rows[dgd_SampleImage.RowCount - 1].Selected = true;
                        dgd_SampleImage.CurrentCell = dgd_SampleImage.Rows[dgd_SampleImage.RowCount - 1].Cells[0];
                        m_intSelectedRowIndex = dgd_SampleImage.RowCount - 1;

                        UpdatePictureBox();
                    }
                }
            }
            else
            {
                SRMMessageBox.Show("Maximum of 20 images can be added only.");
            }
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            if (dgd_SampleImage.RowCount > 0 && m_intSelectedRowIndex >= 0)
            {
                if (dgd_SampleImage.RowCount > m_intSelectedRowIndex)
                {
                    dgd_SampleImage.Rows.RemoveAt(m_intSelectedRowIndex);

                    for (int i = 0; i < dgd_SampleImage.RowCount; i++)
                    {
                        dgd_SampleImage.Rows[i].Cells[0].Value = (i + 1).ToString();
                    }
                }

                if (m_smVisionInfo.g_blnViewColorImage && m_arrColorImage.Count > m_intSelectedRowIndex)
                    m_arrColorImage.RemoveAt(m_intSelectedRowIndex);

                if (m_arrImage.Count > m_intSelectedRowIndex)
                    m_arrImage.RemoveAt(m_intSelectedRowIndex);

                if (m_smVisionInfo.g_arrPreTestExpectedResult.Count > m_intSelectedRowIndex)
                {
                    m_smVisionInfo.g_arrPreTestExpectedResult.RemoveAt(m_intSelectedRowIndex);
                    m_smVisionInfo.g_arrPreTestExpectedResult.Add(0);
                }

                if (m_smVisionInfo.g_arrPreTestInspect.Count > m_intSelectedRowIndex)
                {
                    m_smVisionInfo.g_arrPreTestInspect.RemoveAt(m_intSelectedRowIndex);
                    m_smVisionInfo.g_arrPreTestInspect.Add(false);
                }

                if (m_intSelectedRowIndex >= dgd_SampleImage.RowCount)
                {
                    m_intSelectedRowIndex = dgd_SampleImage.RowCount - 1;
                    if (m_intSelectedRowIndex >= 0 && dgd_SampleImage.RowCount > m_intSelectedRowIndex)
                    {
                        dgd_SampleImage.Rows[m_intSelectedRowIndex].Selected = true;
                        dgd_SampleImage.CurrentCell = dgd_SampleImage.Rows[m_intSelectedRowIndex].Cells[0];

                        UpdatePictureBox();
                    }
                }
            }
            else
            {
                m_intSelectedRowIndex = -1;
                SRMMessageBox.Show("Sample Image List is empty.");
            }
        }

        private void LoadImages(string[] arrFileName)
        {
            int i = 0;

            string[] strImageFiles = arrFileName;

            int intTotalLoadImageLimit = strImageFiles.Length;

            long longFirstValidFileLength = 0;
            foreach (string strName in strImageFiles)
            {
                if (strName.IndexOf("_Image") >= 0)
                    continue;

                try
                {
                    bool blnValidImage = false;
                    if (longFirstValidFileLength == 0)
                    {
                        Image objImage = Image.FromFile(strName);

                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            if (objImage.Width == m_smVisionInfo.g_intCameraResolutionWidth && objImage.Height == m_smVisionInfo.g_intCameraResolutionHeight)
                            {
                                blnValidImage = true;
                            }
                        }
                        else
                        {
                            // 2019 04 12-CCENG: if loaded image is PixelFormat.Format24bppRgb (color image), Euresys will auto convert it to mono color during loading image.
                            if ((PixelFormat.Format8bppIndexed == objImage.PixelFormat || PixelFormat.Format24bppRgb == objImage.PixelFormat) &&
                                objImage.Width == m_smVisionInfo.g_intCameraResolutionWidth &&
                                objImage.Height == m_smVisionInfo.g_intCameraResolutionHeight)
                            {
                                blnValidImage = true;
                            }
                        }

                        objImage.Dispose();
                    }
                    else
                    {
                        long fileLength = new System.IO.FileInfo(strName).Length;
                        if (longFirstValidFileLength == fileLength)
                        {
                            blnValidImage = true;
                        }
                    }
                    if (blnValidImage)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                            m_arrColorImage.Add(new List<CImageDrawing>());

                        m_arrImage.Add(new List<ImageDrawing>());

                        if (longFirstValidFileLength == 0)
                        {
                            longFirstValidFileLength = new System.IO.FileInfo(strName).Length;
                        }

                        if (!m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrImage[i].Add(new ImageDrawing());
                            m_arrImage[i][0].LoadImage(strName);

                            //m_arrImage[i][0].SaveImage("D:\\TS\\m_arrImage" + i.ToString() + "0.bmp");

                            for (int x = 1; x < m_smVisionInfo.g_arrImages.Count; x++)
                            {
                                string strDirPath = Path.GetDirectoryName(strName);
                                string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strName) + "_Image" + x.ToString() + ".BMP";

                                m_arrImage[i].Add(new ImageDrawing());
                                if (File.Exists(strPkgView))
                                    m_arrImage[i][x].LoadImage(strPkgView);
                                else
                                    m_arrImage[i][x].LoadImage(strName);

                                //m_arrImage[i][0].SaveImage("D:\\TS\\m_arrImage" + i.ToString() + x.ToString() + ".bmp");
                            }
                            i++;
                        }
                        else if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrColorImage[i].Add(new CImageDrawing());
                            m_arrColorImage[i][0].LoadImage(strName);

                            //m_arrColorImage[i][0].SaveImage("D:\\TS\\m_arrColorImage" + i.ToString() + "0.bmp");

                            m_arrImage[i].Add(new ImageDrawing());
                            m_arrColorImage[i][0].ConvertColorToMono(ref m_arrImage, 0, i);

                            //m_arrImage[i][0].SaveImage("D:\\TS\\m_arrImage" + i.ToString() + "0.bmp");

                            for (int x = 1; x < m_smVisionInfo.g_arrColorImages.Count; x++)
                            {
                                string strDirPath = Path.GetDirectoryName(strName);
                                string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strName) + "_Image" + x.ToString() + ".BMP";

                                m_arrColorImage[i].Add(new CImageDrawing());
                                if (File.Exists(strPkgView))
                                    m_arrColorImage[i][x].LoadImage(strPkgView);
                                else
                                    m_arrColorImage[i][x].LoadImage(strName);

                                //m_arrColorImage[i][x].SaveImage("D:\\TS\\m_arrColorImage" + i.ToString() + x.ToString() + ".bmp");

                                m_arrImage[i].Add(new ImageDrawing());
                                m_arrColorImage[i][x].ConvertColorToMono(ref m_arrImage, x, i);

                                //m_arrImage[i][x].SaveImage("D:\\TS\\m_arrImage" + i.ToString() + x.ToString() + ".bmp");

                            }
                            i++;
                        }

                        if (i >= intTotalLoadImageLimit)
                            break;
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        private void LoadImages(string strFileName, int intIndex, ref bool blnValidImage)
        {
            long longFirstValidFileLength = 0;

            string strDir = Path.GetDirectoryName(strFileName);
            string strName = Path.GetFileNameWithoutExtension(strFileName);
            string strExt = Path.GetExtension(strFileName);

            if (strName.IndexOf("_Image") >= 0)
            {
                strFileName = strDir + "\\" + strName.ToString().Substring(0, strName.IndexOf("_Image")) + strExt;
            }

            try
            {
                blnValidImage = false;
                if (longFirstValidFileLength == 0)
                {
                    Image objImage = Image.FromFile(strFileName);

                    if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        if (objImage.Width == m_smVisionInfo.g_intCameraResolutionWidth && objImage.Height == m_smVisionInfo.g_intCameraResolutionHeight)
                        {
                            blnValidImage = true;
                        }
                    }
                    else
                    {
                        // 2019 04 12-CCENG: if loaded image is PixelFormat.Format24bppRgb (color image), Euresys will auto convert it to mono color during loading image.
                        if ((PixelFormat.Format8bppIndexed == objImage.PixelFormat || PixelFormat.Format24bppRgb == objImage.PixelFormat) &&
                            objImage.Width == m_smVisionInfo.g_intCameraResolutionWidth &&
                            objImage.Height == m_smVisionInfo.g_intCameraResolutionHeight)
                        {
                            blnValidImage = true;
                        }
                    }

                    objImage.Dispose();
                }
                else
                {
                    long fileLength = new System.IO.FileInfo(strFileName).Length;
                    if (longFirstValidFileLength == fileLength)
                    {
                        blnValidImage = true;
                    }
                }
                if (blnValidImage)
                {
                    if (intIndex >= m_arrColorImage.Count)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                            m_arrColorImage.Add(new List<CImageDrawing>());
                    }

                    if (intIndex >= m_arrImage.Count)
                        m_arrImage.Add(new List<ImageDrawing>());

                    if (longFirstValidFileLength == 0)
                    {
                        longFirstValidFileLength = new System.IO.FileInfo(strFileName).Length;
                    }

                    if (!m_smVisionInfo.g_blnViewColorImage)
                    {
                        m_arrImage[intIndex].Add(new ImageDrawing());
                        m_arrImage[intIndex][0].LoadImage(strFileName);

                        //m_arrImage[i][0].SaveImage("D:\\TS\\m_arrImage" + i.ToString() + "0.bmp");

                        for (int x = 1; x < m_smVisionInfo.g_arrImages.Count; x++)
                        {
                            string strDirPath = Path.GetDirectoryName(strFileName);
                            string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + x.ToString() + ".BMP";

                            m_arrImage[intIndex].Add(new ImageDrawing());
                            if (File.Exists(strPkgView))
                                m_arrImage[intIndex][x].LoadImage(strPkgView);
                            else
                                m_arrImage[intIndex][x].LoadImage(strFileName);

                            //m_arrImage[i][0].SaveImage("D:\\TS\\m_arrImage" + i.ToString() + x.ToString() + ".bmp");
                        }
                    }
                    else if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        m_arrColorImage[intIndex].Add(new CImageDrawing());
                        m_arrColorImage[intIndex][0].LoadImage(strFileName);

                        //m_arrColorImage[i][0].SaveImage("D:\\TS\\m_arrColorImage" + i.ToString() + "0.bmp");

                        m_arrImage[intIndex].Add(new ImageDrawing());
                        m_arrColorImage[intIndex][0].ConvertColorToMono(ref m_arrImage, 0, intIndex);

                        //m_arrImage[i][0].SaveImage("D:\\TS\\m_arrImage" + i.ToString() + "0.bmp");

                        for (int x = 1; x < m_smVisionInfo.g_arrColorImages.Count; x++)
                        {
                            string strDirPath = Path.GetDirectoryName(strFileName);
                            string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + x.ToString() + ".BMP";

                            m_arrColorImage[intIndex].Add(new CImageDrawing());
                            if (File.Exists(strPkgView))
                                m_arrColorImage[intIndex][x].LoadImage(strPkgView);
                            else
                                m_arrColorImage[intIndex][x].LoadImage(strFileName);

                            //m_arrColorImage[i][x].SaveImage("D:\\TS\\m_arrColorImage" + i.ToString() + x.ToString() + ".bmp");

                            m_arrImage[intIndex].Add(new ImageDrawing());
                            m_arrColorImage[intIndex][x].ConvertColorToMono(ref m_arrImage, x, intIndex);

                            //m_arrImage[i][x].SaveImage("D:\\TS\\m_arrImage" + i.ToString() + x.ToString() + ".bmp");

                        }
                    }

                }
            }
            catch
            {

            }
        }

        private void dgd_SampleImage_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            m_intSelectedRowIndex = e.RowIndex;

            UpdatePictureBox();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            LoadPreTestSetting(strFolderPath + "General.xml");

            this.Close();
            this.Dispose();
        }
        private void LoadPreTestSetting(string strPath)
        {
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection("PreTest");
            for (int i = 0; i < m_smVisionInfo.g_arrPreTestExpectedResult.Count; i++)
            {
                m_smVisionInfo.g_arrPreTestExpectedResult[i] = objFile.GetValueAsInt("PreTestExpectedResult" + i.ToString(), 0);
                m_smVisionInfo.g_arrPreTestInspect[i] = objFile.GetValueAsBoolean("PreTestInspect" + i.ToString(), false);
            }
        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "Orient":
                case "BottomOrient":

                    break;
                case "Mark":
                case "MarkOrient":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                case "MOLi":
                    SaveMarkSetting();
                    break;
                case "Package":

                    break;
                case "UnitPresent":
                case "BottomPosition":
                case "BottomPositionOrient":
                case "TapePocketPosition":

                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "PadPos":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":

                    break;
                case "Li3D":
                case "Li3DPkg":

                    break;
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                case "IPMLi":
                case "IPMLiPkg":

                    break;
                case "Seal":

                    break;
                case "Barcode":

                    break;
                default:
              
                    break;

            }

            SaveImage();

            this.Close();
            this.Dispose();
        }

        private void SaveImage()
        {
            string strPath = "D:\\PreTest Image\\Recipe\\" + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);

            string[] arrFiles = Directory.GetFiles(strPath, "*SampleImage*", SearchOption.AllDirectories);
            for (int i = 0; i < arrFiles.Length; i++)
            {
                File.Delete(arrFiles[i]);
            }

            if (m_smVisionInfo.g_blnViewColorImage)
            {
                for (int i = 0; i < m_arrColorImage.Count; i++)
                {
                    if (m_arrColorImage[i].Count > 0)
                    {
                        m_arrColorImage[i][0].SaveImage(strPath + "SampleImage" + i.ToString() + ".bmp");
                    }
                    
                    for (int j = 1; j < m_arrColorImage[i].Count; j++)
                    {
                        if (!WantSaveImageAccordingMergeType(j))
                            continue;

                        m_arrColorImage[i][j].SaveImage(strPath + "SampleImage" + i.ToString() + "_Image" + j.ToString() + ".bmp");
                    }
                }
            }
            else
            {

            }
        }
        private bool WantSaveImageAccordingMergeType(int intImageIndex)
        {
            switch (m_smVisionInfo.g_intImageMergeType)
            {
                case 0:
                    return true;
                    break;
                case 1:
                    if (intImageIndex == 1)
                        return false;
                    break;
                case 2:
                    if ((intImageIndex == 1) || (intImageIndex == 2))
                        return false;
                    break;
                case 3:
                    if ((intImageIndex == 1) || (intImageIndex == 3))
                        return false;
                    break;
                case 4:
                    if ((intImageIndex == 1) || (intImageIndex == 2) || (intImageIndex == 4))
                        return false;
                    break;
            }

            return true;
        }

        private void cbo_ViewImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            UpdatePictureBox();

        }

        private void UpdatePictureBox()
        {
            if (m_intSelectedRowIndex < 0 || cbo_ViewImage.SelectedIndex < 0)
                return;

            //string strPath = dgd_SampleImage.Rows[m_intSelectedRowIndex].Cells[2].Value.ToString();

            //if (cbo_ViewImage.SelectedIndex > 0)
            //{
            //    string strDir = Path.GetDirectoryName(strPath);
            //    string strName = Path.GetFileNameWithoutExtension(strPath);
            //    string strExt = Path.GetExtension(strPath);

            //    strPath = strDir + "\\" + strName + "_Image" + cbo_ViewImage.SelectedIndex.ToString() + strExt;
            //}
          
            //if (File.Exists(strPath))
            //{
            if (m_smVisionInfo.g_blnViewColorImage)
            {
                if (m_arrColorImage.Count > m_intSelectedRowIndex && m_arrColorImage[m_intSelectedRowIndex].Count > cbo_ViewImage.SelectedIndex)
                    m_arrColorImage[m_intSelectedRowIndex][cbo_ViewImage.SelectedIndex].RedrawImage(m_Graphic, 200f / m_arrColorImage[m_intSelectedRowIndex][cbo_ViewImage.SelectedIndex].ref_intImageWidth, 200f / m_arrColorImage[m_intSelectedRowIndex][cbo_ViewImage.SelectedIndex].ref_intImageHeight);
                else
                    pic_ImageNew.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
            }
            else
            {
                if (m_arrImage.Count > m_intSelectedRowIndex && m_arrImage[m_intSelectedRowIndex].Count > cbo_ViewImage.SelectedIndex)
                    m_arrImage[m_intSelectedRowIndex][cbo_ViewImage.SelectedIndex].RedrawImage(m_Graphic, 200f / m_arrImage[m_intSelectedRowIndex][cbo_ViewImage.SelectedIndex].ref_intImageWidth, 200f / m_arrImage[m_intSelectedRowIndex][cbo_ViewImage.SelectedIndex].ref_intImageHeight);
                else
                    pic_ImageNew.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
            }
            //}
            //else
            //{
            //    pic_ImageNew.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
            //}

            lbl_ImageName.Text = Path.GetFileName(dgd_SampleImage.Rows[m_intSelectedRowIndex].Cells[2].Value.ToString());
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_blnInitDone && m_blnDrawFirstTime)
            {
                m_blnDrawFirstTime = false;

                UpdatePictureBox();
            }
        }
    }
}
