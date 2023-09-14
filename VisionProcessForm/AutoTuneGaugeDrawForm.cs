using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisionProcessing;
using Common;
using SharedMemory;
using System.Threading;
namespace VisionProcessForm
{
    public partial class AutoTuneGaugeDrawForm : Form
    {
        #region Member Variables
        private bool m_blnInitDone = false;
        private int m_intSelectedImage_Prev = 0;
        private float m_fZoomScalePrev = 1f;
        private float m_fScaleXPrev = 1f;
        private float m_fScaleYPrev = 1f;
        private float m_fZoomCount = 1f;
        private float m_fZoomCountPrev = 1f;
        private float m_fOriScaleX = 1f;
        private float m_fOriScaleY = 1f;
        private int m_intScollValueXPrev = 0;
        private int m_intScollValueYPrev = 0;
        private int m_intZoomImageFocusPointX = 0;
        private int m_intZoomImageFocusPointY = 0;
        private FreeShapeROI m_objROI; 
        private bool m_blnDrawROI = false; 
        List<PointF> m_arrPoints = new List<PointF>();
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        private Graphics m_Graphic;
        private int m_intMouseHitX = 0;
        private int m_intMouseHitY = 0;
        private int m_intMousePointX = 0;
        private int m_intMousePointY = 0;
        private string m_strPath = "";
        private bool m_UpdatePictureBox;
        #endregion
        
        #region Properties
        public bool ref_blnAutoPlace{ get { return chk_AutoPlace.Checked; } }
        public bool ref_blnSetToAll { get { return chk_SetToAll.Checked; } }
        #endregion

        public AutoTuneGaugeDrawForm(VisionInfo smVisionInfo, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo, string strPath)
        {
            InitializeComponent();
            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_Graphic = Graphics.FromHwnd(pic_Image.Handle);
            m_fScaleXPrev = m_smVisionInfo.g_fScaleX;
            m_fScaleYPrev = m_smVisionInfo.g_fScaleY;
            m_fZoomScalePrev = m_smVisionInfo.g_fZoomScale;
            m_fOriScaleX = m_fScaleXPrev / m_fZoomScalePrev;
            m_fOriScaleY = m_fScaleYPrev / m_fZoomScalePrev;
            m_smVisionInfo.g_fZoomScale = 1f;
            m_fZoomCountPrev = 0;
            m_intSelectedImage_Prev = m_smVisionInfo.g_intSelectedImage;
            m_strPath = strPath;
            m_objROI = new FreeShapeROI(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight); //pic_Image.Size.Width, pic_Image.Size.Height

            // Get Total Image View Count
            int intViewImageCount = ImageDrawing.GetImageViewCount(m_smVisionInfo.g_intVisionIndex);

            cbo_ImageNo.Items.Clear();

            for (int i = 0; i < intViewImageCount; i++)
            {
                cbo_ImageNo.Items.Add("Image " + (i + 1).ToString());

                if (m_smVisionInfo.g_intSelectedImage == ImageDrawing.GetArrayImageIndex(i, m_smVisionInfo.g_intVisionIndex))
                {
                    cbo_ImageNo.SelectedIndex = i;
                }
            }
            m_blnInitDone = true;
            m_UpdatePictureBox = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_UpdatePictureBox)
            {
                m_UpdatePictureBox = false;
                DrawZoomInOutImage();
                DrawMainImage();
                //if (pnl_PictureBox.HorizontalScroll.Value == 0)
                    m_intZoomImageFocusPointX = (int)Math.Round(pnl_PictureBox.Size.Width / 2 * m_smVisionInfo.g_fScaleX, 0, MidpointRounding.AwayFromZero);
                //else
                //    m_intZoomImageFocusPointX = pnl_PictureBox.HorizontalScroll.Maximum / 2;

                //if (pnl_PictureBox.VerticalScroll.Value == 0)
                    m_intZoomImageFocusPointY = (int)Math.Round(pnl_PictureBox.Size.Height / 2 * m_smVisionInfo.g_fScaleY, 0, MidpointRounding.AwayFromZero);
                //else
                //    m_intZoomImageFocusPointY = pnl_PictureBox.VerticalScroll.Maximum / 2;
            }
        }

        private void DrawMainImage()
        {
            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic);

            if (m_blnDrawROI)
            {
                if (m_objROI != null)
                {
                    m_objROI.DrawROI(m_Graphic, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY, true);//m_objROI.GetROIHandle()
                    //label1.Text = m_objROI.ref_fCurrentAngle.ToString();
                    //label2.Text = m_objROI.ref_fAngleDeg.ToString();
                    //label3.Text = m_objROI.ref_fAngleDegPrev.ToString();
                }
            }
            else
            {

                for (int i = 0; i < m_arrPoints.Count; i++)
                {
                    m_Graphic.DrawLine(new Pen(Color.Aqua), (m_arrPoints[i].X - 3) * m_smVisionInfo.g_fScaleX, (m_arrPoints[i].Y - 3) * m_smVisionInfo.g_fScaleY, (m_arrPoints[i].X + 3) * m_smVisionInfo.g_fScaleX, (m_arrPoints[i].Y + 3) * m_smVisionInfo.g_fScaleY);
                    m_Graphic.DrawLine(new Pen(Color.Aqua), (m_arrPoints[i].X + 3) * m_smVisionInfo.g_fScaleX, (m_arrPoints[i].Y - 3) * m_smVisionInfo.g_fScaleY, (m_arrPoints[i].X - 3) * m_smVisionInfo.g_fScaleX, (m_arrPoints[i].Y + 3) * m_smVisionInfo.g_fScaleY);

                    if (m_arrPoints.Count == 2)
                    {
                        m_Graphic.DrawLine(new Pen(Color.Aqua), (m_arrPoints[0].X) * m_smVisionInfo.g_fScaleX, (m_arrPoints[0].Y) * m_smVisionInfo.g_fScaleY, (m_arrPoints[1].X) * m_smVisionInfo.g_fScaleX, (m_arrPoints[1].Y) * m_smVisionInfo.g_fScaleY);
                    }
                    if (m_arrPoints.Count == 3)
                    {
                        m_Graphic.DrawLine(new Pen(Color.Aqua), (m_arrPoints[0].X) * m_smVisionInfo.g_fScaleX, (m_arrPoints[0].Y) * m_smVisionInfo.g_fScaleY, (m_arrPoints[1].X) * m_smVisionInfo.g_fScaleX, (m_arrPoints[1].Y) * m_smVisionInfo.g_fScaleY);
                        m_Graphic.DrawLine(new Pen(Color.Aqua), (m_arrPoints[2].X) * m_smVisionInfo.g_fScaleX, (m_arrPoints[2].Y) * m_smVisionInfo.g_fScaleY, (m_arrPoints[1].X) * m_smVisionInfo.g_fScaleX, (m_arrPoints[1].Y) * m_smVisionInfo.g_fScaleY);
                    }
                }

                if (m_arrPoints.Count == 1)
                {
                    m_Graphic.DrawLine(new Pen(Color.Aqua), (m_arrPoints[0].X) * m_smVisionInfo.g_fScaleX, (m_arrPoints[0].Y) * m_smVisionInfo.g_fScaleY, (m_intMousePointX) * m_smVisionInfo.g_fScaleX, (m_intMousePointY) * m_smVisionInfo.g_fScaleY);
                }
                if (m_arrPoints.Count == 2)
                {
                    m_Graphic.DrawLine(new Pen(Color.Aqua), (m_arrPoints[1].X) * m_smVisionInfo.g_fScaleX, (m_arrPoints[1].Y) * m_smVisionInfo.g_fScaleY, (m_intMousePointX) * m_smVisionInfo.g_fScaleX, (m_intMousePointY) * m_smVisionInfo.g_fScaleY);

                    //if (Math.Abs(m_arrPoints[1].X - m_arrPoints[0].X) > Math.Abs(m_arrPoints[0].Y - m_arrPoints[1].Y))
                    //{
                    //    m_Graphic.DrawLine(new Pen(Color.Aqua), (m_arrPoints[0].X) * m_smVisionInfo.g_fScaleX, (m_arrPoints[0].Y) * m_smVisionInfo.g_fScaleY, (m_arrPoints[0].X) * m_smVisionInfo.g_fScaleX, (m_intMousePointY) * m_smVisionInfo.g_fScaleY);
                    //    m_Graphic.DrawLine(new Pen(Color.Aqua), (m_arrPoints[0].X) * m_smVisionInfo.g_fScaleX, (m_intMousePointY) * m_smVisionInfo.g_fScaleY, (m_intMousePointX) * m_smVisionInfo.g_fScaleX, (m_intMousePointY) * m_smVisionInfo.g_fScaleY);
                    //}
                    //else
                    //{
                    //    m_Graphic.DrawLine(new Pen(Color.Aqua), (m_arrPoints[0].X) * m_smVisionInfo.g_fScaleX, (m_arrPoints[0].Y) * m_smVisionInfo.g_fScaleY, (m_intMousePointX) * m_smVisionInfo.g_fScaleX, (m_arrPoints[0].Y) * m_smVisionInfo.g_fScaleY);
                    //    m_Graphic.DrawLine(new Pen(Color.Aqua), (m_intMousePointX) * m_smVisionInfo.g_fScaleX, (m_arrPoints[0].Y) * m_smVisionInfo.g_fScaleY, (m_intMousePointX) * m_smVisionInfo.g_fScaleX, (m_intMousePointY) * m_smVisionInfo.g_fScaleY);
                    //}
                }
                if (m_arrPoints.Count == 3)
                {
                    m_Graphic.DrawLine(new Pen(Color.Aqua), (m_arrPoints[2].X) * m_smVisionInfo.g_fScaleX, (m_arrPoints[2].Y) * m_smVisionInfo.g_fScaleY, (m_intMousePointX) * m_smVisionInfo.g_fScaleX, (m_intMousePointY) * m_smVisionInfo.g_fScaleY);
                }
            }
            
        }

        private void pic_Image_MouseDown(object sender, MouseEventArgs e)
        {
            int intPositionX = m_intMouseHitX = (int)Math.Round(e.X / m_smVisionInfo.g_fScaleX, 0, MidpointRounding.AwayFromZero);
            int intPositionY = m_intMouseHitY = (int)Math.Round(e.Y / m_smVisionInfo.g_fScaleY, 0, MidpointRounding.AwayFromZero);


            if (m_blnDrawROI)
            {
                //ClearDragHandle();
                m_objROI.VerifyROIArea(intPositionX, intPositionY);
                
            }
            else
            {
                //m_arrPoints.Add(new PointF(intPositionX, intPositionY));
               
                //if (m_arrPoints.Count == 3)
                //{
                //    m_blnDrawROI = true;
                //    int intMinX = int.MaxValue;
                //    int intMinY = int.MaxValue;
                //    int intMaxX = 0;
                //    int intMaxY = 0;
                //    for (int i = 0; i < m_arrPoints.Count; i++)
                //    {
                //        if (intMinX > m_arrPoints[i].X)
                //            intMinX = (int)Math.Round(m_arrPoints[i].X);

                //        if (intMinY > m_arrPoints[i].Y)
                //            intMinY = (int)Math.Round(m_arrPoints[i].Y);

                //        if (intMaxX < m_arrPoints[i].X)
                //            intMaxX = (int)Math.Round(m_arrPoints[i].X);

                //        if (intMaxY < m_arrPoints[i].Y)
                //            intMaxY = (int)Math.Round(m_arrPoints[i].Y);
                //    }
                //        m_objROI.LoadROISetting(intMinX, intMinY,intMaxX-intMinX, intMaxY-intMinY);
                //    //m_objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                //}
            }
            m_UpdatePictureBox = true;
        }

        private void ClearDragHandle()
        {
            m_objROI.ClearDragHandle();
        }

        private void pic_Image_MouseUp(object sender, MouseEventArgs e)
        {
            int intPositionX = m_intMouseHitX = (int)Math.Round(e.X / m_smVisionInfo.g_fScaleX, 0, MidpointRounding.AwayFromZero);
            int intPositionY = m_intMouseHitY = (int)Math.Round(e.Y / m_smVisionInfo.g_fScaleY, 0, MidpointRounding.AwayFromZero);


            if (m_blnDrawROI)
                ClearDragHandle();
            else
            {
                m_arrPoints.Add(new PointF(intPositionX, intPositionY));

                if (m_arrPoints.Count == 4)
                {
                    m_blnDrawROI = true;
                    //int intMinX = int.MaxValue;
                    //int intMinY = int.MaxValue;
                    //int intMaxX = 0;
                    //int intMaxY = 0;
                    //for (int i = 0; i < m_arrPoints.Count; i++)
                    //{
                    //    if (intMinX > m_arrPoints[i].X)
                    //        intMinX = (int)Math.Round(m_arrPoints[i].X);

                    //    if (intMinY > m_arrPoints[i].Y)
                    //        intMinY = (int)Math.Round(m_arrPoints[i].Y);

                    //    if (intMaxX < m_arrPoints[i].X)
                    //        intMaxX = (int)Math.Round(m_arrPoints[i].X);

                    //    if (intMaxY < m_arrPoints[i].Y)
                    //        intMaxY = (int)Math.Round(m_arrPoints[i].Y);
                    //}
                    //m_objROI.LoadROISetting(intMinX, intMinY, intMaxX - intMinX, intMaxY - intMinY);
                    m_objROI.LoadROISetting(m_arrPoints, m_Graphic, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                }
            }
        }

        private void pic_Image_MouseMove(object sender, MouseEventArgs e)
        {
            m_intMousePointX = (int)Math.Round(e.X / m_smVisionInfo.g_fScaleX, 0, MidpointRounding.AwayFromZero);
            m_intMousePointY = (int)Math.Round(e.Y / m_smVisionInfo.g_fScaleY, 0, MidpointRounding.AwayFromZero);

            if (m_blnDrawROI)
            {
                m_objROI.VerifyROIHandleShape(m_intMousePointX, m_intMousePointY);

                if (m_objROI.GetROIHandle())
                {
                    m_objROI.DragROI(m_intMousePointX, m_intMousePointY);
                }
            }
            m_UpdatePictureBox = true;
        }

        private void btn_Undo_Click(object sender, EventArgs e)
        {
            m_arrPoints.Clear();
            m_blnDrawROI = false;
            ClearDragHandle();
            m_objROI.Dispose();
            m_objROI = new FreeShapeROI(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);//pic_Image.Size.Width, pic_Image.Size.Height
            m_UpdatePictureBox = true;
        }
        private void DrawZoomInOutImage()
        {
            if (m_fZoomCountPrev != m_fZoomCount)
            {
                m_fZoomCountPrev = m_fZoomCount;

                m_smVisionInfo.g_fScaleX = m_fOriScaleX * m_fZoomCount;
                m_smVisionInfo.g_fScaleY = m_fOriScaleY * m_fZoomCount;

                SetScaleToComponents(false, false, false);

                if (m_fZoomCount == 1)
                {
                    pic_Image.Size = pnl_PictureBox.Size;

                    pnl_PictureBox.AutoScroll = false;

                    m_intScollValueXPrev = 0;
                    m_intScollValueYPrev = 0;

                    if ((float)m_smVisionInfo.g_intCameraResolutionHeight / (float)m_smVisionInfo.g_intCameraResolutionWidth != 0.75)
                    {
                        if ((float)pic_Image.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth >= (float)pic_Image.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight)
                        {
                            pnl_PicSideBlock.Location = new Point((int)(m_smVisionInfo.g_intCameraResolutionWidth * m_smVisionInfo.g_fScaleX), 0);
                            pnl_PicSideBlock.Width = (int)(pic_Image.Size.Width - m_smVisionInfo.g_intCameraResolutionWidth * m_smVisionInfo.g_fScaleX);
                            pnl_PicSideBlock.Height = pic_Image.Size.Height;
                            pnl_PicSideBlock.BringToFront();
                        }
                        else
                        {
                            pnl_PicSideBlock.Location = new Point(0, (int)(m_smVisionInfo.g_intCameraResolutionHeight * m_smVisionInfo.g_fScaleY));
                            pnl_PicSideBlock.Width = pic_Image.Size.Width;
                            pnl_PicSideBlock.Height = (int)(pic_Image.Size.Height - m_smVisionInfo.g_intCameraResolutionHeight * m_smVisionInfo.g_fScaleY);
                            pnl_PicSideBlock.BringToFront();
                        }
                    }
                }
                else
                {
                    int intFocusPointX = 0;
                    int intFocusPointY = 0;
                    float fFocusRatioX = 0;
                    float fFocusRatioY = 0;
                    int intPicImageWidthPrev = 0;
                    int intPicImageHeightPrev = 0;

                    intPicImageWidthPrev = pic_Image.Size.Width;
                    intPicImageHeightPrev = pic_Image.Size.Height;
                    pic_Image.Size = new Size((int)Math.Ceiling((float)pnl_PictureBox.Size.Width * m_fZoomCount), (int)Math.Ceiling((float)pnl_PictureBox.Size.Height * m_fZoomCount));
                    if ((float)m_smVisionInfo.g_intCameraResolutionHeight / (float)m_smVisionInfo.g_intCameraResolutionWidth != 0.75)
                    {
                        if ((float)pic_Image.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth >= (float)pic_Image.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight)
                        {
                            pnl_PicSideBlock.Location = new Point((int)(m_smVisionInfo.g_intCameraResolutionWidth * m_smVisionInfo.g_fScaleX), 0);
                            pnl_PicSideBlock.Width = (int)(pic_Image.Size.Width - m_smVisionInfo.g_intCameraResolutionWidth * m_smVisionInfo.g_fScaleX);
                            pnl_PicSideBlock.Height = pic_Image.Size.Height;
                            pnl_PicSideBlock.BringToFront();
                        }
                        else
                        {
                            pnl_PicSideBlock.Location = new Point(0, (int)(m_smVisionInfo.g_intCameraResolutionHeight * m_smVisionInfo.g_fScaleY));
                            pnl_PicSideBlock.Width = pic_Image.Size.Width;
                            pnl_PicSideBlock.Height = (int)(pic_Image.Size.Height - m_smVisionInfo.g_intCameraResolutionHeight * m_smVisionInfo.g_fScaleY);
                            pnl_PicSideBlock.BringToFront();
                        }
                    }

                    fFocusRatioX = (float)m_intZoomImageFocusPointX / intPicImageWidthPrev;
                    fFocusRatioY = (float)m_intZoomImageFocusPointY / intPicImageHeightPrev;
                    intFocusPointX = (int)Math.Ceiling(fFocusRatioX * pic_Image.Size.Width - 320);
                    intFocusPointY = (int)Math.Ceiling(fFocusRatioY * pic_Image.Size.Height - 240);

                    pnl_PictureBox.AutoScroll = true;

                    if ((pic_Image.Size.Width - 623) >= intFocusPointX && intFocusPointX >= 0)
                    {
                        while (pnl_PictureBox.HorizontalScroll.Value != intFocusPointX)
                        {
                            if (intFocusPointX > 0) // 2019-12-26 ZJYEOH : Negative value will cause error
                            {
                                pnl_PictureBox.HorizontalScroll.Value = intFocusPointX;
                                break;
                            }
                            else
                                break;
                            Thread.Sleep(1);
                        }
                    }
                    else if (intFocusPointX < 0)
                    {
                        while (pnl_PictureBox.HorizontalScroll.Value != 0)
                        {
                            pnl_PictureBox.HorizontalScroll.Value = 0;
                            Thread.Sleep(1);
                        }
                    }
                    else
                    {
                        while (pnl_PictureBox.HorizontalScroll.Value != (pic_Image.Size.Width - 623)) // Picture size width - panel width + 17 is the maximum value for horizontal
                        {
                            if ((pic_Image.Size.Width - 623) > 0)// 2019-12-26 ZJYEOH : Negative value will cause error
                            {
                                pnl_PictureBox.HorizontalScroll.Value = (pic_Image.Size.Width - 623);
                                break;
                            }
                            else
                            {
                                break;
                            }
                            Thread.Sleep(1);
                        }
                    }

                    if ((pic_Image.Size.Height - 463) >= intFocusPointY && intFocusPointY >= 0)
                    {
                        while (pnl_PictureBox.VerticalScroll.Value != intFocusPointY)
                        {
                            if (intFocusPointY > 0)// 2019-12-26 ZJYEOH : Negative value will cause error
                            {
                                pnl_PictureBox.VerticalScroll.Value = intFocusPointY;
                                break;
                            }
                            else
                                break;
                            Thread.Sleep(1);
                        }
                    }
                    else if (intFocusPointY < 0)
                    {
                        while (pnl_PictureBox.VerticalScroll.Value != 0)
                        {
                            pnl_PictureBox.VerticalScroll.Value = 0;
                            Thread.Sleep(1);
                        }
                    }
                    else
                    {
                        while (pnl_PictureBox.VerticalScroll.Value != (pic_Image.Size.Height - 463)) // Picture size Height - panel height + 17 is the maximum value for vertical
                        {
                            if ((pic_Image.Size.Height - 463) > 0)// 2019-12-26 ZJYEOH : Negative value will cause error
                            {
                                pnl_PictureBox.VerticalScroll.Value = (pic_Image.Size.Height - 463);
                                break;
                            }
                            else
                                break;
                            Thread.Sleep(1);
                        }
                    }
                }

                m_Graphic = Graphics.FromHwnd(pic_Image.Handle);
            }
        }
        private void DrawZoomInOutImage_Backup()
        {
            if (m_fZoomCountPrev != m_fZoomCount)
            {
                m_fZoomCountPrev = m_fZoomCount;

                m_smVisionInfo.g_fScaleX = m_fOriScaleX * m_fZoomCount;
                m_smVisionInfo.g_fScaleY = m_fOriScaleY * m_fZoomCount;

                SetScaleToComponents(false, false, false);

                if (m_fZoomCount == 1)
                {
                    pic_Image.Size = pnl_PictureBox.Size;

                    pnl_PictureBox.AutoScroll = false;

                    m_intScollValueXPrev = 0;
                    m_intScollValueYPrev = 0;

                    if ((float)pic_Image.Size.Height / (float)pic_Image.Size.Width != 0.75)
                    {
                        if ((float)pic_Image.Size.Width / (float)pic_Image.Size.Width >= (float)pic_Image.Size.Height / (float)pic_Image.Size.Height)
                        {
                            pnl_PicSideBlock.Location = new Point((int)(pic_Image.Size.Width * m_smVisionInfo.g_fScaleX), 0);
                            pnl_PicSideBlock.Width = (int)(pic_Image.Size.Width - pic_Image.Size.Width * m_smVisionInfo.g_fScaleX);
                            pnl_PicSideBlock.Height = pic_Image.Size.Height;
                            pnl_PicSideBlock.BringToFront();
                        }
                        else
                        {
                            pnl_PicSideBlock.Location = new Point(0, (int)(pic_Image.Size.Height * m_smVisionInfo.g_fScaleY));
                            pnl_PicSideBlock.Width = pic_Image.Size.Width;
                            pnl_PicSideBlock.Height = (int)(pic_Image.Size.Height - pic_Image.Size.Height * m_smVisionInfo.g_fScaleY);
                            pnl_PicSideBlock.BringToFront();
                        }
                    }
                }
                else
                {
                    int intFocusPointX = 0;
                    int intFocusPointY = 0;
                    float fFocusRatioX = 0;
                    float fFocusRatioY = 0;
                    int intPicImageWidthPrev = 0;
                    int intPicImageHeightPrev = 0;

                    intPicImageWidthPrev = pic_Image.Size.Width;
                    intPicImageHeightPrev = pic_Image.Size.Height;
                    pic_Image.Size = new Size((int)Math.Ceiling((float)pnl_PictureBox.Size.Width * m_fZoomCount), (int)Math.Ceiling((float)pnl_PictureBox.Size.Height * m_fZoomCount));
                    if ((float)pic_Image.Size.Height / (float)pic_Image.Size.Width != 0.75)
                    {
                        if ((float)pic_Image.Size.Width / (float)pic_Image.Size.Width >= (float)pic_Image.Size.Height / (float)pic_Image.Size.Height)
                        {
                            pnl_PicSideBlock.Location = new Point((int)(pic_Image.Size.Width * m_smVisionInfo.g_fScaleX), 0);
                            pnl_PicSideBlock.Width = (int)(pic_Image.Size.Width - pic_Image.Size.Width * m_smVisionInfo.g_fScaleX);
                            pnl_PicSideBlock.Height = pic_Image.Size.Height;
                            pnl_PicSideBlock.BringToFront();
                        }
                        else
                        {
                            pnl_PicSideBlock.Location = new Point(0, (int)(pic_Image.Size.Height * m_smVisionInfo.g_fScaleY));
                            pnl_PicSideBlock.Width = pic_Image.Size.Width;
                            pnl_PicSideBlock.Height = (int)(pic_Image.Size.Height - pic_Image.Size.Height * m_smVisionInfo.g_fScaleY);
                            pnl_PicSideBlock.BringToFront();
                        }
                    }

                    fFocusRatioX = (float)m_intZoomImageFocusPointX / intPicImageWidthPrev;
                    fFocusRatioY = (float)m_intZoomImageFocusPointY / intPicImageHeightPrev;
                    intFocusPointX = (int)Math.Ceiling(fFocusRatioX * pic_Image.Size.Width - 320);
                    intFocusPointY = (int)Math.Ceiling(fFocusRatioY * pic_Image.Size.Height - 240);

                    pnl_PictureBox.AutoScroll = true;

                    if ((pic_Image.Size.Width - 623) >= intFocusPointX && intFocusPointX >= 0)
                    {
                        while (pnl_PictureBox.HorizontalScroll.Value != intFocusPointX)
                        {
                            if (intFocusPointX > 0) // 2019-12-26 ZJYEOH : Negative value will cause error
                            {
                                pnl_PictureBox.HorizontalScroll.Value = intFocusPointX;
                                break;
                            }
                            else
                                break;
                            Thread.Sleep(1);
                        }
                    }
                    else if (intFocusPointX < 0)
                    {
                        while (pnl_PictureBox.HorizontalScroll.Value != 0)
                        {
                            pnl_PictureBox.HorizontalScroll.Value = 0;
                            Thread.Sleep(1);
                        }
                    }
                    else
                    {
                        while (pnl_PictureBox.HorizontalScroll.Value != (pic_Image.Size.Width - 623)) // Picture size width - panel width + 17 is the maximum value for horizontal
                        {
                            if ((pic_Image.Size.Width - 623) > 0)// 2019-12-26 ZJYEOH : Negative value will cause error
                            {
                                pnl_PictureBox.HorizontalScroll.Value = (pic_Image.Size.Width - 623);
                                break;
                            }
                            else
                            {
                                break;
                            }
                            Thread.Sleep(1);
                        }
                    }

                    if ((pic_Image.Size.Height - 463) >= intFocusPointY && intFocusPointY >= 0)
                    {
                        while (pnl_PictureBox.VerticalScroll.Value != intFocusPointY)
                        {
                            if (intFocusPointY > 0)// 2019-12-26 ZJYEOH : Negative value will cause error
                            {
                                pnl_PictureBox.VerticalScroll.Value = intFocusPointY;
                                break;
                            }
                            else
                                break;
                            Thread.Sleep(1);
                        }
                    }
                    else if (intFocusPointY < 0)
                    {
                        while (pnl_PictureBox.VerticalScroll.Value != 0)
                        {
                            pnl_PictureBox.VerticalScroll.Value = 0;
                            Thread.Sleep(1);
                        }
                    }
                    else
                    {
                        while (pnl_PictureBox.VerticalScroll.Value != (pic_Image.Size.Height - 463)) // Picture size Height - panel height + 17 is the maximum value for vertical
                        {
                            if ((pic_Image.Size.Height - 463) > 0)// 2019-12-26 ZJYEOH : Negative value will cause error
                            {
                                pnl_PictureBox.VerticalScroll.Value = (pic_Image.Size.Height - 463);
                                break;
                            }
                            else
                                break;
                            Thread.Sleep(1);
                        }
                    }
                }

                m_Graphic = Graphics.FromHwnd(pic_Image.Handle);
            }
        }

        private void SetScaleToComponents(bool blnFirstTime, bool blnScaleToPictureBox, bool blnScaleToPictureBox2)
        {
            if (blnFirstTime)
            {
                if ((float)m_smVisionInfo.g_intCameraResolutionHeight / (float)m_smVisionInfo.g_intCameraResolutionWidth != 0.75)
                {
                    if ((float)pnl_PictureBox.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth >= (float)pnl_PictureBox.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight)
                    {
                        m_smVisionInfo.g_fScaleX = m_fOriScaleX = (float)pnl_PictureBox.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight;
                        m_smVisionInfo.g_fScaleY = m_fOriScaleY = (float)pnl_PictureBox.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight;

                        pnl_PicSideBlock.Location = new Point((int)(m_smVisionInfo.g_intCameraResolutionWidth * m_smVisionInfo.g_fScaleX), 0);
                        pnl_PicSideBlock.Width = (int)(pnl_PictureBox.Size.Width - m_smVisionInfo.g_intCameraResolutionWidth * m_smVisionInfo.g_fScaleX);
                        pnl_PicSideBlock.Height = pnl_PictureBox.Size.Height;
                        pnl_PicSideBlock.BringToFront();
                    }
                    else
                    {
                        m_smVisionInfo.g_fScaleX = m_fOriScaleX = (float)pnl_PictureBox.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth;
                        m_smVisionInfo.g_fScaleY = m_fOriScaleY = (float)pnl_PictureBox.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth;

                        pnl_PicSideBlock.Location = new Point(0, (int)(m_smVisionInfo.g_intCameraResolutionHeight * m_smVisionInfo.g_fScaleY));
                        pnl_PicSideBlock.Width = pnl_PictureBox.Size.Width;
                        pnl_PicSideBlock.Height = (int)(pnl_PictureBox.Size.Height - m_smVisionInfo.g_intCameraResolutionHeight * m_smVisionInfo.g_fScaleY);
                        pnl_PicSideBlock.BringToFront();
                    }
                }
                else
                {
                    m_smVisionInfo.g_fScaleX = m_fOriScaleX = (float)pnl_PictureBox.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth;
                    m_smVisionInfo.g_fScaleY = m_fOriScaleY = (float)pnl_PictureBox.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight;
                }
            }

            if (blnScaleToPictureBox)
            {
                if ((float)m_smVisionInfo.g_intCameraResolutionHeight / (float)m_smVisionInfo.g_intCameraResolutionWidth != 0.75)
                {
                    if ((float)pnl_PictureBox.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth >= (float)pnl_PictureBox.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight)
                    {
                        m_smVisionInfo.g_fScaleX = (float)pnl_PictureBox.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight;
                        m_smVisionInfo.g_fScaleY = (float)pnl_PictureBox.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight;

                        pnl_PicSideBlock.Location = new Point((int)(m_smVisionInfo.g_intCameraResolutionWidth * m_smVisionInfo.g_fScaleX), 0);
                        pnl_PicSideBlock.Width = (int)(pnl_PictureBox.Size.Width - m_smVisionInfo.g_intCameraResolutionWidth * m_smVisionInfo.g_fScaleX);
                        pnl_PicSideBlock.Height = pnl_PictureBox.Size.Height;
                        pnl_PicSideBlock.BringToFront();
                    }
                    else
                    {
                        m_smVisionInfo.g_fScaleX = (float)pnl_PictureBox.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth;
                        m_smVisionInfo.g_fScaleY = (float)pnl_PictureBox.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth;

                        pnl_PicSideBlock.Location = new Point(0, (int)(m_smVisionInfo.g_intCameraResolutionHeight * m_smVisionInfo.g_fScaleY));
                        pnl_PicSideBlock.Width = pnl_PictureBox.Size.Width;
                        pnl_PicSideBlock.Height = (int)(pnl_PictureBox.Size.Height - m_smVisionInfo.g_intCameraResolutionHeight * m_smVisionInfo.g_fScaleY);
                        pnl_PicSideBlock.BringToFront();
                    }
                }
                else
                {
                    m_smVisionInfo.g_fScaleX = (float)pnl_PictureBox.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth;
                    m_smVisionInfo.g_fScaleY = (float)pnl_PictureBox.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight;
                }
            }
            

            if (m_smVisionInfo.g_objMemoryImage != null)
            {
                m_smVisionInfo.g_objMemoryImage.ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                m_smVisionInfo.g_objMemoryImage.ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
            }

            if (m_smVisionInfo.g_ojRotateImage != null)
            {
                m_smVisionInfo.g_ojRotateImage.ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                m_smVisionInfo.g_ojRotateImage.ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
            }

            if (m_smVisionInfo.g_objPackageImage != null)
            {
                m_smVisionInfo.g_objPackageImage.ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                m_smVisionInfo.g_objPackageImage.ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
            }

            if (m_smVisionInfo.g_objPkgProcessImage != null)
            {
                m_smVisionInfo.g_objPkgProcessImage.ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                m_smVisionInfo.g_objPkgProcessImage.ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
            }

            if (m_smVisionInfo.g_objSealImage != null)
            {
                m_smVisionInfo.g_objSealImage.ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                m_smVisionInfo.g_objSealImage.ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
            }

            if (m_smVisionInfo.g_arrImages != null)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                {
                    if (m_smVisionInfo.g_arrImages[i] != null)
                    {
                        m_smVisionInfo.g_arrImages[i].ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                        m_smVisionInfo.g_arrImages[i].ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
                    }
                }
            }

            if (m_smVisionInfo.g_arrRotatedImages != null)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
                {
                    if (m_smVisionInfo.g_arrRotatedImages[i] != null)
                    {
                        m_smVisionInfo.g_arrRotatedImages[i].ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                        m_smVisionInfo.g_arrRotatedImages[i].ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
                    }
                }
            }

            for (int h = 0; h < m_smVisionInfo.g_arr5SRotatedImages.Length; h++)
            {
                if (m_smVisionInfo.g_arr5SRotatedImages[h] != null)
                {
                    for (int i = 0; i < m_smVisionInfo.g_arr5SRotatedImages[h].Count; i++)
                    {
                        if (m_smVisionInfo.g_arr5SRotatedImages[h][i] != null)
                        {
                            m_smVisionInfo.g_arr5SRotatedImages[h][i].ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                            m_smVisionInfo.g_arr5SRotatedImages[h][i].ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
                        }
                    }
                }
            }

            if (m_smVisionInfo.g_WorldShape != null)
                m_smVisionInfo.g_WorldShape.SetZoom(m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
        }
        private void SetScaleToComponents_Backup(bool blnFirstTime, bool blnScaleToPictureBox, bool blnScaleToPictureBox2)
        {
            if (blnFirstTime)
            {
                if ((float)pic_Image.Size.Height / (float)pic_Image.Size.Width != 0.75)
                {
                    if ((float)pnl_PictureBox.Size.Width / (float)pic_Image.Size.Width >= (float)pnl_PictureBox.Size.Height / (float)pic_Image.Size.Height)
                    {
                        m_smVisionInfo.g_fScaleX = m_fOriScaleX = (float)pnl_PictureBox.Size.Height / (float)pic_Image.Size.Height;
                        m_smVisionInfo.g_fScaleY = m_fOriScaleY = (float)pnl_PictureBox.Size.Height / (float)pic_Image.Size.Height;

                        pnl_PicSideBlock.Location = new Point((int)(pic_Image.Size.Width * m_smVisionInfo.g_fScaleX), 0);
                        pnl_PicSideBlock.Width = (int)(pnl_PictureBox.Size.Width - pic_Image.Size.Width * m_smVisionInfo.g_fScaleX);
                        pnl_PicSideBlock.Height = pnl_PictureBox.Size.Height;
                        pnl_PicSideBlock.BringToFront();
                    }
                    else
                    {
                        m_smVisionInfo.g_fScaleX = m_fOriScaleX = (float)pnl_PictureBox.Size.Width / (float)pic_Image.Size.Width;
                        m_smVisionInfo.g_fScaleY = m_fOriScaleY = (float)pnl_PictureBox.Size.Width / (float)pic_Image.Size.Width;

                        pnl_PicSideBlock.Location = new Point(0, (int)(pic_Image.Size.Height * m_smVisionInfo.g_fScaleY));
                        pnl_PicSideBlock.Width = pnl_PictureBox.Size.Width;
                        pnl_PicSideBlock.Height = (int)(pnl_PictureBox.Size.Height - pic_Image.Size.Height * m_smVisionInfo.g_fScaleY);
                        pnl_PicSideBlock.BringToFront();
                    }
                }
                else
                {
                    m_smVisionInfo.g_fScaleX = m_fOriScaleX = (float)pnl_PictureBox.Size.Width / (float)pnl_PictureBox.Size.Width;
                    m_smVisionInfo.g_fScaleY = m_fOriScaleY = (float)pnl_PictureBox.Size.Height / (float)pnl_PictureBox.Size.Height;
                }
            }

            if (blnScaleToPictureBox)
            {
                if ((float)pnl_PictureBox.Size.Height / (float)pnl_PictureBox.Size.Width != 0.75)
                {
                    if ((float)pnl_PictureBox.Size.Width / (float)pnl_PictureBox.Size.Width >= (float)pnl_PictureBox.Size.Height / (float)pnl_PictureBox.Size.Height)
                    {
                        m_smVisionInfo.g_fScaleX = (float)pnl_PictureBox.Size.Height / (float)pnl_PictureBox.Size.Height;
                        m_smVisionInfo.g_fScaleY = (float)pnl_PictureBox.Size.Height / (float)pnl_PictureBox.Size.Height;

                        pnl_PicSideBlock.Location = new Point((int)(pnl_PictureBox.Size.Width * m_smVisionInfo.g_fScaleX), 0);
                        pnl_PicSideBlock.Width = (int)(pnl_PictureBox.Size.Width - pnl_PictureBox.Size.Width * m_smVisionInfo.g_fScaleX);
                        pnl_PicSideBlock.Height = pnl_PictureBox.Size.Height;
                        pnl_PicSideBlock.BringToFront();
                    }
                    else
                    {
                        m_smVisionInfo.g_fScaleX = (float)pnl_PictureBox.Size.Width / (float)pnl_PictureBox.Size.Width;
                        m_smVisionInfo.g_fScaleY = (float)pnl_PictureBox.Size.Width / (float)pnl_PictureBox.Size.Width;

                        pnl_PicSideBlock.Location = new Point(0, (int)(pnl_PictureBox.Size.Height * m_smVisionInfo.g_fScaleY));
                        pnl_PicSideBlock.Width = pnl_PictureBox.Size.Width;
                        pnl_PicSideBlock.Height = (int)(pnl_PictureBox.Size.Height - pnl_PictureBox.Size.Height * m_smVisionInfo.g_fScaleY);
                        pnl_PicSideBlock.BringToFront();
                    }
                }
                else
                {
                    m_smVisionInfo.g_fScaleX = (float)pnl_PictureBox.Size.Width / (float)pnl_PictureBox.Size.Width;
                    m_smVisionInfo.g_fScaleY = (float)pnl_PictureBox.Size.Height / (float)pnl_PictureBox.Size.Height;
                }
            }

            if (m_smVisionInfo.g_objMemoryImage != null)
            {
                m_smVisionInfo.g_objMemoryImage.ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                m_smVisionInfo.g_objMemoryImage.ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
            }

            if (m_smVisionInfo.g_ojRotateImage != null)
            {
                m_smVisionInfo.g_ojRotateImage.ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                m_smVisionInfo.g_ojRotateImage.ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
            }

            if (m_smVisionInfo.g_objPackageImage != null)
            {
                m_smVisionInfo.g_objPackageImage.ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                m_smVisionInfo.g_objPackageImage.ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
            }

            if (m_smVisionInfo.g_objPkgProcessImage != null)
            {
                m_smVisionInfo.g_objPkgProcessImage.ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                m_smVisionInfo.g_objPkgProcessImage.ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
            }

            if (m_smVisionInfo.g_objSealImage != null)
            {
                m_smVisionInfo.g_objSealImage.ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                m_smVisionInfo.g_objSealImage.ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
            }

            if (m_smVisionInfo.g_arrImages != null)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                {
                    if (m_smVisionInfo.g_arrImages[i] != null)
                    {
                        m_smVisionInfo.g_arrImages[i].ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                        m_smVisionInfo.g_arrImages[i].ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
                    }
                }
            }

            if (m_smVisionInfo.g_arrRotatedImages != null)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
                {
                    if (m_smVisionInfo.g_arrRotatedImages[i] != null)
                    {
                        m_smVisionInfo.g_arrRotatedImages[i].ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                        m_smVisionInfo.g_arrRotatedImages[i].ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
                    }
                }
            }

            for (int h = 0; h < m_smVisionInfo.g_arr5SRotatedImages.Length; h++)
            {
                if (m_smVisionInfo.g_arr5SRotatedImages[h] != null)
                {
                    for (int i = 0; i < m_smVisionInfo.g_arr5SRotatedImages[h].Count; i++)
                    {
                        if (m_smVisionInfo.g_arr5SRotatedImages[h][i] != null)
                        {
                            m_smVisionInfo.g_arr5SRotatedImages[h][i].ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                            m_smVisionInfo.g_arr5SRotatedImages[h][i].ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
                        }
                    }
                }
            }

            if (m_smVisionInfo.g_WorldShape != null)
                m_smVisionInfo.g_WorldShape.SetZoom(m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
        }
        private void pnl_PictureBox_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                int intScollValue = e.NewValue;
                m_intScollValueXPrev = intScollValue;
            }

            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                int intScollValue = e.NewValue;
                m_intScollValueYPrev = intScollValue;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            m_fZoomCount = (float)Convert.ToDouble(trackBar1.Value) / 10;
            m_smVisionInfo.g_fZoomScale = m_fZoomCount;
            m_UpdatePictureBox = true;
        }

        private void pic_Image_Paint(object sender, PaintEventArgs e)
        {
            m_UpdatePictureBox = true;
        }

        private void AutoTuneGaugeDrawForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.g_intSelectedImage = m_intSelectedImage_Prev;
            m_smVisionInfo.g_fScaleX = m_fScaleXPrev;
            m_smVisionInfo.g_fScaleY = m_fScaleYPrev;
            m_fZoomCount = m_smVisionInfo.g_fZoomScale = m_fZoomScalePrev;
            DrawZoomInOutImage();
        }

        public void GetLineAngle(int intLineIndex, ref float fAngle)
        {
            fAngle = m_objROI.GetLineAngle(intLineIndex);
        }
        public void GetLineAngle(int intLineIndex, int intPadIndex, ref float fAngle)
        {
            switch (intPadIndex)
            {
                case 0: //Center ROI
                case 1: // Top ROI
                    switch (intLineIndex)
                    {
                        case 0: //Top
                            fAngle = m_objROI.GetLineAngle(0);
                            if (fAngle > 0)
                            {
                                fAngle -= 90;
                            }
                            else
                            {
                                fAngle += 90;
                            }
                            break;
                        case 1: //Right
                            fAngle = m_objROI.GetLineAngle(1);
                            break;
                        case 2: //Bottom
                            fAngle = m_objROI.GetLineAngle(2);
                            if (fAngle > 0)
                            {
                                fAngle -= 90;
                            }
                            else
                            {
                                fAngle += 90;
                            }
                            break;
                        case 3: //Left
                            fAngle = m_objROI.GetLineAngle(3);
                            break;
                    }
                    break;
                case 2:
                    switch (intLineIndex)
                    {
                        case 1: //Top
                            fAngle = m_objROI.GetLineAngle(0);
                            if (fAngle > 0)
                            {
                                fAngle -= 90;
                            }
                            else
                            {
                                fAngle += 90;
                            }
                            break;
                        case 2: //Right
                            fAngle = m_objROI.GetLineAngle(1);
                            break;
                        case 3: //Bottom
                            fAngle = m_objROI.GetLineAngle(2);
                            if (fAngle > 0)
                            {
                                fAngle -= 90;
                            }
                            else
                            {
                                fAngle += 90;
                            }
                            break;
                        case 0: //Left
                            fAngle = m_objROI.GetLineAngle(3);
                            break;
                    }
                    break;
                case 3:
                    switch (intLineIndex)
                    {
                        case 2: //Top
                            fAngle = m_objROI.GetLineAngle(0);
                            if (fAngle > 0)
                            {
                                fAngle -= 90;
                            }
                            else
                            {
                                fAngle += 90;
                            }
                            break;
                        case 3: //Right
                            fAngle = m_objROI.GetLineAngle(1);
                            break;
                        case 0: //Bottom
                            fAngle = m_objROI.GetLineAngle(2);
                            if (fAngle > 0)
                            {
                                fAngle -= 90;
                            }
                            else
                            {
                                fAngle += 90;
                            }
                            break;
                        case 1: //Left
                            fAngle = m_objROI.GetLineAngle(3);
                            break;
                    }
                    break;
                case 4:
                    switch (intLineIndex)
                    {
                        case 3: //Top
                            fAngle = m_objROI.GetLineAngle(0);
                            if (fAngle > 0)
                            {
                                fAngle -= 90;
                            }
                            else
                            {
                                fAngle += 90;
                            }
                            break;
                        case 0: //Right
                            fAngle = m_objROI.GetLineAngle(1);
                            break;
                        case 1: //Bottom
                            fAngle = m_objROI.GetLineAngle(2);
                            if (fAngle > 0)
                            {
                                fAngle -= 90;
                            }
                            else
                            {
                                fAngle += 90;
                            }
                            break;
                        case 2: //Left
                            fAngle = m_objROI.GetLineAngle(3);
                            break;
                    }
                    break;
            }
        }

        public PointF GetLineCenterPoint(int intLineIndex)
        {
            return m_objROI.GetLineCenterPoint(intLineIndex);
               
        }

        public float GetLineInterceptPoint(int intLineIndex, float fPointX, float fPointY)
        {
            return m_objROI.GetLineInterceptPoint(intLineIndex, fPointX, fPointY);

        }

        private void cbo_ImageNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
       
            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNo.SelectedIndex, m_smVisionInfo.g_intVisionIndex);

            m_UpdatePictureBox = true;
        }
    }
}
