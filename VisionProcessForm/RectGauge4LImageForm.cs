using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharedMemory;
using VisionProcessing;
using Common;

namespace VisionProcessForm
{
    public partial class RectGauge4LImageForm : Form
    {
        #region Member Variables

        private bool m_blnInitDone = false;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;

        #endregion

        #region Properties
        #endregion

        public RectGauge4LImageForm(VisionInfo smVisionInfo, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            InitializeComponent();

            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;

            UpdateGUI();

            m_blnInitDone = true;
        }

        private void UpdateGUI()
        {
            if (m_smVisionInfo.g_arrPad.Length == 1)
            {
                this.Size = new Size(this.Width - pnl_Left.Width - pnl_Right.Width, this.Height - pnl_Top.Height - pnl_Bottom.Height);
                pnl_Top.Visible = false;
                pnl_Left.Visible = false;
                pnl_Right.Visible = false;
                pnl_Bottom.Visible = false;
                btn_OK.Location = new Point(btn_OK.Location.X - pnl_Left.Width - pnl_Right.Width, btn_OK.Location.Y);
                btn_Cancel.Location = new Point(btn_Cancel.Location.X - pnl_Left.Width - pnl_Right.Width, btn_Cancel.Location.Y);
            }

            // Get Total Image View Count
            int intViewImageCount = ImageDrawing.GetImageViewCount(m_smVisionInfo.g_intVisionIndex);

            // limit image no to txt box
            txt_CenterROI_Top.Maximum = txt_CenterROI_Right.Maximum = txt_CenterROI_Bottom.Maximum = txt_CenterROI_Left.Maximum = intViewImageCount;
            txt_TopROI_Top.Maximum = txt_TopROI_Right.Maximum = txt_TopROI_Bottom.Maximum = txt_TopROI_Left.Maximum = intViewImageCount;
            txt_RightROI_Top.Maximum = txt_RightROI_Right.Maximum = txt_RightROI_Bottom.Maximum = txt_RightROI_Left.Maximum = intViewImageCount;
            txt_BottomROI_Top.Maximum = txt_BottomROI_Right.Maximum = txt_BottomROI_Bottom.Maximum = txt_BottomROI_Left.Maximum = intViewImageCount;
            txt_LeftROI_Top.Maximum = txt_LeftROI_Right.Maximum = txt_LeftROI_Bottom.Maximum = txt_LeftROI_Left.Maximum = intViewImageCount;


            if (m_smVisionInfo.g_arrPad.Length > 0)
            {
                txt_CenterROI_Top.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[0].GetEdgeImageViewNo(0) + 1), txt_CenterROI_Top.Maximum);
                txt_CenterROI_Right.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[0].GetEdgeImageViewNo(1) + 1), txt_CenterROI_Right.Maximum);
                txt_CenterROI_Bottom.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[0].GetEdgeImageViewNo(2) + 1), txt_CenterROI_Bottom.Maximum);
                txt_CenterROI_Left.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[0].GetEdgeImageViewNo(3) + 1), txt_CenterROI_Left.Maximum);
            }

            if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
            {
                txt_TopROI_Top.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[1].GetEdgeImageViewNo(0) + 1), txt_TopROI_Top.Maximum);
                txt_TopROI_Right.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[1].GetEdgeImageViewNo(1) + 1), txt_TopROI_Right.Maximum);
                txt_TopROI_Bottom.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[1].GetEdgeImageViewNo(2) + 1), txt_TopROI_Bottom.Maximum);
                txt_TopROI_Left.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[1].GetEdgeImageViewNo(3) + 1), txt_TopROI_Left.Maximum);
            }

            if (m_smVisionInfo.g_arrPad.Length > 2 && m_smVisionInfo.g_blnCheck4Sides)
            {
                txt_RightROI_Top.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[2].GetEdgeImageViewNo(0) + 1), txt_RightROI_Top.Maximum);
                txt_RightROI_Right.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[2].GetEdgeImageViewNo(1) + 1), txt_RightROI_Right.Maximum);
                txt_RightROI_Bottom.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[2].GetEdgeImageViewNo(2) + 1), txt_RightROI_Bottom.Maximum);
                txt_RightROI_Left.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[2].GetEdgeImageViewNo(3) + 1), txt_RightROI_Left.Maximum);
            }

            if (m_smVisionInfo.g_arrPad.Length > 3 && m_smVisionInfo.g_blnCheck4Sides)
            {
                txt_BottomROI_Top.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[3].GetEdgeImageViewNo(0) + 1), txt_BottomROI_Top.Maximum);
                txt_BottomROI_Right.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[3].GetEdgeImageViewNo(1) + 1), txt_BottomROI_Right.Maximum);
                txt_BottomROI_Bottom.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[3].GetEdgeImageViewNo(2) + 1), txt_BottomROI_Bottom.Maximum);
                txt_BottomROI_Left.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[3].GetEdgeImageViewNo(3) + 1), txt_BottomROI_Left.Maximum);
            }

            if (m_smVisionInfo.g_arrPad.Length > 4 && m_smVisionInfo.g_blnCheck4Sides)
            {
                txt_LeftROI_Top.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[4].GetEdgeImageViewNo(0) + 1), txt_LeftROI_Top.Maximum);
                txt_LeftROI_Right.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[4].GetEdgeImageViewNo(1) + 1), txt_LeftROI_Right.Maximum);
                txt_LeftROI_Bottom.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[4].GetEdgeImageViewNo(2) + 1), txt_LeftROI_Bottom.Maximum);
                txt_LeftROI_Left.Value = Math.Min(Convert.ToDecimal(m_smVisionInfo.g_arrPad[4].GetEdgeImageViewNo(3) + 1), txt_LeftROI_Left.Maximum);
            }
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrPad.Length > 0)
            {
                m_smVisionInfo.g_arrPad[0].SetEdgeImageViewNo(Convert.ToInt32(txt_CenterROI_Top.Value) - 1, 0);
                m_smVisionInfo.g_arrPad[0].SetEdgeImageViewNo(Convert.ToInt32(txt_CenterROI_Right.Value) - 1, 1);
                m_smVisionInfo.g_arrPad[0].SetEdgeImageViewNo(Convert.ToInt32(txt_CenterROI_Bottom.Value) - 1, 2);
                m_smVisionInfo.g_arrPad[0].SetEdgeImageViewNo(Convert.ToInt32(txt_CenterROI_Left.Value) - 1, 3);
            }

            if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
            {
                m_smVisionInfo.g_arrPad[1].SetEdgeImageViewNo(Convert.ToInt32(txt_TopROI_Top.Value) - 1, 0);
                m_smVisionInfo.g_arrPad[1].SetEdgeImageViewNo(Convert.ToInt32(txt_TopROI_Right.Value) - 1, 1);
                m_smVisionInfo.g_arrPad[1].SetEdgeImageViewNo(Convert.ToInt32(txt_TopROI_Bottom.Value) - 1, 2);
                m_smVisionInfo.g_arrPad[1].SetEdgeImageViewNo(Convert.ToInt32(txt_TopROI_Left.Value) - 1, 3);
            }

            if (m_smVisionInfo.g_arrPad.Length > 2 && m_smVisionInfo.g_blnCheck4Sides)
            {
                m_smVisionInfo.g_arrPad[2].SetEdgeImageViewNo(Convert.ToInt32(txt_RightROI_Top.Value) - 1, 0);
                m_smVisionInfo.g_arrPad[2].SetEdgeImageViewNo(Convert.ToInt32(txt_RightROI_Right.Value) - 1, 1);
                m_smVisionInfo.g_arrPad[2].SetEdgeImageViewNo(Convert.ToInt32(txt_RightROI_Bottom.Value) - 1, 2);
                m_smVisionInfo.g_arrPad[2].SetEdgeImageViewNo(Convert.ToInt32(txt_RightROI_Left.Value) - 1, 3);
            }

            if (m_smVisionInfo.g_arrPad.Length > 3 && m_smVisionInfo.g_blnCheck4Sides)
            {
                m_smVisionInfo.g_arrPad[3].SetEdgeImageViewNo(Convert.ToInt32(txt_BottomROI_Top.Value) - 1, 0);
                m_smVisionInfo.g_arrPad[3].SetEdgeImageViewNo(Convert.ToInt32(txt_BottomROI_Right.Value) - 1, 1);
                m_smVisionInfo.g_arrPad[3].SetEdgeImageViewNo(Convert.ToInt32(txt_BottomROI_Bottom.Value) - 1, 2);
                m_smVisionInfo.g_arrPad[3].SetEdgeImageViewNo(Convert.ToInt32(txt_BottomROI_Left.Value) - 1, 3);
            }

            if (m_smVisionInfo.g_arrPad.Length > 4 && m_smVisionInfo.g_blnCheck4Sides)
            {
                m_smVisionInfo.g_arrPad[4].SetEdgeImageViewNo(Convert.ToInt32(txt_LeftROI_Top.Value) - 1, 0);
                m_smVisionInfo.g_arrPad[4].SetEdgeImageViewNo(Convert.ToInt32(txt_LeftROI_Right.Value) - 1, 1);
                m_smVisionInfo.g_arrPad[4].SetEdgeImageViewNo(Convert.ToInt32(txt_LeftROI_Bottom.Value) - 1, 2);
                m_smVisionInfo.g_arrPad[4].SetEdgeImageViewNo(Convert.ToInt32(txt_LeftROI_Left.Value) - 1, 3);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
            this.Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {

        }
    }
}