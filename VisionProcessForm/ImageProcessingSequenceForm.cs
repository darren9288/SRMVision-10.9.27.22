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
using VisionProcessing;
using System.IO;

namespace VisionProcessForm
{
    public partial class ImageProcessingSequenceForm : Form
    {
        int rowIndexFromMouseDown;
        DataGridViewRow rw;
        private ROI m_objMarkTrainROI = new ROI();
        private bool m_blnInitDone = false;
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private string m_strSelectedRecipe;
        
        private ImageDrawing m_Image = new ImageDrawing(true);
        private ImageDrawing m_TempImage = new ImageDrawing(true);
        private Graphics m_Graphic2;
        private ROI m_objROI;
        private bool m_blnUpdateImage = false;
        private int m_intPreviousThreshold;
        private float m_fPreviousThresholdRelative;
        private bool m_blnPreviousWantAutoThresholdRelative;
        private int m_intPreviousErode;
        private int m_intPreviousDilate;
        private int m_intPreviousOpen;
        private int m_intPreviousClose;
        private List<string> m_arrPreviousSeq;
        private bool m_blnWantRotatedImage = false;
        public ImageProcessingSequenceForm(VisionInfo visionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, bool blnWantRotatedImage)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = visionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_blnWantRotatedImage = blnWantRotatedImage;

            m_Graphic2 = Graphics.FromHwnd(pic_ROI.Handle);

            UpdateGUI();

            m_blnInitDone = true;
            timer1.Start();
            if (m_objROI == null)
                m_objROI = new ROI();
            m_objROI.LoadROISetting(m_smVisionInfo.g_arrSealROIs[5][0].ref_ROITotalX, m_smVisionInfo.g_arrSealROIs[5][0].ref_ROITotalY, m_smVisionInfo.g_arrSealROIs[5][0].ref_ROIWidth, m_smVisionInfo.g_arrSealROIs[5][0].ref_ROIHeight);
            if (m_blnWantRotatedImage)
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(5)].CopyTo(ref m_TempImage);
            else
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(5)].CopyTo(ref m_TempImage);

            m_objROI.AttachImage(m_TempImage);

            m_smVisionInfo.g_objSeal.DrawImageProcessingSequence(ref m_objROI, 
                Convert.ToInt32(lbl_ThresholdValue.Text), 
                Convert.ToInt32(txt_Erode.Value.ToString()),
                Convert.ToInt32(txt_Dilate.Value.ToString()),
                Convert.ToInt32(txt_Open.Value.ToString()),
                Convert.ToInt32(txt_Close.Value.ToString()),
                GetSeqList(), cbo_ImageSequence.SelectedIndex);

            int Max = Math.Max((int)(m_objROI.ref_ROIWidth), (int)(m_objROI.ref_ROIHeight));
            pic_ROI.Size = new Size(Math.Min((int)Math.Ceiling((350f / Max) * (m_objROI.ref_ROIWidth)), 350), Math.Min((int)Math.Ceiling((350f / Max) * (m_objROI.ref_ROIHeight)), 350));
            m_Image.SetImageSize(m_objROI.ref_ROIWidth, m_objROI.ref_ROIHeight);
            panel1.Height = Math.Min((int)Math.Ceiling((350f / Max) * (m_objROI.ref_ROIHeight)), 350);
            m_objROI.CopyToImage(ref m_Image);
            m_Image.RedrawImage(m_Graphic2, 350f / Max, 350f / Max);

            pic_ROI.Location = new Point(panel1.Size.Width / 2 - pic_ROI.Width / 2, panel1.Size.Height / 2 - pic_ROI.Height / 2);
            m_blnUpdateImage = true;
        }

        private void UpdateGUI()
        {
            trackBar_Threshold.Value = m_smVisionInfo.g_objSeal.GetMarkTemplateThreshold();
            lbl_ThresholdValue.Text = m_smVisionInfo.g_objSeal.GetMarkTemplateThreshold().ToString();
            txt_Erode.Value = m_smVisionInfo.g_objSeal.GetTemplateErodeValue();
            txt_Dilate.Value = m_smVisionInfo.g_objSeal.GetTemplateDilateValue();
            txt_Open.Value = m_smVisionInfo.g_objSeal.GetTemplateOpenValue();
            txt_Close.Value = m_smVisionInfo.g_objSeal.GetTemplateCloseValue();

            txt_MinErode.Value = m_smVisionInfo.g_objSeal.GetTemplateErodeMinValue();
            txt_MinDilate.Value = m_smVisionInfo.g_objSeal.GetTemplateDilateMinValue();
            txt_MinOpen.Value = m_smVisionInfo.g_objSeal.GetTemplateOpenMinValue();
            txt_MinClose.Value = m_smVisionInfo.g_objSeal.GetTemplateCloseMinValue();
            txt_MinThreshold.Value = m_smVisionInfo.g_objSeal.GetTemplateThresholdMinValue();

            txt_MaxErode.Value = m_smVisionInfo.g_objSeal.GetTemplateErodeMaxValue();
            txt_MaxDilate.Value = m_smVisionInfo.g_objSeal.GetTemplateDilateMaxValue();
            txt_MaxOpen.Value = m_smVisionInfo.g_objSeal.GetTemplateOpenMaxValue();
            txt_MaxClose.Value = m_smVisionInfo.g_objSeal.GetTemplateCloseMaxValue();
            txt_MaxThreshold.Value = m_smVisionInfo.g_objSeal.GetTemplateThresholdMaxValue();

            m_smVisionInfo.g_fThresholdRelativeValue = m_fPreviousThresholdRelative = m_smVisionInfo.g_objSeal.GetMarkTemplateThresholdRelative();
            chk_MinResidue.Checked = m_blnPreviousWantAutoThresholdRelative = m_smVisionInfo.g_objSeal.GetTemplateWantAutoThresholdRelative();
            
            if (chk_MinResidue.Checked)
            {
                trackBar_Threshold.Enabled = false;
                pnl_MinMaxThreshod.Visible = false;
                pnl_Relative.Visible = true;
                if (m_objROI == null)
                    m_objROI = new ROI();
                m_objROI.LoadROISetting(m_smVisionInfo.g_arrSealROIs[5][2].ref_ROITotalX, m_smVisionInfo.g_arrSealROIs[5][2].ref_ROITotalY, m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIWidth, m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIHeight);
                if (m_blnWantRotatedImage)
                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(5)].CopyTo(ref m_TempImage);
                else
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(5)].CopyTo(ref m_TempImage);
                m_objROI.AttachImage(m_TempImage);

                m_smVisionInfo.g_objSeal.DrawImageProcessingSequenceUntilThresholdStep(ref m_objROI,
                    Convert.ToInt32(lbl_ThresholdValue.Text),
                    Convert.ToInt32(txt_Erode.Value.ToString()),
                    Convert.ToInt32(txt_Dilate.Value.ToString()),
                    Convert.ToInt32(txt_Open.Value.ToString()),
                    Convert.ToInt32(txt_Close.Value.ToString()),
                    GetSeqList(), cbo_ImageSequence.SelectedIndex);

                if (m_smVisionInfo.g_fThresholdRelativeValue < 0)
                {
                    txt_Relative.Value = -1;
                    trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 3);
                    lbl_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
                }
                else if (m_smVisionInfo.g_fThresholdRelativeValue == 1)
                {
                    txt_Relative.Value = 101;
                    trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 2);
                    lbl_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
                }
                else
                {
                    txt_Relative.Value = Convert.ToInt32(m_smVisionInfo.g_fThresholdRelativeValue * 100);
                    trackBar_Threshold.Value = ROI.GetRelativeThresholdValue(m_objROI, m_smVisionInfo.g_fThresholdRelativeValue);
                    lbl_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
                }
            }
            else
            {
                trackBar_Threshold.Enabled = true;
                pnl_MinMaxThreshod.Visible = true;
                pnl_Relative.Visible = false;
            }

            m_intPreviousThreshold = m_smVisionInfo.g_objSeal.GetMarkTemplateThreshold();
            m_intPreviousErode = m_smVisionInfo.g_objSeal.GetTemplateErodeValue();
            m_intPreviousDilate = m_smVisionInfo.g_objSeal.GetTemplateDilateValue();
            m_intPreviousOpen = m_smVisionInfo.g_objSeal.GetTemplateOpenValue();
            m_intPreviousClose = m_smVisionInfo.g_objSeal.GetTemplateCloseValue();

            m_arrPreviousSeq = m_smVisionInfo.g_objSeal.GetTemplateImageProcessingSeq();

            List<string> arr_Seq = new List<string>();
            arr_Seq = m_smVisionInfo.g_objSeal.GetTemplateImageProcessingSeq();

            for (int i = 0; i < arr_Seq.Count; i++)
            {
                dgd_Sequence.Rows.Add();
                dgd_Sequence.Rows[dgd_Sequence.RowCount - 1].HeaderCell.Value = dgd_Sequence.RowCount.ToString();
                cbo_ImageSequence.Items.Add(dgd_Sequence.RowCount.ToString());
                cbo_ImageSequence.SelectedIndex = cbo_ImageSequence.Items.Count - 1;
                dgd_Sequence.Rows[dgd_Sequence.RowCount - 1].Cells[0].Value = arr_Seq[i];

                dgd_Sequence.Size = new Size(dgd_Sequence.Width, 25 + (22 * dgd_Sequence.RowCount));

                switch (arr_Seq[i])
                {
                    case "Threshold":
                        Threshold.Checked = true;
                        break;
                    case "Erode":
                        Erode.Checked = true;
                        break;
                    case "Dilate":
                        Dilate.Checked = true;
                        break;
                    case "Open":
                        Open.Checked = true;
                        break;
                    case "Close":
                        Close.Checked = true;
                        break;
                    case "Prewitt":
                        Prewitt.Checked = true;
                        break;
                }

                if (arr_Seq[i] == "Erode")
                {
                    Dilate.Checked = false;
                    grpbox_Dilate.Enabled = false;
                }
                if (arr_Seq[i] == "Dilate")
                {
                    Erode.Checked = false;
                    grpbox_Erode.Enabled = false;
                }
                if (arr_Seq[i] == "Open")
                {
                    Close.Checked = false;
                    grpbox_Close.Enabled = false;
                }
                if (arr_Seq[i] == "Close")
                {
                    Open.Checked = false;
                    grpbox_Open.Enabled = false;
                }
            }
        }

        private List<string> GetSeqList()
        {
            List<string> arrSeq = new List<string>();

            for (int i=0; i < dgd_Sequence.RowCount; i++)
            {
                arrSeq.Add(dgd_Sequence.Rows[i].Cells[0].Value.ToString());
            }

            return arrSeq;
        }

        private void chk_AddToSequence_Click(object sender, EventArgs e)
        {
            int intSelectedRow = 0;
            bool blnFound = false;
            for (int i = 0; i < dgd_Sequence.RowCount; i++)
            {
                if (dgd_Sequence.Rows[i].Cells[0].Value.ToString() == ((CheckBox)sender).Name)
                {
                    blnFound = true;
                    intSelectedRow = i;
                }
            }

            if (!blnFound && ((CheckBox)sender).Checked)
            {
                dgd_Sequence.Rows.Add();
                dgd_Sequence.Rows[dgd_Sequence.RowCount - 1].HeaderCell.Value = dgd_Sequence.RowCount.ToString();
                cbo_ImageSequence.Items.Add(dgd_Sequence.RowCount.ToString());
                cbo_ImageSequence.SelectedIndex = cbo_ImageSequence.Items.Count - 1;
                //dgd_Sequence.Rows[dgd_Sequence.RowCount - 1].Cells[0].Value = dgd_Sequence.RowCount;
                dgd_Sequence.Rows[dgd_Sequence.RowCount - 1].Cells[0].Value = ((CheckBox)sender).Name.ToString();

                dgd_Sequence.Size = new Size(dgd_Sequence.Width, 25 + (22 * dgd_Sequence.RowCount));

                m_smVisionInfo.g_objSeal.SetMarkTemplateThreshold(Convert.ToInt32(lbl_ThresholdValue.Text));

                m_smVisionInfo.g_objSeal.SetTemplateWantAutoThresholdRelative(chk_MinResidue.Checked);
                m_smVisionInfo.g_objSeal.SetMarkTemplateThresholdRelative(m_smVisionInfo.g_fThresholdRelativeValue);

                m_smVisionInfo.g_objSeal.SetTemplateErodeValue(Convert.ToInt32(txt_Erode.Value));
                m_smVisionInfo.g_objSeal.SetTemplateDilateValue(Convert.ToInt32(txt_Dilate.Value));
                m_smVisionInfo.g_objSeal.SetTemplateOpenValue(Convert.ToInt32(txt_Open.Value));
                m_smVisionInfo.g_objSeal.SetTemplateCloseValue(Convert.ToInt32(txt_Close.Value));

                if (((CheckBox)sender).Name == "Erode")
                {
                    Dilate.Checked = false;
                    grpbox_Dilate.Enabled = false;
                }
                if (((CheckBox)sender).Name == "Dilate")
                {
                    Erode.Checked = false;
                    grpbox_Erode.Enabled = false;
                }
                if (((CheckBox)sender).Name == "Open")
                {
                    Close.Checked = false;
                    grpbox_Close.Enabled = false;
                }
                if (((CheckBox)sender).Name == "Close")
                {
                    Open.Checked = false;
                    grpbox_Open.Enabled = false;
                }
            }
            else if (blnFound && !((CheckBox)sender).Checked)
            {
                if (dgd_Sequence.RowCount > intSelectedRow)
                {
                    dgd_Sequence.Rows.RemoveAt(intSelectedRow);
                    int intSelectedSequence = cbo_ImageSequence.SelectedIndex;
                    cbo_ImageSequence.Items.Clear();
                    for (int i = 0; i < dgd_Sequence.RowCount; i++)
                    {
                        dgd_Sequence.Rows[i].HeaderCell.Value = (i + 1).ToString();
                        cbo_ImageSequence.Items.Add((i + 1).ToString());
                    }

                    if (intSelectedSequence == intSelectedRow)
                        cbo_ImageSequence.SelectedIndex = cbo_ImageSequence.Items.Count - 1;
                    else
                    {
                        if (intSelectedSequence < intSelectedRow)
                            cbo_ImageSequence.SelectedIndex = intSelectedSequence;
                        else
                            cbo_ImageSequence.SelectedIndex = intSelectedSequence - 1;
                    }
                    dgd_Sequence.Size = new Size(dgd_Sequence.Width, 25 + (22 * dgd_Sequence.RowCount));

                    if (((CheckBox)sender).Name == "Erode")
                    {
                        grpbox_Dilate.Enabled = true;
                    }
                    if (((CheckBox)sender).Name == "Dilate")
                    {
                        grpbox_Erode.Enabled = true;
                    }
                    if (((CheckBox)sender).Name == "Open")
                    {
                        grpbox_Close.Enabled = true;
                    }
                    if (((CheckBox)sender).Name == "Close")
                    {
                        grpbox_Open.Enabled = true;
                    }
                }
            }

            m_blnUpdateImage = true;
        }

        private void dgd_Sequence_MouseClick(object sender, MouseEventArgs e)
        {
            if (dgd_Sequence.SelectedRows.Count == 1)
            {
                if (e.Button == MouseButtons.Left && dgd_Sequence.SelectedRows[0].Index >= 0)
                {
                    rw = dgd_Sequence.SelectedRows[0];
                    rowIndexFromMouseDown = dgd_Sequence.SelectedRows[0].Index;
                    dgd_Sequence.DoDragDrop(rw, DragDropEffects.Move);
                }
            }
        }

        private void dgd_Sequence_DragEnter(object sender, DragEventArgs e)
        {
            if (dgd_Sequence.SelectedRows.Count > 0)
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void dgd_Sequence_DragDrop(object sender, DragEventArgs e)
        {

            int rowIndexOfItemUnderMouseToDrop;
            Point clientPoint = dgd_Sequence.PointToClient(new Point(e.X, e.Y));
            rowIndexOfItemUnderMouseToDrop = dgd_Sequence.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            if (e.Effect == DragDropEffects.Move && rowIndexOfItemUnderMouseToDrop >= 0)
            {
                //dgd_Sequence.Rows.RemoveAt(rowIndexFromMouseDown);
                //dgd_Sequence.Rows.Insert(rowIndexOfItemUnderMouseToDrop, rw);
                string strFrom = dgd_Sequence.Rows[rowIndexFromMouseDown].Cells[0].Value.ToString();
                string strTo = dgd_Sequence.Rows[rowIndexOfItemUnderMouseToDrop].Cells[0].Value.ToString();
                dgd_Sequence.Rows[rowIndexFromMouseDown].Cells[0].Value = strTo;
                dgd_Sequence.Rows[rowIndexOfItemUnderMouseToDrop].Cells[0].Value = strFrom;
                m_blnUpdateImage = true;
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {

            m_smVisionInfo.g_objSeal.SetMarkTemplateThreshold(m_intPreviousThreshold);
            m_smVisionInfo.g_objSeal.SetTemplateErodeValue(m_intPreviousErode);
            m_smVisionInfo.g_objSeal.SetTemplateDilateValue(m_intPreviousDilate);
            m_smVisionInfo.g_objSeal.SetTemplateOpenValue(m_intPreviousOpen);
            m_smVisionInfo.g_objSeal.SetTemplateCloseValue(m_intPreviousClose);

            m_smVisionInfo.g_objSeal.SetTemplateWantAutoThresholdRelative(m_blnPreviousWantAutoThresholdRelative);
            m_smVisionInfo.g_objSeal.SetMarkTemplateThresholdRelative(m_fPreviousThresholdRelative);

            m_smVisionInfo.g_objSeal.SetTemplateImageProcessingSeq(m_arrPreviousSeq);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_blnUpdateImage)
            {
                m_blnUpdateImage = false;
                
                m_objROI.LoadROISetting(m_smVisionInfo.g_arrSealROIs[5][0].ref_ROITotalX, m_smVisionInfo.g_arrSealROIs[5][0].ref_ROITotalY, m_smVisionInfo.g_arrSealROIs[5][0].ref_ROIWidth, m_smVisionInfo.g_arrSealROIs[5][0].ref_ROIHeight);
                if (m_blnWantRotatedImage)
                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(5)].CopyTo(ref m_TempImage);
                else
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(5)].CopyTo(ref m_TempImage);
                m_objROI.AttachImage(m_TempImage);

                m_smVisionInfo.g_objSeal.DrawImageProcessingSequence(ref m_objROI,
                    Convert.ToInt32(lbl_ThresholdValue.Text),
                    Convert.ToInt32(txt_Erode.Value.ToString()),
                    Convert.ToInt32(txt_Dilate.Value.ToString()),
                    Convert.ToInt32(txt_Open.Value.ToString()),
                    Convert.ToInt32(txt_Close.Value.ToString()),
                    GetSeqList(), cbo_ImageSequence.SelectedIndex);

                int Max = Math.Max((int)(m_objROI.ref_ROIWidth), (int)(m_objROI.ref_ROIHeight));
                pic_ROI.Size = new Size(Math.Min((int)Math.Ceiling((350f / Max) * (m_objROI.ref_ROIWidth)), 350), Math.Min((int)Math.Ceiling((350f / Max) * (m_objROI.ref_ROIHeight)), 350));
                //pic_ROI.Size = new Size(250,250);
                m_Image.SetImageSize(m_objROI.ref_ROIWidth, m_objROI.ref_ROIHeight);
                panel1.Height = Math.Min((int)Math.Ceiling((350f / Max) * (m_objROI.ref_ROIHeight)), 350);
                // panel1.Location = new Point(panel1.Location.X, panel4.Location.Y+panel4.Size.Height);
                m_objROI.CopyToImage(ref m_Image);
                // m_objROI.SaveImage("D:\\m_Image.bmp");
                m_Image.RedrawImage(m_Graphic2, 350f / Max, 350f / Max);// Math.Min(Max / 250f, 250f / Max), Math.Min(Max / 250f, 250f / Max));
                                                                        //pic_ROI.Size = new Size((int)(250 * 250 / Max), (int)(250 * 250 / Max));
                pic_ROI.Location = new Point(panel1.Size.Width / 2 - pic_ROI.Width / 2, panel1.Size.Height / 2 - pic_ROI.Height / 2);

            }
            //int Max2 = Math.Max((int)(m_objROI.ref_ROIWidth), (int)(m_objROI.ref_ROIHeight));
            //m_Image.RedrawImage(m_Graphic2, 350f / Max2, 350f / Max2);
        }

        private void lbl_ThresholdValue_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objSeal.SetMarkTemplateThreshold(Convert.ToInt32(lbl_ThresholdValue.Text));
            m_blnUpdateImage = true;
        }

        private void txt_Erode_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objSeal.SetTemplateErodeValue(Convert.ToInt32(txt_Erode.Value));
            m_blnUpdateImage = true;
        }

        private void txt_Dilate_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objSeal.SetTemplateDilateValue(Convert.ToInt32(txt_Dilate.Value));
            m_blnUpdateImage = true;
        }

        private void txt_Open_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objSeal.SetTemplateOpenValue(Convert.ToInt32(txt_Open.Value));
            m_blnUpdateImage = true;
        }

        private void txt_Close_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objSeal.SetTemplateCloseValue(Convert.ToInt32(txt_Close.Value));
            m_blnUpdateImage = true;
        }

        private void btn_Threshold_Click(object sender, EventArgs e)
        {
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrSealROIs[5][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                //m_smVisionInfo.g_objSeal.SetMarkTemplateThreshold(m_smVisionInfo.g_intThresholdValue);
                lbl_ThresholdValue.Text = m_smVisionInfo.g_intThresholdValue.ToString();
            }
            objThresholdForm.Dispose();
        }

        private void cbo_ImageSequence_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return; 

            m_blnUpdateImage = true;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objSeal.SetTemplateImageProcessingSeq(GetSeqList());

            m_smVisionInfo.g_objSeal.SetTemplateErodeMinValue(Convert.ToInt32(txt_MinErode.Value));
            m_smVisionInfo.g_objSeal.SetTemplateDilateMinValue(Convert.ToInt32(txt_MinDilate.Value));
            m_smVisionInfo.g_objSeal.SetTemplateOpenMinValue(Convert.ToInt32(txt_MinOpen.Value));
            m_smVisionInfo.g_objSeal.SetTemplateCloseMinValue(Convert.ToInt32(txt_MinClose.Value));
            m_smVisionInfo.g_objSeal.SetTemplateThresholdMinValue(Convert.ToInt32(txt_MinThreshold.Value));

            m_smVisionInfo.g_objSeal.SetTemplateErodeMaxValue(Convert.ToInt32(txt_MaxErode.Value));
            m_smVisionInfo.g_objSeal.SetTemplateDilateMaxValue(Convert.ToInt32(txt_MaxDilate.Value));
            m_smVisionInfo.g_objSeal.SetTemplateOpenMaxValue(Convert.ToInt32(txt_MaxOpen.Value));
            m_smVisionInfo.g_objSeal.SetTemplateCloseMaxValue(Convert.ToInt32(txt_MaxClose.Value));
            m_smVisionInfo.g_objSeal.SetTemplateThresholdMaxValue(Convert.ToInt32(txt_MaxThreshold.Value));

            m_smVisionInfo.g_objSeal.SetTemplateWantAutoThresholdRelative(chk_MinResidue.Checked);
            m_smVisionInfo.g_objSeal.SetMarkTemplateThresholdRelative(m_smVisionInfo.g_fThresholdRelativeValue);
        }

        private void trackBar_Threshold_Scroll(object sender, EventArgs e)
        {
            lbl_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
        }

        private void txt_Relative_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_objROI.LoadROISetting(m_smVisionInfo.g_arrSealROIs[5][2].ref_ROITotalX, m_smVisionInfo.g_arrSealROIs[5][2].ref_ROITotalY, m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIWidth, m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIHeight);
            if (m_blnWantRotatedImage)
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(5)].CopyTo(ref m_TempImage);
            else
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(5)].CopyTo(ref m_TempImage);
            m_objROI.AttachImage(m_TempImage);

            m_smVisionInfo.g_objSeal.DrawImageProcessingSequenceUntilThresholdStep(ref m_objROI,
                Convert.ToInt32(lbl_ThresholdValue.Text),
                Convert.ToInt32(txt_Erode.Value.ToString()),
                Convert.ToInt32(txt_Dilate.Value.ToString()),
                Convert.ToInt32(txt_Open.Value.ToString()),
                Convert.ToInt32(txt_Close.Value.ToString()),
                GetSeqList(), cbo_ImageSequence.SelectedIndex);

            m_smVisionInfo.g_fThresholdRelativeValue = Convert.ToInt32(txt_Relative.Value) / 100f;

            if (m_smVisionInfo.g_fThresholdRelativeValue < 0)
            {
                trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 3);
                lbl_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
            }
            else if (m_smVisionInfo.g_fThresholdRelativeValue == 1)
            {
                trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 2);
                lbl_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
            }
            else
            {
                trackBar_Threshold.Value = ROI.GetRelativeThresholdValue(m_objROI, m_smVisionInfo.g_fThresholdRelativeValue);
                lbl_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
            }
            m_smVisionInfo.g_objSeal.SetMarkTemplateThreshold(Convert.ToInt32(lbl_ThresholdValue.Text));
            m_smVisionInfo.g_objSeal.SetTemplateWantAutoThresholdRelative(chk_MinResidue.Checked);
            m_smVisionInfo.g_objSeal.SetMarkTemplateThresholdRelative(m_smVisionInfo.g_fThresholdRelativeValue);
            m_blnUpdateImage = true;

        }

        private void chk_MinResidue_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (chk_MinResidue.Checked)
            {
                trackBar_Threshold.Enabled = false;
                pnl_MinMaxThreshod.Visible = false;
                pnl_Relative.Visible = true;

                m_objROI.LoadROISetting(m_smVisionInfo.g_arrSealROIs[5][2].ref_ROITotalX, m_smVisionInfo.g_arrSealROIs[5][2].ref_ROITotalY, m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIWidth, m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIHeight);
                if (m_blnWantRotatedImage)
                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(5)].CopyTo(ref m_TempImage);
                else
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(5)].CopyTo(ref m_TempImage);
                m_objROI.AttachImage(m_TempImage);

                m_smVisionInfo.g_objSeal.DrawImageProcessingSequenceUntilThresholdStep(ref m_objROI,
                    Convert.ToInt32(lbl_ThresholdValue.Text),
                    Convert.ToInt32(txt_Erode.Value.ToString()),
                    Convert.ToInt32(txt_Dilate.Value.ToString()),
                    Convert.ToInt32(txt_Open.Value.ToString()),
                    Convert.ToInt32(txt_Close.Value.ToString()),
                    GetSeqList(), cbo_ImageSequence.SelectedIndex);

                if (m_smVisionInfo.g_fThresholdRelativeValue < 0)
                {
                    txt_Relative.Value = -1;
                    trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 3);
                    lbl_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
                }
                else if (m_smVisionInfo.g_fThresholdRelativeValue == 1)
                {
                    txt_Relative.Value = 101;
                    trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 2);
                    lbl_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
                }
                else
                {
                    txt_Relative.Value = Convert.ToInt32(m_smVisionInfo.g_fThresholdRelativeValue * 100);
                    trackBar_Threshold.Value = ROI.GetRelativeThresholdValue(m_objROI, m_smVisionInfo.g_fThresholdRelativeValue);
                    lbl_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
                }
            }
            else
            {
                trackBar_Threshold.Enabled = true;
                pnl_MinMaxThreshod.Visible = true;
                pnl_Relative.Visible = false;
            }
            m_blnUpdateImage = true;
        }
    }
}
