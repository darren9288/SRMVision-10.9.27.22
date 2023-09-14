using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Common;

namespace VisionProcessing
{
    public class Crosshair
    {
        #region Member Variables

        private bool m_blnFixROI = true;
        private bool m_blnDrawCrasshair = false;
        private int m_intCrosshairX;
        private int m_intCrosshairY;
        private int m_intHeight = 20;
        private int m_intWidth = 20;
        private int m_intImageWidth = 640;
        private int m_intImageHeight = 480;
        private Pen m_objDrawPen = new Pen(Color.Lime);
        private ROI m_objCrosshairROI = new ROI();

        #endregion

        #region Properties
        public bool ref_blnDrawCrasshair { get { return m_blnDrawCrasshair; } set { m_blnDrawCrasshair = value; } }

        public int ref_intCrosshairX { get { return m_intCrosshairX; } }
        public int ref_intCrosshairY { get { return m_intCrosshairY; } }
        public int ref_intOffSetX { get { return m_intCrosshairX - m_intImageWidth / 2; } }
        public int ref_intOffSetY { get { return m_intCrosshairY - m_intImageHeight / 2; } }
        public ROI ref_objCrosshairROI { get { return m_objCrosshairROI; } }

        #endregion

        /// <summary>
        /// Init crosshair object
        /// </summary>
        /// <param name="blnFixROI">Set True to fix ROI size</param>
        public Crosshair(bool blnFixROI)
        {
            m_intCrosshairX = m_intImageWidth / 2;
            m_intCrosshairY = m_intImageHeight / 2;

            m_blnFixROI = blnFixROI;

            m_objCrosshairROI.LoadROISetting(m_intCrosshairX - m_intWidth / 2, m_intCrosshairY - m_intHeight / 2, 
                                             m_intWidth, m_intHeight);
        }

        public Crosshair(bool blnFixROI, int intImageWidth, int intImageHeight)
        {
            m_intImageWidth = intImageWidth;
            m_intImageHeight = intImageHeight;

            m_intCrosshairX = m_intImageWidth / 2;
            m_intCrosshairY = m_intImageHeight / 2;

            m_blnFixROI = blnFixROI;

            m_objCrosshairROI.LoadROISetting(m_intCrosshairX - m_intWidth / 2, m_intCrosshairY - m_intHeight / 2,
                                             m_intWidth, m_intHeight);
        }

        /// <summary>
        /// Init crosshair object
        /// </summary>
        /// <param name="intCrosshairX">Crosshair center point-x</param>
        /// <param name="intCrosshairY">Crosshair center point-y</param>
        public Crosshair(int intCrosshairX, int intCrosshairY)
        {
            m_intCrosshairX = intCrosshairX;
            m_intCrosshairY = intCrosshairY;

            m_objCrosshairROI.LoadROISetting(m_intCrosshairX - m_intWidth / 2, m_intCrosshairY - m_intHeight / 2,
                                 m_intWidth, m_intHeight);
        }
        /// <summary>
        /// Init crosshair object
        /// </summary>
        /// <param name="intCrosshairX">Crosshair center point-x</param>
        /// <param name="intCrosshairY">Crosshair center point-y</param>
        /// <param name="intWidth">Crosshair ROI width</param>
        /// <param name="intHeight">Crosshair ROI height</param>
        public Crosshair(int intCrosshairX, int intCrosshairY, int intWidth, int intHeight)
        {
            m_intCrosshairX = intCrosshairX;
            m_intCrosshairY = intCrosshairY;
            m_intWidth = intWidth;
            m_intHeight = intHeight;
            m_objCrosshairROI.LoadROISetting(m_intCrosshairX - m_intWidth / 2, m_intCrosshairY - m_intHeight / 2,
                                             m_intWidth, m_intHeight);
        }


        /// <summary>
        /// Drag crosshair according to given new position
        /// </summary>
        /// <param name="intNewPositionX">new position X</param>
        /// <param name="intNewPositionY">new position Y</param>
        /// <returns>true = success to drag, false = otherwise</returns>
        public bool DragCrosshair(int intNewPositionX, int intNewPositionY)
        {
            if (m_objCrosshairROI.GetROIHandle())
            {
                m_objCrosshairROI.DragROI(intNewPositionX, intNewPositionY);
                if (m_blnFixROI)
                {
                    m_objCrosshairROI.ref_ROI.Width = m_intWidth;
                    m_objCrosshairROI.ref_ROI.Height = m_intHeight;
                }
                return true;
            }

            return false;
        }
        /// <summary>
        /// Check whether there is crosshair in given location
        /// </summary>
        /// <param name="intPositionX">Position X on Picture Box</param>
        /// <param name="intPositionY">Position Y on Picture Box</param>
        /// <returns>true = success to find the crosshair, false = otherwise</returns>
        public bool VerifyCorsshair(int intPositionX, int intPositionY)
        {
            if (m_objCrosshairROI.VerifyROIArea(intPositionX, intPositionY)) 
            {
                return true;
            }

            return false;
        }       


        /// <summary>
        /// Crosshair attach image
        /// </summary>
        /// <param name="objImage">Attached image</param>
        public void AttachImage(ImageDrawing objImage)
        {
             m_objCrosshairROI.AttachImage(objImage);
        }
        /// <summary>
        /// Clear Drag Handle when mouse up
        /// </summary>
        public void ClearDragHandle()
        {
            if (m_objCrosshairROI.GetROIHandle())
                m_objCrosshairROI.ClearDragHandle();
        }
        /// <summary>
        /// Draw crosshair line and ROI
        /// </summary>
        /// <param name="g">Picture box graphic</param>
        public void DrawCrosshair(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        { 
            // Draw crosshair line
            g.DrawLine(m_objDrawPen, new PointF(0, (float)m_intCrosshairY * fDrawingScaleY), new PointF(m_intImageWidth - 1, (float)m_intCrosshairY * fDrawingScaleY));
            g.DrawLine(m_objDrawPen, new PointF((float)m_intCrosshairX * fDrawingScaleX, 0), new PointF((float)m_intCrosshairX * fDrawingScaleX, m_intImageHeight - 1));
        }
        /// <summary>
        /// Draw crosshair ROI
        /// </summary>
        /// <param name="g"></param>
        public void DrawCrosshairROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            // Draw crosshair ROI
            if (m_blnFixROI)
                m_objCrosshairROI.DrawROI(g, fDrawingScaleX, fDrawingScaleY, 1, false);
            else
                m_objCrosshairROI.DrawROI(g, fDrawingScaleX, fDrawingScaleY, 1, m_objCrosshairROI.GetROIHandle());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fCalibX"></param>
        public void DrawROIMeasurementValue(Graphics g, float fCalibPixelInMM)
        {
            g.DrawString("W:" + (m_objCrosshairROI.ref_ROIWidth / fCalibPixelInMM * 1000).ToString("F1") +" um",
                         new Font("Verdana", 14), new SolidBrush(Color.Red), m_objCrosshairROI.ref_ROIPositionX + 5, m_objCrosshairROI.ref_ROIPositionY - 43);

            g.DrawString("H:" + (m_objCrosshairROI.ref_ROIHeight / fCalibPixelInMM * 1000).ToString("F1") + " um",
             new Font("Verdana", 14), new SolidBrush(Color.Red), m_objCrosshairROI.ref_ROIPositionX + 5, m_objCrosshairROI.ref_ROIPositionY - 23);

        }
        /// <summary>
        /// Init crosshair center point and attached image
        /// </summary>
        /// <param name="intCrosshairX">Crosshair point-x</param>
        /// <param name="intCrosshairY">Crosshair point-y</param>
        /// <param name="objImage">Attached image</param>
        public void InitCrosshair(int intCrosshairX, int intCrosshairY, ImageDrawing objImage)
        {
            SetCrosshair(intCrosshairX, intCrosshairY);
            m_objCrosshairROI.AttachImage(objImage);
        }

        public void LoadCrosshair(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection(strSectionName);

            m_intCrosshairX = objFile.GetValueAsInt("CrosshairCenterX", m_intImageWidth / 2);
            m_intCrosshairY = objFile.GetValueAsInt("CrosshairCenterY", m_intImageHeight / 2);
            m_intWidth = objFile.GetValueAsInt("CrosshairROIWidth", m_intWidth);
            m_intHeight = objFile.GetValueAsInt("CrosshairROIHeight", m_intHeight);
            m_objCrosshairROI.LoadROISetting(m_intCrosshairX - m_intWidth / 2, m_intCrosshairY - m_intHeight / 2,
                                             m_intWidth, m_intHeight);
        }

        public void SaveCrosshair(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);
            objFile.WriteSectionElement(strSectionName, blnNewSection);

            objFile.WriteElement1Value("CrosshairCenterX", m_intCrosshairX);
            objFile.WriteElement1Value("CrosshairCenterY", m_intCrosshairY);
            objFile.WriteElement1Value("CrosshairROIWidth", m_intWidth);
            objFile.WriteElement1Value("CrosshairROIHeight", m_intHeight);

            objFile.WriteEndElement();
        }
        /// <summary>
        /// Set crosshair point-xy
        /// </summary>
        /// <param name="intCrosshairX">Crosshair point-x</param>
        /// <param name="intCrosshairY">Crosshair point-y</param>
        public void SetCrosshair(int intCrosshairX, int intCrosshairY)
        {
            m_intCrosshairX = intCrosshairX;
            m_intCrosshairY = intCrosshairY;

            m_objCrosshairROI.LoadROISetting(m_intCrosshairX - m_objCrosshairROI.ref_ROIWidth / 2,
                                             m_intCrosshairY - m_objCrosshairROI.ref_ROIHeight / 2, 
                                             m_objCrosshairROI.ref_ROIWidth, 
                                             m_objCrosshairROI.ref_ROIHeight);
        }
        /// <summary>
        /// Set crosshair ROI size
        /// </summary>
        /// <param name="intWidth">Crosshair ROI width</param>
        /// <param name="intHeight">Crosshair ROI height</param>
        public void SetCrosshairROISize(int intWidth, int intHeight)
        {
            m_intWidth = intWidth;
            m_intHeight = intHeight;
            m_objCrosshairROI.LoadROISetting(m_intCrosshairX - m_intWidth / 2, m_intCrosshairY - m_intHeight / 2,
                                             m_intWidth, m_intHeight);
        }

    }
}
