using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VisionProcessing;
using SharedMemory;
using Common;

namespace VisionProcessForm
{
    public partial class SetGroupNoForm : Form
    {
        #region Member variables
        private bool m_blnInitDone = false;
        List<PointF> m_Start = new List<PointF>();
        List<PointF> m_End = new List<PointF>();
        PointF m_Hit = new PointF();

        List<PointF> m_StartOri = new List<PointF>();
        List<PointF> m_EndOri = new List<PointF>();
        
        List<float> m_StartPercentX = new List<float>();
        List<float> m_EndPercentX = new List<float>();
        List<float> m_StartPercentY = new List<float>();
        List<float> m_EndPercentY = new List<float>();

        List<float> m_InwardStartPercent = new List<float>();
        List<float> m_InwardEndPercent = new List<float>();

        List<int> m_MeasureMethod = new List<int>();
        
        private float m_FeretAngle, m_fFeretWidth, m_fFeretHeight, m_FeretStartX, m_FeretStartY;
        private bool m_bUpdatePictureBox = false;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private Graphics m_g, m_g1, m_g2;
        private ROI m_objDrawROI = new ROI();
        private int m_intselectedpad;
        private int m_intPadID;
        private int m_x, m_y, m_w, m_h, m_MaxX, m_MaxY, m_MinX, m_MinY, m_OriX, m_OriY , m_OriW, m_OriH;
        private int m_intLineCount = 2;
        private bool greenfirst, m_blnFeretActivated, m_blnFeretActivatedOri;
        ImageDrawing m_objPadImage = null;
        ImageDrawing m_objRotatedPadImage = null;
        ROI objROI = new ROI();
        private int m_intSelectedLanguage=1;
        private int m_intPadNo = 0;
        public int m_intCount = 0, m_intLengthMode;
        #endregion

        #region Properties
        public int ref_intSetValue { get { return Convert.ToInt32(txt_SetValue.Text); } }

        public bool ref_blnTurnWidthLength { get { return chk_TurnWidthLength.Checked; } }

        public bool ref_blnEnablePad { get { return chk_EnableDisablePad.Checked; } }

        public bool ref_blnFeretActivated { get { return chk_Feret.Checked; } }
        #endregion

      
        public SetGroupNoForm(bool blnFeretActivated, bool blnIsPadEnable, int intSelectedPadNo, VisionInfo smVisionInfo,
              int x, int y, ProductionInfo smProductionInfo, int width, int height, int PadNo, int LineCount,
              float fFeretWidth, float fFeretHeight, int FeretStartX, int FeretStartY, float FeretAngle,
              int MaxX, int MaxY, int MinX, int MinY, int intLengthMode, int intSelectedLanguage, float fAngle, int intPadID)
        {
            InitializeComponent();
            m_intSelectedLanguage = intSelectedLanguage;
            m_intLengthMode = intLengthMode;
            m_blnFeretActivatedOri = blnFeretActivated;
            m_blnFeretActivated = blnFeretActivated;
            m_MaxX = MaxX;
            m_MaxY = MaxY;
            m_MinX = MinX;
            m_MinY = MinY;
            m_fFeretWidth = fFeretWidth;
            m_fFeretHeight = fFeretHeight;
            m_FeretStartX = FeretStartX;
            m_FeretStartY = FeretStartY;
            m_FeretAngle = FeretAngle;
            m_intLineCount = LineCount;
            if (m_intLineCount < 2)
                m_intLineCount = 2;
            //m_strPath = strPath;
            m_intPadNo = PadNo;
            m_x = x;
            m_y = y;
            m_w = width;
            m_h = height;

            m_OriX = x;
            m_OriY = y;
            m_OriW = width;
            m_OriH = height;
            //if (blnFeretActivated)
            //{
            //    m_x = (int)FeretStartX;
            //    m_y = (int)FeretStartY;
            //    m_w = (int)fFeretWidth;
            //    m_h = (int)fFeretHeight;
            //}
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            // ------------------------------
            m_objRotatedPadImage = new ImageDrawing(true);
            m_objPadImage = new ImageDrawing(true);

            chk_DrawCrosshair.Checked = true;

            objROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
            if (m_blnFeretActivated)
            {
                int MaxW = 0, MaxH = 0, MinStartX = 0, MinStartY = 0, FinalMax = 0;
                MaxW = Math.Max(m_MaxX - m_MinX, (int)fFeretWidth);
                MaxH = Math.Max(m_MaxY - m_MinY, (int)fFeretHeight);
                FinalMax = Math.Max(MaxW, MaxH);
                MinStartX = (int)(((MinX + MaxX) / 2) - (FinalMax / 2));
                MinStartY = (int)(((MinY + MaxY) / 2) - (FinalMax / 2));

                objROI.LoadROISetting(MinStartX, MinStartY, FinalMax, FinalMax);
                //MinStartX = (int)(((MinX+MaxX)/2)-(MaxW/2));
                //MinStartY = (int)(((MinY+MaxY)/2)-(MaxH/2));

                //objROI.LoadROISetting(MinStartX, MinStartY, MaxW, MaxH);
                objROI.CopyToImage(ref m_objRotatedPadImage);
                objROI.CopyToImage(ref m_objPadImage);
            }
            else
            {
                objROI.LoadROISetting(m_x, m_y, m_w, m_h);
                objROI.CopyToImage(ref m_objRotatedPadImage);
                objROI.CopyToImage(ref m_objPadImage);
            }
            //objROI.SaveImage("D:\\objROI.bmp");
            //m_objPadImage.SaveImage("D:\\m_objPadImage.bmp");
            // if feret activated and ABS(angle > 0
            if (m_blnFeretActivated)
            {
                float Angle = m_FeretAngle;

                //else if (Angle <= -85)
                //if (Angle >= 85)
                //    Angle = m_FeretAngle - 90;
                //else if (Angle <= -85)
                //    Angle = m_FeretAngle + 90;
                //if (Angle>0 && Angle <= 45)
                //    Angle = -Angle;
                // m_objRotatedPadImage.SaveImage("D:\\RotatedPadBefore.bmp");
                ROI.RotateROI_DifferentImageSize(objROI, Angle, ref m_objRotatedPadImage, (int)fFeretWidth, (int)fFeretHeight);
                //m_objRotatedPadImage.SaveImage("D:\\RotatedPad.bmp");
                objROI.AttachImage(m_objRotatedPadImage);
                //if (m_FeretAngle >= 85 || m_FeretAngle <= -85)
                //{
                //    objROI.LoadROISetting((int)(m_objRotatedPadImage.ref_intImageHeight / 2 - (int)fFeretHeight / 2) - 2,
                //        (int)(m_objRotatedPadImage.ref_intImageWidth / 2 - (int)fFeretWidth / 2) - 2,
                //        (int)fFeretHeight + 4, (int)fFeretWidth + 4);
                //}
                //else
                {
                    objROI.LoadROISetting((int)(m_objRotatedPadImage.ref_intImageWidth / 2 - (int)fFeretWidth / 2) - 2,
                           (int)(m_objRotatedPadImage.ref_intImageHeight / 2 - (int)fFeretHeight / 2) - 2,
                           (int)fFeretWidth + 4, (int)fFeretHeight + 4);
                }
                objROI.CopyToImage(ref m_objPadImage);
                // objROI.SaveImage("D:\\objROI.bmp");
                //m_objPadImage.SaveImage("D:\\m_objPadImage.bmp");
                //Angle = m_FeretAngle;
                //ROI.RotateROI_DifferentImageSize(objROI, Angle, ref m_objPadImage);

            }

            m_g = Graphics.FromHwnd(pic_Image.Handle);

             //m_objPadImage.RedrawImage(m_g);

            // ==============================


            timer_Live.Enabled = true;
            timer_Live.Start();
            m_intselectedpad = intSelectedPadNo;
            m_intPadID = intPadID;
            chk_Feret.Checked = m_blnFeretActivated;
            chk_EnableDisablePad.Checked = blnIsPadEnable;
            this.Text += "" + Convert.ToString(m_intPadID);//intSelectedPadNo
            txt_SetValue.Text = m_intPadID.ToString();//intSelectedPadNo

            txt_PadAngle.Text = Convert.ToInt32(fAngle).ToString();
  
            // m_stcBlobPad = (BlobsFeatures)m_arrTemplateBlobPads[intSelectedPadNo];
            // m_objTemplate = new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
            //int Max = Math.Max((int)Math.Ceiling(width * m_smVisionInfo.g_fScaleX), (int)Math.Ceiling(height * m_smVisionInfo.g_fScaleY));

            if (m_blnFeretActivated)
            {
                m_x = (int)FeretStartX;
                m_y = (int)FeretStartY;
                //if (m_FeretAngle >= 85 || m_FeretAngle <= -85)
                //{
                //    m_h = m_objPadImage.ref_intImageHeight; //(int)fFeretWidth;
                //    m_w = m_objPadImage.ref_intImageWidth; //(int)fFeretHeight;
                //}
                //else
                {
                    m_w = m_objPadImage.ref_intImageWidth; // (int)fFeretWidth;
                    m_h = m_objPadImage.ref_intImageHeight; //(int)fFeretHeight;
                }
            }


            int Max = Math.Max((int)(m_w), (int)(m_h));

            //  m_g = Graphics.FromHwnd(pic_Image.Handle);
            //if (Max > 250)
            //    Max = 250;
            // m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, 128, m_smVisionInfo.g_arrPadROIs[0], 0);
            // m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].NewDrawZoomImage(g, 0.01f, 0, 0, x, y);
            // Image i = new Bitmap(250, 250, m_g);
            //m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(i) ;
            // pic_Image.Image = i;
            // Get a Graphics object for the image
            //  Graphics g2 = Graphics.FromImage(i);

            // Now get handles to device contexts, and perform the bit blitting operation.
            //dc1 = m_g.GetHdc();
            //dc2 = g2.GetHdc();
            //float desWidth = Math.Min((int)Math.Ceiling((250 / Max) * (width * m_smVisionInfo.g_fScaleX)), 250);
            //float desHeight = Math.Min((int)Math.Ceiling((250 / Max) * (height * m_smVisionInfo.g_fScaleY)), 250);
            //float sourceX = x * m_smVisionInfo.g_fScaleX;
            //float sourceY = y * m_smVisionInfo.g_fScaleY;
            //int intSourceX = (int)Math.Round(x * m_smVisionInfo.g_fScaleX);
            //int intSourceY = (int)Math.Round(y * m_smVisionInfo.g_fScaleY);
            //float sourceW = (width + 1) * m_smVisionInfo.g_fScaleX;
            //float sourceH = (height + 1) * m_smVisionInfo.g_fScaleY;
            //int intsourceW = (int)Math.Ceiling((width) * m_smVisionInfo.g_fScaleX);
            //int intsourceH = (int)Math.Ceiling((height) * m_smVisionInfo.g_fScaleY);
            //StretchBlt(dc2, 0, 0, Math.Min((int)Math.Ceiling((250 / Max) * (width * m_smVisionInfo.g_fScaleX)), 250),
            //                      Math.Min((int)Math.Ceiling((250 / Max) * (height * m_smVisionInfo.g_fScaleY)), 250),
            //                      dc1,
            //                      (int)Math.Round(x * m_smVisionInfo.g_fScaleX),
            //                      (int)Math.Round(y * m_smVisionInfo.g_fScaleY),
            //                      (int)Math.Ceiling((width) * m_smVisionInfo.g_fScaleX) + 1,
            //                      (int)Math.Ceiling((height) * m_smVisionInfo.g_fScaleY) + 1,
            //                      0x00CC0020);
            //BitBlt(dc2, 0, 0, 244, 261, dc1, (int)(x * m_smVisionInfo.g_fScaleX), (int)(y * m_smVisionInfo.g_fScaleY), 0x00CC0020);
            // m_g = Graphics.FromHwnd(pic_Image.Handle);
            float ScaleX = 1;
            float ScaleY = 1;
            if (m_blnFeretActivated)
            {
                ScaleX = fFeretWidth / Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250);
                ScaleY = fFeretHeight / Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250);
            }

            // Clean up !!
            // m_g.ReleaseHdc(dc1);
            // g2.ReleaseHdc(dc2);
            //m_g1 = g;
            // m_g2 = g2;
            // m_g2 = Graphics.FromHwnd(pic_Image.Handle);
            //pic_Image.Size = new Size(Math.Min((int)Math.Ceiling((250f / Max) * (m_w * m_smVisionInfo.g_fScaleX)), 250), Math.Min((int)Math.Ceiling((250f / Max) * (m_h * m_smVisionInfo.g_fScaleY)), 250));
            pic_Image.Size = new Size(Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250), Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250));

            //pic_Image.Scale(ScaleX,ScaleY);
            pic_Image.Location = new Point(panel1.Size.Width / 2 - pic_Image.Width / 2, panel1.Size.Height / 2 - pic_Image.Height / 2);

            //RedrawImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], m_g, 244, 261);
            //m_g = Graphics.FromHwnd(pic_Image.Handle);
            //int intPositionX = m_stcBlobPad.re
            //int intPositionY
            //    int intStartX = m_stcBlobPad.fStartX;
            //    int intStartY = m_stcBlobPad.fStartY;
            //    int intEndX = m_stcBlobPad.fEndX;
            //    int intEndY = m_stcBlobPad.fEndY;
            for (int a = 0; a < 12; a++)
            {
                m_StartPercentX.Add(0.0f);
                m_EndPercentX.Add(0.0f);
                m_StartPercentY.Add(0.0f);
                m_EndPercentY.Add(0.0f);

                m_InwardStartPercent.Add(0.0f);
                m_InwardEndPercent.Add(0.0f);

                m_MeasureMethod.Add(0);
            }
            //pic_Image.Size= new Size((int)(width * m_smVisionInfo.g_fScaleX * (250 / Max)), (int)(height * m_smVisionInfo.g_fScaleY * (250 / Max)));
            //  SplitLineHitTest(intPositionX, intPositionY);
            //  m_smVisionInfo.g_arrPadROIs[i][2].DragSplitLine(intPositionX, intPositionY);
            m_smVisionInfo.g_arrPad[m_intPadNo].GetPercentage(intSelectedPadNo,ref m_StartPercentX, ref m_StartPercentY, ref m_EndPercentX, ref m_EndPercentY, 1, ref m_InwardStartPercent, ref m_InwardEndPercent, ref m_MeasureMethod);

            txt_InwardStartPercent1.Text = m_InwardStartPercent[0].ToString();
            trackBar_InwardStartPercent1.Value = Convert.ToInt32(txt_InwardStartPercent1.Text);
            txt_InwardEndPercent1.Text = m_InwardEndPercent[0].ToString();
            trackBar_InwardEndPercent1.Value = Convert.ToInt32(txt_InwardEndPercent1.Text);

            cbo_MeasureMethod1.SelectedIndex = m_MeasureMethod[0];

            m_smVisionInfo.g_arrPad[m_intPadNo].SetROI((int)Math.Round((0 + (pic_Image.Width * (m_StartPercentX[0] / 100))), 0, MidpointRounding.AwayFromZero),//0
                (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * (m_StartPercentY[0] / 100))), 0, MidpointRounding.AwayFromZero),
                (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) * (m_EndPercentX[0] / 100)), 0, MidpointRounding.AwayFromZero),//Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250)
                (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * (m_EndPercentY[0] / 100))), 0, MidpointRounding.AwayFromZero),
                0,
                m_InwardStartPercent[0],
                m_InwardEndPercent[0]);

            if (0 > m_Start.Count - 1)
            {
                m_Start.Add(new PointF((int)Math.Round((0 + (pic_Image.Width * (m_StartPercentX[0] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * m_StartPercentY[0] / 100)), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_Start[0] = new PointF((int)Math.Round((0 + (pic_Image.Width * (m_StartPercentX[0] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * m_StartPercentY[0] / 100)), 0, MidpointRounding.AwayFromZero));
            }

            if (0 > m_End.Count - 1)
            {
                m_End.Add(new PointF((int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) * (m_EndPercentX[0] / 100)), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * (m_EndPercentY[0] / 100))), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_End[0] = new PointF((int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) * (m_EndPercentX[0] / 100)), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * (m_EndPercentY[0] / 100))), 0, MidpointRounding.AwayFromZero));
            }

            if (0 > m_StartOri.Count - 1)
            {
                m_StartOri.Add(new PointF((int)Math.Round((0 + (pic_Image.Width * (m_StartPercentX[0] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * m_StartPercentY[0] / 100)), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_StartOri[0] = new PointF((int)Math.Round((0 + (pic_Image.Width * (m_StartPercentX[0] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * m_StartPercentY[0] / 100)), 0, MidpointRounding.AwayFromZero));
            }

            if (0 > m_EndOri.Count - 1)
            {
                m_EndOri.Add(new PointF((int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) * (m_EndPercentX[0] / 100)), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * (m_EndPercentY[0] / 100))), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_EndOri[0] = new PointF((int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) * (m_EndPercentX[0] / 100)), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * (m_EndPercentY[0] / 100))), 0, MidpointRounding.AwayFromZero));
            }
            // m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedImage].SetROI(0, (int)(pic_Image.Height / 2), Math.Min((int)Math.Ceiling((250 / Max) * (m_w * m_smVisionInfo.g_fScaleX)), 250), (int)(pic_Image.Height / 2));
            UpdateImage(0);


            m_smVisionInfo.g_arrPad[m_intPadNo].GetPercentage(intSelectedPadNo,ref m_StartPercentX, ref m_StartPercentY, ref m_EndPercentX, ref m_EndPercentY, 2, ref m_InwardStartPercent, ref m_InwardEndPercent, ref m_MeasureMethod);

            txt_InwardStartPercent2.Text = m_InwardStartPercent[1].ToString();
            trackBar_InwardStartPercent2.Value = Convert.ToInt32(txt_InwardStartPercent2.Text);
            txt_InwardEndPercent2.Text = m_InwardEndPercent[1].ToString();
            trackBar_InwardEndPercent2.Value = Convert.ToInt32(txt_InwardEndPercent2.Text);

            cbo_MeasureMethod2.SelectedIndex = m_MeasureMethod[1];

            m_smVisionInfo.g_arrPad[m_intPadNo].SetROI((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_StartPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero),
                (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[1] / 100))), 0, MidpointRounding.AwayFromZero),//0
                (int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_EndPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero),
                (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[1] / 100)), 0, MidpointRounding.AwayFromZero),//Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250)
                1,
                m_InwardStartPercent[1],
                m_InwardEndPercent[1]);

            if (1 > m_Start.Count - 1)
            {
                m_Start.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_StartPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[1] / 100))), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_Start[1] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_StartPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[1] / 100))), 0, MidpointRounding.AwayFromZero));
            }

            if (1 > m_End.Count - 1)
            {
                m_End.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_EndPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[1] / 100)), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_End[1] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_EndPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[1] / 100)), 0, MidpointRounding.AwayFromZero));
            }

            if (1 > m_StartOri.Count - 1)
            {
                m_StartOri.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_StartPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[1] / 100))), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_StartOri[1] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_StartPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[1] / 100))), 0, MidpointRounding.AwayFromZero));
            }

            if (1 > m_EndOri.Count - 1)
            {
                m_EndOri.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_EndPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[1] / 100)), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_EndOri[1] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_EndPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[1] / 100)), 0, MidpointRounding.AwayFromZero));
            }

            UpdateImage(1);

            if (m_intLineCount > 2)
            {
                for (int i = 2; i < m_intLineCount; i++)
                {
                    Max = Math.Max((int)(m_w), (int)(m_h));
                    m_smVisionInfo.g_arrPad[m_intPadNo].GetPercentage(intSelectedPadNo, ref m_StartPercentX, ref m_StartPercentY, ref m_EndPercentX, ref m_EndPercentY, i + 1, ref m_InwardStartPercent, ref m_InwardEndPercent, ref m_MeasureMethod);

                    if (i == 2)
                    {
                        gb_Line3.Visible = true;
                        txt_InwardStartPercent3.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent3.Value = Convert.ToInt32(txt_InwardStartPercent3.Text);
                        txt_InwardEndPercent3.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent3.Value = Convert.ToInt32(txt_InwardEndPercent3.Text);
                        cbo_MeasureMethod3.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 3)
                    {
                        gb_Line4.Visible = true;
                        txt_InwardStartPercent4.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent4.Value = Convert.ToInt32(txt_InwardStartPercent4.Text);
                        txt_InwardEndPercent4.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent4.Value = Convert.ToInt32(txt_InwardEndPercent4.Text);
                        cbo_MeasureMethod4.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 4)
                    {
                        gb_Line5.Visible = true;
                        txt_InwardStartPercent5.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent5.Value = Convert.ToInt32(txt_InwardStartPercent5.Text);
                        txt_InwardEndPercent5.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent5.Value = Convert.ToInt32(txt_InwardEndPercent5.Text);
                        cbo_MeasureMethod5.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 5)
                    {
                        gb_Line6.Visible = true;
                        txt_InwardStartPercent6.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent6.Value = Convert.ToInt32(txt_InwardStartPercent6.Text);
                        txt_InwardEndPercent6.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent6.Value = Convert.ToInt32(txt_InwardEndPercent6.Text);
                        cbo_MeasureMethod6.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 6)
                    {
                        gb_Line7.Visible = true;
                        txt_InwardStartPercent7.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent7.Value = Convert.ToInt32(txt_InwardStartPercent7.Text);
                        txt_InwardEndPercent7.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent7.Value = Convert.ToInt32(txt_InwardEndPercent7.Text);
                        cbo_MeasureMethod7.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 7)
                    {
                        gb_Line8.Visible = true;
                        txt_InwardStartPercent8.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent8.Value = Convert.ToInt32(txt_InwardStartPercent8.Text);
                        txt_InwardEndPercent8.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent8.Value = Convert.ToInt32(txt_InwardEndPercent8.Text);
                        cbo_MeasureMethod8.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 8)
                    {
                        gb_Line9.Visible = true;
                        txt_InwardStartPercent9.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent9.Value = Convert.ToInt32(txt_InwardStartPercent9.Text);
                        txt_InwardEndPercent9.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent9.Value = Convert.ToInt32(txt_InwardEndPercent9.Text);
                        cbo_MeasureMethod9.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 9)
                    {
                        gb_Line10.Visible = true;
                        txt_InwardStartPercent10.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent10.Value = Convert.ToInt32(txt_InwardStartPercent10.Text);
                        txt_InwardEndPercent10.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent10.Value = Convert.ToInt32(txt_InwardEndPercent10.Text);
                        cbo_MeasureMethod10.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 10)
                    {
                        gb_Line11.Visible = true;
                        txt_InwardStartPercent11.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent11.Value = Convert.ToInt32(txt_InwardStartPercent11.Text);
                        txt_InwardEndPercent11.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent11.Value = Convert.ToInt32(txt_InwardEndPercent11.Text);
                        cbo_MeasureMethod11.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 11)
                    {
                        gb_Line12.Visible = true;
                        txt_InwardStartPercent12.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent12.Value = Convert.ToInt32(txt_InwardStartPercent12.Text);
                        txt_InwardEndPercent12.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent12.Value = Convert.ToInt32(txt_InwardEndPercent12.Text);
                        cbo_MeasureMethod12.SelectedIndex = m_MeasureMethod[i];
                    }

                    m_smVisionInfo.g_arrPad[m_intPadNo].SetROI((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_StartPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                        (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[i] / 100))), 0, MidpointRounding.AwayFromZero),
                        (int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_EndPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                        (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[i] / 100)), 0, MidpointRounding.AwayFromZero),
                        i,
                        m_InwardStartPercent[i],
                        m_InwardEndPercent[i]);

                    if (m_intLineCount > m_Start.Count - 1)
                    {
                        m_Start.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_StartPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[i] / 100))), 0, MidpointRounding.AwayFromZero)));

                    }
                    else
                    {
                        m_Start[i] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_StartPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[i] / 100))), 0, MidpointRounding.AwayFromZero));
                    }

                    if (m_intLineCount > m_End.Count - 1)
                    {
                        m_End.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) +((pic_Image.Width * (m_EndPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[i] / 100)), 0, MidpointRounding.AwayFromZero)));

                    }
                    else
                    {
                        m_End[i] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_EndPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[i] / 100)), 0, MidpointRounding.AwayFromZero));
                    }


                    if (m_intLineCount > m_StartOri.Count - 1)
                    {
                        m_StartOri.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_StartPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[i] / 100))), 0, MidpointRounding.AwayFromZero)));

                    }
                    else
                    {
                        m_StartOri[i] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_StartPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[i] / 100))), 0, MidpointRounding.AwayFromZero));
                    }

                    if (m_intLineCount > m_EndOri.Count - 1)
                    {
                        m_EndOri.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_EndPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[i] / 100)), 0, MidpointRounding.AwayFromZero)));

                    }
                    else
                    {
                        m_EndOri[i] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_EndPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[i] / 100)), 0, MidpointRounding.AwayFromZero));
                    }
                    UpdateImage(i);
                }
            }
            // m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedImage].DrawLines(g2, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY, 0, 0, intSelectedPadNo);
            // m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedImage].DrawLines(m_g, (250f / Max), (250f / Max), 0, 0, intSelectedPadNo);
            //m_g = g2;
            // m_g = Graphics.FromHwnd(pic_Image.Handle);
            // m_g.Clear(Color.Transparent);

            gb_Line1.Paint += PaintBorderlessGroupBox;
            gb_Line2.Paint += PaintBorderlessGroupBox;
            gb_Line3.Paint += PaintBorderlessGroupBox;
            gb_Line4.Paint += PaintBorderlessGroupBox;
            gb_Line5.Paint += PaintBorderlessGroupBox;
            gb_Line6.Paint += PaintBorderlessGroupBox;
            gb_Line7.Paint += PaintBorderlessGroupBox;
            gb_Line8.Paint += PaintBorderlessGroupBox;
            gb_Line9.Paint += PaintBorderlessGroupBox;
            gb_Line10.Paint += PaintBorderlessGroupBox;
            gb_Line11.Paint += PaintBorderlessGroupBox;
            gb_Line12.Paint += PaintBorderlessGroupBox;

            m_blnInitDone = true;
        }
        private void PaintBorderlessGroupBox(object sender, PaintEventArgs p)
        {
            GroupBox box = (GroupBox)sender;
            Graphics g = p.Graphics;
            Color objColor = Color.Black;

            switch (box.Name)
            {
                case "gb_Line1":
                    if (m_intLengthMode == 2 && !m_blnFeretActivated) //m_w >= m_h
                        greenfirst = true;
                    else if (m_intLengthMode == 1 && !m_blnFeretActivated) //m_w <= m_h
                        greenfirst = false;

                    else if (m_w > m_h && m_blnFeretActivated) //m_w >= m_h
                        greenfirst = true;
                    else
                        greenfirst = false;

                    if (greenfirst)
                    {
                        //p.Graphics.Clear(Color.FromArgb(210, 230, 255));
                        //p.Graphics.DrawString(box.Text, box.Font, Brushes.Lime, 0, 0);
                        objColor = Color.Lime;
                    }
                    else
                    {
                        //p.Graphics.Clear(Color.FromArgb(210, 230, 255));
                        //p.Graphics.DrawString(box.Text, box.Font, Brushes.Blue, 0, 0);
                        objColor = Color.Blue;
                    }
                    break;
                case "gb_Line2":
                    if (m_intLengthMode == 2 && !m_blnFeretActivated) //m_w >= m_h
                        greenfirst = false;
                    else if (m_intLengthMode == 1 && !m_blnFeretActivated) //m_w <= m_h
                        greenfirst = true;

                    else if (m_w > m_h && m_blnFeretActivated) //m_w >= m_h
                        greenfirst = false;
                    else
                        greenfirst = true;

                    if (greenfirst)
                    {
                        //p.Graphics.Clear(Color.FromArgb(210, 230, 255));
                        //p.Graphics.DrawString(box.Text, box.Font, Brushes.Lime, 0, 0);
                        objColor = Color.Lime;
                    }
                    else
                    {
                        //p.Graphics.Clear(Color.FromArgb(210, 230, 255));
                        //p.Graphics.DrawString(box.Text, box.Font, Brushes.Blue, 0, 0);
                        objColor = Color.Blue;
                    }
                    break;
                case "gb_Line3":
                    //p.Graphics.Clear(Color.FromArgb(210, 230, 255));
                    //p.Graphics.DrawString(box.Text, box.Font, Brushes.Red, 0, 0);
                    objColor = Color.Red;
                    break;
                case "gb_Line4":
                    //p.Graphics.Clear(Color.FromArgb(210, 230, 255));
                    //p.Graphics.DrawString(box.Text, box.Font, Brushes.Yellow, 0, 0);
                    objColor = Color.Yellow;
                    break;
                case "gb_Line5":
                    //p.Graphics.Clear(Color.FromArgb(210, 230, 255));
                    //p.Graphics.DrawString(box.Text, box.Font, Brushes.Pink, 0, 0);
                    objColor = Color.Pink;
                    break;
                case "gb_Line6":
                    //p.Graphics.Clear(Color.FromArgb(210, 230, 255));
                    //p.Graphics.DrawString(box.Text, box.Font, Brushes.Orange, 0, 0);
                    objColor = Color.Orange;
                    break;
                case "gb_Line7":
                    //p.Graphics.Clear(Color.FromArgb(210, 230, 255));
                    //p.Graphics.DrawString(box.Text, box.Font, Brushes.Cyan, 0, 0);
                    objColor = Color.Cyan;
                    break;
                case "gb_Line8":
                    //p.Graphics.Clear(Color.FromArgb(210, 230, 255));
                    //p.Graphics.DrawString(box.Text, box.Font, Brushes.Magenta, 0, 0);
                    objColor = Color.Magenta;
                    break;
                case "gb_Line9":
                    //p.Graphics.Clear(Color.FromArgb(210, 230, 255));
                    //p.Graphics.DrawString(box.Text, box.Font, Brushes.Silver, 0, 0);
                    objColor = Color.Silver;
                    break;
                case "gb_Line10":
                    //p.Graphics.Clear(Color.FromArgb(210, 230, 255));
                    //p.Graphics.DrawString(box.Text, box.Font, Brushes.Wheat, 0, 0);
                    objColor = Color.Wheat;
                    break;
                case "gb_Line11":
                    //p.Graphics.Clear(Color.FromArgb(210, 230, 255));
                    //p.Graphics.DrawString(box.Text, box.Font, Brushes.DarkGoldenrod, 0, 0);
                    objColor = Color.DarkGoldenrod;
                    break;
                case "gb_Line12":
                    //p.Graphics.Clear(Color.FromArgb(210, 230, 255));
                    //p.Graphics.DrawString(box.Text, box.Font, Brushes.MediumSeaGreen, 0, 0);
                    objColor = Color.MediumSeaGreen;
                    break;
            }

            if (box != null)
            {
                Brush textBrush = new SolidBrush(SystemColors.ControlText);
                Brush borderBrush = new SolidBrush(objColor);
                Pen borderPen = new Pen(borderBrush);
                SizeF strSize = g.MeasureString(box.Text, box.Font);
                Rectangle rect = new Rectangle(box.ClientRectangle.X,
                                               box.ClientRectangle.Y + (int)(strSize.Height / 2),
                                               box.ClientRectangle.Width - 2,
                                               box.ClientRectangle.Height - (int)(strSize.Height / 2) - 1);

                // Clear text and border
                g.Clear(this.BackColor);

                // Draw text
                g.DrawString(box.Text, box.Font, textBrush, box.Padding.Left, 0);

                // Drawing Border
                //Left
                g.DrawLine(borderPen, rect.Location, new Point(rect.X, rect.Y + rect.Height));
                //Right
                g.DrawLine(borderPen, new Point(rect.X + rect.Width, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                //Bottom
                g.DrawLine(borderPen, new Point(rect.X, rect.Y + rect.Height), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                //Top1
                g.DrawLine(borderPen, new Point(rect.X, rect.Y), new Point(rect.X + box.Padding.Left, rect.Y));
                //Top2
                g.DrawLine(borderPen, new Point(rect.X + box.Padding.Left + (int)(strSize.Width), rect.Y), new Point(rect.X + rect.Width, rect.Y));
            }
        }
        private void cbo_MeasureMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intTag = Convert.ToInt32(((ComboBox)sender).Tag);

            switch (intTag)
            {
                case 0:
                    m_MeasureMethod[intTag] = cbo_MeasureMethod1.SelectedIndex;
                    break;
                case 1:
                    m_MeasureMethod[intTag] = cbo_MeasureMethod2.SelectedIndex;
                    break;
                case 2:
                    m_MeasureMethod[intTag] = cbo_MeasureMethod3.SelectedIndex;
                    break;
                case 3:
                    m_MeasureMethod[intTag] = cbo_MeasureMethod4.SelectedIndex;
                    break;
                case 4:
                    m_MeasureMethod[intTag] = cbo_MeasureMethod5.SelectedIndex;
                    break;
                case 5:
                    m_MeasureMethod[intTag] = cbo_MeasureMethod6.SelectedIndex;
                    break;
                case 6:
                    m_MeasureMethod[intTag] = cbo_MeasureMethod7.SelectedIndex;
                    break;
                case 7:
                    m_MeasureMethod[intTag] = cbo_MeasureMethod8.SelectedIndex;
                    break;
                case 8:
                    m_MeasureMethod[intTag] = cbo_MeasureMethod9.SelectedIndex;
                    break;
                case 9:
                    m_MeasureMethod[intTag] = cbo_MeasureMethod10.SelectedIndex;
                    break;
                case 10:
                    m_MeasureMethod[intTag] = cbo_MeasureMethod11.SelectedIndex;
                    break;
                case 11:
                    m_MeasureMethod[intTag] = cbo_MeasureMethod12.SelectedIndex;
                    break;
            }

            m_bUpdatePictureBox = true;
        }

        private void btn_AutoAngle_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (Math.Round(m_FeretAngle) > 90)
                txt_PadAngle.Text = Math.Round(180 - m_FeretAngle).ToString();
            else if (Math.Round(m_FeretAngle) < -90)
                txt_PadAngle.Text = Math.Round(180 + m_FeretAngle).ToString();
            else
                txt_PadAngle.Text = Math.Round(m_FeretAngle).ToString();

            m_bUpdatePictureBox = true;
        }

        private void txt_PadAngle_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_bUpdatePictureBox = true;
        }

        private void chk_DrawCrosshair_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_bUpdatePictureBox = true;
        }

        private void btn_Extend_Click(object sender, EventArgs e)
        {
            if (btn_Extend.Text.Contains(">>"))
            {
                if (m_intSelectedLanguage == 2)
                    btn_Extend.Text = "编辑尺寸 << ";
                else if (m_intSelectedLanguage == 1)
                    btn_Extend.Text = "Edit Dimension <<";
                this.Size = new Size(980, 460);
            }
            else if (btn_Extend.Text.Contains("<<"))
            {
                if (m_intSelectedLanguage == 2)
                    btn_Extend.Text = "编辑尺寸 >>";
                else if (m_intSelectedLanguage == 1)
                    btn_Extend.Text = "Edit Dimension >>";
                this.Size = new Size(252, 460);
            }
        }

        private void chk_Feret_Click(object sender, EventArgs e)
        {
            m_blnFeretActivated = chk_Feret.Checked;
            m_smVisionInfo.g_arrPad[m_intPadNo].EnableDisableFeret(m_intselectedpad - 1, chk_Feret.Checked);
            // timer_Live.Stop();
            m_bUpdatePictureBox = false;
            RedrawPic();
            //timer_Live.Start();
            m_bUpdatePictureBox = true;
        }

        private void RedrawPic()
        {
            m_x = m_OriX;
            m_y = m_OriY;
            m_w = m_OriW;
            m_h = m_OriH;

            m_objRotatedPadImage = new ImageDrawing(true);
            m_objPadImage = new ImageDrawing(true);


            objROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
            if (chk_Feret.Checked)
            {
                int MaxW = 0, MaxH = 0, MinStartX = 0, MinStartY = 0, FinalMax = 0;
                MaxW = Math.Max(m_MaxX - m_MinX, (int)m_fFeretWidth);
                MaxH = Math.Max(m_MaxY - m_MinY, (int)m_fFeretHeight);
                FinalMax = Math.Max(MaxW, MaxH);
                MinStartX = (int)(((m_MinX + m_MaxX) / 2) - (FinalMax / 2));
                MinStartY = (int)(((m_MinY + m_MaxY) / 2) - (FinalMax / 2));

                objROI.LoadROISetting(MinStartX, MinStartY, FinalMax, FinalMax);
                //MinStartX = (int)(((MinX+MaxX)/2)-(MaxW/2));
                //MinStartY = (int)(((MinY+MaxY)/2)-(MaxH/2));

                //objROI.LoadROISetting(MinStartX, MinStartY, MaxW, MaxH);
                objROI.CopyToImage(ref m_objRotatedPadImage);
                objROI.CopyToImage(ref m_objPadImage);
            }
            else
            {
                objROI.LoadROISetting(m_x, m_y, m_w, m_h);
                objROI.CopyToImage(ref m_objRotatedPadImage);
                objROI.CopyToImage(ref m_objPadImage);
            }
            //objROI.SaveImage("D:\\objROI.bmp");
            //m_objPadImage.SaveImage("D:\\m_objPadImage.bmp");
            // if feret activated and ABS(angle > 0
            if (chk_Feret.Checked)
            {
                float Angle = m_FeretAngle;

                //else if (Angle <= -85)
                //if (Angle >= 85)
                //    Angle = m_FeretAngle - 90;
                //else if (Angle <= -85)
                //    Angle = m_FeretAngle + 90;
                //if (Angle>0 && Angle <= 45)
                //    Angle = -Angle;
                // m_objRotatedPadImage.SaveImage("D:\\RotatedPadBefore.bmp");
                ROI.RotateROI_DifferentImageSize(objROI, Angle, ref m_objRotatedPadImage, (int)m_fFeretWidth, (int)m_fFeretHeight);
                //m_objRotatedPadImage.SaveImage("D:\\RotatedPad.bmp");
                objROI.AttachImage(m_objRotatedPadImage);
                //if (m_FeretAngle >= 85 || m_FeretAngle <= -85)
                //{
                //    objROI.LoadROISetting((int)(m_objRotatedPadImage.ref_intImageHeight / 2 - (int)fFeretHeight / 2) - 2,
                //        (int)(m_objRotatedPadImage.ref_intImageWidth / 2 - (int)fFeretWidth / 2) - 2,
                //        (int)fFeretHeight + 4, (int)fFeretWidth + 4);
                //}
                //else
                {
                    objROI.LoadROISetting((int)(m_objRotatedPadImage.ref_intImageWidth / 2 - (int)m_fFeretWidth / 2) - 2,
                           (int)(m_objRotatedPadImage.ref_intImageHeight / 2 - (int)m_fFeretHeight / 2) - 2,
                           (int)m_fFeretWidth + 4, (int)m_fFeretHeight + 4);
                }
                objROI.CopyToImage(ref m_objPadImage);
                // objROI.SaveImage("D:\\objROI.bmp");
                //m_objPadImage.SaveImage("D:\\m_objPadImage.bmp");
                //Angle = m_FeretAngle;
                //ROI.RotateROI_DifferentImageSize(objROI, Angle, ref m_objPadImage);

            }

            //m_g = Graphics.FromHwnd(pic_Image.Handle);

          
           

        

            // m_stcBlobPad = (BlobsFeatures)m_arrTemplateBlobPads[intSelectedPadNo];
            // m_objTemplate = new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
            //int Max = Math.Max((int)Math.Ceiling(width * m_smVisionInfo.g_fScaleX), (int)Math.Ceiling(height * m_smVisionInfo.g_fScaleY));

            if (chk_Feret.Checked)
            {
                m_x = (int)m_FeretStartX;
                m_y = (int)m_FeretStartY;
                //if (m_FeretAngle >= 85 || m_FeretAngle <= -85)
                //{
                //    m_h = m_objPadImage.ref_intImageHeight; //(int)fFeretWidth;
                //    m_w = m_objPadImage.ref_intImageWidth; //(int)fFeretHeight;
                //}
                //else
                {
                    m_w = m_objPadImage.ref_intImageWidth; // (int)fFeretWidth;
                    m_h = m_objPadImage.ref_intImageHeight; //(int)fFeretHeight;
                }
            }


            int Max = Math.Max((int)(m_w), (int)(m_h));

            //  m_g = Graphics.FromHwnd(pic_Image.Handle);
            //if (Max > 250)
            //    Max = 250;
            // m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, 128, m_smVisionInfo.g_arrPadROIs[0], 0);
            // m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].NewDrawZoomImage(g, 0.01f, 0, 0, x, y);
            // Image i = new Bitmap(250, 250, m_g);
            //m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(i) ;
            // pic_Image.Image = i;
            // Get a Graphics object for the image
            //  Graphics g2 = Graphics.FromImage(i);

            // Now get handles to device contexts, and perform the bit blitting operation.
            //dc1 = m_g.GetHdc();
            //dc2 = g2.GetHdc();
            //float desWidth = Math.Min((int)Math.Ceiling((250 / Max) * (width * m_smVisionInfo.g_fScaleX)), 250);
            //float desHeight = Math.Min((int)Math.Ceiling((250 / Max) * (height * m_smVisionInfo.g_fScaleY)), 250);
            //float sourceX = x * m_smVisionInfo.g_fScaleX;
            //float sourceY = y * m_smVisionInfo.g_fScaleY;
            //int intSourceX = (int)Math.Round(x * m_smVisionInfo.g_fScaleX);
            //int intSourceY = (int)Math.Round(y * m_smVisionInfo.g_fScaleY);
            //float sourceW = (width + 1) * m_smVisionInfo.g_fScaleX;
            //float sourceH = (height + 1) * m_smVisionInfo.g_fScaleY;
            //int intsourceW = (int)Math.Ceiling((width) * m_smVisionInfo.g_fScaleX);
            //int intsourceH = (int)Math.Ceiling((height) * m_smVisionInfo.g_fScaleY);
            //StretchBlt(dc2, 0, 0, Math.Min((int)Math.Ceiling((250 / Max) * (width * m_smVisionInfo.g_fScaleX)), 250),
            //                      Math.Min((int)Math.Ceiling((250 / Max) * (height * m_smVisionInfo.g_fScaleY)), 250),
            //                      dc1,
            //                      (int)Math.Round(x * m_smVisionInfo.g_fScaleX),
            //                      (int)Math.Round(y * m_smVisionInfo.g_fScaleY),
            //                      (int)Math.Ceiling((width) * m_smVisionInfo.g_fScaleX) + 1,
            //                      (int)Math.Ceiling((height) * m_smVisionInfo.g_fScaleY) + 1,
            //                      0x00CC0020);
            //BitBlt(dc2, 0, 0, 244, 261, dc1, (int)(x * m_smVisionInfo.g_fScaleX), (int)(y * m_smVisionInfo.g_fScaleY), 0x00CC0020);
            // m_g = Graphics.FromHwnd(pic_Image.Handle);
            float ScaleX = 1;
            float ScaleY = 1;
            if (chk_Feret.Checked)
            {
                ScaleX = m_fFeretWidth / Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250);
                ScaleY = m_fFeretHeight / Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250);
            }

            // Clean up !!
            // m_g.ReleaseHdc(dc1);
            // g2.ReleaseHdc(dc2);
            //m_g1 = g;
            // m_g2 = g2;
            // m_g2 = Graphics.FromHwnd(pic_Image.Handle);
            //pic_Image.Size = new Size(Math.Min((int)Math.Ceiling((250f / Max) * (m_w * m_smVisionInfo.g_fScaleX)), 250), Math.Min((int)Math.Ceiling((250f / Max) * (m_h * m_smVisionInfo.g_fScaleY)), 250));
            pic_Image.Size = new Size(Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250), Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250));

            //pic_Image.Scale(ScaleX,ScaleY);
            pic_Image.Location = new Point(panel1.Size.Width / 2 - pic_Image.Width / 2, panel1.Size.Height / 2 - pic_Image.Height / 2);

            //RedrawImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], m_g, 244, 261);
            //m_g = Graphics.FromHwnd(pic_Image.Handle);
            //int intPositionX = m_stcBlobPad.re
            //int intPositionY
            //    int intStartX = m_stcBlobPad.fStartX;
            //    int intStartY = m_stcBlobPad.fStartY;
            //    int intEndX = m_stcBlobPad.fEndX;
            //    int intEndY = m_stcBlobPad.fEndY;
            for (int a = 0; a < 12; a++)
            {
                m_StartPercentX[a] = 0.0f;
                m_EndPercentX[a] = 0.0f;
                m_StartPercentY[a] = 0.0f;
                m_EndPercentY[a] = 0.0f;

                m_InwardStartPercent[a] = 0.0f;
                m_InwardEndPercent[a] = 0.0f;

                m_MeasureMethod[a] = 0;
            }
            //pic_Image.Size= new Size((int)(width * m_smVisionInfo.g_fScaleX * (250 / Max)), (int)(height * m_smVisionInfo.g_fScaleY * (250 / Max)));
            //  SplitLineHitTest(intPositionX, intPositionY);
            //  m_smVisionInfo.g_arrPadROIs[i][2].DragSplitLine(intPositionX, intPositionY);
            //m_smVisionInfo.g_arrPad[m_intPadNo].GetPercentage(m_intselectedpad, ref m_StartPercentX, ref m_StartPercentY, ref m_EndPercentX, ref m_EndPercentY, 1);

            txt_InwardStartPercent1.Text = m_InwardStartPercent[0].ToString();
            trackBar_InwardStartPercent1.Value = Convert.ToInt32(txt_InwardStartPercent1.Text);
            txt_InwardEndPercent1.Text = m_InwardEndPercent[0].ToString();
            trackBar_InwardEndPercent1.Value = Convert.ToInt32(txt_InwardEndPercent1.Text);

            cbo_MeasureMethod1.SelectedIndex = m_MeasureMethod[0];

            m_smVisionInfo.g_arrPad[m_intPadNo].SetROI((int)Math.Round((0 + (pic_Image.Width * (m_StartPercentX[0] / 100))), 0, MidpointRounding.AwayFromZero),//0
                (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * (m_StartPercentY[0] / 100))), 0, MidpointRounding.AwayFromZero),
                (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) * (m_EndPercentX[0] / 100)), 0, MidpointRounding.AwayFromZero),//Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250)
                (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * (m_EndPercentY[0] / 100))), 0, MidpointRounding.AwayFromZero),
                0,
                m_InwardStartPercent[0],
                m_InwardEndPercent[0]);

            if (0 > m_Start.Count - 1)
            {
                m_Start.Add(new PointF((int)Math.Round((0 + (pic_Image.Width * (m_StartPercentX[0] / 100))), 0, MidpointRounding.AwayFromZero), (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * m_StartPercentY[0] / 100)), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_Start[0] = new PointF((int)Math.Round((0 + (pic_Image.Width * (m_StartPercentX[0] / 100))), 0, MidpointRounding.AwayFromZero), (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * m_StartPercentY[0] / 100)), 0, MidpointRounding.AwayFromZero));
            }

            if (0 > m_End.Count - 1)
            {
                m_End.Add(new PointF((int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) * (m_EndPercentX[0] / 100)), 0, MidpointRounding.AwayFromZero), (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * (m_EndPercentY[0] / 100))), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_End[0] = new PointF((int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) * (m_EndPercentX[0] / 100)), 0, MidpointRounding.AwayFromZero), (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * (m_EndPercentY[0] / 100))), 0, MidpointRounding.AwayFromZero));
            }

            if (0 > m_StartOri.Count - 1)
            {
                m_StartOri.Add(new PointF((int)Math.Round((0 + (pic_Image.Width * (m_StartPercentX[0] / 100))), 0, MidpointRounding.AwayFromZero), (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * m_StartPercentY[0] / 100)), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_StartOri[0] = new PointF((int)Math.Round((0 + (pic_Image.Width * (m_StartPercentX[0] / 100))), 0, MidpointRounding.AwayFromZero), (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * m_StartPercentY[0] / 100)), 0, MidpointRounding.AwayFromZero));
            }

            if (0 > m_EndOri.Count - 1)
            {
                m_EndOri.Add(new PointF((int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) * (m_EndPercentX[0] / 100)), 0, MidpointRounding.AwayFromZero), (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * (m_EndPercentY[0] / 100))), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_EndOri[0] = new PointF((int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250) * (m_EndPercentX[0] / 100)), 0, MidpointRounding.AwayFromZero), (int)Math.Round(((pic_Image.Height / 2) + (pic_Image.Height * (m_EndPercentY[0] / 100))), 0, MidpointRounding.AwayFromZero));
            }
            // m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedImage].SetROI(0, (int)(pic_Image.Height / 2), Math.Min((int)Math.Ceiling((250 / Max) * (m_w * m_smVisionInfo.g_fScaleX)), 250), (int)(pic_Image.Height / 2));
            UpdateImage(0);


            //m_smVisionInfo.g_arrPad[m_intPadNo].GetPercentage(m_intselectedpad, ref m_StartPercentX, ref m_StartPercentY, ref m_EndPercentX, ref m_EndPercentY, 2);

            txt_InwardStartPercent2.Text = m_InwardStartPercent[1].ToString();
            trackBar_InwardStartPercent2.Value = Convert.ToInt32(txt_InwardStartPercent2.Text);
            txt_InwardEndPercent2.Text = m_InwardEndPercent[1].ToString();
            trackBar_InwardEndPercent2.Value = Convert.ToInt32(txt_InwardEndPercent2.Text);

            cbo_MeasureMethod2.SelectedIndex = m_MeasureMethod[1];

            m_smVisionInfo.g_arrPad[m_intPadNo].SetROI((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_StartPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero),
                (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[1] / 100))), 0, MidpointRounding.AwayFromZero),//0
                (int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_EndPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero),
                (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[1] / 100)), 0, MidpointRounding.AwayFromZero),//Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250)
                1,
                m_InwardStartPercent[1],
                m_InwardEndPercent[1]);

            if (1 > m_Start.Count - 1)
            {
                m_Start.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_StartPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero), (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[1] / 100))), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_Start[1] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_StartPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero), (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[1] / 100))), 0, MidpointRounding.AwayFromZero));
            }

            if (1 > m_End.Count - 1)
            {
                m_End.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_EndPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero), (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[1] / 100)), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_End[1] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_EndPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero), (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[1] / 100)), 0, MidpointRounding.AwayFromZero));
            }

            if (1 > m_StartOri.Count - 1)
            {
                m_StartOri.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_StartPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero), (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[1] / 100))), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_StartOri[1] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_StartPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero), (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[1] / 100))), 0, MidpointRounding.AwayFromZero));
            }

            if (1 > m_EndOri.Count - 1)
            {
                m_EndOri.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_EndPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero), (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[1] / 100)), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_EndOri[1] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width * (m_EndPercentX[1] / 100))), 0, MidpointRounding.AwayFromZero), (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[1] / 100)), 0, MidpointRounding.AwayFromZero));
            }

            UpdateImage(1);

            if (m_intLineCount > 2)
            {
                for (int i = 2; i < m_intLineCount; i++)
                {
                    if (i == 2)
                    {
                        gb_Line3.Visible = true;
                        txt_InwardStartPercent3.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent3.Value = Convert.ToInt32(txt_InwardStartPercent3.Text);
                        txt_InwardEndPercent3.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent3.Value = Convert.ToInt32(txt_InwardEndPercent3.Text);
                        cbo_MeasureMethod3.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 3)
                    {
                        gb_Line4.Visible = true;
                        txt_InwardStartPercent4.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent4.Value = Convert.ToInt32(txt_InwardStartPercent4.Text);
                        txt_InwardEndPercent4.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent4.Value = Convert.ToInt32(txt_InwardEndPercent4.Text);
                        cbo_MeasureMethod4.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 4)
                    {
                        gb_Line5.Visible = true;
                        txt_InwardStartPercent5.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent5.Value = Convert.ToInt32(txt_InwardStartPercent5.Text);
                        txt_InwardEndPercent5.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent5.Value = Convert.ToInt32(txt_InwardEndPercent5.Text);
                        cbo_MeasureMethod5.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 5)
                    {
                        gb_Line6.Visible = true;
                        txt_InwardStartPercent6.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent6.Value = Convert.ToInt32(txt_InwardStartPercent6.Text);
                        txt_InwardEndPercent6.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent6.Value = Convert.ToInt32(txt_InwardEndPercent6.Text);
                        cbo_MeasureMethod6.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 6)
                    {
                        gb_Line7.Visible = true;
                        txt_InwardStartPercent7.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent7.Value = Convert.ToInt32(txt_InwardStartPercent7.Text);
                        txt_InwardEndPercent7.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent7.Value = Convert.ToInt32(txt_InwardEndPercent7.Text);
                        cbo_MeasureMethod7.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 7)
                    {
                        gb_Line8.Visible = true;
                        txt_InwardStartPercent8.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent8.Value = Convert.ToInt32(txt_InwardStartPercent8.Text);
                        txt_InwardEndPercent8.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent8.Value = Convert.ToInt32(txt_InwardEndPercent8.Text);
                        cbo_MeasureMethod8.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 8)
                    {
                        gb_Line9.Visible = true;
                        txt_InwardStartPercent9.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent9.Value = Convert.ToInt32(txt_InwardStartPercent9.Text);
                        txt_InwardEndPercent9.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent9.Value = Convert.ToInt32(txt_InwardEndPercent9.Text);
                        cbo_MeasureMethod9.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 9)
                    {
                        gb_Line10.Visible = true;
                        txt_InwardStartPercent10.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent10.Value = Convert.ToInt32(txt_InwardStartPercent10.Text);
                        txt_InwardEndPercent10.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent10.Value = Convert.ToInt32(txt_InwardEndPercent10.Text);
                        cbo_MeasureMethod10.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 10)
                    {
                        gb_Line11.Visible = true;
                        txt_InwardStartPercent11.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent11.Value = Convert.ToInt32(txt_InwardStartPercent11.Text);
                        txt_InwardEndPercent11.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent11.Value = Convert.ToInt32(txt_InwardEndPercent11.Text);
                        cbo_MeasureMethod11.SelectedIndex = m_MeasureMethod[i];
                    }
                    else if (i == 11)
                    {
                        gb_Line12.Visible = true;
                        txt_InwardStartPercent12.Text = m_InwardStartPercent[i].ToString();
                        trackBar_InwardStartPercent12.Value = Convert.ToInt32(txt_InwardStartPercent12.Text);
                        txt_InwardEndPercent12.Text = m_InwardEndPercent[i].ToString();
                        trackBar_InwardEndPercent12.Value = Convert.ToInt32(txt_InwardEndPercent12.Text);
                        cbo_MeasureMethod12.SelectedIndex = m_MeasureMethod[i];
                    }

                    Max = Math.Max((int)(m_w), (int)(m_h));
                    //m_smVisionInfo.g_arrPad[m_intPadNo].GetPercentage(m_intselectedpad, ref m_StartPercentX, ref m_StartPercentY, ref m_EndPercentX, ref m_EndPercentY, i + 1);

                    m_smVisionInfo.g_arrPad[m_intPadNo].SetROI((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_StartPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                        (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[i] / 100))), 0, MidpointRounding.AwayFromZero),
                        (int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_EndPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                        (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[i] / 100)), 0, MidpointRounding.AwayFromZero),
                        i,
                        m_InwardStartPercent[i],
                        m_InwardEndPercent[i]);

                    if (m_intLineCount-1 > m_Start.Count - 1)
                    {
                        m_Start.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_StartPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[i] / 100))), 0, MidpointRounding.AwayFromZero)));

                    }
                    else
                    {
                        m_Start[i] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_StartPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[i] / 100))), 0, MidpointRounding.AwayFromZero));
                    }

                    if (m_intLineCount-1 > m_End.Count - 1)
                    {
                        m_End.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_EndPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[i] / 100)), 0, MidpointRounding.AwayFromZero)));

                    }
                    else
                    {
                        m_End[i] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_EndPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[i] / 100)), 0, MidpointRounding.AwayFromZero));
                    }


                    if (m_intLineCount-1 > m_StartOri.Count - 1)
                    {
                        m_StartOri.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_StartPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[i] / 100))), 0, MidpointRounding.AwayFromZero)));

                    }
                    else
                    {
                        m_StartOri[i] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_StartPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[i] / 100))), 0, MidpointRounding.AwayFromZero));
                    }

                    if (m_intLineCount-1 > m_EndOri.Count - 1)
                    {
                        m_EndOri.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_EndPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[i] / 100)), 0, MidpointRounding.AwayFromZero)));

                    }
                    else
                    {
                        m_EndOri[i] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / (i)) + ((pic_Image.Width * (m_EndPercentX[i] / 100))), 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[i] / 100)), 0, MidpointRounding.AwayFromZero));
                    }
                    UpdateImage(i);
                }
            }
            // m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedImage].DrawLines(g2, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY, 0, 0, intSelectedPadNo);
            // m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedImage].DrawLines(m_g, (250f / Max), (250f / Max), 0, 0, intSelectedPadNo);
            //m_g = g2;
            // m_g = Graphics.FromHwnd(pic_Image.Handle);
            // m_g.Clear(Color.Transparent);

        }

        private void DrawHorizontalLine()
        {
            int Max = Math.Max((int)(m_w), (int)(m_h));

            //m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedImage].SetROI(0, (int)(pic_Image.Height / 2), Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250), (int)(pic_Image.Height / 2),0);
            //m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedImage].SetROI(0, (int)(pic_Image.Height / 2), Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250), (int)(pic_Image.Height / 2));
            // m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedImage].SetROI(0, (int)(pic_Image.Height / 2), Math.Min((int)Math.Ceiling((250 / Max) * (m_w * m_smVisionInfo.g_fScaleX)), 250), (int)(pic_Image.Height / 2));
            //if(m_w>m_h)

            UpdateImage(0);
        }
        private void DrawVerticalLine()
        {

            int Max = Math.Max((int)(m_w), (int)(m_h));
            //m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedImage].SetROI((int)(pic_Image.Width / 2), 0, (int)(pic_Image.Width / 2), Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250),1);
            //m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedImage].SetROI((int)(pic_Image.Width / 2), 0, (int)(pic_Image.Width / 2), Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250));
            UpdateImage(1);
        }
        private void timer_Live_Tick(object sender, EventArgs e)
        {

            //int Max = Math.Max((int)Math.Ceiling(m_w * m_smVisionInfo.g_fScaleX), (int)Math.Ceiling(m_h * m_smVisionInfo.g_fScaleY));
            int Max = Math.Max((int)(m_w), (int)(m_h));
            //if (Max > 250)
            //    Max = 250;
            //m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedImage].DrawLines(m_g);
            //CopyImage();
            //m_g.Clear(Color.Transparent);
            //     CopyImage();
            // if (m_blnFeretActivated != chk_Feret.Checked)
            //{
            //Reset();
            //}
            if (m_bUpdatePictureBox)
            {
                m_bUpdatePictureBox = false;

                m_objPadImage.RedrawImage(m_g, (250f / Max), (250f / Max));
                DrawHorizontalLine();
                DrawVerticalLine();
                for (int i = 2; i < m_intLineCount; i++)
                    UpdateImage(i);


            }
            
        }
        private void UpdateImage(int i)
        {
            int Max = Math.Max((int)(m_w), (int)(m_h));
            if (i == 0) // horizontal line
            {
                if (m_intLengthMode == 2 && !m_blnFeretActivated) //m_w >= m_h
                    greenfirst = true;
                else if (m_intLengthMode == 1 && !m_blnFeretActivated) //m_w <= m_h
                    greenfirst = false;

                else if (m_w > m_h && m_blnFeretActivated) //m_w >= m_h
                    greenfirst = true;
                else
                    greenfirst = false;
            }
            if (i == 1) // vertical line
            {
                if (m_intLengthMode == 2 && !m_blnFeretActivated) //m_w >= m_h
                    greenfirst = false;
                else if (m_intLengthMode == 1 && !m_blnFeretActivated) //m_w <= m_h
                    greenfirst = true;

                else if (m_w > m_h && m_blnFeretActivated) //m_w >= m_h
                    greenfirst = false;
                else
                    greenfirst = true;
            }
            m_smVisionInfo.g_arrPad[m_intPadNo].DrawLines(m_g, (250f / Max), (250f / Max), 0, 0, i, greenfirst, Convert.ToInt32(txt_PadAngle.Text), m_MeasureMethod[i]);

            //if (chk_DrawCrosshair.Checked)
            //{
            //    System.Drawing.Point p1 = new System.Drawing.Point(pic_Image.Width / 2, -pic_Image.Height / 2);
            //    System.Drawing.Point p2 = new System.Drawing.Point(pic_Image.Width / 2, pic_Image.Height * 3 / 2);
            //    System.Drawing.Point p3 = new System.Drawing.Point(-pic_Image.Width / 2, pic_Image.Height/ 2);
            //    System.Drawing.Point p4 = new System.Drawing.Point(pic_Image.Width * 3 / 2, pic_Image.Height / 2);

            //    float newX1 = 0, newY1 = 0;
            //    float newX2 = 0, newY2 = 0;
            //    float newX3 = 0, newY3 = 0;
            //    float newX4 = 0, newY4 = 0;

            //    Math2.RotateWithAngleAccordingToReferencePoint(pic_Image.Width / 2, pic_Image.Height / 2, (float)p1.X, (float)p1.Y, Convert.ToInt32(txt_PadAngle.Text), ref newX1, ref newY1);
            //    Math2.RotateWithAngleAccordingToReferencePoint(pic_Image.Width / 2, pic_Image.Height / 2, (float)p3.X, (float)p3.Y, Convert.ToInt32(txt_PadAngle.Text), ref newX3, ref newY3);
            //    Math2.RotateWithAngleAccordingToReferencePoint(pic_Image.Width / 2, pic_Image.Height / 2, (float)p2.X, (float)p2.Y, Convert.ToInt32(txt_PadAngle.Text), ref newX2, ref newY2);
            //    Math2.RotateWithAngleAccordingToReferencePoint(pic_Image.Width / 2, pic_Image.Height / 2, (float)p4.X, (float)p4.Y, Convert.ToInt32(txt_PadAngle.Text), ref newX4, ref newY4);
            //    p1.X = (int)newX1;
            //    p1.Y = (int)newY1;
            //    p2.X = (int)newX2;
            //    p2.Y = (int)newY2;
            //    p3.X = (int)newX3;
            //    p3.Y = (int)newY3;
            //    p4.X = (int)newX4;
            //    p4.Y = (int)newY4;
            //    m_g.DrawLine(new Pen(Color.Lime), p1.X, p1.Y, p2.X, p2.Y);
            //    m_g.DrawLine(new Pen(Color.Blue), p3.X, p3.Y, p4.X, p4.Y);
            //}
        }
        private void btn_OK_Click(object sender, EventArgs e)
        {
            timer_Live.Stop();

           
            m_smVisionInfo.g_arrPad[m_intPadNo].EnableDisableFeret(m_intselectedpad-1, chk_Feret.Checked);
          

            for (int i = 0; i < m_Start.Count; i++)
            {

                m_StartPercentX[i] = ((m_Start[i].X - m_StartOri[i].X) * 100 / pic_Image.Width) + m_StartPercentX[i];
                m_StartPercentY[i] = ((m_Start[i].Y - m_StartOri[i].Y) * 100 / pic_Image.Height) + m_StartPercentY[i];
            }

            for (int j = 0; j < m_End.Count; j++)
            {

                m_EndPercentX[j] = ((m_End[j].X - m_EndOri[j].X) * 100 / pic_Image.Width) + m_EndPercentX[j];
                m_EndPercentY[j] = ((m_End[j].Y - m_EndOri[j].Y) * 100 / pic_Image.Height) + m_EndPercentY[j];
            }

            if (m_intPadNo > m_smVisionInfo.g_arrPad.Length - 1)
                return;

            string strSectionName = "";

            if (m_intPadNo == 0)
                strSectionName = "CenterROI";
            else if (m_intPadNo == 1)
                strSectionName = "TopROI";
            else if (m_intPadNo == 2)
                strSectionName = "RightROI";
            else if (m_intPadNo == 3)
                strSectionName = "BottomROI";
            else if (m_intPadNo == 4)
                strSectionName = "LeftROI";

            for (int k = 0; k < m_intLineCount; k++)
                m_smVisionInfo.g_arrPad[m_intPadNo].SetPercentage(m_intselectedpad, m_StartPercentX[k], m_StartPercentY[k], m_EndPercentX[k], m_EndPercentY[k], k, m_InwardStartPercent[k], m_InwardEndPercent[k], m_MeasureMethod[k], Convert.ToInt32(txt_PadAngle.Text));
            DisposeAll();
            // timer_Live.Enabled = false;
            m_smVisionInfo.g_arrPad[m_intPadNo].ClearPoints();
            Close();
            Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            timer_Live.Stop();
            m_smVisionInfo.g_arrPad[m_intPadNo].EnableDisableFeret(m_intselectedpad-1, m_blnFeretActivatedOri);
            m_smVisionInfo.g_arrPad[m_intPadNo].ClearPoints();
            //  timer_Live.Enabled = false;
            DisposeAll();

            Close();
            Dispose();
        }

        private void pic_Image_MouseDown(object sender, MouseEventArgs e)
        {
            int intPositionX = (int)Math.Round(e.X / 1.0, 0, MidpointRounding.AwayFromZero);
            int intPositionY = (int)Math.Round(e.Y / 1.0, 0, MidpointRounding.AwayFromZero);
            m_smVisionInfo.g_arrPad[m_intPadNo].SplitLineHitTest(intPositionX, intPositionY, m_g);
        }
        private void pic_Image_MouseMove(object sender, MouseEventArgs e)
        {
            // int Max = Math.Max((int)Math.Ceiling(m_w * m_smVisionInfo.g_fScaleX), (int)Math.Ceiling(m_h * m_smVisionInfo.g_fScaleY));
            int Max = Math.Max((int)(m_w), (int)(m_h));
            //if (Max > 250)
            //    Max = 250;
            int intPositionX = (int)Math.Round(e.X / 1.0, 0, MidpointRounding.AwayFromZero);
            int intPositionY = (int)Math.Round(e.Y / 1.0, 0, MidpointRounding.AwayFromZero);
            //m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedImage].DragSplitLine(intPositionX, intPositionY, m_intselectedpad, m_g, m_x, m_y, (int)(m_x * m_smVisionInfo.g_fScaleX) - 1, (int)(m_y * m_smVisionInfo.g_fScaleY) - 1, Math.Min((int)Math.Ceiling((250 / Max) * (m_w * m_smVisionInfo.g_fScaleX)), 250), Math.Min((int)Math.Ceiling((250 / Max) * (m_h * m_smVisionInfo.g_fScaleY)), 250));
            m_bUpdatePictureBox = true;
            m_smVisionInfo.g_arrPad[m_intPadNo].SplitLineWillHitTest(intPositionX, intPositionY, m_g, 
                panel1.Size.Width / 2 - pic_Image.Width / 2,
                panel1.Size.Height / 2 - pic_Image.Height / 2,
                Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250),
                Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250));
            m_smVisionInfo.g_arrPad[m_intPadNo].DragSplitLine(intPositionX, intPositionY, m_intselectedpad, m_g, m_x, m_y, panel1.Size.Width / 2 - pic_Image.Width / 2, panel1.Size.Height / 2 - pic_Image.Height / 2, Math.Min((int)Math.Ceiling((250f / Max) * (m_w)), 250), Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250));
            m_bUpdatePictureBox = true;
        }
        private void pic_Image_MouseUp(object sender, MouseEventArgs e)
        {
            int intPositionX = (int)Math.Round(e.X / 1.0, 0, MidpointRounding.AwayFromZero);
            int intPositionY = (int)Math.Round(e.Y / 1.0, 0, MidpointRounding.AwayFromZero);

            m_smVisionInfo.g_arrPad[m_intPadNo].ClearSplitLineDragHandler(m_g, ref m_Start, ref m_End, ref m_Hit);

        }
        private void pic_Image_Paint(object sender, PaintEventArgs e)
        {
            m_bUpdatePictureBox = true;
            //CopyImage();
            //m_g.Clear(Color.Transparent);
            // CopyImage();
            // UpdateImage();
            //m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedImage].DrawLines(m_g, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY, 0, 0, m_intselectedpad);
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            if (m_intLineCount == 12)
                return;

            if (m_intLineCount == 2)
            {
                gb_Line3.Visible = true;
                gb_Line3.Location = new Point(gb_Line2.Location.X, gb_Line2.Location.Y + gb_Line2.Size.Height);
                txt_InwardStartPercent3.Text = m_InwardStartPercent[m_intLineCount].ToString();
                trackBar_InwardStartPercent3.Value = Convert.ToInt32(txt_InwardStartPercent3.Text);
                txt_InwardEndPercent3.Text = m_InwardEndPercent[m_intLineCount].ToString();
                trackBar_InwardEndPercent3.Value = Convert.ToInt32(txt_InwardEndPercent3.Text);
                cbo_MeasureMethod3.SelectedIndex = m_MeasureMethod[m_intLineCount];
            }
            else if (m_intLineCount == 3)
            {
                gb_Line4.Visible = true;
                gb_Line4.Location = new Point(gb_Line3.Location.X, gb_Line3.Location.Y + gb_Line3.Size.Height);
                txt_InwardStartPercent4.Text = m_InwardStartPercent[m_intLineCount].ToString();
                trackBar_InwardStartPercent4.Value = Convert.ToInt32(txt_InwardStartPercent4.Text);
                txt_InwardEndPercent4.Text = m_InwardEndPercent[m_intLineCount].ToString();
                trackBar_InwardEndPercent4.Value = Convert.ToInt32(txt_InwardEndPercent4.Text);
                cbo_MeasureMethod4.SelectedIndex = m_MeasureMethod[m_intLineCount];
            }
            else if (m_intLineCount == 4)
            {
                gb_Line5.Visible = true;
                gb_Line5.Location = new Point(gb_Line4.Location.X, gb_Line4.Location.Y + gb_Line4.Size.Height);
                txt_InwardStartPercent5.Text = m_InwardStartPercent[m_intLineCount].ToString();
                trackBar_InwardStartPercent5.Value = Convert.ToInt32(txt_InwardStartPercent5.Text);
                txt_InwardEndPercent5.Text = m_InwardEndPercent[m_intLineCount].ToString();
                trackBar_InwardEndPercent5.Value = Convert.ToInt32(txt_InwardEndPercent5.Text);
                cbo_MeasureMethod5.SelectedIndex = m_MeasureMethod[m_intLineCount];
            }
            else if (m_intLineCount == 5)
            {
                gb_Line6.Visible = true;
                gb_Line6.Location = new Point(gb_Line5.Location.X, gb_Line5.Location.Y + gb_Line5.Size.Height);
                txt_InwardStartPercent6.Text = m_InwardStartPercent[m_intLineCount].ToString();
                trackBar_InwardStartPercent6.Value = Convert.ToInt32(txt_InwardStartPercent6.Text);
                txt_InwardEndPercent6.Text = m_InwardEndPercent[m_intLineCount].ToString();
                trackBar_InwardEndPercent6.Value = Convert.ToInt32(txt_InwardEndPercent6.Text);
                cbo_MeasureMethod6.SelectedIndex = m_MeasureMethod[m_intLineCount];
            }
            else if (m_intLineCount == 6)
            {
                gb_Line7.Visible = true;
                gb_Line7.Location = new Point(gb_Line6.Location.X, gb_Line6.Location.Y + gb_Line6.Size.Height);
                txt_InwardStartPercent7.Text = m_InwardStartPercent[m_intLineCount].ToString();
                trackBar_InwardStartPercent7.Value = Convert.ToInt32(txt_InwardStartPercent7.Text);
                txt_InwardEndPercent7.Text = m_InwardEndPercent[m_intLineCount].ToString();
                trackBar_InwardEndPercent7.Value = Convert.ToInt32(txt_InwardEndPercent7.Text);
                cbo_MeasureMethod7.SelectedIndex = m_MeasureMethod[m_intLineCount];
            }
            else if (m_intLineCount == 7)
            {
                gb_Line8.Visible = true;
                gb_Line8.Location = new Point(gb_Line7.Location.X, gb_Line7.Location.Y + gb_Line7.Size.Height);
                txt_InwardStartPercent8.Text = m_InwardStartPercent[m_intLineCount].ToString();
                trackBar_InwardStartPercent8.Value = Convert.ToInt32(txt_InwardStartPercent8.Text);
                txt_InwardEndPercent8.Text = m_InwardEndPercent[m_intLineCount].ToString();
                trackBar_InwardEndPercent8.Value = Convert.ToInt32(txt_InwardEndPercent8.Text);
                cbo_MeasureMethod8.SelectedIndex = m_MeasureMethod[m_intLineCount];
            }
            else if (m_intLineCount == 8)
            {
                gb_Line9.Visible = true;
                gb_Line9.Location = new Point(gb_Line8.Location.X, gb_Line8.Location.Y + gb_Line8.Size.Height);
                txt_InwardStartPercent9.Text = m_InwardStartPercent[m_intLineCount].ToString();
                trackBar_InwardStartPercent9.Value = Convert.ToInt32(txt_InwardStartPercent9.Text);
                txt_InwardEndPercent9.Text = m_InwardEndPercent[m_intLineCount].ToString();
                trackBar_InwardEndPercent9.Value = Convert.ToInt32(txt_InwardEndPercent9.Text);
                cbo_MeasureMethod9.SelectedIndex = m_MeasureMethod[m_intLineCount];
            }
            else if (m_intLineCount == 9)
            {
                gb_Line10.Visible = true;
                gb_Line10.Location = new Point(gb_Line9.Location.X, gb_Line9.Location.Y + gb_Line9.Size.Height);
                txt_InwardStartPercent10.Text = m_InwardStartPercent[m_intLineCount].ToString();
                trackBar_InwardStartPercent10.Value = Convert.ToInt32(txt_InwardStartPercent10.Text);
                txt_InwardEndPercent10.Text = m_InwardEndPercent[m_intLineCount].ToString();
                trackBar_InwardEndPercent10.Value = Convert.ToInt32(txt_InwardEndPercent10.Text);
                cbo_MeasureMethod10.SelectedIndex = m_MeasureMethod[m_intLineCount];
            }
            else if (m_intLineCount == 10)
            {
                gb_Line11.Visible = true;
                gb_Line11.Location = new Point(gb_Line10.Location.X, gb_Line10.Location.Y + gb_Line10.Size.Height);
                txt_InwardStartPercent11.Text = m_InwardStartPercent[m_intLineCount].ToString();
                trackBar_InwardStartPercent11.Value = Convert.ToInt32(txt_InwardStartPercent11.Text);
                txt_InwardEndPercent11.Text = m_InwardEndPercent[m_intLineCount].ToString();
                trackBar_InwardEndPercent11.Value = Convert.ToInt32(txt_InwardEndPercent11.Text);
                cbo_MeasureMethod11.SelectedIndex = m_MeasureMethod[m_intLineCount];
            }
            else if (m_intLineCount == 11)
            {
                gb_Line12.Visible = true;
                gb_Line12.Location = new Point(gb_Line11.Location.X, gb_Line11.Location.Y + gb_Line11.Size.Height);
                txt_InwardStartPercent12.Text = m_InwardStartPercent[m_intLineCount].ToString();
                trackBar_InwardStartPercent12.Value = Convert.ToInt32(txt_InwardStartPercent12.Text);
                txt_InwardEndPercent12.Text = m_InwardEndPercent[m_intLineCount].ToString();
                trackBar_InwardEndPercent12.Value = Convert.ToInt32(txt_InwardEndPercent12.Text);
                cbo_MeasureMethod12.SelectedIndex = m_MeasureMethod[m_intLineCount];
            }

            int Max = Math.Max((int)(m_w), (int)(m_h));


            m_smVisionInfo.g_arrPad[m_intPadNo].SetROI((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / m_intLineCount) + ((pic_Image.Width * (m_StartPercentX[m_intLineCount] / 100))), 0, MidpointRounding.AwayFromZero),
                (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[m_intLineCount] / 100))), 0, MidpointRounding.AwayFromZero),
                (int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / m_intLineCount) + ((pic_Image.Width * (m_EndPercentX[m_intLineCount] / 100))), 0, MidpointRounding.AwayFromZero),
                (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[m_intLineCount] / 100)), 0, MidpointRounding.AwayFromZero),
                m_intLineCount,
                m_InwardStartPercent[m_intLineCount],
                m_InwardEndPercent[m_intLineCount]);

            if (m_intLineCount > m_Start.Count - 1)
            {
                m_Start.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / m_intLineCount) + ((pic_Image.Width * (m_StartPercentX[m_intLineCount] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[m_intLineCount] / 100))), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_Start[m_intLineCount] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / m_intLineCount) + ((pic_Image.Width * (m_StartPercentX[m_intLineCount] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[m_intLineCount] / 100))), 0, MidpointRounding.AwayFromZero));
            }

            if (m_intLineCount > m_End.Count - 1)
            {
                m_End.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / m_intLineCount) + ((pic_Image.Width * (m_EndPercentX[m_intLineCount] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[m_intLineCount] / 100)), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_End[m_intLineCount] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / m_intLineCount) + ((pic_Image.Width * (m_EndPercentX[m_intLineCount] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[m_intLineCount] / 100)), 0, MidpointRounding.AwayFromZero));
            }


            if (m_intLineCount > m_StartOri.Count - 1)
            {
                m_StartOri.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / m_intLineCount) + ((pic_Image.Width * (m_StartPercentX[m_intLineCount] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[m_intLineCount] / 100))), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_StartOri[m_intLineCount] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / m_intLineCount) + ((pic_Image.Width * (m_StartPercentX[m_intLineCount] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((0 + (pic_Image.Height * (m_StartPercentY[m_intLineCount] / 100))), 0, MidpointRounding.AwayFromZero));
            }

            if (m_intLineCount > m_EndOri.Count - 1)
            {
                m_EndOri.Add(new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / m_intLineCount) + ((pic_Image.Width * (m_EndPercentX[m_intLineCount] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[m_intLineCount] / 100)), 0, MidpointRounding.AwayFromZero)));

            }
            else
            {
                m_EndOri[m_intLineCount] = new PointF((int)Math.Round(((pic_Image.Width / 2) + (pic_Image.Width / 2) / m_intLineCount) + ((pic_Image.Width * (m_EndPercentX[m_intLineCount] / 100))), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) + Math.Min((int)Math.Ceiling((250f / Max) * (m_h)), 250) * (m_EndPercentY[m_intLineCount] / 100)), 0, MidpointRounding.AwayFromZero));
            }
            // m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedImage].SetROI(0, (int)(pic_Image.Height / 2), Math.Min((int)Math.Ceiling((250 / Max) * (m_w * m_smVisionInfo.g_fScaleX)), 250), (int)(pic_Image.Height / 2));
            UpdateImage(m_intLineCount);
            m_intLineCount += 1;
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            if (m_intLineCount == 2)
                return;
            
            if (m_intLineCount == 3)
            {
                gb_Line3.Visible = false;
            }
            else if (m_intLineCount == 4)
            {
                gb_Line4.Visible = false;
            }
            else if (m_intLineCount == 5)
            {
                gb_Line5.Visible = false;
            }
            else if (m_intLineCount == 6)
            {
                gb_Line6.Visible = false;
            }
            else if (m_intLineCount == 7)
            {
                gb_Line7.Visible = false;
            }
            else if (m_intLineCount == 8)
            {
                gb_Line8.Visible = false;
            }
            else if (m_intLineCount == 9)
            {
                gb_Line9.Visible = false;
            }
            else if (m_intLineCount == 10)
            {
                gb_Line10.Visible = false;
            }
            else if (m_intLineCount == 11)
            {
                gb_Line11.Visible = false;
            }
            else if (m_intLineCount == 12)
            {
                gb_Line12.Visible = false;
            }

            m_Start.RemoveAt(m_Start.Count - 1);
            m_End.RemoveAt(m_End.Count - 1);
            m_StartOri.RemoveAt(m_StartOri.Count - 1);
            m_EndOri.RemoveAt(m_EndOri.Count - 1);
            m_smVisionInfo.g_arrPad[m_intPadNo].RemoveLine(m_intLineCount);
            m_intLineCount -= 1;
            m_bUpdatePictureBox = true;
        }
        private void DisposeAll()
        {
            m_objRotatedPadImage.Dispose();
            m_objPadImage.Dispose();
            objROI.Dispose();
            m_g.Dispose();
            pic_Image.Dispose();
        }

        private void txt_InwardStartPercent_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intTag = Convert.ToInt32(((TextBox)sender).Tag);

            switch (intTag)
            {
                case 0:
                    if ((Convert.ToInt32(txt_InwardStartPercent1.Text) + Convert.ToInt32(txt_InwardEndPercent1.Text)) <= 100)
                        m_InwardStartPercent[intTag] = Convert.ToInt32(txt_InwardStartPercent1.Text);
                    else
                    {
                        SRMMessageBox.Show("Start Percent cannot exceed End Percent");
                        m_InwardStartPercent[intTag] = 100 - Convert.ToInt32(txt_InwardEndPercent1.Text);
                        txt_InwardStartPercent1.Text = m_InwardStartPercent[intTag].ToString();
                        trackBar_InwardStartPercent1.Value = Convert.ToInt32(m_InwardStartPercent[intTag]);
                    }
                    break;
                case 1:
                    if ((Convert.ToInt32(txt_InwardStartPercent2.Text) + Convert.ToInt32(txt_InwardEndPercent2.Text)) <= 100)
                        m_InwardStartPercent[intTag] = Convert.ToInt32(txt_InwardStartPercent2.Text);
                    else
                    {
                        SRMMessageBox.Show("Start Percent cannot exceed End Percent");
                        m_InwardStartPercent[intTag] = 100 - Convert.ToInt32(txt_InwardEndPercent2.Text);
                        txt_InwardStartPercent2.Text = m_InwardStartPercent[intTag].ToString();
                        trackBar_InwardStartPercent2.Value = Convert.ToInt32(m_InwardStartPercent[intTag]);
                    }
                    break;
                case 2:
                    if ((Convert.ToInt32(txt_InwardStartPercent3.Text) + Convert.ToInt32(txt_InwardEndPercent3.Text)) <= 100)
                        m_InwardStartPercent[intTag] = Convert.ToInt32(txt_InwardStartPercent3.Text);
                    else
                    {
                        SRMMessageBox.Show("Start Percent cannot exceed End Percent");
                        m_InwardStartPercent[intTag] = 100 - Convert.ToInt32(txt_InwardEndPercent3.Text);
                        txt_InwardStartPercent3.Text = m_InwardStartPercent[intTag].ToString();
                        trackBar_InwardStartPercent3.Value = Convert.ToInt32(m_InwardStartPercent[intTag]);
                    }
                    break;
                case 3:
                    if ((Convert.ToInt32(txt_InwardStartPercent4.Text) + Convert.ToInt32(txt_InwardEndPercent4.Text)) <= 100)
                        m_InwardStartPercent[intTag] = Convert.ToInt32(txt_InwardStartPercent4.Text);
                    else
                    {
                        SRMMessageBox.Show("Start Percent cannot exceed End Percent");
                        m_InwardStartPercent[intTag] = 100 - Convert.ToInt32(txt_InwardEndPercent4.Text);
                        txt_InwardStartPercent4.Text = m_InwardStartPercent[intTag].ToString();
                        trackBar_InwardStartPercent4.Value = Convert.ToInt32(m_InwardStartPercent[intTag]);
                    }
                    break;
                case 4:
                    if ((Convert.ToInt32(txt_InwardStartPercent5.Text) + Convert.ToInt32(txt_InwardEndPercent5.Text)) <= 100)
                        m_InwardStartPercent[intTag] = Convert.ToInt32(txt_InwardStartPercent5.Text);
                    else
                    {
                        SRMMessageBox.Show("Start Percent cannot exceed End Percent");
                        m_InwardStartPercent[intTag] = 100 - Convert.ToInt32(txt_InwardEndPercent5.Text);
                        txt_InwardStartPercent5.Text = m_InwardStartPercent[intTag].ToString();
                        trackBar_InwardStartPercent5.Value = Convert.ToInt32(m_InwardStartPercent[intTag]);
                    }
                    break;
                case 5:
                    if ((Convert.ToInt32(txt_InwardStartPercent6.Text) + Convert.ToInt32(txt_InwardEndPercent6.Text)) <= 100)
                        m_InwardStartPercent[intTag] = Convert.ToInt32(txt_InwardStartPercent6.Text);
                    else
                    {
                        SRMMessageBox.Show("Start Percent cannot exceed End Percent");
                        m_InwardStartPercent[intTag] = 100 - Convert.ToInt32(txt_InwardEndPercent6.Text);
                        txt_InwardStartPercent6.Text = m_InwardStartPercent[intTag].ToString();
                        trackBar_InwardStartPercent6.Value = Convert.ToInt32(m_InwardStartPercent[intTag]);
                    }
                    break;
                case 6:
                    if ((Convert.ToInt32(txt_InwardStartPercent7.Text) + Convert.ToInt32(txt_InwardEndPercent7.Text)) <= 100)
                        m_InwardStartPercent[intTag] = Convert.ToInt32(txt_InwardStartPercent7.Text);
                    else
                    {
                        SRMMessageBox.Show("Start Percent cannot exceed End Percent");
                        m_InwardStartPercent[intTag] = 100 - Convert.ToInt32(txt_InwardEndPercent7.Text);
                        txt_InwardStartPercent7.Text = m_InwardStartPercent[intTag].ToString();
                        trackBar_InwardStartPercent7.Value = Convert.ToInt32(m_InwardStartPercent[intTag]);
                    }
                    break;
                case 7:
                    if ((Convert.ToInt32(txt_InwardStartPercent8.Text) + Convert.ToInt32(txt_InwardEndPercent8.Text)) <= 100)
                        m_InwardStartPercent[intTag] = Convert.ToInt32(txt_InwardStartPercent8.Text);
                    else
                    {
                        SRMMessageBox.Show("Start Percent cannot exceed End Percent");
                        m_InwardStartPercent[intTag] = 100 - Convert.ToInt32(txt_InwardEndPercent8.Text);
                        txt_InwardStartPercent8.Text = m_InwardStartPercent[intTag].ToString();
                        trackBar_InwardStartPercent8.Value = Convert.ToInt32(m_InwardStartPercent[intTag]);
                    }
                    break;
                case 8:
                    if ((Convert.ToInt32(txt_InwardStartPercent9.Text) + Convert.ToInt32(txt_InwardEndPercent9.Text)) <= 100)
                        m_InwardStartPercent[intTag] = Convert.ToInt32(txt_InwardStartPercent9.Text);
                    else
                    {
                        SRMMessageBox.Show("Start Percent cannot exceed End Percent");
                        m_InwardStartPercent[intTag] = 100 - Convert.ToInt32(txt_InwardEndPercent9.Text);
                        txt_InwardStartPercent9.Text = m_InwardStartPercent[intTag].ToString();
                        trackBar_InwardStartPercent9.Value = Convert.ToInt32(m_InwardStartPercent[intTag]);
                    }
                    break;
                case 9:
                    if ((Convert.ToInt32(txt_InwardStartPercent10.Text) + Convert.ToInt32(txt_InwardEndPercent10.Text)) <= 100)
                        m_InwardStartPercent[intTag] = Convert.ToInt32(txt_InwardStartPercent10.Text);
                    else
                    {
                        SRMMessageBox.Show("Start Percent cannot exceed End Percent");
                        m_InwardStartPercent[intTag] = 100 - Convert.ToInt32(txt_InwardEndPercent10.Text);
                        txt_InwardStartPercent10.Text = m_InwardStartPercent[intTag].ToString();
                        trackBar_InwardStartPercent10.Value = Convert.ToInt32(m_InwardStartPercent[intTag]);
                    }
                    break;
                case 10:
                    if ((Convert.ToInt32(txt_InwardStartPercent11.Text) + Convert.ToInt32(txt_InwardEndPercent11.Text)) <= 100)
                        m_InwardStartPercent[intTag] = Convert.ToInt32(txt_InwardStartPercent11.Text);
                    else
                    {
                        SRMMessageBox.Show("Start Percent cannot exceed End Percent");
                        m_InwardStartPercent[intTag] = 100 - Convert.ToInt32(txt_InwardEndPercent11.Text);
                        txt_InwardStartPercent11.Text = m_InwardStartPercent[intTag].ToString();
                        trackBar_InwardStartPercent11.Value = Convert.ToInt32(m_InwardStartPercent[intTag]);
                    }
                    break;
                case 11:
                    if ((Convert.ToInt32(txt_InwardStartPercent12.Text) + Convert.ToInt32(txt_InwardEndPercent12.Text)) <= 100)
                        m_InwardStartPercent[intTag] = Convert.ToInt32(txt_InwardStartPercent12.Text);
                    else
                    {
                        SRMMessageBox.Show("Start Percent cannot exceed End Percent");
                        m_InwardStartPercent[intTag] = 100 - Convert.ToInt32(txt_InwardEndPercent12.Text);
                        txt_InwardStartPercent12.Text = m_InwardStartPercent[intTag].ToString();
                        trackBar_InwardStartPercent12.Value = Convert.ToInt32(m_InwardStartPercent[intTag]);
                    }
                    break;
            }

            m_smVisionInfo.g_arrPad[m_intPadNo].SetInwardPercent(
                intTag,
                m_InwardStartPercent[intTag],
                m_InwardEndPercent[intTag]);

            m_bUpdatePictureBox = true;
        }

        private void txt_InwardEndPercent_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intTag = Convert.ToInt32(((TextBox)sender).Tag);

            switch (intTag)
            {
                case 0:
                    if ((Convert.ToInt32(txt_InwardStartPercent1.Text) + Convert.ToInt32(txt_InwardEndPercent1.Text)) <= 100)
                        m_InwardEndPercent[intTag] = Convert.ToInt32(txt_InwardEndPercent1.Text);
                    else
                    {
                        SRMMessageBox.Show("End Percent cannot exceed Start Percent");
                        m_InwardEndPercent[intTag] = 100 - Convert.ToInt32(txt_InwardStartPercent1.Text);
                        txt_InwardEndPercent1.Text = m_InwardEndPercent[intTag].ToString();
                        trackBar_InwardEndPercent1.Value = Convert.ToInt32(m_InwardEndPercent[intTag]);
                    }
                    break;
                case 1:
                    if ((Convert.ToInt32(txt_InwardStartPercent2.Text) + Convert.ToInt32(txt_InwardEndPercent2.Text)) <= 100)
                        m_InwardEndPercent[intTag] = Convert.ToInt32(txt_InwardEndPercent2.Text);
                    else
                    {
                        SRMMessageBox.Show("End Percent cannot exceed Start Percent");
                        m_InwardEndPercent[intTag] = 100 - Convert.ToInt32(txt_InwardStartPercent2.Text);
                        txt_InwardEndPercent2.Text = m_InwardEndPercent[intTag].ToString();
                        trackBar_InwardEndPercent2.Value = Convert.ToInt32(m_InwardEndPercent[intTag]);
                    }
                    break;
                case 2:
                    if ((Convert.ToInt32(txt_InwardStartPercent3.Text) + Convert.ToInt32(txt_InwardEndPercent3.Text)) <= 100)
                        m_InwardEndPercent[intTag] = Convert.ToInt32(txt_InwardEndPercent3.Text);
                    else
                    {
                        SRMMessageBox.Show("End Percent cannot exceed Start Percent");
                        m_InwardEndPercent[intTag] = 100 - Convert.ToInt32(txt_InwardStartPercent3.Text);
                        txt_InwardEndPercent3.Text = m_InwardEndPercent[intTag].ToString();
                        trackBar_InwardEndPercent3.Value = Convert.ToInt32(m_InwardEndPercent[intTag]);
                    }
                    break;
                case 3:
                    if ((Convert.ToInt32(txt_InwardStartPercent4.Text) + Convert.ToInt32(txt_InwardEndPercent4.Text)) <= 100)
                        m_InwardEndPercent[intTag] = Convert.ToInt32(txt_InwardEndPercent4.Text);
                    else
                    {
                        SRMMessageBox.Show("End Percent cannot exceed Start Percent");
                        m_InwardEndPercent[intTag] = 100 - Convert.ToInt32(txt_InwardStartPercent4.Text);
                        txt_InwardEndPercent4.Text = m_InwardEndPercent[intTag].ToString();
                        trackBar_InwardEndPercent4.Value = Convert.ToInt32(m_InwardEndPercent[intTag]);
                    }
                    break;
                case 4:
                    if ((Convert.ToInt32(txt_InwardStartPercent5.Text) + Convert.ToInt32(txt_InwardEndPercent5.Text)) <= 100)
                        m_InwardEndPercent[intTag] = Convert.ToInt32(txt_InwardEndPercent5.Text);
                    else
                    {
                        SRMMessageBox.Show("End Percent cannot exceed Start Percent");
                        m_InwardEndPercent[intTag] = 100 - Convert.ToInt32(txt_InwardStartPercent5.Text);
                        txt_InwardEndPercent5.Text = m_InwardEndPercent[intTag].ToString();
                        trackBar_InwardEndPercent5.Value = Convert.ToInt32(m_InwardEndPercent[intTag]);
                    }
                    break;
                case 5:
                    if ((Convert.ToInt32(txt_InwardStartPercent6.Text) + Convert.ToInt32(txt_InwardEndPercent6.Text)) <= 100)
                        m_InwardEndPercent[intTag] = Convert.ToInt32(txt_InwardEndPercent6.Text);
                    else
                    {
                        SRMMessageBox.Show("End Percent cannot exceed Start Percent");
                        m_InwardEndPercent[intTag] = 100 - Convert.ToInt32(txt_InwardStartPercent6.Text);
                        txt_InwardEndPercent6.Text = m_InwardEndPercent[intTag].ToString();
                        trackBar_InwardEndPercent6.Value = Convert.ToInt32(m_InwardEndPercent[intTag]);
                    }
                    break;
                case 6:
                    if ((Convert.ToInt32(txt_InwardStartPercent7.Text) + Convert.ToInt32(txt_InwardEndPercent7.Text)) <= 100)
                        m_InwardEndPercent[intTag] = Convert.ToInt32(txt_InwardEndPercent7.Text);
                    else
                    {
                        SRMMessageBox.Show("End Percent cannot exceed Start Percent");
                        m_InwardEndPercent[intTag] = 100 - Convert.ToInt32(txt_InwardStartPercent7.Text);
                        txt_InwardEndPercent7.Text = m_InwardEndPercent[intTag].ToString();
                        trackBar_InwardEndPercent7.Value = Convert.ToInt32(m_InwardEndPercent[intTag]);
                    }
                    break;
                case 7:
                    if ((Convert.ToInt32(txt_InwardStartPercent8.Text) + Convert.ToInt32(txt_InwardEndPercent8.Text)) <= 100)
                        m_InwardEndPercent[intTag] = Convert.ToInt32(txt_InwardEndPercent8.Text);
                    else
                    {
                        SRMMessageBox.Show("End Percent cannot exceed Start Percent");
                        m_InwardEndPercent[intTag] = 100 - Convert.ToInt32(txt_InwardStartPercent8.Text);
                        txt_InwardEndPercent8.Text = m_InwardEndPercent[intTag].ToString();
                        trackBar_InwardEndPercent8.Value = Convert.ToInt32(m_InwardEndPercent[intTag]);
                    }
                    break;
                case 8:
                    if ((Convert.ToInt32(txt_InwardStartPercent9.Text) + Convert.ToInt32(txt_InwardEndPercent9.Text)) <= 100)
                        m_InwardEndPercent[intTag] = Convert.ToInt32(txt_InwardEndPercent9.Text);
                    else
                    {
                        SRMMessageBox.Show("End Percent cannot exceed Start Percent");
                        m_InwardEndPercent[intTag] = 100 - Convert.ToInt32(txt_InwardStartPercent9.Text);
                        txt_InwardEndPercent9.Text = m_InwardEndPercent[intTag].ToString();
                        trackBar_InwardEndPercent9.Value = Convert.ToInt32(m_InwardEndPercent[intTag]);
                    }
                    break;
                case 9:
                    if ((Convert.ToInt32(txt_InwardStartPercent10.Text) + Convert.ToInt32(txt_InwardEndPercent10.Text)) <= 100)
                        m_InwardEndPercent[intTag] = Convert.ToInt32(txt_InwardEndPercent10.Text);
                    else
                    {
                        SRMMessageBox.Show("End Percent cannot exceed Start Percent");
                        m_InwardEndPercent[intTag] = 100 - Convert.ToInt32(txt_InwardStartPercent10.Text);
                        txt_InwardEndPercent10.Text = m_InwardEndPercent[intTag].ToString();
                        trackBar_InwardEndPercent10.Value = Convert.ToInt32(m_InwardEndPercent[intTag]);
                    }
                    break;
                case 10:
                    if ((Convert.ToInt32(txt_InwardStartPercent11.Text) + Convert.ToInt32(txt_InwardEndPercent11.Text)) <= 100)
                        m_InwardEndPercent[intTag] = Convert.ToInt32(txt_InwardEndPercent11.Text);
                    else
                    {
                        SRMMessageBox.Show("End Percent cannot exceed Start Percent");
                        m_InwardEndPercent[intTag] = 100 - Convert.ToInt32(txt_InwardStartPercent11.Text);
                        txt_InwardEndPercent11.Text = m_InwardEndPercent[intTag].ToString();
                        trackBar_InwardEndPercent11.Value = Convert.ToInt32(m_InwardEndPercent[intTag]);
                    }
                    break;
                case 11:
                    if ((Convert.ToInt32(txt_InwardStartPercent12.Text) + Convert.ToInt32(txt_InwardEndPercent12.Text)) <= 100)
                        m_InwardEndPercent[intTag] = Convert.ToInt32(txt_InwardEndPercent12.Text);
                    else
                    {
                        SRMMessageBox.Show("End Percent cannot exceed Start Percent");
                        m_InwardEndPercent[intTag] = 100 - Convert.ToInt32(txt_InwardStartPercent12.Text);
                        txt_InwardEndPercent12.Text = m_InwardEndPercent[intTag].ToString();
                        trackBar_InwardEndPercent12.Value = Convert.ToInt32(m_InwardEndPercent[intTag]);
                    }
                    break;
            }

            m_smVisionInfo.g_arrPad[m_intPadNo].SetInwardPercent(
                intTag,
                m_InwardStartPercent[intTag],
                m_InwardEndPercent[intTag]);

            m_bUpdatePictureBox = true;
        }

        private void trackBar_InwardStartPercent_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intTag = Convert.ToInt32(((TrackBar)sender).Tag);

            switch (intTag)
            {
                case 0:
                    txt_InwardStartPercent1.Text = trackBar_InwardStartPercent1.Value.ToString();
                    break;
                case 1:
                    txt_InwardStartPercent2.Text = trackBar_InwardStartPercent2.Value.ToString();
                    break;
                case 2:
                    txt_InwardStartPercent3.Text = trackBar_InwardStartPercent3.Value.ToString();
                    break;
                case 3:
                    txt_InwardStartPercent4.Text = trackBar_InwardStartPercent4.Value.ToString();
                    break;
                case 4:
                    txt_InwardStartPercent5.Text = trackBar_InwardStartPercent5.Value.ToString();
                    break;
                case 5:
                    txt_InwardStartPercent6.Text = trackBar_InwardStartPercent6.Value.ToString();
                    break;
                case 6:
                    txt_InwardStartPercent7.Text = trackBar_InwardStartPercent7.Value.ToString();
                    break;
                case 7:
                    txt_InwardStartPercent8.Text = trackBar_InwardStartPercent8.Value.ToString();
                    break;
                case 8:
                    txt_InwardStartPercent9.Text = trackBar_InwardStartPercent9.Value.ToString();
                    break;
                case 9:
                    txt_InwardStartPercent10.Text = trackBar_InwardStartPercent10.Value.ToString();
                    break;
                case 10:
                    txt_InwardStartPercent11.Text = trackBar_InwardStartPercent11.Value.ToString();
                    break;
                case 11:
                    txt_InwardStartPercent12.Text = trackBar_InwardStartPercent12.Value.ToString();
                    break;
            }

        }

        private void trackBar_InwardEndPercent_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intTag = Convert.ToInt32(((TrackBar)sender).Tag);

            switch (intTag)
            {
                case 0:
                    txt_InwardEndPercent1.Text = trackBar_InwardEndPercent1.Value.ToString();
                    break;
                case 1:
                    txt_InwardEndPercent2.Text = trackBar_InwardEndPercent2.Value.ToString();
                    break;
                case 2:
                    txt_InwardEndPercent3.Text = trackBar_InwardEndPercent3.Value.ToString();
                    break;
                case 3:
                    txt_InwardEndPercent4.Text = trackBar_InwardEndPercent4.Value.ToString();
                    break;
                case 4:
                    txt_InwardEndPercent5.Text = trackBar_InwardEndPercent5.Value.ToString();
                    break;
                case 5:
                    txt_InwardEndPercent6.Text = trackBar_InwardEndPercent6.Value.ToString();
                    break;
                case 6:
                    txt_InwardEndPercent7.Text = trackBar_InwardEndPercent7.Value.ToString();
                    break;
                case 7:
                    txt_InwardEndPercent8.Text = trackBar_InwardEndPercent8.Value.ToString();
                    break;
                case 8:
                    txt_InwardEndPercent9.Text = trackBar_InwardEndPercent9.Value.ToString();
                    break;
                case 9:
                    txt_InwardEndPercent10.Text = trackBar_InwardEndPercent10.Value.ToString();
                    break;
                case 10:
                    txt_InwardEndPercent11.Text = trackBar_InwardEndPercent11.Value.ToString();
                    break;
                case 11:
                    txt_InwardEndPercent12.Text = trackBar_InwardEndPercent12.Value.ToString();
                    break;
            }
        }

    }
}