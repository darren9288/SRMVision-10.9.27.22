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
using System.Threading;
namespace VisionProcessForm
{
    public partial class AdvancedRectGauge4LPadForm : Form
    {
        #region enum

        public enum ReadFrom { Pad = 0, Calibration = 1 };

        #endregion

        #region Member Variables
        private int m_intVisionType = 0;
        private bool m_blnInitDone = false;
        private bool m_blnWantTopMost = true;
        private bool m_blnSetToAll = true;
        private bool m_blnShowAdvanceSetting = false;
        private bool m_blnShowGraph = false;
        private bool m_blnBlockOtherControlUpdate = false;
        private bool m_blnShowForm = false;
        private int m_intGaugeTolerance = 3;
        private bool m_blnAutoTuneForAllEdges = false;
        private float m_fSampleScore = 0;
        private float m_fDistance1 = float.MaxValue;
        private float m_fDistance2 = float.MaxValue;
        private float m_fAngle = float.MaxValue;
        private int m_intThickness = 10;
        private int m_intDerivative = 5;
        private int m_intThreshold = 5;
        private int m_intThreshold_Temp = 5;
        private int m_intThickness_Temp = 10, m_intDerivative_Temp = 5;
        private float[][] m_arrAngle = { new float[]{ 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 }, new float[] { 0, 0, 0, 0 } };
        private PointF[][] m_arrPosition1 =  
            { new PointF[]{ new PointF(0, 0), new PointF(0, 0) , new PointF(0, 0) , new PointF(0, 0) },
              new PointF[]{ new PointF(0, 0), new PointF(0, 0) , new PointF(0, 0) , new PointF(0, 0) },
              new PointF[]{ new PointF(0, 0), new PointF(0, 0) , new PointF(0, 0) , new PointF(0, 0) },
              new PointF[]{ new PointF(0, 0), new PointF(0, 0) , new PointF(0, 0) , new PointF(0, 0) },
              new PointF[]{ new PointF(0, 0), new PointF(0, 0) , new PointF(0, 0) , new PointF(0, 0) }};
        private PointF[][] m_arrPosition2 =
            { new PointF[]{ new PointF(0, 0), new PointF(0, 0) , new PointF(0, 0) , new PointF(0, 0) },
              new PointF[]{ new PointF(0, 0), new PointF(0, 0) , new PointF(0, 0) , new PointF(0, 0) },
              new PointF[]{ new PointF(0, 0), new PointF(0, 0) , new PointF(0, 0) , new PointF(0, 0) },
              new PointF[]{ new PointF(0, 0), new PointF(0, 0) , new PointF(0, 0) , new PointF(0, 0) },
              new PointF[]{ new PointF(0, 0), new PointF(0, 0) , new PointF(0, 0) , new PointF(0, 0) }};
        private Point m_pPreviousLocation;

        private ReadFrom m_eReadFrom = ReadFrom.Pad;    // Default
        private int m_intLineGaugeSelectedIndex = 0;
        private int m_intPadSelectedIndex = 0;
        private int[] m_arrEdgeEnableMask;
        private string m_strPath = "";
        private VisionInfo m_smVisionInfo;
        private bool m_blnUpdateHistogram = false;
        private string m_strVisionFolderPath;
        private Graphics m_Graphic;
        //private PGauge m_objPointGauge;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        private SRMWaitingFormThread m_thWaitingFormThread;
        #endregion

        #region Properties

        public bool ref_blnShowForm { get { return m_blnShowForm; } }

        #endregion


        public AdvancedRectGauge4LPadForm(VisionInfo smVisionInfo, int intPadSelectedIndex, string strRectGaugePath, string strVisionFolderPath, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo, int intVisionType)
        {
            InitializeComponent();
            m_Graphic = Graphics.FromHwnd(pic_Histogram.Handle);
            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_intPadSelectedIndex = intPadSelectedIndex;
            m_intVisionType = intVisionType;
            //2020-05-27 ZJYEOH : add one index to selected ROI if measure center pkg using side pkg
            if (m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides && (m_eReadFrom == ReadFrom.Pad))
                if (m_intPadSelectedIndex == 0)
                    m_smVisionInfo.g_intSelectedROI = m_intPadSelectedIndex = 1;

            m_smProductionInfo = smProductionInfo;
            m_strPath = strRectGaugePath;
            m_strVisionFolderPath = strVisionFolderPath;
            m_arrEdgeEnableMask = new int[m_smVisionInfo.g_arrPad.Length];

            UpdateGUI();
            UpdatePackageImage();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_blnInitDone = true;
        }

        public AdvancedRectGauge4LPadForm(VisionInfo smVisionInfo, string strPath, ReadFrom eReadFrom, string strVisionFolderPath, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            InitializeComponent();

            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_strPath = strPath;
            m_eReadFrom = eReadFrom;
            m_smProductionInfo = smProductionInfo;
            m_strVisionFolderPath = strVisionFolderPath;
            m_arrEdgeEnableMask = new int[m_smVisionInfo.g_arrPad.Length];

            UpdateGUI();

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

            //2020-05-27 ZJYEOH : remove center if measure center pkg using side pkg
            if (m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides && (m_eReadFrom == ReadFrom.Pad))
                if (cbo_ROI.Items[0].ToString() == "Center")
                    cbo_ROI.Items.RemoveAt(0);

            if (cbo_ROI.Items.Count == 1)
            {
                cbo_ROI.Visible = false;
                lbl_ROI.Visible = false;
            }

            if (cbo_ROI.Items.Count > 0)
            {
                //2020-05-27 ZJYEOH : reduce one index to selected ROI if measure center pkg using side pkg
                if (m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides && (m_eReadFrom == ReadFrom.Pad))
                    cbo_ROI.SelectedIndex = m_smVisionInfo.g_intSelectedROI - 1;
                else
                    cbo_ROI.SelectedIndex = m_smVisionInfo.g_intSelectedROI;

                //2020-05-27 ZJYEOH : add one index to selected ROI if measure center pkg using side pkg
                if (m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides && (m_eReadFrom == ReadFrom.Pad))
                    if (m_smVisionInfo.g_intSelectedROI == 0)
                        cbo_ROI.SelectedIndex = 1;
            }
        }

        private void UpdateGUIPicture()
        {
            //if (chk_SetToAllEdges.Checked)
            //{
            //    if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
            //    {
            //        pic_Up.Image = imageList1.Images[0];
            //        pic_Right.Image = imageList1.Images[1];
            //        pic_Down.Image = imageList1.Images[2];
            //        pic_Left.Image = imageList1.Images[3];
            //    }
            //    else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
            //    {
            //        pic_Up.Image = imageList1.Images[2];
            //        pic_Right.Image = imageList1.Images[3];
            //        pic_Down.Image = imageList1.Images[0];
            //        pic_Left.Image = imageList1.Images[1];
            //    }
            //    else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
            //    {
            //        pic_Up.Image = imageList1.Images[4];
            //        pic_Right.Image = imageList1.Images[5];
            //        pic_Down.Image = imageList1.Images[6];
            //        pic_Left.Image = imageList1.Images[7];
            //    }
            //    else
            //    {
            //        pic_Up.Image = imageList1.Images[6];
            //        pic_Right.Image = imageList1.Images[7];
            //        pic_Down.Image = imageList1.Images[4];
            //        pic_Left.Image = imageList1.Images[5];
            //    }
            //}
            //else
            {
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
            //if (radioBtn_All.Checked)
            //{
            //    if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
            //    {
            //        pic_Up.Image = imageList1.Images[0];
            //        pic_Right.Image = imageList1.Images[1];
            //        pic_Down.Image = imageList1.Images[2];
            //        pic_Left.Image = imageList1.Images[3];
            //    }
            //    else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
            //    {
            //        pic_Up.Image = imageList1.Images[2];
            //        pic_Right.Image = imageList1.Images[3];
            //        pic_Down.Image = imageList1.Images[0];
            //        pic_Left.Image = imageList1.Images[1];
            //    }
            //    else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
            //    {
            //        pic_Up.Image = imageList1.Images[4];
            //        pic_Right.Image = imageList1.Images[5];
            //        pic_Down.Image = imageList1.Images[6];
            //        pic_Left.Image = imageList1.Images[7];
            //    }
            //    else
            //    {
            //        pic_Up.Image = imageList1.Images[6];
            //        pic_Right.Image = imageList1.Images[7];
            //        pic_Down.Image = imageList1.Images[4];
            //        pic_Left.Image = imageList1.Images[5];
            //    }
            //}
            //else
            {
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
            this.Size = new Size(this.Size.Width, this.Size.Height - panel_AdvanceSetting.Size.Height - 106); // panel_Graph.Size.Height --> 106

            chk_SetToAllEdges.Checked = false;   // Default setting when enter form //2021-11-18 ZJYEOH : Changed to false
            lbl_ROI.Visible = cbo_ROI.Visible = (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides);
            if (m_intPadSelectedIndex == 0)
            {
                chk_ApplyToAllSideROI.Visible = false;
                chk_ApplyToAllSideROI.Checked = false;
            }
            else
            {
                chk_ApplyToAllSideROI.Visible = true;
                chk_ApplyToAllSideROI.Checked = false;
            }

            // Get Total Image View Count
            int intViewImageCount = ImageDrawing.GetImageViewCount(m_smVisionInfo.g_intVisionIndex);

            // Update Image No combo box
            cbo_ImageNo.Items.Clear();
            for (int i = 0; i < intViewImageCount; i++)
            {
                cbo_ImageNo.Items.Add((i + 1).ToString());
            }
            m_smVisionInfo.g_intSelectedImage = cbo_ImageNo.SelectedIndex = 0;

            cbo_ImageNoTop.Items.Clear();
            cbo_ImageNoRight.Items.Clear();
            cbo_ImageNoBottom.Items.Clear();
            cbo_ImageNoLeft.Items.Clear();

            for (int i = 0; i < intViewImageCount; i++)
            {
                cbo_ImageNoTop.Items.Add("Image " + (i + 1).ToString());
                cbo_ImageNoRight.Items.Add("Image " + (i + 1).ToString());
                cbo_ImageNoBottom.Items.Add("Image " + (i + 1).ToString());
                cbo_ImageNoLeft.Items.Add("Image " + (i + 1).ToString());
            }

            m_smVisionInfo.g_intSelectedImage = cbo_ImageNoTop.SelectedIndex = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageNo(GetLineGaugeIndex(m_intPadSelectedIndex, 0));

            int intImageIndex = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageNo(GetLineGaugeIndex(m_intPadSelectedIndex, 1));
            if (intImageIndex >= cbo_ImageNoRight.Items.Count)
                intImageIndex = 0;
            cbo_ImageNoRight.SelectedIndex = intImageIndex;

            intImageIndex = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageNo(GetLineGaugeIndex(m_intPadSelectedIndex, 2));
            if (intImageIndex >= cbo_ImageNoBottom.Items.Count)
                intImageIndex = 0;
            cbo_ImageNoBottom.SelectedIndex = intImageIndex;

            intImageIndex = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageNo(GetLineGaugeIndex(m_intPadSelectedIndex, 3));
            if (intImageIndex >= cbo_ImageNoLeft.Items.Count)
                intImageIndex = 0;
            cbo_ImageNoLeft.SelectedIndex = intImageIndex;

            // Update ROI combo box
            UpdateROIComboBox();

            DefineEdgeEnableMask();

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

            UpdateEdgeRadioButtonGUI();

            UpdateSelectedGaugeEdgeMask();

            UpdateGaugeToGUI();

            //panel_AdvanceSetting.Visible = false;
            //panel_Graph.Visible = false;
            //Point pTop = radioBtn_Up.Location;
            //Point pLeft = radioBtn_Left.Location;
            //Point pRight = radioBtn_Right.Location;
            //Point pBottom = radioBtn_Down.Location;

            Point pTop, pLeft, pRight, pBottom;
            Point pTopcbo, pLeftcbo, pRightcbo, pBottomcbo;
            switch (m_intPadSelectedIndex)
            {
                case 0: // Center ROI
                case 1: // Top ROI
                    pTop = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Up.Size.Height);
                    pLeft = new Point(pic_Left.Location.X - radioBtn_Left.Size.Width - 2, pic_Left.Location.Y);
                    pRight = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2, pic_Right.Location.Y);
                    pBottom = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height);

                    pTopcbo = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Up.Size.Height - 2 - cbo_ImageNoTop.Size.Height - 2);
                    pLeftcbo = new Point(pic_Left.Location.X - radioBtn_Left.Size.Width - 2 - cbo_ImageNoLeft.Size.Width - 2, pic_Left.Location.Y);
                    pRightcbo = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2 + radioBtn_Right.Size.Width + 2, pic_Right.Location.Y);
                    pBottomcbo = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height + 2 + radioBtn_Down.Size.Height + 2);

                    radioBtn_Up.Location = pTop;
                    radioBtn_Up.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Right.Location = pRight;
                    radioBtn_Right.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Down.Location = pBottom;
                    radioBtn_Down.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Left.Location = pLeft;
                    radioBtn_Left.CheckAlign = ContentAlignment.MiddleRight;

                    cbo_ImageNoTop.Location = pTopcbo;
                    cbo_ImageNoRight.Location = pRightcbo;
                    cbo_ImageNoBottom.Location = pBottomcbo;
                    cbo_ImageNoLeft.Location = pLeftcbo;
                    break;
                case 2: // Right ROI
                    pTop = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Up.Size.Height);
                    pLeft = new Point(pic_Left.Location.X - radioBtn_Down.Size.Width - 2, pic_Left.Location.Y);
                    pRight = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2, pic_Right.Location.Y);
                    pBottom = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height);

                    pTopcbo = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Left.Size.Height - 2 - cbo_ImageNoTop.Size.Height - 2);
                    pLeftcbo = new Point(pic_Left.Location.X - radioBtn_Down.Size.Width - 2 - cbo_ImageNoLeft.Size.Width - 2, pic_Left.Location.Y);
                    pRightcbo = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2 + radioBtn_Up.Size.Width + 2, pic_Right.Location.Y);
                    pBottomcbo = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height + 2 + radioBtn_Right.Size.Height + 2);

                    radioBtn_Up.Location = pRight;
                    radioBtn_Up.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Right.Location = pBottom;
                    radioBtn_Right.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Down.Location = pLeft;
                    radioBtn_Down.CheckAlign = ContentAlignment.MiddleRight;
                    radioBtn_Left.Location = pTop;
                    radioBtn_Left.CheckAlign = ContentAlignment.MiddleLeft;

                    cbo_ImageNoTop.Location = pRightcbo;
                    cbo_ImageNoRight.Location = pBottomcbo;
                    cbo_ImageNoBottom.Location = pLeftcbo;
                    cbo_ImageNoLeft.Location = pTopcbo;
                    break;
                case 3: // Bottom ROI
                    pTop = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Up.Size.Height);
                    pLeft = new Point(pic_Left.Location.X - radioBtn_Right.Size.Width - 2, pic_Left.Location.Y);
                    pRight = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2, pic_Right.Location.Y);
                    pBottom = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height);

                    pTopcbo = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Down.Size.Height - 2 - cbo_ImageNoTop.Size.Height - 2);
                    pLeftcbo = new Point(pic_Left.Location.X - radioBtn_Right.Size.Width - 2 - cbo_ImageNoLeft.Size.Width - 2, pic_Left.Location.Y);
                    pRightcbo = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2 + radioBtn_Left.Size.Width + 2, pic_Right.Location.Y);
                    pBottomcbo = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height + 2 + radioBtn_Up.Size.Height + 2);

                    radioBtn_Up.Location = pBottom;
                    radioBtn_Up.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Right.Location = pLeft;
                    radioBtn_Right.CheckAlign = ContentAlignment.MiddleRight;
                    radioBtn_Down.Location = pTop;
                    radioBtn_Down.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Left.Location = pRight;
                    radioBtn_Left.CheckAlign = ContentAlignment.MiddleLeft;

                    cbo_ImageNoTop.Location = pBottomcbo;
                    cbo_ImageNoRight.Location = pLeftcbo;
                    cbo_ImageNoBottom.Location = pTopcbo;
                    cbo_ImageNoLeft.Location = pRightcbo;
                    break;
                case 4: // Left ROI
                    pTop = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Up.Size.Height);
                    pLeft = new Point(pic_Left.Location.X - radioBtn_Up.Size.Width - 2, pic_Left.Location.Y);
                    pRight = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2, pic_Right.Location.Y);
                    pBottom = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height);

                    pTopcbo = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Right.Size.Height - 2 - cbo_ImageNoTop.Size.Height - 2);
                    pLeftcbo = new Point(pic_Left.Location.X - radioBtn_Up.Size.Width - 2 - cbo_ImageNoLeft.Size.Width - 2, pic_Left.Location.Y);
                    pRightcbo = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2 + radioBtn_Down.Size.Width + 2, pic_Right.Location.Y);
                    pBottomcbo = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height + 2 + radioBtn_Left.Size.Height + 2);

                    radioBtn_Up.Location = pLeft;
                    radioBtn_Up.CheckAlign = ContentAlignment.MiddleRight;
                    radioBtn_Right.Location = pTop;
                    radioBtn_Right.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Down.Location = pRight;
                    radioBtn_Down.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Left.Location = pBottom;
                    radioBtn_Left.CheckAlign = ContentAlignment.MiddleLeft;

                    cbo_ImageNoTop.Location = pLeftcbo;
                    cbo_ImageNoRight.Location = pTopcbo;
                    cbo_ImageNoBottom.Location = pRightcbo;
                    cbo_ImageNoLeft.Location = pBottomcbo;
                    break;
            }

            //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //{
            //    if (i == m_intPadSelectedIndex)
            //    {
            //        m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
            //        for (int j = 0; j < m_smVisionInfo.g_arrPad[i].GetEdgeImageViewNo().Length; j++)
            //        {
            //            if (m_smVisionInfo.g_arrPad[i].GetEdgeImageViewNo(j) == cbo_ImageNo.SelectedIndex)
            //                m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask |= (m_arrEdgeEnableMask[i] & (0x01 << j));
            //        }
            //    }
            //    else
            //        m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
            //}
        }

        private void ReUpdateGUI()
        {
            //chk_SetToAllEdges.Checked = true;   // Check to all edge everytime change ROI, image view no, or image gauge no.

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
            Point pTopcbo, pLeftcbo, pRightcbo, pBottomcbo;
            switch (m_intPadSelectedIndex)
            {
                case 0: // Center ROI
                case 1: // Top ROI
                    pTop = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Up.Size.Height);
                    pLeft = new Point(pic_Left.Location.X - radioBtn_Left.Size.Width - 2, pic_Left.Location.Y);
                    pRight = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2, pic_Right.Location.Y);
                    pBottom = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height);

                    pTopcbo = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Up.Size.Height - 2 - cbo_ImageNoTop.Size.Height - 2);
                    pLeftcbo = new Point(pic_Left.Location.X - radioBtn_Left.Size.Width - 2 - cbo_ImageNoLeft.Size.Width - 2, pic_Left.Location.Y);
                    pRightcbo = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2 + radioBtn_Right.Size.Width + 2, pic_Right.Location.Y);
                    pBottomcbo = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height + 2 + radioBtn_Down.Size.Height + 2);

                    radioBtn_Up.Location = pTop;
                    radioBtn_Up.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Right.Location = pRight;
                    radioBtn_Right.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Down.Location = pBottom;
                    radioBtn_Down.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Left.Location = pLeft;
                    radioBtn_Left.CheckAlign = ContentAlignment.MiddleRight;
                    
                    cbo_ImageNoTop.Location = pTopcbo;
                    cbo_ImageNoRight.Location = pRightcbo;
                    cbo_ImageNoBottom.Location = pBottomcbo;
                    cbo_ImageNoLeft.Location = pLeftcbo;
                    break;
                case 2: // Right ROI
                    pTop = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Up.Size.Height);
                    pLeft = new Point(pic_Left.Location.X - radioBtn_Down.Size.Width - 2, pic_Left.Location.Y);
                    pRight = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2, pic_Right.Location.Y);
                    pBottom = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height);

                    pTopcbo = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Left.Size.Height - 2 - cbo_ImageNoTop.Size.Height - 2);
                    pLeftcbo = new Point(pic_Left.Location.X - radioBtn_Down.Size.Width - 2 - cbo_ImageNoLeft.Size.Width - 2, pic_Left.Location.Y);
                    pRightcbo = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2 + radioBtn_Up.Size.Width + 2, pic_Right.Location.Y);
                    pBottomcbo = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height + 2 + radioBtn_Right.Size.Height + 2);

                    radioBtn_Up.Location = pRight;
                    radioBtn_Up.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Right.Location = pBottom;
                    radioBtn_Right.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Down.Location = pLeft;
                    radioBtn_Down.CheckAlign = ContentAlignment.MiddleRight;
                    radioBtn_Left.Location = pTop;
                    radioBtn_Left.CheckAlign = ContentAlignment.MiddleLeft;

                    cbo_ImageNoTop.Location = pRightcbo;
                    cbo_ImageNoRight.Location = pBottomcbo;
                    cbo_ImageNoBottom.Location = pLeftcbo;
                    cbo_ImageNoLeft.Location = pTopcbo;
                    break;
                case 3: // Bottom ROI
                    pTop = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Up.Size.Height);
                    pLeft = new Point(pic_Left.Location.X - radioBtn_Right.Size.Width - 2, pic_Left.Location.Y);
                    pRight = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2, pic_Right.Location.Y);
                    pBottom = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height);

                    pTopcbo = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Down.Size.Height - 2 - cbo_ImageNoTop.Size.Height - 2);
                    pLeftcbo = new Point(pic_Left.Location.X - radioBtn_Right.Size.Width - 2 - cbo_ImageNoLeft.Size.Width - 2, pic_Left.Location.Y);
                    pRightcbo = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2 + radioBtn_Left.Size.Width + 2, pic_Right.Location.Y);
                    pBottomcbo = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height + 2 + radioBtn_Up.Size.Height + 2);

                    radioBtn_Up.Location = pBottom;
                    radioBtn_Up.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Right.Location = pLeft;
                    radioBtn_Right.CheckAlign = ContentAlignment.MiddleRight;
                    radioBtn_Down.Location = pTop;
                    radioBtn_Down.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Left.Location = pRight;
                    radioBtn_Left.CheckAlign = ContentAlignment.MiddleLeft;

                    cbo_ImageNoTop.Location = pBottomcbo;
                    cbo_ImageNoRight.Location = pLeftcbo;
                    cbo_ImageNoBottom.Location = pTopcbo;
                    cbo_ImageNoLeft.Location = pRightcbo;
                    break;
                case 4: // Left ROI
                    pTop = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Up.Size.Height);
                    pLeft = new Point(pic_Left.Location.X - radioBtn_Up.Size.Width - 2, pic_Left.Location.Y);
                    pRight = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2, pic_Right.Location.Y);
                    pBottom = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height);

                    pTopcbo = new Point(pic_Up.Location.X, pic_Up.Location.Y - radioBtn_Right.Size.Height - 2 - cbo_ImageNoTop.Size.Height - 2);
                    pLeftcbo = new Point(pic_Left.Location.X - radioBtn_Up.Size.Width - 2 - cbo_ImageNoLeft.Size.Width - 2, pic_Left.Location.Y);
                    pRightcbo = new Point(pic_Right.Location.X + pic_Right.Size.Width + 2 + radioBtn_Down.Size.Width + 2, pic_Right.Location.Y);
                    pBottomcbo = new Point(pic_Down.Location.X, pic_Down.Location.Y + pic_Down.Size.Height + 2 + radioBtn_Left.Size.Height + 2);

                    radioBtn_Up.Location = pLeft;
                    radioBtn_Up.CheckAlign = ContentAlignment.MiddleRight;
                    radioBtn_Right.Location = pTop;
                    radioBtn_Right.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Down.Location = pRight;
                    radioBtn_Down.CheckAlign = ContentAlignment.MiddleLeft;
                    radioBtn_Left.Location = pBottom;
                    radioBtn_Left.CheckAlign = ContentAlignment.MiddleLeft;

                    cbo_ImageNoTop.Location = pLeftcbo;
                    cbo_ImageNoRight.Location = pTopcbo;
                    cbo_ImageNoBottom.Location = pRightcbo;
                    cbo_ImageNoLeft.Location = pBottomcbo;
                    break;
            }

            //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //{
            //    if (i == m_intPadSelectedIndex)
            //    {
            //        m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
            //        for (int j = 0; j < m_smVisionInfo.g_arrPad[i].GetEdgeImageViewNo().Length; j++)
            //        {
            //            if (m_smVisionInfo.g_arrPad[i].GetEdgeImageViewNo(j) == cbo_ImageNo.SelectedIndex)
            //                m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask |= (m_arrEdgeEnableMask[i] & (0x01 << j));
            //        }
            //    }
            //    else
            //        m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
            //}
        }

        private void UpdateCalibrationGaugeToGUI()
        {
            groupBox1.Visible = false;
            cbo_ImageMode.Visible = false;
            lbl_ImageMode.Visible = false;

            if (m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count > 0)
            {
                // ----- Image Gain -----
                txt_ImageGain.Value = Convert.ToDecimal(m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeImageGain(m_intLineGaugeSelectedIndex));
                txt_Gain.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeImageGain(m_intLineGaugeSelectedIndex).ToString();
                trackBar_Gain.Value = Convert.ToInt32(Convert.ToSingle(txt_Gain.Text) * 10f);

                // ----- Image Threshold -----
                chk_EnableThreshold.Enabled = true;
                chk_EnableThreshold.Checked = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeWantImageThreshold(m_intLineGaugeSelectedIndex);

                txt_Threshold.Enabled = chk_EnableThreshold.Checked;
                txt_Threshold.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeImageThreshold(m_intLineGaugeSelectedIndex).ToString();
                trackBar_Threshold.Value = Convert.ToInt32(txt_Threshold.Text);

                txt_OpenClose.Value = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeImageOpenCloseIteration(m_intLineGaugeSelectedIndex);
                if (Convert.ToInt32(txt_OpenClose.Value) > 0)
                {
                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                        lbl_OpenClose.Text = "Open Iteration:";
                    else
                        lbl_OpenClose.Text = "扩展:";
                }
                else if (Convert.ToInt32(txt_OpenClose.Value) < 0)
                {
                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                        lbl_OpenClose.Text = "Close Iteration:";
                    else
                        lbl_OpenClose.Text = "闭合:";
                }
                else
                {
                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                        lbl_OpenClose.Text = "No Iteration:";
                    else
                        lbl_OpenClose.Text = "无图像处理:";
                }

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
                txt_Offset.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeOffset(m_intLineGaugeSelectedIndex).ToString();
                trackBar_Offset.Value = Convert.ToInt32(txt_Offset.Text);
                txt_MeasFilter.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeFilter(m_intLineGaugeSelectedIndex).ToString();
                txt_MeasMinAmp.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeMinAmplitude(m_intLineGaugeSelectedIndex).ToString();
                trackBar_MinAmp.Value = Convert.ToInt32(txt_MeasMinAmp.Text);
                txt_MeasMinArea.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeMinArea(m_intLineGaugeSelectedIndex).ToString();
                txt_FilteringPass.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeFilterPasses(m_intLineGaugeSelectedIndex).ToString();
                txt_FilteringThreshold.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeFilterThreshold(m_intLineGaugeSelectedIndex).ToString();
                txt_Derivative.Text = m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].GetGaugeThreshold(m_intLineGaugeSelectedIndex).ToString();
                trackBar_Derivative.Value = Convert.ToInt32(txt_Derivative.Text);
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
                txt_ImageGain.Enabled = true;

                m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(m_intPadSelectedIndex);

                // ----- Image Gain -----
                txt_ImageGain.Value = Convert.ToDecimal(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageGain(m_intLineGaugeSelectedIndex));
                txt_Gain.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageGain(m_intLineGaugeSelectedIndex).ToString();
                trackBar_Gain.Value = Convert.ToInt32(Convert.ToSingle(txt_Gain.Text) * 10f);

                // ----- Image Threshold -----
                chk_EnableThreshold.Enabled = true;
                chk_EnableThreshold.Checked = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeWantImageThreshold(m_intLineGaugeSelectedIndex);

                txt_Threshold.Enabled = chk_EnableThreshold.Checked;
                txt_Threshold.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageThreshold(m_intLineGaugeSelectedIndex).ToString();
                trackBar_Threshold.Value = Convert.ToInt32(txt_Threshold.Text);

                txt_OpenClose.Value = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageOpenCloseIteration(m_intLineGaugeSelectedIndex);
                if (Convert.ToInt32(txt_OpenClose.Value) > 0)
                {
                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                        lbl_OpenClose.Text = "Open Iteration:";
                    else
                        lbl_OpenClose.Text = "扩展:";
                }
                else if (Convert.ToInt32(txt_OpenClose.Value) < 0)
                {
                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                        lbl_OpenClose.Text = "Close Iteration:";
                    else
                        lbl_OpenClose.Text = "闭合:";
                }
                else
                {
                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                        lbl_OpenClose.Text = "No Iteration:";
                    else
                        lbl_OpenClose.Text = "无图像处理:";
                }

                // ----- Polarity -----
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
                txt_Offset.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeOffset(m_intLineGaugeSelectedIndex).ToString();
                trackBar_Offset.Value = Convert.ToInt32(txt_Offset.Text);
                txt_MeasFilter.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilter(m_intLineGaugeSelectedIndex).ToString();
                txt_MeasMinAmp.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinAmplitude(m_intLineGaugeSelectedIndex).ToString();
                trackBar_MinAmp.Value = Convert.ToInt32(txt_MeasMinAmp.Text);
                txt_MeasMinArea.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinArea(m_intLineGaugeSelectedIndex).ToString();
                txt_FilteringPass.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilterPasses(m_intLineGaugeSelectedIndex).ToString();
                txt_FilteringThreshold.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilterThreshold(m_intLineGaugeSelectedIndex).ToString();
                txt_Derivative.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeThreshold(m_intLineGaugeSelectedIndex).ToString();
                trackBar_Derivative.Value = Convert.ToInt32(txt_Derivative.Text);
                txt_Tolerance.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTolerance(m_intLineGaugeSelectedIndex).ToString();
                txt_SamplingStep.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeSamplingStep(m_intLineGaugeSelectedIndex).ToString();
                txt_MinScore.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinScore(m_intLineGaugeSelectedIndex).ToString();
                txt_MaxAngle.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMaxAngle(m_intLineGaugeSelectedIndex).ToString();
                txt_TiltAngle.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTiltAngle().ToString();
                txt_GroupTol.Text = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeGroupTolerance(m_intLineGaugeSelectedIndex).ToString();
                chk_EnableSubGauge.Checked = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeEnableSubGauge(m_intLineGaugeSelectedIndex);
                cbo_GaugeMeasureMode.SelectedIndex = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMeasureMode(m_intLineGaugeSelectedIndex);
                if (m_intPadSelectedIndex == 0 || m_intPadSelectedIndex == 1 || m_intPadSelectedIndex == 3)
                {
                    if ((cbo_GaugeMeasureMode.SelectedIndex == 2 && (m_intLineGaugeSelectedIndex == 1 || m_intLineGaugeSelectedIndex == 3)) && m_intPadSelectedIndex != 0)
                    {
                        chk_EnableThreshold.Enabled = false;
                        pnl_Thickness.Visible = false;
                        pnl_Offset.Visible = true;
                    }
                    else if ((cbo_GaugeMeasureMode.SelectedIndex == 4 && (m_intLineGaugeSelectedIndex == 0 || m_intLineGaugeSelectedIndex == 2)) && m_intPadSelectedIndex == 0)
                    {
                        chk_EnableThreshold.Enabled = true;
                        pnl_Thickness.Visible = false;
                        pnl_Offset.Visible = true;
                    }
                    else
                    {
                        chk_EnableThreshold.Enabled = true;
                        pnl_Thickness.Visible = true;
                        pnl_Offset.Visible = false;
                    }
                }
                else
                {
                    if (cbo_GaugeMeasureMode.SelectedIndex == 2 && (m_intLineGaugeSelectedIndex == 0 || m_intLineGaugeSelectedIndex == 2))
                    {
                        chk_EnableThreshold.Enabled = false;
                        pnl_Thickness.Visible = false;
                        pnl_Offset.Visible = true;
                    }
                    else
                    {
                        chk_EnableThreshold.Enabled = true;
                        pnl_Thickness.Visible = true;
                        pnl_Offset.Visible = false;
                    }
                }
                //if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetAutoDontCareEnabled(GetLineGaugeSelectedIndex(m_intPadSelectedIndex)))
                //{
                //    chk_EnableThreshold.Enabled = false;
                //}
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.ref_GaugeTransType = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(m_intLineGaugeSelectedIndex);
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.ref_GaugeTransChoice = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransChoice(m_intLineGaugeSelectedIndex);
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_Derivative.Text);
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.ref_GaugeMinArea = Convert.ToInt32(txt_MeasMinArea.Text);
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.ref_GaugeMinAmplitude = Convert.ToInt32(txt_MeasMinAmp.Text);
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.ref_GaugeFilter = Convert.ToInt32(txt_MeasFilter.Text);
                //int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(m_intPadSelectedIndex);
                //switch (LineGaugeSelectedIndex)
                //{
                //    case 0:
                //        m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                //            m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                //            Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);//90 * (1 + GetLineGaugeSelectedIndex(m_intPadSelectedIndex))
                //        break;
                //    case 1:
                //        m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                //           m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                //           Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);//90 * (1 + GetLineGaugeSelectedIndex(m_intPadSelectedIndex))
                //        break;
                //    case 2:
                //        m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
                //          m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                //           Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);//90 * (1 + GetLineGaugeSelectedIndex(m_intPadSelectedIndex))
                //        break;
                //    case 3:
                //        m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                //         m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
                //           Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);//90 * (1 + GetLineGaugeSelectedIndex(m_intPadSelectedIndex))
                //        break;
                //}
                //m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][1]);
                UpdateGUIPicture();
                m_blnUpdateHistogram = true;
            }
        }

        private void radioBtn_Left_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnBlockOtherControlUpdate = true;

            if (sender == radioBtn_Up)
            {
                m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(m_intPadSelectedIndex);
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoTop.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                DefineEdgeEnableMask(cbo_ImageNoTop.SelectedIndex);
            }
            else if (sender == radioBtn_Right)
            {
                m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(m_intPadSelectedIndex);
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoRight.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                DefineEdgeEnableMask(cbo_ImageNoRight.SelectedIndex);
            }
            else if (sender == radioBtn_Down)
            {
                m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(m_intPadSelectedIndex);
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoBottom.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                DefineEdgeEnableMask(cbo_ImageNoBottom.SelectedIndex);
            }
            else if (sender == radioBtn_Left)
            {
                m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(m_intPadSelectedIndex);
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoLeft.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                DefineEdgeEnableMask(cbo_ImageNoLeft.SelectedIndex);
            }

            UpdateSelectedGaugeEdgeMask();

            UpdateGaugeToGUI();

            UpdatePackageImage();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_blnBlockOtherControlUpdate = false;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:

                    string strPointGaugePath = m_strVisionFolderPath + "\\Pad\\PointGauge.xml";

                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0x0F;

                        m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.LoadRectGauge4LMeasurementSettingOnly(m_strPath,  // 2019 10 21 - CCENG: Load Gauge setting roi, dun load edge roi setting.
                                "Pad" + i.ToString());

                        // Set RectGauge4L Placement
                        m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SetGaugePlace_BasedOnEdgeROI();
                        m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], m_smVisionInfo.g_objWhiteImage);

                        // Reset Point Gauge to inspection setting.
                        m_smVisionInfo.g_arrPad[i].ref_objPointGauge.LoadPointGauge(strPointGaugePath,
                                                            "Pad" + i.ToString());
                        // Permanent set minAmp = 0, min area = 0, filter to 1, choice = from begin
                        //m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeTransChoice = 0;       // 2019-09-26 ZJYEOH : User now can select which TransChoice to use
                        //m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeMinAmplitude = 0;      // 2019-12-21 CCENG : No more fix Min Amplitude to 0
                        m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeMinArea = 0;
                        m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeFilter = 1;
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
            //Dispose();
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
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeMinAmplitude(Convert.ToInt32(txt_MeasMinAmp.Text), j);
                                }
                            }
                        }
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
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeMinArea(Convert.ToInt32(txt_MeasMinArea.Text), j);
                                }
                            }
                        }
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

        private void txt_Derivative_TextChanged(object sender, EventArgs e)
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
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeThreshold(Convert.ToInt32(txt_Derivative.Text), j);
                                }
                            }
                        }
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeThreshold(Convert.ToInt32(txt_Derivative.Text), intLineGaugeSelectedIndex);

                        }
                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_Derivative.Text);
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
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeThreshold(Convert.ToInt32(txt_Derivative.Text));
                    break;
            }

            trackBar_Derivative.Value = Convert.ToInt32(txt_Derivative.Text);
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
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeThickness(Convert.ToInt32(txt_MeasThickness.Text), j);
                                }
                            }                            
                        }
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
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeFilter(Convert.ToInt32(txt_MeasFilter.Text), j);
                                }
                            }
                        }
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
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeFilterPasses(Convert.ToInt32(txt_FilteringPass.Text), j);
                                }
                            }
                            
                        }
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
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeFilterThreshold(Convert.ToSingle(txt_FilteringThreshold.Text), j);
                                }
                            }
                            
                        }
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
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeTransType(intTransTypeIndex, j);
                                }
                            }
                        }
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
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeTransChoice(intTransChoiceIndex, j);
                                }
                            }
                            
                        }
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
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetDirection(radioBtn_InOut.Checked, j);
                                }
                            }
                        }
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
            //if (!m_blnInitDone)
            //    return;

            //if (m_blnBlockOtherControlUpdate)
            //    return;

            //switch (m_eReadFrom)
            //{
            //    case ReadFrom.Pad:
            //        int intPadSelectedIndex = 0;
            //        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //        {
            //            if (m_intPadSelectedIndex == 0)
            //            {
            //                intPadSelectedIndex = m_intPadSelectedIndex;
            //            }
            //            else
            //            {
            //                if (i == 0)
            //                    continue;

            //                if (chk_ApplyToAllSideROI.Checked)
            //                {
            //                    intPadSelectedIndex = i;
            //                }
            //                else
            //                {
            //                    intPadSelectedIndex = m_intPadSelectedIndex;
            //                }
            //            }

            //            if (m_blnSetToAll)
            //                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeTolerance(Convert.ToSingle(txt_Tolerance.Text));
            //            else
            //            {
            //                int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
            //                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeTolerance(Convert.ToSingle(txt_Tolerance.Text), intLineGaugeSelectedIndex);
            //            }
            //            int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
            //            switch (LineGaugeSelectedIndex)
            //            {
            //                case 0:
            //                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
            //                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
            //                        Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
            //                    break;
            //                case 1:
            //                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
            //                       m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
            //                       Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
            //                    break;
            //                case 2:
            //                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterX,
            //                      m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalY + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
            //                       Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
            //                    break;
            //                case 3:
            //                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
            //                     m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[LineGaugeSelectedIndex].ref_ROITotalCenterY,
            //                       Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(LineGaugeSelectedIndex) + 90);
            //                    break;
            //            }
            //            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);
            //            if (!chk_ApplyToAllSideROI.Checked)
            //                break;
            //        }
            //        break;
            //    case ReadFrom.Calibration:
            //        for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
            //            m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeTolerance(Convert.ToSingle(txt_Tolerance.Text));
            //        break;
            //}
            //m_blnUpdateHistogram = true;
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeSamplingStep(Convert.ToSingle(txt_SamplingStep.Text), j);
                                }
                            }
                        }
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
            SaveSetting(false);
        }

        private void SaveSetting(bool blnSetSizeAsReference)
        {
            {
                switch (m_eReadFrom)
                {
                    case ReadFrom.Pad:
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                    break;

                                if (i > 0)
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
                                blnSetSizeAsReference); // 2021 12 15 - CCENG: Dun save template size here. Save template size when user press Save + Set Size As Ref button. 
                            }

                            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                                m_smProductionInfo.g_blnSaveRecipeToServer = true;

                            string strPointGaugePath = m_strVisionFolderPath + "\\Pad\\PointGauge.xml";
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                    break;

                                // Reset Point Gauge to inspection setting.
                                m_smVisionInfo.g_arrPad[i].ref_objPointGauge.LoadPointGauge(strPointGaugePath,
                                                                "Pad" + i.ToString());
                                // Permanent set minAmp = 0, min area = 0, filter to 1, choice = from begin
                                //m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeTransChoice = 0;       // 2019-09-26 ZJYEOH : User now can select which TransChoice to use
                                //m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeMinAmplitude = 0;      // 2019-12-21 CCENG : No more fix Min Amplitude to 0
                                m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeMinArea = 0;
                                m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeFilter = 1;
                            }

                            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;
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
                Close();
                //Dispose();
            }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intPadIndex"></param>
        /// <param name="intLineGaugeDirection"></param>
        private int GetLineGaugeIndex(int intPadIndex, int intLineGaugeDirection)
        {
            switch (intPadIndex)
            {
                case 0:
                case 1: // Top ROI
                    {
                        if (intLineGaugeDirection == 0)
                            return 0;
                        else if (intLineGaugeDirection == 1)
                            return 1;
                        else if (intLineGaugeDirection == 2)
                            return 2;
                        else if (intLineGaugeDirection == 3)
                            return 3;
                    }
                    break;
                case 2: // Right ROI
                    {
                        if (intLineGaugeDirection == 0)
                            return 1;
                        else if (intLineGaugeDirection == 1)
                            return 2;
                        else if (intLineGaugeDirection == 2)
                            return 3;
                        else if (intLineGaugeDirection == 3)
                            return 0;
                    }
                    break;
                case 3: // Bottom ROI
                    {
                        if (intLineGaugeDirection == 0)
                            return 2;
                        else if (intLineGaugeDirection == 1)
                            return 3;
                        else if (intLineGaugeDirection == 2)
                            return 0;
                        else if (intLineGaugeDirection == 3)
                            return 1;
                    }
                    break;
                case 4: // Left ROI
                    {
                        if (intLineGaugeDirection == 0)
                            return 3;
                        else if (intLineGaugeDirection == 1)
                            return 0;
                        else if (intLineGaugeDirection == 2)
                            return 1;
                        else if (intLineGaugeDirection == 3)
                            return 2;
                    }
                    break;
            }

            return 0;
        }
        private int GetImageIndex(int intPadIndex, int intLineGaugeDirection)
        {
            switch (intPadIndex)
            {
                case 0:
                case 1: // Top ROI
                    {
                        if (intLineGaugeDirection == 0)
                            return cbo_ImageNoTop.SelectedIndex;
                        else if (intLineGaugeDirection == 1)
                            return cbo_ImageNoRight.SelectedIndex;
                        else if (intLineGaugeDirection == 2)
                            return cbo_ImageNoBottom.SelectedIndex;
                        else if (intLineGaugeDirection == 3)
                            return cbo_ImageNoLeft.SelectedIndex;
                    }
                    break;
                case 2: // Right ROI
                    {
                        if (intLineGaugeDirection == 0)
                            return cbo_ImageNoRight.SelectedIndex;
                        else if (intLineGaugeDirection == 1)
                            return cbo_ImageNoBottom.SelectedIndex;
                        else if (intLineGaugeDirection == 2)
                            return cbo_ImageNoLeft.SelectedIndex;
                        else if (intLineGaugeDirection == 3)
                            return cbo_ImageNoTop.SelectedIndex;
                    }
                    break;
                case 3: // Bottom ROI
                    {
                        if (intLineGaugeDirection == 0)
                            return cbo_ImageNoBottom.SelectedIndex;
                        else if (intLineGaugeDirection == 1)
                            return cbo_ImageNoLeft.SelectedIndex;
                        else if (intLineGaugeDirection == 2)
                            return cbo_ImageNoTop.SelectedIndex;
                        else if (intLineGaugeDirection == 3)
                            return cbo_ImageNoRight.SelectedIndex;
                    }
                    break;
                case 4: // Left ROI
                    {
                        if (intLineGaugeDirection == 0)
                            return cbo_ImageNoLeft.SelectedIndex;
                        else if (intLineGaugeDirection == 1)
                            return cbo_ImageNoTop.SelectedIndex;
                        else if (intLineGaugeDirection == 2)
                            return cbo_ImageNoRight.SelectedIndex;
                        else if (intLineGaugeDirection == 3)
                            return cbo_ImageNoBottom.SelectedIndex;
                    }
                    break;
            }

            return 0;
        }
        private void chk_ApplyToAllSideROI_Click(object sender, EventArgs e)
        {
            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnBlockOtherControlUpdate = true;

            UpdateSelectedGaugeEdgeMask();

            UpdateGaugeToGUI();

            UpdatePackageImage();

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

                //2020-11-20 ZJYEOH : Update label color to Red if setting not same
                CheckSettingSame();

                m_blnUpdateHistogram = false;
                m_smVisionInfo.AT_VM_UpdateHistogram = false;
            }

            if (m_blnWantTopMost)
            {
                if (!this.TopMost)
                    this.TopMost = true;
            }
            else
            {
                if (this.TopMost)
                    this.TopMost = false;
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

            txt_Derivative.Text = trackBar_Derivative.Value.ToString();
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
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeMinScore(Convert.ToInt32(txt_MinScore.Text), j);
                                }
                            }
                        }
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

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeMaxAngle(Convert.ToInt32(txt_MaxAngle.Text), j);
                                }
                            }
                            
                        }
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

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_ImageMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

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
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeImageMode(cbo_ImageMode.SelectedIndex, j);
                        }
                    }
                }
                else
                {
                    int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeImageMode(cbo_ImageMode.SelectedIndex, intLineGaugeSelectedIndex);
                }

                //// ----------------- Update image base on selection of image mode -------------------------------------

                //switch (m_smVisionInfo.g_arrPad[0].ref_objRectGauge4L.GetGaugeImageMode(0))
                //{
                //    default:
                //    case 0:
                //        {
                //            if (m_smVisionInfo.g_blnViewRotatedImage)
                //            {
                //                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_fImageGain);
                //            }
                //            else
                //            {
                //                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_fImageGain);
                //            }
                //        }
                //        break;
                //    case 1:
                //        {
                //            if (m_smVisionInfo.g_blnViewRotatedImage)
                //            {
                //                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPkgProcessImage, m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_fImageGain);
                //                m_smVisionInfo.g_objPkgProcessImage.AddPrewitt(ref m_smVisionInfo.g_objPackageImage);
                //            }
                //            else
                //            {
                //                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPkgProcessImage, m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_fImageGain);
                //                m_smVisionInfo.g_objPkgProcessImage.AddPrewitt(ref m_smVisionInfo.g_objPackageImage);
                //            }

                //        }
                //        break;
                //}

                //// ----------------------------------------------------------------------------------------------------


                //int LineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                //switch (LineGaugeSelectedIndex)
                //{
                //    case 0:
                //        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalCenterX,
                //            m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                //            Convert.ToSingle(txt_Tolerance.Text), 90 * (1 + GetLineGaugeSelectedIndex(intPadSelectedIndex)));
                //        break;
                //    case 1:
                //        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                //           m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalCenterY,
                //           Convert.ToSingle(txt_Tolerance.Text), 90 * (1 + GetLineGaugeSelectedIndex(intPadSelectedIndex)));
                //        break;
                //    case 2:
                //        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalCenterX,
                //           m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                //           Convert.ToSingle(txt_Tolerance.Text), 90 * (1 + GetLineGaugeSelectedIndex(intPadSelectedIndex)));
                //        break;
                //    case 3:
                //        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                //           m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalCenterY,
                //           Convert.ToSingle(txt_Tolerance.Text), 90 * (1 + GetLineGaugeSelectedIndex(intPadSelectedIndex)));
                //        break;
                //}
                //m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);
                if (!chk_ApplyToAllSideROI.Checked)
                    break;
            }

            UpdatePackageImage();

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_UndoDontCareROI_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnWantTopMost = false;

            DontCareAreaSettingForm objDontCareAreaSettingForm = new DontCareAreaSettingForm(m_smVisionInfo, m_smProductionInfo, m_smCustomizeInfo, m_strPath);
            objDontCareAreaSettingForm.ShowDialog();

            m_blnWantTopMost = true;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            objDontCareAreaSettingForm.Dispose();

            //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //{
            //    for (int j = 0; j < 4; j++)
            //    {
            //        if ((m_arrEdgeEnableMask[i] & (0x01 << j)) > 0)
            //        {
            //            if (m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetAutoDontCareEnabled(j))
            //            {
            //                m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SetGaugeWantImageThreshold(true, j);
            //            }
            //        }
            //    }
            //}

            m_blnBlockOtherControlUpdate = true;

            UpdateGaugeToGUI();

            UpdatePackageImage();

            m_blnBlockOtherControlUpdate = false;

        }

        private void cbo_ROI_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnBlockOtherControlUpdate = true;

            m_smVisionInfo.g_intSelectedROI = m_intPadSelectedIndex = cbo_ROI.SelectedIndex;

            //2020-05-27 ZJYEOH : add one index to selected ROI if measure center pkg using side pkg
            if(m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides && (m_eReadFrom == ReadFrom.Pad))
                m_smVisionInfo.g_intSelectedROI = m_intPadSelectedIndex = cbo_ROI.SelectedIndex + 1;

            m_smVisionInfo.g_intSelectedImage = cbo_ImageNoTop.SelectedIndex = Math.Min(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageNo(GetLineGaugeIndex(m_intPadSelectedIndex, 0)), cbo_ImageNoTop.Items.Count - 1);
            cbo_ImageNoRight.SelectedIndex = Math.Min(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageNo(GetLineGaugeIndex(m_intPadSelectedIndex, 1)), cbo_ImageNoRight.Items.Count - 1);
            cbo_ImageNoBottom.SelectedIndex = Math.Min(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageNo(GetLineGaugeIndex(m_intPadSelectedIndex, 2)), cbo_ImageNoBottom.Items.Count - 1);
            cbo_ImageNoLeft.SelectedIndex = Math.Min(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageNo(GetLineGaugeIndex(m_intPadSelectedIndex, 3)), cbo_ImageNoLeft.Items.Count - 1);
            
            DefineEdgeEnableMask();

            UpdateEdgeRadioButtonGUI();

            UpdateSelectedGaugeEdgeMask();


            ReUpdateGUI();

            m_blnBlockOtherControlUpdate = false;

            m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(m_intPadSelectedIndex);
            if (m_intLineGaugeSelectedIndex == 0)
            {
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoTop.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                //DefineEdgeEnableMask(cbo_ImageNoTop.SelectedIndex);
                radioBtn_Up.PerformClick();
            }
            else if (m_intLineGaugeSelectedIndex == 1)
            {
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoRight.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                //DefineEdgeEnableMask(cbo_ImageNoRight.SelectedIndex);
                radioBtn_Right.PerformClick();
            }
            else if (m_intLineGaugeSelectedIndex == 2)
            {
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoBottom.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                //DefineEdgeEnableMask(cbo_ImageNoBottom.SelectedIndex);
                radioBtn_Down.PerformClick();
            }
            else if (m_intLineGaugeSelectedIndex == 3)
            {
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoLeft.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                //DefineEdgeEnableMask(cbo_ImageNoLeft.SelectedIndex);
                radioBtn_Left.PerformClick();
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            
        }

        private void btn_ImageNo_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnBlockOtherControlUpdate = true;

            m_blnWantTopMost = false;

            RectGauge4LImageForm objForm = new RectGauge4LImageForm(m_smVisionInfo, m_smProductionInfo, m_smCustomizeInfo);
            objForm.ShowDialog();

            m_blnWantTopMost = true;

            if (objForm.DialogResult == DialogResult.OK)
            {
                if (m_smVisionInfo.g_arrPad.Length > m_intPadSelectedIndex)
                {
                    cbo_ImageNoTop.SelectedIndex = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].GetEdgeImageViewNo(GetLineGaugeIndex(m_intPadSelectedIndex, 0));
                    cbo_ImageNoRight.SelectedIndex = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].GetEdgeImageViewNo(GetLineGaugeIndex(m_intPadSelectedIndex, 1));
                    cbo_ImageNoBottom.SelectedIndex = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].GetEdgeImageViewNo(GetLineGaugeIndex(m_intPadSelectedIndex, 2));
                    cbo_ImageNoLeft.SelectedIndex = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].GetEdgeImageViewNo(GetLineGaugeIndex(m_intPadSelectedIndex, 3));
                }
                
                if (m_intLineGaugeSelectedIndex == 0)
                {
                    m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoTop.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                    DefineEdgeEnableMask(cbo_ImageNoTop.SelectedIndex);
                }
                else if (m_intLineGaugeSelectedIndex == 1)
                {
                    m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoRight.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                    DefineEdgeEnableMask(cbo_ImageNoRight.SelectedIndex);
                }
                else if (m_intLineGaugeSelectedIndex == 2)
                {
                    m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoBottom.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                    DefineEdgeEnableMask(cbo_ImageNoBottom.SelectedIndex);
                }
                else if (m_intLineGaugeSelectedIndex == 3)
                {
                    m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoLeft.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                    DefineEdgeEnableMask(cbo_ImageNoLeft.SelectedIndex);
                }
            }
            //DefineEdgeEnableMask();

            UpdateEdgeRadioButtonGUI();

            UpdateSelectedGaugeEdgeMask();

            ReUpdateGUI();

            m_blnBlockOtherControlUpdate = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            objForm.Dispose();

        }

        private void txt_ImageGain_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

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

                float fImageGain = Convert.ToSingle(txt_ImageGain.Value);

                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeImageGain(fImageGain, j);
                        }
                    }
                }
                else
                {
                    int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeImageGain(fImageGain, intLineGaugeSelectedIndex);
                }

                if (!chk_ApplyToAllSideROI.Checked)
                    break;
            }

            UpdatePackageImage();

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void cbo_ImageNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnBlockOtherControlUpdate = true;

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNo.SelectedIndex, m_smVisionInfo.g_intVisionIndex);

          //  UpdatePackageImage();

            DefineEdgeEnableMask();

            UpdateEdgeRadioButtonGUI();

            UpdateSelectedGaugeEdgeMask();

            ReUpdateGUI();

            UpdatePackageImage();

            m_blnBlockOtherControlUpdate = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void DefineEdgeEnableMask()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                m_arrEdgeEnableMask[i] = 0;
                if (m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetGaugeImageNo(0) == GetImageIndex(m_intPadSelectedIndex, 0))
                {
                    m_arrEdgeEnableMask[i] |= 0x01;
                }

                if (m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetGaugeImageNo(1) == GetImageIndex(m_intPadSelectedIndex, 1))
                {
                    m_arrEdgeEnableMask[i] |= 0x02;
                }

                if (m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetGaugeImageNo(2) == GetImageIndex(m_intPadSelectedIndex, 2))
                {
                    m_arrEdgeEnableMask[i] |= 0x04;
                }

                if (m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetGaugeImageNo(3) == GetImageIndex(m_intPadSelectedIndex, 3))
                {
                    m_arrEdgeEnableMask[i] |= 0x08;
                }
            }
        }
        private void DefineEdgeEnableMask(int intSelectedImageIndex)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                m_arrEdgeEnableMask[i] = 0;
                if (m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetGaugeImageNo(0) == intSelectedImageIndex)
                {
                    m_arrEdgeEnableMask[i] |= 0x01;
                }

                if (m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetGaugeImageNo(1) == intSelectedImageIndex)
                {
                    m_arrEdgeEnableMask[i] |= 0x02;
                }

                if (m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetGaugeImageNo(2) == intSelectedImageIndex)
                {
                    m_arrEdgeEnableMask[i] |= 0x04;
                }

                if (m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetGaugeImageNo(3) == intSelectedImageIndex)
                {
                    m_arrEdgeEnableMask[i] |= 0x08;
                }
            }
        }

        private void UpdatePackageImage()
        {
            if (m_smVisionInfo.g_blnViewRotatedImage || (m_intVisionType == 0 && ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)))
            {
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            }
            else
            {
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            }

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

                if (m_smVisionInfo.g_blnViewRotatedImage || (m_intVisionType == 0 && ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)))
                {
                    if (m_blnSetToAll)
                    {
                        ImageDrawing objSourceImage = m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage];
                        for (int j = 0; j < 4; j++)
                        {
                            if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageMode(j) == 0)
                                {

                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, j);
                                    if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeWantImageThreshold(j))
                                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.AddOpenCloseForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPkgProcessImage, j);
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddPrewittForEdgeROI(ref m_smVisionInfo.g_objPkgProcessImage, ref m_smVisionInfo.g_objPackageImage, j);
                                }
                            }
                        }
                    }
                    else
                    {
                        int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                        ImageDrawing objSourceImage = m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage];
                        if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageMode(intLineGaugeSelectedIndex) == 0)
                        {
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                            if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeWantImageThreshold(intLineGaugeSelectedIndex))
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.AddOpenCloseForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPkgProcessImage, intLineGaugeSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddPrewittForEdgeROI(ref m_smVisionInfo.g_objPkgProcessImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);

                        }
                    }
                }
                else
                {
                    if (m_blnSetToAll)
                    {
                        ImageDrawing objSourceImage = m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage];
                        for (int j = 0; j < 4; j++)
                        {
                            if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageMode(j) == 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, j);
                                    if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeWantImageThreshold(j))
                                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.AddOpenCloseForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPkgProcessImage, j);
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddPrewittForEdgeROI(ref m_smVisionInfo.g_objPkgProcessImage, ref m_smVisionInfo.g_objPackageImage, j);
                                }
                            }
                            
                        }
                    }
                    else
                    {
                        int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                        ImageDrawing objSourceImage = m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage];
                        if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageMode(intLineGaugeSelectedIndex) == 0)
                        {
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                            if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeWantImageThreshold(intLineGaugeSelectedIndex))
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.AddOpenCloseForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPkgProcessImage, intLineGaugeSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddPrewittForEdgeROI(ref m_smVisionInfo.g_objPkgProcessImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                        }
                    }
                }

                if (!chk_ApplyToAllSideROI.Checked)
                    break;
            }

        }

        private void chk_SetToAllEdges_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnBlockOtherControlUpdate = true;

            UpdateSelectedGaugeEdgeMask();

            UpdateGaugeToGUI();

            UpdatePackageImage();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_blnBlockOtherControlUpdate = false;
        }

        private void UpdateSelectedGaugeEdgeMask()
        {
            if (chk_SetToAllEdges.Checked)
            {
                m_blnSetToAll = true;

                if (m_intPadSelectedIndex == 0)
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i == 0)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = m_arrEdgeEnableMask[i];
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
                    }
                }
                else
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i == 0)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
                        }
                        else if (chk_ApplyToAllSideROI.Checked || i == m_intPadSelectedIndex)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = m_arrEdgeEnableMask[i];
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
                    }
                }
            }
            else
            {
                m_blnSetToAll = false;

                if (m_intPadSelectedIndex == 0)
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i == 0)
                        {
                            m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(0);
                            m_smVisionInfo.g_arrPad[0].ref_intSelectedGaugeEdgeMask = (m_arrEdgeEnableMask[0] & (0x01 << m_intLineGaugeSelectedIndex));
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
                    }

                }
                else
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i == 0)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;
                        }
                        else if (chk_ApplyToAllSideROI.Checked || i == m_intPadSelectedIndex)
                        {
                            m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(i);
                            m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = (m_arrEdgeEnableMask[i] & (0x01 << m_intLineGaugeSelectedIndex));
                        }
                        else
                            m_smVisionInfo.g_arrPad[i].ref_intSelectedGaugeEdgeMask = 0;

                    }
                }
            }
        }

        private void UpdateEdgeRadioButtonGUI()
        {
            if (m_intPadSelectedIndex < m_smVisionInfo.g_arrPad.Length)
            {
                // ----- Image No -----
                bool blnAlreadyChecked = false;
                int intSelectedEdge = -1;
                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageNo(GetLineGaugeIndex(m_intPadSelectedIndex, 0)) == cbo_ImageNoTop.SelectedIndex)
                {
                    if (intSelectedEdge == -1)
                        intSelectedEdge = 0;

                    radioBtn_Up.Enabled = true;

                    if (radioBtn_Up.Checked)
                        blnAlreadyChecked = true;
                }
                else
                {
                    radioBtn_Up.Enabled = false;
                }

                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageNo(GetLineGaugeIndex(m_intPadSelectedIndex, 1)) == cbo_ImageNoRight.SelectedIndex)
                {
                    if (intSelectedEdge == -1)
                        intSelectedEdge = 1;

                    radioBtn_Right.Enabled = true;

                    if (radioBtn_Right.Checked)
                        blnAlreadyChecked = true;
                }
                else
                {
                    radioBtn_Right.Enabled = false;
                }

                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageNo(GetLineGaugeIndex(m_intPadSelectedIndex, 2)) == cbo_ImageNoBottom.SelectedIndex)
                {
                    if (intSelectedEdge == -1)
                        intSelectedEdge = 2;

                    radioBtn_Down.Enabled = true;

                    if (radioBtn_Down.Checked)
                        blnAlreadyChecked = true;
                }
                else
                {
                    radioBtn_Down.Enabled = false;
                }

                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageNo(GetLineGaugeIndex(m_intPadSelectedIndex, 3)) == cbo_ImageNoLeft.SelectedIndex)
                {
                    if (intSelectedEdge == -1)
                        intSelectedEdge = 3;

                    radioBtn_Left.Enabled = true;

                    if (radioBtn_Left.Checked)
                        blnAlreadyChecked = true;
                }
                else
                {
                    radioBtn_Left.Enabled = false;
                }

                if (!blnAlreadyChecked)
                {
                    if (intSelectedEdge >= 0)
                    {
                        switch (intSelectedEdge)
                        {
                            case 0:
                                radioBtn_Up.Checked = true;
                                break;
                            case 1:
                                radioBtn_Right.Checked = true;
                                break;
                            case 2:
                                radioBtn_Down.Checked = true;
                                break;
                            case 3:
                                radioBtn_Left.Checked = true;
                                break;
                        }
                    }
                }
            }
        }

        private void AdvancedRectGauge4LPadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_blnShowForm = false;
        }

        private void AdvancedRectGauge4LPadForm_Load(object sender, EventArgs e)
        {
            m_blnShowForm = true;
        }

        private void cbo_ImageNoTop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnBlockOtherControlUpdate = true;

            if (m_smVisionInfo.g_arrPad.Length > m_intPadSelectedIndex)
            {
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].SetEdgeImageViewNo(cbo_ImageNoTop.SelectedIndex, GetLineGaugeIndex(m_intPadSelectedIndex, 0));
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].SetEdgeImageViewNo(cbo_ImageNoRight.SelectedIndex, GetLineGaugeIndex(m_intPadSelectedIndex, 1));
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].SetEdgeImageViewNo(cbo_ImageNoBottom.SelectedIndex, GetLineGaugeIndex(m_intPadSelectedIndex, 2));
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].SetEdgeImageViewNo(cbo_ImageNoLeft.SelectedIndex, GetLineGaugeIndex(m_intPadSelectedIndex, 3));
            }

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoTop.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
            
            DefineEdgeEnableMask(cbo_ImageNoTop.SelectedIndex);

            UpdateEdgeRadioButtonGUI();

            UpdateSelectedGaugeEdgeMask();

            ReUpdateGUI();

            UpdatePackageImage();

            radioBtn_Up.PerformClick();

            m_blnBlockOtherControlUpdate = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_ImageNoRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnBlockOtherControlUpdate = true;

            if (m_smVisionInfo.g_arrPad.Length > m_intPadSelectedIndex)
            {
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].SetEdgeImageViewNo(cbo_ImageNoTop.SelectedIndex, GetLineGaugeIndex(m_intPadSelectedIndex, 0));
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].SetEdgeImageViewNo(cbo_ImageNoRight.SelectedIndex, GetLineGaugeIndex(m_intPadSelectedIndex, 1));
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].SetEdgeImageViewNo(cbo_ImageNoBottom.SelectedIndex, GetLineGaugeIndex(m_intPadSelectedIndex, 2));
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].SetEdgeImageViewNo(cbo_ImageNoLeft.SelectedIndex, GetLineGaugeIndex(m_intPadSelectedIndex, 3));
            }

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoRight.SelectedIndex, m_smVisionInfo.g_intVisionIndex);

            DefineEdgeEnableMask(cbo_ImageNoRight.SelectedIndex);

            UpdateEdgeRadioButtonGUI();

            UpdateSelectedGaugeEdgeMask();

            ReUpdateGUI();

            UpdatePackageImage();

            radioBtn_Right.PerformClick();

            m_blnBlockOtherControlUpdate = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_ImageNoBottom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnBlockOtherControlUpdate = true;

            if (m_smVisionInfo.g_arrPad.Length > m_intPadSelectedIndex)
            {
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].SetEdgeImageViewNo(cbo_ImageNoTop.SelectedIndex, GetLineGaugeIndex(m_intPadSelectedIndex, 0));
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].SetEdgeImageViewNo(cbo_ImageNoRight.SelectedIndex, GetLineGaugeIndex(m_intPadSelectedIndex, 1));
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].SetEdgeImageViewNo(cbo_ImageNoBottom.SelectedIndex, GetLineGaugeIndex(m_intPadSelectedIndex, 2));
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].SetEdgeImageViewNo(cbo_ImageNoLeft.SelectedIndex, GetLineGaugeIndex(m_intPadSelectedIndex, 3));
            }

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoBottom.SelectedIndex, m_smVisionInfo.g_intVisionIndex);

            DefineEdgeEnableMask(cbo_ImageNoBottom.SelectedIndex);

            UpdateEdgeRadioButtonGUI();

            UpdateSelectedGaugeEdgeMask();

            ReUpdateGUI();

            UpdatePackageImage();

            radioBtn_Down.PerformClick();

            m_blnBlockOtherControlUpdate = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_ImageNoLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnBlockOtherControlUpdate = true;

            if (m_smVisionInfo.g_arrPad.Length > m_intPadSelectedIndex)
            {
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].SetEdgeImageViewNo(cbo_ImageNoTop.SelectedIndex, GetLineGaugeIndex(m_intPadSelectedIndex, 0));
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].SetEdgeImageViewNo(cbo_ImageNoRight.SelectedIndex, GetLineGaugeIndex(m_intPadSelectedIndex, 1));
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].SetEdgeImageViewNo(cbo_ImageNoBottom.SelectedIndex, GetLineGaugeIndex(m_intPadSelectedIndex, 2));
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].SetEdgeImageViewNo(cbo_ImageNoLeft.SelectedIndex, GetLineGaugeIndex(m_intPadSelectedIndex, 3));
            }

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoLeft.SelectedIndex, m_smVisionInfo.g_intVisionIndex);

            DefineEdgeEnableMask(cbo_ImageNoLeft.SelectedIndex);

            UpdateEdgeRadioButtonGUI();

            UpdateSelectedGaugeEdgeMask();

            ReUpdateGUI();

            UpdatePackageImage();

            radioBtn_Left.PerformClick();

            m_blnBlockOtherControlUpdate = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_EnableSubGauge_CheckedChanged(object sender, EventArgs e)
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
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeEnableSubGauge(chk_EnableSubGauge.Checked, j);
                                }
                            }

                        }
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeEnableSubGauge(chk_EnableSubGauge.Checked, intLineGaugeSelectedIndex);
                        }

                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }
                    break;
                case ReadFrom.Calibration:
                    //for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                    //    m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeEnableSubGauge(Convert.ToInt32(txt_MaxAngle.Text));
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Threshold_TextChanged(object sender, EventArgs e)
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
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeImageThreshold(Convert.ToInt32(txt_Threshold.Text), j);
                                }
                            }
                        }
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeImageThreshold(Convert.ToInt32(txt_Threshold.Text), intLineGaugeSelectedIndex);

                        }
                        //m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.ref_GaugeImageThreshold = Convert.ToInt32(txt_Threshold.Text);
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
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeImageThreshold(Convert.ToInt32(txt_Threshold.Text));
                    break;
            }
            trackBar_Threshold.Value = Convert.ToInt32(txt_Threshold.Text);

            UpdatePackageImage();
            
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Gain_TextChanged(object sender, EventArgs e)
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

                        float fImageGain = Convert.ToSingle(txt_Gain.Text);

                        if (m_blnSetToAll)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeImageGain(fImageGain, j);
                                }
                            }
                        }
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeImageGain(fImageGain, intLineGaugeSelectedIndex);
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
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeImageGain(Convert.ToSingle(txt_Gain.Text));
                    break;
            }
            trackBar_Gain.Value = Convert.ToInt32(Convert.ToSingle(txt_Gain.Text) * 10f);

            UpdatePackageImage();

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void trackBar_Threshold_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Threshold.Text = trackBar_Threshold.Value.ToString();
        }

        private void trackBar_Gain_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Gain.Text = (Convert.ToSingle(trackBar_Gain.Value) / 10).ToString();
        }

        private void chk_EnableThreshold_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;
            
            txt_Threshold.Enabled = chk_EnableThreshold.Checked;
            trackBar_Threshold.Enabled = chk_EnableThreshold.Checked;

            float fImageGain = Convert.ToSingle(txt_Gain.Text);
            int intImageThreshld = Convert.ToInt32(txt_Threshold.Text);
            
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
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMeasureMode(j) != 2 || intPadSelectedIndex == 0)// && !m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetAutoDontCareEnabled(j))
                                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeWantImageThreshold(chk_EnableThreshold.Checked, j);
                                    else if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMeasureMode(j) == 2 && (j == 0 || j == 2) && (intPadSelectedIndex == 0 || intPadSelectedIndex == 1 || intPadSelectedIndex == 3))
                                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeWantImageThreshold(chk_EnableThreshold.Checked, j);
                                    else if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMeasureMode(j) == 2 && (j == 1 || j == 3) && (intPadSelectedIndex == 2 || intPadSelectedIndex == 4))
                                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeWantImageThreshold(chk_EnableThreshold.Checked, j);
                                    
                                }
                            }
                        }
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMeasureMode(intLineGaugeSelectedIndex) != 2 || intPadSelectedIndex == 0)// && !m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetAutoDontCareEnabled(intLineGaugeSelectedIndex))
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeWantImageThreshold(chk_EnableThreshold.Checked, intLineGaugeSelectedIndex);
                            else if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMeasureMode(intLineGaugeSelectedIndex) == 2 && (intLineGaugeSelectedIndex == 0 || intLineGaugeSelectedIndex == 2) && (intPadSelectedIndex == 0 || intPadSelectedIndex == 1 || intPadSelectedIndex == 3))
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeWantImageThreshold(chk_EnableThreshold.Checked, intLineGaugeSelectedIndex);
                            else if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMeasureMode(intLineGaugeSelectedIndex) == 2 && (intLineGaugeSelectedIndex == 1 || intLineGaugeSelectedIndex == 3) && (intPadSelectedIndex == 2 || intPadSelectedIndex == 4))
                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeWantImageThreshold(chk_EnableThreshold.Checked, intLineGaugeSelectedIndex);

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
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeWantImageThreshold(chk_EnableThreshold.Checked);
                    break;
            }
                        UpdatePackageImage();

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_OpenClose_ValueChanged(object sender, EventArgs e)
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
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeImageOpenCloseIteration(Convert.ToInt32(txt_OpenClose.Value), j);
                                }
                            }
                        }
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeImageOpenCloseIteration(Convert.ToInt32(txt_OpenClose.Value), intLineGaugeSelectedIndex);

                        }
                        //m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.ref_GaugeImageThreshold = Convert.ToInt32(txt_Threshold.Text);
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
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeImageOpenCloseIteration(Convert.ToInt32(txt_OpenClose.Value));
                    break;
            }

            if (Convert.ToInt32(txt_OpenClose.Value) > 0)
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    lbl_OpenClose.Text = "Open Iteration:";
                else
                    lbl_OpenClose.Text = "扩展:";
            }
            else if (Convert.ToInt32(txt_OpenClose.Value) < 0)
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    lbl_OpenClose.Text = "Close Iteration:";
                else
                    lbl_OpenClose.Text = "闭合:";
            }
            else
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    lbl_OpenClose.Text = "No Iteration:";
                else
                    lbl_OpenClose.Text = "无图像处理:";
            }
            UpdatePackageImage();

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_AutoTuning_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            AutoTuneGaugeDrawForm objAutoTuneGaugeDrawForm = new AutoTuneGaugeDrawForm(m_smVisionInfo, m_smProductionInfo, m_smCustomizeInfo, m_strPath);
            if (objAutoTuneGaugeDrawForm.ShowDialog() == DialogResult.OK)
            {
                m_blnAutoTuneForAllEdges = objAutoTuneGaugeDrawForm.ref_blnSetToAll;
                if (objAutoTuneGaugeDrawForm.ref_blnAutoPlace)
                {
                    switch (m_eReadFrom)
                    {
                        case ReadFrom.Pad:
                            if (m_blnAutoTuneForAllEdges)
                            {
                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(0).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(0).Y), 0);
                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(1).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(1).Y), 1);
                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(2).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(2).Y), 2);
                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(3).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(3).Y), 3);
                            }
                            else
                            {
                                switch (m_intPadSelectedIndex)
                                {
                                    case 0:
                                    case 1:
                                        switch (GetLineGaugeSelectedIndex(m_intPadSelectedIndex))
                                        {
                                            case 0:
                                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(0).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(0).Y), 0);
                                                break;
                                            case 1:
                                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(1).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(1).Y), 1);
                                                break;
                                            case 2:
                                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(2).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(2).Y), 2);
                                                break;
                                            case 3:
                                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(3).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(3).Y), 3);
                                                break;
                                        }
                                        break;
                                    case 2:
                                        switch (GetLineGaugeSelectedIndex(m_intPadSelectedIndex))
                                        {
                                            case 1:
                                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(0).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(0).Y), 0);
                                                break;
                                            case 2:
                                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(1).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(1).Y), 1);
                                                break;
                                            case 3:
                                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(2).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(2).Y), 2);
                                                break;
                                            case 0:
                                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(3).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(3).Y), 3);
                                                break;
                                        }
                                        break;
                                    case 3:
                                        switch (GetLineGaugeSelectedIndex(m_intPadSelectedIndex))
                                        {
                                            case 2:
                                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(0).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(0).Y), 0);
                                                break;
                                            case 3:
                                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(1).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(1).Y), 1);
                                                break;
                                            case 0:
                                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(2).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(2).Y), 2);
                                                break;
                                            case 1:
                                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(3).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(3).Y), 3);
                                                break;
                                        }
                                        break;
                                    case 4:
                                        switch (GetLineGaugeSelectedIndex(m_intPadSelectedIndex))
                                        {
                                            case 3:
                                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(0).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(0).Y), 0);
                                                break;
                                            case 0:
                                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(1).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(1).Y), 1);
                                                break;
                                            case 1:
                                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(2).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(2).Y), 2);
                                                break;
                                            case 2:
                                                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(3).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(3).Y), 3);
                                                break;
                                        }
                                        break;
                                }

                            }
                            break;
                            //case ReadFrom.Calibration:
                            //    if (m_blnAutoTuneForAllEdges)
                            //    {
                            //        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[0].SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(0).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(0).Y));
                            //        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[1].SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(1).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(1).Y));
                            //        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[2].SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(2).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(2).Y));
                            //        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[3].SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(3).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(3).Y));
                            //    }
                            //    else
                            //    {
                            //        objAutoTuneGaugeDrawForm.GetLineAngle(0, m_intPadSelectedIndex, ref m_arrAngle[m_intPadSelectedIndex][0]);
                            //        objAutoTuneGaugeDrawForm.GetLineAngle(1, m_intPadSelectedIndex, ref m_arrAngle[m_intPadSelectedIndex][1]);
                            //        objAutoTuneGaugeDrawForm.GetLineAngle(2, m_intPadSelectedIndex, ref m_arrAngle[m_intPadSelectedIndex][2]);
                            //        objAutoTuneGaugeDrawForm.GetLineAngle(3, m_intPadSelectedIndex, ref m_arrAngle[m_intPadSelectedIndex][3]);
                            //    }
                            //    break;
                    }
                }
                switch (m_eReadFrom)
                {
                    case ReadFrom.Pad:
                        //objAutoTuneGaugeDrawForm.GetLineAngle(0, m_intPadSelectedIndex, ref m_arrAngle[m_intPadSelectedIndex][0]);
                        //objAutoTuneGaugeDrawForm.GetLineAngle(1, m_intPadSelectedIndex, ref m_arrAngle[m_intPadSelectedIndex][1]);
                        //objAutoTuneGaugeDrawForm.GetLineAngle(2, m_intPadSelectedIndex, ref m_arrAngle[m_intPadSelectedIndex][2]);
                        //objAutoTuneGaugeDrawForm.GetLineAngle(3, m_intPadSelectedIndex, ref m_arrAngle[m_intPadSelectedIndex][3]);
                        //switch (m_intPadSelectedIndex)
                        //{
                        //    case 0:
                        //    case 1:
                        //m_arrPosition[m_intPadSelectedIndex][0] = objAutoTuneGaugeDrawForm.GetLineCenterPoint(0);
                        //m_arrPosition[m_intPadSelectedIndex][1] = objAutoTuneGaugeDrawForm.GetLineCenterPoint(1);
                        //m_arrPosition[m_intPadSelectedIndex][2] = objAutoTuneGaugeDrawForm.GetLineCenterPoint(2);
                        //m_arrPosition[m_intPadSelectedIndex][3] = objAutoTuneGaugeDrawForm.GetLineCenterPoint(3);

                        m_arrPosition1[m_intPadSelectedIndex][0].Y = objAutoTuneGaugeDrawForm.GetLineInterceptPoint(0, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[0].ref_ROITotalCenterX, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[0].ref_ROITotalCenterY - (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[0].ref_ROIHeight / 10));
                        m_arrPosition1[m_intPadSelectedIndex][1].X = objAutoTuneGaugeDrawForm.GetLineInterceptPoint(1, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[1].ref_ROITotalCenterX - (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[1].ref_ROIWidth / 10), m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[1].ref_ROITotalCenterY);
                        m_arrPosition1[m_intPadSelectedIndex][2].Y = objAutoTuneGaugeDrawForm.GetLineInterceptPoint(2, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[2].ref_ROITotalCenterX, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[2].ref_ROITotalCenterY - (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[2].ref_ROIHeight / 10));
                        m_arrPosition1[m_intPadSelectedIndex][3].X = objAutoTuneGaugeDrawForm.GetLineInterceptPoint(3, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[3].ref_ROITotalCenterX - (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[3].ref_ROIWidth / 10), m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[3].ref_ROITotalCenterY);

                        m_arrPosition2[m_intPadSelectedIndex][0].Y = objAutoTuneGaugeDrawForm.GetLineInterceptPoint(0, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[0].ref_ROITotalCenterX, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[0].ref_ROITotalCenterY + (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[0].ref_ROIHeight / 10));
                        m_arrPosition2[m_intPadSelectedIndex][1].X = objAutoTuneGaugeDrawForm.GetLineInterceptPoint(1, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[1].ref_ROITotalCenterX + (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[1].ref_ROIWidth / 10), m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[1].ref_ROITotalCenterY);
                        m_arrPosition2[m_intPadSelectedIndex][2].Y = objAutoTuneGaugeDrawForm.GetLineInterceptPoint(2, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[2].ref_ROITotalCenterX, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[2].ref_ROITotalCenterY + (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[2].ref_ROIHeight / 10));
                        m_arrPosition2[m_intPadSelectedIndex][3].X = objAutoTuneGaugeDrawForm.GetLineInterceptPoint(3, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[3].ref_ROITotalCenterX + (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[3].ref_ROIWidth / 10), m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[3].ref_ROITotalCenterY);

                        //        break;
                        //    case 2:
                        //        m_arrPosition[m_intPadSelectedIndex][1] = objAutoTuneGaugeDrawForm.GetLineCenterPoint(0);
                        //        m_arrPosition[m_intPadSelectedIndex][2] = objAutoTuneGaugeDrawForm.GetLineCenterPoint(1);
                        //        m_arrPosition[m_intPadSelectedIndex][3] = objAutoTuneGaugeDrawForm.GetLineCenterPoint(2);
                        //        m_arrPosition[m_intPadSelectedIndex][0] = objAutoTuneGaugeDrawForm.GetLineCenterPoint(3);
                        //        break;
                        //    case 3:
                        //        m_arrPosition[m_intPadSelectedIndex][2] = objAutoTuneGaugeDrawForm.GetLineCenterPoint(0);
                        //        m_arrPosition[m_intPadSelectedIndex][3] = objAutoTuneGaugeDrawForm.GetLineCenterPoint(1);
                        //        m_arrPosition[m_intPadSelectedIndex][0] = objAutoTuneGaugeDrawForm.GetLineCenterPoint(2);
                        //        m_arrPosition[m_intPadSelectedIndex][1] = objAutoTuneGaugeDrawForm.GetLineCenterPoint(3);
                        //        break;
                        //    case 4:
                        //        m_arrPosition[m_intPadSelectedIndex][3] = objAutoTuneGaugeDrawForm.GetLineCenterPoint(0);
                        //        m_arrPosition[m_intPadSelectedIndex][0] = objAutoTuneGaugeDrawForm.GetLineCenterPoint(1);
                        //        m_arrPosition[m_intPadSelectedIndex][1] = objAutoTuneGaugeDrawForm.GetLineCenterPoint(2);
                        //        m_arrPosition[m_intPadSelectedIndex][2] = objAutoTuneGaugeDrawForm.GetLineCenterPoint(3);
                        //        break;
                        //}
                        break;
                    //case ReadFrom.Calibration:
                    //    objAutoTuneGaugeDrawForm.GetLineAngle(0, m_intPadSelectedIndex, ref m_arrAngle[m_intPadSelectedIndex][0]);
                    //    objAutoTuneGaugeDrawForm.GetLineAngle(1, m_intPadSelectedIndex, ref m_arrAngle[m_intPadSelectedIndex][1]);
                    //    objAutoTuneGaugeDrawForm.GetLineAngle(2, m_intPadSelectedIndex, ref m_arrAngle[m_intPadSelectedIndex][2]);
                    //    objAutoTuneGaugeDrawForm.GetLineAngle(3, m_intPadSelectedIndex, ref m_arrAngle[m_intPadSelectedIndex][3]);
                    //    break;
                }
               
                StartWaiting("Auto Tuning...");
                
                if (m_blnAutoTuneForAllEdges)
                {
                    if (!radioBtn_Up.Checked)
                        radioBtn_Up.PerformClick();
                }

                Timer_AutoTune.Enabled = true;
            }


            objAutoTuneGaugeDrawForm.Dispose();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void Timer_AutoTune_Tick(object sender, EventArgs e)
        {
            if (m_blnBlockOtherControlUpdate)
                return;

            m_smVisionInfo.g_blnViewAutoTuning = true;
            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(m_intPadSelectedIndex);
            if (chk_EnableThreshold.Checked)
            {
                //int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(m_intPadSelectedIndex);
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetGaugeImageThreshold(m_intThreshold_Temp, intLineGaugeSelectedIndex);
            }
            else
            {
                //int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(m_intPadSelectedIndex);
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetGaugeThickness(m_intThickness_Temp, intLineGaugeSelectedIndex);
                m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetGaugeThreshold(m_intDerivative_Temp, intLineGaugeSelectedIndex);
            }

            //m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetEdgeROIPlacementLimit2(m_smVisionInfo.g_arrPadROIs[m_intPadSelectedIndex][0]);
            m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetGaugePlace_BasedOnEdgeROI();
            
            // 2020-06-15 ZJYEOH : Hide tolerance changing as finaaly will measure gauge using ROI Size as tolerance
            //if (chk_EnableThreshold.Checked)
            //{
            //    //int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(m_intPadSelectedIndex);
            //    switch (m_intPadSelectedIndex)
            //    {
            //        case 0:
            //        case 1:
            //            switch (GetLineGaugeSelectedIndex(m_intPadSelectedIndex))
            //            {
            //                case 0:
            //                    intLineGaugeSelectedIndex = 0;
            //                    break;
            //                case 1:
            //                    intLineGaugeSelectedIndex = 1;
            //                    break;
            //                case 2:
            //                    intLineGaugeSelectedIndex = 2;
            //                    break;
            //                case 3:
            //                    intLineGaugeSelectedIndex = 3;
            //                    break;
            //            }
            //            break;
            //        case 2:
            //            switch (GetLineGaugeSelectedIndex(m_intPadSelectedIndex))
            //            {
            //                case 1:
            //                    intLineGaugeSelectedIndex = 0;
            //                    break;
            //                case 2:
            //                    intLineGaugeSelectedIndex = 1;
            //                    break;
            //                case 3:
            //                    intLineGaugeSelectedIndex = 2;
            //                    break;
            //                case 0:
            //                    intLineGaugeSelectedIndex = 3;
            //                    break;
            //            }
            //            break;
            //        case 3:
            //            switch (GetLineGaugeSelectedIndex(m_intPadSelectedIndex))
            //            {
            //                case 2:
            //                    intLineGaugeSelectedIndex = 0;
            //                    break;
            //                case 3:
            //                    intLineGaugeSelectedIndex = 1;
            //                    break;
            //                case 0:
            //                    intLineGaugeSelectedIndex = 2;
            //                    break;
            //                case 1:
            //                    intLineGaugeSelectedIndex = 3;
            //                    break;
            //            }
            //            break;
            //        case 4:
            //            switch (GetLineGaugeSelectedIndex(m_intPadSelectedIndex))
            //            {
            //                case 3:
            //                    intLineGaugeSelectedIndex = 0;
            //                    break;
            //                case 0:
            //                    intLineGaugeSelectedIndex = 1;
            //                    break;
            //                case 1:
            //                    intLineGaugeSelectedIndex = 2;
            //                    break;
            //                case 2:
            //                    intLineGaugeSelectedIndex = 3;
            //                    break;
            //            }
            //            break;
            //    }

            //    if (m_intThreshold_Temp > 249 && m_fSampleScore == 0 && m_intGaugeTolerance != m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetEdgeROISize(intLineGaugeSelectedIndex))
            //    {
            //        m_fSampleScore = 0;
            //        m_fAngle = 1000;
            //        m_intThreshold_Temp = 5;
            //        m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetGaugeImageThreshold(m_intThreshold_Temp, GetLineGaugeSelectedIndex(m_intPadSelectedIndex));
            //        if (m_intGaugeTolerance < 9)
            //            m_intGaugeTolerance += 3;
            //        else if (m_intGaugeTolerance < m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetEdgeROISize(intLineGaugeSelectedIndex, true, m_intGaugeTolerance))
            //        {
            //            m_intGaugeTolerance = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetEdgeROISize(intLineGaugeSelectedIndex, true, m_intGaugeTolerance);
            //        }
            //        else if (m_intGaugeTolerance < m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetEdgeROISize(intLineGaugeSelectedIndex, false, m_intGaugeTolerance))
            //        {
            //            m_intGaugeTolerance = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetEdgeROISize(intLineGaugeSelectedIndex, false, m_intGaugeTolerance);
            //        }

            //    }

            //    m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetGaugeTolerance_BasedOnEdgeROI(Math.Min(m_intGaugeTolerance, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetEdgeROISize(intLineGaugeSelectedIndex)), GetLineGaugeSelectedIndex(m_intPadSelectedIndex));
            //}

            m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_objWhiteImage, false);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(m_intPadSelectedIndex);

            //if ((Math.Abs(m_fAngle - m_arrAngle[m_intPadSelectedIndex][intLineGaugeSelectedIndex]) + 0.5) > (Math.Abs(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectAngle - m_arrAngle[m_intPadSelectedIndex][intLineGaugeSelectedIndex])) &&
            //   (Math.Abs(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectAngle) < m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMaxAngle(intLineGaugeSelectedIndex)))
            //if (Math2.GetDistanceBtw2Points(m_arrPosition[m_intPadSelectedIndex][intLineGaugeSelectedIndex], new PointF(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectCenterX, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectCenterY)) <
            // m_fDistance)
            if ((GetDistance(m_intLineGaugeSelectedIndex, true) < m_fDistance1) && (GetDistance(m_intLineGaugeSelectedIndex, false) < m_fDistance2))
            {
                switch (intLineGaugeSelectedIndex)
                {
                    case 0:
                        if (//(m_fSampleScore <= m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectScore) &&
                            (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectScore >= m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinScore(intLineGaugeSelectedIndex)))
                        {
                            //if (Math.Abs(m_fAngle - m_arrAngle[m_intPadSelectedIndex][intLineGaugeSelectedIndex]) > Math.Abs(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectAngle - m_arrAngle[m_intPadSelectedIndex][intLineGaugeSelectedIndex]))
                            //    m_fAngle = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectAngle;
                            m_fDistance1 = GetDistance(intLineGaugeSelectedIndex, true);// Math2.GetDistanceBtw2Points(m_arrPosition[m_intPadSelectedIndex][intLineGaugeSelectedIndex], new PointF(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectCenterX, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectCenterY));
                            m_fDistance2 = GetDistance(intLineGaugeSelectedIndex, false);
                            if (chk_EnableThreshold.Checked)
                            {
                                m_intThreshold = m_intThreshold_Temp;
                            }
                            else
                            {
                                m_intThickness = m_intThickness_Temp;
                                m_intDerivative = m_intDerivative_Temp;
                            }
                            m_fSampleScore = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectScore;
                        }
                        break;
                    case 1:
                        if (//(m_fSampleScore <= m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectScore) &&
                            (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectScore >= m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinScore(intLineGaugeSelectedIndex)))
                        {
                            //if (Math.Abs(m_fAngle - m_arrAngle[m_intPadSelectedIndex][intLineGaugeSelectedIndex]) > Math.Abs(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectAngle - m_arrAngle[m_intPadSelectedIndex][intLineGaugeSelectedIndex]))
                            //    m_fAngle = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectAngle;
                            m_fDistance1 = GetDistance(intLineGaugeSelectedIndex, true);// Math2.GetDistanceBtw2Points(m_arrPosition[m_intPadSelectedIndex][intLineGaugeSelectedIndex], new PointF(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectCenterX, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectCenterY));
                            m_fDistance2 = GetDistance(intLineGaugeSelectedIndex, false);
                            if (chk_EnableThreshold.Checked)
                            {
                                m_intThreshold = m_intThreshold_Temp;
                            }
                            else
                            {
                                m_intThickness = m_intThickness_Temp;
                                m_intDerivative = m_intDerivative_Temp;
                            }
                            m_fSampleScore = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectScore;
                        }
                        break;
                    case 2:
                        if (//(m_fSampleScore <= m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectScore) &&
                            (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectScore >= m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinScore(intLineGaugeSelectedIndex)))
                        {
                            //if (Math.Abs(m_fAngle - m_arrAngle[m_intPadSelectedIndex][intLineGaugeSelectedIndex]) > Math.Abs(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectAngle - m_arrAngle[m_intPadSelectedIndex][intLineGaugeSelectedIndex]))
                            //    m_fAngle = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectAngle;
                            m_fDistance1 = GetDistance(intLineGaugeSelectedIndex, true);// Math2.GetDistanceBtw2Points(m_arrPosition[m_intPadSelectedIndex][intLineGaugeSelectedIndex], new PointF(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectCenterX, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectCenterY));
                            m_fDistance2 = GetDistance(intLineGaugeSelectedIndex, false);
                            if (chk_EnableThreshold.Checked)
                            {
                                m_intThreshold = m_intThreshold_Temp;
                            }
                            else
                            {
                                m_intThickness = m_intThickness_Temp;
                                m_intDerivative = m_intDerivative_Temp;
                            }
                            m_fSampleScore = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectScore;
                        }
                        break;
                    case 3:
                        if (//(m_fSampleScore <= m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectScore) &&
                           (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectScore >= m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinScore(intLineGaugeSelectedIndex)))
                        {
                            //if (Math.Abs(m_fAngle - m_arrAngle[m_intPadSelectedIndex][intLineGaugeSelectedIndex]) > Math.Abs(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectAngle - m_arrAngle[m_intPadSelectedIndex][intLineGaugeSelectedIndex]))
                            //    m_fAngle = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectAngle;
                            m_fDistance1 = GetDistance(intLineGaugeSelectedIndex, true);// Math2.GetDistanceBtw2Points(m_arrPosition[m_intPadSelectedIndex][intLineGaugeSelectedIndex], new PointF(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectCenterX, m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectCenterY));
                            m_fDistance2 = GetDistance(intLineGaugeSelectedIndex, false);
                            if (chk_EnableThreshold.Checked)
                            {
                                m_intThreshold = m_intThreshold_Temp;
                            }
                            else
                            {
                                m_intThickness = m_intThickness_Temp;
                                m_intDerivative = m_intDerivative_Temp;
                            }
                            m_fSampleScore = m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineGaugeSelectedIndex].ref_ObjectScore;
                        }
                        break;
                }
            }

            if (chk_EnableThreshold.Checked)
            {
                m_intThreshold_Temp += 2;

                if (m_intThreshold_Temp > 254)
                {
                    m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetGaugePlace_BasedOnEdgeROI(); // 2020-06-12 ZJYEOH : resize back the tolerance as it is being modified for threshold auto tune
                    m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetGaugeImageThreshold(m_intThreshold, intLineGaugeSelectedIndex);
                    m_fSampleScore = 0;
                    m_fDistance1 = float.MaxValue;
                    m_fDistance2 = float.MaxValue;
                    m_fAngle = float.MaxValue;
                    m_intThickness = 10;
                    m_intDerivative = 5;
                    m_intThreshold = 5;
                    m_intGaugeTolerance = 3;
                    m_intThreshold_Temp = 5;
                    m_intThickness_Temp = 10;
                    m_intDerivative_Temp = 5;
                    m_smVisionInfo.g_blnViewAutoTuning = false;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                    if (m_blnAutoTuneForAllEdges)
                    {
                        if (radioBtn_Up.Checked)
                        {
                            radioBtn_Right.PerformClick();
                        }
                        else if (radioBtn_Right.Checked)
                        {
                            radioBtn_Down.PerformClick();
                        }
                        else if (radioBtn_Down.Checked)
                        {
                            radioBtn_Left.PerformClick();
                        }
                        else if (radioBtn_Left.Checked)
                        {
                            radioBtn_Left.PerformClick();
                            StopWaiting();
                            Timer_AutoTune.Enabled = false;
                        }
                    }
                    else
                    {
                        if (radioBtn_Up.Checked)
                            radioBtn_Up.PerformClick();
                        else if (radioBtn_Right.Checked)
                            radioBtn_Right.PerformClick();
                        else if (radioBtn_Down.Checked)
                            radioBtn_Down.PerformClick();
                        else if (radioBtn_Left.Checked)
                        {
                            radioBtn_Left.PerformClick();
                        }
                        StopWaiting();
                        Timer_AutoTune.Enabled = false;
                    }

                    //if (m_blnAutoTuneForAllEdges)
                    //{
                    //    if (!radioBtn_Right.Checked)
                    //        radioBtn_Right.PerformClick();
                    //    else if (!radioBtn_Down.Checked)
                    //        radioBtn_Down.PerformClick();
                    //    else if (!radioBtn_Left.Checked)
                    //        radioBtn_Left.PerformClick();
                    //}
                }

            }
            else
            {
                m_intDerivative_Temp += 5;

                if (m_intDerivative_Temp > 65)
                {
                    m_intThickness_Temp += 5;
                    m_intDerivative_Temp = 5;
                    if (m_intThickness_Temp > 75)
                    {
                        m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetGaugeThickness(m_intThickness, intLineGaugeSelectedIndex);
                        m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.SetGaugeThreshold(m_intDerivative, intLineGaugeSelectedIndex);
                        m_fSampleScore = 0;
                        m_fDistance1 = float.MaxValue;
                        m_fDistance2 = float.MaxValue;
                        m_fAngle = float.MaxValue;
                        m_intThickness = 10;
                        m_intDerivative = 5;
                        m_intThreshold = 5;
                        m_intGaugeTolerance = 3;
                        m_intThreshold_Temp = 5;
                        m_intThickness_Temp = 10;
                        m_intDerivative_Temp = 5;
                        m_smVisionInfo.g_blnViewAutoTuning = false;
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                        if (m_blnAutoTuneForAllEdges)
                        {
                            if (radioBtn_Up.Checked)
                            {
                                radioBtn_Right.PerformClick();
                            }
                            else if (radioBtn_Right.Checked)
                            {
                                radioBtn_Down.PerformClick();
                            }
                            else if (radioBtn_Down.Checked)
                            {
                                radioBtn_Left.PerformClick();
                            }
                            else  if (radioBtn_Left.Checked)
                            {
                                radioBtn_Left.PerformClick();
                                StopWaiting();
                                Timer_AutoTune.Enabled = false;
                            }
                        }
                        else
                        {
                            if (radioBtn_Up.Checked)
                                radioBtn_Up.PerformClick();
                            else if (radioBtn_Right.Checked)
                                radioBtn_Right.PerformClick();
                            else if (radioBtn_Down.Checked)
                                radioBtn_Down.PerformClick();
                            else if (radioBtn_Left.Checked)
                            {
                                radioBtn_Left.PerformClick();
                            }
                            StopWaiting();
                            Timer_AutoTune.Enabled = false;
                        }

                        //if (m_blnAutoTuneForAllEdges)
                        //{
                        //    if (!radioBtn_Right.Checked)
                        //        radioBtn_Right.PerformClick();
                        //    else if (!radioBtn_Down.Checked)
                        //        radioBtn_Down.PerformClick();
                        //    else if (!radioBtn_Left.Checked)
                        //        radioBtn_Left.PerformClick();
                        //}
                    }
                }
            }

            //Thread.Sleep(100);
        }
        private void StartWaiting(string StrMessage)
        {
            m_thWaitingFormThread = new SRMWaitingFormThread();
            m_thWaitingFormThread.SetStartSplash(StrMessage);
            m_pPreviousLocation = new Point(this.Location.X, this.Location.Y);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            this.Location = new Point(resolution.Width, resolution.Height);
        }

        private void StopWaiting()
        {
            m_thWaitingFormThread.SetStopSplash();
            this.Location = m_pPreviousLocation;
        }

        private float GetDistance(int intLineIndex, bool blnDistance1)
        {
            switch (intLineIndex)
            {
                case 0:
                case 2:
                    if (blnDistance1)
                        return Math.Abs(m_arrPosition1[m_intPadSelectedIndex][intLineIndex].Y - m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineIndex].ref_ObjectLine.GetPointY(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[intLineIndex].ref_ROITotalCenterX - (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[intLineIndex].ref_ROIWidth / 10)));
                    else
                        return Math.Abs(m_arrPosition2[m_intPadSelectedIndex][intLineIndex].Y - m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineIndex].ref_ObjectLine.GetPointY(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[intLineIndex].ref_ROITotalCenterX - (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[intLineIndex].ref_ROIWidth / 10)));
                    

                case 1:
                case 3:
                    if (blnDistance1)
                        return Math.Abs(m_arrPosition1[m_intPadSelectedIndex][intLineIndex].X - m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineIndex].ref_ObjectLine.GetPointX(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[intLineIndex].ref_ROITotalCenterY - (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[intLineIndex].ref_ROIHeight / 10)));
                   else
                        return Math.Abs(m_arrPosition1[m_intPadSelectedIndex][intLineIndex].X - m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrLineGauge[intLineIndex].ref_ObjectLine.GetPointX(m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[intLineIndex].ref_ROITotalCenterY - (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[intLineIndex].ref_ROIHeight / 10)));
            }
            return 0;
        }

        private void txt_Offset_TextChanged(object sender, EventArgs e)
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
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeOffset(Convert.ToInt32(txt_Offset.Text), j);
                                }
                            }
                        }
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeOffset(Convert.ToInt32(txt_Offset.Text), intLineGaugeSelectedIndex);
                        }

                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }

                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeOffset(Convert.ToInt32(txt_Offset.Text));
                    break;
            }
            trackBar_Offset.Value = Convert.ToInt32(txt_Offset.Text);
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void trackBar_Offset_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Offset.Text = trackBar_Offset.Value.ToString();
        }

        private void cbo_GaugeMeasureMode_SelectedIndexChanged(object sender, EventArgs e)
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

                        for (int j = 0; j < 4; j++)
                        {
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeEnableSubGauge(chk_EnableSubGauge.Checked, j);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeMeasureMode(cbo_GaugeMeasureMode.SelectedIndex, j);
                        }

                        if (intPadSelectedIndex == 0 || intPadSelectedIndex == 1 || intPadSelectedIndex == 3)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMeasureMode(j) == 2 && (j == 1 || j == 3)) && intPadSelectedIndex != 0)
                                {
                                    if (!m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeWantImageThreshold(j))
                                    {
                                        chk_EnableThreshold.Checked = true;
                                        chk_EnableThreshold.Enabled = false;
                                        txt_Threshold.Enabled = chk_EnableThreshold.Checked;
                                        trackBar_Threshold.Enabled = chk_EnableThreshold.Checked;

                                        float fImageGain = Convert.ToSingle(txt_Gain.Text);
                                        int intImageThreshld = Convert.ToInt32(txt_Threshold.Text);

                                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeWantImageThreshold(chk_EnableThreshold.Checked, j);

                                        switch (j)
                                        {
                                            case 0:
                                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROITotalCenterX,
                                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                                                    Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(j) + 90);
                                                break;
                                            case 1:
                                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROITotalX + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                                                   m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROITotalCenterY,
                                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(j) + 90);
                                                break;
                                            case 2:
                                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROITotalCenterX,
                                                  m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROITotalY + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(j) + 90);
                                                break;
                                            case 3:
                                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                                                 m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROITotalCenterY,
                                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(j) + 90);
                                                break;
                                        }
                                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);

                                    }
                                }
                                else if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMeasureMode(j) != 2 || intPadSelectedIndex == 0)
                                {
                                    chk_EnableThreshold.Enabled = true;
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMeasureMode(j) == 2 && (j == 0 || j == 2))
                                {
                                    if (!m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeWantImageThreshold(j))
                                    {
                                        chk_EnableThreshold.Checked = true;
                                        chk_EnableThreshold.Enabled = false;
                                        txt_Threshold.Enabled = chk_EnableThreshold.Checked;
                                        trackBar_Threshold.Enabled = chk_EnableThreshold.Checked;

                                        float fImageGain = Convert.ToSingle(txt_Gain.Text);
                                        int intImageThreshld = Convert.ToInt32(txt_Threshold.Text);

                                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeWantImageThreshold(chk_EnableThreshold.Checked, j);

                                        switch (j)
                                        {
                                            case 0:
                                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROITotalCenterX,
                                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROITotalY + Convert.ToSingle(txt_Tolerance.Text),
                                                    Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(j) + 90);
                                                break;
                                            case 1:
                                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROITotalX + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIWidth - Convert.ToSingle(txt_Tolerance.Text),
                                                   m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROITotalCenterY,
                                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(j) + 90);
                                                break;
                                            case 2:
                                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROITotalCenterX,
                                                  m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROITotalY + m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIHeight - Convert.ToSingle(txt_Tolerance.Text),
                                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(j) + 90);
                                                break;
                                            case 3:
                                                m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.SetGaugePlacement(m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROITotalX + Convert.ToSingle(txt_Tolerance.Text),
                                                 m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROITotalCenterY,
                                                   Convert.ToSingle(txt_Tolerance.Text), m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetAngle(j) + 90);
                                                break;
                                        }
                                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objPointGauge.Measure(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);

                                    }
                                }
                                else if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMeasureMode(j) != 2)
                                {
                                    chk_EnableThreshold.Enabled = true;
                                }
                            }
                        }

                        // 2020 11 28 - need to reallocate gauge setting bcos it may be changed when user select other measure mode.
                        m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SetGaugePlace_BasedOnEdgeROI();
                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }
                    break;
                case ReadFrom.Calibration:
                    //for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                    //    m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeEnableSubGauge(Convert.ToInt32(txt_MaxAngle.Text));
                    break;
            }

            m_blnBlockOtherControlUpdate = true;
            UpdateGaugeToGUI();
            m_blnBlockOtherControlUpdate = false;

            UpdatePackageImage();

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_ResetPosition_Click(object sender, EventArgs e)
        {
            int intLimitStartX = m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageWidth;
            int intLimitStartY = m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageHeight;
            int intLimitEndX = 0;
            int intLimitEndY = 0;
            // Set RectGauge4L Placement
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;
                
                //m_smVisionInfo.g_arrPad[i].SetRectGauge4LPlacement(pCenter, 0, m_smVisionInfo.g_arrPadROIs[i][1].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[i][1].ref_ROIHeight);
                m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ResetEdgeROIPlacement(m_smVisionInfo.g_arrPadROIs[i][0]);
                m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SetGaugePlace_BasedOnEdgeROI();
                
                for (int j = 0; j < m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI.Count; j++)
                {
                    if (intLimitStartX > m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIPositionX)
                        intLimitStartX = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIPositionX;
                    if (intLimitStartY > m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIPositionY)
                        intLimitStartY = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIPositionY;

                    if (intLimitEndX < m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIPositionX + m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIWidth)
                        intLimitEndX = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIPositionX + m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIWidth;
                    if (intLimitEndY < m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIPositionY + m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIHeight)
                        intLimitEndY = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIPositionY + m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIHeight;
                }

            }

            if (m_smVisionInfo.g_intLearnStepNo == 2 && (m_smVisionInfo.g_strSelectedPage == "Pad" || m_smVisionInfo.g_strSelectedPage == "Pad5S" || m_smVisionInfo.g_strSelectedPage == "BottomOrientPad" || m_smVisionInfo.g_strSelectedPage == "BottomOPadPkg"))
                m_smVisionInfo.g_objTopParentROI.LoadROISetting(intLimitStartX, intLimitStartY, intLimitEndX - intLimitStartX, intLimitEndY - intLimitStartY);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GroupTol_TextChanged(object sender, EventArgs e)
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
                            for (int j = 0; j < 4; j++)
                            {
                                if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeGroupTolerance(Convert.ToSingle(txt_GroupTol.Text), j);
                                }
                            }
                        }
                        else
                        {
                            int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.SetGaugeGroupTolerance(Convert.ToSingle(txt_GroupTol.Text), intLineGaugeSelectedIndex);
                        }

                        if (!chk_ApplyToAllSideROI.Checked)
                            break;
                    }
                    break;
                case ReadFrom.Calibration:
                    for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                        m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeGroupTolerance(Convert.ToSingle(txt_GroupTol.Text));
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_SaveAndSetSizeAsReference_Click(object sender, EventArgs e)
        {
            SaveSetting(true);
        }

        private void CheckSettingSame()
        {
            bool blnGainSame = true,
             blnThresholdSame = true,
             blnOpenCloseSame = true,
             blnPolaritySame = true,
             blnDirectionSame = true,
             blnSearchSame = true,
             blnImageModeSame = true,
             blnThicknessSame = true,
             blnFilterSame = true,
             blnMinAmplitudeSame = true,
             blnMinAreaSame = true,
             blnFilterPassSame = true,
             blnFilterThresholdSame = true,
             blnDerivativeSame = true,
             blnSamplingStepSame = true,
             blnMinScoreSame = true,
             blnMaxAngleSame = true,
             blnGaugeMeasureModeSame = true,
             blnGaugeOffsetSame = true;

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
                int intCurrentLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (intCurrentLineGaugeSelectedIndex == j)
                            continue;

                        if ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0)
                        {
                            if (blnGainSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageGain(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageGain(j))
                                {
                                    blnGainSame = false;
                                }
                            }

                            if (blnThresholdSame)
                            {
                                if ((m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeWantImageThreshold(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeWantImageThreshold(j))
                                    || m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageThreshold(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageThreshold(j))
                                {
                                    blnThresholdSame = false;
                                }
                            }

                            if (blnOpenCloseSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageOpenCloseIteration(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageOpenCloseIteration(j))
                                {
                                    blnOpenCloseSame = false;
                                }
                            }

                            if (blnPolaritySame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(j))
                                {
                                    blnPolaritySame = false;
                                }
                            }

                            if (blnDirectionSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(j))
                                {
                                    blnDirectionSame = false;
                                }
                            }

                            if (blnSearchSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransChoice(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransChoice(j))
                                {
                                    blnSearchSame = false;
                                }
                            }

                            if (blnImageModeSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageMode(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageMode(j))
                                {
                                    blnImageModeSame = false;
                                }
                            }

                            if (blnThicknessSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeThickness(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeThickness(j))
                                {
                                    blnThicknessSame = false;
                                }
                            }

                            if (blnFilterSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilter(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilter(j))
                                {
                                    blnFilterSame = false;
                                }
                            }

                            if (blnMinAmplitudeSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinAmplitude(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinAmplitude(j))
                                {
                                    blnMinAmplitudeSame = false;
                                }
                            }

                            if (blnMinAreaSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinArea(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinArea(j))
                                {
                                    blnMinAreaSame = false;
                                }
                            }

                            if (blnFilterPassSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilterPasses(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilterPasses(j))
                                {
                                    blnFilterPassSame = false;
                                }
                            }

                            if (blnFilterThresholdSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilterThreshold(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilterThreshold(j))
                                {
                                    blnFilterThresholdSame = false;
                                }
                            }

                            if (blnDerivativeSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeThreshold(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeThreshold(j))
                                {
                                    blnDerivativeSame = false;
                                }
                            }

                            if (blnSamplingStepSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeSamplingStep(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeSamplingStep(j))
                                {
                                    blnSamplingStepSame = false;
                                }
                            }

                            if (blnMinScoreSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinScore(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinScore(j))
                                {
                                    blnMinScoreSame = false;
                                }
                            }

                            if (blnMaxAngleSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMaxAngle(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMaxAngle(j))
                                {
                                    blnMaxAngleSame = false;
                                }
                            }

                            if (blnGaugeMeasureModeSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMeasureMode(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMeasureMode(j))
                                {
                                    blnGaugeMeasureModeSame = false;
                                }
                            }

                            if(blnGaugeOffsetSame)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeOffset(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeOffset(j))
                                {
                                    blnGaugeOffsetSame = false;
                                }
                            }
                            
                        }
                    }
                }

                if (!chk_ApplyToAllSideROI.Checked)
                    break;
            }

            if (chk_ApplyToAllSideROI.Checked)
            {

                for (int j = 0; j < 4; j++)
                {

                    if (!m_blnSetToAll)
                    {
                        if (m_intLineGaugeSelectedIndex != GetLineGaugeIndex(m_intPadSelectedIndex, j))
                        {
                            continue;
                        }
                    }

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

                        if (m_intPadSelectedIndex == intPadSelectedIndex)
                            continue;

                        int intCurrentLineGaugeSelectedIndex = GetLineGaugeIndex(m_intPadSelectedIndex, j);
                        int intNextLineGaugeSelectedIndex = GetLineGaugeIndex(intPadSelectedIndex, j);
                        if (((m_arrEdgeEnableMask[m_intPadSelectedIndex] & (0x01 << j)) > 0) && ((m_arrEdgeEnableMask[intPadSelectedIndex] & (0x01 << j)) > 0))
                        {
                            if (blnGainSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageGain(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageGain(intNextLineGaugeSelectedIndex))
                                {
                                    blnGainSame = false;
                                }
                            }

                            if (blnThresholdSame)
                            {
                                if ((m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeWantImageThreshold(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeWantImageThreshold(intNextLineGaugeSelectedIndex))
                                    || m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageThreshold(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageThreshold(intNextLineGaugeSelectedIndex))
                                {
                                    blnThresholdSame = false;
                                }
                            }

                            if (blnOpenCloseSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageOpenCloseIteration(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageOpenCloseIteration(intNextLineGaugeSelectedIndex))
                                {
                                    blnOpenCloseSame = false;
                                }
                            }

                            if (blnPolaritySame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransType(intNextLineGaugeSelectedIndex))
                                {
                                    blnPolaritySame = false;
                                }
                            }

                            if (blnDirectionSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.IsDirectionInToOut(intNextLineGaugeSelectedIndex))
                                {
                                    blnDirectionSame = false;
                                }
                            }

                            if (blnSearchSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransChoice(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeTransChoice(intNextLineGaugeSelectedIndex))
                                {
                                    blnSearchSame = false;
                                }
                            }

                            if (blnImageModeSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageMode(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageMode(intNextLineGaugeSelectedIndex))
                                {
                                    blnImageModeSame = false;
                                }
                            }

                            if (blnThicknessSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeThickness(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeThickness(intNextLineGaugeSelectedIndex))
                                {
                                    blnThicknessSame = false;
                                }
                            }

                            if (blnFilterSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilter(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilter(intNextLineGaugeSelectedIndex))
                                {
                                    blnFilterSame = false;
                                }
                            }

                            if (blnMinAmplitudeSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinAmplitude(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinAmplitude(intNextLineGaugeSelectedIndex))
                                {
                                    blnMinAmplitudeSame = false;
                                }
                            }

                            if (blnMinAreaSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinArea(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinArea(intNextLineGaugeSelectedIndex))
                                {
                                    blnMinAreaSame = false;
                                }
                            }

                            if (blnFilterPassSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilterPasses(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilterPasses(intNextLineGaugeSelectedIndex))
                                {
                                    blnFilterPassSame = false;
                                }
                            }

                            if (blnFilterThresholdSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilterThreshold(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeFilterThreshold(intNextLineGaugeSelectedIndex))
                                {
                                    blnFilterThresholdSame = false;
                                }
                            }

                            if (blnDerivativeSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeThreshold(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeThreshold(intNextLineGaugeSelectedIndex))
                                {
                                    blnDerivativeSame = false;
                                }
                            }

                            if (blnSamplingStepSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeSamplingStep(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeSamplingStep(intNextLineGaugeSelectedIndex))
                                {
                                    blnSamplingStepSame = false;
                                }
                            }

                            if (blnMinScoreSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinScore(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMinScore(intNextLineGaugeSelectedIndex))
                                {
                                    blnMinScoreSame = false;
                                }
                            }

                            if (blnMaxAngleSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMaxAngle(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMaxAngle(intNextLineGaugeSelectedIndex))
                                {
                                    blnMaxAngleSame = false;
                                }
                            }

                            if (blnGaugeMeasureModeSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMeasureMode(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeMeasureMode(intNextLineGaugeSelectedIndex))
                                {
                                    blnGaugeMeasureModeSame = false;
                                }
                            }

                            if (blnGaugeOffsetSame)
                            {
                                if (m_smVisionInfo.g_arrPad[m_intPadSelectedIndex].ref_objRectGauge4L.GetGaugeOffset(intCurrentLineGaugeSelectedIndex) != m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeOffset(intNextLineGaugeSelectedIndex))
                                {
                                    blnGaugeOffsetSame = false;
                                }
                            }
                            
                        }
                    }
                }
            }

            if (blnGainSame)
                lbl_Gain.ForeColor = Color.Black;
            else
                lbl_Gain.ForeColor = Color.Red;

            if (blnThresholdSame)
                lbl_Threshold.ForeColor = Color.Black;
            else
                lbl_Threshold.ForeColor = Color.Red;

            if (blnOpenCloseSame)
                lbl_OpenClose.ForeColor = Color.Black;
            else
                lbl_OpenClose.ForeColor = Color.Red;

            if (blnPolaritySame)
                lbl_Polarity.ForeColor = Color.Black;
            else
                lbl_Polarity.ForeColor = Color.Red;

            if (blnDirectionSame)
                lbl_Direction.ForeColor = Color.Black;
            else
                lbl_Direction.ForeColor = Color.Red;

            if (blnSearchSame)
                lbl_Search.ForeColor = Color.Black;
            else
                lbl_Search.ForeColor = Color.Red;

            if (blnImageModeSame)
                lbl_ImageMode.ForeColor = Color.Black;
            else
                lbl_ImageMode.ForeColor = Color.Red;

            if (blnThicknessSame)
                lbl_Thickness.ForeColor = Color.Black;
            else
                lbl_Thickness.ForeColor = Color.Red;

            if (blnFilterSame)
                lbl_Filter.ForeColor = Color.Black;
            else
                lbl_Filter.ForeColor = Color.Red;

            if (blnMinAmplitudeSame)
                lbl_MinAmplitude.ForeColor = Color.Black;
            else
                lbl_MinAmplitude.ForeColor = Color.Red;

            if (blnMinAreaSame)
                lbl_MinArea.ForeColor = Color.Black;
            else
                lbl_MinArea.ForeColor = Color.Red;

            if (blnFilterPassSame)
                lbl_FilterPass.ForeColor = Color.Black;
            else
                lbl_FilterPass.ForeColor = Color.Red;

            if (blnFilterThresholdSame)
                lbl_FilterThreshold.ForeColor = Color.Black;
            else
                lbl_FilterThreshold.ForeColor = Color.Red;

            if (blnDerivativeSame)
                lbl_Derivative.ForeColor = Color.Black;
            else
                lbl_Derivative.ForeColor = Color.Red;

            if (blnSamplingStepSame)
                lbl_SamplingStep.ForeColor = Color.Black;
            else
                lbl_SamplingStep.ForeColor = Color.Red;

            if (blnMinScoreSame)
                lbl_MinScore.ForeColor = Color.Black;
            else
                lbl_MinScore.ForeColor = Color.Red;

            if (blnMaxAngleSame)
                lbl_MaxAngle.ForeColor = Color.Black;
            else
                lbl_MaxAngle.ForeColor = Color.Red;

            if (blnGaugeMeasureModeSame)
                lbl_GaugeMeasureMode.ForeColor = Color.Black;
            else
                lbl_GaugeMeasureMode.ForeColor = Color.Red;

            if (blnGaugeMeasureModeSame)
                lbl_GaugeMeasureMode.ForeColor = Color.Black;
            else
                lbl_GaugeMeasureMode.ForeColor = Color.Red;

            if (blnGaugeOffsetSame)
                lbl_GaugeOffset.ForeColor = Color.Black;
            else
                lbl_GaugeOffset.ForeColor = Color.Red;

        }
    }
}