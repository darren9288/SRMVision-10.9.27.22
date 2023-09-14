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
    public partial class AdvancedRectGauge4LForm : Form
    {
        #region enum

        public enum ReadFrom { Pad = 0, Calibration = 1 };

        #endregion

        #region Member Variables
        private bool m_blnFirstInit = true;
        private bool m_blnInitDone = false;
        private bool m_blnSetToAll = true;
        private bool m_blnShowAdvanceSetting = false;
        private bool m_blnShowGraph = false;
        private bool m_blnBlockOtherControlUpdate = false;

        private ReadFrom m_eReadFrom = ReadFrom.Pad;    // Default
        private int m_intLineGaugeSelectedIndex = 0;
        private int m_intPadSelectedIndex = 0;
        private string m_strPath = "";
        private VisionInfo m_smVisionInfo;
        private bool m_blnUpdateHistogram = false;
        private string m_path;
        private Graphics m_Graphic;
        //private PGauge m_objPointGauge;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        #endregion

        #region Properties
        #endregion


        public AdvancedRectGauge4LForm(VisionInfo smVisionInfo, int intPadSelectedIndex, string strPath, string Path, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            InitializeComponent();
            m_Graphic = Graphics.FromHwnd(pic_Histogram.Handle);
            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            //m_smVisionInfo.g_blncboImageView = false;
            m_intPadSelectedIndex = intPadSelectedIndex;
            m_strPath = strPath;
            m_path = Path;
            m_smProductionInfo = smProductionInfo;
            UpdateROIComboBox();
            m_blnFirstInit = true;
            UpdateGUI();
            m_blnFirstInit = false;
            m_blnInitDone = true;
        }

        public AdvancedRectGauge4LForm(VisionInfo smVisionInfo, string strPath, ReadFrom eReadFrom, string Path, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            InitializeComponent();
           
            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            //m_smVisionInfo.g_blncboImageView = false;
            m_strPath = strPath;
            m_eReadFrom = eReadFrom;
            m_path = Path;
            m_smProductionInfo = smProductionInfo;
            UpdateROIComboBox();
            m_blnFirstInit = true;
            UpdateGUI();
            m_blnFirstInit = false;
            m_blnInitDone = true;
        }

        private void UpdateROIComboBox()
        {
            cbo_ROI.Items.Clear();
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == 0)
                    cbo_ROI.Items.Add("Center");
                else if (i == 1)
                    cbo_ROI.Items.Add("Top");
                else if (i == 2)
                    cbo_ROI.Items.Add("Right");
                else if (i == 3)
                    cbo_ROI.Items.Add("Bottom");
                else if (i == 4)
                    cbo_ROI.Items.Add("Left");
            }

            if (cbo_ROI.Items.Count == 1)
            {
                cbo_ROI.Visible = false;
                lbl_ROI.Visible = false;
            }

            if (cbo_ROI.Items.Count > 0)
            {
                cbo_ROI.SelectedIndex = m_smVisionInfo.g_intSelectedROI;
            }
        }

        private void UpdateGUIPicture()
        {
            if (radioBtn_All.Checked)
            {
                if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
                {
                    pic_Up.Image = imageList1.Images[0];
                    pic_Right.Image = imageList1.Images[1];
                    pic_Down.Image = imageList1.Images[2];
                    pic_Left.Image = imageList1.Images[3];
                }
                else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
                {
                    pic_Up.Image = imageList1.Images[2];
                    pic_Right.Image = imageList1.Images[3];
                    pic_Down.Image = imageList1.Images[0];
                    pic_Left.Image = imageList1.Images[1];
                }
                else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
                {
                    pic_Up.Image = imageList1.Images[4];
                    pic_Right.Image = imageList1.Images[5];
                    pic_Down.Image = imageList1.Images[6];
                    pic_Left.Image = imageList1.Images[7];
                }
                else
                {
                    pic_Up.Image = imageList1.Images[6];
                    pic_Right.Image = imageList1.Images[7];
                    pic_Down.Image = imageList1.Images[4];
                    pic_Left.Image = imageList1.Images[5];
                }
            }
            else
            {
                //switch (m_intLineGaugeSelectedIndex)
                //{
                //    case 0: //Top
                //        if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
                //            pic_Up.Image = imageList1.Images[0];
                //        else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
                //            pic_Up.Image = imageList1.Images[2];
                //        else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
                //            pic_Up.Image = imageList1.Images[4];
                //        else
                //            pic_Up.Image = imageList1.Images[6];
                //        break;
                //    case 1: // Right
                //        if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
                //            pic_Right.Image = imageList1.Images[1];
                //        else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
                //            pic_Right.Image = imageList1.Images[3];
                //        else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
                //            pic_Right.Image = imageList1.Images[5];
                //        else
                //            pic_Right.Image = imageList1.Images[7];
                //        break;
                //    case 2: //Bottom
                //        if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
                //            pic_Down.Image = imageList1.Images[2];
                //        else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
                //            pic_Down.Image = imageList1.Images[0];
                //        else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
                //            pic_Down.Image = imageList1.Images[6];
                //        else
                //            pic_Down.Image = imageList1.Images[4];
                //        break;
                //    case 3://Left
                //        if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
                //            pic_Left.Image = imageList1.Images[3];
                //        else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
                //            pic_Left.Image = imageList1.Images[1];
                //        else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
                //            pic_Left.Image = imageList1.Images[7];
                //        else
                //            pic_Left.Image = imageList1.Images[5];
                //        break;
                //}
                
                //Top
                if ((m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(0) == 0) && (!m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(0)))
                    pic_Up.Image = imageList1.Images[0];
                else if ((m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(0) == 0) && (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(0)))
                    pic_Up.Image = imageList1.Images[2];
                else if ((m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(0) == 1) && (!m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(0)))
                    pic_Up.Image = imageList1.Images[4];
                else
                    pic_Up.Image = imageList1.Images[6];

                // Right
                if ((m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(1) == 0) && (!m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(1)))
                    pic_Right.Image = imageList1.Images[1];
                else if ((m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(1) == 0) && (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(1)))
                    pic_Right.Image = imageList1.Images[3];
                else if ((m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(1) == 1) && (!m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(1)))
                    pic_Right.Image = imageList1.Images[5];
                else
                    pic_Right.Image = imageList1.Images[7];

                //Bottom
                if ((m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(2) == 0) && (!m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(2)))
                    pic_Down.Image = imageList1.Images[2];
                else if ((m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(2) == 0) && (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(2)))
                    pic_Down.Image = imageList1.Images[0];
                else if ((m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(2) == 1) && (!m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(2)))
                    pic_Down.Image = imageList1.Images[6];
                else
                    pic_Down.Image = imageList1.Images[4];

                //Left
                if ((m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(3) == 0) && (!m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(3)))
                    pic_Left.Image = imageList1.Images[3];
                else if ((m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(3) == 0) && (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(3)))
                    pic_Left.Image = imageList1.Images[1];
                else if ((m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(3) == 1) && (!m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(3)))
                    pic_Left.Image = imageList1.Images[7];
                else
                    pic_Left.Image = imageList1.Images[5];
            }
        }
        private void UpdateCalibrationGUIPicture()
        {
            if (radioBtn_All.Checked)
            {
                if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
                {
                    pic_Up.Image = imageList1.Images[0];
                    pic_Right.Image = imageList1.Images[1];
                    pic_Down.Image = imageList1.Images[2];
                    pic_Left.Image = imageList1.Images[3];
                }
                else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
                {
                    pic_Up.Image = imageList1.Images[2];
                    pic_Right.Image = imageList1.Images[3];
                    pic_Down.Image = imageList1.Images[0];
                    pic_Left.Image = imageList1.Images[1];
                }
                else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
                {
                    pic_Up.Image = imageList1.Images[4];
                    pic_Right.Image = imageList1.Images[5];
                    pic_Down.Image = imageList1.Images[6];
                    pic_Left.Image = imageList1.Images[7];
                }
                else
                {
                    pic_Up.Image = imageList1.Images[6];
                    pic_Right.Image = imageList1.Images[7];
                    pic_Down.Image = imageList1.Images[4];
                    pic_Left.Image = imageList1.Images[5];
                }
            }
            else
            {
                //switch (m_intLineGaugeSelectedIndex)
                //{
                //    case 0: //Top
                //        if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
                //            pic_Up.Image = imageList1.Images[0];
                //        else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
                //            pic_Up.Image = imageList1.Images[2];
                //        else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
                //            pic_Up.Image = imageList1.Images[4];
                //        else
                //            pic_Up.Image = imageList1.Images[6];
                //        break;
                //    case 1: // Right
                //        if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
                //            pic_Right.Image = imageList1.Images[1];
                //        else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
                //            pic_Right.Image = imageList1.Images[3];
                //        else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
                //            pic_Right.Image = imageList1.Images[5];
                //        else
                //            pic_Right.Image = imageList1.Images[7];
                //        break;
                //    case 2: //Bottom
                //        if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
                //            pic_Down.Image = imageList1.Images[2];
                //        else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
                //            pic_Down.Image = imageList1.Images[0];
                //        else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
                //            pic_Down.Image = imageList1.Images[6];
                //        else
                //            pic_Down.Image = imageList1.Images[4];
                //        break;
                //    case 3://Left
                //        if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
                //            pic_Left.Image = imageList1.Images[3];
                //        else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
                //            pic_Left.Image = imageList1.Images[1];
                //        else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
                //            pic_Left.Image = imageList1.Images[7];
                //        else
                //            pic_Left.Image = imageList1.Images[5];
                //        break;
                //}
                //Top
                if ((m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTransType(0) == 0) && (!m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].IsDirectionInToOut(m_intLineGaugeSelectedIndex)))
                    pic_Up.Image = imageList1.Images[0];
                else if ((m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTransType(0) == 0) && (m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].IsDirectionInToOut(m_intLineGaugeSelectedIndex)))
                    pic_Up.Image = imageList1.Images[2];
                else if ((m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTransType(0) == 1) && (!m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].IsDirectionInToOut(m_intLineGaugeSelectedIndex)))
                    pic_Up.Image = imageList1.Images[4];
                else
                    pic_Up.Image = imageList1.Images[6];

                // Right
                if ((m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTransType(1) == 0) && (!m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].IsDirectionInToOut(m_intLineGaugeSelectedIndex)))
                    pic_Right.Image = imageList1.Images[1];
                else if ((m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTransType(1) == 0) && (m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].IsDirectionInToOut(m_intLineGaugeSelectedIndex)))
                    pic_Right.Image = imageList1.Images[3];
                else if ((m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTransType(1) == 1) && (!m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].IsDirectionInToOut(m_intLineGaugeSelectedIndex)))
                    pic_Right.Image = imageList1.Images[5];
                else
                    pic_Right.Image = imageList1.Images[7];

                //Bottom
                if ((m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTransType(2) == 0) && (!m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].IsDirectionInToOut(m_intLineGaugeSelectedIndex)))
                    pic_Down.Image = imageList1.Images[2];
                else if ((m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTransType(2) == 0) && (m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].IsDirectionInToOut(m_intLineGaugeSelectedIndex)))
                    pic_Down.Image = imageList1.Images[0];
                else if ((m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTransType(2) == 1) && (!m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].IsDirectionInToOut(m_intLineGaugeSelectedIndex)))
                    pic_Down.Image = imageList1.Images[6];
                else
                    pic_Down.Image = imageList1.Images[4];

                //Left
                if ((m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTransType(3) == 0) && (!m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].IsDirectionInToOut(m_intLineGaugeSelectedIndex)))
                    pic_Left.Image = imageList1.Images[3];
                else if ((m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTransType(3) == 0) && (m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].IsDirectionInToOut(m_intLineGaugeSelectedIndex)))
                    pic_Left.Image = imageList1.Images[1];
                else if ((m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTransType(3) == 1) && (!m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].IsDirectionInToOut(m_intLineGaugeSelectedIndex)))
                    pic_Left.Image = imageList1.Images[7];
                else
                    pic_Left.Image = imageList1.Images[5];
            }
        }

        private void UpdateGUI()
        {
            radioBtn_All.Checked = true;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    picUnitROI.Image = ils_ImageListTree.Images[m_intPadSelectedIndex];
                    if (m_intPadSelectedIndex == 0)
                    {
                        lbl_TiltAngle.Visible = false;
                        txt_TiltAngle.Visible = false;
                    }
                    else
                    {
                        lbl_TiltAngle.Visible = true;
                        txt_TiltAngle.Visible = true;
                    }
                    UpdateGaugeToGUI();
                    
                    if (m_intPadSelectedIndex == 0)
                    {
                        chk_ApplyToAllSideROI.Visible = false;
                    }
                    else
                    {
                        chk_ApplyToAllSideROI.Visible = true;
                    }
                    //panel_AdvanceSetting.Visible = false;
                    //panel_Graph.Visible = false;
                    //Point pTop = radioBtn_Up.Location;
                    //Point pLeft = radioBtn_Left.Location;
                    //Point pRight = radioBtn_Right.Location;
                    //Point pBottom = radioBtn_Down.Location;

                    Point pTop, pLeft, pRight, pBottom;

                    switch (m_intPadSelectedIndex)
                    {
                        case 0: // Center ROI
                        case 1: // Top ROI
                            pTop = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Up.Size.Height);
                            pLeft = new Point(pic_Left.Location.X - radioBtn_Left.Size.Width - 2, pic_Left.Location.Y);
                            pRight = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2, pic_Right.Location.Y);
                            pBottom = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height);
                            radioBtn_Up.Location = pTop;
                            radioBtn_Up.CheckAlign = ContentAlignment.MiddleLeft;
                            radioBtn_Right.Location = pRight;
                            radioBtn_Right.CheckAlign = ContentAlignment.MiddleLeft;
                            radioBtn_Down.Location = pBottom;
                            radioBtn_Down.CheckAlign = ContentAlignment.MiddleLeft;
                            radioBtn_Left.Location = pLeft;
                            radioBtn_Left.CheckAlign = ContentAlignment.MiddleRight;
                            break;
                        case 2: // Right ROI
                            pTop = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Up.Size.Height);
                            pLeft = new Point(pic_Left.Location.X - radioBtn_Down.Size.Width - 2, pic_Left.Location.Y);
                            pRight = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2, pic_Right.Location.Y);
                            pBottom = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height);
                            radioBtn_Up.Location = pRight;
                            radioBtn_Up.CheckAlign = ContentAlignment.MiddleLeft;
                            radioBtn_Right.Location = pBottom;
                            radioBtn_Right.CheckAlign = ContentAlignment.MiddleLeft;
                            radioBtn_Down.Location = pLeft;
                            radioBtn_Down.CheckAlign = ContentAlignment.MiddleRight;
                            radioBtn_Left.Location = pTop;
                            radioBtn_Left.CheckAlign = ContentAlignment.MiddleLeft;
                            break;
                        case 3: // Bottom ROI
                            pTop = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Up.Size.Height);
                            pLeft = new Point(pic_Left.Location.X - radioBtn_Right.Size.Width - 2, pic_Left.Location.Y);
                            pRight = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2, pic_Right.Location.Y);
                            pBottom = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height);
                            radioBtn_Up.Location = pBottom;
                            radioBtn_Up.CheckAlign = ContentAlignment.MiddleLeft;
                            radioBtn_Right.Location = pLeft;
                            radioBtn_Right.CheckAlign = ContentAlignment.MiddleRight;
                            radioBtn_Down.Location = pTop;
                            radioBtn_Down.CheckAlign = ContentAlignment.MiddleLeft;
                            radioBtn_Left.Location = pRight;
                            radioBtn_Left.CheckAlign = ContentAlignment.MiddleLeft;
                            break;
                        case 4: // Left ROI
                            pTop = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Up.Size.Height);
                            pLeft = new Point(pic_Left.Location.X - radioBtn_Up.Size.Width - 2, pic_Left.Location.Y);
                            pRight = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2, pic_Right.Location.Y);
                            pBottom = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height);
                            radioBtn_Up.Location = pLeft;
                            radioBtn_Up.CheckAlign = ContentAlignment.MiddleRight;
                            radioBtn_Right.Location = pTop;
                            radioBtn_Right.CheckAlign = ContentAlignment.MiddleLeft;
                            radioBtn_Down.Location = pRight;
                            radioBtn_Down.CheckAlign = ContentAlignment.MiddleLeft;
                            radioBtn_Left.Location = pBottom;
                            radioBtn_Left.CheckAlign = ContentAlignment.MiddleLeft;
                            break;
                    }

                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i == m_intPadSelectedIndex)
                            m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0x0F;
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
                    }
                    if (m_blnFirstInit)
                        this.Size = new Size(this.Size.Width, this.Size.Height - panel_AdvanceSetting.Size.Height - 166); //panel_Graph.Size.Height --> 166
                    break;
                case ReadFrom.Calibration:
                    btn_UndoDontCareROI.Visible = false;
                    lbl_TiltAngle.Visible = false;
                    txt_TiltAngle.Visible = false;
                    UpdateCalibrationGaugeToGUI();
                    chk_ApplyToAllSideROI.Visible = false;
                    panel_SelectEdge.Visible = false;
                    panel_AdvanceSetting.Visible = false;
                    panel_Graph.Visible = false;
                    if (m_blnFirstInit)
                        this.Size = new Size(this.Size.Width, this.Size.Height - panel_AdvanceSetting.Size.Height - panel_SelectEdge.Size.Height - 166);//panel_Graph.Size.Height --> 166
                    break;
            }
        }

        private void UpdateCalibrationGaugeToGUI()
        {
            groupBox1.Visible = false;
            cbo_ImageMode.Visible = false;
            lbl_ImageMode.Visible = false;

            if (m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count > 0)
            {
                if (m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTransType(m_intLineGaugeSelectedIndex) == 0)
                    radioBtn_BW.Checked = true;
                else
                    radioBtn_WB.Checked = true;
                if (m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].IsDirectionInToOut(m_intLineGaugeSelectedIndex))
                    radioBtn_InOut.Checked = true;
                else
                    radioBtn_OutIn.Checked = true;

                if (m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTransChoice(m_intLineGaugeSelectedIndex) == 0)
                    radioBtn_FromBegin.Checked = true;
                else if (m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTransChoice(m_intLineGaugeSelectedIndex) == 1)
                    radioBtn_FromEnd.Checked = true;
                else
                    radioBtn_LargeAmplitude.Checked = true;

                txt_MeasThickness.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeThickness(m_intLineGaugeSelectedIndex).ToString();
                trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text); 
                txt_MeasFilter.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeFilter(m_intLineGaugeSelectedIndex).ToString();
                txt_MeasMinAmp.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeMinAmplitude(m_intLineGaugeSelectedIndex).ToString();
                trackBar_MinAmp.Value = Convert.ToInt32(txt_MeasMinAmp.Text);
                txt_MeasMinArea.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeMinArea(m_intLineGaugeSelectedIndex).ToString();
                txt_FilteringPass.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeFilterPasses(m_intLineGaugeSelectedIndex).ToString();
                txt_FilteringThreshold.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeFilterThreshold(m_intLineGaugeSelectedIndex).ToString();
                txt_threshold.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeThreshold(m_intLineGaugeSelectedIndex).ToString();
                trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);
                txt_Tolerance.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTolerance(m_intLineGaugeSelectedIndex).ToString();
                txt_SamplingStep.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeSamplingStep(m_intLineGaugeSelectedIndex).ToString();
                txt_MinScore.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeMinScore(m_intLineGaugeSelectedIndex).ToString();
                txt_MaxAngle.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeMaxAngle(m_intLineGaugeSelectedIndex).ToString();
                //txt_TiltAngle.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeTiltAngle().ToString();
                UpdateCalibrationGUIPicture();
            }
        }

        private void UpdateGaugeToGUI()
        {
            if (m_intPadSelectedIndex < m_smVisionInfo.g_arrPad.Length)
            {
                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(m_intLineGaugeSelectedIndex) == 0)
                    radioBtn_BW.Checked = true;
                else
                    radioBtn_WB.Checked = true;
                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(m_intLineGaugeSelectedIndex))
                    radioBtn_InOut.Checked = true;
                else
                    radioBtn_OutIn.Checked = true;

                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransChoice(m_intLineGaugeSelectedIndex) == 0)
                    radioBtn_FromBegin.Checked = true;
                else if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransChoice(m_intLineGaugeSelectedIndex) == 1)
                    radioBtn_FromEnd.Checked = true;
                else
                    radioBtn_LargeAmplitude.Checked = true;

                cbo_ImageMode.SelectedIndex = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageMode(m_intLineGaugeSelectedIndex);
                txt_MeasThickness.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeThickness(m_intLineGaugeSelectedIndex).ToString();
                trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text); 
                txt_MeasFilter.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilter(m_intLineGaugeSelectedIndex).ToString();
                txt_MeasMinAmp.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinAmplitude(m_intLineGaugeSelectedIndex).ToString();
                trackBar_MinAmp.Value = Convert.ToInt32(txt_MeasMinAmp.Text);
                txt_MeasMinArea.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinArea(m_intLineGaugeSelectedIndex).ToString();
                txt_FilteringPass.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilterPasses(m_intLineGaugeSelectedIndex).ToString();
                txt_FilteringThreshold.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilterThreshold(m_intLineGaugeSelectedIndex).ToString();
                txt_threshold.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeThreshold(m_intLineGaugeSelectedIndex).ToString();
                trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);
                txt_Tolerance.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTolerance(m_intLineGaugeSelectedIndex).ToString();
                txt_SamplingStep.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeSamplingStep(m_intLineGaugeSelectedIndex).ToString();
                txt_MinScore.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinScore(m_intLineGaugeSelectedIndex).ToString();
                txt_MaxAngle.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMaxAngle(m_intLineGaugeSelectedIndex).ToString();
                txt_TiltAngle.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTiltAngle().ToString();
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.ref_GaugeTransType = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(m_intLineGaugeSelectedIndex);
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.ref_GaugeTransChoice = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransChoice(m_intLineGaugeSelectedIndex);
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.ref_GaugeMinArea = Convert.ToInt32(txt_MeasMinArea.Text);
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.ref_GaugeMinAmplitude = Convert.ToInt32(txt_MeasMinAmp.Text);
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.ref_GaugeFilter = Convert.ToInt32(txt_MeasFilter.Text);
                int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(m_intPadSelectedIndex);
                switch (LineGaugeSelectedIndex)
                {
                    case 0:
                        m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                            m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                            Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);//90 * (1 + GetLineGaugeSelectedIndex(m_intPadSelectedIndex))
                        break;
                    case 1:
                        m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                           m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                           Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);//90 * (1 + GetLineGaugeSelectedIndex(m_intPadSelectedIndex))
                        break;
                    case 2:
                        m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                          m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                           Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);//90 * (1 + GetLineGaugeSelectedIndex(m_intPadSelectedIndex))
                        break;
                    case 3:
                        m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                         m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                           Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);//90 * (1 + GetLineGaugeSelectedIndex(m_intPadSelectedIndex))
                        break;
                }
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][1]);
                UpdateGUIPicture();
                m_blnUpdateHistogram = true;
            }
        }

        private void radioBtn_Left_Click(object sender, EventArgs e)
        {
            m_blnBlockOtherControlUpdate = true;

            if (sender == radioBtn_Up)
            {
                m_blnSetToAll = false;

                switch (m_eReadFrom)
                {
                    case ReadFrom.Pad:
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (chk_ApplyToAllSideROI.Checked || i == m_intPadSelectedIndex)
                                {
                                    m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(i);
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = (0x0F & (0x01 << m_intLineGaugeSelectedIndex));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
                                
                            }
                        }
                        break;
                }
            }
            else if (sender == radioBtn_Right)
            {
                m_blnSetToAll = false;
                switch (m_eReadFrom)
                {
                    case ReadFrom.Pad:
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (chk_ApplyToAllSideROI.Checked || i == m_intPadSelectedIndex)
                                {
                                    m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(i);
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = (0x0F & (0x01 << m_intLineGaugeSelectedIndex));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
                            }
                        }
                        break;
                }
            }
            else if (sender == radioBtn_Down)
            {
                m_blnSetToAll = false;
                switch (m_eReadFrom)
                {
                    case ReadFrom.Pad:
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (chk_ApplyToAllSideROI.Checked || i == m_intPadSelectedIndex)
                                {
                                    m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(i);
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = (0x0F & (0x01 << m_intLineGaugeSelectedIndex));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
                            }
                        }
                        break;
                }
            }
            else if (sender == radioBtn_Left)
            {
                m_blnSetToAll = false;
                switch (m_eReadFrom)
                {
                    case ReadFrom.Pad:
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (chk_ApplyToAllSideROI.Checked || i == m_intPadSelectedIndex)
                                {
                                    m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(i);
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = (0x0F & (0x01 << m_intLineGaugeSelectedIndex));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
                            }
                        }
                        break;
                }
            }
            else if (sender == radioBtn_All)
            {
                m_blnSetToAll = true;
                switch (m_eReadFrom)
                {
                    case ReadFrom.Pad:
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (chk_ApplyToAllSideROI.Checked || i == m_intPadSelectedIndex)
                                {
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0x0F;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
                            }
                        }
                        break;
                }
            }

            UpdateGaugeToGUI();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_blnBlockOtherControlUpdate = false;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0x0F;

                        m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.LoadRectGauge4LMeasurementSettingOnly(m_strPath,  // 2019 10 21 - CCENG: Load Gauge setting roi, dun load edge roi setting.
                                "Pad" + i.ToString());
                    }

                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].LoadRectGauge4L(m_strPath,    
                                "CalibrationRectGauge4L", false);
                    break;
            }
            //m_smVisionInfo.g_blncboImageView = true;
            Close();
            Dispose();
        }

        private void txt_MeasMinAmp_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;
            
            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    int intPadSelectedIndex = 0;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_intPadSelectedIndex == 0)
                        {
                            intPadSelectedIndex = m_intPadSelectedIndex;
                        }
                        else
                        {
                            if (i == 0)
                                continue;

                            if (chk_ApplyToAllSideROI.Checked)
                            {
                                intPadSelectedIndex = i;
                            }
                            else
                            {
                                intPadSelectedIndex = m_intPadSelectedIndex;
                            }
                        }

                        if (m_blnSetToAll)
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeMinAmplitude(Convert.ToInt32(txt_MeasMinAmp.Text));
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeMinAmplitude(Convert.ToInt32(txt_MeasMinAmp.Text), intLineGaugeSelectedIndex);
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.ref_GaugeMinAmplitude = Convert.ToInt32(txt_MeasMinAmp.Text);
                        int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                        switch (LineGaugeSelectedIndex)
                        {
                            case 0:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                                    Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 1:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                                   m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 2:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                  m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 3:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                                 m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);
                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }
                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeMinAmplitude(Convert.ToInt32(txt_MeasMinAmp.Text));
                    break;
            }
            trackBar_MinAmp.Value = Convert.ToInt32(txt_MeasMinAmp.Text);
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    int intPadSelectedIndex = 0;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_intPadSelectedIndex == 0)
                        {
                            intPadSelectedIndex = m_intPadSelectedIndex;
                        }
                        else
                        {
                            if (i == 0)
                                continue;

                            if (chk_ApplyToAllSideROI.Checked)
                            {
                                intPadSelectedIndex = i;
                            }
                            else
                            {
                                intPadSelectedIndex = m_intPadSelectedIndex;
                            }
                        }

                        if (m_blnSetToAll)
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeMinArea(Convert.ToInt32(txt_MeasMinArea.Text));
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeMinArea(Convert.ToInt32(txt_MeasMinArea.Text), intLineGaugeSelectedIndex);
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.ref_GaugeMinArea = Convert.ToInt32(txt_MeasMinArea.Text);
                        int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                        switch (LineGaugeSelectedIndex)
                        {
                            case 0:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                                    Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 1:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                                   m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 2:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                  m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 3:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                                 m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);
                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }
                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeMinArea(Convert.ToInt32(txt_MeasMinArea.Text));
                    break;
            }

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_threshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    int intPadSelectedIndex = 0;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_intPadSelectedIndex == 0)
                        {
                            intPadSelectedIndex = m_intPadSelectedIndex;
                        }
                        else
                        {
                            if (i == 0)
                                continue;

                            if (chk_ApplyToAllSideROI.Checked)
                            {
                                intPadSelectedIndex = i;
                            }
                            else
                            {
                                intPadSelectedIndex = m_intPadSelectedIndex;
                            }
                        }

                        if (m_blnSetToAll)
                        {
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeThreshold(Convert.ToInt32(txt_threshold.Text));
                          
                        }
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeThreshold(Convert.ToInt32(txt_threshold.Text), intLineGaugeSelectedIndex);
            
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                        int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                        switch (LineGaugeSelectedIndex)
                        {
                            case 0:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                                    Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 1:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                                   m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 2:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                  m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 3:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                                 m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);
                        

                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    } 
                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeThreshold(Convert.ToInt32(txt_threshold.Text));
                    break;
            }

            trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasThickness_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    int intPadSelectedIndex = 0;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_intPadSelectedIndex == 0)
                        {
                            intPadSelectedIndex = m_intPadSelectedIndex;
                        }
                        else
                        {
                            if (i == 0)
                                continue;

                            if (chk_ApplyToAllSideROI.Checked)
                            {
                                intPadSelectedIndex = i;
                            }
                            else
                            {
                                intPadSelectedIndex = m_intPadSelectedIndex;
                            }
                        }

                        if (m_blnSetToAll)
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeThickness(Convert.ToInt32(txt_MeasThickness.Text));
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeThickness(Convert.ToInt32(txt_MeasThickness.Text), intLineGaugeSelectedIndex);
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                        int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                        switch (LineGaugeSelectedIndex)
                        {
                            case 0:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                                    Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 1:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                                   m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 2:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                  m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 3:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                                 m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);

                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }

                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeThickness(Convert.ToInt32(txt_MeasThickness.Text));
                    break;
            }
            trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasFilter_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    int intPadSelectedIndex = 0;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_intPadSelectedIndex == 0)
                        {
                            intPadSelectedIndex = m_intPadSelectedIndex;
                        }
                        else
                        {
                            if (i == 0)
                                continue;

                            if (chk_ApplyToAllSideROI.Checked)
                            {
                                intPadSelectedIndex = i;
                            }
                            else
                            {
                                intPadSelectedIndex = m_intPadSelectedIndex;
                            }
                        }

                        if (m_blnSetToAll)
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeFilter(Convert.ToInt32(txt_MeasFilter.Text));
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeFilter(Convert.ToInt32(txt_MeasFilter.Text), intLineGaugeSelectedIndex);
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.ref_GaugeFilter = Convert.ToInt32(txt_MeasFilter.Text);
                        int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                        switch (LineGaugeSelectedIndex)
                        {
                            case 0:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                                    Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 1:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                                   m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 2:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                  m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 3:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                                 m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);
                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }
                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeFilter(Convert.ToInt32(txt_MeasFilter.Text));
                    break;
            }

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_FilteringPass_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    int intPadSelectedIndex = 0;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_intPadSelectedIndex == 0)
                        {
                            intPadSelectedIndex = m_intPadSelectedIndex;
                        }
                        else
                        {
                            if (i == 0)
                                continue;

                            if (chk_ApplyToAllSideROI.Checked)
                            {
                                intPadSelectedIndex = i;
                            }
                            else
                            {
                                intPadSelectedIndex = m_intPadSelectedIndex;
                            }
                        }

                        if (m_blnSetToAll)
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeFilterPasses(Convert.ToInt32(txt_FilteringPass.Text));
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeFilterPasses(Convert.ToInt32(txt_FilteringPass.Text), intLineGaugeSelectedIndex);
                        }
                     
                        int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                        switch (LineGaugeSelectedIndex)
                        {
                            case 0:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                                    Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 1:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                                   m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 2:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                  m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 3:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                                 m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);
                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }
                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeFilterPasses(Convert.ToInt32(txt_FilteringPass.Text));
                    break;
            }

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_FilteringThreshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    int intPadSelectedIndex = 0;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_intPadSelectedIndex == 0)
                        {
                            intPadSelectedIndex = m_intPadSelectedIndex;
                        }
                        else
                        {
                            if (i == 0)
                                continue;

                            if (chk_ApplyToAllSideROI.Checked)
                            {
                                intPadSelectedIndex = i;
                            }
                            else
                            {
                                intPadSelectedIndex = m_intPadSelectedIndex;
                            }
                        }

                        if (m_blnSetToAll)
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeFilterThreshold(Convert.ToSingle(txt_FilteringThreshold.Text));
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeFilterThreshold(Convert.ToSingle(txt_FilteringThreshold.Text), intLineGaugeSelectedIndex);
                        }
                       
                        int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                        switch (LineGaugeSelectedIndex)
                        {
                            case 0:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                                    Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 1:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                                   m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 2:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                  m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 3:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                                 m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);
                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }
                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeFilterThreshold(Convert.ToSingle(txt_FilteringThreshold.Text));
                    break;
            }

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void radioBtn_Polarity_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            int intTransTypeIndex;
            if (radioBtn_BW.Checked)
                intTransTypeIndex = 0;
            else
                intTransTypeIndex = 1;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    int intPadSelectedIndex = 0;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_intPadSelectedIndex == 0)
                        {
                            intPadSelectedIndex = m_intPadSelectedIndex;
                        }
                        else
                        {
                            if (i == 0)
                                continue;

                            if (chk_ApplyToAllSideROI.Checked)
                            {
                                intPadSelectedIndex = i;
                            }
                            else
                            {
                                intPadSelectedIndex = m_intPadSelectedIndex;
                            }
                        }

                        if (m_blnSetToAll)
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeTransType(intTransTypeIndex);
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeTransType(intTransTypeIndex, intLineGaugeSelectedIndex);
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.ref_GaugeTransType = intTransTypeIndex;
                        int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                        switch (LineGaugeSelectedIndex)
                        {
                            case 0:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                                    Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 1:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                                   m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 2:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                  m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 3:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                                 m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);
                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }
                    UpdateGUIPicture();
                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeTransType(intTransTypeIndex);
                    UpdateCalibrationGUIPicture();
                    break;
            }
            
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
      
        private void radioBtn_Search_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;
            int intTransChoiceIndex;
            if (radioBtn_FromBegin.Checked)
                intTransChoiceIndex = 0;
            else if (radioBtn_FromEnd.Checked)
                intTransChoiceIndex = 1;
            else
                intTransChoiceIndex = 2;
            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    int intPadSelectedIndex = 0;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_intPadSelectedIndex == 0)
                        {
                            intPadSelectedIndex = m_intPadSelectedIndex;
                        }
                        else
                        {
                            if (i == 0)
                                continue;

                            if (chk_ApplyToAllSideROI.Checked)
                            {
                                intPadSelectedIndex = i;
                            }
                            else
                            {
                                intPadSelectedIndex = m_intPadSelectedIndex;
                            }
                        }

                        if (m_blnSetToAll)
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeTransChoice(intTransChoiceIndex);
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeTransChoice(intTransChoiceIndex, intLineGaugeSelectedIndex);
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.ref_GaugeTransChoice = intTransChoiceIndex;
                        int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                        switch (LineGaugeSelectedIndex)
                        {
                            case 0:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                                    Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 1:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                                   m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 2:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                  m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 3:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                                 m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);
                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }
                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeTransChoice(intTransChoiceIndex);
                    break;
            }
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void radioBtn_Direction_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;
          
            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    int intPadSelectedIndex = 0;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_intPadSelectedIndex == 0)
                        {
                            intPadSelectedIndex = m_intPadSelectedIndex;
                        }
                        else
                        {
                            if (i == 0)
                                continue;

                            if (chk_ApplyToAllSideROI.Checked)
                            {
                                intPadSelectedIndex = i;
                            }
                            else
                            {
                                intPadSelectedIndex = m_intPadSelectedIndex;
                            }
                        }

                        if (m_blnSetToAll)
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetDirection(radioBtn_InOut.Checked);
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetDirection(radioBtn_InOut.Checked, intLineGaugeSelectedIndex);
                        }
                       
                        int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                        switch (LineGaugeSelectedIndex)
                        {
                            case 0:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                                    Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex)+90);
                                break;
                            case 1:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                                   m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 2:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                  m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 3:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                                 m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);
                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SetGaugeTiltAngle(i);
                    }
                    UpdateGUIPicture();
                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetDirection(radioBtn_InOut.Checked);
                    UpdateCalibrationGUIPicture();
                    break;
            }
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        
        private void txt_Tolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    int intPadSelectedIndex = 0;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_intPadSelectedIndex == 0)
                        {
                            intPadSelectedIndex = m_intPadSelectedIndex;
                        }
                        else
                        {
                            if (i == 0)
                                continue;

                            if (chk_ApplyToAllSideROI.Checked)
                            {
                                intPadSelectedIndex = i;
                            }
                            else
                            {
                                intPadSelectedIndex = m_intPadSelectedIndex;
                            }
                        }

                        if (m_blnSetToAll)
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeTolerance(Convert.ToSingle(txt_Tolerance.Text));
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeTolerance(Convert.ToSingle(txt_Tolerance.Text), intLineGaugeSelectedIndex);
                        }
                        int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                        switch (LineGaugeSelectedIndex)
                        {
                            case 0:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                                    Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 1:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                                   m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 2:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                  m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 3:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                                 m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);
                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }
                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeTolerance(Convert.ToSingle(txt_Tolerance.Text));
                    break;
            }
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SamplingStep_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    int intPadSelectedIndex = 0;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_intPadSelectedIndex == 0)
                        {
                            intPadSelectedIndex = m_intPadSelectedIndex;
                        }
                        else
                        {
                            if (i == 0)
                                continue;

                            if (chk_ApplyToAllSideROI.Checked)
                            {
                                intPadSelectedIndex = i;
                            }
                            else
                            {
                                intPadSelectedIndex = m_intPadSelectedIndex;
                            }
                        }

                        if (m_blnSetToAll)
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeSamplingStep(Convert.ToSingle(txt_SamplingStep.Text));
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeSamplingStep(Convert.ToSingle(txt_SamplingStep.Text), intLineGaugeSelectedIndex);
                        }
                        int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                        switch (LineGaugeSelectedIndex)
                        {
                            case 0:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                                    Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 1:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                                   m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 2:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                                  m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                            case 3:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                                 m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
                                break;
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);
                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }
                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeSamplingStep(Convert.ToSingle(txt_SamplingStep.Text));
                    break;
            }
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        if (i>0)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                int count = 0;
                                int TiltAngle = 0;

                                if (i == 1 && j == 0)
                                {
                                    TiltAngle = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetGaugeTiltAngle();
                                    count = 0;
                                }
                                else if (i == 1 && j == 1)
                                {
                                    TiltAngle = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetGaugeTiltAngle();
                                    count = 1;
                                }
                                else if (i == 2 && j == 0)
                                {
                                    TiltAngle = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetGaugeTiltAngle();
                                    count = 2;
                                }
                                else if (i == 2 && j == 1)
                                {
                                    TiltAngle = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetGaugeTiltAngle();
                                    count = 3;
                                }
                                else if (i == 3 && j == 0)
                                {
                                    TiltAngle = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetGaugeTiltAngle();
                                    count = 4;
                                }
                                else if (i == 3 && j == 1)
                                {
                                    TiltAngle = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetGaugeTiltAngle();
                                    count = 5;
                                }
                                else if (i == 4 && j == 0)
                                {
                                    TiltAngle = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetGaugeTiltAngle();
                                    count = 6;
                                }
                                else if (i == 4 && j == 1)
                                {
                                    TiltAngle = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetGaugeTiltAngle();
                                    count = 7;
                                }
                                m_smVisionInfo.g_arrPad[i].SaveTiltAnglePkgImage(m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] +
              "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Package\\Template\\",
                                                                 m_smVisionInfo.g_arrRotatedImages[0],
                                                                 m_smVisionInfo.g_arrPadROIs[i], i, TiltAngle, count);
                            }
                        }
                        m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0x0F;
                        m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SaveRectGauge4L(
                        m_strPath,
                        false,
                        "Pad" + i.ToString(),
                        true,
                        true);

                       
                    }
                    break;
                case ReadFrom.Calibration:
                    m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].SaveRectGauge4L(
                    m_strPath,
                    false,
                    "CalibrationRectGauge4L",
                    true,
                    true);
                    break;
            }

            //m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;
            Close();
            Dispose();
        }

        private void btn_ShowAdv_Click(object sender, EventArgs e)
        {
            m_blnShowAdvanceSetting = !m_blnShowAdvanceSetting;

            if (m_blnShowAdvanceSetting)
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    btn_ShowAdv.Text = "Hide Adv.";
                else
                    btn_ShowAdv.Text = "关闭高级";

                panel_AdvanceSetting.Visible = true;
                this.Size = new Size(this.Size.Width, this.Size.Height + panel_AdvanceSetting.Size.Height);
                
                if (m_blnShowGraph)
                {
                    m_blnShowGraph = !m_blnShowGraph;

                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                        btn_ShowGraph.Text = "Show Graph";
                    else
                        btn_ShowGraph.Text = "显示图表";
                    panel_Graph.Visible = false;
                    this.Size = new Size(this.Size.Width, this.Size.Height - panel_Graph.Size.Height);
                }
            }
            else
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    btn_ShowAdv.Text = "Show Adv.";
                else
                    btn_ShowAdv.Text = "显示高级";
                panel_AdvanceSetting.Visible = false;
                this.Size = new Size(this.Size.Width, this.Size.Height - panel_AdvanceSetting.Size.Height);
            }
        }

        private void panel_Setting_Paint(object sender, PaintEventArgs e)
        {

        }

        private int GetLineGaugeSelectedIndex(int intPadIndex)
        {
            switch (intPadIndex)
            {
                case 0:
                case 1: // Top ROI
                    {
                        if (radioBtn_Up.Checked)
                            return 0;
                        else if (radioBtn_Right.Checked)
                            return 1;
                        else if (radioBtn_Down.Checked)
                            return 2;
                        else if (radioBtn_Left.Checked)
                            return 3;
                    }
                    break;
                case 2: // Right ROI
                    {
                        if (radioBtn_Up.Checked)
                            return 1;
                        else if (radioBtn_Right.Checked)
                            return 2;
                        else if (radioBtn_Down.Checked)
                            return 3;
                        else if (radioBtn_Left.Checked)
                            return 0;
                    } 
                    break;
                case 3: // Bottom ROI
                    {
                        if (radioBtn_Up.Checked)
                            return 2;
                        else if (radioBtn_Right.Checked)
                            return 3;
                        else if (radioBtn_Down.Checked)
                            return 0;
                        else if (radioBtn_Left.Checked)
                            return 1;
                    } 
                    break;
                case 4: // Left ROI
                    {
                        if (radioBtn_Up.Checked)
                            return 3;
                        else if (radioBtn_Right.Checked)
                            return 0;
                        else if (radioBtn_Down.Checked)
                            return 1;
                        else if (radioBtn_Left.Checked)
                            return 2;
                    } 
                    break;
            }

            return 0;
        }

        private void chk_ApplyToAllSideROI_Click(object sender, EventArgs e)
        {
            m_blnBlockOtherControlUpdate = true;

            if (radioBtn_Up.Checked)
            {
                m_blnSetToAll = false;

                switch (m_eReadFrom)
                {
                    case ReadFrom.Pad:
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (chk_ApplyToAllSideROI.Checked || i == m_intPadSelectedIndex)
                                {
                                    m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(i);

                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = (0x0F & (0x01 << m_intLineGaugeSelectedIndex));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;

                            }
                        }
                        break;
                }
            }
            else if (radioBtn_Right.Checked)
            {
                m_blnSetToAll = false;
                switch (m_eReadFrom)
                {
                    case ReadFrom.Pad:
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (chk_ApplyToAllSideROI.Checked || i == m_intPadSelectedIndex)
                                {
                                    m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(i);
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = (0x0F & (0x01 << m_intLineGaugeSelectedIndex));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
                            }
                        }
                        break;
                }
            }
            else if (radioBtn_Down.Checked)
            {
                m_blnSetToAll = false;
                switch (m_eReadFrom)
                {
                    case ReadFrom.Pad:
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (chk_ApplyToAllSideROI.Checked || i == m_intPadSelectedIndex)
                                {
                                    m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(i);
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = (0x0F & (0x01 << m_intLineGaugeSelectedIndex));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
                            }
                        }
                        break;
                }
            }
            else if (radioBtn_Left.Checked)
            {
                m_blnSetToAll = false;
                switch (m_eReadFrom)
                {
                    case ReadFrom.Pad:
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (chk_ApplyToAllSideROI.Checked || i == m_intPadSelectedIndex)
                                {
                                    m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(i);
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = (0x0F & (0x01 << m_intLineGaugeSelectedIndex));
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
                            }
                        }
                        break;
                }
            }
            else if (radioBtn_All.Checked)
            {
                m_blnSetToAll = true;
                switch (m_eReadFrom)
                {
                    case ReadFrom.Pad:
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (chk_ApplyToAllSideROI.Checked || i == m_intPadSelectedIndex)
                                {
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0x0F;
                                }
                                else
                                    m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
                            }
                        }
                        break;
                }
            }

            UpdateGaugeToGUI();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_blnBlockOtherControlUpdate = false;

        }

        private void btn_ShowGraph_Click(object sender, EventArgs e)
        {
            m_blnShowGraph = !m_blnShowGraph;

            if (m_blnShowGraph)
            {
                m_smVisionInfo.AT_VM_UpdateHistogram = true;
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    btn_ShowGraph.Text = "Hide Graph";
                else
                    btn_ShowGraph.Text = "关闭图表";
                panel_Graph.Visible = true;
                this.Size = new Size(this.Size.Width, this.Size.Height + panel_Graph.Size.Height);
                
                if (m_blnShowAdvanceSetting)
                {
                    m_blnShowAdvanceSetting = !m_blnShowAdvanceSetting;

                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                        btn_ShowAdv.Text = "Show Adv.";
                    else
                        btn_ShowAdv.Text = "显示高级";
                    panel_AdvanceSetting.Visible = false;
                    this.Size = new Size(this.Size.Width, this.Size.Height - panel_AdvanceSetting.Size.Height);
                }
            }
            else
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    btn_ShowGraph.Text = "Show Graph";
                else
                    btn_ShowGraph.Text = "显示图表";
                panel_Graph.Visible = false;
                this.Size = new Size(this.Size.Width, this.Size.Height - panel_Graph.Size.Height);
            }
        }

        private void pic_Histogram_Paint(object sender, PaintEventArgs e)
        {
            m_blnUpdateHistogram = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (m_blnUpdateHistogram || m_smVisionInfo.AT_VM_UpdateHistogram)
            {
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.DrawLineProfile(m_Graphic, pic_Histogram.Width, pic_Histogram.Height);

                m_blnUpdateHistogram = false;
                m_smVisionInfo.AT_VM_UpdateHistogram = false;
            }
        }

        private void trackBar_Thickness_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_MeasThickness.Text = trackBar_Thickness.Value.ToString();
        }

        private void trackBar_Derivative_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_threshold.Text = trackBar_Derivative.Value.ToString();
        }

        private void txt_MinScore_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    int intPadSelectedIndex = 0;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_intPadSelectedIndex == 0)
                        {
                            intPadSelectedIndex = m_intPadSelectedIndex;
                        }
                        else
                        {
                            if (i == 0)
                                continue;

                            if (chk_ApplyToAllSideROI.Checked)
                            {
                                intPadSelectedIndex = i;
                            }
                            else
                            {
                                intPadSelectedIndex = m_intPadSelectedIndex;
                            }
                        }

                        if (m_blnSetToAll)
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeMinScore(Convert.ToInt32(txt_MinScore.Text));
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeMinScore(Convert.ToInt32(txt_MinScore.Text), intLineGaugeSelectedIndex);
                        }

                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }
                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeMinScore(Convert.ToInt32(txt_MinScore.Text));
                    break;
            }
        }

        private void txt_MaxAngle_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    int intPadSelectedIndex = 0;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_intPadSelectedIndex == 0)
                        {
                            intPadSelectedIndex = m_intPadSelectedIndex;
                        }
                        else
                        {
                            if (i == 0)
                                continue;

                            if (chk_ApplyToAllSideROI.Checked)
                            {
                                intPadSelectedIndex = i;
                            }
                            else
                            {
                                intPadSelectedIndex = m_intPadSelectedIndex;
                            }
                        }

                        if (m_blnSetToAll)
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeMaxAngle(Convert.ToInt32(txt_MaxAngle.Text));
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeMaxAngle(Convert.ToInt32(txt_MaxAngle.Text), intLineGaugeSelectedIndex);
                        }

                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }
                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeMaxAngle(Convert.ToInt32(txt_MaxAngle.Text));
                    break;
            }
        }

        private void trackBar_MinAmp_Scroll(object sender, EventArgs e)
        {

            if (!m_blnInitDone)
                return;

            txt_MeasMinAmp.Text = trackBar_MinAmp.Value.ToString();
        }

        private void txt_TiltAngle_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    int intPadSelectedIndex = 0;
                    for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        //if (m_intPadSelectedIndex == 0)
                        //{
                        //    intPadSelectedIndex = m_intPadSelectedIndex;
                        //}
                        //else
                        //{
                        //    if (i == 0)
                        //        continue;

                        //    if (chk_ApplyToAllSideROI.Checked)
                        //    {
                        //        intPadSelectedIndex = i;
                        //    }
                        //    else
                        //    {
                        //        intPadSelectedIndex = m_intPadSelectedIndex;
                        //    }
                        //}

                        //if (m_blnSetToAll)
                        //    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeTiltAngle(Convert.ToInt32(txt_TiltAngle.Text), intPadSelectedIndex);
                        //else
                        //{
                        //    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeTiltAngle(Convert.ToInt32(txt_TiltAngle.Text), intPadSelectedIndex);
                        //}

                        //if (!chk_ApplyToAllSideROI.Checked)
                        //    break;

                        // 2019-09-23 ZJYEOH : Always set to all Side ROIs automatically
                        m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SetGaugeTiltAngle(Convert.ToInt32(txt_TiltAngle.Text), i);
                    }
                    break;
                case ReadFrom.Calibration:
                    //for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                    //    m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeTiltAngle(Convert.ToInt32(txt_TiltAngle.Text));
                    break;
            }

            
        }

        private void cbo_ImageMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    int intPadSelectedIndex = 0;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_intPadSelectedIndex == 0)
                        {
                            intPadSelectedIndex = m_intPadSelectedIndex;
                        }
                        else
                        {
                            if (i == 0)
                                continue;

                            if (chk_ApplyToAllSideROI.Checked)
                            {
                                intPadSelectedIndex = i;
                            }
                            else
                            {
                                intPadSelectedIndex = m_intPadSelectedIndex;
                            }
                        }

                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeImageMode(cbo_ImageMode.SelectedIndex);

                        // ----------------- Update image base on selection of image mode -------------------------------------

                        switch (m_smVisionInfo.g_arrPad[0].ref_objRectGauge4L.GetGaugeImageMode(0))
                        {
                            default:
                            case 0:
                                {
                                    if (m_smVisionInfo.g_blnViewRotatedImage)
                                    {
                                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_fImageGain);
                                    }
                                    else
                                    {
                                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_fImageGain);
                                    }
                                }
                                break;
                            case 1:
                                {
                                    if (m_smVisionInfo.g_blnViewRotatedImage)
                                    {
                                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPkgProcessImage, m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_fImageGain);
                                        m_smVisionInfo.g_objPkgProcessImage.AddPrewitt(ref m_smVisionInfo.g_objPackageImage);
                                    }
                                    else
                                    {
                                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPkgProcessImage, m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_fImageGain);
                                        m_smVisionInfo.g_objPkgProcessImage.AddPrewitt(ref m_smVisionInfo.g_objPackageImage);
                                    }

                                }
                                break;
                        }

                        // ----------------------------------------------------------------------------------------------------


                        int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                        switch (LineGaugeSelectedIndex)
                        {
                            case 0:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalCenterX,
                                    m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                                    Convert.ToSingle(txt_Tolerance.Text), 90 * (1 + GetLineGaugeSelectedIndex(intPadSelectedIndex)));
                                break;
                            case 1:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                                   m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), 90 * (1 + GetLineGaugeSelectedIndex(intPadSelectedIndex)));
                                break;
                            case 2:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalCenterX,
                                   m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                                   Convert.ToSingle(txt_Tolerance.Text), 90 * (1 + GetLineGaugeSelectedIndex(intPadSelectedIndex)));
                                break;
                            case 3:
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                                   m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalCenterY,
                                   Convert.ToSingle(txt_Tolerance.Text), 90 * (1 + GetLineGaugeSelectedIndex(intPadSelectedIndex)));
                                break;
                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);
                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }
                    break;
                case ReadFrom.Calibration:
                    break;
            }
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_UndoDontCareROI_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            DontCareAreaSettingForm objDontCareAreaSettingForm = new DontCareAreaSettingForm(m_smVisionInfo, m_smProductionInfo, m_smCustomizeInfo, m_strPath);
            objDontCareAreaSettingForm.ShowDialog();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            objDontCareAreaSettingForm.Dispose();
        }

        private void cbo_ROI_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedROI = m_intPadSelectedIndex = cbo_ROI.SelectedIndex;
            m_blnInitDone = false;
            UpdateGUI();
            m_blnInitDone = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
    }
}