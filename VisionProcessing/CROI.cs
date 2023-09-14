using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif
using Common;
using System.Windows.Forms;

namespace VisionProcessing
{
    public class CROI
    {
        #region Member Variables
        private int m_intStartOffsetX = 0;
        private int m_intStartOffsetY = 0;
        private int m_intType;          // 1 = SearchROI, 2 = TrainROI, 3 = Don't care area, 4 = SubROI
        private string m_strROIName;

        private EColorLookup m_objColorLookup;
        private EDragHandle m_Handler;
        private EDragHandle m_Handler2;
        private EROIC24 m_CROI;
        private EImageC24 m_objParentCImage;
        private Font m_Font = new Font("Verdana", 10);
        // Local Use
        private EImageC24 m_objZoomCImage;
        private EImageC24 m_objDisplayCImage;
        private ERGBColor m_objRGBColor = new ERGBColor(0, 0, 0);
        private Color[] m_Color = new Color[]{Color.Red, Color.Yellow, Color.Lime, Color.DeepPink, Color.Cyan, Color.Fuchsia,
                                              Color.Plum, Color.Honeydew, Color.LawnGreen, Color.Ivory, Color.Cornsilk, Color.DarkOrange};
        #endregion

        #region Properties
        public int ref_intStartOffsetX { get { return m_intStartOffsetX; } set { m_intStartOffsetX = value; } }
        public int ref_intStartOffsetY { get { return m_intStartOffsetY; } set { m_intStartOffsetY = value; } }
        public int ref_intType { get { return m_intType; } set { m_intType = value; } }
        public int ref_ROIPositionX { get { return m_CROI.OrgX; } set { m_CROI.OrgX = value; } }
        public int ref_ROIPositionY { get { return m_CROI.OrgY; } set { m_CROI.OrgY = value; } }
        public int ref_ROIHeight { get { return m_CROI.Height; } set { m_CROI.Height = value; } }
        public int ref_ROIWidth { get { return m_CROI.Width; } set { m_CROI.Width = value; } }
        public int ref_ROICenterX { get { return m_CROI.OrgX + m_CROI.Width / 2; } }
        public int ref_ROICenterY { get { return m_CROI.OrgY + m_CROI.Height / 2; } }
        public int ref_ROITotalCenterX { get { return m_CROI.TotalOrgX + m_CROI.Width / 2; } }
        public int ref_ROITotalCenterY { get { return m_CROI.TotalOrgY + m_CROI.Height / 2; } }
        public int ref_ROITotalX { get { return m_CROI.TotalOrgX; } }
        public int ref_ROITotalY { get { return m_CROI.TotalOrgY; } }
        public string ref_strROIName { get { return m_strROIName; } set { m_strROIName = value; } }

        public EROIC24 ref_CROI { get { return m_CROI; } set { m_CROI = value; } }
        #endregion


        public CROI()
        {
            m_intType = 1;
            m_strROIName = "ROI";
            m_CROI = new EROIC24();
            m_objColorLookup = new EColorLookup();
        }

        public CROI(string strText, int intType)
        {
            m_intType = intType;
            m_strROIName = strText;
            m_CROI = new EROIC24();
            m_objColorLookup = new EColorLookup();
        }



        public void Dispose()
        {
            m_Font.Dispose();

            if (m_CROI != null)
                m_CROI.Dispose();

            if (m_objParentCImage != null)
                m_objParentCImage.Dispose();

            if (m_objZoomCImage != null)
                m_objZoomCImage.Dispose();

            if (m_objDisplayCImage != null)
                m_objDisplayCImage.Dispose();

            if (m_objColorLookup != null)
                m_objColorLookup.Dispose();
        }


        /// <summary>
        /// Check whether image had been attached to this ROI
        /// </summary>
        /// <returns>false if there is no image being attached to ROI</returns>
        public bool CheckROIParent()
        {
            if (m_CROI.Parent == null)
                return false;

            return true;
        }

        /// <summary>
        /// return true if movement or changing is allowed for this ROI Frame
        /// </summary>
        public bool GetROIHandle()
        {
            if (m_Handler != EDragHandle.NoHandle)
                return true;
            else
                return false;
        }
        public bool GetROIHandle2()
        {
            if (m_Handler2 != EDragHandle.NoHandle)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Set RO handle - set true if Detects the cursor is placed within ROI area
        /// Usually call this function in Mouse_Down event
        /// </summary>
        /// <param name="nNewXPoint">the point of mouse move over</param>
        /// <param name="nNewYPoint">the point of mouse move over</param>
        public bool VerifyROIArea(int nNewXPoint, int nNewYPoint)
        {
            //m_Handler = m_CROI.HitTest(nNewXPoint, nNewYPoint);

            //return GetROIHandle();

            if (m_CROI.TopParent == null)
                return false;

            int intRangeTolerance = 10;
            if (m_CROI.Width < 40 || m_CROI.Height < 40)
                intRangeTolerance = 3;
            else if (m_CROI.Width < 100 || m_CROI.Height < 100)
                intRangeTolerance = 5;

            if (((nNewXPoint < (m_CROI.TotalOrgX + intRangeTolerance))
                    && (nNewYPoint > (m_CROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY) && (nNewXPoint > m_CROI.TotalOrgX))
                   || ((nNewXPoint > (m_CROI.TotalOrgX - intRangeTolerance))
                    && (nNewYPoint < (m_CROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY) && (nNewXPoint < m_CROI.TotalOrgX))
                     || ((nNewXPoint < (m_CROI.TotalOrgX + intRangeTolerance))
                    && (nNewYPoint < (m_CROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY) && (nNewXPoint > m_CROI.TotalOrgX))
                     || ((nNewXPoint > (m_CROI.TotalOrgX - intRangeTolerance))
                    && (nNewYPoint > (m_CROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY) && (nNewXPoint < m_CROI.TotalOrgX))
                    )
            {
                m_Handler = m_CROI.HitTest(m_CROI.TotalOrgX, m_CROI.TotalOrgY);

            }
            else if (((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width + intRangeTolerance))
                 && (nNewYPoint > (m_CROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY) && (nNewXPoint > m_CROI.TotalOrgX + m_CROI.Width))
                || ((nNewXPoint > (m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
                 && (nNewYPoint < (m_CROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY) && (nNewXPoint < m_CROI.TotalOrgX + m_CROI.Width))
                  || ((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width + intRangeTolerance))
                 && (nNewYPoint < (m_CROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY) && (nNewXPoint > m_CROI.TotalOrgX + m_CROI.Width))
                  || ((nNewXPoint > (m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
                 && (nNewYPoint > (m_CROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY) && (nNewXPoint < m_CROI.TotalOrgX + m_CROI.Width))
                 )
            {
                m_Handler = m_CROI.HitTest(m_CROI.TotalOrgX + m_CROI.Width, m_CROI.TotalOrgY);

            }
            else if (((nNewXPoint < (m_CROI.TotalOrgX + intRangeTolerance))
              && (nNewYPoint > (m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height) && (nNewXPoint > m_CROI.TotalOrgX))
             || ((nNewXPoint > (m_CROI.TotalOrgX - intRangeTolerance))
              && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY + m_CROI.Height) && (nNewXPoint < m_CROI.TotalOrgX))
               || ((nNewXPoint < (m_CROI.TotalOrgX + intRangeTolerance))
              && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY + m_CROI.Height) && (nNewXPoint > m_CROI.TotalOrgX))
               || ((nNewXPoint > (m_CROI.TotalOrgX - intRangeTolerance))
              && (nNewYPoint > (m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height) && (nNewXPoint < m_CROI.TotalOrgX))
              )
            {
                m_Handler = m_CROI.HitTest(m_CROI.TotalOrgX, m_CROI.TotalOrgY + m_CROI.Height);

            }
            else if (((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width + intRangeTolerance))
              && (nNewYPoint > (m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height) && (nNewXPoint > m_CROI.TotalOrgX + m_CROI.Width))
             || ((nNewXPoint > (m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
              && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY + m_CROI.Height) && (nNewXPoint < m_CROI.TotalOrgX + m_CROI.Width))
               || ((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width + intRangeTolerance))
              && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY + m_CROI.Height) && (nNewXPoint > m_CROI.TotalOrgX + m_CROI.Width))
               || ((nNewXPoint > (m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
              && (nNewYPoint > (m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height) && (nNewXPoint < m_CROI.TotalOrgX + m_CROI.Width))
              )
            {
                m_Handler = m_CROI.HitTest(m_CROI.TotalOrgX + m_CROI.Width, m_CROI.TotalOrgY + m_CROI.Height);

            }
            else if (((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
                && (nNewYPoint > (m_CROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + intRangeTolerance) && (nNewXPoint > m_CROI.TotalOrgX + intRangeTolerance)))
            {
                m_Handler = m_CROI.HitTest(m_CROI.TotalOrgX + m_CROI.Width / 2, m_CROI.TotalOrgY);

            }
            //else if (((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width / 2 + intRangeTolerance))
            //  && (nNewYPoint > (m_CROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY) && (nNewXPoint > m_CROI.TotalOrgX + m_CROI.Width / 2))
            // || ((nNewXPoint > (m_CROI.TotalOrgX + m_CROI.Width / 2 - intRangeTolerance))
            //  && (nNewYPoint < (m_CROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY) && (nNewXPoint < m_CROI.TotalOrgX + m_CROI.Width / 2))
            //   || ((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width / 2 + intRangeTolerance))
            //  && (nNewYPoint < (m_CROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY) && (nNewXPoint > m_CROI.TotalOrgX + m_CROI.Width / 2))
            //   || ((nNewXPoint > (m_CROI.TotalOrgX + m_CROI.Width / 2 - intRangeTolerance))
            //  && (nNewYPoint > (m_CROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY) && (nNewXPoint < m_CROI.TotalOrgX + m_CROI.Width / 2))
            //  )
            //{
            //    m_Handler = m_CROI.HitTest(m_CROI.TotalOrgX + m_CROI.Width / 2, m_CROI.TotalOrgY);

            //}
            else if (((nNewXPoint < (m_CROI.TotalOrgX + intRangeTolerance))
                && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY + intRangeTolerance) && (nNewXPoint > m_CROI.TotalOrgX - intRangeTolerance)))
            {
                m_Handler = m_CROI.HitTest(m_CROI.TotalOrgX, m_CROI.TotalOrgY + m_CROI.Height / 2);

            }
            // else if (((nNewXPoint < (m_CROI.TotalOrgX + intRangeTolerance))
            // && (nNewYPoint > (m_CROI.TotalOrgY + m_CROI.Height / 2 - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height / 2) && (nNewXPoint > m_CROI.TotalOrgX))
            //|| ((nNewXPoint > (m_CROI.TotalOrgX - intRangeTolerance))
            // && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height / 2 + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY + m_CROI.Height / 2) && (nNewXPoint < m_CROI.TotalOrgX))
            //  || ((nNewXPoint < (m_CROI.TotalOrgX + intRangeTolerance))
            // && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height / 2 + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY + m_CROI.Height / 2) && (nNewXPoint > m_CROI.TotalOrgX))
            //  || ((nNewXPoint > (m_CROI.TotalOrgX - intRangeTolerance))
            // && (nNewYPoint > (m_CROI.TotalOrgY + m_CROI.Height / 2 - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height / 2) && (nNewXPoint < m_CROI.TotalOrgX))
            // )
            // {
            //     m_Handler = m_CROI.HitTest(m_CROI.TotalOrgX, m_CROI.TotalOrgY + m_CROI.Height / 2);

            // }
            else if (((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
              && (nNewYPoint > (m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height + intRangeTolerance) && (nNewXPoint > m_CROI.TotalOrgX + intRangeTolerance)))
            {
                m_Handler = m_CROI.HitTest(m_CROI.TotalOrgX + m_CROI.Width / 2, m_CROI.TotalOrgY + m_CROI.Height);

            }
            //else if (((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width / 2 + intRangeTolerance))
            //    && (nNewYPoint > (m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height) && (nNewXPoint > m_CROI.TotalOrgX + m_CROI.Width / 2))
            //    || ((nNewXPoint > (m_CROI.TotalOrgX + m_CROI.Width / 2 - intRangeTolerance))
            //    && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY + m_CROI.Height) && (nNewXPoint < m_CROI.TotalOrgX + m_CROI.Width / 2))
            //    || ((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width / 2 + intRangeTolerance))
            //    && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY + m_CROI.Height) && (nNewXPoint > m_CROI.TotalOrgX + m_CROI.Width / 2))
            //    || ((nNewXPoint > (m_CROI.TotalOrgX + m_CROI.Width / 2 - intRangeTolerance))
            //    && (nNewYPoint > (m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height) && (nNewXPoint < m_CROI.TotalOrgX + m_CROI.Width / 2))
            //    )
            //{
            //    m_Handler = m_CROI.HitTest(m_CROI.TotalOrgX + m_CROI.Width / 2, m_CROI.TotalOrgY + m_CROI.Height);

            //}
            else if (((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width + intRangeTolerance))
                && (nNewYPoint > (m_CROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance) && (nNewXPoint > m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance)))
            {
                m_Handler = m_CROI.HitTest(m_CROI.TotalOrgX + m_CROI.Width, m_CROI.TotalOrgY + m_CROI.Height / 2);

            }
            //   else if (((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width + intRangeTolerance))
            // && (nNewYPoint > (m_CROI.TotalOrgY + m_CROI.Height / 2 - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height / 2) && (nNewXPoint > m_CROI.TotalOrgX + m_CROI.Width))
            //|| ((nNewXPoint > (m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
            // && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height / 2 + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY + m_CROI.Height / 2) && (nNewXPoint < m_CROI.TotalOrgX + m_CROI.Width))
            //  || ((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width + intRangeTolerance))
            // && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height / 2 + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY + m_CROI.Height / 2) && (nNewXPoint > m_CROI.TotalOrgX + m_CROI.Width))
            //  || ((nNewXPoint > (m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
            // && (nNewYPoint > (m_CROI.TotalOrgY + m_CROI.Height / 2 - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height / 2) && (nNewXPoint < m_CROI.TotalOrgX + m_CROI.Width))
            // )
            //   {
            //       m_Handler = m_CROI.HitTest(m_CROI.TotalOrgX + m_CROI.Width, m_CROI.TotalOrgY + m_CROI.Height / 2);

            //   }
            else
            {
                m_Handler = m_CROI.HitTest(nNewXPoint, nNewYPoint);
            }
            return GetROIHandle();

        }


        /// <summary>
        /// Get RGB value from selected column
        /// </summary>
        /// <param name="intHitX">location x</param>
        /// <param name="intHitY">location y</param>
        /// <returns>RGB value</returns>
        public int[] GetRGBPixelValue(int intHitX, int intHitY)
        {
            EC24 objColor = new EC24();
      
            objColor = m_CROI.GetPixel(intHitX, intHitY);
            int[] intRGB = new int[3];
            intRGB[0] = objColor.C0;
            intRGB[1] = objColor.C1;
            intRGB[2] = objColor.C2;

            return intRGB;
        }




        /// <summary>
        ///  Attach color child ROI to specific ROI
        /// </summary>
        /// <param name="objCROI">color child ROI</param>
        public void AttachImage(CROI objCROI)
        {
            //if (m_CROI.OrgX < 0)
            //    m_CROI.OrgX = 0;
            //if (m_CROI.OrgY < 0)
            //    m_CROI.OrgY = 0;

            //if ((m_CROI.OrgX + m_CROI.Width) > objCROI.ref_ROIWidth)
            //    m_CROI.Width = objCROI.ref_ROIWidth - m_CROI.OrgX;
            //if ((m_CROI.OrgY + m_CROI.Height) > objCROI.ref_ROIHeight)
            //    m_CROI.Height = objCROI.ref_ROIHeight - m_CROI.OrgY;

            //m_CROI.Detach();
            //m_CROI.Attach(objCROI.ref_CROI);

            try
            {
                if (objCROI.ref_CROI.TopParent == null)
                    return;

                m_CROI.Detach();
                if (m_CROI.OrgX > objCROI.ref_CROI.TopParent.Width)
                    m_CROI.OrgX = 0;
                if (m_CROI.OrgY > objCROI.ref_CROI.TopParent.Height)
                    m_CROI.OrgY = 0;

                if ((m_CROI.OrgX + m_CROI.Width) > objCROI.ref_CROI.TopParent.Width)
                    m_CROI.Width = objCROI.ref_CROI.TopParent.Width - m_CROI.OrgX;

                if ((m_CROI.OrgY + m_CROI.Height) > objCROI.ref_CROI.TopParent.Height)
                    m_CROI.Height = objCROI.ref_CROI.TopParent.Height - m_CROI.OrgY;

                m_CROI.Attach(objCROI.ref_CROI.TopParent);
            }
            catch
            {
            }

        }

        /// <summary>
        /// Attach color image to specific ROI
        /// </summary>
        /// <param name="objCImageDrawing">color image</param>
        public void AttachImage(CImageDrawing objCImageDrawing)
        {
            //m_CROI.Detach();
            //m_CROI.Attach(objCImageDrawing.ref_objMainCImage);
            try
            {
                m_CROI.Detach();
                if (m_CROI.OrgX > objCImageDrawing.ref_objMainCImage.Width)
                    m_CROI.OrgX = 0;
                if (m_CROI.OrgY > objCImageDrawing.ref_objMainCImage.Height)
                    m_CROI.OrgY = 0;

                if ((m_CROI.OrgX + m_CROI.Width) > objCImageDrawing.ref_objMainCImage.Width)
                    m_CROI.Width = objCImageDrawing.ref_objMainCImage.Width - m_CROI.OrgX;

                if ((m_CROI.OrgY + m_CROI.Height) > objCImageDrawing.ref_objMainCImage.Height)
                    m_CROI.Height = objCImageDrawing.ref_objMainCImage.Height - m_CROI.OrgY;

                m_CROI.Attach(objCImageDrawing.ref_objMainCImage);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Set the ROI Frame event that no more movement or changes is done on this moment
        /// Call this function in Mouse_Up event
        /// </summary>
        public void ClearDragHandle()
        {
            m_Handler = EDragHandle.NoHandle;
        }
        
        /// <summary>
        /// if use this function, 2 ROIs are using different memory address
        /// If one of ROI's size/location is changed, another won't be affected
        /// However, if their image source are being changed, both attached image will be changed.
        /// After using this function, need to attach image to new ROI object
        /// </summary>
        /// <param name="objROI">ROI</param>
        public void CopyToNew(CROI objROI)
        {
            objROI.ref_CROI.SetPlacement(m_CROI.OrgX, m_CROI.OrgY, m_CROI.Width, m_CROI.Height);
            objROI.ref_strROIName = m_strROIName;
            objROI.ref_intType = m_intType;
        }

        /// <summary>
        /// Move ROI Frame to new position on graph
        /// </summary>
        /// <param name="nPositionX">the point of mouse move over</param>
        /// <param name="nPositionY">the point of mouse move over</param>
        public void DragROI(int nNewPositionX, int nNewPositionY)
        {
            //try
            //{
            //    m_CROI.Drag(m_Handler, nNewPositionX, nNewPositionY);
            //}
            //catch (Exception ex)
            //{
            //    string str = ex.ToString();
            //}
            {
                try
                {
                    m_CROI.Drag(m_Handler, nNewPositionX, nNewPositionY);

                    bool blnAdjustX = false;
                    bool blnAdjustY = false;
                    if (ref_ROITotalX + ref_ROIWidth >= m_CROI.TopParent.Width)
                    {
                        int intOffet = m_CROI.TopParent.Width - (ref_ROITotalX + ref_ROIWidth) - 1;
                        LoadROISetting(m_CROI.OrgX + intOffet, m_CROI.OrgY, m_CROI.Width, m_CROI.Height);

                        blnAdjustX = true;
                    }

                    if (ref_ROITotalY + ref_ROIHeight >= m_CROI.TopParent.Height)
                    {
                        int intOffet = m_CROI.TopParent.Height - (ref_ROITotalY + ref_ROIHeight) - 1;
                        LoadROISetting(m_CROI.OrgX, m_CROI.OrgY + intOffet, m_CROI.Width, m_CROI.Height);

                        blnAdjustY = true;
                    }

                    if (ref_ROITotalX < 0)
                    {
                        if (blnAdjustX)
                            LoadROISetting(m_CROI.OrgX, m_CROI.OrgY, m_CROI.Width + ref_ROITotalX, m_CROI.Height);
                        else
                            LoadROISetting(0, m_CROI.OrgY, m_CROI.Width, m_CROI.Height);
                    }

                    if (ref_ROITotalY < 0)
                    {
                        if (blnAdjustY)
                            LoadROISetting(m_CROI.OrgX, m_CROI.OrgY, m_CROI.Width, m_CROI.Height + ref_ROITotalX);
                        else
                            LoadROISetting(m_CROI.OrgX, 0, m_CROI.Width, m_CROI.Height);
                    }

                    if (ref_ROITotalX <= m_CROI.Parent.TotalOrgX)
                    {
                        LoadROISetting(0, m_CROI.OrgY, m_CROI.Width, m_CROI.Height);
                    }

                    if (ref_ROITotalY <= m_CROI.Parent.TotalOrgY)
                    {
                        LoadROISetting(m_CROI.OrgX, 0, m_CROI.Width, m_CROI.Height);
                    }

                    if (ref_ROITotalX + ref_ROIWidth >= m_CROI.Parent.Width)
                    {
                        LoadROISetting(m_CROI.OrgX, m_CROI.OrgY, m_CROI.Width, m_CROI.Height);
                    }

                    if (ref_ROITotalY + ref_ROIHeight >= m_CROI.Parent.Height)
                    {
                        LoadROISetting(m_CROI.OrgX, m_CROI.OrgY, m_CROI.Width, m_CROI.Height);
                    }

                }
                catch (Exception ex)
                {
                    string str = ex.ToString();
                }
            }
        }

        /// <summary>
        /// Redraw ROI
        /// </summary>
        /// <param name="g">destination to draw image</param>
        public void Draw(Graphics g)
        {
            m_CROI.Draw(g);
        }

        /// <summary>
        ///  Draw a rectangle frame around an image or ROI
        /// </summary>
        /// <param name="g">window destination to put the drawing up</param>
        /// <param name="blnHandler">true = frame can be modified (change shape or move position), false if otherwise</param>
        public void DrawROI(Graphics g, bool blnHandler)
        {
            //m_CROI.DrawFrame(g, new Pen(Color.Red), EFramePosition.On, blnHandler);
            m_CROI.DrawFrame(g, EFramePosition.On, blnHandler);
        }

        /// <summary>
        /// Load color image from selected path
        /// </summary>
        /// <param name="strPath">selected path</param>
        /// <param name="blnHaveparent">true = has parent, false = does not has parent</param>
        public void LoadImage(string strPath, bool blnHaveparent)
        {
            if (!blnHaveparent)
            {
                Image objImage = Image.FromFile(strPath);
                m_objParentCImage = new EImageC24();
                m_objParentCImage.SetSize(objImage.Width, objImage.Height);

                m_CROI.Detach();
                m_CROI.Attach(m_objParentCImage);
                LoadROISetting(0, 0, objImage.Width, objImage.Height);
            }
            m_CROI.Load(strPath);
        }

        /// <summary>
        ///  When start software, all color ROI information needed to be loaded from CPU memory or file
        /// </summary>
        /// <param name="intOrgX">ROI top left angle position x</param>
        /// <param name="intOrgY">ROI top left angle position y</param>
        /// <param name="intWidth">ROI frame width</param>
        /// <param name="intHeight">ROI frame height</param>
        public void LoadROISetting(int nOrgX, int nOrgY, int nWidth, int nHeight)
        {
            //m_CROI.SetPlacement(intOrgX, intOrgY, intWidth, intHeight);
            {
                if (nOrgX < 0)
                    nOrgX = 0;
                if (nOrgY < 0)
                    nOrgY = 0;
                if (nWidth < 0)
                    nWidth = 1;
                if (nHeight < 0)
                    nHeight = 1;

                if (m_CROI.Parent != null)
                {
                    try
                    {
                        if (nOrgX > m_CROI.Parent.Width)
                            nOrgX = 0;
                        if (nOrgY > m_CROI.Parent.Height)
                            nOrgY = 0;

                        if ((nOrgX + nWidth) > m_CROI.Parent.Width)
                        {
                            //int intOffset = nWidth - ((nOrgX + nWidth) - m_CROI.Parent.Width);     // 2020 02 17 - CCENG: Scenario get inoffset = 50 is wrong when nWidth = 51, nOrgX = 2, and parent.width = 
                            int intOffset = (nOrgX + nWidth) - m_CROI.Parent.Width;
                            if (nOrgX - intOffset >= 0)
                                nOrgX -= intOffset;
                            else
                                nWidth = nWidth - ((nOrgX + nWidth) - m_CROI.Parent.Width);
                        }

                        if ((nOrgY + nHeight) > m_CROI.Parent.Height)
                        {
                            //int intOffset = nHeight - ((nOrgY + nHeight) - m_CROI.Parent.Height);  // 2020 02 17 - CCENG: Scenario get inoffset = 50 is wrong when 
                            int intOffset = (nOrgY + nHeight) - m_CROI.Parent.Height;
                            if (nOrgY - intOffset >= 0)
                                nOrgY -= intOffset;
                            else
                                nHeight = nHeight - ((nOrgY + nHeight) - m_CROI.Parent.Height);
                        }
                    }
                    catch { }
                }


                if (nWidth < 0)
                    nWidth = 0;
                if (nHeight < 0)
                    nHeight = 0;

                m_CROI.SetPlacement(nOrgX, nOrgY, nWidth, nHeight);
                //m_intOriHeight = nHeight;
                //m_intOriWidth = nWidth;
            }
        }
        public static void LoadROISetting(ref EROIC24 objROI, int nOrgX, int nOrgY, int nWidth, int nHeight)
        {
            if (nOrgX < 0)
                nOrgX = 0;
            if (nOrgY < 0)
                nOrgY = 0;
            if (nWidth < 0)
                nWidth = 1;
            if (nHeight < 0)
                nHeight = 1;

            if (objROI.Parent != null)
            {
                try
                {
                    if (nOrgX > objROI.Parent.Width)
                        nOrgX = 0;
                    if (nOrgY > objROI.Parent.Height)
                        nOrgY = 0;

                    if ((nOrgX + nWidth) > objROI.Parent.Width)
                    {
                        //int intOffset = nWidth - ((nOrgX + nWidth) - m_ROI.Parent.Width);     // 2020 02 17 - CCENG: Scenario get inoffset = 50 is wrong when nWidth = 51, nOrgX = 2, and parent.width = 
                        int intOffset = (nOrgX + nWidth) - objROI.Parent.Width;
                        if (nOrgX - intOffset >= 0)
                            nOrgX -= intOffset;
                        else
                            nWidth = nWidth - ((nOrgX + nWidth) - objROI.Parent.Width);
                    }

                    if ((nOrgY + nHeight) > objROI.Parent.Height)
                    {
                        //int intOffset = nHeight - ((nOrgY + nHeight) - m_ROI.Parent.Height);  // 2020 02 17 - CCENG: Scenario get inoffset = 50 is wrong when 
                        int intOffset = (nOrgY + nHeight) - objROI.Parent.Height;
                        if (nOrgY - intOffset >= 0)
                            nOrgY -= intOffset;
                        else
                            nHeight = nHeight - ((nOrgY + nHeight) - objROI.Parent.Height);
                    }
                }
                catch { }
            }


            if (nWidth < 0)
                nWidth = 0;
            if (nHeight < 0)
                nHeight = 0;

            objROI.SetPlacement(nOrgX, nOrgY, nWidth, nHeight);
        }

        public void SaveImage(string strFilePath)
        {
            try
            {
                m_CROI.Save(strFilePath);
            }
            catch
            {
            }
        }



        public void AddExtraGain(float fGain)
        {
            if (fGain != 1f)
            {
                m_objColorLookup.AdjustGainOffset(EColorSystem.Rgb, fGain, 0.00f, fGain, 0.00f, fGain, 0.00f);
                m_objColorLookup.Transform(m_CROI, m_CROI);
            }
        }



        /// <summary>
        /// Convert color image to gray scale image
        /// </summary>
        /// <param name="objDestROI">gray scale destination image</param>
        public static void ConvertToMono(CROI objSource, ROI objDestROI)
        {
            objDestROI.ref_ROIWidth = objSource.ref_ROIWidth;
            objDestROI.ref_ROIHeight = objSource.ref_ROIHeight;

            EasyImage.Convert(objSource.ref_CROI, objDestROI.ref_ROI);
        }

        /// <summary>
        ///  Rotate image to less than 90 degree of rotation angle. SourceImage must not be same object with rotatedImage.
        /// </summary>
        /// <param name="objSourceImage">source image</param>
        /// <param name="objSearchROI">search ROI</param>
        /// <param name="fRotateAngle">rotation angle</param>
        /// <param name="objRotatedImage">destination image</param>
       
        public static void Rotate0Degree(CImageDrawing objSourceImage, CROI objSearchROI, float fRotateAngle, int intInterpolation, ref List<CImageDrawing> arrRotatedImage, int intImageIndex)
        {
            EROIC24 searchROI = new EROIC24();
            EROIC24 destinationROI = new EROIC24();

            CImageDrawing objRotatedImage = arrRotatedImage[intImageIndex];
            objSourceImage.CopyTo(ref objRotatedImage); // copy source to rotated image to prevent rotated image non-ROI area become black

            searchROI.SetSize(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
            searchROI.OrgX = objSearchROI.ref_ROITotalX;
            searchROI.OrgY = objSearchROI.ref_ROITotalY;
            searchROI.Attach(objSourceImage.ref_objMainCImage);

            destinationROI.SetSize(searchROI.Width, searchROI.Height);
            destinationROI.OrgX = searchROI.OrgX;
            destinationROI.OrgY = searchROI.OrgY;
            destinationROI.Attach(objRotatedImage.ref_objMainCImage);
            try
            {
                EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f, searchROI.Height / 2f, destinationROI.Width / 2f, destinationROI.Height / 2f, 1, 1, fRotateAngle, destinationROI, intInterpolation);
            }
            catch { }

            searchROI.Dispose();
            destinationROI.Dispose();
        }
        public static void Rotate0Degree(CROI objSearchROI, float fRotateAngle, int intInterpolation, ref List<CImageDrawing> arrRotatedImage, int intImageIndex)
        {
            EROIC24 searchROI = new EROIC24();
            EROIC24 destinationROI = new EROIC24();

            CImageDrawing objRotatedImage = arrRotatedImage[intImageIndex];
            CImageDrawing objSourceImage = new CImageDrawing(true);
            objRotatedImage.CopyTo(ref objSourceImage);        //Copy source to new object to prevent rotation image blur

            searchROI.SetSize(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
            searchROI.OrgX = objSearchROI.ref_ROIPositionX;
            searchROI.OrgY = objSearchROI.ref_ROIPositionY;
            searchROI.Attach(objSourceImage.ref_objMainCImage);

            destinationROI.SetSize(searchROI.Width, searchROI.Height);
            destinationROI.OrgX = searchROI.OrgX;
            destinationROI.OrgY = searchROI.OrgY;
            destinationROI.Attach(objRotatedImage.ref_objMainCImage);

            try
            {
                EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f, searchROI.Height / 2f, destinationROI.Width / 2f, destinationROI.Height / 2f, 1, 1, fRotateAngle, destinationROI, intInterpolation);
            }
            catch { }
            //objSourceImage.Dispose();
            //searchROI.Dispose();
            //destinationROI.Dispose();

            searchROI.Dispose();
            destinationROI.Dispose();
            searchROI = null;
            destinationROI = null;
            objSourceImage.Dispose();
            objSourceImage = null;
        }
        /// <summary>
        /// Get color pixel value of specific mouse hit position
        /// </summary>
        /// <param name="intHitX">mouse hit X</param>
        /// <param name="intHitY">mouse hit Y</param>
        /// <returns>color pixel value</returns>
        public Color24 GetColor(int intHitX, int intHitY)
        {
            Color24 objColor = new Color24();

            objColor.ref_Color24 = m_CROI.GetPixel(intHitX, intHitY);

            return objColor;
        }
        public void CopyImage(ref CROI objROI)
        {
            try
            {
                if (objROI.ref_CROI.Width == m_CROI.Width && objROI.ref_CROI.Height == m_CROI.Height)
                    EasyImage.Copy(m_CROI, objROI.m_CROI);
            }
            catch
            { }
        }
        public static void SaveFile(string strFilePath, List<CROI> arrROIList)
        {
            XmlParser objFile = new XmlParser(strFilePath);

            objFile.WriteSectionElement("Unit0", true);
            for (int i = 0; i < arrROIList.Count; i++)
            {
                objFile.WriteElement1Value("ROI" + i, "");

                objFile.WriteElement2Value("Name", arrROIList[i].ref_strROIName);
                objFile.WriteElement2Value("Type", arrROIList[i].ref_intType);
                objFile.WriteElement2Value("PositionX", arrROIList[i].ref_ROIPositionX);
                objFile.WriteElement2Value("PositionY", arrROIList[i].ref_ROIPositionY);
                objFile.WriteElement2Value("Width", arrROIList[i].ref_ROIWidth);
                objFile.WriteElement2Value("Height", arrROIList[i].ref_ROIHeight);
                objFile.WriteElement2Value("StartOffsetX", arrROIList[i].ref_intStartOffsetX);
                objFile.WriteElement2Value("StartOffsetY", arrROIList[i].ref_intStartOffsetY);
                //if (arrROIList[i].ref_intType == 1)
                //{
                //    float fPixelAverage = arrROIList[i].GetROIAreaPixel();
                //    objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                //    arrROIList[i].SetROIPixelAverage(fPixelAverage);
                //}
            }

            objFile.WriteEndElement();
        }
        public static void LoadFile(string strPath, List<CROI> arrROIList)
        {
            if (arrROIList == null)
                return;

            arrROIList.Clear();

            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection("Unit0");
            int intChildCount = objFile.GetSecondSectionCount();
            for (int j = 0; j < intChildCount; j++)
            {
                CROI objROI = new CROI();
                objFile.GetSecondSection("ROI" + j);
                objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 2);
                objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 2);
                objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 2);
                objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 2);
                objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 2);
                objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 2);
                objROI.ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 2);
                objROI.ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 2);
                //objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 2));

                //if (objROI.ref_intType > 1)
                //{
                //    objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                //    objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                //}

                arrROIList.Add(objROI);
            }
        }
        public static void LoadFile(string strPath, List<List<CROI>> arrROIList)
        {
            if (arrROIList == null)
            {
                SRMMessageBox.Show("CROI.cs->LoadFile()->arrROIList is null.");
                return;
            }

            for (int i = 0; i < arrROIList.Count; i++)
            {
                for (int j = 0; j < arrROIList[i].Count; j++)
                {
                    arrROIList[i][j].Dispose();
                }
            }
            arrROIList.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount = 0;
            CROI objROI;

            int intCount = objFile.GetFirstSectionCount();
            for (int i = 0; i < intCount; i++)
            {
                arrROIList.Add(new List<CROI>());
                objFile.GetFirstSection("Unit" + i);
                intChildCount = objFile.GetSecondSectionCount();
                for (int j = 0; j < intChildCount; j++)
                {
                    objROI = new CROI();
                    objFile.GetSecondSection("ROI" + j);
                    objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 2);
                    objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 2);
                    objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 2);
                    objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 2);
                    objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 2);
                    objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 2);
                    //objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 2));
                    objROI.ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 2);
                    objROI.ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 2);

                    ////if (objROI.ref_intType > 1)
                    ////{
                    //objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                    //objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                    ////}

                    arrROIList[i].Add(objROI);
                }
            }
        }
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, int intROINumber)
        {
            DrawROI(g, fDrawingScaleX, fDrawingScaleY, blnHandler, m_strROIName, m_intType, intROINumber);
        }
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, string strText, int intType, int intROINumber)
        {
            try
            {
                if (m_CROI.Parent == null)
                    return;

                if (m_strROIName != strText)
                    m_strROIName = strText;
                if (m_intType != intType)
                    m_intType = intType;

                if (m_strROIName == "ReTest ROI")
                    m_Color[0] = Color.GreenYellow;
                // Frame Position ON = frame is centered on image edge, Inside = outer edge of frame remain inside of image, Outside = inner edge of frame remain outside of image</param>
                //m_CROI.DrawFrame(g, new Pen(m_Color[intROINumber], 2), EFramePosition.Outside, blnHandler, fDrawingScaleX, fDrawingScaleY);
                //m_CROI.DrawFrame(g, new ERGBColor(m_Color[intROINumber].R, m_Color[intROINumber].G, m_Color[intROINumber].B), blnHandler, fDrawingScaleX, fDrawingScaleY);
                if (m_objRGBColor.Red != m_Color[intROINumber].R)
                    m_objRGBColor.Red = m_Color[intROINumber].R;
                if (m_objRGBColor.Green != m_Color[intROINumber].G)
                    m_objRGBColor.Green = m_Color[intROINumber].G;
                if (m_objRGBColor.Blue != m_Color[intROINumber].B)
                    m_objRGBColor.Blue = m_Color[intROINumber].B;

                if (m_CROI.OrgX < 0)
                {
                    m_CROI.Width += m_CROI.OrgX;
                    m_CROI.OrgX = 0;
                }
                if (m_CROI.OrgY < 0)
                {
                    m_CROI.Height += m_CROI.OrgY;
                    m_CROI.OrgY = 0;
                }
                if (m_CROI.Width < 10)
                    m_CROI.Width = 10;
                if (m_CROI.Height < 10)
                    m_CROI.Height = 10;

                if (m_CROI.Parent != null)
                {
                    if (m_CROI.Width > m_CROI.Parent.Width)
                        m_CROI.Width = m_CROI.Parent.Width;

                    if (m_CROI.Height > m_CROI.Parent.Height)
                        m_CROI.Height = m_CROI.Parent.Height;

                    if (m_CROI.OrgX > m_CROI.Parent.Width - 10)
                        m_CROI.OrgX = m_CROI.Parent.Width - 10;

                    if (m_CROI.OrgY > m_CROI.Parent.Height - 10)
                        m_CROI.OrgY = m_CROI.Parent.Height - 10;

                    if ((m_CROI.OrgX + m_CROI.Width) > m_CROI.Parent.Width)
                        m_CROI.Width = m_CROI.Parent.Width - m_CROI.OrgX;

                    if ((m_CROI.OrgY + m_CROI.Height) > m_CROI.Parent.Height)
                        m_CROI.Height = m_CROI.Parent.Height - m_CROI.OrgY;

                }

                m_CROI.DrawFrame(g, m_objRGBColor, blnHandler, fDrawingScaleX, fDrawingScaleY);
                DrawString(intROINumber, g, fDrawingScaleX, fDrawingScaleY);
            }
            catch
            {

            }
        }
        private void DrawString(int intColorNo, Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            int intTextPositionX = 0, intTestPositionY = 0;

            if (m_intType >= 2)
            {
                intTextPositionX = m_CROI.TotalOrgX;
                intTestPositionY = m_CROI.TotalOrgY;
                g.DrawString(m_strROIName, m_Font, new SolidBrush(m_Color[intColorNo]), intTextPositionX * fDrawingScaleX, intTestPositionY * fDrawingScaleY);
            }
            else
                g.DrawString(m_strROIName, m_Font, new SolidBrush(m_Color[intColorNo]), m_CROI.OrgX * fDrawingScaleX, m_CROI.OrgY * fDrawingScaleY);
        }
        public bool VerifyROIHandleShape(int nNewXPoint, int nNewYPoint)
        {
            if (m_CROI.TopParent == null)
                return false;

            int intRangeTolerance = 10;
            if (m_CROI.Width < 40 || m_CROI.Height < 40)
                intRangeTolerance = 3;
            else if (m_CROI.Width < 100 || m_CROI.Height < 100)
                intRangeTolerance = 5;

            if (((nNewXPoint < (m_CROI.TotalOrgX + intRangeTolerance))
                    && (nNewYPoint > (m_CROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + intRangeTolerance) && (nNewXPoint > m_CROI.TotalOrgX - intRangeTolerance))
                   || ((nNewXPoint > (m_CROI.TotalOrgX - intRangeTolerance))
                    && (nNewYPoint < (m_CROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY - intRangeTolerance) && (nNewXPoint < m_CROI.TotalOrgX + intRangeTolerance))
                     || ((nNewXPoint < (m_CROI.TotalOrgX + intRangeTolerance))
                    && (nNewYPoint < (m_CROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY - intRangeTolerance) && (nNewXPoint > m_CROI.TotalOrgX - intRangeTolerance))
                     || ((nNewXPoint > (m_CROI.TotalOrgX - intRangeTolerance))
                    && (nNewYPoint > (m_CROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + intRangeTolerance) && (nNewXPoint < m_CROI.TotalOrgX + intRangeTolerance))
                    )
            {
                m_Handler2 = m_CROI.HitTest(m_CROI.TotalOrgX, m_CROI.TotalOrgY);
            }
            else if (((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width + intRangeTolerance))
                 && (nNewYPoint > (m_CROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + intRangeTolerance) && (nNewXPoint > m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
                || ((nNewXPoint > (m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
                 && (nNewYPoint < (m_CROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY - intRangeTolerance) && (nNewXPoint < m_CROI.TotalOrgX + m_CROI.Width + intRangeTolerance))
                  || ((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width + intRangeTolerance))
                 && (nNewYPoint < (m_CROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY - intRangeTolerance) && (nNewXPoint > m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
                  || ((nNewXPoint > (m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
                 && (nNewYPoint > (m_CROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + intRangeTolerance) && (nNewXPoint < m_CROI.TotalOrgX + m_CROI.Width + intRangeTolerance))
                 )
            {
                m_Handler2 = m_CROI.HitTest(m_CROI.TotalOrgX + m_CROI.Width, m_CROI.TotalOrgY);
            }
            else if (((nNewXPoint < (m_CROI.TotalOrgX + intRangeTolerance))
              && (nNewYPoint > (m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height + intRangeTolerance) && (nNewXPoint > m_CROI.TotalOrgX - intRangeTolerance))
             || ((nNewXPoint > (m_CROI.TotalOrgX - intRangeTolerance))
              && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance) && (nNewXPoint < m_CROI.TotalOrgX + intRangeTolerance))
               || ((nNewXPoint < (m_CROI.TotalOrgX + intRangeTolerance))
              && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance) && (nNewXPoint > m_CROI.TotalOrgX - intRangeTolerance))
               || ((nNewXPoint > (m_CROI.TotalOrgX - intRangeTolerance))
              && (nNewYPoint > (m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height + intRangeTolerance) && (nNewXPoint < m_CROI.TotalOrgX + intRangeTolerance))
              )
            {
                m_Handler2 = m_CROI.HitTest(m_CROI.TotalOrgX, m_CROI.TotalOrgY + m_CROI.Height);
            }
            else if (((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width + intRangeTolerance))
              && (nNewYPoint > (m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height + intRangeTolerance) && (nNewXPoint > m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
             || ((nNewXPoint > (m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
              && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance) && (nNewXPoint < m_CROI.TotalOrgX + m_CROI.Width + intRangeTolerance))
               || ((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width + intRangeTolerance))
              && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height + intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance) && (nNewXPoint > m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
               || ((nNewXPoint > (m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
              && (nNewYPoint > (m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height + intRangeTolerance) && (nNewXPoint < m_CROI.TotalOrgX + m_CROI.Width + intRangeTolerance))
              )
            {
                m_Handler2 = m_CROI.HitTest(m_CROI.TotalOrgX + m_CROI.Width, m_CROI.TotalOrgY + m_CROI.Height);

            }
            else if (((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
                && (nNewYPoint > (m_CROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + intRangeTolerance) && (nNewXPoint > m_CROI.TotalOrgX + intRangeTolerance)))
            {
                m_Handler2 = m_CROI.HitTest(m_CROI.TotalOrgX + m_CROI.Width / 2, m_CROI.TotalOrgY);

            }
            else if (((nNewXPoint < (m_CROI.TotalOrgX + intRangeTolerance))
                && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance)) && (nNewYPoint > m_CROI.TotalOrgY + intRangeTolerance) && (nNewXPoint > m_CROI.TotalOrgX - intRangeTolerance)))
            {
                m_Handler2 = m_CROI.HitTest(m_CROI.TotalOrgX, m_CROI.TotalOrgY + m_CROI.Height / 2);

            }
            else if (((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance))
              && (nNewYPoint > (m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height + intRangeTolerance) && (nNewXPoint > m_CROI.TotalOrgX + intRangeTolerance)))
            {
                m_Handler2 = m_CROI.HitTest(m_CROI.TotalOrgX + m_CROI.Width / 2, m_CROI.TotalOrgY + m_CROI.Height);

            }
            else if (((nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width + intRangeTolerance))
                && (nNewYPoint > (m_CROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint < m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance) && (nNewXPoint > m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance)))
            {
                m_Handler2 = m_CROI.HitTest(m_CROI.TotalOrgX + m_CROI.Width, m_CROI.TotalOrgY + m_CROI.Height / 2);

            }
            else if ((nNewXPoint > (m_CROI.TotalOrgX + intRangeTolerance)) && (nNewXPoint < (m_CROI.TotalOrgX + m_CROI.Width - intRangeTolerance)) && (nNewYPoint > (m_CROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint < (m_CROI.TotalOrgY + m_CROI.Height - intRangeTolerance)))
            {
                m_Handler2 = EDragHandle.Inside;
            }
            else
            {
                m_Handler2 = EDragHandle.NoHandle;
            }

            switch (m_Handler2)
            {
                case EDragHandle.NoHandle:
                    Cursor.Current = Cursors.Default;
                    break;
                case EDragHandle.Inside:
                    Cursor.Current = Cursors.SizeAll;
                    break;
                case EDragHandle.North:
                    Cursor.Current = Cursors.SizeNS;
                    break;
                case EDragHandle.East:
                    Cursor.Current = Cursors.SizeWE;
                    break;
                case EDragHandle.South:
                    Cursor.Current = Cursors.SizeNS;
                    break;
                case EDragHandle.West:
                    Cursor.Current = Cursors.SizeWE;
                    break;
                case EDragHandle.NorthWest:
                    Cursor.Current = Cursors.SizeNWSE;
                    break;
                case EDragHandle.SouthWest:
                    Cursor.Current = Cursors.SizeNESW;
                    break;
                case EDragHandle.NorthEast:
                    Cursor.Current = Cursors.SizeNESW;
                    break;
                case EDragHandle.SouthEast:
                    Cursor.Current = Cursors.SizeNWSE;
                    break;
            }
            return true;
        }
        public void DrawZoomImage(Graphics g, int intPanelWidth, int intPanelHeight)
        {
            if (m_CROI.TopParent == null)
            {
                return;
            }

            if (intPanelWidth == 0)
                intPanelWidth = 1;

            if (intPanelHeight == 0)
                intPanelHeight = 1;

            float fScaleRate = Math.Min(intPanelWidth / (float)m_CROI.Width, intPanelHeight / (float)m_CROI.Height);
            fScaleRate = Math.Min(fScaleRate, 1f);
            
            if (m_objZoomCImage == null)
                m_objZoomCImage = new EImageC24();

            CImageDrawing.SetImageSize(m_objZoomCImage, Convert.ToInt32(m_CROI.Width * fScaleRate), Convert.ToInt32(m_CROI.Height * fScaleRate));

            EImageC24 objCImage = new EImageC24();
            CImageDrawing.SetImageSize(objCImage, Convert.ToInt32(m_CROI.Width * fScaleRate), Convert.ToInt32(m_CROI.Height * fScaleRate));

            EasyImage.Copy(objCImage, m_objZoomCImage);    //Clear image memory to 0

            EasyImage.ScaleRotate(m_CROI, (m_CROI.Width) / 2, (m_CROI.Height) / 2,
                (m_objZoomCImage.Width) / 2, (m_objZoomCImage.Height) / 2, fScaleRate, fScaleRate, 0.0f, m_objZoomCImage);

            if (m_objDisplayCImage == null)
                m_objDisplayCImage = new EImageC24();

            CImageDrawing.SetImageSize(m_objDisplayCImage, intPanelWidth, intPanelHeight);
            CImageDrawing.SetImageSize(objCImage, intPanelWidth, intPanelHeight); 
            EasyImage.Copy(objCImage, m_objDisplayCImage);    //Clear image memory to 0

            EROIC24 objDisplayROI = new EROIC24();
            objDisplayROI.Detach();
            objDisplayROI.Attach(m_objDisplayCImage);
            objDisplayROI.SetPlacement(m_objDisplayCImage.Width / 2 - m_objZoomCImage.Width / 2, m_objDisplayCImage.Height / 2 - m_objZoomCImage.Height / 2, m_objZoomCImage.Width, m_objZoomCImage.Height);
            EasyImage.Copy(m_objZoomCImage, objDisplayROI);
            objDisplayROI.Dispose();    // 2018 07 10 - Make sure local ROI is disposed.
            objCImage.Dispose();
            m_objDisplayCImage.Draw(g);

        }

        public void ThresholdTo_ROIToImage(ref ImageDrawing objDestinationImage, int[] intColorThreshold, int[] intColorTolerance, int intColorFormat, int intCloseIteration, bool blnInvertBlackWhite)
        {

            if (m_CROI.Width != objDestinationImage.ref_intImageWidth || m_CROI.Height != objDestinationImage.ref_intImageHeight)
            {
                objDestinationImage.SetImageSize(m_CROI.Width, m_CROI.Height);
            }
           
            Color24 objMinColor = ColorProcessing.CalculateMinColor(intColorThreshold, intColorTolerance);
            Color24 objMaxColor = ColorProcessing.CalculateMaxColor(intColorThreshold, intColorTolerance);


            if (intColorFormat == 1)
            {
                EasyImage.Threshold(m_CROI, objDestinationImage.ref_objMainImage, objMinColor.ref_Color24, objMaxColor.ref_Color24);
            }
            else if (intColorFormat == 0)
            {
                m_objColorLookup.ConvertFromRgb(EColorSystem.Lsh);
                EasyImage.Threshold(m_CROI, objDestinationImage.ref_objMainImage, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookup);
            }
            else
            {
                m_objColorLookup.ConvertFromRgb(EColorSystem.Ysh);
                EasyColor.GetComponent(m_CROI, objDestinationImage.ref_objMainImage, 1, m_objColorLookup);
#if (Debug_2_12 || Release_2_12)
                EasyImage.Threshold(objDestinationImage.ref_objMainImage, objDestinationImage.ref_objMainImage, (uint)intColorThreshold[0]);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                EasyImage.Threshold(objDestinationImage.ref_objMainImage, objDestinationImage.ref_objMainImage, intColorThreshold[0]);
#endif

            }

            if (intCloseIteration > 0)
            {
#if (Debug_2_12 || Release_2_12)
                EasyImage.CloseBox(objDestinationImage.ref_objMainImage, objDestinationImage.ref_objMainImage, (uint)intCloseIteration);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                EasyImage.CloseBox(objDestinationImage.ref_objMainImage, objDestinationImage.ref_objMainImage, intCloseIteration);
#endif
            }

            if (blnInvertBlackWhite)
            {
                EasyImage.Oper(EArithmeticLogicOperation.Invert, objDestinationImage.ref_objMainImage, objDestinationImage.ref_objMainImage);
            }
        }
    }
}
