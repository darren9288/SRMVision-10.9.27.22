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

namespace AutoMode
{
    public partial class RecipeVerificationForm : Form
    {
        private VisionInfo[] m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private string m_strSelectedRecipe;
        private CustomOption m_smCustomizeInfo;
        private bool m_blnInitDone = false;
        private List<List<ImageDrawing>> m_arrImage = new List<List<ImageDrawing>>();
        private List<List<CImageDrawing>> m_arrColorImage = new List<List<CImageDrawing>>();
        private string m_strImagePath;
        private int m_intSelectedRowIndex = -1;
        private Graphics m_Graphic;
        private bool m_blnDrawFirstTime = false;
        private int m_intSelectedVision = 0;
        private List<string> m_arrRowErrorMessage = new List<string>();
        private List<Color> m_arrRowColor = new List<Color>();
        private ImageDrawing m_objImage = new ImageDrawing();
        private CImageDrawing m_objCImage = new CImageDrawing();

        public RecipeVerificationForm(ProductionInfo smProductionInfo, VisionInfo[] smVisionInfo, CustomOption smCustomizeInfo, int intSelectedVision)
        {
            InitializeComponent();

            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            
            m_smCustomizeInfo = smCustomizeInfo;

            if (intSelectedVision < 0)
                m_intSelectedVision = 0;
            else
                m_intSelectedVision = intSelectedVision;

            m_strSelectedRecipe = m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo[m_intSelectedVision].g_intVisionIndex];

            cbo_VisionModule.Items.Clear();
            for (int i = 0; i < m_smVisionInfo.Length; i++)
            {
                if (m_smVisionInfo[i] != null)
                    cbo_VisionModule.Items.Add(m_smVisionInfo[i].g_strVisionDisplayName.ToString());
            }

            if (cbo_VisionModule.Items.Count > m_intSelectedVision)
                cbo_VisionModule.SelectedIndex = m_intSelectedVision;

            m_Graphic = Graphics.FromHwnd(pic_Image.Handle);
            
            m_strImagePath = "D:\\PreTest Image\\Recipe\\" + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo[m_intSelectedVision].g_intVisionIndex] + "\\" + m_smVisionInfo[m_intSelectedVision].g_strVisionFolderName + "\\";

            UpdateGUI();

            cbo_ViewImage.Items.Clear();
            cbo_ViewImage.Items.Add("Image 1");
            cbo_ViewImage.SelectedIndex = 0;
            for (int i = 1; i < m_smVisionInfo[m_intSelectedVision].g_arrImages.Count; i++)
            {
                cbo_ViewImage.Items.Add("Image " + (i + 1).ToString());
            }

            UpdateDetails();

            m_blnInitDone = true;
        }
        private void UpdateGUI()
        {
            dgd_Result.Rows.Clear();

            switch (m_smVisionInfo[m_intSelectedVision].g_strVisionName)
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
                    if ((m_smVisionInfo[m_intSelectedVision].g_arrMarks[0].ref_intFailOptionMask_ForPreTest & 0x01) > 0)
                    {
                        dgd_Result.Rows.Add();
                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[0].Value = "ExcessMark";
                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[2].Value = "Fail";
                        if ((m_smVisionInfo[m_intSelectedVision].g_arrMarks[0].ref_intFailResultMask_ForPreTest & 0x01) > 0)
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[4].Value = "Fail";
                        else
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[4].Value = "Pass";
                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Value = GetFinalResult(dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[2].Value.ToString(), dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[4].Value.ToString());
                        if (dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Value.ToString() == "Pass")
                        {
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Style.BackColor = Color.Lime;
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Style.SelectionBackColor = Color.Lime;
                        }
                        else
                        {
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Style.BackColor = Color.Red;
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Style.SelectionBackColor = Color.Red;
                        }
                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[8].Value = "Details";
                    }

                    if ((m_smVisionInfo[m_intSelectedVision].g_arrMarks[0].ref_intFailOptionMask_ForPreTest & 0x02) > 0)
                    {
                        dgd_Result.Rows.Add();
                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[0].Value = "ExtraMark";
                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[2].Value = "Fail";
                        if ((m_smVisionInfo[m_intSelectedVision].g_arrMarks[0].ref_intFailResultMask_ForPreTest & 0x02) > 0)
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[4].Value = "Fail";
                        else
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[4].Value = "Pass";
                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Value = GetFinalResult(dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[2].Value.ToString(), dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[4].Value.ToString());
                        if (dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Value.ToString() == "Pass")
                        {
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Style.BackColor = Color.Lime;
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Style.SelectionBackColor = Color.Lime;
                        }
                        else
                        {
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Style.BackColor = Color.Red;
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Style.SelectionBackColor = Color.Red;
                        }
                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[8].Value = "Details";
                    }

                    if ((m_smVisionInfo[m_intSelectedVision].g_arrMarks[0].ref_intFailOptionMask_ForPreTest & 0x10) > 0)
                    {
                        dgd_Result.Rows.Add();
                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[0].Value = "MissingMark";
                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[2].Value = "Fail";
                        if ((m_smVisionInfo[m_intSelectedVision].g_arrMarks[0].ref_intFailResultMask_ForPreTest & 0x10) > 0)
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[4].Value = "Fail";
                        else
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[4].Value = "Pass";
                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Value = GetFinalResult(dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[2].Value.ToString(), dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[4].Value.ToString());
                        if (dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Value.ToString() == "Pass")
                        {
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Style.BackColor = Color.Lime;
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Style.SelectionBackColor = Color.Lime;
                        }
                        else
                        {
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Style.BackColor = Color.Red;
                            dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Style.SelectionBackColor = Color.Red;
                        }
                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[8].Value = "Details";
                    }
                    
                    if (Directory.Exists(m_strImagePath))
                    {
                        string[] arrFiles = Directory.GetFiles(m_strImagePath, "*SampleImage*", SearchOption.AllDirectories);
                        
                        foreach (string dd in arrFiles)
                        {
                            if (dd.Contains("SampleImage") && !dd.Contains("_Image"))
                            {
                                string strName = Path.GetFileNameWithoutExtension(dd);
                                int intIndex = Convert.ToInt32(strName.Substring(strName.LastIndexOf('e') + 1, strName.Length - strName.LastIndexOf('e') - 1));
                                if (m_smVisionInfo[m_intSelectedVision].g_arrPreTestInspect[intIndex])
                                {
                                    dgd_Result.Rows.Add();
                                    dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[0].Value = strName;
                                    if (m_smVisionInfo[m_intSelectedVision].g_arrPreTestExpectedResult[intIndex] == 1)
                                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[2].Value = "Fail";
                                    else
                                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[2].Value = "Pass";
                                    if (m_smVisionInfo[m_intSelectedVision].g_arrPreTestResult[intIndex])
                                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[4].Value = "Pass";
                                    else
                                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[4].Value = "Fail";
                                    dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Value = GetFinalResult(dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[2].Value.ToString(), dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[4].Value.ToString());
                                    if (dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Value.ToString() == "Pass")
                                    {
                                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Style.BackColor = Color.Lime;
                                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Style.SelectionBackColor = Color.Lime;
                                    }
                                    else
                                    {
                                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Style.BackColor = Color.Red;
                                        dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[6].Style.SelectionBackColor = Color.Red;
                                    }
                                    dgd_Result.Rows[dgd_Result.RowCount - 1].Cells[8].Value = "Details";
                                }
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
                    SRMMessageBox.Show("VisionPage() --> There is no such vision module name " + m_smVisionInfo[m_intSelectedVision].g_strVisionName + " in this SRMVision software version.");
                    break;

            }

            if (dgd_Result.RowCount > 0)
            {
                dgd_Result.Rows[0].Selected = true;
                dgd_Result.CurrentCell = dgd_Result.Rows[0].Cells[0];
                m_intSelectedRowIndex = 0;
            }

        }
        private void UpdateDetails()
        {
         
            lbl_ImageName.Text = "-";
            lbl_ExpectedResult.Text = "---";
            lbl_VerificationResult.Text = "---";
            lbl_FinalResult.Text = "---";
            lbl_FinalResult.ForeColor = Color.Black;

            switch (m_smVisionInfo[m_intSelectedVision].g_strVisionName)
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

                    if (dgd_Result.RowCount > m_intSelectedRowIndex)
                    {
                        lbl_ImageName.Text = dgd_Result.Rows[m_intSelectedRowIndex].Cells[0].Value.ToString();
                        lbl_ExpectedResult.Text = dgd_Result.Rows[m_intSelectedRowIndex].Cells[2].Value.ToString();
                        lbl_VerificationResult.Text = dgd_Result.Rows[m_intSelectedRowIndex].Cells[4].Value.ToString();
                        //switch (lbl_ImageName.Text)
                        //{
                        //    case "ExcessMark":
                        //        if ((m_smVisionInfo[m_intSelectedVision].g_arrMarks[0].ref_intFailResultMask_ForPreTest & 0x01) > 0)
                        //            lbl_VerificationResult.Text = "Fail";
                        //        else
                        //            lbl_VerificationResult.Text = "Pass";
                        //        break;
                        //    case "ExtraMark":
                        //        if ((m_smVisionInfo[m_intSelectedVision].g_arrMarks[0].ref_intFailResultMask_ForPreTest & 0x02) > 0)
                        //            lbl_VerificationResult.Text = "Fail";
                        //        else
                        //            lbl_VerificationResult.Text = "Pass";
                        //        break;
                        //    case "MissingMark":
                        //        if ((m_smVisionInfo[m_intSelectedVision].g_arrMarks[0].ref_intFailResultMask_ForPreTest & 0x10) > 0)
                        //            lbl_VerificationResult.Text = "Fail";
                        //        else
                        //            lbl_VerificationResult.Text = "Pass";
                        //        break;
                        //    default:
                        //        if (lbl_ImageName.Text.Contains("SampleImage"))
                        //        {
                        //            string strImageName = lbl_ImageName.Text;
                        //            int intIndex = Convert.ToInt32(strImageName.Substring(strImageName.LastIndexOf('e'), strImageName.Length - strImageName.LastIndexOf('e')));
                        //            if (m_smVisionInfo[m_intSelectedVision].g_arrPreTestResult[intIndex])
                        //                lbl_VerificationResult.Text = "Pass";
                        //            else
                        //                lbl_VerificationResult.Text = "Fail";
                        //        }
                        //        break;
                        //}
                        lbl_FinalResult.Text = dgd_Result.Rows[m_intSelectedRowIndex].Cells[6].Value.ToString();
                        if (lbl_FinalResult.Text == "Pass")
                            lbl_FinalResult.ForeColor = Color.Lime;
                        else
                            lbl_FinalResult.ForeColor = Color.Red;
                    }
                    //UpdateDetailsMsg();
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
                    SRMMessageBox.Show("VisionPage() --> There is no such vision module name " + m_smVisionInfo[m_intSelectedVision].g_strVisionName + " in this SRMVision software version.");
                    break;

            }
            UpdatePictureBox();
            UpdateDetailsMsg();
        }
        private string GetFinalResult(string strExpect, string strVerify)
        {
            if (strExpect == strVerify)
                return "Pass";
            else
                return "Fail";
        }

        private void dgd_Result_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            m_intSelectedRowIndex = e.RowIndex;
            UpdateDetails();
            if (e.ColumnIndex == 8)
            {
                if (!pnl_Details.Visible)
                {
                    pnl_Details.Visible = true;
                    pnl_Details.Location = new Point(3, 30);
                    m_blnDrawFirstTime = true;
                }
            }
        }

        private void cbo_VisionModule_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_intSelectedVision = cbo_VisionModule.SelectedIndex;
            m_strSelectedRecipe = m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo[m_intSelectedVision].g_intVisionIndex];

            cbo_ViewImage.Items.Clear();
            cbo_ViewImage.Items.Add("Image 1");
            cbo_ViewImage.SelectedIndex = 0;
            for (int i = 1; i < m_smVisionInfo[m_intSelectedVision].g_arrImages.Count; i++)
            {
                cbo_ViewImage.Items.Add("Image " + (i + 1).ToString());
            }

            UpdateGUI();
            UpdateDetails();
        }

        private void lst_Details_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                e.DrawBackground();

                int intFirstRowIndex = 0;
                if (lst_Details.Items.Count > 0)
                {
                    intFirstRowIndex = lst_Details.IndexFromPoint(0, 0);
                }


                int r = 0;
                int intWordSize;
                if (m_smCustomizeInfo.g_intLanguageCulture == 1) // English
                    intWordSize = 5;
                else
                    intWordSize = 9;    // Chinese char has bigger size compare to english char
                int intMaxLengthText = e.Bounds.Width / intWordSize;

                for (int i = 0; i < m_arrRowErrorMessage.Count; i++)
                {
                    Rectangle rect = new Rectangle(0, e.Bounds.Height * r, e.Bounds.Width, e.Bounds.Height);

                    if ((rect.Y + rect.Height) > lst_Details.Height)
                        return;

                    SolidBrush backgroundBrush = new SolidBrush(Color.White);
                    // Text color brush
                    SolidBrush textBrush = new SolidBrush(m_arrRowColor[intFirstRowIndex]);

                    // Draw the background
                    e.Graphics.FillRectangle(backgroundBrush, rect);


                    if (m_arrRowErrorMessage[intFirstRowIndex].Length > intMaxLengthText)
                    {
                        List<string> arrSubRowErrorMessage = new List<string>();
                        for (int s = 0; s < m_arrRowErrorMessage[intFirstRowIndex].Length; s += intMaxLengthText)
                        {
                            if (s != 0)
                            {
                                r++;

                                rect = new Rectangle(0, e.Bounds.Height * r, e.Bounds.Width, e.Bounds.Height);

                                if ((rect.Y + rect.Height) > lst_Details.Height)
                                    return;

                                e.Graphics.FillRectangle(backgroundBrush, rect);
                            }
                            int intMaxLength = Math.Min(m_arrRowErrorMessage[intFirstRowIndex].Length - (s), intMaxLengthText);
                            arrSubRowErrorMessage.Add(m_arrRowErrorMessage[intFirstRowIndex].Substring(s, intMaxLength));
                            e.Graphics.DrawString(m_arrRowErrorMessage[intFirstRowIndex].Substring(s, intMaxLength), e.Font, textBrush, rect, StringFormat.GenericDefault);

                        }
                    }
                    else
                    {
                        e.Graphics.DrawString(m_arrRowErrorMessage[intFirstRowIndex], e.Font, textBrush, rect, StringFormat.GenericDefault);
                    }
                    //float fff = m_arrRowErrorMessage[intFirstRowIndex].Length;
                    //// Draw the text
                    //e.Graphics.DrawString(m_arrRowErrorMessage[intFirstRowIndex].Substring(0, Math.Min(rect.Width, m_arrRowErrorMessage[intFirstRowIndex].Length)), e.Font, textBrush, rect, StringFormat.GenericDefault);

                    //
                    backgroundBrush.Dispose();
                    textBrush.Dispose();

                    intFirstRowIndex++;
                    r++;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void UpdateDetailsMsg()
        {
            lst_Details.Items.Clear();
            m_arrRowErrorMessage.Clear();
            m_arrRowColor.Clear();

            if (m_intSelectedRowIndex < 0 || dgd_Result.RowCount <= m_intSelectedRowIndex)
                return;

            if (dgd_Result.Rows[m_intSelectedRowIndex].Cells[0].Value.ToString().Contains("SampleImage"))
            {
                string strName = dgd_Result.Rows[m_intSelectedRowIndex].Cells[0].Value.ToString();
                int intIndex = Convert.ToInt32(strName.Substring(strName.LastIndexOf('e') + 1, strName.Length - strName.LastIndexOf('e') - 1));
                if (m_smVisionInfo[m_intSelectedVision].g_arrPreTestErrorMessage[intIndex] != "" && m_smVisionInfo[m_intSelectedVision].g_arrPreTestErrorMessage[intIndex] != null)
                {
                    //Translate here
                    string strErrorMessage = LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, m_smVisionInfo[m_intSelectedVision].g_arrPreTestErrorMessage[intIndex]);
                    string strErrorMessage2 = "";

                    if (!m_smVisionInfo[m_intSelectedVision].g_arrPreTestErrorMessage[intIndex].Contains("Pass"))
                        m_smVisionInfo[m_intSelectedVision].g_arrPreTestErrorMessageColor[intIndex] = Color.Red;

                    if (lst_Details.Items.Count < 1000)
                        lst_Details.Items.Insert(0, "");
                    else
                    {
                        lst_Details.Items.Insert(0, "");   // 2019 07 05 - insert first row and remove last row
                        lst_Details.Items.RemoveAt(1000);
                    }

                    m_arrRowErrorMessage.Insert(0, "");
                    m_arrRowColor.Insert(0, m_smVisionInfo[m_intSelectedVision].g_arrPreTestErrorMessageColor[intIndex]);

                    string[] strResult = strErrorMessage.Split('*');

                    for (int i = Math.Min(100, strResult.Length - 1); i >= 0; i--)
                    {
                        if (strResult[i] != "")
                        {
                            m_arrRowErrorMessage.Insert(0, DateTime.Now.ToString() + ": " + strResult[i]);
                            m_arrRowColor.Insert(0, m_smVisionInfo[m_intSelectedVision].g_arrPreTestErrorMessageColor[intIndex]);

                            if (lst_Details.Items.Count < 1000)
                                lst_Details.Items.Insert(0, "");
                            else
                            {
                                lst_Details.Items.Insert(0, "");   // 2019 07 05 - insert first row and remove last row
                                lst_Details.Items.RemoveAt(1000);
                            }

                            if (m_arrRowErrorMessage.Count > 1000)
                            {
                                m_arrRowErrorMessage.RemoveRange(1000, m_arrRowErrorMessage.Count - 1000);
                                m_arrRowColor.RemoveRange(1000, m_arrRowColor.Count - 1000);
                            }
                        }
                    }

                    lst_Details.SelectedIndex = 0;
                }
            }
        }

        private void btn_HideDetails_Click(object sender, EventArgs e)
        {
            pnl_Details.Visible = false;
        }

        private void btn_Prev_Click(object sender, EventArgs e)
        {
            if (dgd_Result.RowCount <= m_intSelectedRowIndex)
                return;

            if (m_intSelectedRowIndex > 0)
            {
                m_intSelectedRowIndex--;
                dgd_Result.Rows[m_intSelectedRowIndex].Selected = true;
                dgd_Result.CurrentCell = dgd_Result.Rows[m_intSelectedRowIndex].Cells[0];

                UpdateDetails();
            }
            else
            {
                if (dgd_Result.RowCount > 0)
                {
                    m_intSelectedRowIndex = dgd_Result.RowCount - 1;
                    dgd_Result.Rows[m_intSelectedRowIndex].Selected = true;
                    dgd_Result.CurrentCell = dgd_Result.Rows[m_intSelectedRowIndex].Cells[0];

                    UpdateDetails();
                }
            }
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            if (dgd_Result.RowCount <= m_intSelectedRowIndex)
                return;

            if ((m_intSelectedRowIndex + 1) < dgd_Result.RowCount)
            {
                m_intSelectedRowIndex++;
                dgd_Result.Rows[m_intSelectedRowIndex].Selected = true;
                dgd_Result.CurrentCell = dgd_Result.Rows[m_intSelectedRowIndex].Cells[0];

                UpdateDetails();
            }
            else
            {
                if (dgd_Result.RowCount > 0)
                {
                    m_intSelectedRowIndex = 0;
                    dgd_Result.Rows[m_intSelectedRowIndex].Selected = true;
                    dgd_Result.CurrentCell = dgd_Result.Rows[m_intSelectedRowIndex].Cells[0];

                    UpdateDetails();
                }
            }
        }
        private void UpdatePictureBox()
        {
            if (m_intSelectedRowIndex < 0 || cbo_ViewImage.SelectedIndex < 0)
                return;

            string strPath = m_strImagePath + lbl_ImageName.Text + ".bmp";

            if (dgd_Result.RowCount > 0)
                lbl_ImageName.Text = dgd_Result.Rows[m_intSelectedRowIndex].Cells[0].Value.ToString();
            else
                lbl_ImageName.Text = "-";

            if (cbo_ViewImage.SelectedIndex > 0)
            {
                strPath = m_strImagePath + lbl_ImageName.Text + "_Image" + cbo_ViewImage.SelectedIndex.ToString() + ".bmp";
            }

            if (File.Exists(strPath))
            {
                //pic_Image.Image = Image.FromFile(strPath);
                if (m_smVisionInfo[m_intSelectedVision].g_blnViewColorImage)
                {
                    CImageDrawing objImage = new CImageDrawing();
                    objImage.LoadImage(strPath);
                    objImage.CopyTo(m_objCImage);
                    m_objCImage.RedrawImage(m_Graphic, 440f / m_objCImage.ref_intImageWidth, 330f / m_objCImage.ref_intImageHeight);
                    objImage.Dispose();
                }
                else
                {
                    ImageDrawing objImage = new ImageDrawing();
                    objImage.LoadImage(strPath);
                    objImage.CopyTo(m_objImage);
                    m_objImage.RedrawImage(m_Graphic, 440f / m_objImage.ref_intImageWidth, 330f / m_objImage.ref_intImageHeight);
                    objImage.Dispose();
                }
            }
            else
            {
                pic_Image.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp");
            }

        }

        private void cbo_ViewImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            UpdatePictureBox();
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
